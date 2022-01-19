using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlueNoiseSampling
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class PoissonDiskSampler
    {
        IRng _rng;
        ISpatialStorage _mitchellPoints;
        uint _maxCandidates;
        uint _maxPoints;


        public PoissonDiskSampler(IRng rng, Func<ISpatialStorage> StorageConstructor, uint maxCandidates, uint maxPoints)
        {
            _rng=rng;
            _mitchellPoints=StorageConstructor();
            _maxCandidates=maxCandidates;
            _maxPoints=maxPoints;
        }

        /// <summary>
        /// Blue noise approximation sampling via Poisson-Disk sampling
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public IList<IPoint> Sample(IPoint[] points)
        {
            //best case scenario fastpath
            if (_maxCandidates >= points.Length)
            {
                foreach (var item in points)
                {
                    _mitchellPoints.Add(item);
                }
                return _mitchellPoints.ToList();
            }

            double maxDistance; int maxDistIndex;
            //Collection initialization and choosing the first oint
            _mitchellPoints.Clear();
            IPoint[] candidates = new IPoint[_maxCandidates];
            _mitchellPoints.Add(_rng.PickRandomElement(points));
            int selectedPointsCounter = 1;

            do
            {
                //Get Candidates
                for (int i = 0; i < _maxCandidates; i++)
                {
                    candidates[i] = _rng.PickRandomElement(points);
                }
                //Find candidate futherst from the existing points
                maxDistance = 0;
                maxDistIndex = 0;
                for (int i = 0; i < _maxCandidates; i++)
                {
                    var candidate = candidates[i];
                    var distance = _mitchellPoints.DistanceSquared(candidate);
                    if (maxDistance < distance)
                    { maxDistance = distance; maxDistIndex = i; }
                }
                _mitchellPoints.Add(candidates[maxDistIndex]);

                //increment counter and send progress event
                selectedPointsCounter += 1;
            } while (selectedPointsCounter < _maxPoints);

            return _mitchellPoints.ToList();
        }

        public IList<IPoint<T>> Sample<T>(IPoint<T>[] points) => Sample((IPoint[])points).Cast<IPoint<T>>().ToList();
    }
}
