using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAM.Web.Models
{
    public partial class BPsExcluidos
    {
        public int Id { get; set; }

        [Required]
        public int TipoIncorporacao { get; set; }

        [Required]
        public int InitialId { get; set; }

        public string SiglaInicial { get; set; }

        [Required]
        public string Chapa { get; set; }

        [Required]
        public int ItemMaterial { get; set; }

        [Required]
        public int GrupoMaterial { get; set; }

        [Required]
        public int StateConservationId { get; set; }

        [Required]
        public int ManagerUnitId { get; set; }

        public int? AdministrativeUnitId { get; set; }

        public int? ResponsibleId { get; set; }

        public string Processo { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public virtual DateTime DataAquisicao { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DataIncorporacao { get; set; }

        [Required]
        public virtual decimal ValorAquisicao { get; set; }
        public bool flagAcervo { get; set; }
        public bool flagTerceiro { get; set; }
        public bool flagDecretoSEFAZ { get; set; }

        [Required]
        public DateTime DataAcao { get; set; }
        [Required]
        public int LoginAcao { get; set; }

        public string NotaLancamento { get; set; }
        public string NotaLancamentoEstorno { get; set; }
        public string NotaLancamentoDepreciacao { get; set; }
        public string NotaLancamentoDepreciacaoEstorno { get; set; }
        public string NotaLancamentoReclassificacao { get; set; }
        public string NotaLancamentoReclassificacaoEstorno { get; set; }

        public string Observacoes { get; set; }
        public byte FlagModoExclusao { get; set; }
        public string MotivoExclusao { get; set; }
        public string NumeroDocumento { get; set; }
    }
}