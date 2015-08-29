using System;

namespace HearthCap.Features.ArenaSessions
{
    public class SelectedArenaSessionChanged
    {
        public object Source { get; protected set; }

        public Guid? Id { get; protected set; }

        public SelectedArenaSessionChanged(object source, Guid? id)
        {
            Source = source;
            Id = id;
        }
    }
}
