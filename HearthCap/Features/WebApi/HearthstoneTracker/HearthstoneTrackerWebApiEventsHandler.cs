// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HearthstoneTrackerWebApiEventsHandler.cs" company="">
//   
// </copyright>
// <summary>
//   The hearthstone tracker web api events handler.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.WebApi.HearthstoneTracker
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Caliburn.Micro;

    using HearthCap.Core.GameCapture.HS.Events;
    using HearthCap.Features.WebApi.Hmac;

    using NLog;

    using LogManager = NLog.LogManager;

    /// <summary>
    /// The hearthstone tracker web api events handler.
    /// </summary>
    public class HearthstoneTrackerWebApiEventsHandler : IWebApiEventsHandler, 
        IHandleWithTask<GameStarted>, 
        IHandleWithTask<GameEnded>, 
        IHandleWithTask<ArenaSessionStarted>, 
        IHandleWithTask<ArenaSessionEnded>
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The provider descriptor.
        /// </summary>
        private WebApiProviderDescriptor providerDescriptor;

        /// <summary>
        /// The client.
        /// </summary>
        private HttpClient client;

        /// <summary>
        /// The base url.
        /// </summary>
        private string baseUrl;

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="providerDescriptor">
        /// The provider descriptor.
        /// </param>
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
                new AddUserAgentHandler {
                        InnerHandler =
                            new RequestContentMd5Handler {
                                    InnerHandler = signingHandler
                                }
                    });
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task Handle(GameStarted message)
        {
            await this.PostAsync("gamestarted", message);
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task Handle(GameEnded message)
        {
            await this.PostAsync("gameended", message);
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task Handle(ArenaSessionStarted message)
        {
            await this.PostAsync("arenastarted", message);
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task Handle(ArenaSessionEnded message)
        {
            await this.PostAsync("arenaended", message);
        }

        /// <summary>
        /// The post async.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
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