using SAM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAM.Web.Models
{
    public class EstadoItemInventario
    {
        public EstadoItemInventario()
        {
            this.ItemInventarios = new List<ItemInventario>();
        }
        public int Id { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        public virtual ICollection<ItemInventario> ItemInventarios { get; set; }
    }
}