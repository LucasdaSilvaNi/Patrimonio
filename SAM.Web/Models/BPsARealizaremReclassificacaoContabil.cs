using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAM.Web.Models
{
    public class BPsARealizaremReclassificacaoContabil
    {
        public Int64 Id { get; set; }
        public int AssetId { get; set; }
        public int GrupoMaterial { get; set; }
        public int IdUGE { get; set; }
    }
}