// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegistrySettings.cs" company="">
//   
// </copyright>
// <summary>
//   The registry settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Shell.UserPreferences
{
    using System;
    using System.Security.AccessControl;
    using System.Windows;

    using Microsoft.Win32;

    /// <summary>
    /// The registry settings.
    /// </summary>
    public abstract class RegistrySettings : IDisposable
    {
        /// <summary>
        /// The section name.
        /// </summary>
        private readonly string sectionName;

        /// <summary>
        /// The section.
        /// </summary>
        private readonly RegistryKey section;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrySettings"/> class.
        /// </summary>
        /// <param name="sectionName">
        /// The section name.
        /// </param>
        protected RegistrySettings(string sectionName)
        {
            this.sectionName = sectionName;
            this.section = this.EnsureSectionExists(sectionName);
        }

        /// <summary>
        /// Gets the section.
        /// </summary>
        public RegistryKey Section
        {
            get
            {
                return this.section;
            }
        }

        /// <summary>
        /// The ensure section exists.
        /// </summary>
        /// <param name="sectionName">
        /// The section name.
        /// </param>
        /// <returns>
        /// The <see cref="RegistryKey"/>.
        /// </returns>
        private RegistryKey EnsureSectionExists(string sectionName)
        {
            var sub = Registry.CurrentUser.OpenSubKey(sectionName, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.CreateSubKey | RegistryRights.QueryValues | RegistryRights.ReadKey | RegistryRights.SetValue | RegistryRights.WriteKey);
            if (sub == null)
            {
                sub = Registry.CurrentUser.CreateSubKey(sectionName);
            }

            return sub;
        }

        /// <summary>
        /// The get or create.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T GetOrCreate<T>(string key, T defaultValue = default (T))
        {
            var value = this.section.GetValue(key, defaultValue);
            if (value == null || value == (object)default (T))
            {
                value = defaultValue;
                this.section.SetValue(key, defaultValue, RegistryValueKind.String);
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
            catch (Exception ex)
            {
                value = defaultValue;
                this.section.SetValue(key, defaultValue, RegistryValueKind.String);
            }

            return (T)value;
        }

        /// <summary>
        /// The set value.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="kind">
        /// The kind.
        /// </param>
        public void SetValue(string key, object value, RegistryValueKind kind = RegistryValueKind.String)
        {
            object realValue = value;
            if (value is bool)
            {
                realValue = (bool)value ? "1" : "0";
            }

            this.section.SetValue(key, realValue, kind);
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Section.Dispose();
        }
    }
}