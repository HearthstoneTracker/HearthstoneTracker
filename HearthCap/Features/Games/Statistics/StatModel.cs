namespace HearthCap.Features.Games.Statistics
{
    using System;
    using System.Windows.Media;

    using Caliburn.Micro;

    public class StatModel : PropertyChangedBase
    {
        private string category;

        private float number;

        private Brush brush;

        public StatModel() { }

        public StatModel(string category, float number, Brush brush = null)
        {
            this.category = category;
            this.number = (float)Math.Round(number, 0);
            this.brush = brush;
        }

        public string Category
        {
            get
            {
                return this.category;
            }
            set
            {
                if (value == this.category)
                {
                    return;
                }
                this.category = value;
                this.NotifyOfPropertyChange(() => this.Category);
            }
        }

        public float Number
        {
            get
            {
                return this.number;
            }
            set
            {
                if (value.Equals(this.number))
                {
                    return;
                }
                this.number = value;
                this.NotifyOfPropertyChange(() => this.Number);
            }
        }

        public Brush Brush
        {
            get
            {
                return this.brush;
            }
            set
            {
                if (Equals(value, this.brush))
                {
                    return;
                }
                this.brush = value;
                this.NotifyOfPropertyChange(() => this.Brush);
            }
        }
    }
}