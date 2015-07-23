namespace HearthCap.Core.GameCapture.HS.Events
{
    using System.Drawing;

    public class DeckScreenshotTaken
    {
        public Bitmap Image { get; protected set; }

        public DeckScreenshotTaken(Bitmap image)
        {
            this.Image = image;
        }
    }
}