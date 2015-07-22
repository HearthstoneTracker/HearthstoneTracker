namespace HearthCap.Shell.UserPreferences
{
    using System;
    using System.Security.AccessControl;
    using System.Windows;

    using Microsoft.Win32;

    public abstract class RegistrySettings : IDisposable
    {
        private readonly string sectionName;

        private readonly RegistryKey section;

        protected RegistrySettings(string sectionName)
        {
            this.sectionName = sectionName;
            this.section = this.EnsureSectionExists(sectionName);
        }

        public RegistryKey Section
        {
            get
            {
                return this.section;
            }
        }

        private RegistryKey EnsureSectionExists(string sectionName)
        {
            var sub = Registry.CurrentUser.OpenSubKey(sectionName, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.CreateSubKey | RegistryRights.QueryValues | RegistryRights.ReadKey | RegistryRights.SetValue | RegistryRights.WriteKey);
            if (sub == null)
            {
                sub = Registry.CurrentUser.CreateSubKey(sectionName);
            }
            return sub;
        }

        public T GetOrCreate<T>(string key, T defaultValue = default (T))
        {
            var value = section.GetValue(key, defaultValue);
            if (value == null || value == (object)default (T))
            {
                value = defaultValue;
                section.SetValue(key, defaultValue, RegistryValueKind.String);
            }

            try
            {
                if (typeof(T) == typeof(int))
                {
                    value = int.Parse(value.ToString());
                }
                if (typeof(T) == typeof(double))
                {
                    value = double.Parse(value.ToString());
                }
                if (typeof(T) == typeof(bool))
                {
                    value = value.ToString() == "1" || value.ToString().ToLower() == "true";
                }
                if (typeof(T) == typeof(WindowState))
                {
                    value = Enum.Parse(typeof(WindowState), value.ToString());
                }
                if (typeof(T).IsEnum)
                {
                    value = Enum.Parse(typeof(T), value.ToString());
                }
            }
            catch (Exception)
            {
                value = defaultValue;
                section.SetValue(key, defaultValue, RegistryValueKind.String);
            }
            return (T)value;
        }

        public void SetValue(string key, object value, RegistryValueKind kind = RegistryValueKind.String)
        {
            object realValue = value;
            if (value is bool)
            {
                realValue = (bool)value ? "1" : "0";
            }
            section.SetValue(key, realValue, kind);
        }

        public void Dispose()
        {
            this.Section.Dispose();
        }
    }
}