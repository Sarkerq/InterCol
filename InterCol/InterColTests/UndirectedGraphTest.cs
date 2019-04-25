﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InterCol;
using System.IO;
using System.Text.RegularExpressions;

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
        public void SaveAndLoadResultsInIdenticalGraph()
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

            graph.Save("test.txt");
            var graph2 = UndirectedGraph.Load("test.txt");
            Assert.IsTrue(UndirectedGraph.Identical(graph, graph2));
        }
        [TestMethod]
        public void LoadAndSaveResultsInIdenticalGraph()
        {
            string graphPathCommon = "./";
            List<string> graphPaths = new List<string>()
            {
                "C10.txt",
                "K4.txt",
                "Empty5.txt",
                "Matching12.txt"
            };
            List<string> fullGraphPaths = graphPaths.Select(p => graphPathCommon + p).ToList();
            foreach (string path in fullGraphPaths)
            {
                string savedPath = "tmp/test.txt";

                string fullSavePath = graphPathCommon + savedPath;
                var graph = UndirectedGraph.Load(path);
                graph.Save(fullSavePath);
                string origFileContents = File.ReadAllText(path);
                string savedFileContents = File.ReadAllText(fullSavePath);
                Assert.IsTrue(StringEqualToWhitespace(origFileContents, savedFileContents));
            }
        }

        private bool StringEqualToWhitespace(string s1, string s2)
        {
            string normalized1 = Regex.Replace(s1, @"\s", "");
            string normalized2 = Regex.Replace(s2, @"\s", "");

            return String.Equals(
                normalized1,
                normalized2,
                StringComparison.OrdinalIgnoreCase);
        }

        [TestMethod]
        public void C10LoadsCorrect()
        {
            var graph = UndirectedGraph.Load("C10.txt");
            Assert.IsTrue(graph.VerticeCount() == 10);
            Assert.IsTrue(graph.EdgeCount() == 10);
            for (int i = 0; i < 10; i++)
                Assert.IsTrue(graph[i, (i + 1) % 10] == 1);
        }
        [TestMethod]
        public void K4LoadsCorrect()
        {
            var graph = UndirectedGraph.Load("K4.txt");
            Assert.IsTrue(graph.VerticeCount() == 4);
            Assert.IsTrue(graph.EdgeCount() == 6);
            for (int i = 0; i < 3; i++)
                for (int j = i + 1; j < 3; j++)
                    Assert.IsTrue(graph[i, j] == 1);

        }
        [TestMethod]
        public void Empty5LoadsCorrect()
        {
            var graph = UndirectedGraph.Load("Empty5.txt");
            Assert.IsTrue(graph.VerticeCount() == 5);
            Assert.IsTrue(graph.EdgeCount() == 0);
        }
        [TestMethod]
        public void Matching12LoadsCorrect()
        {
            var graph = UndirectedGraph.Load("Matching12.txt");
            Assert.IsTrue(graph.VerticeCount() == 12);
            Assert.IsTrue(graph.EdgeCount() == 6);
            for (int i = 0; i < 12; i += 2)
                Assert.IsTrue(graph[i, i + 1] == 1);
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
