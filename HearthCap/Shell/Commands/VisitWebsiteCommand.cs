namespace HearthCap.Shell.Commands
{
    public class VisitWebsiteCommand
    {
        private static string defaultWebsite = "http://www.google.com";

        /// <summary>Initializes a new instance of the <see cref="VisitWebsiteCommand"/> class. 
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        /// <param name="website">The website.</param>
        public VisitWebsiteCommand(string website = null)
        {
            this.Website = website ?? DefaultWebsite;
        }

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

        public string Website { get; set; }
    }
}