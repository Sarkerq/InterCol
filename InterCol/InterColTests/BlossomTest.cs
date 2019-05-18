using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InterCol;
using System.IO;
using System.Text.RegularExpressions;

namespace InterColTests
{
    [TestClass]
    public class BlossomTest
    {
        string _graphPathCommon = "../../GraphExamples/";

        [TestMethod]
        public void BlossomAlgorithmBigTest()
        {
            List<Edge> internetResults =
            new List<Edge>()
            {
               new Edge(0,13),
               new Edge(1,15),
               new Edge(2,3),
               new Edge(4,5),
               new Edge(6,7),
               new Edge(8,11),
               new Edge(9,14),
               new Edge(10,12),
            };

            var graph = UndirectedGraph.Load(_graphPathCommon + "Blossom.txt");
            List<Edge> edges = new Blossom(graph).MaximumMatching();

            Assert.IsTrue(internetResults.Count == edges.Count);

            foreach (var internetEdge in internetResults)
            {
                Edge foundEdge = edges.Single(e => (e.V1 == internetEdge.V1 && e.V2 == internetEdge.V2) || (e.V2 == internetEdge.V1 && e.V1 == internetEdge.V2));
                edges.Remove(foundEdge);
            }
        }
        [TestMethod]
        public void MatchingAlgorithmBlossomTests()
        {
            List<List<Edge>> handCheckedResults =
            new List<List<Edge>>()
            {
                new List<Edge>(){new Edge(0,2), new Edge(1,3), new Edge(4,6), new Edge(5,7) }
                ,null
                ,null
                ,null
                ,null
                ,null
            };

            for (int ii = 1; ii < 6; ii++)
            {
                var graph = UndirectedGraph.Load(_graphPathCommon + "MatchingColorTest" + ii.ToString() + ".txt");
                var result = new MatchingAlgorithm().ColorGraph(graph);

                int colorIndex = 0;
                for (int i = 0; i < result.AdjacencyMatrix.GetLength(0); i++)
                    for (int j = i + 1; j < result.AdjacencyMatrix.GetLength(0); j++)
                        if (result[i, j] == 1)
                        {
                            //Assert.IsTrue(result.ColorMatrix[i, j] == handCheckedResults[ii][colorIndex]);
                            colorIndex++;
                        }
            }
        }
    }
}

