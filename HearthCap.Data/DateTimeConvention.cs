using System;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace HearthCap.Data
{
    public class DateTimeConvention : Convention
    {
        public DateTimeConvention()
        {
            Properties<DateTime>()
                .Configure(c => c.HasColumnType("datetime"));
        }
    }
}
