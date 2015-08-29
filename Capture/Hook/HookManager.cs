using System;
using System.Collections.Generic;
using System.Diagnostics;
using EasyHook;

namespace Capture.Hook
{
    public class HookManager
    {
        #region Static Fields

        internal static List<Int32> HookedProcesses = new List<Int32>();

        /*
         * Please note that we have obtained this information with system privileges.
         * So if you get client requests with a process ID don't try to open the process
         * as this will fail in some cases. Just search the ID in the following list and
         * extract information that is already there...
         * 
         * Of course you can change the way this list is implemented and the information
         * it contains but you should keep the code semantic.
         */

        internal static List<ProcessInfo> ProcessList = new List<ProcessInfo>();

        private static List<Int32> ActivePIDList = new List<Int32>();

        #endregion

        #region Public Methods and Operators

        public static void AddHookedProcess(Int32 processId)
        {
            lock (HookedProcesses)
            {
                HookedProcesses.Add(processId);
            }
        }

        public static ProcessInfo[] EnumProcesses()
        {
            var result = new List<ProcessInfo>();
            var procList = Process.GetProcesses();

            for (var i = 0; i < procList.Length; i++)
            {
                var proc = procList[i];

                try
                {
                    var info = new ProcessInfo();

                    info.FileName = proc.MainModule.FileName;
                    info.Id = proc.Id;
                    info.Is64Bit = RemoteHooking.IsX64Process(proc.Id);
                    info.User = RemoteHooking.GetProcessIdentity(proc.Id).Name;

                    result.Add(info);
                }
                catch
                {
                }
            }

            return result.ToArray();
        }

        public static bool IsHooked(Int32 processId)
        {
            lock (HookedProcesses)
            {
                return HookedProcesses.Contains(processId);
            }
        }

        public static void RemoveHookedProcess(Int32 processId)
        {
            lock (HookedProcesses)
            {
                HookedProcesses.Remove(processId);
            }
        }

        #endregion

        [Serializable]
        public class ProcessInfo
        {
            #region Fields

            public String FileName;

            public Int32 Id;

            public Boolean Is64Bit;

            public String User;

            #endregion
        }
    }
}
