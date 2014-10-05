// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HearthstoneHelper.cs" company="">
//   
// </copyright>
// <summary>
//   The hearthstone helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.Util
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Text;
    using System.Threading;

    using NLog;

    /// <summary>
    /// The hearthstone helper.
    /// </summary>
    public static class HearthstoneHelper
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The hearthston e_ processname.
        /// </summary>
        private const string HEARTHSTONE_PROCESSNAME = "hearthstone";

        /// <summary>
        /// The hearthston e_ classname.
        /// </summary>
        private const string HEARTHSTONE_CLASSNAME = "UnityWndClass";

        /// <summary>
        /// The cached.
        /// </summary>
        private static IntPtr cached = IntPtr.Zero;

        /// <summary>
        /// The notfound logged.
        /// </summary>
        private static bool notfoundLogged;

        /// <summary>
        /// The get hearthstone window.
        /// </summary>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        public static IntPtr GetHearthstoneWindow()
        {
            if (cached != IntPtr.Zero && NativeMethods.IsWindow(cached))
            {
                return cached;
            }

            try
            {
                cached = NativeMethods.FindWindow("UnityWndClass", "Hearthstone");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            if (cached != IntPtr.Zero)
            {
                Log.Debug("Found hearthstone window handle (using FindWindow).");
                notfoundLogged = false;
                return cached;
            }

            var processes = Process.GetProcessesByName(HEARTHSTONE_PROCESSNAME);
            if (processes.Length > 0)
            {
                if (processes.Length > 1)
                {
                    Log.Debug("Multiple Hearthstone processed were found.");
                }

                foreach (var process in processes)
                {
                    try
                    {
                        var sb = new StringBuilder(100);
                        var hwnd = process.MainWindowHandle;
                        NativeMethods.GetClassName(hwnd, sb, sb.Capacity);
                        var classname = sb.ToString();
                        if (string.Equals(classname, HEARTHSTONE_CLASSNAME, StringComparison.InvariantCultureIgnoreCase))
                        {
                            Log.Debug("Found hearthstone window handle (using Process).");
                            notfoundLogged = false;
                            cached = process.MainWindowHandle;
                        }
                        else
                        {
                            Log.Debug("Ignoring Hearthstone window handle: " + classname);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                    }
                }
            }

            if (cached == IntPtr.Zero)
            {
                if (!notfoundLogged)
                {
                    notfoundLogged = true;
                    Log.Warn("Hearthstone process not found.");
                }
            }

            return cached;
        }

        /// <summary>
        /// The set window to foreground.
        /// </summary>
        public static void SetWindowToForeground()
        {
            var handle = GetHearthstoneWindow();
            if (handle != IntPtr.Zero)
            {
                int i = 0;

                while (!NativeMethods.IsWindowInForeground(handle))
                {
                    if (i == 0)
                    {
                        // Initial sleep if target window is not in foreground - just to let things settle
                        Thread.Sleep(50);
                    }

                    if (NativeMethods.IsIconic(handle))
                    {
                        // Minimized so send restore
                        NativeMethods.ShowWindow(handle, NativeMethods.WindowShowStyle.Restore);
                    }
                    else
                    {
                        // Already Maximized or Restored so just bring to front
                        NativeMethods.SetForegroundWindow(handle);
                    }

                    Thread.Sleep(250);

                    // Check if the target process main window is now in the foreground
                    if (NativeMethods.IsWindowInForeground(handle))
                    {
                        return;
                    }

                    // Prevent an infinite loop
                    if (i > 10)
                    {
                        return;
                    }

                    i++;
                }
            }
        }
    }

    /// <summary>
    /// The native methods.
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    internal static class NativeMethods
    {
        /// <summary>
        /// The is window in foreground.
        /// </summary>
        /// <param name="hWnd">
        /// The h wnd.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal static bool IsWindowInForeground(IntPtr hWnd)
        {
            return hWnd == GetForegroundWindow();
        }

        #region kernel32

        /// <summary>
        /// The get module handle.
        /// </summary>
        /// <param name="lpModuleName">
        /// The lp module name.
        /// </param>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        #endregion

        #region user32

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
        internal static extern bool IsWindow(IntPtr hWnd);

        /// <summary>
        /// The find window.
        /// </summary>
        /// <param name="lpClassName">
        /// The lp class name.
        /// </param>
        /// <param name="lpWindowName">
        /// The lp window name.
        /// </param>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        #region ShowWindow

        /// <summary>
        /// Shows a Window
        /// </summary>
        /// <remarks>
        /// <para>
        /// To perform certain special effects when showing or hiding a
        /// window, use AnimateWindow.
        /// </para>
        /// <para>
        /// The first time an application calls ShowWindow, it should use
        /// the WinMain function's nCmdShow parameter as its nCmdShow parameter.
        /// Subsequent calls to ShowWindow must use one of the values in the
        /// given list, instead of the one specified by the WinMain function's
        /// nCmdShow parameter.
        /// </para>
        /// <para>
        /// As noted in the discussion of the nCmdShow parameter, the
        /// nCmdShow value is ignored in the first call to ShowWindow if the
        /// program that launched the application specifies startup information
        /// in the structure. In this case, ShowWindow uses the information
        /// specified in the STARTUPINFO structure to show the window. On
        /// subsequent calls, the application must call ShowWindow with nCmdShow
        /// set to SW_SHOWDEFAULT to use the startup information provided by the
        /// program that launched the application. This behavior is designed for
        /// the following situations: 
        /// </para>
        /// <list type="">
        /// <item>
        /// Applications create their main window by calling CreateWindow
        ///    with the WS_VISIBLE flag set. 
        /// </item>
        /// <item>
        /// Applications create their main window by calling CreateWindow
        ///    with the WS_VISIBLE flag cleared, and later call ShowWindow with the
        ///    SW_SHOW flag set to make it visible.
        /// </item>
        /// </list>
        /// </remarks>
        /// <param name="hWnd">
        /// Handle to the window.
        /// </param>
        /// <param name="nCmdShow">
        /// Specifies how the window is to be shown.
        /// This parameter is ignored the first time an application calls
        /// ShowWindow, if the program that launched the application provides a
        /// STARTUPINFO structure. Otherwise, the first time ShowWindow is called,
        /// the value should be the value obtained by the WinMain function in its
        /// nCmdShow parameter. In subsequent calls, this parameter can be one of
        /// the WindowShowStyle members.
        /// </param>
        /// <returns>
        /// If the window was previously visible, the return value is nonzero.
        /// If the window was previously hidden, the return value is zero.
        /// </returns>
        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, WindowShowStyle nCmdShow);

        /// <summary>
        /// The get class name.
        /// </summary>
        /// <param name="hWnd">
        /// The h wnd.
        /// </param>
        /// <param name="lpClassName">
        /// The lp class name.
        /// </param>
        /// <param name="nMaxCount">
        /// The n max count.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        /// <summary>Enumeration of the different ways of showing a window using
        /// ShowWindow</summary>
        internal enum WindowShowStyle : uint
        {
            /// <summary>Hides the window and activates another window.</summary>
            /// <remarks>See SW_HIDE</remarks>
            Hide = 0, 

            /// <summary>Activates and displays a window. If the window is minimized
            /// or maximized, the system restores it to its original size and
            /// position. An application should specify this flag when displaying
            /// the window for the first time.</summary>
            /// <remarks>See SW_SHOWNORMAL</remarks>
            ShowNormal = 1, 

            /// <summary>Activates the window and displays it as a minimized window.</summary>
            /// <remarks>See SW_SHOWMINIMIZED</remarks>
            ShowMinimized = 2, 

            /// <summary>Activates the window and displays it as a maximized window.</summary>
            /// <remarks>See SW_SHOWMAXIMIZED</remarks>
            ShowMaximized = 3, 

            /// <summary>Maximizes the specified window.</summary>
            /// <remarks>See SW_MAXIMIZE</remarks>
            Maximize = 3, 

            /// <summary>Displays a window in its most recent size and position.
            /// This value is similar to "ShowNormal", except the window is not
            /// actived.</summary>
            /// <remarks>See SW_SHOWNOACTIVATE</remarks>
            ShowNormalNoActivate = 4, 

            /// <summary>Activates the window and displays it in its current size
            /// and position.</summary>
            /// <remarks>See SW_SHOW</remarks>
            Show = 5, 

            /// <summary>Minimizes the specified window and activates the next
            /// top-level window in the Z order.</summary>
            /// <remarks>See SW_MINIMIZE</remarks>
            Minimize = 6, 

            /// <summary>Displays the window as a minimized window. This value is
            /// similar to "ShowMinimized", except the window is not activated.</summary>
            /// <remarks>See SW_SHOWMINNOACTIVE</remarks>
            ShowMinNoActivate = 7, 

            /// <summary>Displays the window in its current size and position. This
            /// value is similar to "Show", except the window is not activated.</summary>
            /// <remarks>See SW_SHOWNA</remarks>
            ShowNoActivate = 8, 

            /// <summary>Activates and displays the window. If the window is
            /// minimized or maximized, the system restores it to its original size
            /// and position. An application should specify this flag when restoring
            /// a minimized window.</summary>
            /// <remarks>See SW_RESTORE</remarks>
            Restore = 9, 

            /// <summary>Sets the show state based on the SW_ value specified in the
            /// STARTUPINFO structure passed to the CreateProcess function by the
            /// program that started the application.</summary>
            /// <remarks>See SW_SHOWDEFAULT</remarks>
            ShowDefault = 10, 

            /// <summary>Windows 2000/XP: Minimizes a window, even if the thread
            /// that owns the window is hung. This flag should only be used when
            /// minimizing windows from a different thread.</summary>
            /// <remarks>See SW_FORCEMINIMIZE</remarks>
            ForceMinimized = 11
        }

        #endregion

        /// <summary>
        /// The GetForegroundWindow function returns a handle to the foreground window.
        /// </summary>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// The set foreground window.
        /// </summary>
        /// <param name="hWnd">
        /// The h wnd.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);

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
        internal static extern bool IsIconic(IntPtr hWnd);

        #endregion
    }
}