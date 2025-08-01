﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Presentation_Layer
{
    public class ValueToWidthConverter : IValueConverter
    {
        public static readonly NullToBoolConverter Instance = new NullToBoolConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double d)
            {
                double scaled = Math.Min(d * 2, 200);
                return Math.Max(scaled, 5);
            }
            return 5;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
