namespace HearthCap.Core.GameCapture.HS
{
    using System;
    using System.Drawing;

    public static class ResolutionHelper
    {
        static Func<decimal, decimal> up = (p) => Math.Round(p, 0);
        static Func<decimal, decimal> down = (p) => Math.Round(p, 0);

        public static Rectangle CorrectRectangle(Size resolution, Rectangle rect, int baseResolution)
        {
            var scaling = GetScaleFactor(resolution, baseResolution);

            //if (scaling < 1)
            //{
            //    up = (p) => Math.Ceiling(p);
            //    down = (p) => Math.Floor(p);
            //}
            //else if (scaling > 1)
            //{
            //    up = (p) => Math.Floor(p);
            //    down = (p) => Math.Ceiling(p);
            //}

            var x = (int)((up(scaling * rect.X + GetBoardX(resolution))));
            var y = (int)((up(scaling * rect.Y)));
            var width = (int)(down(rect.Width * scaling));
            var height = (int)(down(rect.Height * scaling));

            return new Rectangle(x, y, width, height);
        }

        public static decimal GetScaleFactor(Size resolution, int baseResolution)
        {
            if (resolution.Height == baseResolution)
            {
                return 1;
            }

            return resolution.Height / (decimal)baseResolution;
        }

        public static decimal GetBoardWidth(Size resolution)
        {
            return resolution.Height / (decimal)3 * 4;
        }

        public static decimal GetBoardX(Size resolution)
        {
            decimal x = (resolution.Width - GetBoardWidth(resolution)) / 2;
            return x;
        }

        public static Size CorrectSize(Size resolution, Size rect, int baseResolution)
        {
            var scaling = GetScaleFactor(resolution, baseResolution);

            //if (scaling < 1)
            //{
            //    up = (p) => Math.Ceiling(p);
            //    down = (p) => Math.Floor(p);
            //}
            //else if (scaling > 1)
            //{
            //    up = (p) => Math.Floor(p);
            //    down = (p) => Math.Ceiling(p);
            //}

            var width = (int)(down(rect.Width * scaling));
            var height = (int)(down(rect.Height * scaling));
            return new Size(width, height);
        }

        public static Rectangle CorrectPoints(Size resolution, Rectangle rect, int baseResolution)
        {
            var scaling = GetScaleFactor(resolution, baseResolution);

            //if (scaling < 1)
            //{
            //    up = (p) => Math.Ceiling(p);
            //    down = (p) => Math.Floor(p);
            //}
            //else if (scaling > 1)
            //{
            //    up = (p) => Math.Floor(p);
            //    down = (p) => Math.Ceiling(p);
            //}

            var x = (int)((up(scaling * rect.X)) + GetBoardX(resolution));
            var y = (int)((up(scaling * rect.Y)));
            return new Rectangle(x, y, rect.Width, rect.Height);
        }
    }
}