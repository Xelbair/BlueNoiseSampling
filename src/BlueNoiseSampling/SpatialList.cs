using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlueNoiseSampling
{
    /// <summary>
    /// Naive <see cref="ISpatialStorage"/> implementation by utilizing collection backed by list
    /// </summary>
    public class SpatialList : ISpatialStorage
    {

        private List<IPoint> _points;

        public SpatialList()
        {
            _points = new List<IPoint>();
        }

        public SpatialList(int capacity)
        {
            _points = new List<IPoint>(capacity);
        }

        /// <inheritdoc/>
        public void Add(IPoint point) => _points.Add(point);
        /// <inheritdoc/>

        public void AddRange(IEnumerable<IPoint> points) => _points.AddRange(points);

        public void Clear() => _points.Clear();

        /// <inheritdoc/>

        public double Distance(IPoint point) => Math.Sqrt(DistanceSquared(point));
        /// <inheritdoc/>
        public double DistanceSquared(IPoint point)
        {
            var result = double.MaxValue;
            foreach (var pt in _points)
            {
                var currentDistance = point.DistanceSquared(pt);
                if (currentDistance < result)
                    result = currentDistance;
            }
            return result;
        }

        /// <inheritdoc/>
        public double ManhattanDistance(IPoint point)
        {
            var result = double.MaxValue;
            foreach (var pt in _points)
            {
                var currentDistance = point.ManhattanDistance(pt);
                if (currentDistance < result)
                    result = currentDistance;
            }
            return result;
        }

        public List<IPoint> ToList() => _points.ToList();
    }
}
