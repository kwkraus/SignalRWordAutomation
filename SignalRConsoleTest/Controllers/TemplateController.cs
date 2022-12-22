using SignalRConsoleTest.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SignalRConsoleTest.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TemplateController : ApiController
    {
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [ActionName("GetTemplateList")]
        public IHttpActionResult GetTemplateList()
        {

            string connectionString = string.Empty;
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var connectionStringsSection = (ConnectionStringsSection)config.GetSection("connectionStrings");
            if (connectionStringsSection != null)
            {
                connectionString = connectionStringsSection.ConnectionStrings["Access"].ConnectionString;
            }

            string queryString = "SELECT * FROM Scenarios WHERE scenType='T'";

            List<string> templateList = new List<string>();

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                using (OleDbCommand command = new OleDbCommand(queryString, connection))
                {
                    try
                    {
                        connection.Open();
                        OleDbDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            templateList.Add(reader[1].ToString());
                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            return Ok(templateList);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [ActionName("GetTemplate")]
        public IHttpActionResult GetTemplate(string templateName)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT sfp.Sequence, sfp.Name, sfp.Description ");
            query.Append("FROM Scenarios s RIGHT JOIN ScenarioFormParagraphs sfp ON s.scenID = sfp.scenID ");
            query.Append($"WHERE (s.scenName) = '{templateName}' ");
            query.Append($"ORDER BY sfp.Sequence");
            string queryString = query.ToString();

            string connectionString = string.Empty;
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var connectionStringsSection = (ConnectionStringsSection)config.GetSection("connectionStrings");
            if (connectionStringsSection != null)
            {
                connectionString = connectionStringsSection.ConnectionStrings["Access"].ConnectionString;
            }

            Template template = new Template();
            template.title = templateName;
            template.userTemplateFormParagraphs = new List<TemplateFP>();

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                using (OleDbCommand command = new OleDbCommand(queryString, connection))
                {
                    try
                    {
                        connection.Open();
                        OleDbDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            TemplateFP fp = new TemplateFP();
                            string fpId = reader[1].ToString();
                            int fpSequence = Int32.Parse(reader[0].ToString());
                            string fpDescription = reader[2].ToString();

                            string searchTxt = "fp =";
                            fp.id = fpId.Substring(fpId.IndexOf(searchTxt) + searchTxt.Length).Trim();
                            fp.displayOrder = fpSequence;
                            fp.formparagraph = fpDescription;

                            template.userTemplateFormParagraphs.Add(fp);

                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            return Ok(template);
        }

        [HttpPost]
        [ActionName("Upload")]
        public IHttpActionResult Upload(List<string> templatesToUpload)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT s.scenName, s.scenType, sfp.Sequence, sfp.Name ");
            query.Append("FROM Scenarios s INNER JOIN ScenarioFormParagraphs sfp ON s.scenID = sfp.scenID ");
            query.Append("WHERE s.scenType = 'T'");
            //string queryString = "SELECT s.scenName, s.scenType, sfp.Sequence, sfp.Name FROM Scenarios s INNER JOIN ScenarioFormParagraphs sfp ON s.scenID = sfp.scenID";
            //string queryString = "SELECT * FROM Scenarios";
            string queryString = query.ToString();

            return Ok();
        }
    }
}