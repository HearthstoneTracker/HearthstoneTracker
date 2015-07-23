namespace HearthCap.Features.Decks
{
    using System;

    using Caliburn.Micro;

    public class AvailableDecksModel : PropertyChangedBase
    {
        private BindableCollection<DeckModel> availableDecks;

        private DeckModel selectedDeck;

        private string slot;

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
                this.NotifyOfPropertyChange(() => this.SelectedDeck);
            }
        }

        public BindableCollection<DeckModel> AvailableDecks
        {
            get
            {
                return this.availableDecks;
            }
            set
            {
                if (Equals(value, this.availableDecks))
                {
                    return;
                }
                this.availableDecks = value;
                this.NotifyOfPropertyChange(() => this.AvailableDecks);
            }
        }

        public string Slot
        {
            get
            {
                return this.slot;
            }
            set
            {
                if (value == this.slot)
                {
                    return;
                }
                this.slot = value;
                this.NotifyOfPropertyChange(() => this.Slot);
                this.NotifyOfPropertyChange(() => this.SlotLabel);
            }
        }

        public string SlotLabel
        {
            get
            {
                return String.Format("Slot {0}:", this.Slot);
            }
        }
    }
}