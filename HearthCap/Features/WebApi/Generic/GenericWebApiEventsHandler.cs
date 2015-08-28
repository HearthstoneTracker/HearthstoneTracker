namespace HearthCap.Features.WebApi.Generic
{
    using System;
    using System.Net.Http;

    using Caliburn.Micro;

    using HearthCap.Core.GameCapture.HS.Events;
    using HearthCap.Features.GameManager.Events;
    using HearthCap.Features.WebApi.Hmac;

    using NLog;

    public class GenericWebApiEventsHandler : IWebApiEventsHandler,
        IHandle<GameStarted>,
        IHandle<GameEnded>,
        IHandle<ArenaSessionStarted>,
        IHandle<ArenaSessionEnded>,
        IDisposable
    {
        private static readonly Logger Log = NLog.LogManager.GetCurrentClassLogger();

        private WebApiProviderDescriptor providerDescriptor;

        private HttpClient client;

        private string baseUrl;

        private bool _disposed;

        public GenericWebApiEventsHandler()
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
                new RequestContentMd5Handler()
                {
                    InnerHandler = signingHandler
                });            
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(GameStarted message)
        {
            this.Post("gamestarted", message);
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(GameEnded message)
        {
            this.Post("gameended", message);
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(ArenaSessionStarted message)
        {
            Post("arenastarted", message);
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(ArenaSessionEnded message)
        {
            Post("arenaended", message);
        }

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

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if (client != null)
                {
                    client.Dispose();
                }
            }

            _disposed = true;
        }
    }
}