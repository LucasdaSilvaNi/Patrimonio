using SAM.Web.Models;
using SAM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SAM.Web.services
{
    public class ManagerUnitServices
    {
        private SAMContext Context = null;
        public ManagerUnitServices(SAMContext context)
        {
            this.Context = context;
        }

        public async Task<ManagerUnitViewModel> ConsultarPorCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                throw new Exception("Código da UGE está vázio");
            }

            IQueryable<ManagerUnit> query = (from m in this.Context.ManagerUnits where m.Code.Equals(codigo) select m);
            var retorno = await query.AsNoTracking().FirstOrDefaultAsync();
            ManagerUnitViewModel managerUnitViewModel = ManagerUnitViewModel.SetViewModel(retorno);

            return managerUnitViewModel;
        }
        public async Task<string> GetDataReferencia(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                throw new Exception("Código da UGE está vázio");
            }

            IQueryable<string> query = (from m in this.Context.ManagerUnits where m.Code.Equals(codigo) select m.ManagmentUnit_YearMonthReference);
            return await query.AsNoTracking().FirstOrDefaultAsync();
        }
    }
}