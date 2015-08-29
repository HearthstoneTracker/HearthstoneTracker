using System.ComponentModel.DataAnnotations;

namespace HearthCap.Data
{
    public class SettingsItem
    {
        protected SettingsItem()
        {
        }

        public SettingsItem(string key, Settings settings)
        {
            Key = key;
            Settings = settings;
        }

        [Key]
        public string Key { get; protected set; }

        public Settings Settings { get; protected set; }

        public string StringValue { get; set; }

        public int IntValue { get; set; }
    }
}
