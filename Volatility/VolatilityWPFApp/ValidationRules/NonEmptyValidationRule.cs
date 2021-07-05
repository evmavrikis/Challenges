using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Globalization;
namespace VolatilityWPFApp
{
    public class NonEmptyValidationRule:ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var s = (string)value;
            s = s.Trim(' ');
            bool valid = (s.Length != 0);
            return new ValidationResult(valid, valid?null: "Empty text is not allowed");
        }
    }
}
