using System;
using HearthCap.Shell.UserPreferences;
using MahApps.Metro.Controls;

namespace HearthCap.Shell.Flyouts
{
    public class FlyoutRegistrySettings : RegistrySettings
    {
        public FlyoutRegistrySettings()
            : base(@"Software\HearthstoneTracker\Flyouts")
        {
        }

        public Position GetPosition(Type type, Position defaultPosition = Position.Right)
        {
            return GetOrCreate(type.Name, defaultPosition);
        }

        public Position GetPosition<TType>(Position defaultPosition = Position.Right)
        {
            return GetPosition(typeof(TType), defaultPosition);
        }

        public void SetPosition(Type type, Position position)
        {
            SetValue(type.Name, position);
        }

        public void SetPosition<TType>(Position position)
        {
            SetPosition(typeof(TType), position);
        }
    }
}
