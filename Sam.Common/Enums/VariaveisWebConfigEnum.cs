using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Sam.Common.Enums
{
    public class VariaveisWebConfigEnum
    {
        #region Configurações de Email
        public static string eMailParaEnvioSuporteSam
        {
            get
            {
                return ConfigurationManager.AppSettings["eMailParaEnvioSuporteSam"];
            }
        }

        public static string senhaEMailParaEnvioSuporteSam
        {
            get
            {
                return ConfigurationManager.AppSettings["senhaEMailParaEnvioSuporteSam"];
            }
        }

        public static string EnderecoServidorEMail
        {
            get
            {
                return ConfigurationManager.AppSettings["enderecoServidorEMail"];
            }
        }

        public static int PortaEnvioEmail
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings["portaEnvioEmail"]);
            }
        }

        public static bool UtilizaSSL
        {
            get
            {
                return bool.Parse(ConfigurationManager.AppSettings["utilizaSSL"]);
            }
        }

        #endregion

        #region Configurações de proxy
        public static string EnderecoProxy
        {
            get
            {
                return ConfigurationManager.AppSettings["enderecoProxy"];
            }
        }

        public static string UserNameProxy
        {
            get
            {
                return ConfigurationManager.AppSettings["userNameProxy"];
            }
        }

        public static string PassProxy
        {
            get
            {
                return ConfigurationManager.AppSettings["passProxy"];
            }
        }

        #endregion

    }
}
