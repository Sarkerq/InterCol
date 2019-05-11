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
            NotUsedColors = new List<int>[graph.NotUsedColors.Length];
            for (int i = 0; i < NotUsedColors.Length; i++)
                NotUsedColors[i] = new List<int>(graph.NotUsedColors[i]);
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

        public UndirectedGraph Subgraph(List<int> vertices)
        {
            var subgraph = new UndirectedGraph(this);
            subgraph.RemoveVertices(Enumerable.Range(0, subgraph.VerticeCount()).Where(i => !vertices.Contains(i)).ToList());

            return subgraph;
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
            if (EdgesForVertex(edge.V1, true).Count == 0
                && EdgesForVertex(edge.V2, true).Count == 0)
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
                return 0;
            });

            return rankedColors[0].Color;
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

        public void ColorEdge(Edge edge)
        {
            ColorMatrix[edge.V1, edge.V2] = ColorMatrix[edge.V2, edge.V1] = edge.Color;
            NotUsedColors[edge.V1].Remove(edge.Color);
            NotUsedColors[edge.V2].Remove(edge.Color);
        }

        public bool IsSubgraphConsistent(List<int> subgraphVertices)
        {
            var subgraphVerticesClone = new List<int>(subgraphVertices);

            List<int> verticesBfs = new List<int>();
            verticesBfs.Add(subgraphVerticesClone[0]);
            subgraphVerticesClone.RemoveAt(0);
            while (verticesBfs.Count > 0)
            {
                var currentVertice = verticesBfs[0];
                verticesBfs.RemoveAt(0);

                List<int> verticesToRemove = new List<int>();
                for (int i = 0; i < subgraphVerticesClone.Count; i++)
                {
                    if (AdjacencyMatrix[currentVertice, subgraphVerticesClone[i]] == 1)
                    {
                        verticesBfs.Add(subgraphVerticesClone[i]);
                        verticesToRemove.Add(subgraphVerticesClone[i]);
                    }
                }
                foreach (int vertice in verticesToRemove)
                {
                    subgraphVerticesClone.Remove(vertice);
                }
            }

            return subgraphVerticesClone.Count == 0;
        }

        public int SubgraphEdgesCount(List<int> subgraphVertices)
        {
            int result = 0;

            foreach (var v1 in subgraphVertices)
            {
                foreach (var v2 in subgraphVertices)
                {
                    if (this[v1, v2] == 1)
                        result++;
                }
            }

            return result / 2;
        }

        internal List<List<int>> GetConnectionsWithSubgraph(List<int> vertices)
        {
            List<List<int>> connections = new List<List<int>>();
            for (int i = 0; i < AdjacencyMatrix.GetLength(0); i++)
            {
                connections.Add(new List<int>());
                foreach (int vertice in vertices)
                {
                    if (AdjacencyMatrix[i, vertice] == 1) connections[i].Add(vertice);
                }
            }
            return connections;
        }

        public static bool Identical(UndirectedGraph first, UndirectedGraph second)
        {
            if (first.AdjacencyMatrix.GetLength(0) != second.AdjacencyMatrix.GetLength(0))
                return false;
            for (int i = 0; i < first.AdjacencyMatrix.GetLength(0); i++)
                for (int j = 0; j < first.AdjacencyMatrix.GetLength(0); j++)
                    if (first[i, j] != second[i, j])
                        return false;
            return true;
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
            var toRemove = new List<Tuple<int, int>>() { new Tuple<int, int>(e1, e2) };
            RemoveEdges(toRemove, throwOnNotExist);
        }

        public void RemoveEdges(List<Tuple<int, int>> toRemove, bool throwOnNotExist = false)
        {
            for (int i = 0; i < toRemove.Count; i++)
            {
                if (throwOnNotExist)
                {
                    if (AdjacencyMatrix[toRemove[i].Item1, toRemove[i].Item2] == 0)
                        throw new Exception("Edge (" + toRemove[i].Item1.ToString() + " , " + toRemove[i].Item2.ToString() + ") does not exist");
                }
                AdjacencyMatrix[toRemove[i].Item1, toRemove[i].Item2] = 0;
                AdjacencyMatrix[toRemove[i].Item2, toRemove[i].Item1] = 0;
            }
        }

        public void AddVertex()
        {
            AddVertices(1);
        }

        public void AddVertices(int amount)
        {
            int oldVerticesAmount = AdjacencyMatrix.GetLength(0);
            int newVerticesAmount = oldVerticesAmount + amount;
            AdjacencyMatrix = ResizeArray(AdjacencyMatrix, newVerticesAmount, newVerticesAmount);
        }


        private T[,] ResizeArray<T>(T[,] original, int x, int y)
        {
            T[,] newArray = new T[x, y];
            int minX = Math.Min(original.GetLength(0), newArray.GetLength(0));
            int minY = Math.Min(original.GetLength(1), newArray.GetLength(1));

            for (int i = 0; i < minY; ++i)
                Array.Copy(original, i * original.GetLength(0), newArray, i * newArray.GetLength(0), minX);

            return newArray;
        }
        public void RemoveVertex(int v)
        {
            RemoveVertices(new List<int>() { v });
        }

        public void RemoveVertices(List<int> toRemove)
        {
            Debug.Assert(toRemove.Distinct().Count() == toRemove.Count);
            int newSize = AdjacencyMatrix.GetLength(0) - toRemove.Count;
            int[,] newAdjacencyMatrix = new int[newSize, newSize];
            int oldi = 0, oldj = 0;
            for (int i = 0; i < newSize; i++, oldi++)
            {
                oldj = 0;
                if (toRemove.Contains(oldi))
                {
                    i--;
                    continue;
                }
                for (int j = 0; j < newSize; j++, oldj++)
                {
                    if (toRemove.Contains(oldj))
                    {
                        j--;
                        continue;
                    }
                    newAdjacencyMatrix[i, j] = AdjacencyMatrix[oldi, oldj];
                }
            }
            AdjacencyMatrix = newAdjacencyMatrix;
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
