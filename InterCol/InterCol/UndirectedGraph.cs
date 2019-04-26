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
        private int[,] _adjacencyMatrix;

        public int[,] AdjacencyMatrix
        {
            get => _adjacencyMatrix;
        }

        public UndirectedGraph(int v)
        {
            _adjacencyMatrix = new int[v, v];
        }

        public UndirectedGraph(UndirectedGraph graph)
        {
            _adjacencyMatrix = new int[graph._adjacencyMatrix.GetLength(0), graph._adjacencyMatrix.GetLength(0)];
            for (int i = 0; i < _adjacencyMatrix.GetLength(0); i++)
                for (int j = 0; j < _adjacencyMatrix.GetLength(1); j++)
                    _adjacencyMatrix[i, j] = graph._adjacencyMatrix[i, j];
        }


        public int this[int e1, int e2]
        {
            get
            {
                return _adjacencyMatrix[e1, e2];
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
                    if (_adjacencyMatrix[currentVertice, subgraphVerticesClone[i]] == 1)
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
            for (int i = 0; i < _adjacencyMatrix.GetLength(0); i++)
            {
                connections.Add(new List<int>());
                foreach (int vertice in vertices)
                {
                    if (_adjacencyMatrix[i, vertice] == 1) connections[i].Add(vertice);
                }
            }
            return connections;
        }

        public static bool Identical(UndirectedGraph first, UndirectedGraph second)
        {
            if (first._adjacencyMatrix.GetLength(0) != second._adjacencyMatrix.GetLength(0))
                return false;
            for (int i = 0; i < first._adjacencyMatrix.GetLength(0); i++)
                for (int j = 0; j < first._adjacencyMatrix.GetLength(0); j++)
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
                    if (_adjacencyMatrix[toAdd[i].Item1, toAdd[i].Item2] == 1)
                        throw new Exception("Edge (" + toAdd[i].Item1.ToString() + " , " + toAdd[i].Item2.ToString() + ") already exists");
                }
                _adjacencyMatrix[toAdd[i].Item1, toAdd[i].Item2] = 1;
                _adjacencyMatrix[toAdd[i].Item2, toAdd[i].Item1] = 1;
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
                    if (_adjacencyMatrix[toRemove[i].Item1, toRemove[i].Item2] == 0)
                        throw new Exception("Edge (" + toRemove[i].Item1.ToString() + " , " + toRemove[i].Item2.ToString() + ") does not exist");
                }
                _adjacencyMatrix[toRemove[i].Item1, toRemove[i].Item2] = 0;
                _adjacencyMatrix[toRemove[i].Item2, toRemove[i].Item1] = 0;
            }
        }

        public void AddVertex()
        {
            AddVertices(1);
        }

        public void AddVertices(int amount)
        {
            int oldVerticesAmount = _adjacencyMatrix.GetLength(0);
            int newVerticesAmount = oldVerticesAmount + amount;
            _adjacencyMatrix = ResizeArray(_adjacencyMatrix, newVerticesAmount, newVerticesAmount);
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
            int newSize = _adjacencyMatrix.GetLength(0) - toRemove.Count;
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
                    newAdjacencyMatrix[i, j] = _adjacencyMatrix[oldi, oldj];
                }
            }
            _adjacencyMatrix = newAdjacencyMatrix;
        }
        public int VerticeDegree(int v)
        {
            int degree = 0;
            for (int i = 0; i < _adjacencyMatrix.GetLength(0); i++)
            {
                degree += _adjacencyMatrix[v, i];
            }
            return degree;
        }
        public int VerticeCount()
        {
            return _adjacencyMatrix.GetLength(0);
        }
        public int EdgeCount()
        {
            return Enumerable.Range(0, _adjacencyMatrix.GetLength(0)).Select(i => VerticeDegree(i)).Sum() / 2;
        }
        public void Save(string filename)
        {
            string csv = VerticeCount().ToString() + "\n" + EdgeCount().ToString() + "\n";
            for (int i = 0; i < _adjacencyMatrix.GetLength(0); i++)
                for (int j = i + 1; j < _adjacencyMatrix.GetLength(0); j++)
                {
                    if (_adjacencyMatrix[i, j] == 1)
                        csv += (i+1).ToString() + " " + (j+1).ToString() + "\n";
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
                graph._adjacencyMatrix[int.Parse(vertices[0]) - 1, int.Parse(vertices[1]) - 1] = 1;
                graph._adjacencyMatrix[int.Parse(vertices[1]) - 1, int.Parse(vertices[0]) - 1] = 1;

            }
            return graph;
        }

    }
}
