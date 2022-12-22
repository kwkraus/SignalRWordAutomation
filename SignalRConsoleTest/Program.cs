using Microsoft.Owin.Hosting;
using System;

namespace SignalRSelfHost
{
    class Program
    {
        static void Main(string[] args)
        {
            // This will *ONLY* bind to localhost, if you want to bind to all addresses
            // use http://*:8080 to bind to all addresses. 
            // See http://msdn.microsoft.com/en-us/library/system.net.httplistener.aspx 
            // for more information.
            string url = "http://localhost:5050";
            using (WebApp.Start(url))
            {
                Console.WriteLine("Server running on {0}, press Enter key to exit Program", url);
                Console.ReadLine();
            }

            //create word instance on startup to improve performance on initial doc opening
            //WordFactory.CreateInstance();
        }
    }
}