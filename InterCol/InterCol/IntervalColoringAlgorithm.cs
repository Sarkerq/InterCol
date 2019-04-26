using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterCol
{
    interface IntervalColoringAlgorithm
    {
         List<int> ColorGraph(UndirectedGraph graph);
    }
}
