namespace HearthCap.Core.GameCapture.HS.Events
{
    using System.Drawing;

    public class ArenaDeckScreenshotTaken
    {
        public Bitmap Image { get; protected set; }

        public ArenaDeckScreenshotTaken(Bitmap image)
        {
            this.Image = image;
        }
    }
}