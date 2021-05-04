using System;
using Rhino;
using Rhino.Commands;
using System.Collections.Generic;
using Rhino.Geometry;
using Grasshopper.Kernel;
using System.Linq;
using Grasshopper.Kernel.Types.Transforms;
using Parametric_FEM_Toolbox.RFEM;

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


        public static List<Brep> ExtrudeMembersToBrep(RFMember iMember, List<RFCroSec> iCroSecs, double length_segment, out string msg)
        {
            var outBreps = new List<Brep>();
            msg = "";

            // Check input
            var cs_indeces = iCroSecs.Select(x => x.No);
            if (iMember.EndCrossSectionNo == 0) // In case of tension members, etc.
            {
                iMember.EndCrossSectionNo = iMember.StartCrossSectionNo;
            }
            if (!(cs_indeces.Contains(iMember.StartCrossSectionNo)) || (!(cs_indeces.Contains(iMember.EndCrossSectionNo))))
            {
                msg = $"Provide cross sections for member No {iMember.No}.";
                return null;
            }

            // Get base geometry            
            var crosecs1 = iCroSecs.Where(x => x.No == iMember.StartCrossSectionNo).ToList()[0].Shape;
            var crosecs2 = iCroSecs.Where(x => x.No == iMember.EndCrossSectionNo).ToList()[0].Shape;
            var baseline = iMember.BaseLine.ToCurve();

            // Check geometry
            if ((crosecs1.Sum(x => x.SpanCount) != crosecs2.Sum(x => x.SpanCount)) || (crosecs1.Count != crosecs2.Count))
            {
                msg = $"Provide similar cross sections for member No {iMember.No}.";
                return null;
            }

            // Generate tween curves - still on the origin!
            List<Curve> segments;
            int nCroSecsInter;
            length_segment = Math.Max(length_segment, 0.05); // Check minimum vlaue
            if (baseline.Degree <= 1)
            {
                nCroSecsInter = 2;
            }
            else
            {
                nCroSecsInter = Math.Max((int)(baseline.GetLength() / length_segment), 2);
            }
            var loft_crvs = Component_ExtrudeMembers.GenerateCroSecs(baseline, crosecs1, crosecs2, nCroSecsInter, 0.001, 0.001, out segments);

            // Orient cross sections
            loft_crvs = Component_ExtrudeMembers.OrientCroSecs(loft_crvs, segments, nCroSecsInter, iMember.Frames[0], crosecs1.Count);

            // Extrude members
            for (int i = 0; i < segments.Count; i++)
            {
                for (int j = 0; j < crosecs1.Count; j++)
                {
                    outBreps.AddRange(Brep.CreateFromLoft(loft_crvs[i][j], Point3d.Unset, Point3d.Unset, LoftType.Normal, false));
                }
            }
            return outBreps;
        }

        public static List<Mesh> ExtrudeMembersToMesh(List<List<List<Curve>>> loft_crvs, int nFaces, out string msg)
        {
            var outmeshes = new List<Mesh>();
            msg = "";

            for (int i = 0; i < loft_crvs.Count; i++)
            {
                for (int j = 0; j < loft_crvs[i].Count; j++)
                {
                    var beam_mesh = new Mesh(); // for each of the curves that make one cross section
                    for (int k = 0; k < loft_crvs[i][j].Count; k++)
                    {
                        var exploded_segments = new List<Curve>();
                        if (loft_crvs[i][j][k].SpanCount > 1)
                        {
                            // exploded_segments = loft_crvs[i][j][k].DuplicateSegments().ToList();
                            var crv = loft_crvs[i][j][k].ToNurbsCurve();
                            // Get spilt parameters
                            var split_t = new List<double>();
                            for (int n = 0; n < crv.SpanCount; n++)
                            {
                                split_t.Add(crv.SpanDomain(n).T0);
                                //split_t.Add(crv.SpanDomain(n).T1);
                            }
                            split_t.Add(crv.SpanDomain(crv.SpanCount - 1).T1);
                            //split_t = split_t.Distinct().ToList();
                            exploded_segments = crv.Split(split_t).ToList();
                        }
                        else
                        {
                            exploded_segments.Add(loft_crvs[i][j][k]);
                        }
                        //var real_segments = (exploded_segments.Where(x => x.GetLength()>0.001)).ToList();
                        var real_segments = exploded_segments;
                        var counter_nodes = loft_crvs[i][j][k].IsClosed ? real_segments.Count * nFaces : real_segments.Count * nFaces + 1;
                        for (int n = 0; n < real_segments.Count; n++)
                        {
                            var domain = real_segments[n].Domain;
                            for (int m = 0; m < nFaces; m++)
                            {
                                // Add vertex
                                if (!(real_segments[n].IsClosed & m == nFaces - 1 & n == real_segments.Count - 1)) // for closed section shapes ignore last point
                                {
                                    var t = (double)m / (double)nFaces * (domain.T1 - domain.T0) + domain.T0;
                                    var pt = real_segments[n].PointAt(t);
                                    beam_mesh.Vertices.Add(pt.X, pt.Y, pt.Z);
                                }
                                // Add mesh face
                                if ((m + n) > 0 & k > 0)
                                {
                                    int a = (m - 1) + n * nFaces + counter_nodes * (k - 1);
                                    int b = ((m) + n * nFaces) % counter_nodes + counter_nodes * (k - 1); // get first node in the section if it is closed
                                    int c = ((m) + n * nFaces) % counter_nodes + counter_nodes * (k);
                                    int d = (m - 1) + n * nFaces + counter_nodes * (k);
                                    beam_mesh.Faces.AddFace(new MeshFace(a, b, c, d));
                                }
                            }
                        }
                        // Add last face
                        if (k > 0)
                        {
                            int a1 = (nFaces * real_segments.Count - 1) + counter_nodes * (k - 1);
                            int b1 = (nFaces * real_segments.Count) % counter_nodes + counter_nodes * (k - 1);
                            int c1 = (nFaces * real_segments.Count) % counter_nodes + counter_nodes * (k);
                            int d1 = (nFaces * real_segments.Count - 1) % counter_nodes + counter_nodes * (k);
                            beam_mesh.Faces.AddFace(new MeshFace(a1, b1, c1, d1));
                        }
                    }
                    outmeshes.Add(beam_mesh);
                }
            }
            return outmeshes;
        }
    }
}