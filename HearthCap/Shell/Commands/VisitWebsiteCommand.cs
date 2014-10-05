// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VisitWebsiteCommand.cs" company="">
//   
// </copyright>
// <summary>
//   The visit website command.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Shell.Commands
{
    /// <summary>
    /// The visit website command.
    /// </summary>
    public class VisitWebsiteCommand
    {
        /// <summary>
        /// The default website.
        /// </summary>
        private static string defaultWebsite = "http://www.google.com";

        /// <summary>
        /// Initializes a new instance of the <see cref="VisitWebsiteCommand"/> class. 
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        /// <param name="website">
        /// The website.
        /// </param>
        public VisitWebsiteCommand(string website = null)
        {
            this.Website = website ?? DefaultWebsite;
        }

        /// <summary>
        /// Gets or sets the default website.
        /// </summary>
        public static string DefaultWebsite
        {
            get
            {
                return defaultWebsite;
            }

            set
            {
                defaultWebsite = value;
            }
        }

        /// <summary>
        /// Gets or sets the website.
        /// </summary>
        public string Website { get; set; }
    }
}