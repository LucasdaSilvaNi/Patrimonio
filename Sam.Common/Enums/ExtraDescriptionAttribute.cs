using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Common.Enums
{
    public class ExtraDescriptionAttribute  : Attribute
    {
        private string description;
        public string Description { get { return description; } }

        //**ADDED**
        private string extraInfo;
        public string ExtraInfo 
        { 
            get { return extraInfo; } 
            set { extraInfo = value; } 
        }

        public ExtraDescriptionAttribute(string description)
        {
            this.description = description;
            this.extraInfo = string.Empty;
        }
    }
}
