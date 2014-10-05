// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeckSettingsViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The deck settings view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Decks
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media.Imaging;

    using Caliburn.Micro;
    using Caliburn.Micro.Recipes.Filters;

    using HearthCap.Core.GameCapture.HS.Commands;
    using HearthCap.Core.GameCapture.HS.Events;
    using HearthCap.Features.Core;
    using HearthCap.Features.Decks.ModelMappers;
    using HearthCap.Shell.Flyouts;

    using MahApps.Metro.Controls;

    using Microsoft.WindowsAPICodePack.Dialogs;

    /// <summary>
    /// The deck settings view model.
    /// </summary>
    [Export(typeof(IFlyout))]
    public class DeckSettingsViewModel : FlyoutViewModel, 
        IHandle<DeckScreenshotTaken>, 
        IHandle<SelectDeck>
    {
        /// <summary>
        /// The events.
        /// </summary>
        private readonly IEventAggregator events;

        /// <summary>
        /// The deck manager.
        /// </summary>
        private readonly IDeckManager deckManager;

        /// <summary>
        /// The decks.
        /// </summary>
        private readonly BindableCollection<DeckModel> decks = new BindableCollection<DeckModel>();

        /// <summary>
        /// The available decks.
        /// </summary>
        private readonly BindableCollection<AvailableDecksModel> availableDecks = new BindableCollection<AvailableDecksModel>();

        /// <summary>
        /// The servers.
        /// </summary>
        private readonly BindableServerCollection servers = BindableServerCollection.Instance;

        /// <summary>
        /// The selected deck.
        /// </summary>
        private DeckModel selectedDeck;

        /// <summary>
        /// The deck name.
        /// </summary>
        private string deckName;

        /// <summary>
        /// The deck slot.
        /// </summary>
        private string deckSlot;

        /// <summary>
        /// The selected server.
        /// </summary>
        private ServerItemModel selectedServer;

        /// <summary>
        /// The can take screenshot.
        /// </summary>
        private bool canTakeScreenshot;

        /// <summary>
        /// The deck screenshot.
        /// </summary>
        private BitmapImage deckScreenshot;

        /// <summary>
        /// The deck notes.
        /// </summary>
        private string deckNotes;

        /// <summary>
        /// The show hidden decks.
        /// </summary>
        private bool showHiddenDecks;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeckSettingsViewModel"/> class.
        /// </summary>
        /// <param name="events">
        /// The events.
        /// </param>
        /// <param name="deckManager">
        /// The deck manager.
        /// </param>
        [ImportingConstructor]
        public DeckSettingsViewModel(
            IEventAggregator events, 
            IDeckManager deckManager)
        {
            this.events = events;
            this.deckManager = deckManager;

            this.SetPosition(Position.Left);
            this.Name = Flyouts.Decks;
            this.Header = this.DisplayName = "Decks:";

            // note the setting of backing field instead of property, to not trigger deck refreshing yet.
            this.selectedServer = this.servers.Default ?? this.servers.First();
            events.Subscribe(this);
            this.CanTakeScreenshot = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether show hidden decks.
        /// </summary>
        public bool ShowHiddenDecks
        {
            get
            {
                return this.showHiddenDecks;
            }

            set
            {
                if (value.Equals(this.showHiddenDecks))
                {
                    return;
                }

                this.showHiddenDecks = value;
                this.NotifyOfPropertyChange(() => this.ShowHiddenDecks);
                this.RefreshDecks();
            }
        }

        /// <summary>
        /// Gets the available decks.
        /// </summary>
        public BindableCollection<AvailableDecksModel> AvailableDecks
        {
            get
            {
                return this.availableDecks;
            }
        }

        /// <summary>
        /// Gets the decks.
        /// </summary>
        public BindableCollection<DeckModel> Decks
        {
            get
            {
                return this.decks;
            }
        }

        /// <summary>
        /// Gets or sets the deck name.
        /// </summary>
        public string DeckName
        {
            get
            {
                return this.deckName;
            }

            set
            {
                if (value == this.deckName)
                {
                    return;
                }

                this.deckName = value;
                this.NotifyOfPropertyChange(() => this.DeckName);
            }
        }

        /// <summary>
        /// Gets the slots.
        /// </summary>
        public IList<string> Slots
        {
            get
            {
                var lst = new List<string>();
                lst.Add(string.Empty);
                for (int i = 1; i <= 9; i++)
                {
                    lst.Add(i.ToString(CultureInfo.InvariantCulture));
                }

                return lst;
            }
        }

        /// <summary>
        /// Gets or sets the deck slot.
        /// </summary>
        public string DeckSlot
        {
            get
            {
                return this.deckSlot;
            }

            set
            {
                if (value == this.deckSlot)
                {
                    return;
                }

                this.deckSlot = value;
                this.NotifyOfPropertyChange(() => this.DeckSlot);
            }
        }

        /// <summary>
        /// Gets or sets the deck notes.
        /// </summary>
        public string DeckNotes
        {
            get
            {
                return this.deckNotes;
            }

            set
            {
                if (value == this.deckNotes)
                {
                    return;
                }

                this.deckNotes = value;
                this.NotifyOfPropertyChange(() => this.DeckNotes);
            }
        }

        /// <summary>
        /// Gets or sets the selected deck.
        /// </summary>
        public DeckModel SelectedDeck
        {
            get
            {
                return this.selectedDeck;
            }

            set
            {
                if (Equals(value, this.selectedDeck))
                {
                    return;
                }

                this.selectedDeck = value;
                if (this.selectedDeck != null)
                {
                    this.DeckName = this.selectedDeck.Name;
                    this.DeckSlot = this.selectedDeck.Key;
                    this.DeckNotes = this.selectedDeck.Notes;

                    if (this.selectedDeck.Image != null)
                    {
                        this.SetDeckScreenshot(this.selectedDeck.Image.Image);
                    }
                    else
                    {
                        this.DeckScreenshot = null;
                    }
                }
                else
                {
                    this.DeckScreenshot = null;
                }

                this.CancelTakeScreenshot();
                this.NotifyOfPropertyChange(() => this.SelectedDeck);
                this.NotifyOfPropertyChange(() => this.CanDeleteDeck);
            }
        }

        /// <summary>
        /// Gets the servers.
        /// </summary>
        public BindableServerCollection Servers
        {
            get
            {
                return this.servers;
            }
        }

        /// <summary>
        /// Gets or sets the selected server.
        /// </summary>
        public ServerItemModel SelectedServer
        {
            get
            {
                return this.selectedServer;
            }

            set
            {
                if (Equals(value, this.selectedServer))
                {
                    return;
                }

                this.selectedServer = value;
                this.RefreshDecks();
                this.NotifyOfPropertyChange(() => this.SelectedServer);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether can take screenshot.
        /// </summary>
        public bool CanTakeScreenshot
        {
            get
            {
                return this.canTakeScreenshot;
            }

            set
            {
                if (value.Equals(this.canTakeScreenshot))
                {
                    return;
                }

                this.canTakeScreenshot = value;
                this.NotifyOfPropertyChange(() => this.CanTakeScreenshot);
            }
        }

        /// <summary>
        /// Gets or sets the deck screenshot.
        /// </summary>
        public BitmapImage DeckScreenshot
        {
            get
            {
                return this.deckScreenshot;
            }

            set
            {
                this.deckScreenshot = value;
                this.NotifyOfPropertyChange(() => this.DeckScreenshot);
            }
        }

        /// <summary>
        /// The take screenshot.
        /// </summary>
        public void TakeScreenshot()
        {
            this.CanTakeScreenshot = false;
            this.events.PublishOnBackgroundThread(new RequestDeckScreenshot());
        }

        /// <summary>
        /// The cancel take screenshot.
        /// </summary>
        public void CancelTakeScreenshot()
        {
            if (this.CanTakeScreenshot == false)
            {
                this.CanTakeScreenshot = true;
                this.events.PublishOnBackgroundThread(new RequestDeckScreenshot {
                    Cancel = true
                });
            }
        }

        /// <summary>
        /// The add deck.
        /// </summary>
        public void AddDeck()
        {
            var deck = new DeckModel {
                               Name = "New deck", 
                               Server = this.SelectedServer.Name
                           };
            this.deckManager.AddDeck(deck);

            this.RefreshDecks();
            this.SelectedDeck = this.Decks.FirstOrDefault(x => x.Id == deck.Id);
        }

        /// <summary>
        /// The delete deck.
        /// </summary>
        [Dependencies("SelectedDeck")]
        public void DeleteDeck()
        {
            if (MessageBox.Show("Delete this deck?", "Delete this deck?", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            {
                return;
            }

            if (!string.IsNullOrEmpty(this.SelectedDeck.Key))
            {
                this.RemoveFromSlot(this.SelectedDeck.Key);
            }

            this.deckManager.DeleteDeck(this.SelectedDeck.Id);
            this.SelectedDeck = null;
            this.RefreshDecks();
        }

        /// <summary>
        /// The undelete deck.
        /// </summary>
        [Dependencies("SelectedDeck")]
        public void UndeleteDeck()
        {
            this.deckManager.UndeleteDeck(this.SelectedDeck.Id);

            this.RefreshDecks();
        }

        /// <summary>
        /// Gets a value indicating whether can delete deck.
        /// </summary>
        public bool CanDeleteDeck
        {
            get
            {
                return this.SelectedDeck != null && !this.SelectedDeck.Deleted;
            }
        }

        /// <summary>
        /// The update deck.
        /// </summary>
        /// <param name="suppressRefresh">
        /// The suppress refresh.
        /// </param>
        [Dependencies("DeckName", "HasNewScreenshot", "DeckSlot", "DeckNotes")]
        public void UpdateDeck(bool suppressRefresh = false)
        {
            this.SelectedDeck.Name = this.DeckName;
            this.SelectedDeck.Key = this.DeckSlot;
            this.SelectedDeck.Notes = this.DeckNotes;
            this.deckManager.UpdateDeck(this.SelectedDeck);

            // UpdateDeckImageFromBitmapImage(SelectedDeck);
            this.NotifyOfPropertyChange(() => this.DeckName);
            if (!suppressRefresh)
            {
                this.RefreshDecks();
            }
        }

        /// <summary>
        /// The update deck image from bitmap image.
        /// </summary>
        /// <param name="deckModel">
        /// The deck model.
        /// </param>
        private void UpdateDeckImageFromBitmapImage(DeckModel deckModel)
        {
            if (this.DeckScreenshot == null)
            {
                return;
            }

            using (var ms = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(this.DeckScreenshot));
                enc.Save(ms);
                ms.Position = 0;
                this.deckManager.UpdateDeckImage(deckModel.Id, ms.ToArray());
            }
        }

        /// <summary>
        /// The save as new deck.
        /// </summary>
        [Dependencies("DeckName")]
        public void SaveAsNewDeck()
        {
            var deck = new DeckModel {
                Name = this.DeckName, 
                Server = this.SelectedServer.Name, 
                Key = this.DeckSlot, 
                Notes = this.DeckNotes
            };
            this.deckManager.AddDeck(deck);
            this.UpdateDeckImageFromBitmapImage(deck);

            this.RefreshDecks();
            this.SelectedDeck = this.Decks.FirstOrDefault(x => x.Id == deck.Id);
        }

        /// <summary>
        /// The can save as new deck.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CanSaveAsNewDeck()
        {
            return !string.IsNullOrEmpty(this.DeckName);
        }

        /// <summary>
        /// The can update deck.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CanUpdateDeck()
        {
            return !string.IsNullOrEmpty(this.DeckName) && this.HasChanged();
        }

        /// <summary>
        /// The has changed.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool HasChanged()
        {
            return this.SelectedDeck.Name != this.DeckName ||
                   this.SelectedDeck.Key != this.DeckSlot ||
                   this.SelectedDeck.Notes != this.DeckNotes;
        }

        /// <summary>
        /// The remove from slot.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        protected void RemoveFromSlot(AvailableDecksModel model)
        {
            RemoveFromSlot(model.Slot);
        }

        /// <summary>
        /// The remove from slot.
        /// </summary>
        /// <param name="slot">
        /// The slot.
        /// </param>
        protected void RemoveFromSlot(string slot)
        {
            var start = Convert.ToInt32(slot) - 1;
            if (this.AvailableDecks[start].SelectedDeck != null)
            {
                this.AvailableDecks[start].SelectedDeck.Key = string.Empty;
                this.deckManager.UpdateDeck(this.AvailableDecks[start].SelectedDeck, true);
            }

            for (int i = start + 1; i < 9; i++)
            {
                if (this.AvailableDecks[i].SelectedDeck != null)
                {
                    this.AvailableDecks[i].SelectedDeck.Key = i.ToString(CultureInfo.InvariantCulture);
                    this.deckManager.UpdateDeck(this.AvailableDecks[i].SelectedDeck, true);
                }
            }

            // events.PublishOnBackgroundThread(new DecksUpdated(SelectedServer.Name));
        }

        /// <summary>
        /// The cancel.
        /// </summary>
        public void Cancel()
        {
            this.SelectedDeck = null;
        }

        /// <summary>
        /// The save screenshot to disk.
        /// </summary>
        public void SaveScreenshotToDisk()
        {
            if (this.SelectedDeck == null || this.DeckScreenshot == null)
                return;

            var invalid = new string(Path.GetInvalidFileNameChars());
            var defaultFilename = invalid.Aggregate(this.DeckName, (current, c) => current.Replace(c.ToString(CultureInfo.InvariantCulture), "_"));
            defaultFilename += ".png";
            var dialog = new CommonSaveFileDialog
                             {
                                 OverwritePrompt = true, 
                                 DefaultExtension = ".png", 
                                 DefaultFileName = defaultFilename, 
                                 EnsureValidNames = true, 
                                 Title = "Save deck screenshot", 
                                 AllowPropertyEditing = false, 
                                 RestoreDirectory = true, 
                                 Filters =
                                     {
                                         new CommonFileDialogFilter("PNG", ".png")
                                     }
                             };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var filename = dialog.FileName;
                using (var fs = File.Create(filename))
                {
                    BitmapEncoder enc = new PngBitmapEncoder();
                    enc.Frames.Add(BitmapFrame.Create(this.DeckScreenshot));
                    enc.Save(fs);
                }
            }
        }

        /// <summary>
        /// The refresh decks.
        /// </summary>
        private void RefreshDecks()
        {
            // deckManager.ClearCache();
            var oldSelected = this.SelectedDeck;
            var d = this.deckManager.GetDecks(this.SelectedServer.Name, this.ShowHiddenDecks);
            this.Decks.Clear();
            this.Decks.AddRange(d.ToModel());
            if (oldSelected != null)
            {
                this.SelectedDeck = this.Decks.FirstOrDefault(x => x.Id == oldSelected.Id);
            }

            this.RefreshAvailableDecks();
        }

        /// <summary>
        /// The refresh available decks.
        /// </summary>
        private void RefreshAvailableDecks()
        {
            if (this.AvailableDecks.Count == 0)
            {
                for (int i = 1; i <= 9; i++)
                {
                    var key = i.ToString(CultureInfo.InvariantCulture);
                    var model = new AvailableDecksModel {
                                        Slot = key, 
                                        AvailableDecks =
                                            new BindableCollection<DeckModel>(this.Decks.Where(x => x.Key == key || string.IsNullOrEmpty(x.Key))), 
                                        SelectedDeck = this.Decks.FirstOrDefault(x => x.Key == key)
                                    };
                    model.PropertyChanged += this.AvailableDecksModel_OnPropertyChanged;
                    this.AvailableDecks.Add(model);
                }

                this.NotifyOfPropertyChange(() => this.AvailableDecks);
            }
            else
            {
                for (int i = 1; i <= 9; i++)
                {
                    var key = i.ToString(CultureInfo.InvariantCulture);
                    this.AvailableDecks[i - 1].AvailableDecks =
                        new BindableCollection<DeckModel>(this.Decks.Where(x => x.Key == key || string.IsNullOrEmpty(x.Key)));
                    this.AvailableDecks[i - 1].PropertyChanged -= this.AvailableDecksModel_OnPropertyChanged;
                    this.AvailableDecks[i - 1].SelectedDeck = this.Decks.FirstOrDefault(x => x.Key == key);
                    this.AvailableDecks[i - 1].PropertyChanged += this.AvailableDecksModel_OnPropertyChanged;
                }

                this.NotifyOfPropertyChange(() => this.AvailableDecks);
            }
        }

        /// <summary>
        /// Called when initializing.
        /// </summary>
        protected override void OnInitialize()
        {
            this.RefreshDecks();
        }

        /// <summary>
        /// The available decks model_ on property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        private void AvailableDecksModel_OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "SelectedDeck")
            {
                var model = (AvailableDecksModel)sender;
                if (model.SelectedDeck != null)
                {
                    model.SelectedDeck.Key = model.Slot;
                    this.deckManager.UpdateDeck(model.SelectedDeck);
                    this.RefreshDecks();
                }
            }
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(DeckScreenshotTaken message)
        {
            if (this.SelectedDeck == null)
                return;

            SetDeckScreenshot(message.Image);
            message.Image.Dispose();

            this.CanTakeScreenshot = true;
            this.UpdateDeck(true);
            this.UpdateDeckImageFromBitmapImage(this.SelectedDeck);
            this.RefreshDecks();
        }

        /// <summary>
        /// The set deck screenshot.
        /// </summary>
        /// <param name="image">
        /// The image.
        /// </param>
        private void SetDeckScreenshot(byte[] image)
        {
            Execute.OnUIThread(
                () =>
                {
                    var ms = new MemoryStream(image);
                    var bi = new BitmapImage();
                    bi.BeginInit();
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.StreamSource = ms;
                    bi.EndInit();
                    this.DeckScreenshot = bi;
                    ms.Dispose();
                });
        }

        /// <summary>
        /// The set deck screenshot.
        /// </summary>
        /// <param name="image">
        /// The image.
        /// </param>
        private void SetDeckScreenshot(Bitmap image)
        {
            Execute.OnUIThread(
                () =>
                {
                    var ms = new MemoryStream();
                    var bi = new BitmapImage();
                    bi.BeginInit();
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    image.Save(ms, ImageFormat.Bmp);
                    ms.Seek(0, SeekOrigin.Begin);
                    bi.StreamSource = ms;
                    bi.EndInit();
                    this.DeckScreenshot = bi;
                    ms.Dispose();
                });
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(SelectDeck message)
        {
            if (message.Deck == null)
            {
                this.IsOpen = false;
                this.SelectedDeck = null;
            }
            else
            {
                this.SelectedServer = this.Servers.FirstOrDefault(x => x.Name == message.Deck.Server);
                if (message.Deck.Deleted)
                {
                    this.ShowHiddenDecks = true;
                }

                this.RefreshDecks();
                this.IsOpen = true;
                this.SelectedDeck = this.Decks.FirstOrDefault(x => x.Id == message.Deck.Id);
            }
        }
    }
}