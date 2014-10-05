// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextFilesEventsListener.cs" company="">
//   
// </copyright>
// <summary>
//   The text files events listener.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.TextFiles
{
    using System.Collections.Generic;
    using System.Collections.Specialized;

    using Caliburn.Micro;

    /// <summary>
    /// The text files events listener.
    /// </summary>
    public abstract class TextFilesEventsListener
    {
        /// <summary>
        /// The templates.
        /// </summary>
        private IObservableCollection<TextFileModel> templates = new BindableCollection<TextFileModel>();

        /// <summary>
        /// The variables.
        /// </summary>
        private readonly IList<KeyValuePair<string, string>> variables = new List<KeyValuePair<string, string>>();

        /// <summary>
        /// Gets or sets the manager.
        /// </summary>
        protected internal TextFilesManager Manager { get; set; }

        /// <summary>
        /// Gets the variables.
        /// </summary>
        public IList<KeyValuePair<string, string>> Variables
        {
            get
            {
                return this.variables;
            }
        }

        /// <summary>
        /// Gets or sets the templates.
        /// </summary>
        public IObservableCollection<TextFileModel> Templates
        {
            get
            {
                return this.templates;
            }

            set
            {
                if (this.templates != null)
                {
                    this.templates.CollectionChanged -= this.TemplatesChanged;
                }

                this.templates = value;
                this.templates.CollectionChanged += this.TemplatesChanged;
            }
        }

        /// <summary>
        /// The refresh.
        /// </summary>
        protected void Refresh()
        {
            if (this.Manager != null)
            {
                this.Manager.Refresh();
            }
        }

        /// <summary>
        /// The should handle.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected internal abstract bool ShouldHandle(string content);

        /// <summary>
        /// The handle.
        /// </summary>
        /// <param name="currentContent">
        /// The current content.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected internal abstract string Handle(string currentContent);

        /// <summary>
        /// The templates changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="notifyCollectionChangedEventArgs">
        /// The notify collection changed event args.
        /// </param>
        protected virtual void TemplatesChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            // TODO: very naive. should only handle changed items
            this.Refresh();
        }
    }
}