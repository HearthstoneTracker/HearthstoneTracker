namespace HearthCap.Features.WebApi
{
    using Caliburn.Micro;

    public interface IWebApiProviderScreen : IScreen
    {
        bool IsEnabled { get; set; }

        void Initialize();
    }
}