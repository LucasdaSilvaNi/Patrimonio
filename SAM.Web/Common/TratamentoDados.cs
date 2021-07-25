using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace SAM.Web.Common
{
    public class TratamentoDados
    {
        public static bool ValidarCPF(string _cpf)
        {
            bool retorno = true;

            StringBuilder expressao = new StringBuilder();
            expressao.Append("^(\\d{9})(\\d{2})$");

            Regex cpf = new Regex(expressao.ToString());
            if (cpf.IsMatch(_cpf))
            {
                int somaDigitoUm = Convert.ToInt32(_cpf.Substring(0, 1)) * 10
                         + Convert.ToInt32(_cpf.Substring(1, 1)) * 9
                         + Convert.ToInt32(_cpf.Substring(2, 1)) * 8
                         + Convert.ToInt32(_cpf.Substring(3, 1)) * 7
                         + Convert.ToInt32(_cpf.Substring(4, 1)) * 6
                         + Convert.ToInt32(_cpf.Substring(5, 1)) * 5
                         + Convert.ToInt32(_cpf.Substring(6, 1)) * 4
                         + Convert.ToInt32(_cpf.Substring(7, 1)) * 3
                         + Convert.ToInt32(_cpf.Substring(8, 1)) * 2;

                //Calculando Primeiro DÃ­gito Verificador
                int resto = somaDigitoUm % 11;

                if ((resto == 0) || (resto == 1))
                {
                    resto = 0;
                }
                else
                {
                    resto = 11 - resto;
                }

                if (Convert.ToInt32(_cpf.Substring(9, 1)) != resto)
                {
                    retorno = false;
                }

                //Calculando Segundo Digito Verificador
                int somaDigitoDois = Convert.ToInt32(_cpf.Substring(0, 1)) * 11
                                   + Convert.ToInt32(_cpf.Substring(1, 1)) * 10
                                   + Convert.ToInt32(_cpf.Substring(2, 1)) * 9
                                   + Convert.ToInt32(_cpf.Substring(3, 1)) * 8
                                   + Convert.ToInt32(_cpf.Substring(4, 1)) * 7
                                   + Convert.ToInt32(_cpf.Substring(5, 1)) * 6
                                   + Convert.ToInt32(_cpf.Substring(6, 1)) * 5
                                   + Convert.ToInt32(_cpf.Substring(7, 1)) * 4
                                   + Convert.ToInt32(_cpf.Substring(8, 1)) * 3
                                   + Convert.ToInt32(_cpf.Substring(9, 1)) * 2;


                resto = somaDigitoDois % 11;

                if (resto == 0 || resto == 1)
                {
                    resto = 0;
                }
                else
                {
                    resto = 11 - resto;
                }

                if (Convert.ToInt32(_cpf.Substring(10, 1)) != resto)
                {
                    retorno = false;
                }
            }
            else
            {
                retorno = false;
            }

            // Caso coloque todos os numeros iguais
            switch (_cpf)
            {
                case "00000000000":
                    retorno = false;
                    break;
                case "11111111111":
                    retorno = false;
                    break;
                case "2222222222":
                    retorno = false;
                    break;
                case "33333333333":
                    retorno = false;
                    break;
                case "44444444444":
                    retorno = false;
                    break;
                case "55555555555":
                    retorno = false;
                    break;
                case "66666666666":
                    retorno = false;
                    break;
                case "77777777777":
                    retorno = false;
                    break;
                case "88888888888":
                    retorno = false;
                    break;
                //case "99999999999":
                //    retorno = false;
                //    break;
            }

            return retorno;
        }

        public static decimal truncarDuasCasas(decimal? valorFracionario)
        {
            //TODO verificar para DecimalNumberSeparator
            string[] casaDecimais = valorFracionario.ToString().Split(',');
            if (casaDecimais.Length > 1)
            {

                string valorDecimal = casaDecimais[0];
                string casaDecimal = (casaDecimais[1].Length >= 2) ? casaDecimais[1].Substring(0, 2) : casaDecimais[1];
                string retorno = valorDecimal + "," + casaDecimal;
                return decimal.Parse(retorno);
            }
            else
            {
                return decimal.Parse(valorFracionario.ToString());
            }

        }

    }
}