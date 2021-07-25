using PatrimonioBusiness.integracao.entidades;
using PatrimonioBusiness.integracao.maps;
using System.Data.Common;
using System.Data.Entity;

namespace PatrimonioBusiness.integracao.contexto
{
    internal class IntegracaoContexto : DbContext
    {
        public virtual DbSet<MovimentoIntegracao> MovimentoIntegracoes { get; set; }

        private IntegracaoContexto(DbConnection existingConnection, bool contextOwnsConnection) : base(existingConnection, contextOwnsConnection)
        {

        }

        internal static IntegracaoContexto GetInstancia(DbConnection existingConnection, bool contextOwnsConnection)
        {
            return new IntegracaoContexto(existingConnection, contextOwnsConnection);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Configurations.Add(MovimentoIntegracaoMap.Create());
            modelBuilder.HasDefaultSchema("dbo");
        }
    }
}
