using System;
using System.ComponentModel.DataAnnotations;

namespace HearthCap.Data
{
    public class ArenaDeckImage
    {
        public ArenaDeckImage()
        {
            Id = Guid.NewGuid();
            Created = DateTime.Now;
            Modified = DateTime.Now;
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
