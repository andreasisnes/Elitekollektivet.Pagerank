using System;
using System.Linq;
using System.Threading.Tasks;

namespace Isnes.Pagerank.Extensions
{
    public static class JaggedArrayExtension
    {
        public static T[,] ToMultidimensional<T>(this T[][] source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            T[,] result = new T[source.Length, source[0].Length];
            Parallel.ForEach(Enumerable.Range(0, source.Length), i =>
            {
                for (int j = 0; j < source[0].Length; j++)
                {
                    result[i, j] = source[i][j];
                }
            });

            return result;
        }
    }
}