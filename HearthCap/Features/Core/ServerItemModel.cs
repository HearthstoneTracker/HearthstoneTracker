using Caliburn.Micro;

namespace HearthCap.Features.Core
{
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
            get { return name; }
            set
            {
                if (value == name)
                {
                    return;
                }
                name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                if (value.Equals(isChecked))
                {
                    return;
                }
                isChecked = value;
                NotifyOfPropertyChange(() => IsChecked);
            }
        }
    }
}
