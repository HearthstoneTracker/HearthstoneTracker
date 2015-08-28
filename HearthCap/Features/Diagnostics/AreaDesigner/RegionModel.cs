namespace HearthCap.Features.Diagnostics.AreaDesigner
{
    using System;

    using Caliburn.Micro;

    public class RegionModel : PropertyChangedBase
    {
        private int xPos;

        private int yPos;

        private int height;

        private int width;

        private int minHeight;

        private int minWidth;

        public event EventHandler<RegionMovingEventArgs> Moving;

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

        public event EventHandler<RegionResizingEventArgs> Resizing;

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

                if (OnMoving(value, YPos))
                {
                    this.xPos = value;
                    this.NotifyOfPropertyChange(() => this.XPos);
                }
            }
        }

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

                if (OnMoving(XPos, value))
                {
                    this.yPos = value;
                    this.NotifyOfPropertyChange(() => this.YPos);
                }
            }
        }

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

                if (OnResizing(Width, value))
                {
                    this.height = value;
                    this.NotifyOfPropertyChange(() => this.Height);
                }
            }
        }

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
                if (OnResizing(value, Height))
                {
                    this.width = value;
                    this.NotifyOfPropertyChange(() => this.Width);
                }
            }
        }

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