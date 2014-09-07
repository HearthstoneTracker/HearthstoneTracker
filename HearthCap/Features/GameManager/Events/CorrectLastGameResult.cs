namespace HearthCap.Features.GameManager.Events
{
    using System;

    public class CorrectLastGameResult
    {
        public CorrectLastGameResult(Guid id)
        {
            this.Id = id;
        }

        public Guid Id { get; set; }

        public bool? Won { get; set; }
    }
}