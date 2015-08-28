namespace HearthCap.Features.Statistics
{
    using System;

    using Caliburn.Micro;

    using HearthCap.Data;

    public class StatModel : PropertyChangedBase
    {
        private Hero hero;

        private int totalGames;

        private int wins;

        private int losses;

        private int winsCoin;

        private int winsNoCoin;

        private decimal winRate;

        private decimal lossRate;

        private decimal winRateCoin;

        private decimal winRateNoCoin;

        private int lossesCoin;

        private int lossesNoCoin;

        private decimal lossRateCoin;

        private decimal lossRateNoCoin;

        private int globalTotal;

        public Hero Hero
        {
            get
            {
                return this.hero;
            }
            set
            {
                if (Equals(value, this.hero))
                {
                    return;
                }
                this.hero = value;
                this.NotifyOfPropertyChange(() => this.Hero);
            }
        }

        public int TotalGames
        {
            get
            {
                return this.totalGames;
            }
            set
            {
                if (value == this.totalGames)
                {
                    return;
                }
                this.totalGames = value;
                this.NotifyOfPropertyChange(() => this.TotalGames);
            }
        }

        public int Wins
        {
            get
            {
                return this.wins;
            }
            set
            {
                if (value == this.wins)
                {
                    return;
                }
                this.wins = value;
                this.NotifyOfPropertyChange(() => this.Wins);
            }
        }

        public int Losses
        {
            get
            {
                return this.losses;
            }
            set
            {
                if (value == this.losses)
                {
                    return;
                }
                this.losses = value;
                this.NotifyOfPropertyChange(() => this.Losses);
            }
        }

        public int WinsCoin
        {
            get
            {
                return this.winsCoin;
            }
            set
            {
                if (value == this.winsCoin)
                {
                    return;
                }
                this.winsCoin = value;
                this.NotifyOfPropertyChange(() => this.WinsCoin);
                this.NotifyOfPropertyChange(() => this.TotalCoin);
            }
        }

        public int WinsNoCoin
        {
            get
            {
                return this.winsNoCoin;
            }
            set
            {
                if (value == this.winsNoCoin)
                {
                    return;
                }
                this.winsNoCoin = value;
                this.NotifyOfPropertyChange(() => this.WinsNoCoin);
                this.NotifyOfPropertyChange(() => this.TotalNoCoin);
            }
        }

        public decimal WinRate
        {
            get
            {
                return this.winRate;
            }
            set
            {
                if (value == this.winRate)
                {
                    return;
                }
                this.winRate = value;
                this.NotifyOfPropertyChange(() => this.WinRate);
            }
        }

        public decimal LossRate
        {
            get
            {
                return this.lossRate;
            }
            set
            {
                if (value == this.lossRate)
                {
                    return;
                }
                this.lossRate = value;
                this.NotifyOfPropertyChange(() => this.LossRate);
            }
        }

        public decimal WinRateCoin
        {
            get
            {
                return this.winRateCoin;
            }
            set
            {
                if (value == this.winRateCoin)
                {
                    return;
                }
                this.winRateCoin = value;
                this.NotifyOfPropertyChange(() => this.WinRateCoin);
            }
        }

        public decimal WinRateNoCoin
        {
            get
            {
                return this.winRateNoCoin;
            }
            set
            {
                if (value == this.winRateNoCoin)
                {
                    return;
                }
                this.winRateNoCoin = value;
                this.NotifyOfPropertyChange(() => this.WinRateNoCoin);
            }
        }

        public int LossesCoin
        {
            get
            {
                return this.lossesCoin;
            }
            set
            {
                if (value == this.lossesCoin)
                {
                    return;
                }
                this.lossesCoin = value;
                this.NotifyOfPropertyChange(() => this.LossesCoin);
                this.NotifyOfPropertyChange(() => this.TotalCoin);
            }
        }

        public int LossesNoCoin
        {
            get
            {
                return this.lossesNoCoin;
            }
            set
            {
                if (value == this.lossesNoCoin)
                {
                    return;
                }
                this.lossesNoCoin = value;
                this.NotifyOfPropertyChange(() => this.LossesNoCoin);
                this.NotifyOfPropertyChange(() => this.TotalNoCoin);
            }
        }

        public decimal LossRateCoin
        {
            get
            {
                return this.lossRateCoin;
            }
            set
            {
                if (value == this.lossRateCoin)
                {
                    return;
                }
                this.lossRateCoin = value;
                this.NotifyOfPropertyChange(() => this.LossRateCoin);
            }
        }

        public decimal LossRateNoCoin
        {
            get
            {
                return this.lossRateNoCoin;
            }
            set
            {
                if (value == this.lossRateNoCoin)
                {
                    return;
                }
                this.lossRateNoCoin = value;
                this.NotifyOfPropertyChange(() => this.LossRateNoCoin);
            }
        }

        public int TotalCoin
        {
            get
            {
                return LossesCoin + WinsCoin;
            }
        }

        public int TotalNoCoin
        {
            get
            {
                return LossesNoCoin + WinsNoCoin;
            }
        }

        public decimal PlayedVsRatio
        {
            get
            {
                if (GlobalTotal == 0) return 0;
                return Math.Round(TotalGames / (decimal)GlobalTotal * 100, 0);
            }
        }

        public int GlobalTotal
        {
            get
            {
                return this.globalTotal;
            }
            set
            {
                if (value == this.globalTotal)
                {
                    return;
                }
                this.globalTotal = value;
                this.NotifyOfPropertyChange(() => this.GlobalTotal);
            }
        }
    }
}