using SAM.Web.Autorizacao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAM.Web.services
{
    public class AuthorizeServices
    {
        private string key = "sam2020083677544";
        private string iv = "sam2020081234567"; //16 bytes
        private string conteudo = "Sam Patrimonio_Sam Estoque";
        /* Convert.ToBase64String para converter bytes em string
           Convert.FromBase64String para conveter string em bytes
         */
        private CriptografiaAes criptografiaAes;

        public AuthorizeServices()
        {
            this.criptografiaAes = new CriptografiaAes(this.key, this.iv);
        }
        

        public bool ValidateToken(string token)
        {
            criptografiaAes.EncryptAesManaged(conteudo);
            var resultado = criptografiaAes.Encrypted;

            var base64 = Convert.ToBase64String(resultado);

            if (base64.Equals(token))
                return true;

            return false;
        }
        public byte[] Encrypt(string _conteudo)
        {
            criptografiaAes.EncryptAesManaged(_conteudo);
            var resultado = criptografiaAes.Encrypted;

            return resultado;
        }
        public string Dencrypt(byte[] encrypted)
        {
            criptografiaAes.DencryptAesManaged(encrypted);
            var resultado = criptografiaAes.Decrypted;

            return resultado;
        }

       
    }
}