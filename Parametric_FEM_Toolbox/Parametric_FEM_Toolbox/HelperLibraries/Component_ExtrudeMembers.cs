using System;
using Rhino;
using Rhino.Commands;
using System.Collections.Generic;
using Rhino.Geometry;
using Grasshopper.Kernel;
using System.Linq;
using Grasshopper.Kernel.Types.Transforms;

namespace Parametric_FEM_Toolbox.HelperLibraries
{
    public static class Component_ExtrudeMembers
    {

        public static List<Curve> CreateTweenCurves(Curve curveA, Curve curveB, int n, double tol, double ang_tol)
        {
            var tweens = new List<Curve>();

            List<double> t0 = new List<double>();
            List<double> t1 = new List<double>();
            t0.Add(curveA.Domain.T0);
            t0.Add(curveB.Domain.T0);
            t1.Add(curveA.Domain.T0);
            t1.Add(curveB.Domain.T0);

            // Create loft from base curves
            var base_curves = new List<Curve>() { curveA, curveB };
            var loft_curves = new List<Curve>();
            var tol_factor = 1.0;
            for (int i = 0; i < 2; i++)
            {
                var segemnts = base_curves[i].DuplicateSegments();
                Curve crv = null;
                if (segemnts.Length <= 1)
                {
                    crv = ((base_curves[i].Degree != 2) ? base_curves[i] : base_curves[i].Fit(4, tol, ang_tol));
                }
                else
                {
                    Point3d pointAtStart = base_curves[i].PointAtStart;
                    Interval domain = base_curves[i].Domain;
                    for (int j = 0; j < segemnts.Length; j++)
                    {
                        if (segemnts[j].Degree == 2)
                        {
                            segemnts[j] = segemnts[j].Fit(4, tol, ang_tol);
                        }
                    }
                    Curve[] segments2 = Curve.JoinCurves(segemnts);
                    segments2[0].ClosestPoint(pointAtStart, out var t);
                    segments2[0].ChangeClosedCurveSeam(t);
                    segments2[0].Domain = domain;
                    crv = segments2[0];
                }
                while (crv.IsClosed)
                {
                    Point3d testPoint = crv.PointAtLength(tol * (double)tol_factor);
                    Point3d testPoint2 = crv.PointAtLength(crv.GetLength() - tol * (double)tol_factor);
                    crv.ClosestPoint(testPoint, out var t2);
                    crv.ClosestPoint(testPoint2, out var t3);
                    Interval domain2 = new Interval(t2, t3);
                    domain2.MakeIncreasing();
                    crv = crv.Trim(domain2);
                    tol_factor++;
                    if (tol_factor == 1000)
                    {
                        break;
                    }
                }
                loft_curves.Add(crv);
            }
            var loft = Brep.CreateFromLoft(loft_curves, Point3d.Unset, Point3d.Unset, LoftType.Normal, closed: false);
            var surface = loft[0].Surfaces[0];

            // Create tween curves
            for (int i = 0; i < n; i++)
            {
                double normalized_parameter = (i + 1.0) / (n + 1.0);
                var constantParameter = surface.Domain(0).ParameterAt(normalized_parameter);
                var crv2 = surface.IsoCurve(1, constantParameter);
                if (crv2.IsClosable(tol * (double)(tol_factor * 2)))
                {
                    crv2.SetEndPoint(crv2.PointAtLength(0.0));
                }
                crv2.Domain = new Interval(t0.Min(), t1.Max());
                tweens.Add(crv2);
            }

            // Output
            return tweens;
        }


        public static Curve CreateTweenCurve(Curve curveA, Curve curveB, double curve_parameter, double tol, double ang_tol)
        {

            List<double> t0 = new List<double>();
            List<double> t1 = new List<double>();
            t0.Add(curveA.Domain.T0);
            t0.Add(curveB.Domain.T0);
            t1.Add(curveA.Domain.T0);
            t1.Add(curveB.Domain.T0);

            // Create loft from base curves
            var base_curves = new List<Curve>() { curveA, curveB };
            var loft_curves = new List<Curve>();
            var tol_factor = 1.0;
            for (int i = 0; i < 2; i++)
            {
                var segemnts = base_curves[i].DuplicateSegments();
                Curve crv = null;
                if (segemnts.Length <= 1)
                {
                    crv = ((base_curves[i].Degree != 2) ? base_curves[i] : base_curves[i].Fit(4, tol, ang_tol));
                }
                else
                {
                    Point3d pointAtStart = base_curves[i].PointAtStart;
                    Interval domain = base_curves[i].Domain;
                    for (int j = 0; j < segemnts.Length; j++)
                    {
                        if (segemnts[j].Degree == 2)
                        {
                            segemnts[j] = segemnts[j].Fit(4, tol, ang_tol);
                        }
                    }
                    Curve[] segments2 = Curve.JoinCurves(segemnts);
                    segments2[0].ClosestPoint(pointAtStart, out var t);
                    segments2[0].ChangeClosedCurveSeam(t);
                    segments2[0].Domain = domain;
                    crv = segments2[0];
                }
                while (crv.IsClosed)
                {
                    Point3d testPoint = crv.PointAtLength(tol * (double)tol_factor);
                    Point3d testPoint2 = crv.PointAtLength(crv.GetLength() - tol * (double)tol_factor);
                    crv.ClosestPoint(testPoint, out var t2);
                    crv.ClosestPoint(testPoint2, out var t3);
                    Interval domain2 = new Interval(t2, t3);
                    domain2.MakeIncreasing();
                    crv = crv.Trim(domain2);
                    tol_factor++;
                    if (tol_factor == 1000)
                    {
                        break;
                    }
                }
                loft_curves.Add(crv);
            }
            var loft = Brep.CreateFromLoft(loft_curves, Point3d.Unset, Point3d.Unset, LoftType.Normal, closed: false);
            var surface = loft[0].Surfaces[0];

            var constantParameter = surface.Domain(0).ParameterAt(curve_parameter);
            var crv2 = surface.IsoCurve(1, constantParameter);
            if (crv2.IsClosable(tol * (double)(tol_factor * 2)))
            {
                crv2.SetEndPoint(crv2.PointAtLength(0.0));
            }
            crv2.Domain = new Interval(t0.Min(), t1.Max());
            return crv2;
        }

        public static Plane GetOrientedPlane(Plane start_plane, Curve base_curve, double t)
        {
            var outPLane = new Plane(start_plane);
            var curve_parameter = t * (base_curve.Domain.T1 - base_curve.Domain.T0) + base_curve.Domain.T0; // not normalized
            var pointTo = new Point3d(base_curve.PointAt(curve_parameter));
            outPLane.Translate(pointTo - start_plane.Origin);
            var tangent = base_curve.TangentAt(curve_parameter);
            //var tangent = base_curve.TangentAt(t);
            var angle = Vector3d.VectorAngle(tangent, outPLane.Normal);
            var axis = Vector3d.CrossProduct(outPLane.Normal, tangent);
            outPLane.Rotate(angle, axis);
            return outPLane;
        }

        public static List<List<List<Curve>>> GenerateCroSecs (Curve baseline, List<Curve> startCroSec, List<Curve> endCroSec, int nCroSecsInter, double tol, double ang_tol, out List<Curve> segments)
        {
            var loft_crvs = new List<List<List<Curve>>>(); // this includes also the original sections: Number segments - Number Cross Sections - NUmber stations :D
            var t_list = new List<double>();
            //var segments = new List<Curve>();
            segments = new List<Curve>();
            // nCroSecsInter = 2;
            //nCroSecsInter = 2;

            if (baseline.Degree <= 1)
            {
                if (baseline.SpanCount > 1)
                {
                    segments = baseline.DuplicateSegments().ToList();
                }
                else
                {
                    segments.Add(baseline);
                }
                t_list.Add(baseline.Domain.T0);
                t_list.Add(baseline.Domain.T1);
                var normalized_parameter = 0.0;                
                foreach (var line in segments)
                {
                    var curves_in_segment = new List<List<Curve>>();
                    var domain = line.Domain;
                    var crosecs_original = nCroSecsInter;
                    //var length1 = line.GetLength();
                    //var length2 = baseline.GetLength();
                    //var crosecs2 = (int)(nCroSecsInter * line.GetLength() / baseline.GetLength());
                    nCroSecsInter = Math.Max((int)(nCroSecsInter * line.GetLength() / baseline.GetLength()), 2);
                    for (int i = 0; i < startCroSec.Count; i++)
                    {
                        var crvs = new List<Curve>();                        
                        for (int j = 0; j < nCroSecsInter; j++)
                        {
                            //normalized_parameter += (double)j / (double)(nCroSecsInter-1) * (domain.T1 - domain.T0) + domain.T0;
                            normalized_parameter = (domain.T0 + (double)j / (double)(nCroSecsInter - 1)*(domain.T1-domain.T0))/(t_list[1]-t_list[0]);
                            crvs.Add(Component_ExtrudeMembers.CreateTweenCurve(startCroSec[i], endCroSec[i], normalized_parameter, tol, ang_tol));
                        }
                        //curves_in_segment.Add(crvs);
                        //var crvs = new List<Curve>();
                        //crvs.Add(startCroSec[i]);
                        //crvs.AddRange();
                        //crvs.Add(endCroSec[i]);
                        curves_in_segment.Add(crvs);
                    }
                    loft_crvs.Add(curves_in_segment);
                    nCroSecsInter = crosecs_original; // So it works for next segment as well
                }
            }
            else
            {
                segments.Add(baseline);                
                var curves_in_segment = new List<List<Curve>>();
                for (int i = 0; i < startCroSec.Count; i++)
                {
                    curves_in_segment.Add(Component_ExtrudeMembers.CreateTweenCurves(startCroSec[i], endCroSec[i], nCroSecsInter, tol, ang_tol).ToList());
                }
                loft_crvs.Add(curves_in_segment);
            }
            return loft_crvs;
        }

        public static List<List<List<Curve>>> OrientCroSecs(List<List<List<Curve>>> cross_sections, List<Curve> segments, int nCroSecsInter, Plane start_frame, int crosecs_count)
        {
            var croSecs = cross_sections;
            var baseline_length = segments.Sum(x => x.GetLength());       
            for (int k = 0; k < segments.Count; k++)
            {
                //var domain = segments[k].Domain;
                var crosecs_original = nCroSecsInter;
                nCroSecsInter = Math.Max((int)(nCroSecsInter * segments[k].GetLength()/ baseline_length),2); // Correction polylines
                for (int i = 0; i<nCroSecsInter; i++) // loop through each station or curve parameter
                {

                    var normalized_parameter = (double)i / (nCroSecsInter - 1.0);
                    var plane = Component_ExtrudeMembers.GetOrientedPlane(start_frame, segments[k], normalized_parameter);
                    var orientation = (ITransform)new Orientation(new Plane(Point3d.Origin, new Vector3d(0, 0, 1)), plane);
                    var trans = orientation.ToMatrix();
                    for (int j = 0; j< crosecs_count; j++) // loop through each one of the curves that make up a cross section
                    {
                        var crv = croSecs[k][j][i].DuplicateCurve();
                        crv.Transform(trans);
                        croSecs[k][j][i] = crv;
                    }
                }
                nCroSecsInter = crosecs_original; // So it works for next segment as well
            }
            return croSecs;
        }
    }
}