using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SAM.Web.Models.Mapping
{
    public class DadosImportacaoBemMap : EntityTypeConfiguration<DadosImportacaoBem>
    {
        public DadosImportacaoBemMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Table & Column Mappings
            this.ToTable("DadosImportacaoBens");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.ImportacaoPlanilhaId).HasColumnName("ImportacaoPlanilhaId");
            this.Property(t => t.Data_Importacao).HasColumnName("Data_Importacao");
            this.Property(t => t.Sigla).HasColumnName("Sigla");
            this.Property(t => t.Sigla_Descricao).HasColumnName("Sigla_Descricao");
            this.Property(t => t.Chapa).HasColumnName("Chapa");
            this.Property(t => t.Codigo_do_Item).HasColumnName("Codigo_do_Item");
            this.Property(t => t.Descricao_do_Item).HasColumnName("Descricao_do_Item");
            this.Property(t => t.CPF_Responsavel).HasColumnName("CPF_Responsavel");
            this.Property(t => t.Nome_Responsavel).HasColumnName("Nome_Responsavel");
            this.Property(t => t.Cargo_Responsavel).HasColumnName("Cargo_Responsavel");
            this.Property(t => t.Data_Aquisicao).HasColumnName("Data_Aquisicao");
            this.Property(t => t.Valor_Aquisicao).HasColumnName("Valor_Aquisicao");
            this.Property(t => t.Data_Inclusao).HasColumnName("Data_Inclusao");
            this.Property(t => t.Estado_de_Conservacao).HasColumnName("Estado_de_Conservacao");
            this.Property(t => t.Codigo_Orgao).HasColumnName("Codigo_Orgao");
            this.Property(t => t.Codigo_UO).HasColumnName("Codigo_UO");
            this.Property(t => t.Nome_UO).HasColumnName("Nome_UO");
            this.Property(t => t.Codigo_UGE).HasColumnName("Codigo_UGE");
            this.Property(t => t.Nome_UGE).HasColumnName("Nome_UGE");
            this.Property(t => t.Codigo_UA).HasColumnName("Codigo_UA");
            this.Property(t => t.Nome_UA).HasColumnName("Nome_UA");
            this.Property(t => t.Codigo_Divisao).HasColumnName("Codigo_Divisao");
            this.Property(t => t.Nome_Divisao).HasColumnName("Nome_Divisao");
            this.Property(t => t.Conta_Auxiliar).HasColumnName("Conta_Auxiliar");
            this.Property(t => t.Contabil_Auxiliar).HasColumnName("Contabil_Auxiliar");
            this.Property(t => t.Nome_Conta_Auxiliar).HasColumnName("Nome_Conta_Auxiliar");
            this.Property(t => t.Numero_Serie).HasColumnName("Numero_Serie");
            this.Property(t => t.Data_Fabricacao).HasColumnName("Data_Fabricacao");
            this.Property(t => t.Data_Garantia).HasColumnName("Data_Garantia");
            this.Property(t => t.Numero_Chassi).HasColumnName("Numero_Chassi");
            this.Property(t => t.Marca).HasColumnName("Marca");
            this.Property(t => t.Modelo).HasColumnName("Modelo");
            this.Property(t => t.Placa).HasColumnName("Placa");
            this.Property(t => t.Sigla_Antiga).HasColumnName("Sigla_Antiga");
            this.Property(t => t.Chapa_Antiga).HasColumnName("Chapa_Antiga");
            this.Property(t => t.NEmpenho).HasColumnName("NEmpenho");
            this.Property(t => t.Observacoes).HasColumnName("Observacoes");
            this.Property(t => t.Descricao_Adicional).HasColumnName("Descricao_Adicional");
            this.Property(t => t.Nota_Fiscal).HasColumnName("Nota_Fiscal");
            this.Property(t => t.CNPJ_Fornecedor).HasColumnName("CNPJ_Fornecedor");
            this.Property(t => t.Nome_Fornecedor).HasColumnName("Nome_Fornecedor");
            this.Property(t => t.Telefone_Fornecedor).HasColumnName("Telefone_Fornecedor");
            this.Property(t => t.CEP_Fornecedor).HasColumnName("CEP_Fornecedor");
            this.Property(t => t.Endereco_Fornecedor).HasColumnName("Endereco_Fornecedor");
            this.Property(t => t.Cidade_Fornecedor).HasColumnName("Cidade_Fornecedor");

            this.Property(t => t.Estado_Fornecedor).HasColumnName("Estado_Fornecedor");
            this.Property(t => t.Inf_Complementares_Fornecedores).HasColumnName("Inf_Complementares_Fornecedores");
            this.Property(t => t.CPF_CNPJ_Terceiro).HasColumnName("CPF_CNPJ_Terceiro");
            this.Property(t => t.Nome_Terceiro).HasColumnName("Nome_Terceiro");
            this.Property(t => t.Telefone_terceiro).HasColumnName("Telefone_terceiro");
            this.Property(t => t.CEP_Terceiro).HasColumnName("CEP_Terceiro");
            this.Property(t => t.Endereco_Terceiro).HasColumnName("Endereco_Terceiro");
            this.Property(t => t.Cidade_Terceiro).HasColumnName("Cidade_Terceiro");
            this.Property(t => t.Estado_Terceiro).HasColumnName("Estado_Terceiro");
            this.Property(t => t.Acervo).HasColumnName("Acervo");
            this.Property(t => t.Terceiro).HasColumnName("Terceiro");

            //Relationships
            this.HasRequired(t => t.ImportacaoPlanilha)
                .WithMany(t => t.DadosImportacaoBens)
                .HasForeignKey(d => d.ImportacaoPlanilhaId);
        }
    }
}
