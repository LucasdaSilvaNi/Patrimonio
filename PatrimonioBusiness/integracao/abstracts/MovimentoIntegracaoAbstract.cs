using PatrimonioBusiness.bases;
using PatrimonioBusiness.integracao.interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatrimonioBusiness.integracao.abstracts
{
    public abstract class MovimentoIntegracaoAbstract
    {
        protected IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted;

        public string retornoDepreciacao { get; protected set; }
        public int totalRegistrosRetorno { get; protected set; }
        public int registroIndex { get; protected set; }
        public int totalRegistroPorPagina { get; protected set; }
        public string filtro { get; protected set; }
        public IMovimentoIntegracao  movimentoIntegracao { get; set; }

        protected MovimentoIntegracaoAbstract(IsolationLevel isolationLevel)
        {
            this.isolationLevel = isolationLevel;
           
        }


        public abstract Task<MovimentoIntegracaoAbstract> IncluirAsync(IMovimentoIntegracao movimentoIntegracao);
        public abstract Task<MovimentoIntegracaoAbstract> AtualizarAsync(IMovimentoIntegracao movimentoIntegracao);
        public abstract Task<MovimentoIntegracaoAbstract> ExcluirAsync(IMovimentoIntegracao movimentoIntegracao);
        public abstract Task<IMovimentoIntegracao> Get(int Id);
        public abstract Task<IList<IMovimentoIntegracao>> GetsAsync(int codigoUge, string mesReferencia);
        public abstract IList<IMovimentoIntegracao> Gets();
        public abstract Task<IList<IMovimentoIntegracao>> Gets(int paginaIndex, string filtro, int registroPorPagina);
        public abstract Task<IList<IMovimentoIntegracao>> Gets(int UgeId);
        public abstract Task<IList<IMovimentoIntegracao>> Gets(int codigoUA, int codigoItemMaterial);
        public abstract Task<bool> ConfirmarIntegracao(int movimentoIntegracaoId);
        public abstract Task<MovimentoIntegracaoAbstract> DepreciarIncorporacao(int codigoDaUge,DateTime dataFinalDaDepreciacao,int assetStartId);
        public abstract Task<MovimentoIntegracaoAbstract> DepreciarIntegracao(int codigoDaUge, DateTime dataFinalDaDepreciacao, int assetStartId);
    }
}
