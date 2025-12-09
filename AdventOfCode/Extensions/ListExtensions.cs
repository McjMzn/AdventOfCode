using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public static string Render(this char[,] array, char placeholder)
        {
            var builder = new StringBuilder();
            for (var y = 0; y < array.GetLength(0); y++)
            {
                for (var x = 0; x < array.GetLength(1); x++)
                {
                    var c = array[y, x];
                    builder.Append(char.IsWhiteSpace(c) ? placeholder : c);
                    builder.Append(" ");
                }

                builder.AppendLine();
            }

            return builder.ToString();
        }
    }
}
