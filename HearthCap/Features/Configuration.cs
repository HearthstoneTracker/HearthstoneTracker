using System.ComponentModel.Composition;
using HearthCap.Shell.Commands;
using HearthCap.StartUp;

namespace HearthCap.Features
{
    [Export(typeof(IStartupTask))]
    public class Configuration : IStartupTask
    {
        public void Run()
        {
            VisitWebsiteCommand.DefaultWebsite = "http://hearthstonetracker.com";
        }
    }
}
