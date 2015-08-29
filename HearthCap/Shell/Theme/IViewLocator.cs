using System;
using System.Windows;

namespace HearthCap.Shell.Theme
{
    public interface IViewLocator
    {
        UIElement GetOrCreateViewType(Type viewType);
    }
}
