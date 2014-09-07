namespace HearthCap.Data
{
    using System;
    using System.Data.Entity.ModelConfiguration.Conventions;

    public class DateTimeConvention : Convention
    {
        public DateTimeConvention()
        {
            this.Properties<DateTime>()
                .Configure(c => c.HasColumnType("datetime"));
        }
    }
}