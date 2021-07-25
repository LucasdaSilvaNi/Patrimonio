using Sam.Domain.Entity;
using SAM.Web.Common.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAM.Web.Common
{
    public class EventoSiafem
    {
        public EventoSiafemEntity RetornaParametrosEventoSiafem(int? TipoMovimento)
        {
            EventoSiafemEntity EventoSiafem = new EventoSiafemEntity();
            switch (TipoMovimento)
            {
                #region Tipos de Incorporação

                case (int)EnumMovimentType.IncorpAnimaisPesquisaSememPeixe:

                    EventoSiafem.EventoEstoque = "ESTOQUE LONGO PRAZO";
                    EventoSiafem.EventoTipoEntradaSaidaReclassificacaoDepreciacao = "ANIMAIS PESQUISA / SEMEM / PEIXE - PAT.";
                    EventoSiafem.EventoTipoEstoque = "PERMANENTE";
                    EventoSiafem.EventoTipoMaterial = "MaterialPermanente";
                    EventoSiafem.EventoTipoMovimentacao = "PATRIMÔNIO - ANIMAIS/SEMEM/PEIXE";
                    EventoSiafem.EventoTipoMovimento = "ENTRADA";

                    break;

                case (int)EnumMovimentType.IncorpComodatoConcedidoBensMoveis:

                    EventoSiafem.EventoEstoque = "ESTOQUE LONGO PRAZO";
                    EventoSiafem.EventoTipoEntradaSaidaReclassificacaoDepreciacao = "COMODATO - CONCEDIDOS - BENS MÓVEIS";
                    EventoSiafem.EventoTipoEstoque = "PERMANENTE";
                    EventoSiafem.EventoTipoMaterial = "MaterialPermanente";
                    EventoSiafem.EventoTipoMovimentacao = "PATRIMÔNIO - BENS MÓVEIS";
                    EventoSiafem.EventoTipoMovimento = "ENTRADA";

                    break;

                case (int)EnumMovimentType.IncorpComodatoDeTerceirosRecebidos:

                    EventoSiafem.EventoEstoque = "ESTOQUE LONGO PRAZO";
                    EventoSiafem.EventoTipoEntradaSaidaReclassificacaoDepreciacao = "COMODATO/DE TERCEIROS - RECEBIDOS";
                    EventoSiafem.EventoTipoEstoque = "PERMANENTE";
                    EventoSiafem.EventoTipoMaterial = "MaterialPermanente";
                    EventoSiafem.EventoTipoMovimentacao = "COMODATO/DE TERCEIROS - RECEBIDOS";
                    EventoSiafem.EventoTipoMovimento = "ENTRADA";

                    break;

                case (int)EnumMovimentType.IncorpConfiscoBensMoveis:

                    EventoSiafem.EventoEstoque = "ESTOQUE LONGO PRAZO";
                    EventoSiafem.EventoTipoEntradaSaidaReclassificacaoDepreciacao = "CONFISCO - BENS MÓVEIS";
                    EventoSiafem.EventoTipoEstoque = "PERMANENTE";
                    EventoSiafem.EventoTipoMaterial = "MaterialPermanente";
                    EventoSiafem.EventoTipoMovimentacao = "PATRIMÔNIO - BENS MÓVEIS";
                    EventoSiafem.EventoTipoMovimento = "ENTRADA";

                    break;

                case (int)EnumMovimentType.IncorpDoacaoConsolidacao:

                    EventoSiafem.EventoEstoque = "ESTOQUE LONGO PRAZO";
                    EventoSiafem.EventoTipoEntradaSaidaReclassificacaoDepreciacao = "DOAÇÃO CONSOLIDAÇÃO";
                    EventoSiafem.EventoTipoEstoque = "PERMANENTE";
                    EventoSiafem.EventoTipoMaterial = "MaterialPermanente";
                    EventoSiafem.EventoTipoMovimentacao = "PATRIMÔNIO - BENS MÓVEIS";
                    EventoSiafem.EventoTipoMovimento = "ENTRADA";

                    break;

                case (int)EnumMovimentType.IncorpDoacaoIntraNoEstado:

                    EventoSiafem.EventoEstoque = "ESTOQUE LONGO PRAZO";
                    EventoSiafem.EventoTipoEntradaSaidaReclassificacaoDepreciacao = "DOAÇÃO INTRA - NO ESTADO";
                    EventoSiafem.EventoTipoEstoque = "PERMANENTE";
                    EventoSiafem.EventoTipoMaterial = "MaterialPermanente";
                    EventoSiafem.EventoTipoMovimentacao = "PATRIMÔNIO - BENS MÓVEIS";
                    EventoSiafem.EventoTipoMovimento = "ENTRADA";

                    break;

                case (int)EnumMovimentType.IncorpDoacaoMunicipio:

                    EventoSiafem.EventoEstoque = "ESTOQUE LONGO PRAZO";
                    EventoSiafem.EventoTipoEntradaSaidaReclassificacaoDepreciacao = "DOAÇÃO MUNICÍPIO";
                    EventoSiafem.EventoTipoEstoque = "PERMANENTE";
                    EventoSiafem.EventoTipoMaterial = "MaterialPermanente";
                    EventoSiafem.EventoTipoMovimentacao = "PATRIMÔNIO - BENS MÓVEIS";
                    EventoSiafem.EventoTipoMovimento = "ENTRADA";

                    break;

                case (int)EnumMovimentType.IncorpDoacaoOutrosEstados:

                    EventoSiafem.EventoEstoque = "ESTOQUE LONGO PRAZO";
                    EventoSiafem.EventoTipoEntradaSaidaReclassificacaoDepreciacao = "DOAÇÃO OUTROS ESTADOS";
                    EventoSiafem.EventoTipoEstoque = "PERMANENTE";
                    EventoSiafem.EventoTipoMaterial = "MaterialPermanente";
                    EventoSiafem.EventoTipoMovimentacao = "PATRIMÔNIO - BENS MÓVEIS";
                    EventoSiafem.EventoTipoMovimento = "ENTRADA";

                    break;

                case (int)EnumMovimentType.IncorpDoacaoUniao:

                    EventoSiafem.EventoEstoque = "ESTOQUE LONGO PRAZO";
                    EventoSiafem.EventoTipoEntradaSaidaReclassificacaoDepreciacao = "DOAÇÃO UNIÃO";
                    EventoSiafem.EventoTipoEstoque = "PERMANENTE";
                    EventoSiafem.EventoTipoMaterial = "MaterialPermanente";
                    EventoSiafem.EventoTipoMovimentacao = "PATRIMÔNIO - BENS MÓVEIS";
                    EventoSiafem.EventoTipoMovimento = "ENTRADA";

                    break;

                case (int)EnumMovimentType.IncorpVegetal:

                    EventoSiafem.EventoEstoque = "ESTOQUE LONGO PRAZO";
                    EventoSiafem.EventoTipoEntradaSaidaReclassificacaoDepreciacao = "VEGETAL";
                    EventoSiafem.EventoTipoEstoque = "PERMANENTE";
                    EventoSiafem.EventoTipoMaterial = "MaterialPermanente";
                    EventoSiafem.EventoTipoMovimentacao = "PATRIMÔNIO - ÁRVORE";
                    EventoSiafem.EventoTipoMovimento = "ENTRADA";

                    break;

                case (int)EnumMovimentType.IncorpMudancaDeCategoriaRevalorizacao:

                    EventoSiafem.EventoEstoque = "ESTOQUE LONGO PRAZO";
                    EventoSiafem.EventoTipoEntradaSaidaReclassificacaoDepreciacao = "MUDANÇA DE CATEGORIA / REVALORIZAÇÃO";
                    EventoSiafem.EventoTipoEstoque = "PERMANENTE";
                    EventoSiafem.EventoTipoMaterial = "MaterialPermanente";
                    EventoSiafem.EventoTipoMovimentacao = "PATRIMÔNIO - ANIMAIS/SEMEM/PEIXE";
                    EventoSiafem.EventoTipoMovimento = "ENTRADA";

                    break;

                case (int)EnumMovimentType.IncorpNascimentoDeAnimais:

                    EventoSiafem.EventoEstoque = "ESTOQUE LONGO PRAZO";
                    EventoSiafem.EventoTipoEntradaSaidaReclassificacaoDepreciacao = "NASCIMENTO ANIMAIS";
                    EventoSiafem.EventoTipoEstoque = "PERMANENTE";
                    EventoSiafem.EventoTipoMaterial = "MaterialPermanente";
                    EventoSiafem.EventoTipoMovimentacao = "PATRIMÔNIO - ANIMAIS/SEMEM/PEIXE";
                    EventoSiafem.EventoTipoMovimento = "ENTRADA";

                    break;

                case (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGEDoacao:

                    EventoSiafem.EventoEstoque = "ESTOQUE LONGO PRAZO";
                    EventoSiafem.EventoTipoEntradaSaidaReclassificacaoDepreciacao = "RECEBIMENTO INSERVÍVEL DA UGE - DOAÇÃO";
                    EventoSiafem.EventoTipoEstoque = "PERMANENTE";
                    EventoSiafem.EventoTipoMaterial = "MaterialPermanente";
                    EventoSiafem.EventoTipoMovimentacao = "INSERVÍVEL NA UGE - BENS MÓVEIS";
                    EventoSiafem.EventoTipoMovimento = "ENTRADA";

                    break;

                case (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGETranferencia:

                    EventoSiafem.EventoEstoque = "ESTOQUE LONGO PRAZO";
                    EventoSiafem.EventoTipoEntradaSaidaReclassificacaoDepreciacao = "RECEBIMENTO INSERVÍVEL DA UGE - TRANSFERÊNCIA";
                    EventoSiafem.EventoTipoEstoque = "PERMANENTE";
                    EventoSiafem.EventoTipoMaterial = "MaterialPermanente";
                    EventoSiafem.EventoTipoMovimentacao = "INSERVÍVEL NA UGE - BENS MÓVEIS";
                    EventoSiafem.EventoTipoMovimento = "ENTRADA";

                    break;

                case (int)EnumMovimentType.IncorpTransferenciaMesmoOrgaoPatrimoniado:

                    EventoSiafem.EventoEstoque = "ESTOQUE LONGO PRAZO";
                    EventoSiafem.EventoTipoEntradaSaidaReclassificacaoDepreciacao = "TRANSFERÊNCIA MESMO ÓRGÃO - PATRIMONIADO";
                    EventoSiafem.EventoTipoEstoque = "PERMANENTE";
                    EventoSiafem.EventoTipoMaterial = "MaterialPermanente";
                    EventoSiafem.EventoTipoMovimentacao = "PATRIMÔNIO - BENS MÓVEIS";
                    EventoSiafem.EventoTipoMovimento = "ENTRADA";

                    break;

                case (int)EnumMovimentType.IncorpTransferenciaOutroOrgaoPatrimoniado:

                    EventoSiafem.EventoEstoque = "ESTOQUE LONGO PRAZO";
                    EventoSiafem.EventoTipoEntradaSaidaReclassificacaoDepreciacao = "TRANSFERÊNCIA OUTRO ÓRGÃO - PATRIMONIADO";
                    EventoSiafem.EventoTipoEstoque = "PERMANENTE";
                    EventoSiafem.EventoTipoMaterial = "MaterialPermanente";
                    EventoSiafem.EventoTipoMovimentacao = "PATRIMÔNIO - BENS MÓVEIS";
                    EventoSiafem.EventoTipoMovimento = "ENTRADA";

                    break;

                #endregion

                #region Tipo de Movimentação

                case (int)EnumMovimentType.MovSaidaInservivelUGETransferencia:

                    EventoSiafem.EventoEstoque = "ESTOQUE LONGO PRAZO";
                    EventoSiafem.EventoTipoEntradaSaidaReclassificacaoDepreciacao = "SAÍDA INSERVÍVEL DA UGE - TRANSFERÊNCIA";
                    EventoSiafem.EventoTipoEstoque = "PERMANENTE";
                    EventoSiafem.EventoTipoMaterial = "MaterialPermanente";
                    EventoSiafem.EventoTipoMovimentacao = "PATRIMÔNIO - BENS MÓVEIS";
                    EventoSiafem.EventoTipoMovimento = "SAÍDA";

                    break;

                case (int)EnumMovimentType.MovSaidaInservivelUGEDoacao:

                    EventoSiafem.EventoEstoque = "ESTOQUE LONGO PRAZO";
                    EventoSiafem.EventoTipoEntradaSaidaReclassificacaoDepreciacao = "SAÍDA INSERVIVEL DA UGE - DOAÇÃO";
                    EventoSiafem.EventoTipoEstoque = "PERMANENTE";
                    EventoSiafem.EventoTipoMaterial = "MaterialPermanente";
                    EventoSiafem.EventoTipoMovimentacao = "PATRIMÔNIO - BENS MÓVEIS";
                    EventoSiafem.EventoTipoMovimento = "SAÍDA";

                    break;

                case (int)EnumMovimentType.MovComodatoConcedidoBensMoveis:

                    EventoSiafem.EventoEstoque = "ESTOQUE LONGO PRAZO";
                    EventoSiafem.EventoTipoEntradaSaidaReclassificacaoDepreciacao = "COMODATO - CONCEDIDOS - BENS MÓVEIS";
                    EventoSiafem.EventoTipoEstoque = "PERMANENTE";
                    EventoSiafem.EventoTipoMaterial = "MaterialPermanente";
                    EventoSiafem.EventoTipoMovimentacao = "PATRIMÔNIO - BENS MÓVEIS";
                    EventoSiafem.EventoTipoMovimento = "SAÍDA";

                    break;

                case (int)EnumMovimentType.MovComodatoTerceirosRecebidos:

                    EventoSiafem.EventoEstoque = "ESTOQUE LONGO PRAZO";
                    EventoSiafem.EventoTipoEntradaSaidaReclassificacaoDepreciacao = "COMODATO/DE TERCEIROS - RECEBIDOS";
                    EventoSiafem.EventoTipoEstoque = "PERMANENTE";
                    EventoSiafem.EventoTipoMaterial = "MaterialPermanente";
                    EventoSiafem.EventoTipoMovimentacao = "COMODATO/DE TERCEIROS - RECEBIDOS";
                    EventoSiafem.EventoTipoMovimento = "SAÍDA";

                    break;

                case (int)EnumMovimentType.MovDoacaoConsolidacao:

                    EventoSiafem.EventoEstoque = "ESTOQUE LONGO PRAZO";
                    EventoSiafem.EventoTipoEntradaSaidaReclassificacaoDepreciacao = "DOAÇÃO CONSOLIDAÇÃO";
                    EventoSiafem.EventoTipoEstoque = "PERMANENTE";
                    EventoSiafem.EventoTipoMaterial = "MaterialPermanente";
                    EventoSiafem.EventoTipoMovimentacao = "PATRIMÔNIO - BENS MÓVEIS";
                    EventoSiafem.EventoTipoMovimento = "SAÍDA";

                    break;

                case (int)EnumMovimentType.MovDoacaoIntraNoEstado:

                    EventoSiafem.EventoEstoque = "ESTOQUE LONGO PRAZO";
                    EventoSiafem.EventoTipoEntradaSaidaReclassificacaoDepreciacao = "DOAÇÃO INTRA - NO ESTADO";
                    EventoSiafem.EventoTipoEstoque = "PERMANENTE";
                    EventoSiafem.EventoTipoMaterial = "MaterialPermanente";
                    EventoSiafem.EventoTipoMovimentacao = "PATRIMÔNIO - BENS MÓVEIS";
                    EventoSiafem.EventoTipoMovimento = "SAÍDA";

                    break;

                case (int)EnumMovimentType.MovDoacaoMunicipio:

                    EventoSiafem.EventoEstoque = "ESTOQUE LONGO PRAZO";
                    EventoSiafem.EventoTipoEntradaSaidaReclassificacaoDepreciacao = "DOAÇÃO MUNICÍPIO";
                    EventoSiafem.EventoTipoEstoque = "PERMANENTE";
                    EventoSiafem.EventoTipoMaterial = "MaterialPermanente";
                    EventoSiafem.EventoTipoMovimentacao = "PATRIMÔNIO - BENS MÓVEIS";
                    EventoSiafem.EventoTipoMovimento = "SAÍDA";

                    break;

                case (int)EnumMovimentType.MovDoacaoOutrosEstados:

                    EventoSiafem.EventoEstoque = "ESTOQUE LONGO PRAZO";
                    EventoSiafem.EventoTipoEntradaSaidaReclassificacaoDepreciacao = "DOAÇÃO OUTROS ESTADOS";
                    EventoSiafem.EventoTipoEstoque = "PERMANENTE";
                    EventoSiafem.EventoTipoMaterial = "MaterialPermanente";
                    EventoSiafem.EventoTipoMovimentacao = "PATRIMÔNIO - BENS MÓVEIS";
                    EventoSiafem.EventoTipoMovimento = "SAÍDA";

                    break;

                case (int)EnumMovimentType.MovDoacaoUniao:

                    EventoSiafem.EventoEstoque = "ESTOQUE LONGO PRAZO";
                    EventoSiafem.EventoTipoEntradaSaidaReclassificacaoDepreciacao = "DOAÇÃO UNIÃO";
                    EventoSiafem.EventoTipoEstoque = "PERMANENTE";
                    EventoSiafem.EventoTipoMaterial = "MaterialPermanente";
                    EventoSiafem.EventoTipoMovimentacao = "PATRIMÔNIO - BENS MÓVEIS";
                    EventoSiafem.EventoTipoMovimento = "SAÍDA";

                    break;

                case (int)EnumMovimentType.MovExtravioFurtoRouboBensMoveis:

                    EventoSiafem.EventoEstoque = "ESTOQUE LONGO PRAZO";
                    EventoSiafem.EventoTipoEntradaSaidaReclassificacaoDepreciacao = "EXTRAVIO, FURTO, ROUBO - BENS MOVEIS";
                    EventoSiafem.EventoTipoEstoque = "PERMANENTE";
                    EventoSiafem.EventoTipoMaterial = "MaterialPermanente";
                    EventoSiafem.EventoTipoMovimentacao = "PATRIMÔNIO - BENS MÓVEIS";
                    EventoSiafem.EventoTipoMovimento = "SAÍDA";

                    break;

                case (int)EnumMovimentType.MovPerdaInvoluntariaBensMoveis:

                    EventoSiafem.EventoEstoque = "ESTOQUE LONGO PRAZO";
                    EventoSiafem.EventoTipoEntradaSaidaReclassificacaoDepreciacao = "PERDAS INVOLUNTÁRIAS - BENS MÓVEIS";
                    EventoSiafem.EventoTipoEstoque = "PERMANENTE";
                    EventoSiafem.EventoTipoMaterial = "MaterialPermanente";
                    EventoSiafem.EventoTipoMovimentacao = "PATRIMÔNIO - BENS MÓVEIS";
                    EventoSiafem.EventoTipoMovimento = "SAÍDA";

                    break;

                case (int)EnumMovimentType.MovPerdaInvoluntariaInservivelBensMoveis:

                    EventoSiafem.EventoEstoque = "ESTOQUE LONGO PRAZO";
                    EventoSiafem.EventoTipoEntradaSaidaReclassificacaoDepreciacao = "PERDAS INVOLUNTÁRIAS - INSERVÍVEL BENS MÓVEIS";
                    EventoSiafem.EventoTipoEstoque = "PERMANENTE";
                    EventoSiafem.EventoTipoMaterial = "MaterialPermanente";
                    EventoSiafem.EventoTipoMovimentacao = "INSERVÍVEL NA UGE - BENS MÓVEIS";
                    EventoSiafem.EventoTipoMovimento = "SAÍDA";

                    break;

                case (int)EnumMovimentType.MovMorteAnimalPatrimoniado:

                    EventoSiafem.EventoEstoque = "ESTOQUE LONGO PRAZO";
                    EventoSiafem.EventoTipoEntradaSaidaReclassificacaoDepreciacao = "MORTE ANIMAL - PATRIMONIADO";
                    EventoSiafem.EventoTipoEstoque = "PERMANENTE";
                    EventoSiafem.EventoTipoMaterial = "MaterialPermanente";
                    EventoSiafem.EventoTipoMovimentacao = "PATRIMÔNIO - ANIMAIS/SEMEM/PEIXE";
                    EventoSiafem.EventoTipoMovimento = "SAÍDA";

                    break;

                case (int)EnumMovimentType.MovMorteVegetalPatrimoniado:

                    EventoSiafem.EventoEstoque = "ESTOQUE LONGO PRAZO";
                    EventoSiafem.EventoTipoEntradaSaidaReclassificacaoDepreciacao = "MORTE VEGETAL - PATRIMONIADO";
                    EventoSiafem.EventoTipoEstoque = "PERMANENTE";
                    EventoSiafem.EventoTipoMaterial = "MaterialPermanente";
                    EventoSiafem.EventoTipoMovimentacao = "PATRIMÔNIO - ÁRVORE";
                    EventoSiafem.EventoTipoMovimento = "SAÍDA";

                    break;

                case (int)EnumMovimentType.MovMudancaCategoriaDesvalorizacao:

                    EventoSiafem.EventoEstoque = "ESTOQUE LONGO PRAZO";
                    EventoSiafem.EventoTipoEntradaSaidaReclassificacaoDepreciacao = "MUDANÇA DE CATEGORIA / DESVALORIZAÇÃO";
                    EventoSiafem.EventoTipoEstoque = "PERMANENTE";
                    EventoSiafem.EventoTipoMaterial = "MaterialPermanente";
                    EventoSiafem.EventoTipoMovimentacao = "PATRIMÔNIO - ANIMAIS/SEMEM/PEIXE";
                    EventoSiafem.EventoTipoMovimento = "SAÍDA";

                    break;

                case (int)EnumMovimentType.MovSementesPlantasInsumosArvores:

                    EventoSiafem.EventoEstoque = "ESTOQUE LONGO PRAZO";
                    EventoSiafem.EventoTipoEntradaSaidaReclassificacaoDepreciacao = "SEMENTES, PLANTAS, INSUMOS E ARVORES";
                    EventoSiafem.EventoTipoEstoque = "PERMANENTE";
                    EventoSiafem.EventoTipoMaterial = "MaterialPermanente";
                    EventoSiafem.EventoTipoMovimentacao = "PATRIMÔNIO - ÁRVORE";
                    EventoSiafem.EventoTipoMovimento = "SAÍDA";

                    break;

                case (int)EnumMovimentType.MovTransferenciaOutroOrgaoPatrimoniado:

                    EventoSiafem.EventoEstoque = "ESTOQUE LONGO PRAZO";
                    EventoSiafem.EventoTipoEntradaSaidaReclassificacaoDepreciacao = "TRANSFERÊNCIA OUTRO ORGÃO - PATRIMÔNIADO";
                    EventoSiafem.EventoTipoEstoque = "PERMANENTE";
                    EventoSiafem.EventoTipoMaterial = "MaterialPermanente";
                    EventoSiafem.EventoTipoMovimentacao = "PATRIMÔNIO - BENS MÓVEIS";
                    EventoSiafem.EventoTipoMovimento = "SAÍDA";

                    break;

                case (int)EnumMovimentType.MovTransferenciaMesmoOrgaoPatrimoniado:

                    EventoSiafem.EventoEstoque = "ESTOQUE LONGO PRAZO";
                    EventoSiafem.EventoTipoEntradaSaidaReclassificacaoDepreciacao = "TRANSFERÊNCIA MESMO ORGÃO - PATRIMÔNIADO";
                    EventoSiafem.EventoTipoEstoque = "PERMANENTE";
                    EventoSiafem.EventoTipoMaterial = "MaterialPermanente";
                    EventoSiafem.EventoTipoMovimentacao = "PATRIMÔNIO - BENS MÓVEIS";
                    EventoSiafem.EventoTipoMovimento = "SAÍDA";

                    break;

                case (int)EnumMovimentType.MovVendaLeilaoSemoventes:

                    EventoSiafem.EventoEstoque = "ESTOQUE LONGO PRAZO";
                    EventoSiafem.EventoTipoEntradaSaidaReclassificacaoDepreciacao = "VENDA/LEILÃO - SEMOVENTES";
                    EventoSiafem.EventoTipoEstoque = "PERMANENTE";
                    EventoSiafem.EventoTipoMaterial = "MaterialPermanente";
                    EventoSiafem.EventoTipoMovimentacao = "PATRIMÔNIO - ANIMAIS/SEMEM/PEIXES";
                    EventoSiafem.EventoTipoMovimento = "SAÍDA";

                    break;

                    #endregion
            }

            return EventoSiafem;
        }
    }
}