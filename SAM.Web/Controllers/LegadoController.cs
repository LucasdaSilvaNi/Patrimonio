using SAM.Web.Common;
using SAM.Web.Legado;
using SAM.Web.Legados;
using SAM.Web.Models;
using SAM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;


namespace SAM.Web.Controllers
{
    [Authorize]
    public class LegadoController : Controller
    {
        private ImportarBanco legado = null;
        private SAMContext contexto = new SAMContext();
        private string caminhoDosArquivosExcel = string.Empty;
        private string caminhoDosArquivosJson = string.Empty;
        public LegadoController()
        {
            
        }
        // GET: Legado
        public ActionResult Importar()
        {
            legado = ImportarBanco.createInstance(contexto, string.Empty, string.Empty);
            var Destinos = legado.getDatabaseDetino().Where(x => x.baseDeDestino.Equals(contexto.Database.Connection.Database)).ToList();

            ViewBag.Origens = new SelectList(legado.getDatabaseOrigemLocal(), "IdOrigem", "baseDeOrigem");
            ViewBag.Destinos = new SelectList(Destinos, "IdDestino", "baseDeDestino");
            ImportarLegadoViewModel model = new ImportarLegadoViewModel();

            return View(model);
        }

        [HttpGet]
        public JsonResult getOrgaosSeremExportados(int baseId)
        {
            if (Session["administradorGeral"] == null)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }

            legado = ImportarBanco.createInstance(contexto, string.Empty, string.Empty);
            var destinos = legado.getDatabaseDetino();
            var baseDestino = destinos.Where(x => x.IdDestino == baseId).Select(x => x.baseDeDestino).FirstOrDefault();

            ImportarBanco banco = ImportarBanco.createInstance(contexto, string.Empty,string.Empty);

            IList<ExportarLegadoViewModel> models = banco.getOrgaosSeremExportados(baseDestino);

            models.Insert(0, new ExportarLegadoViewModel { IdOrgao = 0, orgaoNome = "Selecione o orgão para gerar o arquivo" });

            var Jsons = Configuracao.converterObjetoParaJSON<IList<ExportarLegadoViewModel>>(models);

            return Json(Jsons, JsonRequestBehavior.AllowGet);
        }
       
        [HttpGet]
        public ActionResult ExportarExcel()
        {
            if (Session["administradorGeral"] == null)
            {
                if(Session["UsuarioLogado"] != null)
                    return RedirectToAction("Index", "Principal");
                else
                    return RedirectToAction("Index", "Home");
            }

            legado = ImportarBanco.createInstance(contexto, string.Empty, string.Empty);
            var bases = legado.getDatabaseDetino().Where(x => x.baseDeDestino.Equals(contexto.Database.Connection.Database)).ToList();

            ViewBag.Bases = new SelectList(bases, "IdDestino", "baseDeDestino");
            IList<String> Orgaos = new List<String>();
            Orgaos.Add("Selecione o Orgão");
            ViewBag.Orgaos = new SelectList(Orgaos);

            ExportarLegadoViewModel model = new ExportarLegadoViewModel();

            return View(model);
        }

        [HttpGet]
        public ActionResult ImportarPlanilhasDeCarga()
        {
            if (Session["administradorGeral"] == null)
            {
                if (Session["UsuarioLogado"] != null)
                    return RedirectToAction("Index", "Principal");
                else
                    return RedirectToAction("Index", "Home");
            }

            legado = ImportarBanco.createInstance(contexto, string.Empty, string.Empty);
            var bases = legado.getDatabaseDetino().Where(x => x.baseDeDestino.Equals(contexto.Database.Connection.Database)).ToList();

            ViewBag.Bases = new SelectList(bases, "IdDestino", "baseDeDestino");
            IList<String> Orgaos = new List<String>();
            Orgaos.Add("Selecione o Orgão");
            ViewBag.Orgaos = new SelectList(Orgaos);

            ExportarLegadoViewModel model = new ExportarLegadoViewModel();

            return View(model);
        }

        [HttpPost]
        public JsonResult ExportarExcel(ExportarLegadoViewModel model)
        {
            IList<RetornoImportacao> retornoMensagem = new List<RetornoImportacao>();
            IDictionary<string, IList<RetornoImportacao>> resultado = new Dictionary<string, IList<RetornoImportacao>>();

            retornoMensagem.Add(new RetornoImportacao { mensagemImportacao = "<h3>Erro : Verifique a planilha, não foi possivél realizar a exportação!</h3>", quantidadeDeRegistros = 0, caminhoDoArquivoDownload = string.Empty });
            resultado.Add("Mensagem", retornoMensagem);
            string mensagem = Configuracao.converterObjetoParaJSON<IDictionary<string, IList<RetornoImportacao>>>(resultado);


            if (this.Request.Files.Count > 0 && this.Request.Files[0].ContentLength > 0)
            {
                var arquivoExcel = this.Request.Files[0];
               
                legado = ImportarBanco.createInstance(contexto, string.Empty, string.Empty);
                ImportarLegadoViewModel _model = new ImportarLegadoViewModel();
                var destinos = legado.getDatabaseDetino();
               
                var baseDestino = destinos.Where(x => x.IdDestino == model.IdBase).Select(x => x.baseDeDestino).FirstOrDefault();
                string extensao = Path.GetExtension(arquivoExcel.FileName);
                string nomeDoArquivo = string.Empty; 

                try
                {
                    createCaminhoDosArquivosExcel(baseDestino);
                    nomeDoArquivo =  caminhoDosArquivosExcel + "\\" + arquivoExcel.FileName;
                    arquivoExcel.SaveAs(nomeDoArquivo);
                    arquivoExcel = null;

                }
                catch (UnauthorizedAccessException exAut)
                {
                    resultado = new Dictionary<string, IList<RetornoImportacao>>();

                    retornoMensagem = new List<RetornoImportacao>();
                    retornoMensagem.Add(new RetornoImportacao { mensagemImportacao = "Erro :" + exAut.Message, quantidadeDeRegistros = 0, caminhoDoArquivoDownload = string.Empty });
                    resultado.Add("Erro", retornoMensagem);

                    mensagem = Configuracao.converterObjetoParaJSON<IDictionary<string, IList<RetornoImportacao>>>(resultado);
                    return Json(mensagem, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    resultado = new Dictionary<string, IList<RetornoImportacao>>();

                    retornoMensagem = new List<RetornoImportacao>();
                    retornoMensagem.Add(new RetornoImportacao { mensagemImportacao = "Erro :" + ex.Message, quantidadeDeRegistros = 0, caminhoDoArquivoDownload = string.Empty });
                    resultado.Add("Erro", retornoMensagem);

                    mensagem = Configuracao.converterObjetoParaJSON<IDictionary<string, IList<RetornoImportacao>>>(resultado);
                    return Json(mensagem, JsonRequestBehavior.AllowGet);
                }

                 
                ImportarBanco banco = ImportarBanco.createInstance(contexto, caminhoDosArquivosExcel, converterCaminhoFisicoParaVirtual(caminhoDosArquivosExcel));


                try
                {
                   // @"C:\Projetos\Prodesp - SCPweb\Fontes\Desenvolvimento\main\SAM.Web\Legado\Arquivos\MigracaoSAMPatrimonio_SEFAZ\MigracaoSAMPatrimonio_SEFAZ0_ExcelInventario.xlsx";
                    resultado = banco.createExportacao(baseDestino, nomeDoArquivo);
                    mensagem = Configuracao.converterObjetoParaJSON<IDictionary<string, IList<RetornoImportacao>>>(resultado);
                }
                catch (Exception ex)
                {
                    resultado = new Dictionary<string, IList<RetornoImportacao>>();

                    retornoMensagem = new List<RetornoImportacao>();
                    retornoMensagem.Add(new RetornoImportacao { mensagemImportacao = "Erro :" + ex.Message, quantidadeDeRegistros = 0, caminhoDoArquivoDownload = string.Empty });
                    resultado.Add("Erro", retornoMensagem);

                    mensagem = Configuracao.converterObjetoParaJSON<IDictionary<string, IList<RetornoImportacao>>>(resultado);
                }

            }else
            {
                ModelState.AddModelError("PostFile", "Selecione a planilha do Excel.");
            }
            return Json(mensagem, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ImportarPlanilhasDeCarga(ExportarLegadoViewModel model)
        {
            IList<RetornoImportacao> retornoMensagem = new List<RetornoImportacao>();
            IDictionary<string, IList<RetornoImportacao>> resultado = new Dictionary<string, IList<RetornoImportacao>>();

            retornoMensagem.Add(new RetornoImportacao { mensagemImportacao = "<h3>Erro : Verifique as planilhas, não foi possivél realizar a importação das planilhas!</h3>", quantidadeDeRegistros = 0, caminhoDoArquivoDownload = string.Empty });
            resultado.Add("Mensagem", retornoMensagem);
            string mensagem = Configuracao.converterObjetoParaJSON<IDictionary<string, IList<RetornoImportacao>>>(resultado);


            if (this.Request.Files.Count > 0 && this.Request.Files[0].ContentLength > 0)
            {
                var arquivoExcel = this.Request.Files[0];

                legado = ImportarBanco.createInstance(contexto, string.Empty, string.Empty);
                ImportarLegadoViewModel _model = new ImportarLegadoViewModel();
                var origens = legado.getDatabaseOrigem();
                var destinos = legado.getDatabaseDetino();

                ViewBag.Origens = new SelectList(origens, "IdOrigem", "baseDeOrigem");
                ViewBag.Destinos = new SelectList(destinos, "IdDestino", "baseDeDestino");
                var baseOrigem = origens.Where(x => x.IdOrigem == model.IdBase).Select(x => x.baseDeOrigem).FirstOrDefault();
                var baseDestino = destinos.Where(x => x.IdDestino == model.IdBase).Select(x => x.baseDeDestino).FirstOrDefault();
                string extensao = Path.GetExtension(arquivoExcel.FileName);
                string nomeDoArquivo = string.Empty;

                try
                {
                    createCaminhoDosArquivosExcel(baseDestino);
                    nomeDoArquivo = caminhoDosArquivosExcel + "\\" + arquivoExcel.FileName;
                    arquivoExcel.SaveAs(nomeDoArquivo);
                    arquivoExcel = null;

                }
                catch (UnauthorizedAccessException exAut)
                {
                    resultado = new Dictionary<string, IList<RetornoImportacao>>();

                    retornoMensagem = new List<RetornoImportacao>();
                    retornoMensagem.Add(new RetornoImportacao { mensagemImportacao = "Erro :" + exAut.Message, quantidadeDeRegistros = 0, caminhoDoArquivoDownload = string.Empty });
                    resultado.Add("Erro", retornoMensagem);

                    mensagem = Configuracao.converterObjetoParaJSON<IDictionary<string, IList<RetornoImportacao>>>(resultado);
                    return Json(mensagem, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    resultado = new Dictionary<string, IList<RetornoImportacao>>();

                    retornoMensagem = new List<RetornoImportacao>();
                    retornoMensagem.Add(new RetornoImportacao { mensagemImportacao = "Erro :" + ex.Message, quantidadeDeRegistros = 0, caminhoDoArquivoDownload = string.Empty });
                    resultado.Add("Erro", retornoMensagem);

                    mensagem = Configuracao.converterObjetoParaJSON<IDictionary<string, IList<RetornoImportacao>>>(resultado);
                    return Json(mensagem, JsonRequestBehavior.AllowGet);
                }


                ImportarBanco banco = ImportarBanco.createInstance(contexto, caminhoDosArquivosExcel, converterCaminhoFisicoParaVirtual(caminhoDosArquivosExcel));


                try
                {
                    // @"C:\Projetos\Prodesp - SCPweb\Fontes\Desenvolvimento\main\SAM.Web\Legado\Arquivos\MigracaoSAMPatrimonio_SEFAZ\MigracaoSAMPatrimonio_SEFAZ0_ExcelInventario.xlsx";
                    banco.createImportacaoPlanilha(baseDestino, nomeDoArquivo);
                    mensagem = Configuracao.converterObjetoParaJSON<IDictionary<string, IList<RetornoImportacao>>>(resultado);
                }
                catch (Exception ex)
                {
                    resultado = new Dictionary<string, IList<RetornoImportacao>>();

                    retornoMensagem = new List<RetornoImportacao>();
                    retornoMensagem.Add(new RetornoImportacao { mensagemImportacao = "Erro :" + ex.Message, quantidadeDeRegistros = 0, caminhoDoArquivoDownload = string.Empty });
                    resultado.Add("Erro", retornoMensagem);

                    mensagem = Configuracao.converterObjetoParaJSON<IDictionary<string, IList<RetornoImportacao>>>(resultado);
                }

            }
            else
            {
                ModelState.AddModelError("PostFile", "Selecione a planilha do Excel.");
            }
            return Json(mensagem, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Importar(ImportarLegadoViewModel model)
        {
            legado = ImportarBanco.createInstance(contexto, string.Empty, string.Empty);
            IList<ImportarLegadoViewModel> origens;
            string baseOrigem = string.Empty;

            try
            {
                origens = legado.getDatabaseOrigemLocal();
                baseOrigem = origens.Where(x => x.IdOrigem == model.IdOrigem).Select(x => x.baseDeOrigem).FirstOrDefault();
            }
            catch (Exception e)
            {
                return Json(new { erro = "Houve um erro no processamento das bases no banco. Por favor, tente novamente mais tarde"}, JsonRequestBehavior.AllowGet);
            }

            IList<RetornoImportacao> retornoMensagem = new List<RetornoImportacao>();
            IDictionary<string, IList<RetornoImportacao>> resultado = new Dictionary<string, IList<RetornoImportacao>>();

            retornoMensagem.Add(new RetornoImportacao { mensagemImportacao = "Erro : Verifique as bases de dados, não foi possivél realizar a importação!", quantidadeDeRegistros = 0, caminhoDoArquivoDownload = string.Empty });
            resultado.Add("Mensagem", retornoMensagem);
            string mensagem = Configuracao.converterObjetoParaJSON<IDictionary<string, IList<RetornoImportacao>>>(resultado);

            if (ModelState.IsValid)
            {
                createCaminhoDosArquivosExcel(baseOrigem);

                ImportarBanco banco = ImportarBanco.createInstance(contexto, caminhoDosArquivosExcel, converterCaminhoFisicoParaVirtual(caminhoDosArquivosExcel));
                

                try
                {
                    resultado = banco.createExcel(baseOrigem);
                    mensagem = Configuracao.converterObjetoParaJSON<IDictionary<string, IList<RetornoImportacao>>>(resultado);
                }
                catch (Exception ex)
                {
                    resultado = new Dictionary<string, IList<RetornoImportacao>>();

                    retornoMensagem = new List<RetornoImportacao>();
                    retornoMensagem.Add(new RetornoImportacao { mensagemImportacao = "Erro :" + ex.Message, quantidadeDeRegistros = 0, caminhoDoArquivoDownload = string.Empty });
                    resultado.Add("Erro", retornoMensagem);

                    mensagem = Configuracao.converterObjetoParaJSON<IDictionary<string, IList<RetornoImportacao>>>(resultado);
                }
            }

            return Json(mensagem, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ImportarExcel(ImportarLegadoViewModel model)
        {

            legado = ImportarBanco.createInstance(contexto, string.Empty, string.Empty);
            ImportarLegadoViewModel _model = new ImportarLegadoViewModel();
            var origens = legado.getDatabaseOrigem();
            var destinos = legado.getDatabaseDetino();
            IList<RetornoImportacao> retornoMensagem = new List<RetornoImportacao>();
            IDictionary<string, IList<RetornoImportacao>> resultado = new Dictionary<string, IList<RetornoImportacao>>();

            retornoMensagem.Add(new RetornoImportacao { mensagemImportacao = "Erro : Verifique as bases de dados, não foi possivél realizar a importação!", quantidadeDeRegistros = 0, caminhoDoArquivoDownload = string.Empty });
            resultado.Add("Mensagem", retornoMensagem);
            string mensagem = Configuracao.converterObjetoParaJSON<IDictionary<string, IList<RetornoImportacao>>>(resultado);

            ViewBag.Origens = new SelectList(origens, "IdOrigem", "baseDeOrigem");
            ViewBag.Destinos = new SelectList(destinos, "IdDestino", "baseDeDestino");

            if (ModelState.IsValid)
            {
                var baseOrigem = origens.Where(x => x.IdOrigem == model.IdOrigem).Select(x => x.baseDeOrigem).FirstOrDefault();
                var baseDestino = destinos.Where(x => x.IdDestino == model.IdDestino).Select(x => x.baseDeDestino).FirstOrDefault();

                createCaminhoDosArquivosExcel(baseOrigem);

                ImportarBanco banco = ImportarBanco.createInstance(contexto, caminhoDosArquivosExcel, converterCaminhoFisicoParaVirtual(caminhoDosArquivosExcel));

              
                

                try
                {
                    string filePath = @"C:\Projetos\Prodesp - SCPweb\Fontes\Desenvolvimento\main\SAM.Web\Legado\Arquivos\MigracaoSAMPatrimonio_SEFAZ\MigracaoSAMPatrimonio_SEFAZ0_ExcelInventario.xlsx";
                    resultado = banco.executarImportacao(baseOrigem, baseDestino);
                    resultado = banco.createExportacao(baseDestino, filePath); //banco.createImportacao(baseOrigem, baseDestino);
                    mensagem = Configuracao.converterObjetoParaJSON<IDictionary<string, IList<RetornoImportacao>>>(resultado);
                }
                catch(Exception ex)
                {
                    resultado = new Dictionary<string, IList<RetornoImportacao>>();

                    retornoMensagem = new List<RetornoImportacao>();
                    retornoMensagem.Add(new RetornoImportacao { mensagemImportacao = "Erro :" + ex.Message, quantidadeDeRegistros = 0, caminhoDoArquivoDownload = string.Empty });
                    resultado.Add("Erro", retornoMensagem);

                    mensagem = Configuracao.converterObjetoParaJSON<IDictionary<string, IList<RetornoImportacao>>>(resultado);
                }
            }

            return Json(mensagem, JsonRequestBehavior.AllowGet);
        }

        private void createCaminhoDosArquivosExcel(String baseOrigem)
        {
            string caminhoRelativo = Request.Path.Replace("Importar", "Arquivos").Replace("ExportarExcel","Exportar") + "/" + baseOrigem;
            caminhoDosArquivosExcel = HttpContext.Server.MapPath(caminhoRelativo);
            if (!Directory.Exists(caminhoDosArquivosExcel))
            {
                Directory.CreateDirectory(caminhoDosArquivosExcel);
            }
           
        }

        private void createCaminhoDosArquivosJson(int orgaoId)
        {
            string caminhoRelativo = Request.Path.Replace("Exportar", "Arquivos") + "/Json/" + orgaoId;
            caminhoDosArquivosJson = HttpContext.Server.MapPath(caminhoRelativo);
            if (!Directory.Exists(caminhoDosArquivosJson))
            {
                Directory.CreateDirectory(caminhoDosArquivosJson);
            }

        }

        private string converterCaminhoFisicoParaVirtual(string caminhoFisico)
        {
            string caminhoVirtual = "/Patrimonio/" + caminhoFisico.Substring(HttpContext.Request.PhysicalApplicationPath.Length).Replace("\\", "/");

            return caminhoVirtual;
        }
    }
}