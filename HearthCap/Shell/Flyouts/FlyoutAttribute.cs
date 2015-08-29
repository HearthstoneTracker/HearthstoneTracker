using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using HearthCap.Shell.Theme;
using MahApps.Metro.Controls;

namespace HearthCap.Shell.Flyouts
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class)]
    public class FlyoutAttribute : ExportAttribute
    {
        public FlyoutAttribute()
            : base(typeof(IFlyout))
        {
        }

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
            if (themeManager == null)
            {
                throw new ArgumentNullException("themeManager");
            }
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

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
            if (themeManager == null)
            {
                throw new ArgumentNullException("themeManager");
            }
            if (metadata == null)
            {
                throw new ArgumentNullException("metadata");
            }
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            header = metadata.Header;
            position = metadata.DefaultPosition;
            name = metadata.Key;
            isModal = metadata.IsModal;
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
                if (value)
                {
                    model.Activate();
                }
                else
                {
                    model.Deactivate(false);
                }

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

        public IScreen Model
        {
            get { return model; }
            set
            {
                if (Equals(value, model))
                {
                    return;
                }
                model = value;
                NotifyOfPropertyChange(() => Model);
            }
        }

        public FlyoutTheme Theme
        {
            get { return themeManager.FlyoutTheme; }
        }

        protected void SetPosition(Position defaultPosition)
        {
            using (var reg = new FlyoutRegistrySettings())
            {
                Position = reg.GetPosition(GetType(), defaultPosition);
            }
        }
    }
}
