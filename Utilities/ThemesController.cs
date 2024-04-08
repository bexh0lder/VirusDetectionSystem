using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace VirusDetectionSystem.Utilities
{
    public static class ThemesController
    {
        // 定义了一个枚举类型ThemeTypes，包含了四种主题类型
        public enum ThemeTypes
        {
            Light, ColourfulLight,
            Dark, ColourfulDark
        }

        // 当前的主题
        public static ThemeTypes CurrentTheme { get; set; }

        // 获取或设置当前应用的资源字典
        private static ResourceDictionary ThemeDictionary
        {
            get { return Application.Current.Resources.MergedDictionaries[0]; }
            set { Application.Current.Resources.MergedDictionaries[0] = value; }
        }

        // 根据颜色名称获取对应的SolidColorBrush
        public static SolidColorBrush GetSolidBrush(string name)
        {
            object brush = ThemeDictionary[name];
            return brush is SolidColorBrush bruh ? bruh : new SolidColorBrush(Colors.Transparent);
        }

        // 根据提供的Uri更改主题
        private static void ChangeTheme(Uri uri)
        {
            ThemeDictionary = new ResourceDictionary() { Source = uri };
        }

        // 根据提供的主题类型设置主题
        public static void SetTheme(ThemeTypes theme)
        {
            string themeName = null;
            CurrentTheme = theme;
            switch (theme)
            {
                case ThemeTypes.Dark: themeName = "DarkTheme"; break;
                case ThemeTypes.Light: themeName = "LightTheme"; break;
                case ThemeTypes.ColourfulDark: themeName = "ColourfulDarkTheme"; break;
                case ThemeTypes.ColourfulLight: themeName = "ColourfulLightTheme"; break;
            }

            try
            {
                if (!string.IsNullOrEmpty(themeName))
                    ChangeTheme(new Uri($"Themes/{themeName}.xaml", UriKind.Relative));
            }
            catch { }
        }
    }
}
