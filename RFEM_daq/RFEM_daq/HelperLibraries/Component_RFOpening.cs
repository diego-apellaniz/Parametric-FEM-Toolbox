using System;
using System.Collections.Generic;
using System.Linq;
using Dlubal.RFEM5;
using RFEM_daq.Utilities;
using RFEM_daq.RFEM;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace RFEM_daq.HelperLibraries
{
    public static class Component_RFOpening
    {
        public static void SetGeometry(Brep surface, int interpolatedPoints, ref RFOpening rfOpening)
        {
            var myBoundaryLines = new List<NurbsCurve>();
            foreach (var brepEdge in surface.Edges)
            {
                myBoundaryLines.Add(brepEdge.ToNurbsCurve());
            }
            var joinedEdges = Curve.JoinCurves(myBoundaryLines).ToList();
            // In case of openings, countours must be sorted
            var edges = new List<RFLine>();
            foreach (var e in joinedEdges[0].DuplicateSegments())
            {
                var myRFLine = new RFLine();
                Component_RFLine.SetGeometry(e, ref myRFLine);
                edges.Add(myRFLine);
            }
            rfOpening.Edges = edges.ToArray();
        }

        public static void SetGeometry(Brep surface, ref RFOpening rfOpening)
        {
            var myBoundaryLines = new List<NurbsCurve>();
            foreach (var brepEdge in surface.Edges)
            {
                myBoundaryLines.Add(brepEdge.ToNurbsCurve());
            }
            var joinedEdges = Curve.JoinCurves(myBoundaryLines).ToList();
            // In case of openings, countours must be sorted
            if (joinedEdges.Count > 1)
            {
                joinedEdges = joinedEdges.OrderByDescending(x => x.GetLength()).ToList();
            }
            var edges = new List<RFLine>();
            foreach (var e in joinedEdges[0].DuplicateSegments())
            {
                var myRFLine = new RFLine();
                Component_RFLine.SetGeometry(e, ref myRFLine);
                edges.Add(myRFLine);
            }
            if (!(rfOpening.IsPlanar()))
            {
                return;
            }
            rfOpening.Edges = edges.ToArray();
        }
    }    
}
