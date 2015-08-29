using Caliburn.Micro;
using HearthCap.Shell.Theme;
using HearthCap.StartUp;
using MahApps.Metro.Controls;

namespace HearthCap.Shell.Flyouts
{
    public abstract class FlyoutViewModel : Screen, IFlyout
    {
        private string header;

        private bool isOpen;

        private Position position;

        private string name;

        private bool isModal;

        private readonly IThemeManager themeManager;

        protected FlyoutViewModel()
        {
            themeManager = AppBootstrapper.Container.GetExportedValue<IThemeManager>();
            themeManager.FlyoutThemeChanged += (sender, args) => NotifyOfPropertyChange("Theme");
        }

        public string Header
        {
            get { return header; }

            set
            {
                if (value == header)
                {
                    return;
                }

                header = value;
                NotifyOfPropertyChange(() => Header);
            }
        }

        public bool IsOpen
        {
            get { return isOpen; }

            set
            {
                if (value.Equals(isOpen))
                {
                    return;
                }

                isOpen = value;
                NotifyOfPropertyChange(() => IsOpen);
            }
        }

        public Position Position
        {
            get { return position; }

            set
            {
                if (value == position)
                {
                    return;
                }

                position = value;
                using (var reg = new FlyoutRegistrySettings())
                {
                    reg.SetPosition(GetType(), value);
                }
                NotifyOfPropertyChange(() => Position);
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                if (value == name)
                {
                    return;
                }
                name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        public bool IsModal
        {
            get { return isModal; }
            set
            {
                if (value.Equals(isModal))
                {
                    return;
                }
                isModal = value;
                NotifyOfPropertyChange(() => IsModal);
            }
        }

        public FlyoutTheme Theme
        {
            get { return themeManager.FlyoutTheme; }
        }

        protected internal void SetPosition(Position defaultPosition)
        {
            using (var reg = new FlyoutRegistrySettings())
            {
                Position = reg.GetPosition(GetType(), defaultPosition);
            }
        }
    }

    public abstract class FlyoutViewModel<T> : Conductor<T>, IFlyout
        where T : class
    {
        private readonly InnerFlyout inner = new InnerFlyout();

        private class InnerFlyout : FlyoutViewModel
        {
        }

        protected FlyoutViewModel()
        {
            inner.PropertyChanged += (sender, args) => NotifyOfPropertyChange(args.PropertyName);
        }

        public new class Collection
        {
            public class OneActive : Conductor<T>.Collection.OneActive, IFlyout
            {
                private readonly InnerFlyout inner = new InnerFlyout();

                public OneActive()
                {
                    inner.PropertyChanged += (sender, args) => NotifyOfPropertyChange(args.PropertyName);
                }

                protected void SetPosition(Position defaultPosition)
                {
                    inner.SetPosition(defaultPosition);
                }

                public string Header
                {
                    get { return inner.Header; }
                    set { inner.Header = value; }
                }

                public bool IsOpen
                {
                    get { return inner.IsOpen; }
                    set { inner.IsOpen = value; }
                }

                public Position Position
                {
                    get { return inner.Position; }
                    set { inner.Position = value; }
                }

                public string Name
                {
                    get { return inner.Name; }
                    set { inner.Name = value; }
                }

                public bool IsModal
                {
                    get { return inner.IsModal; }
                    set { inner.IsModal = value; }
                }

                public FlyoutTheme Theme
                {
                    get { return inner.Theme; }
                }
            }

            public class AllActive : Conductor<T>.Collection.AllActive, IFlyout
            {
                private readonly InnerFlyout inner = new InnerFlyout();

                public AllActive()
                {
                    inner.PropertyChanged += (sender, args) => NotifyOfPropertyChange(args.PropertyName);
                }

                public string Header
                {
                    get { return inner.Header; }
                    set { inner.Header = value; }
                }

                public bool IsOpen
                {
                    get { return inner.IsOpen; }
                    set { inner.IsOpen = value; }
                }

                public Position Position
                {
                    get { return inner.Position; }
                    set { inner.Position = value; }
                }

                public string Name
                {
                    get { return inner.Name; }
                    set { inner.Name = value; }
                }

                public bool IsModal
                {
                    get { return inner.IsModal; }
                    set { inner.IsModal = value; }
                }

                public FlyoutTheme Theme
                {
                    get { return inner.Theme; }
                }

                protected void SetPosition(Position defaultPosition)
                {
                    inner.SetPosition(defaultPosition);
                }
            }
        }

        public string Header
        {
            get { return inner.Header; }
            set { inner.Header = value; }
        }

        public Position Position
        {
            get { return inner.Position; }
            set { inner.Position = value; }
        }

        public bool IsOpen
        {
            get { return inner.IsOpen; }
            set { inner.IsOpen = value; }
        }

        public string Name
        {
            get { return inner.Name; }
            set { inner.Name = value; }
        }

        public bool IsModal
        {
            get { return inner.IsModal; }
            set { inner.IsModal = value; }
        }

        public FlyoutTheme Theme
        {
            get { return inner.Theme; }
        }

        protected void SetPosition(Position defaultPosition)
        {
            inner.SetPosition(defaultPosition);
        }
    }
}
