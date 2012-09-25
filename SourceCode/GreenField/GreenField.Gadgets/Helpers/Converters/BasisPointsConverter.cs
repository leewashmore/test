﻿using System;
using System.Windows.Data;


namespace GreenField.Gadgets.Helpers
{
    public class BasisPointsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Decimal basisPoints = (Decimal)value * 10000;
            return basisPoints.ToString("n0");  //return basis points with no decimal places
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}