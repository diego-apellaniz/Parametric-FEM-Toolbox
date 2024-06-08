using System;
using System.Collections.Generic;
using Dlubal.RFEM5;
using Grasshopper.Kernel.Types;
using Parametric_FEM_Toolbox.HelperLibraries;
using Rhino.Geometry;


namespace Parametric_FEM_Toolbox.RFEM
{
    [Serializable]
    public class RFLineRelease : IGrassRFEM
    {
          
        //Standard constructors
        public RFLineRelease()
        {
        }
        public RFLineRelease(LineRelease release)
        {
            Comment = release.Comment;
            ID = release.ID;
            IsValid = release.IsValid;
            No = release.No;
            Tag = release.Tag;
            LineNo = release.LineNo;
            AxisSystem = release.AxisSystem;
            AxisSystemFromObjectNo = release.AxisSystemFromObjectNo;
            GeneratedLineNo = release.GeneratedLineNo;
            Location = release.Location;
            ReleasedMembers = release.ReleasedMembers;
            ReleasedSolids = release.ReleasedSolids;
            ReleasedSurfaces = release.ReleasedSurfaces;
            Rotation = release.Rotation;
            DefinitionNodes = release.DefinitionNodes;
            HelpNodePlane = release.HelpNodePlane;
            TypeNo = release.TypeNo;
            ToModify = false;
            ToDelete = false;
        }

        public RFLineRelease(RFLineRelease other) : this((LineRelease)other)
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
        public int LineNo { get; set; }
        public LineReleaseAxisSystem AxisSystem { get; set; }
        public int AxisSystemFromObjectNo { get; set; }
        public int GeneratedLineNo { get; set; }
        public ReleaseLocation Location { get; set; }
        public string ReleasedMembers { get; set; }
        public string ReleasedSolids { get; set; }
        public string ReleasedSurfaces { get; set; }
        public double Rotation { get; set; }
        public PlaneType HelpNodePlane { get; set; }
        public string DefinitionNodes { get; set; }
        public int TypeNo { get; set; }


        // Additional Properties to the RFEM Struct
        public bool ToModify { get; set; }
        public bool ToDelete { get; set; }
        //public int NewNo { get; set; }

        // Display Info of the RFEM Objects on Panels
        // Parameters are separated by ";". The component split text can be used to break the string down into a list.
        public override string ToString()
        {
            return string.Format($"RFEM-LineRelease;No:{No};LineNo:{LineNo};TypeNo:{TypeNo};Members:{ReleasedMembers};Surfaces:{ReleasedSolids};Solids:{ReleasedSurfaces};" +
                $"Tag:{((Tag == "") ? "-" : Tag)};" +
                $"IsValid:{IsValid};ID:{((ID == "") ? "-" : ID)};" +
                $"ToModify:{ToModify};ToDelete:{ToDelete};Comment:{((Comment == "") ? "-" : Comment)};");
        }           

        //Operator to retrieve a Node from an rfNode.
        public static implicit operator LineRelease(RFLineRelease release)
        {
            LineRelease myMemberHinge = new LineRelease
            {
                Comment = release.Comment,
                ID = release.ID,
                IsValid = release.IsValid,
                No = release.No,
                Tag = release.Tag,
                LineNo = release.LineNo,
                AxisSystem = release.AxisSystem,
                AxisSystemFromObjectNo = release.AxisSystemFromObjectNo,
                GeneratedLineNo = release.GeneratedLineNo,
                Location = release.Location,
                ReleasedMembers = release.ReleasedMembers,
                ReleasedSolids = release.ReleasedSolids,
                ReleasedSurfaces = release.ReleasedSurfaces,
                Rotation = release.Rotation,
                DefinitionNodes = release.DefinitionNodes,
                HelpNodePlane = release.HelpNodePlane,
                TypeNo = release.TypeNo,
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
