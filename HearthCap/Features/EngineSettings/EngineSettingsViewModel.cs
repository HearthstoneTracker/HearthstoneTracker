namespace HearthCap.Features.EngineSettings
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Configuration;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    using Caliburn.Micro;

    using HearthCap.Core.GameCapture;
    using HearthCap.Data;
    using HearthCap.Features.EngineControl;
    using HearthCap.Features.Settings;
    using HearthCap.Shell.Settings;
    using HearthCap.Util;

    public abstract class SpeedScreen : Screen, ISettingsScreen
    {
        public int Order { get; set; }
    }

    [Export(typeof(ISettingsScreen))]
    public class EngineSettingsViewModel : SettingsScreen
    {
        private readonly Func<HearthStatsDbContext> dbContext;

        private readonly ICaptureEngine captureEngine;

        private readonly SettingsManager settingsManager;

        private IEnumerable<SettingModel> defaultSpeeds = new[]
                                                                   {
                                                                       new SettingModel()
                                                                           {
                                                                               Name = "Slow (5 fps)",
                                                                               Value = (int)HearthCap.Core.GameCapture.Speeds.Slow
                                                                           },
                                                                       new SettingModel()
                                                                           {
                                                                               Name = "Medium (10 fps)",
                                                                               Value = (int)HearthCap.Core.GameCapture.Speeds.Medium
                                                                           },
                                                                       new SettingModel()
                                                                           {
                                                                               Name = "Fast (20 fps)",
                                                                               Value = (int)HearthCap.Core.GameCapture.Speeds.Fast
                                                                           },
                                                                       new SettingModel()
                                                                           {
                                                                               Name = "Very fast (30 fps)",
                                                                               Value = (int)HearthCap.Core.GameCapture.Speeds.VeryFast
                                                                           },
                                                                       new SettingModel()
                                                                           {
                                                                               Name = "Insanely Fast (60 fps)",
                                                                               Value = (int)HearthCap.Core.GameCapture.Speeds.InsanelyFast
                                                                           },
                                                                       new SettingModel()
                                                                           {
                                                                               Name = "No delay (not recommended!)",
                                                                               Value = (int)HearthCap.Core.GameCapture.Speeds.NoDelay
                                                                           }
                                                                   };

        private SettingModel selectedSpeed;

        private SettingModel selectedEngine;

        private BindableCollection<SettingModel> engines = new BindableCollection<SettingModel>(new[]
                                                                                                    {
                                                                                                        new SettingModel("Auto detect (default)", CaptureMethod.AutoDetect),
                                                                                                        new SettingModel("Screen Capture (supports background capture)", CaptureMethod.Wdm),
                                                                                                        new SettingModel("DirectX (supports bg & full-screen)", CaptureMethod.DirectX),
                                                                                                        new SettingModel("Hook (fast, 100% accurate, beta)", CaptureMethod.Log)
                                                                                                    });

        [ImportingConstructor]
        public EngineSettingsViewModel(
            Func<HearthStatsDbContext> dbContext,
            ICaptureEngine captureEngine,
            SettingsManager settingsManager)
        {
            this.dbContext = dbContext;
            this.captureEngine = captureEngine;
            this.settingsManager = settingsManager;
            this.DisplayName = "Engine settings:";
            this.Speeds = new BindableCollection<SettingModel>(defaultSpeeds);
            Order = 0;
        }

        public IObservableCollection<SettingModel> Speeds { get; set; }

        public SettingModel SelectedSpeed
        {
            get
            {
                return this.selectedSpeed;
            }
            set
            {
                if (Equals(value, this.selectedSpeed))
                {
                    return;
                }
                this.selectedSpeed = value;
                UpdateSettings();
                this.NotifyOfPropertyChange(() => this.SelectedSpeed);
            }
        }

        public SettingModel SelectedEngine
        {
            get
            {
                return this.selectedEngine;
            }
            set
            {
                if (value == this.selectedEngine)
                {
                    return;
                }
                this.selectedEngine = value;
                UpdateSettings();
                this.NotifyOfPropertyChange(() => this.SelectedEngine);
            }
        }

        public IObservableCollection<SettingModel> Engines
        {
            get
            {
                return engines;
            }
        }

        private void UpdateSettings()
        {
            if (PauseNotify.IsPaused(this)) return;

            this.captureEngine.Speed = (int)this.SelectedSpeed.Value;
            this.captureEngine.CaptureMethod = ((CaptureMethod)this.SelectedEngine.Value);
            using (var reg = new EngineRegistrySettings())
            {
                reg.Speed = (int)this.SelectedSpeed.Value;
                reg.CaptureMethod = (CaptureMethod)this.SelectedEngine.Value;
            }
        }

        private void LoadSettings()
        {
            using (PauseNotify.For(this))
            {
                using (var reg = new EngineRegistrySettings())
                {
                    this.SelectedSpeed = this.Speeds.FirstOrDefault(speed => (int)speed.Value == reg.Speed);
                    this.SelectedEngine = this.Engines.FirstOrDefault(key => (CaptureMethod)key.Value == reg.CaptureMethod);
                }
            }
        }

        /// <summary>
        /// Called when initializing.
        /// </summary>
        protected override void OnInitialize()
        {
            this.LoadSettings();
        }
    }
}