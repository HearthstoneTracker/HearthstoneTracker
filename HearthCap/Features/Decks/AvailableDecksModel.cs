// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AvailableDecksModel.cs" company="">
//   
// </copyright>
// <summary>
//   The available decks model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Decks
{
    using Caliburn.Micro;

    /// <summary>
    /// The available decks model.
    /// </summary>
    public class AvailableDecksModel : PropertyChangedBase
    {
        /// <summary>
        /// The available decks.
        /// </summary>
        private BindableCollection<DeckModel> availableDecks;

        /// <summary>
        /// The selected deck.
        /// </summary>
        private DeckModel selectedDeck;

        /// <summary>
        /// The slot.
        /// </summary>
        private string slot;

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
                this.NotifyOfPropertyChange(() => this.SelectedDeck);
            }
        }

        /// <summary>
        /// Gets or sets the available decks.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the slot.
        /// </summary>
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

        /// <summary>
        /// Gets the slot label.
        /// </summary>
        public string SlotLabel
        {
            get
            {
                return string.Format("Slot {0}:", this.Slot);
            }
        }
    }
}