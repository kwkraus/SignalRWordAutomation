using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRConsoleTest
{
    public static class Helper
    {
        public static byte[] GetRandomByteArray(int ArrayLength)
        {
            Random rnd = new Random();
            Byte[] b = new Byte[10];
            rnd.NextBytes(b);
            return b;
        }

        public static string GetBase64StringFromPath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException("FilePath cannot be empty or null");

            FileStream fs;
            string encodedData = string.Empty;

            try
            {
                using (fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    byte[] filebytes = new byte[fs.Length];
                    fs.Read(filebytes, 0, Convert.ToInt32(fs.Length));
                    encodedData = Convert.ToBase64String(filebytes, Base64FormattingOptions.InsertLineBreaks);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"File {filePath} was not found", ex);
            }

            return encodedData;
        }

        //public void ForceForegroundWindow(IntPtr hWnd)
        //{
        //    uint a;
        //    WinAPI.LockSetForegroundWindow(WinAPI.LSFW_UNLOCK);
        //    WinAPI.AllowSetForegroundWindow(WinAPI.ASFW_ANY);

        //    IntPtr hWndForeground = WinAPI.GetForegroundWindow();
        //    SendKeys.SendWait("{UP}");
        //    if (hWndForeground.ToInt32() != 0)
        //    {
        //        if (hWndForeground != hWnd)
        //        {
        //            uint thread1 = WinAPI.GetWindowThreadProcessId(hWndForeground, out a);
        //            uint thread2 = WinAPI.GetCurrentThreadId();


        //            if (thread1 != thread2)
        //            {
        //                WinAPI.AttachThreadInput(thread1, thread2, true);
        //                WinAPI.LockSetForegroundWindow(WinAPI.LSFW_UNLOCK);
        //                WinAPI.AllowSetForegroundWindow(WinAPI.ASFW_ANY);
        //                WinAPI.BringWindowToTop(hWnd);
        //                if (WinAPI.IsIconic(hWnd))
        //                {
        //                    WinAPI.ShowWindow(hWnd, WinAPI.ShowWindowFlags.SW_SHOWNORMAL);
        //                }
        //                else
        //                {
        //                    WinAPI.ShowWindow(hWnd, WinAPI.ShowWindowFlags.SW_SHOW);
        //                }
        //                WinAPI.SetFocus(hWnd);
        //                WinAPI.AttachThreadInput(thread1, thread2, false);
        //            }
        //            else
        //            {
        //                WinAPI.AttachThreadInput(thread1, thread2, true);
        //                WinAPI.LockSetForegroundWindow(WinAPI.LSFW_UNLOCK);
        //                WinAPI.AllowSetForegroundWindow(WinAPI.ASFW_ANY);
        //                WinAPI.BringWindowToTop(hWnd);
        //                WinAPI.SetForegroundWindow(hWnd);
        //                WinAPI.SetFocus(hWnd);
        //                WinAPI.AttachThreadInput(thread1, thread2, false);

        //            }
        //            if (WinAPI.IsIconic(hWnd))
        //            {
        //                WinAPI.AttachThreadInput(thread1, thread2, true);
        //                WinAPI.LockSetForegroundWindow(WinAPI.LSFW_UNLOCK);
        //                WinAPI.AllowSetForegroundWindow(WinAPI.ASFW_ANY);
        //                WinAPI.BringWindowToTop(hWnd);
        //                WinAPI.ShowWindow(hWnd, WinAPI.ShowWindowFlags.SW_SHOWNORMAL);
        //                WinAPI.SetFocus(hWnd);
        //                WinAPI.AttachThreadInput(thread1, thread2, false);
        //            }
        //            else
        //            {
        //                WinAPI.BringWindowToTop(hWnd);
        //                WinAPI.ShowWindow(hWnd, WinAPI.ShowWindowFlags.SW_SHOW);
        //            }
        //        }
        //        WinAPI.SetForegroundWindow(hWnd);
        //        WinAPI.SetFocus(hWnd);
        //    }
        //    else
        //    {
        //        uint thread1 = WinAPI.GetWindowThreadProcessId(hWndForeground, out a);
        //        uint thread2 = WinAPI.GetCurrentThreadId();
        //        try
        //        {
        //            WinAPI.AttachThreadInput(thread1, thread2, true);
        //        }
        //        catch
        //        {
        //            uint failure = 1;
        //        }
        //        WinAPI.LockSetForegroundWindow(WinAPI.LSFW_UNLOCK);
        //        WinAPI.AllowSetForegroundWindow(WinAPI.ASFW_ANY);
        //        WinAPI.BringWindowToTop(hWnd);
        //        WinAPI.SetForegroundWindow(hWnd);

        //        WinAPI.ShowWindow(hWnd, WinAPI.ShowWindowFlags.SW_SHOW);
        //        WinAPI.SetFocus(hWnd);
        //        WinAPI.AttachThreadInput(thread1, thread2, false);
        //    }
        //}
    }
}