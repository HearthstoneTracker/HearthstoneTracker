namespace HearthCap.Features.Settings
{
    public class SettingModel
    {
        public SettingModel()
        {
        }

        public SettingModel(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }

        public object Value { get; set; }
    }
}
