using System.Configuration;

namespace SAM.Web.Common.Enum
{
    public class EnumVariaveisWebConfig
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
    }
}