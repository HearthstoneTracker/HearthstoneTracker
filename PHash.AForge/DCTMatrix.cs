using System;
using System.Collections.Generic;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace PHash.AForge
{
    public static class DctMatrix
    {
        private static readonly IDictionary<int, Matrix<double>> dctMatrixes = new Dictionary<int, Matrix<double>>();

        static DctMatrix()
        {
            dctMatrixes[8] = CreateDCTMatrix(8);
            dctMatrixes[32] = CreateDCTMatrix(32);

            Control.CheckDistributionParameters = false;
            // MathNet.Numerics.Control.ParallelizeOrder = 32;
            // MathNet.Numerics.Control.UseManaged();
            Control.UseSingleThread();
        }

        public static Matrix<double> CreateDCTMatrix(int n)
        {
            if (dctMatrixes.ContainsKey(n))
            {
                return dctMatrixes[n];
            }

            var matrix = new DenseMatrix(n);
            var val = 1d / Math.Sqrt(n);
            for (var i = 0; i < n; i++)
            {
                matrix[0, i] = val;
            }

            var sqrt2 = Math.Sqrt(2d / n);
            for (var x = 0; x < n; x++)
            {
                for (var y = 1; y < n; y++)
                {
                    matrix[x, y] = sqrt2 * Math.Cos((Math.PI / 2 / n) * y * (2 * x + 1));
                }
            }

            dctMatrixes[n] = matrix;
            return matrix;
        }

        public static Matrix<double> FastDCT(Matrix<double> complex)
        {
            var n = complex.ColumnCount;
            var kernel = CreateDCTMatrix(n);
            return kernel.Multiply(complex).TransposeAndMultiply(kernel);
        }
    }
}
