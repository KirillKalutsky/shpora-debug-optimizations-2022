using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using JPEG.Utilities;

namespace JPEG
{
	public class DCT
	{
        public static float[,] DCTTransform(float[,] matrix)
        {
            var height = matrix.GetLength(0);
            var width = matrix.GetLength(1);
            var dct = new float[width, width];

            var partitioner = Partitioner.Create(0, width, width / Environment.ProcessorCount);

            Parallel.ForEach(partitioner, partition =>
            {
                ParallelTransform(dct, partition.Item1, partition.Item2, matrix, width, height);
            });

            return dct;
        }

        private static void ParallelTransform(float[,] result, int firstIndexStart,
            int firstIndexEnd, float[,] matrix, int width, int height)
        {
            for (var i = firstIndexStart; i < firstIndexEnd; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    var ci = Math.Sqrt(2) / Math.Sqrt(width);
                    var cj = Math.Sqrt(2) / Math.Sqrt(height);

                    if (i == 0)
                        ci = 1 / Math.Sqrt(width);
                    
                    if (j == 0)
                        cj = 1 / Math.Sqrt(height);

                    var sum = 0.0;
                    for (var k = 0; k < width; k++)
                    {
                        for (var l = 0; l < height; l++)
                        {
                            sum += matrix[k, l] *
                                   Math.Cos((2 * k + 1) * i * Math.PI / (2 * width)) *
                                   Math.Cos((2 * l + 1) * j * Math.PI / (2 * height));
                        }
                    }
                    result[i, j] = (float)(ci * cj * sum);
                }
            }
        }

        public static float[,] IDCTTransform(float[,] matrix)
        {
            var height = matrix.GetLength(0);
            var width = matrix.GetLength(1);
            var dct = new float[width,width];

            var partitioner = Partitioner.Create(0, height,  height/ Environment.ProcessorCount);

            Parallel.ForEach(partitioner, partition =>
            {
                ParallelITransform(dct, partition.Item1, partition.Item2, matrix, height, width);
            });

            return dct;
        }

        private static void ParallelITransform(float[,] result, int startIndex,
            int endIndex, float[,] matrix, int height, int width)
        {
            for (var k = startIndex; k < endIndex; k++)
            {
                for (var l = 0; l < width; l++)
                {
                    var sum = 0.0;
                    for (var i = 0; i < height; i++)
                    {
                        for (var j = 0; j < width; j++)
                        {
                            var ci = Math.Sqrt(2) / Math.Sqrt(height);
                            var cj = Math.Sqrt(2) / Math.Sqrt(width);

                            if (i == 0)
                                ci = 1 / Math.Sqrt(height);

                            if (j == 0)
                                cj = 1 / Math.Sqrt(width);

                            sum += ci * cj * matrix[i, j] *
                                   Math.Cos((2 * k + 1) * i * Math.PI / (2 * height)) *
                                   Math.Cos((2 * l + 1) * j * Math.PI / (2 * width));
                        }
                    }
                    result[k, l] = (float)sum;
                }
            }
        }

        public static double[,] DCT2D(double[,] input)
		{
			var height = input.GetLength(0);
			var width = input.GetLength(1);
			var coeffs = new double[width, height];

			MathEx.LoopByTwoVariables(
				0, width,
				0, height,
				(u, v) =>
				{
					var sum = MathEx
						.SumByTwoVariables(
							0, width,
							0, height,
							(x, y) => BasisFunction(input[x, y], u, v, x, y, height, width));

					coeffs[u, v] = sum * Beta(height, width) * Alpha(u) * Alpha(v);
				});
			
			return coeffs;
		}

		public static void IDCT2D(double[,] coeffs, double[,] output)
		{
			for(var x = 0; x < coeffs.GetLength(1); x++)
			{
				for(var y = 0; y < coeffs.GetLength(0); y++)
				{
					var sum = MathEx
						.SumByTwoVariables(
							0, coeffs.GetLength(1),
							0, coeffs.GetLength(0),
							(u, v) => BasisFunction(coeffs[u, v], u, v, x, y, coeffs.GetLength(0), coeffs.GetLength(1)) * Alpha(u) * Alpha(v));

					output[x, y] = sum * Beta(coeffs.GetLength(0), coeffs.GetLength(1));
				}
			}
		}

		public static double BasisFunction(double a, double u, double v, double x, double y, int height, int width)
		{
			var b = Math.Cos(((2d * x + 1d) * u * Math.PI) / (2 * width));
			var c = Math.Cos(((2d * y + 1d) * v * Math.PI) / (2 * height));

			return a * b * c;
		}

		private static double Alpha(int u)
		{
			if(u == 0)
				return 1 / Math.Sqrt(2);
			return 1;
		}

		private static double Beta(int height, int width)
		{
			return 1d / width + 1d / height;
		}
	}
}