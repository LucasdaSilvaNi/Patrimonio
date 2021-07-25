using System;
using System.Configuration;
using System.Linq;
using System.Web.Configuration;


namespace Sam.Integracao.SIAF.Configuracao
{
    public class ConfiguracoesSIAF
    {
        private bool exibeCabecalhoMsgErro;
        /// <summary>
        /// Define se a mensagem de erro ira descriminar sistema (SIAFEM/SIAFISICO) de origem do erro
        /// </summary>
        public bool ExibeMsgErroCompleta
        {
            get { return exibeCabecalhoMsgErro; }
            set { exibeCabecalhoMsgErro = value; }
        }


        public static bool ambienteDesenvolvimento
        {
            get
            {
                bool valorChave = false;

                try
                {
                    if (ConfigurationManager.AppSettings.AllKeys.Contains("ambienteDesenvolvimento"))
                        valorChave = (ConfigurationManager.AppSettings["ambienteDesenvolvimento"].ToLowerInvariant() == "true");
                }
                catch (Exception)
                {
                }

                return valorChave;
            }
        }
        public static string enderecoProxy
        {
            get { return WebConfigurationManager.AppSettings["enderecoProxy"]; }
        }
        public static string userNameProxy
        {
            get { return WebConfigurationManager.AppSettings["userNameProxy"]; }
        }
        public static string passProxy
        {
            get { return WebConfigurationManager.AppSettings["passProxy"]; }
        }
        public static string wsURLConsulta
        {
            get { return WebConfigurationManager.AppSettings["wsURLConsulta"]; }
        }
        public static string userNameConsulta
        {
            get { return WebConfigurationManager.AppSettings["userNameConsulta"]; }
        }
        public static string passConsulta
        {
            get { return WebConfigurationManager.AppSettings["passConsulta"]; }
        }
        public static string wsURLEnvio
        {
            get { return WebConfigurationManager.AppSettings["wsURLEnvio"]; }
        }
        public static string userNameEnvio
        {
            get { return WebConfigurationManager.AppSettings["userNameEnvio"]; }
        }
        public static string passEnvio
        {
            get { return WebConfigurationManager.AppSettings["passEnvio"]; }
        }
    }
}
