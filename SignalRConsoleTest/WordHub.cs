using Microsoft.AspNet.SignalR;
using System;
using System.Threading.Tasks;

namespace SignalRConsoleTest
{
    public class WordHub : Hub
    {
        public void OpenDoc(string docUri, string docId)
        {
            string docName = WordFactory.OpenDocument(docUri, docId);
            Clients.Caller.addMessage("server_" + Context.ConnectionId, "Word opened doc " + docName);
            Console.WriteLine($"Doc {docName} was opened");
        }

        public void CloseDoc(string docUri)
        {
            //add code here to close the specific doc/app instance with docUri and notify the group of the closure
            WordFactory.CloseDocument(docUri);
            Clients.All.docClosed();
            Console.WriteLine($"Doc {docUri} was closed");
        }

        public void CloseWord()
        {
            WordFactory.CloseWord();
            Clients.All.docClosed();
            Console.WriteLine($"Connection: {Context.ConnectionId} Word was closed");
        }

        public void FocusDocPInoke(string docUri)
        {
            bool result = WordFactory.FocusDocumentPinvoke(docUri);
            Clients.All.addMessage($"{nameof(FocusDocPInoke)} returned with {result.ToString()}");
        }

        public void FocusDocApp(string docUri)
        {
            WordFactory.FocusDocumentApp(docUri);
        }

        public override Task OnConnected()
        {
            Clients.Others.addMessage("server_" + Context.ConnectionId, Context.ConnectionId + " has connected");
            Console.WriteLine("Connected: " + Context.ConnectionId);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Clients.Others.addMessage("server_" + Context.ConnectionId, Context.ConnectionId + " has disconnected");
            Console.WriteLine("Disconnected: " + Context.ConnectionId);

            return base.OnDisconnected(stopCalled);
        }
    }
}
