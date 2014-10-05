// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatModel.cs" company="">
//   
// </copyright>
// <summary>
//   The stat model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Games.Statistics
{
    using System;
    using System.Windows.Media;

    using Caliburn.Micro;

    /// <summary>
    /// The stat model.
    /// </summary>
    public class StatModel : PropertyChangedBase
    {
        /// <summary>
        /// The category.
        /// </summary>
        private string category;

        /// <summary>
        /// The number.
        /// </summary>
        private float number;

        /// <summary>
        /// The brush.
        /// </summary>
        private Brush brush;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatModel"/> class.
        /// </summary>
        public StatModel() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StatModel"/> class.
        /// </summary>
        /// <param name="category">
        /// The category.
        /// </param>
        /// <param name="number">
        /// The number.
        /// </param>
        /// <param name="brush">
        /// The brush.
        /// </param>
        public StatModel(string category, float number, Brush brush = null)
        {
            this.category = category;
            this.number = (float)Math.Round(number, 0);
            this.brush = brush;
        }

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the number.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the brush.
        /// </summary>
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