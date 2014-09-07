namespace HearthCap.Features.WebApi.Hmac
{
    using System;

    public class MessageRepresentation
    {
        public string Representation { get; set; }
        public string ApiKey { get; set; }
        public DateTime Date { get; set; }
    }
}