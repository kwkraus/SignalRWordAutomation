using System;
using System.IO;

namespace SignalRConsoleTest
{
    public static class Helper
    {
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
    }
}