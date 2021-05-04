using System;
using System.Collections.Generic;
using Dlubal.RFEM5;
using Grasshopper.Kernel.Types;
using Parametric_FEM_Toolbox.HelperLibraries;
using Rhino.Geometry;


namespace Parametric_FEM_Toolbox.RFEM
{
    [Serializable]
    public class RFLineLoad : IGrassRFEM
    {
          
        //Standard constructors
        public RFLineLoad()
        {
        }
        public RFLineLoad(LineLoad load, List<RFLine> baselines, int loadcase)
        {
            LineList = load.LineList;
            Comment = load.Comment;
            ID = load.ID;
            IsValid = load.IsValid;
            No = load.No;
            Tag = load.Tag;
            DistanceA = load.DistanceA;
            DistanceB = load.DistanceB;
            if (load.RelativeDistances)
            {
                DistanceA /= 100;
                DistanceB /= 100;
            }
            Magnitude1 = load.Magnitude1/1000;
            Magnitude2 = load.Magnitude2;
            if (load.Distribution != LoadDistributionType.ConcentratedNxQType && load.Distribution != LoadDistributionType.Concentrated2x2QType)
            {
                Magnitude2 /= 1000;
            }
                Magnitude3 = load.Magnitude3/1000;
            LoadArray = load.LoadArray;
            OverTotalLength = load.OverTotalLength;
            RelativeDistances = load.RelativeDistances;
            LoadDirType = load.Direction;
            LoadDistType = load.Distribution;
            LoadRefType = load.ReferenceTo;
            LoadType = load.Type;
            LoadCase = loadcase;
            BaseLines = baselines;
            ToModify = false;
            ToDelete = false;
        }

        public RFLineLoad(RFLineLoad other) : this(other, other.BaseLines, other.LoadCase)
        {
            ToModify = other.ToModify;
            ToDelete = other.ToDelete;
            var newBaseLines = new List<RFLine>();
            foreach (var edge in other.BaseLines)
            {
                newBaseLines.Add(new RFLine(edge));
            }
            BaseLines = newBaseLines;
        }

        //Properties to Wrap Fields from RFEM Struct
        public string Comment { get; set; }
        public string ID { get; set; }
        public bool IsValid { get; set; }
        public int No { get; set; }
        public string Tag { get; set; }
        public string LineList { get; set; }
        public double DistanceA { get; set; }
        public double DistanceB { get; set; }
        public double Magnitude1 { get; set; }
        public double Magnitude2 { get; set; }
        public double Magnitude3 { get; set; }
        public double[,] LoadArray { get; set; }
        public bool OverTotalLength { get; set; }
        public bool RelativeDistances { get; set; }
        public LoadDirectionType LoadDirType { get; set; }
        public LoadDistributionType LoadDistType { get; set; }
        public LineLoadReferenceType LoadRefType { get; set; }
        public LoadType LoadType { get; set; }
        // Additional Properties to the RFEM Struct
        public int LoadCase { get; set; }
        public List<RFLine> BaseLines { get; set; }
        public bool ToModify { get; set; }
        public bool ToDelete { get; set; }
        //public int NewNo { get; set; }

        // Display Info of the RFEM Objects on Panels
        // Parameters are separated by ";". The component split text can be used to break the string down into a list.
        public override string ToString()
        {
            return string.Format($"RFEM-LineLoad;No:{No};LoadCase:{LoadCase};" +
                $"F1:{Magnitude1.ToString("0.00")}[kN/m]|[kN];F2:{Magnitude2.ToString("0.00")}[kN/m]|[m]|[-];F3:{Magnitude3.ToString("0.00")}[kN/m];" +
                 $"t1:{DistanceA.ToString("0.00")}[-];t2:{DistanceB.ToString("0.00")}[-];" +
                $"OverTotalLength:{OverTotalLength.ToString()};RelativeDistances:{RelativeDistances.ToString()};" +
                $"LoadArray:{LoadArray.ToLabelString()}" +
                $"LoadType:{LoadType.ToString()};LoadDistType:{LoadDistType.ToString()};LoadDirType:{LoadDirType.ToString()};LoadRefType:{LoadRefType.ToString()};" +
                $"LineList:{((LineList == "") ? "-" : LineList)};IsValid:{IsValid};ID:{((ID == "") ? "-" : ID)};Tag:{((Tag == "") ? "-" : Tag)};" +
                $"ToModify:{ToModify};ToDelete:{ToDelete};Comment:{((Comment == "") ? "-" : Comment)};");
        }           

        //Operator to retrieve a Node from an rfNode.
        public static implicit operator LineLoad(RFLineLoad load)
        {
            LineLoad myLoad = new LineLoad
            {
                Comment = load.Comment,
                ID = load.ID,
                IsValid = load.IsValid,
                No = load.No,
                Tag = load.Tag,
                LineList = load.LineList,
             };
            myLoad.OverTotalLength = load.OverTotalLength;
            myLoad.RelativeDistances = load.RelativeDistances;
            myLoad.Direction = load.LoadDirType;
            myLoad.Distribution = load.LoadDistType;
            myLoad.ReferenceTo = load.LoadRefType;
            myLoad.DistanceA = load.DistanceA;
            myLoad.DistanceB = load.DistanceB;
            if (myLoad.RelativeDistances)
            {
                myLoad.DistanceA *= 100;
                myLoad.DistanceB *= 100;
            }
            myLoad.Magnitude1 = load.Magnitude1*1000;
            myLoad.Magnitude2 = load.Magnitude2;
            if (myLoad.Distribution != LoadDistributionType.ConcentratedNxQType && myLoad.Distribution != LoadDistributionType.Concentrated2x2QType)
            {
                myLoad.Magnitude2 *= 1000;
            }            
            myLoad.Magnitude3 = load.Magnitude3*1000;
            myLoad.LoadArray = load.LoadArray;
            myLoad.Type  = load.LoadType;

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
            if (BaseLines.Count == 1)
            {
                return BaseLines[0].ConvertToGH_Line(ref target);
            }
            return false;
        }
        public bool ToGH_Curve<T>(ref T target)
        {
            if (BaseLines.Count == 1)
            {
                return BaseLines[0].ConvertToGH_Curve(ref target);
            }
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
