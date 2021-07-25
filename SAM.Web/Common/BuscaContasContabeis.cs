using SAM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAM.Web.Common
{
    public class BuscaContasContabeis
    {
        private SAMContext contexto;

        public List<AuxiliaryAccount> GetContaAuxiliarPorGrupoMaterial(int? codigoGrupoMaterial)
        {
            contexto = new SAMContext();
            List<AuxiliaryAccount> query;

            query = (from ac in contexto.AuxiliaryAccounts
                     join rag in contexto.RelationshipAuxiliaryAccountItemGroups on ac.Id equals rag.AuxiliaryAccountId
                     join mg in contexto.MaterialGroups on rag.MaterialGroupId equals mg.Id
                     where mg.Code == codigoGrupoMaterial &&
                           ac.Status == true &&
                           !ac.Description.Contains("Importado do legado")
                     select ac).ToList();

            query.ForEach(o => o.Description = o.ContaContabilApresentacao + " - " + o.Description);

            contexto.Dispose();

            return query.OrderBy(p => p.BookAccount).ToList();
        }

        public List<AuxiliaryAccount> GetContaContabilPorTipo(int tipo)
        {
            contexto = new SAMContext();
            List<AuxiliaryAccount> query;

            query = (from ac in contexto.AuxiliaryAccounts
                                where ac.RelacionadoBP == tipo
                                   && ac.Status == true
                                select ac).ToList();

            query.ForEach(o => o.Description = o.ContaContabilApresentacao + " - " + o.Description);

            contexto.Dispose();
            return query.OrderBy(p => p.BookAccount).ToList();
        }

    }
}