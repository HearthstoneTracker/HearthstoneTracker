// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericWebApiEventsHandler.cs" company="">
//   
// </copyright>
// <summary>
//   The generic web api events handler.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.WebApi.Generic
{
    using System;
    using System.Net.Http;

    using Caliburn.Micro;

    using HearthCap.Core.GameCapture.HS.Events;
    using HearthCap.Features.WebApi.Hmac;

    using NLog;

    using LogManager = NLog.LogManager;

    /// <summary>
    /// The generic web api events handler.
    /// </summary>
    public class GenericWebApiEventsHandler : IWebApiEventsHandler, 
        IHandle<GameStarted>, 
        IHandle<GameEnded>, 
        IHandle<ArenaSessionStarted>, 
        IHandle<ArenaSessionEnded>
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
                new RequestContentMd5Handler {
                    InnerHandler = signingHandler
                });            
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(GameStarted message)
        {
            this.Post("gamestarted", message);
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(GameEnded message)
        {
            this.Post("gameended", message);
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(ArenaSessionStarted message)
        {
            this.Post("arenastarted", message);
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(ArenaSessionEnded message)
        {
            this.Post("arenaended", message);
        }

        /// <summary>
        /// The post.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        private void Post(string path, object message)
        {
            try
            {
                var url = this.baseUrl + path;
                this.client.PostAsJsonAsync(url, message);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
    }
}