using System;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Helper
{
    /// <summary>
    /// Logger class
    /// </summary>
    public sealed class LogHelper
    {
        private readonly string ProgramName;
        private readonly string Path;
        private readonly string Ip;
        public DbHelper DbHelper;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">Write text file path</param>
        /// <param name="tag">Title for log line</param>
        public LogHelper(string path)
        {
            ProgramName = AppDomain.CurrentDomain.FriendlyName;
            Path = path;

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            for (int i = 0; i < host.AddressList.Length; i++)
            {
                if (host.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    Ip = host.AddressList[i].ToString();
                    break;
                }
            }
        }

        /// <summary>
        /// Get directory
        /// </summary>
        /// <param name="logHelperMode"></param>
        /// <returns></returns>
        private string GetDirectory(LogHelperMode logHelperMode)
        {
            string fullPath = Path;

            if (logHelperMode == LogHelperMode.Yearly)
            {
                fullPath += DateTime.Now.ToString("yyyy");
            }
            else if (logHelperMode == LogHelperMode.Monthly)
            {
                fullPath += DateTime.Now.ToString("yyyy")
                    + @"\" + DateTime.Now.ToString("MM");
            }
            else if (logHelperMode == LogHelperMode.Daily)
            {
                fullPath += DateTime.Now.ToString("yyyy")
                    + @"\" + DateTime.Now.ToString("MM")
                    + @"\" + DateTime.Now.ToString("dd");
            }

            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            return fullPath;
        }

        /// <summary>
        /// Write log text file ( each month each file )
        /// </summary>
        /// <param name="str">Log string</param>
        public void WriteText(string tag, string str
            , LogHelperMode logHelperMode = LogHelperMode.Yearly)
        {
            FileStream fs;
            string fullPath = GetDirectory(logHelperMode) + ".txt";

            if (!File.Exists(fullPath))
            {
                fs = File.Create(fullPath);
            }
            else
            {
                fs = new FileStream(fullPath, FileMode.Append);
            }

            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss]\n")
                    + "<" + ProgramName + "-" + tag + ">\n" + str + "\n");
            }

            fs.Close();            
        }

        /// <summary>
        /// Write log to file ( txt )
        /// </summary>
        /// <param name="str">Log file string</param>
        /// <param name="title">File name</param>
        /// <returns>0:wrote successfully, 1:file already exist</returns>
        public void WriteFile(string title, string body
            , LogHelperMode logHelperMode = LogHelperMode.Yearly)
        {
            string directory = GetDirectory(logHelperMode) + @"\";
            
            string fullPath = directory + title + ".txt";

            using (FileStream fs = File.Create(fullPath))
            {
                fs.Write(Encoding.ASCII.GetBytes(body), 0, body.Length);
            }
        }

        /// <summary>
        /// Datatable to csv file
        /// </summary>
        /// <param name="dt">Datatable object</param>        
        /// <param name="title">Csv file title</param>
        /// <returns>0:wrote successfully, 1:file already exist</returns>
        public void WriteFile(DataTable dt, string title = ""
            , LogHelperMode logHelperMode = LogHelperMode.Yearly)
        {
            string directory = GetDirectory(logHelperMode) + @"\";

            string fullPath = directory + title + ".csv";

            using (FileStream fs = File.Create(fullPath))
            {
                StringBuilder sb = new StringBuilder();
                string comma = "";

                foreach (DataColumn col in dt.Columns)
                {
                    sb.Append(comma + "\"" + col.ColumnName + "\"");
                    comma = ",";
                }

                foreach (DataRow row in dt.Rows)
                {
                    comma = "";
                    sb.Append(Environment.NewLine);
                    foreach (DataColumn col in dt.Columns)
                    {
                        sb.Append(comma + "\"" + row[col] + "\"");
                        comma = ",";
                    }
                }

                byte[] bytes = Encoding.Default.GetBytes(sb.ToString());
                fs.Write(bytes, 0, bytes.Length);
            }
        }

        /// <summary>
        /// Write log in db
        /// Columns
        /// ip : varchar(50)
        /// machine : nvarchar(200)
        /// tag : nvarchar(200)
        /// description : nvarchar(1000)
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="desc"></param>
        public void WriteDb(string tag, string desc)
        {
            WriteText(tag, desc);
            DbHelper.Query("insert into Logz (ip, Program, Tag, Descriptionz) values ('"
                + Ip + "', '" + ProgramName + "', '" + tag + "', '" + desc.Replace("'", "''") + "')");
        }
    }

    public enum LogHelperMode
    {
        Yearly = 0,
        Monthly = 1,
        Daily = 2
    }
}
