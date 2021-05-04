using Dlubal.RFEM5;
using Parametric_FEM_Toolbox.RFEM;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dlubal.RFEM3;
using Grasshopper.Kernel.Geometry.SpatialTrees;

namespace Parametric_FEM_Toolbox.HelperLibraries
{
    public static class UtilLibrary
    {
        #region general methods
        public static double ToDegrees(this double rad)
        {
            return Math.Round(rad * 180 / Math.PI, 2);
        }
        public static List<int> ToInt(this string str)
        {
            List<int> output = new List<int>();
            if (!(str.Length > 0))
            {
                return output;
            }
            if (str.Contains("-") && !str.Contains(","))
            {
                string[] strSplit = str.Split(new char[] { '-' });
                int boundA = System.Convert.ToInt32(strSplit[0]);
                int boundB = System.Convert.ToInt32(strSplit[1]);
                if (boundA < boundB)
                {
                    for (int i = boundA; i < boundB + 1; i++)
                    {
                        output.Add(i);
                    }
                }
                else if (boundB < boundA)
                {
                    for (int i = boundA; i > boundB - 1; i--)
                    {
                        output.Add(i);
                    }
                }
            }
            else if (str.Contains(",") && !str.Contains("-"))
            {
                string[] strSplit = str.Split(new char[] { ',' });
                foreach (string s in strSplit)
                {
                    output.Add(System.Convert.ToInt32(s));
                }
            }
            else if (str.Contains("-") && str.Contains(","))
            {
                string[] strSplit = str.Split(new char[] { ',' });
                foreach (string s in strSplit)
                {
                    if (s.Contains("-"))
                    {
                        string[] strSplit_B = s.Split(new char[] { '-' });
                        int boundA = System.Convert.ToInt32(strSplit_B[0]);
                        int boundB = System.Convert.ToInt32(strSplit_B[1]);
                        if (boundA < boundB)
                        {
                            for (int i = boundA; i < boundB + 1; i++)
                            {
                                output.Add(i);
                            }
                        }
                        else if (boundB < boundA)
                        {
                            for (int i = boundA; i > boundB - 1; i--)
                            {
                                output.Add(i);
                            }
                        }
                    }
                    else
                    {
                        output.Add(System.Convert.ToInt32(s));
                    }
                }
            }
            else
            {
                output.Add(System.Convert.ToInt32(str));
            }
            return output;
        }

        public static bool Includes(this List<Interval> Intervals, double Value)
        {
            foreach (var interval in Intervals)
            {
                if (interval.IncludesParameter(Value))
                {
                    return true;
                }
            }
            return false;
        }

        public static string DOF(this double parameter, string units)
        {
            switch (parameter)
            {
                case (-0.001):
                    {
                        return "Fixed";
                    }
                case (0):
                    {
                        return "Free";
                    }
            }
            return parameter.ToString("0.00") + units;
        }

        public static string ToLabelString(this List<Plane> planelist)
        {
            var output = "";
            foreach (var item in planelist)
            {
                output += "[" + item.ToString() + "],";
            }
            return output.Substring(0, output.Length - 1);
        }

        public static IEnumerable<string> GetEnumerable(Type type)
        {
            var enumList = new List<string>();
            foreach (var item in Enum.GetValues(type))
            {
                enumList.Add(item.ToString());
            }
            IEnumerable<string> myEnum = enumList;
            return myEnum;
        }

        public static string DescriptionRFTypes(Type type)
        {
            var description = type.Name + ": ";
            foreach (var rfType in GetEnumerable(type))
            {
                description += rfType.ToString() + ", ";
            }
            description = description.Substring(0, description.Length - 2) + ".";
            return description;
        }

        public static List<string> ListRFTypes(Type type)
        {
            var outList = new List<string>();
            foreach (var rfType in GetEnumerable(type))
            {
                outList.Add(rfType.ToString());
            }
            return outList;
        }

        public static bool NullLineExists(IEnumerable<Dlubal.RFEM5.Line> alllines, IEnumerable<RFLine> edges, ref string boundarylinelist)
        {
            // In order to define quadrangles, at least four boundary lines are required.
            // This method provides the only? way to create a suitable null line for the quad definition
            var nodeNos = edges.Select(x => x.NodeList).SelectMany(x => x.ToInt()).ToList();
            var hSetNodes = new HashSet<int>(nodeNos);
            // Does a null line already exist?
            foreach (var line in alllines)
            {
                foreach (var node in hSetNodes)
                {
                    var targetNodeList = node.ToString() + "," + node.ToString();
                    if (line.NodeList == targetNodeList)
                    {
                        // Node list must be set either clockwise or counterclockwise
                        var newboundarylist = new int[4];
                        newboundarylist[0] = (line.No);
                        foreach (var edge in edges)
                        {
                            var nodelist = edge.NodeList.ToInt();
                            if (nodelist.Contains(node))
                            {
                                if (newboundarylist[1] == 0)
                                {
                                    newboundarylist[1] = edge.No;
                                } else
                                {
                                    newboundarylist[3] = edge.No;
                                }
                            } else
                            {
                                newboundarylist[2] = edge.No;
                            }
                        }
                        boundarylinelist = string.Join(",", newboundarylist);
                        return true;
                    }
                }
            }
            return false;
        }




        #endregion

        #region Geometry

        //Converts an RFEM Point3D into a Rhino Point3d and viceversa
        public static Point3d ToPoint3d(this Point3D pt)
        {
            return new Point3d(pt.X, pt.Y, pt.Z);
        }
        public static Point3d[] ToPoint3d(this Point3D[] pts)
        {
            if (!(pts == null))
            {
                return Array.ConvertAll(pts, new Converter<Point3D, Point3d>(ToPoint3d));
            }
            return null;
        }
        public static Point3d[,] ToPoint3d(this Point3D[,] pts)
        {
            if (!(pts == null))
            {
                var myPts = new Point3d[pts.GetLength(0), pts.GetLength(1)];
                for (int i = 0; i < pts.GetLength(0); i++)
                {
                    for (int j = 0; j < pts.GetLength(1); j++)
                    {
                        myPts[i, j] = pts[i, j].ToPoint3d();
                    }
                }
                return myPts;
            }
            return null;
        }
        public static Point3d ToPoint3d(this Node n, IModelData data)
        {
            // A little bit of recursion in case the ref node has another ref node...
            if (n.RefObjectNo == 0)
            {
                return new Point3d(n.X, n.Y, n.Z);
            }
            var refNode = data.GetNode(n.RefObjectNo, ItemAt.AtNo).GetData();
            return new Point3d(new Point3d(n.X, n.Y, n.Z) + refNode.ToPoint3d(data));
        }
        public static Point3D ToPoint3D(this Point3d pt)
        {
            Point3D pt1 = new Point3D
            {
                X = pt.X,
                Y = pt.Y,
                Z = pt.Z
            };
            return pt1;
        }
        public static Point3D[] ToPoint3D(this Point3d[] pts)
        {
            return Array.ConvertAll(pts, new Converter<Point3d, Point3D>(ToPoint3D));
        }
        public static Point3D[,] ToPoint3D(this Point3d[,] pts)
        {
            var myPts = new Point3D[pts.GetLength(0), pts.GetLength(1)];
            for (int i = 0; i < pts.GetLength(0); i++)
            {
                for (int j = 0; j < pts.GetLength(1); j++)
                {
                    myPts[i, j] = pts[i, j].ToPoint3D();
                }
            }
            return myPts;
        }

        public static string ToLabelString(this Point3d[,] pts)
        {
            if (!(pts == null))
            {
                string label = "";
                int i = 1;
                foreach (Point3d pt in pts)
                {
                    label = label + "P" + i + "(" + pt.ToString() + "),";
                    i++;
                }
                if (pts.Length == 0)
                {
                    label = "-";
                }
                else
                {
                    label = label.Substring(0, label.Length - 1);
                    return label;
                }
                return label;
            }
            return "-";
        }
        public static string ToLabelString(this Point3d[] pts)
        {
            if (!(pts == null))
            {
                string label = "";
                int i = 1;
                foreach (Point3d pt in pts)
                {
                    label = label + "P" + i + "(" + pt.ToString() + "),";
                    i++;
                }
                if (pts.Length == 0)
                {
                    label = "-";
                }
                else
                {
                    label = label.Substring(0, label.Length - 1);
                    return label;
                }
                return label;
            }
            return "-";
        }
        public static string ToLabelString(this double[] values)
        {
            if (!(values == null))
            {
                string label = "[";
                int i = 1;
                foreach (double val in values)
                {
                    label = label + val.ToString("0.00") + ",";
                    i++;
                }
                label = label.Substring(0, label.Length - 1) + "]";
                return label;
            }
            return "-";
        }
        public static string ToLabelString(this double[,] values)
        {
            if (!(values == null))
            {
                string label = "[";
                int i = 1;
                foreach (double val in values)
                {
                    label = label + val.ToString("0.00") + ",";
                    i++;
                }
                label = label.Substring(0, label.Length - 1) + "]";
                return label;
            }
            return "-";
        }

        public static Point3d CreateAuxiliaryPoint(Curve edge1, Curve edge2, double distance)
        {
            var vector1 = new Vector3d(edge1.TangentAtStart);
            var vector2 = new Vector3d(edge2.TangentAtStart);
            var basePoint = new Point3d((edge1.PointAt(edge1.Domain.Mid) + edge2.PointAt(edge2.Domain.Mid)) / 2);
            var traslate = Vector3d.CrossProduct(vector1, vector2);
            traslate.Unitize();
            return basePoint + traslate * distance;

        }

        public static Brep CreateNonPlanarBrep(IEnumerable<Curve> boundary, double startTolerance)
        {
            // Iterates tolerance values until a valid surface is created!
            var tolerance = startTolerance;
            var counter = 0;
            var myBrep = new Brep[0];
            while (counter < 1000)
            {
                myBrep = Brep.CreatePlanarBreps(boundary, tolerance);
                if (myBrep != null)
                {
                    return myBrep[0];
                }
                else
                {
                    tolerance *= 1.1;
                    counter += 1;
                }
            }
            return null;
        }

        public static Point3d ToPoint3d(this Dlubal.RFEM3.IPOINT_2D point2D, bool scale = true)
        {
            var scaleFactor = Convert.ToDouble(scale) / 1000;
            return new Point3d(point2D.x * scaleFactor, point2D.y * scaleFactor, 0);
        }


        public static bool IsVertical(this Curve curve)
        {
            if (curve.IsClosed)
            {
                return false;
            }
            //var tol = 0.00328084269363222; // Dlubal criterium
            var tol = 0.001;
            var start = curve.PointAtStart;
            var end = curve.PointAtEnd;
            if (Math.Abs(start.X - end.X) < tol)
            {
                return (Math.Abs(start.Y - end.Y) < tol);
            }
            return false;
        }

        public static bool IsHorizontal(this Curve curve)
        {
            if (curve.IsClosed)
            {
                return false;
            }
            //var tol = 0.00328084269363222; // Dlubal criterium
            var tol = 0.001;
            var start = curve.PointAtStart;
            var end = curve.PointAtEnd;
            return (Math.Abs(start.Z - end.Z) < tol);
        }

        public class PointIndex
        {
            public Point3d Point;
            public int Index;            
        }

        public static void PointIndexCoordinates(PointIndex pi, out double x, out double y, out double z)
        {
            x = pi.Point.X;
            y = pi.Point.Y;
            z = pi.Point.Z;
        }

        public static List<PointIndex> CullClosestPoints(List<Point3d> points, List<RFNode> existingNodes, ref int lastNodeIndex, double tol)
        {
            var outPoints = new List<PointIndex>();
            BoundingBox empty = BoundingBox.Empty;

            // Create PointIndexes
            var existingPointIndices = new List<PointIndex>(points.Count);
            for (int i = 0; i < existingNodes.Count; i++)
            {
                PointIndex pointIndex = new PointIndex();
                pointIndex.Index = existingNodes[i].No;
                pointIndex.Point = existingNodes[i].Location;
                existingPointIndices.Add(pointIndex);
                empty.Union(pointIndex.Point);
            }
            var list2 = new List<PointIndex>(points.Count);
            for (int i = 0; i < points.Count; i++)
            {
                PointIndex pointIndex = new PointIndex();
                pointIndex.Index = -1;
                pointIndex.Point = points[i];
                list2.Add(pointIndex);
                empty.Union(pointIndex.Point);
            }

            
            empty.Inflate(tol);
            Node3d<PointIndex> val = new Node3d<PointIndex>((Coordinates3d<PointIndex>)PointIndexCoordinates, empty, 30);
            int firstItem = 0;
            if (existingNodes.Count>0)
            {
                val.AddRange(existingPointIndices); // list to compare points to
            }
            else
            {
                var node = list2[0];
                node.Index = 1;
                firstItem = 1;
                lastNodeIndex = 1;
                val.Add(node); // list to compare points to
                outPoints.Add(node);
            }
            
            for (int j = firstItem; j <= list2.Count - 1; j++)
            {
                PointIndex pointIndex2 = list2[j];
                while (true)
                {
                    Index3d<PointIndex> val2 = val.NearestItem(pointIndex2);
                    if (val2 == null)
                    {
                        lastNodeIndex++;
                        pointIndex2.Index = lastNodeIndex;
                        val.Add(pointIndex2);
                        outPoints.Add(pointIndex2);
                        
                        break;
                    }
                    PointIndex item = val2.Item;
                    double num4 = pointIndex2.Point.DistanceTo(item.Point);
                    if (num4 > tol || double.IsNaN(num4))
                    {
                        lastNodeIndex++;
                        pointIndex2.Index = lastNodeIndex;
                        val.Add(pointIndex2);
                        outPoints.Add(pointIndex2);
                        break;
                    }
                    pointIndex2.Point = Point3d.Unset;
                    pointIndex2.Index = val2.Item.Index;
                    outPoints.Add(pointIndex2);
                    break;
                }
                List<PointIndex> itemsGlobal = val.ItemsGlobal;
            }
            return outPoints;
        }

        #endregion
    }
}
