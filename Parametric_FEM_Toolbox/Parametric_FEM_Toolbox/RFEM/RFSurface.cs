using System;
using Dlubal.RFEM5;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Parametric_FEM_Toolbox.Utilities;
using Parametric_FEM_Toolbox.RFEM;
using System.Collections.Generic;
using System.Linq;
using Parametric_FEM_Toolbox.HelperLibraries;

namespace Parametric_FEM_Toolbox.RFEM
{
    [Serializable]
    public class RFSurface : IGrassRFEM
    {
        //Standard constructors
        public RFSurface()
        {
        }
        public RFSurface(Dlubal.RFEM5.Surface surface, RFLine[] edges, RFOpening[] openings)
        {
            Comment = surface.Comment;
            ID = surface.ID;
            IsGenerated = surface.IsGenerated;
            IsValid = surface.IsValid;
            No = surface.No;
            Tag = surface.Tag;
            Area = surface.Area;
            BoundaryLineCount = surface.BoundaryLineCount;
            BoundaryLineList = surface.BoundaryLineList;
            ControlPoints = surface.ControlPoints.ToPoint3d();
            Eccentricity = surface.Eccentricity;
            GeometryType = surface.GeometryType;
            IntegratedLineCount = surface.IntegratedLineCount;
            IntegratedLineList = surface.IntegratedLineList;
            IntegratedNodeCount = surface.IntegratedNodeCount;
            IntegratedNodeList = surface.IntegratedNodeList;
            MaterialNo = surface.MaterialNo;
            SetIntegratedObjects = surface.SetIntegratedObjects;
            StiffnessType = surface.StiffnessType;
            ThicknessType = surface.Thickness.Type;
            Thickness = surface.Thickness.Constant;
            Edges = edges;
            Openings = openings;
            ToModify = false;
            ToDelete = false;
        }
        public RFSurface(Dlubal.RFEM5.Surface surface) : this(surface, null, null)
        {
        }

        public RFSurface(RFSurface other) : this(other, null, null)
        {
            ToModify = other.ToModify;
            ToDelete = other.ToDelete;

            var newEdges = new List<RFLine>();
            if (other.Edges != null)
            {
                foreach (var edge in other.Edges)
                {
                    newEdges.Add(new RFLine(edge));
                }
                Edges = newEdges.ToArray();
                if (other.Openings != null)
                {
                    var newOpenings = new List<RFOpening>();
                    foreach (var op in other.Openings)
                    {
                        newOpenings.Add(new RFOpening(op));
                    }
                    Openings = newOpenings.ToArray();
                }
            }

            if (GeometryType == SurfaceGeometryType.NurbsSurfaceType)
            {
                ControlPoints = other.ControlPoints;
                OrderX = other.OrderX;
                OrderY = other.OrderY;
                KnotsX = other.KnotsX;
                KnotsY = other.KnotsY;
                Nodes = other.Nodes;
                Weights = other.Weights;
            }
        }

        // Properties to Wrap Fields from RFEM Struct
        public string Comment { get; set; }
        public string ID { get; set; }
        public bool IsGenerated { get; set; }
        public bool IsValid { get; set; }
        public int No { get; set; }
        public string Tag { get; set; }
        public double Area { get; set; }
        public int BoundaryLineCount { get; set; }
        public string BoundaryLineList { get; set; }
        public Point3d[,] ControlPoints { get; set; }
        public int[,] Nodes { get; set; }
        public double[,] Weights { get; set; }
        public double[] KnotsX { get; set; }
        public double[] KnotsY { get; set; }
        public int OrderX { get; set; }
        public int OrderY { get; set; }
        public double Eccentricity { get; set; }
        public SurfaceGeometryType GeometryType { get; set;}
        public int IntegratedLineCount { get; set; }
        public string IntegratedLineList { get; set; }
        public int IntegratedNodeCount { get; set; }
        public string IntegratedNodeList { get; set; }
        public int MaterialNo { get; set; }
        public bool SetIntegratedObjects { get; set; }
        public SurfaceStiffnessType StiffnessType { get; set; }
        public SurfaceThicknessType ThicknessType { get; set; }
        public double Thickness { get; set; }
        // Additional Properties to the RFEM Struct
        public RFLine[] Edges { get; set; }
        public RFOpening[] Openings { get; set; }
        public bool ToModify { get; set; }
        public bool ToDelete { get; set; }
        //public int NewNo { get; set; }

        // Display Info of the RFEM Objects on Panels
        // Parameters are separated by ";". The component split text can be used to break the string down into a list.
        public override string ToString()
        {
            return string.Format($"RFEM-Surface;No:{No};Area:{Area}[m2];MaterialNo:{MaterialNo};" +
                $"Thickness:{Thickness}[m];Type:{GeometryType};ThicknessType:{ThicknessType};StiffnessType:{StiffnessType};BoundaryLineCount:{BoundaryLineCount};" +
                $"BoundaryLineList:{((BoundaryLineList == "") ? "-" : BoundaryLineList)};Eccentricity:{Eccentricity};" +
                $"IntegratedLineCount:{IntegratedLineCount};IntegratedLineList:{((IntegratedLineList == "") ? "-" : IntegratedLineList)};" +
                $"IntegratedNodeCount:{IntegratedNodeCount};IntegratedNodeList:{((IntegratedNodeList == "") ? "-" : IntegratedNodeList)};" +
                $"SetIntegratedObjects:{SetIntegratedObjects};ControlPoints:{ControlPoints.ToLabelString()};Tag:{((Tag == "") ? "-" : Tag)};" +
                //$"Weights:{(Weights.ToString())};KnotsX:{(KnotsX.ToLabelString())};KnotsY:{(KnotsY.ToLabelString())};" +
                //$"OrderU:{(OrderX.ToString())};OrderV:{(OrderY.ToString())};" +
                $"IsValid:{IsValid};IsGenerated:{IsGenerated};ID:{((ID == "") ? "-" : ID)};" +
                $"ToModify:{ToModify};ToDelete:{ToDelete};Comment:{((Comment == "") ? "-" : Comment)};");
        }

        //Operator to retrieve a Line from an rfLine.
        public static implicit operator Dlubal.RFEM5.Surface(RFSurface surface)
        {
            Dlubal.RFEM5.Surface mySurface = new Dlubal.RFEM5.Surface
            {
                Comment = surface.Comment,
                ID = surface.ID,
                IsGenerated = surface.IsGenerated,
                IsValid = surface.IsValid,
                No = surface.No,
                Tag = surface.Tag,
                Area = surface.Area,
                BoundaryLineCount = surface.BoundaryLineCount,
                BoundaryLineList = surface.BoundaryLineList,
                // ControlPoints = surface.ControlPoints.ToPoint3D(),
                Eccentricity = surface.Eccentricity,
                GeometryType = surface.GeometryType,
                IntegratedNodeCount = surface.IntegratedNodeCount,
                IntegratedNodeList = surface.IntegratedNodeList,
                IntegratedLineCount = surface.IntegratedLineCount,
                IntegratedLineList = surface.IntegratedLineList,
                MaterialNo = surface.MaterialNo,
                SetIntegratedObjects = surface.SetIntegratedObjects,
                StiffnessType = surface.StiffnessType,                
            };
            mySurface.Thickness.Type = surface.ThicknessType;
            mySurface.Thickness.Constant = surface.Thickness;
            return mySurface;
        }

        public static implicit operator Dlubal.RFEM5.NurbsSurface(RFSurface surface)
        {
            var val  = (Dlubal.RFEM5.Surface)surface;
            val.GeometryType = SurfaceGeometryType.NurbsSurfaceType;
            val.StiffnessType = SurfaceStiffnessType.NullStiffnessType;            
            Dlubal.RFEM5.NurbsSurface mySurface = new Dlubal.RFEM5.NurbsSurface
            {
                General = surface,
                Nodes = surface.Nodes,
                Weights = surface.Weights,
                OrderX = surface.OrderX,
                OrderY = surface.OrderY,
                KnotsX = surface.KnotsX,
                KnotsY = surface.KnotsY
            };
            return mySurface;
        }
        // Casting to GH Data Types
        public Brep ToBrep()
        {
            //if (IsPlanar())
            //{
            //    return ToPlanarBrep();
            //}
            //else
            //{
            //    return ToNonPlanarBrep();
            //}
            if (GeometryType == SurfaceGeometryType.NurbsSurfaceType)
            {
                return ToNurbsSurface();
            }
            var myEdges = new List<Curve>();
            var sEdges = from e in Edges
                         select e.ToCurve();
            myEdges.AddRange(Curve.JoinCurves(sEdges));
            if (!(Openings == null))
            {
                foreach (var o in Openings)
                {
                    var oEdges = from e in o.Edges
                                 select e.ToCurve();
                    myEdges.AddRange(Curve.JoinCurves(oEdges));
                }
            }
            return UtilLibrary.CreateNonPlanarBrep(myEdges, 0.001);
        }
        private Brep ToPlanarBrep()
        {
            var myEdges = new List<Curve>();
            var sEdges = from e in Edges
                         select e.ToCurve();
            myEdges.AddRange(Curve.JoinCurves(sEdges));
            if (!(Openings == null))
            {
                foreach (var o in Openings)
                {
                    var oEdges = from e in o.Edges
                                 select e.ToCurve();
                    myEdges.AddRange(Curve.JoinCurves(oEdges));
                }
            }
            return Brep.CreatePlanarBreps(myEdges, 1)[0];
            //return Rhino.Geometry.Brep.CreateEdgeSurface(myEdges).Faces[0].Brep;
        }

        public Brep ToNurbsSurface()
        {

            var nurbs_surface = Rhino.Geometry.NurbsSurface.Create(
                    3,
                    false,
                    OrderX,
                    OrderY,
                    ControlPoints.GetLength(0),
                    ControlPoints.GetLength(1)
                    );

            // add the knots
            for (int u = 1; u < nurbs_surface.KnotsU.Count; u++)
            {
                nurbs_surface.KnotsU[u - 1] = (KnotsX[u] - KnotsX[0]) / (KnotsX[nurbs_surface.KnotsU.Count - 1] - KnotsX[0]);
            }
            nurbs_surface.KnotsU[nurbs_surface.KnotsU.Count - 1] = nurbs_surface.KnotsU[nurbs_surface.KnotsU.Count - 2];
            //nurbs_surface.KnotsU[u] = KnotsX[u];
            for (int v = 1; v < nurbs_surface.KnotsV.Count; v++)
            {
                nurbs_surface.KnotsV[v- 1] = (KnotsY[v] - KnotsY[0]) / (KnotsY[nurbs_surface.KnotsV.Count - 1] - KnotsY[0]);
            }
            nurbs_surface.KnotsV[nurbs_surface.KnotsV.Count - 1] = nurbs_surface.KnotsV[nurbs_surface.KnotsV.Count - 2];
            //nurbs_surface.KnotsV[v] = KnotsY[v];

            // add the control points
            for (int u = 0; u < nurbs_surface.Points.CountU; u++)
            {
                for (int v = 0; v < nurbs_surface.Points.CountV; v++)
                {
                    nurbs_surface.Points.SetPoint(u, v, ControlPoints[u, v]);
                }
            }

            if (nurbs_surface.IsValid)
            {
                return nurbs_surface.ToBrep();
            }
            return null;
        }


        //private Brep ToNonPlanarBrep()
        //{
        //    // Returns a Nurbs Surface of Degree 2
        //    //var srfc = Rhino.Geometry.NurbsSurface.CreateFromPoints(ControlPoints.OfType<Point3d>().ToList(), ControlPoints.GetLength(0), ControlPoints.GetLength(1), 2, 2);
        //    // return srfc.ToBrep();
        //    return Rhino.Geometry.Brep.CreateEdgeSurface(Edges.Select(x => x.ToCurve())).Faces[0].Brep;            
        //}
        public Boolean IsPlanar()
        {
            if(Brep.CreatePlanarBreps(Edges.Select(x=>x.ToCurve()), 0.0005)!=null)
            {
                return true;
            }
            return false;
            
        }
        // Convert RFEM Object into Rhino Geometry.
        // These methods are later implemented by the class GH_RFEM.
        public bool ToGH_Integer<T>(ref T target)
        {
            object obj = new GH_Integer(No);
            target = (T)obj;
            return true;
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
            object obj = new GH_Surface(ToBrep());
            target = (T)obj;  
            return true;
        }
        public bool ToGH_Brep<T>(ref T target)
        {
            object obj = new GH_Brep(ToBrep());
            target = (T)obj;
            return true;
        }
        public bool ToGH_Plane<T>(ref T target)
        {
            return false;
        }
    }
}
