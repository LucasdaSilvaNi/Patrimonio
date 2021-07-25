using Newtonsoft.Json;
using SAM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    [Serializable]
    public partial class ImportacaoPlanilha
    {
        public int Id { get; set; }
        public string NomeArquivo { get; set; }        
        public bool Processado { get; set; }
        public string Login_Importacao { get; set; }
        public DateTime Data_Importacao { get; set; }
        public string Login_Processamento { get; set; }
        public DateTime? Data_Processamento { get; set; }
        public virtual ICollection<DadosImportacaoBem> DadosImportacaoBens { get; set; }
        public virtual ICollection<DadosImportacaoUsuario> DadosImportacaoUsuarios { get; set; }
    }
}
