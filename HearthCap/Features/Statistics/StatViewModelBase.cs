// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatViewModelBase.cs" company="">
//   
// </copyright>
// <summary>
//   The stat view model base.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Statistics
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;

    using Caliburn.Micro;

    using HearthCap.Data;
    using HearthCap.Features.Core;
    using HearthCap.Framework;
    using HearthCap.Util;

    /// <summary>
    /// The stat view model base.
    /// </summary>
    public abstract class StatViewModelBase : Screen, IStatsViewModel
    {
        /// <summary>
        /// The show win ratio.
        /// </summary>
        private bool showWinRatio;

        /// <summary>
        /// The show wins.
        /// </summary>
        private bool showWins;

        /// <summary>
        /// The show win ratio coin.
        /// </summary>
        private bool showWinRatioCoin;

        /// <summary>
        /// The show win ratio no coin.
        /// </summary>
        private bool showWinRatioNoCoin;

        /// <summary>
        /// The show wins coin.
        /// </summary>
        private bool showWinsCoin;

        /// <summary>
        /// The show wins no coin.
        /// </summary>
        private bool showWinsNoCoin;

        /// <summary>
        /// The from date.
        /// </summary>
        private DateTime? fromDate;

        /// <summary>
        /// The game mode.
        /// </summary>
        private string gameMode;

        /// <summary>
        /// The to date.
        /// </summary>
        private DateTime? toDate;

        /// <summary>
        /// The show total games.
        /// </summary>
        private bool showTotalGames;

        /// <summary>
        /// The server.
        /// </summary>
        private ServerItemModel server;

        /// <summary>
        /// The show total games by coin.
        /// </summary>
        private bool showTotalGamesByCoin;

        /// <summary>
        /// The show played vs ratio.
        /// </summary>
        private bool showPlayedVsRatio;

        /// <summary>
        /// The search.
        /// </summary>
        private string search;

        /// <summary>
        /// The refresh data.
        /// </summary>
        public abstract void RefreshData();

        /// <summary>
        /// Gets or sets the busy.
        /// </summary>
        public IBusyWatcher Busy { get; set; }

        /// <summary>
        /// Gets or sets the from date.
        /// </summary>
        public DateTime? FromDate
        {
            get
            {
                return this.fromDate;
            }

            set
            {
                if (value.Equals(this.fromDate))
                {
                    return;
                }

                this.fromDate = value;
                this.NotifyOfPropertyChange(() => this.FromDate);
            }
        }

        /// <summary>
        /// Gets or sets the to date.
        /// </summary>
        public DateTime? ToDate
        {
            get
            {
                return this.toDate;
            }

            set
            {
                if (value.Equals(this.toDate))
                {
                    return;
                }

                this.toDate = value;
                this.NotifyOfPropertyChange(() => this.ToDate);
            }
        }

        /// <summary>
        /// Gets or sets the game mode.
        /// </summary>
        public string GameMode
        {
            get
            {
                return this.gameMode;
            }

            set
            {
                if (value == this.gameMode)
                {
                    return;
                }

                this.gameMode = value;
                this.NotifyOfPropertyChange(() => this.GameMode);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether show win ratio.
        /// </summary>
        public bool ShowWinRatio
        {
            get
            {
                return this.showWinRatio;
            }

            set
            {
                if (value.Equals(this.showWinRatio) || this.IsLastSelected(value))
                {
                    return;
                }

                this.showWinRatio = value;
                this.NotifyOfPropertyChange(() => this.ShowWinRatio);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether show wins.
        /// </summary>
        public bool ShowWins
        {
            get
            {
                return this.showWins;
            }

            set
            {
                if (value.Equals(this.showWins) || this.IsLastSelected(value))
                {
                    return;
                }

                this.showWins = value;
                this.NotifyOfPropertyChange(() => this.ShowWins);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether show win ratio coin.
        /// </summary>
        public bool ShowWinRatioCoin
        {
            get
            {
                return this.showWinRatioCoin;
            }

            set
            {
                if (value.Equals(this.showWinRatioCoin) || this.IsLastSelected(value))
                {
                    return;
                }

                this.showWinRatioCoin = value;
                this.NotifyOfPropertyChange(() => this.ShowWinRatioCoin);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether show win ratio no coin.
        /// </summary>
        public bool ShowWinRatioNoCoin
        {
            get
            {
                return this.showWinRatioNoCoin;
            }

            set
            {
                if (value.Equals(this.showWinRatioNoCoin) || this.IsLastSelected(value))
                {
                    return;
                }

                this.showWinRatioNoCoin = value;
                this.NotifyOfPropertyChange(() => this.ShowWinRatioNoCoin);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether show wins coin.
        /// </summary>
        public bool ShowWinsCoin
        {
            get
            {
                return this.showWinsCoin;
            }

            set
            {
                if (value.Equals(this.showWinsCoin) || this.IsLastSelected(value))
                {
                    return;
                }

                this.showWinsCoin = value;
                this.NotifyOfPropertyChange(() => this.ShowWinsCoin);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether show wins no coin.
        /// </summary>
        public bool ShowWinsNoCoin
        {
            get
            {
                return this.showWinsNoCoin;
            }

            set
            {
                if (value.Equals(this.showWinsNoCoin) || this.IsLastSelected(value))
                {
                    return;
                }

                this.showWinsNoCoin = value;
                this.NotifyOfPropertyChange(() => this.ShowWinsNoCoin);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether show total games.
        /// </summary>
        public bool ShowTotalGames
        {
            get
            {
                return this.showTotalGames;
            }

            set
            {
                if (value.Equals(this.showTotalGames) || this.IsLastSelected(value))
                {
                    return;
                }

                this.showTotalGames = value;
                this.NotifyOfPropertyChange(() => this.ShowTotalGames);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether show played vs ratio.
        /// </summary>
        public bool ShowPlayedVsRatio
        {
            get
            {
                return this.showPlayedVsRatio;
            }

            set
            {
                if (value.Equals(this.showPlayedVsRatio) || this.IsLastSelected(value))
                {
                    return;
                }

                this.showPlayedVsRatio = value;
                this.NotifyOfPropertyChange(() => this.ShowPlayedVsRatio);
            }
        }

        /// <summary>
        /// Gets or sets the server.
        /// </summary>
        public ServerItemModel Server
        {
            get
            {
                return this.server;
            }

            set
            {
                if (Equals(value, this.server))
                {
                    return;
                }

                this.server = value;
                this.NotifyOfPropertyChange(() => this.Server);
            }
        }

        /// <summary>
        /// Gets or sets the search.
        /// </summary>
        public string Search
        {
            get
            {
                return this.search;
            }

            set
            {
                if (value == this.search)
                {
                    return;
                }

                this.search = value;
                this.NotifyOfPropertyChange(() => this.Search);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether show total games by coin.
        /// </summary>
        public bool ShowTotalGamesByCoin
        {
            get
            {
                return this.showTotalGamesByCoin;
            }

            set
            {
                if (value.Equals(this.showTotalGamesByCoin) || this.IsLastSelected(value))
                {
                    return;
                }

                this.showTotalGamesByCoin = value;
                this.NotifyOfPropertyChange(() => this.ShowTotalGamesByCoin);
            }
        }

        /// <summary>
        /// The is last selected.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="min">
        /// The min.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsLastSelected(bool value, int min = 1)
        {
            if (value) return false;
            return Truth(
                this.ShowWinRatio, 
                this.ShowWinRatioCoin, 
                this.ShowWinRatioNoCoin, 
                this.ShowWins, 
                this.ShowWinsCoin, 
                this.ShowTotalGames, 
                this.ShowTotalGamesByCoin, 
                this.ShowPlayedVsRatio, 
                this.ShowWinsNoCoin) <= min;
        }

        /// <summary>
        /// The truth.
        /// </summary>
        /// <param name="booleans">
        /// The booleans.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int Truth(params bool[] booleans)
        {
            return booleans.Count(b => b);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StatViewModelBase"/> class.
        /// </summary>
        protected StatViewModelBase()
        {
            this.GameMode = string.Empty;

            using (var reg = new StatRegistrySettings(this.GetType()))
            {
                this.ShowWinRatio = reg.ShowWinRatio;
                this.ShowWinRatioCoin = reg.ShowWinRatioCoin;
                this.ShowWinRatioNoCoin = reg.ShowWinRatioNoCoin;
                this.ShowWins = reg.ShowWins;
                this.ShowWinsCoin = reg.ShowWinsCoin;
                this.ShowWinsNoCoin = reg.ShowWinsNoCoin;
                this.ShowTotalGames = reg.ShowTotalGames;
                this.ShowTotalGamesByCoin = reg.ShowTotalGamesByCoin;
                this.ShowPlayedVsRatio = reg.ShowPlayedVsRatio;
            }

            if (this.IsLastSelected(false, 0))
            {
                this.ShowWinRatio = true;
                this.ShowWins = true;
                this.ShowTotalGames = true;
            }

            this.Busy = new BusyWatcher();
            this.PropertyChanged += this.OnPropertyChanged;
        }

        /// <summary>
        /// The on property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ShowWinRatio":
                case "ShowWinRatioCoin":
                case "ShowWinRatioNoCoin":
                case "ShowWins":
                case "ShowWinsCoin":
                case "ShowWinsNoCoin":
                case "ShowTotalGames":
                case "ShowTotalGamesByCoin":
                case "ShowPlayedVsRatio":
                    using (var reg = new StatRegistrySettings(this.GetType()))
                    {
                        reg.ShowWinRatio = this.ShowWinRatio;
                        reg.ShowWinRatioCoin = this.ShowWinRatioCoin;
                        reg.ShowWinRatioNoCoin = this.ShowWinRatioNoCoin;
                        reg.ShowWins = this.ShowWins;
                        reg.ShowWinsCoin = this.ShowWinsCoin;
                        reg.ShowWinsNoCoin = this.ShowWinsNoCoin;
                        reg.ShowTotalGames = this.ShowTotalGames;
                        reg.ShowTotalGamesByCoin = this.ShowTotalGamesByCoin;
                        reg.ShowPlayedVsRatio = this.ShowPlayedVsRatio;
                    }

                    break;
            }
        }

        /// <summary>
        /// The get filter expression.
        /// </summary>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        protected Expression<Func<GameResult, bool>> GetFilterExpression()
        {
            var query = PredicateBuilder.True<GameResult>(); 
            if (this.FromDate != null)
            {
                var filterFromDate = this.FromDate.Value.SetToBeginOfDay();
                query = query.And(x => x.Started >= filterFromDate);
            }

            if (this.ToDate != null)
            {
                var filterToDate = this.ToDate.Value.SetToEndOfDay();
                query = query.And(x => x.Started <= filterToDate);
            }

            if (this.Server != null && !string.IsNullOrEmpty(this.Server.Name))
            {
                var serverName = this.Server.Name;
                query = query.And(x => x.Server == serverName);
            }

            if (!string.IsNullOrWhiteSpace(this.GameMode))
            {
                GameMode gm;
                if (Enum.TryParse(this.GameMode, out gm))
                {
                    query = query.And(x => x.GameMode == gm);
                }
            }

            if (!string.IsNullOrEmpty(this.Search))
            {
                var s = this.Search.ToLowerInvariant().Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var keyword in s)
                {
                    string keyword1 = keyword;
                    query = query.And(x =>
                        x.Notes.ToLower().Contains(keyword1) ||
                        x.Hero.ClassName.ToLower().Contains(keyword1) ||
                        x.OpponentHero.ClassName.ToLower().Contains(keyword1) ||
                        x.Deck.Name.ToLower().Contains(keyword1));
                }
            }

            return query;
        }
    }
}