using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BirthdayReminder.Converter
{
    [ValueConversion(typeof(DateTime), typeof(string))]
    public class LongDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime date = (DateTime)value;
            return date.ToString("dd MMMM yyyy");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string date = value.ToString();
            DateTime resultDate;
            if (DateTime.TryParse(date, out resultDate))
            {
                return resultDate;
            }
            return value;
        }
    }
}
