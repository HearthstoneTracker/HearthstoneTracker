// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStatsViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The StatsViewModel interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Statistics
{
    using System;

    using HearthCap.Features.Core;

    /// <summary>
    /// The StatsViewModel interface.
    /// </summary>
    public interface IStatsViewModel
    {
        /// <summary>
        /// The refresh data.
        /// </summary>
        void RefreshData();

        /// <summary>
        /// Gets or sets the from date.
        /// </summary>
        DateTime? FromDate { get; set; }

        /// <summary>
        /// Gets or sets the to date.
        /// </summary>
        DateTime? ToDate { get; set; }

        /// <summary>
        /// Gets or sets the game mode.
        /// </summary>
        string GameMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show win ratio.
        /// </summary>
        bool ShowWinRatio { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show win ratio coin.
        /// </summary>
        bool ShowWinRatioCoin { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show win ratio no coin.
        /// </summary>
        bool ShowWinRatioNoCoin { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show wins.
        /// </summary>
        bool ShowWins { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show wins coin.
        /// </summary>
        bool ShowWinsCoin { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show wins no coin.
        /// </summary>
        bool ShowWinsNoCoin { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show total games.
        /// </summary>
        bool ShowTotalGames { get; set; }

        /// <summary>
        /// Gets or sets the server.
        /// </summary>
        ServerItemModel Server { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show total games by coin.
        /// </summary>
        bool ShowTotalGamesByCoin { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show played vs ratio.
        /// </summary>
        bool ShowPlayedVsRatio { get; set; }

        /// <summary>
        /// Gets or sets the search.
        /// </summary>
        string Search { get; set; }
    }
}