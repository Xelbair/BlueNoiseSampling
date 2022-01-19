using System.Drawing;
using System.Numerics;

namespace BlueNoiseSampling
{
    public sealed class Point<T> : IPoint<T>
    {
        public Point(T data, float x, float y, float z)
        {
            Data=data;
            Coordinates= new Vector3(x,y,z);
        }

        public T Data { get; }

        public float X => Coordinates.X;

        public float Y => Coordinates.Y;

        public float Z => Coordinates.Z;

        public Vector3 Coordinates { get; }
    }
}