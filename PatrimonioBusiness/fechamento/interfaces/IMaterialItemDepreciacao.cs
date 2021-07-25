using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatrimonioBusiness.fechamento.interfaces
{
    public interface IMaterialItemDepreciacao
    {
        int AssetId { get; set; }
        int MaterialItemCode { get; set; }
        DateTime AcquisitionDate { get; set; }
        DateTime MovimentDate { get; set; }
        String NumberIdentification { get; set; }
        String Mensagem { get; set; }
        String MesReferencia { get; set; }
    }
}
