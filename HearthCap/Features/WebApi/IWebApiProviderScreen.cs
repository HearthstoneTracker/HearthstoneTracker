using Caliburn.Micro;

namespace HearthCap.Features.WebApi
{
    public interface IWebApiProviderScreen : IScreen
    {
        bool IsEnabled { get; set; }

        void Initialize();
    }
}
