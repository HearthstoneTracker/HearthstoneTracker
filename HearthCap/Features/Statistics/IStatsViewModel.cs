namespace HearthCap.Features.Statistics
{
    using System;

    using HearthCap.Features.Core;

    public interface IStatsViewModel
    {
        void RefreshData();

        DateTime? FromDate { get; set; }

        DateTime? ToDate { get; set; }

        string GameMode { get; set; }

        bool ShowWinRatio { get; set; }

        bool ShowWinRatioCoin { get; set; }

        bool ShowWinRatioNoCoin { get; set; }

        bool ShowWins { get; set; }

        bool ShowWinsCoin { get; set; }

        bool ShowWinsNoCoin { get; set; }

        bool ShowTotalGames { get; set; }

        ServerItemModel Server { get; set; }

        bool ShowTotalGamesByCoin { get; set; }

        bool ShowPlayedVsRatio { get; set; }

        string Search { get; set; }
    }
}