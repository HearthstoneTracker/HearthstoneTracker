using System;
using Caliburn.Micro;
using Caliburn.Micro.Recipes.Filters;

namespace HearthCap.Features.Decks
{
    public class EditDeckViewModel : Screen
    {
        private string deckName;

        private string deckSlot;

        private DeckModel selectedDeck;

        public event EventHandler Saved;

        protected virtual void OnSaved()
        {
            var handler = Saved;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public string DeckName
        {
            get { return deckName; }
            set
            {
                if (value == deckName)
                {
                    return;
                }
                deckName = value;
                NotifyOfPropertyChange(() => DeckName);
            }
        }

        public string DeckSlot
        {
            get { return deckSlot; }
            set
            {
                if (value == deckSlot)
                {
                    return;
                }
                deckSlot = value;
                NotifyOfPropertyChange(() => DeckSlot);
            }
        }

        public void Load(DeckModel deck)
        {
            selectedDeck = deck;
            DeckName = deck.Name;
            DeckSlot = deck.Key;
        }

        [Dependencies("DeckName")]
        public void Save()
        {
            OnSaved();
        }

        public bool CanSave()
        {
            return !String.IsNullOrEmpty(DeckName);
        }
    }
}
