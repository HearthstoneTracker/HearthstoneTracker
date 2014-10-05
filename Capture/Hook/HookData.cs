// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HookData.cs" company="">
//   
// </copyright>
// <summary>
//   The hook data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Capture.Hook
{
    using System;
    using System.Runtime.InteropServices;

    using EasyHook;

    /// <summary>
    /// The hook data.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class HookData<T> : HookData
        where T : class
    {
        #region Fields

        /// <summary>
        /// The original.
        /// </summary>
        private readonly T original;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HookData{T}"/> class.
        /// </summary>
        /// <param name="func">
        /// The func.
        /// </param>
        /// <param name="inNewProc">
        /// The in new proc.
        /// </param>
        /// <param name="owner">
        /// The owner.
        /// </param>
        public HookData(IntPtr func, Delegate inNewProc, object owner)
            : base(func, inNewProc, owner)
        {
            this.original = (T)(object)Marshal.GetDelegateForFunctionPointer(func, typeof(T));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the original.
        /// </summary>
        public T Original
        {
            get
            {
                return this.original;
            }
        }

        #endregion
    }

    /// <summary>
    /// The hook data.
    /// </summary>
    public class HookData
    {
        #region Fields

        /// <summary>
        /// The func.
        /// </summary>
        private readonly IntPtr func;

        /// <summary>
        /// The in new proc.
        /// </summary>
        private readonly Delegate inNewProc;

        /// <summary>
        /// The owner.
        /// </summary>
        private readonly object owner;

        /// <summary>
        /// The is hooked.
        /// </summary>
        private bool isHooked;

        /// <summary>
        /// The local hook.
        /// </summary>
        private LocalHook localHook;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HookData"/> class.
        /// </summary>
        /// <param name="func">
        /// The func.
        /// </param>
        /// <param name="inNewProc">
        /// The in new proc.
        /// </param>
        /// <param name="owner">
        /// The owner.
        /// </param>
        public HookData(IntPtr func, Delegate inNewProc, object owner)
        {
            this.func = func;
            this.inNewProc = inNewProc;
            this.owner = owner;
            this.CreateHook();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the hook.
        /// </summary>
        public LocalHook Hook
        {
            get
            {
                return this.localHook;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The create hook.
        /// </summary>
        public void CreateHook()
        {
            if (this.localHook != null)
            {
                return;
            }

            this.localHook = LocalHook.Create(this.func, this.inNewProc, this.owner);
        }

        /// <summary>
        /// The re hook.
        /// </summary>
        public void ReHook()
        {
            if (this.isHooked)
            {
                return;
            }

            this.isHooked = true;
            this.Hook.ThreadACL.SetExclusiveACL(new[] { 0 });
        }

        /// <summary>
        /// The un hook.
        /// </summary>
        public void UnHook()
        {
            // if (!this.isHooked) return;
            // this.isHooked = false;
            // this.Hook.ThreadACL.SetInclusiveACL(new Int32[] { 0 });

            // if (localHook == null) return;
            // this.isHooked = false;
            // this.Hook.ThreadACL.SetInclusiveACL(new Int32[] { 0 });
            // localHook.Dispose();
            // localHook = null;
        }

        #endregion
    }
}