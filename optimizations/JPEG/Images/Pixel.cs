using System;
using System.Linq;

namespace JPEG.Images
{
    public struct Pixel
    {

        private readonly PixelFormat format;

        public Pixel(float firstComponent, float secondComponent, float thirdComponent, PixelFormat pixelFormat)
        {
            if (!new[] { PixelFormat.RGB, PixelFormat.YCbCr }.Contains(pixelFormat))
                throw new FormatException("Unknown pixel format: " + pixelFormat);
            format = pixelFormat;

            this.firstComponent = firstComponent;
            this.secondComponent = secondComponent;
            this.thirdComponent = thirdComponent;
        }


        private readonly float firstComponent;
        private readonly float secondComponent;
        private readonly float thirdComponent;

        public float R => format == PixelFormat.RGB ? firstComponent : (float)((298.082 * firstComponent + 408.583 * Cr) / 256.0 - 222.921);
        public float G => format == PixelFormat.RGB ? secondComponent : (float)((298.082 * Y - 100.291 * Cb - 208.120 * Cr) / 256.0 + 135.576);
        public float B => format == PixelFormat.RGB ? thirdComponent : (float)((298.082 * Y + 516.412 * Cb) / 256.0 - 276.836);

        public float Y => format == PixelFormat.YCbCr ? firstComponent : (float)(16.0 + (65.738 * R + 129.057 * G + 24.064 * B) / 256.0);
        public float Cb => format == PixelFormat.YCbCr ? secondComponent : (float)(128.0 + (-37.945 * R - 74.494 * G + 112.439 * B) / 256.0);
        public float Cr => format == PixelFormat.YCbCr ? thirdComponent : (float)(128.0 + (112.439 * R - 94.154 * G - 18.285 * B) / 256.0);
    }
}