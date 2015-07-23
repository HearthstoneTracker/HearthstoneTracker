namespace HearthCap.Features.Charts
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;

    using Caliburn.Micro;

    using HearthCap.Data;

    public interface IChartTab : INotifyPropertyChanged
    {
        int Order { get; set; }

        void RefreshData(Expression<Func<GameResult, bool>> gameFilter, Expression<Func<ArenaSession, bool>> arenaFilter);
    }

    public abstract class ChartTab : Screen, IChartTab
    {
        private int order;

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

        public abstract void RefreshData(Expression<Func<GameResult, bool>> gameFilter, Expression<Func<ArenaSession, bool>> arenaFilter);
    }
}