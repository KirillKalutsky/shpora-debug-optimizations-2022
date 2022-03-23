using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace JPEG.Images
{
    class Matrix
    {
        private readonly Pixel[] Pixels;
        public readonly int Height;
        public readonly int Width;

        public Matrix(int height, int width)
        {
            if (height % 8 != 0)
            {
                height += 8 - height % 8;
            }
            if (width % 8 != 0)
            {
                width += 8 - width % 8;
            }

            Height = height;
            Width = width;

            Pixels = new Pixel[height * width];
            for (var i = 0; i < height; ++i)
                for (var j = 0; j < width; ++j)
                    Pixels[i * width + j] = new Pixel(0, 0, 0, PixelFormat.RGB);
        }



        public static unsafe explicit operator Matrix(Bitmap bmp)
        {
            var height = bmp.Height;
            var width = bmp.Width;
            var matrix = new Matrix(height, width);

            var data = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            int bytesPerPixel = 3;

            int stride = data.Stride;

            for (var j = 0; j < height; j++)
            {
                byte* row = (byte*)data.Scan0 + j * stride;
                for (var i = 0; i < width; i++)
                {
                    var index = i * bytesPerPixel;
                    var pixel = Color.FromArgb(row[index], row[index + 1], row[index + 2]);
                    matrix[j, i] = new Pixel(pixel.R, pixel.G, pixel.B, PixelFormat.RGB);
                }
            }

            bmp.UnlockBits(data);

            return matrix;
        }

        public static unsafe explicit operator Bitmap(Matrix matrix)
        {
            var bmp = new Bitmap(matrix.Width, matrix.Height);

            var data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            int bytesPerPixel = 3;

            int stride = data.Stride;

            for (var j = 0; j < matrix.Height; j++)
            {
                byte* row = (byte*)data.Scan0 + j * stride;
                for (var i = 0; i < matrix.Width; i++)
                {
                    var pixel = matrix[j , i];
                    var index = i * bytesPerPixel;
                    row[index] = (byte)ToByte(pixel.R);
                    row[index + 1] = (byte)ToByte(pixel.G);
                    row[index + 2] = (byte)ToByte(pixel.B);
                }
            }

            bmp.UnlockBits(data);

            return bmp;
        }

        /* public static explicit operator Matrix(Bitmap bmp)
         {
             var height = bmp.Height - bmp.Height % 8;
             var width = bmp.Width - bmp.Width % 8;
             var matrix = new Matrix(height, width);

             for (var j = 0; j < height; j++)
             {
                 for (var i = 0; i < width; i++)
                 {
                     var pixel = bmp.GetPixel(i, j);
                     matrix[j, i] = new Pixel(pixel.R, pixel.G, pixel.B, PixelFormat.RGB);
                 }
             }

             return matrix;
         }*/

        /*public static explicit operator Bitmap(Matrix matrix)
        {
            var bmp = new Bitmap(matrix.Width, matrix.Height);

            for (var j = 0; j < bmp.Height; j++)
            {
                for (var i = 0; i < bmp.Width; i++)
                {
                    var pixel = matrix[j, i];
                    bmp.SetPixel(i, j, Color.FromArgb(ToByte(pixel.R), ToByte(pixel.G), ToByte(pixel.B)));
                }
            }

            return bmp;
        }*/

        public Pixel this[int index1, int index2]
        {

            get
            {
                try
                {
                    return Pixels[index1 * Width + index2];
                }
                catch (IndexOutOfRangeException exception)
                {
                    throw exception;
                }
            }

            set
            {
                try
                {
                    Pixels[index1 * Width + index2] = value;
                }
                catch (IndexOutOfRangeException exception)
                {
                    throw exception;
                }
            }
        }


        public static int ToByte(double d)
        {
            var val = (int)d;
            if (val > byte.MaxValue)
                return byte.MaxValue;
            if (val < byte.MinValue)
                return byte.MinValue;
            return val;
        }
    }
}