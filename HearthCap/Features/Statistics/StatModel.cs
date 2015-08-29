using System;
using Caliburn.Micro;
using HearthCap.Data;

namespace HearthCap.Features.Statistics
{
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
            get { return hero; }
            set
            {
                if (Equals(value, hero))
                {
                    return;
                }
                hero = value;
                NotifyOfPropertyChange(() => Hero);
            }
        }

        public int TotalGames
        {
            get { return totalGames; }
            set
            {
                if (value == totalGames)
                {
                    return;
                }
                totalGames = value;
                NotifyOfPropertyChange(() => TotalGames);
            }
        }

        public int Wins
        {
            get { return wins; }
            set
            {
                if (value == wins)
                {
                    return;
                }
                wins = value;
                NotifyOfPropertyChange(() => Wins);
            }
        }

        public int Losses
        {
            get { return losses; }
            set
            {
                if (value == losses)
                {
                    return;
                }
                losses = value;
                NotifyOfPropertyChange(() => Losses);
            }
        }

        public int WinsCoin
        {
            get { return winsCoin; }
            set
            {
                if (value == winsCoin)
                {
                    return;
                }
                winsCoin = value;
                NotifyOfPropertyChange(() => WinsCoin);
                NotifyOfPropertyChange(() => TotalCoin);
            }
        }

        public int WinsNoCoin
        {
            get { return winsNoCoin; }
            set
            {
                if (value == winsNoCoin)
                {
                    return;
                }
                winsNoCoin = value;
                NotifyOfPropertyChange(() => WinsNoCoin);
                NotifyOfPropertyChange(() => TotalNoCoin);
            }
        }

        public decimal WinRate
        {
            get { return winRate; }
            set
            {
                if (value == winRate)
                {
                    return;
                }
                winRate = value;
                NotifyOfPropertyChange(() => WinRate);
            }
        }

        public decimal LossRate
        {
            get { return lossRate; }
            set
            {
                if (value == lossRate)
                {
                    return;
                }
                lossRate = value;
                NotifyOfPropertyChange(() => LossRate);
            }
        }

        public decimal WinRateCoin
        {
            get { return winRateCoin; }
            set
            {
                if (value == winRateCoin)
                {
                    return;
                }
                winRateCoin = value;
                NotifyOfPropertyChange(() => WinRateCoin);
            }
        }

        public decimal WinRateNoCoin
        {
            get { return winRateNoCoin; }
            set
            {
                if (value == winRateNoCoin)
                {
                    return;
                }
                winRateNoCoin = value;
                NotifyOfPropertyChange(() => WinRateNoCoin);
            }
        }

        public int LossesCoin
        {
            get { return lossesCoin; }
            set
            {
                if (value == lossesCoin)
                {
                    return;
                }
                lossesCoin = value;
                NotifyOfPropertyChange(() => LossesCoin);
                NotifyOfPropertyChange(() => TotalCoin);
            }
        }

        public int LossesNoCoin
        {
            get { return lossesNoCoin; }
            set
            {
                if (value == lossesNoCoin)
                {
                    return;
                }
                lossesNoCoin = value;
                NotifyOfPropertyChange(() => LossesNoCoin);
                NotifyOfPropertyChange(() => TotalNoCoin);
            }
        }

        public decimal LossRateCoin
        {
            get { return lossRateCoin; }
            set
            {
                if (value == lossRateCoin)
                {
                    return;
                }
                lossRateCoin = value;
                NotifyOfPropertyChange(() => LossRateCoin);
            }
        }

        public decimal LossRateNoCoin
        {
            get { return lossRateNoCoin; }
            set
            {
                if (value == lossRateNoCoin)
                {
                    return;
                }
                lossRateNoCoin = value;
                NotifyOfPropertyChange(() => LossRateNoCoin);
            }
        }

        public int TotalCoin
        {
            get { return LossesCoin + WinsCoin; }
        }

        public int TotalNoCoin
        {
            get { return LossesNoCoin + WinsNoCoin; }
        }

        public decimal PlayedVsRatio
        {
            get
            {
                if (GlobalTotal == 0)
                {
                    return 0;
                }
                return Math.Round(TotalGames / (decimal)GlobalTotal * 100, 0);
            }
        }

        public int GlobalTotal
        {
            get { return globalTotal; }
            set
            {
                if (value == globalTotal)
                {
                    return;
                }
                globalTotal = value;
                NotifyOfPropertyChange(() => GlobalTotal);
            }
        }
    }
}
