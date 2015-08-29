using System.Collections.Generic;
using System.Collections.Specialized;
using Caliburn.Micro;

namespace HearthCap.Features.TextFiles
{
    public abstract class TextFilesEventsListener
    {
        private IObservableCollection<TextFileModel> templates = new BindableCollection<TextFileModel>();

        private readonly IList<KeyValuePair<string, string>> variables = new List<KeyValuePair<string, string>>();

        protected internal TextFilesManager Manager { get; set; }

        public IList<KeyValuePair<string, string>> Variables
        {
            get { return variables; }
        }

        public IObservableCollection<TextFileModel> Templates
        {
            get { return templates; }
            set
            {
                if (templates != null)
                {
                    templates.CollectionChanged -= TemplatesChanged;
                }
                templates = value;
                templates.CollectionChanged += TemplatesChanged;
            }
        }

        protected void Refresh()
        {
            if (Manager != null)
            {
                Manager.Refresh();
            }
        }

        protected internal abstract bool ShouldHandle(string content);
        protected internal abstract string Handle(string currentContent);

        protected virtual void TemplatesChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            // TODO: very naive. should only handle changed items
            Refresh();
        }
    }
}
