using System;

namespace HearthCap.Features.GameManager.Events
{
    public class CorrectLastGameResult
    {
        public CorrectLastGameResult(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }

        public bool? Won { get; set; }
    }
}
