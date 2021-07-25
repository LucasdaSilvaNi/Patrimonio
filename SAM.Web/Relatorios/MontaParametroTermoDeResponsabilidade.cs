using Microsoft.Reporting.WebForms;
using SAM.Web.Models;
using SAM.Web.ViewModels;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;

namespace SAM.Web.Relatorios
{
    public class MontaParametroTermoDeResponsabilidade
    {
        public DataTable dados { get; set; }
        public string descricaoOrgao { get; set; }
        public string descricaoUGE { get; set; }
        public string descricaoUO { get; set; }
        public string UACompleto { get; set; }
        public string nomeResponsavel { get; set; }
        public string caminhoRelatorio { get; set; }
        private SAMContext db { get;set;}
        public MontaParametroTermoDeResponsabilidade(RelatorioTermoDeResponsabilidadeViewModel param)
        {
            dados = new BuscaDadosTermoDeResponsabilidade(param).dtRetornoDados;

        }

        public void BuscaDados(RelatorioTermoDeResponsabilidadeViewModel param) {
            using (db = new SAMContext())
            {
                descricaoOrgao = db.Institutions.Find(param.InstitutionId).Description;
                descricaoUO = db.BudgetUnits.Find(param.BudgetUnitId).Description;

                descricaoUGE = db.ManagerUnits.Find(param.ManagerUnitId).Description;

                AdministrativeUnit ua = db.AdministrativeUnits.Find(param.AdministrativeUnitId);

                if(ua == null)
                    UACompleto = string.Empty;
                else
                    UACompleto = string.Format("{0} - {1}", ua.Code, ua.Description);

                nomeResponsavel = db.Responsibles.Find(param.idResponsable).Name;
            }
        }
    }
}