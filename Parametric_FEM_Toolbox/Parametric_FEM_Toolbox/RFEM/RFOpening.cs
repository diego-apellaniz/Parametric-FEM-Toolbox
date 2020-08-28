using System;
using Dlubal.RFEM5;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Parametric_FEM_Toolbox.Utilities;
using Parametric_FEM_Toolbox.RFEM;
using System.Collections.Generic;
using System.Linq;

namespace Parametric_FEM_Toolbox.RFEM
{
    [Serializable]
    public class RFOpening : IGrassRFEM
    {
        //Standard constructors
        public RFOpening()
        {
        }
        public RFOpening(Dlubal.RFEM5.Opening opening, RFLine[] edges)
        {
            Comment = opening.Comment;
            ID = opening.ID;
            IsGenerated = opening.IsGenerated;
            IsValid = opening.IsValid;
            No = opening.No;
            InSurfaceNo = opening.InSurfaceNo;
            Tag = opening.Tag;
            Area = opening.Area;
            BoundaryLineCount = opening.BoundaryLineCount;
            BoundaryLineList = opening.BoundaryLineList;
            Edges = edges;
            ToModify = false;
            ToDelete = false;
        }
        public RFOpening(Dlubal.RFEM5.Opening opening) : this(opening, null)
        {
        }

        public RFOpening(RFOpening other) : this(other, null)
        {
            ToModify = other.ToModify;
            ToDelete = other.ToDelete;
            var newEdges = new List<RFLine>();
            foreach (var edge in other.Edges)
            {
                newEdges.Add(new RFLine(edge));
            }
            Edges = newEdges.ToArray();
        }

        // Properties to Wrap Fields from RFEM Struct
        public string Comment { get; set; }
        public string ID { get; set; }
        public bool IsGenerated { get; set; }
        public bool IsValid { get; set; }
        public int No { get; set; }
        public string Tag { get; set; }
        public double Area { get; set; }
        public int BoundaryLineCount { get; set; }
        public string BoundaryLineList { get; set; }
        public int InSurfaceNo { get; set; }
        // Additional Properties to the RFEM Struct
        public RFLine[] Edges { get; set; }
        public bool ToModify { get; set; }
        public bool ToDelete { get; set; }
        //public int NewNo { get; set; }

        // Display Info of the RFEM Objects on Panels
        // Parameters are separated by ";". The component split text can be used to break the string down into a list.
        public override string ToString()
        {
            return string.Format($"RFEM-Opening;No:{No};Area:{Area}[m2];InSurfaceNo:{InSurfaceNo};" +
                $"BoundaryLineCount:{BoundaryLineCount};BoundaryLineList:{((BoundaryLineList == "") ? "-" : BoundaryLineList)};" +
                $"Tag:{((Tag == "") ? "-" : Tag)};" +
                $"IsValid:{IsValid};IsGenerated:{IsGenerated};ID:{((ID == "") ? "-" : ID)};" +
                $"ToModify:{ToModify};ToDelete:{ToDelete};Comment:{((Comment == "") ? "-" : Comment)};");
        }

        //Operator to retrieve a Line from an rfLine.
        public static implicit operator Opening(RFOpening opening)
        {
            var myOpening = new Opening
            {
                Comment = opening.Comment,
                ID = opening.ID,
                IsGenerated = opening.IsGenerated,
                IsValid = opening.IsValid,
                No = opening.No,
                Tag = opening.Tag,
                Area = opening.Area,
                BoundaryLineCount = opening.BoundaryLineCount,
                BoundaryLineList = opening.BoundaryLineList,
                InSurfaceNo = opening.InSurfaceNo
            };
            return myOpening;
        }
        // Casting to GH Data Types
        public Brep ToPlanarBrep()
        {
            var sEdges = from e in Edges
                         select e.ToCurve();       
            return Brep.CreatePlanarBreps(Curve.JoinCurves(sEdges), 0.01)[0];
        }
        public Boolean IsPlanar()
        {
            var myEdges = new List<Curve>();
            var sEdges = from e in Edges
                         select e.ToCurve();
            myEdges.AddRange(Curve.JoinCurves(sEdges));
            return myEdges[0].IsPlanar(0.001);
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
        // COMPLETE WITH OTHER LINETYPES!!! FIXED POLYLINE!!!
        public bool ToGH_Curve<T>(ref T target)
        {
            return false;
        }
        public bool ToGH_Surface<T>(ref T target)
        {
            if (IsPlanar())
            {
                object obj = new GH_Surface(ToPlanarBrep());
                target = (T)obj;
                return true;
            }
            return false;
            
        }
        public bool ToGH_Brep<T>(ref T target)
        {
            if (IsPlanar())
            {
                object obj = new GH_Surface(ToPlanarBrep());
                target = (T)obj;
                return true;
            }
            return false;
        }
        public bool ToGH_Plane<T>(ref T target)
        {
            return false;
        }
    }
}