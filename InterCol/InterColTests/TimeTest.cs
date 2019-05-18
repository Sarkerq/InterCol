using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InterCol;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Globalization;

namespace InterColTests
{
    [TestClass]
    class TimeTest
    {
        [TestMethod]
        public void Test()
        {
            var columns = new string[5] { "Algorithm", "Vertices Count", "Edges Percent", "Calls Limit", "Time" };
            var separator = ",";
            var filePath = "./times.csv";
            File.WriteAllText(filePath, string.Join(separator, columns) + Environment.NewLine);

            Stopwatch stopwatch;
            UndirectedGraph graph, result;
            for (int i = 3; i < 8; i++)
                for (double j = 0.2d; j < 0.8d; j += 0.2d)
                {
                    graph = GraphGenerator.Generate(i, j);
                    for (int k = 1; k <= 4; k++)
                    {
                        stopwatch = Stopwatch.StartNew();
                        result = new EdgeAlgorithm(k).ColorGraph(graph);
                        stopwatch.Stop();
                        File.AppendAllText(filePath, string.Join(separator, "Edge", i, j.ToString(CultureInfo.InvariantCulture), k, stopwatch.ElapsedMilliseconds) + Environment.NewLine);

                        stopwatch = Stopwatch.StartNew();
                        result = new MatchingAlgorithm(k).ColorGraph(graph);
                        stopwatch.Stop();
                        File.AppendAllText(filePath, string.Join(separator, "Matching", i, j.ToString(CultureInfo.InvariantCulture), k, stopwatch.ElapsedMilliseconds) + Environment.NewLine);
                    }
                }
        }
    }
}

