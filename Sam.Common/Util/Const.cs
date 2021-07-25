using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Sam.Common.Util
{
    public class Const
    {
        static readonly string pathApplication = HttpContext.Current.Request.ApplicationPath == "/" ? "" : HttpContext.Current.Request.ApplicationPath;
        public static string ReportPath = pathApplication + "/Relatorios/";
        public static string ReportUrl = pathApplication + "/Relatorios/imprimirRelatorio.aspx";
        public static string ImageUrl = pathApplication + "/Relatorios/obterImagem.ashx";
        public static string ReportScript = "<script language='javascript'>" + "window.open('" + ReportUrl + "', 'CustomPopUp', " + "'width=800,height=600,toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=yes, resizable=yes, copyhistory=no, top=0, left=0')" + "</script>";
        // webservices
        // "https://siafemhom.intra.fazenda.sp.gov.br/siafisico/RecebeMSG.asmx";
        // "https://172.16.36.20/siafisico/RecebeMSG.asmx"
        // "https://www6.fazenda.sp.gov.br/SIAFISICO/RecebeMSG.asmx"

        public static bool isSamWebDebugged = (System.Diagnostics.Debugger.IsAttached);
     }
}
