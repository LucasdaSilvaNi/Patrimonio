using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAM.Web.Legados
{
    public class ValidarDropListBox : ValidationAttribute
    {
        public ValidarDropListBox(string errorMessage) : base(errorMessage)
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var _value = (int)value;

            if (_value < 1)
            {
                return new ValidationResult(base.ErrorMessage);
            }
            return ValidationResult.Success;
        }
    }
}