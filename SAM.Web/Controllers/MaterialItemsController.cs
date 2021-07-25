using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SAM.Web.Models;
using PagedList;
using SAM.Web.ViewModels;
using Sam.Integracao.SIAF.Core;
using Sam.Common.Util;
using Sam.Common;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using Sam.Integracao.SIAF.Configuracao;
using SAM.Web.Common;

namespace SAM.Web.Controllers
{
    public class MaterialItemsController : BaseController
    {
        private SAMContext db = new SAMContext();

        // GET: MaterialItems
        public ActionResult Index()
        {
            try
            {
                var query = (from q in db.MaterialItems select q).AsQueryable();
                return View(query.ToList());
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // GET: MaterialItems/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                MaterialItem materialItem = db.MaterialItems.Find(id);
                if (materialItem == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                return View(materialItem);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // GET: MaterialItems/Create
        public ActionResult Create()
        {
            try
            {
                ViewBag.Materiais = new SelectList(db.Materials.OrderBy(m => m.Description), "Id", "Description");
                return View();
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: MaterialItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Code,Description,MaterialId")] MaterialItem materialItem)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.MaterialItems.Add(materialItem);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(materialItem);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // GET: MaterialItems/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                MaterialItem materialItem = db.MaterialItems.Find(id);
                if (materialItem == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                ViewBag.Materiais = new SelectList(db.Materials.OrderBy(m => m.Description), "Id", "Description", materialItem.MaterialId);
                return View(materialItem);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }

        }

        // POST: MaterialItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,Description,MaterialId")] MaterialItem materialItem)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(materialItem).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(materialItem);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // GET: MaterialItems/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                MaterialItem materialItem = db.MaterialItems.Find(id);
                if (materialItem == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                return View(materialItem);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: MaterialItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                MaterialItem materialItem = db.MaterialItems.Find(id);
                db.MaterialItems.Remove(materialItem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult GridItemMaterial()
        {
            try
            {
                IList<MaterialItemViewModel> materialItems = new List<MaterialItemViewModel>();
                return PartialView(materialItems.ToPagedList(1, 1));
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        public ActionResult GridItemMaterial(string sortOrder, string searchString, string currentFilter, int? page)
        {
            try
            {
                //using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted }))
                //{
                //    var retorno = (from im in db.MaterialItems
                //                   join m in db.Materials on im.MaterialId equals m.Id
                //                   join mc in db.MaterialClasses on m.MaterialClassId equals mc.Id
                //                   join mg in db.MaterialGroups on mc.MaterialGroupId equals mg.Id
                //                   select new MaterialItemViewModel
                //                   {
                //                       Code = im.Code,
                //                       Description = im.Description,
                //                       Id = im.Id,
                //                       MaterialId = im.MaterialId,
                //                       LifeCycle = mg.LifeCycle,
                //                       RateDepreciationMonthly = mg.RateDepreciationMonthly,
                //                       ResidualValue = mg.ResidualValue
                //                   }).ToList();

                //    int pageSize = 6;
                //    int pageNumber = (page ?? 1);
                //    transaction.Complete();
                //    return PartialView(retorno.ToPagedList(pageNumber, pageSize));
                //}

                return View(GetMaterialItemFromSIAFISICO(searchString));
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpGet]
        public JsonResult GetItemMaterial(string materialItemCode)
        {

            float codigo = Convert.ToInt64(materialItemCode);
            //Código de itens materiais reservados para acervo e terceiro
            if (codigo == 5628121 || codigo == 5628156)
            {
                List<MensagemModel> mensagens = new List<MensagemModel>();
                MensagemModel mensagem = new MensagemModel();
                mensagem.Id = 0;
                mensagem.Mensagem = "Código de Item Material inválido para essa operação";
                mensagens.Add(mensagem);
                return Json(mensagens, JsonRequestBehavior.AllowGet);
            }

            MaterialItem materialItem = GetMaterialItemFromSIAFISICO(materialItemCode.Trim());
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var retorno = serializer.Serialize(materialItem);

            var _retorno = Json(retorno, JsonRequestBehavior.AllowGet);

            return ValidaRetornoSIAFISICO(_retorno);
        }

        [HttpGet]
        public JsonResult GetItemMaterialBD(string materialItemCode)
        {
            float codigo = Convert.ToInt64(materialItemCode);

            //Código de itens materiais reservados para acervo e terceiro
            if (codigo == 5628121 || codigo == 5628156)
            {
                List<MensagemModel> mensagens = new List<MensagemModel>();
                MensagemModel mensagem = new MensagemModel();
                mensagem.Id = 0;
                mensagem.Mensagem = "Código de Item Material inválido para essa operação";
                mensagens.Add(mensagem);
                return Json(mensagens, JsonRequestBehavior.AllowGet);
            }

            // Pesquisar somente os itens com status D - Deletado e I - Inativo na tabela Item_Siafisico do banco
            ItemSiafisico ItemSiafisico = (from m in db.ItemSiafisicos where m.Cod_Item_Mat == codigo select m).FirstOrDefault();

            MaterialItem materialItem = new MaterialItem();

            if (ItemSiafisico.IsNotNull())
            {
                materialItem = new MaterialItemViewModel
                {
                    MaterialGroupCode = (int)ItemSiafisico.Cod_Grupo,
                    MaterialGroupDescription = ItemSiafisico.Nome_Grupo,
                    MaterialItemCode = ItemSiafisico.Cod_Item_Mat.ToString(),
                    Code = (int)ItemSiafisico.Cod_Item_Mat,
                    Description = ItemSiafisico.Nome_Item_Mat,
                    Material = ItemSiafisico.Nome_Material,
                    Natureza1 = "4490"
                };
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var retorno = serializer.Serialize(materialItem);

            var _retorno = Json(retorno, JsonRequestBehavior.AllowGet);

            return ValidaRetornoSIAFISICO(_retorno);
        }

        public Asset ValidaItemMaterialImportacao(string materialItemCode)
        {
            Asset asset = new Asset();
            MaterialItemViewModel rowMaterialItem = null;
            ProcessadorServicoSIAF svcSIAFISICO = null;
            string msgEstimulo = null;
            string ugeConsulta = null;
            string anoBase = null;
            string retornoMsgEstimulo = null;
            string patternXmlConsulta = null;
            string loginUsuarioSIAFISICO = null;
            string senhaUsuarioSIAFISICO = null;

            #region Consulta Item Material

            msgEstimulo = GeradorEstimuloSIAF.SiafisicoDocConsultaI(materialItemCode.ToString());
            anoBase = DateTime.Now.Year.ToString();
            ugeConsulta = "380236";
            patternXmlConsulta = "/MSG/SISERRO/Doc_Retorno/SFCODOC/SiafisicoDocConsultaI/documento/";

            try
            {
                svcSIAFISICO = new ProcessadorServicoSIAF();
                loginUsuarioSIAFISICO = ConfiguracoesSIAF.userNameConsulta;
                senhaUsuarioSIAFISICO = ConfiguracoesSIAF.passConsulta;
                retornoMsgEstimulo = svcSIAFISICO.ConsumirWS(loginUsuarioSIAFISICO, senhaUsuarioSIAFISICO, anoBase, ugeConsulta, msgEstimulo, true, true);

                if (svcSIAFISICO.ErroProcessamentoWs)
                {
                    asset.flagVerificado = 1;
                    return asset;
                }
                else
                {
                    string lStrGrupo = XmlUtil.getXmlValue(retornoMsgEstimulo, String.Format("{0}{1}", patternXmlConsulta, "Grupo"));
                    string lStrItem = XmlUtil.getXmlValue(retornoMsgEstimulo, String.Format("{0}{1}", patternXmlConsulta, "Item"));
                    string lStrItem1 = XmlUtil.getXmlValue(retornoMsgEstimulo, String.Format("{0}{1}", patternXmlConsulta, "Item1"));
                    string lStrMaterial = XmlUtil.getXmlValue(retornoMsgEstimulo, String.Format("{0}{1}", patternXmlConsulta, "Material"));
                    string lStrNatureza1 = XmlUtil.getXmlValue(retornoMsgEstimulo, String.Format("{0}{1}", patternXmlConsulta, "Natureza"));
                    string lStrNatureza2 = XmlUtil.getXmlValue(retornoMsgEstimulo, String.Format("{0}{1}", patternXmlConsulta, "Natureza2"));
                    string lStrNatureza3 = XmlUtil.getXmlValue(retornoMsgEstimulo, String.Format("{0}{1}", patternXmlConsulta, "Natureza3"));

                    rowMaterialItem = new MaterialItemViewModel()
                    {
                        Code = Int32.Parse(materialItemCode),
                        Description = string.Format("{0}{1}", lStrItem, lStrItem1).Replace(lStrItem.BreakLine(0), "").Trim(),
                        MaterialId = int.Parse(lStrMaterial.BreakLine(0)),
                        MaterialItemCode = lStrItem.BreakLine(0),
                        MaterialGroupCode = int.Parse(lStrGrupo.BreakLine(0)),
                        Natureza1 = lStrNatureza1,
                        Natureza2 = lStrNatureza2,
                        Natureza3 = lStrNatureza3
                    };

                    // ROTINA PARA VERIFICAR SE È UM ITEM DE BEM PERMANENTE, AGUARDANDO DEFINICAO
                    //if (!_objDeserializado.Natureza1.Substring(0,4).Contains("4490") && !_objDeserializado.Natureza2.Substring(0, 4).Contains("4490") && !_objDeserializado.Natureza3.Substring(0, 4).Contains("4490"))
                    //{
                    //    asset.flagVerificado = 1;
                    //    return asset;
                    //}
                }
            }
            catch (Exception ex)
            {
                asset.flagVerificado = 1;
                return asset;
            }

            #endregion

            #region Consulta Grupo de Material

            asset.MaterialItemCode = rowMaterialItem.Code;
            asset.MaterialItemDescription = rowMaterialItem.Description;
            asset.MaterialGroupCode = rowMaterialItem.MaterialGroupCode;

            var _materialGrupo = (from x in db.MaterialGroups where x.Code == rowMaterialItem.MaterialGroupCode select x).FirstOrDefault();

            if(_materialGrupo == null)
            {
                asset.flagVerificado = 2;
                return asset;
            }

            #endregion

            asset.MaterialGroupCode = _materialGrupo.Code;
            asset.LifeCycle = _materialGrupo.LifeCycle;
            asset.ResidualValue = _materialGrupo.ResidualValue;
            asset.RateDepreciationMonthly = _materialGrupo.RateDepreciationMonthly;
            asset.flagVerificado = null;

            return asset;
        }

        public JsonResult ValidaRetornoSIAFISICO(JsonResult _retorno)
        {
            var _objDeserializado = JsonConvert.DeserializeObject<MaterialItemViewModel>(_retorno.Data.ToString());

            MaterialGroup _materialgroup = new MaterialGroup();
            _materialgroup = GetMaterialGroup(_objDeserializado.MaterialGroupCode);

            MensagemModel mensagem = new MensagemModel();
            mensagem.Id = 0;
            mensagem.Mensagem = "";

            List<MensagemModel> mensagens = new List<MensagemModel>();

            if (TempData["Mensagens"].IsNotNull())
            {
                mensagem.Id = 0;
                mensagem.Mensagem = TempData["Mensagens"].ToString();
                mensagens.Add(mensagem);
                return Json(mensagens, JsonRequestBehavior.AllowGet);
            }
            if (_objDeserializado.Description == null)
            {
                mensagem.Id = 0;
                mensagem.Mensagem = "A Descrição do Item Material está nulo no SIAFISICO, favor entrar em contato com o suporte.";
                mensagens.Add(mensagem);
                return Json(mensagens, JsonRequestBehavior.AllowGet);
            }
            if (_objDeserializado.MaterialItemCode == null)
            {
                mensagem.Id = 0;
                mensagem.Mensagem = "O código do Item Material está nulo no SIAFISICO, favor entrar em contato com o suporte.";
                mensagens.Add(mensagem);
                return Json(mensagens, JsonRequestBehavior.AllowGet);
            }
            if (!_objDeserializado.Natureza1.Substring(0,4).Contains("4490") && !_objDeserializado.Natureza2.Substring(0, 4).Contains("4490") && !_objDeserializado.Natureza3.Substring(0, 4).Contains("4490"))
            {
                mensagem.Id = 0;
                mensagem.Mensagem = "O Item Material informado não é um tipo de Despesa de Bem Permanente, favor verificar!";
                mensagens.Add(mensagem);
                return Json(mensagens, JsonRequestBehavior.AllowGet);
            }
            if (_materialgroup == null)
            {
                mensagem.Id = 0;
                mensagem.Mensagem = "O Grupo do Material selecionado não está cadastrado no sistema, favor entrar em contato com a Administração.";
                mensagens.Add(mensagem);
                return Json(mensagens, JsonRequestBehavior.AllowGet);
            }
            if (_objDeserializado.Material == null)
            {
                mensagem.Id = 0;
                mensagem.Mensagem = "A Descrição Resumida do Item Material está nulo no SIAFISICO, favor entrar em contato com o suporte.";
                mensagens.Add(mensagem);
                return Json(mensagens, JsonRequestBehavior.AllowGet);
            }
            else
            {
                _objDeserializado.MaterialGroupDescription = _materialgroup.Description;

                var objSerialize = JsonConvert.SerializeObject(_objDeserializado);

                return Json(objSerialize, JsonRequestBehavior.AllowGet);
            }
        }

        public MaterialItem GetMaterialItemFromSIAFISICO(string materialItemCode)
        {
            try
            {
                var rowMaterialItem = RecuperarCadastroItemMaterialDoSiafisico(materialItemCode);
                return rowMaterialItem;
            }
            catch (Exception excErroExecucao)
            {
                TempData["Mensagens"] = excErroExecucao.Message;
                return new MaterialItem();
            }
        }

        public MaterialItem RecuperarCadastroItemMaterialDoSiafisico(string materialItemCode)
        {
            MaterialItemViewModel rowMaterialItem = null;
            ProcessadorServicoSIAF svcSIAFISICO = null;
            string msgEstimulo = null;
            string ugeConsulta = null;
            string anoBase = null;
            string retornoMsgEstimulo = null;
            string patternXmlConsulta = null;
            string loginUsuarioSIAFISICO = null;
            string senhaUsuarioSIAFISICO = null;

            msgEstimulo = GeradorEstimuloSIAF.SiafisicoDocConsultaI(materialItemCode.ToString());
            anoBase = DateTime.Now.Year.ToString();
            ugeConsulta = "380236";
            patternXmlConsulta = "/MSG/SISERRO/Doc_Retorno/SFCODOC/SiafisicoDocConsultaI/documento/";


            try
            {
                svcSIAFISICO = new ProcessadorServicoSIAF();
                //retornoMsgEstimulo = svcSIAFISICO.ConsumirWS(anoBase, ugeConsulta, msgEstimulo, true, true, true);
                loginUsuarioSIAFISICO = ConfiguracoesSIAF.userNameConsulta;
                senhaUsuarioSIAFISICO = ConfiguracoesSIAF.passConsulta;
                retornoMsgEstimulo = svcSIAFISICO.ConsumirWS(loginUsuarioSIAFISICO, senhaUsuarioSIAFISICO, anoBase, ugeConsulta, msgEstimulo, true, true);


                if (svcSIAFISICO.ErroProcessamentoWs)
                {
                    throw new Exception(svcSIAFISICO.ErroRetornoWs);
                }
                else
                {
                    string lStrStatusOperacao = XmlUtil.getXmlValue(retornoMsgEstimulo, String.Format("{0}{1}", patternXmlConsulta, "StatusOperacao"));
                    string lStrClasse = XmlUtil.getXmlValue(retornoMsgEstimulo, String.Format("{0}{1}", patternXmlConsulta, "Classe"));
                    string lStrClasse1 = XmlUtil.getXmlValue(retornoMsgEstimulo, String.Format("{0}{1}", patternXmlConsulta, "Classe1"));
                    string lStrGrupo = XmlUtil.getXmlValue(retornoMsgEstimulo, String.Format("{0}{1}", patternXmlConsulta, "Grupo"));
                    string lStrGrupo1 = XmlUtil.getXmlValue(retornoMsgEstimulo, String.Format("{0}{1}", patternXmlConsulta, "Grupo1"));
                    string lStrItem = XmlUtil.getXmlValue(retornoMsgEstimulo, String.Format("{0}{1}", patternXmlConsulta, "Item"));
                    string lStrItem1 = XmlUtil.getXmlValue(retornoMsgEstimulo, String.Format("{0}{1}", patternXmlConsulta, "Item1"));
                    string lStrMaterial = XmlUtil.getXmlValue(retornoMsgEstimulo, String.Format("{0}{1}", patternXmlConsulta, "Material"));
                    string lStrNatureza1 = XmlUtil.getXmlValue(retornoMsgEstimulo, String.Format("{0}{1}", patternXmlConsulta, "Natureza"));
                    string lStrNatureza2 = XmlUtil.getXmlValue(retornoMsgEstimulo, String.Format("{0}{1}", patternXmlConsulta, "Natureza2"));
                    string lStrNatureza3 = XmlUtil.getXmlValue(retornoMsgEstimulo, String.Format("{0}{1}", patternXmlConsulta, "Natureza3"));


                    rowMaterialItem = new MaterialItemViewModel()
                    {
                        Code = Int32.Parse(materialItemCode),
                        Description = string.Format("{0}{1}", lStrItem, lStrItem1).Replace(lStrItem.BreakLine(0), "").Trim(),
                        Material = lStrMaterial.Replace(lStrMaterial.BreakLine(0),"").Trim(),
                        MaterialId = int.Parse(lStrMaterial.BreakLine(0)),
                        MaterialItemCode = lStrItem.BreakLine(0),
                        MaterialGroupCode = int.Parse(lStrGrupo.BreakLine(0)),
                        Natureza1 = lStrNatureza1,
                        Natureza2 = lStrNatureza2,
                        Natureza3 = lStrNatureza3
                    };
                }
            }
            catch (Exception excErroExecucao)
            {
                //transportar mensagem de erro para a UI e/ou LOG
                throw excErroExecucao;
            }

            return rowMaterialItem;
        }

        private MaterialGroup GetMaterialGroup(int code)
        {
            var mg = (from x in db.MaterialGroups where x.Code == code select x).ToList().FirstOrDefault();
            return mg;
        }

    }
}
