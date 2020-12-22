﻿using System;
using System.Collections.Generic;
using Dlubal.RFEM5;
using Grasshopper.Kernel.Types;
using Parametric_FEM_Toolbox.HelperLibraries;
using Rhino.Geometry;


namespace Parametric_FEM_Toolbox.RFEM
{
    [Serializable]
    public class RFMemberHinge : IGrassRFEM
    {
          
        //Standard constructors
        public RFMemberHinge()
        {
        }
        public RFMemberHinge(MemberHinge hinge)
        {
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
            ToModify = false;
            ToDelete = false;
        }

        public RFMemberHinge(RFMemberHinge other) : this((MemberHinge)other)
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
        public double Tx { get; set; }
        public double Ty { get; set; }
        public double Tz { get; set; }
        public double Rx { get; set; }
        public double Ry { get; set; }
        public double Rz { get; set; }
        // Additional Properties to the RFEM Struct
        public bool ToModify { get; set; }
        public bool ToDelete { get; set; }
        //public int NewNo { get; set; }

        // Display Info of the RFEM Objects on Panels
        // Parameters are separated by ";". The component split text can be used to break the string down into a list.
        public override string ToString()
        {
            return string.Format($"RFEM-MemberHinge;No:{No};Ux:{Tx.DOF("kN/m")};Uy:{Ty.DOF("kN/m")};Uz:{Tz.DOF("kN/m")};" +
                $"φx:{Rx.DOF("kNm/rad")};φy:{Ry.DOF("kNm/rad")};φz:{Rz.DOF("kNm/rad")};Tag:{((Tag == "") ? "-" : Tag)};" +
                $"IsValid:{IsValid};ID:{((ID == "") ? "-" : ID)};" +
                $"ToModify:{ToModify};ToDelete:{ToDelete};Comment:{((Comment == "") ? "-" : Comment)};");
        }           

        //Operator to retrieve a Node from an rfNode.
        public static implicit operator MemberHinge(RFMemberHinge memberHinge)
        {
            MemberHinge myMemberHinge = new MemberHinge
            {
                Comment = memberHinge.Comment,
                ID = memberHinge.ID,
                IsValid = memberHinge.IsValid,
                No = memberHinge.No,
                Tag = memberHinge.Tag,
                TranslationalConstantX = memberHinge.Tx*1000,
                TranslationalConstantY = memberHinge.Ty * 1000,
                TranslationalConstantZ = memberHinge.Tz * 1000,
                RotationalConstantX = memberHinge.Rx * 1000,
                RotationalConstantY = memberHinge.Ry * 1000,
                RotationalConstantZ = memberHinge.Rz * 1000,
             };
            return myMemberHinge;
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
