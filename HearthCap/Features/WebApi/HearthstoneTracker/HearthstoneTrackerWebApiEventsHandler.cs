namespace HearthCap.Features.WebApi.HearthstoneTracker
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Caliburn.Micro;

    using HearthCap.Core.GameCapture.HS.Events;
    using HearthCap.Features.WebApi.Hmac;

    using NLog;

    public class HearthstoneTrackerWebApiEventsHandler : IWebApiEventsHandler,
        IHandleWithTask<GameStarted>,
        IHandleWithTask<GameEnded>,
        IHandleWithTask<ArenaSessionStarted>,
        IHandleWithTask<ArenaSessionEnded>
    {
        private static readonly Logger Log = NLog.LogManager.GetCurrentClassLogger();

        private WebApiProviderDescriptor providerDescriptor;

        private HttpClient client;

        private string baseUrl;

        public HearthstoneTrackerWebApiEventsHandler()
        {
        }

        public void Initialize(WebApiProviderDescriptor providerDescriptor)
        {
            this.providerDescriptor = providerDescriptor;
            this.baseUrl = this.providerDescriptor.Data["Url"];
            if (!this.baseUrl.EndsWith("/"))
            {
                this.baseUrl = this.baseUrl + "/";
            }

            var signingHandler = new HmacSigningHandler(
                this.providerDescriptor.Data["ApiKey"],
                this.providerDescriptor.Data["SecretKey"],
                new CanonicalRepresentationBuilder(),
                new HmacSignatureCalculator());

            this.client = new HttpClient(
                new AddUserAgentHandler()
                    {
                        InnerHandler =
                            new RequestContentMd5Handler()
                                {
                                    InnerHandler = signingHandler
                                }
                    });
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public async Task Handle(GameStarted message)
        {
            await this.PostAsync("gamestarted", message);
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public async Task Handle(GameEnded message)
        {
            await this.PostAsync("gameended", message);
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public async Task Handle(ArenaSessionStarted message)
        {
            await this.PostAsync("arenastarted", message);
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public async Task Handle(ArenaSessionEnded message)
        {
            await this.PostAsync("arenaended", message);
        }

        private async Task PostAsync(string path, object message)
        {
            try
            {
                var url = this.baseUrl + path;
                await this.client.PostAsJsonAsync(url, message);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
    }
}