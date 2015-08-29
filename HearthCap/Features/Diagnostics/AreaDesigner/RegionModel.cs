using System;
using Caliburn.Micro;

namespace HearthCap.Features.Diagnostics.AreaDesigner
{
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
            var handler = Moving;
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
            var handler = Resizing;
            if (handler != null)
            {
                handler(this, e);
            }

            return !e.Cancel;
        }

        public int XPos
        {
            get { return xPos; }
            set
            {
                if (value == xPos
                    || value < 0)
                {
                    return;
                }

                if (OnMoving(value, YPos))
                {
                    xPos = value;
                    NotifyOfPropertyChange(() => XPos);
                }
            }
        }

        public int YPos
        {
            get { return yPos; }
            set
            {
                if (value == yPos
                    || value < 0)
                {
                    return;
                }

                if (OnMoving(XPos, value))
                {
                    yPos = value;
                    NotifyOfPropertyChange(() => YPos);
                }
            }
        }

        public int Height
        {
            get { return height; }
            set
            {
                if (value == height
                    || value < 0)
                {
                    return;
                }

                if (OnResizing(Width, value))
                {
                    height = value;
                    NotifyOfPropertyChange(() => Height);
                }
            }
        }

        public int Width
        {
            get { return width; }
            set
            {
                if (value == width
                    || value < 0)
                {
                    return;
                }
                if (OnResizing(value, Height))
                {
                    width = value;
                    NotifyOfPropertyChange(() => Width);
                }
            }
        }

        public int MinHeight
        {
            get { return minHeight; }
            set
            {
                if (value == minHeight
                    || value < 0)
                {
                    return;
                }
                minHeight = value;
                NotifyOfPropertyChange(() => MinHeight);
            }
        }

        public int MinWidth
        {
            get { return minWidth; }
            set
            {
                if (value == minWidth
                    || value < 0)
                {
                    return;
                }
                minWidth = value;
                NotifyOfPropertyChange(() => MinWidth);
            }
        }
    }
}
