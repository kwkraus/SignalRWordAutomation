using Microsoft.AspNet.SignalR;
using Microsoft.Office.Interop.Word;
using SignalRConsoleTest.Wrappers;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace SignalRConsoleTest
{
    public class WordFactory
    {
        /* source http://www.pinvoke.net/default.aspx/Enums/ShowWindowCommand.html */
        enum ShowWindowCommands
        {
            /// <summary>
            /// Hides the window and activates another window.
            /// </summary>
            Hide = 0,
            /// <summary>
            /// Activates and displays a window. If the window is minimized or 
            /// maximized, the system restores it to its original size and position.
            /// An application should specify this flag when displaying the window 
            /// for the first time.
            /// </summary>
            Normal = 1,
            /// <summary>
            /// Activates the window and displays it as a minimized window.
            /// </summary>
            ShowMinimized = 2,
            /// <summary>
            /// Maximizes the specified window.
            /// </summary>
            Maximize = 3, // is this the right value?
            /// <summary>
            /// Activates the window and displays it as a maximized window.
            /// </summary>       
            ShowMaximized = 3,
            /// <summary>
            /// Displays a window in its most recent size and position. This value 
            /// is similar to <see cref="Win32.ShowWindowCommand.Normal"/>, except 
            /// the window is not activated.
            /// </summary>
            ShowNoActivate = 4,
            /// <summary>
            /// Activates the window and displays it in its current size and position. 
            /// </summary>
            Show = 5,
            /// <summary>
            /// Minimizes the specified window and activates the next top-level 
            /// window in the Z order.
            /// </summary>
            Minimize = 6,
            /// <summary>
            /// Displays the window as a minimized window. This value is similar to
            /// <see cref="Win32.ShowWindowCommand.ShowMinimized"/>, except the 
            /// window is not activated.
            /// </summary>
            ShowMinNoActive = 7,
            /// <summary>
            /// Displays the window in its current size and position. This value is 
            /// similar to <see cref="Win32.ShowWindowCommand.Show"/>, except the 
            /// window is not activated.
            /// </summary>
            ShowNA = 8,
            /// <summary>
            /// Activates and displays the window. If the window is minimized or 
            /// maximized, the system restores it to its original size and position. 
            /// An application should specify this flag when restoring a minimized window.
            /// </summary>
            Restore = 9,
            /// <summary>
            /// Sets the show state based on the SW_* value specified in the 
            /// STARTUPINFO structure passed to the CreateProcess function by the 
            /// program that started the application.
            /// </summary>
            ShowDefault = 10,
            /// <summary>
            ///  <b>Windows 2000/XP:</b> Minimizes a window, even if the thread 
            /// that owns the window is not responding. This flag should only be 
            /// used when minimizing windows from a different thread.
            /// </summary>
            ForceMinimize = 11
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        static extern bool AllowSetForegroundWindow(int dwProcessId);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("user32.dll")]
        static extern bool ShowWindowAsync(HandleRef hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hWnd, ShowWindowCommands nCmdShow);

        private static Application WordApp = null;
        private static readonly IHubContext context = GlobalHost.ConnectionManager.GetHubContext<WordHub>();

        public static void CreateInstance()
        {
            if (WordApp == null)
            {
                Application wordinstance;
                try
                {
                    wordinstance = (Application)Marshal.GetActiveObject("Word.Application");
                }
                catch (COMException)
                {
                    wordinstance = new Application();
                }

                ApplicationEvents4_Event events = (ApplicationEvents4_Event)wordinstance;

                events.Quit += new ApplicationEvents4_QuitEventHandler(QuitHandler);

                wordinstance.DocumentBeforeClose += Wordinstance_DocumentBeforeClose;
                wordinstance.Visible = true;
                WordApp = wordinstance;

                context.Clients.All.addMessage("CreateInstance", "New Word Instance created");
            }
        }

        /// <summary>
        /// Function uses Pinvoke method to bring window up front
        /// </summary>
        /// <param name="docUri"></param>
        /// <returns></returns>
        internal static bool FocusDocumentPinvoke(string docUri)
        {
            if (WordApp != null)
            {
                Document doc = WordApp.Documents[docUri];

                string windowCaption = doc.ActiveWindow.Caption;

                IntPtr docHandle = FindWindow(null, windowCaption + " - Word");
                Process currentProc = Process.GetCurrentProcess();

                int currentRetry = 0;

                for (;;)
                {
                    //we need restore window from minimized state
                    bool showResult = ShowWindow(docHandle, ShowWindowCommands.Restore);
                    Console.WriteLine($"ShowWindow returned with {showResult.ToString()}");

                    //then set focus
                    bool setResult = SetForegroundWindow(docHandle);
                    Console.WriteLine($"SetForegroundWindow returned with {setResult.ToString()}");

                    if (!setResult)
                    {
                        if (currentRetry > 3)
                            return false;

                        currentRetry++;
                        Console.WriteLine($"retrying SetForegroundWindow Call: {currentRetry}");
                        Thread.Sleep(100);
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Function uses Word app to activate word window.
        /// </summary>
        /// <param name="docUri"></param>
        /// <returns></returns>
        internal static bool FocusDocumentApp(string docUri)
        {
            if (WordApp != null)
            {
                Document doc = WordApp.Documents[docUri];
                using (var win = new WindowWrapper(doc.ActiveWindow))
                {
                    win.Activate();
                    win.SetFocus();
                    win.WindowState = WdWindowState.wdWindowStateNormal;

                    WordApp.Activate();
                }
            }

            return false;
        }

        private static void QuitHandler()
        {
            if (WordApp != null)
            {
                WordApp = null;

                context.Clients.All.addMessage("QuitHandler", "Word closed");
                Console.WriteLine("QuitHandler: Word closed");
            }
        }

        public static string OpenDocument(string docUri, string docId)
        {
            Document doc = null;

            try
            {
                if (WordApp == null) WordFactory.CreateInstance();

                if (string.IsNullOrEmpty(docUri))
                {
                    doc = WordApp.Documents.Add();
                }
                else
                {
                    doc = WordApp.Documents.Open(docUri);

                    string windowCaption = doc.ActiveWindow.Caption;
                    Console.WriteLine(windowCaption);

                    IntPtr docHandle = FindWindow(null, windowCaption + " - Word");

                    ShowWindowAsync(new HandleRef(null, docHandle), 9); //need if the window is minimized

                    bool result = SetForegroundWindow(docHandle);


                    DocumentEvents2_Event events = (DocumentEvents2_Event)doc;

                    //example of how to use lambda expressions to create an anonymous function for event handler
                    events.Close += () =>
                    {
                        context.Clients.All.addMessage("CloseDocument-Lambda", doc.FullName + " was closed with DocId:" + docId);
                        Console.WriteLine("CloseDocument-Lambda: Closed Document " + doc.FullName);
                    };
                }

                WordApp.Activate();

                Console.WriteLine("Doc Opened with Doc.Name=" + doc.FullName);
                return doc.FullName;

            }
            catch (COMException ex)
            {
                string msg = string.Empty;

                switch (ex.HResult)
                {
                    case -2146824128:
                        msg = $"Can't open document {docUri}: Please check to see if dialog window is open";
                        break;

                    default:
                        msg = ex.HResult + ":" + ex.Message;
                        break;

                }
                Console.WriteLine(msg);
                context.Clients.All.addMessage(msg);

                return string.Empty;
            }
        }

        public static void CloseDocument(string docUri)
        {
            Document doc;

            if (string.IsNullOrEmpty(docUri))
            {
                Console.WriteLine("No document name specified.");
                return;
            }

            try
            {
                if (WordApp != null)
                {
                    doc = WordApp.Documents[docUri];
                    string fullName = doc.FullName;

                    doc.Close(WdSaveOptions.wdDoNotSaveChanges);

                    context.Clients.All.addMessage("CloseDocument", "Word closed doc " + fullName);
                    Console.WriteLine($"CloseDocument: Doc {fullName} was closed");

                    if (WordApp.Documents.Count == 0)
                    {
                        WordApp.Quit();
                        WordApp = null;
                    }
                }
            }
            catch (Exception)
            {
                string message = $"CloseDocument: Can't close document {docUri}, please check for open dialog box";

                context.Clients.All.addMessage(message);
                Console.WriteLine(message);
            }
        }

        private static void Wordinstance_DocumentBeforeClose(Document Doc, ref bool Cancel)
        {
            context.Clients.All.addMessage("DocumentBeforeClose", "Word closing doc " + Doc.FullName);
            Console.WriteLine($"DocumentBeforeClose: Doc {Doc.FullName} is closing");
        }

        public static void CloseWord()
        {
            string result = "Word already Closed";

            if (WordApp != null)
            {
                try
                {
                    foreach (Document doc in WordApp.Documents)
                    {
                        doc.Activate();
                        WordApp.Activate();
                        WordFactory.CloseDocument(doc.FullName);
                    }

                    //WordApp.Quit(WdSaveOptions.wdDoNotSaveChanges);
                    result = "Word Closed";
                }
                catch (System.Runtime.InteropServices.COMException ex)
                {
                    switch (ex.HResult)
                    {
                        case -2147417846:  //Dialog box is open and blocking us
                            WordApp.Activate();
                            context.Clients.All.addMessage("CloseWord", "Can't close Word, please check for open dialog box");
                            return;

                        case -2147023174:  //Word Instance died without us knowing, need to set back to null to recover
                            context.Clients.All.addMessage("CloseWord", "Word Failed, attempting to recover...");
                            break;

                        default:  //this is to catch the unknown and bubble up the details
                            context.Clients.All.addMessage("CloseWord", $"Oops... Something went wrong  Code {ex.HResult}  Message: {ex.Message}");
                            return;

                    }
                }
            }

            WordApp = null;
            context.Clients.All.addMessage("CloseWord", result);
            Console.WriteLine($"CloseWord: {result}");
        }
    }
}