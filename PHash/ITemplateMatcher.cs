using System.Drawing;

namespace PHash
{
    public interface ITemplateMatcher
    {
        float IsMatch(Bitmap source, Bitmap template, float threshold = 0.90f);
    }
}
