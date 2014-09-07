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

    [Export(typeof(IFlyout))]
    public class DeckSettingsViewModel : FlyoutViewModel,
        IHandle<DeckScreenshotTaken>,
        IHandle<SelectDeck>
    {
        private readonly IEventAggregator events;

        private readonly IDeckManager deckManager;

        private readonly BindableCollection<DeckModel> decks = new BindableCollection<DeckModel>();

        private readonly BindableCollection<AvailableDecksModel> availableDecks = new BindableCollection<AvailableDecksModel>();

        private readonly BindableServerCollection servers = BindableServerCollection.Instance;

        private DeckModel selectedDeck;

        private string deckName;

        private string deckSlot;

        private ServerItemModel selectedServer;

        private bool canTakeScreenshot;

        private BitmapImage deckScreenshot;

        private string deckNotes;

        private bool showHiddenDecks;

        [ImportingConstructor]
        public DeckSettingsViewModel(
            IEventAggregator events,
            IDeckManager deckManager)
        {
            this.events = events;
            this.deckManager = deckManager;

            SetPosition(Position.Left);
            this.Name = Flyouts.Decks;
            this.Header = this.DisplayName = "Decks:";
            // note the setting of backing field instead of property, to not trigger deck refreshing yet.
            this.selectedServer = servers.Default ?? servers.First();
            events.Subscribe(this);
            CanTakeScreenshot = true;
        }

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
                RefreshDecks();
            }
        }

        public BindableCollection<AvailableDecksModel> AvailableDecks
        {
            get
            {
                return availableDecks;
            }
        }

        public BindableCollection<DeckModel> Decks
        {
            get
            {
                return this.decks;
            }
        }

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

        public IList<string> Slots
        {
            get
            {
                var lst = new List<string>();
                lst.Add("");
                for (int i = 1; i <= 9; i++)
                {
                    lst.Add(i.ToString(CultureInfo.InvariantCulture));
                }
                return lst;
            }
        }

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
                if (selectedDeck != null)
                {
                    DeckName = selectedDeck.Name;
                    DeckSlot = selectedDeck.Key;
                    DeckNotes = selectedDeck.Notes;

                    if (selectedDeck.Image != null)
                    {
                        SetDeckScreenshot(selectedDeck.Image.Image);
                    }
                    else
                    {
                        DeckScreenshot = null;
                    }
                }
                else
                {
                    DeckScreenshot = null;
                }
                CancelTakeScreenshot();
                this.NotifyOfPropertyChange(() => this.SelectedDeck);
                this.NotifyOfPropertyChange(() => this.CanDeleteDeck);
            }
        }

        public BindableServerCollection Servers
        {
            get
            {
                return this.servers;
            }
        }

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
                RefreshDecks();
                this.NotifyOfPropertyChange(() => this.SelectedServer);
            }
        }

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

        public void TakeScreenshot()
        {
            CanTakeScreenshot = false;
            events.PublishOnBackgroundThread(new RequestDeckScreenshot());
        }

        public void CancelTakeScreenshot()
        {
            if (CanTakeScreenshot == false)
            {
                CanTakeScreenshot = true;
                events.PublishOnBackgroundThread(new RequestDeckScreenshot()
                {
                    Cancel = true
                });
            }
        }

        public void AddDeck()
        {
            var deck = new DeckModel()
                           {
                               Name = "New deck",
                               Server = SelectedServer.Name
                           };
            deckManager.AddDeck(deck);

            RefreshDecks();
            SelectedDeck = Decks.FirstOrDefault(x => x.Id == deck.Id);
        }

        [Dependencies("SelectedDeck")]
        public void DeleteDeck()
        {
            if (MessageBox.Show("Delete this deck?", "Delete this deck?", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            {
                return;
            }

            if (!String.IsNullOrEmpty(SelectedDeck.Key))
            {
                RemoveFromSlot(SelectedDeck.Key);
            }

            deckManager.DeleteDeck(SelectedDeck.Id);
            SelectedDeck = null;
            RefreshDecks();
        }

        [Dependencies("SelectedDeck")]
        public void UndeleteDeck()
        {
            deckManager.UndeleteDeck(SelectedDeck.Id);

            RefreshDecks();
        }

        public bool CanDeleteDeck
        {
            get
            {
                return SelectedDeck != null && !SelectedDeck.Deleted;
            }
        }

        [Dependencies("DeckName", "HasNewScreenshot", "DeckSlot", "DeckNotes")]
        public void UpdateDeck(bool suppressRefresh = false)
        {
            SelectedDeck.Name = DeckName;
            SelectedDeck.Key = DeckSlot;
            SelectedDeck.Notes = DeckNotes;
            deckManager.UpdateDeck(SelectedDeck);
            // UpdateDeckImageFromBitmapImage(SelectedDeck);
            NotifyOfPropertyChange(() => DeckName);
            if (!suppressRefresh)
            {
                RefreshDecks();
            }
        }

        private void UpdateDeckImageFromBitmapImage(DeckModel deckModel)
        {
            if (DeckScreenshot == null)
            {
                return;
            }

            using (var ms = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(DeckScreenshot));
                enc.Save(ms);
                ms.Position = 0;
                deckManager.UpdateDeckImage(deckModel.Id, ms.ToArray());
            }
        }

        [Dependencies("DeckName")]
        public void SaveAsNewDeck()
        {
            var deck = new DeckModel()
            {
                Name = DeckName,
                Server = SelectedServer.Name,
                Key = DeckSlot,
                Notes = DeckNotes
            };
            deckManager.AddDeck(deck);
            UpdateDeckImageFromBitmapImage(deck);

            RefreshDecks();
            SelectedDeck = Decks.FirstOrDefault(x => x.Id == deck.Id);
        }

        public bool CanSaveAsNewDeck()
        {
            return !String.IsNullOrEmpty(DeckName);
        }

        public bool CanUpdateDeck()
        {
            return !String.IsNullOrEmpty(DeckName) && HasChanged();
        }

        private bool HasChanged()
        {
            return SelectedDeck.Name != DeckName ||
                   SelectedDeck.Key != DeckSlot ||
                   SelectedDeck.Notes != DeckNotes;
        }

        protected void RemoveFromSlot(AvailableDecksModel model)
        {
            RemoveFromSlot(model.Slot);
        }

        protected void RemoveFromSlot(string slot)
        {
            var start = Convert.ToInt32(slot) - 1;
            if (AvailableDecks[start].SelectedDeck != null)
            {
                AvailableDecks[start].SelectedDeck.Key = string.Empty;
                deckManager.UpdateDeck(AvailableDecks[start].SelectedDeck, true);
            }
            for (int i = start + 1; i < 9; i++)
            {
                if (AvailableDecks[i].SelectedDeck != null)
                {
                    AvailableDecks[i].SelectedDeck.Key = i.ToString(CultureInfo.InvariantCulture);
                    deckManager.UpdateDeck(AvailableDecks[i].SelectedDeck, true);
                }
            }

            // events.PublishOnBackgroundThread(new DecksUpdated(SelectedServer.Name));
        }

        public void Cancel()
        {
            SelectedDeck = null;
        }

        public void SaveScreenshotToDisk()
        {
            if (SelectedDeck == null || DeckScreenshot == null)
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
                    enc.Frames.Add(BitmapFrame.Create(DeckScreenshot));
                    enc.Save(fs);
                }
            }
        }

        private void RefreshDecks()
        {
            // deckManager.ClearCache();
            var oldSelected = SelectedDeck;
            var d = deckManager.GetDecks(SelectedServer.Name, ShowHiddenDecks);
            this.Decks.Clear();
            this.Decks.AddRange(d.ToModel());
            if (oldSelected != null)
            {
                SelectedDeck = Decks.FirstOrDefault(x => x.Id == oldSelected.Id);
            }

            RefreshAvailableDecks();
        }

        private void RefreshAvailableDecks()
        {
            if (AvailableDecks.Count == 0)
            {
                for (int i = 1; i <= 9; i++)
                {
                    var key = i.ToString(CultureInfo.InvariantCulture);
                    var model = new AvailableDecksModel()
                                    {
                                        Slot = key,
                                        AvailableDecks =
                                            new BindableCollection<DeckModel>(this.Decks.Where(x => x.Key == key || String.IsNullOrEmpty(x.Key))),
                                        SelectedDeck = this.Decks.FirstOrDefault(x => x.Key == key)
                                    };
                    model.PropertyChanged += AvailableDecksModel_OnPropertyChanged;
                    AvailableDecks.Add(model);
                }
                NotifyOfPropertyChange(() => AvailableDecks);
            }
            else
            {
                for (int i = 1; i <= 9; i++)
                {
                    var key = i.ToString(CultureInfo.InvariantCulture);
                    AvailableDecks[i - 1].AvailableDecks =
                        new BindableCollection<DeckModel>(this.Decks.Where(x => x.Key == key || String.IsNullOrEmpty(x.Key)));
                    AvailableDecks[i - 1].PropertyChanged -= AvailableDecksModel_OnPropertyChanged;
                    AvailableDecks[i - 1].SelectedDeck = this.Decks.FirstOrDefault(x => x.Key == key);
                    AvailableDecks[i - 1].PropertyChanged += AvailableDecksModel_OnPropertyChanged;
                }
                NotifyOfPropertyChange(() => AvailableDecks);
            }
        }

        /// <summary>
        /// Called when initializing.
        /// </summary>
        protected override void OnInitialize()
        {
            RefreshDecks();
        }

        private void AvailableDecksModel_OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "SelectedDeck")
            {
                var model = (AvailableDecksModel)sender;
                if (model.SelectedDeck != null)
                {
                    model.SelectedDeck.Key = model.Slot;
                    deckManager.UpdateDeck(model.SelectedDeck);
                    RefreshDecks();
                }
            }
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(DeckScreenshotTaken message)
        {
            if (SelectedDeck == null)
                return;

            SetDeckScreenshot(message.Image);
            message.Image.Dispose();

            CanTakeScreenshot = true;
            UpdateDeck(true);
            UpdateDeckImageFromBitmapImage(SelectedDeck);
            RefreshDecks();
        }

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
                    DeckScreenshot = bi;
                    ms.Dispose();
                });
        }

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
                    DeckScreenshot = bi;
                    ms.Dispose();
                });
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(SelectDeck message)
        {
            if (message.Deck == null)
            {
                IsOpen = false;
                SelectedDeck = null;
            }
            else
            {
                SelectedServer = Servers.FirstOrDefault(x => x.Name == message.Deck.Server);
                if (message.Deck.Deleted)
                {
                    ShowHiddenDecks = true;
                }
                RefreshDecks();
                IsOpen = true;
                SelectedDeck = Decks.FirstOrDefault(x => x.Id == message.Deck.Id);
            }
        }
    }
}