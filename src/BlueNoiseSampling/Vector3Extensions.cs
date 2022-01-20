using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace BlueNoiseSampling
{
    internal static class Vector3Extensions
    {

        public static float GetCoordinate(this Vector3  vec, int index)
        {
            switch (index%3)
            {
                case 0: return vec.X;
                case 1: return vec.Y;
                case 2: return vec.Z;
                default:
                    throw new Exception();
            }
        }
    }
}
