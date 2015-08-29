using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using HearthCap.Data;
using HearthCap.Framework;
using HearthCap.Shell.Events;

namespace HearthCap.Features.Core
{
    [Export(typeof(GlobalData))]
    public class GlobalData : PropertyChangedBase, IHandleWithTask<ShellReady>
    {
        private readonly Func<HearthStatsDbContext> dbContext;

        private readonly IEventAggregator eventAggregator;

        private Items cache;

        [ImportingConstructor]
        public GlobalData(Func<HearthStatsDbContext> dbContext,
            IEventAggregator eventAggregator)
        {
            this.dbContext = dbContext;
            this.eventAggregator = eventAggregator;
            eventAggregator.Subscribe(this);
        }

        [Import]
        public IBusyWatcher Busy { get; set; }

        public ThemeConfiguration ThemeConfiguration { get; protected set; }

        public async Task<Items> GetAsync()
        {
            if (cache == null)
            {
                await Initialize();
            }

            return cache;
        }

        public Items Get()
        {
            if (cache == null)
            {
                Initialize().Wait();
            }

            return cache;
        }

        public async Task RefreshData()
        {
            cache = null;
            await Initialize();
        }

        /// <summary>
        ///     Handle the message with a Task.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        ///     The Task that represents the operation.
        /// </returns>
        public async Task Handle(ShellReady message)
        {
            await Initialize();
        }

        protected async Task Initialize()
        {
            if (cache != null)
            {
                return;
            }

            using (var bsy = Busy.GetTicket())
            {
                using (var context = dbContext())
                {
                    var heroes = context.Heroes.Select(hero => hero);
                    var heroesresult = await heroes.ToListAsync();
                    cache = new Items(heroesresult);
                }
            }
        }

        public class Items : PropertyChangedBase
        {
            private readonly BindableCollection<Hero> heroes;

            public Items(IEnumerable<Hero> heroes)
            {
                this.heroes = new BindableCollection<Hero>(heroes);
                NotifyOfPropertyChange(() => Heroes);
            }

            public IObservableCollection<Hero> Heroes
            {
                get { return heroes; }
            }
        }
    }
}
