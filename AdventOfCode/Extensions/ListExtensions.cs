using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Extensions
{
    public static class ListExtensions
    {
        public static List<T> GetByBitMask<T>(this List<T> list, int mask)
        {
            var result = new List<T>();
            for (var i = 0; i < list.Count(); i++)
            {
                if (((1 << i) & mask) > 0)
                {
                    result.Add(list[i]);
                }
            }

            return result;
        }
    }
}
