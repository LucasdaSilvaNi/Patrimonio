using PatrimonioBusiness.bases;
using PatrimonioBusiness.integracao.contexto;
using PatrimonioBusiness.integracao.entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatrimonioBusiness.integracao.repositorio
{
    internal class MovimentoIntegracaoRepositorio: BaseRepositorio<MovimentoIntegracao>
    {
        private IQueryable<MovimentoIntegracao> query;
        private IntegracaoContexto integracaoContexto = null;
        private MovimentoIntegracaoRepositorio(IntegracaoContexto integracaoContexto, IsolationLevel isolationLevel):base(isolationLevel, integracaoContexto)
        {
            base.isolationLevel = isolationLevel;
            this.integracaoContexto = (IntegracaoContexto)base.GetContexto();

        }
        internal static MovimentoIntegracaoRepositorio GetInstancia(IntegracaoContexto integracaoContexto, IsolationLevel isolationLevel)
        {
            return new MovimentoIntegracaoRepositorio(integracaoContexto, isolationLevel);
        }

       

        public IQueryable<MovimentoIntegracao> Get()
        {
            return this.createIQueryable.AsNoTracking();
        }

        public IQueryable<MovimentoIntegracao> Get(string sqlCommand)
        {
            this.createIQueryable =this.integracaoContexto.MovimentoIntegracoes.SqlQuery(sqlCommand).AsQueryable();
            return this.createIQueryable.AsNoTracking();
        }
        
        public Int64 GetSum(string sqlCommand)
        {
            return this.integracaoContexto.Database.SqlQuery<Int64>(sqlCommand).AsQueryable().Sum();

        }

        public MovimentoIntegracaoRepositorio IncluirEntidadeRelacionada(string entidadeSerIncluida)
        {
            this.createIQueryable = this.integracaoContexto.MovimentoIntegracoes.Include(entidadeSerIncluida);
            return this;
        }

        public MovimentoIntegracaoRepositorio Set(Func<MovimentoIntegracao, bool> predicate)
        {
            this.createIQueryable = this.integracaoContexto.MovimentoIntegracoes.Where(predicate).AsQueryable();
            return this;
        }

        public MovimentoIntegracaoRepositorio Set(IQueryable<MovimentoIntegracao> query)
        {
            this.createIQueryable = query;
            return this;
        }
        public MovimentoIntegracaoRepositorio Set(string query)
        {
            this.createIQueryable = this.Contexto().MovimentoIntegracoes.SqlQuery(query).AsQueryable();
            return this;
        }
        public MovimentoIntegracaoRepositorio SetAll()
        {
            this.createIQueryable = this.integracaoContexto.MovimentoIntegracoes;
            return this;
        }

        internal override void  Atualizar(MovimentoIntegracao _movimentacao)
        {
            this.integracaoContexto.Entry(_movimentacao).State = EntityState.Modified;
        }

        internal IntegracaoContexto Contexto()
        {
            return this.integracaoContexto;
        }
    }
}
