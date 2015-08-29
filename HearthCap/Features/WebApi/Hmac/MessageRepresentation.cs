using System;

namespace HearthCap.Features.WebApi.Hmac
{
    public class MessageRepresentation
    {
        public string Representation { get; set; }
        public string ApiKey { get; set; }
        public DateTime Date { get; set; }
    }
}
