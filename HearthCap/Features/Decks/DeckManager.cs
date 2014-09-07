namespace HearthCap.Features.Decks
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Data.Entity;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using Caliburn.Micro;

    using HearthCap.Data;
    using HearthCap.Features.Core;
    using HearthCap.StartUp;

    using Omu.ValueInjecter;

    [Export(typeof(IDeckManager))]
    [Export(typeof(IStartupTask))]
    public class DeckManager : IDeckManager, IStartupTask
    {
        private readonly IEventAggregator events;

        private readonly Func<HearthStatsDbContext> dbContext;

        private readonly IDictionary<string, IList<Deck>> cachedDecks = new Dictionary<string, IList<Deck>>();

        private readonly BindableServerCollection servers = BindableServerCollection.Instance;

        [ImportingConstructor]
        public DeckManager(IEventAggregator events, Func<HearthStatsDbContext> dbContext)
        {
            this.events = events;
            this.dbContext = dbContext;
        }

        public IEnumerable<Deck> GetDecks(string server, bool includeDeleted = false)
        {
            if (string.IsNullOrEmpty(server))
            {
                server = "EU";
            }

            if (!cachedDecks.ContainsKey(server) || includeDeleted)
            {
                using (var context = dbContext())
                {
                    var q = context.Decks.Query().Where(x => x.Server == server);
                    if (!includeDeleted)
                    {
                        q = q.Where(x => !x.Deleted);
                    }
                    var result = q.OrderBy(x => String.IsNullOrEmpty(x.Key) ? "z" : x.Key).ThenBy(x => x.Name).ToList();

                    //if (result.Count < 9)
                    //{
                    //    for (int i = 1; i <= 9; i++)
                    //    {
                    //        var key = i.ToString(CultureInfo.InvariantCulture);
                    //        if (result.All(x => x.Key != key))
                    //        {
                    //            var deck = new Deck()
                    //                           {
                    //                               Server = server,
                    //                               Key = key,
                    //                               Name = String.Format("Deck #{0}", key)
                    //                           };
                    //            result.Add(deck);
                    //            context.Decks.Add(deck);
                    //        }
                    //    }
                    //    context.SaveChanges();
                    //    result = result.OrderBy(x => String.IsNullOrEmpty(x.Key) ? "z" : x.Key).ThenBy(x => x.Name).ToList();
                    //}
                    if (includeDeleted)
                    {
                        return result;
                    }
                    cachedDecks[server] = result;
                }
            }
            return cachedDecks[server];
        }

        public IEnumerable<Deck> GetAllDecks(bool includeDeleted = false)
        {
            using (var context = dbContext())
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

        public void UpdateDeckImage(Guid deckId, byte[] image)
        {
            using (var context = dbContext())
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
                
                ClearCache();
                context.SaveChanges();
            }
        }

        public void ClearCache()
        {
            cachedDecks.Clear();
        }

        public void AddDeck(DeckModel deck)
        {
            using (var context = dbContext())
            {
                var newdeck = new Deck();
                newdeck.InjectFrom(deck);
                context.Decks.Add(newdeck);
                if (!String.IsNullOrEmpty(deck.Key))
                {
                    var existingSlots = context.Decks.Where(x => x.Key == deck.Key && x.Server == deck.Server && x.Id != deck.Id).ToList();
                    foreach (var existingSlot in existingSlots)
                    {
                        existingSlot.Key = null;
                    }
                }

                context.SaveChanges();
                deck.InjectFrom(newdeck);
                ClearCache();
                events.PublishOnBackgroundThread(new DeckUpdated(newdeck));
            }
        }

        public void UpdateDeck(DeckModel deck, bool suppressEvent = false)
        {
            using (var context = dbContext())
            {
                var newdeck = context.Decks.Query().FirstOrDefault(x => x.Id == deck.Id);
                if (newdeck == null)
                {
                    throw new ArgumentException("deck not found", "deck");
                }
                var oldName = newdeck.Name;
                context.Entry(newdeck).CurrentValues.SetValues(deck);

                if (!String.IsNullOrEmpty(deck.Key))
                {
                    var existingSlots = context.Decks.Where(x => x.Key == deck.Key && x.Server == deck.Server && x.Id != deck.Id).ToList();
                    foreach (var existingSlot in existingSlots)
                    {
                        existingSlot.Key = null;
                    }
                }

                context.SaveChanges();
                ClearCache();

                if (!suppressEvent)
                {
                    events.PublishOnBackgroundThread(new DeckUpdated(newdeck));                    
                }
            }
        }

        public void UndeleteDeck(Guid id)
        {
            using (var context = dbContext())
            {
                var deck = context.Decks.FirstOrDefault(x => x.Id == id);
                if (deck == null)
                {
                    throw new ArgumentException("deck not found", "deck");
                }

                deck.Deleted = false;
                context.SaveChanges();
                ClearCache();
                events.PublishOnBackgroundThread(new DeckUpdated(deck));
            }
        }

        public void DeleteDeck(Guid id)
        {
            using (var context = dbContext())
            {
                var deck = context.Decks.FirstOrDefault(x => x.Id == id);
                if (deck == null)
                {
                    throw new ArgumentException("deck not found", "deck");
                }

                if (!String.IsNullOrEmpty(deck.Key))
                {
                    throw new InvalidOperationException("cannot delete deck with a slot defined");
                }

                deck.Deleted = true;
                context.SaveChanges();
                ClearCache();
                events.PublishOnBackgroundThread(new DeckUpdated(deck));
            }
        }

        public Deck GetOrCreateDeckBySlot(string server, string slot)
        {
            using (var context = dbContext())
            {
                var deck = context.Decks.FirstOrDefault(x => x.Key == slot && (x.Server == server));
                if (deck == null)
                {
                    deck = new Deck()
                               {
                                   Server = server,
                                   Key = slot,
                                   Name = String.Format("New deck {0} ({1})", slot, server)
                               };
                    context.Decks.Add(deck);
                    context.SaveChanges();
                    events.PublishOnBackgroundThread(new DeckUpdated(deck));
                    ClearCache();
                }
                return deck;
            }
        }

        public Deck GetDeckById(Guid id)
        {
            using (var context = dbContext())
            {
                return context.Decks.FirstOrDefault(x => x.Id == id);
            }
        }

        public void Run()
        {
            // migrate decks / servers
            using (var context = dbContext())
            {
                var currentServer = servers.DefaultName;
                if (String.IsNullOrEmpty(currentServer))
                {
                    currentServer = "EU";
                }
                var decks = context.Decks.Where(x => String.IsNullOrEmpty(x.Server)).ToList();
                foreach (var deck in decks)
                {
                    deck.Server = currentServer;
                }
                context.SaveChanges();
                ClearCache();
            }
        }
    }
}