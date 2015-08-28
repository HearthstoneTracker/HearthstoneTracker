namespace HearthCap.Features.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;

    using Caliburn.Micro;

    using HearthCap.Data;
    using HearthCap.Framework;
    using HearthCap.Shell;
    using HearthCap.Shell.Events;
    using HearthCap.Shell.Theme;
    using HearthCap.StartUp;

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
            this.cache = null;
            await this.Initialize();
        }

        /// <summary>
        /// Handle the message with a Task.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        /// The Task that represents the operation.
        /// </returns>
        public async Task Handle(ShellReady message)
        {
            await this.Initialize();
        }

        protected async Task Initialize()
        {
            if (this.cache != null)
            {
                return;
            }

            using (var bsy = this.Busy.GetTicket())
            using (var context = this.dbContext())
            {
                var heroes = context.Heroes.Select(hero => hero);
                var heroesresult = await heroes.ToListAsync();
                this.cache = new Items(heroesresult);
            }
        }

        public class Items : PropertyChangedBase
        {
            private BindableCollection<Hero> heroes;

            public Items(IEnumerable<Hero> heroes)
            {
                this.heroes = new BindableCollection<Hero>(heroes);
                this.NotifyOfPropertyChange(() => this.Heroes);
            }

            public IObservableCollection<Hero> Heroes
            {
                get
                {
                    return this.heroes;
                }
            }
        }
    }
}