using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterCol
{
    public static class ListCSV
    {
        public static void Save(Tuple<List<int>, List<int>> toSave, string filename)
        {
            string csv = "";
            for (int i = 0; i < toSave.Item1.Count; i++)
            {
                csv += toSave.Item1[i].ToString();
                if (i == toSave.Item1.Count - 1)
                    csv += Environment.NewLine;
                else
                    csv += ",";
            }
            for (int i = 0; i < toSave.Item2.Count; i++)
            {
                csv += toSave.Item2[i].ToString();
                if (i != toSave.Item2.Count - 1)
                    csv += ",";
            }
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + filename, csv.TrimEnd(new char[] { '\r', '\n' }));
        }
        public static Tuple<List<int>, List<int>> Load(string filename)
        {
            string readText = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + filename).Replace("\r", string.Empty);

            List<List<int>> separatedVerticeLists = readText.Split(new char[] { '\n' }).Select(verticelist => verticelist.Split(new char[] { ',' }).
                                                          Select(vertice => int.Parse(vertice)).ToList()).ToList();

            return new Tuple<List<int>, List<int>>(separatedVerticeLists[0], separatedVerticeLists[1]);
        }
    }
}
