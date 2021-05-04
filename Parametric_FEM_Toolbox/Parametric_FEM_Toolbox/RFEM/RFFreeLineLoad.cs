using System;
using System.Collections.Generic;
using Dlubal.RFEM5;
using Grasshopper.Kernel.Types;
using Parametric_FEM_Toolbox.HelperLibraries;
using Rhino.Geometry;


namespace Parametric_FEM_Toolbox.RFEM
{
    [Serializable]
    public class RFFreeLineLoad : IGrassRFEM
    {
          
        //Standard constructors
        public RFFreeLineLoad()
        {
        }
        public RFFreeLineLoad(FreeLineLoad load, int loadcase)
        {
            SurfaceList = load.SurfaceList;
            Comment = load.Comment;
            ID = load.ID;
            IsValid = load.IsValid;
            No = load.No;
            Tag = load.Tag;
            Position1 = load.Position1.ToPoint3d();
            Position2 = load.Position2.ToPoint3d();
            Magnitude1 = load.Magnitude1/1000;
            Magnitude2 = load.Magnitude2/1000;
            LoadDirType = load.Direction;
            LoadDistType = load.Distribution;
            ProjectionPlane = load.ProjectionPlane;
            LoadCase = loadcase;
            ToModify = false;
            ToDelete = false;
        }

        public RFFreeLineLoad(RFFreeLineLoad other) : this(other, other.LoadCase)
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
        public double Magnitude1 { get; set; }
        public double Magnitude2 { get; set; }
        public Point3d Position1 { get; set; }
        public Point3d Position2 { get; set; }
        public LoadDirectionType LoadDirType { get; set; }
        public LoadDistributionType LoadDistType { get; set; }
        public PlaneType ProjectionPlane { get; set; }
        // Additional Properties to the RFEM Struct
        public int LoadCase { get; set; }
        public bool ToModify { get; set; }
        public bool ToDelete { get; set; }
        //public int NewNo { get; set; }

        // Display Info of the RFEM Objects on Panels
        // Parameters are separated by ";". The component split text can be used to break the string down into a list.
        public override string ToString()
        {
            return string.Format($"RFEM-FreeLineLoad;No:{No};LoadCase:{LoadCase};" +                
                $"F1:{Magnitude1.ToString("0.00")}[kN/m]|[kN];F2:{Magnitude2.ToString("0.00")}[kN/m]|[m]|[-];" +
                $"P1x:{Position1.X.ToString("0.00")}[kN];P1y:{Position1.Y.ToString("0.00")}[kN];P1z:{Position1.Z.ToString("0.00")}[kN];" +
                $"P2x:{Position2.X.ToString("0.00")}[kN];P2y:{Position2.Y.ToString("0.00")}[kN];P2z:{Position2.Z.ToString("0.00")}[kN];" +
                $"LoadDistType:{LoadDistType.ToString()};LoadDirType:{LoadDirType.ToString()};ProjectionPlane:{ProjectionPlane.ToString()};" +
                $"SurfaceList:{((SurfaceList == "") ? "-" : SurfaceList)};IsValid:{IsValid};ID:{((ID == "") ? "-" : ID)};Tag:{((Tag == "") ? "-" : Tag)};" +
                $"ToModify:{ToModify};ToDelete:{ToDelete};Comment:{((Comment == "") ? "-" : Comment)};");
        }           

        //Operator to retrieve a Node from an rfNode.
        public static implicit operator FreeLineLoad(RFFreeLineLoad load)
        {
            FreeLineLoad myLoad = new FreeLineLoad
            {
                Comment = load.Comment,
                ID = load.ID,
                IsValid = load.IsValid,
                No = load.No,
                Tag = load.Tag,
                SurfaceList = load.SurfaceList,
             };
            myLoad.Position1 = load.Position1.ToPoint3D();
            myLoad.Position2 = load.Position2.ToPoint3D();
            myLoad.Direction = load.LoadDirType;
            myLoad.Distribution = load.LoadDistType;
            myLoad.Magnitude1 = load.Magnitude1*1000;
            myLoad.Magnitude2 = load.Magnitude2*1000;
            myLoad.ProjectionPlane  = load.ProjectionPlane;
            return myLoad;
        }

        public Rhino.Geometry.Line ToLine()
        {
            return new Rhino.Geometry.Line(Position1, Position2);
        }

        public bool ConvertToGH_Line<T>(ref T target)
        {
            var crv = ToLine();
            object obj = new GH_Line(crv);
            target = (T)obj;
            return true;
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
            return ConvertToGH_Line(ref target);
        }
        public bool ToGH_Curve<T>(ref T target)
        {
            return false;
        }
        public bool ToGH_Surface<T>(ref T target)
        {
            return false;
        }
        public bool ToGH_Brep<T>(ref T target)
        {
            return false;
        }
        public bool ToGH_Plane<T>(ref T target)
        {
            return false;
        }

    }
}
