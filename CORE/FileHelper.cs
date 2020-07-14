using System;
using System.Data;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace CORE
{
    /// <summary>
    /// Class helps connect ftp server and upload and download files
    /// </summary>
    public sealed class FileHelper
    {
        public string rootpath;
        public byte[] iv;
        public string userid;
        public string password;

        public DbHelper dbHelper;
        public Logger logger;

        /// <summary>
        /// Constructor ( IV = Initial Vector that user should define for encrypting )
        /// </summary>
        /// <param name="rootpath">Ftp path</param>
        /// <param name="userid">Ftp authenication id</param>
        /// <param name="password">Ftp authenication password</param>
        /// <param name="iv">Initial vector</param>
        /// <param name="dbHelper">DbHelper instance</param>
        /// <param name="logger">Logger instance</param>
        public FileHelper(string rootpath, string userid, string password, string iv, DbHelper dbHelper, Logger logger)
        {
            this.dbHelper = dbHelper;
            this.logger = logger;
            this.rootpath = rootpath;
            this.userid = userid;
            this.password = password;
            this.iv = Encoding.Default.GetBytes(iv);
        }

        /// <summary>
        /// Encrypt and upload file to ftp
        /// </summary>
        /// <param name="fileName">file title</param>
        /// <param name="file">file binary</param>
        /// <param name="objectType">type of module where file belongs ( schedule, user etc .. )</param>
        /// <param name="objectId">module id</param>
        /// <returns>File table key (0: Upload error, nothing uploaded)</returns>
        public int Upload(string fileName, byte[] file, int objectType, int objectId)
        {
            byte[] key = GetKey();
            byte[] encrypted = Encrypt(file, key);

            string[] fileRow = new string[2];
            dbHelper.AddOutput("@id", SqlDbType.Int);
            dbHelper.AddOutput("@title", SqlDbType.VarChar, 16);
            dbHelper.CallSP("p_GetFileRow", ref fileRow);

            if (fileRow[0] == "-1") return 0;

            try
            {
                FtpWebRequest req = (FtpWebRequest)WebRequest.Create(rootpath + "/" + fileRow[1] + ".ef");
                req.Method = WebRequestMethods.Ftp.UploadFile;
                req.Credentials = new NetworkCredential(userid, password);
                req.KeepAlive = false;
                req.UseBinary = true;                
                
                using (Stream stream = req.GetRequestStream())                
                    stream.Write(encrypted, 0, encrypted.Length);
                
            }catch(Exception e)
            {
                logger.WriteText(e.ToString());
                dbHelper.AddInput("@id", fileRow[0], SqlDbType.Int);
                dbHelper.CallQuery("delete [File] where id = " + fileRow[0]);
                return 0;
            }

            dbHelper.AddInput("@KeyCode", key, SqlDbType.Binary);
            dbHelper.CallQuery("update [File] set [RegisteredDate] = getdate()"
                + ", [Directory] = '" + rootpath
                + "', [Name] = '" + fileName
                + "', [KeyCode] = @KeyCode"
                + " where id = " + fileRow[0]);

            return int.Parse(fileRow[0]);
        }

        /// <summary>
        /// Decrypt and download file id
        /// </summary>
        /// <param name="id">File id ( Table key )</param>
        /// <returns>Bytearray file</returns>
        public byte[] DownLoad(int id)
        {
            //get id from path

            DataSet ds = new DataSet();            
            dbHelper.CallQuery("select [EncryptedName], [KeyCode] from [File] where id = " + id, ref ds);
            DataRow dr = ds.Tables[0].Rows[0];
            byte[] data = null;

            try
            {
                FtpWebRequest req = (FtpWebRequest)WebRequest.Create(rootpath + "/" + dr["EncryptedName"] + ".ef");
                req.Method = WebRequestMethods.Ftp.DownloadFile;
                req.Credentials = new NetworkCredential(userid, password);
                req.KeepAlive = false;
                req.UseBinary = true;

                using (FtpWebResponse resp = (FtpWebResponse)req.GetResponse())
                using (Stream stream = resp.GetResponseStream())
                using (MemoryStream ms = new MemoryStream())
                {
                    byte[] buf;
                    int count = 0;

                    do {
                        buf = new byte[1024];
                        count = stream.Read(buf, 0, 1024);
                        ms.Write(buf, 0, count);
                    } while (stream.CanRead && count > 0);

                    data = ms.ToArray();
                }

            }catch(Exception e)
            {
                logger.WriteText(e.ToString());
                return null;
            }            
            
            return Decrypt(data, (byte[])dr["KeyCode"]);
        }

        //Decrypt byte[]
        private byte[] Decrypt(byte[] data, byte[] key)
        {
            MemoryStream ms = new MemoryStream();
            Rijndael alg = Rijndael.Create();
            alg.Key = key;
            alg.IV = iv;

            using(CryptoStream cs = new CryptoStream
                (ms, alg.CreateDecryptor(), CryptoStreamMode.Write))
            {
                cs.Write(data, 0, data.Length);
            }
            
            byte[] decryptedData = ms.ToArray();
            return decryptedData;
        }

        //Encrypt byte[]
        private byte[] Encrypt(byte[] data, byte[] key)
        {
            MemoryStream ms = new MemoryStream();
            Rijndael alg = Rijndael.Create();
            alg.Key = key;
            alg.IV = iv;

            using(CryptoStream cs = new CryptoStream
                (ms, alg.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cs.Write(data, 0, data.Length);
            }
            
            byte[] encryptedData = ms.ToArray();
            return encryptedData;
        }

        //Get encrypt key
        private byte[] GetKey()
        {
            Random r = new Random();
            byte[] array = new byte[16];
            r.NextBytes(array);
            return array;
        }
    }
}
