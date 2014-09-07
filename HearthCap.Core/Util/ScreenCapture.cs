namespace HearthCap.Core.Util
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Forms;

    using NLog;

    using Point = System.Drawing.Point;

    /// <summary>
    /// Provides functions to capture the entire screen, or a particular window, and save it to a file.
    /// </summary>
    public class ScreenCapture
    {
        private readonly Logger Log = NLog.LogManager.GetCurrentClassLogger();

        public Rectangle GetWindowSize(IntPtr hWnd)
        {
            User32.RECT rect = new User32.RECT();
            User32.GetWindowRect(hWnd, ref rect);
            return rect;
        }

        public Rectangle GetClientSize(IntPtr hWnd)
        {
            User32.RECT rect = new User32.RECT();
            User32.GetClientRect(hWnd, ref rect);
            return rect;
        }

        /// <summary>
        /// Gets a segment of the desktop as an image.
        /// </summary>
        /// <returns>A <see cref="System.Drawing.Image"/> containg an image of the full desktop.</returns>
        public Image GetDesktopBitmapBg(IntPtr hWnd, bool forcePrintWindow = false, bool forceBitBlt = false)
        {
            var rect = new User32.RECT();
            User32.GetClientRect(hWnd, ref rect);

            bool dwmEnabled;
            DWM.DwmIsCompositionEnabled(out dwmEnabled);
            if ((!dwmEnabled && !forcePrintWindow) || forceBitBlt)
            {
                return this.GetDesktopBitmap(hWnd, rect);
            }

            var img = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppRgb);
            using (var g = Graphics.FromImage(img))
            {
                IntPtr dc = g.GetHdc();
                // User32.RedrawWindow(hWnd, IntPtr.Zero, IntPtr.Zero, User32.RedrawWindowFlags.Frame | User32.RedrawWindowFlags.Invalidate | User32.RedrawWindowFlags.Erase | User32.RedrawWindowFlags.UpdateNow | User32.RedrawWindowFlags.AllChildren);
                bool success = User32.PrintWindow(hWnd, dc, 1);
                g.ReleaseHdc(dc);
                GDI32.DeleteDC(dc);
                if (!success && !forcePrintWindow)
                {
                    return this.GetDesktopBitmap(hWnd, rect);
                }
                if (!forcePrintWindow && img.Width > 64 && img.Height > 64 && img.IsAllBlack())
                {
                    return this.GetDesktopBitmap(hWnd, rect);
                }
            }
            return img;
        }

        private Image GetDesktopBitmap(IntPtr hWnd, User32.RECT rect)
        {
            var crect = new User32.RECT(rect.Left, rect.Top, rect.Right, rect.Bottom);
            var p = new User32.POINT(0, 0);
            User32.ClientToScreen(hWnd, ref p);
            crect.Top = p.Y;
            crect.Left = p.X;
            crect.Bottom = p.Y + crect.Bottom;
            crect.Right = p.X + crect.Right;
            if (VisibilityTester.HitTest(crect, hWnd, new[] { new Point(p.X + 1, p.Y + 1) }, IntPtr.Zero))
            {
                return GetDesktopBitmap(hWnd);
            }
            return null;
        }

        /// <summary>
        /// Gets a segment of the desktop as an image.
        /// </summary>
        /// <returns>A <see cref="System.Drawing.Image"/> containg an image of the full desktop.</returns>
        public Image GetDesktopBitmap(IntPtr hWnd)
        {
            Image capture = null;

            try
            {
                var crect = new User32.RECT();
                User32.GetClientRect(hWnd, ref crect);
                var p = new User32.POINT(0, 0);
                User32.ClientToScreen(hWnd, ref p);
                crect.Top = p.Y;
                crect.Left = p.X;
                crect.Bottom = p.Y + crect.Bottom;
                crect.Right = p.X + crect.Right;
                capture = this.GetDesktopBitmap(crect);

                return capture;
            }
            finally
            {
            }
        }

        /// <summary>
        /// Gets a segment of the desktop as an image.
        /// </summary>
        /// <param name="rectangle">The rectangular area to capture.</param>
        /// <returns>A <see cref="System.Drawing.Image"/> containg an image of the desktop 
        /// at the specified coordinates</returns>
        public Image GetDesktopBitmap(Rectangle rectangle)
        {
            return this.GetDesktopBitmap(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        /// <summary>
        /// Retrieves an image of the specified part of your screen.
        /// </summary>
        /// <param name="x">The X coordinate of the requested area</param> 
        /// <param name="y">The Y coordinate of the requested area</param> 
        /// <param name="width">The width of the requested area</param> 
        /// <param name="height">The height of the requested area</param> 
        /// <returns>A <see cref="System.Drawing.Image"/> of the desktop at 
        /// the specified coordinates.</returns> 
        public Image GetDesktopBitmap(int x, int y, int width, int height)
        {
            //Create the image and graphics to capture the portion of the desktop.
            Image destinationImage = new Bitmap(width, height, PixelFormat.Format32bppRgb);
            using (Graphics destinationGraphics = Graphics.FromImage(destinationImage))
            {

                IntPtr destinationGraphicsHandle = IntPtr.Zero;
                IntPtr windowDC = IntPtr.Zero;

                try
                {
                    //Pointers for window handles
                    destinationGraphicsHandle = destinationGraphics.GetHdc();
                    windowDC = User32.GetDC(IntPtr.Zero);

                    //Get the screencapture
                    var dwRop = GDI32.TernaryRasterOperations.SRCCOPY;

                    GDI32.BitBlt(destinationGraphicsHandle, 0, 0, width, height, windowDC, x, y, dwRop);
                }
                finally
                {
                    destinationGraphics.ReleaseHdc(destinationGraphicsHandle);
                    GDI32.DeleteDC(windowDC);
                }
            }

            // Don't forget to dispose this image
            return destinationImage;
        }

        /// <summary>
        /// Gets a segment of the desktop as an image.
        /// </summary>
        /// <returns>A <see cref="System.Drawing.Image"/> containg an image of the full desktop.</returns>
        public Image GetWindowBitmap(IntPtr hWnd)
        {
            User32.RECT windowRect = new User32.RECT();
            User32.GetWindowRect(hWnd, ref windowRect);
            int width = windowRect.Right - windowRect.Left;
            int height = windowRect.Bottom - windowRect.Top;

            Image destinationImage = new Bitmap(width, height, PixelFormat.Format32bppRgb);
            using (Graphics destinationGraphics = Graphics.FromImage(destinationImage))
            {
                IntPtr destinationGraphicsHandle = IntPtr.Zero;
                IntPtr windowDC = IntPtr.Zero;

                try
                {
                    //Pointers for window handles
                    destinationGraphicsHandle = destinationGraphics.GetHdc();
                    windowDC = User32.GetWindowDC(hWnd);

                    //Get the screencapture
                    var dwRop = GDI32.TernaryRasterOperations.SRCCOPY | GDI32.TernaryRasterOperations.CAPTUREBLT;
                    User32.RedrawWindow(hWnd, IntPtr.Zero, IntPtr.Zero, User32.RedrawWindowFlags.InternalPaint);

                    GDI32.BitBlt(destinationGraphicsHandle, 0, 0, width, height, windowDC, 0, 0, dwRop);
                }
                finally
                {
                    destinationGraphics.ReleaseHdc(destinationGraphicsHandle);
                    GDI32.DeleteDC(destinationGraphicsHandle);
                    // User32.ReleaseDC(windowDC)
                    GDI32.DeleteDC(windowDC);
                }
            }

            // Don't forget to dispose this image
            return destinationImage;
        }

        private class DWM
        {
            [DllImport("dwmapi.dll")]
            public static extern int DwmIsCompositionEnabled(out bool enabled);
        }

        /// <summary>
        /// Helper class containing Gdi32 API functions
        /// </summary>
        private class GDI32
        {
            [DllImport("gdi32.dll")]
            public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hObjectSource, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

            /// <summary>
            ///        Deletes the specified device context (DC).
            /// </summary>
            /// <param name="hdc">A handle to the device context.</param>
            /// <returns>If the function succeeds, the return value is <c>true</c>. If the function fails, the return value is <c>false</c>.</returns>
            [DllImport("gdi32.dll", EntryPoint = "DeleteDC")]
            public static extern bool DeleteDC([In] IntPtr hdc);

            /// <summary>
            ///     Specifies a raster-operation code. These codes define how the color data for the
            ///     source rectangle is to be combined with the color data for the destination
            ///     rectangle to achieve the final color.
            /// </summary>
            public enum TernaryRasterOperations : uint
            {
                /// <summary>dest = source</summary>
                SRCCOPY = 0x00CC0020,
                /// <summary>dest = source OR dest</summary>
                SRCPAINT = 0x00EE0086,
                /// <summary>dest = source AND dest</summary>
                SRCAND = 0x008800C6,
                /// <summary>dest = source XOR dest</summary>
                SRCINVERT = 0x00660046,
                /// <summary>dest = source AND (NOT dest)</summary>
                SRCERASE = 0x00440328,
                /// <summary>dest = (NOT source)</summary>
                NOTSRCCOPY = 0x00330008,
                /// <summary>dest = (NOT src) AND (NOT dest)</summary>
                NOTSRCERASE = 0x001100A6,
                /// <summary>dest = (source AND pattern)</summary>
                MERGECOPY = 0x00C000CA,
                /// <summary>dest = (NOT source) OR dest</summary>
                MERGEPAINT = 0x00BB0226,
                /// <summary>dest = pattern</summary>
                PATCOPY = 0x00F00021,
                /// <summary>dest = DPSnoo</summary>
                PATPAINT = 0x00FB0A09,
                /// <summary>dest = pattern XOR dest</summary>
                PATINVERT = 0x005A0049,
                /// <summary>dest = (NOT dest)</summary>
                DSTINVERT = 0x00550009,
                /// <summary>dest = BLACK</summary>
                BLACKNESS = 0x00000042,
                /// <summary>dest = WHITE</summary>
                WHITENESS = 0x00FF0062,
                /// <summary>
                /// Capture window as seen on screen.  This includes layered windows 
                /// such as WPF windows with AllowsTransparency="true"
                /// </summary>
                CAPTUREBLT = 0x40000000
            }
        }

        /// <summary>
        /// Helper class containing User32 API functions
        /// </summary>
        private class User32
        {
            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern bool IsWindowVisible(IntPtr hWnd);

            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);

            [DllImport("user32.dll")]
            public static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

            [DllImport("user32.dll")]
            public static extern IntPtr GetDesktopWindow();

            [DllImport("user32.dll")]
            public static extern IntPtr GetDC(IntPtr hWnd);

            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowDC(IntPtr hWnd);

            [DllImport("user32.dll")]
            public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);

            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);

            [DllImport("user32.dll")]
            public static extern IntPtr GetClientRect(IntPtr hWnd, ref RECT rect);

            [DllImport("user32.dll")]
            public static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, RedrawWindowFlags flags);

            [StructLayout(LayoutKind.Sequential)]
            public struct POINT
            {
                public int X;
                public int Y;

                public POINT(int x, int y)
                {
                    this.X = x;
                    this.Y = y;
                }

                public POINT(Point pt) : this(pt.X, pt.Y) { }

                public static implicit operator Point(POINT p)
                {
                    return new Point(p.X, p.Y);
                }

                public static implicit operator POINT(Point p)
                {
                    return new POINT(p.X, p.Y);
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int Left, Top, Right, Bottom;

                public RECT(int left, int top, int right, int bottom)
                {
                    Left = left;
                    Top = top;
                    Right = right;
                    Bottom = bottom;
                }

                public RECT(System.Drawing.Rectangle r) : this(r.Left, r.Top, r.Right, r.Bottom) { }

                public int X
                {
                    get { return Left; }
                    set { Right -= (Left - value); Left = value; }
                }

                public int Y
                {
                    get { return Top; }
                    set { Bottom -= (Top - value); Top = value; }
                }

                public int Height
                {
                    get { return Bottom - Top; }
                    set { Bottom = value + Top; }
                }

                public int Width
                {
                    get { return Right - Left; }
                    set { Right = value + Left; }
                }

                public System.Drawing.Point Location
                {
                    get { return new System.Drawing.Point(Left, Top); }
                    set { X = value.X; Y = value.Y; }
                }

                public System.Drawing.Size Size
                {
                    get { return new System.Drawing.Size(Width, Height); }
                    set { Width = value.Width; Height = value.Height; }
                }

                public static implicit operator System.Drawing.Rectangle(RECT r)
                {
                    return new System.Drawing.Rectangle(r.Left, r.Top, r.Width, r.Height);
                }

                public static implicit operator RECT(System.Drawing.Rectangle r)
                {
                    return new RECT(r);
                }

                public static bool operator ==(RECT r1, RECT r2)
                {
                    return r1.Equals(r2);
                }

                public static bool operator !=(RECT r1, RECT r2)
                {
                    return !r1.Equals(r2);
                }

                public bool Equals(RECT r)
                {
                    return r.Left == Left && r.Top == Top && r.Right == Right && r.Bottom == Bottom;
                }

                public override bool Equals(object obj)
                {
                    if (obj is RECT)
                        return Equals((RECT)obj);
                    else if (obj is System.Drawing.Rectangle)
                        return Equals(new RECT((System.Drawing.Rectangle)obj));
                    return false;
                }

                public override int GetHashCode()
                {
                    return ((System.Drawing.Rectangle)this).GetHashCode();
                }

                public override string ToString()
                {
                    return string.Format(System.Globalization.CultureInfo.CurrentCulture, "{{Left={0},Top={1},Right={2},Bottom={3}}}", Left, Top, Right, Bottom);
                }
            }
            //[StructLayout(LayoutKind.Sequential)]
            //public struct RECT
            //{
            //    public int left;
            //    public int top;
            //    public int right;
            //    public int bottom;

            //    public Rectangle ToRectangle()
            //    {
            //        return new Rectangle(this.left, this.top, this.right - this.left, this.bottom - this.top);
            //    }
            //}

            [Flags()]
            public enum RedrawWindowFlags : uint
            {
                /// <summary>
                /// Invalidates the rectangle or region that you specify in lprcUpdate or hrgnUpdate.
                /// You can set only one of these parameters to a non-NULL value. If both are NULL, RDW_INVALIDATE invalidates the entire window.
                /// </summary>
                Invalidate = 0x1,

                /// <summary>Causes the OS to post a WM_PAINT message to the window regardless of whether a portion of the window is invalid.</summary>
                InternalPaint = 0x2,

                /// <summary>
                /// Causes the window to receive a WM_ERASEBKGND message when the window is repainted.
                /// Specify this value in combination with the RDW_INVALIDATE value; otherwise, RDW_ERASE has no effect.
                /// </summary>
                Erase = 0x4,

                /// <summary>
                /// Validates the rectangle or region that you specify in lprcUpdate or hrgnUpdate.
                /// You can set only one of these parameters to a non-NULL value. If both are NULL, RDW_VALIDATE validates the entire window.
                /// This value does not affect internal WM_PAINT messages.
                /// </summary>
                Validate = 0x8,

                NoInternalPaint = 0x10,

                /// <summary>Suppresses any pending WM_ERASEBKGND messages.</summary>
                NoErase = 0x20,

                /// <summary>Excludes child windows, if any, from the repainting operation.</summary>
                NoChildren = 0x40,

                /// <summary>Includes child windows, if any, in the repainting operation.</summary>
                AllChildren = 0x80,

                /// <summary>Causes the affected windows, which you specify by setting the RDW_ALLCHILDREN and RDW_NOCHILDREN values, to receive WM_ERASEBKGND and WM_PAINT messages before the RedrawWindow returns, if necessary.</summary>
                UpdateNow = 0x100,

                /// <summary>
                /// Causes the affected windows, which you specify by setting the RDW_ALLCHILDREN and RDW_NOCHILDREN values, to receive WM_ERASEBKGND messages before RedrawWindow returns, if necessary.
                /// The affected windows receive WM_PAINT messages at the ordinary time.
                /// </summary>
                EraseNow = 0x200,

                Frame = 0x400,

                NoFrame = 0x800
            }
        }

        public static bool IsFullScreen(IntPtr hWnd)
        {
            bool runningFullScreen = false;
            var appBounds = new User32.RECT();

            if (!hWnd.Equals(IntPtr.Zero))
            {
                User32.GetWindowRect(hWnd, ref appBounds);

                var screenBounds = Screen.FromHandle(hWnd).Bounds;
                if ((appBounds.Bottom - appBounds.Top) == screenBounds.Height && (appBounds.Right - appBounds.Left) == screenBounds.Width)
                {
                    runningFullScreen = true;
                }
            }
            return runningFullScreen;
        }
    }

    public static class VisibilityTester
    {
        private delegate bool CallBackPtr(int hwnd, int lParam);
        private static CallBackPtr callBackPtr;

        /// <summary>
        /// The enumerated pointers of actually visible windows
        /// </summary>
        public static List<IntPtr> enumedwindowPtrs = new List<IntPtr>();
        /// <summary>
        /// The enumerated rectangles of actually visible windows
        /// </summary>
        public static List<Rectangle> enumedwindowRects = new List<Rectangle>();

        /// <summary>
        /// Does a hit test for specified control (is point of control visible to user)
        /// </summary>
        /// <param name="ctrlRect">the rectangle (usually Bounds) of the control</param>
        /// <param name="ctrlHandle">the handle for the control</param>
        /// <param name="points">the point(s) to test (usually MousePosition)</param>
        /// <param name="ExcludeWindow">a control or window to exclude from hit test (means point is visible through this window)</param>
        /// <returns>boolean value indicating if p is visible for ctrlRect</returns>
        public static bool HitTest(Rectangle ctrlRect, IntPtr ctrlHandle, Point[] points, IntPtr ExcludeWindow)
        {
            // clear results
            enumedwindowPtrs.Clear();
            enumedwindowRects.Clear();

            // Create callback and start enumeration
            callBackPtr = new CallBackPtr(EnumCallBack);
            EnumDesktopWindows(IntPtr.Zero, callBackPtr, 0);

            // Go from last to first window, and substract them from the ctrlRect area
            Region r = new Region(ctrlRect);

            bool StartClipping = false;
            for (int i = enumedwindowRects.Count - 1; i >= 0; i--)
            {
                if (StartClipping && enumedwindowPtrs[i] != ExcludeWindow)
                {
                    r.Exclude(enumedwindowRects[i]);
                }

                if (enumedwindowPtrs[i] == ctrlHandle) StartClipping = true;
            }

            // return boolean indicating if point is visible to clipped (truly visible) window
            foreach (var point in points)
            {
                var visible = r.IsVisible(point);
                if (!visible) return false;
            }
            return true;
        }

        /// <summary>
        /// Window enumeration callback
        /// </summary>
        private static bool EnumCallBack(int hwnd, int lParam)
        {
            // If window is visible and not minimized (isiconic)
            if (IsWindow((IntPtr)hwnd) && IsWindowVisible((IntPtr)hwnd) && !IsIconic((IntPtr)hwnd))
            {
                // add the handle and windowrect to "found windows" collection
                enumedwindowPtrs.Add((IntPtr)hwnd);

                RECT rct;

                if (GetWindowRect((IntPtr)hwnd, out rct))
                {
                    // add rect to list
                    enumedwindowRects.Add(new Rectangle(rct.Left, rct.Top, rct.Right - rct.Left, rct.Bottom - rct.Top));
                }
                else
                {
                    // invalid, make empty rectangle
                    enumedwindowRects.Add(new Rectangle(0, 0, 0, 0));
                }
            }

            return true;
        }


        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int EnumDesktopWindows(IntPtr hDesktop, CallBackPtr callPtr, int lPar);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner

            public override string ToString()
            {
                return Left + "," + Top + "," + Right + "," + Bottom;
            }
        }
    }
}
