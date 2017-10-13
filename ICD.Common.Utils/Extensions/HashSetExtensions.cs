#if STANDARD
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;

namespace ICD.Common.Utils.Extensions
{
    public static class HashSetExtensions
    {
        [PublicAPI]
        public static HashSet<T> Subtract<T>(this HashSet<T> extends, IEnumerable<T> set)
        {
            HashSet<T> subtractSet = new HashSet<T>(extends);

            if (set == null)
                return subtractSet;

            foreach (T item in set)
                subtractSet.Remove(item);

            return subtractSet;
        }

        [PublicAPI]
        public static bool IsSubsetOf<T>(this HashSet<T> extends, HashSet<T> set)
        {
            HashSet<T> setToCompare = set ?? new HashSet<T>();
            return extends.All(setToCompare.Contains);
        }

        [PublicAPI]
        public static HashSet<T> Intersection<T>(this HashSet<T> extends, HashSet<T> set)
        {
            HashSet<T> intersectionSet = new HashSet<T>();

            if (set == null)
                return intersectionSet;

            foreach (T item in extends.Where(set.Contains))
                intersectionSet.Add(item);

            foreach (T item in set.Where(extends.Contains))
                intersectionSet.Add(item);

            return intersectionSet;
        }

        /// <summary>
        /// Returns items that are not common between both sets.
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        [PublicAPI]
        public static HashSet<T> NonIntersection<T>(this HashSet<T> extends, HashSet<T> set)
        {
            return new HashSet<T>(extends.Subtract(set).Union(set.Subtract(extends)));
        }

        [PublicAPI]
        public static bool IsProperSubsetOf<T>(this HashSet<T> extends, HashSet<T> set)
        {
            HashSet<T> setToCompare = set ?? new HashSet<T>();

            // Is a proper subset if A is a subset of B and A != B
            return (extends.IsSubsetOf(setToCompare) && !setToCompare.IsSubsetOf(extends));
        }

        [PublicAPI]
        public static bool IsSupersetOf<T>(this HashSet<T> extends, HashSet<T> set)
        {
            HashSet<T> setToCompare = set ?? new HashSet<T>();
            return setToCompare.IsSubsetOf(extends);
        }

        [PublicAPI]
        public static bool IsProperSupersetOf<T>(this HashSet<T> extends, HashSet<T> set)
        {
            HashSet<T> setToCompare = set ?? new HashSet<T>();

            // B is a proper superset of A if B is a superset of A and A != B
            return (extends.IsSupersetOf(setToCompare) && !setToCompare.IsSupersetOf(extends));
        }
    }
}
#endif