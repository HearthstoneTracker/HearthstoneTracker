using System;
using System.Windows.Media;
using Caliburn.Micro;

namespace HearthCap.Features.ArenaSessions.Statistics
{
    public class StatModel : PropertyChangedBase
    {
        private string category;

        private float number;

        private Brush brush;

        public StatModel()
        {
        }

        public StatModel(string category, float number, Brush brush = null)
        {
            this.category = category;
            this.number = (float)Math.Round(number, 0);
            this.brush = brush;
        }

        public string Category
        {
            get { return category; }
            set
            {
                if (value == category)
                {
                    return;
                }
                category = value;
                NotifyOfPropertyChange(() => Category);
            }
        }

        public float Number
        {
            get { return number; }
            set
            {
                if (value.Equals(number))
                {
                    return;
                }
                number = value;
                NotifyOfPropertyChange(() => Number);
            }
        }

        public Brush Brush
        {
            get { return brush; }
            set
            {
                if (Equals(value, brush))
                {
                    return;
                }
                brush = value;
                NotifyOfPropertyChange(() => Brush);
            }
        }
    }
}
