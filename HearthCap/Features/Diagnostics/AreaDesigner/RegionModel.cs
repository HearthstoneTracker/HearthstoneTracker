// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegionModel.cs" company="">
//   
// </copyright>
// <summary>
//   The region model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Diagnostics.AreaDesigner
{
    using System;

    using Caliburn.Micro;

    /// <summary>
    /// The region model.
    /// </summary>
    public class RegionModel : PropertyChangedBase
    {
        /// <summary>
        /// The x pos.
        /// </summary>
        private int xPos;

        /// <summary>
        /// The y pos.
        /// </summary>
        private int yPos;

        /// <summary>
        /// The height.
        /// </summary>
        private int height;

        /// <summary>
        /// The width.
        /// </summary>
        private int width;

        /// <summary>
        /// The min height.
        /// </summary>
        private int minHeight;

        /// <summary>
        /// The min width.
        /// </summary>
        private int minWidth;

        /// <summary>
        /// The moving.
        /// </summary>
        public event EventHandler<RegionMovingEventArgs> Moving;

        /// <summary>
        /// The on moving.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected virtual bool OnMoving(int x, int y)
        {
            var e = new RegionMovingEventArgs(x, y);
            var handler = this.Moving;
            if (handler != null)
            {
                handler(this, e);
            }

            return !e.Cancel;
        }

        /// <summary>
        /// The resizing.
        /// </summary>
        public event EventHandler<RegionResizingEventArgs> Resizing;

        /// <summary>
        /// The on resizing.
        /// </summary>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected virtual bool OnResizing(int width, int height)
        {
            var e = new RegionResizingEventArgs(width, height);
            var handler = this.Resizing;
            if (handler != null)
            {
                handler(this, e);
            }

            return !e.Cancel;
        }

        /// <summary>
        /// Gets or sets the x pos.
        /// </summary>
        public int XPos
        {
            get
            {
                return this.xPos;
            }

            set
            {
                if (value == this.xPos || value < 0)
                {
                    return;
                }

                if (this.OnMoving(value, this.YPos))
                {
                    this.xPos = value;
                    this.NotifyOfPropertyChange(() => this.XPos);
                }
            }
        }

        /// <summary>
        /// Gets or sets the y pos.
        /// </summary>
        public int YPos
        {
            get
            {
                return this.yPos;
            }

            set
            {
                if (value == this.yPos || value < 0)
                {
                    return;
                }

                if (this.OnMoving(this.XPos, value))
                {
                    this.yPos = value;
                    this.NotifyOfPropertyChange(() => this.YPos);
                }
            }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public int Height
        {
            get
            {
                return this.height;
            }

            set
            {
                if (value == this.height || value < 0)
                {
                    return;
                }

                if (this.OnResizing(this.Width, value))
                {
                    this.height = value;
                    this.NotifyOfPropertyChange(() => this.Height);
                }
            }
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public int Width
        {
            get
            {
                return this.width;
            }

            set
            {
                if (value == this.width || value < 0)
                {
                    return;
                }

                if (this.OnResizing(value, this.Height))
                {
                    this.width = value;
                    this.NotifyOfPropertyChange(() => this.Width);
                }
            }
        }

        /// <summary>
        /// Gets or sets the min height.
        /// </summary>
        public int MinHeight
        {
            get
            {
                return this.minHeight;
            }

            set
            {
                if (value == this.minHeight || value < 0)
                {
                    return;
                }

                this.minHeight = value;
                this.NotifyOfPropertyChange(() => this.MinHeight);
            }
        }

        /// <summary>
        /// Gets or sets the min width.
        /// </summary>
        public int MinWidth
        {
            get
            {
                return this.minWidth;
            }

            set
            {
                if (value == this.minWidth || value < 0)
                {
                    return;
                }

                this.minWidth = value;
                this.NotifyOfPropertyChange(() => this.MinWidth);
            }
        }
    }
}