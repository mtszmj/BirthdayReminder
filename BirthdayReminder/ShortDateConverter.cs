using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BirthdayReminder
{
    [ValueConversion(typeof(DateTime), typeof(string))]
    class ShortDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime date = (DateTime)value;
            return date.ToString("dd MMMM");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string date = value.ToString();
            DateTime resultDate;
            if(DateTime.TryParse(date, out resultDate))
            {
                return resultDate;
            }
            return value;
        }
    }
}
