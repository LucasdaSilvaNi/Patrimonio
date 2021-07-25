using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Common
{
    public class NivelAcessoEnum
    {
        public static int Orgao 
        { 
            get
            {
                return Convert.ToInt32(System.Web.Configuration.WebConfigurationManager.AppSettings["Orgao"].ToString());
            }
        }

        public static int UO
        {
            get
            {
                return Convert.ToInt32(System.Web.Configuration.WebConfigurationManager.AppSettings["UO"].ToString());
            }
        }

        public static int UGE
        {
            get
            {
                return Convert.ToInt32(System.Web.Configuration.WebConfigurationManager.AppSettings["UGE"].ToString());
            }
        }

        public static int UA
        {
            get
            {
                return Convert.ToInt32(System.Web.Configuration.WebConfigurationManager.AppSettings["UA"].ToString());
            }
        }

        public static int UA_GESTOR
        {
            get
            {
                return Convert.ToInt32(System.Web.Configuration.WebConfigurationManager.AppSettings["UA_GESTOR"].ToString());
            }
        }

        public static int DIVISAO
        {
            get
            {
                return Convert.ToInt32(System.Web.Configuration.WebConfigurationManager.AppSettings["DIVISAO"].ToString());
            }
        }

        public static int DIVISAO_UA
        {
            get
            {
                return Convert.ToInt32(System.Web.Configuration.WebConfigurationManager.AppSettings["DIVISAO_UA"].ToString());
            }
        }

        public static int GESTOR
        {
            get
            {
                return Convert.ToInt32(System.Web.Configuration.WebConfigurationManager.AppSettings["GESTOR"].ToString());
            }
        }

        public static int ALMOXARIFADO
        {
            get
            {
                return Convert.ToInt32  (System.Web.Configuration.WebConfigurationManager.AppSettings["ALMOXARIFADO"].ToString());
            }
        } 
    }
}
