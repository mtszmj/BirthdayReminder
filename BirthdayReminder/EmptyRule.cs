using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BirthdayReminder
{
    class EmptyRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string text = value.ToString();
            if(text.Equals(String.Empty))
            {
                return new ValidationResult(false, "Imię nie może być puste");
            }
            else
            {
                return ValidationResult.ValidResult;
            }
        }
    }
}
