using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SAM.Web.Models;
using SAM.Web.Common;
using PagedList;
using System.Web.Script.Serialization;
using SAM.Web.ViewModels;
using Newtonsoft.Json;
using System.IO;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using SAM.Web.Common.Enum;

namespace SAM.Web.Controllers
{
    public class MobilesController : Controller
    {
        private SAMContext db = new SAMContext();
        private BaseController baseController = new BaseController();
        private int _institutionId;
        private int? _budgetUnitId;
        private int? _managerUnitId;
        private int? _administrativeUnitId;
        private int? _sectionId;

        public object Datatable { get; private set; }

        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            var lstRetorno = (from m in db.Mobiles select m)
                .Include(a => a.RelatedAdministrativeUnits)
                .Include(m => m.RelatedManagerUnits)
                .Include(b => b.RelatedBudgetUnits)
                .Include(i => i.RelatedInstitutions);

            //Filter
            lstRetorno = SearchByFilter(searchString, lstRetorno);
            //Pagination
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            var result = lstRetorno.OrderBy(s => s.Id).Skip(((pageNumber) - 1) * pageSize).Take(pageSize);
            var retorno = new StaticPagedList<Mobile>(result, pageNumber, pageSize, lstRetorno.Count());

            return View(retorno);
        }
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                    baseController.MensagemErro(CommonMensagens.IdentificadorNulo);

                var mobile = (from m in db.Mobiles where m.Id == id select m)
                    .Include(a => a.RelatedAdministrativeUnits)
                    .Include(m => m.RelatedManagerUnits)
                    .Include(b => b.RelatedBudgetUnits)
                    .Include(i => i.RelatedInstitutions);

                if (mobile == null)
                    return baseController.MensagemErro(CommonMensagens.RegistroNaoExistente);

                return View(mobile.ToList()[0]);
            }
            catch (Exception ex)
            {
                return baseController.MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        public ActionResult Create()
        {
            CarregaHierarquia();
            return View();
        }
        private void CarregaHierarquia()
        {
            ViewBag.AdministrativeUnitId = new SelectList(db.AdministrativeUnits, "Id", "Description");
            ViewBag.BudgetUnitId = new SelectList(db.BudgetUnits, "Id", "Description");
            ViewBag.InstitutionId = new SelectList(db.Institutions, "Id", "Description");
            ViewBag.ManagerUnitId = new SelectList(db.ManagerUnits, "Id", "Description");
        }
        public void getHierarquiaPerfil()
        {
            User u = UserCommon.CurrentUser();
            var perfLogado = baseController.BuscaHierarquiaPerfilLogadoPorUsuario(u.Id);
            _institutionId = perfLogado.InstitutionId;
            _budgetUnitId = perfLogado.BudgetUnitId;
            _managerUnitId = perfLogado.ManagerUnitId;
            _administrativeUnitId = perfLogado.AdministrativeUnitId;
            _sectionId = perfLogado.SectionId;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Brand,Model,MacAddress,InstitutionId,BudgetUnitId,ManagerUnitId,AdministrativeUnitId,Status")] Mobile mobile)
        {
            if (ModelState.IsValid && VerificaDuplicidade(mobile.MacAddress, mobile.InstitutionId))
            {
                db.Mobiles.Add(mobile);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            CarregaHierarquia();
            return View(mobile);
        }
        public string Login(string usuario, string senha)
        {
            User _usuario = new User();
            if (usuario != null && senha != null)
            {
                senha = UserCommon.EncryptPassword(senha);
                _usuario = (from l in db.Users where l.CPF == usuario && l.Password == senha select l).FirstOrDefault();

                if (_usuario != null)
                {
                    RelationshipUserProfile relacionamento = baseController.BuscaPerfilLogadoMobile(_usuario.Id);

                    if (relacionamento.ProfileId == (int)Common.Enum.EnumProfile.OperadordeUGE)
                    {
                        RelationshipUserProfileInstitution relacionaProfile = (from r in db.RelationshipUserProfileInstitutions where r.RelationshipUserProfileId == relacionamento.Id select r).FirstOrDefault();
                        return Retorna_UGE((int)relacionaProfile.ManagerUnitId);
                    }

                    if (relacionamento.ProfileId == (int)Common.Enum.EnumProfile.OperadordeUO)
                    {
                        RelationshipUserProfileInstitution relacionaProfile = (from r in db.RelationshipUserProfileInstitutions where r.RelationshipUserProfileId == relacionamento.Id select r).FirstOrDefault();
                        return Retorna_UGE_PorUO((int)relacionaProfile.BudgetUnitId);
                    }
                }
            }
            return null;
        }
        public string Retorna_UGE_PorUO(int uoId)
        {
            if (E_valido(uoId))
            {
                List<ManagerUnitMobileViewModel> lista_retorno = new List<ManagerUnitMobileViewModel>();

                var listauge = (from uge in db.ManagerUnits
                                where uge.BudgetUnitId == uoId && uge.Status == true
                                select uge);

                foreach (var item in listauge.OrderBy(x => x.Code).ToList())
                {
                    lista_retorno.Add(new ManagerUnitMobileViewModel
                    {
                        Id = item.Id,
                        Code = item.Code,
                        Description = item.Description
                    });
                }
                string retorno = JsonConvert.SerializeObject(lista_retorno);
                return retorno;
            }
            return null;
        }
        public string Retorna_UGE(int ugeId)
        {
            if (E_valido(ugeId))
            {
                List<ManagerUnitMobileViewModel> lista_retorno = new List<ManagerUnitMobileViewModel>();

                var listauge = (from uge in db.ManagerUnits
                                where uge.Id == ugeId && uge.Status == true
                                select uge).FirstOrDefault();

                lista_retorno.Add(new ManagerUnitMobileViewModel
                {
                    Id = listauge.Id,
                    Code = listauge.Code,
                    Description = listauge.Description
                });

                string retorno = JsonConvert.SerializeObject(lista_retorno);
                return retorno;
            }
            return null;
        }
        public string Retorna_UA(int ugeId)
        {
            if (E_valido(ugeId))
            {
                List<AdministrativeUnitMobileViewModel> lista_retorno = new List<AdministrativeUnitMobileViewModel>();

                var listaua = (from ua in db.AdministrativeUnits
                               where ua.ManagerUnitId == ugeId && ua.Status == true
                               select ua);

                foreach (var item in listaua.OrderBy(x => x.Code).ToList())
                {
                    lista_retorno.Add(new AdministrativeUnitMobileViewModel
                    {
                        Id = item.Id,
                        Code = item.Code,
                        Description = item.Description
                    });
                }
                string retorno = JsonConvert.SerializeObject(lista_retorno);
                return retorno;
            }
            return null;
        }
        public string Retorna_Responsaveis(int uaId)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();

            List<UserViewModel> lista_retorno = new List<UserViewModel>();

            var lista_responsavel = (from r in db.Responsibles
                                     where r.AdministrativeUnitId == uaId && r.Status == true
                                     select r).OrderBy(x => x.Name).ToList();

            // Mudar quando os responsaveis virarem usuarios
            //var lista_responsavel = (from r in db.Users
            //                         join a in db.RelationshipUserProfiles on r.Id equals a.UserId
            //                         join b in db.RelationshipUserProfileInstitutions on a.Id equals b.RelationshipUserProfileId
            //                         where b.AdministrativeUnitId == uaId && r.Status == true && a.FlagResponsavel == true
            //                         select r).OrderBy(x => x.Name).ToList();

            foreach (var item in lista_responsavel)
            {
                lista_retorno.Add(new UserViewModel
                {
                    Id = item.Id,
                    Name = item.Name
                });
            }

            string retorno = JsonConvert.SerializeObject(lista_retorno);
            return retorno;
        }
        public string Retorna_Divisoes(int responsavelId)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();

            List<SectionMobileViewModel> lista_retorno = new List<SectionMobileViewModel>();

            var lista_divisao = (from s in db.Sections where s.ResponsibleId == responsavelId && s.Status == true select s).OrderBy(x => x.Code).ToList();

            // Mudar quando os responsaveis virarem usuarios
            //var lista_divisao = (from s in db.Sections
            //                     join b in db.RelationshipUserProfileInstitutions on s.Id equals b.SectionId
            //                     join a in db.RelationshipUserProfiles on b.RelationshipUserProfileId equals a.Id
            //                     where a.UserId == responsavelId && a.FlagResponsavel == true
            //                     select s).OrderBy(x => x.Description).ToList();

            foreach (var item in lista_divisao)
            {
                lista_retorno.Add(new SectionMobileViewModel
                {
                    Id = item.Id,
                    Code = item.Code.ToString(),
                    Description = item.Description
                });
            }

            string retorno = JsonConvert.SerializeObject(lista_retorno);
            return retorno;
        }
        public string Retorna_BensPatrimoniais(int responsavelId)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            List<AssetMobileViewModel> lista_retorno = new List<AssetMobileViewModel>();

            //var lstAgrupada = (from am in db.AssetMovements
            //                   where am.Status == true
            //                   && am.SectionId == divisaoId
            //                   group am by new { am.AssetId} into g
            //                   select new AssetMovementsViewModelAgrupado
            //                   {
            //                       Ultimoregistro = g.Max(p => p.Id),
            //                       AssetId = g.Key.AssetId
            //                   });

            //var lstAssetMovements = (from am in db.AssetMovements
            //                         join r in lstAgrupada
            //                         on am.Id equals r.Ultimoregistro
            //                         select am);

            var lstAssetEAssetsMovement = (from a in db.Assets
                                           join am in db.AssetMovements on a.Id equals am.AssetId
                                           where am.Status == true &&
                                                 !a.flagVerificado.HasValue &&
                                                 am.ResponsibleId == responsavelId
                                           select new AssetEAssetMovimentViewModel
                                           {
                                               Asset = a,
                                               AssetMoviment = am
                                           });

            var lstDadosBD = lstAssetEAssetsMovement.AsQueryable();

            foreach (var item in lstDadosBD)
            {
                lista_retorno.Add(new AssetMobileViewModel
                {
                    AssetId = item.Asset.Id,
                    Code = item.Asset.NumberIdentification.ToString(),
                    Descricao = item.Asset.MaterialItemDescription,
                    Estado = 0,
                    Item = item.Asset.MaterialItemCode.ToString(),
                    InitialName = item.Asset.InitialName,
                });
            }

            string retorno = JsonConvert.SerializeObject(lista_retorno);
            return retorno;
        }
        private bool VerificaMacAddress(string macaddress)
        {
            bool retorno = false;
            var mobile = (from m in db.Mobiles where m.MacAddress == macaddress select m);

            if (mobile.ToList().Count > 0)
                retorno = true;

            return retorno;
        }
        private bool VerificaDuplicidade(string macAddress, int institutionId)
        {
            bool retorno = true;
            var result = (from m in db.Mobiles where m.MacAddress == macAddress && m.InstitutionId == institutionId select m);

            if (result.ToList().Count > 0)
                retorno = false;

            return retorno;
        }
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                    return baseController.MensagemErro(CommonMensagens.IdentificadorNulo);
                Mobile mobile = db.Mobiles.Find(id);
                if (mobile == null)
                    return baseController.MensagemErro(CommonMensagens.RegistroNaoExistente);

                CarregaHierarquia(mobile);
                return View(mobile);
            }
            catch (Exception ex)
            {
                return baseController.MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        private void CarregaHierarquia(Mobile mobile)
        {
            ViewBag.AdministrativeUnitId = new SelectList(db.AdministrativeUnits, "Id", "Description", mobile.AdministrativeUnitId);
            ViewBag.BudgetUnitId = new SelectList(db.BudgetUnits, "Id", "Description", mobile.BudgetUnitId);
            ViewBag.InstitutionId = new SelectList(db.Institutions, "Id", "Description", mobile.InstitutionId);
            ViewBag.ManagerUnitId = new SelectList(db.ManagerUnits, "Id", "Description", mobile.ManagerUnitId);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Brand,Model,MacAddress,InstitutionId,BudgetUnitId,ManagerUnitId,AdministrativeUnitId,Status")] Mobile mobile)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mobile).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AdministrativeUnitId = new SelectList(db.AdministrativeUnits, "Id", "Description", mobile.AdministrativeUnitId);
            ViewBag.BudgetUnitId = new SelectList(db.BudgetUnits, "Id", "Code", mobile.BudgetUnitId);
            ViewBag.InstitutionId = new SelectList(db.Institutions, "Id", "Code", mobile.InstitutionId);
            ViewBag.ManagerUnitId = new SelectList(db.ManagerUnits, "Id", "Code", mobile.ManagerUnitId);
            return View(mobile);
        }

        public string FinalizaInventario(string estimulo)
        {
            //string fmtDescricaoOrigemInventario = "Inventário realizado via {0}'{1}' em {2}";
            string fmtDescricaoOrigemInventario = "Inventário realizado via {0}'{1}'"; //Alteração solicitada por Marcia Cristina Mucheroni em 05/12/2018
            string descricaoOrigemInventario = null;
            string descritivoInventario = "Dispositivo ";
            string tipoInventario = null;
            using (var _transacation = db.Database.BeginTransaction())
            {
                try
                {
                    JavaScriptSerializer ser = new JavaScriptSerializer();
                    Inventario _Inventario = ser.Deserialize<Inventario>(estimulo);
                    _Inventario.DataInventario = DateTime.Now;

                    if (String.IsNullOrWhiteSpace(_Inventario.OrigemDados))
                        tipoInventario = "Android";
                    //else if (!String.IsNullOrWhiteSpace(_Inventario.TipoInventario) && (_Inventario.TipoInventario == TipoInventario.ColetorDados.GetHashCode().ToString()))
                    else if (_Inventario.TipoInventario.HasValue && (_Inventario.TipoInventario == EnumTipoInventario.ColetorDados.GetHashCode()))
                    {
                        descritivoInventario = "Coletor de Dados ";
                        tipoInventario = _Inventario.OrigemDados;
                    }
                    //else if (!String.IsNullOrWhiteSpace(_Inventario.TipoInventario) && (_Inventario.TipoInventario == TipoInventario.InventarioManual.GetHashCode().ToString()))
                    else if (_Inventario.TipoInventario.HasValue && (_Inventario.TipoInventario == EnumTipoInventario.InventarioManual.GetHashCode()))
                    {
                        descritivoInventario = null;
                        tipoInventario = "Relatório Inventário Manual";
                    }


                    descricaoOrigemInventario = String.Format(fmtDescricaoOrigemInventario, descritivoInventario, tipoInventario, DateTime.Now);
                    _Inventario.Descricao = descricaoOrigemInventario;
                    //_Inventario.Status = "0";
                    _Inventario.Status = EnumStatusInventario.Pendente.GetHashCode().ToString();

                    db.Inventarios.Add(_Inventario);
                    db.SaveChanges();

                    _transacation.Commit();
                    return JsonConvert.SerializeObject(_Inventario.Id);
                }
                catch (Exception ex)
                {
                    _transacation.Rollback();
                    return JsonConvert.SerializeObject(string.Empty);
                    // return JsonConvert.SerializeObject(mensagem.Mensagem = "ERRO: " + ex.Message);
                }
            }
        }
        public string FinalizaInventarioItem(string estimulo)
        {
            MensagemModel mensagem = new MensagemModel();
            mensagem.Id = 0;
            mensagem.Mensagem = "";
            using (var _transaction = db.Database.BeginTransaction())
            {
                try
                {
                    JavaScriptSerializer ser = new JavaScriptSerializer();

                    List<ItemInventario> _ItemInventario = ser.Deserialize<List<ItemInventario>>(estimulo);

                    _ItemInventario.RemoveAll(x => x.Code == null);

                    foreach (var item in _ItemInventario)
                    {
                        //if (item.Estado == 2)
                        if (item.Estado == EnumSituacaoFisicaBP.OutraUA.GetHashCode())
                        {
                            var patrimonio = (from p in db.Assets where p.NumberIdentification == item.Code && p.DiferenciacaoChapa == "" select p).FirstOrDefault();
                            if (patrimonio == null)
                                //item.Estado = 3;
                                item.Estado = EnumSituacaoFisicaBP.OutroResponsavel.GetHashCode();
                        }
                        else if (item.Estado == EnumSituacaoFisicaBP.Desconhecido.GetHashCode())
                        {
                            item.Estado = null;
                        }
                    }


                    db.ItemInventarios.AddRange(_ItemInventario);
                    db.SaveChanges();

                    _transaction.Commit();
                    return JsonConvert.SerializeObject(mensagem.Mensagem = "1");
                }
                catch (Exception ex)
                {
                    _transaction.Rollback();
                    return JsonConvert.SerializeObject(mensagem.Mensagem = "0");
                    // return JsonConvert.SerializeObject(mensagem.Mensagem = "ERRO: " + ex.Message);
                }
            }
        }
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                    return baseController.MensagemErro(CommonMensagens.IdentificadorNulo);

                var mobile = (from m in db.Mobiles where m.Id == id select m)
                    .Include(a => a.RelatedAdministrativeUnits)
                    .Include(m => m.RelatedManagerUnits)
                    .Include(b => b.RelatedBudgetUnits)
                    .Include(i => i.RelatedInstitutions);

                if (mobile == null)
                    return baseController.MensagemErro(CommonMensagens.RegistroNaoExistente);

                return View(mobile.ToList()[0]);
            }
            catch (Exception ex)
            {
                return baseController.MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Mobile mobile = db.Mobiles.Find(id);
            db.Mobiles.Remove(mobile);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Métodos privados
        private bool E_valido(int? valor)
        {
            bool retorno = false;

            if (valor != null && valor != 0)
                retorno = true;

            return retorno;
        }

        /// <summary>
        /// Filtra a pesquisa
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private IQueryable<Mobile> SearchByFilter(string searchString, IQueryable<Mobile> result)
        {
            if (!String.IsNullOrEmpty(searchString))
                result = result.Where(s => s.Name.Contains(searchString) || s.Brand.Contains(searchString) || s.Model.Contains(searchString));

            return result;
        }
        #endregion


        public void teste()
        {

            string table = "<!html pt-br><head></head><body><table>";
            string endtable = "</table></body></html>";
            string corpo = "";
            DataTable dt = new DataTable();
            dt.Columns.Add("Código", Type.GetType("System.String"));
            dt.Columns.Add("Descrição", Type.GetType("System.String"));
            dt.Columns.Add("Situação", Type.GetType("System.String"));
            dt.Columns.Add("Orgão Codigo", Type.GetType("System.String"));
            dt.Columns.Add("Orgão", Type.GetType("System.String"));
            dt.Columns.Add("UO Codigo", Type.GetType("System.String"));
            dt.Columns.Add("UO", Type.GetType("System.String"));
            dt.Columns.Add("UGE Codigo", Type.GetType("System.String"));
            dt.Columns.Add("UGE", Type.GetType("System.String"));

            //for (int i = 1; i < 9999999; i++)
            for (int i = 88000; i < 9999999; i++)
            {
                string teste = "000000";
                teste = teste + i.ToString();
                DataRow newCustomersRow = dt.NewRow();

                if (teste.Length > 7)
                    teste = teste.Substring((teste.Length - 7));

                var web = new HtmlWeb();
                var document = web.Load("http://www.fazenda.sp.gov.br/ua/ua.asp?ua=" + teste);
                var page = document.DocumentNode;

                var outerHTML = page.SelectNodes("//table")[2].OuterHtml;
                corpo += outerHTML.Substring(18).Replace("</table>", "");

                corpo = table + corpo + endtable;
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(corpo);
                corpo = "";

                newCustomersRow["Código"] = teste;
                if ((doc.DocumentNode.ChildNodes["body"].ChildNodes[0].ChildNodes[1].ChildNodes[3].ChildNodes[0]).InnerText != "")
                {
                    newCustomersRow["Descrição"] = (doc.DocumentNode.ChildNodes["body"].ChildNodes[0].ChildNodes[1].ChildNodes[3].ChildNodes[0]).InnerText.Substring(10);
                    newCustomersRow["Situação"] = (doc.DocumentNode.ChildNodes["body"].ChildNodes[0].ChildNodes[3].ChildNodes[3].ChildNodes[0]).InnerText;
                    newCustomersRow["Orgão Codigo"] = (doc.DocumentNode.ChildNodes["body"].ChildNodes[0].ChildNodes[5].ChildNodes[3].ChildNodes[0]).InnerText.Split('-')[0].Trim();
                    newCustomersRow["Orgão"] = (doc.DocumentNode.ChildNodes["body"].ChildNodes[0].ChildNodes[5].ChildNodes[3].ChildNodes[0]).InnerText.Split('-')[1].Trim();
                    newCustomersRow["UO Codigo"] = (doc.DocumentNode.ChildNodes["body"].ChildNodes[0].ChildNodes[7].ChildNodes[3].ChildNodes[0]).InnerText.Split('-')[0].Trim();
                    newCustomersRow["UO"] = (doc.DocumentNode.ChildNodes["body"].ChildNodes[0].ChildNodes[7].ChildNodes[3].ChildNodes[0]).InnerText.Split('-')[1].Trim();
                    newCustomersRow["UGE Codigo"] = (doc.DocumentNode.ChildNodes["body"].ChildNodes[0].ChildNodes[9].ChildNodes[3].ChildNodes[0]).InnerText.Split('-')[0].Trim();
                    newCustomersRow["UGE"] = (doc.DocumentNode.ChildNodes["body"].ChildNodes[0].ChildNodes[9].ChildNodes[3].ChildNodes[0]).InnerText.Split('-')[1].Trim();
                    dt.Rows.Add(newCustomersRow);
                }

            }
        }
    }
}
