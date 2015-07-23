namespace HearthCap.Features.Core
{
    public class ServerChanged
    {
        public ServerItemModel Server { get; protected set; }

        public ServerChanged(ServerItemModel server)
        {
            this.Server = server;
        }
    }
}