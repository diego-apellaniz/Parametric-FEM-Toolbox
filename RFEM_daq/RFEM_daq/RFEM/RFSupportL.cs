using System;
using System.Collections.Generic;
using Dlubal.RFEM5;
using Grasshopper.Kernel.Types;
using RFEM_daq.HelperLibraries;
using Rhino.Geometry;


namespace RFEM_daq.RFEM
{
    [Serializable]
    public class RFSupportL : IGrassRFEM
    {
          
        //Standard constructors
        public RFSupportL()
        {
        }
        public RFSupportL(LineSupport support, List<RFLine> baseLines)
        {
            LineList = support.LineList;
            Comment = support.Comment;
            ID = support.ID;
            IsValid = support.IsValid;
            No = support.No;
            Tag = support.Tag;
            Rx = support.RestraintConstantX/1000;
            Ry = support.RestraintConstantY / 1000;
            Rz = support.RestraintConstantZ / 1000;
            Tx = support.SupportConstantX / 1000;
            Ty = support.SupportConstantY / 1000;
            Tz = support.SupportConstantZ / 1000;
            RSType = support.ReferenceSystem;
            BaseLines = baseLines;
            ToModify = false;
            ToDelete = false;
        }

        public RFSupportL(RFSupportL other) : this(other, null)
        {
            ToModify = other.ToModify;
            ToDelete = other.ToDelete;
            var newEdges = new List<RFLine>();
            foreach (var edge in other.BaseLines)
            {
                newEdges.Add(new RFLine(edge));
            }
            BaseLines = newEdges;
        }

        //Properties to Wrap Fields from RFEM Struct
        public string Comment { get; set; }
        public string ID { get; set; }
        public bool IsValid { get; set; }
        public int No { get; set; }
        public string Tag { get; set; }
        public string LineList { get; set; }
        public ReferenceSystemType RSType { get; set; }
        public double Tx { get; set; }
        public double Ty { get; set; }
        public double Tz { get; set; }
        public double Rx { get; set; }
        public double Ry { get; set; }
        public double Rz { get; set; }
        // Additional Properties to the RFEM Struct
        public List<RFLine> BaseLines { get; set; }
        public bool ToModify { get; set; }
        public bool ToDelete { get; set; }
        //public int NewNo { get; set; }

        // Display Info of the RFEM Objects on Panels
        // Parameters are separated by ";". The component split text can be used to break the string down into a list.
        public override string ToString()
        {
            return string.Format($"RFEM-LineSupport;No:{No};Ux:{Tx.DOF("kN/m")};Uy:{Ty.DOF("kN/m")};Uz:{Tz.DOF("kN/m")};" +
                $"φx:{Rx.DOF("kNm/rad")};φy:{Ry.DOF("kNm/rad")};φz:{Rz.DOF("kNm/rad")};Tag:{((Tag == "") ? "-" : Tag)};" +
                $"LineList:{((LineList == "") ? "-" : LineList)};IsValid:{IsValid};ID:{((ID == "") ? "-" : ID)};ReferenceSystemType:{RSType.ToString()};" +
                $"ToModify:{ToModify};ToDelete:{ToDelete};Comment:{((Comment == "") ? "-" : Comment)};");
        }           

        //Operator to retrieve a Node from an rfNode.
        public static implicit operator LineSupport(RFSupportL support)
        {
            LineSupport mySupport = new LineSupport
            {
                Comment = support.Comment,
                ID = support.ID,
                IsValid = support.IsValid,
                No = support.No,
                Tag = support.Tag,
                LineList = support.LineList,
                SupportConstantX = support.Tx*1000,
                SupportConstantY = support.Ty * 1000,
                SupportConstantZ = support.Tz * 1000,
                RestraintConstantX = support.Rx * 1000,
                RestraintConstantY = support.Ry * 1000,
                RestraintConstantZ = support.Rz * 1000,
                ReferenceSystem = support.RSType
             };
            return mySupport;
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
