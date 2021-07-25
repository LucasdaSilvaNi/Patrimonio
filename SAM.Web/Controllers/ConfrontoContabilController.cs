using SAM.Web.Models;
using SAM.Web.Context;
using SAM.Web.Common;
using SAM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using System.Text;
using System.Data.Entity;

namespace SAM.Web.Controllers
{
    public class ConfrontoContabilController : BaseController
    {
        SAMContext contexto;
        HierarquiaContext contextoHierarquia;
        private int _institutionId;
        private int? _budgetUnitId;
        private int? _managerUnitId;

        // GET: ConfrontoContabil
        public void getHierarquiaPerfil()
        {
            if (HttpContext == null || HttpContext.Items["RupId"] == null)
            {
                User u = UserCommon.CurrentUser();
                var perflLogado = BuscaHierarquiaPerfilLogadoPorUsuario(u.Id);
                _institutionId = perflLogado.InstitutionId;
                _budgetUnitId = perflLogado.BudgetUnitId;
                _managerUnitId = perflLogado.ManagerUnitId;
            }
            else
            {
                var perflLogado = BuscaHierarquiaPerfilLogado((int)HttpContext.Items["RupId"]);
                _institutionId = perflLogado.InstitutionId;
                _budgetUnitId = perflLogado.BudgetUnitId;
                _managerUnitId = perflLogado.ManagerUnitId;
            }
        }

        public ActionResult Index()
        {
            try
            {
                getHierarquiaPerfil();
                CarregaComboBoxes(_institutionId, _budgetUnitId ?? 0, _managerUnitId ?? 0);

                ConfrontoContabilViewModel model = new ConfrontoContabilViewModel();
                model.InstitutionId = _institutionId;
                model.BudgetUnitId = _budgetUnitId;
                model.ManagerUnitId = _managerUnitId;

                return View(model);
            }
            catch (Exception e) {
                GravaLogErroSemRetorno(e);
                return MensagemErro(CommonMensagens.SistemaInstavel);
            }
        }

        [HttpPost]
        public ActionResult GerarExcel(ConfrontoContabilViewModel viewmodel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    AtualizacoesContabeis(viewmodel.InstitutionId, viewmodel.BudgetUnitId, viewmodel.ManagerUnitId);
                    viewmodel.BudgetUnitId = viewmodel.BudgetUnitId == null ? 0 : viewmodel.BudgetUnitId;
                    viewmodel.ManagerUnitId = viewmodel.ManagerUnitId == null ? 0 : viewmodel.ManagerUnitId;

                    List<DadosSIAFEMContaContabilViewModel> result = BuscaDadosParaExcel(viewmodel.InstitutionId, viewmodel.BudgetUnitId, viewmodel.ManagerUnitId, viewmodel.MesRef);

                    if (result != null && result.Count > 0)
                    {
                        var objMandaParaDetaConta = new IntegracaoContabilizaSPController();
                        var resultado = objMandaParaDetaConta.obtemDadosContabeisNoSIAFEM(result);

                        GridView gv = new GridView();

                        gv.AutoGenerateColumns = false;
                        gv.Columns.Add(new BoundField { HeaderText = "Orgão", DataField = "Orgao" });
                        gv.Columns.Add(new BoundField { HeaderText = "UO", DataField = "UO" });
                        gv.Columns.Add(new BoundField { HeaderText = "UGE", DataField = "UGE" });
                        gv.Columns.Add(new BoundField { HeaderText = "Mês referência", DataField = "Mes" });
                        gv.Columns.Add(new BoundField { HeaderText = "Ano referência", DataField = "Ano" });
                        gv.Columns.Add(new BoundField { HeaderText = "Conta", DataField = "Conta" });
                        gv.Columns.Add(new BoundField { HeaderText = "Descrição da Conta", DataField = "DescricaoConta" });
                        gv.Columns.Add(new BoundField { HeaderText = "Valor Contábil SAM", DataField = "ValorContabilSAM" });
                        gv.Columns.Add(new BoundField { HeaderText = "Valor Contábil SIAFEM", DataField = "ValorContabilSIAFEM" });
                        gv.Columns.Add(new BoundField { HeaderText = "Diferença (SAM - SIAFEM)", DataField = "Diferenca" });

                        gv.DataSource = resultado;
                        gv.DataBind();

                        Response.ClearContent();
                        Response.Buffer = true;
                        Response.AddHeader("content-disposition", string.Format("attachment;filename=ConfrontoContabil_SAM_DetaConta_{0}.xls", DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")));
                        Response.ContentType = "application/ms-excel";
                        Response.Charset = "";
                        StringWriter sw = new StringWriter();
                        HtmlTextWriter htw = new HtmlTextWriter(sw);
                        gv.RenderControl(htw);
                        Response.Output.Write(sw.ToString());
                        Response.Flush();
                        Response.End();
                    }
                }

                if (!ModelState.IsValid)
                {
                    getHierarquiaPerfil();
                    CarregaComboBoxes(_institutionId, _budgetUnitId ?? 0, _managerUnitId ?? 0);
                }

                return View("Index", viewmodel);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        #region Métodos de Atualização

        private void AtualizacoesContabeis(int IdOrgao, int? IdUO, int? IdUGE) {
            using (contexto = new SAMContext()) {
                IdUO = IdUO == 0 ? null : IdUO;
                IdUGE = IdUGE == 0 ? null : IdUGE;

                var lista = contexto.HouveAlteracaoContabeis
                                    .Where(h => h.IdOrgao == IdOrgao &&
                                                (h.IdUO == IdUO || IdUO == null) &&
                                                (h.IdUGE == IdUGE || IdUGE == null) &&
                                                h.HouveAlteracao).ToList();
                
                if (lista.Count > 0) {
                    StringBuilder builder = new StringBuilder();
                    foreach (var item in lista) {
                        try
                        {
                            StringBuilder builderParametros = new StringBuilder();

                            builderParametros.Append("@IdOrgao =" + item.IdOrgao.ToString());
                            builderParametros.Append(",@IdUO =" + item.IdUO.ToString());
                            builderParametros.Append(",@IdUGE =" + item.IdUGE.ToString());
                            builderParametros.Append(",@IdContaContabil =" + item.IdContaContabil.ToString());

                            builder.Clear().Append("EXEC [ATUALIZA_SALDO_VISAO_GERAL_POR_CONTA_CONTABIL_HOJE] ");
                            builder.Append(builderParametros);

                            contexto.Database.ExecuteSqlCommand(builder.ToString());

                            builder.Clear().Append("EXEC [ATUALIZA_SALDO_VISAO_GERAL_POR_CONTA_DEPRECIACAO_HOJE] ");
                            builder.Append(builderParametros);

                            contexto.Database.ExecuteSqlCommand(builder.ToString());

                            item.HouveAlteracao = false;
                            contexto.Entry(item).State = EntityState.Modified;
                            contexto.SaveChanges();
                        }
                        catch (Exception e) {
                            GravaLogErroSemRetorno(e);
                        }
                    }
                }
            }
        }

        #endregion


        #region BuscaDados
        private List<DadosSIAFEMContaContabilViewModel> BuscaDadosParaExcel(int IdOrgao, int? IdUO, int? IdUGE, string MesRef)
        {
            if (Convert.ToInt32(MesRef) == 0)
            {
                return BuscaDadosAtuais(IdOrgao, IdUO, IdUGE);
            }
            else {
                return BuscaDadosPorMesRef(IdOrgao, IdUO, IdUGE, MesRef);
            }
        }

        private List<DadosSIAFEMContaContabilViewModel> BuscaDadosAtuais(int IdOrgao, int? IdUO, int? IdUGE) {
            StringBuilder builderParametros = new StringBuilder();

            builderParametros.Append("@IdOrgao =" + IdOrgao.ToString());
            if (IdUO != null && IdUO != 0)
            {
                builderParametros.Append(",@IdUO =" + IdUO.ToString());
            }
            if (IdUGE != null && IdUGE != 0)
            {
                builderParametros.Append(",@IdUGE =" + IdUGE.ToString());
            }

            StringBuilder builder = new StringBuilder();

            builder.Append("EXEC [SALDO_VISAO_GERAL_POR_CONTA_CONTABIL_HOJE] ");
            builder.Append(builderParametros);

            List<DadosSIAFEMContaContabilViewModel> result = null;
            try
            {
                using (contextoHierarquia = new HierarquiaContext())
                {
                    result = contextoHierarquia.Database.SqlQuery<DadosSIAFEMContaContabilViewModel>(builder.ToString()).ToList();
                }

                if (result != null && result.Count > 0)
                {
                    builder.Clear().Append("EXEC [SALDO_VISAO_GERAL_POR_CONTA_DEPRECIACAO_HOJE] ");
                    builder.Append(builderParametros);

                    using (contextoHierarquia = new HierarquiaContext())
                    {
                        var listaContaDepreciacao = contextoHierarquia.Database.SqlQuery<DadosSIAFEMContaContabilViewModel>(builder.ToString()).ToList();
                        result.AddRange(listaContaDepreciacao);
                    }
                }
                else
                {
                    ModelState.AddModelError("ManagerUnitId", "Não foram encontrados dados para essa pesquisa");
                }
            }
            catch (Exception e)
            {
                GravaLogErroSemRetorno(e);
                ModelState.AddModelError("ManagerUnitId", "Não foi possível buscar os dados atuais nesse momento. Por gentileza, tente novamente mais tarde");
            }

            return result;
       }


        private List<DadosSIAFEMContaContabilViewModel> BuscaDadosPorMesRef(int IdOrgao, int? IdUO, int? IdUGE, string MesRef)
        {
            StringBuilder builderParametros = new StringBuilder();
            builderParametros.Append("@IdOrgao =" + IdOrgao.ToString());
            builderParametros.Append(",@MesRef ='" + MesRef+"'");
            if (IdUO != null && IdUO != 0)
            {
                builderParametros.Append(",@IdUO =" + IdUO.ToString());
            }
            if (IdUGE != null && IdUGE != 0)
            {
                builderParametros.Append(",@IdUGE =" + IdUGE.ToString());
            }

            StringBuilder builder = new StringBuilder();

            builder.Append("EXEC [SAM_BUSCA_VALORES_FECHAMENTO_UGES_COM_MES_REF_FECHADO] ");
            builder.Append(builderParametros);

            List<DadosSIAFEMContaContabilViewModel> result = null;
            try
            {
                using (contextoHierarquia = new HierarquiaContext())
                {
                    result = contextoHierarquia.Database.SqlQuery<DadosSIAFEMContaContabilViewModel>(builder.ToString()).ToList();
                }

                if (result.Count == 0)
                {
                    ModelState.AddModelError("ManagerUnitId", "Não foram encontrados dados para essa pesquisa");
                }
            }
            catch (Exception e) {
                GravaLogErroSemRetorno(e);
                ModelState.AddModelError("ManagerUnitId", "Não foi possível buscar os dados do fechamento nesse momento. Por gentileza, tente novamente mais tarde");
            }

            return result;
        }

        #endregion

        #region Combo
        private void CarregaComboBoxes(int IdOrgao = 0, int IdUO = 0, int IdUGE = 0, int IdContaDepreciacao = 0)
        {

            using (contextoHierarquia = new HierarquiaContext())
            {
               ViewBag.Institutions = new SelectList(GetOrgaosIntegrados(IdOrgao), "Id", "Description", IdOrgao);

               if (IdUO != 0)
                   ViewBag.BudgetUnits = new SelectList(GetUos(IdUO), "Id", "Description", IdUO);
               else
                   ViewBag.BudgetUnits = new SelectList(GetUosPorOrgaoId(IdOrgao), "Id", "Description", IdUO);

               if (IdUGE != 0)
                   ViewBag.ManagerUnits = new SelectList(GetUGEIntegradas(IdUGE), "Id", "Description", IdUGE);
               else
                   ViewBag.ManagerUnits = new SelectList(GetUgesIntegradasPorUoId(IdUO), "Id", "Description", IdUGE);
            }
        }

        private List<Institution> GetOrgaosIntegrados(int IdDoOrgao)
        {
            List<Institution> query;
            if (IdDoOrgao != 0)
            {
                query = (from i in contextoHierarquia.Institutions
                         where i.flagIntegracaoSiafem
                         && i.Id == IdDoOrgao
                         select i).ToList();
            }
            else {
                query = (from i in contextoHierarquia.Institutions
                         where i.flagIntegracaoSiafem
                         select i).ToList();
            }

            query.ForEach(o => o.Description = o.Code + " - " + o.Description);

            return query.ToList().OrderBy(o => int.Parse(o.Code)).ToList();
        }

        private List<BudgetUnit> GetUos(int? IdUO = null)
        {
            List<BudgetUnit> query;

            if (IdUO != null && IdUO != 0)
            {
                    query = (from b in contextoHierarquia.BudgetUnits
                             where b.Id == IdUO && b.Status == true
                             select b).ToList();

                    query.ForEach(o => o.Description = o.Code + " - " + o.Description);
            }
            else
            {
                query = new List<BudgetUnit>();
                BudgetUnit budgetUnit = new BudgetUnit();
                budgetUnit.Id = 0;
                budgetUnit.Code = "0";
                budgetUnit.Description = "Selecione a UO";
                query.Add(budgetUnit);
            }

            return query.ToList().OrderBy(u => int.Parse(u.Code)).ToList();
        }

        public List<BudgetUnit> GetUosPorOrgaoId(int _institutionId)
        {
            List<BudgetUnit> query;

            if (_institutionId != 0)
            {
                query = (from b in contextoHierarquia.BudgetUnits
                         where b.InstitutionId == _institutionId
                            && b.Status == true
                         select b).ToList();
            }
            else {
                query = new List<BudgetUnit>();
            }

            query.ForEach(o => o.Description = o.Code + " - " + o.Description);

            BudgetUnit budgetUnit = new BudgetUnit();
            budgetUnit.Id = 0;
            budgetUnit.Code = "0";
            budgetUnit.Description = "Selecione a UO";

            query.Add(budgetUnit);

            return query.ToList().OrderBy(u => int.Parse(u.Code)).ToList();
        }
        private List<ManagerUnit> GetUGEIntegradas(int? IdUge = null)
        {
            List<ManagerUnit> query;

            if (IdUge.HasValue && IdUge != 0)
            {
                    query = (from m in contextoHierarquia.ManagerUnits
                             where m.Id == IdUge
                             && m.Status
                             && m.FlagIntegracaoSiafem
                             select m).ToList();
                    query.ForEach(o => o.Description = o.Code + " - " + o.Description);
            }
            else
            {
                query = new List<ManagerUnit>();
                ManagerUnit managerUnit = new ManagerUnit();

                managerUnit.Id = 0;
                managerUnit.Code = "0";
                managerUnit.Description = "Selecione a UGE";
                query.Add(managerUnit);
            }

            return query.ToList().OrderBy(uge => int.Parse(uge.Code)).ToList();
        }

        public List<ManagerUnit> GetUgesIntegradasPorUoId(int? _budgetUnitId)
        {
            List<ManagerUnit> query;

            if (_budgetUnitId.HasValue && _budgetUnitId != 0)
            {
                query = (from m in contextoHierarquia.ManagerUnits
                         where m.BudgetUnitId == _budgetUnitId 
                         && m.Status == true
                         && m.FlagIntegracaoSiafem == true
                         select m).ToList();

                query.ForEach(o => o.Description = o.Code + " - " + o.Description);
            }
            else
            {
                query = new List<ManagerUnit>();

            }

            ManagerUnit managerUnit = new ManagerUnit();
            managerUnit.Id = 0;
            managerUnit.Code = "0";
            managerUnit.Description = "Selecione a UGE";
            query.Add(managerUnit);

            return query.ToList().OrderBy(uge => int.Parse(uge.Code)).ToList();
        }
        #endregion

    }
}