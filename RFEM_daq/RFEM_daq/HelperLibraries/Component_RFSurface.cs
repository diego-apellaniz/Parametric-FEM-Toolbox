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
    public static class Component_RFSurface
    {
        public static void SetGeometry(Brep surface,  ref RFSurface rfSurface)
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
            rfSurface.Edges = edges.ToArray();
            // Openings
            if (joinedEdges.Count > 1)
            {
                var openings = new List<RFOpening>();
                for (int i = 1; i < joinedEdges.Count; i++)
                {
                    var myOpening = new Opening();
                    myOpening.InSurfaceNo = rfSurface.No;
                    var opEdges = new List<RFLine>();
                    foreach (var e in joinedEdges[i].DuplicateSegments())
                    {
                        var myRFLine = new RFLine();
                        Component_RFLine.SetGeometry(e, ref myRFLine);
                        opEdges.Add(myRFLine);
                    }
                    openings.Add(new RFOpening(myOpening, opEdges.ToArray()));
                }
                rfSurface.Openings = openings.ToArray();
            }
            // Check if surface is Planar to assign type
            if (rfSurface.IsPlanar())
            {
                rfSurface.GeometryType = SurfaceGeometryType.PlaneSurfaceType;
            }
            else
            {
                rfSurface.GeometryType = SurfaceGeometryType.QuadrangleSurfaceType;
                //var interpolatedPoints = 4;
                //// Assign control points
                //var myControlPoints = new Point3d[interpolatedPoints + 2, interpolatedPoints + 2];
                //var srfc = surface.Faces[0];
                //var uDomain = srfc.Domain(0);
                //var vDomain = srfc.Domain(1);
                //var derivatives = new Vector3d[0];
                //for (int i = 0; i < interpolatedPoints + 2; i++)
                //{
                //    var u = uDomain.T0 + i * (uDomain.T1 - uDomain.T0) / (interpolatedPoints + 1);
                //    for (int j = 0; j < interpolatedPoints + 2; j++)
                //    {
                //        var v = vDomain.T0 + j * (vDomain.T1 - vDomain.T0) / (interpolatedPoints + 1);
                //        srfc.Evaluate(u, v, 0, out myControlPoints[i, j], out derivatives);
                //    }
                //}
                //rfSurface.ControlPoints = myControlPoints;

                //SetGeometryNURBS(surface, ref rfSurface);
            }
        }

        //public static void SetGeometryNURBS(Brep surface, ref RFSurface rfSurface)
        //{
        //    var myNurbsSurface = surface.Faces[0].ToNurbsSurface();
        //    var edgesNURBS = new NurbsCurve[4];
        //    // Get boundary Lines - UNTRIMMED!!!
        //    var edge1 = new NurbsCurve(myNurbsSurface.OrderU - 1, myNurbsSurface.Points.CountU);
        //    for (int i = 0; i < myNurbsSurface.Points.CountU; i++)
        //    {
        //    edge1.Points[i] = myNurbsSurface.Points.GetControlPoint(i, 0);                
        //    }
        //    for (int i = 0; i < myNurbsSurface.KnotsU.Count; i++)
        //    {
        //        edge1.Knots[i] = myNurbsSurface.KnotsU[i];
        //    }
        //    var edge2 = new NurbsCurve(myNurbsSurface.OrderV - 1, myNurbsSurface.Points.CountV);
        //    for (int i = 0; i < myNurbsSurface.Points.CountV; i++)
        //    {
        //        edge2.Points[i] = myNurbsSurface.Points.GetControlPoint(0, i);
        //    }
        //    for (int i = 0; i < myNurbsSurface.KnotsV.Count; i++)
        //    {
        //        edge2.Knots[i] = myNurbsSurface.KnotsV[i];
        //    }
        //    var edge3 = new NurbsCurve(myNurbsSurface.OrderU - 1, myNurbsSurface.Points.CountU);
        //    for (int i = 0; i < myNurbsSurface.Points.CountU; i++)
        //    {
        //        edge3.Points[i] = myNurbsSurface.Points.GetControlPoint(i, myNurbsSurface.Points.CountV-1);
        //    }
        //    for (int i = 0; i < myNurbsSurface.KnotsU.Count; i++)
        //    {
        //        edge3.Knots[i] = myNurbsSurface.KnotsU[i];
        //    }
        //    var edge4 = new NurbsCurve(myNurbsSurface.OrderV - 1, myNurbsSurface.Points.CountV);
        //    for (int i = 0; i < myNurbsSurface.Points.CountV; i++)
        //    {
        //        edge4.Points[i] = myNurbsSurface.Points.GetControlPoint(myNurbsSurface.Points.CountU-1, i);
        //    }
        //    for (int i = 0; i < myNurbsSurface.KnotsV.Count; i++)
        //    {
        //        edge4.Knots[i] = myNurbsSurface.KnotsV[i];
        //    }
        //    edgesNURBS[0] = edge1;
        //    edgesNURBS[1] = edge2;
        //    edgesNURBS[2] = edge3;
        //    edgesNURBS[3] = edge4;

        //    //var myBoundaryLines = new List<NurbsCurve>();
        //    ////Untrim surface
        //    //var unTrimmed = surface.Faces[0].DuplicateSurface().ToBrep();
        //    //foreach (var brepEdge in unTrimmed.Edges)
        //    //{
        //    //    myBoundaryLines.Add(brepEdge.ToNurbsCurve());
        //    //}
        //    //var joinedEdges = Curve.JoinCurves(myBoundaryLines).ToList();
        //    //// In case of openings, countours must be sorted
        //    //if (joinedEdges.Count > 1)
        //    //{
        //    //    joinedEdges = joinedEdges.OrderByDescending(x => x.GetLength()).ToList();
        //    //}
        //    var edges = new List<RFLine>();
        //    //foreach (var e in joinedEdges[0].DuplicateSegments())
        //    foreach (var e in edgesNURBS)
        //        {
        //        var myRFLine = new RFLine();
        //        // If line has just 2 points - insert additional one (otherwise it doesnw work in RFEM)
        //        //var eNURBS = e.ToNurbsCurve();
        //        if (e.Points.Count <=2)
        //        {
        //            var splitted = e.Rebuild(3,e.Degree, true);
        //        }
        //        Component_RFLine.SetGeometry(e, ref myRFLine);
        //        edges.Add(myRFLine);
        //    }
        //    rfSurface.Edges = edges.ToArray();
        //    //// Openings
        //    //if (joinedEdges.Count > 1)
        //    //{
        //    //    var openings = new List<RFOpening>();
        //    //    for (int i = 1; i < joinedEdges.Count; i++)
        //    //    {
        //    //        var myOpening = new Opening();
        //    //        myOpening.InSurfaceNo = rfSurface.No;
        //    //        var opEdges = new List<RFLine>();
        //    //        foreach (var e in joinedEdges[i].DuplicateSegments())
        //    //        {
        //    //            var myRFLine = new RFLine();
        //    //            Component_RFLine.SetGeometry(e, ref myRFLine);
        //    //            opEdges.Add(myRFLine);
        //    //        }
        //    //        openings.Add(new RFOpening(myOpening, opEdges.ToArray()));
        //    //    }
        //    //    rfSurface.Openings = openings.ToArray();
        //    //}
        //    // Check if surface is Planar to assign type
        //        rfSurface.GeometryType = SurfaceGeometryType.NurbsSurfaceType;
        //    // Assign Nurbsurface data
        //        var utNURBS = surface.Faces[0].DuplicateSurface().ToNurbsSurface();
        //        var myControlPoints = new Point3d[utNURBS.Points.CountU, utNURBS.Points.CountV];
        //        var myWeights = new double[utNURBS.Points.CountU, utNURBS.Points.CountV];
        //        for (int i = 0; i < utNURBS.Points.CountU; i++)
        //        {
        //            for (int j = 0; j < utNURBS.Points.CountV; j++)
        //            {
        //            var myPt = new Point3d();
        //            utNURBS.Points.GetPoint(i, j, out myPt);
        //            myControlPoints[i, j] = myPt;
        //            myWeights[i, j] = utNURBS.Points.GetWeight(i, j);
        //            }
        //        }

        //          modify formulation - see curve knots!!!!!!!!!!!

        //        var countKnotsX = utNURBS.Points.CountU + utNURBS.OrderU;
        //        var myKnotX = new double[countKnotsX];
        //        for (int i = 0; i < countKnotsX-2; i++)
        //        {
        //        myKnotX[i + 1] = utNURBS.KnotsU[i]/ utNURBS.KnotsU[countKnotsX-3];
        //        }
        //        myKnotX[0] = myKnotX[1];
        //        myKnotX[countKnotsX-1] = myKnotX[countKnotsX - 2];
        //        var countKnotsY = utNURBS.Points.CountV + utNURBS.OrderV;
        //        var myKnotY = new double[countKnotsY];
        //        for (int i = 0; i < countKnotsY-2; i++)
        //        {
        //            myKnotY[i + 1] = utNURBS.KnotsV[i] / utNURBS.KnotsV[countKnotsY - 3];
        //        }
        //        myKnotY[0] = myKnotY[1];
        //        myKnotY[countKnotsY - 1] = myKnotY[countKnotsY - 2];
        //        // Assign nurbs values
        //        rfSurface.ControlPoints = myControlPoints;
        //        rfSurface.Weights = myWeights;
        //        rfSurface.KnotsX = myKnotX;
        //        rfSurface.KnotsY = myKnotY;
        //        rfSurface.OrderX = utNURBS.OrderU;
        //        rfSurface.OrderY = utNURBS.OrderV;
        //}
    }
}
