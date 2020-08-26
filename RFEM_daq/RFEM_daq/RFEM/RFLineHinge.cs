using System;
using System.Collections.Generic;
using Dlubal.RFEM5;
using Grasshopper.Kernel.Types;
using RFEM_daq.HelperLibraries;
using Rhino.Geometry;


namespace RFEM_daq.RFEM
{
    [Serializable]
    public class RFLineHinge : IGrassRFEM
    {
          
        //Standard constructors
        public RFLineHinge()
        {
        }
        public RFLineHinge(LineHinge hinge, RFLine baseLine)
        {
            LineNo = hinge.LineNo;
            SfcNo = hinge.SurfaceNo;
            Comment = hinge.Comment;
            ID = hinge.ID;
            IsValid = hinge.IsValid;
            No = hinge.No;
            Tag = hinge.Tag;
            Rx = hinge.RotationalConstantX/1000;
            Ry = hinge.RotationalConstantY / 1000;
            Rz = hinge.RotationalConstantZ / 1000;
            Tx = hinge.TranslationalConstantX / 1000;
            Ty = hinge.TranslationalConstantY / 1000;
            Tz = hinge.TranslationalConstantZ / 1000;
            Side = hinge.Side;
            BaseLine = baseLine;
            ToModify = false;
            ToDelete = false;
        }

        public RFLineHinge(RFLineHinge other) : this(other, null)
        {
            ToModify = other.ToModify;
            ToDelete = other.ToDelete;
            BaseLine = new RFLine(other.BaseLine);
        }

        //Properties to Wrap Fields from RFEM Struct
        public string Comment { get; set; }
        public string ID { get; set; }
        public bool IsValid { get; set; }
        public int No { get; set; }
        public string Tag { get; set; }
        public int LineNo { get; set; }
        public int SfcNo { get; set; }
        public double Tx { get; set; }
        public double Ty { get; set; }
        public double Tz { get; set; }
        public double Rx { get; set; }
        public double Ry { get; set; }
        public double Rz { get; set; }
        public Dlubal.RFEM5.HingeSideType Side { get; set; }
        // Additional Properties to the RFEM Struct
        public RFLine BaseLine { get; set; }
        public bool ToModify { get; set; }
        public bool ToDelete { get; set; }
        //public int NewNo { get; set; }

        // Display Info of the RFEM Objects on Panels
        // Parameters are separated by ";". The component split text can be used to break the string down into a list.
        public override string ToString()
        {
            return string.Format($"RFEM-LineHinge;No:{No};Ux:{Tx.DOF("kN/m")};Uy:{Ty.DOF("kN/m")};Uz:{Tz.DOF("kN/m")};" +
                $"φx:{Rx.DOF("kNm/rad")};φy:{Ry.DOF("kNm/rad")};φz:{Rz.DOF("kNm/rad")};Tag:{((Tag == "") ? "-" : Tag)};" +
                $"LineNo:{LineNo};SfcNo:{SfcNo}IsValid:{IsValid};ID:{((ID == "") ? "-" : ID)};Side:{Side.ToString()};" +
                $"ToModify:{ToModify};ToDelete:{ToDelete};Comment:{((Comment == "") ? "-" : Comment)};");
        }           

        //Operator to retrieve a Node from an rfNode.
        public static implicit operator LineHinge(RFLineHinge lineHinge)
        {
            LineHinge myLineHinge = new LineHinge
            {
                Comment = lineHinge.Comment,
                ID = lineHinge.ID,
                IsValid = lineHinge.IsValid,
                No = lineHinge.No,
                Tag = lineHinge.Tag,
                LineNo = lineHinge.LineNo,
                SurfaceNo = lineHinge.SfcNo,
                TranslationalConstantX = lineHinge.Tx*1000,
                TranslationalConstantY = lineHinge.Ty * 1000,
                TranslationalConstantZ = lineHinge.Tz * 1000,
                RotationalConstantX = lineHinge.Rx * 1000,
                RotationalConstantY = lineHinge.Ry * 1000,
                RotationalConstantZ = lineHinge.Rz * 1000,
                Side = lineHinge.Side
             };
            return myLineHinge;
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
            return BaseLine.ConvertToGH_Line(ref target);
        }
        public bool ToGH_Curve<T>(ref T target)
        {
            return BaseLine.ConvertToGH_Curve(ref target);
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
