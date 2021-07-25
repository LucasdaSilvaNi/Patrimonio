using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Sam.Common;
using Sam.Common.Util;
using Sam.Integracao.SIAF.Core;
using Sam.Integracao.SIAF.Configuracao;
using SAM.Web.Context;
using SAM.Web.Models;
using SAM.Web.ViewModels;

namespace SAM.Web.Common
{
    public class BuscaItensMateriais : Controller
    {
        ItemMaterialContext db = new ItemMaterialContext();

        public MaterialItemViewModel GetItemMaterial(string materialItemCode, ref string msgErroSIAFISICO)
        {
            try
            {

                float codigo = Convert.ToInt64(materialItemCode);
                //Código de itens materiais reservados para acervo e terceiro
                if (codigo == 5628121 || codigo == 5628156)
                {
                    return null;
                }

                MaterialItemViewModel materialItem = GetMaterialItemFromSIAFISICO(materialItemCode.Trim());

                if (materialItem == null)
                {
                    msgErroSIAFISICO = TempData["Mensagens"].ToString();
                    return null;
                }

                materialItem = ValidaRetornoSIAFISICO(materialItem);

                if (materialItem == null)
                    msgErroSIAFISICO = TempData["Mensagens"].ToString();

                return materialItem;
            }
            catch (Exception e) {
                msgErroSIAFISICO = "Houve erro ao processar no SIAFISICO. Por favor, tente novamente mais tarde!";
                return null;
            }
        }

        [HttpGet]
        public MaterialItemViewModel GetItemMaterialBD(string materialItemCode)
        {
            float codigo = Convert.ToInt64(materialItemCode);

            // Pesquisar somente os itens com status D - Deletado e I - Inativo na tabela Item_Siafisico do banco
            ItemSiafisico ItemSiafisico = (from m in db.ItemSiafisicos where m.Cod_Item_Mat == codigo select m).FirstOrDefault();

            MaterialItemViewModel materialItem = new MaterialItemViewModel
                {
                    MaterialGroupCode = (int)ItemSiafisico.Cod_Grupo,
                    MaterialGroupDescription = ItemSiafisico.Nome_Grupo,
                    MaterialItemCode = ItemSiafisico.Cod_Item_Mat.ToString(),
                    Code = (int)ItemSiafisico.Cod_Item_Mat,
                    Description = ItemSiafisico.Nome_Item_Mat,
                    Material = ItemSiafisico.Nome_Material,
                    Natureza1 = "4490"
                };

            MaterialGroup _materialgroup = GetMaterialGroup(materialItem.MaterialGroupCode);

            materialItem.MaterialGroupDescription = _materialgroup.Description;
            materialItem.LifeCycle = _materialgroup.LifeCycle;
            materialItem.ResidualValue = _materialgroup.ResidualValue;
            materialItem.RateDepreciationMonthly = _materialgroup.RateDepreciationMonthly;
            
            return materialItem;
        }

        public string BuscaMensagemValidaCodigoItemMaterial(string materialItemCode)
        {
            float codigo = Convert.ToInt64(materialItemCode);

            //Código de itens materiais reservados para acervo e terceiro
            if (codigo == 5628121 || codigo == 5628156)
            {
                return "Código de Item Material inválido para essa operação";
            }

            // Pesquisar somente os itens com status D - Deletado e I - Inativo na tabela Item_Siafisico do banco
            ItemSiafisico ItemSiafisico = (from m in db.ItemSiafisicos where m.Cod_Item_Mat == codigo select m).FirstOrDefault();

            MaterialItemViewModel materialItem = null;

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
            else {
                return "O código do Item Material está nulo no SIAFISICO, favor entrar em contato com o suporte.";
            }

            if (materialItem.Description == null)
            {
                return "A Descrição do Item Material está nulo no SIAFISICO, favor entrar em contato com o suporte.";
            }
            if (materialItem.MaterialItemCode == null)
            {
                return "O código do Item Material está nulo no SIAFISICO, favor entrar em contato com o suporte.";
            }
            if (!materialItem.Natureza1.Substring(0, 4).Contains("4490") && !materialItem.Natureza2.Substring(0, 4).Contains("4490") && !materialItem.Natureza3.Substring(0, 4).Contains("4490"))
            {
                return "O Item Material informado não é um tipo de Despesa de Bem Permanente, favor verificar!";
            }
            MaterialGroup _materialgroup = GetMaterialGroup(materialItem.MaterialGroupCode);
            if (_materialgroup == null)
            {
                return "O Grupo do Material selecionado não está cadastrado no sistema, favor entrar em contato com a Administração.";
            }
            if (materialItem.Material == null)
            {
                return "A Descrição Resumida do Item Material está nulo no SIAFISICO, favor entrar em contato com o suporte.";
            }

            return String.Empty;
        }

        public MaterialItemViewModel ValidaRetornoSIAFISICO(MaterialItemViewModel materialItem)
        {
            MensagemModel mensagem = new MensagemModel();
            mensagem.Id = 0;
            mensagem.Mensagem = "";

            if (TempData["Mensagens"].IsNotNull())
            {
                return null;
            }
            if (materialItem.Description.IsNullOrEmpty())
            {
                TempData["Mensagens"] = "Item Material não encontrado, favor entrar em contato com o suporte.";
                return null;
            }

            if (((MaterialItemViewModel)materialItem).Description == null)
            {
                TempData["Mensagens"] = "A Descrição do Item Material está nulo no SIAFISICO, favor entrar em contato com o suporte.";
                return null;
            }
            if (((MaterialItemViewModel)materialItem).MaterialItemCode == null)
            {
                TempData["Mensagens"] = "O código do Item Material está nulo no SIAFISICO, favor entrar em contato com o suporte.";
                return null;
            }
            if (!((MaterialItemViewModel)materialItem).Natureza1.Substring(0, 4).Contains("4490") && !((MaterialItemViewModel)materialItem).Natureza2.Substring(0, 4).Contains("4490") && !((MaterialItemViewModel)materialItem).Natureza3.Substring(0, 4).Contains("4490"))
            {
                TempData["Mensagens"] = "O Item Material informado não é um tipo de Despesa de Bem Permanente, favor verificar!";
                return null;
            }

            MaterialGroup _materialgrupo = (from a in db.MaterialGroups where a.Code == ((MaterialItemViewModel)materialItem).MaterialGroupCode select a).FirstOrDefault();
            if (_materialgrupo == null)
            {
                TempData["Mensagens"] = "O Grupo do Material selecionado não está cadastrado no sistema, favor entrar em contato com a Administração.";
                return null;
            }
            if (((MaterialItemViewModel)materialItem).Material == null)
            {
                TempData["Mensagens"] = "A Descrição Resumida do Item Material está nulo no SIAFISICO, favor entrar em contato com o suporte.";
                return null;
            }

            MaterialGroup _materialgroup = GetMaterialGroup(materialItem.MaterialGroupCode);
            
            materialItem.MaterialGroupDescription = _materialgroup.Description;
            materialItem.LifeCycle = _materialgroup.LifeCycle;
            materialItem.ResidualValue = _materialgroup.ResidualValue;
            materialItem.RateDepreciationMonthly = _materialgroup.RateDepreciationMonthly;
            

            return materialItem;
        }

        public MaterialItemViewModel GetMaterialItemFromSIAFISICO(string materialItemCode)
        {
            try
            {
                var rowMaterialItem = RecuperarCadastroItemMaterialDoSiafisico(materialItemCode);
                return rowMaterialItem;
            }
            catch (Exception excErroExecucao)
            {
                TempData["Mensagens"] = excErroExecucao.Message;
                return null;
            }
        }

        public MaterialItemViewModel RecuperarCadastroItemMaterialDoSiafisico(string materialItemCode)
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
                        Material = lStrMaterial.Replace(lStrMaterial.BreakLine(0), "").Trim(),
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
            return (from x in db.MaterialGroups where x.Code == code select x).ToList().FirstOrDefault();
        }
    }
}