//namespace HearthCap.Logging
//{
//    using System;
//    using System.ComponentModel.Composition;
//    using System.IO;

//    using Caliburn.Micro;

//    using HearthCap.Shell.Events;

//    using LogManager = NLog.LogManager;

//    [Export(typeof(AppDataFolderManager))]
//    public class AppDataFolderManager
//    {
//        private readonly IAppLogManager appLogManager;

//        private readonly IEventAggregator events;

//        [ImportingConstructor]
//        public AppDataFolderManager(IAppLogManager appLogManager, IEventAggregator events)
//        {
//            this.appLogManager = appLogManager;
//            this.events = events;
//        }

//        public void Initialize()
//        {
//            string appFolderName;
//            using (var reg = new DataDirectorySettings())
//            {
//                if (String.IsNullOrEmpty(reg.DataDirectory) || !Directory.Exists(reg.DataDirectory))
//                {
//                    appFolderName = "HearthstoneTracker";
//                    appFolderName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), appFolderName);
//                    reg.DataDirectory = appFolderName;
//                }
//                appFolderName = reg.DataDirectory;
//            }

//            var logFolder = Path.Combine(appFolderName, "logs");

//            if (!Directory.Exists(appFolderName))
//            {
//                Directory.CreateDirectory(appFolderName);
//            }

//            if (!Directory.Exists(logFolder))
//            {
//                Directory.CreateDirectory(logFolder);
//            }

//            AppDomain.CurrentDomain.SetData("DataDirectory", appFolderName);
//        }

//        public void ChangeDataFolder(string path)
//        {
//            if (!Directory.Exists(path))
//                throw new DirectoryNotFoundException(path);

//            var logFolder = Path.Combine(path, "logs");

//            LogManager.Shutdown();
//            if (!Directory.Exists(logFolder))
//            {
//                Directory.CreateDirectory(logFolder);
//            }
//            this.appLogManager.Initialize(logFolder);
//            LogManager.ReconfigExistingLoggers();

//            AppDomain.CurrentDomain.SetData("DataDirectory", path);
//            this.events.PublishOnBackgroundThread(new RefreshAll());
//        }
//    }
//}