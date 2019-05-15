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
        public void EdgeAlgorithmColoringTests()
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

            foreach(var internetEdge in internetResults)
            {
                Edge foundEdge = edges.Single(e => (e.V1 == internetEdge.V1 && e.V2 == internetEdge.V2) || (e.V2 == internetEdge.V1 && e.V1 == internetEdge.V2));
                edges.Remove(foundEdge);
            }
        }
    }
}

