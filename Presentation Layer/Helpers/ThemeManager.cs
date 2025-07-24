using System;
using System.Windows;

namespace Presentation_Layer.Helpers
{
    public static class ThemeManager
    {
        public static void SetTheme(string theme)
        {
            var dict = new ResourceDictionary();
            dict.Source = new Uri($"Themes/{theme}Theme.xaml", UriKind.Relative);

            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(dict);
        }
    }
}
