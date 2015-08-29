using System;

namespace HearthCap.Features.AutoUpdate
{
    public class UpdateInfo
    {
        public string Title { get; set; }

        public Version Version { get; set; }

        public string Updater { get; set; }

        public string File { get; set; }
    }
}