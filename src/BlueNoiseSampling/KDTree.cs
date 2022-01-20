using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace BlueNoiseSampling
{
    public sealed class KDTree : ISpatialStorage
    {
        private class KDTreeNode
        {
            private class BoundingBox
            {
                readonly float _xMin;
                readonly float _xMax;
                readonly float _yMin;
                readonly float _yMax;

                public BoundingBox(float xMin, float xMax, float yMin, float yMax)
                {
                    _xMin=xMin;
                    _xMax=xMax;
                    _yMin=yMin;
                    _yMax=yMax;
                }

                public BoundingBox(IPoint center, float radius)
                {
                    _xMin = center.X-radius;
                    _xMax = center.X+radius;

                    _yMin = center.Y-radius;
                    _yMax = center.Y+radius;
                }

                public float XMin => _xMin;
                public float XMax => _xMax;
                public float YMin => _yMin;
                public float YMax => _yMax;

                public (BoundingBox lesser, BoundingBox higher) SplitOnY(float yAxis)
                {
                    var lesser = new BoundingBox(_xMin, _xMax, _yMin, yAxis);
                    var higher = new BoundingBox(_xMin, _xMax, yAxis, _yMax);
                    return (lesser, higher);
                }

                public (BoundingBox lesser, BoundingBox higher) SplitOnX(float xAxis)
                {
                    var lesser = new BoundingBox(_xMin, xAxis, _yMin, _yMax);
                    var higher = new BoundingBox(xAxis, _xMax, _yMin, _yMax);
                    return (lesser, higher);
                }

                public bool IsValid()
                {
                    return _xMin < _xMax && _yMin < _yMax;
                }

                public bool Within(IPoint point)
                {
                    return point.X >= _xMin && point.X <= _xMax && point.Y >= _yMin && point.Y <= _yMax;
                }

                public bool WithinX(IPoint point)
                {
                    return point.X >= _xMin && point.X <= _xMax;
                }

                public bool WithinY(IPoint point)
                {
                    return point.Y >= _yMin && point.Y <= _yMax;
                }

                public bool Intersects(BoundingBox other)
                {
                    return !(other.XMin > _xMax
                          || other.XMax < _xMin
                          || other.XMax > _yMin
                          || other.YMin < _xMax);
                }
            }


            IPoint _point;
            int _k;

            KDTreeNode _lesserBranch;
            KDTreeNode _higherBranch;


            public KDTreeNode(IPoint data, int k)
            {
                _point = data;
                //mod 2 becasuse we are deling with 2d data
                _k = k%2;
            }

            public void Add(IPoint point)
            {
                if (point.Coordinates.GetCoordinate(_k) > _point.Coordinates.GetCoordinate(_k))
                {
                    if (_higherBranch is null) _higherBranch = new KDTreeNode(point, _k+1);
                    else _higherBranch.Add(point);
                }
                else
                {
                    if (_lesserBranch is null) _lesserBranch = new KDTreeNode(point, _k+1);
                    else _lesserBranch.Add(point);
                }
            }

            /// <summary>
            /// Method that calculates the minimum distance from the whole tree
            /// </summary>
            /// <param name="point"></param>
            /// <returns></returns>
            public float Distance(IPoint point)
            {
                var currentDistance = Vector3.Distance(_point.Coordinates, point.Coordinates);
                var boundingBox = new BoundingBox(point, currentDistance);
                return Distance(point, boundingBox);
            }

            private float Distance(IPoint point, BoundingBox searchEnvelope)
            {
                var splitOnX = _k%2==0;
                var currentAxis = splitOnX ? _point.X : _point.Y;
                var boxes = splitOnX ? searchEnvelope.SplitOnY(currentAxis) : searchEnvelope.SplitOnX(currentAxis);
                var currentDistance = Vector3.Distance(_point.Coordinates, point.Coordinates);

                //we need to check if one of our halves is 100% outside of search envelope to prune it
                //it will be invalid if our current axis is outside of envelope
                var lesserDistance = boxes.lesser.IsValid() ? _lesserBranch?.Distance(point, boxes.lesser)??float.MaxValue : float.MaxValue;
                var higherDistance = boxes.higher.IsValid() ? _higherBranch?.Distance(point, boxes.higher)??float.MaxValue : float.MaxValue;
                return Math.Min(currentDistance, Math.Min(lesserDistance, higherDistance));
            }

            public float DistanceSquared(IPoint point)
            {
                var currentDistance = Vector3.Distance(_point.Coordinates, point.Coordinates);
                var boundingBox = new BoundingBox(point, currentDistance);
                return DistanceSquared(point, boundingBox);
            }

            private float DistanceSquared(IPoint point, BoundingBox searchEnvelope)
            {
                var splitOnX = _k%2==0;
                var currentAxis = splitOnX ? _point.X : _point.Y;
                var boxes = splitOnX ? searchEnvelope.SplitOnY(currentAxis) : searchEnvelope.SplitOnX(currentAxis);
                var currentDistance = Vector3.DistanceSquared(_point.Coordinates, point.Coordinates);

                //we need to check if one of our halves is 100% outside of search envelope to prune it
                //it will be invalid if our current axis is outside of envelope
                var lesserDistance = boxes.lesser.IsValid() ? _lesserBranch?.DistanceSquared(point, boxes.lesser)??float.MaxValue : float.MaxValue;
                var higherDistance = boxes.higher.IsValid() ? _higherBranch?.DistanceSquared(point, boxes.higher)??float.MaxValue : float.MaxValue;
                return Math.Min(currentDistance, Math.Min(lesserDistance, higherDistance));
            }

            public float ManhattanDistance(IPoint point)
            {
                var currentDistance = Vector3.Distance(_point.Coordinates, point.Coordinates);
                var boundingBox = new BoundingBox(point, currentDistance);
                return ManhattanDistance(point, boundingBox);
            }

            private float ManhattanDistance(IPoint point, BoundingBox searchEnvelope)
            {
                var splitOnX = _k%2==0;
                var currentAxis = splitOnX ? _point.X : _point.Y;
                var boxes = splitOnX ? searchEnvelope.SplitOnY(currentAxis) : searchEnvelope.SplitOnX(currentAxis);
                var currentDistance = point.ManhattanDistance(_point); 

                //we need to check if one of our halves is 100% outside of search envelope to prune it
                //it will be invalid if our current axis is outside of envelope
                var lesserDistance = boxes.lesser.IsValid() ? _lesserBranch?.ManhattanDistance(point, boxes.lesser)??float.MaxValue : float.MaxValue;
                var higherDistance = boxes.higher.IsValid() ? _higherBranch?.ManhattanDistance(point, boxes.higher)??float.MaxValue : float.MaxValue;
                return Math.Min(currentDistance, Math.Min(lesserDistance, higherDistance));
            }

            public List<IPoint> ToList()
            {
                var list = new List<IPoint>();
                ToList(list);
                return list;
            }

            private void ToList(List<IPoint> list)
            {
                list.Add(_point);
                _lesserBranch?.ToList(list);
                _higherBranch?.ToList(list);
            }
        }

        KDTreeNode _root;

        public void Add(IPoint point)
        {
            if (_root is null)
            {
                _root = new KDTreeNode(point, 0);
            }
            else
            {
                _root.Add(point);
            }
        }

        public void AddRange(IEnumerable<IPoint> points)
        {
            foreach (var point in points)
            {
                Add(point);
            }
        }

        public void Clear()
        {
            _root = null;
        }

        public double Distance(IPoint point) => _root.Distance(point);

        public double DistanceSquared(IPoint point) => _root.DistanceSquared(point);

        public double ManhattanDistance(IPoint point) => _root.ManhattanDistance(point);

        public List<IPoint> ToList() => _root.ToList();
    }
}
