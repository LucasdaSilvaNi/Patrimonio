using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public class Relacionamento__Asset_AssetMovements_AuditoriaIntegracao
    {
        public int Id { get; set; }

        public int AssetId { get; set; }
        public int AssetMovementsId { get; set; }
        public int AuditoriaIntegracaoId { get; set; }

    }
}