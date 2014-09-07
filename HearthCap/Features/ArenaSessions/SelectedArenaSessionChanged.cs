namespace HearthCap.Features.ArenaSessions
{
    using System;

    public class SelectedArenaSessionChanged
    {
        public object Source { get; protected set; }

        public Guid? Id { get; protected set; }

        public SelectedArenaSessionChanged(object source, Guid? id)
        {
            this.Source = source;
            this.Id = id;
        }
    }
}