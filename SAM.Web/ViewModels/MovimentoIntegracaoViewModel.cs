using PatrimonioBusiness.integracao.interfaces;
using SAM.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SAM.Web.ViewModels
{
    [NotMapped]
    public class MovimentoIntegracaoViewModel : IMovimentoIntegracao
    {
        public MovimentoIntegracaoViewModel()
        {
            
        }
        public int Id { get; set; }
        [Display(Name = "Movimento")]
        public int MovimentoId { get; set; }
        [Display(Name = "Item do Movimento")]
        public int MovimentoItemId { get; set; }
        [Display(Name = "Tipo Movimento")]
        [Required(ErrorMessage ="Selecione o tipo de movimento")]
        public int TipoMovimentoId { get; set; }
        [Display(Name = "UGE")]
        [Required(ErrorMessage ="Selecione a UGE")]
        public int UgeId { get; set; }
        [Display(Name = "Almoxarifado")]
        public int AlmoxarifadoId { get; set; }
        [Display(Name = "Item de Material")]
        [Required(ErrorMessage ="Selecione o item de material")]
        public int ItemMaterialId { get; set; }
        [Display(Name = "SubItem de Material")]
        public int SubItemMaterialId { get; set; }
        [Display(Name = "Número Documento")]
        public string NumeroDocumento { get; set; }
        [Display(Name = "Mês/Ano de Referência")]
        public string AnoMesReferencia { get; set; }
        public string Empenho { get; set; }
        [Display(Name = "Observações")]
        public string Obeservacoes { get; set; }
        [Display(Name = "Almoxarifado Origem/Destino")]
        public int? AlmoxarifadoOrigemDestinoId { get; set; }
        [Display(Name = "Data de Movimento")]
        public DateTime DataMovimento { get; set; }
        [Display(Name = "Data da Operação")]
        public DateTime? DataOperacao { get; set; }
        [Display(Name = "Quantidade Movimentada")]
        public Decimal QuantidadeMovimentada { get; set; }
        [Display(Name = "Valor Unitário")]
        public decimal ValorUnitario { get; set; }
        public bool Ativo { get; set; }
        public bool ItemAtivo { get; set; }
        [Display(Name = "Integração Realizada")]
        public int? IntegracaoRealizado { get; set; }
        [Display(Name = "Data de Inclusão")]
        public DateTime DataInclusao { get; set; }
        [Display(Name = "Data de Alteração")]
        public DateTime? DataAlteracao { get; set; }
        public int ItemMaterialCode { get ; set; }
        public int CodigoOrgao { get ; set ; }
        public int CodigoUge { get ; set ; }
        public int? CodigoUa { get ; set ; }
        public int CodigoDivisao { get ; set ; }
        public long CodigoResponsavel { get ; set ; }
        public string ItemMaterialDescricao { get; set; }
        public AssetEAssetMovimentViewModel GetAssetEAssetMovimentViewModel()
        {
            return new AssetEAssetMovimentViewModel()
            {
                Asset = new Asset
                {
                    Id = 0,
                    MovementTypeId = this.TipoMovimentoId,
                    MovimentDate = this.DataMovimento,
                    ValueAcquisition = this.ValorUnitario,
                    MaterialItemCode = this.ItemMaterialCode
                },
                AssetMoviment = new AssetMovements()
                {
                    MovementTypeId = this.TipoMovimentoId,
                    NumberDoc = this.NumeroDocumento,
                    MovimentDate = this.DataMovimento,
                    Observation = this.Obeservacoes,


                },
                codigo_descricao_orgao_emissor_transferencia = this.CodigoOrgao.ToString(),
                codigo_descricao_uge_emissor_transferencia = this.CodigoUge.ToString()
            };
        }
    }
}