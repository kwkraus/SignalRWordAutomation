using System.Collections.Generic;

namespace SignalRConsoleTest.Entities
{
    public class Template
    {
        public string title { get; set; }
        public List<TemplateFP> userTemplateFormParagraphs { get; set; }

    }

    public class TemplateFP
    {
        public string id { get; set; }
        public int displayOrder { get; set; }
        public string formparagraph { get; set; }
    }
}
