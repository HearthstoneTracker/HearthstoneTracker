namespace Capture.Hook
{
    using System;
    using System.Runtime.InteropServices;

    using EasyHook;

    public class HookData<T> : HookData
        where T : class
    {
        #region Fields

        private readonly T original;

        #endregion

        #region Constructors and Destructors

        public HookData(IntPtr func, Delegate inNewProc, object owner)
            : base(func, inNewProc, owner)
        {
            this.original = (T)(object)Marshal.GetDelegateForFunctionPointer(func, typeof(T));
        }

        #endregion

        #region Public Properties

        public T Original
        {
            get
            {
                return this.original;
            }
        }

        #endregion
    }

    public class HookData
    {
        #region Fields

        private readonly IntPtr func;

        private readonly Delegate inNewProc;

        private readonly object owner;

        private bool isHooked;

        private LocalHook localHook;

        #endregion

        #region Constructors and Destructors

        public HookData(IntPtr func, Delegate inNewProc, object owner)
        {
            this.func = func;
            this.inNewProc = inNewProc;
            this.owner = owner;
            this.CreateHook();
        }

        #endregion

        #region Public Properties

        public LocalHook Hook
        {
            get
            {
                return this.localHook;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void CreateHook()
        {
            if (this.localHook != null)
            {
                return;
            }
            this.localHook = LocalHook.Create(this.func, this.inNewProc, this.owner);
        }

        public void ReHook()
        {
            if (this.isHooked)
            {
                return;
            }
            this.isHooked = true;
            this.Hook.ThreadACL.SetExclusiveACL(new Int32[] { 0 });
        }

        public void UnHook()
        {
            //if (!this.isHooked) return;
            //this.isHooked = false;
            //this.Hook.ThreadACL.SetInclusiveACL(new Int32[] { 0 });

            //if (localHook == null) return;
            //this.isHooked = false;
            //this.Hook.ThreadACL.SetInclusiveACL(new Int32[] { 0 });
            //localHook.Dispose();
            //localHook = null;
        }

        #endregion
    }
}