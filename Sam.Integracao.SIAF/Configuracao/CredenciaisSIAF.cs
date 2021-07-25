using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Configuration;


namespace Sam.Integracao.SIAF.Core
{
    public static class CredenciaisSIAF
    {
        public static string userNameConsulta
        {
            get { return WebConfigurationManager.AppSettings["userNameConsulta"]; }
        }
        public static string passConsulta
        {
            get { return WebConfigurationManager.AppSettings["passConsulta"]; }
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
