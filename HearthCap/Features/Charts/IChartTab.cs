using System;
using System.ComponentModel;
using System.Linq.Expressions;
using Caliburn.Micro;
using HearthCap.Data;

namespace HearthCap.Features.Charts
{
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
            get { return order; }
            set
            {
                if (value == order)
                {
                    return;
                }
                order = value;
                NotifyOfPropertyChange(() => Order);
            }
        }

        public abstract void RefreshData(Expression<Func<GameResult, bool>> gameFilter, Expression<Func<ArenaSession, bool>> arenaFilter);
    }
}
