// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlobalData.cs" company="">
//   
// </copyright>
// <summary>
//   The global data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Core
{
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

    /// <summary>
    /// The global data.
    /// </summary>
    [Export(typeof(GlobalData))]
    public class GlobalData : PropertyChangedBase, IHandleWithTask<ShellReady>
    {
        /// <summary>
        /// The db context.
        /// </summary>
        private readonly Func<HearthStatsDbContext> dbContext;

        /// <summary>
        /// The event aggregator.
        /// </summary>
        private readonly IEventAggregator eventAggregator;

        /// <summary>
        /// The cache.
        /// </summary>
        private Items cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalData"/> class.
        /// </summary>
        /// <param name="dbContext">
        /// The db context.
        /// </param>
        /// <param name="eventAggregator">
        /// The event aggregator.
        /// </param>
        [ImportingConstructor]
        public GlobalData(Func<HearthStatsDbContext> dbContext, 
            IEventAggregator eventAggregator)
        {
            this.dbContext = dbContext;
            this.eventAggregator = eventAggregator;
            eventAggregator.Subscribe(this);
        }

        /// <summary>
        /// Gets or sets the busy.
        /// </summary>
        [Import]
        public IBusyWatcher Busy { get; set; }

        /// <summary>
        /// Gets or sets the theme configuration.
        /// </summary>
        public ThemeConfiguration ThemeConfiguration { get; protected set; }

        /// <summary>
        /// The get async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<Items> GetAsync()
        {
            if (this.cache == null)
            {
                await this.Initialize();
            }

            return this.cache;
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <returns>
        /// The <see cref="Items"/>.
        /// </returns>
        public Items Get()
        {
            if (this.cache == null)
            {
                this.Initialize().Wait();
            }

            return this.cache;
        }

        /// <summary>
        /// The refresh data.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task RefreshData()
        {
            this.cache = null;
            await this.Initialize();
        }

        /// <summary>
        /// Handle the message with a Task.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The Task that represents the operation.
        /// </returns>
        public async Task Handle(ShellReady message)
        {
            await this.Initialize();
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
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

        /// <summary>
        /// The items.
        /// </summary>
        public class Items : PropertyChangedBase
        {
            /// <summary>
            /// The heroes.
            /// </summary>
            private BindableCollection<Hero> heroes;

            /// <summary>
            /// Initializes a new instance of the <see cref="Items"/> class.
            /// </summary>
            /// <param name="heroes">
            /// The heroes.
            /// </param>
            public Items(IEnumerable<Hero> heroes)
            {
                this.heroes = new BindableCollection<Hero>(heroes);
                this.NotifyOfPropertyChange(() => this.Heroes);
            }

            /// <summary>
            /// Gets the heroes.
            /// </summary>
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