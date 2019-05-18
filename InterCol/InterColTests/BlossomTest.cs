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
                ,new List<Edge>(){new Edge(0,1), new Edge(2,3), new Edge(4,5), new Edge(6,7), new Edge(8,9), new Edge(10,11)}
                ,new List<Edge>(){new Edge(0,1), new Edge(2,3), new Edge(4,5)}
                ,new List<Edge>(){new Edge(0,1), new Edge(2,3), new Edge(4,5), new Edge(6,7)}
                ,new List<Edge>(){new Edge(0,1)}
                ,new List<Edge>(){new Edge(0,1), new Edge(2,3), new Edge(4,5), new Edge(6,7)}
                ,new List<Edge>(){new Edge(0,1), new Edge(2,3), new Edge(4,5), new Edge(6,7)}
            };

            for (int ii = 1; ii <= 7; ii++)
            {
                var graph = UndirectedGraph.Load(_graphPathCommon + "MatchingColorTest" + ii.ToString() + ".txt");
                List<Edge> edges = new Blossom(graph).MaximumMatching();
                Assert.IsTrue(handCheckedResults[ii - 1].Count == edges.Count);

                Assert.IsTrue(edges.Intersect(handCheckedResults[ii - 1], new EdgeComparer()).Count() == edges.Count);
            }
        }
    }
}

