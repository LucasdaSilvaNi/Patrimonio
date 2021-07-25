using System;
using System.Collections.Generic;

namespace SAM.Web.ViewModels
{
    public class ManagedSystemViewModel
    {
        public ManagedSystemViewModel()
        {
            this.ModulesViewModel = new List<ModuleViewModel>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Nullable<int> Sequence { get; set; }
        public List<ModuleViewModel> ModulesViewModel { get; set; }
    }
}
