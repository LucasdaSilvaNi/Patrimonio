using AutoMapper;
using SAM.Web.Context;
using SAM.Web.Models;
using SAM.Web.ViewModels;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PagedList;
using System.Collections.Generic;
using SAM.Web.Common;
using System.Transactions;
using System.Web.Security;
using Newtonsoft.Json;
using SAM.Web.Common.Enum;
using System.Web.Script.Serialization;
using System.Threading.Tasks;

namespace SAM.Web.Controllers
{
    public class UsersController : BaseController
    {
        private SAMContext db;
        private Hierarquia hierarquia;
        private int? _administrativeUnitId;
        private int? _budgetUnitId;
        private int _institutionId;
        private int? _managerUnitId;
        private int? _sectionId;
        private int? _perfilId;
        private RelationshipUserProfile rup;

        private void GetIdsDaHierarquia()
        {
            User u = UserCommon.CurrentUser();
            rup = BuscaPerfilLogado(u.Id);
            var perflLogado = BuscaHierarquiaPerfilLogado(rup.Id);
            _institutionId = perflLogado.InstitutionId;
            _budgetUnitId = perflLogado.BudgetUnitId;
            _managerUnitId = perflLogado.ManagerUnitId;
            _administrativeUnitId = perflLogado.AdministrativeUnitId;
            _sectionId = perflLogado.SectionId;
            _perfilId = perflLogado.RelatedRelationshipUserProfile.ProfileId;
        }

        #region Actions
        #region Index Actions

        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            try
            {
                ViewBag.CPF = (string)HttpContext.Items["CPF"];

                return View();
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        public async Task<JsonResult> IndexJSONResult(UserModel assetEAsset)
        {
            string draw = Request.Form["draw"].ToString();
            string order = Request.Form["order[0][column]"].ToString();
            string orderDir = Request.Form["order[0][dir]"].ToString();
            int startRec = Convert.ToInt32(Request.Form["start"].ToString());
            int length = Convert.ToInt32(Request.Form["length"].ToString());
            string currentFilter = Request.Form["currentFilter"].ToString();


            int totalRegistros = 0;

            try
            {
                IQueryable<UserModel> lstRetorno;

                GetIdsDaHierarquia();

                db = new SAMContext();

                // Tratamento para perfis Responsavel
                if (rup.ProfileId == (int)EnumProfile.Responsavel)
                {

                    if (!string.IsNullOrEmpty(currentFilter) && !string.IsNullOrWhiteSpace(currentFilter))
                        lstRetorno = (from s in db.Users
                                      join rp in db.RelationshipUserProfiles
                                      on s.Id equals rp.UserId
                                      join ri in db.RelationshipUserProfileInstitutions.Include("RelatedInstitution").Include("RelatedBudgetUnit").Include("RelatedManagerUnit")
                                      on rp.Id equals ri.RelationshipUserProfileId
                                      where s.Id == rup.UserId && rp.DefaultProfile == true
                                      select new UserModel
                                      {
                                          Id = s.Id,
                                          CPF = s.CPF,
                                          Name = s.Name,
                                          Orgao = ri.RelatedInstitution.Code,
                                          Uo = ri.RelatedBudgetUnit.Code,
                                          Uge = ri.RelatedManagerUnit.Code,
                                          Email = s.Email,
                                          AddedDate = s.AddedDate,
                                          Status = s.Status
                                      }).Where(s => s.Name.Contains(currentFilter)
                                        || s.CPF.Contains(currentFilter)
                                        || s.Orgao.Contains(currentFilter)
                                        || s.Uo.Contains(currentFilter)
                                        || s.Uge.Contains(currentFilter)
                                        || s.Email.Contains(currentFilter)
                                        || s.AddedDate.ToString().Contains(currentFilter)
                                        ).AsNoTracking();

                    else
                        lstRetorno = (from s in db.Users
                                      join rp in db.RelationshipUserProfiles
                                      on s.Id equals rp.UserId
                                      join ri in db.RelationshipUserProfileInstitutions.Include("RelatedInstitution").Include("RelatedBudgetUnit").Include("RelatedManagerUnit")
                                      on rp.Id equals ri.RelationshipUserProfileId
                                      where s.Id == rup.UserId && rp.DefaultProfile == true
                                      select new UserModel
                                      {
                                          Id = s.Id,
                                          CPF = s.CPF,
                                          Name = s.Name,
                                          Orgao = ri.RelatedInstitution.Code,
                                          Uo = ri.RelatedBudgetUnit.Code,
                                          Uge = ri.RelatedManagerUnit.Code,
                                          Email = s.Email,
                                          AddedDate = s.AddedDate,
                                          Status = s.Status
                                      }).AsNoTracking();

                    totalRegistros = await lstRetorno.CountAsync();

                    lstRetorno = lstRetorno.OrderBy(u => u.Name).Skip(startRec).Take(length);
                    var resultResponsavel = await lstRetorno.ToListAsync();

                    return Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRegistros, recordsFiltered = totalRegistros, data = resultResponsavel }, JsonRequestBehavior.AllowGet);
                }

                if (!string.IsNullOrEmpty(currentFilter) && !string.IsNullOrWhiteSpace(currentFilter))
                    lstRetorno = (from s in db.Users
                                  join rp in db.RelationshipUserProfiles
                                  on s.Id equals rp.UserId
                                  join ri in db.RelationshipUserProfileInstitutions.Include("RelatedInstitution").Include("RelatedBudgetUnit").Include("RelatedManagerUnit")
                                  on rp.Id equals ri.RelationshipUserProfileId
                                  where rp.DefaultProfile == true && s.Status == true
                                  select new UserModel
                                  {
                                      Id = s.Id,
                                      CPF = s.CPF,
                                      Name = s.Name,
                                      Orgao = ri.RelatedInstitution.Code,
                                      Uo = ri.RelatedBudgetUnit.Code,
                                      Uge = ri.RelatedManagerUnit.Code,
                                      Email = s.Email,
                                      AddedDate = s.AddedDate,
                                      Status = s.Status
                                  }).Where(s => s.Name.Contains(currentFilter)
                                        || s.CPF.Contains(currentFilter)
                                        || s.Orgao.Contains(currentFilter)
                                        || s.Uo.Contains(currentFilter)
                                        || s.Uge.Contains(currentFilter)
                                        || s.Email.Contains(currentFilter)
                                        || s.AddedDate.ToString().Contains(currentFilter)
                                   ).AsNoTracking();
                else
                    lstRetorno = (from s in db.Users
                                  join rp in db.RelationshipUserProfiles
                                  on s.Id equals rp.UserId
                                  join ri in db.RelationshipUserProfileInstitutions.Include("RelatedInstitution").Include("RelatedBudgetUnit").Include("RelatedManagerUnit")
                                  on rp.Id equals ri.RelationshipUserProfileId
                                  where rp.DefaultProfile == true && s.Status == true
                                  select new UserModel
                                  {
                                      Id = s.Id,
                                      CPF = s.CPF,
                                      Name = s.Name,
                                      Orgao = ri.RelatedInstitution.Code,
                                      Uo = ri.RelatedBudgetUnit.Code,
                                      Uge = ri.RelatedManagerUnit.Code,
                                      Email = s.Email,
                                      AddedDate = s.AddedDate,
                                      Status = s.Status
                                  }).AsNoTracking();

                if (!PerfilAdmGeral())
                {
                    lstRetorno = (from s in lstRetorno join r in db.RelationshipUserProfiles on s.Id equals r.UserId where r.DefaultProfile == true && r.RelatedProfile.flagPerfilMaster != true select s).AsNoTracking();

                    if (_sectionId.HasValue && _sectionId != 0)
                    {
                        lstRetorno = (from r in lstRetorno
                                      join p in db.RelationshipUserProfiles on r.Id equals p.UserId
                                      join i in db.RelationshipUserProfileInstitutions on p.Id equals i.RelationshipUserProfileId
                                      where p.DefaultProfile == true && i.SectionId == _sectionId
                                      select r).AsNoTracking().Distinct().AsQueryable();
                    }
                    else if (_administrativeUnitId.HasValue && _administrativeUnitId != 0)
                    {
                        lstRetorno = (from r in lstRetorno
                                      join p in db.RelationshipUserProfiles on r.Id equals p.UserId
                                      join i in db.RelationshipUserProfileInstitutions on p.Id equals i.RelationshipUserProfileId
                                      where p.DefaultProfile == true &&  i.AdministrativeUnitId == _administrativeUnitId
                                      select r).AsNoTracking().Distinct().AsQueryable();
                    }
                    else if (_managerUnitId.HasValue && _managerUnitId != 0)
                    {
                        lstRetorno = (from r in lstRetorno
                                      join p in db.RelationshipUserProfiles on r.Id equals p.UserId
                                      join i in db.RelationshipUserProfileInstitutions on p.Id equals i.RelationshipUserProfileId
                                      where p.DefaultProfile == true &&  i.ManagerUnitId == _managerUnitId
                                      select r).AsNoTracking().Distinct().AsQueryable();
                    }
                    else if (_budgetUnitId.HasValue && _budgetUnitId != 0)
                    {
                        lstRetorno = (from r in lstRetorno
                                      join p in db.RelationshipUserProfiles on r.Id equals p.UserId
                                      join i in db.RelationshipUserProfileInstitutions on p.Id equals i.RelationshipUserProfileId
                                      where p.DefaultProfile == true &&  i.BudgetUnitId == _budgetUnitId
                                      select r).Distinct().AsQueryable();
                    }
                    else if (_institutionId != 0)
                    {
                        lstRetorno = (from r in lstRetorno
                                      join p in db.RelationshipUserProfiles on r.Id equals p.UserId
                                      join i in db.RelationshipUserProfileInstitutions on p.Id equals i.RelationshipUserProfileId
                                      where p.DefaultProfile == true &&  i.InstitutionId == _institutionId
                                      select r).AsNoTracking().Distinct().AsQueryable();
                    }
                }
                else
                {
                    lstRetorno = (from r in lstRetorno
                                  join p in db.RelationshipUserProfiles on r.Id equals p.UserId
                                  join i in db.RelationshipUserProfileInstitutions on p.Id equals i.RelationshipUserProfileId
                                  select r).AsNoTracking().Distinct().AsQueryable();
                }

                totalRegistros = await lstRetorno.CountAsync();

                lstRetorno = lstRetorno.OrderBy(u => u.Name.Trim()).Skip(startRec).Take(length);
                var result = await lstRetorno.ToListAsync();

                return Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRegistros, recordsFiltered = totalRegistros, data = result }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(MensagemErro(CommonMensagens.PadraoException, ex), JsonRequestBehavior.AllowGet);
            }


        }
        #endregion
        #region Details Actions
        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                db = new SAMContext();
                User user = db.Users.Find(id);
                if (user == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                // Recupera o perfil do id informado
                var userHierarquia = (from r in db.RelationshipUserProfiles
                                      join u in db.Users on r.UserId equals u.Id
                                      join ri in db.RelationshipUserProfileInstitutions on r.Id equals ri.RelationshipUserProfileId
                                      where u.Id == user.Id && r.DefaultProfile == true
                                      select ri).AsNoTracking().FirstOrDefault();

                if (ValidarRequisicao(userHierarquia.InstitutionId, userHierarquia.BudgetUnitId, userHierarquia.ManagerUnitId, userHierarquia.AdministrativeUnitId, userHierarquia.SectionId))
                    return View(user);
                else
                    return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Create Actions
        // GET: Users/Create
        public ActionResult Create()
        {
            try
            {
                SetCarregaHierarquia();
                return View();
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        private bool cpfCadastrado(string cpf)
        {
            using (db = new SAMContext())
            {
                var retorno = db.Users.Where(c => c.Status == true &&
                                                  c.CPF.Equals(cpf)).FirstOrDefault();

                if (retorno != null)
                    return true;

                return false;
            }

        }

        // POST: users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id, CPF, Name, Email, Password, PasswordNew, Phrase, relationshipUserProfileInstitution")]UserModel user)
        {
            try
            {
                db = new SAMContext();
                if (user.CPF == null || !TratamentoDados.ValidarCPF(user.CPF.Replace(".", string.Empty).Replace("-", string.Empty)))
                {
                    ModelState.AddModelError("CPF", "Numeração de CPF inválido.");
                    SetCarregaHierarquia();
                    return View(user);
                }
                else if ((from u in db.Users where u.CPF == user.CPF.Replace(".", string.Empty).Replace("-", string.Empty) && u.Status == true select u.Id).Any())
                {
                    ModelState.AddModelError("CPF", "CPF já está cadastrado e ativo para outro usuário.");
                    SetCarregaHierarquia();
                    return View(user);
                }

                if (ModelState.IsValid)
                {
                    using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted }))
                    {
                        Mapper.CreateMap<UserModel, User>();
                        var objUser = Mapper.Map<User>(user);

                        objUser.CPF = objUser.CPF.Replace(".", string.Empty).Replace("-", string.Empty);
                        objUser.Status = true;
                        objUser.Password = UserCommon.EncryptPassword(objUser.Password);
                        objUser.AddedDate = DateTime.Now;

                        db.Entry(objUser).State = EntityState.Added;
                        db.SaveChanges();

                        //Verifica se ja existe algum usuário com o mesmo CPF
                        var usuario = (from u in db.Users
                                       where u.CPF == user.CPF
                                       select u).AsNoTracking().FirstOrDefault();


                        //Salva relacionamentos
                        List<RelationshipUserProfileInstitutionModel> listRelationshipUserProfileInstitutionModel = user.relationshipUserProfileInstitution != "" ?
                                                                        Newtonsoft.Json.JsonConvert.DeserializeObject<List<RelationshipUserProfileInstitutionModel>>(user.relationshipUserProfileInstitution) :
                                                                        new List<RelationshipUserProfileInstitutionModel>();


                        var listRelationshipUserProfileInstitutionDB = (from a in db.RelationshipUserProfiles
                                                                        join b in db.RelationshipUserProfileInstitutions on a.Id equals b.RelationshipUserProfileId
                                                                        where a.UserId == objUser.Id
                                                                        select b
                                                                        ).AsNoTracking().ToList();


                        List<RelationshipUserProfileInstitutionModel> listRelationshipUserProfileInstitutionModelIncluir = listRelationshipUserProfileInstitutionModel.ToList();

                        //Inclusão relacionamentos
                        if (listRelationshipUserProfileInstitutionModelIncluir.Any())
                        {
                            foreach (var item in listRelationshipUserProfileInstitutionModelIncluir)
                            {
                                //RelationshipUserProfile
                                var relationshipUserProfile = new RelationshipUserProfile();
                                relationshipUserProfile.UserId = objUser.Id;
                                relationshipUserProfile.ProfileId = item.ProfileId;
                                relationshipUserProfile.DefaultProfile = item.ProfilePadrao;
                                relationshipUserProfile.FlagResponsavel = item.ProfileId == (int)EnumProfile.Responsavel ? true : false;

                                db.Entry(relationshipUserProfile).State = EntityState.Added;
                                db.SaveChanges();

                                //RelationshipUserProfileInstitution
                                var relationshipUserProfileInstitution = new RelationshipUserProfileInstitution();
                                relationshipUserProfileInstitution.RelationshipUserProfileId = relationshipUserProfile.Id;
                                relationshipUserProfileInstitution.InstitutionId = item.InstitutionId;
                                relationshipUserProfileInstitution.Status = true;
                                relationshipUserProfileInstitution.BudgetUnitId = item.BudgetUnitId == null || item.BudgetUnitId == 0 ? null : item.BudgetUnitId;
                                relationshipUserProfileInstitution.ManagerUnitId = item.ManagerUnitId == null || item.ManagerUnitId == 0 ? null : item.ManagerUnitId;
                                relationshipUserProfileInstitution.AdministrativeUnitId = item.AdministrativeUnitId == null || item.AdministrativeUnitId == 0 ? null : item.AdministrativeUnitId;
                                relationshipUserProfileInstitution.SectionId = item.SectionId == null || item.SectionId == 0 ? null : item.SectionId;

                                db.Entry(relationshipUserProfileInstitution).State = EntityState.Added;
                                db.SaveChanges();
                            }
                        }

                        transaction.Complete();
                    }

                    return RedirectToAction("Index");
                }

                SetCarregaHierarquia();
                return View(user);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }


        #endregion
        #region Edit Actions
        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                db = new SAMContext();
                UserModel user = Users(id.Value);
                if (user == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                // Recupera o perfil do id informado
                var userHierarquia = (from r in db.RelationshipUserProfiles
                                      join u in db.Users on r.UserId equals u.Id
                                      join ri in db.RelationshipUserProfileInstitutions on r.Id equals ri.RelationshipUserProfileId
                                      where u.Id == user.Id && r.DefaultProfile == true
                                      select ri).AsNoTracking().FirstOrDefault();

                if (ValidarRequisicao(userHierarquia.InstitutionId, userHierarquia.BudgetUnitId ?? 0, userHierarquia.ManagerUnitId ?? 0, userHierarquia.AdministrativeUnitId ?? 0, userHierarquia.SectionId ?? 0))
                {
                    SetCarregaHierarquia();

                    user.AdmGeral = PerfilAdmGeral();

                    List<RelationshipUserProfileInstitutionModel> query = (from a in db.RelationshipUserProfiles.Include("Profile")
                                                                           join b in db.RelationshipUserProfileInstitutions.Include("Institution").Include("BudgetUnit") on a.Id equals b.RelationshipUserProfileId

                                                                           where a.UserId == user.Id

                                                                           select new RelationshipUserProfileInstitutionModel()
                                                                           {
                                                                               Id = b.Id,
                                                                               ProfileId = a.RelatedProfile.Id,
                                                                               DescProfile = a.RelatedProfile.Description,
                                                                               RelationshipUserProfileId = a.Id,
                                                                               ProfilePadrao = a.DefaultProfile,
                                                                               InstitutionId = b.RelatedInstitution.Id,
                                                                               InstitutionCode = b.RelatedInstitution.Code,
                                                                               BudgetUnitId = b.RelatedBudgetUnit.Id,
                                                                               BudgetUnitCode = b.RelatedBudgetUnit.Code,
                                                                               ManagerUnitId = b.RelatedManagerUnit.Id,
                                                                               ManagerUnitCode = b.RelatedManagerUnit.Code,
                                                                               AdministrativeUnitId = b.RelatedAdministrativeUnit.Id,
                                                                               AdministrativeUnitCode = b.RelatedAdministrativeUnit.Code,
                                                                               SectionId = b.RelatedSection.Id,
                                                                               SectionCode = b.RelatedSection.Code
                                                                           }
                                                                           ).AsNoTracking().ToList();

                    user.relationshipUserProfileInstitution = new JavaScriptSerializer().Serialize(query.ToList());

                    return View(user);
                }
                else
                    return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        private UserModel Users(int userId)
        {

            IQueryable<User> query = (from u in db.Users.Include("RelationshipUserProfiles")
                                      join p in db.RelationshipUserProfiles.Include("RelationshipUserProfileInstitutions") on u.Id equals p.UserId into relatedProfile
                                      from rl in relatedProfile.DefaultIfEmpty()
                                      join r in db.RelationshipUserProfileInstitutions on rl.Id equals r.RelationshipUserProfileId into relatedProfileInstitution
                                      from rpi in relatedProfileInstitution.DefaultIfEmpty()
                                      where u.Id == userId
                                        && u.Status == true

                                      select u).AsQueryable();
            var user = query.FirstOrDefault();

            UserModel _user = null;

            Mapper.CreateMap<User, UserModel>()
                .ForMember(m => m.AdministrativeUnits, op => op.Ignore())
                .ForMember(m => m.AssetMovements, op => op.Ignore())
                .ForMember(m => m.BudgetUnits, op => op.Ignore())
                .ForMember(m => m.Historics, op => op.Ignore())
                .ForMember(m => m.HistoricSupports, op => op.Ignore())
                .ForMember(m => m.Institutions, op => op.Ignore())
                .ForMember(m => m.Inventarios, op => op.Ignore())
                .ForMember(m => m.Managers, op => op.Ignore())
                .ForMember(m => m.ManagerUnits, op => op.Ignore())
                .ForMember(m => m.Sections, op => op.Ignore())
                .ForMember(m => m.Supports, op => op.Ignore());

            _user = Mapper.Map<UserModel>(user);

            return _user;
        }
        // POST: users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id, CPF, Name, Email, Password, DataUltimoTreinamento, PasswordNew, Phrase, relationshipUserProfileInstitution")]UserModel user)
        {
            try
            {
                db = new SAMContext();
                if (user.CPF == null || !TratamentoDados.ValidarCPF(user.CPF.Replace(".", string.Empty).Replace("-", string.Empty)))
                {
                    ModelState.AddModelError("CPF", "Numeração de CPF inválido.");
                    SetCarregaHierarquia();
                    return View(user);
                }
                else if ((from u in db.Users where u.Id != user.Id && u.CPF == user.CPF.Replace(".", string.Empty).Replace("-", string.Empty) && u.Status == true select u.Id).Any())
                {
                    ModelState.AddModelError("CPF", "CPF já está cadastrado e ativo para outro usuário.");
                    SetCarregaHierarquia();
                    return View(user);
                }

                if (ModelState.IsValid)
                {
                    using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted }))
                    {
                        db.Configuration.AutoDetectChangesEnabled = false;
                        db.Configuration.ProxyCreationEnabled = false;
                        db.Configuration.LazyLoadingEnabled = false;

                        //Salva Usuário
                        var usuarioDB = (from u in db.Users
                                         where u.Id == user.Id
                                         select u).AsNoTracking().FirstOrDefault();


                        usuarioDB.CPF = user.CPF.Replace(".", string.Empty).Replace("-", string.Empty);
                        usuarioDB.Name = user.Name;
                        usuarioDB.Email = user.Email;
                        usuarioDB.Phrase = user.Phrase;

                        if (PerfilAdmGeral() == true)
                        {
                            usuarioDB.DataUltimoTreinamento = user.DataUltimoTreinamento;
                        }

                        if (user.PasswordNew != null)
                        {
                            usuarioDB.Password = UserCommon.EncryptPassword(user.PasswordNew);
                        }

                        db.Entry(usuarioDB).State = EntityState.Modified;
                        db.SaveChanges();

                        //Salva relacionamentos
                        List<RelationshipUserProfileInstitutionModel> listRelationshipUserProfileInstitutionModel = user.relationshipUserProfileInstitution != "" ?
                                                                        Newtonsoft.Json.JsonConvert.DeserializeObject<List<RelationshipUserProfileInstitutionModel>>(user.relationshipUserProfileInstitution) :
                                                                        new List<RelationshipUserProfileInstitutionModel>();


                        var listRelationshipUserProfileInstitutionDB = (from a in db.RelationshipUserProfiles
                                                                        join b in db.RelationshipUserProfileInstitutions on a.Id equals b.RelationshipUserProfileId

                                                                        where a.UserId == user.Id
                                                                        select b
                                                                        ).AsNoTracking().ToList();


                        List<RelationshipUserProfileInstitutionModel> listRelationshipUserProfileInstitutionModelIncluir = listRelationshipUserProfileInstitutionModel.Where(x => x.Id == 0).ToList();
                        List<RelationshipUserProfileInstitutionModel> listRelationshipUserProfileInstitutionModelAlterar = listRelationshipUserProfileInstitutionModel.Where(x => x.Id != 0).ToList();
                        List<RelationshipUserProfileInstitution> listRelationshipUserProfileInstitutionModelDeletar = listRelationshipUserProfileInstitutionDB.Where(x => !listRelationshipUserProfileInstitutionModel.Exists(y => y.Id == x.Id)).ToList();


                        //Deleta relacionamentos
                        if (listRelationshipUserProfileInstitutionModelDeletar.Any())
                        {
                            foreach (var item in listRelationshipUserProfileInstitutionModelDeletar)
                            {
                                db.Entry(item).State = EntityState.Deleted;

                                db.Entry(db.RelationshipUserProfiles.Where(x => x.Id == item.RelationshipUserProfileId && x.UserId == user.Id).FirstOrDefault()).State = EntityState.Deleted;
                            }

                            db.SaveChanges();
                        }

                        //Altera relacionamentos
                        if (listRelationshipUserProfileInstitutionModelAlterar.Any())
                        {
                            foreach (var item in listRelationshipUserProfileInstitutionModelAlterar)
                            {
                                var relationshipUserProfiles = (from a in db.RelationshipUserProfiles
                                                                where a.Id == item.RelationshipUserProfileId &&
                                                                      a.UserId == user.Id

                                                                select a
                                                                ).AsNoTracking().FirstOrDefault();

                                relationshipUserProfiles.DefaultProfile = item.ProfilePadrao;

                                db.Entry(relationshipUserProfiles).State = EntityState.Modified;

                            }
                            db.SaveChanges();
                        }

                        //Inclusão relacionamentos
                        if (listRelationshipUserProfileInstitutionModelIncluir.Any())
                        {
                            foreach (var item in listRelationshipUserProfileInstitutionModelIncluir)
                            {
                                //RelationshipUserProfile
                                var relationshipUserProfile = new RelationshipUserProfile();
                                relationshipUserProfile.UserId = user.Id;
                                relationshipUserProfile.ProfileId = item.ProfileId;
                                relationshipUserProfile.DefaultProfile = item.ProfilePadrao;
                                relationshipUserProfile.FlagResponsavel = item.ProfileId == (int)EnumProfile.Responsavel ? true : false;

                                db.Entry(relationshipUserProfile).State = EntityState.Added;
                                db.SaveChanges();

                                //RelationshipUserProfileInstitution
                                var relationshipUserProfileInstitution = new RelationshipUserProfileInstitution();
                                relationshipUserProfileInstitution.RelationshipUserProfileId = relationshipUserProfile.Id;
                                relationshipUserProfileInstitution.InstitutionId = item.InstitutionId;
                                relationshipUserProfileInstitution.Status = true;
                                relationshipUserProfileInstitution.BudgetUnitId = item.BudgetUnitId == null || item.BudgetUnitId == 0 ? null : item.BudgetUnitId;
                                relationshipUserProfileInstitution.ManagerUnitId = item.ManagerUnitId == null || item.ManagerUnitId == 0 ? null : item.ManagerUnitId;
                                relationshipUserProfileInstitution.AdministrativeUnitId = item.AdministrativeUnitId == null || item.AdministrativeUnitId == 0 ? null : item.AdministrativeUnitId;
                                relationshipUserProfileInstitution.SectionId = item.SectionId == null || item.SectionId == 0 ? null : item.SectionId;

                                db.Entry(relationshipUserProfileInstitution).State = EntityState.Added;
                                db.SaveChanges();
                            }
                        }

                        transaction.Complete();
                    }

                    return RedirectToAction("Index");
                }

                SetCarregaHierarquia();
                return View(user);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region ChangePassword Actions
        // GET: Users/ChangePassword/5
        public ActionResult ChangePassword(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                db = new SAMContext();
                if (id != db.Users.FirstOrDefault(m => m.CPF == HttpContext.User.Identity.Name && m.Status == true).Id)
                {
                    return RedirectToAction("Mensagem", "Home", new { m = "Você não pode alterar a senha que não seja do seu usuário." });
                }

                this.db.Configuration.AutoDetectChangesEnabled = false;
                this.db.Configuration.LazyLoadingEnabled = false;

                User user = db.Users.Find(id);
                if (user == null)
                    return HttpNotFound();

                UserViewModel userViewModel = new UserViewModel();

                Mapper.CreateMap<User, UserViewModel>();
                userViewModel = Mapper.Map<UserViewModel>(user);

                return View(userViewModel);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: users/ChangePassword/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword([Bind(Include = "Id, CPF, Status, Name, Email, Password, Phrase, AddedDate, InvalidAttempts, ChangePassword, NewPassword, ConfirmPassword, CurrentPassword")] UserViewModel user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (user.Password != Common.UserCommon.EncryptPassword(user.CurrentPassword))
                    {
                        ModelState.AddModelError("SenhaIncorreta", "A Senha informada não corresponde a senha atual!");
                        return View(user);
                    }

                    user.CPF = user.CPF.Replace(".", string.Empty).Replace("-", string.Empty);
                    user.Password = Common.UserCommon.EncryptPassword(user.NewPassword);
                    user.ChangePassword = false;

                    User u = new User();
                    Mapper.CreateMap<UserViewModel, User>();
                    u = Mapper.Map<User>(user);

                    db = new SAMContext();
                    db.Entry(u).State = EntityState.Modified;

                    db.SaveChanges();
                    FormsAuthentication.SignOut();
                    return RedirectToAction("Index", "Login");
                }
                return View(user);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion

        #region Delete Actions
        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                db = new SAMContext();
                User user = db.Users.Find(id);
                if (user == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                if (user.CPF.Equals(Session["UsuarioLogado"]))
                    return MensagemErro(CommonMensagens.ExcluirProprioUsuario);

                // Recupera o perfil do id informado
                var userHierarquia = (from r in db.RelationshipUserProfiles
                                      join u in db.Users on r.UserId equals u.Id
                                      join ri in db.RelationshipUserProfileInstitutions on r.Id equals ri.RelationshipUserProfileId
                                      where u.Id == user.Id && r.DefaultProfile == true
                                      select ri).AsNoTracking().FirstOrDefault();

                if (ValidarRequisicao(userHierarquia.InstitutionId, userHierarquia.BudgetUnitId, userHierarquia.ManagerUnitId, userHierarquia.AdministrativeUnitId, userHierarquia.SectionId))
                    return View(user);
                else
                    return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                db = new SAMContext();
                var user = (from u in db.Users
                            where u.Id == id
                            select u).FirstOrDefault();

                user.Status = false;
                user.Email = user.Email == null ? "teste@teste.com" : user.Email;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        #endregion

        #endregion
        #region Métodos Privados
        private void SetCarregaHierarquia()
        {
            GetIdsDaHierarquia();
            hierarquia = new Hierarquia();

            if (PerfilAdmGeral())
            {
                ViewBag.Profiles = new SelectList(hierarquia.GetProfiles(true), "Id", "Description");
                ViewBag.Institutions = new SelectList(hierarquia.GetOrgaos(null), "Id", "Description");
                ViewBag.BudgetUnits = new SelectList(hierarquia.GetUos(null), "Id", "Description");
                ViewBag.ManagerUnits = new SelectList(hierarquia.GetUges(null), "Id", "Description");
                ViewBag.AdministrativeUnits = new SelectList(hierarquia.GetUas(null), "Id", "Description");
                ViewBag.Sections = new SelectList(hierarquia.GetDivisoes(null), "Id", "Description");
            }
            else
            {
                var profiles = hierarquia.GetProfiles(false);

                profiles.RemoveAll(x => x.Id == (int)EnumProfile.AdministradorFinanceiroSEFAZ
                                      || x.Id == (int)EnumProfile.AdministradorGeral);

                if (_perfilId == (int)EnumProfile.AdministradordeUO)
                {
                    profiles.RemoveAll(x => x.Id == (int)EnumProfile.AdministradordeOrgao);
                }
                else if (_perfilId == (int)EnumProfile.OperadordeUO)
                {
                    profiles.RemoveAll(x => x.Id == (int)EnumProfile.AdministradordeOrgao);
                    profiles.RemoveAll(x => x.Id == (int)EnumProfile.AdministradordeUO);
                    profiles.RemoveAll(x => x.Id == (int)EnumProfile.AdministradorDoPatrimonio);
                }
                else if (_perfilId == (int)EnumProfile.OperadordeUGE)
                {
                    profiles.RemoveAll(x => x.Id == (int)EnumProfile.AdministradordeOrgao);
                    profiles.RemoveAll(x => x.Id == (int)EnumProfile.AdministradordeUO);
                    profiles.RemoveAll(x => x.Id == (int)EnumProfile.AdministradorDoPatrimonio);
                    profiles.RemoveAll(x => x.Id == (int)EnumProfile.OperadordeUO);
                }
                else if (_perfilId == (int)EnumProfile.Responsavel)
                {
                    profiles.RemoveAll(x => x.Id == (int)EnumProfile.AdministradordeOrgao);
                    profiles.RemoveAll(x => x.Id == (int)EnumProfile.AdministradordeUO);
                    profiles.RemoveAll(x => x.Id == (int)EnumProfile.AdministradorDoPatrimonio);
                    profiles.RemoveAll(x => x.Id == (int)EnumProfile.OperadordeUO);
                    profiles.RemoveAll(x => x.Id == (int)EnumProfile.OperadordeUGE);
                }

                ViewBag.Profiles = new SelectList(profiles, "Id", "Description");
                ViewBag.Institutions = new SelectList(hierarquia.GetOrgaos(_institutionId), "Id", "Description");

                if (_budgetUnitId.HasValue && _budgetUnitId != 0)
                    ViewBag.BudgetUnits = new SelectList(hierarquia.GetUos(_budgetUnitId), "Id", "Description");
                else
                    ViewBag.BudgetUnits = new SelectList(hierarquia.GetUosPorOrgaoId(_institutionId), "Id", "Description");

                if (_managerUnitId.HasValue && _managerUnitId != 0)
                    ViewBag.ManagerUnits = new SelectList(hierarquia.GetUges(_managerUnitId), "Id", "Description");
                else
                    ViewBag.ManagerUnits = new SelectList(hierarquia.GetUgesPorUoId(_budgetUnitId), "Id", "Description");

                if (_administrativeUnitId.HasValue && _administrativeUnitId != 0)
                    ViewBag.AdministrativeUnits = new SelectList(hierarquia.GetUas(_administrativeUnitId), "Id", "Description");
                else
                    ViewBag.AdministrativeUnits = new SelectList(hierarquia.GetUasPorUgeId(_managerUnitId), "Id", "Description");

                if (_sectionId.HasValue && _sectionId != 0)
                    ViewBag.Sections = new SelectList(hierarquia.GetDivisoes(_sectionId), "Id", "Description");
                else
                    ViewBag.Sections = new SelectList(hierarquia.GetDivisoesPorUaId(_administrativeUnitId), "Id", "Description");
            }
        }


        #endregion
    }
}