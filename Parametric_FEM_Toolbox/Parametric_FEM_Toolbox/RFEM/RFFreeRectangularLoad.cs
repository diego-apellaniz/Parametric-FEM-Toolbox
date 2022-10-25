using System;
using System.Collections.Generic;
using Dlubal.RFEM5;
using Grasshopper.Kernel.Types;
using Parametric_FEM_Toolbox.HelperLibraries;
using Rhino.Geometry;
using System.Linq;


namespace Parametric_FEM_Toolbox.RFEM
{
    [Serializable]
    public class RFFreeRectangularLoad : IGrassRFEM
    {
          
        //Standard constructors
        public RFFreeRectangularLoad()
        {
        }
        public RFFreeRectangularLoad(FreeRectangularLoad load, int loadcase)
        {
            SurfaceList = load.SurfaceList;
            Comment = load.Comment;
            ID = load.ID;
            IsValid = load.IsValid;
            No = load.No;
            Tag = load.Tag;
            LoadDirType = load.Direction;
            LoadDistType = load.Distribution;
            ProjectionType = load.ProjectionPlane;
            Magnitude1 = load.Magnitude1/1000;
            Magnitude2 = load.Magnitude2/1000;
            Position1 = load.Position1.ToPoint3d();
            Position2 = load.Position2.ToPoint3d();
            LoadCase = loadcase;
            ToModify = false;
            ToDelete = false;
        }

        public RFFreeRectangularLoad(RFFreeRectangularLoad other) : this(other, other.LoadCase)
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
        public Point3d Position1    { get; set; }
        public Point3d Position2 { get; set; }

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
                 $"F1:{Magnitude1.ToString("0.00")}[kN/m²];F2:{Magnitude2.ToString("0.00")}[kN/m²];" +
                $"ProjectionType:{ProjectionType.ToString()};LoadDistType:{LoadDistType.ToString()};LoadDirType:{LoadDirType.ToString()};" +
                $"SurfaceList:{((SurfaceList == "") ? "-" : SurfaceList)};" +
                $"IsValid:{IsValid};ID:{((ID == "") ? "-" : ID)};Tag:{((Tag == "") ? "-" : Tag)};" +
                $"ToModify:{ToModify};ToDelete:{ToDelete};Comment:{((Comment == "") ? "-" : Comment)};");
        }           

        //Operator to retrieve a Node from an rfNode.
        public static implicit operator FreeRectangularLoad(RFFreeRectangularLoad load)
        {
            FreeRectangularLoad myLoad = new FreeRectangularLoad
            {
                Comment = load.Comment,
                ID = load.ID,
                IsValid = load.IsValid,
                No = load.No,
                Tag = load.Tag,
                SurfaceList = load.SurfaceList
            };
            myLoad.ProjectionPlane = load.ProjectionType;
            myLoad.Direction = load.LoadDirType;
            myLoad.Distribution = load.LoadDistType;
            myLoad.Position1 = load.Position1.ToPoint3D();
            myLoad.Position2 = load.Position2.ToPoint3D();
            myLoad.Magnitude1 = load.Magnitude1 * 1000;
            myLoad.Magnitude2 = load.Magnitude2 * 1000;

            return myLoad;
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
