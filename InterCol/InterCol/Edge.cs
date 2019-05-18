using System;
using System.Collections.Generic;

namespace InterCol
{
    public class Edge
    {
        public int V1;
        public int V2;
        public int Color;
        public Edge(int v1, int v2, int color = 0)
        {
            V1 = v1;
            V2 = v2;
            Color = color;
        }
        public Edge(Edge edge)
        {
            V1 = edge.V1;
            V2 = edge.V2;
            Color = edge.Color;
        }
    }
    public class EdgeComparer : IEqualityComparer<Edge>
    {
        public bool Equals(Edge e1, Edge e2)
        {
            if ((e1.V1 == e2.V1 && e1.V2 == e2.V2 )|| (e1.V1 == e2.V2 && e1.V2 == e2.V1))
            {
                return true;
            }
            return false;
        }

        public int GetHashCode(Edge e)
        {
            int maxV = Math.Max(e.V1, e.V2);
            int minV = Math.Min(e.V1, e.V2);
            return (int)Math.Pow(5, maxV) * (int)Math.Pow(3, minV);
        }
    }
}