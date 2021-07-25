using SAM.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SAM.Web.ViewModels
{
    [NotMapped]
    public class SuppliersViewModel
    {
        public int Id { get; set; }

        public string CPFCNPJ { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Telephone { get; set; }
        public SuppliersViewModel Create(Supplier supplier)
        {
           /* return new ViewModels.SuppliersViewModel()*/
           return new SuppliersViewModel()
            {
                Id = supplier.Id,
                CPFCNPJ = supplier.CPFCNPJ,
                Name = supplier.Name,
                Email = supplier.Email,
                Telephone = supplier.Telephone
            }; 
        }
    }
}