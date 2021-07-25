using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Linq;
using System.Security.Cryptography;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections.Specialized;
using System.Web.Configuration;

namespace Sam.Common.Util
{
    public class Helper
    {
        public static string Criptografar(string value)
        {
            StringBuilder result = new StringBuilder();

            if (!string.IsNullOrEmpty(value))
            {
                MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();
                byte[] bValue = System.Text.Encoding.UTF8.GetBytes(value.ToUpper());
                bValue = MD5.ComputeHash(bValue);
                
                foreach (byte b in bValue)
                {
                    result.Append(b.ToString("X2").ToUpper());
                }
            }
            return result.ToString();
        }

        public static bool IsDate(string value)
        {
            try
            {
                DateTime dt = DateTime.Parse(value);
                if(dt != DateTime.MinValue  && dt != DateTime.MaxValue)
                    return true;
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static string RemoverAcentos(string texto)
        {
            string ComAcentos = "!@#$%¨&*()-?:{}][ÄÅÁÂÀÃäáâàãÉÊËÈéêëèÍÎÏÌíîïìÖÓÔÒÕöóôòõÜÚÛüúûùÇç ";
            string SemAcentos = "_________________AAAAAAaaaaaEEEEeeeeIIIIiiiiOOOOOoooooUUUuuuuCc_";

            for (int i = 0; i < ComAcentos.Length; i++)
                texto = texto.Replace(ComAcentos[i].ToString(), SemAcentos[i].ToString()).Trim();

            return texto;

        }

        public static double ObterTimeoutDoConfig()
        {
            var _sessao = (SessionStateSection)WebConfigurationManager.GetSection("system.web/sessionState");
            return _sessao.Timeout.TotalMinutes;
        }
    }
}
