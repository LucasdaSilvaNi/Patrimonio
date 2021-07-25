using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Sam.Common;


namespace Sam.Integracao.SIAF.Simulacra
{
    public static class simulaSIAFEM_SIAFISICO
    {
        public static string gerarSIAFNLEmLiq_SIAFNL_OpFlex(string msgEstimulo)
        {
            string strRetorno = null;
            string codigoMsgSIAF = null;
            string nomeMsgSIAF = null;

            string nlLiquidacaoFake = String.Format("{0}NL{1:D5}", DateTime.Now.Year, (new Random()).Next(0, 10000));
            StringBuilder sbXml = new StringBuilder();

            var gerarNLEmLiq = !msgEstimulo.Contains("SiafemDocNLEmLiq");
            var gerarNLPatrimonial = !gerarNLEmLiq && !msgEstimulo.Contains("SiafemDocNL");

            if (gerarNLPatrimonial)
            {
                nomeMsgSIAF = "SiafemDocNlPatrimonial";
                codigoMsgSIAF = "SIAFNlPatrimonial";
            }
            else if (gerarNLEmLiq)
            {
                nomeMsgSIAF = "SiafemDocNL";
                codigoMsgSIAF = "SIAFNL001";
            }
            else
            {
                nomeMsgSIAF = "SiafemDocNLEmLiq";
                codigoMsgSIAF = "SIAFNLEmLiq";
            }

            sbXml.AppendLine("<MSG>");
            sbXml.AppendLine("<BCMSG>");
            sbXml.AppendLine("<Doc_Estimulo>");
            sbXml.AppendLine(msgEstimulo);
            sbXml.AppendLine("</Doc_Estimulo>");
            sbXml.AppendLine("</BCMSG>");
            sbXml.AppendLine("<SISERRO>");
            sbXml.AppendLine("<Doc_Retorno>");
            sbXml.AppendLine("<SIAFDOC>");
            sbXml.AppendLine(String.Format("<cdMsg>{0}</cdMsg>", codigoMsgSIAF));
            sbXml.AppendLine(String.Format("<{0}>", nomeMsgSIAF));
            sbXml.AppendLine("<documento>");
            sbXml.AppendLine(String.Format("<NumeroNL>{0}</NumeroNL>", nlLiquidacaoFake));
            sbXml.AppendLine("<MsgErro></MsgErro>");
            sbXml.AppendLine("</documento>");
            sbXml.AppendLine(String.Format("</{0}>", nomeMsgSIAF));
            sbXml.AppendLine("</SIAFDOC>");
            sbXml.AppendLine("</Doc_Retorno>");
            sbXml.AppendLine("</SISERRO>");
            sbXml.AppendLine("</MSG>");

            strRetorno = sbXml.ToString();
            strRetorno = XmlUtil.IndentarXml(strRetorno);

            return strRetorno;
        }

        public static string gerarSIAFNLEmLiq_SIAFNL_OpFlex(string msgEstimulo, bool simulaDevolucaoErroSIAF = false, string msgErroSimulado = null)
        {
            string strRetorno = null;
            string codigoMsgSIAF = null;
            string nomeMsgSIAF = null;
            string nomeTagErroMsgSIAF = null;

            string nlSIAF_Falsa = null;
            StringBuilder sbXml = new StringBuilder();

            var gerarNLEmLiq = msgEstimulo.Contains("SiafemDocNLEmLiq");
            var gerarNLPatrimonial = !gerarNLEmLiq && !msgEstimulo.Contains("SiafemDocNL");

            if (gerarNLPatrimonial)
            {
                nomeMsgSIAF = "SiafemDocNlPatrimonial";
                codigoMsgSIAF = "SIAFNlPatrimonial";
            }
            else if (gerarNLEmLiq)
            {
                nomeMsgSIAF = "SiafemDocNL";
                codigoMsgSIAF = "SIAFNL001";
            }
            else
            {
                nomeMsgSIAF = "SiafemDocNLEmLiq";
                codigoMsgSIAF = "SIAFNLEmLiq";
            }

            if (!simulaDevolucaoErroSIAF)
            {
                msgErroSimulado = null;
                nomeTagErroMsgSIAF = "MsgErro";
                nlSIAF_Falsa = String.Format("{0}NZ{1:D5}", DateTime.Now.Year, (new Random()).Next(0, 10000));
            }
            else
            {
                nomeTagErroMsgSIAF = "NOME_TAG_ERRO";
            }



            sbXml.AppendLine("<MSG>");
            sbXml.AppendLine("<BCMSG>");
            sbXml.AppendLine("<Doc_Estimulo>");
            sbXml.AppendLine(msgEstimulo);
            sbXml.AppendLine("</Doc_Estimulo>");
            sbXml.AppendLine("</BCMSG>");
            sbXml.AppendLine("<SISERRO>");
            sbXml.AppendLine("<Doc_Retorno>");
            sbXml.AppendLine("<SIAFDOC>");
            sbXml.AppendLine(String.Format("<cdMsg>{0}</cdMsg>", codigoMsgSIAF));
            sbXml.AppendLine(String.Format("<{0}>", nomeMsgSIAF));
            sbXml.AppendLine("<documento>");
            sbXml.AppendLine(String.Format("<NumeroNL>{0}</NumeroNL>", nlSIAF_Falsa));
            sbXml.AppendLine(String.Format("<{0}>{1}</{0}>", nomeTagErroMsgSIAF, msgErroSimulado));
            sbXml.AppendLine("</documento>");
            sbXml.AppendLine(String.Format("</{0}>", nomeMsgSIAF));
            sbXml.AppendLine("</SIAFDOC>");
            sbXml.AppendLine("</Doc_Retorno>");
            sbXml.AppendLine("</SISERRO>");
            sbXml.AppendLine("</MSG>");

            strRetorno = sbXml.ToString();
            strRetorno = XmlUtil.IndentarXml(strRetorno);

            return strRetorno;
        }
    }

}