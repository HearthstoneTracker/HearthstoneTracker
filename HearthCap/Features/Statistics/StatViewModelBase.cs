namespace HearthCap.Features.Statistics
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Linq.Expressions;

    using Caliburn.Micro;

    using HearthCap.Data;
    using HearthCap.Features.Core;
    using HearthCap.Framework;
    using HearthCap.Util;

    public abstract class StatViewModelBase : Screen, IStatsViewModel
    {
        private bool showWinRatio;

        private bool showWins;

        private bool showWinRatioCoin;

        private bool showWinRatioNoCoin;

        private bool showWinsCoin;

        private bool showWinsNoCoin;

        private DateTime? fromDate;

        private string gameMode;

        private DateTime? toDate;

        private bool showTotalGames;

        private ServerItemModel server;

        private bool showTotalGamesByCoin;

        private bool showPlayedVsRatio;

        private string search;

        public abstract void RefreshData();

        public IBusyWatcher Busy { get; set; }

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

        private bool IsLastSelected(bool value, int min = 1)
        {
            if (value) return false;
            return Truth(
                ShowWinRatio,
                ShowWinRatioCoin,
                ShowWinRatioNoCoin,
                ShowWins,
                ShowWinsCoin,
                ShowTotalGames,
                ShowTotalGamesByCoin,
                ShowPlayedVsRatio,
                ShowWinsNoCoin) <= min;
        }

        private static int Truth(params bool[] booleans)
        {
            return booleans.Count(b => b);
        }

        protected StatViewModelBase()
        {
            GameMode = String.Empty;

            using (var reg = new StatRegistrySettings(this.GetType()))
            {
                ShowWinRatio = reg.ShowWinRatio;
                ShowWinRatioCoin = reg.ShowWinRatioCoin;
                ShowWinRatioNoCoin = reg.ShowWinRatioNoCoin;
                ShowWins = reg.ShowWins;
                ShowWinsCoin = reg.ShowWinsCoin;
                ShowWinsNoCoin = reg.ShowWinsNoCoin;
                ShowTotalGames = reg.ShowTotalGames;
                ShowTotalGamesByCoin = reg.ShowTotalGamesByCoin;
                ShowPlayedVsRatio = reg.ShowPlayedVsRatio;
            }
            if (IsLastSelected(false, 0))
            {
                ShowWinRatio = true;
                ShowWins = true;
                ShowTotalGames = true;
            }
            Busy = new BusyWatcher();
            this.PropertyChanged += OnPropertyChanged;
        }

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
                        reg.ShowWinRatio = ShowWinRatio;
                        reg.ShowWinRatioCoin = ShowWinRatioCoin;
                        reg.ShowWinRatioNoCoin = ShowWinRatioNoCoin;
                        reg.ShowWins = ShowWins;
                        reg.ShowWinsCoin = ShowWinsCoin;
                        reg.ShowWinsNoCoin = ShowWinsNoCoin;
                        reg.ShowTotalGames = ShowTotalGames;
                        reg.ShowTotalGamesByCoin = ShowTotalGamesByCoin;
                        reg.ShowPlayedVsRatio = ShowPlayedVsRatio;
                    }
                    break;
            }
        }

        protected Expression<Func<GameResult, bool>> GetFilterExpression()
        {
            var query = PredicateBuilder.True<GameResult>(); ;
            if (FromDate != null)
            {
                var filterFromDate = FromDate.Value.SetToBeginOfDay();
                query = query.And(x => x.Started >= filterFromDate);
            }
            if (ToDate != null)
            {
                var filterToDate = ToDate.Value.SetToEndOfDay();
                query = query.And(x => x.Started <= filterToDate);
            }

            if (this.Server != null && !String.IsNullOrEmpty(Server.Name))
            {
                var serverName = Server.Name;
                query = query.And(x => x.Server == serverName);
            }

            if (!string.IsNullOrWhiteSpace(GameMode))
            {
                GameMode gm;
                if (Enum.TryParse(GameMode, out gm))
                {
                    query = query.And(x => x.GameMode == gm);
                }
            }

            if (!String.IsNullOrEmpty(Search))
            {
                var s = Search.ToLowerInvariant().Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
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