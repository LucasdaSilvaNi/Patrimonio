using SAM.Web.Models;
using SAM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAM.Web.Incorporacao
{
    public class BuscaNumerosDeDocumentos
    {
        private SAMContext contexto;
        private int idUO { get; set; }
        private int idUGE { get; set; }

        public BuscaNumerosDeDocumentos() { }

        public BuscaNumerosDeDocumentos(SAMContext contexto, int idUO, int idUGE)
        {
            this.contexto = contexto;
            this.idUO = idUO;
            this.idUGE = idUGE;
        }

        public IQueryable<NumeroDocumentosIncorpEmLote> TipoMovimentacaoEmEspecifico(int tipoIncorporacao, string pesquisa = null) {
            try
            {
                if (contexto == null)
                    contexto = new SAMContext();

                if (idUO <= 0 && idUGE <= 0)
                    return null;


                int movimentacaoRelacionada = contexto.MovementTypes.Find(tipoIncorporacao).TipoMovimentacaoRelacionada ?? 0;

                IQueryable<NumeroDocumentosIncorpEmLote> lista;

                if (string.IsNullOrEmpty(pesquisa) || string.IsNullOrWhiteSpace(pesquisa))
                    lista = (from a in contexto.Assets
                         join am in contexto.AssetMovements
                         on a.Id equals am.AssetId
                         join m in contexto.ManagerUnits
                         on am.SourceDestiny_ManagerUnitId equals m.Id
                         where am.MovementTypeId == movimentacaoRelacionada
                         && am.Status
                         && am.FlagEstorno != true
                         && m.BudgetUnitId == idUO
                         && (idUGE <= 0 || m.Id == idUGE)
                         select new NumeroDocumentosIncorpEmLote {
                             NumeroDeDocumento = am.NumberDoc,
                             OrigemUGE = m.Code
                         }).Distinct().OrderBy(am => am.NumeroDeDocumento).AsQueryable();
                else
                    lista = (from a in contexto.Assets
                             join am in contexto.AssetMovements
                             on a.Id equals am.AssetId
                             join m in contexto.ManagerUnits
                             on am.SourceDestiny_ManagerUnitId equals m.Id
                             where am.MovementTypeId == movimentacaoRelacionada
                             && am.Status
                             && am.FlagEstorno != true
                             && m.BudgetUnitId == idUO
                             && (idUGE <= 0 || m.Id == idUGE)
                             && (am.NumberDoc.Contains(pesquisa) || m.Code.Contains(pesquisa))
                             select new NumeroDocumentosIncorpEmLote
                             {
                                 NumeroDeDocumento = am.NumberDoc,
                                 OrigemUGE = m.Code
                             }).Distinct().OrderBy(am => am.NumeroDeDocumento).AsQueryable();

                return lista;

            } catch (Exception e) {
                throw e;
            }
        }
    }
}