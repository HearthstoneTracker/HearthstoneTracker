namespace HearthCap.Shell.Flyouts
{
    public class ToggleFlyoutCommand
    {
        public ToggleFlyoutCommand(string name, bool? isModal = null)
        {
            Name = name;
            IsModal = isModal;
        }

        public string Name { get; set; }

        public bool? IsModal { get; set; }

        public bool? Show { get; set; }
    }
}
