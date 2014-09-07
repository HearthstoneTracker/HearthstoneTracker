namespace HearthCap.Shell.Theme
{
    using System;
    using System.Windows;

    public interface IViewLocator
    {
        UIElement GetOrCreateViewType(Type viewType);
    }
}