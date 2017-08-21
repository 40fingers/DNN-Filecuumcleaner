using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FortyFingers.FilecuumCleaner.Library;

namespace FortyFingers.FilecuumCleaner.Components
{
    public class Common
    {
        public const string SchedulerItemFullTypeName = "FortyFingers.FilecuumCleaner.Components.FilecuumCleanerTask, 40Fingers.DNN.Modules.FilecuumCleaner";

        public const int RootPathHost = -1;
        public const int RootPathWebsite = -2;

        public static NLog.Logger GetLogger(NLog.LogLevel logLevel = null)
        {
            return Logger.GetLogger("FilecuumCleaner", "40Fingers\\FilecuumCleaner", "log", logLevel);
        }
    }
}