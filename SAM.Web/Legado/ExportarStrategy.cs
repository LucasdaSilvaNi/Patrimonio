using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SAM.Web.Legados
{
    public abstract class ExportarStrategy
    {
        public abstract void Create(DataRowCollection rows, string baseDeDestino);
    }
}