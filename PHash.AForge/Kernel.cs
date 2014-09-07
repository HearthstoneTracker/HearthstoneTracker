namespace PHash.AForge
{
    public static class Kernel
    {
        public static T[,] Create<T>(int width, int height, T value)
        {
            var m = new T[width, height];
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    m[x, y] = value;
                }
            }

            return m;
        }
    }
}