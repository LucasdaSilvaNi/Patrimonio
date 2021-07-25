using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicoOnlineServer.ViewModels
{
    public class DataTablesResponseViewModel
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public int RecordsFiltered { get; set; }
        public int RecordsTotal { get; set; }
        public DataTableOrderViewModel[] Order { get; set; }
        public DataTableColumnViewModel[] Columns { get; set; }
        public DataTableSearchViewModel Search { get; set; }
    }
}
