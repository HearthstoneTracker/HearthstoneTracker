// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsManager.cs" company="">
//   
// </copyright>
// <summary>
//   The settings manager.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Shell.Settings
{
    using System;
    using System.ComponentModel.Composition;
    using System.Data.Entity;
    using System.Linq;

    using HearthCap.Data;

    /// <summary>
    /// The settings manager.
    /// </summary>
    [Export(typeof(SettingsManager))]
    public class SettingsManager
    {
        /// <summary>
        /// The db context.
        /// </summary>
        private readonly Func<HearthStatsDbContext> dbContext;

        /// <summary>
        /// The current profile.
        /// </summary>
        private Settings currentProfile;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsManager"/> class.
        /// </summary>
        /// <param name="dbContext">
        /// The db context.
        /// </param>
        [ImportingConstructor]
        public SettingsManager(Func<HearthStatsDbContext> dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <summary>
        /// The load.
        /// </summary>
        /// <param name="profile">
        /// The profile.
        /// </param>
        /// <param name="force">
        /// The force.
        /// </param>
        public void Load(string profile, bool force = false)
        {
            if (!force && this.currentProfile != null && this.currentProfile.Key == profile)
            {
                return;
            }

            Settings engineSettings;
            using (var context = this.dbContext())
            {
                engineSettings = context.Settings.Include(s => s.Items).FirstOrDefault(x => x.Key == profile);
                var changed = false;
                if (engineSettings == null)
                {
                    engineSettings = new Settings {
                                             Key = profile
                                         };
                    context.Settings.Add(engineSettings);
                    changed = true;
                }

                if (changed)
                {
                    context.SaveChanges();
                }
            }

            this.currentProfile = engineSettings;
        }

        /// <summary>
        /// The get or create.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="default">
        /// The default.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetOrCreate(string key, int @default)
        {
            if (this.currentProfile == null)
            {
                this.Load("default");
            }

            if (this.currentProfile == null) return @default;

            var item = this.currentProfile.Items.FirstOrDefault(i => i.Key == key);
            if (item == null)
            {
                using (var context = this.dbContext())
                {
                    item = new SettingsItem(key, this.currentProfile) { IntValue = @default };
                    context.Settings.Attach(this.currentProfile);

                    // context.Entry(currentProfile).Collection(x=>x.Items).Load();
                    this.currentProfile.Items.Add(item);
                    context.SaveChanges();
                }

                return @default;
            }

            return item.IntValue;
        }

        /// <summary>
        /// The set.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public void Set(string key, int value)
        {
            GetOrCreate(key, value);
            using (var context = this.dbContext())
            {
                var item = this.currentProfile.Items.First(x => x.Key == key);
                context.Settings.Attach(this.currentProfile);
                context.SettingsItems.Attach(item);
                item.IntValue = value;
                context.SaveChanges();
            }
        }

        /// <summary>
        /// The get or create.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="default">
        /// The default.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetOrCreate(string key, string @default)
        {
            if (this.currentProfile == null)
            {
                this.Load("default");
            }

            if (this.currentProfile == null) return @default;

            var item = this.currentProfile.Items.FirstOrDefault(i => i.Key == key);
            if (item == null)
            {
                using (var context = this.dbContext())
                {
                    item = new SettingsItem(key, this.currentProfile) { StringValue = @default };
                    context.Settings.Attach(this.currentProfile);

                    // context.Entry(currentProfile).Collection(x=>x.Items).Load();
                    this.currentProfile.Items.Add(item);
                    context.SaveChanges();
                }

                return @default;
            }

            return item.StringValue;
        }

        /// <summary>
        /// The set.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public void Set(string key, string value)
        {
            GetOrCreate(key, value);
            using (var context = this.dbContext())
            {
                var item = this.currentProfile.Items.First(x => x.Key == key);
                context.Settings.Attach(this.currentProfile);
                context.SettingsItems.Attach(item);
                item.StringValue = value;
                context.SaveChanges();
            }
        }
    }
}