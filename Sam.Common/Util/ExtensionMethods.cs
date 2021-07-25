using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Reflection;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Transactions;
using System.Diagnostics;
using System.Globalization;
using System.Web;
using System.ComponentModel;

namespace Sam.Common.Util
{
    public static partial class ExtensionMethods
    {
        #region Reflection
        public static int obterAlmoxarifadoId(IList listaDataSource)
        {
            #region Variaveis
            object valor = null;
            int IdRetorno = -1;
            bool _travaFim = false;
            var itemLista = listaDataSource[0];
            MemberInfo metaElementoLista = null;
            Dictionary<string, MemberInfo> dicTipos = new Dictionary<string, MemberInfo>();
            Dictionary<string, MemberInfo> dicSubTipos = new Dictionary<string, MemberInfo>();
            #endregion Variaveis

            itemLista.GetType()
                     .ObterSubTipos()
                     .ForEach(subClasse => dicTipos.Add(subClasse.Name, subClasse));

            if (dicTipos.ContainsKey("Almoxarifado"))
            {
                metaElementoLista = dicTipos["Almoxarifado"];
                valor = metaElementoLista.ObterValorCampoAninhado("Almoxarifado", "Id", itemLista);

                if (valor.IsNotNull())
                {
                   return (int)valor;
                }
                else
                {
                    foreach (var subTipo in dicTipos)
                    {
                        if (dicTipos.TryGetValue(subTipo.Key, out metaElementoLista))
                        {
                            valor = metaElementoLista.ObterValorCampo(subTipo.Key, itemLista);

                            if (valor.IsNull())
                                continue;

                            metaElementoLista.ObterSubTipos().ForEach(_insideSubTipo =>
                            {
                                if ((_insideSubTipo.Name == "Almoxarifado") && _insideSubTipo.ObterValorCampoAninhado(_insideSubTipo.Name, "Id", valor).IsNotNull())
                                {
                                    IdRetorno = (int)_insideSubTipo.ObterValorCampoAninhado(_insideSubTipo.Name, "Id", valor);
                                    _travaFim = true;
                                }
                            });
                            if (_travaFim)
                                break;
                        }
                    }
                }
            }

            return IdRetorno;
        }

        public static List<MemberInfo> ObterSubTipos(this Type tipoEntidade)
        {
            Func<MemberInfo, bool> predicate = (tipoCampo => (((tipoCampo.MemberType == MemberTypes.Field) && (((FieldInfo)tipoCampo).FieldType.BaseType.Name == "BaseEntity")) ||
                                                   ((tipoCampo.MemberType == MemberTypes.Property) && (((PropertyInfo)tipoCampo).PropertyType.BaseType.Name == "BaseEntity"))));

            return tipoEntidade.GetMembers()
                               .Where(predicate)
                               .ToList();
        }

        public static List<MemberInfo> ObterSubTipos(this MemberInfo membroTipoEntidade)
        {
            Func<MemberInfo, bool> predicate = (tipoCampo => (((tipoCampo.MemberType == MemberTypes.Field) && (((FieldInfo)tipoCampo).FieldType.BaseType.Name == "BaseEntity")) ||
                                                   ((tipoCampo.MemberType == MemberTypes.Property) && (((PropertyInfo)tipoCampo).PropertyType.BaseType.Name == "BaseEntity"))));

            var tipoEntidade = ((((PropertyInfo)membroTipoEntidade).PropertyType) ?? (((FieldInfo)membroTipoEntidade).FieldType));

            return tipoEntidade.GetMembers()
                               .Where(predicate)
                               .ToList();
        }

        public static object ObterValorCampo(this MemberInfo metaElementoTipo, string nomeCampo, object objValor)
        {
            object valor = null;
            metaElementoTipo = objValor.GetType().GetMembers().Where(campoMembro => campoMembro.Name.ToLowerInvariant() == nomeCampo.ToLowerInvariant()).FirstOrDefault();

            if (objValor.IsNotNull() && metaElementoTipo.IsNotNull())
                valor = (metaElementoTipo.MemberType == MemberTypes.Property) ? ((PropertyInfo)metaElementoTipo).GetValue(objValor, null) : ((FieldInfo)metaElementoTipo).GetValue(objValor);

            return valor;
        }

        public static object ObterValorCampoAninhado(this MemberInfo metaElementoTipo, string nomeClasseAninhada, string nomeCampo, object objValor)
        {
            object valor = null;
            metaElementoTipo = objValor.GetType().GetMembers().Where(campoMembro => campoMembro.Name.ToLowerInvariant() == nomeClasseAninhada.ToLowerInvariant()).FirstOrDefault();

            if (objValor.IsNotNull() && metaElementoTipo.IsNotNull())
            {
                valor = (metaElementoTipo.MemberType == MemberTypes.Property) ? ((PropertyInfo)metaElementoTipo).GetValue(objValor, null) : ((FieldInfo)metaElementoTipo).GetValue(objValor);
                
                metaElementoTipo = objValor.GetType().GetMembers().Where(campoMembro => campoMembro.Name.ToLowerInvariant() == nomeCampo.ToLowerInvariant()).FirstOrDefault();
                valor = metaElementoTipo.ObterValorCampo(nomeCampo, valor);
            }

            return valor;
        }

        public static T ObterAtributoDoTipo<T>(this Enum valorEnum) where T : System.Attribute
        {
            var type = valorEnum.GetType();
            var memInfo = type.GetMember(valorEnum.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            return (attributes.Length > 0) ? (T)attributes[0] : null;
        }

        public static string ObterDescricao(this Enum valorEnum)
        {
            return valorEnum.ObterAtributoDoTipo<DescriptionAttribute>().Description;
        }

        #endregion Reflection

        #region Strings
        /// <summary>
        /// Método customizado para tratar retorno (mal-formado) da mensagem XML SIAFDetaContaGen
        /// </summary>
        /// <param name="pStrLinha"></param>
        /// <returns></returns>
        public static string BreakLineForEmpenho(this string pStrLinha)
        {
            int iIndiceCodigo = 0;
            string[] lArrString = new string[] { };
            string lStrRetorno = String.Empty;

            lArrString = pStrLinha.Split(Constante.CST_SEPARADOR_SPLIT, StringSplitOptions.RemoveEmptyEntries);

            if (lArrString.Count() == 4)
            { 
                lStrRetorno = lArrString[1]; 
            }
            else if (lArrString.Count() == 3)
            {
                iIndiceCodigo = lArrString[0].IndexOf("NE");
                lStrRetorno = lArrString[0].Substring(iIndiceCodigo - 4);
            }

            return lStrRetorno;
        }
        public static string BreakLine(this string pStrLinha, int index)
        {
            string[] lArrString = new string[] { };
            string lStrRetorno = String.Empty;

            lArrString = pStrLinha.Split(Constante.CST_SEPARADOR_SPLIT, StringSplitOptions.RemoveEmptyEntries);
            lStrRetorno = lArrString[index];

            return lStrRetorno;
        }
        public static string[] BreakLine(this string pStrLinha)
        {
            string[] lArrString = null;
            string lStrRetorno = String.Empty;

            lArrString = pStrLinha.Split(Constante.CST_SEPARADOR_SPLIT, StringSplitOptions.RemoveEmptyEntries);

            if (lArrString.Length == 0 || lArrString == null)
                lArrString = new string[] { };

            return lArrString;
        }
        public static string[] BreakLine(this string pStrLinha, char[] pChrSeparador)
        {
            string[] lArrString = null;
            string lStrRetorno = String.Empty;

            lArrString = pStrLinha.Split(pChrSeparador, StringSplitOptions.RemoveEmptyEntries);

            if (lArrString.Length == 0 || lArrString == null)
                lArrString = new string[] { };

            return lArrString;
        }
        public static string[] BreakLine(this string pStrLinha, string pStrSeparador)
        {
            char[] arrSeparador = new char[] { pStrSeparador[0] };
            return ExtensionMethods.BreakLine(pStrLinha, arrSeparador);
        }
        public static string BreakLine(this string pStrLinha, char pChrSeparador, int index)
        {
            string[] lArrString = new string[] { pChrSeparador.ToString() };
            string lStrRetorno = String.Empty;

            lArrString = pStrLinha.Split(lArrString, StringSplitOptions.RemoveEmptyEntries);
            lStrRetorno = lArrString[index];

            return lStrRetorno;
        }
        public static string BreakLine(this string pStrLinha, string pStrSeparador, int index)
        {
            char chrSeparador = pStrSeparador.ToCharArray()[0];
            return ExtensionMethods.BreakLine(pStrLinha, chrSeparador, index);
        }

        /// <summary>
        /// Método para limpeza de string com grandes espaços em branco.
        /// </summary>
        /// <param name="pStrXmlOrigem"></param>
        /// <returns></returns>
        public static string RetirarGrandesEspacosEmBranco(this string pStrXmlOrigem)
        {
            string lStrRetorno = string.Empty;

            lStrRetorno = pStrXmlOrigem;

            //tratamento contra XML's deformados por grandes espaços em branco.
            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex(@"[ ]{2,}", options);
            lStrRetorno = regex.Replace(lStrRetorno, @"");


            return lStrRetorno;
        }
        public static string[] SplitOn(this string textoParaQuebra, int numeroMaximoCaracteres, bool limparGrandesEspacosEQuebrasDeLinhas = false)
        {
            IList<string> linhasQuebradas = new List<string>();

            var linhaSombra = textoParaQuebra;
            if (string.IsNullOrEmpty(linhaSombra) == false)
            {
                
                string targetGroup = "Line";
                string theRegex = string.Format(@"(?<{0}>.{{1,{1}}})(?:\W|$)", targetGroup, numeroMaximoCaracteres);

                if (limparGrandesEspacosEQuebrasDeLinhas)
                {
                    do
                        linhaSombra = linhaSombra.RetirarQuebrasDeLinhaEEspacosEmBranco();
                    while (linhaSombra.IndexOf("  ") != -1);
                }

                MatchCollection matches = Regex.Matches(linhaSombra, theRegex, RegexOptions.IgnoreCase
                                                                          | RegexOptions.Multiline
                                                                          | RegexOptions.ExplicitCapture
                                                                          | RegexOptions.CultureInvariant
                                                                          | RegexOptions.Compiled);
                if (matches != null)
                    if (matches.Count > 0)
                        foreach (Match m in matches)
                            linhasQuebradas.Add(m.Groups[targetGroup].Value);

            }

            if (limparGrandesEspacosEQuebrasDeLinhas)
                linhasQuebradas.ToList().ForEach(linhaTexto => linhaTexto = linhaTexto.RetirarQuebrasDeLinhaEEspacosEmBranco());

            return (linhasQuebradas.ToArray());
        }

        public static string SafeReplace(this string input, string find, string replace, bool matchWholeWord)
        {
            string textToFind = matchWholeWord ? string.Format("{0}", find) : find;
            var debugTextReturn = Regex.Replace(input, textToFind, replace, RegexOptions.ExplicitCapture);
            return Regex.Replace(input, textToFind, replace);
        }
        public static string ToJavaScriptString(this String value)
        {
            return HttpUtility.JavaScriptStringEncode(value);
        }

        public static string RetirarCaracteresEspeciaisCpfCnpj(this string cpfCnpj)
        {
            string retornoSomenteNumerico = null;
            string fmtRetornoCpfCnpj = null;
            bool ehCnpj = false;




            if (!String.IsNullOrWhiteSpace(cpfCnpj))
            {
                ehCnpj = cpfCnpj.Contains("/");
                retornoSomenteNumerico = cpfCnpj.Replace("-", "").Replace(".", "").Replace("/", "");

                fmtRetornoCpfCnpj = (ehCnpj ? "{0:00000000000000}" : "{0:00000000000}");
                retornoSomenteNumerico = String.Format(fmtRetornoCpfCnpj, retornoSomenteNumerico);
            }


            return retornoSomenteNumerico;
        }
        #endregion Strings

        #region Datas
        public static int MonthDiff(this DateTime dataInicial, DateTime dataFinal)
        {
            return Math.Abs((dataFinal.Month - dataInicial.Month) + (12 * (dataFinal.Year - dataInicial.Year)));
        }

        public static int MonthTotalDays(this DateTime dataAtual)
        {
            return DateTime.DaysInMonth(dataAtual.Year, dataAtual.Month);
        }

        public static DateTime MonthLastDay(this DateTime dataAtual)
        {
            return new DateTime(dataAtual.Year, dataAtual.Month, DateTime.DaysInMonth(dataAtual.Year, dataAtual.Month));
        }

        public static DateTime? ZerarHoras(this DateTime? dataParaTransformar)
        {
            DateTime? dataTransformada = null;

            if (dataParaTransformar.HasValue)
            {
                dataTransformada = new DateTime(dataParaTransformar.Value.Year,  //Ano
                                                dataParaTransformar.Value.Month, //Mes
                                                dataParaTransformar.Value.Day,   //Dia
                                                0,                               //Hora
                                                0,                               //Minutos
                                                0,                               //Segundos
                                                0);                              //Milissegundos
            }

            return dataTransformada;
        }
        public static DateTime  ZerarHoras(this DateTime dataParaTransformar)
        {
            var dtTemp = new DateTime(dataParaTransformar.Year,    //Ano
                                      dataParaTransformar.Month,   //Mes
                                      dataParaTransformar.Day,     //Dia
                                      0,                           //Hora
                                      0,                           //Minutos
                                      0,                           //Segundos
                                      0);                          //Milissegundos

            return dtTemp;
        }
        public static DateTime RetornarMesAnoReferenciaDateTime(this string anoMesRef)
        {
            DateTime dtRetorno = new DateTime(0);

            if (anoMesRef.IsNotNull())
                //dtRetorno = DateTime.Parse(String.Format("01-{0}", anoMesRef.Insert(2, "-"), "dd-MM-yyyy"));
                dtRetorno = DateTime.Parse(String.Format("01-{0}-{1}", anoMesRef.Substring(4, 2), anoMesRef.Substring(0, 4), "dd-MM-yyyy"));

            return dtRetorno;
        }
        public static DateTime RetornarMesAnoReferenciaDateTimeBPPendente(this string anoMesRef)
        {
            DateTime dtRetorno = new DateTime(0);

            if (anoMesRef.IsNotNull())
                dtRetorno = DateTime.Parse(String.Format("01-{0}-{1}", anoMesRef.Substring(4, 2), anoMesRef.Substring(0, 4), "dd-MM-yyyy"));

            return dtRetorno;
        }
        public static DateTime RetornarMesAnoReferenciaAnteriorDateTime(this string anoMesRef)
        {
            DateTime dtRetorno = new DateTime(0);

            if (anoMesRef.IsNotNull())
                dtRetorno = DateTime.Parse(String.Format("{0}-01", anoMesRef.Insert(4, "-"))).AddMonths(-1);

            return dtRetorno;
        }
        public static DateTime RetornarData(this string dataEmTexto)
        {
            DateTime dtRetorno = new DateTime(0);

            if (!String.IsNullOrWhiteSpace(dataEmTexto))
                DateTime.TryParse(dataEmTexto.Insert(4, "-").Insert(2, "-"), out dtRetorno);


            return dtRetorno;
        }
        public static string RetornarMesAnoReferenciaAnterior(this string anoMesRef)
        {
            string strRetorno = null;

            if (anoMesRef.IsNotNull() && anoMesRef.Length == 6)
            {
                int iAno = Int32.Parse(anoMesRef.Substring(0, 4));
                int iMes = Int32.Parse(anoMesRef.Substring(4, 2));

                if (iMes == 1)
                { iAno -= 1; iMes = 12; }
                else
                { iMes -= 1; }

                strRetorno = String.Format("{0:0000}{1:00}", iAno, iMes);
            }

            return strRetorno;
        }
        public static int RetornarAnoMesReferenciaNumeral(this string anoMesRef)
        {
            int iRetorno = -1;

            if (anoMesRef.Length != 6 || !Int32.TryParse(anoMesRef, out iRetorno))
                throw new ArgumentNullException("Erro ao converter Ano/Mes Referência de almoxarifado para inteiro.");


            return iRetorno;
        }
        public static int RetornarAnoMesReferenciaNumeral(this DateTime dataAnoMesRef)
        {
            int iRetorno = -1;

            if (dataAnoMesRef.Ticks == 0 || !Int32.TryParse(dataAnoMesRef.ToString("yyyyMM"), out iRetorno))
                throw new ArgumentNullException("Erro ao converter Ano/Mes Referência de almoxarifado para inteiro.");


            return iRetorno;
        }
        #endregion Datas

        #region Coleções

        /// <summary>
        /// Helper methods to make it easier to throw exceptions.
        /// </summary>
        //public static class ThrowHelper
        //{
        internal static void ThrowIfNull<T>(this T argument, string name) where T : class
        {
            if (argument == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        internal static void ThrowIfNegative(this int argument, string name)
        {
            if (argument < 0)
            {
                throw new ArgumentOutOfRangeException(name);
            }
        }

        internal static void ThrowIfNonPositive(this int argument, string name)
        {
            if (argument <= 0)
            {
                throw new ArgumentOutOfRangeException(name);
            }
        }
        //}

        /// <summary>
        /// Returns the maximal element of the given sequence, based on
        /// the given projection.
        /// </summary>
        /// <remarks>
        /// If more than one element has the maximal projected value, the first
        /// one encountered will be returned. This overload uses the default comparer
        /// for the projected type. This operator uses immediate execution, but
        /// only buffers a single result (the current maximal element).
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="selector">Selector to use to pick the results to compare</param>
        /// <returns>The maximal element, according to the projection.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> is empty</exception>

        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> selector)
        {
            return source.MaxBy(selector, Comparer<TKey>.Default);
        }

        /// <summary>
        /// Returns the maximal element of the given sequence, based on
        /// the given projection and the specified comparer for projected values.
        /// </summary>
        /// <remarks>
        /// If more than one element has the maximal projected value, the first
        /// one encountered will be returned. This overload uses the default comparer
        /// for the projected type. This operator uses immediate execution, but
        /// only buffers a single result (the current maximal element).
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="selector">Selector to use to pick the results to compare</param>
        /// <param name="comparer">Comparer to use to compare projected values</param>
        /// <returns>The maximal element, according to the projection.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>, <paramref name="selector"/>
        /// or <paramref name="comparer"/> is null</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> is empty</exception>

        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");
            if (comparer == null) throw new ArgumentNullException("comparer");
            using (var sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                {
                    throw new InvalidOperationException("Sequence contains no elements");
                }
                var max = sourceIterator.Current;
                var maxKey = selector(max);
                while (sourceIterator.MoveNext())
                {
                    var candidate = sourceIterator.Current;
                    var candidateProjected = selector(candidate);
                    if (comparer.Compare(candidateProjected, maxKey) > 0)
                    {
                        max = candidate;
                        maxKey = candidateProjected;
                    }
                }
                return max;
            }
        }

        /// <summary>
        /// Returns all distinct elements of the given source, where "distinctness"
        /// is determined via a projection and the default eqaulity comparer for the projected type.
        /// </summary>
        /// <remarks>
        /// This operator uses deferred execution and streams the results, although
        /// a set of already-seen keys is retained. If a key is seen multiple times,
        /// only the first element with that key is returned.
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="keySelector">Projection for determining "distinctness"</param>
        /// <returns>A sequence consisting of distinct elements from the source sequence,
        /// comparing them by the specified key projection.</returns>

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            return source.DistinctBy(keySelector, null);
        }

        /// <summary>
        /// Returns all distinct elements of the given source, where "distinctness"
        /// is determined via a projection and the specified comparer for the projected type.
        /// </summary>
        /// <remarks>
        /// This operator uses deferred execution and streams the results, although
        /// a set of already-seen keys is retained. If a key is seen multiple times,
        /// only the first element with that key is returned.
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="keySelector">Projection for determining "distinctness"</param>
        /// <param name="comparer">The equality comparer to use to determine whether or not keys are equal.
        /// If null, the default equality comparer for <c>TSource</c> is used.</param>
        /// <returns>A sequence consisting of distinct elements from the source sequence,
        /// comparing them by the specified key projection.</returns>

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            source.ThrowIfNull("source");
            keySelector.ThrowIfNull("keySelector");
            return DistinctByImpl(source, keySelector, comparer);
        }

        public static IEnumerable<T> ExceptBy<T, TKey>(this IEnumerable<T> items, IEnumerable<T> other, Func<T, TKey> getKey)
        {
            return from item in items
                   join otherItem in other on getKey(item)
                   equals getKey(otherItem) into tempItems
                   from temp in tempItems.DefaultIfEmpty()
                   where ReferenceEquals(null, temp) || temp.Equals(default(T))
                   select item;

        }

        private static IEnumerable<TSource> DistinctByImpl<TSource, TKey>(IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            HashSet<TKey> knownKeys = new HashSet<TKey>(comparer);
            foreach (TSource element in source)
            {
                if (knownKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
            //
            // On platforms where LINQ is available but no HashSet<T>
            // (like on Silverlight), implement this operator using
            // existing LINQ operators. Using GroupBy is slightly less
            // efficient since it has do all the grouping work before
            // it can start to yield any one element from the source.
            //

            //return source.GroupBy(keySelector, comparer).Select(g => g.First());
            //yield return source.GroupBy(keySelector, comparer).Select(g => g.First());

        }

        public static void ValidarParametro(this SortedList listaParametros, string nomeParametro, string valorParametro)
        {

            if (!listaParametros.ContainsKey(nomeParametro))
                listaParametros.Add(nomeParametro, valorParametro);
            else if (String.IsNullOrWhiteSpace(listaParametros[nomeParametro].ToString()) || (listaParametros[nomeParametro].ToString() != valorParametro))
                listaParametros[nomeParametro] = valorParametro;
        }

        public static bool ExisteParametroValor(this SortedList listaParametros, string nomeParametro)
        {
            return (listaParametros.ContainsKey(nomeParametro) && !String.IsNullOrWhiteSpace(listaParametros[nomeParametro].ToString()));
        }

        public static void InserirValor(this IDictionary listaParametros, string nomeParametro, string valorParametro)
        {
            string strAux;
            if (listaParametros[nomeParametro].IsNull())
                listaParametros.Add(nomeParametro, valorParametro);
            else if (listaParametros[nomeParametro].IsNotNull())
            {
                strAux = listaParametros[nomeParametro].ToString();
                listaParametros[nomeParametro] = String.Format("{0}{1}", strAux, valorParametro);
            }
        }

        public static void InserirValor(this IDictionary listaParametros, string nomeParametro, object valorParametro)
        {
            string strAux;
            if (listaParametros[nomeParametro].IsNull())
                listaParametros.Add(nomeParametro, valorParametro);
            else if (listaParametros[nomeParametro].IsNotNull())
            {
                strAux = listaParametros[nomeParametro].ToString();
                listaParametros[nomeParametro] = valorParametro;
            }
        }

        public static void InserirValor<X, Y>(this IDictionary<X, Y> listaParametros, X nomeParametro, Y valorParametro)
        {
            if (!listaParametros.ContainsKey(nomeParametro))
                listaParametros.Add(nomeParametro, valorParametro);
            else
                listaParametros[nomeParametro] = valorParametro;
        }

        public static void InserirValor(this IDictionary<string, Sam.Common.Siafem.ItemEmpenhoRepete> listaParametros, string nomeParametro, Sam.Common.Siafem.ItemEmpenhoRepete valorParametro)
        {
            Sam.Common.Siafem.ItemEmpenhoRepete objAux;

            if (listaParametros.ContainsKey(nomeParametro))
            {
                objAux = listaParametros[nomeParametro];
                listaParametros[nomeParametro] = valorParametro;
            }
            else
            {
                listaParametros.Add(nomeParametro, valorParametro);
            }
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> pObjEnumerable)
        {
            return pObjEnumerable == null || !pObjEnumerable.Any();
        }
        public static bool IsNullOrEmpty<TKey, TValue>(this Dictionary<TKey, TValue> pDic)
        {
            return pDic == null || !pDic.Any();
        }
        public static bool IsNotNullAndNotEmpty<T>(this IEnumerable<T> pObjEnumerable)
        {
            return pObjEnumerable != null && pObjEnumerable.Any();
        }
        public static bool IsNotNullAndNotEmpty<TKey, TValue>(this Dictionary<TKey, TValue> pDic)
        {
            return pDic != null && pDic.Any();
        }

        [DebuggerStepThroughAttribute]
        public static bool HasElements<T>(this IList<T> listaDados)
        {
            //return (listaDados != null && listaDados.Count > 0);
            return ((listaDados != null && listaDados.Count > 0) && (listaDados[0] != null));
        }

        [DebuggerStepThroughAttribute]
        public static bool HasElements(this IDictionary datasourceDados)
        {
            //return datasourceDados.Count > 0;
            return ((datasourceDados.Values.Count > 0) && (datasourceDados[0] != null));
        }

        [DebuggerStepThroughAttribute]
        public static bool HasElements<T>(this IEnumerable<T> listaDados)
        {
            //return (listaDados != null && listaDados.Count() > 0);
            return ((listaDados != null && listaDados.Count() > 0) && (listaDados.GetEnumerator().MoveNext()));
        }

        /// <summary>
        /// Splits an array into several smaller arrays.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="array">The array to split.</param>
        /// <param name="size">The size of the smaller arrays.</param>
        /// <returns>An array containing smaller arrays.</returns>
        [DebuggerStepThroughAttribute]
        public static IList<IList<T>> ParticionadorLista<T>(this IEnumerable<T> array, int tamanhoLote)
        {
            IList<IList<T>> list = new List<IList<T>>();
            for (int i = 0; i < array.Count(); i += tamanhoLote)
                list.Add(array.ToList().GetRange(i, Math.Min(tamanhoLote, array.Count() - i)));
            return list;
        }
        #endregion

        #region Truncamento Decimais
        [Obsolete]
        public static Decimal Truncar(this Decimal valorDecimal, int numeroCasasDecimais)
        {
            int iPosicaoVirgula = -1;
            string strFomatoNumero = string.Format("N{0}", numeroCasasDecimais + 5);
            string strValorDecimal = string.Empty;

            strValorDecimal = valorDecimal.ToString(strFomatoNumero);

            if (!String.IsNullOrWhiteSpace(strValorDecimal) && strValorDecimal.Contains(","))
                iPosicaoVirgula = strValorDecimal.IndexOf(",") + 1;
            else if (!String.IsNullOrWhiteSpace(strValorDecimal) && !strValorDecimal.Contains(","))
                strValorDecimal = String.Format("{0},", strValorDecimal).PadRight(numeroCasasDecimais, '0');


            strValorDecimal = strValorDecimal.Substring(0, iPosicaoVirgula + numeroCasasDecimais);

            return Decimal.Parse(strValorDecimal);
        }

        public static Decimal Truncar(this Decimal valorDecimal, int numeroCasasDecimais, bool useMathFunction)
        {
            if (!useMathFunction)
                return Truncar(valorDecimal, numeroCasasDecimais);

            decimal fatorMultiplicador = (decimal)Math.Pow(10, numeroCasasDecimais);
            decimal valorTransformado = (Math.Truncate(valorDecimal * fatorMultiplicador) / fatorMultiplicador);

            return valorTransformado;
        }

        /// <summary>
        /// Função criada para suporte a utilização de data de corte para cálculos com duas ou quatro casas decimais no sistema.
        /// </summary>
        /// <param name="valorDecimal"></param>
        /// <param name="anoMesRef"></param>
        /// <param name="usaDataCorteSAP"></param>
        /// <returns></returns>
        public static Decimal Truncar(this Decimal valorDecimal, string anoMesRef, bool usaDataCorteSAP)
        {

            int numeroCasasDecimais = (anoMesRef.RetornarAnoMesReferenciaNumeral() <= Constante.CST_ANO_MES_DATA_CORTE_SAP) ? 2 : 4;
            decimal fatorMultiplicador = (decimal)Math.Pow(10, numeroCasasDecimais);
            decimal valorTransformado = (Math.Truncate(valorDecimal * fatorMultiplicador) / fatorMultiplicador);

            return valorTransformado;
        }

        /// <summary>
        /// Retorna valor decimal após a vÃ­rgula
        /// </summary>
        /// <param name="valorDecimal"></param>
        /// <param name="anoMesRef"></param>
        /// <param name="usaDataCorteSAP"></param>
        /// <returns></returns>
        public static int ParteFracionaria(this Decimal valorDecimal)
        {
            int valorParteDecimal = 0;

            if (valorDecimal.ToString().Contains(",") || valorDecimal.ToString().Contains("."))
            {
                int indicePosVirgula = ((valorDecimal.ToString().IndexOf(",") == -1) ? valorDecimal.ToString().IndexOf(".") : valorDecimal.ToString().IndexOf(","));
                Int32.TryParse(valorDecimal.ToString().Substring(indicePosVirgula + 1), out valorParteDecimal);
            }

            return valorParteDecimal;
        }
        public static string ParteFracionaria(this Decimal valorDecimal, bool retornaString = true)
        {
            string valorParteDecimal = "";

            if (valorDecimal.ToString().Contains(",") || valorDecimal.ToString().Contains("."))
            {
                int indicePosVirgula = ((valorDecimal.ToString().IndexOf(",") == -1) ? valorDecimal.ToString().IndexOf(".") : valorDecimal.ToString().IndexOf(","));
                valorParteDecimal = valorDecimal.ToString().Substring(indicePosVirgula + 1);
            }

            return valorParteDecimal;
        }

        public static decimal truncarDuasCasas(this float valorFracionario)
        {
            //TODO verificar para DecimalNumberSeparator
            string[] casaDecimais = valorFracionario.ToString().Split(',');
            if (casaDecimais.Length > 1)
            {

                string valorDecimal = casaDecimais[0];
                //string casaDeciaml = casaDecimais[1].Substring(0, 2);
                string casaDecimal = (casaDecimais[1].Length >= 2) ? casaDecimais[1].Substring(0, 2) : casaDecimais[1];
                string retorno = valorDecimal + "," + casaDecimal;
                return decimal.Parse(retorno);
            }
            else
            {
                return decimal.Parse(valorFracionario.ToString());
            }

        }
        public static decimal truncarDuasCasas(this decimal valorFracionario)
        {
            //TODO verificar para DecimalNumberSeparator
            string[] casaDecimais = valorFracionario.ToString().Split(',');
            if (casaDecimais.Length > 1)
            {

                string valorDecimal = casaDecimais[0];
                //string casaDeciaml = casaDecimais[1].Substring(0, 2);
                string casaDecimal = (casaDecimais[1].Length >= 2) ? casaDecimais[1].Substring(0, 2) : casaDecimais[1];
                string retorno = valorDecimal + "," + casaDecimal;
                return decimal.Parse(retorno);
            }
            else
            {
                return decimal.Parse(valorFracionario.ToString());
            }

        }
        public static decimal truncarQuatroCasas(this decimal valorFracionario)
        {
            //TODO verificar para DecimalNumberSeparator
            string[] casaDecimais = valorFracionario.ToString().Split(',');
            if (casaDecimais.Length > 1)
            {

                string valorDecimal = casaDecimais[0];
                //string casaDeciaml = casaDecimais[1].Substring(0, 2);
                string casaDecimal = (casaDecimais[1].Length >= 4) ? casaDecimais[1].Substring(0, 4) : casaDecimais[1];
                string retorno = valorDecimal + "," + casaDecimal;
                return decimal.Parse(retorno);
            }
            else
            {
                return decimal.Parse(valorFracionario.ToString());
            }

        }
        public static decimal truncarQuatroCasas(this float valorFracionario)
        {
            //TODO verificar para DecimalNumberSeparator
            string[] casaDecimais = valorFracionario.ToString().Split(',');
            if (casaDecimais.Length > 1)
            {

                string valorDecimal = casaDecimais[0];
                //string casaDeciaml = casaDecimais[1].Substring(0, 2);
                string casaDecimal = (casaDecimais[1].Length >= 4) ? casaDecimais[1].Substring(0, 4) : casaDecimais[1];
                string retorno = valorDecimal + "," + casaDecimal;
                return decimal.Parse(retorno);
            }
            else
            {
                return decimal.Parse(valorFracionario.ToString());
            }

        }

        /// <summary>
        /// Retorna valor decimal após a vÃ­rgula
        /// </summary>
        /// <param name="valorDecimal"></param>
        /// <param name="anoMesRef"></param>
        /// <param name="usaDataCorteSAP"></param>
        /// <returns></returns>
        public static bool PossuiValorFracionario(this Decimal valorDecimal)
        {
            int valorParteDecimal = 0;

            if (valorDecimal.ToString().Contains(",") || valorDecimal.ToString().Contains("."))
            {
                int indicePosVirgula = ((valorDecimal.ToString().IndexOf(",") == -1) ? valorDecimal.ToString().IndexOf(".") : valorDecimal.ToString().IndexOf(","));
                Int32.TryParse(valorDecimal.ToString().Substring(indicePosVirgula + 1), out valorParteDecimal);
            }

            return (valorParteDecimal > 0);
        }
        #endregion Truncamento Decimais

        #region XmlDocument
        public static XmlDocument LoadXmlDocument(this XmlDocument xmlDocument, string xml)
        {
            xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml);

            return xmlDocument;
        }

        public static string RetirarLinhasEEspacosEmBranco(this string textoParaLimpar)
        {
            return textoParaLimpar.Replace(Environment.NewLine, "")
                                  .Replace(" ", "")
                                  .Replace("\n", "")
                                  .Replace("\r", "")
                                  .Replace("\t", "");
        }
        public static string RetirarQuebrasDeLinhaEEspacosEmBranco(this string textoParaLimpar)
        {
            return textoParaLimpar.Replace("  ", " ")
                                  .Replace("\n\r", "  ")
                                  .Replace("\r\n", "  ")
                                  .Replace("\r", " ")
                                  .Replace("\n", " ")
                                  .Replace("\t", " ")
                                  .Trim();
        }
        #endregion XmlDocument

    }

    public static partial class ExtensionMethodsInfra
    {
        public static TransactionScope CreateNoLockTransaction()
        {
            var options = new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted };
            return new TransactionScope(TransactionScopeOption.Suppress, options);
        }

        public static List<T> ToListNoLock<T>(this IEnumerable<T> query)
        {
            using (TransactionScope ts = CreateNoLockTransaction())
            {
                return query.ToList();
            }
        }

        public static List<T> ToListReadUncommitted<T>(this IQueryable<T> query)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                List<T> toReturn = query.ToList();
                scope.Complete();
                return toReturn;
            }
        }

        public static int CountReadUncommitted<T>(this IQueryable<T> query)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted }))
            {
                int toReturn = query.Count();
                scope.Complete();
                return toReturn;
            }
        }
    }

    public static partial class ControlExtensions
    {
        /// <summary>
        /// recursively finds a child control of the specified parent.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Control FindControlRecursive(this Control control, string id)
        {
            if (control == null) return null;
            //try to find the control at the current level
            Control ctrl = control.FindControl(id);

            if (ctrl == null)
            {
                //search the children
                foreach (Control child in control.Controls)
                {
                    ctrl = FindControlRecursive(child, id);

                    if (ctrl != null) break;
                }
            }
            return ctrl;
        }

        /// <summary>
        /// Insere a opção padrão '- Selecione -', no DropDownList
        /// </summary>
        /// <param name="ddlControleSelecao"></param>
        /// <param name="limparCombo"></param>
        public static void InserirElementoSelecione(this ListControl ddlControleSelecao, bool limparCombo = false)
        {
            if (limparCombo)
                ddlControleSelecao.Items.Clear();

            ddlControleSelecao.Items.Insert(0, new ListItem("Todos", "0", true));
        }
        /// <summary>
        /// Insere uma opção com descrição textual escolhida pelo desenvolvedor, em um dado DropDownList
        /// </summary>
        /// <param name="ddlControleSelecao"></param>
        /// <param name="limparCombo"></param>
        public static void InserirNovaOpcaoLista(this ListControl ddlControleSelecao, string descricaoOpcao, int indiceOpcao, bool limparCombo = false)
        {
            if (limparCombo)
                ddlControleSelecao.Items.Clear();

            ddlControleSelecao.Items.Insert(0, new ListItem(descricaoOpcao, indiceOpcao.ToString(), true));
        }
        /// <summary>
        /// Verifica se a opção 'Todos' está selecionado no dropdownlist que está invocando o método.
        /// </summary>
        /// <param name="ddlControleSelecao"></param>
        /// <param name="descricaoOpcao"></param>
        /// <param name="indiceOpcao"></param>
        /// <param name="limparCombo"></param>
        public static bool OpcaoTodosSelecionada(this ListControl ddlControleSelecao)
        {
            ListItem opcaoTodos = null;
            opcaoTodos = ddlControleSelecao.Items.Cast<ListItem>().Where(opcaoLista => (opcaoLista.Value == "0")
                                                                                    && (opcaoLista.Selected == true)
                                                                                    && (opcaoLista.Text.ToLowerInvariant() == "Todos".ToLowerInvariant())).FirstOrDefault();

            return (opcaoTodos.IsNotNull());
        }

        public static int ObterIndiceValorTexto(this ListControl ctrlListaDeDados, string valorTextoPesquisa)
        {
            int idxRetorno = -1;
            ListItem itemPesquisado = null;

            var listaItens = ctrlListaDeDados.Items.Cast<ListItem>().ToList();

            if (listaItens.IsNotNullAndNotEmpty())
            {
                itemPesquisado = listaItens.Where(item => item.Text == valorTextoPesquisa).FirstOrDefault();

                if (itemPesquisado.IsNotNull())
                    Int32.TryParse(itemPesquisado.Value, out idxRetorno);
            }

            return idxRetorno;
        }

        public static void ReformatarValorNumerico(this ITextControl objTexto, string strFormatoNumerico)
        {
            string strRetorno = "";

            if (objTexto.IsNotNull() && !String.IsNullOrWhiteSpace(objTexto.Text))
                strRetorno = Decimal.Parse(objTexto.Text).ToString(strFormatoNumerico);

            objTexto.Text = strRetorno;
        }
        public static void ReformatarValorNumerico(this HtmlTableCell objTexto, string strFormatoNumerico)
        {
            string strRetorno = "";

            if (objTexto.IsNotNull() && !String.IsNullOrWhiteSpace(objTexto.InnerText.Trim()))
                strRetorno = Decimal.Parse(objTexto.InnerText.Trim()).ToString(strFormatoNumerico);

            objTexto.InnerText = strRetorno;
        }
        public static void ReformatarValorNumerico(this TableCell objTexto, string strFormatoNumerico)
        {
            string strRetorno = "";

            if (objTexto.IsNotNull() && !String.IsNullOrWhiteSpace(objTexto.Text.Trim()))
                strRetorno = Decimal.Parse(objTexto.Text.Trim()).ToString(strFormatoNumerico);

            objTexto.Text = strRetorno;
        }
    }

    public static partial class EntityFrameworkExtensions
    {
        public static EntitySet<T> ToEntitySet<T>(this IEnumerable<T> source) where T : class
        {
            var es = new EntitySet<T>();
            es.AddRange(source);
            return es;
        }
        public static EntityCollection<T> ToEntityCollection<T>(this IEnumerable<T> source) where T : class
        {
            var ec = new EntityCollection<T>();

            foreach (var s in source)
            {
                ec.Add(s);
            }
            return ec;
        }

        public static void IsEntityEmpty<T>(this T source) where T : EntityObject
        {
            Type lTpObjeto = typeof(T);
            T tt = Activator.CreateInstance<T>();

            if (source == tt)
            {
                throw new Exception("O objeto está vazio.");
            }
        }
        public static void IsClassEmpty<T>(this T source) where T : class
        {
            Type lTpObjeto = typeof(T);
            T tt = Activator.CreateInstance<T>();

            if (source == tt)
            {
                throw new Exception("O objeto está vazio.");
            }

            if (source == null)
            {
                throw new Exception("O objeto está nulo.");
            }

        }
    }
}
