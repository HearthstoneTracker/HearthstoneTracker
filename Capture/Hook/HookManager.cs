// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HookManager.cs" company="">
//   
// </copyright>
// <summary>
//   The hook manager.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Capture.Hook
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    using EasyHook;

    /// <summary>
    /// The hook manager.
    /// </summary>
    public class HookManager
    {
        #region Static Fields

        /// <summary>
        /// The hooked processes.
        /// </summary>
        internal static List<int> HookedProcesses = new List<int>();

        /*
         * Please note that we have obtained this information with system privileges.
         * So if you get client requests with a process ID don't try to open the process
         * as this will fail in some cases. Just search the ID in the following list and
         * extract information that is already there...
         * 
         * Of course you can change the way this list is implemented and the information
         * it contains but you should keep the code semantic.
         */

        /// <summary>
        /// The process list.
        /// </summary>
        internal static List<ProcessInfo> ProcessList = new List<ProcessInfo>();

        /// <summary>
        /// The active pid list.
        /// </summary>
        private static List<int> ActivePIDList = new List<int>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The add hooked process.
        /// </summary>
        /// <param name="processId">
        /// The process id.
        /// </param>
        public static void AddHookedProcess(int processId)
        {
            lock (HookedProcesses)
            {
                HookedProcesses.Add(processId);
            }
        }

        /// <summary>
        /// The enum processes.
        /// </summary>
        /// <returns>
        /// The <see cref="ProcessInfo[]"/>.
        /// </returns>
        public static ProcessInfo[] EnumProcesses()
        {
            List<ProcessInfo> result = new List<ProcessInfo>();
            Process[] procList = Process.GetProcesses();

            for (int i = 0; i < procList.Length; i++)
            {
                Process proc = procList[i];

                try
                {
                    ProcessInfo info = new ProcessInfo();

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

        /// <summary>
        /// The is hooked.
        /// </summary>
        /// <param name="processId">
        /// The process id.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsHooked(int processId)
        {
            lock (HookedProcesses)
            {
                return HookedProcesses.Contains(processId);
            }
        }

        /// <summary>
        /// The remove hooked process.
        /// </summary>
        /// <param name="processId">
        /// The process id.
        /// </param>
        public static void RemoveHookedProcess(int processId)
        {
            lock (HookedProcesses)
            {
                HookedProcesses.Remove(processId);
            }
        }

        #endregion

        /// <summary>
        /// The process info.
        /// </summary>
        [Serializable]
        public class ProcessInfo
        {
            #region Fields

            /// <summary>
            /// The file name.
            /// </summary>
            public string FileName;

            /// <summary>
            /// The id.
            /// </summary>
            public int Id;

            /// <summary>
            /// The is 64 bit.
            /// </summary>
            public bool Is64Bit;

            /// <summary>
            /// The user.
            /// </summary>
            public string User;

            #endregion
        }
    }
}