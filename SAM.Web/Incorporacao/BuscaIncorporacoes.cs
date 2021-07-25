using SAM.Web.Common.Enum;
using SAM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAM.Web.Incorporacao
{
    public class BuscaIncorporacoesAtivas
    {
        private SAMContext db;
        public List<MovementType> listaIncorporacoes;
        public BuscaIncorporacoesAtivas(SAMContext param, int IdOrgao){
            db = param;

            if (db == null)
                db = new SAMContext();

            listaIncorporacoes = db.MovementTypes.Where(m => m.GroupMovimentId == (int)EnumGroupMoviment.Incorporacao && m.Status == true).OrderBy(m => m.Description).ToList();

            if (!OrgaoFundoSocial(IdOrgao))
                listaIncorporacoes.RemoveAll(
                        e => e.Id == (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGEDoacao
                          || e.Id == (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGETranferencia);

        }

        public List<MovementType> BuscaAceitesEmLote()
        {
            return listaIncorporacoes.Where(i => i.IncorporacaoEmLote == true).ToList();
        }

        public List<MovementType> BuscaIncorporacoesSimples()
        {
            return listaIncorporacoes.Where(i => i.IncorporacaoSimples == true).ToList();
        }

        private bool OrgaoFundoSocial(int IdOrgao)
        {
            return (from i in db.Institutions
                    where i.Id == IdOrgao
                    && i.Code == "51004"
                    select i.Id).Any();
        }
    }
}