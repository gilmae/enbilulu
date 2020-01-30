using System.Collections.Generic;
using System.Linq;


namespace EnbiluluServer
{
    public static class Utils
    {
        public static string Coalesce(params string[] strings)
        {
            foreach (string s in strings)
            {
                if (!string.IsNullOrEmpty(s))
                {
                    return s;
                }


            }
            return string.Empty;
        }

        public static string Coalesce(this string initValue, params string[] strings)
        {
            if (!string.IsNullOrEmpty(initValue))
            {
                return initValue;
            }

            foreach (string s in strings)
            {
                if (!string.IsNullOrEmpty(s))
                {
                    return s;
                }


            }
            return string.Empty;
        }

        public static T MergeLeft<T, K, V>(this T me, params IDictionary<K, V>[] others)
            where T : IDictionary<K, V>, new()
        {
            T newMap = new T();
            foreach (IDictionary<K, V> src in
                (new List<IDictionary<K, V>> { me }).Concat(others))
            {
                // ^-- echk. Not quite there type-system.
                foreach (KeyValuePair<K, V> p in src)
                {
                    newMap[p.Key] = p.Value;
                }
            }
            return newMap;
        }
    }
}
