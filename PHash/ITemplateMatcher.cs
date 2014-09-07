namespace PHash
{
    using System.Drawing;

    public interface ITemplateMatcher
    {
        float IsMatch(Bitmap source, Bitmap template, float threshold = 0.90f);
    }
}