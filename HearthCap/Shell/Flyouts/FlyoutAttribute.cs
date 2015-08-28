namespace HearthCap.Shell.Flyouts
{
    using System;
    using System.ComponentModel.Composition;

    using Caliburn.Micro;

    using HearthCap.Shell.Theme;

    using MahApps.Metro.Controls;

    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class FlyoutAttribute : ExportAttribute
    {
        public FlyoutAttribute() : base(typeof(IFlyout)) { }

        public string Key { get; set; }

        public Position DefaultPosition { get; set; }

        public bool IsModal { get; set; }

        public string Header { get; set; }
    }

    public interface IFlyoutMetadata
    {
        string Key { get; }

        Position DefaultPosition { get; }

        bool IsModal { get; }

        string Header { get; }
    }

    public class Flyout : Conductor<IScreen>
    {
        private string header;

        private bool isOpen;

        private Position position;

        private string name;

        private bool isModal;

        private readonly IThemeManager themeManager;

        private IScreen model;

        public Flyout(IThemeManager themeManager, IScreen model)
        {
            if (themeManager == null) { throw new ArgumentNullException("themeManager"); }
            if (model == null) { throw new ArgumentNullException("model"); }

            this.themeManager = themeManager;
            this.model = model;
            // this.ActivateWith(model);
            // this.DeactivateWith(model);
            model.Activated += (sender, args) =>
                {
                    isOpen = true;
                    NotifyOfPropertyChange(() => IsOpen);
                };
            model.Deactivated += (sender, args) =>
                {
                    IsOpen = false;
                    NotifyOfPropertyChange(() => IsOpen);
                };
        }

        public Flyout(IThemeManager themeManager, IFlyoutMetadata metadata, IScreen model)
            : this(themeManager, model)
        {
            if (themeManager == null) { throw new ArgumentNullException("themeManager"); }
            if (metadata == null) { throw new ArgumentNullException("metadata"); }
            if (model == null) { throw new ArgumentNullException("model"); }

            this.header = metadata.Header;
            this.position = metadata.DefaultPosition;
            this.name = metadata.Key;
            this.isModal = metadata.IsModal;
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
                if (value)
                {
                    model.Activate();
                }
                else
                {
                    model.Deactivate(false);
                }

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

        public IScreen Model
        {
            get
            {
                return this.model;
            }
            set
            {
                if (Equals(value, this.model))
                {
                    return;
                }
                this.model = value;
                this.NotifyOfPropertyChange(() => this.Model);
            }
        }

        public FlyoutTheme Theme
        {
            get
            {
                return themeManager.FlyoutTheme;
            }
        }

        protected void SetPosition(Position defaultPosition)
        {
            using (var reg = new FlyoutRegistrySettings())
            {
                this.Position = reg.GetPosition(this.GetType(), defaultPosition);
            }
        }
    }
}