﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace VirusDetectionSystem.Converters
{
    /// <summary>
    /// This VisibilityToBoolean converter convert Visibility <-> Boolean
    /// </summary>
    public sealed class VisibilityToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            (Visibility)value == Visibility.Visible;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            (bool)value == true
                ? Visibility.Visible
                : Visibility.Collapsed;
    }
}
