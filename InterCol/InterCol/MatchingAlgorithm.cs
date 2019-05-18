using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterCol
{
    public class MatchingAlgorithm : IntervalColoringAlgorithm
    {
        private const int callsLimit = 5;
        public UndirectedGraph ColorGraph(UndirectedGraph graph)
        {
            graph.InitializeNotUsedColors();
            return ColorGraphRecursion(graph);
        }

        private UndirectedGraph ColorGraphRecursion(UndirectedGraph graph)
        {
            if (graph.NotColoredEdges.Count == 0)
                return graph;

            var graphForMatching = new UndirectedGraph(graph);
            graphForMatching.Edges = graphForMatching.NotColoredEdges;
            var matchings = GetGraphMatchings(graphForMatching);
            foreach (var matching in matchings)
            {
                var graphPrim = new UndirectedGraph(graph);
                bool anyEdgeColored = false;
                foreach (var edge in matching)
                {
                    var color = graphPrim.CalculateColor(edge);
                    if (color != null)
                    {
                        graphPrim.ColorEdge(new Edge(edge.V1, edge.V2, color.Value));
                        anyEdgeColored = true;
                    }
                    else
                        anyEdgeColored = false; //czy nie zwracac albo wychodzic z petli od razu?
                }
                if (anyEdgeColored)
                    return ColorGraphRecursion(graphPrim);
            }

            return null;
        }

        private List<List<Edge>> GetGraphMatchings(UndirectedGraph graph)
        {
            var matchings = new List<List<Edge>>();
            var matching = new Blossom(graph).MaximumMatching();
            matchings.Add(matching);

            matching.Sort((e1, e2) => graph.SortEdgesByColored(e1, e2));
            var listOfEdgesToDelete = matching.Select(e => new List<Edge>() { e }).ToList();
            while (listOfEdgesToDelete.Count > 0)
            {
                var edgesToDelete = listOfEdgesToDelete[0];
                listOfEdgesToDelete.RemoveAt(0);

                matching = GetGraphMatching(graph, edgesToDelete);
                matchings.Add(matching);
                if (matchings.Count >= callsLimit)
                    return matchings;

                matching.Sort((e1, e2) => graph.SortEdgesByColored(e1, e2));
                foreach (var edge in matching)
                {
                    var newEdgesToDelete = edgesToDelete.ConvertAll(e => new Edge(e));
                    newEdgesToDelete.Add(edge);
                    listOfEdgesToDelete.Add(newEdgesToDelete);
                }
            }
            return matchings;
        }

        private List<Edge> GetGraphMatching(UndirectedGraph graph, List<Edge> edgesToDelete)
        {
            var graphPrim = new UndirectedGraph(graph);
            graphPrim.RemoveEdges(edgesToDelete);
            var matching = new Blossom(graphPrim).MaximumMatching();
            return matching;
        }
    }
}
