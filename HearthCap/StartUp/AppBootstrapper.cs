// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppBootstrapper.cs" company="">
//   
// </copyright>
// <summary>
//   The app bootstrapper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.StartUp
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Markup;

    using Caliburn.Micro;
    using Caliburn.Micro.Recipes.Filters.Framework;

    using HearthCap.Core.GameCapture;
    using HearthCap.Data;
    using HearthCap.Logging;
    using HearthCap.Shell;
    using HearthCap.Shell.Dialogs;
    using HearthCap.Shell.TrayIcon;

    using Configuration = HearthCap.Data.Migrations.Configuration;

    /// <summary>
    /// The app bootstrapper.
    /// </summary>
    public class AppBootstrapper : BootstrapperBase, IDisposable
    {
        /// <summary>
        /// The application data directory.
        /// </summary>
        private string applicationDataDirectory;

        /// <summary>
        /// The container.
        /// </summary>
        private static CompositionContainer container;

        /// <summary>
        /// The log manager.
        /// </summary>
        private IAppLogManager logManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppBootstrapper"/> class.
        /// </summary>
        public AppBootstrapper()
        {
            this.Initialize();
        }

        /// <summary>
        /// Gets the container.
        /// </summary>
        public static CompositionContainer Container
        {
            get
            {
                return container;
            }
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            if (Container != null)
            {
                Container.Dispose();
            }

            if (this.logManager != null)
            {
                this.logManager.Flush();
                this.logManager.Dispose();
            }
        }

        /// <summary>
        /// The configure.
        /// </summary>
        protected override void Configure()
        {
            this.InitializeApplicationDataDirectory();

            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement), 
                new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            // var aggregateCatalog = new AggregateCatalog(AssemblySource.Instance.Select(x => new AssemblyCatalog(x)));
            var aggregateCatalog =
                new AggregateCatalog(
                    new ComposablePartCatalog[]
                        {
                            new AssemblyCatalog(this.GetType().Assembly), new AssemblyCatalog(typeof(AutoCaptureEngine).Assembly), 
                            new AssemblyCatalog(typeof(IRepository<>).Assembly) 
                        });
            container = new CompositionContainer(aggregateCatalog);
            var batch = new CompositionBatch();
            batch.AddExportedValue(container);
            batch.AddExportedValue<IEventAggregator>(new EventAggregator());
            batch.AddExportedValue<IWindowManager>(new CustomWindowManager());
            batch.AddExportedValue<Func<IMessageBox>>(() => container.GetExportedValue<IMessageBox>());
            batch.AddExportedValue<Func<HearthStatsDbContext>>(() => new HearthStatsDbContext());

            // batch.AddExportedValue<IWindowManager>(new AppWindowManager());
            // batch.AddExportedValue(MessageBus.Current);
            var compose = container.GetExportedValue<CompositionBuilder>();
            compose.Compose(batch);

            // var composeTasks = this.GetAllInstances<ICompositionTask>();
            // composeTasks.Apply(s => s.Compose(batch));
            container.Compose(batch);

            FilterFrameworkCoreCustomization.Hook();

            var logPath = Path.Combine((string)AppDomain.CurrentDomain.GetData("DataDirectory"), "logs");
            container.GetExportedValue<IAppLogManager>().Initialize(logPath);
            container.GetExportedValue<CrashManager>().WireUp();

            this.Application.Activated +=
                (s, e) => container.GetExportedValue<IEventAggregator>().PublishOnCurrentThread(new ApplicationActivatedEvent());
            this.Application.Deactivated +=
                (s, e) => container.GetExportedValue<IEventAggregator>().PublishOnCurrentThread(new ApplicationDeActivatedEvent());
        }

        /// <summary>
        /// The initialize application data directory.
        /// </summary>
        private void InitializeApplicationDataDirectory()
        {
            string appFolderName;
            using (var reg = new DataDirectorySettings())
            {
                appFolderName = reg.DataDirectory;
                if (string.IsNullOrEmpty(reg.DataDirectory) || !Directory.Exists(reg.DataDirectory))
                {
                    appFolderName = "HearthstoneTracker";
                    appFolderName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), appFolderName);
                    if (!Directory.Exists(appFolderName))
                    {
                        Directory.CreateDirectory(appFolderName);
                    }

                    reg.DataDirectory = appFolderName;
                }
            }

            var logFolder = Path.Combine(appFolderName, "logs");

            if (!Directory.Exists(logFolder))
            {
                Directory.CreateDirectory(logFolder);
            }

            AppDomain.CurrentDomain.SetData("DataDirectory", appFolderName);
        }

        /// <summary>
        /// The get all instances.
        /// </summary>
        /// <param name="serviceType">
        /// The service type.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
        }

        /// <summary>
        /// The get all instances.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        protected IEnumerable<T> GetAllInstances<T>()
        {
            return container.GetExportedValues<T>();
        }

        /// <summary>
        /// The get instance.
        /// </summary>
        /// <param name="serviceType">
        /// The service type.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        protected override object GetInstance(Type serviceType, string key)
        {
            var str = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
            var obj = container.GetExportedValues<object>(str).FirstOrDefault<object>();
            if (obj == null)
            {
                CultureInfo invariantCulture = CultureInfo.InvariantCulture;
                var objArray = new object[] { str };
                throw new InvalidOperationException(string.Format(invariantCulture, "Could not locate any exported values for '{0}'.", objArray));
            }

            return obj;
        }

        /// <summary>
        /// The build up.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        protected override void BuildUp(object instance)
        {
            container.SatisfyImportsOnce(instance);
        }

        /// <summary>
        /// The on startup.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            var currentUICult = Thread.CurrentThread.CurrentUICulture.Name;
            var currentCult = Thread.CurrentThread.CurrentCulture.Name;

            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(currentUICult);
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(currentCult);

            this.InitializeDatabase();

            var startupTasks = this.GetAllInstances(typeof(IStartupTask)).Cast<IStartupTask>();
            startupTasks.Apply(s => s.Run());

            // var userSettings = (UserPreferences)GetInstance(typeof(UserPreferences), null);
            var trayIcon = (TrayIconViewModel)this.GetInstance(typeof(TrayIconViewModel), null);
            var windowManager = (CustomWindowManager)this.GetInstance(typeof(CustomWindowManager), null);
            var start = windowManager.MainWindow<StartupViewModel>();
            start.Show();
            start.Hide();

            // DisplayRootViewFor<StartupViewModel>();
        }

        /// <summary>
        /// The initialize database.
        /// </summary>
        private void InitializeDatabase()
        {
            // Only seed when needed
            var dataDir = (string)AppDomain.CurrentDomain.GetData("DataDirectory");
            var seedFile = Path.Combine(dataDir, "db.seed");
            int currentSeed = 0;
            if (File.Exists(seedFile))
            {
                int.TryParse(File.ReadAllText(seedFile), out currentSeed);
            }

            if (currentSeed < Configuration.SeedVersion)
            {
                using (var context = new HearthStatsDbContext())
                {
                    new DbInitializer().InitializeDatabase(context);
                }

                File.WriteAllText(seedFile, Configuration.SeedVersion.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                Task.Run(
                    () =>
                    {
                        using (var context = new HearthStatsDbContext())
                        {
                        }
                    });
            }
        }
    }

    /// <summary>
    /// The composition builder.
    /// </summary>
    [Export(typeof(CompositionBuilder))]
    public class CompositionBuilder
    {
        /// <summary>
        /// The tasks.
        /// </summary>
        private readonly IEnumerable<ICompositionTask> tasks;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionBuilder"/> class.
        /// </summary>
        /// <param name="tasks">
        /// The tasks.
        /// </param>
        [ImportingConstructor]
        public CompositionBuilder([ImportMany]IEnumerable<ICompositionTask> tasks)
        {
            this.tasks = tasks;
        }

        /// <summary>
        /// The compose.
        /// </summary>
        /// <param name="batch">
        /// The batch.
        /// </param>
        public void Compose(CompositionBatch batch)
        {
            this.tasks.Apply(s => s.Compose(batch));
        }
    }
}