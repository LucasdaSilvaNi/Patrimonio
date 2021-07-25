using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatrimonioBusiness.fechamento.contexto
{
    internal class FechamentoContexto: DbContext
    {
        private FechamentoContexto(DbConnection existingConnection, bool contextOwnsConnection) : base(existingConnection, contextOwnsConnection)
        {
        }

        internal static FechamentoContexto GetInstancia(DbConnection existingConnection, bool contextOwnsConnection)
        {
            return new FechamentoContexto(existingConnection, contextOwnsConnection);
        }


        public override string ToString()
        {
            return base.ToString();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
