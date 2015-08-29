using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using Caliburn.Micro;
using HearthCap.Data;
using HearthCap.Features.Core;
using HearthCap.Framework;
using HearthCap.Util;

namespace HearthCap.Features.Statistics
{
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
            get { return fromDate; }
            set
            {
                if (value.Equals(fromDate))
                {
                    return;
                }
                fromDate = value;
                NotifyOfPropertyChange(() => FromDate);
            }
        }

        public DateTime? ToDate
        {
            get { return toDate; }
            set
            {
                if (value.Equals(toDate))
                {
                    return;
                }
                toDate = value;
                NotifyOfPropertyChange(() => ToDate);
            }
        }

        public string GameMode
        {
            get { return gameMode; }
            set
            {
                if (value == gameMode)
                {
                    return;
                }
                gameMode = value;
                NotifyOfPropertyChange(() => GameMode);
            }
        }

        public bool ShowWinRatio
        {
            get { return showWinRatio; }
            set
            {
                if (value.Equals(showWinRatio)
                    || IsLastSelected(value))
                {
                    return;
                }
                showWinRatio = value;
                NotifyOfPropertyChange(() => ShowWinRatio);
            }
        }

        public bool ShowWins
        {
            get { return showWins; }
            set
            {
                if (value.Equals(showWins)
                    || IsLastSelected(value))
                {
                    return;
                }
                showWins = value;
                NotifyOfPropertyChange(() => ShowWins);
            }
        }

        public bool ShowWinRatioCoin
        {
            get { return showWinRatioCoin; }
            set
            {
                if (value.Equals(showWinRatioCoin)
                    || IsLastSelected(value))
                {
                    return;
                }
                showWinRatioCoin = value;
                NotifyOfPropertyChange(() => ShowWinRatioCoin);
            }
        }

        public bool ShowWinRatioNoCoin
        {
            get { return showWinRatioNoCoin; }
            set
            {
                if (value.Equals(showWinRatioNoCoin)
                    || IsLastSelected(value))
                {
                    return;
                }
                showWinRatioNoCoin = value;
                NotifyOfPropertyChange(() => ShowWinRatioNoCoin);
            }
        }

        public bool ShowWinsCoin
        {
            get { return showWinsCoin; }
            set
            {
                if (value.Equals(showWinsCoin)
                    || IsLastSelected(value))
                {
                    return;
                }
                showWinsCoin = value;
                NotifyOfPropertyChange(() => ShowWinsCoin);
            }
        }

        public bool ShowWinsNoCoin
        {
            get { return showWinsNoCoin; }
            set
            {
                if (value.Equals(showWinsNoCoin)
                    || IsLastSelected(value))
                {
                    return;
                }
                showWinsNoCoin = value;
                NotifyOfPropertyChange(() => ShowWinsNoCoin);
            }
        }

        public bool ShowTotalGames
        {
            get { return showTotalGames; }
            set
            {
                if (value.Equals(showTotalGames)
                    || IsLastSelected(value))
                {
                    return;
                }
                showTotalGames = value;
                NotifyOfPropertyChange(() => ShowTotalGames);
            }
        }

        public bool ShowPlayedVsRatio
        {
            get { return showPlayedVsRatio; }
            set
            {
                if (value.Equals(showPlayedVsRatio)
                    || IsLastSelected(value))
                {
                    return;
                }
                showPlayedVsRatio = value;
                NotifyOfPropertyChange(() => ShowPlayedVsRatio);
            }
        }

        public ServerItemModel Server
        {
            get { return server; }
            set
            {
                if (Equals(value, server))
                {
                    return;
                }
                server = value;
                NotifyOfPropertyChange(() => Server);
            }
        }

        public string Search
        {
            get { return search; }
            set
            {
                if (value == search)
                {
                    return;
                }
                search = value;
                NotifyOfPropertyChange(() => Search);
            }
        }

        public bool ShowTotalGamesByCoin
        {
            get { return showTotalGamesByCoin; }
            set
            {
                if (value.Equals(showTotalGamesByCoin)
                    || IsLastSelected(value))
                {
                    return;
                }
                showTotalGamesByCoin = value;
                NotifyOfPropertyChange(() => ShowTotalGamesByCoin);
            }
        }

        private bool IsLastSelected(bool value, int min = 1)
        {
            if (value)
            {
                return false;
            }
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

            using (var reg = new StatRegistrySettings(GetType()))
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
            PropertyChanged += OnPropertyChanged;
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
                    using (var reg = new StatRegistrySettings(GetType()))
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
            var query = PredicateBuilder.True<GameResult>();
            ;
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

            if (Server != null
                && !String.IsNullOrEmpty(Server.Name))
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
                    var keyword1 = keyword;
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
