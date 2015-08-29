using System;
using Caliburn.Micro;

namespace HearthCap.Features.Decks
{
    public class AvailableDecksModel : PropertyChangedBase
    {
        private BindableCollection<DeckModel> availableDecks;

        private DeckModel selectedDeck;

        private string slot;

        public DeckModel SelectedDeck
        {
            get { return selectedDeck; }
            set
            {
                if (Equals(value, selectedDeck))
                {
                    return;
                }
                selectedDeck = value;
                NotifyOfPropertyChange(() => SelectedDeck);
            }
        }

        public BindableCollection<DeckModel> AvailableDecks
        {
            get { return availableDecks; }
            set
            {
                if (Equals(value, availableDecks))
                {
                    return;
                }
                availableDecks = value;
                NotifyOfPropertyChange(() => AvailableDecks);
            }
        }

        public string Slot
        {
            get { return slot; }
            set
            {
                if (value == slot)
                {
                    return;
                }
                slot = value;
                NotifyOfPropertyChange(() => Slot);
                NotifyOfPropertyChange(() => SlotLabel);
            }
        }

        public string SlotLabel
        {
            get { return String.Format("Slot {0}:", Slot); }
        }
    }
}
