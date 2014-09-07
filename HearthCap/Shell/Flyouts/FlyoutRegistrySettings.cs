namespace HearthCap.Shell.Flyouts
{
    using System;

    using HearthCap.Shell.UserPreferences;

    using MahApps.Metro.Controls;

    public class FlyoutRegistrySettings : RegistrySettings
    {
        public FlyoutRegistrySettings()
            : base(@"Software\HearthstoneTracker\Flyouts")
        {
        }

        public Position GetPosition(Type type, Position defaultPosition = Position.Right)
        {
            return this.GetOrCreate(type.Name, defaultPosition);
        }

        public Position GetPosition<TType>(Position defaultPosition = Position.Right)
        {
            return this.GetPosition(typeof(TType), defaultPosition);
        }

        public void SetPosition(Type type, Position position)
        {
            this.SetValue(type.Name, position);
        }

        public void SetPosition<TType>(Position position)
        {
            this.SetPosition(typeof(TType), position);
        }
    }
}