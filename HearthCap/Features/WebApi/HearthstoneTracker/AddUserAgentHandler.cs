using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace HearthCap.Features.WebApi.HearthstoneTracker
{
    public class AddUserAgentHandler : DelegatingHandler
    {
        private static readonly Version version = Assembly.GetEntryAssembly().GetName().Version;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.UserAgent.Clear();
            request.Headers.UserAgent.Add(new ProductInfoHeaderValue("HearthstoneTracker", GetVersion()));
            var response = await base.SendAsync(request, cancellationToken);
            return response;
        }

        private string GetVersion()
        {
            return String.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
        }
    }
}
