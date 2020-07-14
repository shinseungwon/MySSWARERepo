using System;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CORE
{
    /// <summary>
    /// Logger class
    /// </summary>
    public sealed class Logger
    {
        private readonly string Path;
        private readonly string ProgramName;
        private readonly string FullPath;
        private readonly string Ip;
        private DbHelper Db;
        public enum FileType { TXT, CSV };        

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">Write text file path</param>
        /// <param name="tag">Title for log line</param>
        public Logger(string path)
        {
            Path = path;
            ProgramName = System.AppDomain.CurrentDomain.FriendlyName;
            FullPath = path + @"\" + DateTime.Now.ToString("yyyyMM");

            if (!Directory.Exists(FullPath))
            {
                Directory.CreateDirectory(FullPath);
            }

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            for (int i = 0; i < host.AddressList.Length; i++)
            {
                if (host.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    Ip = host.AddressList[i].ToString();
                }
            }
        }

        /// <summary>
        /// Set DbHelper object for writedb
        /// </summary>
        /// <param name="dbHelper"></param>
        public void SetDbHelper(DbHelper dbHelper)
        {
            Db = dbHelper;
        }

        /// <summary>
        /// Write log text file ( each month each file )
        /// </summary>
        /// <param name="str">Log string</param>
        public void WriteText(string str)
        {
            FileStream fs;

            if (!File.Exists(FullPath + ".txt"))
            {
                fs = File.Create(FullPath + ".txt");
            }
            else
            {
                fs = new FileStream(FullPath + ".txt", FileMode.Append);
            }

            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss]") + "[" + ProgramName + "]" + str);
            }
        }

        /// <summary>
        /// Write log to file ( txt, csv )
        /// </summary>
        /// <param name="str">Log file string</param>
        /// <param name="type">File type</param>
        /// <param name="title">File name</param>
        /// <returns>0:wrote successfully, 1:file already exist</returns>
        public int WriteFile(string str, FileType type, string title = "")
        {
            string FullPathWithTitle = FullPath + @"\"
                + DateTime.Now.ToString("yyyyMMddHHmmss") + title + "." + type.ToString();

            if (File.Exists(FullPathWithTitle)) return 1;
            else
            {
                using (FileStream fs = File.Create(FullPathWithTitle))
                {
                    fs.Write(Encoding.ASCII.GetBytes(str), 0, str.Length);
                }
                return 0;
            }
        }

        /// <summary>
        /// Datatable to csv file
        /// </summary>
        /// <param name="dt">Datatable object</param>        
        /// <param name="title">Csv file title</param>
        /// <returns>0:wrote successfully, 1:file already exist</returns>
        public int WriteFile(DataTable dt, string title = "")
        {
            string FullPathWithTitle = FullPath + @"\"
                + DateTime.Now.ToString("yyyyMMddHHmmss") + title + ".csv";

            if (File.Exists(FullPathWithTitle)) return 1;
            else
            {
                using (FileStream fs = File.Create(FullPathWithTitle))
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
                return 0;
            }
        }

        /// <summary>
        /// Write log to database table
        /// </summary>
        /// <param name="db">DbHelper object</param>
        /// <param name="str">Log string</param>
        /// <param name="part">Write table name 'Logs' + part</param>
        /// <returns>Count of rows affected(if no dbHelper return -1)</returns>
        public int WriteDb(string tag, string desc)
        {
            WriteText("<DB:" + ProgramName + ">(" + tag + ") " + desc);

            return Db != null 
                ? Db.CallQuery("insert into [Log] (ip, machine, tag, description) values ('" 
                + Ip + "', '" + ProgramName + "', '" + tag + "', '" + desc + "')") 
                : -1;            
        }
    }
}
