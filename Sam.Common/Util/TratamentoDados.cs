using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;


namespace Sam.Common.Util
{
    public static class TratamentoDados
    {
        /* Expressões regulares
         * \w : qualquer caracter alfanumérico incluindo underline, ou seja, equivale a [a-zA-Z0-9_]
         * +  : encontra uma ou mais ocorrências, ou seja, equivale a {1,}
         * *  : encontra zero ou mais ocorrências, ou seja, equivale a {0,}
         * () : agrupamento de caracteres para criar uma cláusula de condição
        */

        /// <summary>
        /// Quando for(em) passado(s) espaço(s) em branco, como um valor válido,
        /// retornará null, quando não: retornará valor passado
        /// </summary>
        /// <param name="valorString"></param>
        /// <returns>valorString</returns>
        public static bool ValidaString(string valorString)
        {
            if (valorString != null)
                valorString = valorString.Trim();
            bool retorno = !string.IsNullOrEmpty(valorString);

            return retorno;
        }

        /// <summary>
        /// Validação de e-mail digitado pelo usuário. Ex.: usuario@prodesp.gov.br
        /// </summary>
        /// <param name="_email">E-mail informado pelo usuário</param>
        /// <returns>E-mail válido ou não</returns>
        public static bool ValidarEmail(string _email)
        {
            return Regex.Match(_email, @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$").Success;
        }

        /// <summary>
        /// Validação de DDD do telefone, digitado pelo usuário. Ex.: 11
        /// </summary>
        /// <param name="_telefone">DDD informado pelo usuário</param>
        /// <returns>DDD válido ou não</returns>
        public static bool ValidarDDD(string _ddd)
        {
            return Regex.Match(_ddd, @"^\d{2}$").Success;
        }

        /// <summary>
        /// Validação de telefone, sem DDD e com máscara, digitado pelo usuário. Ex.: 1234-5678
        /// </summary>
        /// <param name="_telefone">Telefone informado pelo usuário</param>
        /// <returns>Telefone válido ou não</returns>
        public static bool ValidarTelefoneSemDDDComMascara(string _telefone)
        {
            return Regex.Match(_telefone, @"^\d{4}-\d{4}$").Success;
        }

        /// <summary>
        /// Validação de telefone, sem DDD e sem máscara, digitado pelo usuário. Ex.: 12345678
        /// </summary>
        /// <param name="_telefone">Telefone informado pelo usuário</param>
        /// <returns>Telefone válido ou não</returns>
        public static bool ValidarTelefoneSemDDDSemMascara(string _telefone)
        {
            return Regex.Match(_telefone, @"^\d{8}$").Success;
        }

        /// <summary>
        /// Validação de telefone, com DDD, digitado pelo usuário. Ex.: (11)1234-5678
        /// </summary>
        /// <param name="_telefone">Telefone informado pelo usuário</param>
        /// <returns>Telefone válido ou não</returns>
        public static bool ValidarTelefoneComDDDComMascara(string _telefone)
        {
            return Regex.Match(_telefone, @"^\((\d{2})\)(\d{4})-(\d{4})$").Success;
        }

        /// <summary>
        /// Verifica se a quantidade a liquidar pelo SIAFISICO é divisível
        /// </summary>
        /// <param name="valorString"></param>
        /// <returns></returns>

        public static decimal ValidarQtdeLiquidar(int qtdeLiq, int qtdeMov, int qtdSiafisico)
        {
            // pega a qtde em unidades por embalagem
            decimal qtdeLiqUnidade = qtdeLiq / qtdSiafisico;
            decimal qtdeMovSiafisico = 0;

            // se o resto for zero, prosseguir
            if (qtdeLiqUnidade != 0)
            {
                if (qtdeMov % qtdeLiqUnidade == 0)
                {
                    qtdeMovSiafisico = qtdeMov / qtdeLiqUnidade;
                }
            }
            return qtdeMovSiafisico;
        }

        public static int ParseIntNull(int? intNull)
        {
            return (intNull == null) ? 0 : (int)intNull;
        }

        public static int ParseIntNull(string valor)
        {
            int _retorno;
            int.TryParse(valor.Trim(), out _retorno);

            return _retorno;
        }

        public static byte ParseByteNull(byte? valor)
        {
            return valor.GetValueOrDefault(0);
        }

        public static byte ParseByteNull(string valor)
        {
            byte _retorno;
            byte.TryParse(valor.Trim(), out _retorno);

            return _retorno;
        }

        public static decimal ParseDecimalNull(decimal? decNull)
        {
            return (decNull == null) ? 0 : (decimal)decNull;
        }

        public static string ParseStringNull(string stringNull)
        {
            return (stringNull == null) ? string.Empty : stringNull.ToString();
        }

        public static DateTime ParseDateTimeNull(DateTime? dateTimeNull)
        {
            return (dateTimeNull == null) ? DateTime.MaxValue : (DateTime)dateTimeNull;
        }

        public static string TryParseString(string valorString)
        {
            return String.IsNullOrEmpty(valorString) ? null : valorString.Trim();
        }

        public static bool ValidaNumero(string numero)
        {
            Regex rx = new Regex(@"^\d+$");
            return rx.IsMatch(numero);
        }

        public static int? TryParseInt32(string inteiro)
        {
            int? valor = null;
            int parametro;
            int parametroInt;
            if (int.TryParse(inteiro, out parametro))
                valor = parametro;

            bool parametroBoolean;
            if (int.TryParse(inteiro, out parametroInt))
                valor = parametroInt;
            if (bool.TryParse(inteiro, out parametroBoolean))
                valor = Convert.ToInt32(parametroBoolean);
            return valor;
        }

        public static long? TryParseLong(string inteiro)
        {
            long? valor = null;
            long parametro;
            if (long.TryParse(inteiro, out parametro))
                valor = parametro;

            return valor;
        }

        public static int? TryParseInt16(string inteiro)
        {
            short? valor = null;
            short parametro;
            if (short.TryParse(inteiro, out parametro))
                valor = parametro;

            return valor;
        }

        public static decimal? TryParseDecimal(string inteiro, bool blnUseCultureInfo)
        {
            if (!blnUseCultureInfo)
                return TryParseDecimal(inteiro);

            CultureInfo cInfo = new CultureInfo("pt-BR");
            //NumberStyles numStyle = (NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowLeadingSign);
            NumberStyles numStyle = (NumberStyles.Any);

            decimal? valor = null;
            decimal parametro;
            if (!decimal.TryParse(inteiro, numStyle, cInfo, out parametro))
            {
                cInfo = new CultureInfo("en-US");
                decimal.TryParse(inteiro, numStyle, cInfo, out parametro);
            }

            valor = parametro;

            return valor;
        }

        public static decimal? TryParseDecimal(string inteiro)
        {
            decimal? valor = null;
            decimal parametro;
            if (decimal.TryParse(inteiro, out parametro))
                valor = parametro;

            return valor;
        }

        public static bool? TryParseBool(string booleano)
        {
            bool? valor = null;
            byte byteBooleano;
            if (byte.TryParse(booleano, out byteBooleano))
            {
                valor = Convert.ToBoolean(byteBooleano);
            }

            return valor;
        }

        public static short? TryParseShort(string inteiro)
        {
            short? valor = null;
            short parametro;
            if (short.TryParse(inteiro, out parametro))
                valor = parametro;

            return valor;
        }

        public static DateTime? TryParseDateTime(string datetime)
        {
            DateTime? valor = null;
            try
            {
                DateTime parametro;
                if (DateTime.TryParse(datetime, out parametro))
                {
                    if (parametro > DateTime.MinValue || parametro < DateTime.MaxValue)
                        if (parametro > System.Data.SqlTypes.SqlDateTime.MinValue || parametro < System.Data.SqlTypes.SqlDateTime.MaxValue)
                            valor = parametro;
                }

            }
            catch
            {
                valor = null;
            }
            return valor;
        }

        public static double? TryParseDouble(string inteiro)
        {
            double? valor = null;
            double parametro;
            if (double.TryParse(inteiro, out parametro))
                valor = parametro;

            return valor;
        }

        public static byte? TryParseByte(string _byte)
        {

            byte? valor = null;
            byte parametro;
            if (byte.TryParse(_byte, out parametro))
                valor = parametro;

            return valor;
        }

        public static String TryParseMesAno(string _mesAno)
        {
            string valor = _mesAno;
            try
            {
                DateTime parametro;
                if (_mesAno.Length == 7 && DateTime.TryParse("01/" + _mesAno, out parametro))
                {
                    if (parametro > DateTime.MinValue || parametro < DateTime.MaxValue)
                        if (parametro > System.Data.SqlTypes.SqlDateTime.MinValue || parametro < System.Data.SqlTypes.SqlDateTime.MaxValue)
                            valor = _mesAno.Replace("/", "");
                }
            }
            catch
            {
                valor = null;
            }
            return valor;
        }

        /// <summary>
        /// Retira caracteres especiais contidos na string informada
        /// </summary>
        /// <param name="str">String contendo caracteres especiais (MASCARA) </param>
        /// <returns>String sem a mascara</returns>
        public static String RetirarMascara(string str)
        {
            Regex replaceAcentos = new Regex("[.|_|/|&|(|)|-]", RegexOptions.Compiled);
            str = replaceAcentos.Replace(str, "");

            return str;
        }
        
        public static bool ValidarCNPJ(string _cnpj)
        {
            bool retorno = true;

            StringBuilder expressao = new StringBuilder();
            expressao.Append("^(\\d{8})(\\d{4})(\\d{2})$");

            Regex regCnpj = new Regex(expressao.ToString());
            if ((regCnpj.IsMatch(_cnpj)) && (_cnpj != "00000000000000"))
            {
                int somaDigitoUm = Convert.ToInt32(_cnpj.Substring(0, 1)) * 5
                         + Convert.ToInt32(_cnpj.Substring(1, 1)) * 4
                         + Convert.ToInt32(_cnpj.Substring(2, 1)) * 3
                         + Convert.ToInt32(_cnpj.Substring(3, 1)) * 2
                         + Convert.ToInt32(_cnpj.Substring(4, 1)) * 9
                         + Convert.ToInt32(_cnpj.Substring(5, 1)) * 8
                         + Convert.ToInt32(_cnpj.Substring(6, 1)) * 7
                         + Convert.ToInt32(_cnpj.Substring(7, 1)) * 6
                         + Convert.ToInt32(_cnpj.Substring(8, 1)) * 5
                         + Convert.ToInt32(_cnpj.Substring(9, 1)) * 4
                         + Convert.ToInt32(_cnpj.Substring(10, 1)) * 3
                         + Convert.ToInt32(_cnpj.Substring(11, 1)) * 2;

                //Calculando Primeiro DÃ­gito Verificador
                int resto = 11 - (somaDigitoUm % 11);

                if (resto > 9)
                {
                    resto = 0;
                }

                if (Convert.ToInt32(_cnpj.Substring(12, 1)) != resto)
                {
                    retorno = false;
                }

                //Calculando Segundo Digito Verificador
                int somaDigitoDois = Convert.ToInt32(_cnpj.Substring(0, 1)) * 6
                                   + Convert.ToInt32(_cnpj.Substring(1, 1)) * 5
                                   + Convert.ToInt32(_cnpj.Substring(2, 1)) * 4
                                   + Convert.ToInt32(_cnpj.Substring(3, 1)) * 3
                                   + Convert.ToInt32(_cnpj.Substring(4, 1)) * 2
                                   + Convert.ToInt32(_cnpj.Substring(5, 1)) * 9
                                   + Convert.ToInt32(_cnpj.Substring(6, 1)) * 8
                                   + Convert.ToInt32(_cnpj.Substring(7, 1)) * 7
                                   + Convert.ToInt32(_cnpj.Substring(8, 1)) * 6
                                   + Convert.ToInt32(_cnpj.Substring(9, 1)) * 5
                                   + Convert.ToInt32(_cnpj.Substring(10, 1)) * 4
                                   + Convert.ToInt32(_cnpj.Substring(11, 1)) * 3
                                   + Convert.ToInt32(_cnpj.Substring(12, 1)) * 2;


                resto = 11 - (somaDigitoDois % 11);

                if (resto > 9)
                {
                    resto = 0;
                }

                if (Convert.ToInt32(_cnpj.Substring(13, 1)) != resto)
                {
                    retorno = false;
                }
            }
            else
            {
                retorno = false;
            }

            return retorno;
        }

        public static void Trim<T>(ref T instancia){
            //Buscar todas as propriedades da instancia passada
            PropertyInfo[] propriedades = instancia.GetType().GetProperties();
            foreach (PropertyInfo propriedade in propriedades)
            {
                //Analisa se o valor da proprieade pode ser lida
                if (propriedade.CanRead)
                {
                    //Pega o valor da propriedade
                    var valor = propriedade.GetValue(instancia, null);
                    
                    //Se a propriedade não for nula
                    if (valor != null)
                    {
                        //Se a propriedade for string
                        if (valor.GetType() == "".GetType())
                        {
                            //Se a propriedade puder ser escrita, setar o valor atual sem espaços
                            if (propriedade.CanWrite)
                            {
                                valor = valor.ToString().Trim();
                                propriedade.SetValue(instancia, valor, null);
                            }
                        }
                    }
                }
            }
        }

        // esta função define o anomês de referência ao almoxarifado
        public static string ValidarAnoMesRef(string anoMes, int numMeses) 
        {
            if (anoMes.Length != 6)
            {
                return "";
            }
            DateTime dataRef = new DateTime(Convert.ToInt32(anoMes.Substring(0, 4)), Convert.ToInt32(anoMes.Substring(4, 2)), 1);
            dataRef = dataRef.AddMonths(numMeses);
            return dataRef.Date.Year.ToString().PadLeft(4, '0') + dataRef.Date.Month.ToString().PadLeft(2, '0');
        }

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

            return retorno;
        }

        [DebuggerStepThroughAttribute]
        public static bool IsNull(this object objParam)
        {
            return (objParam == null);
        }

        [DebuggerStepThroughAttribute]
        public static bool IsNotNull(this object objParam)
        {
            return (objParam != null);
        }

        public static decimal? CalcularPrecoMedioSaldo(decimal? SaldoValor, decimal? quantidade)
        {
            var valorZero = Constante.CST_AUXILIAR_COMPILACAO_CONDICIONAL_VALOR_ZERO;

            if (SaldoValor == null)
                throw new Exception("Não foi possível calcular o preço médio. A quantidade o saldo está nulo");

            if (quantidade == null)
                throw new Exception("Não foi possível calcular o preço médio. A quantidade está nula");

            //Se o saldo em valor ou em qtde estiver zerado, retorna 0 como preço médio
            if (SaldoValor == 0.00m || quantidade == valorZero)
                return valorZero;

            try
            {
                if ((quantidade.HasValue) && (quantidade > 0))
                {
                    decimal? precoUnit = (SaldoValor / quantidade);

                    return precoUnit.Value.Truncar(4, true);
                }
                else
                    throw new Exception("Não foi possível calcular o preço médio.");
            }
            catch
            {
                throw new Exception("Não foi possível calcular o preço médio.");

            }
        }
    }
}
