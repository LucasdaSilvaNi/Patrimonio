using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SAM.Web.Common
{
    public static class CommonMensagens
    {
        public static string SemPermissaoDeAcesso { get { return "Você não tem permissão para acessar este conteúdo!"; } }
        public static string RegistroNaoExistente { get { return "Registro não existe na base de dados, favor verificar as informações!"; } }
        public static string IdentificadorNulo { get { return "O identificador da requisição esta vazio, favor verificar as informações!"; } }
        public static string PadraoException { get { return "Ocorreu uma exceção, favor informar ao suporte o número do Log: "; } }
        public static string ExcluirProprioUsuario { get { return "Não é possivel excluir o próprio usuário logado!"; } }
        public static string ExcluirRegistroComVinculos { get { return "Não é possivel excluir este registro, pois está sendo utilizado em outro processo!"; } }
        public static string OperacaoInvalidaIntegracaoFechamento { get { return "Operação Inválida! Por gentileza, verifique se a UGE não possui pendências referente ao fechamento para resolvê-las. Caso não, verifique se na tela de fechamento há dados a serem enviados por essa UGE ao Contabiliza."; } }
        public static string SistemaInstavel { get { return "Caro usuário, verificamos que o sistema está sobrecarregado, o que compromaterá essa operação. Estamos trabalhando para erradicar essas situações. Por gentileza, tente acessar o sistema novamente mais tarde"; } }

        //public static string RegistroJaEstaCadastrado { get { return "Registro já está cadastrado!"; } }
        //public static string MesJaCadastrado { get { return "Mês Selecionado já foi cadastrado!"; } }
    }
}