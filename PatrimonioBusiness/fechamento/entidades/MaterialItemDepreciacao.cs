using PatrimonioBusiness.fechamento.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatrimonioBusiness.fechamento.entidades
{
    internal class MaterialItemDepreciacao : IMaterialItemDepreciacao
    {

        private DateTime acquisitionDate;
        private String mensagem;

        public DateTime AcquisitionDate
        {
            get { return this.acquisitionDate; }
            set
            {
                if (value.Year <= 1900)
                    this.mensagem = "Ano de aquisição inválido";

                this.acquisitionDate = value;
            }
        }

        public int AssetId
        {
            get; set;
        }

        public int MaterialItemCode
        {
            get; set;
        }

        public string Mensagem
        {
            get { return this.mensagem; }
            set { this.mensagem = value; }
        }

        public string MesReferencia
        {
            get;set;
        }

        public DateTime MovimentDate
        {
            get; set;
        }

        public string NumberIdentification
        {
            get; set;
        }
    }
}
