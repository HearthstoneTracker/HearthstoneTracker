using System;
using System.Net.Http;
using System.Threading.Tasks;
using Caliburn.Micro;
using HearthCap.Core.GameCapture.HS.Events;
using HearthCap.Features.WebApi.Hmac;
using LogManager = NLog.LogManager;

namespace HearthCap.Features.WebApi.HearthstoneTracker
{
    public sealed class HearthstoneTrackerWebApiEventsHandler : IWebApiEventsHandler,
        IHandleWithTask<GameStarted>,
        IHandleWithTask<GameEnded>,
        IHandleWithTask<ArenaSessionStarted>,
        IHandleWithTask<ArenaSessionEnded>,
        IDisposable
    {
        private static readonly NLog.Logger Log = LogManager.GetCurrentClassLogger();

        private WebApiProviderDescriptor providerDescriptor;

        private HttpClient client;

        private string baseUrl;

        public void Initialize(WebApiProviderDescriptor providerDescriptor)
        {
            this.providerDescriptor = providerDescriptor;
            baseUrl = this.providerDescriptor.Data["Url"];
            if (!baseUrl.EndsWith("/"))
            {
                baseUrl = baseUrl + "/";
            }

            var signingHandler = new HmacSigningHandler(
                this.providerDescriptor.Data["ApiKey"],
                this.providerDescriptor.Data["SecretKey"],
                new CanonicalRepresentationBuilder(),
                new HmacSignatureCalculator());

            client = new HttpClient(
                new AddUserAgentHandler
                    {
                        InnerHandler =
                            new RequestContentMd5Handler
                                {
                                    InnerHandler = signingHandler
                                }
                    });
        }

        /// <summary>
        ///     Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public async Task Handle(GameStarted message)
        {
            await PostAsync("gamestarted", message);
        }

        /// <summary>
        ///     Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public async Task Handle(GameEnded message)
        {
            await PostAsync("gameended", message);
        }

        /// <summary>
        ///     Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public async Task Handle(ArenaSessionStarted message)
        {
            await PostAsync("arenastarted", message);
        }

        /// <summary>
        ///     Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public async Task Handle(ArenaSessionEnded message)
        {
            await PostAsync("arenaended", message);
        }

        private async Task PostAsync(string path, object message)
        {
            try
            {
                var url = baseUrl + path;
                await client.PostAsJsonAsync(url, message);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (client != null)
            {
                client.Dispose();
            }
        }
    }
}
