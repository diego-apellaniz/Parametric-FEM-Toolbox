using System;
using Dlubal.RFEM5;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Parametric_FEM_Toolbox.Utilities;
using Parametric_FEM_Toolbox.HelperLibraries;
using System.Collections.Generic;
using System.Linq;

namespace Parametric_FEM_Toolbox.RFEM
{
    [Serializable]
    public class RFMember : IGrassRFEM
    {
        //Standard constructors
        public RFMember()
        {
        }
        public RFMember(Member member, RFLine baseLine, double kcry, double kcrz, CoordinateSystem sys1, CoordinateSystem sys2, Point3d ecc1, Point3d ecc2)
        {
            Comment = member.Comment == null? "": member.Comment;
            ID = member.ID;
            IsGenerated = member.IsGenerated;
            IsValid = member.IsValid;
            No = member.No;
            Tag = member.Tag;
            Length = member.Length;
            RotationAngle = member.Rotation.Angle * 180 / Math.PI;
            RotationHelpNodeNo = member.Rotation.HelpNodeNo;
            RotationPlane = member.Rotation.Plane;
            RotationType = member.Rotation.Type;
            Type = member.Type;
            Weight = member.Weight;
            DivisionNo = member.DivisionNo;
            EccentricityNo = member.EccentricityNo;
            StartCrossSectionNo = member.StartCrossSectionNo;
            EndCrossSectionNo = member.EndCrossSectionNo;
            StartHingeNo = member.StartHingeNo;
            EndHingeNo = member.EndHingeNo;
            FoundationNo = member.FoundationNo;
            LineNo = member.LineNo;
            NonlinearityNo = member.NonlinearityNo;
            RibNo = member.RibNo;
            TaperShape = member.TaperShape;
            Kcry = kcry;
            Kcrz = kcrz;
            BaseLine = baseLine;
            EccStart = new Vector3d(ecc1);
            EccEnd = new Vector3d(ecc2);
            SetFrames2(sys1, sys2);
            ToModify = false;
            ToDelete = false;
        }
        public RFMember(Member member, RFLine baseLine) : this(member, baseLine, 1.0, 1.0, new CoordinateSystem(), new CoordinateSystem(), Point3d.Origin, Point3d.Origin)
        {            
        }        
        public RFMember (Member member) : this (member, null)
        {
        }

        public RFMember(RFMember other) : this(other, null, other.Kcry, other.Kcrz, new CoordinateSystem(), new CoordinateSystem(), Point3d.Origin, Point3d.Origin)
        {
            ToModify = other.ToModify;
            ToDelete = other.ToDelete;
            if (other.BaseLine != null)
            {
                BaseLine = new RFLine(other.BaseLine);
                Frames = new List<Plane>(other.Frames);
            }
            if (other.Type == MemberType.ResultBeamType)
            {
                ExceptMembers = other.ExceptMembers;
                ExceptSolids = other.ExceptSolids;
                ExceptSurfaces = other.ExceptSurfaces;
                IncludeMembers = other.IncludeMembers;
                IncludeSolids = other.IncludeSolids;
                IncludeSurfaces = other.IncludeSurfaces;
                Integrate = other.Integrate;
                Parameters = other.Parameters;
            }
            Frames = other.Frames;
            EccStart = other.EccStart;
            EccEnd = other.EccEnd;
        }


        // Properties to Wrap Fields from RFEM Struct
        public string Comment { get; set; }
        public string ID { get; set; }
        public bool IsGenerated { get; set; }
        public bool IsValid { get; set; }
        public int No { get; set; }
        public string Tag { get; set; }
        public double Length { get; set; }
        public double Weight { get; set; }
        public double RotationAngle { get; set; }
        public int RotationHelpNodeNo { get; set; }
        public PlaneType RotationPlane { get; set; }
        public RotationType RotationType { get; set; }
        public MemberType Type { get; set; }
        public TaperShapeType TaperShape { get; set; }
        public int DivisionNo { get; set; }
        public int EccentricityNo { get; set; }
        public int StartCrossSectionNo { get; set; }
        public int EndCrossSectionNo { get; set; }
        public int StartHingeNo { get; set; }
        public int EndHingeNo { get; set; }
        public int FoundationNo { get; set; }
        public int LineNo { get; set; }
        public int NonlinearityNo { get; set; }
        public int RibNo { get; set; }
        public double Kcry { get; set; }
        public double Kcrz { get; set; }
        // Additional properties for Result Beams
        public string ExceptMembers { get; set; }
        public string ExceptSolids { get; set; }
        public string ExceptSurfaces { get; set; }
        public string IncludeMembers { get; set; }
        public string IncludeSolids { get; set; }
        public string IncludeSurfaces { get; set; }
        public IntegrateStressesAndForcesType Integrate { get; set; }
        public List<double> Parameters { get; set; }
        public Vector3d EccStart { get; set; }
        public Vector3d EccEnd { get; set; }
        // Additional Properties to the RFEM Struct
        public RFLine BaseLine { get; set; }
        public List<Plane> Frames { get; set; }
        public bool ToModify { get; set; }
        public bool ToDelete { get; set; }
        //public int NewNo { get; set; }

        // Display Info of the RFEM Objects on Panels
        // Parameters are separated by ";". The component split text can be used to break the string down into a list.
        public override string ToString()
        {
            return string.Format($"RFEM-Member;No:{No};Length:{Length}[m];Weight:{Weight}[kg];Type:{Type};StartCrossSectionNo:{StartCrossSectionNo};" +
                $"EndCrossSectionNo:{EndCrossSectionNo};TaperShape:{TaperShape};StartHingeNo:{StartHingeNo};EndHingeNo:{EndHingeNo};" +
                $"Kcr,y:{Kcry};Kcr,z:{Kcrz};" +
                $"EccentricityNo:{EccentricityNo};DivisionNo:{DivisionNo};LineNo:{LineNo};FoundationNo:{FoundationNo};NonlinearityNo:{NonlinearityNo};" +
                $"RibNo:{RibNo};Tag:{((Tag == "") ? "-" : Tag)};" +
                $"IsValid:{IsValid};IsGenerated:{IsGenerated};ID:{((ID == "") ? "-" : ID)};RotationAngle:{RotationAngle}[°];" +
                $"RotationHelpNodeNo:{RotationHelpNodeNo};RotationPlane:{RotationPlane};RotationType:{RotationType};" +
                $"ToModify:{ToModify};ToDelete:{ToDelete};Comment:{((Comment == "") ? "-" : Comment)};");
        }

        //Operator to retrieve a Line from an rfLine.
        public static implicit operator Member(RFMember member)
        {
            Dlubal.RFEM5.Member myMember = new Dlubal.RFEM5.Member
            {
                Comment = member.Comment,
                ID = member.ID,
                IsGenerated = member.IsGenerated,
                IsValid = member.IsValid,
                No = member.No,
                Tag = member.Tag,
                Length = member.Length
            };
            myMember.Rotation.Angle = member.RotationAngle*Math.PI/180;
            myMember.Rotation.HelpNodeNo = member.RotationHelpNodeNo;
            myMember.Rotation.Plane = member.RotationPlane;
            myMember.Rotation.Type = member.RotationType;
            myMember.Type = member.Type;
            myMember.TaperShape = member.TaperShape;
            myMember.Weight = member.Weight;
            myMember.DivisionNo = member.DivisionNo;
            myMember.EccentricityNo = member.EccentricityNo;
            myMember.StartCrossSectionNo = member.StartCrossSectionNo;
            myMember.EndCrossSectionNo = member.EndCrossSectionNo;
            myMember.StartHingeNo = member.StartHingeNo;
            myMember.EndHingeNo = member.EndHingeNo;
            myMember.FoundationNo = member.FoundationNo;
            myMember.LineNo = member.LineNo;
            myMember.NonlinearityNo = member.NonlinearityNo;
            myMember.RibNo = member.RibNo;
            return myMember;
        }

        public static implicit operator ResultBeam(RFMember member)
        {
            var myResultBeam = new ResultBeam
            {
                No = member.No,
                ExceptMembers = member.ExceptMembers,
                ExceptSolids = member.ExceptSolids,
                ExceptSurfaces = member.ExceptSurfaces,
                IncludeMembers = member.IncludeMembers,
                IncludeSolids = member.IncludeSolids,
                IncludeSurfaces = member.IncludeSurfaces,
                Integrate = member.Integrate,                
            };
            if (member.Parameters != null)
            {
                myResultBeam.Parameters = member.Parameters.ToArray();
            }
            return myResultBeam;
        }

        public void SetResultBeam(ResultBeam beam)
        {
            ExceptMembers = beam.ExceptMembers;
            ExceptSolids = beam.ExceptSolids;
            ExceptSurfaces = beam.ExceptSurfaces;
            IncludeMembers = beam.IncludeMembers;
            IncludeSolids = beam.IncludeSolids;
            IncludeSurfaces = beam.IncludeSurfaces;
            Integrate = beam.Integrate;
            if (beam.Parameters != null)
            {
                Parameters = beam.Parameters.ToList();
            }
        }

        //Set Frames -> used when creating a member in GH
        public void SetFrames()
        {
            var baseCrv = BaseLine.ToCurve();
            var frame1 = new Plane();
            var frame2 = new Plane();
            var outFrames = new List<Plane>();
            if (baseCrv.IsClosed)
            {
                baseCrv.PerpendicularFrameAt(0, out frame1);
                baseCrv.PerpendicularFrameAt(baseCrv.Domain.Max, out frame2);

            }
            else if (baseCrv.IsVertical())
            {
                var vecX = Vector3d.YAxis;
                var vecY = Vector3d.CrossProduct(baseCrv.TangentAtStart, vecX);
                frame1 = new Plane(baseCrv.PointAtStart, vecX, vecY);
                frame2 = new Plane(baseCrv.PointAtEnd, vecX, vecY);
            }
            else if (baseCrv.IsHorizontal())
            {
                var vecY = Vector3d.ZAxis;
                var vecX = Vector3d.CrossProduct(vecY, baseCrv.TangentAtStart);
                frame1 = new Plane(baseCrv.PointAtStart, vecX, vecY);
                frame2 = new Plane(baseCrv.PointAtEnd, vecX, vecY);
            }
            else // curve is slopped
            {
                var vecX = Vector3d.CrossProduct(Vector3d.ZAxis, baseCrv.TangentAtStart);
                var vecY = Vector3d.CrossProduct(baseCrv.TangentAtStart, vecX);
                frame1 = new Plane(baseCrv.PointAtStart, vecX, vecY);
                frame2 = new Plane(baseCrv.PointAtEnd, vecX, vecY);
            }
            // Rotate axis?
            if (RotationAngle != 0)
            {
                frame1.Rotate(RotationAngle * Math.PI/180, frame1.Normal);
                frame2.Rotate(RotationAngle * Math.PI / 180, frame2.Normal);
            }
            // Output
            outFrames.Add(frame1);
            outFrames.Add(frame2);
            Frames = outFrames;
        }

        // used to get the 100% accurate axis system when importing members from RFEM
        public void SetFrames2(CoordinateSystem sys1, CoordinateSystem sys2)
        {
            if (BaseLine == null) // in case that we are copying from other member, because it will be null in this step, but frames will be copied from the other member anyway
                return;
            var vecX1 = new Vector3d(sys1.AxisY.ToPoint3d());
            var vecY1 = new Vector3d(sys1.AxisZ.ToPoint3d());
            var vecX2 = new Vector3d(sys2.AxisY.ToPoint3d());
            var vecY2 = new Vector3d(sys2.AxisZ.ToPoint3d());
            var baseCrv = BaseLine.ToCurve();
            var frame1 = new Plane(baseCrv.PointAtStart, vecX1, vecY1);
            var frame2 = new Plane(baseCrv.PointAtStart, vecX2, vecY2);
            Frames = new List<Plane>() { frame1, frame2 };
        }

        public Curve GetEccentricBaseline()
        {
            var baseCrv = BaseLine.ToCurve();
            if (EccStart.IsZero && EccEnd.IsZero)
                return baseCrv;

            var gh_crv = new GH_Curve(baseCrv);
            var crv_destination = (IGH_GeometricGoo)gh_crv.DuplicateCurve();
            var list1 = new List<Point3d>() { baseCrv.PointAtStart, baseCrv.PointAtEnd };
            var list2 = new List<Vector3d>() { EccStart, EccEnd };
            var spatialMorph = new HelperLibraries.UtilLibrary.SpatialMorph(list1, list2);
            spatialMorph.PreserveStructure = false;
            spatialMorph.QuickPreview = false;

            crv_destination = crv_destination.Morph(spatialMorph);
            var crv = (GH_Curve)crv_destination;

            return crv.Value;
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
