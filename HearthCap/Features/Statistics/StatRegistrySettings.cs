namespace HearthCap.Features.Statistics
{
    using System;

    using HearthCap.Shell.UserPreferences;

    public class StatRegistrySettings : RegistrySettings
    {
        public StatRegistrySettings(Type statViewModelType)
            : base(String.Format(@"Software\HearthstoneTracker\{0}", statViewModelType.Name))
        {
        }

        public bool ShowWinRatio
        {
            get
            {
                return this.GetOrCreate("ShowWinRatio", true);
            }
            set
            {
                this.SetValue("ShowWinRatio", value);
            }
        }

        public bool ShowWinRatioCoin
        {
            get
            {
                return this.GetOrCreate("ShowWinRatioCoin", true);
            }
            set
            {
                this.SetValue("ShowWinRatioCoin", value);
            }
        }

        public bool ShowWinRatioNoCoin
        {
            get
            {
                return this.GetOrCreate("ShowWinRatioNoCoin", true);
            }
            set
            {
                this.SetValue("ShowWinRatioNoCoin", value);
            }
        }

        public bool ShowWins
        {
            get
            {
                return this.GetOrCreate("ShowWins", true);
            }
            set
            {
                this.SetValue("ShowWins", value);
            }
        }

        public bool ShowWinsCoin
        {
            get
            {
                return this.GetOrCreate("ShowWinsCoin", true);
            }
            set
            {
                this.SetValue("ShowWinsCoin", value);
            }
        }

        public bool ShowWinsNoCoin
        {
            get
            {
                return this.GetOrCreate("ShowWinsNoCoin", true);
            }
            set
            {
                this.SetValue("ShowWinsNoCoin", value);
            }
        }

        public bool ShowTotalGames
        {
            get
            {
                return this.GetOrCreate("ShowTotalGames", true);
            }
            set
            {
                this.SetValue("ShowTotalGames", value);
            }
        }

        public bool ShowTotalGamesByCoin
        {
            get
            {
                return this.GetOrCreate("ShowTotalGamesByCoin", true);
            }
            set
            {
                this.SetValue("ShowTotalGamesByCoin", value);
            }
        }

        public bool ShowPlayedVsRatio
        {
            get
            {
                return this.GetOrCreate("ShowPlayedVsRatio", true);
            }
            set
            {
                this.SetValue("ShowPlayedVsRatio", value);
            }
        }

    }
}