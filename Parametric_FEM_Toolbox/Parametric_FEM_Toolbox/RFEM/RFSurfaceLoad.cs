using System;
using System.Collections.Generic;
using Dlubal.RFEM5;
using Grasshopper.Kernel.Types;
using Parametric_FEM_Toolbox.HelperLibraries;
using Rhino.Geometry;


namespace Parametric_FEM_Toolbox.RFEM
{
    [Serializable]
    public class RFSurfaceLoad : IGrassRFEM
    {
          
        //Standard constructors
        public RFSurfaceLoad()
        {
        }
        public RFSurfaceLoad(SurfaceLoad load, int loadcase)
        {
            SurfaceList = load.SurfaceList;
            Comment = load.Comment;
            ID = load.ID;
            IsValid = load.IsValid;
            No = load.No;
            Tag = load.Tag;
            LoadDirType = load.Direction;
            LoadDistType = load.Distribution;
            LoadType = load.Type;
            Node1No = load.Node1No;
            Node2No = load.Node2No;
            Node3No = load.Node3No;
            Magnitude1 = load.Magnitude1;
            Magnitude2 = load.Magnitude2;
            Magnitude3 = load.Magnitude3;
            Magnitude4 = load.Magnitude4;
            Magnitude5 = load.Magnitude5;
            Magnitude6 = load.Magnitude6;
            if (LoadType == LoadType.ForceType )
            {
                Magnitude1 /=  1000;
                Magnitude2 /=  1000;
                Magnitude3 /=  1000;
            }
            LoadCase = loadcase;
            ToModify = false;
            ToDelete = false;
        }

        public RFSurfaceLoad(RFSurfaceLoad other) : this(other, other.LoadCase)
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
        public int Node1No { get; set; }
        public int Node2No { get; set; }
        public int Node3No { get; set; }
        public double Magnitude1 { get; set; }
        public double Magnitude2 { get; set; }
        public double Magnitude3 { get; set; }
        public double Magnitude4 { get; set; }
        public double Magnitude5 { get; set; }
        public double Magnitude6 { get; set; }
        public LoadDirectionType LoadDirType { get; set; }
        public LoadDistributionType LoadDistType { get; set; }
        public LoadType LoadType { get; set; }
        // Additional Properties to the RFEM Struct
        public int LoadCase { get; set; }
        public bool ToModify { get; set; }
        public bool ToDelete { get; set; }
        //public int NewNo { get; set; }

        // Display Info of the RFEM Objects on Panels
        // Parameters are separated by ";". The component split text can be used to break the string down into a list.
        public override string ToString()
        {
            return string.Format($"RFEM-SurfaceLoad;No:{No};LoadCase:{LoadCase};" +
                 $"F1:{Magnitude1.ToString("0.00")}[kN/m²];F2:{Magnitude2.ToString("0.00")}[kN/m²];F3:{Magnitude3.ToString("0.00")}[kN/m²];" +
                $"T4:{Magnitude4.ToString("0.00")};T5:{Magnitude5.ToString("0.00")};T6:{Magnitude6.ToString("0.00")};" +
                $"Node1No:{Node1No.ToString()};Node2No:{Node2No.ToString()};Node3No:{Node3No.ToString()};" +
                $"LoadType:{LoadType.ToString()};LoadDistType:{LoadDistType.ToString()};LoadDirType:{LoadDirType.ToString()};" +
                $"SurfaceList:{((SurfaceList == "") ? "-" : SurfaceList)};IsValid:{IsValid};ID:{((ID == "") ? "-" : ID)};Tag:{((Tag == "") ? "-" : Tag)};" +
                $"ToModify:{ToModify};ToDelete:{ToDelete};Comment:{((Comment == "") ? "-" : Comment)};");
        }           

        //Operator to retrieve a Node from an rfNode.
        public static implicit operator SurfaceLoad(RFSurfaceLoad load)
        {
            SurfaceLoad myLoad = new SurfaceLoad
            {
                Comment = load.Comment,
                ID = load.ID,
                IsValid = load.IsValid,
                No = load.No,
                Tag = load.Tag,
                SurfaceList = load.SurfaceList,
             };
            myLoad.Type = load.LoadType;
            myLoad.Direction = load.LoadDirType;
            myLoad.Distribution = load.LoadDistType;
            myLoad.Node1No = load.Node1No;
            myLoad.Node2No = load.Node2No;
            myLoad.Node3No = load.Node3No;
            if (load.LoadType == LoadType.ForceType)
            {
                myLoad.Magnitude1 = load.Magnitude1 * 1000;
                myLoad.Magnitude2 = load.Magnitude2 * 1000;
                myLoad.Magnitude3 = load.Magnitude3 * 1000;
            }
            else
            {
                myLoad.Magnitude1 = load.Magnitude1;
                myLoad.Magnitude2 = load.Magnitude2;
                myLoad.Magnitude3 = load.Magnitude3;
                myLoad.Magnitude4 = load.Magnitude4;
                myLoad.Magnitude5 = load.Magnitude5;
                myLoad.Magnitude6 = load.Magnitude6;
            }
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
