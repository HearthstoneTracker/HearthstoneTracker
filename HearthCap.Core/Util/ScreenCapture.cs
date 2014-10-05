// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScreenCapture.cs" company="">
//   
// </copyright>
// <summary>
//   Provides functions to capture the entire screen, or a particular window, and save it to a file.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.Util
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    using NLog;

    using Point = System.Drawing.Point;

    /// <summary>
    /// Provides functions to capture the entire screen, or a particular window, and save it to a file.
    /// </summary>
    public class ScreenCapture
    {
        /// <summary>
        /// The log.
        /// </summary>
        private readonly Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The get window size.
        /// </summary>
        /// <param name="hWnd">
        /// The h wnd.
        /// </param>
        /// <returns>
        /// The <see cref="Rectangle"/>.
        /// </returns>
        public Rectangle GetWindowSize(IntPtr hWnd)
        {
            User32.RECT rect = new User32.RECT();
            User32.GetWindowRect(hWnd, ref rect);
            return rect;
        }

        /// <summary>
        /// The get client size.
        /// </summary>
        /// <param name="hWnd">
        /// The h wnd.
        /// </param>
        /// <returns>
        /// The <see cref="Rectangle"/>.
        /// </returns>
        public Rectangle GetClientSize(IntPtr hWnd)
        {
            User32.RECT rect = new User32.RECT();
            User32.GetClientRect(hWnd, ref rect);
            return rect;
        }

        /// <summary>
        /// Gets a segment of the desktop as an image.
        /// </summary>
        /// <param name="hWnd">
        /// The h Wnd.
        /// </param>
        /// <param name="forcePrintWindow">
        /// The force Print Window.
        /// </param>
        /// <param name="forceBitBlt">
        /// The force Bit Blt.
        /// </param>
        /// <returns>
        /// A <see cref="System.Drawing.Image"/> containg an image of the full desktop.
        /// </returns>
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

        /// <summary>
        /// The get desktop bitmap.
        /// </summary>
        /// <param name="hWnd">
        /// The h wnd.
        /// </param>
        /// <param name="rect">
        /// The rect.
        /// </param>
        /// <returns>
        /// The <see cref="Image"/>.
        /// </returns>
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
        /// <param name="hWnd">
        /// The h Wnd.
        /// </param>
        /// <returns>
        /// A <see cref="System.Drawing.Image"/> containg an image of the full desktop.
        /// </returns>
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
        /// <param name="rectangle">
        /// The rectangular area to capture.
        /// </param>
        /// <returns>
        /// A <see cref="System.Drawing.Image"/> containg an image of the desktop 
        /// at the specified coordinates
        /// </returns>
        public Image GetDesktopBitmap(Rectangle rectangle)
        {
            return this.GetDesktopBitmap(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        /// <summary>
        /// Retrieves an image of the specified part of your screen.
        /// </summary>
        /// <param name="x">
        /// The X coordinate of the requested area
        /// </param>
        /// <param name="y">
        /// The Y coordinate of the requested area
        /// </param>
        /// <param name="width">
        /// The width of the requested area
        /// </param>
        /// <param name="height">
        /// The height of the requested area
        /// </param>
        /// <returns>
        /// A <see cref="System.Drawing.Image"/> of the desktop at 
        /// the specified coordinates.
        /// </returns>
        public Image GetDesktopBitmap(int x, int y, int width, int height)
        {
            // Create the image and graphics to capture the portion of the desktop.
            Image destinationImage = new Bitmap(width, height, PixelFormat.Format32bppRgb);
            using (Graphics destinationGraphics = Graphics.FromImage(destinationImage))
            {

                IntPtr destinationGraphicsHandle = IntPtr.Zero;
                IntPtr windowDC = IntPtr.Zero;

                try
                {
                    // Pointers for window handles
                    destinationGraphicsHandle = destinationGraphics.GetHdc();
                    windowDC = User32.GetDC(IntPtr.Zero);

                    // Get the screencapture
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
        /// <param name="hWnd">
        /// The h Wnd.
        /// </param>
        /// <returns>
        /// A <see cref="System.Drawing.Image"/> containg an image of the full desktop.
        /// </returns>
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
                    // Pointers for window handles
                    destinationGraphicsHandle = destinationGraphics.GetHdc();
                    windowDC = User32.GetWindowDC(hWnd);

                    // Get the screencapture
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

        /// <summary>
        /// The dwm.
        /// </summary>
        private class DWM
        {
            /// <summary>
            /// The dwm is composition enabled.
            /// </summary>
            /// <param name="enabled">
            /// The enabled.
            /// </param>
            /// <returns>
            /// The <see cref="int"/>.
            /// </returns>
            [DllImport("dwmapi.dll")]
            public static extern int DwmIsCompositionEnabled(out bool enabled);
        }

        /// <summary>
        /// Helper class containing Gdi32 API functions
        /// </summary>
        private class GDI32
        {
            /// <summary>
            /// The bit blt.
            /// </summary>
            /// <param name="hObject">
            /// The h object.
            /// </param>
            /// <param name="nXDest">
            /// The n x dest.
            /// </param>
            /// <param name="nYDest">
            /// The n y dest.
            /// </param>
            /// <param name="nWidth">
            /// The n width.
            /// </param>
            /// <param name="nHeight">
            /// The n height.
            /// </param>
            /// <param name="hObjectSource">
            /// The h object source.
            /// </param>
            /// <param name="nXSrc">
            /// The n x src.
            /// </param>
            /// <param name="nYSrc">
            /// The n y src.
            /// </param>
            /// <param name="dwRop">
            /// The dw rop.
            /// </param>
            /// <returns>
            /// The <see cref="bool"/>.
            /// </returns>
            [DllImport("gdi32.dll")]
            public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hObjectSource, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

            /// <summary>
            /// Deletes the specified device context (DC).
            /// </summary>
            /// <param name="hdc">
            /// A handle to the device context.
            /// </param>
            /// <returns>
            /// If the function succeeds, the return value is <c>true</c>. If the function fails, the return value is <c>false</c>.
            /// </returns>
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
            /// <summary>
            /// The is window visible.
            /// </summary>
            /// <param name="hWnd">
            /// The h wnd.
            /// </param>
            /// <returns>
            /// The <see cref="bool"/>.
            /// </returns>
            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern bool IsWindowVisible(IntPtr hWnd);

            /// <summary>
            /// The print window.
            /// </summary>
            /// <param name="hwnd">
            /// The hwnd.
            /// </param>
            /// <param name="hDC">
            /// The h dc.
            /// </param>
            /// <param name="nFlags">
            /// The n flags.
            /// </param>
            /// <returns>
            /// The <see cref="bool"/>.
            /// </returns>
            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);

            /// <summary>
            /// The client to screen.
            /// </summary>
            /// <param name="hWnd">
            /// The h wnd.
            /// </param>
            /// <param name="lpPoint">
            /// The lp point.
            /// </param>
            /// <returns>
            /// The <see cref="bool"/>.
            /// </returns>
            [DllImport("user32.dll")]
            public static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

            /// <summary>
            /// The get desktop window.
            /// </summary>
            /// <returns>
            /// The <see cref="IntPtr"/>.
            /// </returns>
            [DllImport("user32.dll")]
            public static extern IntPtr GetDesktopWindow();

            /// <summary>
            /// The get dc.
            /// </summary>
            /// <param name="hWnd">
            /// The h wnd.
            /// </param>
            /// <returns>
            /// The <see cref="IntPtr"/>.
            /// </returns>
            [DllImport("user32.dll")]
            public static extern IntPtr GetDC(IntPtr hWnd);

            /// <summary>
            /// The get window dc.
            /// </summary>
            /// <param name="hWnd">
            /// The h wnd.
            /// </param>
            /// <returns>
            /// The <see cref="IntPtr"/>.
            /// </returns>
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowDC(IntPtr hWnd);

            /// <summary>
            /// The release dc.
            /// </summary>
            /// <param name="hWnd">
            /// The h wnd.
            /// </param>
            /// <param name="hDC">
            /// The h dc.
            /// </param>
            /// <returns>
            /// The <see cref="IntPtr"/>.
            /// </returns>
            [DllImport("user32.dll")]
            public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);

            /// <summary>
            /// The get window rect.
            /// </summary>
            /// <param name="hWnd">
            /// The h wnd.
            /// </param>
            /// <param name="rect">
            /// The rect.
            /// </param>
            /// <returns>
            /// The <see cref="IntPtr"/>.
            /// </returns>
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);

            /// <summary>
            /// The get client rect.
            /// </summary>
            /// <param name="hWnd">
            /// The h wnd.
            /// </param>
            /// <param name="rect">
            /// The rect.
            /// </param>
            /// <returns>
            /// The <see cref="IntPtr"/>.
            /// </returns>
            [DllImport("user32.dll")]
            public static extern IntPtr GetClientRect(IntPtr hWnd, ref RECT rect);

            /// <summary>
            /// The redraw window.
            /// </summary>
            /// <param name="hWnd">
            /// The h wnd.
            /// </param>
            /// <param name="lprcUpdate">
            /// The lprc update.
            /// </param>
            /// <param name="hrgnUpdate">
            /// The hrgn update.
            /// </param>
            /// <param name="flags">
            /// The flags.
            /// </param>
            /// <returns>
            /// The <see cref="bool"/>.
            /// </returns>
            [DllImport("user32.dll")]
            public static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, RedrawWindowFlags flags);

            /// <summary>
            /// The point.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct POINT
            {
                /// <summary>
                /// The x.
                /// </summary>
                public int X;

                /// <summary>
                /// The y.
                /// </summary>
                public int Y;

                /// <summary>
                /// Initializes a new instance of the <see cref="POINT"/> struct.
                /// </summary>
                /// <param name="x">
                /// The x.
                /// </param>
                /// <param name="y">
                /// The y.
                /// </param>
                public POINT(int x, int y)
                {
                    this.X = x;
                    this.Y = y;
                }

                /// <summary>
                /// Initializes a new instance of the <see cref="POINT"/> struct.
                /// </summary>
                /// <param name="pt">
                /// The pt.
                /// </param>
                public POINT(Point pt)
                    : this(pt.X, pt.Y)
                {
                }

                /// <summary>
                /// The op_ implicit.
                /// </summary>
                /// <param name="p">
                /// The p.
                /// </param>
                /// <returns>
                /// </returns>
                public static implicit operator Point(POINT p)
                {
                    return new Point(p.X, p.Y);
                }

                /// <summary>
                /// The op_ implicit.
                /// </summary>
                /// <param name="p">
                /// The p.
                /// </param>
                /// <returns>
                /// </returns>
                public static implicit operator POINT(Point p)
                {
                    return new POINT(p.X, p.Y);
                }
            }

            /// <summary>
            /// The rect.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                /// <summary>
                /// The left.
                /// </summary>
                public int Left;

                /// <summary>
                /// The top.
                /// </summary>
                public int Top;

                /// <summary>
                /// The right.
                /// </summary>
                public int Right;

                /// <summary>
                /// The bottom.
                /// </summary>
                public int Bottom;

                /// <summary>
                /// Initializes a new instance of the <see cref="RECT"/> struct.
                /// </summary>
                /// <param name="left">
                /// The left.
                /// </param>
                /// <param name="top">
                /// The top.
                /// </param>
                /// <param name="right">
                /// The right.
                /// </param>
                /// <param name="bottom">
                /// The bottom.
                /// </param>
                public RECT(int left, int top, int right, int bottom)
                {
                    this.Left = left;
                    this.Top = top;
                    this.Right = right;
                    this.Bottom = bottom;
                }

                /// <summary>
                /// Initializes a new instance of the <see cref="RECT"/> struct.
                /// </summary>
                /// <param name="r">
                /// The r.
                /// </param>
                public RECT(Rectangle r)
                    : this(r.Left, r.Top, r.Right, r.Bottom)
                {
                }

                /// <summary>
                /// Gets or sets the x.
                /// </summary>
                public int X
                {
                    get
                    {
                        return this.Left;
                    }

                    set
                    {
                        this.Right -= this.Left - value;
                        this.Left = value;
                    }
                }

                /// <summary>
                /// Gets or sets the y.
                /// </summary>
                public int Y
                {
                    get
                    {
                        return this.Top;
                    }

                    set
                    {
                        this.Bottom -= this.Top - value;
                        this.Top = value;
                    }
                }

                /// <summary>
                /// Gets or sets the height.
                /// </summary>
                public int Height
                {
                    get
                    {
                        return this.Bottom - this.Top;
                    }

                    set
                    {
                        this.Bottom = value + this.Top;
                    }
                }

                /// <summary>
                /// Gets or sets the width.
                /// </summary>
                public int Width
                {
                    get
                    {
                        return this.Right - this.Left;
                    }

                    set
                    {
                        this.Right = value + this.Left;
                    }
                }

                /// <summary>
                /// Gets or sets the location.
                /// </summary>
                public Point Location
                {
                    get
                    {
                        return new Point(this.Left, this.Top);
                    }

                    set
                    {
                        this.X = value.X;
                        this.Y = value.Y;
                    }
                }

                /// <summary>
                /// Gets or sets the size.
                /// </summary>
                public Size Size
                {
                    get
                    {
                        return new Size(this.Width, this.Height);
                    }

                    set
                    {
                        this.Width = value.Width;
                        this.Height = value.Height;
                    }
                }

                /// <summary>
                /// The op_ implicit.
                /// </summary>
                /// <param name="r">
                /// The r.
                /// </param>
                /// <returns>
                /// </returns>
                public static implicit operator Rectangle(RECT r)
                {
                    return new Rectangle(r.Left, r.Top, r.Width, r.Height);
                }

                /// <summary>
                /// The op_ implicit.
                /// </summary>
                /// <param name="r">
                /// The r.
                /// </param>
                /// <returns>
                /// </returns>
                public static implicit operator RECT(Rectangle r)
                {
                    return new RECT(r);
                }

                /// <summary>
                /// The ==.
                /// </summary>
                /// <param name="r1">
                /// The r 1.
                /// </param>
                /// <param name="r2">
                /// The r 2.
                /// </param>
                /// <returns>
                /// </returns>
                public static bool operator ==(RECT r1, RECT r2)
                {
                    return r1.Equals(r2);
                }

                /// <summary>
                /// The !=.
                /// </summary>
                /// <param name="r1">
                /// The r 1.
                /// </param>
                /// <param name="r2">
                /// The r 2.
                /// </param>
                /// <returns>
                /// </returns>
                public static bool operator !=(RECT r1, RECT r2)
                {
                    return !r1.Equals(r2);
                }

                /// <summary>
                /// The equals.
                /// </summary>
                /// <param name="r">
                /// The r.
                /// </param>
                /// <returns>
                /// The <see cref="bool"/>.
                /// </returns>
                public bool Equals(RECT r)
                {
                    return r.Left == this.Left && r.Top == this.Top && r.Right == this.Right && r.Bottom == this.Bottom;
                }

                /// <summary>
                /// The equals.
                /// </summary>
                /// <param name="obj">
                /// The obj.
                /// </param>
                /// <returns>
                /// The <see cref="bool"/>.
                /// </returns>
                public override bool Equals(object obj)
                {
                    if (obj is RECT)
                    {
                        return this.Equals((RECT)obj);
                    }
                    else if (obj is Rectangle)
                    {
                        return this.Equals(new RECT((Rectangle)obj));
                    }

                    return false;
                }

                /// <summary>
                /// The get hash code.
                /// </summary>
                /// <returns>
                /// The <see cref="int"/>.
                /// </returns>
                public override int GetHashCode()
                {
                    return ((Rectangle)this).GetHashCode();
                }

                /// <summary>
                /// The to string.
                /// </summary>
                /// <returns>
                /// The <see cref="string"/>.
                /// </returns>
                public override string ToString()
                {
                    return string.Format(
                        CultureInfo.CurrentCulture, 
                        "{{Left={0},Top={1},Right={2},Bottom={3}}}", 
                        this.Left, 
                        this.Top, 
                        this.Right, 
                        this.Bottom);
                }
            }

            // [StructLayout(LayoutKind.Sequential)]
            // public struct RECT
            // {
            // public int left;
            // public int top;
            // public int right;
            // public int bottom;

            // public Rectangle ToRectangle()
            // {
            // return new Rectangle(this.left, this.top, this.right - this.left, this.bottom - this.top);
            // }
            // }

            /// <summary>
            /// The redraw window flags.
            /// </summary>
            [Flags]
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

                /// <summary>
                /// The no internal paint.
                /// </summary>
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

                /// <summary>
                /// The frame.
                /// </summary>
                Frame = 0x400, 

                /// <summary>
                /// The no frame.
                /// </summary>
                NoFrame = 0x800
            }
        }

        /// <summary>
        /// The is full screen.
        /// </summary>
        /// <param name="hWnd">
        /// The h wnd.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
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

    /// <summary>
    /// The visibility tester.
    /// </summary>
    public static class VisibilityTester
    {
        /// <summary>
        /// The call back ptr.
        /// </summary>
        /// <param name="hwnd">
        /// The hwnd.
        /// </param>
        /// <param name="lParam">
        /// The l param.
        /// </param>
        private delegate bool CallBackPtr(int hwnd, int lParam);

        /// <summary>
        /// The call back ptr.
        /// </summary>
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
        /// <param name="ctrlRect">
        /// the rectangle (usually Bounds) of the control
        /// </param>
        /// <param name="ctrlHandle">
        /// the handle for the control
        /// </param>
        /// <param name="points">
        /// the point(s) to test (usually MousePosition)
        /// </param>
        /// <param name="ExcludeWindow">
        /// a control or window to exclude from hit test (means point is visible through this window)
        /// </param>
        /// <returns>
        /// boolean value indicating if p is visible for ctrlRect
        /// </returns>
        public static bool HitTest(Rectangle ctrlRect, IntPtr ctrlHandle, Point[] points, IntPtr ExcludeWindow)
        {
            // clear results
            enumedwindowPtrs.Clear();
            enumedwindowRects.Clear();

            // Create callback and start enumeration
            callBackPtr = EnumCallBack;
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
        /// <param name="hwnd">
        /// The hwnd.
        /// </param>
        /// <param name="lParam">
        /// The l Param.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
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

        /// <summary>
        /// The is window visible.
        /// </summary>
        /// <param name="hWnd">
        /// The h wnd.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindowVisible(IntPtr hWnd);

        /// <summary>
        /// The is window.
        /// </summary>
        /// <param name="hWnd">
        /// The h wnd.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindow(IntPtr hWnd);

        /// <summary>
        /// The is iconic.
        /// </summary>
        /// <param name="hWnd">
        /// The h wnd.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsIconic(IntPtr hWnd);

        /// <summary>
        /// The enum desktop windows.
        /// </summary>
        /// <param name="hDesktop">
        /// The h desktop.
        /// </param>
        /// <param name="callPtr">
        /// The call ptr.
        /// </param>
        /// <param name="lPar">
        /// The l par.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport("user32.dll")]
        private static extern int EnumDesktopWindows(IntPtr hDesktop, CallBackPtr callPtr, int lPar);

        /// <summary>
        /// The get window rect.
        /// </summary>
        /// <param name="hWnd">
        /// The h wnd.
        /// </param>
        /// <param name="lpRect">
        /// The lp rect.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        /// <summary>
        /// The rect.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            /// <summary>
            /// The left.
            /// </summary>
            public int Left;        // x position of upper-left corner

            /// <summary>
            /// The top.
            /// </summary>
            public int Top;         // y position of upper-left corner

            /// <summary>
            /// The right.
            /// </summary>
            public int Right;       // x position of lower-right corner

            /// <summary>
            /// The bottom.
            /// </summary>
            public int Bottom;      // y position of lower-right corner

            /// <summary>
            /// The to string.
            /// </summary>
            /// <returns>
            /// The <see cref="string"/>.
            /// </returns>
            public override string ToString()
            {
                return this.Left + "," + this.Top + "," + this.Right + "," + this.Bottom;
            }
        }
    }
}
