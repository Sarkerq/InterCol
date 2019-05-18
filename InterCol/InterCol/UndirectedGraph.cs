using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterCol
{
    public class UndirectedGraph
    {
        public int[,] AdjacencyMatrix { get; private set; }
        public int[,] ColorMatrix { get; private set; }
        public List<int>[] NotUsedColors { get; set; }

        public UndirectedGraph(int v)
        {
            AdjacencyMatrix = new int[v, v];
            ColorMatrix = new int[v, v];
        }
        public UndirectedGraph(UndirectedGraph graph)
        {
            AdjacencyMatrix = new int[graph.AdjacencyMatrix.GetLength(0), graph.AdjacencyMatrix.GetLength(0)];
            for (int i = 0; i < AdjacencyMatrix.GetLength(0); i++)
                for (int j = 0; j < AdjacencyMatrix.GetLength(1); j++)
                    AdjacencyMatrix[i, j] = graph.AdjacencyMatrix[i, j];
            ColorMatrix = new int[graph.ColorMatrix.GetLength(0), graph.ColorMatrix.GetLength(0)];
            for (int i = 0; i < ColorMatrix.GetLength(0); i++)
                for (int j = 0; j < ColorMatrix.GetLength(1); j++)
                    ColorMatrix[i, j] = graph.ColorMatrix[i, j];
            if (graph.NotUsedColors != null)
            {
                NotUsedColors = new List<int>[graph.NotUsedColors.Length];
                for (int i = 0; i < NotUsedColors.Length; i++)
                    NotUsedColors[i] = new List<int>(graph.NotUsedColors[i]);
            }
        }
        public void InitializeNotUsedColors()
        {
            var notUsedColors = Enumerable.Range(1, 2 * EdgeCount()).ToList();
            NotUsedColors = Enumerable.Repeat(notUsedColors, VerticeCount()).ToArray();
        }
        public int this[int e1, int e2]
        {
            get
            {
                return AdjacencyMatrix[e1, e2];
            }
        }
        public void AddEdge(int e1, int e2, bool throwOnExist = false)
        {
            var toAdd = new List<Tuple<int, int>>() { new Tuple<int, int>(e1, e2) };
            AddEdges(toAdd, throwOnExist);
        }
        public List<Edge> Edges
        {
            get
            {
                List<Edge> edges = new List<Edge>();
                for (int i = 0; i < AdjacencyMatrix.GetLength(0); i++)
                    for (int j = i + 1; j < AdjacencyMatrix.GetLength(0); j++)
                        if (AdjacencyMatrix[i, j] == 1)
                            edges.Add(new Edge(i, j, ColorMatrix[i, j]));
                return edges;
            }
            set
            {
                AdjacencyMatrix = new int[AdjacencyMatrix.GetLength(0), AdjacencyMatrix.GetLength(1)];
                ColorMatrix = new int[ColorMatrix.GetLength(0), ColorMatrix.GetLength(1)];
                InitializeNotUsedColors();
                foreach (var edge in value)
                {
                    AdjacencyMatrix[edge.V1, edge.V2] = 1;
                    AdjacencyMatrix[edge.V2, edge.V1] = 1;

                    if (edge.Color != 0)
                        ColorEdge(edge);
                }
            }
        }
        public List<Edge> NotColoredEdges
        {
            get
            {
                return Edges.FindAll(e => e.Color == 0);
            }
        }
        public List<Edge> EdgesForVertex(int vertex, bool? colored = null)
        {
            List<Edge> edges = new List<Edge>();
            for (int i = 0; i < AdjacencyMatrix.GetLength(0); i++)
                if (AdjacencyMatrix[i, vertex] == 1 && (colored == null || (ColorMatrix[i, vertex] != 0) == colored))
                    edges.Add(new Edge(i, vertex, ColorMatrix[i, vertex]));
            return edges;
        }
        public int? CalculateColor(Edge edge)
        {
            var edges1 = EdgesForVertex(edge.V1).ToList();
            var edges2 = EdgesForVertex(edge.V2).ToList();
            var coloredEdges1 = edges1.Where(e => e.Color != 0).ToList();
            var coloredEdges2 = edges2.Where(e => e.Color != 0).ToList();

            if (coloredEdges1.Count == 0
                && coloredEdges2.Count == 0)
            {
                return EdgeCount();
            }

            var rankedColors = new List<ColorPosition>();

            foreach (var color in NotUsedColors[edge.V1])
            {
                if (NotUsedColors[edge.V2].Contains(color))
                {
                    var position = CalculateColorPositionForEdge(edge, color);
                    rankedColors.Add(position);
                }
            }

            if (rankedColors.Count == 0)
                return null;

            rankedColors.Sort((c1, c2) =>
            {
                if (c1.LessColoredVPosition == null || c1.MoreColoredVPosition == null)
                    return -1;
                if (c2.LessColoredVPosition == null || c2.MoreColoredVPosition == null)
                    return 1;
                var rank1 = c1.LessColoredVPosition + c1.MoreColoredVPosition;
                var rank2 = c2.LessColoredVPosition + c2.MoreColoredVPosition;
                if (rank1 > rank2)
                    return 1;
                if (rank1 < rank2)
                    return -1;
                if (c1.MoreColoredVPosition > c2.MoreColoredVPosition)
                    return 1;
                if (c1.MoreColoredVPosition < c2.MoreColoredVPosition)
                    return -1;
                return c1.Color > c2.Color ? 1 : (c1.Color == c2.Color ? 0 : -1);
            });

            for (int i = 0; i < rankedColors.Count; i++)
            {
                int color = rankedColors[i].Color;
                if (ValidateColor(coloredEdges1, color, edges1.Count) && ValidateColor(coloredEdges2, color, edges2.Count))
                    return color;
            }

            return null;
        }

        private ColorPosition CalculateColorPositionForEdge(Edge edge, int color)
        {
            var edges1 = EdgesForVertex(edge.V1);
            var edges2 = EdgesForVertex(edge.V2);
            var coloredEdges1 = edges1.FindAll(e => e.Color != 0);
            var coloredEdges2 = edges2.FindAll(e => e.Color != 0);
            var position1 = CalculateColorPosition(coloredEdges1.Select(e => e.Color), color);
            var position2 = CalculateColorPosition(coloredEdges2.Select(e => e.Color), color);

            var colorPosition = new ColorPosition()
            {
                Color = color
            };
            if (edges1.Count - coloredEdges1.Count < edges2.Count - coloredEdges2.Count || coloredEdges2.Count == 0)
            {
                colorPosition.MoreColoredVPosition = position1;
                colorPosition.LessColoredVPosition = position2;
            }
            else
            {
                colorPosition.MoreColoredVPosition = position2;
                colorPosition.LessColoredVPosition = position1;
            }
            return colorPosition;
        }

        private int? CalculateColorPosition(IEnumerable<int> usedColors, int color)
        {
            if (usedColors.Count() == 0)
                return 0;
            if (usedColors.Contains(color))
                return null;
            var minUsedColor = usedColors.Min();
            var maxUsedColor = usedColors.Max();
            if (color > minUsedColor && color < maxUsedColor)
                return -1;
            if (color < minUsedColor)
                return minUsedColor - color + 1;
            if (color > maxUsedColor)
                return color - maxUsedColor + 1;
            return null;
        }

        private bool ValidateColor(IEnumerable<Edge> coloredEdges, int color, int edgesCount)
        {
            var colors = coloredEdges.Select(e => e.Color).Concat(new List<int> { color });
            return colors.Max() - colors.Min() < edgesCount;
        }

        public void ColorEdge(Edge edge)
        {
            ColorMatrix[edge.V1, edge.V2] = ColorMatrix[edge.V2, edge.V1] = edge.Color;
            NotUsedColors[edge.V1].Remove(edge.Color);
            NotUsedColors[edge.V2].Remove(edge.Color);
        }

        public void AddEdges(List<Tuple<int, int>> toAdd, bool throwOnExist = false)
        {
            for (int i = 0; i < toAdd.Count; i++)
            {
                if (throwOnExist)
                {
                    if (AdjacencyMatrix[toAdd[i].Item1, toAdd[i].Item2] == 1)
                        throw new Exception("Edge (" + toAdd[i].Item1.ToString() + " , " + toAdd[i].Item2.ToString() + ") already exists");
                }
                AdjacencyMatrix[toAdd[i].Item1, toAdd[i].Item2] = 1;
                AdjacencyMatrix[toAdd[i].Item2, toAdd[i].Item1] = 1;
            }
        }
        public void RemoveEdge(int e1, int e2, bool throwOnNotExist = false)
        {
            var toRemove = new List<Edge>() { new Edge(e1, e2) };
            RemoveEdges(toRemove, throwOnNotExist);
        }

        public void RemoveEdges(List<Edge> toRemove, bool throwOnNotExist = false)
        {
            for (int i = 0; i < toRemove.Count; i++)
            {
                if (throwOnNotExist)
                {
                    if (AdjacencyMatrix[toRemove[i].V1, toRemove[i].V2] == 0)
                        throw new Exception("Edge (" + toRemove[i].V1.ToString() + " , " + toRemove[i].V2.ToString() + ") does not exist");
                }
                AdjacencyMatrix[toRemove[i].V1, toRemove[i].V2] = 0;
                AdjacencyMatrix[toRemove[i].V2, toRemove[i].V1] = 0;
            }
        }
        public int VerticeDegree(int v)
        {
            int degree = 0;
            for (int i = 0; i < AdjacencyMatrix.GetLength(0); i++)
            {
                degree += AdjacencyMatrix[v, i];
            }
            return degree;
        }
        public int VerticeCount()
        {
            return AdjacencyMatrix.GetLength(0);
        }
        public int EdgeCount()
        {
            return Enumerable.Range(0, AdjacencyMatrix.GetLength(0)).Select(i => VerticeDegree(i)).Sum() / 2;
        }
        public int SortEdgesByColored(Edge e1, Edge e2)
        {
            var min1 = Math.Min(
                EdgesForVertex(e1.V1, false).Count,
                EdgesForVertex(e1.V2, false).Count);
            var min2 = Math.Min(
                EdgesForVertex(e2.V1, false).Count,
                EdgesForVertex(e2.V2, false).Count);
            return min1 > min2 ? 1 : (min1 == min2 ? 0 : -1);
        }
        public void Save(string filename)
        {
            string csv = VerticeCount().ToString() + "\n" + EdgeCount().ToString() + "\n";
            for (int i = 0; i < AdjacencyMatrix.GetLength(0); i++)
                for (int j = i + 1; j < AdjacencyMatrix.GetLength(0); j++)
                {
                    if (AdjacencyMatrix[i, j] == 1)
                        csv += (i + 1).ToString() + " " + (j + 1).ToString() + "\n";
                }
            File.WriteAllText(filename, csv.TrimEnd(new char[] { '\r', '\n' }));
        }
        public static UndirectedGraph Load(string filename)
        {
            string[] readTexts = File.ReadAllText(filename).TrimEnd('\n').Split('\n');
            if (readTexts.Length != int.Parse(readTexts[1]) + 2)
                throw new Exception("Bad graph format - edge count incorrect");
            int verticeCount = int.Parse(readTexts[0]);
            var graph = new UndirectedGraph(verticeCount);
            for (int i = 2; i < readTexts.Length; i++)
            {
                string[] vertices = readTexts[i].Split(' ');
                if (vertices.Length != 2)
                    throw new Exception("Bad graph format - too many vertices in line " + i.ToString());
                graph.AdjacencyMatrix[int.Parse(vertices[0]) - 1, int.Parse(vertices[1]) - 1] = 1;
                graph.AdjacencyMatrix[int.Parse(vertices[1]) - 1, int.Parse(vertices[0]) - 1] = 1;

            }
            return graph;
        }

    }
}
