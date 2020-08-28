using System;
using System.Collections.Generic;
using System.Linq;
using Dlubal.RFEM5;
using Parametric_FEM_Toolbox.Utilities;
using Parametric_FEM_Toolbox.RFEM;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Parametric_FEM_Toolbox.HelperLibraries
{
    public static class Component_RFLine
    {
        public static void SetGeometry(Curve curve, ref RFLine rFLine)
        {
            var inControlPoints = new List<Point3d>();
            rFLine.Order = curve.Degree + 1;
            var myNurbs = curve.ToNurbsCurve();
            foreach (var pt in myNurbs.Points)
            {
                inControlPoints.Add(pt.Location);
            }
            rFLine.ControlPoints = inControlPoints.ToArray();
            if  (curve.Degree > 1)
                {
                var inWeights = new double[myNurbs.Points.Count];
                var myKnots = new double[curve.Degree + myNurbs.Points.Count + 1];
                for (int i = 0; i < myNurbs.Points.Count; i++)
                {
                    inWeights[i] = myNurbs.Points[i].Weight;
                }
                // Knot vector for rfem must have n+p+1 knots between 0.0 and 1.0
                for (int i = 0; i < myNurbs.Knots.Count; i++)
                {
                    myKnots[i + 1] = (myNurbs.Knots[i]- myNurbs.Knots[0]) / (myNurbs.Knots[myNurbs.Knots.Count-1] - myNurbs.Knots[0]);
                }
                myKnots[0] = myKnots[1];
                myKnots[curve.Degree + myNurbs.Points.Count] = myKnots[curve.Degree + myNurbs.Points.Count-1];
                rFLine.Weights = inWeights.ToArray();
                rFLine.Knots = myKnots;
            }
             

            //inControlPoints.Add(curve.PointAtStart);
            //for (int i = 0; i < curve.SpanCount; i++)
            //{
            //    var t1 = curve.SpanDomain(i).T1;
            //    if (curve.Degree > 1)
            //    {
            //        var t0 = curve.SpanDomain(i).T0;
            //        for (int j = 1; j <= interpolatedPoints; j++)
            //        {
            //            inControlPoints.Add(curve.PointAt(t0 + j * (t1 - t0) / (interpolatedPoints + 1)));
            //        }
            //    }
            //    inControlPoints.Add(curve.PointAt(t1));
            //}
            //rFLine.ControlPoints = inControlPoints.ToArray();
            if (curve.Degree > 1)
            {
                rFLine.Type = LineType.NurbSplineType;
            }
            else
            {
                rFLine.Type = LineType.PolylineType;
            }
            //rFLine.Vertices = null;
        }
    }
}
