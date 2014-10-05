// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Configuration.cs" company="">
//   
// </copyright>
// <summary>
//   The configuration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features
{
    using System.ComponentModel.Composition;

    using HearthCap.Shell.Commands;
    using HearthCap.StartUp;

    /// <summary>
    /// The configuration.
    /// </summary>
    [Export(typeof(IStartupTask))]
    public class Configuration : IStartupTask
    {
        /// <summary>
        /// The run.
        /// </summary>
        public void Run()
        {
            VisitWebsiteCommand.DefaultWebsite = "http://hearthstonetracker.com";
        }
    }
}