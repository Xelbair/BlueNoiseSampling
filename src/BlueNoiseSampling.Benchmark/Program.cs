using System;
using System.Collections.Generic;
using System.Drawing;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace BlueNoiseSampling.Benchmark
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<SpatialSTorageBenchmark>();
        }
    }


    public class SpatialSTorageBenchmark
    {
        private const int N = 1000;
        private const uint MAX_CANDIDATES = 10;
        private const double PERCENTAGE = 0.01f;
        private const string TEST_IMAGE = @"test.jpg";


        private readonly PoissonDiskSampler _spatialListSampler;
        private readonly PoissonDiskSampler _kdTreeSampler;
        private readonly List<IPoint<Color>> _data;

        public SpatialSTorageBenchmark()
        {
            using var img = Bitmap.FromFile(TEST_IMAGE);
            using var bmp = new Bitmap(img);
            _data = new List<IPoint<Color>>();
            //simply create a list of colors based on all pixels in image, other implementation could forgo color field, and just read pixels from source image whne writting the output
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    _data.Add(new Point<Color>(bmp.GetPixel(i, j), i, j, 0));
                }
            }

            var rng = new CachingCryptoStableRNG();
            _spatialListSampler = new PoissonDiskSampler(rng,()=>new SpatialList(), MAX_CANDIDATES,(uint)(_data.Count*PERCENTAGE));
            _kdTreeSampler = new PoissonDiskSampler(rng,()=>new KDTree(), MAX_CANDIDATES,(uint)(_data.Count*PERCENTAGE));
        }

        [Benchmark]
        public object BenchmarkSpatialList()
        {
            return _spatialListSampler.Sample(_data.ToArray());
        }
        [Benchmark]
        public object BenchmarkKDTreet()
        {
            return _kdTreeSampler.Sample(_data.ToArray());
        }
    }
}
