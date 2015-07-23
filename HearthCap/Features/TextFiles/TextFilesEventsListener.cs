namespace HearthCap.Features.TextFiles
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;

    using Caliburn.Micro;

    public abstract class TextFilesEventsListener
    {
        private IObservableCollection<TextFileModel> templates = new BindableCollection<TextFileModel>();

        private readonly IList<KeyValuePair<string, string>> variables = new List<KeyValuePair<string, string>>();

        protected internal TextFilesManager Manager { get; set; }

        public IList<KeyValuePair<string, string>> Variables
        {
            get
            {
                return this.variables;
            }
        }

        public IObservableCollection<TextFileModel> Templates
        {
            get
            {
                return this.templates;
            }
            set
            {
                if (templates != null)
                {
                    templates.CollectionChanged -= TemplatesChanged;
                }
                this.templates = value;
                templates.CollectionChanged += TemplatesChanged;
            }
        }

        protected void Refresh()
        {
            if (this.Manager != null)
            {
                Manager.Refresh();
            }
        }

        protected internal abstract bool ShouldHandle(string content);
        protected internal abstract string Handle(string currentContent);

        protected virtual void TemplatesChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            // TODO: very naive. should only handle changed items
            this.Refresh();
        }
    }
}