using Sam.Common.Enums;
using SAM.Web.Common.Enum;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;



namespace Sam.Common
{
    public class ConfiguracaoEMail
    {
        //Configurações de Email
        private static SmtpClient servidorEMail = null;
        private static string enderecoServidorEMail = EnumVariaveisWebConfig.EnderecoServidorEMail;
        private static int portaEnvioEmail = EnumVariaveisWebConfig.PortaEnvioEmail;
        private static bool utilizaSSL = EnumVariaveisWebConfig.UtilizaSSL;

        private static string loginEMailSuporteSam = EnumVariaveisWebConfig.eMailParaEnvioSuporteSam;
        private static string senhaEMailSuporteSam = EnumVariaveisWebConfig.senhaEMailParaEnvioSuporteSam;

        public static SmtpClient ObterServidorEnvioEMail()
        {
            servidorEMail = new SmtpClient();
            servidorEMail.Host = enderecoServidorEMail;
            servidorEMail.Port = portaEnvioEmail;
            servidorEMail.UseDefaultCredentials = true;
            servidorEMail.Credentials = ObterCredenciais();
            servidorEMail.EnableSsl = utilizaSSL;
            servidorEMail.DeliveryMethod = SmtpDeliveryMethod.Network;

            return servidorEMail;
        }

        private static ICredentialsByHost ObterCredenciais()
        {
            return new NetworkCredential(loginEMailSuporteSam, senhaEMailSuporteSam);
        }
    }
}
