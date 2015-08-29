using System;
using HearthCap.Shell.UserPreferences;

namespace HearthCap.Features.Statistics
{
    public class StatRegistrySettings : RegistrySettings
    {
        public StatRegistrySettings(Type statViewModelType)
            : base(String.Format(@"Software\HearthstoneTracker\{0}", statViewModelType.Name))
        {
        }

        public bool ShowWinRatio
        {
            get { return GetOrCreate("ShowWinRatio", true); }
            set { SetValue("ShowWinRatio", value); }
        }

        public bool ShowWinRatioCoin
        {
            get { return GetOrCreate("ShowWinRatioCoin", true); }
            set { SetValue("ShowWinRatioCoin", value); }
        }

        public bool ShowWinRatioNoCoin
        {
            get { return GetOrCreate("ShowWinRatioNoCoin", true); }
            set { SetValue("ShowWinRatioNoCoin", value); }
        }

        public bool ShowWins
        {
            get { return GetOrCreate("ShowWins", true); }
            set { SetValue("ShowWins", value); }
        }

        public bool ShowWinsCoin
        {
            get { return GetOrCreate("ShowWinsCoin", true); }
            set { SetValue("ShowWinsCoin", value); }
        }

        public bool ShowWinsNoCoin
        {
            get { return GetOrCreate("ShowWinsNoCoin", true); }
            set { SetValue("ShowWinsNoCoin", value); }
        }

        public bool ShowTotalGames
        {
            get { return GetOrCreate("ShowTotalGames", true); }
            set { SetValue("ShowTotalGames", value); }
        }

        public bool ShowTotalGamesByCoin
        {
            get { return GetOrCreate("ShowTotalGamesByCoin", true); }
            set { SetValue("ShowTotalGamesByCoin", value); }
        }

        public bool ShowPlayedVsRatio
        {
            get { return GetOrCreate("ShowPlayedVsRatio", true); }
            set { SetValue("ShowPlayedVsRatio", value); }
        }
    }
}
