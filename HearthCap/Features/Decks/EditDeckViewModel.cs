namespace HearthCap.Features.Decks
{
    using System;

    using Caliburn.Micro;
    using Caliburn.Micro.Recipes.Filters;

    public class EditDeckViewModel : Screen
    {
        private string deckName;

        private string deckSlot;

        private DeckModel selectedDeck;

        public EditDeckViewModel()
        {
            
        }

        public event EventHandler Saved;

        protected virtual void OnSaved()
        {
            var handler = this.Saved;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
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

        public void Load(DeckModel deck)
        {
            this.selectedDeck = deck;
            this.DeckName = deck.Name;
            this.DeckSlot = deck.Key;
        }

        [Dependencies("DeckName")]
        public void Save()
        {
            this.OnSaved();
        }

        public bool CanSave()
        {
            return !String.IsNullOrEmpty(this.DeckName);
        }
    }
}