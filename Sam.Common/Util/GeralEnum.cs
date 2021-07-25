using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using Sam.Common.Enums;

namespace Sam.Common.Util
{
    public class GeralEnum
    {

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public enum TipoUGE
        {
            Normal = 0,
            [Description("Contábil")]
            Contabil = 1
        }

        public enum TipoMovimentoAgrupamento
        {
            Entrada = 1,
            Saida = 2,
            Requisicao = 3,
            ConsumoImediato = 4,

            Todos = 9
        }

        public enum PerfilNivel
        {
            TipoPadao = 1,
            TipoRequisitante = 2
        }

        [Obsolete("Eliminar @SAM Fase - Etapa 2")]
        public enum TipoRequisicao
        {
            [Description("Nova")]
            Nova = 1,
            [Description("Estorno")]
            Estorno = 2,
            [Description("Nota")]
            NotaFornecimento = 3,
        }

        public enum OperacaoSaida
        {
            [Description("Nova")]
            Nova = 1,
            [Description("Estorno")]
            Estorno = 2,
            [Description("Nota")]
            NotaFornecimento = 3,
        }

        public enum OperacaoEntrada
        {
            [Description("Nova")]
            Nova = 1,
            [Description("Estorno")]
            Estorno = 2,
            [Description("Nota de Recebimento")]
            NotaRecebimento = 3,
        }

        public enum TipoMovimento
        {
            [Description("Entrada por Empenho")]
            [ExtraDescription("RECEBIMENTO DE MATERIAL PELO ALMOXARIFADO - FORNECIMENTO POR CONTRATACAO")]
            EntradaPorEmpenho = 2,
            [Description("Empenho de Serv. Volta Mat. Consumo")]
            [ExtraDescription("ENTRADA DE MATERIAL POR OUTRAS FORMAS NAO CLASSIFICADA OU AVULSA")]
            EntradaAvulsa = 5,
            [Description("Entrada por Transferência")]
            [ExtraDescription("ENTRADA DE MATERIAL POR UNIDADE QUE NAO UTILIZA O SISTEMA SAM")]
            EntradaPorTransferencia = 6,
            [Description("Doação por Orgão Implantado")]
            [ExtraDescription("ENTRADA DE MATERIAL POR DOACAO IMPLANTADO")]
            EntradaPorDoacaoImplantado = 7,
            [Description("Entrada por Devolução")]
            [ExtraDescription("DEVOLUCAO DE MATERIAL DISPONIBILIZADO PARA CONSUMO")]
            EntradaPorDevolucao = 8,
            [Description("Material Transformado")]
            [ExtraDescription("ENTRADA DE MATERIAL TRANSFORMADO")]
            EntradaPorMaterialTransformado = 9,
            [Description("Requisição Pendente")]
            [ExtraDescription("")]
            RequisicaoPendente = 10,
            
            [Description("Requisição Aprovada")]
            [ExtraDescription("SAIDA DE MATERIAL - DISPONIBILIZADO PARA CONSUMO")]
            RequisicaoAprovada = 11,
            [Description("Saída por Transferência")]
            [ExtraDescription("SAIDA DE MATERIAL - TRANSFERENCIA A OUTRAS UNIDADES")]
            SaidaPorTransferencia = 12,
            [Description("Saída por Doação")]
            [ExtraDescription("SAIDA DE MATERIAL - POR DOAÇÃO")]
            SaidaPorDoacao = 13,
            [Description("Outras Saídas")]
            [ExtraDescription("SAIDA DE MATERIAL - POR OUTROS FATOS NÃO LISTADOS.")]
            OutrasSaidas = 14,
            [Description("Requisição Finalizada")]
            [ExtraDescription("")]
            RequisicaoFinalizada = 16,
            [Description("Requisição Cancelada")]
            [ExtraDescription("")]
            RequisicaoCancelada = 17,

            [Description("Transformação")]
            [ExtraDescription("SAIDA DE MATERIAL - LEVADO A TRANSFORMACAO")]
            SaidaPorMaterialTransformado = 18,
            
            [Description("Extravio / Furto / Roubo")]
            [ExtraDescription("SAIDA DE MATERIAL - POR EXTRAVIO OU FURTO")]
            SaidaPorExtravioFurtoRoubo = 19,
            
            [Description("Incorporação Indevida")]
            [ExtraDescription("SAIDA DE MATERIAL - POR INDEVIDA INCORPORACAO")]
            SaidaPorIncorporacaoIndevida = 20,

            [Description("Perda")]
            [ExtraDescription("SAIDA DE MATERIAL - POR PERDAS")]
            SaidaPorPerda = 21,

            [Description("Inservivel / Quebra")]
            [ExtraDescription("SAIDA DE MATERIAL - INSERVIVEL OU QUEBRADOS")]
            SaidaInservivelQuebra = 22,

            [Description("Transferência para almoxarifado não implantado")]
            [ExtraDescription("SAIDA DE MATERIAL - POR UNIDADE QUE NAO UTILIZA O SISTEMA SAM")]
            SaidaPorTransferenciaParaAlmoxNaoImplantado = 23,

            [Description("Amostra / Exposição / Análise")]
            [ExtraDescription("SAIDA DE MATERIAL - PARA EXPOSICAO, AMOSTRA OU ANÁLISE")]
            SaidaParaAmostraExposicaoAnalise = 24,

            [Description("Reclassificação (transf. mesma UGE)")]
            [ExtraDescription("RECLASSIFICACAO DO MATERIAL EM ESTOQUE - PARA CONTA DE ALMOXARIFADO")]
            SaidaPorReclassificacao = 25,

            [Description("Transferência de almoxarifado não implantado")]
            [ExtraDescription("ENTRADA DE MATERIAL - POR UNIDADE QUE NAO UTILIZA O SISTEMA SAM")]
            EntradaPorTransferenciaDeAlmoxNaoImplantado = 26,

            [Description("Restos a Pagar")]
            [ExtraDescription("RECEBIMENTO DE MATERIAL PELO ALMOXARIFADO - FORNECIMENTO POR RESTOS A PAGAR")]
            EntradaPorRestosAPagar = 27,

            [Description("Entrada Inventário")]
            [ExtraDescription("ENTRADA DE MATERIAL POR INVENTARIO")]
            EntradaInventario = 28,
                        
            [Description("Entrada Por Doação")]
            [ExtraDescription("ENTRADA DE MATERIAL POR DOACAO")]
            EntradaPorDoacao = 29,


            [Description("Consumo por Empenho")]
            [ExtraDescription("CONSUMO IMEDIATO EMPENHO")]
            ConsumoImediatoEmpenho = 30,

            [Description("Consumo por Empenho - Restos a Pagar")]
            [ExtraDescription("CONSUMO IMEDIATO EMPENHO - FORNECIMENTO POR RESTOS A PAGAR")]
            EntradaPorRestosAPagarConsumoImediatoEmpenho = 31,

            [Description("Todos")]
            [ExtraDescription("TODOS")]
            Todos = 99
        }

        public enum EmpenhoEvento
        {
            [Description("EMPENHO DE DOTACAO RESERVADA")]
            DotacaoReservada = 400051,
            [Description("EMPENHO DA DESPESA")]
            Despesa = 400091,
            [Description("EMPENHO BEC")]
            BEC = 401891,
        }

        public enum EmpenhoLicitacao
        {
            [Description("CONVITE")]
            Convite = 2,
            [Description("DISPENSA LICITACAO")]
            Dispensa = 5,
            [Description("INEXIGIVEL")]
            Inexigivel = 6,
            [Description("PREGAO")]
            Pregao = 7
        }

        public enum ControleSituacao
        {
            [Description("Pendente")]
            Pendente = 1,
            [Description("Finalizado com sucesso")]
            FinalizadoSucesso = 2,
            [Description("Finalizado com erro")]
            FinalizadaErro = 3,
            [Description("Cancelado")]
            Cancelado = 4
        }

        public enum TipoEmpenho 
        {
            [Description("CONVITE")]
            Convite = 2,
            [Description("DISPENSA LICITACAO")]
            Dispensa = 5,
            [Description("INEXIGIVEL")]
            Inexigivel = 6,
            [Description("PREGAO")]
            Pregao = 7,
            [Description("EMPENHO BEC")]
            BEC = 401891,
        }

        public enum TipoControle
        {
            [Description("SubItemMaterial")]
            SubItemMaterial = 1,
            [Description("Catálogo do Almoxarifado")]
            CatalogoAlmoxarifado = 2,
            [Description("Inventário do Almoxarifado")]
            InventarioAlmoxarifado = 3,
            [Description("Divisão Complemento")]
            Divisao = 4,
            [Description("Grupo Material")]
            GrupoMaterial = 5,
            [Description("Classe Material")]
            ClasseMaterial = 6,
            [Description("Material")]
            Material = 7,
            [Description("Item Material")]
            ItemMaterial = 8,
            [Description("Natureza Despesa")]
            NaturezaDespesa = 9,
            [Description("Item Natureza")]
            ItemNatureza = 10,
            [Description("Unidade Administrativa")]
            UnidadeAdm = 11,
            [Description("Almoxarifado")]
            Almoxarifado = 12,
            [Description("Responsavel")]
            Responsavel = 13,
            [Description("Usuario")]
            Usuario = 14,
            [Description("Perfil Requesitante")]
            PerfilRequisitante = 15

        }

        public enum IndicadorDisponivel
        {
            [Description("Não")]
            Nao = 0,
            [Description("Sim. Até o limite.")]
            SimAteLimite = 1,
            [Description("Sim")]
            Sim = 2
        }

        public enum CargaErro
        {
            [Description("Código do Subitem inválido.")]
            CodigoSubitemInvalido = 1,
            [Description("Código já cadastrado.")]
            CodigoCadastrado = 2,
            [Description("Código do Item inválido.")]
            CodigoItemInvalido = 3,
            [Description("Relação Item/Subitem já cadastrada.")]
            RelacaoItemSubItemCadastrada = 4,
            [Description("Descrição do Subitem inválida.")]
            DescricaoSubItemInvalida = 5,
            [Description("Natureza de despesa inválida.")]
            NaturezaDespesaInvalida = 6,
            [Description("Natureza de despesa não cadastrada para o Item.")]
            naturezaDespesaNaoCadastrada = 7,
            [Description("Unidade de Fornecimento inválida.")]
            UnidadeFornecimentoInvalida = 8,
            [Description("Indicador de Lote inválido.")]
            IndicadorLoteInvalido = 9,
            [Description("Indicador de Atividade inválido.")]
            IndicadorAtividadeInvalido = 10,
            [Description("Indicador Disponivel inválido.")]
            IndicadorDisponivelInvalido = 11,
            [Description("Estoque MÃ­nimo inválido.")]
            EstoqueMinInvalido = 12,
            [Description("Estoque Máximo inválido.")]
            EstoqueMaxInvalido = 13,
            [Description("Código de Conta Auxiliar inválido.")]
            CodigoContaAuxliarInvalida = 14,
            [Description("Código do Almoxarifado inválido.")]
            CodigoAlmoxarifadoInvalido = 15,
            [Description("Código da UGE inválido.")]
            CodigoUGEInvalido = 16,
            [Description("Qtde Saldo do Subitem inválida.")]
            QtdSaldoSubItemInvalido = 17,
            [Description("Valor Saldo do Subitem inválido.")]
            ValorSaldoInvalido = 18,
            [Description("Data Vencimento Lote Subitem inválido.")]
            DataVencimentoInvalido = 19,
            [Description("Identificação Lote Subitem inválida.")]
            IndentificacaoLoteInvalida = 20,
            [Description("Data de Fabricação Lote Subitem inválida.")]
            DataFabricacaoLoteInvalida = 21,
            [Description("Gestor inválido.")]
            GestorInvalido = 22,
            [Description("Sequencia da linha da Planilha inválida.")]
            SequenciaPlanilhaInvalida = 23,
            [Description("O Almoxariado já contém o SubItem associado no catálogo.")]
            AlmoxarifadoContemSubItemAssociado = 24,
            [Description("Codigo SubItem não cadastrado para o Gestor.")]
            CodigoSubItemNaoCadastradoGestor = 25,
            [Description("O Numero do documento já está cadastrado.")]
            NumeroDocumentoCadastrado = 26,
            [Description("Documento Inválido")]
            DocumentoInvalido = 27,
            [Description("Código inválido.")]
            CodigoInvalido = 28,
            [Description("Descriçao  inválida.")]
            DescricaoInvalida = 29,
            [Description("Logradouro inválido.")]
            LogradouroInvalido = 30,
            [Description("Numero do Logradouro inválido.")]
            NumeroLogradouroInvalido = 31,
            [Description("CEP Inválido.")]
            CEPInvalido = 32,
            [Description("Ãrea inválida.")]
            AreaInvalida = 33,
            [Description("Código do responsável inválido.")]
            CodigoResponsavelInvalido = 35,
            [Description("Código do Centro de Custo inválido.")]
            CodigoCentroCustoInvalido = 36,
            [Description("Código da UA inválido.")]
            CodigoUAInvalido = 37,
            [Description("UF inválida.")]
            SiglaUFInvalida = 38,
            [Description("Divisão já cadastrada.")]
            DivisaoJaCadastrada = 39,

            [Description("Data do documento inválida.")]
            DataDocumentoInvalido = 40,
            [Description("Data do movimento inválida.")]
            DataMovimentoInvalido = 41,
            [Description("Data da operação inválida.")]
            DataOperacao = 42,
            [Description("Ano/Mês referencia invalido")]
            AnoMesRefInvalido = 43,
            [Description("Fornecedor padrao não cadastrado")]
            FornecedorPadraoNaoCadastrado = 44,
            [Description("Natureza de Despesa para este Item não esta cadastrado no SIAFEM")]
            NaturezaItem = 45,
            [Description("Bairro inválido")]
            BairroInvalido = 46,
            [Description("Municipio inválido")]
            MunicipioInvalido = 47,
            [Description("UA pertence a outro Gestor")]
            UaGestor = 48,
            [Description("CPF obrigatório")]
            CPFObrigatorio = 49,
            [Description("CPF inválido")]
            CPFInvalido = 50,
            [Description("Nome obrigatório")]
            NomeObrigatorio = 51,
            [Description("Fone inválido")]
            FoneInvalido = 52,
            [Description("CPF Cadastrado")]
            CPFCadastrado = 53,           
            [Description("Código da Divisão inválido.")]
            DivisaoInvalido = 54,
            [Description("Estrutura Orgão e/ou UGE e/ou UO está(ão) inválido(s).")]
            EstruturaOrgaoUGEUOInvalido = 55,
            [Description("Perfil Requisitante cadastrado")]
            PerfilRequisitanteCadastrado = 56,
            [Description("Complemento acima do permitido.")]
            ComplementoAcimaPermitido = 57,
            [Description("CPF informado está associado a outro usuário Ativo no sistema.")]
            CPFAtivoComOutroUsuario = 58,
        }

        public enum LiquidacaoEvento
        {
            [Description("SERVICOS EM GERAL")]
            ServicosEmGeral = 511200,
            [Description("SEGUROS EM GERAL")]
            SegurosEmGeral = 511201,
            [Description("MATERIAL DE CONSUMO")]
            MaterialDeConsumo = 511202,
            [Description("MATERIAL PERMANENTE")]
            MaterialPermanente = 511203,
            [Description("ALUGUEIS")]
            Alugueis = 511204,
            [Description("IMPORTACAO DE MAT. CONSUMO")]
            ImportacaoMaterialDeConsumo = 511208,
            [Description("IMPORTACAO DE MAT. PERMANENTE")]
            ImportacaoMaterialPermanente = 511215,
            [Description("MAT.PRIMA - ATIV. INDL.")]
            MateriaPrima = 511210,
            [Description("MAT.EMBALAGEM")]
            MaterialEmbalagem = 511211,


            [Description("SERVICOS EM GERAL - BEC")]
            ServicosEmGeralBEC = 511300,
            [Description("SEGUROS EM GERAL - BEC")]
            SegurosEmGeralBEC = 511301,
            [Description("MATERIAL DE CONSUMO - BEC")]
            MaterialDeConsumoBEC = 511302,
            [Description("MATERIAL PERMANENTE - BEC")]
            MaterialPermanenteBEC = 511303,
            [Description("MAT.PRIMA - ATIV. INDL. BEC")]
            MateriaPrimaBEC = 511310,
            [Description("MAT.EMBALAGEM - BEC")]
            MaterialEmbalagemBEC = 511311,

            [Description("SERVICOS EM GERAL - PREGAO")]
            ServicosEmGeralPregao = 511700,
            [Description("SEGUROS EM GERAL - PREGAO")]
            SegurosEmGeralPregao = 511701,
            [Description("MATERIAL DE CONSUMO - PREGAO")]
            MaterialDeConsumoPregao = 511702,
            [Description("MATERIAL PERMANENTE - PREGAO")]
            MaterialPermanentePregao = 511703,
            [Description("MAT.PRIMA - ATIV. INDL. PREGAO")]
            MateriaPrimaPregao = 511710,
            [Description("MAT.EMBALAGEM - PREGAO")]
            MaterialEmbalagemPregao = 511711,
        }

        public enum TipoPerfil
        {
            [Description("Operador Almoxarifado")]
            OperadorAlmoxarifado = 1,

            [Description("Requisitante")]
            Requisitante = 2,

            [Description("Administrador Gestor")]
            AdministradorGestor = 3,

            [Description("Administrador Geral")]
            AdministradorGeral = 4,

            [Description("Administrador Orgao")]
            AdministradorOrgao = 5,

            [Description("Financeiro")]
            Financeiro = 7,

            [Description("Consulta Relatorio")]
            ConsultaRelatorio = 9,

            [Description("Requisitante Geral")]
            RequisitanteGeral = 10,

            [Description("Administrador Financeiro SEFAZ")]
            AdministradorFinanceiroSEFAZ = 12, //Alterado para ID que esta em produção

            [Description("Comercial Prodesp")]
            ComercialProdesp = 13,
        }

        public enum SituacaoFechamento
        {
            Simular = 0,
            Executar = 1,
        }

        public enum FormatoExportacaoRelatorio
        {
            [Description("Word")]
            Word = 0,
            [Description("Excel")]
            Excel,
            [Description("PDF")]
            PDF
        }

        public enum TipoDeConsultaRequisicaoMaterial
        {
            DivisaoId = 0,
            TipoMovimento = 1,
            NumeroDocumento = 2
        }

        public enum Orgao
        {
            SAP = 1,
            FCASA = 2,
            PRODESP = 3
        }

        public enum casasDecimais
        {
            paraQuantidade = 3,
            paraValorMonetario = 2,
            paraPrecoMedioUnitario = 4
        }

        public enum Sistema
        {
            SEG = 1,
            SAM = 2,
            PAT = 4
        }

        public enum TipoExportacao
        {
            Analitica = 1,
            Sintetica = 2,
            ConsumoMedio = 3
        }

        public enum TipoNotaSIAF
        {
            [Description("Nota de Empenho")]
            NE = 1,
            [Description("NL Liquidação")]
            NL_Liquidacao = 2,
            [Description("NL Consumo")]
            NL_Consumo = 3,
            [Description("NL Reclassificação")]
            NL_Reclassificacao = 4,
            [Description("NL Depreciação")]
            NL_Depreciacao = 5,
            [Description("Tipo Desconhecido")]
            Desconhecido = 999
        }

        public enum TipoLancamentoSIAF
        {
            Normal = 1,
            Estorno = 2,
        }

        public enum SistemaSIAF
        {
            [Description("SIAFEM")]
            SIAFEM = 1,
            [Description("SIAFISICO")]
            SIAFISICO = 2
        }

        public enum TipoMaterial : int
        {
            [Description("Indeterminado")]
            Indeterminado = 0,
            [Description("Material de Consumo")]
            MaterialConsumo = 2,
            [Description("Material Permanente")]
            MaterialPermanente = 3,
            [Description("Servicos Em Geral")]
            ServicosEmGeral,
            [Description("Seguros Em Geral")]
            SegurosEmGeral,
            [Description("Alugueis")]
            Alugueis,
            [Description("Importacao de Material de Consumo")]
            ImportacaoMaterialDeConsumo,
            [Description("Importacao de Material Permanente")]
            ImportacaoMaterialPermanente,
            [Description("Atividade Industrial Materia-Prima")]
            AtividadeIndustrialMateriaPrima,
            [Description("Atividade Industrial Material Embalagem")]
            AtividadeIndustrialMaterialEmbalagem,
        }

        public enum TipoPesquisa
        {
            SemFiltro,
            Orgao,
            UO,
            Gestor,
            UGE,
            Almox,
            UA,
            Divisao,
            Usuario,
            ID
        }

        public static IDictionary<TipoPerfil, PerfilNivelAcessoEnum.PerfilNivelAcesso> ListaPerfilNivelAcesso = new Dictionary<TipoPerfil, PerfilNivelAcessoEnum.PerfilNivelAcesso>()
        {
            [TipoPerfil.OperadorAlmoxarifado] = PerfilNivelAcessoEnum.PerfilNivelAcesso.Nivel_I,
            [TipoPerfil.AdministradorGestor] = PerfilNivelAcessoEnum.PerfilNivelAcesso.Nivel_I,
            [TipoPerfil.AdministradorOrgao] = PerfilNivelAcessoEnum.PerfilNivelAcesso.Nivel_I,
            [TipoPerfil.Requisitante] = PerfilNivelAcessoEnum.PerfilNivelAcesso.Nivel_II,
            [TipoPerfil.ConsultaRelatorio] = PerfilNivelAcessoEnum.PerfilNivelAcesso.Nivel_II,
            [TipoPerfil.RequisitanteGeral] = PerfilNivelAcessoEnum.PerfilNivelAcesso.Nivel_II,
            [TipoPerfil.AdministradorGeral] = PerfilNivelAcessoEnum.PerfilNivelAcesso.Nivel_III ,
            [TipoPerfil.Financeiro] = PerfilNivelAcessoEnum.PerfilNivelAcesso.Nivel_III,
            [TipoPerfil.AdministradorFinanceiroSEFAZ] = PerfilNivelAcessoEnum.PerfilNivelAcesso.Nivel_III
        };

        public static PerfilNivelAcessoEnum.PerfilNivelAcesso ObterPerfilNivelAcessoPorPerfil(TipoPerfil perfil)
        {
            return ListaPerfilNivelAcesso[perfil];
        }

        public static List<TipoPerfil> ObterPerfilPorPerfilNivelAcesso(PerfilNivelAcessoEnum.PerfilNivelAcesso perfilNivelAcesso)
        {
            List<TipoPerfil> _listaPerfil = new List<TipoPerfil>();

            foreach (KeyValuePair<TipoPerfil, PerfilNivelAcessoEnum.PerfilNivelAcesso> _item in ListaPerfilNivelAcesso)
            {
                if (_item.Value == perfilNivelAcesso)
                    _listaPerfil.Add(_item.Key);
            }

            return _listaPerfil;
        }

        public enum NaturezaDespesa
        {
            Permanente = 4490,
        }
    }
}

