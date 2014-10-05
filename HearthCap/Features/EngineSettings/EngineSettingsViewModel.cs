// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EngineSettingsViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The speed screen.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.EngineSettings
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;

    using Caliburn.Micro;

    using HearthCap.Core.GameCapture;
    using HearthCap.Data;
    using HearthCap.Features.EngineControl;
    using HearthCap.Features.Settings;
    using HearthCap.Shell.Settings;
    using HearthCap.Util;

    /// <summary>
    /// The speed screen.
    /// </summary>
    public abstract class SpeedScreen : Screen, ISettingsScreen
    {
        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        public int Order { get; set; }
    }

    /// <summary>
    /// The engine settings view model.
    /// </summary>
    [Export(typeof(ISettingsScreen))]
    public class EngineSettingsViewModel : SettingsScreen
    {
        /// <summary>
        /// The db context.
        /// </summary>
        private readonly Func<HearthStatsDbContext> dbContext;

        /// <summary>
        /// The capture engine.
        /// </summary>
        private readonly ICaptureEngine captureEngine;

        /// <summary>
        /// The settings manager.
        /// </summary>
        private readonly SettingsManager settingsManager;

        /// <summary>
        /// The default speeds.
        /// </summary>
        private IEnumerable<SettingModel> defaultSpeeds = new[]
                                                                   {
                                                                       new SettingModel {
                                                                               Name = "Slow (5 fps)", 
                                                                               Value = (int)HearthCap.Core.GameCapture.Speeds.Slow
                                                                           }, 
                                                                       new SettingModel {
                                                                               Name = "Medium (10 fps)", 
                                                                               Value = (int)HearthCap.Core.GameCapture.Speeds.Medium
                                                                           }, 
                                                                       new SettingModel {
                                                                               Name = "Fast (20 fps)", 
                                                                               Value = (int)HearthCap.Core.GameCapture.Speeds.Fast
                                                                           }, 
                                                                       new SettingModel {
                                                                               Name = "Very fast (30 fps)", 
                                                                               Value = (int)HearthCap.Core.GameCapture.Speeds.VeryFast
                                                                           }, 
                                                                       new SettingModel {
                                                                               Name = "Insanely Fast (60 fps)", 
                                                                               Value = (int)HearthCap.Core.GameCapture.Speeds.InsanelyFast
                                                                           }, 
                                                                       new SettingModel {
                                                                               Name = "No delay (not recommended!)", 
                                                                               Value = (int)HearthCap.Core.GameCapture.Speeds.NoDelay
                                                                           }
                                                                   };

        /// <summary>
        /// The selected speed.
        /// </summary>
        private SettingModel selectedSpeed;

        /// <summary>
        /// The selected engine.
        /// </summary>
        private SettingModel selectedEngine;

        /// <summary>
        /// The engines.
        /// </summary>
        private BindableCollection<SettingModel> engines = new BindableCollection<SettingModel>(new[]
                                                                                                    {
                                                                                                        new SettingModel("Auto detect (default)", CaptureMethod.AutoDetect), 
                                                                                                        new SettingModel("Screen Capture (supports background capture)", CaptureMethod.Wdm), 
                                                                                                        new SettingModel("DirectX (supports bg & full-screen)", CaptureMethod.DirectX), 
                                                                                                        new SettingModel("Hook (fast, 100% accurate, beta)", CaptureMethod.Log)
                                                                                                    });

        /// <summary>
        /// Initializes a new instance of the <see cref="EngineSettingsViewModel"/> class.
        /// </summary>
        /// <param name="dbContext">
        /// The db context.
        /// </param>
        /// <param name="captureEngine">
        /// The capture engine.
        /// </param>
        /// <param name="settingsManager">
        /// The settings manager.
        /// </param>
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
            this.Speeds = new BindableCollection<SettingModel>(this.defaultSpeeds);
            this.Order = 0;
        }

        /// <summary>
        /// Gets or sets the speeds.
        /// </summary>
        public IObservableCollection<SettingModel> Speeds { get; set; }

        /// <summary>
        /// Gets or sets the selected speed.
        /// </summary>
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
                this.UpdateSettings();
                this.NotifyOfPropertyChange(() => this.SelectedSpeed);
            }
        }

        /// <summary>
        /// Gets or sets the selected engine.
        /// </summary>
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
                this.UpdateSettings();
                this.NotifyOfPropertyChange(() => this.SelectedEngine);
            }
        }

        /// <summary>
        /// Gets the engines.
        /// </summary>
        public IObservableCollection<SettingModel> Engines
        {
            get
            {
                return this.engines;
            }
        }

        /// <summary>
        /// The update settings.
        /// </summary>
        private async void UpdateSettings()
        {
            if (PauseNotify.IsPaused(this)) return;

            this.captureEngine.Speed = (int)this.SelectedSpeed.Value;
            this.captureEngine.CaptureMethod = (CaptureMethod)this.SelectedEngine.Value;
            using (var reg = new EngineRegistrySettings())
            {
                reg.Speed = (int)this.SelectedSpeed.Value;
                reg.CaptureMethod = (CaptureMethod)this.SelectedEngine.Value;
            }
        }

        /// <summary>
        /// The load settings.
        /// </summary>
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