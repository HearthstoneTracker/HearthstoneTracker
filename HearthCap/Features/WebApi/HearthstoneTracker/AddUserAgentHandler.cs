namespace HearthCap.Features.WebApi.HearthstoneTracker
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Reflection;
    using System.Threading.Tasks;

    public class AddUserAgentHandler : DelegatingHandler
    {
        private static Version version = Assembly.GetEntryAssembly().GetName().Version;

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            request.Headers.UserAgent.Clear();
            request.Headers.UserAgent.Add(new ProductInfoHeaderValue("HearthstoneTracker", this.GetVersion()));
            var response = await base.SendAsync(request, cancellationToken);
            return response;
        }

        private string GetVersion()
        {
            return String.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
        }
    }
}