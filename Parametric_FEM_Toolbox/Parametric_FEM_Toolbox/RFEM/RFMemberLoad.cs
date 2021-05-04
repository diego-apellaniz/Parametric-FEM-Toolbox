using System;
using System.Collections.Generic;
using Dlubal.RFEM5;
using Grasshopper.Kernel.Types;
using Parametric_FEM_Toolbox.HelperLibraries;
using Rhino.Geometry;


namespace Parametric_FEM_Toolbox.RFEM
{
    [Serializable]
    public class RFMemberLoad : IGrassRFEM
    {
          
        //Standard constructors
        public RFMemberLoad()
        {
        }
        public RFMemberLoad(MemberLoad load, int loadcase)
        {
            MemberList = load.ObjectList;
            Comment = load.Comment;
            ID = load.ID;
            IsValid = load.IsValid;
            No = load.No;
            Tag = load.Tag;
            LoadDirType = load.Direction;
            LoadDistType = load.Distribution;
            LoadRefType = load.ReferenceTo;
            LoadType = load.Type;
            DistanceA = load.DistanceA;
            DistanceB = load.DistanceB;
            if (load.RelativeDistances)
            {
                DistanceA /= 100;
                DistanceB /= 100;
            }
            if (LoadType == LoadType.ForceType || LoadType == LoadType.MomentType || LoadType == LoadType.InitialPrestressType || LoadType == LoadType.EndPrestressType)
            {
                Magnitude1 = load.Magnitude1 / 1000;
                Magnitude2 = load.Magnitude2;
                if (load.Distribution != LoadDistributionType.ConcentratedNxQType && load.Distribution != LoadDistributionType.Concentrated2x2QType)
                {
                    Magnitude2 /= 1000;
                }
                Magnitude3 = load.Magnitude3 / 1000;
            }
            else
            {
                Magnitude1 = load.Magnitude1;
                Magnitude2 = load.Magnitude2;
                Magnitude3 = load.Magnitude3;
                Magnitude4 = load.Magnitude4;
                Magnitude5 = load.Magnitude5;
                Magnitude6 = load.Magnitude6;
            }
            LoadArray = load.LoadArray;
            OverTotalLength = load.OverTotalLength;
            RelativeDistances = load.RelativeDistances;
            LoadCase = loadcase;
            ToModify = false;
            ToDelete = false;
        }

        public RFMemberLoad(RFMemberLoad other) : this(other, other.LoadCase)
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
        public string MemberList { get; set; }
        public double DistanceA { get; set; }
        public double DistanceB { get; set; }
        public double Magnitude1 { get; set; }
        public double Magnitude2 { get; set; }
        public double Magnitude3 { get; set; }
        public double Magnitude4 { get; set; }
        public double Magnitude5 { get; set; }
        public double Magnitude6 { get; set; }
        public double[,] LoadArray { get; set; }
        public bool OverTotalLength { get; set; }
        public bool RelativeDistances { get; set; }
        public LoadDirectionType LoadDirType { get; set; }
        public LoadDistributionType LoadDistType { get; set; }
        public MemberLoadReferenceType LoadRefType { get; set; }
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
            return string.Format($"RFEM-MemberLoad;No:{No};LoadCase:{LoadCase};" +
                 $"F1:{Magnitude1.ToString("0.00")}[kN/m]|[kN];F2:{Magnitude2.ToString("0.00")}[kN/m]|[m]|[-];F3:{Magnitude3.ToString("0.00")}[kN/m];" +
                $"T4:{Magnitude4.ToString("0.00")};T5:{Magnitude5.ToString("0.00")};T6:{Magnitude6.ToString("0.00")};" +
                $"t1:{DistanceA.ToString("0.00")}[-];t2:{DistanceB.ToString("0.00")}[-];" +
                $"OverTotalLength:{OverTotalLength.ToString()};RelativeDistances:{RelativeDistances.ToString()};" +
                $"LoadArray:{LoadArray.ToLabelString()}" +
                $"LoadType:{LoadType.ToString()};LoadDistType:{LoadDistType.ToString()};LoadDirType:{LoadDirType.ToString()};LoadRefType:{LoadRefType.ToString()};" +
                $"MemberList:{((MemberList == "") ? "-" : MemberList)};IsValid:{IsValid};ID:{((ID == "") ? "-" : ID)};Tag:{((Tag == "") ? "-" : Tag)};" +
                $"ToModify:{ToModify};ToDelete:{ToDelete};Comment:{((Comment == "") ? "-" : Comment)};");
        }           

        //Operator to retrieve a Node from an rfNode.
        public static implicit operator MemberLoad(RFMemberLoad load)
        {
            MemberLoad myLoad = new MemberLoad
            {
                Comment = load.Comment,
                ID = load.ID,
                IsValid = load.IsValid,
                No = load.No,
                Tag = load.Tag,
                ObjectList = load.MemberList,
             };
            myLoad.OverTotalLength = load.OverTotalLength;
            myLoad.RelativeDistances = load.RelativeDistances;
            myLoad.Type = load.LoadType;
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
            if (load.LoadType == LoadType.ForceType || load.LoadType == LoadType.MomentType || load.LoadType == LoadType.InitialPrestressType || load.LoadType == LoadType.EndPrestressType)
            {
                myLoad.Magnitude1 = load.Magnitude1 * 1000;
                if (myLoad.Distribution != LoadDistributionType.ConcentratedNxQType && myLoad.Distribution != LoadDistributionType.Concentrated2x2QType)
                {
                    myLoad.Magnitude2 = load.Magnitude2 * 1000;
                }
                else
                {
                    myLoad.Magnitude2 = load.Magnitude2;
                }
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
            myLoad.LoadArray = load.LoadArray;            

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
