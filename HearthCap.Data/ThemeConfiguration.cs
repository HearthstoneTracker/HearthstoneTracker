using System;

namespace HearthCap.Data
{
    public class ThemeConfiguration : IEntityWithId<Guid>
    {
        public Guid Id { get; protected set; }

        public string Name { get; set; }

        public string Accent { get; set; }

        public string Theme { get; set; }

        public ThemeConfiguration()
        {
            Id = Guid.NewGuid();
        }
    }
}
