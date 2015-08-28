namespace HearthCap.Features.TextFiles
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Caliburn.Micro;

    using HearthCap.Data;

    using NLog;

    using Omu.ValueInjecter;

    [Export(typeof(TextFilesManager))]
    public class TextFilesManager
    {
        private Logger Log = NLog.LogManager.GetCurrentClassLogger();

        private readonly IEventAggregator events;

        private readonly IRepository<TextFileTemplate> repository;

        private readonly TextFilesEventsListener[] textFilesEventsListeners;

        private readonly BindableCollection<TextFileModel> templates = new BindableCollection<TextFileModel>();

        private static readonly object fileLock = new object();

        [ImportingConstructor]
        public TextFilesManager(
            IEventAggregator events,
            IRepository<TextFileTemplate> repository,
            [ImportMany] TextFilesEventsListener[] textFilesEventsListeners)
        {
            this.events = events;
            this.repository = repository;
            this.textFilesEventsListeners = textFilesEventsListeners;

            this.LoadTextTemplates();

            foreach (var listener in textFilesEventsListeners)
            {
                listener.Templates = this.templates;
                listener.Manager = this;
            }
        }

        public IEnumerable<TextFilesEventsListener> Listeners
        {
            get
            {
                return this.textFilesEventsListeners;
            }
        }

        public BindableCollection<TextFileModel> Templates
        {
            get
            {
                return this.templates;
            }
        }

        private void LoadTextTemplates()
        {
            var tpls = this.repository.ToList((q) => q.OrderBy(x => x.Filename));
            this.templates.IsNotifying = false;
            foreach (var tpl in tpls)
            {
                var model = new TextFileModel();
                model.InjectFrom(tpl);
                this.templates.Add(model);
            }
            this.templates.IsNotifying = true;
            this.templates.Refresh();
        }

        public void Refresh()
        {
            foreach (var textFileModel in this.templates)
            {
                string content = textFileModel.Template;

                foreach (var listener in this.Listeners)
                {
                    if (listener.ShouldHandle(content))
                    {
                        content = listener.Handle(content);
                    }
                }

                // TODO: this will fail if we have multiple same filenames, but that 'should' not happen
                this.WriteFile(textFileModel, content);
            }
        }

        private void WriteFile(TextFileModel template, string content)
        {
            try
            {
                lock (fileLock)
                {
                    using (var file = File.CreateText(template.Filename))
                    {
                        file.Write(content);
                    }                    
                }
            }
            catch (Exception ex)
            {
                // Fail silently :-)
                Log.Error(ex.ToString);
            }
        }
    }
}