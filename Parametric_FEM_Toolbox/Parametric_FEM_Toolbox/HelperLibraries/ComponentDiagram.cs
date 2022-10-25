using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace Parametric_FEM_Toolbox.HelperLibraries
{
    public static class ComponentDiagram
    {
        public static double[,] ToDiagramArray (this List<Point3d> points)
        {
            if (points == null)
                return new double[2, 2];
            var count = points.Count;
            var outArray = new double[count, 2];
            for (int i = 0; i < points.Count; i++)
            {
                outArray[i, 0] = points[i].X;
                outArray[i, 1] = points[i].Y*1000;
            }
            return outArray;
        }

        public static List<Point3d> ToPointList(this double[,] diagram_points)
        {
            var out_list = new List<Point3d>();
            if (diagram_points == null)
                return out_list;            
            for (int i = 0; i < diagram_points.GetLength(0); i++)
            {
                var x = diagram_points[i, 0];
                var y = diagram_points[i, 1]/1000;
                out_list.Add(new Point3d(x, y, 0));
            }
            return out_list;
        }
    }
}
