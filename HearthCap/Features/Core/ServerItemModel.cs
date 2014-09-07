namespace HearthCap.Features.Core
{
    using Caliburn.Micro;

    public class ServerItemModel : PropertyChangedBase
    {
        private string name;

        private bool isChecked;

        public ServerItemModel(string name)
        {
            this.name = name;
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (value == this.name)
                {
                    return;
                }
                this.name = value;
                this.NotifyOfPropertyChange(() => this.Name);
            }
        }

        public bool IsChecked
        {
            get
            {
                return this.isChecked;
            }
            set
            {
                if (value.Equals(this.isChecked))
                {
                    return;
                }
                this.isChecked = value;
                this.NotifyOfPropertyChange(() => this.IsChecked);
            }
        }
    }
}