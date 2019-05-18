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
    public class TimeTest
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
            for (int i = 8; i < 11; i++)
                for (double j = 0.2d; j < 0.8d; j += 0.2d)
                    for (int k = 1; k <= 5; k++)
                        for (int l = 0; l < 1; l++)
                        {
                            graph = GraphGenerator.Generate(i, j);

                            stopwatch = Stopwatch.StartNew();
                            result = new EdgeAlgorithm(k).ColorGraph(graph);
                            stopwatch.Stop();
                            File.AppendAllText(filePath, string.Join(separator, "Edge", i, j.ToString(CultureInfo.InvariantCulture), k, stopwatch.ElapsedMilliseconds) + Environment.NewLine);

                            //stopwatch = Stopwatch.StartNew();
                            //result = new MatchingAlgorithm().ColorGraph(graph);
                            //stopwatch.Stop();
                            //matchingAlgorithmTimes.Add(stopwatch.ElapsedMilliseconds);
                        }
        }
    }
}

