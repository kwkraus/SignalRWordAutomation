using Microsoft.AspNet.SignalR;
using System;
using System.Web.Http;

namespace SignalRConsoleTest.Controllers
{
    public class WordController : ApiController
    {
        // GET api/values 
        [HttpGet]
        [ActionName("CloseAll")]
        public void Close()
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<WordHub>();
            context.Clients.All.addMessage("Word closed from AutoClose Macro");
            context.Clients.All.docClosed();
            Console.WriteLine("Word called CloseAll WebApi to notify browser");
        }

        [HttpGet]
        [HttpHead]
        public IHttpActionResult GetWordIds(string id)
        {
            return Ok(id);

        }
    }
}
