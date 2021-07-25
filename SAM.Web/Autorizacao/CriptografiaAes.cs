using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace SAM.Web.Autorizacao
{
    public class CriptografiaAes
    {
        private AesManaged aesManaged = null;

     
        public byte[] Key { get; private set; }
        public byte[] Iv { get; private set; }
        public byte[] Encrypted { get; private set; }
        public string Decrypted { get; private set; }

        public CriptografiaAes(string key, string iv)
        {
            this.aesManaged = new AesManaged();
            this.aesManaged.GenerateKey();
            this.aesManaged.GenerateIV();

            this.Key = Encoding.UTF8.GetBytes(key);
            this.Iv = Encoding.UTF8.GetBytes(iv);

        }
        public void EncryptAesManaged(string content)
        {
            try
            {

                // Encrypt string    
                this.Encrypted = Encrypt(content, this.Key, this.Iv);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void DencryptAesManaged(byte[] encrypted)
        {
            try
            {

                this.Decrypted = Decrypt(encrypted, this.Key, this.Iv);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private byte[] Encrypt(string plainText, byte[] Key, byte[] IV)
        {
            byte[] encrypted;

            ICryptoTransform encryptor = this.aesManaged.CreateEncryptor(Key, IV);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter sw = new StreamWriter(cs))
                        sw.Write(plainText);
                    encrypted = ms.ToArray();
                }
            }

            return encrypted;
        }
        private string Decrypt(byte[] cipherText, byte[] Key, byte[] IV)
        {
            string plaintext = null;

            ICryptoTransform decryptor = this.aesManaged.CreateDecryptor(Key, IV);
            using (MemoryStream ms = new MemoryStream(cipherText))
            {
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader reader = new StreamReader(cs))
                        plaintext = reader.ReadToEnd();
                }
            }

            return plaintext;
        }
    }
}