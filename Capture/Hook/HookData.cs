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
            original = (T)(object)Marshal.GetDelegateForFunctionPointer(func, typeof(T));
        }

        #endregion

        #region Public Properties

        public T Original
        {
            get
            {
                return original;
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
            CreateHook();
        }

        #endregion

        #region Public Properties

        public LocalHook Hook
        {
            get
            {
                return localHook;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void CreateHook()
        {
            if (localHook != null)
            {
                return;
            }
            localHook = LocalHook.Create(func, inNewProc, owner);
        }

        public void ReHook()
        {
            if (isHooked)
            {
                return;
            }
            isHooked = true;
            Hook.ThreadACL.SetExclusiveACL(new Int32[] { 0 });
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