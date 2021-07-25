using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatrimonioBusiness.fechamento.interfaces
{
    public interface IDepreciacaoErro
    {
        int ErrorNumber { get; set; }
        String ErrorMessage { get; set; }
        int MaterialItemCode { get; set; }
        int AssetStartId { get; set; }
        int AssetId { get; set; }
    }
}
