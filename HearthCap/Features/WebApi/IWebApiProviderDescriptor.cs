using System.ComponentModel;

namespace HearthCap.Features.WebApi
{
    public interface IWebApiProviderDescriptor : INotifyPropertyChanged
    {
        string ProviderKey { get; }

        string ProviderName { get; set; }

        string ProviderDescription { get; set; }

        bool IsEnabled { get; set; }

        IWebApiEventsHandler EventsHandler { get; }

        void Initialize();
    }
}
