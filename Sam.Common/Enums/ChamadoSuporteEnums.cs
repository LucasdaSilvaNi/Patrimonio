using System.ComponentModel;




namespace Sam.Common.Enums
{
    namespace ChamadoSuporteEnums
    {
        public enum StatusAtendimentoChamado : byte
        {
            [Description("Todos")]
            Todos = 0,

            [Description("Aberto")]
            Aberto = 1,

            [Description("Em Atendimento")]
            EmAtendimento = 2,

            [Description("Aguardando Usuário")]
            AguardandoRetornoUsuario = 3,

            [Description("Concluído")]
            Concluido = 4,

            [Description("Finalizado")]
            Finalizado = 5,

            [Description("Reaberto")]
            Reaberto = 6
        }

        public enum FuncionalidadeSistema : byte
        {
            [Description("Login - Alterar Senha")]
            Login = 1,

            [Description("Inicio - Tela Chamados")]
            TelaChamadosSuporte = 2,

            [Description("Inicio - Tela Material Apoio")]
            AreaMaterialApoio = 3,

            //Tipos Entrada Material
            [Description("Entrada Material (Empenho)")]
            EntradaMaterialEmpenho = 4,

            [Description("Entrada Material (Empenho Serviço Volta Material Consumo)")]
            EntradaMaterialEmpenhoServicoVoltaMaterialConsumo = 5,

            [Description("Entrada Material (Transferência)")]
            EntradaMaterialTransferência = 6,

            [Description("Entrada Material (Doação)")]
            EntradaMaterialDoacao = 7,

            [Description("Entrada Material (Devolução)")]
            EntradaMaterialDevolucao = 8,

            [Description("Entrada Material (Material Transformado)")]
            EntradaMaterialMaterialTransformado = 9,

            [Description("Entrada Material (Transferência Almox Não Implantado)")]
            EntradaMaterialTransferenciaAlmoxNaoImplantado = 10,

            [Description("Entrada Material (Empenho de Restos a Pagar)")]
            EntradaMaterialEmpenhoRestosAPagar = 11,

            [Description("Entrada Material (Inventario)")]
            EntradaMaterialInventario = 12,

            //Tipos Saida Material
            [Description("Saída Material (Requisição)")]
            SaídaMaterialRequisicao = 13,

            [Description("Saída Material (Transferência)")]
            SaídaMaterialTransferencia = 14,

            [Description("Saída Material (Doação)")]
            SaídaMaterialDoacao = 15,

            //[Description("Requisição")]
            //Requisicao,

            [Description("Saída Material (Outras)")]
            SaidaMaterialOutras = 16,

            [Description("Saída Material (Transformação)")]
            SaidaMaterialTransformado = 17,

            [Description("Saída Material (Extravio / Furto / Roubo)")]
            SaidaMaterialExtravioFurtoRoubo = 18,

            [Description("Saída Material (Incorporação Indevida)")]
            SaidaMaterialIncorporacaoIndevida = 19,

            [Description("Saída Material (Perda)")]
            SaidaMaterialPerda = 20,

            [Description("Saída Material (Inservível / Quebra)")]
            SaidaMaterialInservivelQuebra = 21,

            [Description("Saída Material (Transferência Almox Não Implantado)")]
            SaidaMaterialTransferenciaAlmoxNaoImplantado = 22,

            [Description("Saída Material (Amostra / Exposição / Análise)")]
            SaidaMaterialAmostraExposicaoAnalise = 23,

            //Fechamento
            [Description("Fechamento Mensal (Análise)")]
            FechamentoMensalFechamento = 24,

            [Description("Fechamento Mensal (Simulação)")]
            FechamentoMensalSimulacao = 25,

            [Description("Fechamento Mensal (Fechamento / Reabertura)")]
            FechamentoMensalFechamentoReabertura = 26,

            //Consultas
            [Description("Fechamento Mensal (NL Consumo)")]
            FechamentoMensal_NL_Consumo = 27,

            [Description("Consulta - Estoque - Sintética")]
            ConsultaPosicaoSintetica = 28,

            [Description("Consulta - Estoque - Analitica")]
            ConsultaPosicaoAnalitica = 29,

            [Description("Consulta - Estoque - Ficha Prateleira")]
            ConsultaFichaPrateleira = 30,

            [Description("Consulta - Movimentação - Entrada")]
            ConsultaMovimentacoesEntrada = 31,

            [Description("Consulta - Movimentação - Saída")]
            ConsultaMovimentacoesSaida = 32,

            [Description("Consulta - Movimentação - Transferência")]
            ConsultaMovimentacoesTransferencia = 33,

            [Description("Consulta - Consumo - Perfil Almoxarifado")]
            ConsultaConsumo_Perfil_Almox = 34,

            [Description("Consulta - Consumo - Perfil Requisitante")]
            ConsultaConsumo_Perfil_Requisitante = 35,

            [Description("Consulta - Consumo - Perfil Subitem")]
            ConsultaConsumo_Perfil_Subitem = 36,

            [Description("Relatório Mensais - Balancete")]
            RelatorioMensal_Balancete = 37,

            [Description("Relatório Mensais - Balancete Anual")]
            RelatorioMensal_BalanceteAnual = 38,

            [Description("Relatório Mensais - Inventário")]
            RelatorioMensal_Inventario = 39,

            [Description("Relatório Mensais - Analítico")]
            RelatorioMensal_Analitico = 40,

            [Description("Relatório Mensais - Exportação de Custo")]
            RelatorioMensal_ExportacaoCustos = 41,

            [Description("Gerência do Catálogo (Almoxarifado)")]
            GerenciaCatalogoAlmox_PerfilOperadorAlmox = 42,

            [Description("Notas Pendentes SIAFEM")]
            NotasPendentesSIAFEM = 43,

            [Description("Requisição Material - (Requisição)")]
            ModuloRequisicao_Requisicao_PerfilRequisitante = 44,

            [Description("Consulta Catalogo - (Requisitante)")]
            ConsultaCatalogo_PerfilRequisitante = 45,

            [Description("Requisição Material - (Requisição)")]
            ModuloRequisicao_Requisicao_PerfilRequisitanteGeral = 46,

            [Description("Consulta Catalogo - (Requisitante Geral)")]
            ConsultaCatalogo_PerfilRequisitanteGeral = 47,

            //Tabelas
            [Description("Cadastro Usuários (Admin Gestor")]
            CadastroUsuarios_PerfilAdminGestor = 48,

            [Description("Gerência do Catálogo (Almoxarifado)")]
            GerenciaCatalogoAlmox_PerfilAdminGestor = 49,

            [Description("Cadastro Usuários (Admin Orgão")]
            CadastroUsuarios_PerfilAdminOrgao = 50,

            //Catalogo
            [Description("Catalogo - Item")]
            Catalogo_ItemMaterial = 51,

            [Description("Catalogo - Subitem")]
            Catalogo_SubitemMaterial = 52,

            [Description("Catalogo - Relação Item Subitem")]
            Relacao_ItemMaterial_SubitemMaterial = 53,

            [Description("Catalogo - Cadastros em Geral)")]
            Catalogo_CadastrosEmGeral = 54,

            [Description("Estrutura Organizacional - Cadastros em Geral)")]
            EstruturaOrganizacional_CadastrosEmGeral = 55

        }

        public enum TipoChamadoSuporte : byte
        {
            [Description("Erro")]
            Erro = 1,

            [Description("Dúvida")]
            Duvida,

            [Description("Melhoria")]
            Melhoria,

            [Description("Solicitação")]
            Solicitacao,
        }

        public enum SistemaModulo : byte 
        {
            [Description("SAM (Estoque)")]
            Estoque = 0,

            [Description("SAM (Patrimônio)")]
            Patrimonio
        }

        public enum AmbienteSistema : byte
        {
            [Description("Produção")]
            Producao = 0,

            [Description("Homologação")]
            Homologacao
        }
    }
}
