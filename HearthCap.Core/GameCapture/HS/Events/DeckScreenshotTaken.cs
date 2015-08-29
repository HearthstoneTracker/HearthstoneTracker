using System.Drawing;

namespace HearthCap.Core.GameCapture.HS.Events
{
    public class DeckScreenshotTaken
    {
        public Bitmap Image { get; protected set; }

        public DeckScreenshotTaken(Bitmap image)
        {
            Image = image;
        }
    }
}
