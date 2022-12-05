using System;
using System.Collections.Generic;
using Dlubal.RFEM5;
using Grasshopper.Kernel.Types;
using Parametric_FEM_Toolbox.HelperLibraries;
using Rhino.Geometry;


namespace Parametric_FEM_Toolbox.RFEM
{
    [Serializable]
    public class RFLineReleaseType : IGrassRFEM
    {
          
        //Standard constructors
        public RFLineReleaseType()
        {
        }
        public RFLineReleaseType(LineReleaseType release)
        {
            Comment = release.Comment;
            ID = release.ID;
            IsValid = release.IsValid;
            No = release.No;
            Tag = release.Tag;
            RotationalConstantX = release.RotationalConstantX / 1000;
            TranslationalConstantX = release.TranslationalConstantX / 1000;
            TranslationalConstantY = release.TranslationalConstantY / 1000;
            TranslationalConstantZ = release.TranslationalConstantZ / 1000;
            RotationalNonlinearityX = release.RotationalNonlinearityX;
            TranslationalNonlinearityX=release.TranslationalNonlinearityX;
            TranslationalNonlinearityY = release.TranslationalNonlinearityY;
            TranslationalNonlinearityZ= release.TranslationalNonlinearityZ;
            ToModify = false;
            ToDelete = false;
        }

        public RFLineReleaseType(RFLineReleaseType other) : this((LineReleaseType)other)
        {
            DiagramTransX = other.DiagramTransX;
            DiagramTransY = other.DiagramTransY;
            DiagramTransZ = other.DiagramTransZ;
            DiagramRotX = other.DiagramRotX;
            PartialActivityTransX = other.PartialActivityTransX;
            PartialActivityTransY = other.PartialActivityTransY;
            PartialActivityTransZ = other.PartialActivityTransZ;
            PartialActivityRotX = other.PartialActivityRotX;
            ToModify = other.ToModify;
            ToDelete = other.ToDelete;
        }

        //Properties to Wrap Fields from RFEM Struct
        public string Comment { get; set; }
        public string ID { get; set; }
        public bool IsValid { get; set; }
        public int No { get; set; }
        public string Tag { get; set; }
        public double RotationalConstantX { get; set; }
        public double TranslationalConstantX { get; set; }
        public double TranslationalConstantY { get; set; }
        public double TranslationalConstantZ { get; set; }
        public NonlinearityType RotationalNonlinearityX { get; set; }
        public NonlinearityType TranslationalNonlinearityX { get; set; }
        public NonlinearityType TranslationalNonlinearityY { get; set; }
        public NonlinearityType TranslationalNonlinearityZ { get; set; }



        // Additional Properties to the RFEM Struct
        public bool ToModify { get; set; }
        public bool ToDelete { get; set; }
        public RFDiagram DiagramTransX { get; set; }
        public RFDiagram DiagramTransY { get; set; }
        public RFDiagram DiagramTransZ { get; set; }
        public RFDiagram DiagramRotX { get; set; }
        public RFPartialActivity PartialActivityTransX { get; set; }
        public RFPartialActivity PartialActivityTransY { get; set; }
        public RFPartialActivity PartialActivityTransZ { get; set; }
        public RFPartialActivity PartialActivityRotX { get; set; }

        // Display Info of the RFEM Objects on Panels
        // Parameters are separated by ";". The component split text can be used to break the string down into a list.
        public override string ToString()
        {
            return string.Format($"RFEM-LineReleaseType;No:{No};;" +
                $"Tag:{((Tag == "") ? "-" : Tag)};" +
                $"IsValid:{IsValid};ID:{((ID == "") ? "-" : ID)};" +
                $"ToModify:{ToModify};ToDelete:{ToDelete};Comment:{((Comment == "") ? "-" : Comment)};");
        }           

        //Operator to retrieve a Node from an rfNode.
        public static implicit operator LineReleaseType(RFLineReleaseType release)
        {
            LineReleaseType myMemberHinge = new LineReleaseType
            {
                Comment = release.Comment,
                ID = release.ID,
                IsValid = release.IsValid,
                No = release.No,
                Tag = release.Tag,
                RotationalConstantX = release.RotationalConstantX * 1000,
                TranslationalConstantX = release.TranslationalConstantX * 1000,
                TranslationalConstantY = release.TranslationalConstantY * 1000,
                TranslationalConstantZ = release.TranslationalConstantZ * 1000,
                TranslationalNonlinearityX = release.TranslationalNonlinearityX,
                TranslationalNonlinearityY = release.TranslationalNonlinearityY,
                TranslationalNonlinearityZ = release.TranslationalNonlinearityZ,
                RotationalNonlinearityX = release.RotationalNonlinearityX
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
