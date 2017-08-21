using System;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace FortyFingers.FilecuumCleaner.Library
{
    public class Logger
    {
        private static volatile Logger instance;
        private static object syncRoot = new Object();

        public NLog.Logger Nlogger { get; set; }

        private Logger(string loggerName, string folderName, string filenameExt, LogLevel logLevel = null)
        {
            // Step 1. Create configuration object 
            LoggingConfiguration config = new LoggingConfiguration();

            // Step 2. Create targets and add them to the configuration 
            FileTarget fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);

            // Step 3. Set target properties 
            fileTarget.FileName = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory,
                                               String.Format("Portals\\_default\\Logs\\{0}\\{1}-{2}.{3}", folderName,
                                                             DateTime.Now.ToString("yyyy-MM-dd"), loggerName, filenameExt));
            fileTarget.Layout = "${time}: ${message}";

            // Step 4. Define rules
            LoggingRule rule = new LoggingRule(loggerName, logLevel ?? LogLevel.Debug, fileTarget);
            config.LoggingRules.Add(rule);

            // Step 5. Activate the configuration
            LogManager.Configuration = config;

            Nlogger = LogManager.GetLogger(loggerName);
        }

        public static NLog.Logger GetLogger(string loggerName, string folderName, string filenameExt, LogLevel logLevel = null)
        {
            return new Logger(loggerName, folderName, filenameExt, logLevel).Nlogger;
        }

        //public static Logger Instance(string loggerName, string folderName, string filenameExt)
        //{
        //    if (instance == null)
        //    {
        //        lock (syncRoot)
        //        {
        //            if (instance == null)
        //                instance = new Logger(loggerName, folderName, filenameExt);
        //        }
        //    }

        //    return instance;
        //}



    }
}