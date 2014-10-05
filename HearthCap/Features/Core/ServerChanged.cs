// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerChanged.cs" company="">
//   
// </copyright>
// <summary>
//   The server changed.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Core
{
    /// <summary>
    /// The server changed.
    /// </summary>
    public class ServerChanged
    {
        /// <summary>
        /// Gets or sets the server.
        /// </summary>
        public ServerItemModel Server { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerChanged"/> class.
        /// </summary>
        /// <param name="server">
        /// The server.
        /// </param>
        public ServerChanged(ServerItemModel server)
        {
            this.Server = server;
        }
    }
}