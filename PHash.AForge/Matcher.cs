namespace PHash.AForge
{
    using System.Drawing;

    using global::AForge.Imaging;

    public class Matcher : ITemplateMatcher
    {
        public float IsMatch(Bitmap source, Bitmap template, float threshold = 0.80f)
        {
            var tm = new ExhaustiveTemplateMatching(threshold);
            var matchings = tm.ProcessImage(source, template);
            return matchings.Length > 0 ? matchings[0].Similarity : -1;
        }
    }
}