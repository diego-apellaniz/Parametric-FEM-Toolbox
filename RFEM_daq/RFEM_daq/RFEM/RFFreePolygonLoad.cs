using System;
using System.Collections.Generic;
using Dlubal.RFEM5;
using Grasshopper.Kernel.Types;
using RFEM_daq.HelperLibraries;
using Rhino.Geometry;
using System.Linq;


namespace RFEM_daq.RFEM
{
    [Serializable]
    public class RFFreePolygonLoad : IGrassRFEM
    {
          
        //Standard constructors
        public RFFreePolygonLoad()
        {
        }
        public RFFreePolygonLoad(FreePolygonLoad load, int loadcase)
        {
            SurfaceList = load.SurfaceList;
            CornerPointList = load.CornerPointList;
            Comment = load.Comment;
            ID = load.ID;
            IsValid = load.IsValid;
            No = load.No;
            Tag = load.Tag;
            LoadDirType = load.Direction;
            LoadDistType = load.Distribution;
            ProjectionType = load.ProjectionPlane;
            Node1No = load.Point1No;
            Node2No = load.Point2No;
            Node3No = load.Point3No;
            Magnitude1 = load.Magnitude1/1000;
            Magnitude2 = load.Magnitude2/1000;
            Magnitude3 = load.Magnitude3/1000;
            Position = load.Position;
            LoadCase = loadcase;
            ToModify = false;
            ToDelete = false;
        }

        public RFFreePolygonLoad(RFFreePolygonLoad other) : this(other, other.LoadCase)
        {
            ToModify = other.ToModify;
            ToDelete = other.ToDelete;
        }

        //Properties to Wrap Fields from RFEM Struct
        public string Comment { get; set; }
        public string ID { get; set; }
        public bool IsValid { get; set; }
        public int No { get; set; }
        public string Tag { get; set; }
        public string SurfaceList { get; set; }
        public string CornerPointList { get; set; }
        public int Node1No { get; set; }
        public int Node2No { get; set; }
        public int Node3No { get; set; }
        public double Magnitude1 { get; set; }
        public double Magnitude2 { get; set; }
        public double Magnitude3 { get; set; }
        public double Position { get; set; }
        public List<Brep> Polygon { get; set; }
        public LoadDirectionType LoadDirType { get; set; }
        public LoadDistributionType LoadDistType { get; set; }
        public PlaneType ProjectionType { get; set; }
        // Additional Properties to the RFEM Struct
        public int LoadCase { get; set; }
        public bool ToModify { get; set; }
        public bool ToDelete { get; set; }
        //public int NewNo { get; set; }

        // Display Info of the RFEM Objects on Panels
        // Parameters are separated by ";". The component split text can be used to break the string down into a list.
        public override string ToString()
        {
            return string.Format($"RFEM-FreePolygonLoad;No:{No};LoadCase:{LoadCase};" +
                 $"F1:{Magnitude1.ToString("0.00")}[kN/m²];F2:{Magnitude2.ToString("0.00")}[kN/m²];F3:{Magnitude3.ToString("0.00")}[kN/m²];" +
                $"Node1No:{Node1No.ToString()};Node2No:{Node2No.ToString()};Node3No:{Node3No.ToString()};Position:{Position.ToString("0.00")}[m];" +
                $"ProjectionType:{ProjectionType.ToString()};LoadDistType:{LoadDistType.ToString()};LoadDirType:{LoadDirType.ToString()};" +
                $"SurfaceList:{((SurfaceList == "") ? "-" : SurfaceList)};CornerPointList:{((CornerPointList == "") ? "-" : CornerPointList)};" +
                $"IsValid:{IsValid};ID:{((ID == "") ? "-" : ID)};Tag:{((Tag == "") ? "-" : Tag)};" +
                $"ToModify:{ToModify};ToDelete:{ToDelete};Comment:{((Comment == "") ? "-" : Comment)};");
        }           

        //Operator to retrieve a Node from an rfNode.
        public static implicit operator FreePolygonLoad(RFFreePolygonLoad load)
        {
            FreePolygonLoad myLoad = new FreePolygonLoad
            {
                Comment = load.Comment,
                ID = load.ID,
                IsValid = load.IsValid,
                No = load.No,
                Tag = load.Tag,
                SurfaceList = load.SurfaceList,
                CornerPointList = load.CornerPointList,
                Position = load.Position
            };
            myLoad.ProjectionPlane = load.ProjectionType;
            myLoad.Direction = load.LoadDirType;
            myLoad.Distribution = load.LoadDistType;
            myLoad.Point1No = load.Node1No;
            myLoad.Point2No = load.Node2No;
            myLoad.Point3No = load.Node3No;
            myLoad.Magnitude1 = load.Magnitude1 * 1000;
            myLoad.Magnitude2 = load.Magnitude2 * 1000;
            myLoad.Magnitude3 = load.Magnitude3 * 1000;
            return myLoad;
        }

        public void SetCornerPoints()
        {
            if (Polygon == null)
            {
                return;
            }
            var corners = Polygon[0].Vertices.Select(x => x.Location).ToList();
            var value1 = 0.0;
            var value2 = 0.0;
            var cornerlist = "";
            for (int i = 0; i < corners.Count; i++)
            {
                switch (ProjectionType)
                {
                    case (PlaneType.PlaneXY):
                        {
                            value1 = corners[i].X;
                            value2 = corners[i].Y;
                            if (i == 0)
                            {
                                Position = corners[i].Z;
                            }
                            break;
                        }
                    case (PlaneType.PlaneXZ):
                        {
                            value1 = corners[i].X;
                            value2 = corners[i].Z;
                            if (i == 0)
                            {
                                Position = corners[i].Y;
                            }
                            break;
                        }
                    case (PlaneType.PlaneYZ):
                        {
                            value1 = corners[i].Y;
                            value2 = corners[i].Z;
                            if (i == 0)
                            {
                                Position = corners[i].X;
                            }
                            break;
                        }
                    default:
                        {
                            return;
                        }
                }
                cornerlist += value1.ToString() + "," + value2.ToString() + ";";
            }
            cornerlist = cornerlist.Substring(0, cornerlist.Length - 1);
            CornerPointList = cornerlist;
        }

        public List<Brep> GetPolygons(List<RFSurface> surfacelist)
        {
            var outPolygon = new List<Brep>();
            var separator = new char[] { ';', ',' };
            var coordList = CornerPointList.Split(separator);
            var pointList = new List<Point3d>();
            for (int i = 0; i < coordList.Length / 2; i++)
            {
                var value1 = Convert.ToDouble(coordList[i * 2]);
                var value2 = Convert.ToDouble(coordList[i * 2 + 1]);
                switch (ProjectionType)
                {
                    case (PlaneType.PlaneXZ):
                        {
                            pointList.Add(new Point3d(value1, Position, value2));
                            break;
                        }
                    case (PlaneType.PlaneYZ):
                        {
                            pointList.Add(new Point3d(Position, value1, value2));
                            break;
                        }
                    default:
                        {
                            pointList.Add(new Point3d(value1, value2, Position));
                            break;
                        }
                }
            }
            var polycurve = new PolylineCurve(pointList);
            var dir = new Vector3d();
            switch (ProjectionType)
            {
                case (PlaneType.PlaneXZ):
                    {
                        dir = Vector3d.YAxis;
                        break;
                    }
                case (PlaneType.PlaneYZ):
                    {
                        dir = Vector3d.XAxis;
                        break;
                    }
                default:
                    {
                        dir = Vector3d.ZAxis;
                        break;
                    }
            }
            foreach (var srfc in surfacelist)
            {
                
                var projectedcurve = Curve.ProjectToBrep(polycurve, srfc.ToBrep(),dir, 0.001);
                if (projectedcurve != null)
                {
                    outPolygon.Add(HelperLibraries.UtilLibrary.CreateNonPlanarBrep(projectedcurve, 0.001));
                }
            }
            return outPolygon;
        }


        // Casting to GH Data Types
        public bool ToGH_Integer<T>(ref T target)
        {
            object obj = new GH_Integer(No);
            target = (T)obj;
            return true;
        }
        public bool ConvertToGH_Plane<T>(ref T target)
        {
            return false;
        }
        public bool ToGH_Point<T>(ref T target)
        {
            return false;
        }
        public bool ToGH_Line<T>(ref T target)
        {
            return false;
        }
        public bool ToGH_Curve<T>(ref T target)
        {
            return false;
        }
        public bool ToGH_Surface<T>(ref T target)
        {
            if(Polygon.Count == 1)
            {
                object obj = new GH_Surface(Polygon[0]);
                target = (T)obj;
                return true;
            }
            return false;
        }
        public bool ToGH_Brep<T>(ref T target)
        {
            if (Polygon.Count == 1)
            {
                object obj = new GH_Brep(Polygon[0]);
                target = (T)obj;
                return true;
            }
            return false;
        }
        public bool ToGH_Plane<T>(ref T target)
        {
            return false;
        }
    }
}
