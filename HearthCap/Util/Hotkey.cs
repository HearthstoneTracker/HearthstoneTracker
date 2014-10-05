// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Hotkey.cs" company="">
//   
// </copyright>
// <summary>
//   The hot key.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Util
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Windows.Input;
    using System.Windows.Interop;

    /// <summary>
    /// The hot key.
    /// </summary>
    public class HotKey : IDisposable
    {
        /// <summary>
        /// The _dict hot key to cal back proc.
        /// </summary>
        private static Dictionary<int, HotKey> _dictHotKeyToCalBackProc;

        /// <summary>
        /// The register hot key.
        /// </summary>
        /// <param name="hWnd">
        /// The h wnd.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="fsModifiers">
        /// The fs modifiers.
        /// </param>
        /// <param name="vlc">
        /// The vlc.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vlc);

        /// <summary>
        /// The unregister hot key.
        /// </summary>
        /// <param name="hWnd">
        /// The h wnd.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        /// <summary>
        /// The wm hot key.
        /// </summary>
        public const int WmHotKey = 0x0312;

        /// <summary>
        /// The _disposed.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Gets the key.
        /// </summary>
        public Key Key { get; private set; }

        /// <summary>
        /// Gets the key modifiers.
        /// </summary>
        public KeyModifier KeyModifiers { get; private set; }

        /// <summary>
        /// Gets the action.
        /// </summary>
        public Action<HotKey> Action { get; private set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int Id { get; set; }

        // ******************************************************************
        /// <summary>
        /// Initializes a new instance of the <see cref="HotKey"/> class.
        /// </summary>
        /// <param name="k">
        /// The k.
        /// </param>
        /// <param name="keyModifiers">
        /// The key modifiers.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="register">
        /// The register.
        /// </param>
        public HotKey(Key k, KeyModifier keyModifiers, Action<HotKey> action, bool register = true)
        {
            this.Key = k;
            this.KeyModifiers = keyModifiers;
            this.Action = action;
            if (register)
            {
                this.Register();
            }
        }

        // ******************************************************************
        /// <summary>
        /// The register.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Register()
        {
            int virtualKeyCode = KeyInterop.VirtualKeyFromKey(this.Key);
            this.Id = virtualKeyCode + ((int)this.KeyModifiers * 0x10000);
            bool result = RegisterHotKey(IntPtr.Zero, this.Id, (UInt32)this.KeyModifiers, (UInt32)virtualKeyCode);

            if (_dictHotKeyToCalBackProc == null)
            {
                _dictHotKeyToCalBackProc = new Dictionary<int, HotKey>();
                ComponentDispatcher.ThreadFilterMessage += ComponentDispatcherThreadFilterMessage;
            }

            _dictHotKeyToCalBackProc.Add(this.Id, this);

            Debug.Print(result.ToString() + ", " + this.Id + ", " + virtualKeyCode);
            return result;
        }

        // ******************************************************************
        /// <summary>
        /// The unregister.
        /// </summary>
        public void Unregister()
        {
            HotKey hotKey;
            if (_dictHotKeyToCalBackProc.TryGetValue(this.Id, out hotKey))
            {
                UnregisterHotKey(IntPtr.Zero, this.Id);
            }
        }

        // ******************************************************************
        /// <summary>
        /// The component dispatcher thread filter message.
        /// </summary>
        /// <param name="msg">
        /// The msg.
        /// </param>
        /// <param name="handled">
        /// The handled.
        /// </param>
        private static void ComponentDispatcherThreadFilterMessage(ref MSG msg, ref bool handled)
        {
            if (!handled)
            {
                if (msg.message == WmHotKey)
                {
                    HotKey hotKey;

                    if (_dictHotKeyToCalBackProc.TryGetValue((int)msg.wParam, out hotKey))
                    {
                        if (hotKey.Action != null)
                        {
                            hotKey.Action.Invoke(hotKey);
                        }

                        handled = true;
                    }
                }
            }
        }

        // ******************************************************************
        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // ******************************************************************
        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be _disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be _disposed.
        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    this.Unregister();
                }

                // Note disposing has been done.
                this._disposed = true;
            }
        }
    }

    // ******************************************************************
    /// <summary>
    /// The key modifier.
    /// </summary>
    [Flags]
    public enum KeyModifier
    {
        /// <summary>
        /// The none.
        /// </summary>
        None = 0x0000, 

        /// <summary>
        /// The alt.
        /// </summary>
        Alt = 0x0001, 

        /// <summary>
        /// The ctrl.
        /// </summary>
        Ctrl = 0x0002, 

        /// <summary>
        /// The no repeat.
        /// </summary>
        NoRepeat = 0x4000, 

        /// <summary>
        /// The shift.
        /// </summary>
        Shift = 0x0004, 

        /// <summary>
        /// The win.
        /// </summary>
        Win = 0x0008
    }

    // ******************************************************************
}