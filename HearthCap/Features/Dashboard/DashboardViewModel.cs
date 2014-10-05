// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DashboardViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The dashboard view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Dashboard
{
    using System;
    using System.ComponentModel.Composition;
    using System.Threading.Tasks;

    using Caliburn.Micro;

    using HearthCap.Data;
    using HearthCap.Features.Core;
    using HearthCap.Framework;
    using HearthCap.Shell.Tabs;

    // [Export(typeof(ITab))]
    /// <summary>
    /// The dashboard view model.
    /// </summary>
    public class DashboardViewModel : TabViewModel
    {
        /// <summary>
        /// The db context.
        /// </summary>
        private readonly Func<HearthStatsDbContext> dbContext;

        /// <summary>
        /// The header.
        /// </summary>
        private string header;

        /// <summary>
        /// The heroes.
        /// </summary>
        private BindableCollection<Hero> heroes;

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardViewModel"/> class.
        /// </summary>
        /// <param name="dbContext">
        /// The db context.
        /// </param>
        [ImportingConstructor]
        public DashboardViewModel(Func<HearthStatsDbContext> dbContext)
        {
            this.dbContext = dbContext;
            this.DisplayName = "Dashboard";
            this.Header = "Dashboard!";
            this.Order = 0;
        }

        /// <summary>
        /// Gets or sets the busy.
        /// </summary>
        [Import]
        public IBusyWatcher Busy { get; set; }

        /// <summary>
        /// Gets or sets the global data.
        /// </summary>
        [Import]
        public GlobalData GlobalData { get; set; }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        public string Header
        {
            get
            {
                return this.header;
            }

            set
            {
                this.header = value;
                this.NotifyOfPropertyChange(() => this.Header);
            }
        }

        /// <summary>
        /// Called when initializing.
        /// </summary>
        protected override async void OnInitialize()
        {
            await this.LoadData();
        }

        /// <summary>
        /// The load data.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task LoadData()
        {
            var data = await this.GlobalData.GetAsync();
            this.heroes = new BindableCollection<Hero>(data.Heroes);
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