using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace PHash
{
    /// <summary>Generate and match perceptual hashes from images.</summary>
    public abstract class PerceptualHash : IPerceptualHash
    {
        /// <summary>Calculates the hamming distance between two hashes.</summary>
        /// <param name="a">First hash.</param>
        /// <param name="b">Second hash.</param>
        /// <returns>The <see cref="int" />.</returns>
        public static int HammingDistance(ulong hash1, ulong hash2)
        {
            var x = hash1 ^ hash2;
            const ulong m1 = 0x5555555555555555UL;
            const ulong m2 = 0x3333333333333333UL;
            const ulong h01 = 0x0101010101010101UL;
            const ulong m4 = 0x0f0f0f0f0f0f0f0fUL;
            x -= (x >> 1) & m1;
            x = (x & m2) + ((x >> 2) & m2);
            x = (x + (x >> 4)) & m4;
            return (int)((x * h01) >> 56);
        }

        /// <summary>Find best matching hash in a given set.</summary>
        /// <param name="hash">The hash to compare.</param>
        /// <param name="set">Set of hashes to match against.</param>
        /// <returns>The <see cref="CompareResult" />.</returns>
        public static CompareResult FindBest(ulong hash, IList<ulong> set)
        {
            var minIndex = -1;
            var minDist = sizeof (ulong) * 8;
            for (var i = 0; i < set.Count; i++)
            {
                var dist = HammingDistance(hash, set[i]);
                if (dist < minDist)
                {
                    minDist = dist;
                    minIndex = i;
                }
            }

            return new CompareResult(minIndex, minDist);
        }

        /// <summary>Find n best matches.</summary>
        /// <param name="n">Number of matches to return</param>
        /// <param name="hash">The hash to compare.</param>
        /// <param name="set">Set of hashes to match against.</param>
        /// <returns>Returns <see cref="IEnumerable{T}" /> of <see cref="CompareResult" /> with n best matches</returns>
        public static IEnumerable<CompareResult> FindNBest(int n, ulong hash, IList<ulong> set)
        {
            if (n == 1)
            {
                return new List<CompareResult> { FindBest(hash, set) };
            }

            var nbestDistances = new List<int>();
            var nbestObjectIndexes = new List<int>();

            for (var i = 0; i < set.Count; i++)
            {
                var dist = HammingDistance(hash, set[i]);
                for (var j = 0; j < nbestDistances.Count; j++)
                {
                    if (dist < nbestDistances[j])
                    {
                        for (var k = n - 1; k > j; k--)
                        {
                            nbestDistances[k] = nbestDistances[k - 1];
                            nbestObjectIndexes[k] = nbestObjectIndexes[k - 1];
                        }

                        nbestDistances[j] = dist;
                        nbestObjectIndexes[j] = i;
                        break;
                    }
                }
            }

            var nbestObjects = new List<CompareResult>();
            for (var i = 0; i < n; i++)
            {
                nbestObjects.Add(new CompareResult(nbestDistances[i], nbestObjectIndexes[i]));
            }

            return nbestObjects;
        }

        /// <summary>Create hash from a <see cref="Bitmap" />.</summary>
        /// <param name="image">Bitmap to create a hash from.</param>
        /// <returns>The hash of type <see cref="ulong" />.</returns>
        public abstract ulong Create(BitmapData image);

        public virtual ulong Create(Bitmap image)
        {
            var data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            var result = Create(data);
            image.UnlockBits(data);
            return result;
        }

        /// <summary>Hash compare result.</summary>
        public class CompareResult
        {
            /// <summary>Initializes a new instance of the <see cref="CompareResult" /> class.</summary>
            /// <param name="index">The index.</param>
            /// <param name="distance">The distance.</param>
            public CompareResult(int index, int distance)
            {
                Index = index;
                Distance = distance;
            }

            /// <summary>Gets or sets the index of the item in the original list.</summary>
            public int Index { get; protected set; }

            /// <summary>Gets or sets the hamming distance result.</summary>
            public int Distance { get; protected set; }
        }
    }
}
