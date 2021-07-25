using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAM.Web.ViewModels
{
    [NotMapped]
    public class RelacaoGrupomaterialContaContabilViewModel
    {
        public int Id { get; set; }
        public int CodigoGrupoMaterial { get; set; }
        public string DescricaoGrupoMaterial { get; set; }
        public int? ContaContabil { get; set; }
        public string DescricaoContaContabil { get; set; }
    }
}