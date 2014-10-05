// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FlyoutAttribute.cs" company="">
//   
// </copyright>
// <summary>
//   The flyout attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Shell.Flyouts
{
    using System;
    using System.ComponentModel.Composition;

    using Caliburn.Micro;

    using HearthCap.Shell.Theme;

    using MahApps.Metro.Controls;

    /// <summary>
    /// The flyout attribute.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class FlyoutAttribute : ExportAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FlyoutAttribute"/> class.
        /// </summary>
        public FlyoutAttribute() : base(typeof(IFlyout)) { }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the default position.
        /// </summary>
        public Position DefaultPosition { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is modal.
        /// </summary>
        public bool IsModal { get; set; }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        public string Header { get; set; }
    }

    /// <summary>
    /// The FlyoutMetadata interface.
    /// </summary>
    public interface IFlyoutMetadata
    {
        /// <summary>
        /// Gets the key.
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Gets the default position.
        /// </summary>
        Position DefaultPosition { get; }

        /// <summary>
        /// Gets a value indicating whether is modal.
        /// </summary>
        bool IsModal { get; }

        /// <summary>
        /// Gets the header.
        /// </summary>
        string Header { get; }
    }

    /// <summary>
    /// The flyout.
    /// </summary>
    public class Flyout : Conductor<IScreen>
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
        /// The model.
        /// </summary>
        private IScreen model;

        /// <summary>
        /// Initializes a new instance of the <see cref="Flyout"/> class.
        /// </summary>
        /// <param name="themeManager">
        /// The theme manager.
        /// </param>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
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
                    this.isOpen = true;
                    this.NotifyOfPropertyChange(() => this.IsOpen);
                };
            model.Deactivated += (sender, args) =>
                {
                    this.IsOpen = false;
                    this.NotifyOfPropertyChange(() => this.IsOpen);
                };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Flyout"/> class.
        /// </summary>
        /// <param name="themeManager">
        /// The theme manager.
        /// </param>
        /// <param name="metadata">
        /// The metadata.
        /// </param>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
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
                if (value)
                {
                    this.model.Activate();
                }
                else
                {
                    this.model.Deactivate(false);
                }

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
        /// Gets or sets the model.
        /// </summary>
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
        protected void SetPosition(Position defaultPosition)
        {
            using (var reg = new FlyoutRegistrySettings())
            {
                this.Position = reg.GetPosition(this.GetType(), defaultPosition);
            }
        }
    }
}