using PatrimonioBusiness.fechamento.interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatrimonioBusiness.fechamento.entidades
{
    internal class DepreciacaoErro : IDepreciacaoErro
    {
        private DepreciacaoErro() { }
        internal static DepreciacaoErro GetInstancia()
        {
            return new DepreciacaoErro();
        }

        internal static IDepreciacaoErro ConverterDataRowParaLista(DataRow row)
        {
            IDepreciacaoErro depreciacao = new DepreciacaoErro();
            depreciacao.ErrorMessage = GetMensagemDataInvalida(row["ErrorMessage"].ToString());
            depreciacao.ErrorNumber =int.Parse(row["ErrorNumber"].ToString());
            depreciacao.MaterialItemCode = int.Parse(row["MaterialItemCode"].ToString());
            depreciacao.AssetId = int.Parse(row["AssetId"].ToString());
            return depreciacao;
        }
        public int AssetStartId
        {
            get;set;
        }

        public string ErrorMessage
        {
            get; set;
        }

        public int ErrorNumber
        {
            get; set;
        }

        public int MaterialItemCode
        {
            get; set;
        }

        public int AssetId
        {
            get;set;
        }

        private static string GetMensagemDataInvalida(string mensagem)
        {
            if (mensagem.Contains("dados date em um tipo de dados datetime") || mensagem.Contains("date data type to a datetime data type"))
                return "Data de aquisição inválida, ano da data é menor que 1900. ";

            return mensagem;
        }

    }
}
