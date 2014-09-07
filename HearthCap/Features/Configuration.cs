namespace HearthCap.Features
{
    using System.ComponentModel.Composition;

    using HearthCap.Shell.Commands;
    using HearthCap.StartUp;

    [Export(typeof(IStartupTask))]
    public class Configuration : IStartupTask
    {
        public void Run()
        {
            VisitWebsiteCommand.DefaultWebsite = "http://hearthstonetracker.com";
        }
    }
}