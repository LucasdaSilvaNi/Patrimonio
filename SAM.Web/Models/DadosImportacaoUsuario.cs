using Newtonsoft.Json;
using SAM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    [Serializable]
    public partial class DadosImportacaoUsuario
    {
        public int Id { get; set; }
        public int ImportacaoPlanilhaId { get; set; }
        public DateTime Data_Importacao { get; set; }

        public int CARGA_SEQ { get; set; }
        public string PERFIL { get; set; }
        public string USUARIO_CPF { get; set; }
        public string USUARIO_NOME_USUARIO { get; set; }
        public string ORGAO_CODIGO { get; set; }
        public string UO_CODIGO { get; set; }
        public string UGE_CODIGO { get; set; }

        public ImportacaoPlanilha ImportacaoPlanilha { get; set; }
    }
}
