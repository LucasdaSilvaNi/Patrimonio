using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Net;
using System.IO;
using Sam.Common.Util;


namespace Sam.Common
{
    public class WebUtil
    {
        private string getjQueryCode(string jsCodetoRun)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("$(document).ready(function() {");
            sb.AppendLine(jsCodetoRun);
            sb.AppendLine(" });");
            return sb.ToString();
        }

        public void runJScript(Page page, string jsCode)
        {   
            ScriptManager requestSM = ScriptManager.GetCurrent(page);
            if (requestSM != null && requestSM.IsInAsyncPostBack)
                ScriptManager.RegisterStartupScript(page, typeof(Page), Guid.NewGuid().ToString(), getjQueryCode(jsCode), true);
            else
                page.ClientScript.RegisterStartupScript(typeof(Page), Guid.NewGuid().ToString(), getjQueryCode(jsCode), true);
        }

        public string GetIPAddress()
        {
            string strHostName = System.Net.Dns.GetHostName();
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];

            return ipAddress.ToString();
        }

        public string GetIPAddress(bool? novoMetodo)
        {
            if (!(novoMetodo.IsNotNull() && true))
                return GetIPAddress();


            var usuarioComProxy = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            var usuarioNaoUtilizandoProxy = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            var remoteUser = HttpContext.Current.Request.ServerVariables["REMOTE_USER"];

            var ipUsuario = (usuarioComProxy ?? usuarioNaoUtilizandoProxy);
            var ipUsuario02 = System.Web.HttpContext.Current.Request.UserHostAddress;
            var teste = GetIPv4Address(ipUsuario);

            IPHostEntry ipHostInfo = Dns.GetHostEntry(ipUsuario);
            IPAddress ipAddress = ipHostInfo.AddressList[0];

            return ipAddress.ToString();
        }

        /// <summary>
        /// Retorna endereço IP no formato IPv4, para gravação no banco.
        /// </summary>
        /// <param name="sHostNameOrAddress">The host name or IP address to resolve.</param>
        /// <returns>Primeiro endereço IP da máquina (caso possua mais de uma placa de rede ativa), ou nulo.</returns>
        public static string GetIPv4Address(string hostnameOuEnderecoIP)
        {
            try
            {
                // Get the list of IP addresses for the specified host
                IPAddress[] aIPHostAddresses = Dns.GetHostAddresses(hostnameOuEnderecoIP);

                // First try to find a real IPV4 address in the list
                foreach (IPAddress ipHost in aIPHostAddresses)
                    if (ipHost.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        return ipHost.ToString();

                // If that didn't work, try to lookup the IPV4 addresses for IPV6 addresses in the list
                foreach (IPAddress ipHost in aIPHostAddresses)
                    if (ipHost.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    {
                        IPHostEntry ihe = Dns.GetHostEntry(ipHost);
                        foreach (IPAddress ipEntry in ihe.AddressList)
                            if (ipEntry.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                                return ipEntry.ToString();
                    }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
            }
            return null;
        }

        private static string ReturnSubnetmask(String ipaddress)
        {
            uint firstOctet = ReturnFirtsOctet(ipaddress);

            if (firstOctet >= 0 && firstOctet <= 127)
                return "255.0.0.0";
            else if (firstOctet >= 128 && firstOctet <= 191)
                return "255.255.0.0";
            else if (firstOctet >= 192 && firstOctet <= 223)
                return "255.255.255.0";
            else return "0.0.0.0";
        }

        private static uint ReturnFirtsOctet(string ipAddress)
        {
            System.Net.IPAddress iPAddress = System.Net.IPAddress.Parse(ipAddress);
            byte[] byteIP = iPAddress.GetAddressBytes();
            uint ipInUint = (uint)byteIP[0];
            return ipInUint;
        }
    }
}
