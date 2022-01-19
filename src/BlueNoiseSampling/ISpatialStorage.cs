using System.Collections.Generic;

namespace BlueNoiseSampling
{
    public interface ISpatialStorage
    {
        /// <summary>
        /// Adds a single point to the collection
        /// </summary>
        /// <param name="point"></param>
        public void Add(IPoint point);

        /// <summary>
        /// Adds a collection of points to storage
        /// </summary>
        /// <param name="points"></param>
        public void AddRange(IEnumerable<IPoint> points);

        /// <summary>
        /// Computes manhattan distance between <paramref name="point"/> and the closest point in <see cref="ISpatialStorage"/>
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public double ManhattanDistance(IPoint point);

        /// <summary>
        /// Computes euclidean distance between <paramref name="point"/> and the closest point in <see cref="ISpatialStorage"/>
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public double Distance(IPoint point);
        /// <summary>
        /// Squared distance for purposes of comparision
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public double DistanceSquared(IPoint point);

        public List<IPoint> ToList();

        public void Clear();
    }
}