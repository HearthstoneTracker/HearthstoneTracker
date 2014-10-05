// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StartHearthstoneViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The start hearthstone view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.StartHearthstone
{
    using System;
    using System.ComponentModel.Composition;
    using System.Diagnostics;
    using System.IO;
    using System.Windows;

    using HearthCap.Core.Util;
    using HearthCap.Shell.CommandBar;
    using HearthCap.Shell.UserPreferences;

    using Microsoft.Win32;
    using Microsoft.WindowsAPICodePack.Dialogs;

    /// <summary>
    /// The start hearthstone view model.
    /// </summary>
    [Export(typeof(ICommandBarItem))]
    public class StartHearthstoneViewModel : CommandBarItemViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StartHearthstoneViewModel"/> class.
        /// </summary>
        [ImportingConstructor]
        public StartHearthstoneViewModel()
        {
            this.Order = -3;
        }

        /// <summary>
        /// The start hearthstone.
        /// </summary>
        public void StartHearthstone()
        {
            try
            {
                var wnd = HearthstoneHelper.GetHearthstoneWindow();
                if (wnd != IntPtr.Zero) return;

                var hsLocation = string.Empty;
                using (var settings = new HearthstoneRegistrySettings())
                {
                    hsLocation = settings.BattleNetLocation;
                    if (string.IsNullOrEmpty(hsLocation) || !File.Exists(hsLocation))
                    {
                        // try default
                        var def = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Battle.net");
                        def = Path.Combine(def, "Battle.net Launcher.exe");
                        if (File.Exists(def))
                        {
                            settings.BattleNetLocation = hsLocation = def;
                        }
                        else
                        {
                            if (this.AskHearthstoneLocation(out hsLocation))
                            {
                                settings.BattleNetLocation = hsLocation;
                            }
                        }
                    }
                }

                if (File.Exists(hsLocation))
                {
                    var start = new ProcessStartInfo(hsLocation)
                                    {
                                        Arguments = "--exec=\"launch WTCG\""
                                    };

                    Process.Start(start);
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// The ask hearthstone location.
        /// </summary>
        /// <param name="hsLocation">
        /// The hs location.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool AskHearthstoneLocation(out string hsLocation)
        {
            hsLocation = string.Empty;
            var dialog = new CommonOpenFileDialog
                             {
                                 EnsurePathExists = true, 
                                 EnsureFileExists = true, 
                                 DefaultDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Battle.net"), 
                                 InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Battle.net"), 
                                 Multiselect = false, 
                                 EnsureValidNames = true, 
                                 Title = "Find Battle.net location", 
                                 IsFolderPicker = true, 
                                 AllowNonFileSystemItems = false, 
                                 AllowPropertyEditing = false, 
                                 RestoreDirectory = true
                             };

            do
            {
                var result = dialog.ShowDialog();
                if (result != CommonFileDialogResult.Ok)
                {
                    return false;
                }

                var launcher = Path.Combine(dialog.FileName, "Battle.net Launcher.exe");
                if (File.Exists(launcher))
                {
                    hsLocation = launcher;
                    return true;
                }

                var msg = MessageBox.Show("Could not find 'Battle.net Launcher.exe'. Try again?", "Battle.net launcher not found.", MessageBoxButton.YesNo);
                if (msg == MessageBoxResult.No)
                {
                    return false;
                }
            }
            while (true);

        }
    }

    /// <summary>
    /// The hearthstone registry settings.
    /// </summary>
    public class HearthstoneRegistrySettings : RegistrySettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HearthstoneRegistrySettings"/> class.
        /// </summary>
        public HearthstoneRegistrySettings()
            : base(@"Software\HearthstoneTracker")
        {
        }

        /// <summary>
        /// Gets or sets the battle net location.
        /// </summary>
        public string BattleNetLocation
        {
            get
            {
                return this.GetOrCreate("BattleNetLocation", string.Empty);
            }

            set
            {
                this.SetValue("BattleNetLocation", value, RegistryValueKind.String);
            }
        }
    }
}