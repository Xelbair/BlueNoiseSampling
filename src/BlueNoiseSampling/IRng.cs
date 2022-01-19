using System;
using System.Collections.Generic;
using System.Text;

namespace BlueNoiseSampling
{
    public interface IRng
    {
        /// <summary>
        /// returns a random value between 0 and <paramref name="maxvalue"/>
        /// </summary>
        /// <param name="maxvalue"></param>
        /// <returns></returns>
        uint Next(uint maxvalue);

        /// <summary>
        /// pick a random element from an array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        T PickRandomElement<T>(T[] source)
        {
            var val = Next((uint)(source.Length-1));
            return source[val%source.Length];
        }

        /// <summary>
        /// Shuffle array in place
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        T[] Shuffle<T>(T[] source)
        {
            // basic fisher-yates shuffle
            for (uint i = (uint)source.Length-1; i>=1; i--)
            {
                var j = Next(i + 1);
                T swap = source[i];
                source[i] = source[j];
                source[j] = swap;
            }
            return source;
        }
    }
}
