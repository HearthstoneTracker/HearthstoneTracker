namespace HearthCap.Shell.Flyouts
{
    using MahApps.Metro.Controls;

    public class ToggleFlyoutCommand
    {
        public ToggleFlyoutCommand(string name, bool? isModal = null)
        {
            this.Name = name;
            this.IsModal = isModal;
        }

        public string Name { get; set; }

        public bool? IsModal { get; set; }

        public bool? Show { get; set; }
    }
}