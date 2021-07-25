using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Common
{
    public class Perfil
    {


        public static int ADMINISTRADOR_GESTOR
        {
            get
            {
                //return Convert.ToInt16(System.Web.Configuration.WebConfigurationManager.AppSettings["ADMINISTRADOR_GESTOR"].ToString());
                return (int)EnumPerfil.ADMINISTRADOR_GESTOR;
            }
        }

        public static int ADMINISTRADOR_ORGAO
        {
            get
            {
                //return Convert.ToInt16(System.Web.Configuration.WebConfigurationManager.AppSettings["ADMINISTRADOR_ORGAO"].ToString());
                return (int)EnumPerfil.ADMINISTRADOR_ORGAO;
            }
        }

        public static int ADMINISTRADOR_GERAL
        {
            get
            {
                //return Convert.ToInt16(System.Web.Configuration.WebConfigurationManager.AppSettings["ADMINISTRADOR_GERAL"].ToString());
                return (int)EnumPerfil.ADMINISTRADOR_GERAL;
            }
        }

        public static int REQUISITANTE
        {
            get
            {
                return (int)EnumPerfil.REQUISITANTE;
            }
        }

        public static int REQUISITANTE_GERAL
        {
            get
            {
                // return Convert.ToInt16(System.Web.Configuration.WebConfigurationManager.AppSettings["REQUISITANTE_GERAL"].ToString());
                return (int)EnumPerfil.REQUISITANTE_GERAL;
            }
        }

        public static int OPERADOR_ALMOXARIFADO
        {
            get
            {
                //return Convert.ToInt16(System.Web.Configuration.WebConfigurationManager.AppSettings["OPERADOR_ALMOXARIFADO"].ToString());
                return (int)EnumPerfil.OPERADOR_ALMOXARIFADO;
            }
        }

        public static int ADMINISTRADOR_PAT
        {
            get
            {
                return (int)EnumPerfil.ADMINISTRADOR_PAT;
            }
        }

        public static int NUCLEO_PATRIMONIO_PAT
        {
            get
            {
                //return Convert.ToInt16(System.Web.Configuration.WebConfigurationManager.AppSettings["NUCLEO_PATRIMONIO_PAT"].ToString());
                return (int)EnumPerfil.NUCLEO_PATRIMONIO_PAT;
            }
        }

        public static int ALMOXARIFADO_PAT
        {
            get
            {
                //return Convert.ToInt16(System.Web.Configuration.WebConfigurationManager.AppSettings["ALMOXARIFADO_PAT"].ToString());
                return (int)EnumPerfil.ALMOXARIFADO_PAT;
            }
        }

        public static int OPERADOR_UGE_PAT
        {
            get
            {
                //return Convert.ToInt16(System.Web.Configuration.WebConfigurationManager.AppSettings["OPERADOR_UGE_PAT"].ToString());
                return (int)EnumPerfil.OPERADOR_UGE_PAT;
            }
        }

        public static int CONSULTA_UGE_PAT
        {
            get
            {
                //return Convert.ToInt16(System.Web.Configuration.WebConfigurationManager.AppSettings["CONSULTA_UGE_PAT"].ToString());
                return (int)EnumPerfil.CONSULTA_UGE_PAT;
            }
        }

        public static int OPERADOR_UA_PAT
        {
            get
            {
                //return Convert.ToInt16(System.Web.Configuration.WebConfigurationManager.AppSettings["OPERADOR_UA_PAT"].ToString());
                return (int)EnumPerfil.OPERADOR_UA_PAT;
            }
        }

        public static int CONSULTA_UA_PAT
        {
            get
            {
                //return Convert.ToInt16(System.Web.Configuration.WebConfigurationManager.AppSettings["CONSULTA_UA_PAT"].ToString());
                return (int)EnumPerfil.CONSULTA_UA_PAT;
            }
        }

        public static int CONSULTA_GERAL_PAT
        {
            get
            {
                //return Convert.ToInt16(System.Web.Configuration.WebConfigurationManager.AppSettings["CONSULTA_GERAL_PAT"].ToString());
                return (int)EnumPerfil.CONSULTA_GERAL_PAT;
            }
        }

        public static int OPERADOR_UNICO_PAT
        {
            get
            {
                //return Convert.ToInt16(System.Web.Configuration.WebConfigurationManager.AppSettings["OPERADOR_UNICO_PAT"].ToString());
                return (int)EnumPerfil.OPERADOR_UNICO_PAT;
            }
        }

        public static int COMERCIAL_PRODESP
        {
            get
            {
                return EnumPerfil.COMERCIAL_PRODESP.GetHashCode();
            }
        }

    }
}
