// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DctMatrix.cs" company="">
//   
// </copyright>
// <summary>
//   The dct matrix.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PHash.AForge
{
    using System;
    using System.Collections.Generic;

    using MathNet.Numerics;
    using MathNet.Numerics.LinearAlgebra;
    using MathNet.Numerics.LinearAlgebra.Double;

    /// <summary>
    /// The dct matrix.
    /// </summary>
    public static class DctMatrix
    {
        /// <summary>
        /// The dct matrixes.
        /// </summary>
        private static readonly IDictionary<int, Matrix<double>> dctMatrixes = new Dictionary<int, Matrix<double>>();

        /// <summary>
        /// Initializes static members of the <see cref="DctMatrix"/> class.
        /// </summary>
        static DctMatrix()
        {
            dctMatrixes[8] = CreateDCTMatrix(8);
            dctMatrixes[32] = CreateDCTMatrix(32);

            Control.CheckDistributionParameters = false;

            // MathNet.Numerics.Control.ParallelizeOrder = 32;
            // MathNet.Numerics.Control.UseManaged();
            Control.UseSingleThread();
        }

        /// <summary>
        /// The create dct matrix.
        /// </summary>
        /// <param name="n">
        /// The n.
        /// </param>
        /// <returns>
        /// The <see cref="Matrix"/>.
        /// </returns>
        public static Matrix<double> CreateDCTMatrix(int n)
        {
            if (dctMatrixes.ContainsKey(n))
            {
                return dctMatrixes[n];
            }

            var matrix = new DenseMatrix(n);
            var val = 1d / Math.Sqrt(n);
            for (int i = 0; i < n; i++)
            {
                matrix[0, i] = val;
            }

            var sqrt2 = Math.Sqrt(2d / n);
            for (int x = 0; x < n; x++)
            {
                for (int y = 1; y < n; y++)
                {
                    matrix[x, y] = sqrt2 * Math.Cos((Math.PI / 2 / n) * y * (2 * x + 1));
                }
            }

            dctMatrixes[n] = matrix;
            return matrix;
        }

        /// <summary>
        /// The fast dct.
        /// </summary>
        /// <param name="complex">
        /// The complex.
        /// </param>
        /// <returns>
        /// The <see cref="Matrix"/>.
        /// </returns>
        public static Matrix<double> FastDCT(Matrix<double> complex)
        {
            var n = complex.ColumnCount;
            var kernel = CreateDCTMatrix(n);
            return kernel.Multiply(complex).TransposeAndMultiply(kernel);
        }
    }
}