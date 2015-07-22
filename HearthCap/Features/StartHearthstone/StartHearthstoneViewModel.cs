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
    using Microsoft.WindowsAPICodePack.Dialogs.Controls;

    [Export(typeof(ICommandBarItem))]
    public class StartHearthstoneViewModel : CommandBarItemViewModel
    {
        [ImportingConstructor]
        public StartHearthstoneViewModel()
        {
            this.Order = -3;
        }

        public void StartHearthstone()
        {
            try
            {
                var wnd = HearthstoneHelper.GetHearthstoneWindow();
                if (wnd != IntPtr.Zero) return;

                var hsLocation = String.Empty;
                using (var settings = new HearthstoneRegistrySettings())
                {
                    hsLocation = settings.BattleNetLocation;
                    if (String.IsNullOrEmpty(hsLocation) || !File.Exists(hsLocation))
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
                            if (AskHearthstoneLocation(out hsLocation))
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
            catch (Exception)
            {
                // TODO: check, show error message instead of ignoring exception
            }
        }

        private bool AskHearthstoneLocation(out string hsLocation)
        {
            hsLocation = String.Empty;
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

    public class HearthstoneRegistrySettings : RegistrySettings
    {
        public HearthstoneRegistrySettings()
            : base(@"Software\HearthstoneTracker")
        {
        }

        public string BattleNetLocation
        {
            get
            {
                return GetOrCreate("BattleNetLocation", String.Empty);
            }
            set
            {
                SetValue("BattleNetLocation", value, RegistryValueKind.String);
            }
        }
    }
}