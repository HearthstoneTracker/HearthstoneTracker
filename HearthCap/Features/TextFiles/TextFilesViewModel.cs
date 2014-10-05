// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextFilesViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The text files view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.TextFiles
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Threading.Tasks;

    using Caliburn.Micro;

    using HearthCap.Data;
    using HearthCap.Features.Analytics;
    using HearthCap.Shell.Flyouts;

    using Microsoft.Win32;

    using Omu.ValueInjecter;

    /// <summary>
    /// The text files view model.
    /// </summary>
    [Export(typeof(IFlyout))]
    public class TextFilesViewModel : FlyoutViewModel
    {
        /// <summary>
        /// The events.
        /// </summary>
        private readonly IEventAggregator events;

        /// <summary>
        /// The repository.
        /// </summary>
        private readonly IRepository<TextFileTemplate> repository;

        /// <summary>
        /// The text files manager.
        /// </summary>
        private readonly TextFilesManager textFilesManager;

        /// <summary>
        /// The templates.
        /// </summary>
        private readonly BindableCollection<TextFileModel> templates = new BindableCollection<TextFileModel>();

        /// <summary>
        /// The selected template.
        /// </summary>
        private TextFileModel selectedTemplate;

        /// <summary>
        /// The variables.
        /// </summary>
        private readonly BindableCollection<KeyValuePair<string, string>> variables = new BindableCollection<KeyValuePair<string, string>>();

        /// <summary>
        /// The selected variable.
        /// </summary>
        private KeyValuePair<string, string>? selectedVariable;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextFilesViewModel"/> class.
        /// </summary>
        /// <param name="events">
        /// The events.
        /// </param>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="textFilesManager">
        /// The text files manager.
        /// </param>
        [ImportingConstructor]
        public TextFilesViewModel(IEventAggregator events, 
            IRepository<TextFileTemplate> repository, 
            TextFilesManager textFilesManager)
        {
            this.Name = Flyouts.TextFiles;
            this.Header = this.DisplayName = "Auto generated text files:";

            this.events = events;
            this.repository = repository;
            this.textFilesManager = textFilesManager;

            events.Subscribe(this);
            this.templates = textFilesManager.Templates;
            this.InitializeVariables(textFilesManager.Listeners);
            this.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "IsOpen" && this.IsOpen)
                {
                    Tracker.TrackEventAsync(Tracker.FlyoutsCategory, "Open", this.Name, 1);
                }
            };
        }

        /// <summary>
        /// The initialize variables.
        /// </summary>
        /// <param name="filesEventsListeners">
        /// The files events listeners.
        /// </param>
        private void InitializeVariables(IEnumerable<TextFilesEventsListener> filesEventsListeners)
        {
            foreach (var textFilesEventsListener in filesEventsListeners)
            {
                this.variables.AddRange(textFilesEventsListener.Variables);
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
        /// Gets or sets the selected template.
        /// </summary>
        public TextFileModel SelectedTemplate
        {
            get
            {
                return this.selectedTemplate;
            }

            set
            {
                if (Equals(value, this.selectedTemplate))
                {
                    return;
                }

                this.selectedTemplate = value;
                this.NotifyOfPropertyChange(() => this.SelectedTemplate);
            }
        }

        /// <summary>
        /// Gets the variables.
        /// </summary>
        public IObservableCollection<KeyValuePair<string, string>> Variables
        {
            get
            {
                return this.variables;
            }
        }

        /// <summary>
        /// Gets or sets the selected variable.
        /// </summary>
        public KeyValuePair<string, string>? SelectedVariable
        {
            get
            {
                return this.selectedVariable;
            }

            set
            {
                if (value.Equals(this.selectedVariable))
                {
                    return;
                }

                this.selectedVariable = value;
                this.NotifyOfPropertyChange(() => this.SelectedVariable);
            }
        }

        /// <summary>
        /// The choose file.
        /// </summary>
        public void ChooseFile()
        {
            string file;
            if (this.OpenSaveAsDialog(out file) == true)
            {
                this.SelectedTemplate.Filename = file;
            }
        }

        /// <summary>
        /// The add new.
        /// </summary>
        public void AddNew()
        {
            string file;
            if (this.OpenSaveAsDialog(out file) == true)
            {
                var model = new TextFileModel {
                                    Filename = file, 
                                    Template = "%carena_wins% - %carena_losses%"
                                };
                this.Templates.Add(model);
                this.SelectedTemplate = model;
                this.Save();
            }
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task Delete()
        {
            if (this.SelectedTemplate == null) return;
            await this.repository.Delete(this.SelectedTemplate.Id);
            this.Templates.Remove(this.SelectedTemplate);
            this.SelectedTemplate = null;
        }

        /// <summary>
        /// The insert variable.
        /// </summary>
        public void InsertVariable()
        {
            if (this.SelectedTemplate == null || this.SelectedVariable == null) return;

            this.SelectedTemplate.Template += this.SelectedVariable.Value.Key;
        }

        /// <summary>
        /// The save.
        /// </summary>
        public async void Save()
        {
            var entity = new TextFileTemplate();
            entity.InjectFrom(this.SelectedTemplate);
            await this.repository.SaveOrUpdateAsync(entity, entity.Id);
            this.textFilesManager.Refresh();
        }

        /// <summary>
        /// The open save as dialog.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <returns>
        /// The <see cref="bool?"/>.
        /// </returns>
        private bool? OpenSaveAsDialog(out string file)
        {
            var dlg = new SaveFileDialog
                          {
                              FileName = "filename", 
                              DefaultExt = ".txt", 
                              Filter = "Text documents |*.txt"
                          };

            var result = dlg.ShowDialog();
            file = dlg.FileName;
            return result;
        }

        /// <summary>
        /// Called when initializing.
        /// </summary>
        protected override async void OnInitialize()
        {
        }
    }
}
