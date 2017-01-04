using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Parascan1.Data;


namespace Parascan1.Processing
{
    class Program
    {
        static void Main(string[] args)
        {
            #region "*** Check for Running Instance and Close ***"

            string STRING_PROCESSNAME = SharedValueController.PROCESS_NAME;
            //const int SW_HIDE = 0;
            int intProcessCount = 0;
            int intProcessCountMaximum = Properties.Settings.Default.KEY_CACHE_APPLICATION_THREADS;
            bool IsLimitedThreads = true;
            bool IsRealTime = false;
            string str;

            foreach (string argument in args)
            {
                if (argument.ToUpper() == SharedValueController.STRING_ARGUMENT_NOTHREADLIMIT)
                {
                    IsLimitedThreads = false;
                }
                else if (argument.ToUpper() == SharedValueController.STRING_ARGUMENT_REALTIME)
                {
                    IsRealTime = true;
                    intProcessCountMaximum++;
                }
            }

            foreach (Process objectProcess in Process.GetProcesses())
            {
                str = objectProcess.ProcessName;
                if ((objectProcess.ProcessName.Trim().ToUpper().Contains(STRING_PROCESSNAME) == true) && (IsLimitedThreads == true))
                {
                    intProcessCount++;
                    if (intProcessCount > intProcessCountMaximum) { return; }
                }
            }

            #endregion
            if ((Properties.Settings.Default.BOOL_ISPERFORMANCEREALTIME == true) || (IsRealTime == true))
            {
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
            }
            else
            {
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            }
            FilterController objectFilterController = new FilterController();
            objectFilterController.ProcessFilters();
        }


       
    }
}
