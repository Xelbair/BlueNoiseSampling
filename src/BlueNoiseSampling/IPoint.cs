using System.Numerics;

namespace BlueNoiseSampling
{

    /// <summary>
    /// Interface defining a point in 3d space for purpose of sampling
    /// </summary>
    public interface IPoint
    {

        public float X { get;  }
        public float Y { get;  }
        public float Z { get;  }
        public Vector3 Coordinates { get; }
        /// <summary>
        /// Manhatta distance between this <see cref="IPoint"/> and <paramref name="point"/> 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        float ManhattanDistance(IPoint point)
        {
            var delta = Vector3.Subtract(Coordinates, point.Coordinates);
            delta = Vector3.Abs(delta);
            return delta.X+delta.Y+delta.Z;
        }
        /// <summary>
        /// Distance between this <see cref="IPoint"/> and <paramref name="point"/> 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        float Distance(IPoint point) => Vector3.Distance(Coordinates, point.Coordinates);
        /// <summary>
        /// Squared distance between this <see cref="IPoint"/> and <paramref name="point"/> 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        float DistanceSquared(IPoint point) => Vector3.DistanceSquared(Coordinates, point.Coordinates);
    }

    public interface IPoint<T> :IPoint
    {
        public T Data { get; }
    }
}