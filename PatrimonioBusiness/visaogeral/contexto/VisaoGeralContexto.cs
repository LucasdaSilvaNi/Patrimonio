using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatrimonioBusiness.visaogeral.contexto
{
    internal class VisaoGeralContexto : DbContext
    {

        private VisaoGeralContexto(DbConnection existingConnection, bool contextOwnsConnection) : base(existingConnection, contextOwnsConnection)
        {
        }

        internal static VisaoGeralContexto GetInstancia(DbConnection existingConnection, bool contextOwnsConnection)
        {
            return new VisaoGeralContexto(existingConnection, contextOwnsConnection);
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
