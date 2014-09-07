namespace HearthCap.Features.Settings
{
    using HearthCap.Core.GameCapture;

    public class SettingModel
    {
        public SettingModel()
        {
        }

        public SettingModel(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }

        public string Name { get; set; }

        public object Value { get; set; }
    }
}