// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextFilesManager.cs" company="">
//   
// </copyright>
// <summary>
//   The text files manager.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.TextFiles
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.IO;
    using System.Linq;

    using Caliburn.Micro;

    using HearthCap.Data;

    using NLog;

    using Omu.ValueInjecter;

    using LogManager = NLog.LogManager;

    /// <summary>
    /// The text files manager.
    /// </summary>
    [Export(typeof(TextFilesManager))]
    public class TextFilesManager
    {
        /// <summary>
        /// The log.
        /// </summary>
        private Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The events.
        /// </summary>
        private readonly IEventAggregator events;

        /// <summary>
        /// The repository.
        /// </summary>
        private readonly IRepository<TextFileTemplate> repository;

        /// <summary>
        /// The text files events listeners.
        /// </summary>
        private readonly TextFilesEventsListener[] textFilesEventsListeners;

        /// <summary>
        /// The templates.
        /// </summary>
        private readonly BindableCollection<TextFileModel> templates = new BindableCollection<TextFileModel>();

        /// <summary>
        /// The file lock.
        /// </summary>
        private static readonly object fileLock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="TextFilesManager"/> class.
        /// </summary>
        /// <param name="events">
        /// The events.
        /// </param>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="textFilesEventsListeners">
        /// The text files events listeners.
        /// </param>
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

        /// <summary>
        /// Gets the listeners.
        /// </summary>
        public IEnumerable<TextFilesEventsListener> Listeners
        {
            get
            {
                return this.textFilesEventsListeners;
            }
        }

        /// <summary>
        /// Gets the templates.
        /// </summary>
        public BindableCollection<TextFileModel> Templates
        {
            get
            {
                return this.templates;
            }
        }

        /// <summary>
        /// The load text templates.
        /// </summary>
        private void LoadTextTemplates()
        {
            var tpls = this.repository.ToList(q => q.OrderBy(x => x.Filename));
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

        /// <summary>
        /// The refresh.
        /// </summary>
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

        /// <summary>
        /// The write file.
        /// </summary>
        /// <param name="template">
        /// The template.
        /// </param>
        /// <param name="content">
        /// The content.
        /// </param>
        private void WriteFile(TextFileModel template, string content)
        {
            try
            {
                lock (fileLock)
                {
                    using (var file = File.CreateText(template.Filename))
                    {
                        file.Write(content);
                        file.Close();
                    }                    
                }
            }
            catch (Exception ex)
            {
                // Fail silently :-)
                this.Log.Error(ex.ToString);
            }
        }
    }
}