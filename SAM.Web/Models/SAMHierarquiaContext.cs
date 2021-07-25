namespace SAM.Web.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using SAM.Web.Models.Mapping;

    public partial class SAMHierarquiaContext : DbContext
    {
        public SAMHierarquiaContext()
            : base("name=SAMHierarquiaContext")
        {
        }

        public virtual DbSet<TB_ALMOXARIFADO> TB_ALMOXARIFADO { get; set; }
        public virtual DbSet<TB_DIVISAO> TB_DIVISAO { get; set; }
        public virtual DbSet<TB_GESTOR> TB_GESTOR { get; set; }
        public virtual DbSet<TB_ORGAO> TB_ORGAO { get; set; }
        public virtual DbSet<TB_UA> TB_UA { get; set; }
        public virtual DbSet<TB_UGE> TB_UGE { get; set; }
        public virtual DbSet<TB_UNIDADE> TB_UNIDADE { get; set; }
        public virtual DbSet<TB_UO> TB_UO { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TB_ALMOXARIFADO>()
                .Property(e => e.TB_ALMOXARIFADO_DESCRICAO)
                .IsUnicode(false);

            modelBuilder.Entity<TB_ALMOXARIFADO>()
                .Property(e => e.TB_ALMOXARIFADO_LOGRADOURO)
                .IsUnicode(false);

            modelBuilder.Entity<TB_ALMOXARIFADO>()
                .Property(e => e.TB_ALMOXARIFADO_NUMERO)
                .IsUnicode(false);

            modelBuilder.Entity<TB_ALMOXARIFADO>()
                .Property(e => e.TB_ALMOXARIFADO_COMPLEMENTO)
                .IsUnicode(false);

            modelBuilder.Entity<TB_ALMOXARIFADO>()
                .Property(e => e.TB_ALMOXARIFADO_BAIRRO)
                .IsUnicode(false);

            modelBuilder.Entity<TB_ALMOXARIFADO>()
                .Property(e => e.TB_ALMOXARIFADO_MUNICIPIO)
                .IsUnicode(false);

            modelBuilder.Entity<TB_ALMOXARIFADO>()
                .Property(e => e.TB_ALMOXARIFADO_CEP)
                .IsUnicode(false);

            modelBuilder.Entity<TB_ALMOXARIFADO>()
                .Property(e => e.TB_ALMOXARIFADO_TELEFONE)
                .IsUnicode(false);

            modelBuilder.Entity<TB_ALMOXARIFADO>()
                .Property(e => e.TB_ALMOXARIFADO_FAX)
                .IsUnicode(false);

            modelBuilder.Entity<TB_ALMOXARIFADO>()
                .Property(e => e.TB_ALMOXARIFADO_RESPONSAVEL)
                .IsUnicode(false);

            modelBuilder.Entity<TB_ALMOXARIFADO>()
                .Property(e => e.TB_ALMOXARIFADO_MES_REF_INICIAL)
                .IsUnicode(false);

            modelBuilder.Entity<TB_ALMOXARIFADO>()
                .Property(e => e.TB_ALMOXARIFADO_MES_REF)
                .IsUnicode(false);

            modelBuilder.Entity<TB_DIVISAO>()
                .Property(e => e.TB_DIVISAO_DESCRICAO)
                .IsUnicode(false);

            modelBuilder.Entity<TB_DIVISAO>()
                .Property(e => e.TB_DIVISAO_LOGRADOURO)
                .IsUnicode(false);

            modelBuilder.Entity<TB_DIVISAO>()
                .Property(e => e.TB_DIVISAO_NUMERO)
                .IsUnicode(false);

            modelBuilder.Entity<TB_DIVISAO>()
                .Property(e => e.TB_DIVISAO_COMPLEMENTO)
                .IsUnicode(false);

            modelBuilder.Entity<TB_DIVISAO>()
                .Property(e => e.TB_DIVISAO_BAIRRO)
                .IsUnicode(false);

            modelBuilder.Entity<TB_DIVISAO>()
                .Property(e => e.TB_DIVISAO_MUNICIPIO)
                .IsUnicode(false);

            modelBuilder.Entity<TB_DIVISAO>()
                .Property(e => e.TB_DIVISAO_CEP)
                .IsUnicode(false);

            modelBuilder.Entity<TB_DIVISAO>()
                .Property(e => e.TB_DIVISAO_TELEFONE)
                .IsUnicode(false);

            modelBuilder.Entity<TB_DIVISAO>()
                .Property(e => e.TB_DIVISAO_FAX)
                .IsUnicode(false);

            modelBuilder.Entity<TB_GESTOR>()
                .Property(e => e.TB_GESTOR_NOME)
                .IsUnicode(false);

            modelBuilder.Entity<TB_GESTOR>()
                .Property(e => e.TB_GESTOR_NOME_REDUZIDO)
                .IsUnicode(false);

            modelBuilder.Entity<TB_GESTOR>()
                .Property(e => e.TB_GESTOR_LOGRADOURO)
                .IsUnicode(false);

            modelBuilder.Entity<TB_GESTOR>()
                .Property(e => e.TB_GESTOR_NUMERO)
                .IsUnicode(false);

            modelBuilder.Entity<TB_GESTOR>()
                .Property(e => e.TB_GESTOR_COMPLEMENTO)
                .IsUnicode(false);

            modelBuilder.Entity<TB_GESTOR>()
                .Property(e => e.TB_GESTOR_TELEFONE)
                .IsUnicode(false);

            modelBuilder.Entity<TB_GESTOR>()
                .HasMany(e => e.TB_ALMOXARIFADO)
                .WithRequired(e => e.TB_GESTOR)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TB_GESTOR>()
                .HasMany(e => e.TB_UA)
                .WithRequired(e => e.TB_GESTOR)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TB_GESTOR>()
                .HasMany(e => e.TB_UNIDADE)
                .WithRequired(e => e.TB_GESTOR)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TB_ORGAO>()
                .Property(e => e.TB_ORGAO_DESCRICAO)
                .IsUnicode(false);

            modelBuilder.Entity<TB_ORGAO>()
                .HasMany(e => e.TB_GESTOR)
                .WithRequired(e => e.TB_ORGAO)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TB_ORGAO>()
                .HasMany(e => e.TB_UO)
                .WithRequired(e => e.TB_ORGAO)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TB_UA>()
                .Property(e => e.TB_UA_DESCRICAO)
                .IsUnicode(false);

            modelBuilder.Entity<TB_UA>()
                .HasMany(e => e.TB_DIVISAO)
                .WithRequired(e => e.TB_UA)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TB_UGE>()
                .Property(e => e.TB_UGE_DESCRICAO)
                .IsUnicode(false);

            modelBuilder.Entity<TB_UGE>()
                .Property(e => e.TB_UGE_TIPO)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<TB_UGE>()
                .HasMany(e => e.TB_ALMOXARIFADO)
                .WithOptional(e => e.TB_UGE)
                .HasForeignKey(e => e.TB_UGE_ID);

            modelBuilder.Entity<TB_UGE>()
                .HasMany(e => e.TB_ALMOXARIFADO1)
                .WithOptional(e => e.TB_UGE1)
                .HasForeignKey(e => e.TB_UGE_ID);

            modelBuilder.Entity<TB_UGE>()
                .HasMany(e => e.TB_UA)
                .WithRequired(e => e.TB_UGE)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TB_UNIDADE>()
                .Property(e => e.TB_UNIDADE_DESCRICAO)
                .IsUnicode(false);

            modelBuilder.Entity<TB_UO>()
                .Property(e => e.TB_UO_DESCRICAO)
                .IsUnicode(false);

            modelBuilder.Entity<TB_UO>()
                .HasMany(e => e.TB_UGE)
                .WithRequired(e => e.TB_UO)
                .WillCascadeOnDelete(false);
        }
    }
}
