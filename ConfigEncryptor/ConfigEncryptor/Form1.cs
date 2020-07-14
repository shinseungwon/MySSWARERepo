using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace ConfigEncryptor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public string Encrypt(string key, string data)
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

        public string Decrypt(string key, string data)
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

        private void EncryptButton_Click(object sender, EventArgs e)
        {
            string key = KeyTextBox.Text;
            string data = DecryptTextBox.Text;                        
            EncryptTextBox.Text = Encrypt(key, data);
        }

        private void DecryptButton_Click(object sender, EventArgs e)
        {
            string key = KeyTextBox.Text;
            string data = EncryptTextBox.Text;
            DecryptTextBox.Text = Decrypt(key, data);
        }
    }
}