using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace Sam.Common.Util
{
    public static class FakeReturnXml
    {
        public static string SiafemDocDetaContaGen_UGE_441101_GESTAO_44047_MES_2012DEZ()
        {
            string lStrRetorno = string.Empty;
            StringBuilder lSbXml = new StringBuilder();

            lSbXml.AppendLine("<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetaContaGen</cdMsg><SiafemDocDetaContaGen><documento><CodigoUG>441101</CodigoUG><Gestao>44047</Gestao><Mes>Dez</Mes><ContaContabil>192410101</ContaContabil><ContaCorrente></ContaCorrente><Opcao>1</Opcao></documento></SiafemDocDetaContaGen></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC>");
            lSbXml.AppendLine("<cdMsg>SIAFDetaconta</cdMsg>");
            lSbXml.AppendLine("	<SiafemDetaconta>");
            lSbXml.AppendLine("	<documento>");
            lSbXml.AppendLine("	    <StatusOperacao>true</StatusOperacao>");
            lSbXml.AppendLine("	    <MsgErro></MsgErro>");
            lSbXml.AppendLine("	    <UG>441101</UG>");
            lSbXml.AppendLine("	    <Mes>Dez</Mes>");
            lSbXml.AppendLine("	    <Valor>563.901,52D</Valor>");
            lSbXml.AppendLine("	    <MovimentoaDebito></MovimentoaDebito>");
            lSbXml.AppendLine("	    <MovimentoaCredito></MovimentoaCredito>");
            lSbXml.AppendLine("	    <SaldoateoMes></SaldoateoMes>");
            lSbXml.AppendLine("     <Repete>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>00001714000114 2012NE00427 33903041 004</ContaCorrente>            <ValorConta>1.500,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>00001714000114 2012NE00428 33903041 004</ContaCorrente>            <ValorConta>1.520,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>00001714000114 2012NE00431 33903041 004</ContaCorrente>            <ValorConta>1.000,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>00007804000112 2012NE00104 33903041 004</ContaCorrente>            <ValorConta>0,01</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>00121325000122 2012NE00004 33903041 001</ContaCorrente>            <ValorConta>1.000,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>00121325000122 2012NE00096 33903041 004</ContaCorrente>            <ValorConta>98,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>00169311000189 2012NE00030 33903041 004</ContaCorrente>            <ValorConta>100,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>00169311000189 2012NE00113 33903041 004</ContaCorrente>            <ValorConta>2.000,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>00169311000189 2012NE00114 33903041 004</ContaCorrente>            <ValorConta>250,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>00169311000189 2012NE00130 33903041 004</ContaCorrente>            <ValorConta>1.320,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>00169311000189 2012NE00277 33903041 004</ContaCorrente>            <ValorConta>2.000,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>00169311000189 2012NE00282 33903041 004</ContaCorrente>            <ValorConta>100,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>00169311000189 2012NE00283 33903041 004</ContaCorrente>            <ValorConta>1.000,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>00169311000189 2012NE00339 33903041 004</ContaCorrente>            <ValorConta>2.870,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>00270077000181 2012NE00136 33903041 004</ContaCorrente>            <ValorConta>240,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>00270077000181 2012NE00272 33903041 004</ContaCorrente>            <ValorConta>750,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>00270077000181 2012NE00278 33903041 004</ContaCorrente>            <ValorConta>100,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>00270077000181 2012NE00290 33903041 004</ContaCorrente>            <ValorConta>15,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>00270083000139 2012NE00279 33903041 004</ContaCorrente>            <ValorConta>100,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>00473491000198 2012NE00015 33903041 001</ContaCorrente>            <ValorConta>100,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>00473491000198 2012NE00031 33903041 004</ContaCorrente>            <ValorConta>100,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>00868548871    2012NE00051 33903041 004</ContaCorrente>            <ValorConta>500,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>00868548871    2012NE00059 33903980 004</ContaCorrente>            <ValorConta>10,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>00868548871    2012NE00062 33903041 004</ContaCorrente>            <ValorConta>10,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>00868548871    2012NE00063 33903041 004</ContaCorrente>            <ValorConta>10,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>00868548871    2012NE00064 33903041 004</ContaCorrente>            <ValorConta>5,99</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>00868548871    2012NE00065 33903041 004</ContaCorrente>            <ValorConta>10,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>00868548871    2012NE00068 33903980 004</ContaCorrente>            <ValorConta>20,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("		  <Documento>         <ContaCorrente>00868548871    2012NE00070 33903041 001</ContaCorrente>            <ValorConta>9,99</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("		  <Documento>         <ContaCorrente>00868548871    2012NE00072 33903041 001</ContaCorrente>            <ValorConta>7,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("		  <Documento>         <ContaCorrente>00868548871    2012NE00073 33903041 001</ContaCorrente>            <ValorConta>99,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("		  <Documento>         <ContaCorrente>00868548871    2012NE00202 33903041 004</ContaCorrente>            <ValorConta>10,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("		  <Documento>         <ContaCorrente>00868548871    2012NE00240 33903041 004</ContaCorrente>            <ValorConta>9.997,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("		  <Documento>         <ContaCorrente>00868548871    2012NE00247 33903041 001</ContaCorrente>            <ValorConta>500,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("		  <Documento>         <ContaCorrente>00868548871    2012NE00294 33903050 004</ContaCorrente>            <ValorConta>1.000,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("		  <Documento>         <ContaCorrente>00868548871    2012NE00331 33903050 004</ContaCorrente>            <ValorConta>60.000,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("		  <Documento>         <ContaCorrente>00868548871    2012NE00342 33903041 004</ContaCorrente>            <ValorConta>10,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("		  <Documento>         <ContaCorrente>00868548871    2012NE00381 33903041 004</ContaCorrente>            <ValorConta>5,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("		  <Documento>         <ContaCorrente>00868548871    2012NE00383 33903041 004</ContaCorrente>            <ValorConta>20,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("		  <Documento>         <ContaCorrente>00868548871    2012NE00396 33903041 004</ContaCorrente>            <ValorConta>10,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("		  <Documento>         <ContaCorrente>00868548871    2012NE00423 33903041 004</ContaCorrente>            <ValorConta>50,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("		  <Documento>         <ContaCorrente>00868548871    2012NE00434 33903041 004</ContaCorrente>            <ValorConta>10,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("		  <Documento>         <ContaCorrente>00868548871    2012NE00467 33903041 004</ContaCorrente>            <ValorConta>10,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("		  <Documento>         <ContaCorrente>00868548871    2012NE00468 33903041 004</ContaCorrente>            <ValorConta>10,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("		  <Documento>         <ContaCorrente>00868548871    2012NE00469 33903041 004</ContaCorrente>            <ValorConta>10,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("		  <Documento>         <ContaCorrente>01211015000161 2012NE00325 33903041 004</ContaCorrente>            <ValorConta>729,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("		  <Documento>         <ContaCorrente>01211015000161 2012NE00337 33903041 004</ContaCorrente>            <ValorConta>1.232,70</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("		  <Documento>         <ContaCorrente>61695227000193 2012NE00133 33903041 004</ContaCorrente>            <ValorConta>81,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("		  <Documento>         <ContaCorrente>61695227000193 2012NE00270 33903041 004</ContaCorrente>            <ValorConta>707,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("		  <Documento>         <ContaCorrente>61695227000193 2012NE00271 33903041 004</ContaCorrente>            <ValorConta>1.801,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("		  <Documento>         <ContaCorrente>61794921000168 2012NE00067 33903041 004</ContaCorrente>            <ValorConta>900,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("		  <Documento>         <ContaCorrente>61794921000168 2012NE00122 33903041 004</ContaCorrente>            <ValorConta>1.779,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("		  <Documento>         <ContaCorrente>61794921000168 2012NE00124 33903041 004</ContaCorrente>            <ValorConta>1.500,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("		  <Documento>         <ContaCorrente>61794921000168 2012NE00275 33903041 004</ContaCorrente>            <ValorConta>3.340,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("		  <Documento>         <ContaCorrente>61794921000168 2012NE00276 33903041 004</ContaCorrente>            <ValorConta>700,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("		  <Documento>         <ContaCorrente>61794921000168 2012NE00281 33903041 004</ContaCorrente>            <ValorConta>90,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>61794921000168 2012NE00357 33903041 004</ContaCorrente>            <ValorConta>2.350,50</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>61794921000168 2012NE00448 33903041 004</ContaCorrente>            <ValorConta>135,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00032 33903041 004</ContaCorrente>            <ValorConta>100,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00034 33903041 004</ContaCorrente>            <ValorConta>100,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00035 33903041 004</ContaCorrente>            <ValorConta>100,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00036 33903041 004</ContaCorrente>            <ValorConta>100,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00037 33903041 004</ContaCorrente>            <ValorConta>100,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00040 33903041 004</ContaCorrente>            <ValorConta>100,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00041 33903041 004</ContaCorrente>            <ValorConta>100,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00044 33903041 004</ContaCorrente>            <ValorConta>99,98</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00084 33903980 004</ContaCorrente>            <ValorConta>1.000,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00085 33903980 004</ContaCorrente>            <ValorConta>500,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00086 33903041 004</ContaCorrente>            <ValorConta>46,76</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00087 33903041 004</ContaCorrente>            <ValorConta>99,90</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00097 33903041 004</ContaCorrente>            <ValorConta>1.000,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00098 33903041 001</ContaCorrente>            <ValorConta>1.000,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00112 33903041 004</ContaCorrente>            <ValorConta>2.000,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00115 33903041 004</ContaCorrente>            <ValorConta>5.000,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00116 33903041 004</ContaCorrente>            <ValorConta>5.000,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00118 33903972 004</ContaCorrente>            <ValorConta>500,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00119 33903041 004</ContaCorrente>            <ValorConta>1.000,88</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00123 33903041 004</ContaCorrente>            <ValorConta>1.688,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00128 33903041 004</ContaCorrente>            <ValorConta>600,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00137 33903041 004</ContaCorrente>            <ValorConta>700,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00138 33903041 004</ContaCorrente>            <ValorConta>1.400,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00139 33903041 004</ContaCorrente>            <ValorConta>1.500,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00140 33903041 004</ContaCorrente>            <ValorConta>1.600,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00141 33903041 004</ContaCorrente>            <ValorConta>5.000,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00142 33903041 004</ContaCorrente>            <ValorConta>1.100,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00143 33903972 004</ContaCorrente>            <ValorConta>5.000,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00144 33903041 004</ContaCorrente>            <ValorConta>6.216,50</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00145 33903041 004</ContaCorrente>            <ValorConta>5.253,20</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00146 33903041 004</ContaCorrente>            <ValorConta>5.506,60</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00147 33903041 004</ContaCorrente>            <ValorConta>4.896,70</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00148 33903041 004</ContaCorrente>            <ValorConta>3.996,70</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00149 33903041 004</ContaCorrente>            <ValorConta>5.006,70</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00150 33903041 004</ContaCorrente>            <ValorConta>5.000,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00151 33903041 004</ContaCorrente>            <ValorConta>4.000,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00153 33903972 004</ContaCorrente>            <ValorConta>2.926,70</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00154 33903972 004</ContaCorrente>            <ValorConta>4.420,05</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00155 33903041 004</ContaCorrente>            <ValorConta>1.500,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00156 33903972 004</ContaCorrente>            <ValorConta>1.500,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00157 33903041 004</ContaCorrente>            <ValorConta>4.000,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00158 33903041 004</ContaCorrente>            <ValorConta>970,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00159 33903041 004</ContaCorrente>            <ValorConta>2.000,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00160 33903972 004</ContaCorrente>            <ValorConta>1.010,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00161 33903972 004</ContaCorrente>            <ValorConta>1.267,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00162 33903041 004</ContaCorrente>            <ValorConta>1.249,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00163 33903041 004</ContaCorrente>            <ValorConta>100,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00165 33903041 004</ContaCorrente>            <ValorConta>1.435,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00166 33903041 004</ContaCorrente>            <ValorConta>1.823,10</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00167 33903041 004</ContaCorrente>            <ValorConta>4.290,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00168 33903041 004</ContaCorrente>            <ValorConta>1.075,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00169 33903041 004</ContaCorrente>            <ValorConta>1.234,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00170 33903041 004</ContaCorrente>            <ValorConta>3.266,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00171 33903041 004</ContaCorrente>            <ValorConta>4.377,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00172 33903041 004</ContaCorrente>            <ValorConta>4.483,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00173 33903041 004</ContaCorrente>            <ValorConta>3.829,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00174 33903041 004</ContaCorrente>            <ValorConta>4.690,50</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00175 33903972 004</ContaCorrente>            <ValorConta>1.000,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00176 33903041 004</ContaCorrente>            <ValorConta>1.037,50</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00177 33903041 004</ContaCorrente>            <ValorConta>3.465,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00178 33903041 004</ContaCorrente>            <ValorConta>1.011,88</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00179 33903041 004</ContaCorrente>            <ValorConta>1.850,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00180 33903041 004</ContaCorrente>            <ValorConta>2.325,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00181 33903041 004</ContaCorrente>            <ValorConta>3.876,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00182 33903041 004</ContaCorrente>            <ValorConta>3.052,50</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00185 33903041 004</ContaCorrente>            <ValorConta>575,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00186 33903041 004</ContaCorrente>            <ValorConta>160,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00187 33903041 004</ContaCorrente>            <ValorConta>3.875,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00188 33903041 004</ContaCorrente>            <ValorConta>2.250,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00189 33903041 004</ContaCorrente>            <ValorConta>2.685,50</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00190 33903041 004</ContaCorrente>            <ValorConta>400,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00191 33903041 004</ContaCorrente>            <ValorConta>1.600,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00192 33903041 004</ContaCorrente>            <ValorConta>1.200,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00193 33903041 004</ContaCorrente>            <ValorConta>139,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00194 33903041 004</ContaCorrente>            <ValorConta>1.496,91</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00195 33903041 004</ContaCorrente>            <ValorConta>1.100,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00196 33903041 004</ContaCorrente>            <ValorConta>1.546,11</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00197 33903041 004</ContaCorrente>            <ValorConta>550,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00198 33903041 004</ContaCorrente>            <ValorConta>1.317,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00199 33903041 004</ContaCorrente>            <ValorConta>1.800,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00200 33903972 004</ContaCorrente>            <ValorConta>1.250,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00201 33903041 004</ContaCorrente>            <ValorConta>821,43</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00203 33903041 004</ContaCorrente>            <ValorConta>2.562,75</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00204 33903041 004</ContaCorrente>            <ValorConta>3.798,40</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00205 33903041 004</ContaCorrente>            <ValorConta>3.408,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00206 33903041 004</ContaCorrente>            <ValorConta>2.481,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00208 33903041 004</ContaCorrente>            <ValorConta>2.462,50</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00209 33903041 004</ContaCorrente>            <ValorConta>1.304,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00210 33903041 004</ContaCorrente>            <ValorConta>18.210,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00211 33903041 004</ContaCorrente>            <ValorConta>2.700,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00212 33903041 004</ContaCorrente>            <ValorConta>1.443,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00213 33903041 004</ContaCorrente>            <ValorConta>2.247,50</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00216 33903041 004</ContaCorrente>            <ValorConta>2.788,50</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00217 33903041 004</ContaCorrente>            <ValorConta>3.882,40</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00218 33903041 004</ContaCorrente>            <ValorConta>5.254,20</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00219 33903041 004</ContaCorrente>            <ValorConta>6.327,60</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00221 33903041 004</ContaCorrente>            <ValorConta>1.356,30</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00222 33903041 004</ContaCorrente>            <ValorConta>300,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00223 33903041 004</ContaCorrente>            <ValorConta>10,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00224 33903041 004</ContaCorrente>            <ValorConta>10,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00225 33903041 004</ContaCorrente>            <ValorConta>1,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00226 33903041 004</ContaCorrente>            <ValorConta>10,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00227 33903041 004</ContaCorrente>            <ValorConta>10,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00229 33903041 004</ContaCorrente>            <ValorConta>10,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00230 33903041 004</ContaCorrente>            <ValorConta>10,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00236 33903041 004</ContaCorrente>            <ValorConta>1.200,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00244 33903041 004</ContaCorrente>            <ValorConta>1.718,85</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00249 33903041 004</ContaCorrente>            <ValorConta>1.000,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00250 33903041 004</ContaCorrente>            <ValorConta>1.000,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00251 33903041 004</ContaCorrente>            <ValorConta>1.000,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00252 33903041 004</ContaCorrente>            <ValorConta>1.000,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00253 33903041 004</ContaCorrente>            <ValorConta>1.000,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00262 33903041 004</ContaCorrente>            <ValorConta>7.332,50</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00263 33903041 004</ContaCorrente>            <ValorConta>512,50</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00264 33903041 004</ContaCorrente>            <ValorConta>500,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00265 33903041 004</ContaCorrente>            <ValorConta>3.150,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00266 33903041 004</ContaCorrente>            <ValorConta>1.025,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00267 33903972 004</ContaCorrente>            <ValorConta>1.500,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00268 33903041 004</ContaCorrente>            <ValorConta>1.500,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00269 33903041 004</ContaCorrente>            <ValorConta>1.500,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00284 33903041 004</ContaCorrente>            <ValorConta>2.167,50</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00285 33903041 004</ContaCorrente>            <ValorConta>2.016,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00291 33903041 004</ContaCorrente>            <ValorConta>4.896,70</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00292 33903041 004</ContaCorrente>            <ValorConta>1.100,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00296 33903041 004</ContaCorrente>            <ValorConta>1.100,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00297 33903041 004</ContaCorrente>            <ValorConta>1.100,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00298 33903972 004</ContaCorrente>            <ValorConta>1.100,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00299 33903041 004</ContaCorrente>            <ValorConta>1.100,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00300 33903041 004</ContaCorrente>            <ValorConta>1.100,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00301 33903041 004</ContaCorrente>            <ValorConta>11,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00307 33903041 004</ContaCorrente>            <ValorConta>3.693,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00308 33903041 004</ContaCorrente>            <ValorConta>6.425,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00309 33903041 004</ContaCorrente>            <ValorConta>1.750,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00311 33903041 004</ContaCorrente>            <ValorConta>900,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00312 33903041 004</ContaCorrente>            <ValorConta>1.500,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00313 33903041 004</ContaCorrente>            <ValorConta>775,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00315 33903972 004</ContaCorrente>            <ValorConta>1.500,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00319 33903041 004</ContaCorrente>            <ValorConta>2.422,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00320 33903972 004</ContaCorrente>            <ValorConta>2.000,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00324 33903041 004</ContaCorrente>            <ValorConta>915,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00327 33903041 004</ContaCorrente>            <ValorConta>671,40</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00328 33903972 004</ContaCorrente>            <ValorConta>2.400,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00329 33903972 004</ContaCorrente>            <ValorConta>2.000,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00330 33903972 004</ContaCorrente>            <ValorConta>1.100,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00333 33903041 004</ContaCorrente>            <ValorConta>1.000,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00334 33903041 004</ContaCorrente>            <ValorConta>1.100,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00338 33903041 004</ContaCorrente>            <ValorConta>2.600,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00340 33903972 004</ContaCorrente>            <ValorConta>3.450,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00341 33903041 004</ContaCorrente>            <ValorConta>1.742,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00343 33903972 004</ContaCorrente>            <ValorConta>2.470,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00344 33903041 004</ContaCorrente>            <ValorConta>2.383,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00345 33903041 004</ContaCorrente>            <ValorConta>2.065,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00346 33903972 004</ContaCorrente>            <ValorConta>2.500,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00347 33903041 004</ContaCorrente>            <ValorConta>1.780,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00348 33903041 004</ContaCorrente>            <ValorConta>3.150,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00350 33903041 004</ContaCorrente>            <ValorConta>1.990,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00351 33903972 004</ContaCorrente>            <ValorConta>3.400,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00352 33903041 004</ContaCorrente>            <ValorConta>110,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00353 33903041 004</ContaCorrente>            <ValorConta>1.500,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00354 33903972 004</ContaCorrente>            <ValorConta>2.100,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00355 33903041 004</ContaCorrente>            <ValorConta>10.000,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00356 33903041 004</ContaCorrente>            <ValorConta>10,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00358 33903041 004</ContaCorrente>            <ValorConta>2.333,50</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00359 33903972 004</ContaCorrente>            <ValorConta>2.900,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00360 33903972 004</ContaCorrente>            <ValorConta>2.300,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00361 33903972 004</ContaCorrente>            <ValorConta>2.540,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00363 33903972 004</ContaCorrente>            <ValorConta>2.540,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00364 33903972 004</ContaCorrente>            <ValorConta>2.265,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00365 33903972 004</ContaCorrente>            <ValorConta>2.000,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00367 33903041 004</ContaCorrente>            <ValorConta>1.650,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00371 33903041 004</ContaCorrente>            <ValorConta>1.500,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00376 33903972 004</ContaCorrente>            <ValorConta>2.600,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00385 33903041 004</ContaCorrente>            <ValorConta>3.000,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00386 33903041 004</ContaCorrente>            <ValorConta>2.550,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00387 33903041 004</ContaCorrente>            <ValorConta>769,93</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00389 33903041 004</ContaCorrente>            <ValorConta>1.050,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00390 33903041 004</ContaCorrente>            <ValorConta>1.440,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00391 33903972 004</ContaCorrente>            <ValorConta>1.540,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00392 33903041 004</ContaCorrente>            <ValorConta>1.610,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00393 33903041 004</ContaCorrente>            <ValorConta>2.550,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00394 33903041 004</ContaCorrente>            <ValorConta>2.550,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00395 33903041 004</ContaCorrente>            <ValorConta>1.263,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00397 33903041 004</ContaCorrente>            <ValorConta>2.550,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00398 33903041 004</ContaCorrente>            <ValorConta>1.100,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00399 33903041 004</ContaCorrente>            <ValorConta>1.100,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00400 33903041 004</ContaCorrente>            <ValorConta>2.850,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00401 33903041 004</ContaCorrente>            <ValorConta>2.100,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00403 33903041 004</ContaCorrente>            <ValorConta>2.960,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00404 33903041 004</ContaCorrente>            <ValorConta>2.000,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00405 33903041 004</ContaCorrente>            <ValorConta>900,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00407 33903041 004</ContaCorrente>            <ValorConta>1.425,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00408 33903041 004</ContaCorrente>            <ValorConta>1.350,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00409 33903041 004</ContaCorrente>            <ValorConta>956,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00413 33903041 004</ContaCorrente>            <ValorConta>1.100,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00414 33903972 004</ContaCorrente>            <ValorConta>1.200,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00415 33903041 004</ContaCorrente>            <ValorConta>6.421,50</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00419 33903041 004</ContaCorrente>            <ValorConta>2.300,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00426 33903041 004</ContaCorrente>            <ValorConta>2.475,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00429 33903041 004</ContaCorrente>            <ValorConta>2.475,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00430 33903041 004</ContaCorrente>            <ValorConta>3.800,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00433 33903972 004</ContaCorrente>            <ValorConta>11.000,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00437 33903041 004</ContaCorrente>            <ValorConta>1.000,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00438 33903972 004</ContaCorrente>            <ValorConta>1.196,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00439 33903041 004</ContaCorrente>            <ValorConta>1.500,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00440 33903041 004</ContaCorrente>            <ValorConta>1.500,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00441 33903041 004</ContaCorrente>            <ValorConta>1.500,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00442 33903972 004</ContaCorrente>            <ValorConta>1.500,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00445 33903972 004</ContaCorrente>            <ValorConta>9.100,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00446 33903041 004</ContaCorrente>            <ValorConta>1.540,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00447 33903041 004</ContaCorrente>            <ValorConta>1.275,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00450 33903041 004</ContaCorrente>            <ValorConta>350,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00451 33903041 004</ContaCorrente>            <ValorConta>210,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00453 33903041 004</ContaCorrente>            <ValorConta>1.000,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00454 33903041 004</ContaCorrente>            <ValorConta>1.000,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00455 33903041 004</ContaCorrente>            <ValorConta>1.275,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00456 33903972 004</ContaCorrente>            <ValorConta>1.133,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00457 33903041 004</ContaCorrente>            <ValorConta>1.000,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00458 33903972 004</ContaCorrente>            <ValorConta>1.000,00</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("       <Documento>         <ContaCorrente>66097668000107 2012NE00466 33903041 004</ContaCorrente>            <ValorConta>1.116,70</ValorConta>            <DebitoCredito>D</DebitoCredito>          </Documento>");
            lSbXml.AppendLine("</Repete>");
            lSbXml.AppendLine(" </documento>");
            lSbXml.AppendLine("</SiafemDetaconta>");
            lSbXml.AppendLine("</SIAFDOC>");
            lSbXml.AppendLine("</Doc_Retorno></SISERRO></MSG>");
            lStrRetorno = lSbXml.ToString();
            return lStrRetorno;
        }

        public static string SiafemDocNLConsumo_UGE_200162_UA_CONSUMIDORA_022522_GESTAO_00001_DATA_EMISSAO_07_MAI_2008()
        {
            StringBuilder lSbRetorno = new StringBuilder();

            lSbRetorno.AppendLine("<MSG>");
            lSbRetorno.AppendLine("	<BCMSG>");
            lSbRetorno.AppendLine("		<Doc_Estimulo>");
            lSbRetorno.AppendLine("			<SIAFDOC>");
            lSbRetorno.AppendLine("			<cdMsg>SIAFNLCONSUMO</cdMsg>");
            lSbRetorno.AppendLine("			<SiafemDocNLConsumo>");
            lSbRetorno.AppendLine("				<documento>");
            lSbRetorno.AppendLine("					<ID>4822</ID>");
            lSbRetorno.AppendLine("					<DataEmissao>07MAI2008</DataEmissao>");
            lSbRetorno.AppendLine("					<UnidadeGestora>200162</UnidadeGestora>");
            lSbRetorno.AppendLine("					<Gestao>00001</Gestao>");
            lSbRetorno.AppendLine("					<Lancamento>N</Lancamento>");
            lSbRetorno.AppendLine("					<UGConsumidora>200142</UGConsumidora>");
            lSbRetorno.AppendLine("					<UAConsumidora>022522</UAConsumidora>");
            lSbRetorno.AppendLine("					<PTRes>200602</PTRes>");
            lSbRetorno.AppendLine("					<ClassificacaoDespesa>333903010</ClassificacaoDespesa>");
            lSbRetorno.AppendLine("					<Valor>00000000000010072</Valor>");
            lSbRetorno.AppendLine("					<Obs01>Consumo almoxarifado (Sistema SCEw)</Obs01>");
            lSbRetorno.AppendLine("					<Obs02></Obs02>");
            lSbRetorno.AppendLine("					<Obs03></Obs03>");
            lSbRetorno.AppendLine("				</documento>");
            lSbRetorno.AppendLine("			</SiafemDocNLConsumo>");
            lSbRetorno.AppendLine("			</SIAFDOC>");
            lSbRetorno.AppendLine("		</Doc_Estimulo>");
            lSbRetorno.AppendLine("	</BCMSG>");
            lSbRetorno.AppendLine("	<SISERRO>");
            lSbRetorno.AppendLine("	<Doc_Retorno>");
            lSbRetorno.AppendLine("		<SIAFDOC>");
            lSbRetorno.AppendLine("		<cdMsg>SIAFNLCONSUMO</cdMsg>");
            lSbRetorno.AppendLine("		<SiafemDocNLConsumo>");
            lSbRetorno.AppendLine("			<documento>");
            lSbRetorno.AppendLine("				<NumeroNL>2008NL00373</NumeroNL>");
            lSbRetorno.AppendLine("				<MsgErro></MsgErro>");
            lSbRetorno.AppendLine("			</documento>");
            lSbRetorno.AppendLine("			</SiafemDocNLConsumo>");
            lSbRetorno.AppendLine("		</SIAFDOC>");
            lSbRetorno.AppendLine("		</Doc_Retorno>");
            lSbRetorno.AppendLine("	</SISERRO>");
            lSbRetorno.AppendLine("</MSG>");

            return lSbRetorno.ToString();
        }
        public static string SiafemDocNLConsumo_UGE_171302_UA_CONSUMIDORA_028243_GESTAO_17048_DATA_EMISSAO_30_ABR_2015()
        {
            StringBuilder sbRetorno = new StringBuilder();

            sbRetorno.AppendLine(@"<MSG>");
            sbRetorno.AppendLine(@"  <BCMSG>");
            sbRetorno.AppendLine(@"    <Doc_Estimulo>");
            sbRetorno.AppendLine(@"      <SIAFDOC>");
            sbRetorno.AppendLine(@"        <cdMsg>SIAFNLCONSUMO</cdMsg>");
            sbRetorno.AppendLine(@"        <SiafemDocNLConsumo>");
            sbRetorno.AppendLine(@"          <documento>");
            sbRetorno.AppendLine(@"            <ID>0001</ID>");
            sbRetorno.AppendLine(@"            <DataEmissao>30ABR2015</DataEmissao>");
            sbRetorno.AppendLine(@"            <UnidadeGestora>171302</UnidadeGestora>");
            sbRetorno.AppendLine(@"            <Gestao>17048</Gestao>");
            sbRetorno.AppendLine(@"            <Lancamento>N</Lancamento>");
            sbRetorno.AppendLine(@"            <UGConsumidora>171302</UGConsumidora>");
            sbRetorno.AppendLine(@"            <UAConsumidora>028243</UAConsumidora>");
            sbRetorno.AppendLine(@"            <PTRes>174811</PTRes>");
            sbRetorno.AppendLine(@"            <ClassificacaoDespesa>33903031</ClassificacaoDespesa>");
            sbRetorno.AppendLine(@"            <Valor>00000000000074629</Valor>");
            sbRetorno.AppendLine(@"            <Obs01>Consumo almoxarifado (Fechamento Sistema SAM)</Obs01>");
            sbRetorno.AppendLine(@"            <Obs02 />");
            sbRetorno.AppendLine(@"            <Obs03 />");
            sbRetorno.AppendLine(@"          </documento>");
            sbRetorno.AppendLine(@"        </SiafemDocNLConsumo>");
            sbRetorno.AppendLine(@"      </SIAFDOC>");
            sbRetorno.AppendLine(@"    </Doc_Estimulo>");
            sbRetorno.AppendLine(@"  </BCMSG>");
            sbRetorno.AppendLine(@"  <SISERRO>");
            sbRetorno.AppendLine(@"    <Doc_Retorno>");
            sbRetorno.AppendLine(@"      <SIAFDOC>");
            sbRetorno.AppendLine(@"        <cdMsg>SIAFNLCONSUMO</cdMsg>");
            sbRetorno.AppendLine(@"        <SiafemDocNLConsumo>");
            sbRetorno.AppendLine(@"          <documento>");
            sbRetorno.AppendLine(@"            <NumeroNL></NumeroNL>");
            sbRetorno.AppendLine(@"            <MsgErro>(0067) MES INVALIDO</MsgErro>");
            sbRetorno.AppendLine(@"          </documento>");
            sbRetorno.AppendLine(@"        </SiafemDocNLConsumo>");
            sbRetorno.AppendLine(@"      </SIAFDOC>");
            sbRetorno.AppendLine(@"    </Doc_Retorno>");
            sbRetorno.AppendLine(@"  </SISERRO>");
            sbRetorno.AppendLine(@"</MSG>");

            return sbRetorno.ToString();
        }

        public static string SiafemDocNLConsumo_OpFlex(string msgEstimulo)
        {
            Random rdn = new Random();
            //string lStrSimulacra = Sam.Common.XmlUtil.lerXml(pStrMsgEstimulo, "/SIAFDOC/SiafemDocNLConsumo/documento/UGConsumidora").InnerText;
            string lStrSimulacra = rdn.Next(10000, 99999).ToString();
            int lIntControle = (String.IsNullOrEmpty(lStrSimulacra)) ? 0 : Int32.Parse(lStrSimulacra);
            string lStrNl = "                 <NumeroNL>" + DateTime.Today.ToString("yyyy") + "NL" + lStrSimulacra + "</NumeroNL>";

            StringBuilder lSbRetorno = new StringBuilder();

            lSbRetorno.AppendLine("<MSG>");
            lSbRetorno.AppendLine(" <BCMSG>");
            lSbRetorno.AppendLine("     <Doc_Estimulo>");
            lSbRetorno.AppendLine(msgEstimulo);
            lSbRetorno.AppendLine("     </Doc_Estimulo>");
            lSbRetorno.AppendLine(" </BCMSG>");
            lSbRetorno.AppendLine(" <SISERRO>");
            lSbRetorno.AppendLine("     <Doc_Retorno>");
            lSbRetorno.AppendLine("         <SIAFDOC>");
            lSbRetorno.AppendLine("             <cdMsg>SIAFNLCONSUMO</cdMsg>");
            lSbRetorno.AppendLine("             <SiafemDocNLConsumo>");
            lSbRetorno.AppendLine("             <documento>");
            lSbRetorno.AppendLine(lStrNl);
            lSbRetorno.AppendLine("             <MsgErro></MsgErro>");
            lSbRetorno.AppendLine("             </documento>");
            lSbRetorno.AppendLine("             </SiafemDocNLConsumo>");
            lSbRetorno.AppendLine("         </SIAFDOC>");
            lSbRetorno.AppendLine("     </Doc_Retorno>");
            lSbRetorno.AppendLine("	</SISERRO>");
            lSbRetorno.AppendLine("</MSG>");

            return lSbRetorno.ToString();
        }

        public static string SiafemDocConsultaEmpenhos_UGE_441101_GESTAO_44047_NUMERONE_2012NE00015()
        {
            string lStrRetorno = string.Empty;
            StringBuilder lSbXml = new StringBuilder();

            lSbXml.AppendLine("<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFConsultaEmpenhos</cdMsg><SiafemDocConsultaEmpenhos><documento><UnidadeGestora>441101</UnidadeGestora><Gestao>44047</Gestao><NumeroNe>2012NE00015</NumeroNe></documento></SiafemDocConsultaEmpenhos></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno>");
            lSbXml.AppendLine("<SIAFDOC>");
            lSbXml.AppendLine(" <cdMsg>SIAFConsultaEmpenhos</cdMsg>");
            lSbXml.AppendLine("    <SiafemDocConsultaEmpenhos>");
            lSbXml.AppendLine(" 	<StatusOperacao>true</StatusOperacao>");
            lSbXml.AppendLine("	    <documento>");
            lSbXml.AppendLine(" 		   <DataConsulta>12/12/2012</DataConsulta>");
            lSbXml.AppendLine(" 		   <HoraConsulta>10:45</HoraConsulta>");
            lSbXml.AppendLine(" 		   <Usuario>PUB. SIAFEM2012</Usuario>");
            lSbXml.AppendLine(" 		   <DataEmissao>02FEV2012</DataEmissao>");
            lSbXml.AppendLine(" 		   <Docto>* NE</Docto>");
            lSbXml.AppendLine(" 		   <NumeroNe>2012NE00015</NumeroNe>");
            lSbXml.AppendLine(" 		   <DataLancamento>02FEV2012</DataLancamento>");
            lSbXml.AppendLine(" 		   <UnidadeGestora>441101  - FUNDACAO DO DESENVOLVIMENTO ADMINISTRATIVO</UnidadeGestora>");
            lSbXml.AppendLine(" 		   <Gestao>44047   - FUNDACAO DO DESENVOLVIMENTO ADMINISTRATIVO</Gestao>");
            lSbXml.AppendLine(" 		   <CgcCpf>00473491000198  - JP - AUTO POSTO UBATUBA LTDA.</CgcCpf>");
            lSbXml.AppendLine(" 		   <GestaoCredor></GestaoCredor>");
            lSbXml.AppendLine(" 		   <Evento>400051  - EMPENHO DE DOTACAO RESERVADA</Evento>");
            lSbXml.AppendLine(" 		   <Ptres>444713</Ptres>");
            lSbXml.AppendLine(" 		   <Uo>44047</Uo>");
            lSbXml.AppendLine(" 		   <Pt>04122440454720000</Pt>");
            lSbXml.AppendLine(" 		   <Fonte>001001001</Fonte>");
            lSbXml.AppendLine(" 		   <Despesa>33903041</Despesa>");
            lSbXml.AppendLine(" 		   <Ugo>441101</Ugo>");
            lSbXml.AppendLine(" 		   <PlanoInterno>000.000.0100</PlanoInterno>");
            lSbXml.AppendLine(" 		   <Modalidade>3 - ESTIMATIVO</Modalidade>");
            lSbXml.AppendLine(" 		   <Licitacao>6 - INEXIGIVEL</Licitacao>");
            lSbXml.AppendLine(" 		   <ReferenciaLegal>LEI..</ReferenciaLegal>");
            lSbXml.AppendLine(" 		   <OrigemMaterial>1</OrigemMaterial>");
            lSbXml.AppendLine(" 		   <NumeroProcesso>1116/2012</NumeroProcesso>");
            lSbXml.AppendLine(" 		   <ServicoouMaterial>2</ServicoouMaterial>");
            lSbXml.AppendLine(" 		   <EmpenhoOriginal></EmpenhoOriginal>");
            lSbXml.AppendLine(" 		   <Oc></Oc>");
            lSbXml.AppendLine(" 		   <NumeroContrato>2012CT00017</NumeroContrato>");
            lSbXml.AppendLine(" 		   <IdentificadorObra></IdentificadorObra>");
            lSbXml.AppendLine(" 		   <Valor>100,00</Valor>");
            lSbXml.AppendLine(" 		   <Local>FUNDAP</Local>");
            lSbXml.AppendLine(" 		   <DataEntrega>02FEV2012</DataEntrega>");
            lSbXml.AppendLine(" 		   <TipoEmpenho>9 - DESPESA NORMAL</TipoEmpenho>");
            lSbXml.AppendLine(" 		   <Lancadopor></Lancadopor>");
            lSbXml.AppendLine(" 		   <DataLancamento>02FEV2012</DataLancamento>");
            lSbXml.AppendLine(" 		   <HoraLancamento>15:21</HoraLancamento>");
            lSbXml.AppendLine(" 		   <Mes01>02</Mes01>");
            lSbXml.AppendLine(" 		   <Mes02></Mes02>");
            lSbXml.AppendLine(" 		   <Mes03></Mes03>");
            lSbXml.AppendLine(" 		   <Mes04></Mes04>");
            lSbXml.AppendLine(" 		   <Mes05></Mes05>");
            lSbXml.AppendLine(" 		   <Mes06></Mes06>");
            lSbXml.AppendLine(" 		   <Mes07></Mes07>");
            lSbXml.AppendLine(" 		   <Mes08></Mes08>");
            lSbXml.AppendLine(" 		   <Mes09></Mes09>");
            lSbXml.AppendLine(" 		   <Mes10></Mes10>");
            lSbXml.AppendLine(" 		   <Mes11></Mes11>");
            lSbXml.AppendLine(" 		   <Mes12></Mes12>");
            lSbXml.AppendLine(" 		   <Mes13></Mes13>");
            lSbXml.AppendLine(" 		   <Valor01>100,00</Valor01>");
            lSbXml.AppendLine(" 		   <Valor02></Valor02>");
            lSbXml.AppendLine(" 		   <Valor03></Valor03>");
            lSbXml.AppendLine(" 		   <Valor04></Valor04>");
            lSbXml.AppendLine(" 		   <Valor05></Valor05>");
            lSbXml.AppendLine(" 		   <Valor06></Valor06>");
            lSbXml.AppendLine(" 		   <Valor07></Valor07>");
            lSbXml.AppendLine(" 		   <Valor08></Valor08>");
            lSbXml.AppendLine(" 		   <Valor09></Valor09>");
            lSbXml.AppendLine(" 		   <Valor10></Valor10>");
            lSbXml.AppendLine(" 		   <Valor11></Valor11>");
            lSbXml.AppendLine(" 		   <Valor12></Valor12>");
            lSbXml.AppendLine(" 		   <Valor13></Valor13>");
            lSbXml.AppendLine("	    <Repete>");
            lSbXml.AppendLine("		    <tabela>");
            lSbXml.AppendLine("		        <sequencia>1</sequencia>");
            lSbXml.AppendLine("		        <item>001</item>");
            lSbXml.AppendLine("		        <material>00000001-9</material>");
            lSbXml.AppendLine("		        <unidade>0001</unidade>");
            lSbXml.AppendLine("		        <qtdeitem>000000001,000</qtdeitem>");
            lSbXml.AppendLine("		        <valorunitario>100,00</valorunitario>");
            lSbXml.AppendLine("		        <precototal>100,00</precototal>");
            lSbXml.AppendLine("             <descricao>PAPEL SULFITE DE PAPELARIA, GRAMATURA 90G/M2, FORMATO CARTA ANVISA, MEDINDO (216X279)MM, ALVURA MINIMA 90%, CONFORME NORMA ISO, OPACIDADE MINIMA DE MINIMA DE 87%, UMIDADE ENTRE 3,5%(+/-1,0), CONFORME NORMA TAPPI 412, CORTE ROTATIVO, PH ALCALINO COR BRANCA, EMBALAGEM REVESTIDA EM BOPP INMETRO, PRODUTO COM CERTIFICACAO AMBIENTAL FSC OU CERFLOR</descricao>");
            lSbXml.AppendLine("             <descricao1></descricao1>");
            lSbXml.AppendLine("             <descricao2></descricao2>");
            lSbXml.AppendLine("             <descricao3></descricao3>");
            lSbXml.AppendLine("		    </tabela>");
            lSbXml.AppendLine("	    </Repete>");
            lSbXml.AppendLine("	    </documento>");
            lSbXml.AppendLine(" 	<MsgRetorno></MsgRetorno>");
            lSbXml.AppendLine("    </SiafemDocConsultaEmpenhos>");
            lSbXml.AppendLine("</SIAFDOC>");
            lSbXml.AppendLine("</Doc_Retorno></SISERRO></MSG>");

            lStrRetorno = lSbXml.ToString();
            return lStrRetorno;
        }
        #region Empenhos com descricao de item grande
        public static string SiafemDocConsultaEmpenhos_UGE_380236_GESTAO_00001_NUMERONE_2013NE00004()
        {
            string lStrRetorno = string.Empty;
            StringBuilder lSbXml = new StringBuilder();

            lSbXml.AppendLine("<SIAFDOC>");
            lSbXml.AppendLine(" <cdMsg>SIAFConsultaEmpenhos</cdMsg>");
            lSbXml.AppendLine("<SiafemDocConsultaEmpenhos>");
            lSbXml.AppendLine(" 	<StatusOperacao>true</StatusOperacao>");
            lSbXml.AppendLine("	<documento>");
            lSbXml.AppendLine(" 		<DataConsulta>27/02/2013</DataConsulta>");
            lSbXml.AppendLine(" 		<HoraConsulta>15:19</HoraConsulta>");
            lSbXml.AppendLine(" 		<Usuario>DOUGLAS</Usuario>");
            lSbXml.AppendLine(" 		<DataEmissao>02JAN2013</DataEmissao>");
            lSbXml.AppendLine(" 		<Docto>* NE</Docto>");
            lSbXml.AppendLine(" 		<NumeroNe>2013NE00004</NumeroNe>");
            lSbXml.AppendLine(" 		<DataLancamento>02JAN2013</DataLancamento>");
            lSbXml.AppendLine(" 		<UnidadeGestora>380236- PENIT. II DE BALBINOS</UnidadeGestora>");
            lSbXml.AppendLine(" 		<Gestao>00001- GOVERNO DO ESTADO DE SAO PAULO</Gestao>");
            lSbXml.AppendLine(" 		<CgcCpf>07993274000178- DILAINI ENCARNACAO GALHARDO LOLI ME</CgcCpf>");
            lSbXml.AppendLine(" 		<GestaoCredor></GestaoCredor>");
            lSbXml.AppendLine(" 		<Evento>400051- EMPENHO DE DOTACAO RESERVADA</Evento>");
            lSbXml.AppendLine(" 		<Ptres>380617</Ptres>");
            lSbXml.AppendLine(" 		<Uo>38006</Uo>");
            lSbXml.AppendLine(" 		<Pt>14421381361410000</Pt>");
            lSbXml.AppendLine(" 		<Fonte>001001001</Fonte>");
            lSbXml.AppendLine(" 		<Despesa>33903010</Despesa>");
            lSbXml.AppendLine(" 		<Ugo>380015</Ugo>");
            lSbXml.AppendLine(" 		<PlanoInterno>007.008.0196</PlanoInterno>");
            lSbXml.AppendLine(" 		<Modalidade>5 - GLOBAL</Modalidade>");
            lSbXml.AppendLine(" 		<Licitacao>7 - PREGAO</Licitacao>");
            lSbXml.AppendLine(" 		<ReferenciaLegal>LEI FEDERAL 8.666/93</ReferenciaLegal>");
            lSbXml.AppendLine(" 		<OrigemMaterial>1</OrigemMaterial>");
            lSbXml.AppendLine(" 		<NumeroProcesso>162/12PIIB</NumeroProcesso>");
            lSbXml.AppendLine(" 		<ServicoouMaterial>2</ServicoouMaterial>");
            lSbXml.AppendLine(" 		<EmpenhoOriginal></EmpenhoOriginal>");
            lSbXml.AppendLine(" 		<Oc></Oc>");
            lSbXml.AppendLine(" 		<NumeroContrato>2013CT00004</NumeroContrato>");
            lSbXml.AppendLine(" 		<IdentificadorObra></IdentificadorObra>");
            lSbXml.AppendLine(" 		<Valor>31.623,48</Valor>");
            lSbXml.AppendLine(" 		<Local>ROD.ACESSO ARCIRIO RIGOTTO KM 2.6 BALBINOS</Local>");
            lSbXml.AppendLine(" 		<DataEntrega>30ABR2013</DataEntrega>");
            lSbXml.AppendLine(" 		<TipoEmpenho>9 - DESPESA NORMAL</TipoEmpenho>");
            lSbXml.AppendLine(" 		<Lancadopor>EVA APARECIDA CACERES - 380236</Lancadopor>");
            lSbXml.AppendLine(" 		<DataLancamento>02JAN2013</DataLancamento>");
            lSbXml.AppendLine(" 		<HoraLancamento>16:26</HoraLancamento>");
            lSbXml.AppendLine(" 		<Mes01>01</Mes01>");
            lSbXml.AppendLine(" 		<Mes02>02</Mes02>");
            lSbXml.AppendLine(" 		<Mes03>03</Mes03>");
            lSbXml.AppendLine(" 		<Mes04>04</Mes04>");
            lSbXml.AppendLine(" 		<Mes05></Mes05>");
            lSbXml.AppendLine(" 		<Mes06></Mes06>");
            lSbXml.AppendLine(" 		<Mes07></Mes07>");
            lSbXml.AppendLine(" 		<Mes08></Mes08>");
            lSbXml.AppendLine(" 		<Mes09></Mes09>");
            lSbXml.AppendLine(" 		<Mes10></Mes10>");
            lSbXml.AppendLine(" 		<Mes11></Mes11>");
            lSbXml.AppendLine(" 		<Mes12></Mes12>");
            lSbXml.AppendLine(" 		<Mes13></Mes13>");
            lSbXml.AppendLine(" 		<Valor01>9.450,00</Valor01>");
            lSbXml.AppendLine(" 		<Valor02>7.862,40</Valor02>");
            lSbXml.AppendLine(" 		<Valor03>7.862,40</Valor03>");
            lSbXml.AppendLine(" 		<Valor04>6.448,68</Valor04>");
            lSbXml.AppendLine(" 		<Valor05></Valor05>");
            lSbXml.AppendLine(" 		<Valor06></Valor06>");
            lSbXml.AppendLine(" 		<Valor07></Valor07>");
            lSbXml.AppendLine(" 		<Valor08></Valor08>");
            lSbXml.AppendLine(" 		<Valor09></Valor09>");
            lSbXml.AppendLine(" 		<Valor10></Valor10>");
            lSbXml.AppendLine(" 		<Valor11></Valor11>");
            lSbXml.AppendLine(" 		<Valor12></Valor12>");
            lSbXml.AppendLine(" 		<Valor13></Valor13>");
            lSbXml.AppendLine("	<Repete>");
            lSbXml.AppendLine("		<tabela>");
            lSbXml.AppendLine("		<sequencia>1</sequencia>");
            lSbXml.AppendLine("		<item>001</item>");
            lSbXml.AppendLine("		<material>00331793-5</material>");
            lSbXml.AppendLine("		<unidade>0187</unidade>");
            lSbXml.AppendLine("		<qtdeitem>000008366,000</qtdeitem>");
            lSbXml.AppendLine("		<valorunitario>3,78</valorunitario>");
            lSbXml.AppendLine("		<precototal>31.623,48</precototal>");
            lSbXml.AppendLine("<descricao>CAFE TRADICIONAL, TORRADO E MOIDO,CONSTITUIDO DE CAFE ATE TIPO 8 NACLASSIFICACAO OFICIAL BRASILEIRA -COB, BEBIDA VARIANDO DEMOLE A RIO,EXCLUINDO-SE O GOSTO RIOZONA, COM UM MAXIMO DE 20% DE DEFEITOS PRETOS,VERDES E ARDIDOS,E AUSENCIA, DE GRAOS PRETOS- VERDES E FERMENTADOS,ADMITINDO-SE GRAOS DE, SAFRAS PASSADAS,ROBUSTA CONILLON, DESDE QUE O GOSTO NAO SEJA PRONUNCIADO E PREPONDERANTE, PONTO DE TORRA MODERADAMENTE ESCURO A MEDIO CLARO, COM QUALIDADE GLOBAL ACEITAVEL MINIMA DE 4,5 PONTOS NAESCALA SENSORIAL DE 0 A 10 DO LOTE ENTREGUE, IMPUREZAS (CASCAS E PAUS), EM G/100G MAXIMA DE 1%, E UMIDADE EM G/100G MAXIMA DE 5%, OBEDECENDO RESOLUCAO SAA 19, DE 05/04/2010, COM EMBALAGEM ALTO VACUO (TIJOLINHO), ROTULAGEM IMPRESSA NO PACOTE, NAO SENDO TOLERADA A PRESENCA DE ETIQUETA AUTO ADESIVA COM ADESCRICAO DO PRODUTO, VALIDADE MINIMA NA DATA DA ENTREGA DE (11) ONZE MESES, DEVENDO OBEDECER AS EXIGENCIASDAS PORTARIA 377, DE</descricao>");
            lSbXml.AppendLine("<descricao1></descricao1>");
            lSbXml.AppendLine("<descricao2></descricao2>");
            lSbXml.AppendLine("<descricao3></descricao3>");
            lSbXml.AppendLine("		</tabela>");
            lSbXml.AppendLine("		<tabela>");
            lSbXml.AppendLine("		<sequencia>2</sequencia>");
            lSbXml.AppendLine("		<item>001</item>");
            lSbXml.AppendLine("		<material>00331793-5</material>");
            lSbXml.AppendLine("		<unidade>0187</unidade>");
            lSbXml.AppendLine("		<qtdeitem>000008366,000</qtdeitem>");
            lSbXml.AppendLine("		<valorunitario>3,78</valorunitario>");
            lSbXml.AppendLine("		<precototal>31.623,48</precototal>");
            lSbXml.AppendLine("<descricao>26/04/1999 E PORT.259/2002, RES-SAA 28 DE 01/06/2007, INSTRUCAO NORMATIVA NR 16, DE 24/05/2010 DO MAPA PARA A ELABORACAO DE LAUDO APOS A ENTREGA DO CAFE</descricao>");
            lSbXml.AppendLine("<descricao1></descricao1>");
            lSbXml.AppendLine("<descricao2></descricao2>");
            lSbXml.AppendLine("<descricao3></descricao3>");
            lSbXml.AppendLine("		</tabela>");
            lSbXml.AppendLine("	</Repete>");
            lSbXml.AppendLine("	</documento>");
            lSbXml.AppendLine(" 	<MsgRetorno></MsgRetorno>");
            lSbXml.AppendLine("</SiafemDocConsultaEmpenhos>");
            lSbXml.AppendLine("</SIAFDOC>");

            lStrRetorno = lSbXml.ToString();

            return lStrRetorno;
        }
        public static string SiafemDocConsultaEmpenhos_UGE_380183_GESTAO_00001_NUMERONE_2013NE00320()
        {
            string strRetrono = string.Empty;
            StringBuilder sbXml = new StringBuilder();

            sbXml.AppendLine("<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFConsultaEmpenhos</cdMsg><SiafemDocConsultaEmpenhos><documento><UnidadeGestora>380183</UnidadeGestora><Gestao>00001</Gestao><NumeroNe>2013NE00320</NumeroNe></documento></SiafemDocConsultaEmpenhos></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno>");
            sbXml.AppendLine("<SIAFDOC>");
            sbXml.AppendLine(" <cdMsg>SIAFConsultaEmpenhos</cdMsg>");
            sbXml.AppendLine("<SiafemDocConsultaEmpenhos>");
            sbXml.AppendLine(" 	<StatusOperacao>true</StatusOperacao>");
            sbXml.AppendLine("	<documento>");
            sbXml.AppendLine(" 		<DataConsulta>27/09/2013</DataConsulta>");
            sbXml.AppendLine(" 		<HoraConsulta>17:07</HoraConsulta>");
            sbXml.AppendLine(" 		<Usuario>DOUGLAS</Usuario>");
            sbXml.AppendLine(" 		<DataEmissao>28JUN2013</DataEmissao>");
            sbXml.AppendLine(" 		<Docto>* NE</Docto>");
            sbXml.AppendLine(" 		<NumeroNe>2013NE00320</NumeroNe>");
            sbXml.AppendLine(" 		<DataLancamento>28JUN2013</DataLancamento>");
            sbXml.AppendLine(" 		<UnidadeGestora>380183- DEPARTAMENTO DE ADMINISTRACAO</UnidadeGestora>");
            sbXml.AppendLine(" 		<Gestao>00001- GOVERNO DO ESTADO DE SAO PAULO</Gestao>");
            sbXml.AppendLine(" 		<CgcCpf>05293074000187- VMI SISTEMAS DE SEGURANCA LTDA</CgcCpf>");
            sbXml.AppendLine(" 		<GestaoCredor></GestaoCredor>");
            sbXml.AppendLine(" 		<Evento>400051- EMPENHO DE DOTACAO RESERVADA</Evento>");
            sbXml.AppendLine(" 		<Ptres>380516</Ptres>");
            sbXml.AppendLine(" 		<Uo>38005</Uo>");
            sbXml.AppendLine(" 		<Pt>14421381361390000</Pt>");
            sbXml.AppendLine(" 		<Fonte>001001001</Fonte>");
            sbXml.AppendLine(" 		<Despesa>44905234</Despesa>");
            sbXml.AppendLine(" 		<Ugo>380014</Ugo>");
            sbXml.AppendLine(" 		<PlanoInterno>005.011.0244</PlanoInterno>");
            sbXml.AppendLine(" 		<Modalidade>1 - ORDINARIO</Modalidade>");
            sbXml.AppendLine(" 		<Licitacao>7 - PREGAO</Licitacao>");
            sbXml.AppendLine(" 		<ReferenciaLegal>LEI 8666/93</ReferenciaLegal>");
            sbXml.AppendLine(" 		<OrigemMaterial></OrigemMaterial>");
            sbXml.AppendLine(" 		<NumeroProcesso>093/13-CRC</NumeroProcesso>");
            sbXml.AppendLine(" 		<ServicoouMaterial>2</ServicoouMaterial>");
            sbXml.AppendLine(" 		<EmpenhoOriginal></EmpenhoOriginal>");
            sbXml.AppendLine(" 		<Oc>2013OC00041</Oc>");
            sbXml.AppendLine(" 		<NumeroContrato>2013CT00220</NumeroContrato>");
            sbXml.AppendLine(" 		<IdentificadorObra></IdentificadorObra>");
            sbXml.AppendLine(" 		<Valor>194.000,00</Valor>");
            sbXml.AppendLine(" 		<Local>ROD JORNALISTA FCO AGUIRRE PROENÇA, KM 5</Local>");
            sbXml.AppendLine(" 		<DataEntrega>25SET2013</DataEntrega>");
            sbXml.AppendLine(" 		<TipoEmpenho>9 - DESPESA NORMAL</TipoEmpenho>");
            sbXml.AppendLine(" 		<Lancadopor>DIELE DA SILVA DALBON - 380183</Lancadopor>");
            sbXml.AppendLine(" 		<DataLancamento>28JUN2013</DataLancamento>");
            sbXml.AppendLine(" 		<HoraLancamento>16:15</HoraLancamento>");
            sbXml.AppendLine(" 		<Mes01>06</Mes01>");
            sbXml.AppendLine(" 		<Mes02></Mes02>");
            sbXml.AppendLine(" 		<Mes03></Mes03>");
            sbXml.AppendLine(" 		<Mes04></Mes04>");
            sbXml.AppendLine(" 		<Mes05></Mes05>");
            sbXml.AppendLine(" 		<Mes06></Mes06>");
            sbXml.AppendLine(" 		<Mes07></Mes07>");
            sbXml.AppendLine(" 		<Mes08></Mes08>");
            sbXml.AppendLine(" 		<Mes09></Mes09>");
            sbXml.AppendLine(" 		<Mes10></Mes10>");
            sbXml.AppendLine(" 		<Mes11></Mes11>");
            sbXml.AppendLine(" 		<Mes12></Mes12>");
            sbXml.AppendLine(" 		<Mes13></Mes13>");
            sbXml.AppendLine(" 		<Valor01>194.000,00</Valor01>");
            sbXml.AppendLine(" 		<Valor02></Valor02>");
            sbXml.AppendLine(" 		<Valor03></Valor03>");
            sbXml.AppendLine(" 		<Valor04></Valor04>");
            sbXml.AppendLine(" 		<Valor05></Valor05>");
            sbXml.AppendLine(" 		<Valor06></Valor06>");
            sbXml.AppendLine(" 		<Valor07></Valor07>");
            sbXml.AppendLine(" 		<Valor08></Valor08>");
            sbXml.AppendLine(" 		<Valor09></Valor09>");
            sbXml.AppendLine(" 		<Valor10></Valor10>");
            sbXml.AppendLine(" 		<Valor11></Valor11>");
            sbXml.AppendLine(" 		<Valor12></Valor12>");
            sbXml.AppendLine(" 		<Valor13></Valor13>");
            sbXml.AppendLine("	<Repete>");
            sbXml.AppendLine("		<tabela>");
            sbXml.AppendLine("		<sequencia>1</sequencia>");
            sbXml.AppendLine("		<item>001</item>");
            sbXml.AppendLine("		<material>00236853-6</material>");
            sbXml.AppendLine("		<unidade>0001</unidade>");
            sbXml.AppendLine("		<qtdeitem>000000001,000</qtdeitem>");
            sbXml.AppendLine("		<valorunitario>118.000,00</valorunitario>");
            sbXml.AppendLine("		<precototal>118.000,00</precototal>");
            sbXml.AppendLine("<descricao>EQUIPAMENTO DE INSPECAO POR RADIACAO, COMPOSTO DE DETECTOR DE MATERIAIS ORGANICOS E INORGANICOS ATRAVES DE RAIO X, MONTADO EM ESTRUTURA DE ACO SOLDADO COM TRATAMENTO ANTICORROSIVO E ALUMINIO, DEVERA POSSUIR TUNEL DE INSPECAO COM DIMENSOES DE 100CMX100CM (ALTXLARG), COM ESTEIRA DE BORRACHA VELOCIDADE MINIMA 0,22M/S DOIS SENTIDOS, SUPORTANDO PESO DE ATE 200KG E A PROVA DA AGUA, COM SISTEMA DE SEGURANCA EM CASO DE EMERGENCIA ATRAVES DO BATAO PUSH BUTTON DESLIGAMENTO GERAL, COM CAPACIDADE DE DETECTAR UM FIO DE NO MINIMO 38AWG E ATE 30AWG CONMFORME NORMA ASTM-F792, O SISTEMA DEVERA POSSUIR UM MICROCOMPUTADOR COM PROCESSADOR MINIMO PENTIUM III OU EQUIVALETE SUPERIOR, COM MEMORIA DE 64MB RAM, HD 8GB MINIMO, 16MB DE VIDEO, CD ROOM DE 40X E FD 1,44MB, DEVERA POSSUIR MONITOR C/DIFERENCIACAO DE CORES PARA MATERIAIS METALICOS ORGANICOS E INORGANICOS, TERA MONITOR COLORIDO DE ALTA RESOLUCAO DE 17\" RESOLUCAO MINIMA DE 0,28MM</descricao>");
            sbXml.AppendLine("<descricao1></descricao1>");
            sbXml.AppendLine("<descricao2></descricao2>");
            sbXml.AppendLine("<descricao3></descricao3>");
            sbXml.AppendLine("		</tabela>");
            sbXml.AppendLine("		<tabela>");
            sbXml.AppendLine("		<sequencia>2</sequencia>");
            sbXml.AppendLine("		<item>001</item>");
            sbXml.AppendLine("		<material>00236853-6</material>");
            sbXml.AppendLine("		<unidade>0001</unidade>");
            sbXml.AppendLine("		<qtdeitem>000000001,000</qtdeitem>");
            sbXml.AppendLine("		<valorunitario>118.000,00</valorunitario>");
            sbXml.AppendLine("		<precototal>118.000,00</precototal>");
            sbXml.AppendLine("<descricao>E 800X600 PIXELS, COM INTERFACEAMENTO COM O OPERADOR VIA TECLADO TIPO MEMBRANA, O SISTEMA DEVERA POSSIBILITAR ZOOM DE 8 VEZES EM PRETO E BRANCO E PSEUDO COLOR, COM ARMAZENAMENTO DE IMAGENS COM DUAS INSPECOES E COM COPIA EM MEIO MAGNETICO OU OTICO REMOVIVEL, ALEM DE INSPECAO NOS DOIS SENTIDOS DA ESTEIRA, POSSIBILITAR A PENETRACAO EM ACO DE 8MM GARANTINDO A PENTETRACAO MINIMA DE 10MMTIPICO EM ACO, RADIACAO TIPICA POR INSPECAO DE 0,1 MR/H, VAZAMENTO DE RADIACAO TIPICA DE 0,05MR/H A 50MM DO EQUIPAMENTO, DEVENDO SER INOFENSIVO AO ORGANISMO HUMANO E NAO DEVE AFETAR DE FORMA ALGUMA O ORGANISMO HUMANONEM O EQUIPAMENTO, REFRIGERACAO DO EQUIPAMENTO ATRAVES DE IMERSAO EM BANHO DE OLEO SELADO OU TECNOLOGIA MAISSEGURA, OPERACAO EM CORRENTE DE 1,0 M/A, VOLTAGEM DO ANODO DE 90 KVP, VOLTAGEM DE 110/220VAC MONOFISICO COM FONTE NEUTRA E TERRA, COM REGIME DE OPERACAO DE 24 HORAS ININTERRUPTAS, DEVERA POSSUIR SISTEMA DE SUP</descricao>");
            sbXml.AppendLine("<descricao1></descricao1>");
            sbXml.AppendLine("<descricao2></descricao2>");
            sbXml.AppendLine("<descricao3></descricao3>");
            sbXml.AppendLine("		</tabela>");
            sbXml.AppendLine("		<tabela>");
            sbXml.AppendLine("		<sequencia>3</sequencia>");
            sbXml.AppendLine("		<item>001</item>");
            sbXml.AppendLine("		<material>00236853-6</material>");
            sbXml.AppendLine("		<unidade>0001</unidade>");
            sbXml.AppendLine("		<qtdeitem>000000001,000</qtdeitem>");
            sbXml.AppendLine("		<valorunitario>118.000,00</valorunitario>");
            sbXml.AppendLine("		<precototal>118.000,00</precototal>");
            sbXml.AppendLine("<descricao>ERVISAO COM INDICACAO DE FALHA NO SISTEMA COM AUTODIAGNOSTICO, DEVERA TER SINALIZADOR EXTERNO QUANDO EM ESTADO DE EMISSAO DE RADICAO E POSSUIR SINAL VISUAL EXTERNO, DEVERA ACOMPANHAR O EQUIPAMENTO UM KIT ESPECIAL FERRAMENTAS PARA MANUTENCAO E CONSERVACAO, A INSTALACAO DO EQUIPAMENTO DEVERA SER COMPLETA COM REGULAGEM DO STAR UP DO SISTEMA, PROPORCIONAR TREINAMENTO DE OPERADORES C/MANUAIS DE MANUTENCAO E DE OPERACAO, COM TESTES DE ACEITE REALIZADOS JUNTAMENTE COM A EQUIPE GERENCIADORA DO EQUIPAMENTO, COM MANUAIS EM PORTUGUES E ASSISTENCIA TECNICA EM 48 HORAS NO LOCAL</descricao>");
            sbXml.AppendLine("<descricao1></descricao1>");
            sbXml.AppendLine("<descricao2></descricao2>");
            sbXml.AppendLine("<descricao3></descricao3>");
            sbXml.AppendLine("		</tabela>");
            sbXml.AppendLine("		<tabela>");
            sbXml.AppendLine("		<sequencia>4</sequencia>");
            sbXml.AppendLine("		<item>002</item>");
            sbXml.AppendLine("		<material>00169363-8</material>");
            sbXml.AppendLine("		<unidade>0001</unidade>");
            sbXml.AppendLine("		<qtdeitem>000000001,000</qtdeitem>");
            sbXml.AppendLine("		<valorunitario>76.000,00</valorunitario>");
            sbXml.AppendLine("		<precototal>76.000,00</precototal>");
            sbXml.AppendLine("<descricao>EQUIPAMENTO DE INSPECAO POR RADIACAO, COMPOSTO DE DETECTOR DE MATERIAIS ORGANICOS E INORGANICOS ATRAVES DE RAIO X, MONTADO EM ESTRUTURA ACO SOLDADO, COM TRATAMENTO ANTICORROSIVO E ALUMINIO,COM RODIZIOS TRANSVERSAIS, DEVERA POSSUIR TUNEL DE INSPECAO COM DIMENSOES DE 30CMX50CM (ALTXLARG), COM ESTEIRA DE BORRACHA,VELOCIDADE MINIMA 0,22M/S DOIS SENTIDOS, SUPORTANDO PESO DE ATE 50KG E A PROVA DE AGUA, COM SISTEMA DE SEGURANCA EM CASO DE EMERGENCIA ATRAVES DE BOTAO PUSH BUTTON DESLIGAMENTO GERAL, COM CAPACIDADE DE DETECTAR UM FIO DE NO MINIMO 38AWG E ATE 30AWG CONFORME NORMA ASTM-F792, O SISTEMA DEVERA POSSUIR UM MICROCOMPUTADOR COM PROCESSADOR MINIMO PENTIUM III OU EQUIVALENTE, COM MEMORIA DE 64MB RAM, HD 8GB MINIMO, 16MB DE VIDEO, CD ROOM DE 40X E FD 1,44MB, DEVERA POSSUIR MONITOR COM DIFERENCIACAO DE CORES PARA MATERAIS METALICOS,ORGANICOS E INORGANICOS, TERA MONITOR COLORIDO DE ALTA RESOLUCAO DE 15\", RESOLUCAO MI</descricao>");
            sbXml.AppendLine("<descricao1></descricao1>");
            sbXml.AppendLine("<descricao2></descricao2>");
            sbXml.AppendLine("<descricao3></descricao3>");
            sbXml.AppendLine("		</tabela>");
            sbXml.AppendLine("		<tabela>");
            sbXml.AppendLine("		<sequencia>5</sequencia>");
            sbXml.AppendLine("		<item>002</item>");
            sbXml.AppendLine("		<material>00169363-8</material>");
            sbXml.AppendLine("		<unidade>0001</unidade>");
            sbXml.AppendLine("		<qtdeitem>000000001,000</qtdeitem>");
            sbXml.AppendLine("		<valorunitario>76.000,00</valorunitario>");
            sbXml.AppendLine("		<precototal>76.000,00</precototal>");
            sbXml.AppendLine("<descricao>NIMA DE 0,28MM E 800X600 PIXELS, COM INTERFACEAMENTO COM O OPERADOR VIA TECLADO TIPO MEMBRANA, O SISTEMA DEVERA POSSIBILITAR ZOOM DE 4 VEZES EM PRETO E BRANCO E PSEUDO COLOR, COM ARMAZENAMENTO DE IMAGENS COM DUAS INSPECOES E COM COPIA EM MEIO MAGNETICO OU OTICO REMOVIVEL, ALEM DE INSPECAO NOS DOIS SENTIDOS DA ESTEIRA, POSSIBILITAR A PENETRACAO EM ACO DE 8MM GARANTIDO E PENETRACAO MINIMA DE 10MM TIPICO EM ACO, RADIACAO TIPICA POR INSPECAO DE 0,1 MR/H, VAZAMENTO DE RADIACAO TIPICA DE 0,05MR/H A 50MM DO EQUIPAMENTO, DEVENDO SER INOFENSIVO AO ORGANISMO HUMANO E NAO DEVE AFETAR NENHUMA FORMA O ORGANISMO HUMANO NEM EQUIPAMENTOS ELETRONICOS E MAGNETICOS, REFRIGERACAO DO EQUIPAMENTO ATRAVES DE IMERSAO EM BANHO DE OLEO SELADO OU TECNOLOGIA MAIS SEGURA, OPERACAO EM CORRENTE DE 1,0M/A, VOLTAGEM DO ANODO DE 90 KVP, VOLTAGEM DE 110/220VAC,MONOFASICO,60HZ,COM FONTE NEUTRA E TERRA, COM REGIME DE OPERACAO DE 24 HORAS ININTERRU</descricao>");
            sbXml.AppendLine("<descricao1></descricao1>");
            sbXml.AppendLine("<descricao2></descricao2>");
            sbXml.AppendLine("<descricao3></descricao3>");
            sbXml.AppendLine("		</tabela>");
            sbXml.AppendLine("		<tabela>");
            sbXml.AppendLine("		<sequencia>6</sequencia>");
            sbXml.AppendLine("		<item>002</item>");
            sbXml.AppendLine("		<material>00169363-8</material>");
            sbXml.AppendLine("		<unidade>0001</unidade>");
            sbXml.AppendLine("		<qtdeitem>000000001,000</qtdeitem>");
            sbXml.AppendLine("		<valorunitario>76.000,00</valorunitario>");
            sbXml.AppendLine("		<precototal>76.000,00</precototal>");
            sbXml.AppendLine("<descricao>PTAS, DEVERA POSSUIR SISTEMA DE SUPERVISAO COM INDICACAO DE FALHA DO SISTEMA, COM AUTODIAGNOSTICO, DEVERA TER SINALIZADOR EXTERNO QUANDO EM ESTADO DE POSSUIR SINAL VISUAL EXTERNO, QUANDO ENERGIZADO EM ESTADO DE EMISSAO DE RADIACAO, DEVERA ACOMPANHAR O EQUIPAMENTO UM KIT ESPECIAL DE FERRAMENTAS PARA MANUTENCAO E CONSERVACAO, A INSTALACAO DO EQUIPAMENTO DEVERA SER COMPLETA COM REGULAGEM DO STAR UP DO SISTEMA, PROPORCIONAR TREINAMENTO TREINAMENTO DE OPERADORES, MANUAIS DE MANUTENCAO E DE OPERACAO, COM TESTES DE ACEITE REALIZADOS JUNTAMENTE COM A COM TESTES DE ACEITE REALIZADOS JUNTAMENTE COM A, COM MANUAIS EM PORTUGUES E ASSISTENCIA TECNICA EM 48 HORAS NO LOCAL PELO PERIODO DE 5 ANOS</descricao>");
            sbXml.AppendLine("<descricao1></descricao1>");
            sbXml.AppendLine("<descricao2></descricao2>");
            sbXml.AppendLine("<descricao3></descricao3>");
            sbXml.AppendLine("		</tabela>");
            sbXml.AppendLine("	</Repete>");
            sbXml.AppendLine("	</documento>");
            sbXml.AppendLine(" 	<MsgRetorno></MsgRetorno>");
            sbXml.AppendLine("</SiafemDocConsultaEmpenhos>");
            sbXml.AppendLine("</SIAFDOC>");
            sbXml.AppendLine("</Doc_Retorno></SISERRO></MSG>");

            strRetrono = sbXml.ToString();
            return strRetrono;
        }
        #endregion Empenhos com descricao de item grande

        public static string SiafisicoDocConsultaI_ItemMaterial_004152603()
        {
            string strRetorno = string.Empty;
            StringBuilder sbXml = new StringBuilder();

            sbXml.AppendLine("<MSG><BCMSG><Doc_Estimulo><SFCODOC><cdMsg>SFCOConsultaI</cdMsg><SiafisicoDocConsultaI><documento><CodigoItem>004152603</CodigoItem></documento></SiafisicoDocConsultaI></SFCODOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno>");
            sbXml.AppendLine("<SFCODOC>");
            sbXml.AppendLine(" <cdMsg>SFCOConsultaI</cdMsg>");
            sbXml.AppendLine(" <SiafisicoDocConsultaI>");
            sbXml.AppendLine("<documento>");
            sbXml.AppendLine("<StatusOperacao>true</StatusOperacao>");
            sbXml.AppendLine("<Classe>2710 MOTORES AUTOMOTIVOS E SEUS AGREGADOS</Classe>");
            sbXml.AppendLine("<Classe1></Classe1>");
            sbXml.AppendLine("<Grupo>27 PECAS E ACESSORIOS PARA AUTOMOVEIS, MOTOCICLETAS, CICLOMOTORES, MOT</Grupo>");
            sbXml.AppendLine("<Grupo1>ONETAS E VEICULOS ESPECIAIS DE BOMBEIROS</Grupo1>");
            sbXml.AppendLine("<Item>004152603 PARAFUSO AUTOMOTIVO,93809149</Item>");
            sbXml.AppendLine("<Item1></Item1>");
            sbXml.AppendLine("<Material>00131326 PARAFUSO AUTOMOTIVO</Material>");
            sbXml.AppendLine("<Natureza>33903050</Natureza>");
            sbXml.AppendLine("<Natureza2>33903053</Natureza2>");
            sbXml.AppendLine("<Natureza3>33903065</Natureza3>");
            sbXml.AppendLine("<MsgRetorno></MsgRetorno>");
            sbXml.AppendLine("</documento>");
            sbXml.AppendLine(" </SiafisicoDocConsultaI>");
            sbXml.AppendLine("</SFCODOC>");
            sbXml.AppendLine("</Doc_Retorno></SISERRO></MSG>");

            strRetorno = sbXml.ToString();

            return strRetorno;
        }

        public static string errSiafemDocDetaContaGen_UGE_441101_GESTAO_44047_MES_2012ABR()
        {
            string strRetorno = string.Empty;
            StringBuilder sbXml = new StringBuilder();

            sbXml.AppendLine("<MSG>	<BCMSG>		<Doc_Estimulo><SIAFDOC>");
            sbXml.AppendLine("    <cdMsg>SIAFDetaContaGen</cdMsg>");
            sbXml.AppendLine("    <SiafemDocDetaContaGen>");
            sbXml.AppendLine("        <documento>");
            sbXml.AppendLine("            <CodigoUG>441101</CodigoUG>");
            sbXml.AppendLine("            <Gestao>44047</Gestao>");
            sbXml.AppendLine("            <Mes>Abr</Mes>");
            sbXml.AppendLine("            <ContaContabil>192410101</ContaContabil>");
            sbXml.AppendLine("            <ContaCorrente></ContaCorrente>");
            sbXml.AppendLine("            <Opcao>1</Opcao>");
            sbXml.AppendLine("        </documento>");
            sbXml.AppendLine("    </SiafemDocDetaContaGen>");
            sbXml.AppendLine("</SIAFDOC>");
            sbXml.AppendLine("</Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC>");
            sbXml.AppendLine("<cdMsg>SIAFDETACONTAGEN</cdMsg>");
            sbXml.AppendLine("    <HorarioAcesso>");
            sbXml.AppendLine("        <Mensagem>");
            sbXml.AppendLine("            <StatusOperacao>false</StatusOperacao>");
            sbXml.AppendLine("            <MsgErro>Mensagem fora do horario de processamento (07:00 - 19:00). Hora atual 19:1.</MsgErro>");
            sbXml.AppendLine("        </Mensagem>");
            sbXml.AppendLine("    </HorarioAcesso>");
            sbXml.AppendLine("</SIAFDOC>");
            sbXml.AppendLine("</Doc_Retorno></SISERRO></MSG>");

            strRetorno = sbXml.ToString();

            return strRetorno;
        }
        public static string errSiafisicoLogin_ACESSO_NAO_PERMITIDO()
        {
            string strRetorno = string.Empty;
            StringBuilder sbXml = new StringBuilder();

            sbXml.AppendLine("<MSG><BCMSG></BCMSG><SISERRO>");
            sbXml.AppendLine("<SFCODOC>");
            sbXml.AppendLine(" <cdMsg>SFCOLOGIN001</cdMsg>");
            sbXml.AppendLine(" <SiafisicoLogin>");
            sbXml.AppendLine("   <login>");
            sbXml.AppendLine("   	<StatusOperacao>false</StatusOperacao>");
            sbXml.AppendLine("   	<MsgErro>- ACESSO NAO PERMITIDO</MsgErro>");
            sbXml.AppendLine("   </login>");
            sbXml.AppendLine(" </SiafisicoLogin>");
            sbXml.AppendLine("</SFCODOC>");
            sbXml.AppendLine("Vai transmitir para WS:http://localhost:8085/RobosSiafisico/RobosSiafisicoNetVHI.jws");
            sbXml.AppendLine("URL WS:http://localhost:8085/RobosSiafisico/RobosSiafisicoNetVHI.jws");
            sbXml.AppendLine();
            sbXml.AppendLine("Servidor:");
            sbXml.AppendLine("---------");
            sbXml.AppendLine("192.168.39.28  srv10496");
            sbXml.AppendLine();
            sbXml.AppendLine("Login:");
            sbXml.AppendLine("------");
            sbXml.AppendLine("Usuário:35593145857");
            sbXml.AppendLine("Ano Base:2013");
            sbXml.AppendLine();
            sbXml.AppendLine();
            sbXml.AppendLine("<SFCODOC>");
            sbXml.AppendLine(" <cdMsg>SFCOLOGIN001</cdMsg>");
            sbXml.AppendLine(" <SiafisicoLogin>");
            sbXml.AppendLine("   <login>");
            sbXml.AppendLine("   	<StatusOperacao>false</StatusOperacao>");
            sbXml.AppendLine("   	<MsgErro>- ACESSO NAO PERMITIDO</MsgErro>");
            sbXml.AppendLine("   </login>");
            sbXml.AppendLine(" </SiafisicoLogin>");
            sbXml.AppendLine("</SFCODOC>");
            sbXml.AppendLine();
            sbXml.AppendLine("</SISERRO></MSG>");

            strRetorno = sbXml.ToString();

            return strRetorno;
        }
        public static string errSiafemLogin_ACESSO_NAO_PERMITIDO()
        {
            string strRetorno = string.Empty;
            StringBuilder sbXml = new StringBuilder();

            sbXml.AppendLine("<ERRO>");
            sbXml.AppendLine("<SIAFDOC>");
            sbXml.AppendLine(" <cdMsg>SIAFLOGIN001</cdMsg>");
            sbXml.AppendLine(" <SiafemLogin>");
            sbXml.AppendLine("   <login>");
            sbXml.AppendLine("   	<StatusOperacao>false</StatusOperacao>");
            sbXml.AppendLine("   	<MsgErro>- ACESSO NAO PERMITIDO</MsgErro>");
            sbXml.AppendLine("   </login>");
            sbXml.AppendLine(" </SiafemLogin>");
            sbXml.AppendLine("</SIAFDOC>");
            sbXml.AppendLine("</ERRO>");

            strRetorno = sbXml.ToString();

            return strRetorno;
        }
        public static string errSiafemLogin_INFORME_CODIGO_E_SENHA()
        {
            string strRetorno = string.Empty;
            StringBuilder sbXml = new StringBuilder();

            sbXml.AppendLine("<ERRO>");
            sbXml.AppendLine("<SIAFDOC>");
            sbXml.AppendLine(" <cdMsg>SIAFLOGIN001</cdMsg>");
            sbXml.AppendLine(" <SiafemLogin>");
            sbXml.AppendLine("<login>");
            sbXml.AppendLine("	<StatusOperacao>false</StatusOperacao>");
            sbXml.AppendLine("	<MsgErro>- INFORME CODIGO E SENHA</MsgErro>");
            sbXml.AppendLine("</login>");
            sbXml.AppendLine(" </SiafemLogin>");
            sbXml.AppendLine("</SIAFDOC>");
            sbXml.AppendLine("</ERRO>");

            strRetorno = sbXml.ToString();

            return strRetorno;
        }
        public static string errSiafemLogin_ERRO_DE_EXECUCAO()
        {
            string strRetorno = string.Empty;
            StringBuilder sbXml = new StringBuilder();

            sbXml.AppendLine("<MSG>");
            sbXml.AppendLine("\t<BCMSG />");
            sbXml.AppendLine("\t<SISERRO>");
            sbXml.AppendLine("\t\t<SIAFDOC>");
            sbXml.AppendLine("\t\t\t<cdMsg>SIAFLOGIN001</cdMsg>");
            sbXml.AppendLine("\t\t\t<SiafemLogin>");
            sbXml.AppendLine("\t\t\t\t<login>");
            sbXml.AppendLine("\t\t\t\t\t<StatusOperacao>false</StatusOperacao>");
            sbXml.AppendLine("\t\t\t\t\t<MsgErro>Erro de Execução</MsgErro>");
            sbXml.AppendLine("\t\t\t\t</login>");
            sbXml.AppendLine("\t\t\t</SiafemLogin>");
            sbXml.AppendLine("\t\t</SIAFDOC>");
            sbXml.AppendLine();
            sbXml.AppendLine("URL WS:http://localhost:8085/RobosSiafem/RobosSiafNetVHISied.jws");
            sbXml.AppendLine();
            sbXml.AppendLine("Servidor:");
            sbXml.AppendLine("---------");
            sbXml.AppendLine("192.168.39.28srv10495");
            sbXml.AppendLine();
            sbXml.AppendLine("Login:");
            sbXml.AppendLine("------");
            sbXml.AppendLine("Usuário:70213099969");
            sbXml.AppendLine("Ano Base:2014");
            sbXml.AppendLine();
            sbXml.AppendLine("\t\t<SIAFDOC>");
            sbXml.AppendLine("\t\t\t<cdMsg>SIAFLOGIN001</cdMsg>");
            sbXml.AppendLine("\t\t\t<SiafemLogin>");
            sbXml.AppendLine("\t\t\t\t<login>");
            sbXml.AppendLine("\t\t\t\t\t<Codigo>70213099969</Codigo>");
            sbXml.AppendLine("\t\t\t\t\t<Senha>PRODESP</Senha>");
            sbXml.AppendLine("\t\t\t\t\t<Ano>2014</Ano>");
            sbXml.AppendLine("\t\t\t\t</login>");
            sbXml.AppendLine("\t\t\t</SiafemLogin>");
            sbXml.AppendLine("\t\t</SIAFDOC>");
            sbXml.AppendLine("\t</SISERRO>");
            sbXml.AppendLine("</MSG>");

            strRetorno = sbXml.ToString();

            return strRetorno;
        }

        public static string errHORARIO_ACESSO()
        {
            string strRetorno = string.Empty;
            StringBuilder sbXml = new StringBuilder();

            sbXml.AppendLine("<MSG>	<BCMSG>		<Doc_Estimulo><SFCODOC>");
            sbXml.AppendLine("<cdMsg>SFCOConsultaI</cdMsg>");
            sbXml.AppendLine("<SiafisicoDocConsultaI>");
            sbXml.AppendLine("<documento>");
            sbXml.AppendLine("<CodigoItem>001663283</CodigoItem>");
            sbXml.AppendLine("</documento>");
            sbXml.AppendLine("</SiafisicoDocConsultaI>");
            sbXml.AppendLine("</SFCODOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SFCODOC>");
            sbXml.AppendLine("<cdMsg>SFCOCONSULTAI</cdMsg>");
            sbXml.AppendLine("<HorarioAcesso>");
            sbXml.AppendLine("<Mensagem>");
            sbXml.AppendLine("<StatusOperacao>false</StatusOperacao>");
            sbXml.AppendLine("<MsgErro>Mensagem fora do horário de processamento (XX:00 - XX:00). Hora atual").Append(DateTime.Now.ToString("hh:mm")).Append(".</MsgErro>");
            sbXml.AppendLine("</Mensagem>");
            sbXml.AppendLine("</HorarioAcesso>");
            sbXml.AppendLine("</SFCODOC>");
            sbXml.AppendLine("</Doc_Retorno></SISERRO></MSG>");

            strRetorno = sbXml.ToString();

            return strRetorno;
        }

        public static string SiafemDocDetPTRES_OpFlex(string strCodigoUGE)
        {
            string strRetorno = null;
            IDictionary<string, string> catalogoChaveRetorno = null;
            KeyValuePair<string, string>[] arrChaveUGEeRetornoWS = null;


            arrChaveUGEeRetornoWS = new KeyValuePair<string, string>[] { 
                                                                            #region Diversas
                                                                            //<!-- 2013-04-16 15:59:28,116 [1] INFO  LogInFile Consulta PtRes --> [ UGE 380101 - GABINETE DO SECRETARIO E ASSESSORIAS ] -->
                                                                            new KeyValuePair<string, string>("380101", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380101</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380101</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380123</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta><Conta><PTRES>380126</PTRES><NomePTRES>COMUNICACAO DE ACOES DO GOVERNO</NomePTRES></Conta><Conta><PTRES>380127</PTRES><NomePTRES>EXPANSAO E APERF. DO SISTEMA PENAL PAULISTA</NomePTRES></Conta><Conta><PTRES>380129</PTRES><NomePTRES>MANUT. E EXPANSAO DO MONITORAMENTO ELETRONIC</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 15:59:31,579 [1] INFO  LogInFile Consulta PtRes  [ UGE 380102 - CONSELHO PENITENCIARIO ] -->
                                                                            new KeyValuePair<string, string>("380102", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380102</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380102</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380123</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 15:59:45,743 [1] INFO  LogInFile Consulta PtRes  [ UGE 380120 - CENTRO DE PROGRESSAO PENITENCIARIA DE FRANCO DA ROCHA ] -->
                                                                            new KeyValuePair<string, string>("380120", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380120</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380120</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380310</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380311</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 15:59:50,236 [1] INFO  LogInFile Consulta PtRes  [ UGE 380127 - PENITENCIARIA  DR.JOSE PARADA NETO  - GUARULHOS ] -->
                                                                            new KeyValuePair<string, string>("380127", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380127</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380127</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380310</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380311</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 15:59:58,223 [1] INFO  LogInFile Consulta PtRes  [ UGE 380148 - PENITENCIARIA  ADRIANO MARREY   DE GUARULHOS ] -->
                                                                            new KeyValuePair<string, string>("380148", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380148</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380148</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380311</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:00:02,653 [1] INFO  LogInFile Consulta PtRes  [ UGE 380153 - PENITENCIARIA  MARIO DE MOURA E ALBUQUERQUE  DE FRANCO DA ROCHA ] -->
                                                                            new KeyValuePair<string, string>("380153", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380153</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380153</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380310</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380311</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:00:07,910 [1] INFO  LogInFile Consulta PtRes  [ UGE 380154 - PENITENCIARIA  NILTON SILVA  DE FRANCO DA ROCHA ] -->
                                                                            new KeyValuePair<string, string>("380154", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380154</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380154</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380310</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380311</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:00:12,465 [1] INFO  LogInFile Consulta PtRes  [ UGE 380169 - CENTRO DE DETENCAO PROVISORIA CHACARA BELEM I ] -->
                                                                            new KeyValuePair<string, string>("380169", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380169</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380169</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380310</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380311</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:00:22,168 [1] INFO  LogInFile Consulta PtRes  [ UGE 380171 - CENTRO DE DETENCAO PROVISORIA DE VILA INDEPENDENCIA ] -->
                                                                            new KeyValuePair<string, string>("380171", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380171</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380171</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380310</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380311</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:00:27,144 [1] INFO  LogInFile Consulta PtRes  [ UGE 380173 - CENTRO DETENCAO PROVISORIA I DE OSASCO  EDERSON VIEIRA DE JESUS ] -->
                                                                            new KeyValuePair<string, string>("380173", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380173</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380173</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380310</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380311</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:00:31,839 [1] INFO  LogInFile Consulta PtRes  [ UGE 380174 - CENTRO DETENCAO PROVISORIA  VANDA RITA BRITO DO REGO  DE OSASCO ] -->
                                                                            new KeyValuePair<string, string>("380174", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380174</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380174</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380310</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380311</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:00:37,377 [1] INFO  LogInFile Consulta PtRes  [ UGE 380175 - CENTRO DE DETENCAO PROVISORIA DE SANTO ANDRE ] -->
                                                                            new KeyValuePair<string, string>("380175", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380175</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380175</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380310</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380311</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:00:45,676 [1] INFO  LogInFile Consulta PtRes  [ UGE 380186 - CENTRO DET.PROVISORIA AG.SEG.PENIT.GIOVANI M.RODRIGUES GUARULHOS ] -->
                                                                            new KeyValuePair<string, string>("380186", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380186</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380186</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380311</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:00:50,138 [1] INFO  LogInFile Consulta PtRes  [ UGE 380187 - CENTRO DE DETENCAO PROVISORIA DE GUARULHOS II ] -->
                                                                            new KeyValuePair<string, string>("380187", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380187</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380187</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380310</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380311</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:00:54,677 [1] INFO  LogInFile Consulta PtRes  [ UGE 380209 - CENTRO DETENCAO PROV.AG.SEG.PENIT.VICENTE LUZAN SILVA-PINHEIROS ] -->
                                                                            new KeyValuePair<string, string>("380209", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380209</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380209</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380310</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380311</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:00:59,996 [1] INFO  LogInFile Consulta PtRes  [ UGE 380210 - PENITENCIARIA AGENTE SEGUR.PENITENC.JOAQUIM FONSECA LOPES  PARELH ] -->
                                                                            new KeyValuePair<string, string>("380210", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380210</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380210</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380310</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380311</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:01:06,455 [1] INFO  LogInFile Consulta PtRes  [ UGE 380214 - CENTRO DE PROGRESSAO PENITENCIARIA DE SAO MIGUEL PAULISTA ] -->
                                                                            new KeyValuePair<string, string>("380214", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380214</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380214</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380310</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380311</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:01:10,932 [1] INFO  LogInFile Consulta PtRes  [ UGE 380220 - C.DET.PROVISORIA AG.SEG.PENIT.NILTON CELESTINO-ITAPECERICA SERRA ] -->
                                                                            new KeyValuePair<string, string>("380220", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380220</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380220</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380310</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380311</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:01:15,424 [1] INFO  LogInFile Consulta PtRes  [ UGE 380221 - CENTRO DET.PROV.AG.SEG.PENIT.WILLIANS NOGUEIRA BENJAMIM-PINHEIROS ] -->
                                                                            new KeyValuePair<string, string>("380221", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380221</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380221</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380310</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380311</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:01:19,964 [1] INFO  LogInFile Consulta PtRes  [ UGE 380225 - CENTRO DE DETENCAO PROVISORIA DE MAUA ] -->
                                                                            new KeyValuePair<string, string>("380225", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380225</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380225</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380310</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380311</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:01:24,503 [1] INFO  LogInFile Consulta PtRes  [ UGE 380233 - CENTRO DE DETENCAO PROVIS. DR. CALIXTO ANTONIO S.BERNARDO CAMPO ] -->
                                                                            new KeyValuePair<string, string>("380233", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380233</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380233</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380310</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380311</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:01:27,951 [1] INFO  LogInFile Consulta PtRes  [ UGE 380234 - CENTRO DE DETENCAO PROVISORIA DE DIADEMA ] -->
                                                                            new KeyValuePair<string, string>("380234", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380234</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380234</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380311</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:01:32,412 [1] INFO  LogInFile Consulta PtRes  [ UGE 380241 - PENITENCIARIA FEMININA DE  SANT ANA ] -->
                                                                            new KeyValuePair<string, string>("380241", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380241</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380241</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380310</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380311</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:01:36,858 [1] INFO  LogInFile Consulta PtRes  [ UGE 380245 - CENTRO DE DETENCAO PROVISORIA IV DE PINHEIROS ] -->
                                                                            new KeyValuePair<string, string>("380245", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380245</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380245</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380310</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380311</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:01:41,350 [1] INFO  LogInFile Consulta PtRes  [ UGE 380246 - CENTRO DE DETENCAO PROVISORIA III DE PINHEIROS ] -->
                                                                            new KeyValuePair<string, string>("380246", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380246</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380246</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380310</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380311</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            #endregion Diversas
                                                                            #region UO 38004
                                                                            //<!-- 2013-04-16 16:01:41,350 [1] INFO  LogInFile Consulta PtRes  [ UO 38004 - COORDENADORIA UNIDADES PRISIONAIS REG.VALE DO PARAIBA E LITORAL ] -->
                                                                            //<!-- 2013-04-16 16:01:45,905 [1] INFO  LogInFile Consulta PtRes  [ UGE 380107 - HOSPITAL CUSTODIA TRATAM.PSIQU.DR.ARNALDO AMADO FERREIRA-TAUBATE ] -->
                                                                            new KeyValuePair<string, string>("380107", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380107</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380107</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380410</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380411</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:01:49,322 [1] INFO  LogInFile Consulta PtRes  [ UGE 380108 - PENITENCIARIA FEMININA  SANTA MARIA EUFRASIA PELLETIER -TREMEMBE ] -->
                                                                            new KeyValuePair<string, string>("380108", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380108</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380108</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380411</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:01:54,189 [1] INFO  LogInFile Consulta PtRes  [ UGE 380118 - PENITENCIARIA  DR.GERALDO ANDRADE VIEIRA  - SAO VICENTE ] -->
                                                                            new KeyValuePair<string, string>("380118", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380118</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380118</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380410</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380411</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:01:58,650 [1] INFO  LogInFile Consulta PtRes  [ UGE 380121 - CENTRO PROGRESSAO PENITENCIARIA DR.RUBENS ALEIXO SENDIN  MONGAGUA ] -->
                                                                            new KeyValuePair<string, string>("380121", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380121</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380121</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380410</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380411</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:02:03,049 [1] INFO  LogInFile Consulta PtRes  [ UGE 380126 - PENITENCIARIA II DE SAO VICENTE ] -->
                                                                            new KeyValuePair<string, string>("380126", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380126</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380126</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380410</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380411</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:02:06,512 [1] INFO  LogInFile Consulta PtRes  [ UGE 380141 - PENITENCIARIA  DR.TARCIZO LEONCE PINHEIRO CINTRA   DE TREMEMBE ] -->
                                                                            new KeyValuePair<string, string>("380141", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380141</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380141</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380411</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:02:10,989 [1] INFO  LogInFile Consulta PtRes  [ UGE 380144 - PENITENCIARIA  DR.JOSE AUGUSTO CESAR SALGADO -TREMEMBE ] -->
                                                                            new KeyValuePair<string, string>("380144", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380144</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380144</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380410</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380411</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:02:15,482 [1] INFO  LogInFile Consulta PtRes  [ UGE 380144 - CENTRO PROGRESSAO PENITENCIARIA DR.EDGARD M.NORONHA-TREMEMBE ] -->
                                                                            new KeyValuePair<string, string>("380144", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380146</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380146</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380410</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380411</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:02:19,912 [1] INFO  LogInFile Consulta PtRes  [ UGE 380188 - CENTRO DE DETENCAO PROVISORIA  DR.FELIX NOBRE DE CAMPOS -TAUBATE ] -->
                                                                            new KeyValuePair<string, string>("380188", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380188</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380188</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380410</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380411</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:02:24,374 [1] INFO  LogInFile Consulta PtRes  [ UGE 380189 - CENTRO DE DETENCAO PROVISORIA  LUIS CESAR LACERDA -SAO VICENTE ] -->
                                                                            new KeyValuePair<string, string>("380189", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380189</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380189</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380410</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380411</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:02:28,819 [1] INFO  LogInFile Consulta PtRes  [ UGE 380195 - PENITENCIARIA I DE POTIM ] -->
                                                                            new KeyValuePair<string, string>("380195", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380195</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380195</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380410</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380411</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:02:33,312 [1] INFO  LogInFile Consulta PtRes  [ UGE 380196 - PENITENCIARIA II DE POTIM ] -->
                                                                            new KeyValuePair<string, string>("380196", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380196</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380196</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380410</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380411</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:02:37,758 [1] INFO  LogInFile Consulta PtRes  [ UGE 380213 - CENTRO DE DETENCAO PROVISORIA DE SUZANO ] -->
                                                                            new KeyValuePair<string, string>("380213", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380213</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380213</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380410</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380411</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:02:42,625 [1] INFO  LogInFile Consulta PtRes  [ UGE 380218 - CENTRO DE DETENCAO PROVISORIA DE SAO JOSE DOS CAMPOS ] -->
                                                                            new KeyValuePair<string, string>("380218", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380218</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380218</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380410</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380411</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:02:47,024 [1] INFO  LogInFile Consulta PtRes  [ UGE 380226 - CENTRO DE DETENCAO PROVISORIA DE PRAIA GRANDE ] -->
                                                                            new KeyValuePair<string, string>("380226", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380226</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380226</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380410</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380411</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:02:50,861 [1] INFO  LogInFile Consulta PtRes  [ UGE 380243 - CENTRO DE DETENCAO PROVISORIA DE CARAGUATATUBA ] -->
                                                                            new KeyValuePair<string, string>("380243", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380243</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380243</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380411</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:02:55,292 [1] INFO  LogInFile Consulta PtRes  [ UGE 380250 - PENITENCIARIA FEMININA II DE TREMEMBE ] -->
                                                                            new KeyValuePair<string, string>("380250", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380250</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380250</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380410</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380411</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            #endregion UO 38004
                                                                            #region UO 38005
                                                                            //<!-- 2013-04-16 16:02:55,292 [1] INFO  LogInFile Consulta PtRes  [ UO 38005 - COORDENADORIA DE UNIDADES PRISIONAIS DA REG.CENTRAL DO ESTADO ] -->
                                                                            //<!-- 2013-04-16 16:02:59,909 [1] INFO  LogInFile Consulta PtRes  [ UGE 380113 - PENITENCIARIA  DR.DANILO PINHEIRO   DE SOROCABA ] -->
                                                                            new KeyValuePair<string, string>("380113", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380113</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380113</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380510</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380511</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:03:05,915 [1] INFO  LogInFile Consulta PtRes  [ UGE 380114 - PENITENCIARIA  DR.ANTONIO DE QUEIROZ FILHO  - ITIRAPINA ] -->
                                                                            new KeyValuePair<string, string>("380114", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380114</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380114</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380510</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380511</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta><Conta><PTRES>380512</PTRES><NomePTRES>EXPANSAO E APERF. DO SISTEMA PENAL PAULISTA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:03:11,203 [1] INFO  LogInFile Consulta PtRes  [ UGE 380129 - PENITENCIARIA DR.ANTONIO DE SOUZA NETO  - SOROCABA ] -->
                                                                            new KeyValuePair<string, string>("380129", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380129</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380129</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380510</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380511</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:03:16,694 [1] INFO  LogInFile Consulta PtRes  [ UGE 380135 - PENITENCIARIA  JAIRO DE ALMEIDA BUENO  DE ITAPETININGA ] -->
                                                                            new KeyValuePair<string, string>("380135", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380135</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380135</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380510</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380511</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta><Conta><PTRES>380512</PTRES><NomePTRES>EXPANSAO E APERF. DO SISTEMA PENAL PAULISTA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:03:21,187 [1] INFO  LogInFile Consulta PtRes  [ UGE 380136 - PENITENCIARIA II DE ITAPETININGA ] -->
                                                                            new KeyValuePair<string, string>("380136", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380136</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380136</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380510</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380511</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:03:26,210 [1] INFO  LogInFile Consulta PtRes  [ UGE 380139 - CENTRO DE PROGRESS O PENITENCIARIA DE HORTOLANDIA ] -->
                                                                            new KeyValuePair<string, string>("380139", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380139</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380139</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380510</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380511</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:03:30,625 [1] INFO  LogInFile Consulta PtRes  [ UGE 380140 - PENITENCIARIA  ODETE LEITE DE CAMPOS CRITTER  - HORTOLANDIA ] -->
                                                                            new KeyValuePair<string, string>("380140", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380140</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380140</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380510</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380511</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:03:35,071 [1] INFO  LogInFile Consulta PtRes  [ UGE 380142 - CENTRO PROGRESSAO PENITENCIARIA  PROF.ATALIBA NOGUEIRA   CAMPINAS ] -->
                                                                            new KeyValuePair<string, string>("380142", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380142</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380142</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380510</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380511</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:03:39,532 [1] INFO  LogInFile Consulta PtRes  [ UGE 380147 - PENITENCIARIA FEMININA DE CAMPINAS ] -->
                                                                            new KeyValuePair<string, string>("380147", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380147</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380147</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380510</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380511</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:03:44,337 [1] INFO  LogInFile Consulta PtRes  [ UGE 380152 - PENITENCIARIA  JOAQUIM DE SYLOS CINTRA  DE CASA BRANCA ] -->
                                                                            new KeyValuePair<string, string>("380152", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380152</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380152</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380510</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380511</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:03:48,799 [1] INFO  LogInFile Consulta PtRes  [ UGE 380157 - PENITENCIARIA  ODON RAMOS MARANHAO  DE IPERO ] -->
                                                                            new KeyValuePair<string, string>("380157", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380157</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380157</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380510</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380511</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:03:53,260 [1] INFO  LogInFile Consulta PtRes  [ UGE 380159 - PENITENCIARIA  JOAO BATISTA DE ARRUDA SAMPAIO  - ITIRAPINA ] -->
                                                                            new KeyValuePair<string, string>("380159", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380159</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380159</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380510</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380511</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:03:57,721 [1] INFO  LogInFile Consulta PtRes  [ UGE 380172 - CENTRO DE DETENCAO PROVISORIA DE CAMPINAS ] -->
                                                                            new KeyValuePair<string, string>("380172", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380172</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380172</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380510</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380511</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:04:02,354 [1] INFO  LogInFile Consulta PtRes  [ UGE 380176 - CENTRO DE DETENCAO PROVISORIA  NELSON FURLAN  - PIRACICABA ] -->
                                                                            new KeyValuePair<string, string>("380176", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380176</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380176</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380510</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380511</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:04:06,785 [1] INFO  LogInFile Consulta PtRes  [ UGE 380177 - CENTRO DE DETENCAO PROVISORIA DE SOROCABA ] -->
                                                                            new KeyValuePair<string, string>("380177", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380177</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380177</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380510</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380511</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:04:11,230 [1] INFO  LogInFile Consulta PtRes  [ UGE 380190 - CENTRO DE DETENCAO PROVISORIA DE HORTOLANDIA ] -->
                                                                            new KeyValuePair<string, string>("380190", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380190</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380190</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380510</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380511</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:04:16,690 [1] INFO  LogInFile Consulta PtRes  [ UGE 380222 - CENTRO DE DETENCAO PROVISORIA DE AMERICANA ] -->
                                                                            new KeyValuePair<string, string>("380222", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380222</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380222</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380510</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380511</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta><Conta><PTRES>380512</PTRES><NomePTRES>EXPANSAO E APERF. DO SISTEMA PENAL PAULISTA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:04:21,198 [1] INFO  LogInFile Consulta PtRes  [ UGE 380239 - PENITENCIARIA I DE GUAREI ] -->
                                                                            new KeyValuePair<string, string>("380239", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380239</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380239</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380510</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380511</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:04:25,598 [1] INFO  LogInFile Consulta PtRes  [ UGE 380240 - PENITENCIARIA II DE GUAREI ] -->
                                                                            new KeyValuePair<string, string>("380240", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380240</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380240</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380510</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380511</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:04:30,246 [1] INFO  LogInFile Consulta PtRes  [ UGE 380242 - PENITENCIARIA III DE HORTOLANDIA ] -->
                                                                            new KeyValuePair<string, string>("380242", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380242</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380242</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380510</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380511</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:04:34,676 [1] INFO  LogInFile Consulta PtRes  [ UGE 380249 - CENTRO DE DETENCAO PROVISORIA DE JUNDIAI ] -->
                                                                            new KeyValuePair<string, string>("380249", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380249</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380249</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380510</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380511</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            #endregion UO 38005
                                                                            #region UO 38006
                                                                            //<!-- 2013-04-16 16:04:34,676 [1] INFO  LogInFile Consulta PtRes  [ UO 38006 - COORDENADORIA DE UNIDADES PRISIONAIS DA REGIAO NOROESTE DO ESTADO ] -->
                                                                            //<!-- 2013-04-16 16:04:40,261 [1] INFO  LogInFile Consulta PtRes  [ UGE 380112 - PENITENCIARIA  DR.PAULO LUCIANO DE CAMPOS  - AVARE ] -->
                                                                            new KeyValuePair<string, string>("380112", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380112</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380112</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380610</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380611</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta><Conta><PTRES>380612</PTRES><NomePTRES>EXPANSAO E APERF. DO SISTEMA PENAL PAULISTA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:04:44,910 [1] INFO  LogInFile Consulta PtRes  [ UGE 380116 - PENITENCIARIA  DR.SEBASTIAO MARTINS SILVEIRA  DE ARARAQUARA ] -->
                                                                            new KeyValuePair<string, string>("380116", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380116</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380116</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380610</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380611</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:04:49,371 [1] INFO  LogInFile Consulta PtRes  [ UGE 380117 - PENITENCIARIA  DR.WALTER FARIA PEREIRA DE QUEIROZ  DE PIRAJUI ] -->
                                                                            new KeyValuePair<string, string>("380117", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380117</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380117</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380610</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380611</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:04:52,803 [1] INFO  LogInFile Consulta PtRes  [ UGE 380125 - PENITENCIARIA DE MARILIA ] -->
                                                                            new KeyValuePair<string, string>("380125", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380125</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380125</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380611</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:04:57,295 [1] INFO  LogInFile Consulta PtRes  [ UGE 380133 - CENTRO DE PROGRESS O PENITENCIARIA  DR.ALBERTO BROCHIERI  - BAURU ] -->
                                                                            new KeyValuePair<string, string>("380133", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380133</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380133</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380610</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380611</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:05:01,695 [1] INFO  LogInFile Consulta PtRes  [ UGE 380134 - CENTRO PROG.PENITENCIARIA  DR.EDUARDO OLIVEIRA VIANNA   DE BAURU ] -->
                                                                            new KeyValuePair<string, string>("380134", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380134</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380134</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380610</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380611</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:05:08,059 [1] INFO  LogInFile Consulta PtRes  [ UGE 380145 - CENTRO DE PROGRESSAO PENITENCIARIA  DR.NOE DE AZEVEDO -BAURU ] -->
                                                                            new KeyValuePair<string, string>("380145", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380145</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380145</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380610</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380611</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta><Conta><PTRES>380612</PTRES><NomePTRES>EXPANSAO E APERF. DO SISTEMA PENAL PAULISTA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:05:13,566 [1] INFO  LogInFile Consulta PtRes  [ UGE 380149 - PENITENCIARIA  VALENTIM ALVES DA SILVA - ALVARO DE CARVALHO ] -->
                                                                            new KeyValuePair<string, string>("380149", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380149</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380149</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380610</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380611</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta><Conta><PTRES>380612</PTRES><NomePTRES>EXPANSAO E APERF. DO SISTEMA PENAL PAULISTA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:05:18,089 [1] INFO  LogInFile Consulta PtRes  [ UGE 380151 - PENITENCIARIA  NELSON MARCONDES DO AMARAL  - AVARE ] -->
                                                                            new KeyValuePair<string, string>("380151", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380151</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380151</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380610</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380611</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:05:23,596 [1] INFO  LogInFile Consulta PtRes  [ UGE 380155 - PENITENCIARIA  OSIRIS SOUZA E SILVA  - GETULINA ] -->
                                                                            new KeyValuePair<string, string>("380155", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380155</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380155</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380610</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380611</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta><Conta><PTRES>380612</PTRES><NomePTRES>EXPANSAO E APERF. DO SISTEMA PENAL PAULISTA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:05:28,089 [1] INFO  LogInFile Consulta PtRes  [ UGE 380156 - PENITENCIARIA  ORLANDO BRANDO FILINTO  - IARAS ] -->
                                                                            new KeyValuePair<string, string>("380156", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380156</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380156</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380610</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380611</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:05:32,566 [1] INFO  LogInFile Consulta PtRes  [ UGE 380158 - PENITENCIARIA  CABO PM MARCELO PIRES DA SILVA  - ITAI ] -->
                                                                            new KeyValuePair<string, string>("380158", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380158</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380158</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380610</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380611</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:05:37,074 [1] INFO  LogInFile Consulta PtRes  [ UGE 380164 - PENITENCIARIA  LUIZ GONZAGA VIEIRA  DE PIRAJUI ] -->
                                                                            new KeyValuePair<string, string>("380164", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380164</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380164</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380610</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380611</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:05:40,506 [1] INFO  LogInFile Consulta PtRes  [ UGE 380166 - PENITENCIARIA DE RIBEIRAO PRETO ] -->
                                                                            new KeyValuePair<string, string>("380166", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380166</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380166</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380611</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:05:45,482 [1] INFO  LogInFile Consulta PtRes  [ UGE 380191 - CENTRO DE DETENCAO PROVISORIA DE RIBEIRAO PRETO ] -->
                                                                            new KeyValuePair<string, string>("380191", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380191</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380191</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380610</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380611</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:05:49,975 [1] INFO  LogInFile Consulta PtRes  [ UGE 380197 - PENITENCIARIA I DE SERRA AZUL ] -->
                                                                            new KeyValuePair<string, string>("380197", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380197</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380197</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380610</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380611</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:05:54,873 [1] INFO  LogInFile Consulta PtRes  [ UGE 380198 - PENITENCIARIA II DE SERRA AZUL ] -->
                                                                            new KeyValuePair<string, string>("380198", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380198</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380198</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380611</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:05:59,381 [1] INFO  LogInFile Consulta PtRes  [ UGE 380215 - PENITENCIARIA FEMININA DE RIBEIRAO PRETO ] -->
                                                                            new KeyValuePair<string, string>("380215", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380215</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380215</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380610</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380611</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:06:03,921 [1] INFO  LogInFile Consulta PtRes  [ UGE 380216 - CENTRO DE DETENCAO PROVISORIA DE BAURU ] -->
                                                                            new KeyValuePair<string, string>("380216", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380216</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380216</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380610</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380611</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:06:08,975 [1] INFO  LogInFile Consulta PtRes  [ UGE 380217 - PENITENCIARIA DE AVANHANDAVA ] -->
                                                                            new KeyValuePair<string, string>("380217", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380217</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380217</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380610</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380611</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:06:13,748 [1] INFO  LogInFile Consulta PtRes  [ UGE 380223 - PENITENCIARIA  TENENTE PM JOSE ALFREDO CINTRA BORIN - REGINOPOLIS ] -->
                                                                            new KeyValuePair<string, string>("380223", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380223</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380223</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380610</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380611</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:06:18,272 [1] INFO  LogInFile Consulta PtRes  [ UGE 380224 - PENITENCIARIA  SARGENTO PM ANTONIO LUIZ DE SOUZA  DE REGINOPOLIS ] -->
                                                                            new KeyValuePair<string, string>("380224", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380224</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380224</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380610</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380611</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:06:22,702 [1] INFO  LogInFile Consulta PtRes  [ UGE 380235 - PENITENCIARIA  RODRIGO DOS SANTOS FREITAS  DE BALBINOS ] -->
                                                                            new KeyValuePair<string, string>("380235", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380235</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380235</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380610</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380611</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:06:32,327 [1] INFO  LogInFile Consulta PtRes  [ UGE 380244 - CENTRO DE DETENCAO PROVISORIA DE SERRA AZUL ] -->
                                                                            new KeyValuePair<string, string>("380244", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380244</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380244</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380610</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380611</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:06:36,773 [1] INFO  LogInFile Consulta PtRes  [ UGE 380248 - CENTRO DE DETENCAO PROVISORIA DE FRANCA ] -->
                                                                            new KeyValuePair<string, string>("380248", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380248</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380248</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380610</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380611</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            #endregion UO 38006
                                                                            #region UO 38007
                                                                            //<!-- 2013-04-16 16:06:36,788 [1] INFO  LogInFile Consulta PtRes  [ UO 38007 - COORDENADORIA DE UNIDADES PRISIONAIS DA REGIAO OESTE DO ESTADO ] -->
                                                                            //<!-- 2013-04-16 16:06:41,328 [1] INFO  LogInFile Consulta PtRes  [ UGE 380106 - CENT DE PROGRES PENITENCIARIA DR.JAVERT DE ANDRADE-S JOSE R PRETO ] -->
                                                                            new KeyValuePair<string, string>("380106", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380106</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380106</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380710</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380711</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:06:46,055 [1] INFO  LogInFile Consulta PtRes  [ UGE 380109 - PENITENCIARIA ZWINGLIO FERREIRA DE PRESIDENTE VENCESLAU ] -->
                                                                            new KeyValuePair<string, string>("380109", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380109</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380109</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380710</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380711</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:06:50,500 [1] INFO  LogInFile Consulta PtRes  [ UGE 380123 - PENITENCIARIA DE PRESIDENTE PRUDENTE  WELLINGTON RODRIGO SEGURA ] -->
                                                                            new KeyValuePair<string, string>("380123", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380123</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380123</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380710</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380711</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:06:54,962 [1] INFO  LogInFile Consulta PtRes  [ UGE 380128 - PENITENCIARIA  SILVIO YOSHIHIKO KINOHARA  - PRESIDENTE BERNARDES ] -->
                                                                            new KeyValuePair<string, string>("380128", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380128</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380128</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380710</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380711</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:06:59,376 [1] INFO  LogInFile Consulta PtRes  [ UGE 380131 - PENITENCIARIA DE ASSIS ] -->
                                                                            new KeyValuePair<string, string>("380131", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380131</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380131</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380710</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380711</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:07:03,791 [1] INFO  LogInFile Consulta PtRes  [ UGE 380137 - PENITENCIARIA  NESTOR CANOA  DE MIRANDOPOLIS ] -->
                                                                            new KeyValuePair<string, string>("380137", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380137</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380137</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380710</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380711</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:07:08,237 [1] INFO  LogInFile Consulta PtRes  [ UGE 380138 - PENITENCIARIA  ASP LINDOLFO TERCARIOL FILHO ] -->
                                                                            new KeyValuePair<string, string>("380138", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380138</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380138</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380710</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380711</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:07:13,182 [1] INFO  LogInFile Consulta PtRes  [ UGE 380150 - PENITENCIARIA DE ANDRADINA ] -->
                                                                            new KeyValuePair<string, string>("380150", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380150</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380150</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380710</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380711</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:07:17,706 [1] INFO  LogInFile Consulta PtRes  [ UGE 380160 - PENITENCIARIA DE JUNQUEIROPOLIS ] -->
                                                                            new KeyValuePair<string, string>("380160", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380160</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380160</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380710</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380711</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:07:22,120 [1] INFO  LogInFile Consulta PtRes  [ UGE 380161 - PENITENCIARIA DE LUCELIA ] -->
                                                                            new KeyValuePair<string, string>("380161", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380161</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380161</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380710</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380711</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:07:26,551 [1] INFO  LogInFile Consulta PtRes  [ UGE 380163 - PENITENCIARIA DE PACAEMBU ] -->
                                                                            new KeyValuePair<string, string>("380163", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380163</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380163</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380710</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380711</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:07:30,996 [1] INFO  LogInFile Consulta PtRes  [ UGE 380165 - PENITENCIARIA  MAURICIO HENRIQUE GUIMARAES PEREIRA -PR.VENCESLAU ] -->
                                                                            new KeyValuePair<string, string>("380165", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380165</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380165</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380710</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380711</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:07:37,002 [1] INFO  LogInFile Consulta PtRes  [ UGE 380167 - PENITENCIARIA  JOAO BATISTA DE SANTANA -RIOLANDIA ] -->
                                                                            new KeyValuePair<string, string>("380167", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380167</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380167</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380710</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380711</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:07:40,949 [1] INFO  LogInFile Consulta PtRes  [ UGE 380168 - PENITENCIARIA DE VALPARAISO ] -->
                                                                            new KeyValuePair<string, string>("380168", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380168</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380168</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380711</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:07:45,504 [1] INFO  LogInFile Consulta PtRes  [ UGE 380192 - CENTRO READAPTACAO PENIT.DR.JOSE ISMAEL PEDROSA-PRES.BERNARDES ] -->
                                                                            new KeyValuePair<string, string>("380192", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380192</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380192</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380710</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380711</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:07:49,981 [1] INFO  LogInFile Consulta PtRes  [ UGE 380199 - PENITENCIARIA  APS.ADRIANO APARECIDO DE PERI   DE DRACENA ] -->
                                                                            new KeyValuePair<string, string>("380199", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380199</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380199</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380710</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380711</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:07:56,423 [1] INFO  LogInFile Consulta PtRes  [ UGE 380200 - PENITENCIARIA DE PRACINHA ] -->
                                                                            new KeyValuePair<string, string>("380200", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380200</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380200</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380710</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380711</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:07:59,918 [1] INFO  LogInFile Consulta PtRes  [ UGE 380201 - PENITENCIARIA  VEREADOR FREDERICO GEOMETTI  DE LAVINIA ] -->
                                                                            new KeyValuePair<string, string>("380201", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380201</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380201</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380710</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380711</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:08:04,363 [1] INFO  LogInFile Consulta PtRes  [ UGE 380202 - PENITENCIARIA DE OSVALDO CRUZ ] -->
                                                                            new KeyValuePair<string, string>("380202", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380202</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380202</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380710</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380711</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:08:09,090 [1] INFO  LogInFile Consulta PtRes  [ UGE 380203 - PENITENCIARIA DE PARAGUACU PAULISTA ] -->
                                                                            new KeyValuePair<string, string>("380203", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380203</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380203</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380710</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380711</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:08:14,567 [1] INFO  LogInFile Consulta PtRes  [ UGE 380204 - CENTRO DE PROGRESSAO PENITENCIARIA DE VALPARAISO ] -->
                                                                            new KeyValuePair<string, string>("380204", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380204</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380204</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380710</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380711</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta><Conta><PTRES>380712</PTRES><NomePTRES>EXPANSAO E APERF. DO SISTEMA PENAL PAULISTA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:08:17,968 [1] INFO  LogInFile Consulta PtRes  [ UGE 380205 - CENTRO DE PROGRESSAO PENITENCIARIA DE PACAEMBU ] -->
                                                                            new KeyValuePair<string, string>("380205", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380205</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380205</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380711</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:08:22,492 [1] INFO  LogInFile Consulta PtRes  [ UGE 380211 - CENTRO DE DETENCAO PROVISORIA DE SAO JOSE DO RIO PRETO ] -->
                                                                            new KeyValuePair<string, string>("380211", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380211</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380211</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380710</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380711</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:08:27,203 [1] INFO  LogInFile Consulta PtRes  [ UGE 380228 - PENITENCIARIA  JOAO AUGUSTINHO PANUCCI  - MARABA PAULISTA ] -->
                                                                            new KeyValuePair<string, string>("380228", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380228</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380228</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380710</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380711</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:08:30,619 [1] INFO  LogInFile Consulta PtRes  [ UGE 380229 - PENITENCIARIA DE FLORIDA PAULISTA ] -->
                                                                            new KeyValuePair<string, string>("380229", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380229</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380229</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380711</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:08:35,377 [1] INFO  LogInFile Consulta PtRes  [ UGE 380230 - PENITENCIARIA DE IRAPURU ] -->
                                                                            new KeyValuePair<string, string>("380230", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380230</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380230</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380710</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380711</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:08:39,854 [1] INFO  LogInFile Consulta PtRes  [ UGE 380231 - PENITENCIARIA DE TUPI PAULISTA ] -->
                                                                            new KeyValuePair<string, string>("380231", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380231</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380231</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380710</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380711</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:08:44,284 [1] INFO  LogInFile Consulta PtRes  [ UGE 380232 - CENTRO DE DETENCAO PROVISORIA TACIO APARECIDO SANTANA - CAIUA ] -->
                                                                            new KeyValuePair<string, string>("380232", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380232</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380232</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380710</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380711</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:08:49,432 [1] INFO  LogInFile Consulta PtRes  [ UGE 380237 - PENITENCIARIA  LUIS APARECIDO FERNANDES  DE LAVINIA II ] -->
                                                                            new KeyValuePair<string, string>("380237", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380237</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380237</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380710</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380711</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:08:56,904 [1] INFO  LogInFile Consulta PtRes  [ UGE 380238 - PENITENCIARIA  AG.SEG. PENITENCIARIA PAULO GUIMARAES -LAVINIA ] -->
                                                                            new KeyValuePair<string, string>("380238", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380238</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380238</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380710</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380711</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:09:01,366 [1] INFO  LogInFile Consulta PtRes  [ UGE 380251 - PENITENCIARIA FEMININA DE TUPI PAULISTA ] -->
                                                                            new KeyValuePair<string, string>("380251", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380251</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380251</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380710</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380711</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:09:05,890 [1] INFO  LogInFile Consulta PtRes  [ UGE 380119 - HOSPITAL CUST.TRAT.PSIQU. PROF.ANDRE TEIXEIRA LIMA -FRANCO ROCHA ] -->
                                                                            new KeyValuePair<string, string>("380119", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380119</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380119</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380810</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta><Conta><PTRES>380811</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:09:10,429 [1] INFO  LogInFile Consulta PtRes  [ UGE 380208 - HOSPITAL DE CUSTODIA E TRATAMENTO PSIQUIATRICO II FRANCO DA ROCHA ] -->
                                                                            new KeyValuePair<string, string>("380208", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380208</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380208</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380810</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta><Conta><PTRES>380811</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>"),
                                                                            //<!-- 2013-04-16 16:09:15,046 [1] INFO  LogInFile Consulta PtRes  [ UGE 380210 - CTO.DETEN.PROV.AG.PENT.JOAQUIM FONSECA LOPES ] -->
                                                                            new KeyValuePair<string, string>("380210", "<MSG><BCMSG><Doc_Estimulo><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><documento><UG>380210</UG><Gestao>00001</Gestao><Mes>SET</Mes><DiaMesInicial>01SET</DiaMesInicial><DiaMesFinal>30SET</DiaMesFinal></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Estimulo></BCMSG><SISERRO><Doc_Retorno><SIAFDOC><cdMsg>SIAFDetPTRES</cdMsg><SiafemDocDetPTRES><UG>380210</UG><Gestao>00001</Gestao><ContaContabil>292110000</ContaContabil><StatusOperacao>true</StatusOperacao><MsgRetorno></MsgRetorno><documento><RepeteContas><Conta><PTRES>380310</PTRES><NomePTRES>ATENCAO INTEGRAL A SAUDE POPULACAO PRISIONAL</NomePTRES></Conta><Conta><PTRES>380311</PTRES><NomePTRES>GERENC.SUPORTE NECESSIDADES BASICAS POP.PENA</NomePTRES></Conta></RepeteContas></documento></SiafemDocDetPTRES></SIAFDOC></Doc_Retorno></SISERRO></MSG>") 
                                                                            #endregion UO 38007
        
            };

            catalogoChaveRetorno = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            arrChaveUGEeRetornoWS.Cast<KeyValuePair<string, string>>().ToList().ForEach(_parUgeRetorno =>
            {
                if (!catalogoChaveRetorno.ContainsKey(_parUgeRetorno.Key))
                    catalogoChaveRetorno.Add(_parUgeRetorno.Key, _parUgeRetorno.Value);
            });


            if (catalogoChaveRetorno.ContainsKey(strCodigoUGE))
                strRetorno = catalogoChaveRetorno[strCodigoUGE];

            return strRetorno;
        }

        public static string gerarSFCONLPregao_UGE_380149_GESTAO_00001_NUMERONE_2014NE00024()
        {
            string strRetorno = string.Empty;
            StringBuilder sbXml = new StringBuilder();

            sbXml.AppendLine("<SFCODOC>");
            sbXml.AppendLine("    <cdMsg>SFCONLPregao</cdMsg>");
            sbXml.AppendLine("    <SFCONLPregao>");
            sbXml.AppendLine("        <documento>");
            sbXml.AppendLine("            <DataEmissao>30NOV2014</DataEmissao>");
            sbXml.AppendLine("            <UnidadeGestora>380149</UnidadeGestora>");
            sbXml.AppendLine("            <Gestao>00001</Gestao>");
            sbXml.AppendLine("            <EmpenhoOriginal>00024</EmpenhoOriginal>");
            sbXml.AppendLine("            <ContratoOriginal></ContratoOriginal>");
            sbXml.AppendLine("            <EVENTOSERVICOGERAL></EVENTOSERVICOGERAL>");
            sbXml.AppendLine("            <EVENTOSEGUROSEMGERAL></EVENTOSEGUROSEMGERAL>");
            sbXml.AppendLine("            <EVENTOMATERIALDECONSUMO>x</EVENTOMATERIALDECONSUMO>");
            sbXml.AppendLine("            <EVENTOMATERIALPERMANENTE></EVENTOMATERIALPERMANENTE>");
            sbXml.AppendLine("            <EVENTOATIVINDUSTRIALMATERIAPRIMA></EVENTOATIVINDUSTRIALMATERIAPRIMA>");
            sbXml.AppendLine("            <EVENTOATIVINDUSTRIALMATEMBALAGEM></EVENTOATIVINDUSTRIALMATEMBALAGEM>");
            sbXml.AppendLine("            <repeticaoItem>");
            sbXml.AppendLine("                <linha>");
            sbXml.AppendLine("                    <Item>00146819-7</Item>");
            sbXml.AppendLine("                    <UnidForn>00003</UnidForn>");
            sbXml.AppendLine("                    <QtdInteiro>2890</QtdInteiro>");
            sbXml.AppendLine("                    <QtdDecimal>000</QtdDecimal>");
            sbXml.AppendLine("                </linha>");
            sbXml.AppendLine("                <linha>");
            sbXml.AppendLine("                    <Item>00146819-7</Item>");
            sbXml.AppendLine("                    <UnidForn>00003</UnidForn>");
            sbXml.AppendLine("                    <QtdInteiro>1679</QtdInteiro>");
            sbXml.AppendLine("                    <QtdDecimal>000</QtdDecimal>");
            sbXml.AppendLine("                </linha>");
            sbXml.AppendLine("                <linha>");
            sbXml.AppendLine("                    <Item>00146819-7</Item>");
            sbXml.AppendLine("                    <UnidForn>00003</UnidForn>");
            sbXml.AppendLine("                    <QtdInteiro>1331</QtdInteiro>");
            sbXml.AppendLine("                    <QtdDecimal>000</QtdDecimal>");
            sbXml.AppendLine("                </linha>");
            sbXml.AppendLine("            </repeticaoItem>");
            sbXml.AppendLine("            <Observacao>Liq. Empenho PREGAO; Chaves SAM: 11111111111; SIAFEM: 11111111111</Observacao>");
            sbXml.AppendLine("            <repeticaoNf>");
            sbXml.AppendLine("                <NotaFiscal>201402411002</NotaFiscal>");
            sbXml.AppendLine("            </repeticaoNf>");
            sbXml.AppendLine("        </documento>");
            sbXml.AppendLine("    </SFCONLPregao>");
            sbXml.AppendLine("</SFCODOC>");

            strRetorno = sbXml.ToString();
            return strRetorno;
        }
        public static string gerarSFCOLiqNLBec_UGE_380149_GESTAO_00001_NUMERONE_2014NE00727()
        {
            string strRetorno = string.Empty;
            StringBuilder sbXml = new StringBuilder();

            sbXml.AppendLine("<SFCODOC>");
            sbXml.AppendLine("    <cdMsg>SFCOLiqNLBec</cdMsg>");
            sbXml.AppendLine("    <SFCOLiqNLBec>");
            sbXml.AppendLine("        <documento>");
            sbXml.AppendLine("            <DataEmissao>30NOV2014</DataEmissao>");
            sbXml.AppendLine("            <UnidadeGestora>380149</UnidadeGestora>");
            sbXml.AppendLine("            <Gestao>00001</Gestao>");
            sbXml.AppendLine("            <EmpenhoOriginal>00727</EmpenhoOriginal>");
            sbXml.AppendLine("            <ContratoOriginal></ContratoOriginal>");
            sbXml.AppendLine("            <SERVICOSEMGERAL></SERVICOSEMGERAL>");
            sbXml.AppendLine("            <SEGUROSEMGERAL></SEGUROSEMGERAL>");
            sbXml.AppendLine("            <MATERIALDECONSUMO>x</MATERIALDECONSUMO>");
            sbXml.AppendLine("            <MATERIALPERMANENTE></MATERIALPERMANENTE>");
            sbXml.AppendLine("            <ALUGUEIS></ALUGUEIS>");
            sbXml.AppendLine("            <IMPORTACAODEMATCONSUMO></IMPORTACAODEMATCONSUMO>");
            sbXml.AppendLine("            <IMPORTACAODEMATPERMANENTE></IMPORTACAODEMATPERMANENTE>");
            sbXml.AppendLine("            <ATIVINDUSTRIALMATERIAPRIMA></ATIVINDUSTRIALMATERIAPRIMA>");
            sbXml.AppendLine("            <repeticaoItem>");
            sbXml.AppendLine("                <linha>");
            sbXml.AppendLine("                    <Item>00138934-3</Item>");
            sbXml.AppendLine("                    <UnidForn>00001</UnidForn>");
            sbXml.AppendLine("                    <QtdInteiro>4</QtdInteiro>");
            sbXml.AppendLine("                    <QtdDecimal>000</QtdDecimal>");
            sbXml.AppendLine("                </linha>");
            sbXml.AppendLine("                <linha>");
            sbXml.AppendLine("                    <Item>00138773-1</Item>");
            sbXml.AppendLine("                    <UnidForn>00001</UnidForn>");
            sbXml.AppendLine("                    <QtdInteiro>5</QtdInteiro>");
            sbXml.AppendLine("                    <QtdDecimal>000</QtdDecimal>");
            sbXml.AppendLine("                </linha>");
            sbXml.AppendLine("                <linha>");
            sbXml.AppendLine("                    <Item>00138999-8</Item>");
            sbXml.AppendLine("                    <UnidForn>00001</UnidForn>");
            sbXml.AppendLine("                    <QtdInteiro>10</QtdInteiro>");
            sbXml.AppendLine("                    <QtdDecimal>000</QtdDecimal>");
            sbXml.AppendLine("                </linha>");
            sbXml.AppendLine("            </repeticaoItem>");
            sbXml.AppendLine("            <Observacao>Liq. Empenho BEC; Chaves SAM: 11111111111; SIAFEM: 11111111111</Observacao>");
            sbXml.AppendLine("            <repeticaoNf>");
            sbXml.AppendLine("                <NotaFiscal>201400067766</NotaFiscal>");
            sbXml.AppendLine("            </repeticaoNf>");
            sbXml.AppendLine("        </documento>");
            sbXml.AppendLine("    </SFCOLiqNLBec>");
            sbXml.AppendLine("</SFCODOC>");

            strRetorno = sbXml.ToString();
            return strRetorno;
        }
        public static string SFCOLiquidaNL_UGE_380149_GESTAO_00001_NUMERONE_2014NE00731()
        {
            string strRetorno = string.Empty;
            StringBuilder sbXml = new StringBuilder();

            sbXml.AppendLine("<MSG>");
            sbXml.AppendLine("    <BCMSG>");
            sbXml.AppendLine("        <Doc_Estimulo>");
            sbXml.AppendLine("            <SFCODOC>");
            sbXml.AppendLine("                <cdMsg>SFCOLiquidaNL</cdMsg>");
            sbXml.AppendLine("                <SFCOLiquidaNL>");
            sbXml.AppendLine("                    <documento>");
            sbXml.AppendLine("                        <DataEmissao>30NOV2014</DataEmissao>");
            sbXml.AppendLine("                        <UnidadeGestora>380149</UnidadeGestora>");
            sbXml.AppendLine("                        <Gestao>00001</Gestao>");
            sbXml.AppendLine("                        <EmpenhoOriginal>00731</EmpenhoOriginal>");
            sbXml.AppendLine("                        <ContratoOriginal />");
            sbXml.AppendLine("                        <SERVICOSEMGERAL></SERVICOSEMGERAL>");
            sbXml.AppendLine("                        <SEGUROSEMGERAL></SEGUROSEMGERAL>");
            sbXml.AppendLine("                        <MATERIALDECONSUMO>x</MATERIALDECONSUMO>");
            sbXml.AppendLine("                        <MATERIALPERMANENTE></MATERIALPERMANENTE>");
            sbXml.AppendLine("                        <ALUGUEIS></ALUGUEIS>");
            sbXml.AppendLine("                        <IMPORTACAODEMATCONSUMO></IMPORTACAODEMATCONSUMO>");
            sbXml.AppendLine("                        <IMPORTACAODEMATPERMANENTE></IMPORTACAODEMATPERMANENTE>");
            sbXml.AppendLine("                        <ATIVINDUSTRIALMATERIAPRIMA></ATIVINDUSTRIALMATERIAPRIMA>");
            sbXml.AppendLine("                        <ATIVINDUSTRIALMATEMBALAGEM></ATIVINDUSTRIALMATEMBALAGEM>");
            sbXml.AppendLine("                        <repeticaoItem>");
            sbXml.AppendLine("                            <linha>");
            sbXml.AppendLine("                                <Item>00202304-0</Item>");
            sbXml.AppendLine("                                <UnidForn>00001</UnidForn>");
            sbXml.AppendLine("                                <QtdInteiro>7</QtdInteiro>");
            sbXml.AppendLine("                                <QtdDecimal>000</QtdDecimal>");
            sbXml.AppendLine("                            </linha>");
            sbXml.AppendLine("                            <linha>");
            sbXml.AppendLine("                                <Item>00425177-6</Item>");
            sbXml.AppendLine("                                <UnidForn>00001</UnidForn>");
            sbXml.AppendLine("                                <QtdInteiro>10</QtdInteiro>");
            sbXml.AppendLine("                                <QtdDecimal>000</QtdDecimal>");
            sbXml.AppendLine("                            </linha>");
            sbXml.AppendLine("                            <linha>");
            sbXml.AppendLine("                                <Item>00240314-5</Item>");
            sbXml.AppendLine("                                <UnidForn>00001</UnidForn>");
            sbXml.AppendLine("                                <QtdInteiro>1</QtdInteiro>");
            sbXml.AppendLine("                                <QtdDecimal>000</QtdDecimal>");
            sbXml.AppendLine("                            </linha>");
            sbXml.AppendLine("                            <linha>");
            sbXml.AppendLine("                                <Item>00240315-3</Item>");
            sbXml.AppendLine("                                <UnidForn>00001</UnidForn>");
            sbXml.AppendLine("                                <QtdInteiro>1</QtdInteiro>");
            sbXml.AppendLine("                                <QtdDecimal>000</QtdDecimal>");
            sbXml.AppendLine("                            </linha>");
            sbXml.AppendLine("                            <linha>");
            sbXml.AppendLine("                                <Item>00245330-4</Item>");
            sbXml.AppendLine("                                <UnidForn>00001</UnidForn>");
            sbXml.AppendLine("                                <QtdInteiro>5</QtdInteiro>");
            sbXml.AppendLine("                                <QtdDecimal>000</QtdDecimal>");
            sbXml.AppendLine("                            </linha>");
            sbXml.AppendLine("                            <linha>");
            sbXml.AppendLine("                                <Item>00218671-3</Item>");
            sbXml.AppendLine("                                <UnidForn>00001</UnidForn>");
            sbXml.AppendLine("                                <QtdInteiro>5</QtdInteiro>");
            sbXml.AppendLine("                                <QtdDecimal>000</QtdDecimal>");
            sbXml.AppendLine("                            </linha>");
            sbXml.AppendLine("                            <linha>");
            sbXml.AppendLine("                                <Item>00220727-3</Item>");
            sbXml.AppendLine("                                <UnidForn>00001</UnidForn>");
            sbXml.AppendLine("                                <QtdInteiro>3</QtdInteiro>");
            sbXml.AppendLine("                                <QtdDecimal>000</QtdDecimal>");
            sbXml.AppendLine("                            </linha>");
            sbXml.AppendLine("                            <linha>");
            sbXml.AppendLine("                                <Item>00337952-3</Item>");
            sbXml.AppendLine("                                <UnidForn>00001</UnidForn>");
            sbXml.AppendLine("                                <QtdInteiro>3</QtdInteiro>");
            sbXml.AppendLine("                                <QtdDecimal>000</QtdDecimal>");
            sbXml.AppendLine("                            </linha>");
            sbXml.AppendLine("                        </repeticaoItem>");
            sbXml.AppendLine("                        <Observacao>Liquidação Empenho SIAFISICO (SAM) Chave SAM 99999999999 Chave SIAFEM 99999999999</Observacao>");
            sbXml.AppendLine("                        <repeticaoNf>");
            sbXml.AppendLine("                            <NotaFiscal>201400000835</NotaFiscal>");
            sbXml.AppendLine("                        </repeticaoNf>");
            sbXml.AppendLine("                    </documento>");
            sbXml.AppendLine("                </SFCOLiquidaNL>");
            sbXml.AppendLine("            </SFCODOC>");
            sbXml.AppendLine("        </Doc_Estimulo>");
            sbXml.AppendLine("    </BCMSG>");
            sbXml.AppendLine("    <SISERRO>");
            sbXml.AppendLine("        <Doc_Retorno>");
            sbXml.AppendLine("            <SFCODOC>");
            sbXml.AppendLine("                <SiafisicoDocLiquidaNL>");
            sbXml.AppendLine("                    <cdMsg>SFCOLiquidaNL</cdMsg>");
            sbXml.AppendLine("                    <StatusOperacao>false</StatusOperacao>");
            sbXml.AppendLine("                    <Msg> (0034) EMPENHO INEXISTENTE </Msg>");
            sbXml.AppendLine("                    <ItemNaoEncontrado></ItemNaoEncontrado>");
            sbXml.AppendLine("                </SiafisicoDocLiquidaNL>");
            sbXml.AppendLine("            </SFCODOC>");
            sbXml.AppendLine("        </Doc_Retorno>");
            sbXml.AppendLine("    </SISERRO>");
            sbXml.AppendLine("</MSG>");

            strRetorno = sbXml.ToString();
            return strRetorno;
        }
        public static string SFCOLiquidaNL_UGE_200119_GESTAO_00001_NUMERONE_2014NE00156__NUMERO_NL_2014NL00450()
        {
            string strRetorno = string.Empty;
            StringBuilder sbXml = new StringBuilder();

            sbXml.AppendLine("<MSG>");
            sbXml.AppendLine("    <BCMSG>");
            sbXml.AppendLine("        <Doc_Estimulo>");
            sbXml.AppendLine("            <SFCODOC>");
            sbXml.AppendLine("                <cdMsg>SFCOLiquidaNL</cdMsg>");
            sbXml.AppendLine("                <SFCOLiquidaNL>");
            sbXml.AppendLine("                    <documento>");
            sbXml.AppendLine("                        <DataEmissao>23MAI2014</DataEmissao>");
            sbXml.AppendLine("                        <UnidadeGestora>200119</UnidadeGestora>");
            sbXml.AppendLine("                        <Gestao>00001</Gestao>");
            sbXml.AppendLine("                        <EmpenhoOriginal>00156</EmpenhoOriginal>");
            sbXml.AppendLine("                        <ContratoOriginal></ContratoOriginal>");
            sbXml.AppendLine("                        <SERVICOSEMGERAL></SERVICOSEMGERAL>");
            sbXml.AppendLine("                        <SEGUROSEMGERAL></SEGUROSEMGERAL>");
            sbXml.AppendLine("                        <MATERIALDECONSUMO>x</MATERIALDECONSUMO>");
            sbXml.AppendLine("                        <MATERIALPERMANENTE></MATERIALPERMANENTE>");
            sbXml.AppendLine("                        <ALUGUEIS></ALUGUEIS>");
            sbXml.AppendLine("                        <IMPORTACAODEMATCONSUMO></IMPORTACAODEMATCONSUMO>");
            sbXml.AppendLine("                        <IMPORTACAODEMATPERMANENTE></IMPORTACAODEMATPERMANENTE>");
            sbXml.AppendLine("                        <ATIVINDUSTRIALMATERIAPRIMA></ATIVINDUSTRIALMATERIAPRIMA>");
            sbXml.AppendLine("                        <ATIVINDUSTRIALMATEMBALAGEM></ATIVINDUSTRIALMATEMBALAGEM>");
            sbXml.AppendLine("                        <repeticaoItem>");
            sbXml.AppendLine("                            <linha>");
            sbXml.AppendLine("                                <Item>00000804-4</Item>");
            sbXml.AppendLine("                                <UnidForn>00001</UnidForn>");
            sbXml.AppendLine("                                <QtdInteiro>1</QtdInteiro>");
            sbXml.AppendLine("                                <QtdDecimal>000</QtdDecimal>");
            sbXml.AppendLine("                            </linha>");
            sbXml.AppendLine("                        </repeticaoItem>");
            sbXml.AppendLine("                        <Observacao>Obs teste l</Observacao>");
            sbXml.AppendLine("                        <repeticaoNf>");
            sbXml.AppendLine("                            <NotaFiscal>201400454654</NotaFiscal>");
            sbXml.AppendLine("                            <NotaFiscal></NotaFiscal>");
            sbXml.AppendLine("                            <NotaFiscal></NotaFiscal>");
            sbXml.AppendLine("                            <NotaFiscal></NotaFiscal>");
            sbXml.AppendLine("                            <NotaFiscal></NotaFiscal>");
            sbXml.AppendLine("                            <NotaFiscal></NotaFiscal>");
            sbXml.AppendLine("                            <NotaFiscal></NotaFiscal>");
            sbXml.AppendLine("                            <NotaFiscal></NotaFiscal>");
            sbXml.AppendLine("                            <NotaFiscal></NotaFiscal>");
            sbXml.AppendLine("                            <NotaFiscal></NotaFiscal>");
            sbXml.AppendLine("                            <NotaFiscal></NotaFiscal>");
            sbXml.AppendLine("                            <NotaFiscal></NotaFiscal>");
            sbXml.AppendLine("                            <NotaFiscal></NotaFiscal>");
            sbXml.AppendLine("                            <NotaFiscal></NotaFiscal>");
            sbXml.AppendLine("                        </repeticaoNf>");
            sbXml.AppendLine("                    </documento>");
            sbXml.AppendLine("                </SFCOLiquidaNL>");
            sbXml.AppendLine("            </SFCODOC>");
            sbXml.AppendLine("        </Doc_Estimulo>");
            sbXml.AppendLine("    </BCMSG>");
            sbXml.AppendLine("    <SISERRO>");
            sbXml.AppendLine("        <Doc_Retorno>");
            sbXml.AppendLine("            <SFCODOC>");
            sbXml.AppendLine("                <SiafisicoDocLiquidaNL>");
            sbXml.AppendLine("                    <cdMsg>SFCOLiquidaNL</cdMsg>");
            sbXml.AppendLine("                    <StatusOperacao>true</StatusOperacao>");
            sbXml.AppendLine("                    <Msg>Liquidacao realizada com sucesso</Msg>");
            sbXml.AppendLine("                    <documento>");
            sbXml.AppendLine("                        <UG>200119</UG>");
            sbXml.AppendLine("                        <Gestao>00001</Gestao>");
            sbXml.AppendLine("                        <NL>2014NL00450</NL>");
            sbXml.AppendLine("                        <NE>2014NE00156</NE>");
            sbXml.AppendLine("                        <CT>2014CT00100</CT>");
            sbXml.AppendLine("                    </documento>");
            sbXml.AppendLine("                </SiafisicoDocLiquidaNL>");
            sbXml.AppendLine("            </SFCODOC>");
            sbXml.AppendLine("        </Doc_Retorno>");
            sbXml.AppendLine("    </SISERRO>");
            sbXml.AppendLine("</MSG>");

            strRetorno = sbXml.ToString();
            return strRetorno;
        }
        public static string SFCOLiquidaNL_UGE_380101_GESTAO_00001_NUMERONE_2015NE00003__NUMERO_NL_2015NL00008()
        {
            string strRetorno = string.Empty;
            StringBuilder sbXml = new StringBuilder();

            sbXml.AppendLine("<MSG>");
            sbXml.AppendLine("    <BCMSG>");
            sbXml.AppendLine("        <Doc_Estimulo>");
            sbXml.AppendLine("            <SFCODOC>");
            sbXml.AppendLine("                <cdMsg>SFCOLiquidaNL</cdMsg>");
            sbXml.AppendLine("                <SFCOLiquidaNL>");
            sbXml.AppendLine("                    <documento>");
            sbXml.AppendLine("                        <DataEmissao>28JAN2015</DataEmissao>");
            sbXml.AppendLine("                        <UnidadeGestora>380101</UnidadeGestora>");
            sbXml.AppendLine("                        <Gestao>00001</Gestao>");
            sbXml.AppendLine("                        <EmpenhoOriginal>00003</EmpenhoOriginal>");
            sbXml.AppendLine("                        <ContratoOriginal></ContratoOriginal>");
            sbXml.AppendLine("                        <SERVICOSEMGERAL></SERVICOSEMGERAL>");
            sbXml.AppendLine("                        <SEGUROSEMGERAL></SEGUROSEMGERAL>");
            sbXml.AppendLine("                        <MATERIALDECONSUMO>x</MATERIALDECONSUMO>");
            sbXml.AppendLine("                        <MATERIALPERMANENTE></MATERIALPERMANENTE>");
            sbXml.AppendLine("                        <ALUGUEIS></ALUGUEIS>");
            sbXml.AppendLine("                        <IMPORTACAODEMATCONSUMO></IMPORTACAODEMATCONSUMO>");
            sbXml.AppendLine("                        <IMPORTACAODEMATPERMANENTE></IMPORTACAODEMATPERMANENTE>");
            sbXml.AppendLine("                        <ATIVINDUSTRIALMATERIAPRIMA></ATIVINDUSTRIALMATERIAPRIMA>");
            sbXml.AppendLine("                        <ATIVINDUSTRIALMATEMBALAGEM></ATIVINDUSTRIALMATEMBALAGEM>");
            sbXml.AppendLine("                        <repeticaoItem>");
            sbXml.AppendLine("                            <linha>");
            sbXml.AppendLine("                                <Item>00013197-0</Item>");
            sbXml.AppendLine("                                <UnidForn>00001</UnidForn>");
            sbXml.AppendLine("                                <QtdInteiro>10</QtdInteiro>");
            sbXml.AppendLine("                                <QtdDecimal>000</QtdDecimal>");
            sbXml.AppendLine("                            </linha>");
            sbXml.AppendLine("                            <linha>");
            sbXml.AppendLine("                                <Item>00013563-1</Item>");
            sbXml.AppendLine("                                <UnidForn>00001</UnidForn>");
            sbXml.AppendLine("                                <QtdInteiro>10</QtdInteiro>");
            sbXml.AppendLine("                                <QtdDecimal>000</QtdDecimal>");
            sbXml.AppendLine("                            </linha>");
            sbXml.AppendLine("                        </repeticaoItem>");
            sbXml.AppendLine("                        <Observacao>eee</Observacao>");
            sbXml.AppendLine("                        <repeticaoNf>");
            sbXml.AppendLine("                            <NotaFiscal>201599999996</NotaFiscal>");
            sbXml.AppendLine("                        </repeticaoNf>");
            sbXml.AppendLine("                    </documento>");
            sbXml.AppendLine("                </SFCOLiquidaNL>");
            sbXml.AppendLine("            </SFCODOC>");
            sbXml.AppendLine("        </Doc_Estimulo>");
            sbXml.AppendLine("    </BCMSG>");
            sbXml.AppendLine("    <SISERRO>");
            sbXml.AppendLine("        <Doc_Retorno>");
            sbXml.AppendLine("            <SFCODOC>");
            sbXml.AppendLine("                <SiafisicoDocLiquidaNL>");
            sbXml.AppendLine("                    <cdMsg>SFCOLiquidaNL</cdMsg>");
            sbXml.AppendLine("                    <StatusOperacao>true</StatusOperacao>");
            sbXml.AppendLine("                    <Msg>Liquidacao realizada com sucesso</Msg>");
            sbXml.AppendLine("                    <documento>");
            sbXml.AppendLine("                        <NL>2015NL00008</NL>");
            sbXml.AppendLine("                        <NE>2015NE00003</NE>");
            sbXml.AppendLine("                        <CT>2015CT00003</CT>");
            sbXml.AppendLine("                        <OC></OC>");
            sbXml.AppendLine("                    </documento>");
            sbXml.AppendLine("                    <ItemNaoEncontrado>");
            sbXml.AppendLine("                    </ItemNaoEncontrado>");
            sbXml.AppendLine("                </SiafisicoDocLiquidaNL>");
            sbXml.AppendLine("            </SFCODOC>");
            sbXml.AppendLine("        </Doc_Retorno>");
            sbXml.AppendLine("    </SISERRO>");
            sbXml.AppendLine("</MSG>");

            strRetorno = sbXml.ToString();
            return strRetorno;
        }

        public static string obter_SFCONLPregao_SFCOLiqNLBec_SFCOLiquidaNL_OpFlex(string msgEstimulo)
        {
            string strRetono = null;

            var docXml = new System.Xml.XmlDocument();
            docXml.LoadXml(msgEstimulo);

            string nomeTransacaoSiafem = Sam.Common.XmlUtil.lerXml(msgEstimulo, "/SFCODOC/cdMsg").InnerText;
            string nomeWSTransacaoSIAFEM = docXml.GetElementsByTagName("cdMsg").Cast<System.Xml.XmlNode>().FirstOrDefault().NextSibling.Name;
            string ugeGeradoraEstimulo = Sam.Common.XmlUtil.lerXml(msgEstimulo, "/SFCODOC/*/documento/UnidadeGestora").InnerText;
            string empenhoSendoPago = Sam.Common.XmlUtil.lerXml(msgEstimulo, "/SFCODOC/*/documento/EmpenhoOriginal").InnerText;


            string nlLiquidacaoFake = String.Format("{0}NL{1:D5}", DateTime.Now.Year, (new Random()).Next(0, 10000));
            string ctLiquidacaoFake = String.Format("{0}CT{1:D5}", DateTime.Now.Year, (new Random()).Next(0, 10000));

            docXml = null;

            StringBuilder sbXml = new StringBuilder();

            sbXml.AppendLine("<MSG>");
            sbXml.AppendLine(" <BCMSG>");
            sbXml.AppendLine("     <Doc_Estimulo>");
            sbXml.AppendLine(msgEstimulo);
            sbXml.AppendLine("     </Doc_Estimulo>");
            sbXml.AppendLine(" </BCMSG>");
            sbXml.AppendLine(" <SISERRO>");
            sbXml.AppendLine("     <Doc_Retorno>");
            sbXml.AppendLine("         <SFCODOC>");
            sbXml.AppendLine(String.Format("             <{0}>{1}", nomeTransacaoSiafem, Environment.NewLine));
            sbXml.AppendLine(String.Format("             <cdMsg>{0}</cdMsg>", nomeWSTransacaoSIAFEM));
            sbXml.AppendLine("                    <StatusOperacao>true</StatusOperacao>");
            sbXml.AppendLine("                    <Msg>Liquidacao realizada com sucesso</Msg>");
            sbXml.AppendLine("             <documento>");
            sbXml.AppendLine(String.Format("                   <UG>{0}</UG>", ugeGeradoraEstimulo));
            sbXml.AppendLine("                        <Gestao>00001</Gestao>");
            sbXml.AppendLine(String.Format("                   <NL>{0}</NL>", nlLiquidacaoFake));
            sbXml.AppendLine(String.Format("                   <NE>{0}</NE>", empenhoSendoPago));
            sbXml.AppendLine(String.Format("                   <CT>{0}</CT>", ctLiquidacaoFake));
            sbXml.AppendLine("             <MsgErro></MsgErro>");
            sbXml.AppendLine("             </documento>");
            sbXml.AppendLine(String.Format("             </{0}>{1}", nomeTransacaoSiafem, Environment.NewLine));
            sbXml.AppendLine("         </SFCODOC>");
            sbXml.AppendLine("     </Doc_Retorno>");
            sbXml.AppendLine("	</SISERRO>");
            sbXml.AppendLine("</MSG>");

            strRetono = sbXml.ToString();
            strRetono = XmlUtil.IndentarXml(strRetono);

            return strRetono;
        }

        public static string gerarSIAFNLEmLiq_SIAFNL_OpFlex(bool gerarNLEmLiq)
        {
            string strRetono = null;
            string codigoMsgSIAF = null;
            string nomeMsgSIAF = null;

            string nlLiquidacaoFake = String.Format("{0}NL{1:D5}", DateTime.Now.Year, (new Random()).Next(0, 10000));
            StringBuilder sbXml = new StringBuilder();

            if(gerarNLEmLiq)
            {
                nomeMsgSIAF = "SiafemDocNL";
                codigoMsgSIAF = "SIAFNL001";
            }
            else
            {
                nomeMsgSIAF = "SiafemDocNLEmLiq";
                codigoMsgSIAF = "SIAFNLEmLiq";
            }

            /*
                <SIAFDOC>
                 <cdMsg>SIAFNL001</cdMsg>
                 <SiafemDocNL>
                  <documento>
                    <NumeroNL>2009NL04276</NumeroNL>
                    <MsgErro></MsgErro>
                  </documento>
                 </SiafemDocNL>
                </SIAFDOC>

              <SIAFDOC>
	            <cdMsg>SIAFNLEmLiq</cdMsg>
	            <SiafemDocNLEmLiq>
		            <documento>
			            <NumeroNL>2015NL00173</NumeroNL>
			            <MsgErro></MsgErro>
		            </documento>
	            </SiafemDocNLEmLiq>
            </SIAFDOC>



            <SIAFDOC>
	            <cdMsg>SIAFNLEmLiq</cdMsg>
	            <SiafemDocNLEmLiq>
		            <documento>
			            <NumeroNL></NumeroNL>
			            <MsgErro>(0017) VALOR INVALIDO</MsgErro>
		            </documento>
	            </SiafemDocNLEmLiq>
            </SIAFDOC>

             
             */
            sbXml.AppendLine("<SIAFDOC>");
            sbXml.AppendLine(String.Format("<cdMsg>{0}</cdMsg>", codigoMsgSIAF));
            sbXml.AppendLine(String.Format("<{0}>", nomeMsgSIAF));
            sbXml.AppendLine("<documento>");
            sbXml.AppendLine(String.Format("<NumeroNL>{0}</NumeroNL>", nlLiquidacaoFake));
            sbXml.AppendLine("<MsgErro></MsgErro>");
            sbXml.AppendLine("</documento>");
            sbXml.AppendLine(String.Format("</{0}>", nomeMsgSIAF));
            sbXml.AppendLine("</SIAFDOC>");

            strRetono = sbXml.ToString();
            strRetono = XmlUtil.IndentarXml(strRetono);

            return strRetono;
        }

        public static string errRetornoVazio()
        {
            StringBuilder sbXml = new StringBuilder();
            string strRetorno = null;

            sbXml.AppendLine("<MSG>");
            sbXml.AppendLine("    <BCMSG>");
            sbXml.AppendLine("        <Doc_Estimulo>");
            sbXml.AppendLine("            <SFCODOC>");
            sbXml.AppendLine("                <cdMsg>SFCOLiquidaNL</cdMsg>");
            sbXml.AppendLine("                <SFCOLiquidaNL>");
            sbXml.AppendLine("                    <documento>");
            sbXml.AppendLine("                        <DataEmissao>15JAN2015</DataEmissao>");
            sbXml.AppendLine("                        <UnidadeGestora>380193</UnidadeGestora>");
            sbXml.AppendLine("                        <Gestao>00001</Gestao>");
            sbXml.AppendLine("                        <EmpenhoOriginal>00002</EmpenhoOriginal>");
            sbXml.AppendLine("                        <ContratoOriginal></ContratoOriginal>");
            sbXml.AppendLine("                        <SERVICOSEMGERAL></SERVICOSEMGERAL>");
            sbXml.AppendLine("                        <SEGUROSEMGERAL></SEGUROSEMGERAL>");
            sbXml.AppendLine("                        <MATERIALDECONSUMO>x</MATERIALDECONSUMO>");
            sbXml.AppendLine("                        <MATERIALPERMANENTE></MATERIALPERMANENTE>");
            sbXml.AppendLine("                        <ALUGUEIS></ALUGUEIS>");
            sbXml.AppendLine("                        <IMPORTACAODEMATCONSUMO></IMPORTACAODEMATCONSUMO>");
            sbXml.AppendLine("                        <IMPORTACAODEMATPERMANENTE></IMPORTACAODEMATPERMANENTE>");
            sbXml.AppendLine("                        <ATIVINDUSTRIALMATERIAPRIMA></ATIVINDUSTRIALMATERIAPRIMA>");
            sbXml.AppendLine("                        <ATIVINDUSTRIALMATEMBALAGEM></ATIVINDUSTRIALMATEMBALAGEM>");
            sbXml.AppendLine("                        <repeticaoItem>");
            sbXml.AppendLine("                            <linha>");
            sbXml.AppendLine("                                <Item>00013197-0</Item>");
            sbXml.AppendLine("                                <UnidForn>00001</UnidForn>");
            sbXml.AppendLine("                                <QtdInteiro>500</QtdInteiro>");
            sbXml.AppendLine("                                <QtdDecimal>000</QtdDecimal>");
            sbXml.AppendLine("                            </linha>");
            sbXml.AppendLine("                            <linha>");
            sbXml.AppendLine("                                <Item>00307246-0</Item>");
            sbXml.AppendLine("                                <UnidForn>00001</UnidForn>");
            sbXml.AppendLine("                                <QtdInteiro>500</QtdInteiro>");
            sbXml.AppendLine("                                <QtdDecimal>000</QtdDecimal>");
            sbXml.AppendLine("                            </linha>");
            sbXml.AppendLine("                        </repeticaoItem>");
            sbXml.AppendLine("                        <Observacao>Teste após correção na entrada por empenho. Empenho 2015NE00002, UGE 380193, Ambiente SIAFEM Treinamento</Observacao>");
            sbXml.AppendLine("                        <repeticaoNf>");
            sbXml.AppendLine("                            <NotaFiscal>201588888877</NotaFiscal>");
            sbXml.AppendLine("                        </repeticaoNf>");
            sbXml.AppendLine("                    </documento>");
            sbXml.AppendLine("                </SFCOLiquidaNL>");
            sbXml.AppendLine("            </SFCODOC>");
            sbXml.AppendLine("        </Doc_Estimulo>");
            sbXml.AppendLine("    </BCMSG>");
            sbXml.AppendLine("    <SISERRO>");
            sbXml.AppendLine("        <Doc_Retorno></Doc_Retorno>");
            sbXml.AppendLine("    </SISERRO>");
            sbXml.AppendLine("</MSG>");

            strRetorno = sbXml.ToString();
            return strRetorno;
        }
    }

}