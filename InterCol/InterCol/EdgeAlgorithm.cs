using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterCol
{
    public class EdgeAlgorithm : IntervalColoringAlgorithm
    {
        private const int callsLimit = 5; 
        public UndirectedGraph ColorGraph(UndirectedGraph graph)
        {
            graph.InitializeNotUsedColors();
            return ColorGraphRecursion(graph);
        }
        
        private UndirectedGraph ColorGraphRecursion(UndirectedGraph graph)
        {
            var notColoredEdges = graph.NotColoredEdges;
            if (notColoredEdges.Count == 0)
                return graph;
            notColoredEdges.Sort((e1, e2) => graph.SortEdgesByColored(e1, e2));
            var limitedSortedNotColoredEdges = notColoredEdges.Take(callsLimit);

            foreach (var e in limitedSortedNotColoredEdges)
            {
                var graphPrim = new UndirectedGraph(graph);
                var minColor = graphPrim.CalculateColor(e);
                if (minColor == null)
                    return null;
                graphPrim.ColorEdge(new Edge(e.V1, e.V2, minColor.Value));

                var result = ColorGraphRecursion(graphPrim);
                if (result != null)
                    return result;
            }

            return null;
        }
    }
}
