using System;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Linq;
using HearthCap.Data;

namespace HearthCap.Shell.Settings
{
    [Export(typeof(SettingsManager))]
    public class SettingsManager
    {
        private readonly Func<HearthStatsDbContext> dbContext;

        private Data.Settings currentProfile;

        [ImportingConstructor]
        public SettingsManager(Func<HearthStatsDbContext> dbContext)
        {
            this.dbContext = dbContext;
        }

        public void Load(string profile, bool force = false)
        {
            if (!force
                && currentProfile != null
                && currentProfile.Key == profile)
            {
                return;
            }

            Data.Settings engineSettings;
            using (var context = dbContext())
            {
                engineSettings = context.Settings.Include(s => s.Items).FirstOrDefault(x => x.Key == profile);
                var changed = false;
                if (engineSettings == null)
                {
                    engineSettings = new Data.Settings
                        {
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

            currentProfile = engineSettings;
        }

        public int GetOrCreate(string key, int @default)
        {
            if (currentProfile == null)
            {
                Load("default");
            }
            if (currentProfile == null)
            {
                return @default;
            }

            var item = currentProfile.Items.FirstOrDefault(i => i.Key == key);
            if (item == null)
            {
                using (var context = dbContext())
                {
                    item = new SettingsItem(key, currentProfile)
                        {
                            IntValue = @default
                        };
                    context.Settings.Attach(currentProfile);
                    // context.Entry(currentProfile).Collection(x=>x.Items).Load();
                    currentProfile.Items.Add(item);
                    context.SaveChanges();
                }

                return @default;
            }
            return item.IntValue;
        }

        public void Set(string key, int value)
        {
            GetOrCreate(key, value);
            using (var context = dbContext())
            {
                var item = currentProfile.Items.First(x => x.Key == key);
                context.Settings.Attach(currentProfile);
                context.SettingsItems.Attach(item);
                item.IntValue = value;
                context.SaveChanges();
            }
        }

        public string GetOrCreate(string key, string @default)
        {
            if (currentProfile == null)
            {
                Load("default");
            }
            if (currentProfile == null)
            {
                return @default;
            }

            var item = currentProfile.Items.FirstOrDefault(i => i.Key == key);
            if (item == null)
            {
                using (var context = dbContext())
                {
                    item = new SettingsItem(key, currentProfile)
                        {
                            StringValue = @default
                        };
                    context.Settings.Attach(currentProfile);
                    // context.Entry(currentProfile).Collection(x=>x.Items).Load();
                    currentProfile.Items.Add(item);
                    context.SaveChanges();
                }

                return @default;
            }
            return item.StringValue;
        }

        public void Set(string key, string value)
        {
            GetOrCreate(key, value);
            using (var context = dbContext())
            {
                var item = currentProfile.Items.First(x => x.Key == key);
                context.Settings.Attach(currentProfile);
                context.SettingsItems.Attach(item);
                item.StringValue = value;
                context.SaveChanges();
            }
        }
    }
}
