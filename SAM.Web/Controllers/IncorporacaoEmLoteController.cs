using SAM.Web.Common;
using SAM.Web.Models;
using SAM.Web.Incorporacao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SAM.Web.Controllers
{
    public class IncorporacaoEmLoteController : BaseController
    {
        private SAMContext db;
        private Hierarquia hierarquia;

        private int _institutionId;
        private int? _budgetUnitId;
        private int? _managerUnitId;
        private int? _administrativeUnitId;
        private int? _sectionId;

        public void getHierarquiaPerfil()
        {
            if (HttpContext == null || HttpContext.Items["RupId"] == null)
            {
                User u = UserCommon.CurrentUser();
                var perflLogado = BuscaHierarquiaPerfilLogadoPorUsuario(u.Id);
                _institutionId = perflLogado.InstitutionId;
                _budgetUnitId = perflLogado.BudgetUnitId;
                _managerUnitId = perflLogado.ManagerUnitId;
                _administrativeUnitId = perflLogado.AdministrativeUnitId;
                _sectionId = perflLogado.SectionId;
            }
            else
            {
                var perflLogado = BuscaHierarquiaPerfilLogado((int)HttpContext.Items["RupId"]);
                _institutionId = perflLogado.InstitutionId;
                _budgetUnitId = perflLogado.BudgetUnitId;
                _managerUnitId = perflLogado.ManagerUnitId;
                _administrativeUnitId = perflLogado.AdministrativeUnitId;
                _sectionId = perflLogado.SectionId;
            }
        }

        public ActionResult Index()
        {
            try
            {
                getHierarquiaPerfil();

                using (db = new SAMContext())
                {
                    if (UGEEstaComPendenciaSIAFEMNoFechamento(_managerUnitId))
                        return MensagemErro(CommonMensagens.OperacaoInvalidaIntegracaoFechamento);

                    CarregaViewBagTipoMovimento();

                    return View();
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        public JsonResult BuscaNumerosDeDocumentosPorIncorporacao(int tipoMovimentacao)
        {
            try
            {
                string draw = Request.Form["draw"].ToString();
                int startRec = Convert.ToInt32(Request.Form["start"].ToString());
                int length = Convert.ToInt32(Request.Form["length"].ToString());
                string pesquisa = Request.Form["pesquisa"].ToString();
                string hierarquiaLogin = Request.Form["currentHier"].ToString();

                if (hierarquiaLogin.Contains(','))
                {
                    int[] IdsHieraquia = Array.ConvertAll<string, int>(hierarquiaLogin.Split(','), int.Parse);
                    _budgetUnitId = IdsHieraquia[1];
                    _managerUnitId = IdsHieraquia[2];
                }
                else
                {
                    getHierarquiaPerfil();
                }

                var objbusca = new BuscaNumerosDeDocumentos(new SAMContext(), _budgetUnitId ?? 0, _managerUnitId ?? 0);
                var lista = objbusca.TipoMovimentacaoEmEspecifico(tipoMovimentacao);

                int totalRegistros = lista.Count();
                var resultado = lista.Skip(startRec).Take(length);

                return Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRegistros, recordsFiltered = totalRegistros, data = resultado }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) {
                GravaLogErroSemRetorno(ex);
                return Json(CommonMensagens.PadraoException, JsonRequestBehavior.AllowGet);
            }
        }

        #region Carrega ViewBags
        private void CarregaViewBagTipoMovimento() {
            ViewBag.MovementTypes = new SelectList(new BuscaIncorporacoesAtivas(db, _institutionId).BuscaAceitesEmLote(), "Id", "Description");
        }
        #endregion

    }
}