using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using Sam.Common;
using Sam.Common.Util;
using SAM.Web.Common;
using SAM.Web.Common.Enum;
using SAM.Web.Context;
using SAM.Web.Models;
using SAM.Web.ViewModels;






namespace SAM.Web.Controllers
{
    public class SupportController : BaseController
    {
        private Hierarquia hierarquia;
        private SupportContext db;

        private int _institutionId;
        private int? _budgetUnitId;
        private int? _managerUnitId;

        private readonly string cstEMailParaEnvioSuporteSAM = EnumVariaveisWebConfig.eMailParaEnvioSuporteSam;

        public void getHierarquiaPerfil()
        {
            User u = UserCommon.CurrentUser();
            var perflLogado = BuscaHierarquiaPerfilLogadoPorUsuario(u.Id);
            _institutionId = perflLogado.InstitutionId;
            _budgetUnitId = perflLogado.BudgetUnitId;
            _managerUnitId = perflLogado.ManagerUnitId;
        }

        #region Index
        [HttpGet]
        public ActionResult Index(int? InstitutionId, int? BudgetUnitId, int? ManagerUnitId, int? SupportStatusProdespId, int? SupportStatusUserId, int? SupportId, int? page, int? valida)
        {
            try
            {
                getHierarquiaPerfil();
                CarregaHierarquia(_institutionId != 0 ? _institutionId : 0,
                              _budgetUnitId != null && _budgetUnitId != 0 ? (int)_budgetUnitId : 0,
                              _managerUnitId != null && _managerUnitId != 0 ? (int)_managerUnitId : 0);

                using (db = new SupportContext())
                {
                    CarregaStatusProdesp();
                    CarregaStatusUser();
                }

                SupportConsultViewModel model = new SupportConsultViewModel();

                model.InstitutionId = _institutionId != 0 ? _institutionId : 0;
                model.BudgetUnitId = _budgetUnitId != null && _budgetUnitId != 0 ? (int)_budgetUnitId : 0;
                model.ManagerUnitId = _managerUnitId != null && _managerUnitId != 0 ? (int)_managerUnitId : 0;
                model.AdmGeral = PerfilAdmGeral();

                return View(model);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        public async Task<JsonResult> IndexJSONResult(SupportConsultViewModel supportConsult)
        {
            string draw = Request.Form["draw"].ToString();
            int startRec = Convert.ToInt32(Request.Form["start"].ToString());
            int length = Convert.ToInt32(Request.Form["length"].ToString());

            int? BudgetUnitId = retornaDadosDaRequisicao(Request.Form["unidadeOrcamentaria"]);
            int? ManagerUnitId = retornaDadosDaRequisicao(Request.Form["unidadeGerencial"]);
            int? institutionId = retornaDadosDaRequisicao(Request.Form["institution"]);
            int? supportStatusProdespId = retornaDadosDaRequisicao(Request.Form["statusProdesp"]);
            int? supportStatusUserId = retornaDadosDaRequisicao(Request.Form["statusUsuario"]);
            int? SupportId = null;
            int paraValidacao;
            string hierarquiaLogin = Request.Form["currentHier"].ToString();

            if (Request.Form["nChamados"] != null && int.TryParse(Request.Form["nChamados"], out paraValidacao)) {
                SupportId = Convert.ToInt32(Request.Form["nChamados"]);
            }

            int totalRegistros = 0;
            IQueryable<SupportGridViewModel> supportGridViewModelDB = null;

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

                supportConsult.AdmGeral = PerfilAdmGeral();
                List<int> suporteProdesp = BuscaUsuariosSuporteProdesp();

                db = new SupportContext();

                if (supportConsult.AdmGeral &&
                   !string.IsNullOrEmpty(supportConsult.historicoContenha) &&
                   !string.IsNullOrWhiteSpace(supportConsult.historicoContenha))
                {
                    var suporteComPalavra = (from h in db.HistoricSupports
                                             where h.Observation.Contains(supportConsult.historicoContenha)
                                             select h.SupportId).Distinct();

                    if (supportStatusProdespId == null && supportStatusUserId == null)
                    {
                        supportGridViewModelDB = (from s in db.Supports.Include("RelatedModule.RelatedModule.RelatedManagedSystem").Include("RelatedSupportStatusUser")
                                                                       .Include("RelatedSupportStatusProdesp")
                                                                       .Include("RelatedUser")
                                                  join m in db.ManagerUnits.Include("RelatedBudgetUnit.RelatedInstitution") on s.ManagerUnitId equals m.Id
                                                  join scp in suporteComPalavra on s.Id equals scp
                                                  where
                                                        s.Id == (SupportId != null ? SupportId : s.Id) &&
                                                        (supportConsult.AdmGeral || s.InstitutionId == (institutionId.HasValue && institutionId != 0 ? institutionId : s.InstitutionId)) &&
                                                        s.BudgetUnitId == (BudgetUnitId.HasValue && BudgetUnitId != 0 ? BudgetUnitId : s.BudgetUnitId) &&
                                                        s.ManagerUnitId == (ManagerUnitId.HasValue && ManagerUnitId != 0 ? ManagerUnitId : s.ManagerUnitId) &&
                                                        (s.SupportStatusProdespId != (int)EnumSupportStatusProdesp.Finalizado || s.SupportStatusUserId != (int)EnumSupportStatusUser.Concluido) &&
                                                        s.RelatedModule.Status == true &&
                                                        s.RelatedModule.ParentId != null
                                                  select new SupportGridViewModel()
                                                  {
                                                      Id = s.Id,
                                                      NameManagerReduced = m.RelatedBudgetUnit.RelatedInstitution.NameManagerReduced,
                                                      BudgetUnitCode = m.RelatedBudgetUnit.Code,
                                                      ManagerUnitCode = m.Code,
                                                      UserDescription = s.RelatedUser.Name,
                                                      InclusionDate = s.InclusionDate,
                                                      LastModifyDate = s.DataLogin,
                                                      UserCPF = s.RelatedUser.CPF,
                                                      Functionanality = s.RelatedModule.RelatedModule.RelatedManagedSystem.Name + " - " + s.RelatedModule.RelatedModule.Name + " - " + s.RelatedModule.Name,// ma.Name + " - " + mp.Name + " - " + m.Name,
                                                      SupportTypeDescription = s.RelatedSupportType.Description,
                                                      SupportStatusUserDescription = s.RelatedSupportStatusUser.Description,
                                                      SupportStatusProdespId = s.SupportStatusProdespId,
                                                      SupportStatusProdespDescription = s.RelatedSupportStatusProdesp.Description,
                                                      Responsavel = s.Responsavel,
                                                      CloseDate = s.CloseDate,
                                                      UltimoAtendimento = suporteProdesp.Contains((from h in db.HistoricSupports
                                                                                                   where s.Id == h.SupportId
                                                                                                   orderby h.InclusionDate descending
                                                                                                   select h).FirstOrDefault().UserId) ? (int)EnumAttendance.Prodesp : (int)EnumAttendance.Usuario
                                                  }).AsNoTracking().AsQueryable().OrderBy(s => s.Id);
                    }
                    else
                    {
                        supportGridViewModelDB = (from s in db.Supports.Include("RelatedModule.RelatedModule.RelatedManagedSystem").Include("RelatedSupportStatusUser")
                                                                       .Include("RelatedSupportStatusProdesp")
                                                                       .Include("RelatedUser")
                                                  join m in db.ManagerUnits.Include("RelatedBudgetUnit.RelatedInstitution") on s.ManagerUnitId equals m.Id
                                                  join scp in suporteComPalavra on s.Id equals scp
                                                  where
                                                        s.Id == (SupportId != null ? SupportId : s.Id) &&
                                                        (supportConsult.AdmGeral || s.InstitutionId == (institutionId.HasValue && institutionId != 0 ? institutionId : s.InstitutionId) &&
                                                        s.BudgetUnitId == (BudgetUnitId.HasValue && BudgetUnitId != 0 ? BudgetUnitId : s.BudgetUnitId) &&
                                                        s.ManagerUnitId == (ManagerUnitId.HasValue && ManagerUnitId != 0 ? ManagerUnitId : s.ManagerUnitId)) &&
                                                        s.SupportStatusProdespId == (supportStatusProdespId != null ? supportStatusProdespId : s.SupportStatusProdespId) &&
                                                        s.SupportStatusUserId == (supportStatusUserId != null ? supportStatusUserId : s.SupportStatusUserId) &&
                                                        s.RelatedModule.Status == true &&
                                                        s.RelatedModule.ParentId != null
                                                  select new SupportGridViewModel()
                                                  {
                                                      Id = s.Id,
                                                      NameManagerReduced = m.RelatedBudgetUnit.RelatedInstitution.NameManagerReduced,
                                                      BudgetUnitCode = m.RelatedBudgetUnit.Code,
                                                      ManagerUnitCode = m.Code,
                                                      UserDescription = s.RelatedUser.Name,
                                                      InclusionDate = s.InclusionDate,
                                                      LastModifyDate = s.DataLogin,
                                                      UserCPF = s.RelatedUser.CPF,
                                                      Functionanality = s.RelatedModule.RelatedModule.RelatedManagedSystem.Name + " - " + s.RelatedModule.RelatedModule.Name + " - " + s.RelatedModule.Name,// ma.Name + " - " + mp.Name + " - " + m.Name,
                                                      SupportTypeDescription = s.RelatedSupportType.Description,
                                                      SupportStatusUserDescription = s.RelatedSupportStatusUser.Description,
                                                      SupportStatusProdespId = s.SupportStatusProdespId,
                                                      SupportStatusProdespDescription = s.RelatedSupportStatusProdesp.Description,
                                                      Responsavel = s.Responsavel,
                                                      CloseDate = s.CloseDate,
                                                      UltimoAtendimento = suporteProdesp.Contains((from h in db.HistoricSupports
                                                                                                   where s.Id == h.SupportId
                                                                                                   orderby h.InclusionDate descending
                                                                                                   select h).FirstOrDefault().UserId) ? (int)EnumAttendance.Prodesp : (int)EnumAttendance.Usuario
                                                  }).AsNoTracking().AsQueryable();
                    }
                }
                else
                {
                    if (supportStatusProdespId == null && supportStatusUserId == null)
                    {
                        supportGridViewModelDB = (from s in db.Supports.Include("RelatedModule.RelatedModule.RelatedManagedSystem").Include("RelatedSupportStatusUser")
                                                                       .Include("RelatedSupportStatusProdesp")
                                                                       .Include("RelatedUser")
                                                  join m in db.ManagerUnits.Include("RelatedBudgetUnit.RelatedInstitution") on s.ManagerUnitId equals m.Id
                                                  where
                                                        s.Id == (SupportId != null ? SupportId : s.Id) &&
                                                        (supportConsult.AdmGeral || s.InstitutionId == (institutionId.HasValue && institutionId != 0 ? institutionId : s.InstitutionId)) &&
                                                        s.BudgetUnitId == (BudgetUnitId.HasValue && BudgetUnitId != 0 ? BudgetUnitId : s.BudgetUnitId) &&
                                                        s.ManagerUnitId == (ManagerUnitId.HasValue && ManagerUnitId != 0 ? ManagerUnitId : s.ManagerUnitId) &&
                                                        (s.SupportStatusProdespId != (int)EnumSupportStatusProdesp.Finalizado || s.SupportStatusUserId != (int)EnumSupportStatusUser.Concluido) &&
                                                        s.RelatedModule.Status == true &&
                                                        s.RelatedModule.ParentId != null
                                                  select new SupportGridViewModel()
                                                  {
                                                      Id = s.Id,
                                                      NameManagerReduced = m.RelatedBudgetUnit.RelatedInstitution.NameManagerReduced,
                                                      BudgetUnitCode = m.RelatedBudgetUnit.Code,
                                                      ManagerUnitCode = m.Code,
                                                      UserDescription = s.RelatedUser.Name,
                                                      InclusionDate = s.InclusionDate,
                                                      LastModifyDate = s.DataLogin,
                                                      UserCPF = s.RelatedUser.CPF,
                                                      Functionanality = s.RelatedModule.RelatedModule.RelatedManagedSystem.Name + " - " + s.RelatedModule.RelatedModule.Name + " - " + s.RelatedModule.Name,// ma.Name + " - " + mp.Name + " - " + m.Name,
                                                      SupportTypeDescription = s.RelatedSupportType.Description,
                                                      SupportStatusUserDescription = s.RelatedSupportStatusUser.Description,
                                                      SupportStatusProdespId = s.SupportStatusProdespId,
                                                      SupportStatusProdespDescription = s.RelatedSupportStatusProdesp.Description,
                                                      Responsavel = s.Responsavel,
                                                      CloseDate = s.CloseDate,
                                                      UltimoAtendimento = suporteProdesp.Contains((from h in db.HistoricSupports
                                                                                                   where s.Id == h.SupportId
                                                                                                   orderby h.InclusionDate descending
                                                                                                   select h).FirstOrDefault().UserId) ? (int)EnumAttendance.Prodesp : (int)EnumAttendance.Usuario
                                                  }).AsNoTracking().AsQueryable().OrderBy(s => s.Id);
                    }
                    else
                    {
                        supportGridViewModelDB = (from s in db.Supports.Include("RelatedModule.RelatedModule.RelatedManagedSystem").Include("RelatedSupportStatusUser")
                                                                       .Include("RelatedSupportStatusProdesp")
                                                                       .Include("RelatedUser")
                                                  join m in db.ManagerUnits.Include("RelatedBudgetUnit.RelatedInstitution") on s.ManagerUnitId equals m.Id
                                                  where
                                                        s.Id == (SupportId != null ? SupportId : s.Id) &&
                                                        (supportConsult.AdmGeral || s.InstitutionId == (institutionId.HasValue && institutionId != 0 ? institutionId : s.InstitutionId) &&
                                                        s.BudgetUnitId == (BudgetUnitId.HasValue && BudgetUnitId != 0 ? BudgetUnitId : s.BudgetUnitId) &&
                                                        s.ManagerUnitId == (ManagerUnitId.HasValue && ManagerUnitId != 0 ? ManagerUnitId : s.ManagerUnitId)) &&
                                                        s.SupportStatusProdespId == (supportStatusProdespId != null ? supportStatusProdespId : s.SupportStatusProdespId) &&
                                                        s.SupportStatusUserId == (supportStatusUserId != null ? supportStatusUserId : s.SupportStatusUserId) &&
                                                        s.RelatedModule.Status == true &&
                                                        s.RelatedModule.ParentId != null
                                                  select new SupportGridViewModel()
                                                  {
                                                      Id = s.Id,
                                                      NameManagerReduced = m.RelatedBudgetUnit.RelatedInstitution.NameManagerReduced,
                                                      BudgetUnitCode = m.RelatedBudgetUnit.Code,
                                                      ManagerUnitCode = m.Code,
                                                      UserDescription = s.RelatedUser.Name,
                                                      InclusionDate = s.InclusionDate,
                                                      LastModifyDate = s.DataLogin,
                                                      UserCPF = s.RelatedUser.CPF,
                                                      Functionanality = s.RelatedModule.RelatedModule.RelatedManagedSystem.Name + " - " + s.RelatedModule.RelatedModule.Name + " - " + s.RelatedModule.Name,// ma.Name + " - " + mp.Name + " - " + m.Name,
                                                      SupportTypeDescription = s.RelatedSupportType.Description,
                                                      SupportStatusUserDescription = s.RelatedSupportStatusUser.Description,
                                                      SupportStatusProdespId = s.SupportStatusProdespId,
                                                      SupportStatusProdespDescription = s.RelatedSupportStatusProdesp.Description,
                                                      Responsavel = s.Responsavel,
                                                      CloseDate = s.CloseDate,
                                                      UltimoAtendimento = suporteProdesp.Contains((from h in db.HistoricSupports
                                                                                                   where s.Id == h.SupportId
                                                                                                   orderby h.InclusionDate descending
                                                                                                   select h).FirstOrDefault().UserId) ? (int)EnumAttendance.Prodesp : (int)EnumAttendance.Usuario
                                                  }).AsNoTracking().AsQueryable();
                    }
                }

                if (supportConsult.DataInclusao != null && supportConsult.DataInclusao.Value.Year > 2017)
                {
                    supportGridViewModelDB = supportGridViewModelDB.Where(s => s.InclusionDate.Year == supportConsult.DataInclusao.Value.Year &&
                                                                               s.InclusionDate.Month == supportConsult.DataInclusao.Value.Month &&
                                                                               s.InclusionDate.Day == supportConsult.DataInclusao.Value.Day);
                }

                if (supportConsult.AdmGeral) {
                    if(supportStatusProdespId == (int)EnumSupportStatusProdesp.AguardandoUsuario &&
                        supportConsult.ultimaResposta != 0)
                        supportGridViewModelDB = supportGridViewModelDB.Where(s => s.UltimoAtendimento == supportConsult.ultimaResposta);
                }

                totalRegistros = await supportGridViewModelDB.CountAsync();

                var result = await supportGridViewModelDB.OrderByDescending(s => s.Id).Skip(startRec).Take(length).ToListAsync();

                return Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRegistros, recordsFiltered = totalRegistros, data = result }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return Json(MensagemErro(CommonMensagens.PadraoException, e), JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Create
        [HttpGet]
        public async Task<ActionResult> Create()
        {
            try
            {
                User u = UserCommon.CurrentUser();

                SupportCRUDViewModel supportViewModel = new SupportCRUDViewModel();
                supportViewModel.UserDescription = u.Name;
                supportViewModel.UserCPF = u.CPF;
                supportViewModel.PerfilId = (int)HttpContext.Items["perfilId"];
                supportViewModel.UserPerfil = BuscaDescricaoPerfil((int)HttpContext.Items["perfilId"]);
                supportViewModel.SupportStatusProdespId = (int)EnumSupportStatusProdesp.Aberto;

                if (PerfilAdmGeral())
                {
                    CarregaHierarquia(0, 0, 0);

                    supportViewModel.InstitutionId = 0;
                    supportViewModel.BudgetUnitId = 0;
                    supportViewModel.ManagerUnitId = 0;
                    supportViewModel.Email = u.Email;
                    
                }
                else
                {
                    getHierarquiaPerfil();
                    CarregaHierarquia(_institutionId, _budgetUnitId ?? 0, _managerUnitId ?? 0);
                    
                    supportViewModel.InstitutionId = _institutionId;
                    supportViewModel.BudgetUnitId = _budgetUnitId ?? 0;
                    supportViewModel.ManagerUnitId = _managerUnitId ?? 0;
                }

                using (db = new SupportContext())
                {
                    CarregaStatusProdesp();
                    CarregaStatusUser();
                    CarregaFunctionality();
                    CarregaSupportType();
                }

                //Limpa a sessão
                if (Session["Anexos"] != null)
                {
                    Session.Remove("Anexos");
                }

                return View(supportViewModel);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "InstitutionId, BudgetUnitId, ManagerUnitId, SupportStatusProdespId, SupportStatusUserId, UserDescription, UserCPF, UserPerfil,  Email, Responsavel, ModuleId, SupportTypeId, Observation")]SupportCRUDViewModel supportCreateViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted }))
                    {
                        User user = UserCommon.CurrentUser();

                        using (db = new SupportContext())
                        {
                            int supportId = InsereSuporte(supportCreateViewModel, user);
                            supportCreateViewModel.Id = supportId;

                            GravaHistoricoSuporte(supportId, supportCreateViewModel.Observation);
                            GravaAnexosSuporte(supportId, 'I');
                        }

                        transaction.Complete();
                    }

                    //AGUARDANDO ATUALIZACAO DE DADOS DE EMAIL POR PARTE DE USUARIOS
                    //prepararMensagemEMail(supportCreateViewModel);

                    return RedirectToAction("Index");
                }
                else
                {
                    getHierarquiaPerfil();

                    CarregaHierarquia(supportCreateViewModel.InstitutionId, supportCreateViewModel.BudgetUnitId, supportCreateViewModel.ManagerUnitId);

                    using (db = new SupportContext())
                    {
                        CarregaStatusProdesp(supportCreateViewModel.SupportStatusProdespId);
                        CarregaStatusUser(supportCreateViewModel.SupportStatusUserId);
                        CarregaFunctionality(supportCreateViewModel.ModuleId);
                        CarregaSupportType(supportCreateViewModel.SupportTypeId);
                    }

                    return View(supportCreateViewModel);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        #endregion

        #region Edit
        [HttpGet]
        public async Task<ActionResult> Edit(int id = 0)
        {
            try
            {
                getHierarquiaPerfil();

                using (db = new SupportContext())
                {
                    //Carrega dados do chamado
                    var supportDB = (from s in db.Supports
                                     where s.Id == id
                                     select new SupportCRUDViewModel {
                                         Id = s.Id,
                                         InstitutionId = s.InstitutionId,
                                         BudgetUnitId = s.BudgetUnitId,
                                         ManagerUnitId = s.ManagerUnitId,
                                         SupportStatusProdespId = s.SupportStatusProdespId,
                                         SupportStatusUserId = s.SupportStatusUserId,
                                         Email = s.Email,
                                         Responsavel = s.Responsavel,
                                         ModuleId = s.ModuleId,
                                         SupportTypeId = s.SupportTypeId,
                                         InclusionDate = s.InclusionDate,
                                         CloseDate = s.CloseDate
                                     }).AsNoTracking().FirstOrDefault();

                    if(supportDB == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    var userDB = (from s in db.Supports
                                  join u in db.Users on s.UserId equals u.Id
                                  where s.Id == id
                                  select u).FirstOrDefault();

                    var profileDB = userDB.RelationshipUsersProfiles.Where(x => x.DefaultProfile == true).FirstOrDefault().RelatedProfile;

                    supportDB.UserDescription = userDB.Name;
                    supportDB.UserCPF = userDB.CPF;
                    supportDB.PerfilId = profileDB.Id;
                    supportDB.UserPerfil = profileDB.Description;

                    CarregaHierarquia(supportDB.InstitutionId, supportDB.BudgetUnitId, supportDB.ManagerUnitId);
                    CarregaStatusProdesp(supportDB.SupportStatusProdespId);
                    CarregaStatusUser(supportDB.SupportStatusUserId);
                    CarregaFunctionality(supportDB.ModuleId);
                    CarregaSupportType(supportDB.SupportTypeId);

                    //Limpa a sessão
                    if (Session["Anexos"] != null)
                    {
                        Session.Remove("Anexos");
                    }

                    //Carrega dados dos anexos
                    var attachmentsDB = (from a in db.AttachmentSupports
                                         where a.SupportId == supportDB.Id &&
                                               a.Status == true
                                         select new AttachmentSupportViewModel()
                                         {
                                             Id = a.Id,
                                             SupportId = a.SupportId,
                                             File = a.File,
                                             Name = a.Name,
                                             Extension = a.Extension,
                                             ContentType = a.ContentType,
                                             InclusionDate = a.InclusionDate,
                                             JaGravadoNoBanco = true
                                         }).ToList();

                    //Caso existam anexos, armazenar na sessão
                    if (attachmentsDB.Any())
                    {
                        Session["Anexos"] = attachmentsDB;
                    }

                    return View(supportDB);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "InstitutionId, BudgetUnitId, ManagerUnitId, Id, SupportStatusProdespId, SupportStatusUserId, UserDescription, UserCPF, UserPerfil,  Email, Responsavel, ModuleId, SupportTypeId, Observation")]SupportCRUDViewModel supportCreateViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted }))
                    {
                        using (db = new SupportContext())
                        {
                            EditaSuporte(supportCreateViewModel);
                            GravaHistoricoSuporte(supportCreateViewModel.Id, supportCreateViewModel.Observation);
                            GravaAnexosSuporte(supportCreateViewModel.Id, 'A');
                        }

                        transaction.Complete();
                    }

                    //AGUARDANDO ATUALIZACAO DE DADOS DE EMAIL POR PARTE DE USUARIOS
                    //prepararMensagemEMail(supportCreateViewModel);

                    return RedirectToAction("Index");
                }
                else
                {
                    getHierarquiaPerfil();

                    CarregaHierarquia(supportCreateViewModel.InstitutionId, supportCreateViewModel.BudgetUnitId, supportCreateViewModel.ManagerUnitId);

                    using (db = new SupportContext())
                    {
                        CarregaStatusProdesp(supportCreateViewModel.SupportStatusProdespId);
                        CarregaStatusUser(supportCreateViewModel.SupportStatusUserId);
                        CarregaFunctionality(supportCreateViewModel.ModuleId);
                        CarregaSupportType(supportCreateViewModel.SupportTypeId);
                    }

                    return View(supportCreateViewModel);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        #endregion

        #region Partial View

        #region RespostaLote

        [HttpPost]
        public ActionResult ResponderLote(SuporteRespostaEmLote model) {
            if ((int)HttpContext.Items["perfilId"] != (int)EnumProfile.AdministradorGeral)
                return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);

            using (db = new SupportContext())
            {
                CarregaStatusProdesp();
            }

            return PartialView("_responderLote", model);
        }

        [HttpPost]
        public JsonResult SalvarRespostaEmLote(SuporteRespostaEmLote model)
        {
            try
            {
                if ((int)HttpContext.Items["perfilId"] != (int)EnumProfile.AdministradorGeral)
                    return Json(new { MsgErro = CommonMensagens.SemPermissaoDeAcesso }, JsonRequestBehavior.AllowGet);

                if (ModelState.IsValid)
                {
                    var dataAcao = DateTime.Now;
                    int idUsuario = UserCommon.CurrentUser().Id;

                    model.listaLoteEscolhidos = model.listaLoteEscolhidos.Distinct().ToList();
                    using (db = new SupportContext())
                    using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted }))
                    {

                        foreach (int id in model.listaLoteEscolhidos) {
                            Support suporte = (from s in db.Supports where s.Id == id select s).FirstOrDefault();
                            suporte.Observation = model.Observacao;
                            suporte.SupportStatusProdespId = model.StatusProdesp;

                            if (model.StatusProdesp == (int)EnumSupportStatusProdesp.Finalizado)
                                suporte.CloseDate = dataAcao;
                            else
                                suporte.CloseDate = null;

                            db.Entry(suporte).State = EntityState.Modified;

                            HistoricSupport historicoSuporte = new HistoricSupport();

                            historicoSuporte.SupportId = id;
                            
                            historicoSuporte.UserId = idUsuario;
                            historicoSuporte.InclusionDate = dataAcao;
                            historicoSuporte.Observation = model.Observacao;
                            historicoSuporte.Status = true;

                            db.Entry(historicoSuporte).State = EntityState.Added;

                            db.SaveChanges();
                        }

                        transaction.Complete();
                    }

                    return Json(new { Msg = "Salvo Com Sucesso" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { MsgErro = "Algum campo está faltando!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e) {
                throw e;
            }
        }
        #endregion

        #endregion

        #region Gerenciamento de Arquivos

        [HttpPost]
        public JsonResult Upload(HttpPostedFileBase evidenceFile)
        {
            try
            {
                //Caso a extensão do arquivo inserido não seja compativel com o sistema, ignorar o arquivo
                if (!ListaExtensoesValidas().Contains(Path.GetExtension(evidenceFile.FileName).Replace('.', ' ').Trim().ToLower()))
                {
                    return Json(new
                    {
                        tipo = "erro",
                        mensagem = "Por favor, selecione um arquivo de extenção permitida para o sistema (" + string.Join(", ", ListaExtensoesValidas().ToArray()) + ")"
                    }, JsonRequestBehavior.AllowGet);
                }

                byte[] arrayImagem = null;
                int ultimoId = 0;
                var lista = new List<AttachmentSupportViewModel>();

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    evidenceFile.InputStream.CopyTo(memoryStream);
                    arrayImagem = memoryStream.ToArray();
                }

                if (Session["Anexos"] == null)
                {

                    lista.Add(new AttachmentSupportViewModel()
                    {
                        Id = 1,
                        File = arrayImagem,
                        Name = evidenceFile.FileName,
                        Extension = Path.GetExtension(evidenceFile.FileName),
                        ContentType = evidenceFile.ContentType,
                        JaGravadoNoBanco = false,
                        InclusionDate = DateTime.Now
                    });

                    Session["Anexos"] = lista;
                }
                else
                {
                    lista = (List<AttachmentSupportViewModel>)Session["Anexos"];
                    ultimoId = lista.Max(x => x.Id);

                    lista.Add(new AttachmentSupportViewModel
                    {
                        Id = ultimoId + 1,
                        File = arrayImagem,
                        Name = evidenceFile.FileName,
                        Extension = Path.GetExtension(evidenceFile.FileName),
                        ContentType = evidenceFile.ContentType,
                        JaGravadoNoBanco = false,
                        InclusionDate = DateTime.Now
                    });

                    Session["Anexos"] = lista;
                }

                return Json(new
                {
                    tipo = "sucesso",
                    mensagem = "Arquivo anexado com sucesso"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return MensagemErroJson(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpGet]
        public ActionResult GetAttachments()
        {
            try
            {
                if (Session["Anexos"] != null)
                {
                    var lista = (List<AttachmentSupportViewModel>)Session["Anexos"];
                    return PartialView("_attachments", lista.OrderBy(x => x.InclusionDate).ToList());
                }
                else
                {
                    return PartialView("_attachments", new List<AttachmentSupportViewModel>());
                }
            }
            catch (Exception ex)
            {
                return MensagemErroJson(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpGet]
        public ActionResult GetHistoric(int supportId)
        {
            try
            {
                using (db = new SupportContext())
                {
                    var historics = (from h in db.HistoricSupports
                                     where h.SupportId == supportId
                                     orderby h.InclusionDate descending
                                     select new HistoricSupportViewModel()
                                     {
                                         SupportId = h.Id,
                                         UserCPF = h.RelatedUser.CPF,
                                         UserName = h.RelatedUser.Name,
                                         InclusionDate = h.InclusionDate,
                                         Observation = h.Observation
                                     }).ToList();

                    return PartialView("_historic", historics);
                }
            }
            catch (Exception ex)
            {
                return MensagemErroJson(CommonMensagens.PadraoException, ex);
            }
        }

        public FileResult DownloadFile(int id, bool jaGravadoNoBanco)
        {
            var lista = (List<AttachmentSupportViewModel>)Session["Anexos"];
            var attachment = lista.Where(x => x.Id == id && x.JaGravadoNoBanco == jaGravadoNoBanco).FirstOrDefault();
            var file = File(attachment.File, "application/octet-stream", attachment.Name);

            var context = System.Web.HttpContext.Current;
            var _buffer = file.FileContents.ToArray();
            context.Response.Clear();
            context.Response.ClearContent();
            context.Response.ClearHeaders();
            context.Response.ContentEncoding = System.Text.Encoding.UTF8;
            context.Response.ContentType = file.ContentType;
            context.Response.AddHeader("Content-disposition", $"attachment;filename={attachment.Name}");
            context.Response.AddHeader("content-length", file.FileContents.Length.ToString());
            context.Response.BinaryWrite(buffer: _buffer);
            context.Response.Flush();
            context.Response.End();
            return file;
        }

        #endregion

        #region Métodos Private

        private int? retornaDadosDaRequisicao(string dados)
        {
            if (!string.IsNullOrEmpty(dados) && !string.IsNullOrWhiteSpace(dados))
                return Convert.ToInt32(dados);
            return null;
        }

        private void CarregaStatusProdesp(int? Id = null)
        {
            var statusProdespDB = (from mt in db.SupportStatusProdesps
                                   where mt.Status == true
                                   select mt).ToList();

            if (Id == null)
                ViewBag.SupportStatusProdesps = new SelectList(statusProdespDB, "Id", "Description");
            else
                ViewBag.SupportStatusProdesps = new SelectList(statusProdespDB, "Id", "Description", Id);
        }

        private void CarregaStatusUser(int? Id = null)
        {
            var statusUserDB = (from mt in db.SupportStatusUsers
                                where mt.Status == true
                                select mt).ToList();

            if (Id == null)
                ViewBag.SupportStatusUsers = new SelectList(statusUserDB, "Id", "Description");
            else
                ViewBag.SupportStatusUsers = new SelectList(statusUserDB, "Id", "Description", Id);
        }

        private void CarregaFunctionality(int? Id = null)
        {
            var statusModuleDB = (from m in db.Modules
                                  join mp in db.Modules on m.ParentId equals mp.Id
                                  join ma in db.ManagedSystems on m.ManagedSystemId equals ma.Id
                                  where m.Status == true &&
                                        m.ParentId != null &&
                                        ma.Id != (int)EnumManagedSystem.Configuracoes //Remove itens da tela de configuração, pois somente usuários do perfil administrador geral poderá editar esta tela
                                  orderby ma.Name, mp.Name, m.Name
                                  select new
                                  {
                                      Id = m.Id,
                                      Description = ma.Name + " - " + mp.Name + " - " + m.Name
                                  }).ToList();

            if (Id == null)
                ViewBag.Modules = new SelectList(statusModuleDB, "Id", "Description");
            else
                ViewBag.Modules = new SelectList(statusModuleDB, "Id", "Description", Id);
        }

        private void CarregaSupportType(int? Id = null)
        {
            var supportTypeDB = (from st in db.SupportTypes
                                 where st.Status == true
                                 select new
                                 {
                                     Id = st.Id,
                                     Description = st.Description
                                 }).ToList();

            if (Id == null)
                ViewBag.SupportTypes = new SelectList(supportTypeDB, "Id", "Description");
            else
                ViewBag.SupportTypes = new SelectList(supportTypeDB, "Id", "Description", Id);
        }

        private List<string> ListaExtensoesValidas()
        {
            return new List<string>()
            {
                "gif",
                "png",
                "jpe",
                "jpeg",
                "jpg",
                "txt",
                "xls",
                "xlsx",
                "doc",
                "docx",
                "ppt",
                "pptx",
                "pdf",
                "odf",
                "rtf"
            };
        }

        private int InsereSuporte(SupportCRUDViewModel supportCreateViewModel, User user)
        {
            //Grava o registro do chamado
            var support = new Support();

            support.InstitutionId = supportCreateViewModel.InstitutionId;
            support.BudgetUnitId = supportCreateViewModel.BudgetUnitId;
            support.ManagerUnitId = supportCreateViewModel.ManagerUnitId;
            support.UserId = user.Id;
            support.SupportStatusProdespId = supportCreateViewModel.SupportStatusProdespId;
            support.InclusionDate = DateTime.Now;

            if (supportCreateViewModel.SupportStatusProdespId == (int)EnumSupportStatusProdesp.Finalizado)
                support.CloseDate = DateTime.Now;
            else
                support.CloseDate = null;

            support.Email = supportCreateViewModel.Email;
            support.SupportStatusUserId = supportCreateViewModel.SupportStatusUserId;
            support.Responsavel = supportCreateViewModel.Responsavel;
            support.ModuleId = supportCreateViewModel.ModuleId;
            support.Login = user.CPF;
            support.DataLogin = DateTime.Now;
            support.SupportTypeId = supportCreateViewModel.SupportTypeId;
            support.Observation = supportCreateViewModel.Observation;
            support.Status = true;

            db.Entry(support).State = EntityState.Added;
            db.SaveChanges();

            return support.Id;
        }

        private void EditaSuporte(SupportCRUDViewModel supportCreateViewModel)
        {
            //Recupera o chamado 
            var supportDB = (from s in db.Supports
                             where s.Id == supportCreateViewModel.Id
                             select s).FirstOrDefault();

            supportDB.SupportStatusProdespId = supportCreateViewModel.SupportStatusProdespId;

            if (supportCreateViewModel.SupportStatusProdespId == (int)EnumSupportStatusProdesp.Finalizado)
                supportDB.CloseDate = DateTime.Now;
            else
                supportDB.CloseDate = null;

            supportDB.Email = supportCreateViewModel.Email;
            supportDB.SupportStatusUserId = supportCreateViewModel.SupportStatusUserId;
            supportDB.Responsavel = supportCreateViewModel.Responsavel;
            supportDB.ModuleId = supportCreateViewModel.ModuleId;
            supportDB.DataLogin = DateTime.Now;
            supportDB.SupportTypeId = supportCreateViewModel.SupportTypeId;
            supportDB.Observation = supportCreateViewModel.Observation;

            db.Entry(supportDB).State = EntityState.Modified;
            db.SaveChanges();
        }

        private void GravaAnexosSuporte(int supportId, char tipoTransacao)
        {
            AttachmentSupport attachment = null;
            List<AttachmentSupportViewModel> listaAnexos = null;

            if (tipoTransacao == 'I')
                listaAnexos = Session["Anexos"] != null ? (List<AttachmentSupportViewModel>)Session["Anexos"] : new List<AttachmentSupportViewModel>();
            else if (tipoTransacao == 'A')
                listaAnexos = Session["Anexos"] != null ? ((List<AttachmentSupportViewModel>)Session["Anexos"]).Where(x => x.JaGravadoNoBanco == false).ToList() : new List<AttachmentSupportViewModel>();

            foreach (var anexo in listaAnexos)
            {
                attachment = new AttachmentSupport();
                attachment.SupportId = supportId;
                attachment.File = anexo.File;
                attachment.Name = anexo.Name;
                attachment.Extension = anexo.Extension;
                attachment.ContentType = anexo.ContentType;
                attachment.InclusionDate = DateTime.Now;
                attachment.Status = true;

                db.Entry(attachment).State = EntityState.Added;
                db.SaveChanges();
            }
        }

        private void GravaHistoricoSuporte(int supportId, string observation)
        {
            User user = UserCommon.CurrentUser();

            var historicSupport = new HistoricSupport()
            {
                SupportId = supportId,
                UserId = user.Id,
                InclusionDate = DateTime.Now,
                Observation = observation,
                Status = true
            };

            db.Entry(historicSupport).State = EntityState.Added;
            db.SaveChanges();
        }

        private void CarregaHierarquia(int modelInstitutionId = 0, int modelBudgetUnitId = 0, int modelManagerUnitId = 0)
        {
            hierarquia = new Hierarquia();
            if (PerfilAdmGeral())
            {
                ViewBag.Institutions = new SelectList(hierarquia.GetOrgaos(null), "Id", "Description", modelInstitutionId);
                if (modelInstitutionId != 0)
                    ViewBag.BudgetUnits = new SelectList(hierarquia.GetUosPorOrgaoId(modelInstitutionId), "Id", "Description", modelBudgetUnitId);
                else
                    ViewBag.BudgetUnits = new SelectList(hierarquia.GetUos(null), "Id", "Description");

                if (modelBudgetUnitId != 0)
                    ViewBag.ManagerUnits = new SelectList(hierarquia.GetUgesPorUoId(modelBudgetUnitId), "Id", "Description", modelManagerUnitId);
                else
                    ViewBag.ManagerUnits = new SelectList(hierarquia.GetUges(null), "Id", "Description");
            }
            else
            {
                ViewBag.Institutions = new SelectList(hierarquia.GetOrgaos(_institutionId), "Id", "Description", modelInstitutionId);

                if (_budgetUnitId.HasValue && _budgetUnitId != 0)
                    ViewBag.BudgetUnits = new SelectList(hierarquia.GetUos(_budgetUnitId), "Id", "Description", modelBudgetUnitId);
                else
                    ViewBag.BudgetUnits = new SelectList(hierarquia.GetUosPorOrgaoId(_institutionId), "Id", "Description", modelBudgetUnitId);

                if (_managerUnitId.HasValue && _managerUnitId != 0)
                    ViewBag.ManagerUnits = new SelectList(hierarquia.GetUges(_managerUnitId), "Id", "Description", modelManagerUnitId);
                else if (modelBudgetUnitId != 0)
                    ViewBag.ManagerUnits = new SelectList(hierarquia.GetUgesPorUoId(modelBudgetUnitId), "Id", "Description", modelManagerUnitId);
                else
                    ViewBag.ManagerUnits = new SelectList(hierarquia.GetUgesPorUoId(_budgetUnitId), "Id", "Description", modelManagerUnitId);
            }
        }

        private async Task prepararMensagemEMail(SupportCRUDViewModel supportCreateViewModel)
        {
            try
            {
                if (supportCreateViewModel != null)
                {
                    var assuntoMensagem = montarAssuntoMensagem(supportCreateViewModel);
                    var corpoMensagem = montarCorpoMensagem(supportCreateViewModel);
                    var listagemEMails = gerarRelacaoEMails(supportCreateViewModel);
                    var despachanteEMail = new DespachanteEMail();


                    despachanteEMail.MontarMensagem(listagemEMails["De"] as string,
                                                    listagemEMails["Para"] as string[],
                                                    listagemEMails["CC"] as string[],
                                                    null,
                                                    assuntoMensagem,
                                                    corpoMensagem);

                    despachanteEMail.EnviarMensagem();
                }
            }
            catch (SmtpException excErroEnvioEmail)
            {
                base.GravaLogErro(excErroEnvioEmail);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string montarCorpoMensagem(SupportCRUDViewModel supportCreateViewModel)
        {
            try
            {
                string fmtTextoMensagem = null;
                string textoMensagem = null;
                string nomeUsuario;
                string statusProdesp = null;
                string statusUsuario = null;
                string statusDaMensagem = null;

                nomeUsuario = supportCreateViewModel.UserDescription;
                statusProdesp = EnumGeral.GetEnumDescription((EnumSupportStatusProdesp)supportCreateViewModel.SupportStatusProdespId);
                statusUsuario = EnumGeral.GetEnumDescription((EnumSupportStatusUser)supportCreateViewModel.SupportStatusUserId);

                if (PerfilAdmGeral())
                    statusDaMensagem = statusProdesp;
                else
                    statusDaMensagem = statusUsuario;

                fmtTextoMensagem = "{0}{1},{0}Seu chamado foi alterado para \"{2}\". Favor verificar.{0}{0}Email automático. Favor não responder.";
                if (supportCreateViewModel != null)
                    textoMensagem = String.Format(fmtTextoMensagem, Environment.NewLine, nomeUsuario, statusDaMensagem);

                return textoMensagem;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool chamadoJahExistente(int supportId)
        {
            bool chamadoJahFoiEditado = false;

            IList<HistoricSupport> historicosVinculadosAoChamado = null;
            historicosVinculadosAoChamado = new List<HistoricSupport>();
            using (SupportContext contextoCamadaDados = new SupportContext())
            {
                historicosVinculadosAoChamado = contextoCamadaDados.HistoricSupports
                                                                   .AsNoTracking()
                                                                   .Where(historicoChamadoSuporteSistema => historicoChamadoSuporteSistema.SupportId == supportId)
                                                                   .ToList();
            }


            chamadoJahFoiEditado = (historicosVinculadosAoChamado.Count() > 1);
            return chamadoJahFoiEditado;
        }

        private string montarAssuntoMensagem(SupportCRUDViewModel supportCreateViewModel)
        {
            try
            {
                string statusChamado = null;
                string fmtAssuntoMensagem = null;
                string assuntoMensagem = null;
                string numeroChamado = null;
                string strCodigoUGE = null;
                int codigoUGE = 0;






                
                fmtAssuntoMensagem = "Sistema SAM - Módulo Patrimonio - Chamado Suporte #{0} - UGE {1} - {2}";
                using (SupportContext contextoCamadaDados = new SupportContext())
                {
                    strCodigoUGE = contextoCamadaDados.ManagerUnits
                                                      .AsNoTracking()
                                                      .Where(ugeSIAFEM => ugeSIAFEM.Id == supportCreateViewModel.ManagerUnitId)
                                                      .Select(ugeSIAFEM => ugeSIAFEM.Code)
                                                      .FirstOrDefault();
                }

                if (!String.IsNullOrEmpty(strCodigoUGE) && Int32.TryParse(strCodigoUGE, out codigoUGE))
                {
                    if (supportCreateViewModel.IsNotNull())
                    {
                        numeroChamado = supportCreateViewModel.Id.ToString("D7");
                        statusChamado = EnumGeral.GetEnumDescription((EnumSupportStatusUser)supportCreateViewModel.SupportStatusUserId);
                        assuntoMensagem = String.Format(fmtAssuntoMensagem, numeroChamado, codigoUGE, statusChamado);
                    }
                }

                return assuntoMensagem;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private SortedList gerarRelacaoEMails(SupportCRUDViewModel supportCreateViewModel)
        {
            try
            {
                string[] enderecosDestinatarios = null;
                string[] enderecosEMailEmCopia = null;

                SortedList listagemEMails = new SortedList(StringComparer.InvariantCultureIgnoreCase);

                enderecosDestinatarios = new string[] { supportCreateViewModel.Email  };
                enderecosEMailEmCopia = new string[] { cstEMailParaEnvioSuporteSAM };

                listagemEMails.InserirValor("De", cstEMailParaEnvioSuporteSAM);
                listagemEMails.InserirValor("Para", enderecosDestinatarios);
                listagemEMails.InserirValor("CC", enderecosEMailEmCopia);

                return listagemEMails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}