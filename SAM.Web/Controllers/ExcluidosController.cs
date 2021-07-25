using SAM.Web.Common;
using SAM.Web.Common.Enum;
using SAM.Web.Models;
using SAM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SAM.Web.Controllers
{
    public class ExcluidosController : BaseController
    {
        private Hierarquia hierarquia;
        private SAMContext db;

        private int _institutionId;
        private int? _budgetUnitId;
        private int? _managerUnitId;
    

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

        // GET: Excluidos
        public ActionResult Index(string sortOrder, string searchString, int? page, string cbFiltro)
        {
            try
            {
               
                //Guarda o Filtro 
                ViewBag.CurrentFilter = searchString;

                CarregaComboFiltro(cbFiltro);
                ViewBag.CurrentFilterCbFiltro = cbFiltro;

                //ViewBag.PerfilPermissao = PerfilAdmGeral();
                return View();

            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        #region
        [HttpPost]
        public async Task<JsonResult> IndexJSONResult(BPsExcluidos assetEAsset)
        {
            string draw = Request.Form["draw"].ToString();
            string order = Request.Form["order[0][column]"].ToString();
            string orderDir = Request.Form["order[0][dir]"].ToString();
            int startRec = Convert.ToInt32(Request.Form["start"].ToString());
            int length = Convert.ToInt32(Request.Form["length"].ToString());
            string currentFilter = Request.Form["currentFilter"].ToString();
            byte excluido = Request.Form["cbFiltro"] == null ? (byte)0 : Convert.ToByte(Request.Form["cbFiltro"].ToString());
            string hierarquiaLogin = Request.Form["currentHier"].ToString();

            List<BPExclusaoViewModel> lstRetorno;
            int totalRegistros = 0;

            try
            {
                if (hierarquiaLogin.Contains(','))
                {
                    int[] IdsHieraquia = Array.ConvertAll<string, int>(hierarquiaLogin.Split(','), int.Parse);
                    _institutionId = IdsHieraquia[0];
                    _budgetUnitId = IdsHieraquia[1];
                    _managerUnitId = IdsHieraquia[2];
                }
                else
                {
                    getHierarquiaPerfil();
                }

                using (db = new SAMContext())
                {

                    db.Configuration.AutoDetectChangesEnabled = false;
                    db.Configuration.ProxyCreationEnabled = false;
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.Configuration.LazyLoadingEnabled = false;

                    if (string.IsNullOrEmpty(currentFilter) || string.IsNullOrWhiteSpace(currentFilter))
                        lstRetorno = (from s in db.BPsExcluidos
                                      join a in db.Responsibles on s.ResponsibleId equals a.Id
                                      join b in db.AdministrativeUnits on s.AdministrativeUnitId equals b.Id
                                      join c in db.ManagerUnits on s.ManagerUnitId equals c.Id
                                      join d in db.Users on s.LoginAcao equals d.Id
                                      where s.FlagModoExclusao == excluido
                                      select new BPExclusaoViewModel
                                      {
                                          Id = s.Id,
                                          Sigla = s.SiglaInicial,
                                          Chapa = s.Chapa,
                                          ItemMaterial = s.ItemMaterial,
                                          GrupoMaterial = s.GrupoMaterial,
                                          IdUGEAtual = s.ManagerUnitId,
                                          UGECode = c.Code,
                                          UACode = b.Code,
                                          NomeResponsavel = a.Name,
                                          Processo = s.Processo,
                                          ValorDeAquisicao = s.ValorAquisicao,
                                          DataDeAquisicaoCompleta = s.DataAquisicao,
                                          DataIncorporacao = s.DataIncorporacao,
                                          flagAcervo = s.flagAcervo,
                                          flagTerceiro = s.flagTerceiro,
                                          flagDecreto = s.flagDecretoSEFAZ,
                                          DataAcao = s.DataAcao,
                                          Cpf = d.CPF,
                                          NotaLancamentoEstorno = s.NotaLancamentoEstorno,
                                          NotaLancamentoDepreciacaoEstorno = s.NotaLancamentoDepreciacaoEstorno,
                                          NotaLancamentoReclassificacaoEstorno = s.NotaLancamentoReclassificacaoEstorno
                                      }).AsNoTracking().ToList();
                    else
                        lstRetorno = (from s in db.BPsExcluidos
                                      join a in db.Responsibles on s.ResponsibleId equals a.Id
                                      join b in db.AdministrativeUnits on s.AdministrativeUnitId equals b.Id

                                      join c in db.ManagerUnits on s.ManagerUnitId equals c.Id
                                      join d in db.Users on s.LoginAcao equals d.Id
                                      where s.FlagModoExclusao == excluido
                                           && (s.SiglaInicial.Contains(currentFilter) ||
                                            s.Chapa.Contains(currentFilter) ||
                                            s.ItemMaterial.ToString().Contains(currentFilter) ||
                                            s.GrupoMaterial.ToString().Contains(currentFilter) ||
                                            c.Code.ToString().Contains(currentFilter) ||
                                            b.Code.ToString().Contains(currentFilter) ||
                                            a.Name.ToString().Contains(currentFilter) ||
                                            s.Processo.Contains(currentFilter) ||
                                            s.ValorAquisicao.ToString().Contains(currentFilter) ||
                                            s.DataAquisicao.ToString().Contains(currentFilter) ||
                                            s.DataIncorporacao.ToString().Contains(currentFilter) ||
                                            s.DataAcao.ToString().Contains(currentFilter) ||
                                            s.LoginAcao.ToString().Contains(currentFilter) ||
                                            s.NotaLancamentoEstorno.Contains(currentFilter) ||
                                            s.NotaLancamentoDepreciacaoEstorno.Contains(currentFilter) ||
                                            s.NotaLancamentoReclassificacaoEstorno.Contains(currentFilter))


                                      select new BPExclusaoViewModel
                                      {
                                          Id = s.Id,
                                          Sigla = s.SiglaInicial,
                                          Chapa = s.Chapa,
                                          ItemMaterial = s.ItemMaterial,
                                          GrupoMaterial = s.GrupoMaterial,
                                          IdUGEAtual = s.ManagerUnitId,
                                          UGECode = c.Code,
                                          UACode = b.Code,
                                          NomeResponsavel = a.Name,
                                          Processo = s.Processo,
                                          ValorDeAquisicao = s.ValorAquisicao,
                                          DataDeAquisicaoCompleta = s.DataAquisicao,
                                          DataIncorporacao = s.DataIncorporacao,
                                          flagAcervo = s.flagAcervo,
                                          flagTerceiro = s.flagTerceiro,
                                          flagDecreto = s.flagDecretoSEFAZ,
                                          DataAcao = s.DataAcao,
                                          Cpf = d.CPF,
                                          NotaLancamentoEstorno = s.NotaLancamentoEstorno,
                                          NotaLancamentoDepreciacaoEstorno = s.NotaLancamentoDepreciacaoEstorno,
                                          NotaLancamentoReclassificacaoEstorno = s.NotaLancamentoReclassificacaoEstorno
                                      }).AsNoTracking().ToList();
                }
            
                if (PerfilAdmGeral() != true)
                {
                    if (ContemValor(_managerUnitId))
                    lstRetorno = (from s in lstRetorno where s.IdUGEAtual == _managerUnitId select s).ToList();
                    else
                    {
                        var listaDeUges = BuscaUGes(_institutionId, _budgetUnitId);
                        lstRetorno = (from s in lstRetorno join uge in listaDeUges on  s.IdUGEAtual equals uge
                                      select s).ToList();

                    }
                }

                totalRegistros = lstRetorno.Count();
                lstRetorno = lstRetorno.OrderBy(u => u.Chapa).Skip(startRec).Take(length).ToList();

                foreach (var item in lstRetorno)
                {
                    if (item.flagAcervo == true)
                    {
                        item.Tipo = "Acervo";
                    }
                    else if (item.flagTerceiro == true)
                    {
                        item.Tipo = "Terceiro";
                    }
                    else if (item.flagDecreto == true)
                    {
                        item.Tipo = "Decreto";
                    }

                }

                switch (order)
                {
                    case "1":
                        lstRetorno = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? lstRetorno.OrderByDescending(p => p.Sigla).ToList() : lstRetorno.OrderBy(p => p.Sigla).ToList();
                        break;
                    case "2":
                        lstRetorno = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? lstRetorno.OrderByDescending(p => p.ItemMaterial).ToList() : lstRetorno.OrderBy(p => p.ItemMaterial).ToList();
                        break;
                    case "3":
                        lstRetorno = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? lstRetorno.OrderByDescending(p => p.GrupoMaterial).ToList() : lstRetorno.OrderBy(p => p.GrupoMaterial).ToList();
                        break;
                    case "4":
                        lstRetorno = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? lstRetorno.OrderByDescending(p => p.UGECode).ToList() : lstRetorno.OrderBy(p => p.UGECode).ToList();
                        break;
                    case "5":
                        lstRetorno = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? lstRetorno.OrderByDescending(p => p.UACode).ToList() : lstRetorno.OrderBy(p => p.UACode).ToList();
                        break;
                    case "6":
                        lstRetorno = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? lstRetorno.OrderByDescending(p => p.Responsavel).ToList() : lstRetorno.OrderBy(p => p.Responsavel).ToList();
                        break;
                    case "7":
                        lstRetorno = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? lstRetorno.OrderByDescending(p => p.Processo).ToList() : lstRetorno.OrderBy(p => p.Processo).ToList();
                        break;
                    case "8":
                        lstRetorno = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? lstRetorno.OrderByDescending(p => p.ValorDeAquisicao).ToList() : lstRetorno.OrderBy(p => p.ValorDeAquisicao).ToList();
                        break;
                    case "9":
                        lstRetorno = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? lstRetorno.OrderByDescending(p => p.DataDeAquisicaoCompleta).ToList() : lstRetorno.OrderBy(p => p.DataDeAquisicaoCompleta).ToList();
                        break;
                    case "10":
                        lstRetorno = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? lstRetorno.OrderByDescending(p => p.DataIncorporacao).ToList() : lstRetorno.OrderBy(p => p.DataIncorporacao).ToList();
                        break;
                    case "11":
                        lstRetorno = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? lstRetorno.OrderByDescending(p => p.Tipo).ToList() : lstRetorno.OrderBy(p => p.Tipo).ToList();
                        break;
                    case "12":
                        lstRetorno = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? lstRetorno.OrderByDescending(p => p.DataAcao).ToList() : lstRetorno.OrderBy(p => p.DataAcao).ToList();
                        break;
                    case "13":
                        lstRetorno = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? lstRetorno.OrderByDescending(p => p.Cpf).ToList() : lstRetorno.OrderBy(p => p.Cpf).ToList();
                        break;
                    case "14":
                        lstRetorno = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? lstRetorno.OrderByDescending(p => p.NotaLancamentoEstorno).ToList() : lstRetorno.OrderBy(p => p.NotaLancamentoEstorno).ToList();
                        break;
                    case "15":
                        lstRetorno = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? lstRetorno.OrderByDescending(p => p.NotaLancamentoDepreciacaoEstorno).ToList() : lstRetorno.OrderBy(p => p.NotaLancamentoDepreciacaoEstorno).ToList();
                        break;
                    case "16":
                        lstRetorno = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? lstRetorno.OrderByDescending(p => p.NotaLancamentoReclassificacaoEstorno).ToList() : lstRetorno.OrderBy(p => p.NotaLancamentoReclassificacaoEstorno).ToList();
                        break;
                    default:
                        lstRetorno = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? lstRetorno.OrderByDescending(p => p.Chapa).ToList() : lstRetorno.OrderBy(p => p.Chapa).ToList();
                        break;
                }

                var result = lstRetorno.ToList();

                return Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRegistros, recordsFiltered = totalRegistros, data = result }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(MensagemErro(CommonMensagens.PadraoException, ex), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        private bool ContemValor(int? variavel)
        {
            bool retorno = false;
            if (variavel.HasValue && variavel != null && variavel != 0)
                retorno = true;
            return retorno;
        }


        #region View Methods
        private void CarregaComboFiltro(string codFiltro)
        {
            var lista = new List<ItemGenericViewModel>();

            var itemGeneric = new ItemGenericViewModel
            {
                Id = 1,
                Description = "Excluidos",
                Ordem = 10
            };
            lista.Add(itemGeneric);

            itemGeneric = new ItemGenericViewModel
            {
                Id = 0,
                Description = "Estornados",
                Ordem = 5
            };
            lista.Add(itemGeneric);

            if (codFiltro != null && codFiltro != "")
                ViewBag.Filtros = new SelectList(lista.OrderBy(x => x.Ordem), "Id", "Description", int.Parse(codFiltro));
            else
                ViewBag.Filtros = new SelectList(lista.OrderBy(x => x.Ordem), "Id", "Description");

        }
        
        private List<int> BuscaUGes(int orgao, int? uo)
        {
            return (from i in db.Institutions
                         join b in db.BudgetUnits on i.Id equals b.InstitutionId
                         join m in db.ManagerUnits on b.Id equals m.BudgetUnitId
                         where i.Id == orgao && (uo == null || b.Id == uo || uo == 0)
                         select m.Id).ToList();

        }
        #endregion

    }
}