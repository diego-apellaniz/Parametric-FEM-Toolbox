using System;
using System.Collections.Generic;
using Dlubal.RFEM5;
using Grasshopper.Kernel.Types;
using Parametric_FEM_Toolbox.HelperLibraries;
using Rhino.Geometry;


namespace Parametric_FEM_Toolbox.RFEM
{
    [Serializable]
    public class RFNodalRelease : IGrassRFEM
    {
          
        //Standard constructors
        public RFNodalRelease()
        {
        }
        public RFNodalRelease(NodalRelease release)
        {
            Comment = release.Comment;
            ID = release.ID;
            IsValid = release.IsValid;
            No = release.No;
            Tag = release.Tag;
            NodeNo = release.NodeNo;
            AxisSystem = release.AxisSystem;
            AxisSystemFromObjectNo = release.AxisSystemFromObjectNo;
            GeneratedNodeNo = release.GeneratedNodeNo;
            Location = release.Location;
            MemberHingeNo = release.MemberHingeNo;
            ReleasedMembers = release.ReleasedMembers;
            ReleasedSolids = release.ReleasedSolids;
            ReleasedSurfaces = release.ReleasedSurfaces;
            ToModify = false;
            ToDelete = false;
        }

        public RFNodalRelease(RFNodalRelease other) : this((NodalRelease)other)
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
        public int NodeNo { get; set; }
        public NodalReleaseAxisSystem AxisSystem { get; set; }
        public int AxisSystemFromObjectNo { get; set; }
        public int GeneratedNodeNo { get; set; }
        public ReleaseLocation Location { get; set; }
        public int MemberHingeNo { get; set; }
        public string ReleasedMembers { get; set; }
        public string ReleasedSolids { get; set; }
        public string ReleasedSurfaces { get; set; }

        // Additional Properties to the RFEM Struct
        public bool ToModify { get; set; }
        public bool ToDelete { get; set; }
        //public int NewNo { get; set; }

        // Display Info of the RFEM Objects on Panels
        // Parameters are separated by ";". The component split text can be used to break the string down into a list.
        public override string ToString()
        {
            return string.Format($"RFEM-NodalRelease;No:{No};NodeNo:{NodeNo};Members:{ReleasedMembers};Surfaces:{ReleasedSolids};Solids:{ReleasedSurfaces};" +
                $"MemberHingeNo:{MemberHingeNo};NodalReleaseAxisSystem:{AxisSystem};AxisSystemFromObjectNo:{AxisSystemFromObjectNo};GeneratedNodeNo:{GeneratedNodeNo};Tag:{((Tag == "") ? "-" : Tag)};" +
                $"IsValid:{IsValid};ID:{((ID == "") ? "-" : ID)};" +
                $"ToModify:{ToModify};ToDelete:{ToDelete};Comment:{((Comment == "") ? "-" : Comment)};");
        }           

        //Operator to retrieve a Node from an rfNode.
        public static implicit operator NodalRelease(RFNodalRelease release)
        {
            NodalRelease myMemberHinge = new NodalRelease
            {
                Comment = release.Comment,
                ID = release.ID,
                IsValid = release.IsValid,
                No = release.No,
                Tag = release.Tag,
                NodeNo = release.NodeNo,
                AxisSystem = release.AxisSystem,
                AxisSystemFromObjectNo = release.AxisSystemFromObjectNo,
                GeneratedNodeNo = release.GeneratedNodeNo,
                Location = release.Location,
                MemberHingeNo = release.MemberHingeNo,
                ReleasedMembers = release.ReleasedMembers,
                ReleasedSolids = release.ReleasedSolids,
                ReleasedSurfaces = release.ReleasedSurfaces
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
