using System;
using System.Collections.Generic;
using System.Windows;
using HearthCap.Data;
using MahApps.Metro;
using MahApps.Metro.Controls;

namespace HearthCap.Shell.Theme
{
    public interface IThemeManager
    {
        ThemeConfiguration CurrentConfiguration { get; }

        FlyoutTheme FlyoutTheme { get; }

        IEnumerable<ResourceDictionary> GetThemeResources();

        void ChangeAccent(Accent accent);

        void ApplyThemeLight();

        void ApplyThemeDark();

        void ApplyFlyoutTheme(FlyoutTheme theme);

        event EventHandler<EventArgs> FlyoutThemeChanged;
    }
}
