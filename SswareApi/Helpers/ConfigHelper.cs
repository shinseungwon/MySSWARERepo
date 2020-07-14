using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Helper
{
    /// <summary>
    /// Conf file
    /// elements divide by line break
    /// key and value divide by : (colon)
    /// key starts with ' is comment
    /// </summary>
    public class ConfigHelper
    {
        public Dictionary<string, string> Configs;

        public ConfigHelper(string filePath)
        {
            Configs = new Dictionary<string, string>();
            string configText = File.ReadAllText(filePath);
            string[] configLines = configText.Replace("\r\n", "\n").Split('\n');
            string[] keyAndValue;
            string key, value;
            int i;

            foreach (string s in configLines)
            {
                if (s.Length > 1)
                {
                    if (!s.StartsWith('`'))
                    {
                        keyAndValue = s.Split(':');
                        key = keyAndValue[0];
                        value = "";
                        for (i = 1; i < keyAndValue.Length; i++)
                        {
                            value += keyAndValue[i];
                            if (i != keyAndValue.Length - 1)
                            {
                                value += ":";
                            }
                        }
                        Configs.Add(key, value);
                    }
                }
            }
        }

        public static string Encrypt(string key, string data)
        {
            Rijndael r = Rijndael.Create();
            byte[] keyBytes = Encoding.ASCII.GetBytes(key);
            byte[] dataBytes = Encoding.ASCII.GetBytes(data);
            r.Key = keyBytes;
            r.IV = keyBytes;

            byte[] encryptedData;

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(
                ms, r.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(dataBytes, 0, dataBytes.Length);
                }
                encryptedData = ms.ToArray();
            }

            return Convert.ToBase64String(encryptedData);
        }

        public static string Decrypt(string key, string data)
        {
            Rijndael r = Rijndael.Create();
            byte[] keyBytes = Encoding.ASCII.GetBytes(key);
            byte[] dataBytes = Convert.FromBase64String(data);
            r.Key = keyBytes;
            r.IV = keyBytes;
            byte[] decryptedData;

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(
                    ms, r.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(dataBytes, 0, dataBytes.Length);
                }
                decryptedData = ms.ToArray();
            }

            return Encoding.ASCII.GetString(decryptedData);
        }
    }
}
