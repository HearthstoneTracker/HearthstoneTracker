using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Caliburn.Micro;
using HearthCap.Data;
using Omu.ValueInjecter;
using LogManager = NLog.LogManager;

namespace HearthCap.Features.TextFiles
{
    [Export(typeof(TextFilesManager))]
    public class TextFilesManager
    {
        private readonly NLog.Logger Log = LogManager.GetCurrentClassLogger();

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

            LoadTextTemplates();

            foreach (var listener in textFilesEventsListeners)
            {
                listener.Templates = templates;
                listener.Manager = this;
            }
        }

        public IEnumerable<TextFilesEventsListener> Listeners
        {
            get { return textFilesEventsListeners; }
        }

        public BindableCollection<TextFileModel> Templates
        {
            get { return templates; }
        }

        private void LoadTextTemplates()
        {
            var tpls = repository.ToList(q => q.OrderBy(x => x.Filename));
            templates.IsNotifying = false;
            foreach (var tpl in tpls)
            {
                var model = new TextFileModel();
                model.InjectFrom(tpl);
                templates.Add(model);
            }
            templates.IsNotifying = true;
            templates.Refresh();
        }

        public void Refresh()
        {
            foreach (var textFileModel in templates)
            {
                var content = textFileModel.Template;

                foreach (var listener in Listeners)
                {
                    if (listener.ShouldHandle(content))
                    {
                        content = listener.Handle(content);
                    }
                }

                // TODO: this will fail if we have multiple same filenames, but that 'should' not happen
                WriteFile(textFileModel, content);
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
