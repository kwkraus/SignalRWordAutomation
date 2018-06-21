using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SignalRConsoleTest.Controllers
{
    public class FileController : ApiController
    {
        [EnableCors(origins: "*", headers: "*", methods: "*")]

        [ActionName("GetEastFiles")]
        public IHttpActionResult GetEastFiles(string ID)
        {
            var files = new List<FileList>();
            string eastFolder = @"\EAST\OACS Interface\";

            string myDocsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string eastPath = myDocsPath + eastFolder;

            if (!Directory.Exists(eastPath))
            {
                return BadRequest("Directory doesn't exist" + eastPath);
            }

            foreach (string filePath in Directory.GetFiles(eastPath))
            {
                if (filePath.Contains(ID) && filePath.Contains(".east"))
                {
                    string[] filePathParts = filePath.Split('\\');
                    string fileName = filePathParts[filePathParts.GetUpperBound(0)].ToString();
                    
                    string encodedData = Helper.GetBase64StringFromPath(filePath);

                    double noOfSeconds = DateTime.UtcNow.Subtract(Convert.ToDateTime("1/1/1970 00:00:00")).TotalSeconds;

                    files.Add(new FileList { filename = fileName, timestamp = noOfSeconds.ToString(), content = encodedData });
                }
            }

            return Ok(files);
        }
    }
}
