using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace BlueNoiseSampling.Testbed
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args is null || args.Length == 0) args = new string[1] { "test.jpg" };

            string testFile = args[0];
            double percentageOfPoints = 0.10;
            uint maxCandidates = 10;
            using var img = Bitmap.FromFile(testFile);
            using var bmp = new Bitmap(img);

            var points = new List<IPoint<Color>>();
            //simply create a list of colors based on all pixels in image, other implementation could forgo color field, and just read pixels from source image whne writting the output
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    points.Add(new BlueNoiseSampling.Point<Color>(bmp.GetPixel(i,j),i,j,0));
                }
            }

            //Initialize the sampler and sample the image
            var rng = new CachingCryptoStableRNG();
            var sampler = new PoissonDiskSampler(rng,()=>new KDTree(),maxCandidates, (uint)Math.Round(bmp.Width * bmp.Height * percentageOfPoints));
            var result = sampler.Sample(points.ToArray());

            //write the result
            using var outImage = new DirectBitmap(bmp.Width, bmp.Height);

            foreach (var point in result)
            {
                var x = (int)Math.Round(point.X);
                var y = (int)Math.Round(point.Y);
                outImage.SetPixel(x, y, point.Data);
            }
            outImage.Bitmap.Save("result.png", ImageFormat.Png);
        }
    }
}
