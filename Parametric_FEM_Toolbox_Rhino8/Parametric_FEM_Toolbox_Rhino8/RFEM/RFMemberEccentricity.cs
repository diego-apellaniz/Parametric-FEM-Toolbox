using System;
using Dlubal.RFEM5;
using Grasshopper.Kernel.Types;
using Parametric_FEM_Toolbox.HelperLibraries;
using Rhino.Geometry;


namespace Parametric_FEM_Toolbox.RFEM
{
    [Serializable]
    public class RFMemberEccentricity : IGrassRFEM
    {

        //Standard constructors
        public RFMemberEccentricity()
        {
        }
        public RFMemberEccentricity(MemberEccentricity eccentricity)
        {
            Comment = eccentricity.Comment;
            ID = eccentricity.ID;
            IsValid = eccentricity.IsValid;
            No = eccentricity.No;
            Tag = eccentricity.Tag;
            ReferenceSystemType = eccentricity.ReferenceSystem;
            Start = eccentricity.Start.ToPoint3d() * 1000;
            End = eccentricity.End.ToPoint3d() * 1000;
            HingeAtStartNode = eccentricity.HingeAtStartNode;
            HingeAtEndNode = eccentricity.HingeAtEndNode;
            VerticalAlignment = eccentricity.VerticalAlignment;
            HorizontalAlignment = eccentricity.HorizontalAlignment;
            TransverseOffset = eccentricity.TransverseOffset;
            ReferenceObjectType = eccentricity.ReferenceObjectType;
            ReferenceObjectNo = eccentricity.ReferenceObjectNo;
            VerticalAxisOffset = eccentricity.VerticalAxisOffset;
            HorizontalAxisOffset = eccentricity.HorizontalAxisOffset;
            StartAdjoiningMembersOffset = eccentricity.StartAdjoiningMembersOffset;
            EndAdjoiningMembersOffset = eccentricity.EndAdjoiningMembersOffset;
            ToModify = false;
            ToDelete = false;
        }

        public RFMemberEccentricity(RFMemberEccentricity other) : this((MemberEccentricity)other)
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
        public ReferenceSystemType ReferenceSystemType { get; set; }
        public Point3d Start { get; set; }
        public Point3d End { get; set; }
        public bool HingeAtStartNode { get; set; }
        public bool HingeAtEndNode { get; set; }
        public VerticalAlignmentType VerticalAlignment { get; set; }
        public HorizontalAlignmentType HorizontalAlignment { get; set; }
        public bool TransverseOffset { get; set; }
        public ModelObjectType ReferenceObjectType { get; set;}
        public int ReferenceObjectNo { get; set; }
        public VerticalAlignmentType VerticalAxisOffset { get; set; }
        public HorizontalAlignmentType HorizontalAxisOffset { get; set; }
        public bool StartAdjoiningMembersOffset { get; set; }
        public bool EndAdjoiningMembersOffset { get; set; }
        // Additional Properties to the RFEM Struct
        public bool ToModify { get; set; }
        public bool ToDelete { get; set; }
        //public int NewNo { get; set; }

        // Display Info of the RFEM Objects on Panels
        // Parameters are separated by ";". The component split text can be used to break the string down into a list.
        public override string ToString()
        {
            return string.Format($"RFEM-MemberEccentricity;No:{No};ReferenceSystemType:{ReferenceSystemType};Start:{Start};End:{End}" +
                $"IsValid:{IsValid};ID:{((ID == "") ? "-" : ID)};" +
                $"ToModify:{ToModify};ToDelete:{ToDelete};Comment:{((Comment == "") ? "-" : Comment)};");
        }           

        //Operator to retrieve a Node from an rfNode.
        public static implicit operator MemberEccentricity(RFMemberEccentricity eccentricity)
        {
            MemberEccentricity myMemberHinge = new MemberEccentricity
            {
                Comment = eccentricity.Comment,
                ID = eccentricity.ID,
                IsValid = eccentricity.IsValid,
                No = eccentricity.No,
                Tag = eccentricity.Tag,
                ReferenceSystem = eccentricity.ReferenceSystemType,
                Start = (eccentricity.Start / 1000).ToPoint3D(),
                End = (eccentricity.End / 1000).ToPoint3D(),
                HingeAtStartNode = eccentricity.HingeAtStartNode,
                HingeAtEndNode = eccentricity.HingeAtEndNode,
                VerticalAlignment = eccentricity.VerticalAlignment,
                HorizontalAlignment = eccentricity.HorizontalAlignment,
                TransverseOffset = eccentricity.TransverseOffset,
                ReferenceObjectType = eccentricity.ReferenceObjectType,
                ReferenceObjectNo = eccentricity.ReferenceObjectNo,
                VerticalAxisOffset = eccentricity.VerticalAxisOffset,
                HorizontalAxisOffset = eccentricity.HorizontalAxisOffset,
                StartAdjoiningMembersOffset = eccentricity.StartAdjoiningMembersOffset,
                EndAdjoiningMembersOffset = eccentricity.EndAdjoiningMembersOffset
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
