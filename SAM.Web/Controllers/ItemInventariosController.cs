using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PagedList;
using Sam.Common.Util;
using SAM.Web.Common;
using SAM.Web.Common.Enum;
using SAM.Web.Models;
using SAM.Web.ViewModels;
using System;
using System.Collections.Generic;
using SAM.Web.Controllers.IntegracaoContabilizaSP;
using System.Data.Entity;
using System.Linq;
using System.Transactions;
using System.Text;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SAM.Web.Controllers
{
    public class ItemInventariosController : BaseController
    {
        private SAMContext db = new SAMContext();
        private Hierarquia hierarquia;
        #region Sessao Aberta/Hierarquia

        private int _institutionId;
        private int? _budgetUnitId;
        private int? _managerUnitId;
        private int? _administrativeUnitId;
        private int? _sectionId;
        private string _login;
        #endregion Sessao Aberta/Hierarquia

        public void getHierarquiaPerfil()
        {
            User u = UserCommon.CurrentUser();
            var perflLogado = BuscaHierarquiaPerfilLogadoPorUsuario(u.Id);
            _institutionId = perflLogado.InstitutionId;
            _budgetUnitId = perflLogado.BudgetUnitId;
            _managerUnitId = perflLogado.ManagerUnitId;
            _administrativeUnitId = perflLogado.AdministrativeUnitId;
            _sectionId = perflLogado.SectionId;
            _login = u.CPF;
        }

        // GET: ItemInventarios
        public ActionResult Index(int InventarioId, string sortOrder, string searchString, string currentFilter, int? page)
        {
            try
            {
                var _inventario = (from s in db.Inventarios where s.Id == InventarioId select s).First();
                if (_inventario.IsNotNull())
                {
                    ViewBag.CurrentFilter = searchString;
                    ViewBag.DescricaoInventario = _inventario.Descricao;
                    ViewBag.InventarioId = InventarioId;
                    ViewBag.DetalhesResponsavel = String.Format("Responsável: {0}, UGE: {1}, UA: {2}", _inventario.RelatedResponsible.Name, _inventario.RelatedAdministrativeUnit.RelatedManagerUnit.Code, _inventario.RelatedAdministrativeUnit.Code);

                    // Atualizar os itens do inventario quando carregar a tela.
                    //AtualizarItensInventario(InventarioId);

                    int pageSize = 10;
                    int pageNumber = (page ?? 1);

                    var lstRetorno = (from s in db.ItemInventarios where s.InventarioId == InventarioId select s);
                    lstRetorno.ToList().ForEach(itemInventario => {
                        if (itemInventario.RelatedEstadoItemInventario != null)
                            itemInventario.DescricaoEstado = itemInventario.RelatedEstadoItemInventario.Descricao;
                        else
                            itemInventario.DescricaoEstado = string.Empty;

                        if (itemInventario.AssetId != null)
                        {
                            if (itemInventario.RelatedAsset.Status)
                            {
                                itemInventario.DescricaoDivisao = (from am in db.AssetMovements
                                                                   where am.AssetId == itemInventario.AssetId
                                                                      && am.Status
                                                                   select am.RelatedSection.Description).FirstOrDefault();
                            }
                            else
                            {
                                int IdUltimoHistorico = (from am in db.AssetMovements
                                                         where am.AssetId == itemInventario.AssetId
                                                            && am.FlagEstorno == null
                                                         select am.Id).Max();

                                itemInventario.DescricaoDivisao = (from am in db.AssetMovements
                                                                   where am.Id == IdUltimoHistorico
                                                                   select am.RelatedSection.Description).FirstOrDefault();
                            }
                        }

                        if (itemInventario.DescricaoDivisao == null)
                            itemInventario.DescricaoDivisao = string.Empty;
                    });

                    var lstRetornoComDivisao = lstRetorno.ToList();
                    var lstFiltro = SearchByFilter(searchString, lstRetornoComDivisao);

                    var result = lstFiltro.OrderBy(s => s.DescricaoDivisao).Skip(((pageNumber) - 1) * pageSize).Take(pageSize);
                    //string codigoDescricao = null;
                    double codigoItemMaterial = 0;
                    ItemSiafisico itemMaterial = null;
                    result.ToList().ForEach(itemInventario =>
                    {
                        if (Double.TryParse(itemInventario.Item, out codigoItemMaterial))
                            itemMaterial = db.ItemSiafisicos.Where(registroItemMaterial => registroItemMaterial.Cod_Item_Mat == codigoItemMaterial).FirstOrDefault();

                        if (itemMaterial != null)
                            itemInventario.Item = String.Format("{0} - {1}", itemInventario.Item, itemMaterial.Nome_Item_Mat);

                        if (itemInventario.Estado == 0 && itemInventario.AssetId != null) {
                            var BPEmOutroItemInventario = (from i in db.ItemInventarios
                                                           where i.Estado != 0
                                                           && i.AssetId == itemInventario.AssetId
                                                           select i).AsNoTracking().FirstOrDefault();

                            if (BPEmOutroItemInventario != null) {

                                switch (BPEmOutroItemInventario.Estado) {
                                    case (int)EnumSituacaoFisicaBP.OutraUA:
                                    case (int)EnumSituacaoFisicaBP.OutraUGE:
                                        itemInventario.RelatedEstadoItemInventario.Descricao = string.Format("Encontrado para UGE {0} (UA {1}) (aguardando movimentação)",
                                                                                                         BPEmOutroItemInventario.RelatedInventario.RelatedAdministrativeUnit.RelatedManagerUnit.Code,
                                                                                                         BPEmOutroItemInventario.RelatedInventario.RelatedAdministrativeUnit.Code);
                                        break;
                                    case (int)EnumSituacaoFisicaBP.Devolvido:
                                    case (int)EnumSituacaoFisicaBP.Transferido:
                                    case (int)EnumSituacaoFisicaBP.Incorporado:
                                    case (int)EnumSituacaoFisicaBP.Movimentado:
                                        itemInventario.RelatedEstadoItemInventario.Descricao = string.Format("{0} para UGE {1} (UA {2})",
                                                                                                         Enum.GetName(typeof(EnumSituacaoFisicaBP), BPEmOutroItemInventario.Estado),
                                                                                                         BPEmOutroItemInventario.RelatedInventario.RelatedAdministrativeUnit.RelatedManagerUnit.Code,
                                                                                                         BPEmOutroItemInventario.RelatedInventario.RelatedAdministrativeUnit.Code);
                                        break;
                                }

                            }

                        }
                    });

                    if (TempData["msgSucesso"] != null)
                    {
                        ViewBag.AlertComSucesso = (string)TempData["msgSucesso"];
                    }

                    var retorno = new StaticPagedList<ItemInventario>(result,
                    pageNumber, pageSize, lstFiltro.Count());

                    //if(!retorno.Any())
                    ViewBag.Status = _inventario.Status;
                    //else
                    //    ViewBag.Status = "1";

                    return View(retorno);
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // GET: ListagemRelatorioInventarioManual
        [HttpGet]
        public ActionResult ListagemRelatorioInventarioManual(int InventarioId = 0, string sortOrder = null, string searchString = null, string currentFilter = null, int? page = null)
        {
            try
            {
                var _inventario = (from s in db.Inventarios where s.Id == InventarioId select s).FirstOrDefault();
                if (_inventario.IsNotNull())
                {
                    ViewBag.DescricaoInventario = _inventario.Descricao;
                    ViewBag.InventarioId = InventarioId;
                    ViewBag.DetalhesResponsavel = String.Format("Responsável: {0}, UGE: {1}, UA: {2}", _inventario.RelatedResponsible.Name, _inventario.RelatedAdministrativeUnit.RelatedManagerUnit.Code, _inventario.RelatedAdministrativeUnit.Code);

                    // Atualizar os itens do inventario quando carregar a tela.
                    //AtualizarItensInventario(InventarioId);

                    int pageSize = 10;
                    int pageNumber = (page ?? 1);
                    ViewBag.Page = pageNumber;

                    var lstRetorno = (from s in db.ItemInventarios where s.InventarioId == InventarioId select s);
                    lstRetorno.ToList().ForEach(itemInventario => {
                        if (itemInventario.RelatedEstadoItemInventario != null)
                            itemInventario.DescricaoEstado = itemInventario.RelatedEstadoItemInventario.Descricao;
                        else
                            itemInventario.DescricaoEstado = string.Empty;

                        if (itemInventario.AssetId != null)
                        {
                            if (itemInventario.RelatedAsset.Status)
                            {
                                itemInventario.DescricaoDivisao = (from am in db.AssetMovements
                                                                   where am.AssetId == itemInventario.AssetId
                                                                      && am.Status
                                                                   select am.RelatedSection.Description).FirstOrDefault();
                            }
                            else
                            {
                                int IdUltimoHistorico = (from am in db.AssetMovements
                                                         where am.AssetId == itemInventario.AssetId
                                                            && am.FlagEstorno == null
                                                         select am.Id).Max();

                                itemInventario.DescricaoDivisao = (from am in db.AssetMovements
                                                                   where am.Id == IdUltimoHistorico
                                                                   select am.RelatedSection.Description).FirstOrDefault();
                            }
                        }

                        if (itemInventario.DescricaoDivisao == null)
                            itemInventario.DescricaoDivisao = string.Empty;
                    });

                    var lstRetornoComDivisao = lstRetorno.ToList();
                    var lstFiltro = SearchByFilter(searchString, lstRetornoComDivisao);

                    var result = lstFiltro.OrderBy(s => s.DescricaoDivisao).Skip(((pageNumber) - 1) * pageSize).Take(pageSize);
                    string codigoDescricao = null;
                    double codigoItemMaterial = 0;
                    ItemSiafisico itemMaterial = null;
                    result.ToList().ForEach(itemInventario =>
                    {
                        if (Double.TryParse(itemInventario.Item, out codigoItemMaterial))
                            itemMaterial = db.ItemSiafisicos.Where(registroItemMaterial => registroItemMaterial.Cod_Item_Mat == codigoItemMaterial).FirstOrDefault();

                        if (itemMaterial != null)
                            itemInventario.Item = String.Format("{0} - {1}", itemInventario.Item, itemMaterial.Nome_Item_Mat);
                    });

                    var retorno = new StaticPagedList<ItemInventario>(result, pageNumber, pageSize, lstRetorno.Count());
                    ViewBag.Status = _inventario.Status;

                    return View(retorno);
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        public JsonResult AtualizaRelatorioInventarioManual(ItemInventarioViewModel viewPagina)
        {
            JsonMensagem jsonMensagem = new JsonMensagem();

            try
            {

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                List<InventarioEncontradoJson> inventariosEncontradoJsons = serializer.Deserialize<List<InventarioEncontradoJson>>(viewPagina.ItemsInventarioID.Replace(@"\", "").Replace(";", ""));
                if (inventariosEncontradoJsons !=null || inventariosEncontradoJsons.Count > 0)
                {
                    foreach (var inventario in inventariosEncontradoJsons)
                    {
                        var _inventario = db.ItemInventarios.Where(x => x.Id == inventario.Id).FirstOrDefault();
                        if(_inventario != null)
                        {
                            _inventario.Estado = (inventario.Encontrado ? 1 : 0); 
                        }
                    }
                    db.SaveChanges();

                    jsonMensagem.Chave = "Sucesso";
                    jsonMensagem.Conteudo = "Atualização realizada com sucesso!";
                    //return Json(jsonMensagem, JsonRequestBehavior.AllowGet);
                    //return RedirectToAction("ListagemRelatorioInventarioManual", "ItemInventarios", new { InventarioId = viewPagina.InventarioId, sortOrder = string.Empty, searchString = string.Empty, currentFilter = string.Empty, page = viewPagina.Page });
                }
                else
                {
                   
                    jsonMensagem.Chave = "Erro";
                    jsonMensagem.Conteudo = "Selecione sim ou não na lista do inventário";
                    //return Json(jsonMensagem, JsonRequestBehavior.AllowGet);
                }


                return Json(jsonMensagem, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                jsonMensagem.Chave = "Erro";
                jsonMensagem.Conteudo = ex.Message;


                return Json(jsonMensagem, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Create(int InventarioId)
        {
            try
            {
                //TODO [Inventario] Verificar como incluir combo de siglas aqui
                getHierarquiaPerfil();

                ViewBag.Initial = new SelectList(db.Initials.Where(i => i.InstitutionId == _institutionId && (i.BudgetUnitId == _budgetUnitId || i.BudgetUnitId == null) && (i.ManagerUnitId == _managerUnitId || i.ManagerUnitId == null) && i.Status == true).OrderBy(i => i.Name), "Id", "Name");
                ViewBag.InventarioId = InventarioId;
                return View();
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ItemInventario itemInventario)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var _patrimonioDuplicado = db.ItemInventarios.Where(i => i.Code == itemInventario.Code && i.InventarioId == itemInventario.InventarioId);
                    if (_patrimonioDuplicado.Any())
                    {
                        ModelState.AddModelError("ItemExistente", "Bem Patrimonial já está cadastrado neste inventário!");
                        return View();
                    }

                    db.ItemInventarios.Add(itemInventario);
                    db.SaveChanges();

                    return RedirectToAction("Index", new { InventarioId = itemInventario.InventarioId });
                }
                ViewBag.InventarioId = itemInventario.InventarioId;
                ModelState.AddModelError("ItemExistente", "");
                return View();
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // Method to extract the maximum value 
        static int extractMaximum(String str)
        {
            int num = 0, res = 0;

            // Start traversing the given string 
            for (int i = 0; i < str.Length; i++)
            {

                // If a numeric value comes, start  
                // converting it into an integer  
                // till there are consecutive 
                // numeric digits 
                if (char.IsDigit(str[i]))
                    num = num * 10 + (str[i] - '0');

                // Update maximum value 
                else
                {
                    res = Math.Max(res, num);

                    // Reset the number 
                    num = 0;
                }
            }

            // Return maximum value 
            return Math.Max(res, num);
        }

        public JsonResult PesquisarBemPatrimonial(string Chapa, int InventarioId)
        {
            ItemInventarioViewModel _itemInventario = new ItemInventarioViewModel();
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            //var _patrimonio = (from s in db.Assets join b in db.AssetMovements on s.Id equals b.AssetId where s.NumberIdentification == Chapa && s.Status == true && b.Status == true select s).Max();
            //var _patrimonio = (from s in db.Assets join b in db.AssetMovements on s.Id equals b.AssetId where s.NumberIdentification == Chapa && s.Status == true && b.Status == true select s).FirstOrDefault();
            var _patrimonio = (from s in db.Assets
                               join b in db.AssetMovements on s.Id equals b.AssetId
                               where s.NumberIdentification == Chapa
                                  //&& s.InitialName == Sigla
                                  && s.Status == true 
                                  && b.Status == true
                               select s).FirstOrDefault();

            Inventario _inventario = (from i in db.Inventarios where i.Id == InventarioId select i).FirstOrDefault();

            if (_patrimonio == null)
            {
                _itemInventario.Mensagem = "Bem Patrimonial não está cadastrado!";
                _itemInventario.Code = Chapa;
                _itemInventario.Estado = 3;
                _itemInventario.EstadoDescricao = (from i in db.EstadoItemInventarios where i.Id == 3 select i.Descricao).FirstOrDefault();
                _itemInventario.Item = "Bem Patrimonial pendente de cadastro";

                var retorno = serializer.Serialize(_itemInventario);
                return Json(retorno, JsonRequestBehavior.AllowGet);
            }
            var _patrimonioDuplicado = db.ItemInventarios.Where(i => i.Code == _patrimonio.NumberIdentification && i.InventarioId == _inventario.Id);
            if (_patrimonioDuplicado.Any())
            {
                _itemInventario.Mensagem = "Bem Patrimonial já está cadastrado neste inventário!";

                var retorno = serializer.Serialize(_itemInventario);
                return Json(retorno, JsonRequestBehavior.AllowGet);
            }
            //if (_inventario.UaId != _patrimonio.AdministrativeUnitId)
            //{
            //    _itemInventario.Mensagem = "Bem Patrimonio pertence a UA: " + _patrimonio.AdministrativeUnitId + " - " + _patrimonio.RelatedAdministrativeUnit.Description + " , favor verificar.";
            //    _itemInventario.Code = _patrimonio.NumberIdentification;
            //    _itemInventario.Estado = 2;
            //    _itemInventario.EstadoDescricao = (from i in db.EstadoItemInventarios where i.Id == 2 select i.Descricao).FirstOrDefault();
            //    _itemInventario.Item = _patrimonio.MaterialItemDescription;

            //    var retorno = serializer.Serialize(_itemInventario);
            //    return Json(retorno, JsonRequestBehavior.AllowGet);

            //}
            //if (_inventario.DivisaoId != _patrimonio.SectionId)
            //{
            //    _itemInventario.Mensagem = "Bem Patrimonio pertence a Divisão: " + _patrimonio.SectionId + " - " + _patrimonio.RelatedSection.Description + " , favor verificar.";
            //    _itemInventario.Code = _patrimonio.NumberIdentification;
            //    _itemInventario.Estado = 2;
            //    _itemInventario.EstadoDescricao = (from i in db.EstadoItemInventarios where i.Id == 2 select i.Descricao).FirstOrDefault();
            //    _itemInventario.Item = _patrimonio.MaterialItemDescription;

            //    var retorno = serializer.Serialize(_itemInventario);
            //    return Json(retorno, JsonRequestBehavior.AllowGet);
            //}
            else
            {
                _itemInventario.Code = _patrimonio.NumberIdentification;
                _itemInventario.Estado = 1;
                _itemInventario.EstadoDescricao = (from i in db.EstadoItemInventarios where i.Id == 1 select i.Descricao).FirstOrDefault();
                _itemInventario.Item = _patrimonio.MaterialItemDescription;

                var retorno = serializer.Serialize(_itemInventario);
                return Json(retorno, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult AtualizarItens(int InventarioId)
        {
            AtualizarItensInventario(InventarioId);
            return Json(new object(), JsonRequestBehavior.AllowGet);
        }

        public void AtualizarItensInventario(int InventarioId)
        {
            var _ItemInventario = (from a in db.ItemInventarios where a.InventarioId == InventarioId && a.Estado != 1 select a).ToList();

            if (_ItemInventario.Any())
            {
                var _inventario = (from a in db.Inventarios where a.Id == InventarioId select a).FirstOrDefault();

                foreach (var item in _ItemInventario)
                {
                    if (item.Estado != 0)
                    {
                        var _asset = (from a in db.Assets
                                      join b in db.AssetMovements on a.Id equals b.AssetId
                                      where 
                                            a.NumberIdentification == item.Code 
                                      &&    b.AdministrativeUnitId == _inventario.UaId 
                                      && b.SectionId == _inventario.DivisaoId 
                                      && a.Status == true 
                                      && b.Status == true
                                      select a).Max();//(a => a.NumberIdentification);

                        //if (_asset.Count == 1)
                        //{
                            item.Item = _asset.MaterialItemDescription;
                            item.Estado = 1;
                            db.Entry(item).State = EntityState.Modified;
                            db.SaveChanges();
                        //}
                    }
                }
            }
        }

        public ActionResult Details(int? id, int tela)
        {
            try
            {
                if(id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                var itemInventario = db.ItemInventarios.Where(i => i.Id == id).FirstOrDefault();

                if(itemInventario == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                if(itemInventario.AssetId == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);


                var asset = db.Assets
                            .Where(a => a.Id == itemInventario.AssetId)
                            .Select(a => new ItemInventarioBPViewModel {
                                Chapa = a.NumberIdentification,
                                Sigla = a.RelatedInitial.Name,
                                GrupoMaterial = a.MaterialGroupCode,
                                CodigoItemMaterial = a.MaterialItemCode,
                                DescricaoMaterial = a.MaterialItemDescription,
                                InventarioId = itemInventario.InventarioId,
                                Status = a.Status
                            }).FirstOrDefault();

                if (asset == null) 
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                AssetMovements HistoricoDoBP;

                if (asset.Status)
                    HistoricoDoBP = (from am in db.AssetMovements where am.AssetId == itemInventario.AssetId && am.Status == true select am).AsNoTracking().FirstOrDefault();
                else
                    HistoricoDoBP = (from am in db.AssetMovements where am.AssetId == itemInventario.AssetId && am.FlagEstorno == null select am).AsNoTracking().OrderByDescending(x => x.Id).FirstOrDefault();

                asset.Orgao = HistoricoDoBP.RelatedInstitution.Description;
                asset.UO = HistoricoDoBP.RelatedBudgetUnit.Description;
                asset.UGE = HistoricoDoBP.RelatedManagerUnit.Description;
                asset.UA = (HistoricoDoBP.RelatedAdministrativeUnit == null ? string.Empty : HistoricoDoBP.RelatedAdministrativeUnit.Description);
                asset.Divisao = (HistoricoDoBP.RelatedSection == null ? string.Empty : HistoricoDoBP.RelatedSection.Description);
                asset.Responsavel = (HistoricoDoBP.RelatedResponsible == null ? string.Empty : HistoricoDoBP.RelatedResponsible.Name);
                asset.ContaContabil = (HistoricoDoBP.RelatedAuxiliaryAccount == null ? string.Empty : HistoricoDoBP.RelatedAuxiliaryAccount.Description);

                ViewBag.ParametroTela = tela;

                return View(asset);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        // GET: Assets/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                ItemInventario _itemInventario = db.ItemInventarios.Find(id);
                if (_itemInventario == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                return View(_itemInventario);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: Assets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                ItemInventario _itemInventario = db.ItemInventarios.Find(id);

                int _inventarioId = _itemInventario.InventarioId;

                db.ItemInventarios.Remove(_itemInventario);
                db.SaveChanges();
                return RedirectToAction("Index", new { InventarioId = _inventarioId });
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        private List<ItemInventario> SearchByFilter(string searchString, List<ItemInventario> result)
        {
            if (!String.IsNullOrEmpty(searchString))
                result = result.Where(s => s.Item.Contains(searchString) ||
                                  s.InitialName.Contains(searchString) ||
                                  s.Code.Contains(searchString) || 
                                  s.DescricaoEstado.Contains(searchString) ||
                                  s.DescricaoDivisao.Contains(searchString)).ToList();
            return result;
        }

        #region MovimentoBPNaoEncontrado

        [HttpGet]
        public ActionResult MovimentoItemInventario(int? ItemInventarioId) {
            try
            {

                if (ItemInventarioId == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                var BP = (from i in db.ItemInventarios
                          join a in db.Assets on i.AssetId equals a.Id
                          where i.Id == ItemInventarioId select a).FirstOrDefault();

                if (BP == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                var PrecisaDeReclassificacao = (from rc in db.BPsARealizaremReclassificacaoContabeis
                                                where rc.AssetId == BP.Id
                                                select rc.Id).Count() > 0;

                if(PrecisaDeReclassificacao)
                    return MensagemErro("Bem Patrimonial precisa atualizar a conta contábil para ser feita a movimentação. Por gentileza, reclassifique a conta desse BP na tela Reclassificação Contábil");

                getHierarquiaPerfil();

                if (UGEEstaComPendenciaSIAFEMNoFechamento(_managerUnitId))
                    return MensagemErro(CommonMensagens.OperacaoInvalidaIntegracaoFechamento);

                CarregaHierarquiaFiltro(_institutionId, _budgetUnitId ?? 0, _managerUnitId ?? 0);
                CarregaHierarquiaDestino();

                using (db = new SAMContext())
                {
                    CarregaComboTipoMovimento();
                    CarregaComboResponsavel();
                    var numeroInventario = (from i in db.ItemInventarios where i.Id == ItemInventarioId select i.InventarioId).FirstOrDefault();
                    ViewBag.InventarioId = numeroInventario;

                    var movimentoViewModel = new MovimentoViewModel();
                    movimentoViewModel.AssetId = (int)BP.Id;
                    movimentoViewModel.UGESiafem = _managerUnitId == null ? string.Empty : db.ManagerUnits.Find((int)_managerUnitId).Code;
                    movimentoViewModel.ItemInventarioId = (int)ItemInventarioId;
                    movimentoViewModel.ManagerUnitId = (from i in db.Inventarios
                                                        join it in db.ItemInventarios
                                                        on i.Id equals it.InventarioId
                                                        where it.Id == ItemInventarioId
                                                        select i.UgeId).FirstOrDefault();
                    //movimentoViewModel.listaAssetEAssetViewModel = ListaHistoricoDeMovimentacoes(id);

                    return View(movimentoViewModel);
                }
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MovimentoItemInventario([Bind(Include = "AssetId, ItemInventarioId, MovementTypeId, InstituationId, BudgetUnitId, ManagerUnitId, InstituationIdDestino, BudgetUnitIdDestino, ManagerUnitIdDestino, AdministrativeUnitIdDestino,  SectionIdDestino, ResponsibleId, NumberProcess, MovimentoDate, Observation, RepairValue, SearchString, CPFCNPJ, ListAssetsParaMovimento, listaAssetEAssetViewModel, LoginSiafem, SenhaSiafem, UGESiafem")] MovimentoViewModel movimentoViewModel)
        {
            try
            {
                getHierarquiaPerfil();

                var mensagemErro = ValidaDataDeMovimentacao(movimentoViewModel.MovimentoDate, movimentoViewModel);
                if (mensagemErro != string.Empty)
                {
                    ModelState.AddModelError("MovimentoDate", mensagemErro);
                }
                else
                {

                    bool orgaoEhImplantado = (from i in db.Institutions
                                              where i.Id == movimentoViewModel.InstituationIdDestino
                                              select i.flagImplantado).FirstOrDefault();

                    if (orgaoEhImplantado == true)
                    {
                        //Valida se a data de transferencia segue as normais padrões
                        mensagemErro = ValidaMesRefUGEDestinoIgualUGEOrigem(movimentoViewModel);
                        if (mensagemErro != string.Empty)
                        {
                            ModelState.AddModelError("MovimentoDate", mensagemErro);
                        }
                    }
                }

                if (!ValidaUltimoHistorico(movimentoViewModel.AssetId, movimentoViewModel.MovementTypeId))
                {
                    ModelState.AddModelError("MsgMovimentacaoEmLote", "O Bem Patrimonial não pode realizar essa movimentação devido ao seu tipo, conta contábil ou último histórico realizado.");
                }

                if (UGEIntegradaAoSIAFEM(movimentoViewModel.ManagerUnitId))
                {
                    if (InvalidaSemLoginSiafem(movimentoViewModel.MovementTypeId, movimentoViewModel.LoginSiafem, movimentoViewModel.SenhaSiafem))
                    {
                        ModelState.AddModelError("NaoDigitouLoginSiafem", "Login SIAFEM obrigatório");
                    }

                    if (!BPSemPendenciaDeNL(movimentoViewModel.AssetId))
                    {
                        ModelState.AddModelError("MsgMovimentacaoEmLote", "O Bem patrimonial não pode ser movimentado pois o BP possui pendência(s) de NL(s). Por favor, resolva essas pendências na tela 'Notas de Lançamentos Pendentes SIAFEM' ou entre contato com seu superior.");
                    }
                }

                switch (movimentoViewModel.MovementTypeId)
                {
                    // Tarefa tira a obrigatoriedade da divisao - 14-09-2018
                    case (int)EnumMovimentType.MovimentacaoInterna:
                        ModelState.Remove("SectionIdDestino");

                        var item = db.AssetMovements
                                     .Where(am => am.Status && am.AssetId == movimentoViewModel.AssetId)
                                     .FirstOrDefault();

                        if (movimentoViewModel.AdministrativeUnitIdDestino == item.AdministrativeUnitId && 
                            movimentoViewModel.ResponsibleId == item.ResponsibleId && 
                            movimentoViewModel.SectionIdDestino == (item.SectionId == null ? 0 : item.SectionId))
                        {
                            ModelState.AddModelError("MsgMovimentacaoEmLote", "Não permitida a movimentação de BPs de um responsável para ele mesmo.");
                            break;
                        }
                        break;
                    case (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado:
                        if (movimentoViewModel.ManagerUnitId == movimentoViewModel.ManagerUnitIdDestino)
                        {
                            ModelState.AddModelError("ManagerUnitIdDestino", "Transferência(s) de BP(s) para mesma UGE serão realizadas via Movimentação Interna");
                        }

                        break;


                        //travado
                    case (int)EnumMovimentType.MovMudancaCategoriaDesvalorizacao:
                        if (!BPTravado(movimentoViewModel.AssetId))
                        {
                            ModelState.AddModelError("MsgMovimentacaoEmLote", "Somente BPs do fora grupo 88 são aceitos para essa movimentação"); // diferente de 88 nao movimenta, 88 movimenta
                        }
                        break;
                    case (int)EnumMovimentType.MovComodatoTerceirosRecebidos:
                        if (!BPsSaoTerceiros(movimentoViewModel.AssetId))
                        {
                            ModelState.AddModelError("MsgMovimentacaoEmLote", "O BP precisa ser do tipo Terceiro para realizar essa movimentação");
                        }
                        break;
                    case (int)EnumMovimentType.MovSaidaInservivelUGETransferencia:
                        if (!MesmaGestaoDoFundoSocial(movimentoViewModel.InstituationId))
                        {
                            ModelState.AddModelError("MsgMovimentacaoEmLote", "UGE somente pode realizar saída de inservível como SAÍDA INSERVÍVEL DA UGE - DOAÇÃO");
                        }
                        else
                        {
                            if (movimentoViewModel.ManagerUnitId == movimentoViewModel.ManagerUnitIdDestino)
                            {
                                ModelState.AddModelError("ManagerUnitIdDestino", "Saída de Inservível precisa que a UGE de destino seja diferente da origem");
                            }

                            if (!BPsNaContaContabilInservivel(movimentoViewModel.AssetId))
                            {
                                ModelState.AddModelError("MsgMovimentacaoEmLote", "Movimentação permitida somente com inservíveis (BPs na conta contábil 123110805)");
                            }
                        }
                        break;
                    case (int)EnumMovimentType.MovSaidaInservivelUGEDoacao:
                        if (MesmaGestaoDoFundoSocial(movimentoViewModel.InstituationId))
                        {
                            ModelState.AddModelError("MsgMovimentacaoEmLote", "UGE somente pode realizar saída de inservível como SAÍDA INSERVÍVEL DA UGE - TRANSFERÊNCIA");
                        }
                        else
                        {
                            if (!BPsNaContaContabilInservivel(movimentoViewModel.AssetId))
                            {
                                ModelState.AddModelError("MsgMovimentacaoEmLote", "Movimentação permitida somente com inservívels (BPs na conta contábil 123110805)");
                            }
                        }
                        break;
                    case (int)EnumMovimentType.MovPerdaInvoluntariaInservivelBensMoveis:
                        if (!BPsNaContaContabilInservivel(movimentoViewModel.AssetId))
                        {
                            ModelState.AddModelError("MsgMovimentacaoEmLote", "Movimentação permitida somente para inservíveis (BPs na conta contábil 123110805)");
                        }
                        break;
                    case (int)EnumMovimentType.MovVendaLeilaoSemoventes:
                        if (BPDoGrupoAnimal(movimentoViewModel.AssetId))
                            ModelState.AddModelError("MsgMovimentacaoEmLote", "Somente BPs do grupo 88 são aceitos para essa movimentação");
                        break;
                }


                if (ModelState.IsValid)
                {
                    if (UGEEstaComPendenciaSIAFEMNoFechamento(movimentoViewModel.ManagerUnitId))
                        return MensagemErro(CommonMensagens.OperacaoInvalidaIntegracaoFechamento);

                    //Numero de documento gerado para a AssetMovements, ele é unico para todos os itens selecionados.
                    string NumberDocGerado = "";
                    IList<AssetMovements> listaMovimentacoesPatrimoniaisEmLote = new List<AssetMovements>();
                    int InventarioIdDoItem = 0;

                    using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted }))
                    {
                        var assetEAssetMovimentBD = (from a in db.Assets
                                                     join am in db.AssetMovements
                                                     on a.Id equals am.AssetId
                                                     where am.Status == true &&
                                                           am.AssetId == movimentoViewModel.AssetId &&
                                                           !a.flagVerificado.HasValue &&
                                                           a.flagDepreciaAcumulada == 1
                                                     select new AssetEAssetMovimentViewModel
                                                     {
                                                         Asset = a,
                                                         AssetMoviment = am
                                                     }).FirstOrDefault();

                        //Desativa a movimentação anterior ---------------------------------------------------------------------------------------------------------------------------------------------
                        var oldsAssetMoviment = (from am in db.AssetMovements
                                                 where am.Status == true &&
                                                       am.AssetId == assetEAssetMovimentBD.Asset.Id
                                                 select am).ToList();

                        int? ContaContabilAntesContaInservivel = null;

                        foreach (var oldAssetMovimentDB in oldsAssetMoviment)
                        {
                            if (oldAssetMovimentDB.Status == true)
                            {
                                ContaContabilAntesContaInservivel = oldAssetMovimentDB.AuxiliaryAccountId;
                                oldAssetMovimentDB.Status = false;

                                db.Entry(oldAssetMovimentDB).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }

                        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                        //Entidade para ser salva no banco, com os campos comuns preenchidos
                        var assetMoviment = new AssetMovements();
                        assetMoviment.AssetId = movimentoViewModel.AssetId;
                        assetMoviment.MovimentDate = Convert.ToDateTime(movimentoViewModel.MovimentoDate);
                        assetMoviment.MovementTypeId = movimentoViewModel.MovementTypeId;
                        assetMoviment.StateConservationId = assetEAssetMovimentBD.AssetMoviment.StateConservationId;
                        assetMoviment.NumberPurchaseProcess = movimentoViewModel.NumberProcess;
                        assetMoviment.InstitutionId = _institutionId;
                        assetMoviment.BudgetUnitId = (from m in db.ManagerUnits where m.Id == movimentoViewModel.ManagerUnitId select m.BudgetUnitId).FirstOrDefault();
                        assetMoviment.ManagerUnitId = movimentoViewModel.ManagerUnitId;
                        assetMoviment.AssetTransferenciaId = null;
                        assetMoviment.Observation = movimentoViewModel.Observation;
                        assetMoviment.Login = _login;
                        assetMoviment.DataLogin = DateTime.Now;

                        if (movimentoViewModel.CPFCNPJ != null && movimentoViewModel.CPFCNPJ != "")
                        {
                            assetMoviment.CPFCNPJ = movimentoViewModel.CPFCNPJ.Replace(".", string.Empty).Replace("-", string.Empty).Replace("/", string.Empty);
                        }

                        // Gera Numero de Documento (ANO + COD_UGE + SEQUENCIA)
                        string Sequencia = BuscaUltimoNumeroDocumentoSaida(assetEAssetMovimentBD.AssetMoviment.ManagerUnitId);

                        //string Sequencia = (from am in db.AssetMovements where am.ManagerUnitId == assetEAssetMovimentBD.AssetMoviment.ManagerUnitId select am.NumberDoc).Max();

                        if (Sequencia.Equals("0"))
                            assetMoviment.NumberDoc = DateTime.Now.Year + assetEAssetMovimentBD.AssetMoviment.RelatedManagerUnit.Code + "0001";
                        else
                        {
                            if (Sequencia.Length > 14)
                            {
                                int contador = Convert.ToInt32(Sequencia.Substring(10, Sequencia.Length - 10));

                                if (contador < 9999)
                                {
                                    assetMoviment.NumberDoc = Sequencia.Substring(0, 10) + (contador + 1).ToString().PadLeft(4, '0');
                                }
                                else
                                {
                                    assetMoviment.NumberDoc = (long.Parse(Sequencia) + 1).ToString();
                                }
                            }
                            else
                                assetMoviment.NumberDoc = (long.Parse(Sequencia) + 1).ToString();
                        }

                        NumberDocGerado = assetMoviment.NumberDoc;
                        // Fim

                        if ((from ram in db.RelationshipAuxiliaryAccountMovementTypes
                             where ram.MovementTypeId == movimentoViewModel.MovementTypeId
                             select ram
                              ).Any())
                        {
                            assetMoviment.AuxiliaryAccountId =
                                (from ram in db.RelationshipAuxiliaryAccountMovementTypes
                                 where ram.MovementTypeId == movimentoViewModel.MovementTypeId
                                 select ram.AuxiliaryAccountId
                                 ).FirstOrDefault();
                        }
                        else
                        {
                            assetMoviment.AuxiliaryAccountId = assetEAssetMovimentBD.AssetMoviment.AuxiliaryAccountId;
                        }


                        switch (movimentoViewModel.MovementTypeId)
                        {

                            #region MOVIMENTACAO INTERNA

                                case (int)EnumMovimentType.MovimentacaoInterna:

                                    assetMoviment.Status = true;
                                    assetMoviment.NumberPurchaseProcess = assetEAssetMovimentBD.AssetMoviment.NumberPurchaseProcess;
                                    assetMoviment.BudgetUnitId = _budgetUnitId ?? movimentoViewModel.BudgetUnitIdDestino; //Caso o perfil não seja a nível de UO, pegar a UO do destino
                                    assetMoviment.ManagerUnitId = _managerUnitId ?? movimentoViewModel.ManagerUnitIdDestino; //Caso o perfil não seja a nível de UGE, pegar a UGE do destino
                                    assetMoviment.AdministrativeUnitId = movimentoViewModel.AdministrativeUnitIdDestino;
                                    if (movimentoViewModel.SectionIdDestino == 0)
                                        assetMoviment.SectionId = null;
                                    else
                                        assetMoviment.SectionId = movimentoViewModel.SectionIdDestino;
                                    assetMoviment.ResponsibleId = movimentoViewModel.ResponsibleId;
                                    assetMoviment.SourceDestiny_ManagerUnitId = null;
                                    assetMoviment.ExchangeId = null;
                                    assetMoviment.ExchangeDate = null;
                                    assetMoviment.ExchangeUserId = null;
                                    assetMoviment.TypeDocumentOutId = null;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    break;

                                #endregion

                            #region INSERVIVEL NA UGE

                                case (int)EnumMovimentType.MovInservivelNaUGE:

                                    //Insere o registro de arrolamento no movimento
                                    assetMoviment.Status = true;
                                    assetMoviment.AdministrativeUnitId = assetEAssetMovimentBD.AssetMoviment.AdministrativeUnitId;
                                    assetMoviment.SectionId = assetEAssetMovimentBD.AssetMoviment.SectionId;
                                    assetMoviment.ResponsibleId = assetEAssetMovimentBD.AssetMoviment.ResponsibleId;
                                    assetMoviment.SourceDestiny_ManagerUnitId = null;
                                    assetMoviment.ExchangeId = assetEAssetMovimentBD.AssetMoviment.ExchangeId;
                                    assetMoviment.ExchangeDate = assetEAssetMovimentBD.AssetMoviment.ExchangeDate;
                                    assetMoviment.ExchangeUserId = assetEAssetMovimentBD.AssetMoviment.ExchangeUserId;
                                    assetMoviment.TypeDocumentOutId = assetEAssetMovimentBD.AssetMoviment.TypeDocumentOutId;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = ContaContabilAntesContaInservivel;

                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    break;

                                #endregion

                            #region DISPONIBILIZADO PARA BOLSA SECRETARIA

                                case (int)EnumMovimentType.DisponibilizadoParaBolsaSecretaria:

                                    //Insere o registro de disponibilização do item para a bolsa da secretária no movimento
                                    assetMoviment.Status = true;
                                    assetMoviment.AdministrativeUnitId = assetEAssetMovimentBD.AssetMoviment.AdministrativeUnitId;
                                    assetMoviment.SectionId = assetEAssetMovimentBD.AssetMoviment.SectionId;
                                    assetMoviment.ResponsibleId = assetEAssetMovimentBD.AssetMoviment.ResponsibleId;
                                    assetMoviment.SourceDestiny_ManagerUnitId = null;
                                    assetMoviment.ExchangeId = assetEAssetMovimentBD.AssetMoviment.ExchangeId;
                                    assetMoviment.ExchangeDate = assetEAssetMovimentBD.AssetMoviment.ExchangeDate;
                                    assetMoviment.ExchangeUserId = assetEAssetMovimentBD.AssetMoviment.ExchangeUserId;
                                    assetMoviment.TypeDocumentOutId = assetEAssetMovimentBD.AssetMoviment.TypeDocumentOutId;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    break;

                                #endregion

                            #region DISPONIBILIZADO PARA BOLSA ESTADUAL

                                case (int)EnumMovimentType.DisponibilizadoParaBolsaEstadual:

                                    //Insere o registro de disponibilização do item para a bolsa do estado no movimento
                                    assetMoviment.Status = true;
                                    assetMoviment.AdministrativeUnitId = assetEAssetMovimentBD.AssetMoviment.AdministrativeUnitId;
                                    assetMoviment.SectionId = assetEAssetMovimentBD.AssetMoviment.SectionId;
                                    assetMoviment.ResponsibleId = assetEAssetMovimentBD.AssetMoviment.ResponsibleId;
                                    assetMoviment.SourceDestiny_ManagerUnitId = null;
                                    assetMoviment.ExchangeId = assetEAssetMovimentBD.AssetMoviment.ExchangeId;
                                    assetMoviment.ExchangeDate = assetEAssetMovimentBD.AssetMoviment.ExchangeDate;
                                    assetMoviment.ExchangeUserId = assetEAssetMovimentBD.AssetMoviment.ExchangeUserId;
                                    assetMoviment.TypeDocumentOutId = assetEAssetMovimentBD.AssetMoviment.TypeDocumentOutId;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    break;

                                #endregion

                            #region RETIRADA DA BOLSA

                                case (int)EnumMovimentType.RetiradaDaBolsa:

                                    //Remove o patrimônio da bolsa
                                    assetMoviment.Status = true;
                                    assetMoviment.AdministrativeUnitId = assetEAssetMovimentBD.AssetMoviment.AdministrativeUnitId;
                                    assetMoviment.SectionId = assetEAssetMovimentBD.AssetMoviment.SectionId;
                                    assetMoviment.ResponsibleId = assetEAssetMovimentBD.AssetMoviment.ResponsibleId;
                                    assetMoviment.SourceDestiny_ManagerUnitId = null;
                                    assetMoviment.ExchangeId = assetEAssetMovimentBD.AssetMoviment.ExchangeId;
                                    assetMoviment.ExchangeDate = assetEAssetMovimentBD.AssetMoviment.ExchangeDate;
                                    assetMoviment.ExchangeUserId = assetEAssetMovimentBD.AssetMoviment.ExchangeUserId;
                                    assetMoviment.TypeDocumentOutId = assetEAssetMovimentBD.AssetMoviment.TypeDocumentOutId;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    break;

                                #endregion

                            #region SAÍDA INSERVIVEL DA UGE - TRANSFERÊNCIA

                                case (int)EnumMovimentType.MovSaidaInservivelUGETransferencia:

                                    assetMoviment.Status = false;
                                    assetMoviment.AdministrativeUnitId = null;
                                    assetMoviment.SectionId = null;
                                    assetMoviment.ResponsibleId = null;
                                    assetMoviment.SourceDestiny_ManagerUnitId = IdUGEFundoSocial();
                                    assetMoviment.ExchangeId = null;
                                    assetMoviment.ExchangeDate = null;
                                    assetMoviment.ExchangeUserId = null;
                                    assetMoviment.TypeDocumentOutId = null;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    var assetTransferidoDB = (from a in db.Assets
                                                              where a.Id == movimentoViewModel.AssetId
                                                              select a).FirstOrDefault();

                                    assetTransferidoDB.Status = false;
                                    db.Entry(assetTransferidoDB).State = EntityState.Modified;
                                    db.SaveChanges();

                                    verificaImplantacaoAceiteAutomatico(assetMoviment, movimentoViewModel);
                                break;

                                #endregion

                            #region SAÍDA INSERVIVEL DA UGE - DOAÇÃO

                                case (int)EnumMovimentType.MovSaidaInservivelUGEDoacao:

                                    assetMoviment.Status = false;
                                    assetMoviment.AdministrativeUnitId = null;
                                    assetMoviment.SectionId = null;
                                    assetMoviment.ResponsibleId = null;
                                    assetMoviment.SourceDestiny_ManagerUnitId = IdUGEFundoSocial();
                                    assetMoviment.ExchangeId = null;
                                    assetMoviment.ExchangeDate = null;
                                    assetMoviment.ExchangeUserId = null;
                                    assetMoviment.TypeDocumentOutId = movimentoViewModel.TypeDocumentOutId;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    var assetDoacaoDB = (from a in db.Assets
                                                         where a.Id == movimentoViewModel.AssetId

                                                         select a).FirstOrDefault();

                                    assetDoacaoDB.Status = false;
                                    db.Entry(assetDoacaoDB).State = EntityState.Modified;
                                    db.SaveChanges();

                                    verificaImplantacaoAceiteAutomatico(assetMoviment, movimentoViewModel);
                                break;

                                #endregion

                            #region COMODATO - CONCEDIDOS - BENS MÓVEIS

                                case (int)EnumMovimentType.MovComodatoConcedidoBensMoveis:

                                    assetMoviment.Status = true;
                                    assetMoviment.AdministrativeUnitId = null;
                                    assetMoviment.SectionId = null;
                                    assetMoviment.ResponsibleId = null;
                                    assetMoviment.SourceDestiny_ManagerUnitId = movimentoViewModel.ManagerUnitIdDestino;
                                    assetMoviment.ExchangeId = null;
                                    assetMoviment.ExchangeDate = null;
                                    assetMoviment.ExchangeUserId = null;
                                    assetMoviment.TypeDocumentOutId = movimentoViewModel.TypeDocumentOutId;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    break;

                                #endregion

                            #region COMODATO/DE TERCEIROS - RECEBIDOS

                                case (int)EnumMovimentType.MovComodatoTerceirosRecebidos:

                                    assetMoviment.Status = false;
                                    assetMoviment.AdministrativeUnitId = null;
                                    assetMoviment.SectionId = null;
                                    assetMoviment.ResponsibleId = null;
                                    assetMoviment.SourceDestiny_ManagerUnitId = movimentoViewModel.ManagerUnitIdDestino;
                                    assetMoviment.ExchangeId = null;
                                    assetMoviment.ExchangeDate = null;
                                    assetMoviment.ExchangeUserId = null;
                                    assetMoviment.TypeDocumentOutId = movimentoViewModel.TypeDocumentOutId;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    var assetComodato = (from a in db.Assets
                                                         where a.Id == movimentoViewModel.AssetId
                                                         select a).FirstOrDefault();

                                    assetComodato.Status = false;
                                    db.Entry(assetComodato).State = EntityState.Modified;
                                    db.SaveChanges();
                                break;

                                #endregion

                            #region DOAÇÃO CONSOLIDAÇÃO

                                case (int)EnumMovimentType.MovDoacaoConsolidacao:

                                    assetMoviment.Status = false;
                                    assetMoviment.AdministrativeUnitId = null;
                                    assetMoviment.SectionId = null;
                                    assetMoviment.ResponsibleId = null;
                                    //assetMoviment.SourceDestiny_ManagerUnitId = movimentoViewModel.ManagerUnitIdDestino;
                                    assetMoviment.ExchangeId = null;
                                    assetMoviment.ExchangeDate = null;
                                    assetMoviment.ExchangeUserId = null;
                                    assetMoviment.TypeDocumentOutId = movimentoViewModel.TypeDocumentOutId;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    var assetDoacaoConsolidacao = (from a in db.Assets
                                                                   where a.Id == movimentoViewModel.AssetId
                                                                   select a).FirstOrDefault();

                                    assetDoacaoConsolidacao.Status = false;
                                    db.Entry(assetDoacaoConsolidacao).State = EntityState.Modified;
                                    db.SaveChanges();

                                    break;
                                #endregion

                            #region DOAÇÃO INTRA - NO ESTADO

                                case (int)EnumMovimentType.MovDoacaoIntraNoEstado:

                                    assetMoviment.Status = true;
                                    assetMoviment.AdministrativeUnitId = null;
                                    assetMoviment.SectionId = null;
                                    assetMoviment.ResponsibleId = null;
                                    assetMoviment.SourceDestiny_ManagerUnitId = movimentoViewModel.ManagerUnitIdDestino;
                                    assetMoviment.ExchangeId = null;
                                    assetMoviment.ExchangeDate = null;
                                    assetMoviment.ExchangeUserId = null;
                                    assetMoviment.TypeDocumentOutId = movimentoViewModel.TypeDocumentOutId;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    verificaImplantacaoAceiteAutomatico(assetMoviment, movimentoViewModel);

                                    break;
                                #endregion

                            #region DOAÇÃO MUNICÍPIO

                                case (int)EnumMovimentType.MovDoacaoMunicipio:

                                    assetMoviment.Status = false;
                                    assetMoviment.AdministrativeUnitId = null;
                                    assetMoviment.SectionId = null;
                                    assetMoviment.ResponsibleId = null;
                                    //assetMoviment.SourceDestiny_ManagerUnitId = movimentoViewModel.ManagerUnitIdDestino;
                                    assetMoviment.ExchangeId = null;
                                    assetMoviment.ExchangeDate = null;
                                    assetMoviment.ExchangeUserId = null;
                                    assetMoviment.TypeDocumentOutId = movimentoViewModel.TypeDocumentOutId;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    var assetDoacaoMunicipio = (from a in db.Assets
                                                                where a.Id == movimentoViewModel.AssetId
                                                                select a).FirstOrDefault();

                                    assetDoacaoMunicipio.Status = false;
                                    db.Entry(assetDoacaoMunicipio).State = EntityState.Modified;
                                    db.SaveChanges();

                                    break;
                                #endregion

                            #region DOAÇÃO OUTROS ESTADOS

                                case (int)EnumMovimentType.MovDoacaoOutrosEstados:

                                    assetMoviment.Status = false;
                                    assetMoviment.AdministrativeUnitId = null;
                                    assetMoviment.SectionId = null;
                                    assetMoviment.ResponsibleId = null;
                                    //assetMoviment.SourceDestiny_ManagerUnitId = movimentoViewModel.ManagerUnitIdDestino;
                                    assetMoviment.ExchangeId = null;
                                    assetMoviment.ExchangeDate = null;
                                    assetMoviment.ExchangeUserId = null;
                                    assetMoviment.TypeDocumentOutId = movimentoViewModel.TypeDocumentOutId;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    var assetDoacaoOutrosEstados = (from a in db.Assets
                                                                    where a.Id == movimentoViewModel.AssetId
                                                                    select a).FirstOrDefault();

                                    assetDoacaoOutrosEstados.Status = false;
                                    db.Entry(assetDoacaoOutrosEstados).State = EntityState.Modified;
                                    db.SaveChanges();

                                    break;
                                #endregion

                            #region DOAÇÃO UNIÃO

                                case (int)EnumMovimentType.MovDoacaoUniao:

                                    assetMoviment.Status = false;
                                    assetMoviment.AdministrativeUnitId = null;
                                    assetMoviment.SectionId = null;
                                    assetMoviment.ResponsibleId = null;
                                    //assetMoviment.SourceDestiny_ManagerUnitId = movimentoViewModel.ManagerUnitIdDestino;
                                    assetMoviment.ExchangeId = null;
                                    assetMoviment.ExchangeDate = null;
                                    assetMoviment.ExchangeUserId = null;
                                    assetMoviment.TypeDocumentOutId = movimentoViewModel.TypeDocumentOutId;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    var assetDoacaoUniao = (from a in db.Assets
                                                            where a.Id == movimentoViewModel.AssetId
                                                            select a).FirstOrDefault();

                                    assetDoacaoUniao.Status = false;
                                    db.Entry(assetDoacaoUniao).State = EntityState.Modified;
                                    db.SaveChanges();

                                    break;
                                #endregion

                            #region VOLTA CONSERTO

                                case (int)EnumMovimentType.VoltaConserto:

                                    assetMoviment.Status = true;
                                    assetMoviment.NumberPurchaseProcess = assetEAssetMovimentBD.AssetMoviment.NumberPurchaseProcess;
                                    assetMoviment.AdministrativeUnitId = assetEAssetMovimentBD.AssetMoviment.AdministrativeUnitId;
                                    assetMoviment.SectionId = assetEAssetMovimentBD.AssetMoviment.SectionId;
                                    assetMoviment.ResponsibleId = assetEAssetMovimentBD.AssetMoviment.ResponsibleId;
                                    assetMoviment.SourceDestiny_ManagerUnitId = null;
                                    assetMoviment.ExchangeId = assetEAssetMovimentBD.AssetMoviment.ExchangeId;
                                    assetMoviment.ExchangeDate = assetEAssetMovimentBD.AssetMoviment.ExchangeDate;
                                    assetMoviment.ExchangeUserId = assetEAssetMovimentBD.AssetMoviment.ExchangeUserId;
                                    assetMoviment.TypeDocumentOutId = assetEAssetMovimentBD.AssetMoviment.TypeDocumentOutId;
                                    assetMoviment.RepairValue = !string.IsNullOrEmpty(movimentoViewModel.RepairValue) ? (decimal?)decimal.Parse(movimentoViewModel.RepairValue) : null;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    break;
                                #endregion

                            #region SAIDA CONSERTO

                                case (int)EnumMovimentType.SaidaConserto:

                                    assetMoviment.Status = true;
                                    assetMoviment.NumberPurchaseProcess = assetEAssetMovimentBD.AssetMoviment.NumberPurchaseProcess;
                                    assetMoviment.AdministrativeUnitId = assetEAssetMovimentBD.AssetMoviment.AdministrativeUnitId;
                                    assetMoviment.SectionId = assetEAssetMovimentBD.AssetMoviment.SectionId;
                                    assetMoviment.ResponsibleId = assetEAssetMovimentBD.AssetMoviment.ResponsibleId;
                                    assetMoviment.SourceDestiny_ManagerUnitId = null;
                                    assetMoviment.ExchangeId = assetEAssetMovimentBD.AssetMoviment.ExchangeId;
                                    assetMoviment.ExchangeDate = assetEAssetMovimentBD.AssetMoviment.ExchangeDate;
                                    assetMoviment.ExchangeUserId = assetEAssetMovimentBD.AssetMoviment.ExchangeUserId;
                                    assetMoviment.TypeDocumentOutId = assetEAssetMovimentBD.AssetMoviment.TypeDocumentOutId;
                                    assetMoviment.RepairValue = !string.IsNullOrEmpty(movimentoViewModel.RepairValue) ? (decimal?)decimal.Parse(movimentoViewModel.RepairValue) : null;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    break;

                                #endregion

                            #region EXTRAVIO, FURTO, ROUBO - BENS MOVEIS

                                case (int)EnumMovimentType.MovExtravioFurtoRouboBensMoveis:

                                    //Inativa o bem
                                    var assetExtraviadoDB = (from a in db.Assets
                                                             where a.Id == movimentoViewModel.AssetId

                                                             select a).FirstOrDefault();

                                    assetExtraviadoDB.Status = false;
                                    db.Entry(assetExtraviadoDB).State = EntityState.Modified;
                                    db.SaveChanges();

                                    //Insere o registro de extravio no movimento
                                    assetMoviment.Status = false;
                                    assetMoviment.AdministrativeUnitId = assetEAssetMovimentBD.AssetMoviment.AdministrativeUnitId;
                                    assetMoviment.SectionId = assetEAssetMovimentBD.AssetMoviment.SectionId;
                                    assetMoviment.ResponsibleId = assetEAssetMovimentBD.AssetMoviment.ResponsibleId;
                                    assetMoviment.SourceDestiny_ManagerUnitId = null;
                                    assetMoviment.ExchangeId = assetEAssetMovimentBD.AssetMoviment.ExchangeId;
                                    assetMoviment.ExchangeDate = assetEAssetMovimentBD.AssetMoviment.ExchangeDate;
                                    assetMoviment.ExchangeUserId = assetEAssetMovimentBD.AssetMoviment.ExchangeUserId;
                                    assetMoviment.TypeDocumentOutId = assetEAssetMovimentBD.AssetMoviment.TypeDocumentOutId;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    break;

                                #endregion

                            #region PERDA INVOLUNTÁRIA - BENS MOVEIS

                                case (int)EnumMovimentType.MovPerdaInvoluntariaBensMoveis:

                                    //Inativa o bem
                                    var assetObsoletoDB = (from a in db.Assets
                                                           where a.Id == movimentoViewModel.AssetId

                                                           select a).FirstOrDefault();

                                    assetObsoletoDB.Status = false;
                                    db.Entry(assetObsoletoDB).State = EntityState.Modified;
                                    db.SaveChanges();


                                    //Insere o registro de obsoleto no movimento
                                    assetMoviment.Status = false;
                                    assetMoviment.AdministrativeUnitId = assetEAssetMovimentBD.AssetMoviment.AdministrativeUnitId;
                                    assetMoviment.SectionId = assetEAssetMovimentBD.AssetMoviment.SectionId;
                                    assetMoviment.ResponsibleId = assetEAssetMovimentBD.AssetMoviment.ResponsibleId;
                                    assetMoviment.SourceDestiny_ManagerUnitId = null;
                                    assetMoviment.ExchangeId = assetEAssetMovimentBD.AssetMoviment.ExchangeId;
                                    assetMoviment.ExchangeDate = assetEAssetMovimentBD.AssetMoviment.ExchangeDate;
                                    assetMoviment.ExchangeUserId = assetEAssetMovimentBD.AssetMoviment.ExchangeUserId;
                                    assetMoviment.TypeDocumentOutId = assetEAssetMovimentBD.AssetMoviment.TypeDocumentOutId;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    break;

                                #endregion

                            #region PERDA INVOLUNTÁRIA INSERVÍVEL - BENS MOVEIS

                                case (int)EnumMovimentType.MovPerdaInvoluntariaInservivelBensMoveis:

                                    //Inativa o bem
                                    var assetDanificadoDB = (from a in db.Assets
                                                             where a.Id == movimentoViewModel.AssetId

                                                             select a).FirstOrDefault();

                                    assetDanificadoDB.Status = false;
                                    db.Entry(assetDanificadoDB).State = EntityState.Modified;
                                    db.SaveChanges();


                                    //Insere o registro de Danificado no movimento
                                    assetMoviment.Status = false;
                                    assetMoviment.AdministrativeUnitId = assetEAssetMovimentBD.AssetMoviment.AdministrativeUnitId;
                                    assetMoviment.SectionId = assetEAssetMovimentBD.AssetMoviment.SectionId;
                                    assetMoviment.ResponsibleId = assetEAssetMovimentBD.AssetMoviment.ResponsibleId;
                                    assetMoviment.SourceDestiny_ManagerUnitId = null;
                                    assetMoviment.ExchangeId = assetEAssetMovimentBD.AssetMoviment.ExchangeId;
                                    assetMoviment.ExchangeDate = assetEAssetMovimentBD.AssetMoviment.ExchangeDate;
                                    assetMoviment.ExchangeUserId = assetEAssetMovimentBD.AssetMoviment.ExchangeUserId;
                                    assetMoviment.TypeDocumentOutId = assetEAssetMovimentBD.AssetMoviment.TypeDocumentOutId;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    break;

                                #endregion

                            #region MORTE ANIMAL - PATRIMONIADO

                                case (int)EnumMovimentType.MovMorteAnimalPatrimoniado:

                                    //Inativa o bem
                                    var assetMorteAnimal = (from a in db.Assets
                                                            where a.Id == movimentoViewModel.AssetId

                                                            select a).FirstOrDefault();

                                    assetMorteAnimal.Status = false;
                                    db.Entry(assetMorteAnimal).State = EntityState.Modified;
                                    db.SaveChanges();

                                    //Insere o registro de obsoleto no sucata
                                    assetMoviment.Status = false;
                                    assetMoviment.AdministrativeUnitId = assetEAssetMovimentBD.AssetMoviment.AdministrativeUnitId;
                                    assetMoviment.SectionId = assetEAssetMovimentBD.AssetMoviment.SectionId;
                                    assetMoviment.ResponsibleId = assetEAssetMovimentBD.AssetMoviment.ResponsibleId;
                                    assetMoviment.SourceDestiny_ManagerUnitId = null;
                                    assetMoviment.ExchangeId = assetEAssetMovimentBD.AssetMoviment.ExchangeId;
                                    assetMoviment.ExchangeDate = assetEAssetMovimentBD.AssetMoviment.ExchangeDate;
                                    assetMoviment.ExchangeUserId = assetEAssetMovimentBD.AssetMoviment.ExchangeUserId;
                                    assetMoviment.TypeDocumentOutId = assetEAssetMovimentBD.AssetMoviment.TypeDocumentOutId;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    break;

                                #endregion

                            #region MUDANÇA DE CATEGORIA / DESVALORIZAÇÃO

                                case (int)EnumMovimentType.MovMudancaCategoriaDesvalorizacao:

                                    assetMoviment.Status = true;
                                    assetMoviment.AdministrativeUnitId = null;
                                    assetMoviment.SectionId = null;
                                    assetMoviment.ResponsibleId = null;
                                    assetMoviment.SourceDestiny_ManagerUnitId = movimentoViewModel.ManagerUnitIdDestino;
                                    assetMoviment.ExchangeId = null;
                                    assetMoviment.ExchangeDate = null;
                                    assetMoviment.ExchangeUserId = null;
                                    assetMoviment.TypeDocumentOutId = movimentoViewModel.TypeDocumentOutId;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    break;
                                #endregion

                            #region SEMENTES, PLANTAS, INSUMOS E ARVORES

                                case (int)EnumMovimentType.MovSementesPlantasInsumosArvores:

                                    var assetMovSementesDB = (from a in db.Assets
                                                              where a.Id == movimentoViewModel.AssetId
                                                              select a).FirstOrDefault();

                                    assetMovSementesDB.Status = false;
                                    db.Entry(assetMovSementesDB).State = EntityState.Modified;
                                    db.SaveChanges();

                                    assetMoviment.Status = false;
                                    assetMoviment.AdministrativeUnitId = assetEAssetMovimentBD.AssetMoviment.AdministrativeUnitId;
                                    assetMoviment.SectionId = assetEAssetMovimentBD.AssetMoviment.SectionId;
                                    assetMoviment.ResponsibleId = assetEAssetMovimentBD.AssetMoviment.ResponsibleId;
                                    assetMoviment.SourceDestiny_ManagerUnitId = null;
                                    assetMoviment.ExchangeId = assetEAssetMovimentBD.AssetMoviment.ExchangeId;
                                    assetMoviment.ExchangeDate = assetEAssetMovimentBD.AssetMoviment.ExchangeDate;
                                    assetMoviment.ExchangeUserId = assetEAssetMovimentBD.AssetMoviment.ExchangeUserId;
                                    assetMoviment.TypeDocumentOutId = assetEAssetMovimentBD.AssetMoviment.TypeDocumentOutId;
                                    assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                    db.Entry(assetMoviment).State = EntityState.Added;
                                    db.SaveChanges();

                                    break;
                                #endregion

                            #region TRANSFERÊNCIA OUTRO ORGÃO - PATRIMÔNIADO

                                case (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado:

                                    bool? orgaoDestinoTransferenciaImplantadoOutroOrgao = (from m in db.Institutions
                                                                                           where m.Id == movimentoViewModel.InstituationIdDestino
                                                                                           select m.flagImplantado).FirstOrDefault();

                                    if (orgaoDestinoTransferenciaImplantadoOutroOrgao == true)
                                    {
                                        assetMoviment.Status = true;
                                        assetMoviment.AdministrativeUnitId = null;
                                        assetMoviment.SectionId = null;
                                        assetMoviment.ResponsibleId = null;
                                        assetMoviment.SourceDestiny_ManagerUnitId = movimentoViewModel.ManagerUnitIdDestino;
                                        assetMoviment.ExchangeId = null;
                                        assetMoviment.ExchangeDate = null;
                                        assetMoviment.ExchangeUserId = null;
                                        assetMoviment.TypeDocumentOutId = null;
                                        assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                        db.Entry(assetMoviment).State = EntityState.Added;
                                        db.SaveChanges();
                                    }
                                    else
                                    {
                                        assetMoviment.Status = false;
                                        assetMoviment.AdministrativeUnitId = null;
                                        assetMoviment.SectionId = null;
                                        assetMoviment.ResponsibleId = null;
                                        assetMoviment.SourceDestiny_ManagerUnitId = movimentoViewModel.ManagerUnitIdDestino;
                                        assetMoviment.ExchangeId = null;
                                        assetMoviment.ExchangeDate = null;
                                        assetMoviment.ExchangeUserId = null;
                                        assetMoviment.TypeDocumentOutId = null;
                                        assetMoviment.flagUGENaoUtilizada = true;
                                        assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                        db.Entry(assetMoviment).State = EntityState.Added;
                                        db.SaveChanges();

                                        var assetTransferidoDB3 = (from a in db.Assets
                                                                   where a.Id == movimentoViewModel.AssetId

                                                                   select a).FirstOrDefault();

                                        assetTransferidoDB3.Status = false;
                                        db.Entry(assetTransferidoDB3).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }

                                    verificaImplantacaoAceiteAutomatico(assetMoviment, movimentoViewModel);

                                    break;

                                #endregion

                            #region TRANSFERÊNCIA MESMO ORGÃO - PATRIMÔNIADO

                                case (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado:

                                    bool? orgaoDestinoTransferenciaImplantadoMesmoOrgao = (from m in db.Institutions
                                                                                           where m.Id == movimentoViewModel.InstituationIdDestino
                                                                                           select m.flagImplantado).FirstOrDefault();

                                    if (orgaoDestinoTransferenciaImplantadoMesmoOrgao == true)
                                    {
                                        assetMoviment.Status = true;
                                        assetMoviment.AdministrativeUnitId = null;
                                        assetMoviment.SectionId = null;
                                        assetMoviment.ResponsibleId = null;
                                        assetMoviment.SourceDestiny_ManagerUnitId = movimentoViewModel.ManagerUnitIdDestino;
                                        assetMoviment.ExchangeId = null;
                                        assetMoviment.ExchangeDate = null;
                                        assetMoviment.ExchangeUserId = null;
                                        assetMoviment.TypeDocumentOutId = null;
                                        assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                        db.Entry(assetMoviment).State = EntityState.Added;
                                        db.SaveChanges();
                                    }
                                    else
                                    {
                                        assetMoviment.Status = false;
                                        assetMoviment.AdministrativeUnitId = null;
                                        assetMoviment.SectionId = null;
                                        assetMoviment.ResponsibleId = null;
                                        assetMoviment.SourceDestiny_ManagerUnitId = movimentoViewModel.ManagerUnitIdDestino;
                                        assetMoviment.ExchangeId = null;
                                        assetMoviment.ExchangeDate = null;
                                        assetMoviment.ExchangeUserId = null;
                                        assetMoviment.TypeDocumentOutId = null;
                                        assetMoviment.flagUGENaoUtilizada = true;
                                        assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                        db.Entry(assetMoviment).State = EntityState.Added;
                                        db.SaveChanges();

                                        var assetTransferidoDB2 = (from a in db.Assets
                                                                   where a.Id == movimentoViewModel.AssetId

                                                                   select a).FirstOrDefault();

                                        assetTransferidoDB2.Status = false;
                                        db.Entry(assetTransferidoDB2).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }

                                    verificaImplantacaoUGEAceiteAutomatico(assetMoviment, movimentoViewModel);

                                    break;

                            #endregion

                            #region VENDA/LEILÃO - SEMOVENTE

                            case (int)EnumMovimentType.MovVendaLeilaoSemoventes:

                                //Inativa o bem
                                var assetAnimalLeiloado = (from a in db.Assets
                                                        where a.Id == movimentoViewModel.AssetId

                                                        select a).FirstOrDefault();

                                assetAnimalLeiloado.Status = false;
                                db.Entry(assetAnimalLeiloado).State = EntityState.Modified;
                                db.SaveChanges();

                                //Insere o registro de obsoleto no sucata
                                assetMoviment.Status = false;
                                assetMoviment.AdministrativeUnitId = assetEAssetMovimentBD.AssetMoviment.AdministrativeUnitId;
                                assetMoviment.SectionId = assetEAssetMovimentBD.AssetMoviment.SectionId;
                                assetMoviment.ResponsibleId = assetEAssetMovimentBD.AssetMoviment.ResponsibleId;
                                assetMoviment.SourceDestiny_ManagerUnitId = null;
                                assetMoviment.ExchangeId = assetEAssetMovimentBD.AssetMoviment.ExchangeId;
                                assetMoviment.ExchangeDate = assetEAssetMovimentBD.AssetMoviment.ExchangeDate;
                                assetMoviment.ExchangeUserId = assetEAssetMovimentBD.AssetMoviment.ExchangeUserId;
                                assetMoviment.TypeDocumentOutId = assetEAssetMovimentBD.AssetMoviment.TypeDocumentOutId;
                                assetMoviment.ContaContabilAntesDeVirarInservivel = assetEAssetMovimentBD.AssetMoviment.ContaContabilAntesDeVirarInservivel;

                                db.Entry(assetMoviment).State = EntityState.Added;
                                db.SaveChanges();

                                break;

                                #endregion
                        }

                        listaMovimentacoesPatrimoniaisEmLote.Add(assetMoviment);

                        //Altera o Estado do Item Inventário para Inventariado Manualmente
                        var itemInventario = (from i in db.ItemInventarios where i.Id == movimentoViewModel.ItemInventarioId select i).AsNoTracking().FirstOrDefault();
                        InventarioIdDoItem = itemInventario.InventarioId;
                        itemInventario.Estado = (int)EnumSituacaoFisicaBP.Inventariado_Manualmente;
                        db.Entry(itemInventario).State = EntityState.Modified;
                        db.SaveChanges();

                        ComputaAlteracaoContabil(assetMoviment.InstitutionId, assetMoviment.BudgetUnitId,
                                                 assetMoviment.ManagerUnitId, (int)assetMoviment.AuxiliaryAccountId);

                        if (assetMoviment.MovementTypeId == (int)EnumMovimentType.MovInservivelNaUGE) {
                            ComputaAlteracaoContabil(assetMoviment.InstitutionId, assetMoviment.BudgetUnitId,
                                                 assetMoviment.ManagerUnitId, (int)assetMoviment.ContaContabilAntesDeVirarInservivel);
                        }

                        transaction.Complete();
                    }

                    var tipoMovPatrimonial = (from m in db.MovementTypes
                                              where m.Id == movimentoViewModel.MovementTypeId
                                              select m).FirstOrDefault();

                    base.initDadosSIAFEM();

                    var dataInicioIntegracao = new DateTime(2020, 06, 01);
                    bool MovimentacaoIntegradoAoSIAFEM = tipoMovPatrimonial.PossuiContraPartidaContabil() && base.ugeIntegradaSiafem && Convert.ToDateTime(movimentoViewModel.MovimentoDate) >= dataInicioIntegracao;

                    if (MovimentacaoIntegradoAoSIAFEM)
                    {
                        var svcIntegracaoSIAFEM = new IntegracaoContabilizaSPController();
                        var tuplas = svcIntegracaoSIAFEM.GeraMensagensExtratoPreProcessamentoSIAFEM(listaMovimentacoesPatrimoniaisEmLote, true);

                        movimentoViewModel.MsgSIAFEM = new List<string>();
                        movimentoViewModel.ListaIdAuditoria = string.Empty;

                        foreach (var item in tuplas)
                        {
                            movimentoViewModel.MsgSIAFEM.Add(item.Item1);
                            movimentoViewModel.ListaIdAuditoria += item.Item3.ToString() + ",";
                        }

                        movimentoViewModel.ListaIdAuditoria += "0";

                        CarregaHierarquiaFiltro(_institutionId, _budgetUnitId ?? 0, _managerUnitId ?? 0);
                        CarregaComboTipoMovimento();

                        CarregaViewBagsPorTipoMovimento(movimentoViewModel);

                        return View(movimentoViewModel);
                    }
                    else
                    {
                        var loginUsuarioSIAFEM = movimentoViewModel.LoginSiafem;
                        var senhaUsuarioSIAFEM = movimentoViewModel.SenhaSiafem;
                        string msgInformeAoUsuario = null;
                        msgInformeAoUsuario = base.processamentoMovimentacaoPatrimonialNoContabilizaSP(movimentoViewModel.MovementTypeId, loginUsuarioSIAFEM, senhaUsuarioSIAFEM, NumberDocGerado, listaMovimentacoesPatrimoniaisEmLote);
                        msgInformeAoUsuario = msgInformeAoUsuario.ToJavaScriptString();
                        TempData["msgSucesso"] = msgInformeAoUsuario;
                        TempData["numeroDocumento"] = NumberDocGerado;
                        TempData.Keep();

                        return RedirectToAction("Index", new { InventarioId = InventarioIdDoItem });
                    }
                }

                //Recarrega os combos da tela                        
                CarregaHierarquiaFiltro(_institutionId, _budgetUnitId ?? 0, _managerUnitId ?? 0);
                CarregaComboTipoMovimento();

                CarregaViewBagsPorTipoMovimento(movimentoViewModel);

                var numeroInventario = (from i in db.ItemInventarios where i.Id == movimentoViewModel.ItemInventarioId select i.InventarioId).FirstOrDefault();
                ViewBag.InventarioId = numeroInventario;

                return View(movimentoViewModel);
            }
            catch (Exception e) {
                return MensagemErro(CommonMensagens.PadraoException, e);
            }
        }

        #region CarregaCombos
        private void CarregaViewBagsPorTipoMovimento(MovimentoViewModel movimentoViewModel)
        {
            //Recarrega todos os combos de acordo com o tipo de movimento
            switch (movimentoViewModel.MovementTypeId)
            {
                case (int)EnumMovimentType.MovimentacaoInterna:
                    CarregaHierarquiaDestinoComUAEDivisao(movimentoViewModel.InstituationIdDestino, movimentoViewModel.BudgetUnitIdDestino, movimentoViewModel.ManagerUnitIdDestino,
                                                          movimentoViewModel.AdministrativeUnitIdDestino, movimentoViewModel.SectionIdDestino);
                    CarregaComboResponsavel(movimentoViewModel.AdministrativeUnitIdDestino, movimentoViewModel.ResponsibleId);
                    break;

                case (int)EnumMovimentType.VoltaConserto:
                case (int)EnumMovimentType.SaidaConserto:
                case (int)EnumMovimentType.MovInservivelNaUGE:
                case (int)EnumMovimentType.DisponibilizadoParaBolsaSecretaria:
                case (int)EnumMovimentType.DisponibilizadoParaBolsaEstadual:
                case (int)EnumMovimentType.RetiradaDaBolsa:
                case (int)EnumMovimentType.MovComodatoTerceirosRecebidos:
                case (int)EnumMovimentType.MovExtravioFurtoRouboBensMoveis:
                case (int)EnumMovimentType.MovMorteAnimalPatrimoniado:
                case (int)EnumMovimentType.MovMudancaCategoriaDesvalorizacao:
                case (int)EnumMovimentType.MovSementesPlantasInsumosArvores:
                case (int)EnumMovimentType.MovPerdaInvoluntariaBensMoveis:
                case (int)EnumMovimentType.MovPerdaInvoluntariaInservivelBensMoveis:
                case (int)EnumMovimentType.MovVendaLeilaoSemoventes:
                    CarregaHierarquiaDestino(_institutionId, _budgetUnitId ?? 0, _managerUnitId ?? 0);
                    break;
                case (int)EnumMovimentType.MovSaidaInservivelUGEDoacao:
                        if (!MesmaGestaoDoFundoSocial(_institutionId))
                            CarregaFundoSocial();
                        else
                            movimentoViewModel.Proibido = true;
                    break;
                case (int)EnumMovimentType.MovSaidaInservivelUGETransferencia:
                        if (MesmaGestaoDoFundoSocial(_institutionId))
                            CarregaFundoSocial();
                        else
                            movimentoViewModel.Proibido = true;
                    break;
                case (int)EnumMovimentType.MovComodatoConcedidoBensMoveis:
                    CarregaHierarquiaDestinoMesmaGestao(movimentoViewModel.InstituationIdDestino, movimentoViewModel.BudgetUnitIdDestino, movimentoViewModel.ManagerUnitIdDestino);
                    break;
                case (int)SAM.Web.Common.Enum.EnumMovimentType.MovDoacaoIntraNoEstado:
                    CarregaHierarquiaDestinoOutrasGestoes(movimentoViewModel.InstituationIdDestino, movimentoViewModel.BudgetUnitIdDestino, movimentoViewModel.ManagerUnitIdDestino);
                    CarregaComboResponsavel(movimentoViewModel.AdministrativeUnitIdDestino, movimentoViewModel.ResponsibleId);
                    break;
                case (int)SAM.Web.Common.Enum.EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado:
                    CarregaHierarquiaDestinoMesmaGestao(movimentoViewModel.InstituationIdDestino, movimentoViewModel.BudgetUnitIdDestino, movimentoViewModel.ManagerUnitIdDestino, false);
                    CarregaComboResponsavel(movimentoViewModel.AdministrativeUnitIdDestino, movimentoViewModel.ResponsibleId);
                    break;
                case (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado:
                    CarregaHierarquiaDestinoMesmoOrgao(movimentoViewModel.InstituationIdDestino, movimentoViewModel.BudgetUnitIdDestino, movimentoViewModel.ManagerUnitIdDestino);
                    break;
            }
        }

        private void CarregaHierarquiaFiltro(int modelInstitutionId = 0, int modelBudgetUnitId = 0, int modelManagerUnitId = 0, int modelAdministrativeUnitId = 0, int modelSectionId = 0, bool mesmaGestao = false, string managerCode = null)
        {
            hierarquia = new Hierarquia();
            if (managerCode == null)
            {
                ViewBag.Institutions = new SelectList(hierarquia.GetOrgaos(modelInstitutionId), "Id", "Description", modelInstitutionId);
            }
            else
            {
                if (mesmaGestao)
                    ViewBag.Institutions = new SelectList(hierarquia.GetOrgaosMesmaGestao(managerCode, modelInstitutionId), "Id", "Description", modelInstitutionId);
                else
                    ViewBag.Institutions = new SelectList(hierarquia.GetOrgaosGestaoDiferente(managerCode, modelInstitutionId), "Id", "Description", modelInstitutionId);
            }


            if (modelBudgetUnitId != 0)
                ViewBag.BudgetUnits = new SelectList(hierarquia.GetUos(modelBudgetUnitId), "Id", "Description", modelBudgetUnitId);
            else
                ViewBag.BudgetUnits = new SelectList(hierarquia.GetUosPorOrgaoId(modelInstitutionId), "Id", "Description", modelBudgetUnitId);

            if (modelManagerUnitId != 0)
                ViewBag.ManagerUnits = new SelectList(hierarquia.GetUges(modelManagerUnitId), "Id", "Description", modelManagerUnitId);
            else
                ViewBag.ManagerUnits = new SelectList(hierarquia.GetUgesPorUoId(modelBudgetUnitId), "Id", "Description", modelManagerUnitId);

            if (modelAdministrativeUnitId != 0)
                ViewBag.AdministrativeUnits = new SelectList(hierarquia.GetUas(modelAdministrativeUnitId), "Id", "Description", modelAdministrativeUnitId);
            else
                ViewBag.AdministrativeUnits = new SelectList(hierarquia.GetUasPorUgeId(modelManagerUnitId), "Id", "Description", modelAdministrativeUnitId);

            if (modelSectionId != 0)
                ViewBag.Sections = new SelectList(hierarquia.GetDivisoes(modelSectionId), "Id", "Description", modelSectionId);
            else
                ViewBag.Sections = new SelectList(hierarquia.GetDivisoesPorUaId(modelAdministrativeUnitId), "Id", "Description", modelSectionId);

            bool habilitaIntegracaoContabilizaSP = false;

            var perflLogado = BuscaHierarquiaPerfilLogado((int)HttpContext.Items["RupId"]);
            if (perflLogado.ManagerUnitId != null && perflLogado.ManagerUnitId != 0) {
                this.initDadosSIAFEM();
                var uge = db.ManagerUnits.Find(perflLogado.ManagerUnitId);

                if (uge != null) {
                    int mesReferencia = int.Parse(uge.ManagmentUnit_YearMonthReference);

                    habilitaIntegracaoContabilizaSP = (base.ugeIntegradaSiafem && (mesReferencia >= uge.MesRefInicioIntegracaoSIAFEM));
                }
            }
            
            
            ViewData["flagIntegracaoSiafem"] = (habilitaIntegracaoContabilizaSP ? 1 : 0);
        }

        private void CarregaHierarquiaDestino(int modelInstitutionId = 0, int modelBudgetUnitId = 0, int modelManagerUnitId = 0, int modelAdministrativeUnitId = 0, int modelSectionId = 0, bool mesmaGestao = false, string managerCode = null, bool? mesmoOrgao = null)
        {
            hierarquia = new Hierarquia();
            if (mesmoOrgao != null)
            {
                if (mesmoOrgao == true)
                    ViewBag.InstitutionsDestino = new SelectList(hierarquia.GetOrgaos(modelInstitutionId), "Id", "Description", modelInstitutionId);
                else
                {
                    List<Institution> lstInstitution;

                    if (managerCode == null)
                    {
                        lstInstitution = hierarquia.GetOrgaos(null);
                        ViewBag.InstitutionsDestino = new SelectList(lstInstitution.Where(i => i.Id != modelInstitutionId), "Id", "Description");
                    }
                    else
                    {
                        if (mesmaGestao)
                        {
                            lstInstitution = hierarquia.GetOrgaosMesmaGestao(managerCode, null);
                            ViewBag.InstitutionsDestino = new SelectList(lstInstitution.Where(i => i.Id != modelInstitutionId), "Id", "Description", modelInstitutionId);
                        }
                        else
                        {
                            lstInstitution = hierarquia.GetOrgaosGestaoDiferente(managerCode, null);
                            ViewBag.InstitutionsDestino = new SelectList(lstInstitution.Where(i => i.Id != modelInstitutionId), "Id", "Description", modelInstitutionId);
                        }
                    }
                }

            }
            else if (managerCode == null)
            {
                ViewBag.InstitutionsDestino = new SelectList(hierarquia.GetOrgaos(modelInstitutionId), "Id", "Description", modelInstitutionId);
            }
            else
            {
                if (mesmaGestao)
                    ViewBag.InstitutionsDestino = new SelectList(hierarquia.GetOrgaosMesmaGestao(managerCode, modelInstitutionId), "Id", "Description", modelInstitutionId);
                else
                    ViewBag.InstitutionsDestino = new SelectList(hierarquia.GetOrgaosGestaoDiferente(managerCode, modelInstitutionId), "Id", "Description", modelInstitutionId);
            }


            if (modelBudgetUnitId != 0)
                ViewBag.BudgetUnitsDestino = new SelectList(hierarquia.GetUos(modelBudgetUnitId), "Id", "Description", modelBudgetUnitId);
            else
                ViewBag.BudgetUnitsDestino = new SelectList(hierarquia.GetUosPorOrgaoId(modelInstitutionId), "Id", "Description", modelBudgetUnitId);

            if (modelManagerUnitId != 0)
                ViewBag.ManagerUnitsDestino = new SelectList(hierarquia.GetUges(modelManagerUnitId), "Id", "Description", modelManagerUnitId);
            else
                ViewBag.ManagerUnitsDestino = new SelectList(hierarquia.GetUgesPorUoId(modelBudgetUnitId), "Id", "Description", modelManagerUnitId);

            if (modelAdministrativeUnitId != 0)
                ViewBag.AdministrativeUnitsDestino = new SelectList(hierarquia.GetUas(modelAdministrativeUnitId), "Id", "Description", modelAdministrativeUnitId);
            else
                ViewBag.AdministrativeUnitsDestino = new SelectList(hierarquia.GetUasPorUgeId(modelManagerUnitId), "Id", "Description", modelAdministrativeUnitId);

            if (modelSectionId != 0)
                ViewBag.SectionsDestino = new SelectList(hierarquia.GetDivisoes(modelSectionId), "Id", "Description", modelSectionId);
            else
                ViewBag.SectionsDestino = new SelectList(hierarquia.GetDivisoesPorUaId(modelAdministrativeUnitId), "Id", "Description", modelSectionId);

        }

        private void CarregaHierarquiaDestinoComUAEDivisao(int modelInstitutionId, int modelBudgetUnitId = 0, int modelManagerUnitId = 0, int modelAdministrativeUnitId = 0, int modelSectionId = 0)
        {
            hierarquia = new Hierarquia();
            if (modelInstitutionId == 0)
                ViewBag.InstitutionsDestino = new SelectList(hierarquia.GetOrgaos(_institutionId), "Id", "Description", _institutionId);
            else
                ViewBag.InstitutionsDestino = new SelectList(hierarquia.GetOrgaos(modelInstitutionId), "Id", "Description", modelInstitutionId);

            if (modelBudgetUnitId != 0)
                ViewBag.BudgetUnitsDestino = new SelectList(hierarquia.GetUos(modelBudgetUnitId), "Id", "Description", modelBudgetUnitId);
            else
                ViewBag.BudgetUnitsDestino = new SelectList(hierarquia.GetUosPorOrgaoId(modelInstitutionId), "Id", "Description", modelBudgetUnitId);

            if ((int)HttpContext.Items["perfilId"] == (int)EnumProfile.OperadordeUGE)
                ViewBag.ManagerUnitsDestino = new SelectList(hierarquia.GetUges(modelManagerUnitId), "Id", "Description", modelManagerUnitId);
            else
                ViewBag.ManagerUnitsDestino = new SelectList(hierarquia.GetUgesPorUoId(modelBudgetUnitId), "Id", "Description", modelManagerUnitId);

            ViewBag.AdministrativeUnitsDestino = new SelectList(hierarquia.GetUasPorUgeId(modelManagerUnitId), "Id", "Description", modelAdministrativeUnitId);

            ViewBag.SectionsDestino = new SelectList(hierarquia.GetDivisoesPorUaId(modelAdministrativeUnitId), "Id", "Description", modelSectionId);

        }

        private void CarregaHierarquiaDestinoOutrasGestoes(int modelInstitutionId = 0, int modelBudgetUnitId = 0, int modelManagerUnitId = 0, int modelAdministrativeUnitId = 0, int modelSectionId = 0)
        {
            if (hierarquia == null)
                hierarquia = new Hierarquia();

            ViewBag.InstitutionsDestino = new SelectList(hierarquia.GetOrgaosGestaoDiferente(RecuperaGestao(), null), "Id", "Description", modelInstitutionId);
            ViewBag.BudgetUnitsDestino = new SelectList(hierarquia.GetUosPorOrgaoId(modelInstitutionId), "Id", "Description", modelBudgetUnitId);
            ViewBag.ManagerUnitsDestino = new SelectList(hierarquia.GetUgesPorUoId(modelBudgetUnitId), "Id", "Description", modelManagerUnitId);
        }

        private void CarregaHierarquiaDestinoMesmaGestao(int modelInstitutionId = 0, int modelBudgetUnitId = 0, int modelManagerUnitId = 0, bool podeMesmaGestao = true)
        {
            if (hierarquia == null)
                hierarquia = new Hierarquia();

            if (podeMesmaGestao)
            {
                ViewBag.InstitutionsDestino = new SelectList(hierarquia.GetOrgaosMesmaGestao(RecuperaGestao(), null), "Id", "Description", modelInstitutionId);
            }
            else
            {
                List<Institution> listaOrgaos = hierarquia.GetOrgaosMesmaGestao(RecuperaGestao(), null);
                ViewBag.InstitutionsDestino = new SelectList(listaOrgaos.Where(i => i.Id != _institutionId), "Id", "Description", modelInstitutionId);
            }

            ViewBag.BudgetUnitsDestino = new SelectList(hierarquia.GetUosPorOrgaoId(modelInstitutionId), "Id", "Description", modelBudgetUnitId);
            ViewBag.ManagerUnitsDestino = new SelectList(hierarquia.GetUgesPorUoId(modelBudgetUnitId), "Id", "Description", modelManagerUnitId);
        }

        private void CarregaHierarquiaDestinoMesmoOrgao(int modelInstitutionId = 0, int modelBudgetUnitId = 0, int modelManagerUnitId = 0)
        {
            if (hierarquia == null)
                hierarquia = new Hierarquia();

            ViewBag.InstitutionsDestino = new SelectList(hierarquia.GetOrgaos(_institutionId), "Id", "Description", modelInstitutionId);
            ViewBag.BudgetUnitsDestino = new SelectList(hierarquia.GetUosPorOrgaoId(_institutionId), "Id", "Description", modelBudgetUnitId);
            ViewBag.ManagerUnitsDestino = new SelectList(hierarquia.GetUgesPorUoId(modelBudgetUnitId), "Id", "Description", modelManagerUnitId);
        }

        private string RecuperaGestao()
        {
                return (from i in db.Institutions
                        where i.Id == _institutionId
                        select i.ManagerCode).FirstOrDefault();
        }

        private void CarregaComboTipoMovimento()
        {
            var MovimentoTypes = (from mt in db.MovementTypes
                                  where mt.Status == true &&
                                        mt.GroupMovimentId == (int)EnumGroupMoviment.Movimentacao
                                  select mt).OrderBy(t => t.Description).ToList();

            ViewBag.MovimentType = new SelectList(MovimentoTypes, "Id", "Description");
        }

        private void CarregaComboResponsavel(int administrativeUnitId = 0, int userId = 0)
        {
            var vResponsible = new List<Responsible>();

            if (administrativeUnitId != 0)
                vResponsible = (from r in db.Responsibles where r.AdministrativeUnitId == administrativeUnitId select r).ToList();

            if (userId != 0)
                ViewBag.User = new SelectList(vResponsible.ToList(), "Id", "Name", userId);
            else
                ViewBag.User = new SelectList(vResponsible.ToList(), "Id", "Name");
        }

        private void CarregaHierarquiaDestinoUAeSection(int modelInstitutionId = 0, int modelBudgetUnitId = 0, int modelManagerUnitId = 0, int modelAdministrativeUnitId = 0, int modelSectionId = 0, bool mesmaGestao = false, string managerCode = null)
        {
            hierarquia = new Hierarquia();
            if (managerCode == null)
            {
                ViewBag.InstitutionsDestino = new SelectList(hierarquia.GetOrgaos(null), "Id", "Description", modelInstitutionId);
            }
            else
            {
                if (mesmaGestao)
                    ViewBag.InstitutionsDestino = new SelectList(hierarquia.GetOrgaosMesmaGestao(managerCode, null), "Id", "Description", modelInstitutionId);
                else
                    ViewBag.InstitutionsDestino = new SelectList(hierarquia.GetOrgaosGestaoDiferente(managerCode, null), "Id", "Description", modelInstitutionId);
            }

            ViewBag.BudgetUnitsDestino = new SelectList(hierarquia.GetUosPorOrgaoId(modelInstitutionId), "Id", "Description", modelBudgetUnitId);

            ViewBag.ManagerUnitsDestino = new SelectList(hierarquia.GetUgesPorUoId(modelBudgetUnitId), "Id", "Description", modelManagerUnitId);

            ViewBag.AdministrativeUnitsDestino = new SelectList(hierarquia.GetUasPorUgeId(modelManagerUnitId), "Id", "Description", modelAdministrativeUnitId);

            ViewBag.SectionsDestino = new SelectList(hierarquia.GetDivisoesPorUaId(modelAdministrativeUnitId), "Id", "Description", modelSectionId);
        }
        #endregion

        private bool MesmaGestaoDoFundoSocial(int institutionId)
        {
            string gestao = (from i in db.Institutions
                             where i.Id == institutionId
                             select i.ManagerCode).FirstOrDefault();

            string gestaoFundoSocial = (from i in db.Institutions
                                        where i.Code == "51004"
                                        select i.ManagerCode).FirstOrDefault();

            return gestao == gestaoFundoSocial;
        }

        private void ComputaAlteracaoContabil(int IdOrgao, int IdUO, int IdUGE, int IdContaContabil)
        {
            var implantado = UGEIntegradaAoSIAFEM(IdUGE);

            if (implantado)
            {
                var registro = db.HouveAlteracaoContabeis
                                 .Where(h => h.IdOrgao == IdOrgao &&
                                             h.IdUO == IdUO &&
                                             h.IdUGE == IdUGE &&
                                             h.IdContaContabil == IdContaContabil)
                                  .FirstOrDefault();

                if (registro != null)
                {
                    registro.HouveAlteracao = true;
                    db.Entry(registro).State = EntityState.Modified;
                }
                else
                {
                    registro = new HouveAlteracaoContabil();
                    registro.IdOrgao = IdOrgao;
                    registro.IdUO = IdUO;
                    registro.IdUGE = IdUGE;
                    registro.IdContaContabil = IdContaContabil;
                    registro.HouveAlteracao = true;
                    db.Entry(registro).State = EntityState.Added;
                }

                db.SaveChanges();
            }
        }

        private bool BPsNaContaContabilInservivel(int idDoBP)
        {
            var IdContaContabil = (from a in db.Assets
                                           join am in db.AssetMovements
                                           on a.Id equals am.AssetId
                                           where a.Id == idDoBP
                                           && am.Status == true
                                           select am.AuxiliaryAccountId).FirstOrDefault();

            var listaOutrasContasContabeis = (from a in db.AuxiliaryAccounts
                                              where a.Id == IdContaContabil
                                              select a.BookAccount).FirstOrDefault();

            return listaOutrasContasContabeis == 123110805;
        }

        private bool ValidaUltimoHistorico(int AssetId, int tipoDoMovimentoARealizar) {
            var HistoricoAtivoBP = (from am in db.AssetMovements
                                    join a in db.Assets
                                    on am.AssetId equals a.Id
                                    where am.AssetId == AssetId
                                    && am.Status == true
                                    select new BPValidoParaViewModel
                                    {
                                        ContaContabilInservivel = (am.AuxiliaryAccountId == 212508),
                                        Terceiro = (a.flagTerceiro == true),
                                        TipoMovimento = am.MovementTypeId
                                    }).FirstOrDefault();

            if (HistoricoAtivoBP == null)
                return false;

            switch (tipoDoMovimentoARealizar) {
                case (int)EnumMovimentType.MovimentacaoInterna:
                case (int)EnumMovimentType.MovExtravioFurtoRouboBensMoveis:
                    return HistoricoAtivoBP.TipoMovimento != (int)EnumMovimentType.Transferencia &&
                           HistoricoAtivoBP.TipoMovimento != (int)EnumMovimentType.Doacao &&
                           HistoricoAtivoBP.TipoMovimento != (int)EnumMovimentType.MovDoacaoIntraNoEstado &&
                           HistoricoAtivoBP.TipoMovimento != (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado &&
                           HistoricoAtivoBP.TipoMovimento != (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado &&
                           HistoricoAtivoBP.TipoMovimento != (int)EnumMovimentType.SaidaConserto &&
                           HistoricoAtivoBP.TipoMovimento != (int)EnumMovimentType.DisponibilizadoParaBolsaSecretaria &&
                           HistoricoAtivoBP.TipoMovimento != (int)EnumMovimentType.DisponibilizadoParaBolsaEstadual;
                case (int)EnumMovimentType.SaidaConserto:
                    return HistoricoAtivoBP.TipoMovimento != (int)EnumMovimentType.Transferencia &&
                           HistoricoAtivoBP.TipoMovimento != (int)EnumMovimentType.Doacao &&
                           HistoricoAtivoBP.TipoMovimento != (int)EnumMovimentType.MovDoacaoIntraNoEstado &&
                           HistoricoAtivoBP.TipoMovimento != (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado &&
                           HistoricoAtivoBP.TipoMovimento != (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado &&
                           HistoricoAtivoBP.TipoMovimento != (int)EnumMovimentType.SaidaConserto &&
                           HistoricoAtivoBP.TipoMovimento != (int)EnumMovimentType.DisponibilizadoParaBolsaSecretaria &&
                           HistoricoAtivoBP.TipoMovimento != (int)EnumMovimentType.DisponibilizadoParaBolsaEstadual &&
                           HistoricoAtivoBP.TipoMovimento != (int)EnumMovimentType.MovInservivelNaUGE &&
                           !HistoricoAtivoBP.ContaContabilInservivel;
                case (int)EnumMovimentType.VoltaConserto:
                    return HistoricoAtivoBP.TipoMovimento == (int)EnumMovimentType.SaidaConserto;
                case (int)EnumMovimentType.MovDoacaoIntraNoEstado:
                case (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado:
                case (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado:
                    return HistoricoAtivoBP.TipoMovimento != (int)EnumMovimentType.Transferencia &&
                           HistoricoAtivoBP.TipoMovimento != (int)EnumMovimentType.Doacao &&
                           HistoricoAtivoBP.TipoMovimento != (int)EnumMovimentType.MovDoacaoIntraNoEstado &&
                           HistoricoAtivoBP.TipoMovimento != (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado &&
                           HistoricoAtivoBP.TipoMovimento != (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado &&
                           HistoricoAtivoBP.TipoMovimento != (int)EnumMovimentType.SaidaConserto &&
                           HistoricoAtivoBP.TipoMovimento != (int)EnumMovimentType.MovInservivelNaUGE &&
                           !HistoricoAtivoBP.ContaContabilInservivel &&
                           !HistoricoAtivoBP.Terceiro;
                case (int)EnumMovimentType.RetiradaDaBolsa:
                    return HistoricoAtivoBP.TipoMovimento == (int)EnumMovimentType.DisponibilizadoParaBolsaSecretaria ||
                           HistoricoAtivoBP.TipoMovimento == (int)EnumMovimentType.DisponibilizadoParaBolsaEstadual;
                case (int)EnumMovimentType.MovSaidaInservivelUGEDoacao:
                case (int)EnumMovimentType.MovSaidaInservivelUGETransferencia:
                case (int)EnumMovimentType.MovPerdaInvoluntariaInservivelBensMoveis:
                    return HistoricoAtivoBP.TipoMovimento == (int)EnumMovimentType.MovInservivelNaUGE ||
                           HistoricoAtivoBP.ContaContabilInservivel;
                case (int)EnumMovimentType.MovComodatoTerceirosRecebidos:
                    return HistoricoAtivoBP.TipoMovimento != (int)EnumMovimentType.SaidaConserto &&
                           HistoricoAtivoBP.Terceiro;
                case (int)EnumMovimentType.MovPerdaInvoluntariaBensMoveis:
                    return HistoricoAtivoBP.TipoMovimento != (int)EnumMovimentType.Transferencia &&
                           HistoricoAtivoBP.TipoMovimento != (int)EnumMovimentType.Doacao &&
                           HistoricoAtivoBP.TipoMovimento != (int)EnumMovimentType.MovDoacaoIntraNoEstado &&
                           HistoricoAtivoBP.TipoMovimento != (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado &&
                           HistoricoAtivoBP.TipoMovimento != (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado &&
                           HistoricoAtivoBP.TipoMovimento != (int)EnumMovimentType.SaidaConserto &&
                           HistoricoAtivoBP.TipoMovimento != (int)EnumMovimentType.MovInservivelNaUGE &&
                           HistoricoAtivoBP.TipoMovimento != (int)EnumMovimentType.DisponibilizadoParaBolsaSecretaria &&
                           HistoricoAtivoBP.TipoMovimento != (int)EnumMovimentType.DisponibilizadoParaBolsaEstadual &&
                           !HistoricoAtivoBP.ContaContabilInservivel;
                default:
                    return HistoricoAtivoBP.TipoMovimento != (int)EnumMovimentType.Transferencia &&
                           HistoricoAtivoBP.TipoMovimento != (int)EnumMovimentType.Doacao &&
                           HistoricoAtivoBP.TipoMovimento != (int)EnumMovimentType.MovDoacaoIntraNoEstado &&
                           HistoricoAtivoBP.TipoMovimento != (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado &&
                           HistoricoAtivoBP.TipoMovimento != (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado &&
                           HistoricoAtivoBP.TipoMovimento != (int)EnumMovimentType.SaidaConserto &&
                           HistoricoAtivoBP.TipoMovimento != (int)EnumMovimentType.MovInservivelNaUGE &&
                           HistoricoAtivoBP.TipoMovimento != (int)EnumMovimentType.DisponibilizadoParaBolsaSecretaria &&
                           HistoricoAtivoBP.TipoMovimento != (int)EnumMovimentType.DisponibilizadoParaBolsaEstadual &&
                           !HistoricoAtivoBP.ContaContabilInservivel &&
                           !HistoricoAtivoBP.Terceiro;
            }
        }

        private int IdUGEFundoSocial()
        {
            return (from i in db.Institutions
                    join b in db.BudgetUnits on i.Id equals b.InstitutionId
                    join m in db.ManagerUnits on b.Id equals m.BudgetUnitId
                    where i.Code == "51004" && m.Code == "510032" && m.Status == true
                    select m.Id).FirstOrDefault();
        }

        private bool BPsSaoTerceiros(int idDoBP)
        {
            var valorNoBanco = (from a in db.Assets
                                where a.Id == idDoBP
                                select a.flagTerceiro).FirstOrDefault();

            return valorNoBanco != null && valorNoBanco == true;
        }

        private bool BPDoGrupoAnimal(int idDoBP)
        {
            return (from a in db.Assets
                    where a.Id == idDoBP
                    select a.MaterialGroupCode).FirstOrDefault() == 88;
        }

        private bool BPTravado(int idsDosBPs)
        {
            var valorNoBanco = (from a in db.Assets
                                where a.Id == idsDosBPs
                                && a.MaterialGroupCode == 88
                                select a.Id).Any();

            return valorNoBanco == true;
        }






        private void CarregaFundoSocial()
        {
            if (db == null)
                db = new SAMContext();
            //Orgao 51004 - UO 5100
            var listaOrgao = db.Institutions.Where(i => i.Code == "51004").ToList();
            listaOrgao.ForEach(l => l.Description = l.Code + " - " + l.Description);

            ViewBag.InstitutionsDestino = new SelectList(listaOrgao, "Id", "Description");

            var listaUO = db.BudgetUnits.Where(i => i.Code == "51004").ToList();
            listaUO.ForEach(l => l.Description = l.Code + " - " + l.Description);

            ViewBag.BudgetUnitIdDestino = new SelectList(listaUO, "Id", "Description");

            var listaUGE = db.ManagerUnits.Where(m => m.Code == "510032" && m.Status == true).ToList();
            listaUGE.ForEach(m => m.Description = m.Code + " - " + m.Description);
            ViewBag.ManagerUnitIdDestino = new SelectList(listaUGE, "Id", "Description");
        }

        public string ValidaDataDeMovimentacao(string strDataDeTransferencia, MovimentoViewModel movimentoViewModel)
        {
            string _mensagemRetorno = "";

            DateTime dataDeTransferencia = Convert.ToDateTime(strDataDeTransferencia);

            if (strDataDeTransferencia == null || strDataDeTransferencia.Trim() == string.Empty)
            {
                _mensagemRetorno = "O campo Data de Movimento é obrigatório";
                return _mensagemRetorno;
            }

            if (dataDeTransferencia.Date > DateTime.Now.Date)
            {
                _mensagemRetorno = "Por favor, informe uma data de transferência igual ou inferior a data atual.";
                return _mensagemRetorno;
            }

            string anoMesReferenciaFechamentoDaUGE = RecuperaAnoMesReferenciaFechamento(movimentoViewModel.ManagerUnitId);
            string anoMesTransferencia = dataDeTransferencia.Year.ToString().PadLeft(4, '0') + dataDeTransferencia.Month.ToString().PadLeft(2, '0');

            if (anoMesReferenciaFechamentoDaUGE != anoMesTransferencia)
            {
                _mensagemRetorno = "Por favor, informe uma data de transferência que corresponda ao mês/ano de fechamento " + anoMesReferenciaFechamentoDaUGE.Substring(4).PadLeft(2, '0') + "/" + anoMesReferenciaFechamentoDaUGE.Substring(0, 4).PadLeft(4, '0') + ".";
                return _mensagemRetorno;
            }

            DateTime dataMaisAtualMovimentacaoPorAssets = (from am in db.AssetMovements where am.AssetId == movimentoViewModel.AssetId && am.FlagEstorno != true && am.DataEstorno == null select am.MovimentDate).Max();

            if (dataMaisAtualMovimentacaoPorAssets.Date > dataDeTransferencia)
            {
                _mensagemRetorno = "Existem itens para a movimentação cuja última movimentação foi em " + dataMaisAtualMovimentacaoPorAssets.Day + "/" + dataMaisAtualMovimentacaoPorAssets.Month + "/" + dataMaisAtualMovimentacaoPorAssets.Year + ". Por favor, informe uma data de transferência igual ou superior.";
                return _mensagemRetorno;
            }

            return _mensagemRetorno;
        }

        public string ValidaMesRefUGEDestinoIgualUGEOrigem(MovimentoViewModel movimentoViewModel) {
            string _mensagemRetorno = string.Empty;

            //if temporário, pois o Fundo Social (o único a receber inservível, não está implantado)
            if ((movimentoViewModel.MovementTypeId != (int)EnumMovimentType.MovSaidaInservivelUGEDoacao) && (movimentoViewModel.MovementTypeId != (int)EnumMovimentType.MovSaidaInservivelUGETransferencia))
            {
                //Valida se a transferencia esta sendo realizada de um mes/ano maior que o remetente
                if (ValidaMesAnoReferenciaDiferenteDaUGEDestinataria(movimentoViewModel.ManagerUnitId, movimentoViewModel.ManagerUnitIdDestino))
                {
                    var managerUnitRemetente = RetornaUGE(movimentoViewModel.ManagerUnitId);
                    var anoRemetente = managerUnitRemetente.ManagmentUnit_YearMonthReference.Substring(0, 4).PadLeft(4, '0');
                    var mesRemetente = managerUnitRemetente.ManagmentUnit_YearMonthReference.Substring(4).PadLeft(2, '0');

                    var managerUnitDestinatario = RetornaUGE(movimentoViewModel.ManagerUnitIdDestino);
                    var anoDestinatario = managerUnitDestinatario.ManagmentUnit_YearMonthReference.Substring(0, 4).PadLeft(4, '0');
                    var mesDestinatario = managerUnitDestinatario.ManagmentUnit_YearMonthReference.Substring(4).PadLeft(2, '0');

                    _mensagemRetorno = $"O mês de referência da UGE \"{managerUnitRemetente.Description})\" - ({mesRemetente + "/" + anoRemetente}) se encontra maior que o mês de referência da UGE de destino \"{managerUnitDestinatario.Description}\" - ({mesDestinatario + "/" + anoDestinatario }), por favor, altere a data de movimento.";

                    return _mensagemRetorno;
                }
            }

            return _mensagemRetorno;
        }

        #region AceiteAutomático
        private void verificaImplantacaoAceiteAutomatico(AssetMovements assetMoviment, MovimentoViewModel movimentoViewModel)
        {
            if (verificaNaoImplantacao((int)assetMoviment.SourceDestiny_ManagerUnitId))
                aceiteAutomatico(assetMoviment, movimentoViewModel);

        }

        private void verificaImplantacaoUGEAceiteAutomatico(AssetMovements assetMoviment, MovimentoViewModel movimentoViewModel)
        {
            bool UGEComoOrgaoNaoImplantado = (from m in db.ManagerUnits
                                              where m.Id == assetMoviment.SourceDestiny_ManagerUnitId
                                              select m.FlagTratarComoOrgao).FirstOrDefault();

            if (UGEComoOrgaoNaoImplantado)
                aceiteAutomatico(assetMoviment, movimentoViewModel);

        }

        public bool verificaNaoImplantacao(int IdUGEDestino)
        {
            bool naoImplantado = (from i in db.Institutions
                                  join b in db.BudgetUnits
                                  on i.Id equals b.InstitutionId
                                  join m in db.ManagerUnits
                                  on b.Id equals m.BudgetUnitId
                                  where m.Id == IdUGEDestino
                                  select (!i.flagImplantado || m.FlagTratarComoOrgao)).FirstOrDefault();

            return (naoImplantado);
        }

        private void aceiteAutomatico(AssetMovements assetMoviment, MovimentoViewModel movimentoViewModel)
        {

            assetMoviment.Status = false;
            db.Entry(assetMoviment).State = EntityState.Modified;

            Asset _assetAntesDaMovimentacao = db.Assets.Find(assetMoviment.AssetId);
            _assetAntesDaMovimentacao.Status = false;
            db.Entry(_assetAntesDaMovimentacao).State = EntityState.Modified;

            Asset assetAposAMovimentacao = assetParaAceiteAutomatico(_assetAntesDaMovimentacao, assetMoviment);

            db.Entry(assetAposAMovimentacao).State = EntityState.Added;

            db.SaveChanges();

            AssetMovements assetMovimentNaNovaUGE = assetMovementsParaAceiteAutomatico(assetAposAMovimentacao, assetMoviment);

            db.Entry(assetMovimentNaNovaUGE).State = EntityState.Added;

            assetMoviment.AssetTransferenciaId = assetAposAMovimentacao.Id;
            db.Entry(assetMoviment).State = EntityState.Modified;
            db.SaveChanges();
        }

        private AssetMovements assetMovementsParaAceiteAutomatico(Asset assetAposAMovimentacao, AssetMovements assetMoviment)
        {
            var UOTransferido = (from b in db.BudgetUnits
                                 join m in db.ManagerUnits
                                 on b.Id equals m.BudgetUnitId
                                 where m.Id == assetMoviment.SourceDestiny_ManagerUnitId
                                 select b).FirstOrDefault();

            AssetMovements assetMovimentNaNovaUGE = new AssetMovements();

            assetMovimentNaNovaUGE.AssetId = assetAposAMovimentacao.Id;
            assetMovimentNaNovaUGE.Status = true;
            assetMovimentNaNovaUGE.MovimentDate = assetMoviment.MovimentDate;
            assetMovimentNaNovaUGE.StateConservationId = assetMoviment.StateConservationId;
            assetMovimentNaNovaUGE.InstitutionId = UOTransferido.InstitutionId;
            assetMovimentNaNovaUGE.BudgetUnitId = UOTransferido.Id;
            assetMovimentNaNovaUGE.ManagerUnitId = Convert.ToInt32(assetMoviment.SourceDestiny_ManagerUnitId);
            assetMovimentNaNovaUGE.ResponsibleId = null;
            assetMovimentNaNovaUGE.AuxiliaryAccountId = assetMoviment.AuxiliaryAccountId;
            assetMovimentNaNovaUGE.Login = assetMoviment.Login;
            assetMovimentNaNovaUGE.DataLogin = assetMoviment.DataLogin;

            switch (assetMoviment.MovementTypeId)
            {
                case (int)EnumMovimentType.MovDoacaoIntraNoEstado:
                    assetMovimentNaNovaUGE.MovementTypeId = (int)EnumMovimentType.IncorpDoacaoIntraNoEstado;
                    break;
                case (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado:
                    assetMovimentNaNovaUGE.MovementTypeId = (int)EnumMovimentType.IncorpTransferenciaOutroOrgaoPatrimoniado;
                    break;
                case (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado:
                    assetMovimentNaNovaUGE.MovementTypeId = (int)EnumMovimentType.IncorpTransferenciaMesmoOrgaoPatrimoniado;
                    break;
                case (int)EnumMovimentType.MovSaidaInservivelUGEDoacao:
                    assetMovimentNaNovaUGE.MovementTypeId = (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGEDoacao;
                    break;
                case (int)EnumMovimentType.MovSaidaInservivelUGETransferencia:
                    assetMovimentNaNovaUGE.MovementTypeId = (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGETranferencia;
                    break;
            }

            return assetMovimentNaNovaUGE;
        }

        #endregion

        #endregion

        private bool InvalidaSemLoginSiafem(int TipoMoviemento, string LoginSiafem, string SenhaSiafem)
        {
            bool NaoDigitouLoginSiafem = false;

            int[] movimentoComLoginSiafem = new int[] {
                (int)EnumMovimentType.MovSaidaInservivelUGEDoacao,
                (int)EnumMovimentType.MovSaidaInservivelUGETransferencia,
                (int)EnumMovimentType.MovComodatoConcedidoBensMoveis,
                (int)EnumMovimentType.MovComodatoTerceirosRecebidos,
                (int)EnumMovimentType.MovDoacaoConsolidacao,
                (int)EnumMovimentType.MovDoacaoIntraNoEstado,
                (int)EnumMovimentType.MovDoacaoMunicipio,
                (int)EnumMovimentType.MovDoacaoOutrosEstados,
                (int)EnumMovimentType.MovDoacaoUniao,
                (int)EnumMovimentType.MovExtravioFurtoRouboBensMoveis,
                (int)EnumMovimentType.MovMorteAnimalPatrimoniado,
                (int)EnumMovimentType.MovMudancaCategoriaDesvalorizacao,
                (int)EnumMovimentType.MovSementesPlantasInsumosArvores,
                (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado,
                (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado,
                (int)EnumMovimentType.MovPerdaInvoluntariaBensMoveis,
                (int)EnumMovimentType.MovPerdaInvoluntariaInservivelBensMoveis,
                (int)EnumMovimentType.MovInservivelNaUGE,
                (int)EnumMovimentType.MovVendaLeilaoSemoventes
            };

            if (movimentoComLoginSiafem.Contains(TipoMoviemento))
            {

                if (string.IsNullOrEmpty(LoginSiafem) ||
                   string.IsNullOrWhiteSpace(LoginSiafem) ||
                   string.IsNullOrEmpty(SenhaSiafem) ||
                   string.IsNullOrWhiteSpace(SenhaSiafem))
                {
                    NaoDigitouLoginSiafem = true;
                }
                else
                {
                    NaoDigitouLoginSiafem = LoginSiafem.Length != 11;
                }

            }
            else
            {
                NaoDigitouLoginSiafem = false;
            }

            return NaoDigitouLoginSiafem;
        }

        private bool BPSemPendenciaDeNL(int IdDoBP)
        {
            var mesRefDataInicioIntegracao = (from a in db.Assets
                                              join am in db.AssetMovements
                                              on a.Id equals am.AssetId
                                              join m in db.ManagerUnits
                                              on am.ManagerUnitId equals m.Id
                                              where a.Id == IdDoBP
                                              && am.Status == true
                                              select m.MesRefInicioIntegracaoSIAFEM).First();


            int ano = Convert.ToInt32(mesRefDataInicioIntegracao.ToString().Substring(0, 4));
            int mes = Convert.ToInt32(mesRefDataInicioIntegracao.ToString().Substring(4, 2));

            DateTime inicioIntegracao = new DateTime(ano, mes, 1);

            var listaIdsDasUltimasMovimentacoes = (from a in db.Assets
                                                   join am in db.AssetMovements
                                                   on a.Id equals am.AssetId
                                                   where a.Id == IdDoBP
                                                   && am.Status == true
                                                   && am.DataLogin >= inicioIntegracao
                                                   select am.Id).Distinct().ToList();

            var listaIdsAuditoria = (from r in db.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos
                                     where listaIdsDasUltimasMovimentacoes.Contains(r.AssetMovementsId)
                                     select r.AuditoriaIntegracaoId).Distinct().ToList();

            if (listaIdsAuditoria.Count == 0)
            {
                return false;
            }
            else
            {
                var possuiPendencias = (from n in db.NotaLancamentoPendenteSIAFEMs
                                        where listaIdsAuditoria.Contains(n.AuditoriaIntegracaoId)
                                        && n.StatusPendencia == 1
                                        select n).Count() > 0;

                return possuiPendencias;
            }
        }

        #region Json

        [HttpPost]
        public JsonResult MovimentaDoItemDoInventario(int valor) {
            string msgInformeAoUsuario = null;
            try
            {
                getHierarquiaPerfil();

                ItemInventario itemInventario = (from i in db.ItemInventarios where i.Id == valor select i).AsNoTracking().FirstOrDefault();

                if (itemInventario == null)
                    return Json(new { Mensagem = "Item não encotrado. Provavelmente já tenha sido retirado do Inventário." }, JsonRequestBehavior.AllowGet);

                Inventario inventario = (from i in db.Inventarios where i.Id == itemInventario.InventarioId select i).AsNoTracking().FirstOrDefault();
                Asset assetOrigem = (from a in db.Assets where a.Id == itemInventario.AssetId select a).AsNoTracking().FirstOrDefault();

                var PrecisaDeReclassificacao = (from rc in db.BPsARealizaremReclassificacaoContabeis
                                                where rc.AssetId == assetOrigem.Id
                                                select rc.Id).Count() > 0;

                if (PrecisaDeReclassificacao)
                    return Json(new { Mensagem = "O Bem Patrimonial não pode ser movimentado pois precisa atualizar a conta contábil. Por favor, entre em contato com a UGE onde o Bem se encontra nesse sistema." } , JsonRequestBehavior.AllowGet);

                AssetMovements assetMovementsAnteriorOrigem = (from am in db.AssetMovements where am.AssetId == itemInventario.AssetId && am.Status select am).AsNoTracking().FirstOrDefault();
                
                int tipoMovimentoId = 0;

                switch (itemInventario.Estado)
                {
                    case 2:
                        tipoMovimentoId = (int)EnumMovimentType.MovimentacaoInterna;
                        break;
                    case 4:
                        if (assetMovementsAnteriorOrigem.ManagerUnitId == inventario.RelatedAdministrativeUnit.ManagerUnitId)
                            tipoMovimentoId = (int)EnumMovimentType.MovimentacaoInterna;
                        else
                            tipoMovimentoId = (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado;
                        break;
                }

                if (!ValidaUltimoHistorico(assetOrigem.Id, tipoMovimentoId)) {
                    return Json(new { Mensagem = "O Bem Patrimonial não pode ser movimentado para sua UGE devido ao seu tipo, conta contábil ou último histórico realizado. Por favor, verifique a situação com UGE no qual esse BP se encontra no sistema." }, JsonRequestBehavior.AllowGet);
                }

                if (UGEIntegradaAoSIAFEM(assetMovementsAnteriorOrigem.ManagerUnitId))
                {
                    if (!BPSemPendenciaDeNL(assetOrigem.Id))
                    {

                        return Json(new { Mensagem = "O Bem Patrimonial não pode ser movimentado pois possui pendência(s) de NL(s). Por favor, entre em contato com a UGE onde o Bem se encontra nesse sistema." }, JsonRequestBehavior.AllowGet);
                    }
                }

                //DateTime DataMovimentacao = RecuperaAnoMesReferenciaFechamento(inventario.RelatedAdministrativeUnit.ManagerUnitId).RetornarMesAnoReferenciaDateTime();

                //string DataMovimentacaoPalavra = DataMovimentacao.ToString("dd/MM/yyyy");

                //if (inventario.RelatedAdministrativeUnit.RelatedManagerUnit.RelatedBudgetUnit.RelatedInstitution.flagImplantado == true)
                //{
                //    //Valida se a data de transferencia segue as normais padrões
                //    var mensagemErro = ValidaDataDeTranferencia(DataMovimentacaoPalavra, valor, tipoMovimentoId, 
                //                                                inventario.RelatedAdministrativeUnit.ManagerUnitId, 
                //                                                itemInventario.RelatedAsset.ManagerUnitId);

                //    if (mensagemErro != string.Empty)
                //    {
                //        return Json(new { Mensagem = mensagemErro }, JsonRequestBehavior.AllowGet);
                //    }
                //}

                // Gera Numero de Documento (ANO + COD_UGE + SEQUENCIA)

                var assetMoviment = new AssetMovements();

                string Sequencia = BuscaUltimoNumeroDocumentoSaida(assetMovementsAnteriorOrigem.ManagerUnitId);

                if (Sequencia.Equals("0"))
                    assetMoviment.NumberDoc = DateTime.Now.Year + assetMovementsAnteriorOrigem.RelatedManagerUnit.Code + "0001";
                else
                {
                    if (Sequencia.Length > 14)
                    {
                        int contador = Convert.ToInt32(Sequencia.Substring(10, Sequencia.Length - 10));

                        if (contador < 9999)
                        {
                            assetMoviment.NumberDoc = Sequencia.Substring(0, 10) + (contador + 1).ToString().PadLeft(4, '0');
                        }
                        else
                        {
                            assetMoviment.NumberDoc = (long.Parse(Sequencia) + 1).ToString();
                        }
                    }
                    else
                        assetMoviment.NumberDoc = (long.Parse(Sequencia) + 1).ToString();
                }


                //Fim Gera Numero de Documento

                DateTime dataMovimento = DateTime.Now;

                assetMoviment.AssetId = assetOrigem.Id;
                assetMoviment.MovimentDate = dataMovimento;
                assetMoviment.StateConservationId = assetMovementsAnteriorOrigem.StateConservationId;
                assetMoviment.NumberPurchaseProcess = assetMovementsAnteriorOrigem.NumberPurchaseProcess;
                assetMoviment.InstitutionId = _institutionId;
                assetMoviment.BudgetUnitId = assetMovementsAnteriorOrigem.BudgetUnitId;
                assetMoviment.ManagerUnitId = assetMovementsAnteriorOrigem.ManagerUnitId;
                assetMoviment.AuxiliaryAccountId = assetMovementsAnteriorOrigem.AuxiliaryAccountId;
                assetMoviment.ContaContabilAntesDeVirarInservivel = assetMovementsAnteriorOrigem.ContaContabilAntesDeVirarInservivel;
                assetMoviment.AssetTransferenciaId = null;
                assetMoviment.Login = _login;
                assetMoviment.DataLogin = dataMovimento;

                assetMoviment.Observation = String.Format("MOVIMENTACAO BEM PATRIMONAL VIA TELA DE INVENTARIO.SIGLA:{0}, CHAPA:{1}; ORIGEM: UO:{2} UGE:{3} UA:{4}; DESTINO: UO:{5} UGE:{6} UA:{7}", assetOrigem.InitialName
                                                                                                                                                                                    , assetOrigem.NumberIdentification
                                                                                                                                                                                    , assetOrigem.RelatedManagerUnit.RelatedBudgetUnit.Code
                                                                                                                                                                                    , assetOrigem.RelatedManagerUnit.Code
                                                                                                                                                                                    , assetMovementsAnteriorOrigem.RelatedAdministrativeUnit.Code
                                                                                                                                                                                    , inventario.RelatedAdministrativeUnit.RelatedManagerUnit.RelatedBudgetUnit.Code
                                                                                                                                                                                    , inventario.RelatedAdministrativeUnit.RelatedManagerUnit.Code
                                                                                                                                                                                    , inventario.RelatedAdministrativeUnit.Code);

                assetMovementsAnteriorOrigem.CPFCNPJ = null;

                if ((from ram in db.RelationshipAuxiliaryAccountMovementTypes
                     where ram.MovementTypeId == tipoMovimentoId
                     select ram
                                      ).Any())
                {
                    assetMoviment.AuxiliaryAccountId =
                        (from ram in db.RelationshipAuxiliaryAccountMovementTypes
                         where ram.MovementTypeId == tipoMovimentoId
                         select ram.AuxiliaryAccountId
                         ).FirstOrDefault();
                }

                assetMoviment.MovementTypeId = tipoMovimentoId;

                using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted }))
                {
                    //Desativa a movimentação anterior ---------------------------------------------------------------------------------------------------------------------------------------------
                    var oldsAssetMoviment = (from am in db.AssetMovements
                                             where am.Status == true &&
                                                   am.AssetId == assetOrigem.Id
                                             select am).ToList();

                    foreach (var oldAssetMovimentDB in oldsAssetMoviment)
                    {
                        if (oldAssetMovimentDB.Status == true)
                        {
                            oldAssetMovimentDB.Status = false;

                            db.Entry(oldAssetMovimentDB).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    oldsAssetMoviment = null;
                    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


                    switch (tipoMovimentoId)
                    {
                        case (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado:
                            bool? orgaoDestinoTransferenciaImplantadoMesmoOrgao = (from m in db.Institutions
                                                                                   where m.Id == _institutionId
                                                                                   select m.flagImplantado).FirstOrDefault();

                            if (orgaoDestinoTransferenciaImplantadoMesmoOrgao == true)
                            {
                                assetMoviment.Status = true;
                                assetMoviment.AdministrativeUnitId = null;
                                assetMoviment.SectionId = null;
                                assetMoviment.ResponsibleId = null;
                                assetMoviment.SourceDestiny_ManagerUnitId = inventario.UgeId;
                                assetMoviment.ExchangeId = null;
                                assetMoviment.ExchangeDate = null;
                                assetMoviment.ExchangeUserId = null;
                                assetMoviment.TypeDocumentOutId = null;

                                db.Entry(assetMoviment).State = EntityState.Added;
                                db.SaveChanges();
                            }
                            else
                            {
                                assetMoviment.Status = false;
                                assetMoviment.AdministrativeUnitId = null;
                                assetMoviment.SectionId = null;
                                assetMoviment.ResponsibleId = null;
                                assetMoviment.SourceDestiny_ManagerUnitId = inventario.UgeId;
                                assetMoviment.ExchangeId = null;
                                assetMoviment.ExchangeDate = null;
                                assetMoviment.ExchangeUserId = null;
                                assetMoviment.TypeDocumentOutId = null;
                                assetMoviment.flagUGENaoUtilizada = true;

                                db.Entry(assetMoviment).State = EntityState.Added;
                                db.SaveChanges();

                                var assetTransferidoDB2 = (from a in db.Assets
                                                           where a.Id == assetOrigem.Id
                                                           select a).FirstOrDefault();

                                assetTransferidoDB2.Status = false;
                                db.Entry(assetTransferidoDB2).State = EntityState.Modified;
                                db.SaveChanges();
                            }

                            aceiteAutomatico(assetMoviment, inventario);

                            itemInventario.Estado = (int)EnumSituacaoFisicaBP.Transferido;
                            db.SaveChanges();

                            break;

                        case (int)EnumMovimentType.MovimentacaoInterna:
                            assetMoviment.Status = true;
                            assetMoviment.NumberPurchaseProcess = assetMovementsAnteriorOrigem.NumberPurchaseProcess;
                            assetMoviment.BudgetUnitId = assetMovementsAnteriorOrigem.BudgetUnitId;
                            assetMoviment.ManagerUnitId = assetMovementsAnteriorOrigem.ManagerUnitId;
                            assetMoviment.AdministrativeUnitId = inventario.UaId;
                            assetMoviment.SectionId = inventario.DivisaoId;
                            assetMoviment.ResponsibleId = inventario.ResponsavelId;
                            assetMoviment.SourceDestiny_ManagerUnitId = null;
                            assetMoviment.ExchangeId = null;
                            assetMoviment.ExchangeDate = null;
                            assetMoviment.ExchangeUserId = null;
                            assetMoviment.TypeDocumentOutId = null;

                            db.Entry(assetMoviment).State = EntityState.Added;
                            db.SaveChanges();

                            itemInventario.Estado = (int)EnumSituacaoFisicaBP.Movimentado;
                            break;

                            
                    }

                    var loginUsuarioSIAFEM = string.Empty;
                    var senhaUsuarioSIAFEM = string.Empty;
                    //var ugeSessaoLogada = assetMoviment.RelatedManagerUnit.Code;
                    List<AssetMovements> listaMovimentacoesPatrimoniaisEmLote = new List<AssetMovements>();
                    listaMovimentacoesPatrimoniaisEmLote.Add(assetMoviment);

                    
                    msgInformeAoUsuario = base.processamentoMovimentacaoPatrimonialNoContabilizaSP(tipoMovimentoId, loginUsuarioSIAFEM, senhaUsuarioSIAFEM, assetMoviment.NumberDoc, listaMovimentacoesPatrimoniaisEmLote, true);
                    msgInformeAoUsuario = msgInformeAoUsuario.ToJavaScriptString();

                    itemInventario.AssetMovementsIdOriginal = assetMovementsAnteriorOrigem.Id;
                    db.Entry(itemInventario).State = EntityState.Modified;
                    db.SaveChanges();

                    //pra origem
                    ComputaAlteracaoContabil(assetMovementsAnteriorOrigem.InstitutionId, assetMovementsAnteriorOrigem.BudgetUnitId,
                                         assetMovementsAnteriorOrigem.ManagerUnitId, (int)assetMovementsAnteriorOrigem.AuxiliaryAccountId);

                    //pro destino
                    ComputaAlteracaoContabil(assetMoviment.InstitutionId, assetMoviment.BudgetUnitId,
                                             assetMoviment.ManagerUnitId, (int)assetMoviment.AuxiliaryAccountId);

                    transaction.Complete();
                }

                return Json(new { resultado = msgInformeAoUsuario }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e) {
                return Json(new { Mensagem = "Erro ao efetuar o processo, favor contatar o administrador responsável!" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DevolucaoFisicaDoItemDoInventario(int valor) {
            try
            {
                ItemInventario itemInventario = (from i in db.ItemInventarios where i.Id == valor select i).AsNoTracking().FirstOrDefault();

                if (itemInventario == null)
                    return Json(new { Mensagem = "Item não encotrado. Provavelmente já tenha sido retirado do Inventário." }, JsonRequestBehavior.AllowGet);

                int AssetMovementsIdOriginal = (from am in db.AssetMovements where am.AssetId == itemInventario.AssetId && am.Status select am.Id).FirstOrDefault();

                itemInventario.Estado = EnumSituacaoFisicaBP.Devolvido.GetHashCode();
                itemInventario.AssetMovementsIdOriginal = AssetMovementsIdOriginal;
                db.Entry(itemInventario).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new { resultado = "Inventário devolvido com Sucesso!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e) {
                return Json(new { Mensagem = "Erro ao efetuar o processo, favor contatar o administrador responsável!" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CorrigeHierarquia(int valor) {
            try
            {
                ItemInventario itemInventario = (from i in db.ItemInventarios where i.Id == valor select i).AsNoTracking().FirstOrDefault();

                if (itemInventario == null)
                    return Json(new { Mensagem = "Item não encotrado. Provavelmente já tenha sido retirado do Inventário." }, JsonRequestBehavior.AllowGet);

                using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted }))
                {
                    Asset BPPendente = (from a in db.Assets where a.Id == itemInventario.AssetId select a).AsNoTracking().FirstOrDefault();
                    BPPendente.ManagerUnitId = itemInventario.RelatedInventario.UgeId;
                    db.Entry(BPPendente).State = EntityState.Modified;
                    db.SaveChanges();

                    AssetMovements HistoricoPendente = (from am in db.AssetMovements where am.AssetId == itemInventario.AssetId select am).AsNoTracking().FirstOrDefault();
                    HistoricoPendente.InstitutionId = itemInventario.RelatedInventario.RelatedAdministrativeUnit.RelatedManagerUnit.RelatedBudgetUnit.InstitutionId;
                    HistoricoPendente.BudgetUnitId = itemInventario.RelatedInventario.RelatedAdministrativeUnit.RelatedManagerUnit.BudgetUnitId;
                    HistoricoPendente.ManagerUnitId = itemInventario.RelatedInventario.UgeId;
                    HistoricoPendente.AdministrativeUnitId = itemInventario.RelatedInventario.UaId;
                    HistoricoPendente.SectionId = itemInventario.RelatedInventario.DivisaoId;
                    HistoricoPendente.ResponsibleId = itemInventario.RelatedInventario.ResponsavelId;
                    db.Entry(HistoricoPendente).State = EntityState.Modified;
                    db.SaveChanges();

                    itemInventario.Estado = (int)EnumSituacaoFisicaBP.OK;
                    db.Entry(itemInventario).State = EntityState.Modified;
                    db.SaveChanges();

                    transaction.Complete();
                }
                return Json(new { resultado = "Hierarquia corrigida com sucesso!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { Mensagem = "Erro ao efetuar o processo, favor contatar o administrador responsável!" }, JsonRequestBehavior.AllowGet);
            }
        }

        private void aceiteAutomatico(AssetMovements assetMoviment, Inventario inventario)
        {

            assetMoviment.Status = false;
            db.Entry(assetMoviment).State = EntityState.Modified;

            Asset _assetAntesDaMovimentacao = db.Assets.Find(assetMoviment.AssetId);
            _assetAntesDaMovimentacao.Status = false;
            db.Entry(_assetAntesDaMovimentacao).State = EntityState.Modified;

            Asset assetAposAMovimentacao = assetParaAceiteAutomatico(_assetAntesDaMovimentacao, assetMoviment);

            db.Entry(assetAposAMovimentacao).State = EntityState.Added;

            db.SaveChanges();

            AssetMovements assetMovimentNaNovaUGE = assetMovementsParaAceiteAutomatico(assetAposAMovimentacao, assetMoviment, inventario);

            db.Entry(assetMovimentNaNovaUGE).State = EntityState.Added;

            assetMoviment.AssetTransferenciaId = assetAposAMovimentacao.Id;
            db.Entry(assetMoviment).State = EntityState.Modified;
            db.SaveChanges();
        }

        private string BuscaUltimoNumeroDocumentoSaida(int IdUGE)
        {

            StringBuilder builder = new StringBuilder();

            builder.Append("EXEC [SAM_BUSCA_ULTIMO_NUMERO_DOCUMENTO_SAIDA] @ManagerUnitId = " + IdUGE.ToString());
            builder.Append(",@Ano = " + DateTime.Now.Year.ToString());

            return db.Database.SqlQuery<string>(builder.ToString()).FirstOrDefault();
        }

        #region aceite Automático da Origem
        private Asset assetParaAceiteAutomatico(Asset _assetAntesDaMovimentacao, AssetMovements assetMoviment)
        {
            Asset assetAposAMovimentacao = new Asset();

            assetAposAMovimentacao.Status = true;

            assetAposAMovimentacao.InitialId = _assetAntesDaMovimentacao.InitialId;
            assetAposAMovimentacao.InitialName = _assetAntesDaMovimentacao.InitialName;
            assetAposAMovimentacao.NumberIdentification = _assetAntesDaMovimentacao.NumberIdentification;
            assetAposAMovimentacao.DiferenciacaoChapa = _assetAntesDaMovimentacao.DiferenciacaoChapa;
            assetAposAMovimentacao.AcquisitionDate = _assetAntesDaMovimentacao.AcquisitionDate;
            assetAposAMovimentacao.ValueAcquisition = _assetAntesDaMovimentacao.ValueAcquisition;
            assetAposAMovimentacao.MaterialItemCode = _assetAntesDaMovimentacao.MaterialItemCode;
            assetAposAMovimentacao.MaterialItemDescription = _assetAntesDaMovimentacao.MaterialItemDescription;
            assetAposAMovimentacao.MaterialGroupCode = _assetAntesDaMovimentacao.MaterialGroupCode;
            assetAposAMovimentacao.LifeCycle = _assetAntesDaMovimentacao.LifeCycle;


            assetAposAMovimentacao.AceleratedDepreciation = false;
            assetAposAMovimentacao.Empenho = null;
            assetAposAMovimentacao.ShortDescriptionItemId = _assetAntesDaMovimentacao.ShortDescriptionItemId;
            assetAposAMovimentacao.OldNumberIdentification = _assetAntesDaMovimentacao.NumberIdentification;
            assetAposAMovimentacao.DiferenciacaoChapaAntiga = _assetAntesDaMovimentacao.DiferenciacaoChapa;
            assetAposAMovimentacao.NumberDoc = _assetAntesDaMovimentacao.NumberDoc;
            assetAposAMovimentacao.MovimentDate = _assetAntesDaMovimentacao.MovimentDate;
            assetAposAMovimentacao.flagDepreciaAcumulada = 1;
            assetAposAMovimentacao.ResidualValueCalc = _assetAntesDaMovimentacao.ResidualValueCalc;
            assetAposAMovimentacao.flagAcervo = _assetAntesDaMovimentacao.flagAcervo;
            assetAposAMovimentacao.flagDepreciationCompleted = false;
            assetAposAMovimentacao.flagTerceiro = _assetAntesDaMovimentacao.flagTerceiro;
            assetAposAMovimentacao.flagAnimalNaoServico = _assetAntesDaMovimentacao.flagAnimalNaoServico;
            assetAposAMovimentacao.flagVindoDoEstoque = false;
            assetAposAMovimentacao.IndicadorOrigemInventarioInicial = (int)EnumOrigemInventarioInicial.NaoEhInventarioInicial;
            assetAposAMovimentacao.ValorDesdobramento = _assetAntesDaMovimentacao.ValorDesdobramento;

            //Campos que são usados da tabela MontlhyDepreciation na Visão Geral
            assetAposAMovimentacao.ValueUpdate = _assetAntesDaMovimentacao.ValueUpdate;
            assetAposAMovimentacao.MonthUsed = _assetAntesDaMovimentacao.MonthUsed;
            assetAposAMovimentacao.DepreciationByMonth = _assetAntesDaMovimentacao.DepreciationByMonth;
            assetAposAMovimentacao.DepreciationAccumulated = _assetAntesDaMovimentacao.DepreciationAccumulated;
            assetAposAMovimentacao.RateDepreciationMonthly = _assetAntesDaMovimentacao.RateDepreciationMonthly;
            assetAposAMovimentacao.ResidualValue = _assetAntesDaMovimentacao.ResidualValue;

            //Campo usado para consulta na tabela MontlhyDepreciation
            assetAposAMovimentacao.AssetStartId = _assetAntesDaMovimentacao.AssetStartId;

            //Campo com valores pedos no registro de AssetMovement
            assetAposAMovimentacao.ManagerUnitId = Convert.ToInt32(assetMoviment.SourceDestiny_ManagerUnitId);
            assetAposAMovimentacao.Login = assetMoviment.Login;
            assetAposAMovimentacao.DataLogin = assetMoviment.DataLogin;

            switch (assetMoviment.MovementTypeId)
            {
                case (int)EnumMovimentType.MovDoacaoIntraNoEstado:
                    assetAposAMovimentacao.MovementTypeId = (int)EnumMovimentType.IncorpDoacaoIntraNoEstado;
                    break;
                case (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado:
                    assetAposAMovimentacao.MovementTypeId = (int)EnumMovimentType.IncorpTransferenciaOutroOrgaoPatrimoniado;
                    break;
                case (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado:
                    assetAposAMovimentacao.MovementTypeId = (int)EnumMovimentType.IncorpTransferenciaMesmoOrgaoPatrimoniado;
                    break;
                case (int)EnumMovimentType.MovSaidaInservivelUGEDoacao:
                    assetAposAMovimentacao.MovementTypeId = (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGEDoacao;
                    break;
                case (int)EnumMovimentType.MovSaidaInservivelUGETransferencia:
                    assetAposAMovimentacao.MovementTypeId = (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGETranferencia;
                    break;
            }

            return assetAposAMovimentacao;
        }

        private AssetMovements assetMovementsParaAceiteAutomatico(Asset assetAposAMovimentacao, AssetMovements assetMoviment, Inventario inventario)
        {
            var UOTransferido = (from b in db.BudgetUnits
                                 join m in db.ManagerUnits
                                 on b.Id equals m.BudgetUnitId
                                 where m.Id == assetMoviment.SourceDestiny_ManagerUnitId
                                 select b).FirstOrDefault();

            AssetMovements assetMovimentNaNovaUGE = new AssetMovements();

            assetMovimentNaNovaUGE.AssetId = assetAposAMovimentacao.Id;
            assetMovimentNaNovaUGE.Status = true;
            assetMovimentNaNovaUGE.MovimentDate = assetMoviment.MovimentDate;
            assetMovimentNaNovaUGE.StateConservationId = assetMoviment.StateConservationId;
            assetMovimentNaNovaUGE.InstitutionId = UOTransferido.InstitutionId;
            assetMovimentNaNovaUGE.BudgetUnitId = UOTransferido.Id;
            assetMovimentNaNovaUGE.ManagerUnitId = Convert.ToInt32(assetMoviment.SourceDestiny_ManagerUnitId);
            assetMovimentNaNovaUGE.AdministrativeUnitId = inventario.UaId;
            assetMovimentNaNovaUGE.SectionId = inventario.DivisaoId;
            assetMovimentNaNovaUGE.ResponsibleId = inventario.ResponsavelId;
            assetMovimentNaNovaUGE.AuxiliaryAccountId = assetMoviment.AuxiliaryAccountId;
            assetMovimentNaNovaUGE.Login = assetMoviment.Login;
            assetMovimentNaNovaUGE.DataLogin = assetMoviment.DataLogin;

            switch (assetMoviment.MovementTypeId)
            {
                case (int)EnumMovimentType.MovDoacaoIntraNoEstado:
                    assetMovimentNaNovaUGE.MovementTypeId = (int)EnumMovimentType.IncorpDoacaoIntraNoEstado;
                    break;
                case (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado:
                    assetMovimentNaNovaUGE.MovementTypeId = (int)EnumMovimentType.IncorpTransferenciaOutroOrgaoPatrimoniado;
                    break;
                case (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado:
                    assetMovimentNaNovaUGE.MovementTypeId = (int)EnumMovimentType.IncorpTransferenciaMesmoOrgaoPatrimoniado;
                    break;
            }

            return assetMovimentNaNovaUGE;
        }
        #endregion aceite Automático da Origem

        #region Exportado da Controller Movimento

        private string RecuperaAnoMesReferenciaFechamento(int _managerUnitId)
        {
            return (from m in db.ManagerUnits
                    where m.Id == _managerUnitId
                    select m.ManagmentUnit_YearMonthReference).FirstOrDefault();
        }

        private bool ValidaMesAnoReferenciaDiferenteDaUGEDestinataria(int managerUnitId, int managerUnitIdDestino)
        {
            if (managerUnitId == managerUnitIdDestino) return false;

            var dataRemetente = RetornaDataUGE(managerUnitId);
            var dataDestinatario = RetornaDataUGE(managerUnitIdDestino);

            return (dataRemetente < dataDestinatario || dataRemetente > dataDestinatario);
        }

        private DateTime RetornaDataUGE(int managerUnitId)
        {
            var UGERemetente = (from m in db.ManagerUnits
                                where m.Id == managerUnitId
                                select m.ManagmentUnit_YearMonthReference).FirstOrDefault();

            var anoRemetente = int.Parse(UGERemetente.Substring(0, 4).PadLeft(4, '0'));
            var mesRemetente = int.Parse(UGERemetente.Substring(4).PadLeft(2, '0'));

            var dataRemetente = new DateTime(anoRemetente, mesRemetente, 1);

            return dataRemetente;
        }
        private ManagerUnit RetornaUGE(int managerUnitId)
        {
            return (from m in db.ManagerUnits
                    where m.Id == managerUnitId
                    select m).FirstOrDefault();
        }

        #endregion

        #endregion Json
    }
}