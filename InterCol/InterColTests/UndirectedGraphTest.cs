using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InterCol;

namespace InterColTests
{
    [TestClass]
    public class UndirectedGraphTest
    {
        [TestMethod]
        public void VerticeDegreeTest()
        {
            var graph = new UndirectedGraph(3);
            graph.AddEdges(new System.Collections.Generic.List<Tuple<int, int>>()
            {
                new Tuple<int, int>(0,1),
                new Tuple<int, int>(2,1),
            });

            Assert.IsTrue(graph.VerticeDegree(1) == 2);
        }
        [TestMethod]
        public void VerticeDegreeTestAfterDeleteEdge()
        {
            var graph = new UndirectedGraph(3);
            graph.AddEdges(new System.Collections.Generic.List<Tuple<int, int>>()
            {
                new Tuple<int, int>(0,1),
                new Tuple<int, int>(2,1),
                new Tuple<int, int>(0,2),
            });
            graph.RemoveEdge(0, 1);
            Assert.IsTrue(graph.VerticeDegree(1) == 1);
        }
        [TestMethod]
        public void VerticeDegreeTestAfterRemovingVertice()
        {
            var graph = new UndirectedGraph(5);
            graph.AddEdges(new System.Collections.Generic.List<Tuple<int, int>>()
            {
                new Tuple<int, int>(0,1),
                new Tuple<int, int>(1,2),
                new Tuple<int, int>(2,3),
                new Tuple<int, int>(3,4),
                new Tuple<int, int>(4,0),

            });
            graph.RemoveVertex(2);
            Assert.IsTrue(graph.VerticeDegree(1) == 1);
        }
        [TestMethod]
        public void CSVSaveAndLoadResultsInIdenticalGraph()
        {
            var graph = new UndirectedGraph(5);
            graph.AddEdges(new System.Collections.Generic.List<Tuple<int, int>>()
            {
                new Tuple<int, int>(0,1),
                new Tuple<int, int>(1,2),
                new Tuple<int, int>(2,3),
                new Tuple<int, int>(3,4),
                new Tuple<int, int>(4,0),

            });

            graph.SaveToCSV("test.csv");
            var graph2 = UndirectedGraph.LoadFromCSV("test.csv");
            Assert.IsTrue(UndirectedGraph.Identical(graph, graph2));
        }
        [TestMethod]
        public void CSVSaveAndLoadResultsInIdenticalList()
        {
            Tuple<List<int>, List<int>> testList = new Tuple<List<int>, List<int>>(new List<int>() { 1, 2, 3, 4 }, new List<int>() { 5, 4, 3, 1 });
            ListCSV.Save(testList, "testlist.csv");
            var testList2 = ListCSV.Load("testlist.csv");
            Assert.IsTrue(testList.Item1.SequenceEqual(testList2.Item1) && testList.Item2.SequenceEqual(testList2.Item2));
        }
        [TestMethod]
        public void AdjacencyMatrixTestAfterDeleteEdge()
        {
            var graph = new UndirectedGraph(3);
            graph.AddEdges(new System.Collections.Generic.List<Tuple<int, int>>()
            {
                new Tuple<int, int>(0,1),
                new Tuple<int, int>(2,1),
                new Tuple<int, int>(0,2),
            });
            graph.RemoveEdge(0, 1);
            Assert.IsTrue(graph.VerticeDegree(1) == 1);
        }
        [TestMethod]
        public void AdjacencyMatrixTestAfterRemovingVertice()
        {
            var graph = new UndirectedGraph(5);
            graph.AddEdges(new System.Collections.Generic.List<Tuple<int, int>>()
            {
                new Tuple<int, int>(0,1),
                new Tuple<int, int>(1,2),
                new Tuple<int, int>(2,3),
                new Tuple<int, int>(3,4),
                new Tuple<int, int>(4,0),

            });
            graph.RemoveVertex(2);
            Assert.IsTrue(graph.VerticeDegree(1) == 1);
        }
    }
}
