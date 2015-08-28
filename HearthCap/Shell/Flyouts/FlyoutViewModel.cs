namespace HearthCap.Shell.Flyouts
{
    using Caliburn.Micro;

    using HearthCap.Shell.Theme;
    using HearthCap.StartUp;

    using MahApps.Metro.Controls;

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
            this.themeManager = AppBootstrapper.Container.GetExportedValue<IThemeManager>();
            themeManager.FlyoutThemeChanged += (sender, args) => NotifyOfPropertyChange("Theme");
        }

        public string Header
        {
            get
            {
                return this.header;
            }

            set
            {
                if (value == this.header)
                {
                    return;
                }

                this.header = value;
                this.NotifyOfPropertyChange(() => this.Header);
            }
        }

        public bool IsOpen
        {
            get
            {
                return this.isOpen;
            }

            set
            {
                if (value.Equals(this.isOpen))
                {
                    return;
                }

                this.isOpen = value;
                this.NotifyOfPropertyChange(() => this.IsOpen);
            }
        }

        public Position Position
        {
            get
            {
                return this.position;
            }

            set
            {
                if (value == this.position)
                {
                    return;
                }

                this.position = value;
                using (var reg = new FlyoutRegistrySettings())
                {
                    reg.SetPosition(this.GetType(), value);
                }
                this.NotifyOfPropertyChange(() => this.Position);
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (value == this.name)
                {
                    return;
                }
                this.name = value;
                this.NotifyOfPropertyChange(() => this.Name);
            }
        }

        public bool IsModal
        {
            get
            {
                return this.isModal;
            }
            set
            {
                if (value.Equals(this.isModal))
                {
                    return;
                }
                this.isModal = value;
                this.NotifyOfPropertyChange(() => this.IsModal);
            }
        }

        public FlyoutTheme Theme
        {
            get
            {
                return themeManager.FlyoutTheme;
            }
        }

        protected internal void SetPosition(Position defaultPosition)
        {
            using (var reg = new FlyoutRegistrySettings())
            {
                this.Position = reg.GetPosition(this.GetType(), defaultPosition);
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
                    this.inner.SetPosition(defaultPosition);
                }

                public string Header
                {
                    get
                    {
                        return this.inner.Header;
                    }
                    set
                    {
                        this.inner.Header = value;
                    }
                }

                public bool IsOpen
                {
                    get
                    {
                        return this.inner.IsOpen;
                    }
                    set
                    {
                        this.inner.IsOpen = value;
                    }
                }

                public Position Position
                {
                    get
                    {
                        return this.inner.Position;
                    }
                    set
                    {
                        this.inner.Position = value;
                    }
                }

                public string Name
                {
                    get
                    {
                        return this.inner.Name;
                    }
                    set
                    {
                        this.inner.Name = value;
                    }
                }

                public bool IsModal
                {
                    get
                    {
                        return this.inner.IsModal;
                    }
                    set
                    {
                        this.inner.IsModal = value;
                    }
                }

                public FlyoutTheme Theme
                {
                    get
                    {
                        return this.inner.Theme;
                    }
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
                    get
                    {
                        return this.inner.Header;
                    }
                    set
                    {
                        this.inner.Header = value;
                    }
                }

                public bool IsOpen
                {
                    get
                    {
                        return this.inner.IsOpen;
                    }
                    set
                    {
                        this.inner.IsOpen = value;
                    }
                }

                public Position Position
                {
                    get
                    {
                        return this.inner.Position;
                    }
                    set
                    {
                        this.inner.Position = value;
                    }
                }

                public string Name
                {
                    get
                    {
                        return this.inner.Name;
                    }
                    set
                    {
                        this.inner.Name = value;
                    }
                }

                public bool IsModal
                {
                    get
                    {
                        return this.inner.IsModal;
                    }
                    set
                    {
                        this.inner.IsModal = value;
                    }
                }

                public FlyoutTheme Theme
                {
                    get
                    {
                        return this.inner.Theme;
                    }
                }

                protected void SetPosition(Position defaultPosition)
                {
                    this.inner.SetPosition(defaultPosition);
                }
            }
        }

        public string Header
        {
            get
            {
                return this.inner.Header;
            }
            set
            {
                this.inner.Header = value;
            }
        }

        public Position Position
        {
            get
            {
                return this.inner.Position;
            }
            set
            {
                this.inner.Position = value;
            }
        }

        public bool IsOpen
        {
            get
            {
                return this.inner.IsOpen;
            }
            set
            {
                this.inner.IsOpen = value;
            }
        }

        public string Name
        {
            get
            {
                return this.inner.Name;
            }
            set
            {
                this.inner.Name = value;
            }
        }

        public bool IsModal
        {
            get
            {
                return this.inner.IsModal;
            }
            set
            {
                this.inner.IsModal = value;
            }
        }

        public FlyoutTheme Theme
        {
            get
            {
                return this.inner.Theme;
            }
        }

        protected void SetPosition(Position defaultPosition)
        {
            this.inner.SetPosition(defaultPosition);
        }
    }
}