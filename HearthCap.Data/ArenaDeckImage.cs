namespace HearthCap.Data
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ArenaDeckImage
    {
        public ArenaDeckImage()
        {
            this.Id = Guid.NewGuid();
            this.Created = DateTime.Now;
            this.Modified = DateTime.Now;
        }

        public Guid Id { get; set; }

        [MaxLength]
        public byte[] Image { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        [Timestamp]
        public byte[] Version { get; protected set; }
    }
}