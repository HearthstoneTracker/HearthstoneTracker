using System;
using System.Net.Http;
using Caliburn.Micro;
using HearthCap.Core.GameCapture.HS.Events;
using HearthCap.Features.WebApi.Hmac;
using LogManager = NLog.LogManager;

namespace HearthCap.Features.WebApi.Generic
{
    public class GenericWebApiEventsHandler : IWebApiEventsHandler,
        IHandle<GameStarted>,
        IHandle<GameEnded>,
        IHandle<ArenaSessionStarted>,
        IHandle<ArenaSessionEnded>,
        IDisposable
    {
        private static readonly NLog.Logger Log = LogManager.GetCurrentClassLogger();

        private WebApiProviderDescriptor providerDescriptor;

        private HttpClient client;

        private string baseUrl;

        private bool _disposed;

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
                new RequestContentMd5Handler
                    {
                        InnerHandler = signingHandler
                    });
        }

        /// <summary>
        ///     Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(GameStarted message)
        {
            Post("gamestarted", message);
        }

        /// <summary>
        ///     Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(GameEnded message)
        {
            Post("gameended", message);
        }

        /// <summary>
        ///     Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(ArenaSessionStarted message)
        {
            Post("arenastarted", message);
        }

        /// <summary>
        ///     Handles the message.
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
                var url = baseUrl + path;
                client.PostAsJsonAsync(url, message);
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
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

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
