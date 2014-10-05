// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FlyoutViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The flyout view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Shell.Flyouts
{
    using Caliburn.Micro;

    using HearthCap.Shell.Theme;
    using HearthCap.StartUp;

    using MahApps.Metro.Controls;

    /// <summary>
    /// The flyout view model.
    /// </summary>
    public abstract class FlyoutViewModel : Screen, IFlyout
    {
        /// <summary>
        /// The header.
        /// </summary>
        private string header;

        /// <summary>
        /// The is open.
        /// </summary>
        private bool isOpen;

        /// <summary>
        /// The position.
        /// </summary>
        private Position position;

        /// <summary>
        /// The name.
        /// </summary>
        private string name;

        /// <summary>
        /// The is modal.
        /// </summary>
        private bool isModal;

        /// <summary>
        /// The theme manager.
        /// </summary>
        private readonly IThemeManager themeManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlyoutViewModel"/> class.
        /// </summary>
        protected FlyoutViewModel()
        {
            this.themeManager = AppBootstrapper.Container.GetExportedValue<IThemeManager>();
            this.themeManager.FlyoutThemeChanged += (sender, args) => this.NotifyOfPropertyChange("Theme");
        }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether is open.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether is modal.
        /// </summary>
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

        /// <summary>
        /// Gets the theme.
        /// </summary>
        public FlyoutTheme Theme
        {
            get
            {
                return this.themeManager.FlyoutTheme;
            }
        }

        /// <summary>
        /// The set position.
        /// </summary>
        /// <param name="defaultPosition">
        /// The default position.
        /// </param>
        protected internal void SetPosition(Position defaultPosition)
        {
            using (var reg = new FlyoutRegistrySettings())
            {
                this.Position = reg.GetPosition(this.GetType(), defaultPosition);
            }
        }
    }

    /// <summary>
    /// The flyout view model.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public abstract class FlyoutViewModel<T> : Conductor<T>, IFlyout
        where T : class
    {
        /// <summary>
        /// The inner.
        /// </summary>
        private readonly InnerFlyout inner = new InnerFlyout();

        /// <summary>
        /// The inner flyout.
        /// </summary>
        private class InnerFlyout : FlyoutViewModel
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlyoutViewModel{T}"/> class.
        /// </summary>
        protected FlyoutViewModel()
        {
            this.inner.PropertyChanged += (sender, args) => this.NotifyOfPropertyChange(args.PropertyName);
        }

        /// <summary>
        /// The collection.
        /// </summary>
        public new class Collection
        {
            /// <summary>
            /// The one active.
            /// </summary>
            public class OneActive : Conductor<T>.Collection.OneActive, IFlyout
            {
                /// <summary>
                /// The inner.
                /// </summary>
                private readonly InnerFlyout inner = new InnerFlyout();

                /// <summary>
                /// Initializes a new instance of the <see cref="OneActive"/> class.
                /// </summary>
                public OneActive()
                {
                    this.inner.PropertyChanged += (sender, args) => this.NotifyOfPropertyChange(args.PropertyName);
                }

                /// <summary>
                /// The set position.
                /// </summary>
                /// <param name="defaultPosition">
                /// The default position.
                /// </param>
                protected void SetPosition(Position defaultPosition)
                {
                    this.inner.SetPosition(defaultPosition);
                }

                /// <summary>
                /// Gets or sets the header.
                /// </summary>
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

                /// <summary>
                /// Gets or sets a value indicating whether is open.
                /// </summary>
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

                /// <summary>
                /// Gets or sets the position.
                /// </summary>
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

                /// <summary>
                /// Gets or sets the name.
                /// </summary>
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

                /// <summary>
                /// Gets or sets a value indicating whether is modal.
                /// </summary>
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

                /// <summary>
                /// Gets the theme.
                /// </summary>
                public FlyoutTheme Theme
                {
                    get
                    {
                        return this.inner.Theme;
                    }
                }
            }

            /// <summary>
            /// The all active.
            /// </summary>
            public class AllActive : Conductor<T>.Collection.AllActive, IFlyout
            {
                /// <summary>
                /// The inner.
                /// </summary>
                private readonly InnerFlyout inner = new InnerFlyout();

                /// <summary>
                /// Initializes a new instance of the <see cref="AllActive"/> class.
                /// </summary>
                public AllActive()
                {
                    this.inner.PropertyChanged += (sender, args) => this.NotifyOfPropertyChange(args.PropertyName);
                }

                /// <summary>
                /// Gets or sets the header.
                /// </summary>
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

                /// <summary>
                /// Gets or sets a value indicating whether is open.
                /// </summary>
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

                /// <summary>
                /// Gets or sets the position.
                /// </summary>
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

                /// <summary>
                /// Gets or sets the name.
                /// </summary>
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

                /// <summary>
                /// Gets or sets a value indicating whether is modal.
                /// </summary>
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

                /// <summary>
                /// Gets the theme.
                /// </summary>
                public FlyoutTheme Theme
                {
                    get
                    {
                        return this.inner.Theme;
                    }
                }

                /// <summary>
                /// The set position.
                /// </summary>
                /// <param name="defaultPosition">
                /// The default position.
                /// </param>
                protected void SetPosition(Position defaultPosition)
                {
                    this.inner.SetPosition(defaultPosition);
                }
            }
        }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether is open.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether is modal.
        /// </summary>
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

        /// <summary>
        /// Gets the theme.
        /// </summary>
        public FlyoutTheme Theme
        {
            get
            {
                return this.inner.Theme;
            }
        }

        /// <summary>
        /// The set position.
        /// </summary>
        /// <param name="defaultPosition">
        /// The default position.
        /// </param>
        protected void SetPosition(Position defaultPosition)
        {
            this.inner.SetPosition(defaultPosition);
        }
    }
}