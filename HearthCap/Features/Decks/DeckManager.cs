// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeckManager.cs" company="">
//   
// </copyright>
// <summary>
//   The deck manager.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Decks
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Data.Entity;
    using System.Linq;

    using Caliburn.Micro;

    using HearthCap.Data;
    using HearthCap.Features.Core;
    using HearthCap.StartUp;

    using Omu.ValueInjecter;

    /// <summary>
    /// The deck manager.
    /// </summary>
    [Export(typeof(IDeckManager))]
    [Export(typeof(IStartupTask))]
    public class DeckManager : IDeckManager, IStartupTask
    {
        /// <summary>
        /// The events.
        /// </summary>
        private readonly IEventAggregator events;

        /// <summary>
        /// The db context.
        /// </summary>
        private readonly Func<HearthStatsDbContext> dbContext;

        /// <summary>
        /// The cached decks.
        /// </summary>
        private readonly IDictionary<string, IList<Deck>> cachedDecks = new Dictionary<string, IList<Deck>>();

        /// <summary>
        /// The servers.
        /// </summary>
        private readonly BindableServerCollection servers = BindableServerCollection.Instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeckManager"/> class.
        /// </summary>
        /// <param name="events">
        /// The events.
        /// </param>
        /// <param name="dbContext">
        /// The db context.
        /// </param>
        [ImportingConstructor]
        public DeckManager(IEventAggregator events, Func<HearthStatsDbContext> dbContext)
        {
            this.events = events;
            this.dbContext = dbContext;
        }

        /// <summary>
        /// The get decks.
        /// </summary>
        /// <param name="server">
        /// The server.
        /// </param>
        /// <param name="includeDeleted">
        /// The include deleted.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<Deck> GetDecks(string server, bool includeDeleted = false)
        {
            if (string.IsNullOrEmpty(server))
            {
                server = "EU";
            }

            if (!this.cachedDecks.ContainsKey(server) || includeDeleted)
            {
                using (var context = this.dbContext())
                {
                    var q = context.Decks.Query().Where(x => x.Server == server);
                    if (!includeDeleted)
                    {
                        q = q.Where(x => !x.Deleted);
                    }

                    var result = q.OrderBy(x => string.IsNullOrEmpty(x.Key) ? "z" : x.Key).ThenBy(x => x.Name).ToList();

                    // if (result.Count < 9)
                    // {
                    // for (int i = 1; i <= 9; i++)
                    // {
                    // var key = i.ToString(CultureInfo.InvariantCulture);
                    // if (result.All(x => x.Key != key))
                    // {
                    // var deck = new Deck()
                    // {
                    // Server = server,
                    // Key = key,
                    // Name = String.Format("Deck #{0}", key)
                    // };
                    // result.Add(deck);
                    // context.Decks.Add(deck);
                    // }
                    // }
                    // context.SaveChanges();
                    // result = result.OrderBy(x => String.IsNullOrEmpty(x.Key) ? "z" : x.Key).ThenBy(x => x.Name).ToList();
                    // }
                    if (includeDeleted)
                    {
                        return result;
                    }

                    this.cachedDecks[server] = result;
                }
            }

            return this.cachedDecks[server];
        }

        /// <summary>
        /// The get all decks.
        /// </summary>
        /// <param name="includeDeleted">
        /// The include deleted.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<Deck> GetAllDecks(bool includeDeleted = false)
        {
            using (var context = this.dbContext())
            {
                var q = context.Decks.AsQueryable();
                if (!includeDeleted)
                {
                    q = q.Where(x => !x.Deleted);
                }

                var result = q.OrderBy(x => x.Key).ThenBy(x => x.Name).ToList();
                return result;
            }
        }

        /// <summary>
        /// The update deck image.
        /// </summary>
        /// <param name="deckId">
        /// The deck id.
        /// </param>
        /// <param name="image">
        /// The image.
        /// </param>
        /// <exception cref="ArgumentException">
        /// </exception>
        public void UpdateDeckImage(Guid deckId, byte[] image)
        {
            using (var context = this.dbContext())
            {
                var deck = context.Decks.Include(x => x.Image).FirstOrDefault(x => x.Id == deckId);
                if (deck == null)
                {
                    throw new ArgumentException("Deck not found: " + deckId);
                }

                if (deck.Image == null)
                {
                    deck.Image = new DeckImage(deck);
                }
                
                deck.Image.Image = image;
                deck.Image.Modified = DateTime.Now;
                
                this.ClearCache();
                context.SaveChanges();
            }
        }

        /// <summary>
        /// The clear cache.
        /// </summary>
        public void ClearCache()
        {
            this.cachedDecks.Clear();
        }

        /// <summary>
        /// The add deck.
        /// </summary>
        /// <param name="deck">
        /// The deck.
        /// </param>
        public void AddDeck(DeckModel deck)
        {
            using (var context = this.dbContext())
            {
                var newdeck = new Deck();
                newdeck.InjectFrom(deck);
                context.Decks.Add(newdeck);
                if (!string.IsNullOrEmpty(deck.Key))
                {
                    var existingSlots = context.Decks.Where(x => x.Key == deck.Key && x.Server == deck.Server && x.Id != deck.Id).ToList();
                    foreach (var existingSlot in existingSlots)
                    {
                        existingSlot.Key = null;
                    }
                }

                context.SaveChanges();
                deck.InjectFrom(newdeck);
                this.ClearCache();
                this.events.PublishOnBackgroundThread(new DeckUpdated(newdeck));
            }
        }

        /// <summary>
        /// The update deck.
        /// </summary>
        /// <param name="deck">
        /// The deck.
        /// </param>
        /// <param name="suppressEvent">
        /// The suppress event.
        /// </param>
        /// <exception cref="ArgumentException">
        /// </exception>
        public void UpdateDeck(DeckModel deck, bool suppressEvent = false)
        {
            using (var context = this.dbContext())
            {
                var newdeck = context.Decks.Query().FirstOrDefault(x => x.Id == deck.Id);
                if (newdeck == null)
                {
                    throw new ArgumentException("deck not found", "deck");
                }

                var oldName = newdeck.Name;
                context.Entry(newdeck).CurrentValues.SetValues(deck);

                if (!string.IsNullOrEmpty(deck.Key))
                {
                    var existingSlots = context.Decks.Where(x => x.Key == deck.Key && x.Server == deck.Server && x.Id != deck.Id).ToList();
                    foreach (var existingSlot in existingSlots)
                    {
                        existingSlot.Key = null;
                    }
                }

                context.SaveChanges();
                this.ClearCache();

                if (!suppressEvent)
                {
                    this.events.PublishOnBackgroundThread(new DeckUpdated(newdeck));                    
                }
            }
        }

        /// <summary>
        /// The undelete deck.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <exception cref="ArgumentException">
        /// </exception>
        public void UndeleteDeck(Guid id)
        {
            using (var context = this.dbContext())
            {
                var deck = context.Decks.FirstOrDefault(x => x.Id == id);
                if (deck == null)
                {
                    throw new ArgumentException("deck not found", "deck");
                }

                deck.Deleted = false;
                context.SaveChanges();
                this.ClearCache();
                this.events.PublishOnBackgroundThread(new DeckUpdated(deck));
            }
        }

        /// <summary>
        /// The delete deck.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <exception cref="ArgumentException">
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        public void DeleteDeck(Guid id)
        {
            using (var context = this.dbContext())
            {
                var deck = context.Decks.FirstOrDefault(x => x.Id == id);
                if (deck == null)
                {
                    throw new ArgumentException("deck not found", "deck");
                }

                if (!string.IsNullOrEmpty(deck.Key))
                {
                    throw new InvalidOperationException("cannot delete deck with a slot defined");
                }

                deck.Deleted = true;
                context.SaveChanges();
                this.ClearCache();
                this.events.PublishOnBackgroundThread(new DeckUpdated(deck));
            }
        }

        /// <summary>
        /// The get or create deck by slot.
        /// </summary>
        /// <param name="server">
        /// The server.
        /// </param>
        /// <param name="slot">
        /// The slot.
        /// </param>
        /// <returns>
        /// The <see cref="Deck"/>.
        /// </returns>
        public Deck GetOrCreateDeckBySlot(string server, string slot)
        {
            using (var context = this.dbContext())
            {
                var deck = context.Decks.FirstOrDefault(x => x.Key == slot && (x.Server == server));
                if (deck == null)
                {
                    deck = new Deck {
                                   Server = server, 
                                   Key = slot, 
                                   Name = string.Format("New deck {0} ({1})", slot, server)
                               };
                    context.Decks.Add(deck);
                    context.SaveChanges();
                    this.events.PublishOnBackgroundThread(new DeckUpdated(deck));
                    this.ClearCache();
                }

                return deck;
            }
        }

        /// <summary>
        /// The get deck by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Deck"/>.
        /// </returns>
        public Deck GetDeckById(Guid id)
        {
            using (var context = this.dbContext())
            {
                return context.Decks.FirstOrDefault(x => x.Id == id);
            }
        }

        /// <summary>
        /// The run.
        /// </summary>
        public void Run()
        {
            // migrate decks / servers
            using (var context = this.dbContext())
            {
                var currentServer = this.servers.DefaultName;
                if (string.IsNullOrEmpty(currentServer))
                {
                    currentServer = "EU";
                }

                var decks = context.Decks.Where(x => string.IsNullOrEmpty(x.Server)).ToList();
                foreach (var deck in decks)
                {
                    deck.Server = currentServer;
                }

                context.SaveChanges();
                this.ClearCache();
            }
        }
    }
}