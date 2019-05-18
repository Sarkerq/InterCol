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
    public class ColoringAlgorithmTests
    {
        string _graphPathCommon = "../../GraphExamples/";



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
        public void EdgeAlgorithmColoringTests()
        {
            List<List<int>> handCheckedResults =
            new List<List<int>>()
            {
                new List<int>(){5,4,4,5,3},
                new List<int>(){7,6,8,7,6,8,7},
                new List<int>(){6,5,4,4,5,6},
                new List<int>(){5,4,3,2,1},
                null,
                new List<int>(){8,6,7,5,8,6,8,7}
            };

            for (int ii = 1; ii <= handCheckedResults.Count; ii++)
            {
                var graph = UndirectedGraph.Load(_graphPathCommon + "ColorTest" + ii.ToString() + ".txt");
                var result = new EdgeAlgorithm().ColorGraph(graph);
                if (handCheckedResults[ii - 1] == null)
                {
                    Assert.IsNull(result);
                    continue;
                }
                int colorIndex = 0;
                for (int i = 0; i < result.AdjacencyMatrix.GetLength(0); i++)
                    for (int j = i + 1; j < result.AdjacencyMatrix.GetLength(0); j++)
                        if (result[i, j] == 1)
                        {
                            Assert.IsTrue(result.ColorMatrix[i, j] == handCheckedResults[ii-1][colorIndex]);
                            colorIndex++;
                        }
            }
        }
        [TestMethod]
        public void MatchingAlgorithmColoringTests()
        {
            List<List<int>> handCheckedResults =
            new List<List<int>>()
            {
                new List<int>(){10,10,7,9,8,9,8,10,10,9},
                new List<int>(){11,10,11,9,11,10,11,9,11,10,11},
                new List<int>(){6,4,5,6,5,6},
                new List<int>(){9,8,7,9,8,9,7,8,9},
                null,
                new List<int>(){8,7,9,8,7,6,8,7,8}
            };

            for (int ii = 5; ii < handCheckedResults.Count; ii++)
            {
                var graph = UndirectedGraph.Load(_graphPathCommon + "MatchingColorTest" + ii.ToString() + ".txt");
                var result = new MatchingAlgorithm(3).ColorGraph(graph);
                if (handCheckedResults[ii - 1] == null)
                {
                    Assert.IsNull(result);
                    continue;
                }
                int colorIndex = 0;
                for (int i = 0; i < result.AdjacencyMatrix.GetLength(0); i++)
                    for (int j = i + 1; j < result.AdjacencyMatrix.GetLength(0); j++)
                        if (result[i, j] == 1)
                        {
                            Assert.IsTrue(result.ColorMatrix[i, j] == handCheckedResults[ii - 1][colorIndex]);
                            colorIndex++;
                        }
            }
        }
    }
}
