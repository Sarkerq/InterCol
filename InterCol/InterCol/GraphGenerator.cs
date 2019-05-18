using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterCol
{
    public static class GraphGenerator
    {
        public static UndirectedGraph Generate(int verticesCount, double edgePercent)
        {
            var graph = new UndirectedGraph(verticesCount);
            int maxEdgesCount = verticesCount * (verticesCount - 1) / 2;
            int edgesCount = (int)(edgePercent * maxEdgesCount);
            var edgesIndexes = GenerateRandomNumbersWithoutRepeating(edgesCount, maxEdgesCount);
            for (int i = 0, edgeIterator = 0; i < verticesCount; i++)
                for (int j = i + 1; j < verticesCount; j++, edgeIterator++)
                {
                    if (edgesIndexes.Contains(edgeIterator))
                        graph.AddEdge(i, j);
                }
            return graph;
        }

        private static List<int> GenerateRandomNumbersWithoutRepeating(int count, int max)
        {
            var numbers = new List<int>();
            int number;
            var random = new Random();
            for (int i = 0; i < count; i++)
            {
                do
                {
                    number = random.Next(max);
                } while (numbers.Contains(number));
                numbers.Add(number);
            }
            return numbers;
        }
    }
}
