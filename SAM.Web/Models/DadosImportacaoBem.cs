using Newtonsoft.Json;
using SAM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    [Serializable]
    public partial class DadosImportacaoBem
    {
        public int Id { get; set; }
        public int ImportacaoPlanilhaId { get; set; }        
        public DateTime Data_Importacao { get; set; }

        public string Sigla { get; set; }
        public string Sigla_Descricao { get; set; }
        public string Chapa { get; set; }
        public string Codigo_do_Item { get; set; }
        public string Descricao_do_Item { get; set; }

        public string CPF_Responsavel { get; set; }
        public string Nome_Responsavel { get; set; }
        public string Cargo_Responsavel { get; set; }
        public string Data_Aquisicao { get; set; }
        public string Valor_Aquisicao { get; set; }

        public string Data_Inclusao { get; set; }
        public string Estado_de_Conservacao { get; set; }
        public string Codigo_Orgao { get; set; }
        public string Codigo_UO { get; set; }
        public string Nome_UO { get; set; }

        public string Codigo_UGE { get; set; }
        public string Nome_UGE { get; set; }
        public string Codigo_UA { get; set; }
        public string Nome_UA { get; set; }
        public string Codigo_Divisao { get; set; }

        public string Nome_Divisao { get; set; }
        public string Conta_Auxiliar { get; set; }
        public string Contabil_Auxiliar { get; set; }
        public string Nome_Conta_Auxiliar { get; set; }
        public string Numero_Serie { get; set; }

        public string Data_Fabricacao { get; set; }
        public string Data_Garantia { get; set; }
        public string Numero_Chassi { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }

        public string Placa { get; set; }
        public string Sigla_Antiga { get; set; }
        public string Chapa_Antiga { get; set; }
        public string NEmpenho { get; set; }
        public string Observacoes { get; set; }

        public string Descricao_Adicional { get; set; }
        public string Nota_Fiscal { get; set; }
        public string CNPJ_Fornecedor { get; set; }
        public string Nome_Fornecedor { get; set; }
        public string Telefone_Fornecedor { get; set; }

        public string CEP_Fornecedor { get; set; }
        public string Endereco_Fornecedor { get; set; }
        public string Cidade_Fornecedor { get; set; }
        public string Estado_Fornecedor { get; set; }
        public string Inf_Complementares_Fornecedores { get; set; }

        public string CPF_CNPJ_Terceiro { get; set; }
        public string Nome_Terceiro { get; set; }
        public string Telefone_terceiro { get; set; }
        public string CEP_Terceiro { get; set; }
        public string Endereco_Terceiro { get; set; }

        public string Cidade_Terceiro { get; set; }
        public string Estado_Terceiro { get; set; }
        public string Acervo { get; set; }
        public string Terceiro { get; set; }

        public ImportacaoPlanilha ImportacaoPlanilha { get; set; }
    }
}
