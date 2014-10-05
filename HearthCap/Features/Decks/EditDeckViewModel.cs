// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EditDeckViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The edit deck view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Decks
{
    using System;

    using Caliburn.Micro;
    using Caliburn.Micro.Recipes.Filters;

    /// <summary>
    /// The edit deck view model.
    /// </summary>
    public class EditDeckViewModel : Screen
    {
        /// <summary>
        /// The deck name.
        /// </summary>
        private string deckName;

        /// <summary>
        /// The deck slot.
        /// </summary>
        private string deckSlot;

        /// <summary>
        /// The selected deck.
        /// </summary>
        private DeckModel selectedDeck;

        /// <summary>
        /// The saved.
        /// </summary>
        public event EventHandler Saved;

        /// <summary>
        /// The on saved.
        /// </summary>
        protected virtual void OnSaved()
        {
            var handler = this.Saved;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
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
        /// The load.
        /// </summary>
        /// <param name="deck">
        /// The deck.
        /// </param>
        public void Load(DeckModel deck)
        {
            this.selectedDeck = deck;
            this.DeckName = deck.Name;
            this.DeckSlot = deck.Key;
        }

        /// <summary>
        /// The save.
        /// </summary>
        [Dependencies("DeckName")]
        public void Save()
        {
            this.OnSaved();
        }

        /// <summary>
        /// The can save.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CanSave()
        {
            return !string.IsNullOrEmpty(this.DeckName);
        }
    }
}