// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatModel.cs" company="">
//   
// </copyright>
// <summary>
//   The stat model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Statistics
{
    using System;

    using Caliburn.Micro;

    using HearthCap.Data;

    /// <summary>
    /// The stat model.
    /// </summary>
    public class StatModel : PropertyChangedBase
    {
        /// <summary>
        /// The hero.
        /// </summary>
        private Hero hero;

        /// <summary>
        /// The total games.
        /// </summary>
        private int totalGames;

        /// <summary>
        /// The wins.
        /// </summary>
        private int wins;

        /// <summary>
        /// The losses.
        /// </summary>
        private int losses;

        /// <summary>
        /// The wins coin.
        /// </summary>
        private int winsCoin;

        /// <summary>
        /// The wins no coin.
        /// </summary>
        private int winsNoCoin;

        /// <summary>
        /// The win rate.
        /// </summary>
        private decimal winRate;

        /// <summary>
        /// The loss rate.
        /// </summary>
        private decimal lossRate;

        /// <summary>
        /// The win rate coin.
        /// </summary>
        private decimal winRateCoin;

        /// <summary>
        /// The win rate no coin.
        /// </summary>
        private decimal winRateNoCoin;

        /// <summary>
        /// The losses coin.
        /// </summary>
        private int lossesCoin;

        /// <summary>
        /// The losses no coin.
        /// </summary>
        private int lossesNoCoin;

        /// <summary>
        /// The loss rate coin.
        /// </summary>
        private decimal lossRateCoin;

        /// <summary>
        /// The loss rate no coin.
        /// </summary>
        private decimal lossRateNoCoin;

        /// <summary>
        /// The global total.
        /// </summary>
        private int globalTotal;

        /// <summary>
        /// Gets or sets the hero.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the total games.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the wins.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the losses.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the wins coin.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the wins no coin.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the win rate.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the loss rate.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the win rate coin.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the win rate no coin.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the losses coin.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the losses no coin.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the loss rate coin.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the loss rate no coin.
        /// </summary>
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

        /// <summary>
        /// Gets the total coin.
        /// </summary>
        public int TotalCoin
        {
            get
            {
                return this.LossesCoin + this.WinsCoin;
            }
        }

        /// <summary>
        /// Gets the total no coin.
        /// </summary>
        public int TotalNoCoin
        {
            get
            {
                return this.LossesNoCoin + this.WinsNoCoin;
            }
        }

        /// <summary>
        /// Gets the played vs ratio.
        /// </summary>
        public decimal PlayedVsRatio
        {
            get
            {
                if (this.GlobalTotal == 0) return 0;
                return Math.Round(this.TotalGames / (decimal)this.GlobalTotal * 100, 0);
            }
        }

        /// <summary>
        /// Gets or sets the global total.
        /// </summary>
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