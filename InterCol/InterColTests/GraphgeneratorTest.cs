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
    public class GraphGeneratorTest
    {
        [TestMethod]
        public void FiveVerticesFiftyPercentEdgesTest()
        {
            int verticesCount = 5;
            double edgePercent = 0.5;
            var graph = GraphGenerator.Generate(verticesCount, edgePercent);

            int expectedEdgesCount = (int)(edgePercent * verticesCount * (verticesCount - 1) / 2);

            Assert.AreEqual(verticesCount, graph.VerticeCount());
            Assert.AreEqual(expectedEdgesCount, graph.EdgeCount());
        }

        [TestMethod]
        public void SevenVerticesTwentyFivePercentEdgesTest()
        {
            int verticesCount = 7;
            double edgePercent = 0.25;
            var graph = GraphGenerator.Generate(verticesCount, edgePercent);

            int expectedEdgesCount = (int)(edgePercent * verticesCount * (verticesCount - 1) / 2);

            Assert.AreEqual(verticesCount, graph.VerticeCount());
            Assert.AreEqual(expectedEdgesCount, graph.EdgeCount());
        }
    }
}

