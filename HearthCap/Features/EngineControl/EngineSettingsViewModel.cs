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
    using HearthCap.Shell.UserPreferences;
    using HearthCap.Util;

    public abstract class SpeedScreen : Screen, ISettingsScreen
    {
        public int Order { get; set; }
    }

    [Export(typeof(ISettingsScreen))]
    public class EngineSettingsViewModel : SettingsScreen
    {
        private readonly IEventAggregator _events;

        private readonly ICaptureEngine _captureEngine;

        private readonly UserPreferences _userPreferences;

        private IEnumerable<SettingModel> defaultSpeeds = new[]
                                                                   {
                                                                       new SettingModel
                                                                           {
                                                                               Name = "Slow (5 fps)",
                                                                               Value = (int)HearthCap.Core.GameCapture.Speeds.Slow
                                                                           },
                                                                       new SettingModel
                                                                           {
                                                                               Name = "Medium (10 fps)",
                                                                               Value = (int)HearthCap.Core.GameCapture.Speeds.Medium
                                                                           },
                                                                       new SettingModel
                                                                           {
                                                                               Name = "Fast (20 fps)",
                                                                               Value = (int)HearthCap.Core.GameCapture.Speeds.Fast
                                                                           },
                                                                       new SettingModel
                                                                           {
                                                                               Name = "Very fast (30 fps)",
                                                                               Value = (int)HearthCap.Core.GameCapture.Speeds.VeryFast
                                                                           },
                                                                       new SettingModel
                                                                           {
                                                                               Name = "Insanely Fast (60 fps)",
                                                                               Value = (int)HearthCap.Core.GameCapture.Speeds.InsanelyFast
                                                                           },
                                                                       new SettingModel
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

        private bool _autoStart;

        private bool _showControls;

        [ImportingConstructor]
        public EngineSettingsViewModel(
            IEventAggregator events,
            ICaptureEngine captureEngine,
            UserPreferences userPreferences)
        {
            _events = events;
            _captureEngine = captureEngine;
            _userPreferences = userPreferences;
            DisplayName = "Engine settings:";
            Speeds = new BindableCollection<SettingModel>(defaultSpeeds);
            Order = 0;
        }

        public IObservableCollection<SettingModel> Speeds { get; set; }

        public SettingModel SelectedSpeed
        {
            get
            {
                return selectedSpeed;
            }
            set
            {
                if (Equals(value, selectedSpeed))
                {
                    return;
                }
                selectedSpeed = value;
                UpdateSettings();
                NotifyOfPropertyChange(() => SelectedSpeed);
            }
        }

        public SettingModel SelectedEngine
        {
            get
            {
                return selectedEngine;
            }
            set
            {
                if (value == selectedEngine)
                {
                    return;
                }
                selectedEngine = value;
                UpdateSettings();
                NotifyOfPropertyChange(() => SelectedEngine);
            }
        }


        public bool AutoStart
        {
            get
            {
                return _autoStart;
            }
            set
            {
                if (value.Equals(_autoStart))
                {
                    return;
                }
                _autoStart = value;
                UpdateSettings();
                NotifyOfPropertyChange(() => AutoStart);
            }
        }

        public bool ShowControls
        {
            get
            {
                return _showControls;
            }
            set
            {
                if (value.Equals(_showControls))
                {
                    return;
                }
                _showControls = value;
                UpdateSettings();
                NotifyOfPropertyChange(() => ShowControls);
            }
        }

        public IObservableCollection<SettingModel> Engines
        {
            get
            {
                return engines;
            }
        }

        public UserPreferences UserPreferences
        {
            get
            {
                return _userPreferences;
            }
        }

        private void UpdateSettings()
        {
            if (PauseNotify.IsPaused(this)) return;

            _captureEngine.Speed = (int)SelectedSpeed.Value;
            _captureEngine.CaptureMethod = ((CaptureMethod)SelectedEngine.Value);
            using (var reg = new EngineRegistrySettings())
            {
                reg.Speed = (int)SelectedSpeed.Value;
                reg.CaptureMethod = (CaptureMethod)SelectedEngine.Value;
                reg.AutoStart = AutoStart;
                reg.SetValue("ShowControls", ShowControls);
                _events.PublishOnBackgroundThread(new EngineRegistrySettingsChanged());
            }
        }

        private void LoadSettings()
        {
            using (PauseNotify.For(this))
            {
                using (var reg = new EngineRegistrySettings())
                {
                    SelectedSpeed = Speeds.FirstOrDefault(speed => (int)speed.Value == reg.Speed);
                    SelectedEngine = Engines.FirstOrDefault(key => (CaptureMethod)key.Value == reg.CaptureMethod);
                    AutoStart = reg.AutoStart;
                    ShowControls = reg.GetOrCreate("ShowControls", true);
                }
            }
        }

        /// <summary>
        /// Called when initializing.
        /// </summary>
        protected override void OnInitialize()
        {
            LoadSettings();
        }
    }
}