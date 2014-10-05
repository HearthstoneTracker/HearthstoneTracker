// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IChartTab.cs" company="">
//   
// </copyright>
// <summary>
//   The ChartTab interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Charts
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;

    using Caliburn.Micro;

    using HearthCap.Data;

    /// <summary>
    /// The ChartTab interface.
    /// </summary>
    public interface IChartTab : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        int Order { get; set; }

        /// <summary>
        /// The refresh data.
        /// </summary>
        /// <param name="gameFilter">
        /// The game filter.
        /// </param>
        /// <param name="arenaFilter">
        /// The arena filter.
        /// </param>
        void RefreshData(Expression<Func<GameResult, bool>> gameFilter, Expression<Func<ArenaSession, bool>> arenaFilter);
    }

    /// <summary>
    /// The chart tab.
    /// </summary>
    public abstract class ChartTab : Screen, IChartTab
    {
        /// <summary>
        /// The order.
        /// </summary>
        private int order;

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        public int Order
        {
            get
            {
                return this.order;
            }

            set
            {
                if (value == this.order)
                {
                    return;
                }

                this.order = value;
                this.NotifyOfPropertyChange(() => this.Order);
            }
        }

        /// <summary>
        /// The refresh data.
        /// </summary>
        /// <param name="gameFilter">
        /// The game filter.
        /// </param>
        /// <param name="arenaFilter">
        /// The arena filter.
        /// </param>
        public abstract void RefreshData(Expression<Func<GameResult, bool>> gameFilter, Expression<Func<ArenaSession, bool>> arenaFilter);
    }
}