using PagedList;
using SAM.Web.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SAM.Web.Common;
using SAM.Web.Context;
using SAM.Web.ViewModels;
using System.Collections.Generic;
using AutoMapper;
using System.Transactions;
using System.Data;
using System.Threading.Tasks;

namespace SAM.Web.Controllers
{
    public class SectionsController : BaseController
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
            User u = UserCommon.CurrentUser();
            var perflLogado = BuscaHierarquiaPerfilLogadoPorUsuario(u.Id);
            _institutionId = perflLogado.InstitutionId;
            _budgetUnitId = perflLogado.BudgetUnitId;
            _managerUnitId = perflLogado.ManagerUnitId;
            _administrativeUnitId = perflLogado.AdministrativeUnitId;
            _sectionId = perflLogado.SectionId;
        }

        #region Actions

        #region Index Actions

        // GET: Sections
        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page, string cbStatus)
        {
            try
            {
                ViewBag.CurrentFilterCbStatus = cbStatus;

                CarregaComboStatus(cbStatus);

                return View();
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        public async Task<JsonResult> IndexJSONResult(SectionsViewModel assetEAsset)
        {
            string draw = Request.Form["draw"].ToString();
            int startRec = Convert.ToInt32(Request.Form["start"].ToString());
            int length = Convert.ToInt32(Request.Form["length"].ToString());
            string currentFilter = Request.Form["currentFilter"].ToString();
            byte statusReq = byte.Parse(Request.Form["cbStatus"].ToString());
            bool status = Convert.ToBoolean(statusReq);
            string hierarquiaLogin = Request.Form["currentHier"].ToString();

            int totalRegistros = 0;
            IQueryable<SectionsViewModel> lstRetorno;

            try
            {
                if (hierarquiaLogin.Contains(','))
                {
                    int[] IdsHieraquia = Array.ConvertAll<string, int>(hierarquiaLogin.Split(','), int.Parse);
                    _institutionId = IdsHieraquia[0];
                    _budgetUnitId = IdsHieraquia[1];
                    _managerUnitId = IdsHieraquia[2];
                    _administrativeUnitId = IdsHieraquia[3];
                    _sectionId = IdsHieraquia[4];
                }
                else
                {
                    getHierarquiaPerfil();
                }

                using (db = new SAMContext())
                {
                    if (!string.IsNullOrEmpty(currentFilter) && !string.IsNullOrWhiteSpace(currentFilter))
                        lstRetorno = (from s in db.Sections
                                      where s.Status == status
                                      select new SectionsViewModel
                                      {
                                          Id = s.Id,
                                          Code = s.Code,
                                          Description = s.Description,
                                          AdministrativeUnitId = s.AdministrativeUnitId,
                                          Status = s.Status,
                                          CodigoUA = s.RelatedAdministrativeUnit.Code,
                                          DescricaoUA = s.RelatedAdministrativeUnit.Description,
                                          ManagerUnitId = s.RelatedAdministrativeUnit.RelatedManagerUnit.Id,
                                          CodigoUGE = s.RelatedAdministrativeUnit.RelatedManagerUnit.Code,
                                          BudgetUnitId = s.RelatedAdministrativeUnit.RelatedManagerUnit.RelatedBudgetUnit.Id,
                                          CodigoUO = s.RelatedAdministrativeUnit.RelatedManagerUnit.RelatedBudgetUnit.Code,
                                          InstitutionId = s.RelatedAdministrativeUnit.RelatedManagerUnit.RelatedBudgetUnit.RelatedInstitution.Id,
                                          CodigoOrgao = s.RelatedAdministrativeUnit.RelatedManagerUnit.RelatedBudgetUnit.RelatedInstitution.Code,
                                      })
                                      .Where(s => s.CodigoOrgao.Contains(currentFilter) ||
                                               s.CodigoUO.Contains(currentFilter) ||
                                               s.CodigoUGE.Contains(currentFilter) ||
                                               s.CodigoUA.ToString().Contains(currentFilter) ||
                                               s.DescricaoUA.Contains(currentFilter) ||
                                               s.Code.ToString().Contains(currentFilter) ||
                                               s.Description.Contains(currentFilter)
                                      ).AsNoTracking();
                    else
                        lstRetorno = (from s in db.Sections
                                      where s.Status == status
                                      select new SectionsViewModel
                                      {
                                          Id = s.Id,
                                          Code = s.Code,
                                          Description = s.Description,
                                          AdministrativeUnitId = s.AdministrativeUnitId,
                                          Status = s.Status,
                                          CodigoUA = s.RelatedAdministrativeUnit.Code,
                                          DescricaoUA = s.RelatedAdministrativeUnit.Description,
                                          ManagerUnitId = s.RelatedAdministrativeUnit.RelatedManagerUnit.Id,
                                          CodigoUGE = s.RelatedAdministrativeUnit.RelatedManagerUnit.Code,
                                          BudgetUnitId = s.RelatedAdministrativeUnit.RelatedManagerUnit.RelatedBudgetUnit.Id,
                                          CodigoUO = s.RelatedAdministrativeUnit.RelatedManagerUnit.RelatedBudgetUnit.Code,
                                          InstitutionId = s.RelatedAdministrativeUnit.RelatedManagerUnit.RelatedBudgetUnit.RelatedInstitution.Id,
                                          CodigoOrgao = s.RelatedAdministrativeUnit.RelatedManagerUnit.RelatedBudgetUnit.RelatedInstitution.Code
                                      }).AsNoTracking();

                    if (PerfilAdmGeral() != true)
                    {
                        if (ContemValor(_sectionId))
                            lstRetorno = (from s in lstRetorno where s.AdministrativeUnitId == _administrativeUnitId select s);
                        else if (ContemValor(_administrativeUnitId))
                            lstRetorno = (from s in lstRetorno where s.AdministrativeUnitId == _administrativeUnitId select s);
                        else if (ContemValor(_managerUnitId))
                            lstRetorno = (from s in lstRetorno where s.ManagerUnitId == _managerUnitId select s);
                        else if (ContemValor(_budgetUnitId))
                            lstRetorno = (from s in lstRetorno where s.BudgetUnitId == _budgetUnitId select s);
                        else if (ContemValor(_institutionId))
                            lstRetorno = (from s in lstRetorno where s.InstitutionId == _institutionId select s);
                    }

                    totalRegistros = await lstRetorno.CountAsync();

                    lstRetorno = lstRetorno.OrderBy(s => s.Id).Skip(startRec).Take(length);

                    lstRetorno = lstRetorno.OrderBy(s => s.CodigoOrgao)
                        .ThenBy(s => s.CodigoUO)
                        .ThenBy(s => s.CodigoUGE)
                        .ThenBy(s => s.CodigoUA)
                        .ThenBy(s => s.Code);

                    var result = await lstRetorno.ToListAsync();

                    foreach (var item in result)
                    {
                        item.Bp = (from am in db.AssetMovements where am.SectionId == item.Id && am.Status == true select am.Id).Count();
                    }

                    return Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRegistros, recordsFiltered = totalRegistros, data = result }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(MensagemErro(CommonMensagens.PadraoException, e), JsonRequestBehavior.AllowGet);
            }

        }


        #endregion Index Actions

        #region Details Actions

        // GET: Sections/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                using (db = new SAMContext())
                {

                    Section section = db.Sections.Find(id);                    

                    if (section == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    SectionsViewModel sectionModel = new SectionsViewModel(section);

                    if (ValidarRequisicao(sectionModel.InstitutionId, sectionModel.BudgetUnitId, sectionModel.ManagerUnitId, sectionModel.AdministrativeUnitId, sectionModel.Id))
                        return View(sectionModel);
                    else
                        return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);
                }
                
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        #endregion Details Actions

        #region Create Actions

        // GET: Sections/Create
        public ActionResult Create()
        {
            try
            {
                using (db = new SAMContext())
                {
                    SetCarregaHierarquia();
                }
                return View();
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: Sections/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SectionsViewModel section)
        {
            try
            {
                db = new SAMContext();
                if (ModelState.IsValid)
                {
                    if (section.ResponsibleId == null)
                    {
                        ModelState.AddModelError("ResponsibleId", "Informe um Responsável para esta Divisão!");
                        SetCarregaHierarquia(section.InstitutionId, section.BudgetUnitId, section.ManagerUnitId, section.AdministrativeUnitId);
                        return View(section);
                    }

                    if ((from b in db.Sections where b.Code == section.Code && b.RelatedAdministrativeUnit.Id == section.AdministrativeUnitId && b.Status == true select b).Any())
                    {
                        ModelState.AddModelError("CodigoJaExiste", "Código já está cadastrado em outra Divisao Ativa, nesta mesma UA!");
                        SetCarregaHierarquia(section.InstitutionId, section.BudgetUnitId, section.ManagerUnitId, section.AdministrativeUnitId);
                        return View(section);
                    }

                    Section _section = new Section();
                    Mapper.CreateMap<SectionsViewModel, Section>();
                    _section = Mapper.Map<Section>(section);

                    //Tratamento do CEP
                    _section.RelatedAddress.PostalCode = section.RelatedAddress.PostalCode.Replace("-", "");
                    _section.Status = true;
                    db.Sections.Add(_section);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }

                SetCarregaHierarquia(section.InstitutionId, section.BudgetUnitId, section.ManagerUnitId, section.AdministrativeUnitId);
                return View(section);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        #endregion

        #region Edit Actions

        // GET: Sections/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                using (db = new SAMContext())
                {
                    Section section = db.Sections.Find(id);
                    if (section == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    if (ValidarRequisicao(section.RelatedAdministrativeUnit.RelatedManagerUnit.RelatedBudgetUnit.RelatedInstitution.Id, section.RelatedAdministrativeUnit.RelatedManagerUnit.RelatedBudgetUnit.Id, section.RelatedAdministrativeUnit.RelatedManagerUnit.Id, section.AdministrativeUnitId, section.Id))
                    {
                        SectionsViewModel _section = new SectionsViewModel(section);
                        _section.AdministrativeUnitIdAux = _section.AdministrativeUnitId;
                        _section.MensagemModal = false;

                        SetCarregaHierarquia(_section.InstitutionId, _section.BudgetUnitId, _section.ManagerUnitId, _section.AdministrativeUnitId);
                        return View(_section);
                    }
                    else
                        return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: Sections/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SectionsViewModel section)
        {
            try
            {
                db = new SAMContext();
                if (!section.Status)
                {
                    int Bps = (from am in db.AssetMovements where am.SectionId == section.Id && am.Status == true select am.Id).Count();

                    if (Bps > 0)
                    {
                        ModelState.AddModelError("Status", "A Divisão não pode ser inativada por que possui " + Bps + " Bps atrelados a ela, para inativar faça uma 'Movimentação Interna' dos Bps dela para outra Divisão.");
                        SetCarregaHierarquia(section.InstitutionId, section.BudgetUnitId, section.ManagerUnitId, section.AdministrativeUnitId);
                        return View(section);
                    }
                }

                ModelState["RelatedAddress.Id"].Errors.Clear();
                if (ModelState.IsValid)
                {
                    if (section.ResponsibleId == null)
                    {
                        ModelState.AddModelError("ResponsibleId", "Informe um Responsável para esta Divisão!");
                        SetCarregaHierarquia(section.InstitutionId, section.BudgetUnitId, section.ManagerUnitId, section.AdministrativeUnitId);
                        return View(section);
                    }

                    using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted }))
                    {
                        if ((from b in db.Sections where b.Code == section.Code && b.RelatedAdministrativeUnit.Id == section.AdministrativeUnitId && b.Id != section.Id && b.Status == true select b).Any())
                        {
                            ModelState.AddModelError("CodigoJaExiste", "Código já está cadastrado em outra Divisao Ativa, nesta mesma UA!");
                            SetCarregaHierarquia(section.InstitutionId, section.BudgetUnitId, section.ManagerUnitId, section.AdministrativeUnitId);
                            return View(section);
                        }

                        if (section.ResponsibleId != section.ResponsibleIdAux)
                        {
                            var BPs = MovimentacoesAtualmenteAtreladasADivisao(section.AdministrativeUnitId, section.Id);
                            if (BPs.Count > 0)
                            {
                                var maisRecente = BPs.OrderByDescending(bp => bp.MovimentDate).FirstOrDefault();

                                if (MovimentacaoMaisRecenteAntesDoMesRefDaUGE(maisRecente))
                                {
                                    DateTime _dataParaMovimento = DataParaMovimento(section.ManagerUnitId);

                                    var retorno = Tranfere_BPs_Por_Responsavel_De_Secao(section.AdministrativeUnitId, section.Id, _dataParaMovimento, section.ResponsibleId, UserCommon.CurrentUser().CPF);

                                    if (retorno != null && retorno.Rows.Count > 0)
                                    {
                                        return MensagemErro("Erro na trasnferencia de BPs: " + retorno.Rows[0]["ErrorMessage"].ToString());
                                    }
                                }
                                else {
                                    string mesRef = maisRecente.MovimentDate.Month.ToString() + "/" + maisRecente.MovimentDate.Year.ToString();
                                    ModelState.AddModelError("BpExistentes", "Não é possível transferir o responsável dessa divisão, pois o histórico mais recente de um BP ativo, com o responsável atual e nessa divisão, foi realizado após o mês de referência atual da UGE. Para ser possível a realização da alteração, sugerimos que a UGE esteja com o mês de referência igual ou superior à " + mesRef);
                                    SetCarregaHierarquia(section.InstitutionId, section.BudgetUnitId, section.ManagerUnitId, section.AdministrativeUnitId);
                                    return View(section);
                                }
                            }
                        }

                        if (section.AddressId == null)
                        {
                            Address _address = section.RelatedAddress;
                            _address.PostalCode = section.RelatedAddress.PostalCode.Replace("-", "");
                            db.Addresses.Add(_address);
                            db.SaveChanges();

                            section.AddressId = _address.Id;
                        }

                        Section _section = new Section();
                        Mapper.CreateMap<SectionsViewModel, Section>();
                        _section = Mapper.Map<Section>(section);

                        //Tratamento do CEP
                        _section.RelatedAddress.PostalCode = section.RelatedAddress.PostalCode.Replace("-", "");
                        _section.Status = section.Status;

                        db.Entry(_section).State = EntityState.Modified;
                        db.Entry(_section.RelatedAddress).State = EntityState.Modified;
                        db.SaveChanges();

                        transaction.Complete();
                    }
                    return RedirectToAction("Index");
                }

                SetCarregaHierarquia(section.InstitutionId, section.BudgetUnitId, section.ManagerUnitId, section.AdministrativeUnitId);
                return View(section);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        #endregion

        #region Deletes Actions

        // GET: Sections/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                using (db = new SAMContext())
                {
                    Section section = db.Sections.Find(id);
                    if (section == null)
                        return MensagemErro(CommonMensagens.RegistroNaoExistente);

                    if (ValidarRequisicao(section.RelatedAdministrativeUnit.RelatedManagerUnit.RelatedBudgetUnit.RelatedInstitution.Id, section.RelatedAdministrativeUnit.RelatedManagerUnit.RelatedBudgetUnit.Id, section.RelatedAdministrativeUnit.RelatedManagerUnit.Id, section.AdministrativeUnitId, section.Id))
                        return View(section);
                    else
                        return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: Sections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                using (db = new SAMContext())
                {
                    int Bps = (from am in db.AssetMovements where am.SectionId == id && am.Status == true select am.Id).Count();

                    if (Bps > 0)
                        return MensagemErro("A Divisão não pode ser excluída por que possui " + Bps + " Bps atrelados a ela, para exclui-la faça uma 'Movimentação Interna' dos Bps dela para outra Divisão.");

                    Section section = db.Sections.Find(id);
                    section.Description = section.Description == null ? "SEM DESCRICAO" : section.Description;
                    section.Status = false;
                    db.Entry(section).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        #endregion 

        #endregion Actions

        #region Métodos Privados

        private void CarregaComboStatus(string codStatus)
        {
            var lista = new List<ItemGenericViewModel>();

            var itemGeneric = new ItemGenericViewModel
            {
                Id = 0,
                Description = "Inativos",
                Ordem = 10
            };

            lista.Add(itemGeneric);

            itemGeneric = new ItemGenericViewModel
            {
                Id = 1,
                Description = "Ativos",
                Ordem = 5
            };

            lista.Add(itemGeneric);

            if (codStatus != null && codStatus != "")
                ViewBag.Status = new SelectList(lista.OrderBy(x => x.Ordem), "Id", "Description", int.Parse(codStatus));
            else
                ViewBag.Status = new SelectList(lista.OrderBy(x => x.Ordem), "Id", "Description");
        }

        private bool ContemValor(int? variavel)
        {
            bool retorno = false;
            if (variavel.HasValue && variavel != null && variavel != 0)
                retorno = true;
            return retorno;
        }

        private void SetCarregaHierarquia(int modelInstitutionId = 0, int modelBudgetUnitId = 0, int modelManagerUnitId = 0, int modelAdministrativeUnitId = 0, int modeUserId = 0)
        {
            getHierarquiaPerfil();
            hierarquia = new Hierarquia();

            var vResponsible = new List<Responsible>();
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

                if (modelManagerUnitId != 0)
                    ViewBag.AdministrativeUnits = new SelectList(hierarquia.GetUasPorUgeId(modelManagerUnitId), "Id", "Description", modelAdministrativeUnitId);
                else
                    ViewBag.AdministrativeUnits = new SelectList(hierarquia.GetUas(null), "Id", "Description");
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

                if (_administrativeUnitId.HasValue && _administrativeUnitId != 0)
                    ViewBag.AdministrativeUnits = new SelectList(hierarquia.GetUas(_administrativeUnitId), "Id", "Description", modelAdministrativeUnitId);
                else if (modelManagerUnitId != 0)
                    ViewBag.AdministrativeUnits = new SelectList(hierarquia.GetUasPorUgeId(modelManagerUnitId), "Id", "Description", modelAdministrativeUnitId);
                else
                    ViewBag.AdministrativeUnits = new SelectList(hierarquia.GetUasPorUgeId(_managerUnitId), "Id", "Description", modelAdministrativeUnitId);
            }

            #region Combo Responsavel

            if (ContemValor(_administrativeUnitId))
            {
                vResponsible = (from r in db.Responsibles where r.AdministrativeUnitId == _administrativeUnitId && r.Status == true select r).AsNoTracking().ToList();
                ViewBag.User = new SelectList(vResponsible.ToList(), "Id", "Name");
            }
            else if (modelAdministrativeUnitId != 0)
            {
                vResponsible = (from r in db.Responsibles where r.AdministrativeUnitId == modelAdministrativeUnitId && r.Status == true select r).AsNoTracking().ToList();
                ViewBag.User = new SelectList(vResponsible.ToList(), "Id", "Name", modeUserId);
            }
            else
                ViewBag.User = new SelectList(vResponsible.ToList(), "Id", "Name");

            #endregion
        }

        private DateTime DataParaMovimento(int UGE)
        {
            var uge = (from m in db.ManagerUnits where m.Id == UGE select m.ManagmentUnit_YearMonthReference).FirstOrDefault();

            //DateTime com o último dia do MesRef UGE
            string ano = uge.Substring(0, 4);
            string mes = uge.Substring(4, 2);

            DateTime _DataParaMovimento = DateTime.Now;

            // Caso o MesRef seja igual ao mes corrente, grava o movimento com a data atual, caso seja menor grava com o ultimo dia do MesRef
            if (!(DateTime.Now.Month == int.Parse(mes) && DateTime.Now.Year == int.Parse(ano)))
                _DataParaMovimento = new DateTime(int.Parse(ano), int.Parse(mes), DateTime.DaysInMonth(int.Parse(ano), int.Parse(mes)));

            return _DataParaMovimento;
        }

        public bool MovimentacaoMaisRecenteAntesDoMesRefDaUGE(AssetMovements movimento)
        {
            if (movimento == null) return false;
            if(movimento.MovimentDate == null || movimento.ManagerUnitId <= 0) return false;

            DateTime UltimoDiaMesRef = MesRefDaUGE(movimento.ManagerUnitId);
            UltimoDiaMesRef = UltimoDiaMesRef.AddMonths(1).AddDays(-1);
            return movimento.MovimentDate <= UltimoDiaMesRef;
        }

        public List<AssetMovements> MovimentacoesAtualmenteAtreladasADivisao(int idUA, int idDivisao)
        {
            return (from am in db.AssetMovements
                    join a in db.Assets on am.AssetId equals a.Id
                    where !a.flagVerificado.HasValue && a.flagDepreciaAcumulada == 1 && am.Status == true && a.Status == true &&
                          am.AdministrativeUnitId == idUA && am.SectionId == idDivisao
                    select am).ToList();
        }

        private DateTime MesRefDaUGE(int UGE)
        {
            var uge = (from m in db.ManagerUnits where m.Id == UGE select m.ManagmentUnit_YearMonthReference).FirstOrDefault();

            if (uge == null) return new DateTime(1900, 1, 1);

            //DateTime com o último dia do MesRef UGE
            string ano = uge.Substring(0, 4);
            string mes = uge.Substring(4, 2);

            return new DateTime(Convert.ToInt32(ano), Convert.ToInt32(mes), 1);
        }

        #endregion

        #region Ajax
        public JsonResult VerificarBPsResponsavel(int UGE, int Ua, int Divisao)
        {
            MensagemModel mensagem = new MensagemModel();
            mensagem.Id = 0;
            mensagem.Mensagem = "";

            List<MensagemModel> mensagens = new List<MensagemModel>();

            using (db = new SAMContext())
            {
                List<AssetMovements> Bps = MovimentacoesAtualmenteAtreladasADivisao(Ua, Divisao);

                if (Bps.Count >= 1)
                {
                    var UltimaMovimentacao = Bps.OrderByDescending(bp => bp.MovimentDate).FirstOrDefault();

                    if (MovimentacaoMaisRecenteAntesDoMesRefDaUGE(UltimaMovimentacao))
                    {
                        DateTime _DataParaMovimento = DataParaMovimento(UGE);
                        mensagem.Id = 1;
                        mensagem.Mensagem = "Existe " + Bps.Count + " BP(s) vinculado(s) há esta Divisão, deseja gerar uma 'Movimentação Interna' com a data " + _DataParaMovimento.ToString().Substring(0, 10) + " transferindo o(s) BP(s) para o novo Responsável selecionado?";
                    }
                    else {
                        mensagem.Id = -1;
                        string mesRef = UltimaMovimentacao.MovimentDate.Month.ToString() + "/" + UltimaMovimentacao.MovimentDate.Year.ToString();
                        mensagem.Mensagem = "Não é possível transferir o responsável dessa divisão, pois o histórico mais recente de um BP ativo, com o responsável atual e nessa divisão, foi realizado após o mês de referência atual da UGE. Para ser possível a realização da alteração, sugerimos que a UGE esteja com o mês de referência igual ou superior à " + mesRef;
                    }

                    mensagens.Add(mensagem);
                    return Json(mensagens, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    mensagem.Id = 0;
                    mensagem.Mensagem = "";
                    mensagens.Add(mensagem);
                    return Json(mensagens, JsonRequestBehavior.AllowGet);
                }
            }
        }
        #endregion

        #region Chamadas de Procedures

        public DataTable Tranfere_BPs_Por_Responsavel_De_Secao(int administrativeUnitId, int sectionId, DateTime dataParaMovimento, int? responsibleId, string login)
        {
            string spName = "TRANSFERE_RESPONSAVEL_DIVISAO";

            List<ListaParamentros> listaParam = new List<ListaParamentros>();
            listaParam.Add(new ListaParamentros { nomeParametro = "@AdministrativeUnitId", valor = administrativeUnitId });
            listaParam.Add(new ListaParamentros { nomeParametro = "@SectionId", valor = sectionId });
            listaParam.Add(new ListaParamentros { nomeParametro = "@DataParaMovimento", valor = dataParaMovimento });
            listaParam.Add(new ListaParamentros { nomeParametro = "@ResponsibleId", valor = responsibleId });
            listaParam.Add(new ListaParamentros { nomeParametro = "@Login", valor = login });

            FunctionsCommon common = new FunctionsCommon();
            return common.ReturnDataFromStoredProcedureReport(listaParam, spName);
        }

        #endregion
    }
}