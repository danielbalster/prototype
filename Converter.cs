﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Prototype
{
    public class BoolToVisibilityConverter : MarkupExtension, IValueConverter
    {
        private static BoolToVisibilityConverter _instance;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visibility = Visibility.Hidden;

            if (parameter != null)
                visibility = (Visibility)parameter;

            return visibility == Visibility.Visible
                ? (((bool)value) ? Visibility.Visible : Visibility.Hidden)
                : (((bool)value) ? Visibility.Hidden : Visibility.Visible);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ?? (_instance = new BoolToVisibilityConverter());
        }
    }
}
