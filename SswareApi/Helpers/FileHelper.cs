using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Helper
{
    /// <summary>
    /// Class helps connect ftp server or file location and upload and download files
    /// </summary>
    public sealed class FileHelper
    {
        private readonly int AccessType; //0 : file server, 1 : ftp
        private readonly string RootPath;        
        private readonly string UserId;
        private readonly string Password;
        private readonly byte[] Iv;
        
        //Common constructor
        private FileHelper(byte[] iv)
        {
            if (iv != null)
            {
                Iv = iv;
            }
            else
            {
                Iv = null;
            }
        }

        /// <summary>
        /// File server connection
        /// </summary>
        /// <param name="rootpath">Directory</param>
        /// <param name="iv">16 digit string</param>
        public FileHelper(string rootpath, byte[] iv = null) : this(iv)
        {
            AccessType = 0;
            RootPath = rootpath;            
        }

        /// <summary>
        /// Ftp connection
        /// </summary>
        /// <param name="rootpath">Ip + directory</param>
        /// <param name="userid"></param>
        /// <param name="password"></param>
        /// <param name="iv">16 digit string</param>
        public FileHelper(string rootpath, string userid, string password, byte[] iv = null) : this(iv)
        {
            AccessType = 1;
            RootPath = rootpath;
            UserId = userid;
            Password = password;            
        }        
        
        /// <summary>
        /// Upload file
        /// </summary>
        /// <param name="filePath">Path where file uploaded</param>
        /// <param name="fileContent">File content</param>
        /// <param name="key">key : byte[16](if not appropriated, set new key and return key</param>
        /// <returns>Key</returns>
        public byte[] Upload(string filePath, byte[] fileContent, bool encrypt = false)
        {
            byte[] key = null;
            byte[] fileBytes ;
            string directoryName;            

            if (encrypt)
            {
                key = GetKey();             
                fileBytes = Encrypt(fileContent, key);                
            }
            else
            {
                fileBytes = fileContent;
            }

            if (AccessType == 0)
            {
                directoryName = Path.GetDirectoryName(RootPath + filePath);

                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }

                using (FileStream fs = File.Open(RootPath + filePath, FileMode.Create))
                using (BinaryWriter wr = new BinaryWriter(fs))
                {
                    wr.Write(fileBytes);
                }
            }
            else
            {
                int index = filePath.LastIndexOf("/");
                FtpWebRequest req;
                if (index != -1)
                {
                    directoryName = RootPath + filePath.Substring(0, index);
                    req = (FtpWebRequest)WebRequest.Create(directoryName);
                    req.Method = WebRequestMethods.Ftp.MakeDirectory;
                    req.Credentials = new NetworkCredential(UserId, Password);
                    req.KeepAlive = false;
                    req.UseBinary = true;

                    FtpWebResponse res = (FtpWebResponse)req.GetResponse();
                    Console.WriteLine(res.StatusCode);
                }
                else
                {
                    directoryName = RootPath + filePath;
                }

                req = (FtpWebRequest)WebRequest.Create(directoryName);
                req.Method = WebRequestMethods.Ftp.UploadFile;
                req.Credentials = new NetworkCredential(UserId, Password);
                req.KeepAlive = false;
                req.UseBinary = true;

                using (Stream s = req.GetRequestStream())
                {
                    s.Write(fileBytes);
                }
            }            
            
            return key;
        }

        /// <summary>
        /// Download file
        /// </summary>
        /// <param name="sourceFilePath">Path where source file exists</param>
        /// <param name="filePath">Path where file want to downloaded</param>
        /// <param name="key">16 digit key for decrypt file</param>
        /// <returns>File byte[]</returns>
        public byte[] Download(string sourceFilePath, string filePath = null, byte[] key = null)
        {            
            byte[] fileBytes = null;            
            sourceFilePath = RootPath + sourceFilePath;
            string targetFilePath = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(targetFilePath))
            {
                Directory.CreateDirectory(targetFilePath);
            }

            if (AccessType == 0)
            {
                fileBytes = File.ReadAllBytes(sourceFilePath);
            }
            else
            {
                FtpWebRequest req = (FtpWebRequest)WebRequest.Create(sourceFilePath);
                req.Method = WebRequestMethods.Ftp.DownloadFile;
                req.Credentials = new NetworkCredential(UserId, Password);
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

                    fileBytes = ms.ToArray();
                }
            }

            if(key != null)
            {
                fileBytes = Decrypt(fileBytes, key);
            }
            
            if(filePath != null)
            {                
                using (FileStream fs = File.Open(filePath, FileMode.Create))
                using (BinaryWriter wr = new BinaryWriter(fs))
                {
                    wr.Write(fileBytes);
                }
            }            

            return fileBytes;
        }

        //Decrypt byte[]
        private byte[] Decrypt(byte[] data, byte[] key)
        {            
            Rijndael r = Rijndael.Create();
            r.Key = key;
            r.IV = Iv;            
            byte[] decryptedData;

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(
                    ms, r.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(data, 0, data.Length);                    
                }
                decryptedData = ms.ToArray();
            }
             
            return decryptedData;
        }

        //Encrypt byte[]
        private byte[] Encrypt(byte[] data, byte[] key)
        {            
            Rijndael r = Rijndael.Create();
            r.Key = key;
            r.IV = Iv;            
            byte[] encryptedData;

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(
                ms, r.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(data, 0, data.Length);                    
                }
                encryptedData = ms.ToArray();
            }
             
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
