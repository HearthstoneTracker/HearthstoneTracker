using System.Drawing;

namespace HearthCap.Core.GameCapture.HS.Events
{
    public class ArenaDeckScreenshotTaken
    {
        public Bitmap Image { get; protected set; }

        public ArenaDeckScreenshotTaken(Bitmap image)
        {
            Image = image;
        }
    }
}
