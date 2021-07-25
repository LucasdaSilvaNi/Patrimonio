namespace SAM.Web.Models
{
    public class HouveAlteracaoContabil
    {
        public int Id { get; set; }
        public int IdOrgao { get; set; }
        public int IdUO { get; set; }
        public int IdUGE { get; set; }
        public int IdContaContabil { get; set; }
        public bool HouveAlteracao { get; set; }
    }
}