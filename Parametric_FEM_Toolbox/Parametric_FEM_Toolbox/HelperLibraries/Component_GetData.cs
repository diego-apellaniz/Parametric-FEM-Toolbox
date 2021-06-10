using System;
using System.Collections.Generic;
using System.Linq;
using Dlubal.RFEM5;
using Dlubal.RFEM3;
using Parametric_FEM_Toolbox.Utilities;
using Parametric_FEM_Toolbox.RFEM;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Runtime.InteropServices;

namespace Parametric_FEM_Toolbox.HelperLibraries
{
    public static class Component_GetData
    {
        #region Nodes
        public static List<Node> FilterNodes(Dlubal.RFEM5.IModelData data, List<RFFilter> filters)
        {
            var outNodes = new List<Node>();
            foreach (var n in data.GetNodes())
            {
                var include = true;
                foreach (var filter in filters)
                {
                    if (!(filter.NodeList is null))
                    {
                        if (!filter.NodeList.Contains(n.No))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.NodeComment is null))
                    {
                        if (!filter.NodeComment.Contains(n.Comment))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.NodesX is null))
                    {
                        var rfNode = new RFNode(n, n.ToPoint3d(data));
                        if (!filter.NodesX.Includes(rfNode.X))
                        {
                            include = false;
                            break;
                        }     
                    }
                    if (!(filter.NodesY is null))
                    {
                        var rfNode = new RFNode(n, n.ToPoint3d(data));
                        if (!filter.NodesY.Includes(rfNode.Y))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.NodesZ is null))
                    {
                        var rfNode = new RFNode(n, n.ToPoint3d(data));
                        if (!filter.NodesZ.Includes(rfNode.Z))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.NodeCS is null))
                    {
                        if (!filter.NodeCS.Contains(n.CS.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.NodeRef is null))
                    {
                        if (!filter.NodeRef.Contains(n.RefObjectNo))
                        {
                            include = false;
                            break;
                        }
                    }
                }
                if (!include) continue;
                outNodes.Add(n);
            }
            return outNodes;
        }

        public static List<RFNode> GetRFNodes(List<Node> nodes, IModelData data)
        {
            // Consider reference nodes!
            return Array.ConvertAll(nodes.ToArray(), x => new RFNode(x, x.ToPoint3d(data))).ToList();
        }
        #endregion

        #region Lines
        public static List<Dlubal.RFEM5.Line> FilterLines(Dlubal.RFEM5.IModelData data, List<RFFilter> filters)
        {
            var outLine = new List<Dlubal.RFEM5.Line>();
            foreach (var line in data.GetLines())
            {
                var include = true;
                foreach (var filter in filters)
                {
                    if (!(filter.LineList is null))
                    {
                        if (!filter.LineList.Contains(line.No))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.LineComment is null))
                    {
                        if (!filter.LineComment.Contains(line.Comment))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.LinesX is null))
                    {
                        foreach (var nodeNo in line.NodeList.ToInt())
                        {
                            var node = data.GetNode(nodeNo, ItemAt.AtNo).GetData();
                            var rfNode = new RFNode(node, node.ToPoint3d(data));
                            if (!filter.LinesX.Includes(rfNode.X))
                            {
                                include = false;
                                break;
                            }
                        }
                    }
                    if (!(filter.LinesY is null))
                    {
                        foreach (var nodeNo in line.NodeList.ToInt())
                        {
                            var node = data.GetNode(nodeNo, ItemAt.AtNo).GetData();
                            var rfNode = new RFNode(node, node.ToPoint3d(data));
                            if (!filter.LinesY.Includes(rfNode.Y))
                            {
                                include = false;
                                break;
                            }
                        }
                    }
                    if (!(filter.LinesZ is null))
                    {
                        foreach (var nodeNo in line.NodeList.ToInt())
                        {
                            var node = data.GetNode(nodeNo, ItemAt.AtNo).GetData();
                            var rfNode = new RFNode(node, node.ToPoint3d(data));
                            if (!filter.LinesZ.Includes(rfNode.Z))
                            {
                                include = false;
                                break;
                            }
                        }
                    }
                    if (!(filter.LineType is null))
                    {
                        if (!filter.LineType.Contains(line.Type.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.LineRotType is null))
                    {
                        if (!filter.LineRotType.Contains(line.Rotation.Type.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.LineRotAngle is null))
                    {
                        if (!filter.LineRotAngle.Includes(line.Rotation.Angle))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.LineNoList is null))
                    {
                        var inNodesNo = line.NodeList.ToInt();
                        if (!(inNodesNo.Intersect(filter.LineNoList).ToArray().Length > 0))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.LineLength is null))
                    {
                        if (!filter.LineLength.Includes(line.Length))
                        {
                            include = false;
                            break;
                        }
                    }
                }
                if (!include) continue;
                outLine.Add(line);
            }
            return outLine;
        }

        public static List<RFLine> GetRFLines(List<Dlubal.RFEM5.Line> lines, IModelData data)
        {
            var rFlines = new List<RFLine>();
            foreach (var l in lines)
            {
                if (!(l.Type == LineType.NurbSplineType))
                {
                    rFlines.Add(new RFLine(l, l.NodeList.ToInt().Select(i => data.GetNode(i, ItemAt.AtNo).GetData().ToPoint3d(data)).ToArray()));
                }
                else
                {
                    var myNurbs = data.GetNurbSpline(l.No, ItemAt.AtNo).GetExtraData();
                    rFlines.Add(new RFLine(l, l.NodeList.ToInt().Select(i => data.GetNode(i, ItemAt.AtNo).GetData().ToPoint3d(data)).ToArray(), myNurbs.Order, myNurbs.Weights, myNurbs.Knots));
                }
            }
            return rFlines;
        }
        #endregion

        #region Members
        public static List<Member> FilterMembers(Dlubal.RFEM5.IModelData data, List<RFFilter> filters)
        {
            var outMembers = new List<Member>();
            foreach (var member in data.GetMembers())
            {
                var include = true;
                foreach (var filter in filters)
                {
                    if (!(filter.MemberList is null))
                    {
                        if (!filter.MemberList.Contains(member.No))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MemberComment is null))
                    {
                        if (!filter.MemberComment.Contains(member.Comment))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MemberX is null))
                    {
                        foreach (var nodeNo in data.GetLine(member.LineNo,ItemAt.AtNo).GetData().NodeList.ToInt())
                        {
                            var node = data.GetNode(nodeNo, ItemAt.AtNo).GetData();
                            var rfNode = new RFNode(node, node.ToPoint3d(data));
                            if (!filter.MemberX.Includes(rfNode.X))
                            {
                                include = false;
                                break;
                            }
                        }
                    }
                    if (!(filter.MemberY is null))
                    {
                        foreach (var nodeNo in data.GetLine(member.LineNo, ItemAt.AtNo).GetData().NodeList.ToInt())
                        {
                            var node = data.GetNode(nodeNo, ItemAt.AtNo).GetData();
                            var rfNode = new RFNode(node, node.ToPoint3d(data));
                            if (!filter.MemberY.Includes(rfNode.Y))
                            {
                                include = false;
                                break;
                            }
                        }
                    }
                    if (!(filter.MemberZ is null))
                    {
                        foreach (var nodeNo in data.GetLine(member.LineNo, ItemAt.AtNo).GetData().NodeList.ToInt())
                        {
                            var node = data.GetNode(nodeNo, ItemAt.AtNo).GetData();
                            var rfNode = new RFNode(node, node.ToPoint3d(data));
                            if (!filter.MemberZ.Includes(rfNode.Z))
                            {
                                include = false;
                                break;
                            }
                        }
                    }
                    if (!(filter.MemberType is null))
                    {
                        if (!filter.MemberType.Contains(member.Type.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MemberLineNo is null))
                    {
                        if (!filter.MemberLineNo.Contains(member.LineNo))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MemberStartCS is null))
                    {
                        if (!filter.MemberStartCS.Contains(member.StartCrossSectionNo))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MemberEndCS is null))
                    {
                        if (!filter.MemberEndCS.Contains(member.EndCrossSectionNo))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MemberRotType is null))
                    {
                        if (!filter.MemberRotType.Contains(member.Rotation.Type.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MemberRotAngle is null))
                    {
                        if (!filter.MemberRotAngle.Includes(member.Rotation.Angle*180/Math.PI))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MemberStartHinge is null))
                    {
                        if (!filter.MemberStartHinge.Contains(member.StartHingeNo))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MemberEndHinge is null))
                    {
                        if (!filter.MemberEndHinge.Contains(member.EndHingeNo))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MemberEcc is null))
                    {
                        if (!filter.MemberEcc.Contains(member.EccentricityNo))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MemberDivision is null))
                    {
                        if (!filter.MemberDivision.Contains(member.DivisionNo))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MemberTaper is null))
                    {
                        if (!filter.MemberTaper.Contains(member.TaperShape.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MemberFactorY is null) || !(filter.MemberFactorZ is null))
                    {
                        var effectiveLengths = data.GetMember(member.No, ItemAt.AtNo).GetEffectiveLengths();
                        if (!(filter.MemberFactorY is null))
                        {
                            if (!filter.MemberFactorY.Includes(effectiveLengths.FactorY))
                            {
                                include = false;
                                break;
                            }
                        }
                        if (!(filter.MemberFactorZ is null))
                        {
                            if (!filter.MemberFactorZ.Includes(effectiveLengths.FactorZ))
                            {
                                include = false;
                                break;
                            }
                        }
                    }     
                    if (!(filter.MemberLength is null))
                    {
                        if (!filter.MemberLength.Includes(member.Length))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MemberWeight is null))
                    {
                        if (!filter.MemberWeight.Includes(member.Weight))
                        {
                            include = false;
                            break;
                        }
                    }
                }
                if (!include) continue;
                outMembers.Add(member);
            }
            return outMembers;
        }

        public static List<RFMember> GetRFMembers(List<Member> members, IModelData data)
        {
            var rfMembers = new List<RFMember>();
            foreach (var m in members)
            {
                var baseline = data.GetLine(m.LineNo, ItemAt.AtNo).GetData();
                var rfLine = new RFLine();
                if (!(baseline.Type == LineType.NurbSplineType))
                {
                    rfLine = (new RFLine(baseline, baseline.NodeList.ToInt().Select(i => data.GetNode(i, ItemAt.AtNo).GetData().ToPoint3d(data)).ToArray()));
                }
                else
                {
                    var myNurbs = data.GetNurbSpline(baseline.No, ItemAt.AtNo).GetExtraData();
                    rfLine = (new RFLine(baseline, baseline.NodeList.ToInt().Select(i => data.GetNode(i, ItemAt.AtNo).GetData().ToPoint3d(data)).ToArray(), myNurbs.Order, myNurbs.Weights, myNurbs.Knots));
                }
                //List<Point3d> vertices = new List<Point3d>();
                //baseline.NodeList.ToInt().ForEach(x => vertices.Add(data.GetNode(x, ItemAt.AtNo).GetData().ToPoint3
                var effectiveLengths = data.GetMember(m.No, ItemAt.AtNo).GetEffectiveLengths();
                var outMember = new RFMember(m, rfLine, effectiveLengths.FactorY, effectiveLengths.FactorZ);
                outMember.SetFrames();
                rfMembers.Add(outMember);
            }
            return rfMembers;
        }
        #endregion

        #region Surfaces
        public static List<Dlubal.RFEM5.Surface> FilterSurfaces(Dlubal.RFEM5.IModelData data, List<RFFilter> filters)
        {
            var outSrfcs = new List<Dlubal.RFEM5.Surface>();
            foreach (var srfc in data.GetSurfaces())
            {
                var include = true;
                foreach (var filter in filters)
                {
                    if (!(filter.SrfcList is null) && filter.SrfcList.Any())
                    {
                        if (!filter.SrfcList.Contains(srfc.No))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SrfcComment is null) && filter.SrfcComment.Any())
                    {
                        if (!filter.SrfcComment.Contains(srfc.Comment))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!((filter.SrfcX is null) && (filter.SrfcY is null) && (filter.SrfcZ is null)))
                    {
                        var nodeNolist = new List<int>();
                        var rfNolist = new List<RFNode>();
                        foreach (var lineNo in srfc.BoundaryLineList.ToInt())
                        {
                            nodeNolist.AddRange(data.GetLine(lineNo, ItemAt.AtNo).GetData().NodeList.ToInt());
                            nodeNolist = nodeNolist.Distinct().ToList();
                        }
                        foreach (var nodeNo in nodeNolist)
                        {
                            var node = data.GetNode(nodeNo, ItemAt.AtNo).GetData();
                            rfNolist.Add( new RFNode(node, node.ToPoint3d(data)));
                        }
                        if (!(filter.SrfcX is null))
                        {
                            foreach (var rfNode in rfNolist)
                            {
                                if (!filter.SrfcX.Includes(rfNode.X))
                                {
                                    include = false;
                                    break;
                                }
                            }
                        }
                        if (!(filter.SrfcY is null))
                        {
                            foreach (var rfNode in rfNolist)
                            {
                                if (!filter.SrfcY.Includes(rfNode.Y))
                                {
                                    include = false;
                                    break;
                                }
                            }
                        }
                        if (!(filter.SrfcZ is null))
                        {
                            foreach (var rfNode in rfNolist)
                            {
                                if (!filter.SrfcZ.Includes(rfNode.Z))
                                {
                                    include = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (!(filter.SrfcType is null))
                    {
                        if (!filter.SrfcType.Contains(srfc.GeometryType.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SrfcStiffType is null))
                    {
                        if (!filter.SrfcStiffType.Contains(srfc.StiffnessType.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SrfcBoundLineNo is null))
                    {
                        var inLinesNo = srfc.BoundaryLineList.ToInt();
                        if (!(inLinesNo.Intersect(filter.SrfcBoundLineNo).ToArray().Length > 0))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SrfcMaterial is null))
                    {
                        if (!filter.SrfcMaterial.Contains(srfc.MaterialNo))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.ThickType is null))
                    {
                        if (!filter.ThickType.Contains(srfc.Thickness.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SrfcThickness is null))
                    {
                        if (!filter.SrfcThickness.Includes(srfc.Thickness.Constant))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SrfcEcc is null))
                    {
                        if (!filter.SrfcEcc.Includes(srfc.Eccentricity))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SrfcIntLineNo is null))
                    {
                        var inLinesNo = srfc.IntegratedLineList.ToInt();
                        foreach (var lineList in filter.SrfcIntLineNo)
                        {
                            var fillines = lineList.ToInt();
                            if (!(fillines.Count == inLinesNo.Count))
                            {
                                include = false;
                                continue;
                            }
                            include = true;
                            foreach (var lineNo in fillines)
                            {
                                if (!inLinesNo.Contains(lineNo))
                                {
                                    include = false;
                                    break;
                                }
                            }
                            if (include)
                            {
                                break;
                            }
                        }
                    }
                    if (!(filter.SrfcIntNodeNo is null))
                    {
                        var inNodesNo = srfc.IntegratedNodeList.ToInt();
                        foreach (var nodeList in filter.SrfcIntNodeNo)
                        {
                            var filnodes = nodeList.ToInt();
                            if (!(filnodes.Count == inNodesNo.Count))
                            {
                                include = false;
                                continue;
                            }
                            include = true;
                            foreach (var nodeNo in filnodes)
                            {
                                if (!inNodesNo.Contains(nodeNo))
                                {
                                    include = false;
                                    break;
                                }
                            }
                            if (include)
                            {
                                break;
                            }
                        }
                    }
                    if (!(filter.SrfcIntOpNo is null))
                    {
                        var inOpsNo = srfc.IntegratedOpeningList.ToInt();
                        foreach (var opList in filter.SrfcIntOpNo)
                        {
                            var filops = opList.ToInt();
                            if (!(filops.Count == inOpsNo.Count))
                            {
                                include = false;
                                continue;
                            }
                            include = true;
                            foreach (var opNo in filops)
                            {
                                if (!inOpsNo.Contains(opNo))
                                {
                                    include = false;
                                    break;
                                }
                            }
                            if (include)
                            {
                                break;
                            }
                        }
                    }
                    if (!(filter.SrfcArea is null))
                    {
                        if (!filter.SrfcArea.Includes(srfc.Area))
                        {
                            include = false;
                            break;
                        }
                    }
                    //if (!(filter.SrfcWeight is null))
                    //{
                    //    if (!filter.SrfcWeight.Includes(srfc.Thickness))
                    //    {
                    //        include = false;
                    //        break;
                    //    }
                    //}
                }
                if (!include) continue;
                outSrfcs.Add(srfc);
            }
            return outSrfcs;
        }

        public static List<RFSurface> GetRFSurfaces(List<Dlubal.RFEM5.Surface> surfaces, IModelData data)
        {
            var rfSurfaces = new List<RFSurface>();
            List<RFNode> nodes = null;

            if (surfaces.Any(x=> x.GeometryType == SurfaceGeometryType.NurbsSurfaceType))
            {
                nodes = GetRFNodes(data.GetNodes().ToList(), data);
            }
            

            foreach (var s in surfaces)
            {
                var edges = new List<RFLine>();
                var lines = from b in s.BoundaryLineList.ToInt()
                            select data.GetLine(b, ItemAt.AtNo).GetData();
                edges = GetRFLines(lines.ToList(), data);
                var openings = from o in data.GetOpenings()
                               where o.InSurfaceNo == s.No
                               select o;
                var rfOpenings = GetRFOpenings(openings.ToList(), data);
                var rfSfc = new RFSurface(s, edges.ToArray(), rfOpenings.ToArray());
                if (s.GeometryType == SurfaceGeometryType.NurbsSurfaceType)
                {
                    var nurbs_surface = data.GetNurbsSurface(s.No, ItemAt.AtNo).GetExtraData();
                    rfSfc.OrderX = nurbs_surface.OrderX;
                    rfSfc.OrderY = nurbs_surface.OrderY;                    
                    rfSfc.KnotsX = nurbs_surface.KnotsX;
                    rfSfc.KnotsY = nurbs_surface.KnotsY;
                    rfSfc.Weights = nurbs_surface.Weights;
                    rfSfc.Nodes = nurbs_surface.Nodes;                    
                    var ctrl_points = new Point3d[rfSfc.Nodes.GetLength(0), rfSfc.Nodes.GetLength(1)];
                    for (int i = 0; i < rfSfc.Nodes.GetLength(0); i++)
                    {
                        for (int j = 0; j < rfSfc.Nodes.GetLength(1); j++)
                        {
                            ctrl_points[i, j] = nodes.Where(x => x.No == rfSfc.Nodes[i, j]).ToArray()[0].Location;
                        }
                    }
                    rfSfc.ControlPoints = ctrl_points;
                }
                // Get Surface Input and Result Axes
                var sufaceaxes = data.GetSurface(s.No, ItemAt.AtNo).GetInputAxes();
                rfSfc.SurfaceAxes = new RFEM.SurfaceAxes(sufaceaxes);
                // Get surface axes
                rfSfc.GetAxes(data);
                // Output
                rfSurfaces.Add(rfSfc);
            }
            return rfSurfaces;
        }
        #endregion

        #region Openings
        public static List<Opening> FilterOpenings(Dlubal.RFEM5.IModelData data, List<RFFilter> filters)
        {
            var outOpenings = new List<Opening>();
            foreach (var opening in data.GetOpenings())
            {
                var include = true;
                foreach (var filter in filters)
                {
                    if (!(filter.OpList is null))
                    {
                        if (!filter.OpList.Contains(opening.No))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.OpComment is null))
                    {
                        if (!filter.OpComment.Contains(opening.Comment))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!((filter.OpX is null) && (filter.OpY is null) && (filter.OpZ is null)))
                    {
                        var nodeNolist = new List<int>();
                        var rfNolist = new List<RFNode>();
                        foreach (var lineNo in opening.BoundaryLineList.ToInt())
                        {
                            nodeNolist.AddRange(data.GetLine(lineNo, ItemAt.AtNo).GetData().NodeList.ToInt());
                            nodeNolist = nodeNolist.Distinct().ToList();
                        }
                        foreach (var nodeNo in nodeNolist)
                        {
                            var node = data.GetNode(nodeNo, ItemAt.AtNo).GetData();
                            rfNolist.Add(new RFNode(node, node.ToPoint3d(data)));
                        }
                        if (!(filter.OpX is null))
                        {
                            foreach (var rfNode in rfNolist)
                            {
                                if (!filter.OpX.Includes(rfNode.X))
                                {
                                    include = false;
                                    break;
                                }
                            }
                        }
                        if (!(filter.OpY is null))
                        {
                            foreach (var rfNode in rfNolist)
                            {
                                if (!filter.OpY.Includes(rfNode.Y))
                                {
                                    include = false;
                                    break;
                                }
                            }
                        }
                        if (!(filter.OpZ is null))
                        {
                            foreach (var rfNode in rfNolist)
                            {
                                if (!filter.OpZ.Includes(rfNode.Z))
                                {
                                    include = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (!(filter.OpBoundLineNo is null))
                    {
                        var inLinesNo = opening.BoundaryLineList.ToInt();
                        if (!(inLinesNo.Intersect(filter.OpBoundLineNo).ToArray().Length > 0))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.OpSrfcNo is null))
                    {
                        if (!filter.OpSrfcNo.Contains(opening.InSurfaceNo))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.OpArea is null))
                    {
                        if (!filter.OpArea.Includes(opening.Area))
                        {
                            include = false;
                            break;
                        }
                    }
                }
                if (!include) continue;
                outOpenings.Add(opening);
            }
            return outOpenings;
        }
        public static List<RFOpening> GetRFOpenings(List<Opening> openings, IModelData data)
        {
            var rfOpenings = new List<RFOpening>();
            foreach (var o in openings)
            {
                var edges = new List<RFLine>();
                var lines = from b in o.BoundaryLineList.ToInt()
                            select data.GetLine(b, ItemAt.AtNo).GetData();
                edges = GetRFLines(lines.ToList(), data);
                rfOpenings.Add(new RFOpening(o, edges.ToArray()));
            }
            return rfOpenings;
        }
        #endregion

        #region supports
        public static List<NodalSupport> FilterSupsP(Dlubal.RFEM5.IModelData data, List<RFFilter> filters)
        {
            var outSups = new List<NodalSupport>();
            foreach (var n in data.GetNodalSupports())
            {
                var include = true;
                foreach (var filter in filters)
                {
                    if (!(filter.SupList is null))
                    {
                        if (!filter.SupList.Contains(n.No))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SupComment is null))
                    {
                        if (!filter.SupComment.Contains(n.Comment))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SupX is null))
                    {
                        foreach (var nodeNo in n.NodeList.ToInt())
                        {
                            var node = data.GetNode(nodeNo, ItemAt.AtNo).GetData();
                            var rfNode = new RFNode(node, node.ToPoint3d(data));
                            if (!filter.SupX.Includes(rfNode.X))
                            {
                                include = false;
                                break;
                            }
                        }
                    }
                    if (!(filter.SupY is null))
                    {
                        foreach (var nodeNo in n.NodeList.ToInt())
                        {
                            var node = data.GetNode(nodeNo, ItemAt.AtNo).GetData();
                            var rfNode = new RFNode(node, node.ToPoint3d(data));
                            if (!filter.SupY.Includes(rfNode.Y))
                            {
                                include = false;
                                break;
                            }
                        }
                    }
                    if (!(filter.SupZ is null))
                    {
                        foreach (var nodeNo in n.NodeList.ToInt())
                        {
                            var node = data.GetNode(nodeNo, ItemAt.AtNo).GetData();
                            var rfNode = new RFNode(node, node.ToPoint3d(data));
                            if (!filter.SupZ.Includes(rfNode.Z))
                            {
                                include = false;
                                break;
                            }
                        }
                    }
                    if (!(filter.SupNodeList is null))
                    {
                        var inNodesNo = n.NodeList.ToInt();
                        if (!(inNodesNo.Intersect(filter.SupNodeList).ToArray().Length > 0))                             
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SupTx is null))
                    {
                        var value = ((n.SupportConstantX >= 0) ? n.SupportConstantX / 1000 : n.SupportConstantX);
                        if (!filter.SupTx.Includes(value))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SupTy is null))
                    {
                        var value = ((n.SupportConstantY >= 0) ? n.SupportConstantY / 1000 : n.SupportConstantY);
                        if (!filter.SupTy.Includes(value))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SupTz is null))
                    {
                        var value = ((n.SupportConstantZ >= 0) ? n.SupportConstantZ / 1000 : n.SupportConstantZ);
                        if (!filter.SupTz.Includes(value))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SupRx is null))
                    {
                        var value = ((n.RestraintConstantX >= 0) ? n.RestraintConstantX / 1000 : n.RestraintConstantX);
                        if (!filter.SupRx.Includes(value))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SupRy is null))
                    {
                        var value = ((n.RestraintConstantY >= 0) ? n.RestraintConstantY / 1000 : n.RestraintConstantY);
                        if (!filter.SupRy.Includes(value))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SupRz is null))
                    {
                        var value = ((n.RestraintConstantZ >= 0) ? n.RestraintConstantZ / 1000 : n.RestraintConstantZ);
                        if (!filter.SupRz.Includes(value))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SupNTx is null))
                    {
                        if (!filter.SupNTx.Contains(n.SupportNonlinearityX.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SupNTy is null))
                    {
                        if (!filter.SupNTy.Contains(n.SupportNonlinearityY.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SupNTz is null))
                    {
                        if (!filter.SupNTz.Contains(n.SupportNonlinearityZ.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SupNRx is null))
                    {
                        if (!filter.SupNRx.Contains(n.RestraintNonlinearityX.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SupNRy is null))
                    {
                        if (!filter.SupNRy.Contains(n.RestraintNonlinearityY.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SupNRz is null))
                    {
                        if (!filter.SupNRz.Contains(n.RestraintNonlinearityZ.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                }
                if (!include) continue;
                outSups.Add(n);
            }
            return outSups;
        }



        public static List<RFSupportP> GetRFSupportsP(List<NodalSupport> supports, IModelData data)
        {
            var rfSupportList = new List<RFSupportP>();
            // Get Planesss
            foreach (var item in supports)
            {
                //var planes = new List<Plane>();
                var nodeList = new List<Node>();
                foreach (var node in item.NodeList.ToInt())
                {                    
                    nodeList.Add(data.GetNode(node, ItemAt.AtNo).GetData());
                }
                var originList = from pt in GetRFNodes(nodeList, data)
                                 select pt.Location;
                var planes = originList.ToList().ConvertAll(x => new Plane(x, Vector3d.XAxis, Vector3d.YAxis));
                var rfSupport = new RFSupportP(item, planes.ToList());
                rfSupport.GetOrientation();
                rfSupportList.Add(rfSupport);
            }
            return rfSupportList;
        }

        #endregion

        #region supportsL

        public static List<LineSupport> FilterSupsL(Dlubal.RFEM5.IModelData data, List<RFFilter> filters)
        {
            var outSups = new List<LineSupport>();
            foreach (var n in data.GetLineSupports())
            {
                var include = true;
                foreach (var filter in filters)
                {
                    if (!(filter.SupLList is null))
                    {
                        if (!filter.SupLList.Contains(n.No))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SupLComment is null))
                    {
                        if (!filter.SupLComment.Contains(n.Comment))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SupLX is null))
                    {
                        foreach (var lineNo in n.LineList.ToInt())
                        { 
                            foreach (var nodeNo in data.GetLine(lineNo, ItemAt.AtNo).GetData().NodeList.ToInt())
                            
                            {
                                var node = data.GetNode(nodeNo, ItemAt.AtNo).GetData();
                                var rfNode = new RFNode(node, node.ToPoint3d(data));
                                if (!filter.SupLX.Includes(rfNode.X))
                                {
                                    include = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (!(filter.SupLY is null))
                    {
                        foreach (var lineNo in n.LineList.ToInt())
                        {
                            foreach (var nodeNo in data.GetLine(lineNo, ItemAt.AtNo).GetData().NodeList.ToInt())

                            {
                                var node = data.GetNode(nodeNo, ItemAt.AtNo).GetData();
                                var rfNode = new RFNode(node, node.ToPoint3d(data));
                                if (!filter.SupLY.Includes(rfNode.Y))
                                {
                                    include = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (!(filter.SupLZ is null))
                    {
                        foreach (var lineNo in n.LineList.ToInt())
                        {
                            foreach (var nodeNo in data.GetLine(lineNo, ItemAt.AtNo).GetData().NodeList.ToInt())

                            {
                                var node = data.GetNode(nodeNo, ItemAt.AtNo).GetData();
                                var rfNode = new RFNode(node, node.ToPoint3d(data));
                                if (!filter.SupLZ.Includes(rfNode.Z))
                                {
                                    include = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (!(filter.SupLRefSys is null))
                    {
                        if (!filter.SupLRefSys.Contains(n.ReferenceSystem.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SupLineList is null))
                    {
                        var inLinesNo = n.LineList.ToInt();
                        if (!(inLinesNo.Intersect(filter.SupLineList).ToArray().Length > 0))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SupLTx is null))
                    {
                        var value = ((n.SupportConstantX >= 0) ? n.SupportConstantX / 1000 : n.SupportConstantX);
                        if (!filter.SupLTx.Includes(value))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SupLTy is null))
                    {
                        var value = ((n.SupportConstantY >= 0) ? n.SupportConstantY / 1000 : n.SupportConstantY);
                        if (!filter.SupLTy.Includes(value))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SupLTz is null))
                    {
                        var value = ((n.SupportConstantZ >= 0) ? n.SupportConstantZ / 1000 : n.SupportConstantZ);
                        if (!filter.SupLTz.Includes(value))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SupLRx is null))
                    {
                        var value = ((n.RestraintConstantX >= 0) ? n.RestraintConstantX / 1000 : n.RestraintConstantX);
                        if (!filter.SupLRx.Includes(value))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SupLRy is null))
                    {
                        var value = ((n.RestraintConstantY >= 0) ? n.RestraintConstantY / 1000 : n.RestraintConstantY);
                        if (!filter.SupLRy.Includes(value))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SupLRz is null))
                    {
                        var value = ((n.RestraintConstantZ >= 0) ? n.RestraintConstantZ / 1000 : n.RestraintConstantZ);
                        if (!filter.SupLRz.Includes(value))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SupLNTx is null))
                    {
                        if (!filter.SupLNTx.Contains(n.SupportNonlinearityX.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SupLNTy is null))
                    {
                        if (!filter.SupLNTy.Contains(n.SupportNonlinearityY.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SupLNTz is null))
                    {
                        if (!filter.SupLNTz.Contains(n.SupportNonlinearityZ.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                }
                if (!include) continue;
                outSups.Add(n);
            }
            return outSups;
        }

        public static List<RFSupportL> GetRFSupportsL(List<LineSupport> supports, IModelData data)
        {
            var rfSupportList = new List<RFSupportL>();
            // Get Planesss
            foreach (var item in supports)
            {
                //var planes = new List<Plane>();
                var lineList = new List<Dlubal.RFEM5.Line>();
                foreach (var line in item.LineList.ToInt())
                {
                    lineList.Add(data.GetLine(line, ItemAt.AtNo).GetData());
                }
                var baseLines = GetRFLines(lineList, data);
                var rfSupport = new RFSupportL(item, baseLines);
                rfSupportList.Add(rfSupport);
            }
            return rfSupportList;
        }

        #endregion

        #region supportsS

        public static List<SurfaceSupport> FilterSupsS(Dlubal.RFEM5.IModelData data, List<RFFilter> filters)
        {
            var outSups = new List<SurfaceSupport>();
            foreach (var n in data.GetSurfaceSupports())
            {
                var include = true;
                foreach (var filter in filters)
                {
                    if (!(filter.SupSList is null))
                    {
                        if (!filter.SupSList.Contains(n.No))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SupSComment is null))
                    {
                        if (!filter.SupSComment.Contains(n.Comment))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SupSX is null))
                    {
                        foreach (var sfcNo in n.SurfaceList.ToInt())
                        {
                            foreach (var lineNo in data.GetSurface(sfcNo, ItemAt.AtNo).GetData().BoundaryLineList.ToInt())
                            {
                                foreach (var nodeNo in data.GetLine(lineNo, ItemAt.AtNo).GetData().NodeList.ToInt())
                                {
                                    var node = data.GetNode(nodeNo, ItemAt.AtNo).GetData();
                                    var rfNode = new RFNode(node, node.ToPoint3d(data));
                                    if (!filter.SupSX.Includes(rfNode.X))
                                    {
                                        include = false;
                                        break;
                                    }
                                }
                            }                            
                        }
                    }
                    if (!(filter.SupSY is null))
                    {
                        foreach (var sfcNo in n.SurfaceList.ToInt())
                        {
                            foreach (var lineNo in data.GetSurface(sfcNo, ItemAt.AtNo).GetData().BoundaryLineList.ToInt())
                            {
                                foreach (var nodeNo in data.GetLine(lineNo, ItemAt.AtNo).GetData().NodeList.ToInt())
                                {
                                    var node = data.GetNode(nodeNo, ItemAt.AtNo).GetData();
                                    var rfNode = new RFNode(node, node.ToPoint3d(data));
                                    if (!filter.SupSY.Includes(rfNode.Y))
                                    {
                                        include = false;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (!(filter.SupSZ is null))
                    {
                        foreach (var sfcNo in n.SurfaceList.ToInt())
                        {
                            foreach (var lineNo in data.GetSurface(sfcNo, ItemAt.AtNo).GetData().BoundaryLineList.ToInt())
                            {
                                foreach (var nodeNo in data.GetLine(lineNo, ItemAt.AtNo).GetData().NodeList.ToInt())
                                {
                                    var node = data.GetNode(nodeNo, ItemAt.AtNo).GetData();
                                    var rfNode = new RFNode(node, node.ToPoint3d(data));
                                    if (!filter.SupSZ.Includes(rfNode.Z))
                                    {
                                        include = false;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (!(filter.SupSrfcList is null))
                    {
                        var inSfcsNo = n.SurfaceList.ToInt();
                        if (!(inSfcsNo.Intersect(filter.SupSrfcList).ToArray().Length > 0))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SupSTx is null))
                    {
                        var value = ((n.SupportConstantX >= 0) ? n.SupportConstantX / 1000 : n.SupportConstantX);
                        if (!filter.SupSTx.Includes(value))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SupSTy is null))
                    {
                        var value = ((n.SupportConstantY >= 0) ? n.SupportConstantY / 1000 : n.SupportConstantY);
                        if (!filter.SupSTy.Includes(value))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SupSTz is null))
                    {
                        var value = ((n.SupportConstantZ >= 0) ? n.SupportConstantZ / 1000 : n.SupportConstantZ);
                        if (!filter.SupSTz.Includes(value))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SupSVxz is null))
                    {
                        var value = ((n.ShearConstantXZ >= 0) ? n.ShearConstantXZ / 1000 : n.ShearConstantXZ);
                        if (!filter.SupSVxz.Includes(value))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SupSVyz is null))
                    {
                        var value = ((n.ShearConstantYZ >= 0) ? n.ShearConstantYZ / 1000 : n.ShearConstantYZ);
                        if (!filter.SupSVyz.Includes(value))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SupSNTz is null))
                    {
                        if (!filter.SupSNTz.Contains(n.SupportNonlinearityZ.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                }
                if (!include) continue;
                outSups.Add(n);
            }
            return outSups;
        }

        public static List<RFSupportS> GetRFSupportsS(List<SurfaceSupport> supports, IModelData data)
        {
            var rfSupportList = new List<RFSupportS>();
            // Get Planesss
            foreach (var item in supports)
            {
                //var planes = new List<Plane>();
                var srfcList = new List<Dlubal.RFEM5.Surface>();
                foreach (var sfc in item.SurfaceList.ToInt())
                {
                    srfcList.Add(data.GetSurface(sfc, ItemAt.AtNo).GetData());
                }
                var baseSfcss = GetRFSurfaces(srfcList, data);
                var rfSupport = new RFSupportS(item, baseSfcss);
                rfSupportList.Add(rfSupport);
            }
            return rfSupportList;
        }

        #endregion

        #region LineHinge

        public static List<LineHinge> FilterLH(Dlubal.RFEM5.IModelData data, List<RFFilter> filters)
        {
            var outHinges = new List<LineHinge>();
            foreach (var n in data.GetLineHinges())
            {
                var include = true;
                foreach (var filter in filters)
                {
                    if (!(filter.LHList is null))
                    {
                        if (!filter.LHList.Contains(n.No))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.LHComment is null))
                    {
                        if (!filter.LHComment.Contains(n.Comment))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.LHLineNo is null))
                    {
                        if (!filter.LHLineNo.Contains(n.LineNo))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.LHSfcNo is null))
                    {
                        if (!filter.LHSfcNo.Contains(n.SurfaceNo))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.LHX is null))
                    {
                            foreach (var nodeNo in data.GetLine(n.LineNo, ItemAt.AtNo).GetData().NodeList.ToInt())

                            {
                                var node = data.GetNode(nodeNo, ItemAt.AtNo).GetData();
                                var rfNode = new RFNode(node, node.ToPoint3d(data));
                                if (!filter.LHX.Includes(rfNode.X))
                                {
                                    include = false;
                                    break;
                                }
                            }
                    }
                    if (!(filter.LHY is null))
                    {
                        foreach (var nodeNo in data.GetLine(n.LineNo, ItemAt.AtNo).GetData().NodeList.ToInt())

                        {
                            var node = data.GetNode(nodeNo, ItemAt.AtNo).GetData();
                            var rfNode = new RFNode(node, node.ToPoint3d(data));
                            if (!filter.LHY.Includes(rfNode.X))
                            {
                                include = false;
                                break;
                            }
                        }
                    }
                    if (!(filter.LHZ is null))
                    {
                        foreach (var nodeNo in data.GetLine(n.LineNo, ItemAt.AtNo).GetData().NodeList.ToInt())

                        {
                            var node = data.GetNode(nodeNo, ItemAt.AtNo).GetData();
                            var rfNode = new RFNode(node, node.ToPoint3d(data));
                            if (!filter.LHZ.Includes(rfNode.X))
                            {
                                include = false;
                                break;
                            }
                        }
                    }
                    if (!(filter.LHTx is null))
                    {
                        var value = ((n.TranslationalConstantX >= 0) ? n.TranslationalConstantX / 1000 : n.TranslationalConstantX);
                        if (!filter.LHTx.Includes(value))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.LHTy is null))
                    {
                        var value = ((n.TranslationalConstantY >= 0) ? n.TranslationalConstantY / 1000 : n.TranslationalConstantY);
                        if (!filter.LHTy.Includes(value))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.LHTz is null))
                    {
                        var value = ((n.TranslationalConstantZ >= 0) ? n.TranslationalConstantZ / 1000 : n.TranslationalConstantZ);
                        if (!filter.LHTz.Includes(value))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.LHRx is null))
                    {
                        var value = ((n.RotationalConstantX >= 0) ? n.RotationalConstantX / 1000 : n.RotationalConstantX);
                        if (!filter.LHRx.Includes(value))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.LHRy is null))
                    {
                        var value = ((n.RotationalConstantY >= 0) ? n.RotationalConstantY / 1000 : n.RotationalConstantY);
                        if (!filter.LHRy.Includes(value))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.LHRz is null))
                    {
                        var value = ((n.RotationalConstantZ >= 0) ? n.RotationalConstantZ / 1000 : n.RotationalConstantZ);
                        if (!filter.LHRz.Includes(value))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.LHSide is null))
                    {
                        if (!filter.LHSide.Contains(n.Side.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                }
                if (!include) continue;
                outHinges.Add(n);
            }
            return outHinges;
        }

        public static List<RFLineHinge> GetRFLineHinges(List<LineHinge> hinges, IModelData data)
        {
            var rfLineHinges = new List<RFLineHinge>();
            // Get Planesss
            foreach (var item in hinges)
            {
                var lineList = new List<Dlubal.RFEM5.Line>();
                lineList.Add(data.GetLine(item.LineNo, ItemAt.AtNo).GetData());
                var baseLines = GetRFLines(lineList, data);
                var rfLineHinge = new RFLineHinge(item, baseLines[0]);
                rfLineHinges.Add(rfLineHinge);
            }
            return rfLineHinges;
        }

        #endregion

        #region MemberHinge

        public static List<MemberHinge> FilterMH(Dlubal.RFEM5.IModelData data, List<RFFilter> filters)
        {
            var outHinges = new List<MemberHinge>();
            foreach (var n in data.GetMemberHinges())
            {
                var include = true;
                foreach (var filter in filters)
                {
                    if (!(filter.MHList is null))
                    {
                        if (!filter.MHList.Contains(n.No))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MHComment is null))
                    {
                        if (!filter.MHComment.Contains(n.Comment))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MHTx is null))
                    {
                        var value = ((n.TranslationalConstantX >= 0) ? n.TranslationalConstantX / 1000 : n.TranslationalConstantX);
                        if (!filter.MHTx.Includes(value))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MHTy is null))
                    {
                        var value = ((n.TranslationalConstantY >= 0) ? n.TranslationalConstantY / 1000 : n.TranslationalConstantY);
                        if (!filter.MHTy.Includes(value))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MHTz is null))
                    {
                        var value = ((n.TranslationalConstantZ >= 0) ? n.TranslationalConstantZ / 1000 : n.TranslationalConstantZ);
                        if (!filter.MHTz.Includes(value))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MHRx is null))
                    {
                        var value = ((n.RotationalConstantX >= 0) ? n.RotationalConstantX / 1000 : n.RotationalConstantX);
                        if (!filter.MHRx.Includes(value))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MHRy is null))
                    {
                        var value = ((n.RotationalConstantY >= 0) ? n.RotationalConstantY / 1000 : n.RotationalConstantY);
                        if (!filter.MHRy.Includes(value))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MHRz is null))
                    {
                        var value = ((n.RotationalConstantZ >= 0) ? n.RotationalConstantZ / 1000 : n.RotationalConstantZ);
                        if (!filter.MHRz.Includes(value))
                        {
                            include = false;
                            break;
                        }
                    }
                }
                if (!include) continue;
                outHinges.Add(n);
            }
            return outHinges;
        }

        public static List<RFMemberHinge> GetRFMemberHinges(List<MemberHinge> hinges, IModelData data)
        {
            var rfMemberHinges = new List<RFMemberHinge>();
            // Get Planesss
            foreach (var item in hinges)
            {                
                var rfMemberHinge = new RFMemberHinge(item);
                rfMemberHinges.Add(rfMemberHinge);
            }
            return rfMemberHinges;
        }

        #endregion

        #region Nodal Release

        public static List<NodalRelease> FilterNR(Dlubal.RFEM5.IModelData data, List<RFFilter> filters)
        {
            var outReleases = new List<NodalRelease>();
            foreach (var n in data.GetNodalReleases())
            {
                var include = true;
                foreach (var filter in filters)
                {
                    if (!(filter.NRList is null))
                    {
                        if (!filter.NRList.Contains(n.No))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.NRComment is null))
                    {
                        if (!filter.NRComment.Contains(n.Comment))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.NRNodeNo is null))
                    {
                        if (!filter.NRNodeNo.Contains(n.NodeNo))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.NRHinge is null))
                    {
                        if (!filter.NRHinge.Contains(n.MemberHingeNo))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.NRReleasedMembersNo is null))
                    {
                        var inLinesNo = n.ReleasedMembers.ToInt();
                        if (!(inLinesNo.Intersect(filter.NRReleasedMembersNo).ToArray().Length > 0))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.NRReleasedSurfacesNo is null))
                    {
                        var inLinesNo = n.ReleasedSurfaces.ToInt();
                        if (!(inLinesNo.Intersect(filter.NRReleasedSurfacesNo).ToArray().Length > 0))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.NRReleasedSolidsNo is null))
                    {
                        var inLinesNo = n.ReleasedSolids.ToInt();
                        if (!(inLinesNo.Intersect(filter.NRReleasedSolidsNo).ToArray().Length > 0))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.NRMemberNo is null))
                    {
                        if (!filter.NRMemberNo.Contains(n.AxisSystemFromObjectNo))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.NRGeneratedNodeNo is null))
                    {
                        if (!filter.NRGeneratedNodeNo.Contains(n.GeneratedNodeNo))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.NRAxisSystem is null))
                    {
                        if (!filter.NRAxisSystem.Contains(n.AxisSystem.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.NRLocation is null))
                    {
                        if (!filter.NRLocation.Contains(n.Location.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                }
                if (!include) continue;
                outReleases.Add(n);
            }
            return outReleases;
        }

        public static List<RFNodalRelease> GetRFNodalReleases(List<NodalRelease> releases, IModelData data)
        {
            var rfNodalReleases = new List<RFNodalRelease>();
            // Get Planesss
            foreach (var item in releases)
            {
                var rFNodalRelease = new RFNodalRelease(item);
                rfNodalReleases.Add(rFNodalRelease);
            }
            return rfNodalReleases;
        }

#endregion

        #region Cross Section

        public static List<CrossSection> FilterCroSecs(Dlubal.RFEM5.IModelData data, List<RFFilter> filters)
        {
            var outCroSec = new List<CrossSection> ();
            foreach (var croSec in data.GetCrossSections())
            {
                var include = true;
                foreach (var filter in filters)
                {
                    if (!(filter.CSList is null))
                    {
                        if (!filter.CSList.Contains(croSec.No))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.CSComment is null))
                    {
                        if (!filter.CSComment.Contains(croSec.Comment))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.CSDes is null))
                    {
                        if (!filter.CSDes.Contains(croSec.Description))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.CSMatNo is null))
                    {
                        if (!filter.CSMatNo.Contains(croSec.MaterialNo))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.CSA is null))
                    {
                        if (!filter.CSA.Includes(croSec.AxialArea))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.CSAy is null))
                    {
                        if (!filter.CSAy.Includes(croSec.ShearAreaY))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.CSAz is null))
                    {
                        if (!filter.CSAz.Includes(croSec.ShearAreaZ))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.CSIy is null))
                    {
                        if (!filter.CSIy.Includes(croSec.BendingMomentY))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.CSIz is null))
                    {
                        if (!filter.CSIz.Includes(croSec.BendingMomentZ))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.CSJt is null))
                    {
                        if (!filter.CSJt.Includes(croSec.TorsionMoment))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.CSRotAngle is null))
                    {
                        if (!filter.CSRotAngle.Includes(croSec.Rotation * 180 / Math.PI))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.CSTempW is null))
                    {
                        if (!filter.CSTempW.Includes(croSec.TemperatureLoadWidth))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.CSTempD is null))
                    {
                        if (!filter.CSTempD.Includes(croSec.TemperatureLoadDepth))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.CSUserDefined is null))
                    {
                        if (!filter.CSUserDefined.Contains(croSec.UserDefined))
                        {
                            include = false;
                            break;
                        }
                    }
                }
                if (!include) continue;
                outCroSec.Add(croSec);
            }
            return outCroSec;
        }

        public static List<RFCroSec> GetRFCroSecs(List<CrossSection> croSecs, IModel model, ref List<string> msg)
        {
            // Use Dlubal.RFEM3 to get shapes of cross sections
            IrfStructure IStructure = (IrfStructure)model;
            IStructure.rfGetApplication().rfLockLicence();
            IrfStructuralData data2 = IStructure.rfGetStructuralData();
            IrfDatabaseCrSc db = data2.rfGetDatabaseCrSc();

            var rfCroSecs = new List<RFCroSec>();
            foreach (var cs in croSecs)
            {
                var csRFEM5 = new RFCroSec(cs);
                IrfCrossSectionDB2 csRFEM3 = db.rfGetCrossSection(cs.Description) as IrfCrossSectionDB2;
                try
                {
                    var cscrvs = csRFEM3.rfGetShape();
                    csRFEM5.SetShape(cscrvs);
                }catch
                {
                    msg.Add($"Shape of Cross Section No. {cs.No} not supported!");
                }                              
                rfCroSecs.Add(csRFEM5);
            }
            IStructure.rfGetApplication().rfUnlockLicence();
            return rfCroSecs;
        }

        public static List<RFCroSec> GetRFCroSecs(List<CrossSection> croSecs, IModelData data)
        {
            var rfCroSecs = new List<RFCroSec>();
            foreach (var cs in croSecs)
            {
                var csRFEM5 = new RFCroSec(cs);
                rfCroSecs.Add(csRFEM5);
            }
            return rfCroSecs;
        }

        #endregion

        #region Material

        public static List<Material> FilterMaterials(Dlubal.RFEM5.IModelData data, List<RFFilter> filters)
        {
            var outMat = new List<Material>();
            foreach (var mat in data.GetMaterials())
            {
                var include = true;
                foreach (var filter in filters)
                {
                    if (!(filter.MatList is null))
                    {
                        if (!filter.MatList.Contains(mat.No))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MatComment is null))
                    {
                        if (!filter.MatComment.Contains(mat.Comment))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MatDes is null))
                    {
                        if (!filter.MatDes.Contains(mat.Description))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MatE is null))
                    {
                        if (!filter.MatE.Includes(mat.ElasticityModulus))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MatGamma is null))
                    {
                        if (!filter.MatGamma.Includes(mat.PartialSafetyFactor))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MatMu is null))
                    {
                        if (!filter.MatMu.Includes(mat.PoissonRatio))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MatW is null))
                    {
                        if (!filter.MatW.Includes(mat.SpecificWeight))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MatG is null))
                    {
                        if (!filter.MatG.Includes(mat.ShearModulus))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MatAlpha is null))
                    {
                        if (!filter.MatAlpha.Includes(mat.ThermalExpansion))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MatModelType is null))
                    {
                        if (!filter.MatModelType.Contains(mat.ModelType.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MatUserDefined is null))
                    {
                        if (!filter.MatUserDefined.Contains(mat.UserDefined))
                        {
                            include = false;
                            break;
                        }
                    }
                }
                if (!include) continue;
                outMat.Add(mat);
            }
            return outMat;
        }

        public static List<RFMaterial> GetRFMaterials(List<Material> mats, IModelData data)
        {
            var rfMats = new List<RFMaterial>();
            foreach (var mat in mats)
            {
                var rfMat = new RFMaterial(mat);
                if (rfMat.ModelType == MaterialModelType.OrthotropicElastic2DType)
                {
                    var modelOrthoElastic = data.GetMaterial(rfMat.No, ItemAt.AtNo).GetModel() as IMaterialOrthotropicElasticModel;
                    if (modelOrthoElastic != null)
                    {
                        rfMat.SetElasticOrthotropic(modelOrthoElastic.GetData());
                    }                    
                }
                rfMats.Add(rfMat);
            }
            return rfMats;
        }

        #endregion

        #region NodalLoads

        public static Dictionary<Tuple<int, int>, NodalLoad> FilterNodalLoads(Dlubal.RFEM5.IModelData data, ILoads loads, List<RFFilter> filters)
        {
            //var outDictionaryLoads = new Dictionary<NodalLoad, int>();
            var outDictionaryLoads = new Dictionary<Tuple<int, int>, NodalLoad>();
            // Select load cases
            var lcAll = loads.GetLoadCases();
            var lcSelected = new List<int>();
            foreach (var lc in lcAll)
            {
                int lcIndex = lc.Loading.No;
                bool include = true;
                foreach (var filter in filters)
                {
                    
                    if (!(filter.NLLC is null))
                    {
                        if (!filter.NLLC.Contains(lc.Loading.No))
                        {
                            include = false;
                            break;
                        }
                    }
                }
                if (include)
                {
                    lcSelected.Add(lcIndex);
                }
            }
            // Select nodal Loads
            // var dicSelected = new Dictionary<NodalLoad, int>();
            var dicSelected = new Dictionary<Tuple<int,int>, NodalLoad>();
            foreach (var index in lcSelected)
            {
                foreach (var load in loads.GetLoadCase(index, ItemAt.AtNo).GetNodalLoads())
                {
                    var loadTuple = new Tuple<int, int>(load.No,index);
                    dicSelected.Add(loadTuple, load);
                }
            }
            foreach (var n in (dicSelected))
            {
                var include = true;
                foreach (var filter in filters)
                {
                    if (!(filter.NLList is null))
                    {
                        if (!filter.NLList.Contains(n.Value.No))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.NLComment is null))
                    {
                        if (!filter.NLComment.Contains(n.Value.Comment))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.NLNodeList is null))
                    {
                        var inNodesNo = n.Value.NodeList.ToInt();
                        if (!(inNodesNo.Intersect(filter.NLNodeList).ToArray().Length > 0))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.NLDefinition is null))
                    {
                        if (!filter.NLDefinition.Contains(n.Value.Definition.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.NLFx is null))
                    {
                        if (n.Value.Definition == LoadDefinitionType.ByComponentsType)
                        {
                            if (!filter.NLFx.Includes(n.Value.Component.Force.X / 1000))
                            {
                                include = false;
                                break;
                            }
                        }
                        else if (n.Value.Definition == LoadDefinitionType.ByDirectionType)
                        {
                            var fx = Vector3d.Multiply(Vector3d.XAxis, n.Value.Direction.Force * RFNodalLoad.SetOrientation(n.Value.Direction.RotationSequence, n.Value.Direction.RotationAngles));
                            if (!filter.NLFx.Includes(fx / 1000))
                            {
                                include = false;
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }                                           
                    }
                    if (!(filter.NLFy is null))
                    {
                        if (n.Value.Definition == LoadDefinitionType.ByComponentsType)
                        {
                            if (!filter.NLFy.Includes(n.Value.Component.Force.Y / 1000))
                            {
                                include = false;
                                break;
                            }
                        }
                        else if (n.Value.Definition == LoadDefinitionType.ByDirectionType)
                        {
                            var fy = Vector3d.Multiply(Vector3d.YAxis, n.Value.Direction.Force * RFNodalLoad.SetOrientation(n.Value.Direction.RotationSequence, n.Value.Direction.RotationAngles));
                            if (!filter.NLFy.Includes(fy / 1000))
                            {
                                include = false;
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (!(filter.NLFz is null))
                    {
                        if (n.Value.Definition == LoadDefinitionType.ByComponentsType)
                        {
                            if (!filter.NLFz.Includes(n.Value.Component.Force.Z / 1000))
                            {
                                include = false;
                                break;
                            }
                        }
                        else if (n.Value.Definition == LoadDefinitionType.ByDirectionType)
                        {
                            var fz = Vector3d.Multiply(Vector3d.ZAxis, n.Value.Direction.Force * RFNodalLoad.SetOrientation(n.Value.Direction.RotationSequence, n.Value.Direction.RotationAngles));
                            if (!filter.NLFz.Includes(fz / 1000))
                            {
                                include = false;
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (!(filter.NLMx is null))
                    {
                        if (n.Value.Definition == LoadDefinitionType.ByComponentsType)
                        {
                            if (!filter.NLMx.Includes(n.Value.Component.Moment.X / 1000))
                            {
                                include = false;
                                break;
                            }
                        }
                        else if (n.Value.Definition == LoadDefinitionType.ByDirectionType)
                        {
                            var mx = Vector3d.Multiply(Vector3d.XAxis, n.Value.Direction.Moment * RFNodalLoad.SetOrientation(n.Value.Direction.RotationSequence, n.Value.Direction.RotationAngles));
                            if (!filter.NLMx.Includes(mx / 1000))
                            {
                                include = false;
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (!(filter.NLMy is null))
                    {
                        if (n.Value.Definition == LoadDefinitionType.ByComponentsType)
                        {
                            if (!filter.NLMy.Includes(n.Value.Component.Moment.Y / 1000))
                            {
                                include = false;
                                break;
                            }
                        }
                        else if (n.Value.Definition == LoadDefinitionType.ByDirectionType)
                        {
                            var my = Vector3d.Multiply(Vector3d.YAxis, n.Value.Direction.Force * RFNodalLoad.SetOrientation(n.Value.Direction.RotationSequence, n.Value.Direction.RotationAngles));
                            if (!filter.NLMy.Includes(my / 1000))
                            {
                                include = false;
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (!(filter.NLMz is null))
                    {
                        if (n.Value.Definition == LoadDefinitionType.ByComponentsType)
                        {
                            if (!filter.NLMz.Includes(n.Value.Component.Moment.Z / 1000))
                            {
                                include = false;
                                break;
                            }
                        }
                        else if (n.Value.Definition == LoadDefinitionType.ByDirectionType)
                        {
                            var mz = Vector3d.Multiply(Vector3d.ZAxis, n.Value.Direction.Force * RFNodalLoad.SetOrientation(n.Value.Direction.RotationSequence, n.Value.Direction.RotationAngles));
                            if (!filter.NLMz.Includes(mz / 1000))
                            {
                                include = false;
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (!(filter.NLX is null))
                    {
                        foreach (var nodeNo in n.Value.NodeList.ToInt())
                        {
                            var node = data.GetNode(nodeNo, ItemAt.AtNo).GetData();
                            var rfNode = new RFNode(node, node.ToPoint3d(data));
                            if (!filter.NLX.Includes(rfNode.X))
                            {
                                include = false;
                                break;
                            }
                        }
                    }
                    if (!(filter.NLY is null))
                    {
                        foreach (var nodeNo in n.Value.NodeList.ToInt())
                        {
                            var node = data.GetNode(nodeNo, ItemAt.AtNo).GetData();
                            var rfNode = new RFNode(node, node.ToPoint3d(data));
                            if (!filter.NLY.Includes(rfNode.X))
                            {
                                include = false;
                                break;
                            }
                        }
                    }
                    if (!(filter.NLZ is null))
                    {
                        foreach (var nodeNo in n.Value.NodeList.ToInt())
                        {
                            var node = data.GetNode(nodeNo, ItemAt.AtNo).GetData();
                            var rfNode = new RFNode(node, node.ToPoint3d(data));
                            if (!filter.NLZ.Includes(rfNode.X))
                            {
                                include = false;
                                break;
                            }
                        }
                    }
                }
                if (!include) continue;
                outDictionaryLoads.Add(n.Key, n.Value);
            }
            return outDictionaryLoads;
        }

        public static List<RFNodalLoad> GetRFNodalLoads(Dictionary<Tuple<int, int>, NodalLoad> loads, IModelData data)
        {
            var rfNodalLoadList = new List<RFNodalLoad>();
            // Get Planesss
            foreach (var item in loads)
            {
                //var planes = new List<Plane>();
                var nodeList = new List<Node>();
                foreach (var node in item.Value.NodeList.ToInt())
                {
                    nodeList.Add(data.GetNode(node, ItemAt.AtNo).GetData());
                }
                var originList = from pt in GetRFNodes(nodeList, data)
                                 select pt.Location;
                var rfNodalLoad = new RFNodalLoad(item.Value, originList.ToList(), item.Key.Item2);
                rfNodalLoadList.Add(rfNodalLoad);
            }
            return rfNodalLoadList;
        }

        #endregion

        #region Line Loads

        public static Dictionary<Tuple<int, int>, LineLoad> FilterLineLoads(Dlubal.RFEM5.IModelData data, ILoads loads, List<RFFilter> filters)
        {
            var outDictionaryLoads = new Dictionary<Tuple<int, int>, LineLoad>();
            // Select load cases
            var lcAll = loads.GetLoadCases();
            var lcSelected = new List<int>();
            foreach (var lc in lcAll)
            {
                int lcIndex = lc.Loading.No;
                bool include = true;
                foreach (var filter in filters)
                {
                    if (!(filter.LLLC is null))
                    {
                        if (!filter.LLLC.Contains(lc.Loading.No))
                        {
                            include = false;
                            break;
                        }
                    }
                }
                if (include)
                {
                    lcSelected.Add(lcIndex);
                }
            }
            // Select nodal Loads
            var dicSelected = new Dictionary<Tuple<int, int>, LineLoad>();
            foreach (var index in lcSelected)
            {
                foreach (var load in loads.GetLoadCase(index, ItemAt.AtNo).GetLineLoads())
                {
                    var loadTuple = new Tuple<int, int>(load.No, index);
                    dicSelected.Add(loadTuple,load);
                }
            }
            foreach (var n in (dicSelected))
            {
                var include = true;
                foreach (var filter in filters)
                {
                    if (!(filter.LLList is null))
                    {
                        if (!filter.LLList.Contains(n.Value.No))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.LLComment is null))
                    {
                        if (!filter.LLComment.Contains(n.Value.Comment))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.LLLineList is null))
                    {
                        var inLinesNo = n.Value.LineList.ToInt();
                        if (!(inLinesNo.Intersect(filter.LLLineList).ToArray().Length > 0))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.LLF1 is null))
                    {
                        if (!filter.LLF1.Includes(n.Value.Magnitude1 / 1000))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.LLF2 is null))
                    {
                        var f2 = n.Value.Magnitude2;
                        if (n.Value.Distribution != LoadDistributionType.ConcentratedNxQType && n.Value.Distribution != LoadDistributionType.Concentrated2x2QType)
                        {
                            f2 /= 1000;
                        }
                        if (!filter.LLF2.Includes(f2))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.LLF3 is null))
                    {
                        if (!filter.LLF3.Includes(n.Value.Magnitude3 / 1000))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.LLt1 is null))
                    {
                        var distanceA = n.Value.DistanceA;
                        if (n.Value.RelativeDistances)
                        {
                            distanceA /= 100;
                        }
                            if (!filter.LLt1.Includes(distanceA))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.LLt2 is null))
                    {
                        var distanceB = n.Value.DistanceB;
                        if (n.Value.RelativeDistances)
                        {
                            distanceB /= 100;
                        }
                        if (!filter.LLt2.Includes(distanceB))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.LLType is null))
                    {
                        if (!filter.LLType.Contains(n.Value.Type.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.LLDir is null))
                    {
                        if (!filter.LLDir.Contains(n.Value.Direction.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.LLDist is null))
                    {
                        if (!filter.LLDist.Contains(n.Value.Distribution.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.LLRef is null))
                    {
                        if (!filter.LLRef.Contains(n.Value.ReferenceTo.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.LLTotalLength is null))
                    {
                        if (!filter.LLTotalLength.Contains(n.Value.OverTotalLength))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.LLRelativeDistances is null))
                    {
                        if (!filter.LLRelativeDistances.Contains(n.Value.OverTotalLength))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.LLX is null))
                    {
                        foreach (var lineNo in n.Value.LineList.ToInt())
                        {
                            foreach (var nodeNo in data.GetLine(lineNo, ItemAt.AtNo).GetData().NodeList.ToInt())

                            {
                                var node = data.GetNode(nodeNo, ItemAt.AtNo).GetData();
                                var rfNode = new RFNode(node, node.ToPoint3d(data));
                                if (!filter.LLX.Includes(rfNode.X))
                                {
                                    include = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (!(filter.LLY is null))
                    {
                        foreach (var lineNo in n.Value.LineList.ToInt())
                        {
                            foreach (var nodeNo in data.GetLine(lineNo, ItemAt.AtNo).GetData().NodeList.ToInt())

                            {
                                var node = data.GetNode(nodeNo, ItemAt.AtNo).GetData();
                                var rfNode = new RFNode(node, node.ToPoint3d(data));
                                if (!filter.LLY.Includes(rfNode.Y))
                                {
                                    include = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (!(filter.LLZ is null))
                    {
                        foreach (var lineNo in n.Value.LineList.ToInt())
                        {
                            foreach (var nodeNo in data.GetLine(lineNo, ItemAt.AtNo).GetData().NodeList.ToInt())

                            {
                                var node = data.GetNode(nodeNo, ItemAt.AtNo).GetData();
                                var rfNode = new RFNode(node, node.ToPoint3d(data));
                                if (!filter.LLZ.Includes(rfNode.Z))
                                {
                                    include = false;
                                    break;
                                }
                            }
                        }
                    }
                }
                if (!include) continue;
                outDictionaryLoads.Add(n.Key, n.Value);
            }
            return outDictionaryLoads;
        }

        public static List<RFLineLoad> GetRFLineLoads(Dictionary<Tuple<int, int>, LineLoad> loads, IModelData data)
        {
            var rfLineLoadList = new List<RFLineLoad>();
            // Get Planesss
            foreach (var item in loads)
            {
                var lineList = new List<Dlubal.RFEM5.Line>();
                foreach (var line in item.Value.LineList.ToInt())
                {
                    lineList.Add(data.GetLine(line, ItemAt.AtNo).GetData());
                }
                var baseLines = GetRFLines(lineList, data);
                var rfLineLoad = new RFLineLoad(item.Value, baseLines.ToList(), item.Key.Item2);
                rfLineLoadList.Add(rfLineLoad);
            }
            return rfLineLoadList;
        }
        #endregion

        #region Member Loads

        public static Dictionary<Tuple<int, int>, MemberLoad> FilterMemberLoads(Dlubal.RFEM5.IModelData data, ILoads loads, List<RFFilter> filters)
        {
            var outDictionaryLoads = new Dictionary<Tuple<int, int>, MemberLoad>();
            //var outNodalLoads = new List<MemberLoad>();
            // Select load cases
            var lcAll = loads.GetLoadCases();
            var lcSelected = new List<int>();
            foreach (var lc in lcAll)
            {
                int lcIndex = lc.Loading.No;
                bool include = true;
                foreach (var filter in filters)
                {
                    if (!(filter.MLLC is null))
                    {
                        if (!filter.MLLC.Contains(lc.Loading.No))
                        {
                            include = false;
                            break;
                        }
                    }
                }
                if (include)
                {
                    lcSelected.Add(lcIndex);
                }
            }
            // Select nodal Loads
            var dicSelected = new Dictionary<Tuple<int, int>, MemberLoad>();
            foreach (var index in lcSelected)
            {
                foreach (var load in loads.GetLoadCase(index, ItemAt.AtNo).GetMemberLoads())
                {
                    var loadTuple = new Tuple<int, int>(load.No, index);
                    dicSelected.Add(loadTuple,load);
                }
            }
            foreach (var n in (dicSelected))
            {
                var include = true;
                foreach (var filter in filters)
                {
                    if (!(filter.MLList is null))
                    {
                        if (!filter.MLList.Contains(n.Value.No))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MLComment is null))
                    {
                        if (!filter.MLComment.Contains(n.Value.Comment))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MLMemberList is null))
                    {
                        var inMembersNo = n.Value.ObjectList.ToInt();
                        if (!(inMembersNo.Intersect(filter.MLMemberList).ToArray().Length > 0))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MLF1 is null))
                    {
                        var magnitude = n.Value.Magnitude1;
                        if (n.Value.Type == LoadType.ForceType || n.Value.Type == LoadType.MomentType)
                        {
                            magnitude *= 0.001;
                        }
                        if (!filter.MLF1.Includes(magnitude))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MLF2 is null))
                    {
                        var magnitude = n.Value.Magnitude2;
                        if (n.Value.Type == LoadType.ForceType || n.Value.Type == LoadType.MomentType)
                        {
                            if (n.Value.Distribution != LoadDistributionType.ConcentratedNxQType && n.Value.Distribution != LoadDistributionType.Concentrated2x2QType)
                            {
                                magnitude /= 1000;
                            }
                        }                        
                        if (!filter.MLF2.Includes(magnitude))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MLF3 is null))
                    {
                        var magnitude = n.Value.Magnitude3;
                        if (n.Value.Type == LoadType.ForceType || n.Value.Type == LoadType.MomentType)
                        {
                            magnitude *= 0.001;
                        }
                        if (!filter.MLF3.Includes(magnitude))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MLT4 is null))
                    {
                        if (!filter.MLT4.Includes(n.Value.Magnitude4))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MLT5 is null))
                    {
                        if (!filter.MLT5.Includes(n.Value.Magnitude5))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MLT6 is null))
                    {
                        if (!filter.MLT6.Includes(n.Value.Magnitude6))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MLt1 is null))
                    {
                        var distanceA = n.Value.DistanceA;
                        if (n.Value.RelativeDistances)
                        {
                            distanceA /= 100;
                        }
                        if (!filter.MLt1.Includes(distanceA))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MLt2 is null))
                    {
                        var distanceB = n.Value.DistanceB;
                        if (n.Value.RelativeDistances)
                        {
                            distanceB /= 100;
                        }
                        if (!filter.MLt2.Includes(distanceB))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MLType is null))
                    {
                        if (!filter.MLType.Contains(n.Value.Type.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MLDir is null))
                    {
                        if (!filter.MLDir.Contains(n.Value.Direction.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MLDist is null))
                    {
                        if (!filter.MLDist.Contains(n.Value.Distribution.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MLRef is null))
                    {
                        if (!filter.MLRef.Contains(n.Value.ReferenceTo.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MLTotalLength is null))
                    {
                        if (!filter.MLTotalLength.Contains(n.Value.OverTotalLength))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MLRelativeDistances is null))
                    {
                        if (!filter.MLRelativeDistances.Contains(n.Value.OverTotalLength))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.MLX is null))
                    {
                        foreach (var lineNo in n.Value.ObjectList.ToInt().Select(x => data.GetMember(x,ItemAt.AtNo).GetData().LineNo))
                        {
                            foreach (var nodeNo in data.GetLine(lineNo, ItemAt.AtNo).GetData().NodeList.ToInt())

                            {
                                var node = data.GetNode(nodeNo, ItemAt.AtNo).GetData();
                                var rfNode = new RFNode(node, node.ToPoint3d(data));
                                if (!filter.MLX.Includes(rfNode.X))
                                {
                                    include = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (!(filter.MLY is null))
                    {
                        foreach (var lineNo in n.Value.ObjectList.ToInt().Select(x => data.GetMember(x, ItemAt.AtNo).GetData().LineNo))
                        {
                            foreach (var nodeNo in data.GetLine(lineNo, ItemAt.AtNo).GetData().NodeList.ToInt())

                            {
                                var node = data.GetNode(nodeNo, ItemAt.AtNo).GetData();
                                var rfNode = new RFNode(node, node.ToPoint3d(data));
                                if (!filter.MLY.Includes(rfNode.Y))
                                {
                                    include = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (!(filter.MLZ is null))
                    {
                        foreach (var lineNo in n.Value.ObjectList.ToInt().Select(x => data.GetMember(x, ItemAt.AtNo).GetData().LineNo))
                        {
                            foreach (var nodeNo in data.GetLine(lineNo, ItemAt.AtNo).GetData().NodeList.ToInt())

                            {
                                var node = data.GetNode(nodeNo, ItemAt.AtNo).GetData();
                                var rfNode = new RFNode(node, node.ToPoint3d(data));
                                if (!filter.MLZ.Includes(rfNode.Z))
                                {
                                    include = false;
                                    break;
                                }
                            }
                        }
                    }
                }
                if (!include) continue;
                outDictionaryLoads.Add(n.Key, n.Value);
            }
            return outDictionaryLoads;
        }

        public static List<RFMemberLoad> GetRFMemberLoads(Dictionary<Tuple<int, int>, MemberLoad> loads, IModelData data)
        {
            var rfMemberLoadList = new List<RFMemberLoad>();
            // Get Planesss
            foreach (var item in loads)
            {
                var rfMemberLoad = new RFMemberLoad(item.Value, item.Key.Item2);
                rfMemberLoadList.Add(rfMemberLoad);
            }
            return rfMemberLoadList;
        }


        #endregion

        #region SurfaceLoads

        public static Dictionary<Tuple<int, int>, SurfaceLoad> FilterSurfaceLoads(Dlubal.RFEM5.IModelData data, ILoads loads, List<RFFilter> filters)
        {
            var outDictionaryLoads = new Dictionary<Tuple<int, int>, SurfaceLoad>();
            //var outNodalLoads = new List<MemberLoad>();
            // Select load cases
            var lcAll = loads.GetLoadCases();
            var lcSelected = new List<int>();
            foreach (var lc in lcAll)
            {
                int lcIndex = lc.Loading.No;
                bool include = true;
                foreach (var filter in filters)
                {
                    if (!(filter.SLLC is null))
                    {
                        if (!filter.SLLC.Contains(lc.Loading.No))
                        {
                            include = false;
                            break;
                        }
                    }
                }
                if (include)
                {
                    lcSelected.Add(lcIndex);
                }
            }
            // Select nodal Loads
            var dicSelected = new Dictionary<Tuple<int, int>, SurfaceLoad>();
            foreach (var index in lcSelected)
            {
                foreach (var load in loads.GetLoadCase(index, ItemAt.AtNo).GetSurfaceLoads())
                {
                    var loadTuple = new Tuple<int, int>(load.No, index);
                    dicSelected.Add(loadTuple,load);
                }
            }
            foreach (var n in (dicSelected))
            {
                var include = true;
                foreach (var filter in filters)
                {
                    if (!(filter.SLList is null))
                    {
                        if (!filter.SLList.Contains(n.Value.No))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SLComment is null))
                    {
                        if (!filter.SLComment.Contains(n.Value.Comment))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SLSurfaceList is null))
                    {
                        var inSrfcsNo = n.Value.SurfaceList.ToInt();
                        if (!(inSrfcsNo.Intersect(filter.SLSurfaceList).ToArray().Length > 0))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SLF1 is null))
                    {
                        var magnitude = n.Value.Magnitude1;
                        if (n.Value.Type == LoadType.ForceType || n.Value.Type == LoadType.MomentType)
                        {
                            magnitude *= 0.001;
                        }
                        if (!filter.SLF1.Includes(magnitude))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SLF2 is null))
                    {
                        var magnitude = n.Value.Magnitude2;
                        if (n.Value.Type == LoadType.ForceType || n.Value.Type == LoadType.MomentType)
                        {
                            magnitude *= 0.001;
                        }
                        if (!filter.SLF2.Includes(magnitude))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SLF3 is null))
                    {
                        var magnitude = n.Value.Magnitude3;
                        if (n.Value.Type == LoadType.ForceType || n.Value.Type == LoadType.MomentType)
                        {
                            magnitude *= 0.001;
                        }
                        if (!filter.SLF3.Includes(magnitude))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SLT4 is null))
                    {
                        if (!filter.SLT4.Includes(n.Value.Magnitude4))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SLT5 is null))
                    {
                        if (!filter.SLT5.Includes(n.Value.Magnitude5))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SLT6 is null))
                    {
                        if (!filter.SLT6.Includes(n.Value.Magnitude6))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SLType is null))
                    {
                        if (!filter.SLType.Contains(n.Value.Type.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SLDir is null))
                    {
                        if (!filter.SLDir.Contains(n.Value.Direction.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SLDist is null))
                    {
                        if (!filter.SLDist.Contains(n.Value.Distribution.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.SLX is null))
                    {
                        foreach (var srfcNo in n.Value.SurfaceList.ToInt())
                        {
                            foreach (var lineNo in data.GetSurface(srfcNo, ItemAt.AtNo).GetData().BoundaryLineList.ToInt())
                            {
                                foreach (var nodeNo in data.GetLine(lineNo, ItemAt.AtNo).GetData().NodeList.ToInt())
                                {
                                    var node = data.GetNode(nodeNo, ItemAt.AtNo).GetData();
                                    var rfNode = new RFNode(node, node.ToPoint3d(data));
                                    if (!filter.SLX.Includes(rfNode.X))
                                    {
                                        include = false;
                                        break;
                                    }
                                }
                            }

                        }
                    }
                    if (!(filter.SLY is null))
                    {
                        foreach (var srfcNo in n.Value.SurfaceList.ToInt())
                        {
                            foreach (var lineNo in data.GetSurface(srfcNo, ItemAt.AtNo).GetData().BoundaryLineList.ToInt())
                            {
                                foreach (var nodeNo in data.GetLine(lineNo, ItemAt.AtNo).GetData().NodeList.ToInt())
                                {
                                    var node = data.GetNode(nodeNo, ItemAt.AtNo).GetData();
                                    var rfNode = new RFNode(node, node.ToPoint3d(data));
                                    if (!filter.SLY.Includes(rfNode.Y))
                                    {
                                        include = false;
                                        break;
                                    }
                                }
                            }

                        }
                    }
                    if (!(filter.SLZ is null))
                    {
                        foreach (var srfcNo in n.Value.SurfaceList.ToInt())
                        {
                            foreach (var lineNo in data.GetSurface(srfcNo, ItemAt.AtNo).GetData().BoundaryLineList.ToInt())
                            {
                                foreach (var nodeNo in data.GetLine(lineNo, ItemAt.AtNo).GetData().NodeList.ToInt())
                                {
                                    var node = data.GetNode(nodeNo, ItemAt.AtNo).GetData();
                                    var rfNode = new RFNode(node, node.ToPoint3d(data));
                                    if (!filter.SLZ.Includes(rfNode.Z))
                                    {
                                        include = false;
                                        break;
                                    }
                                }
                            }

                        }
                    }
                }
                if (!include) continue;
                outDictionaryLoads.Add(n.Key, n.Value);
            }
            return outDictionaryLoads;
        }

        public static List<RFSurfaceLoad> GetRFSurfaceLoads(Dictionary<Tuple<int, int>, SurfaceLoad> loads, IModelData data)
        {
            var rfSurfaceLoadList = new List<RFSurfaceLoad>();
            // Get Planesss
            foreach (var item in loads)
            {
                var rfSurfaceLoad = new RFSurfaceLoad(item.Value, item.Key.Item2);
                rfSurfaceLoadList.Add(rfSurfaceLoad);
            }
            return rfSurfaceLoadList;
        }

        #endregion

        #region PolyLoads

        public static Dictionary<Tuple<int, int>, FreePolygonLoad> FilterPolyLoads(Dlubal.RFEM5.IModelData data, ILoads loads, List<RFFilter> filters)
        {
            var outDictionaryLoads = new Dictionary<Tuple<int, int>, FreePolygonLoad>();
            //var outNodalLoads = new List<MemberLoad>();
            // Select load cases
            var lcAll = loads.GetLoadCases();
            var lcSelected = new List<int>();
            foreach (var lc in lcAll)
            {
                int lcIndex = lc.Loading.No;
                bool include = true;
                foreach (var filter in filters)
                {
                    if (!(filter.PLLC is null))
                    {
                        if (!filter.PLLC.Contains(lc.Loading.No))
                        {
                            include = false;
                            break;
                        }
                    }
                }
                if (include)
                {
                    lcSelected.Add(lcIndex);
                }
            }
            // Select nodal Loads
            var dicSelected = new Dictionary<Tuple<int, int>, FreePolygonLoad>();
            foreach (var index in lcSelected)
            {
                foreach (var load in loads.GetLoadCase(index, ItemAt.AtNo).GetFreePolygonLoads())
                {
                    var loadTuple = new Tuple<int, int>(load.No, index);
                    dicSelected.Add(loadTuple,load);
                }
            }
            foreach (var n in (dicSelected))
            {
                var include = true;
                foreach (var filter in filters)
                {
                    if (!(filter.PLList is null))
                    {
                        if (!filter.PLList.Contains(n.Value.No))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.PLComment is null))
                    {
                        if (!filter.PLComment.Contains(n.Value.Comment))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.PLSurfaceList is null))
                    {
                        var inSrfcsNo = n.Value.SurfaceList.ToInt();
                        if (!(inSrfcsNo.Intersect(filter.PLSurfaceList).ToArray().Length > 0))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.PLF1 is null))
                    {
                        if (!filter.PLF1.Includes(n.Value.Magnitude1/1000))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.PLF2 is null))
                    {
                        if (!filter.PLF2.Includes(n.Value.Magnitude2 / 1000))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.PLF3 is null))
                    {
                        if (!filter.PLF3.Includes(n.Value.Magnitude3 / 1000))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.PLProjection is null))
                    {
                        if (!filter.PLProjection.Contains(n.Value.ProjectionPlane.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.PLDir is null))
                    {
                        if (!filter.PLDir.Contains(n.Value.Direction.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.PLDist is null))
                    {
                        if (!filter.PLDist.Contains(n.Value.Distribution.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                    // Get list of vertices to filter coordinates
                    var polys = new List<Brep>();
                    var vertList = new List<Point3d>();
                    if (!((filter.PLX is null) && (filter.PLY is null) && (filter.PLZ is null)))
                    {
                        var rfsfcs = new List<RFSurface>();
                        var allSfcs = GetRFSurfaces(data.GetSurfaces().ToList(), data);
                        if (n.Value.SurfaceList!= "")
                        {
                            foreach (var sfcNo in n.Value.SurfaceList.ToInt())
                            {
                                rfsfcs.AddRange(allSfcs.Where(x => x.No == sfcNo));
                            }
                        }else
                        {
                            rfsfcs = allSfcs;
                        }
                        var polyLoad = new RFFreePolygonLoad(n.Value, n.Key.Item2);
                        polys = polyLoad.GetPolygons(rfsfcs);
                        foreach (var vertices in polys.Select(x => x.Vertices.Select(y => y.Location)))
                        {
                            vertList.AddRange(vertices);
                        }                        
                    }
                    // filter coordinates
                    if (!(filter.PLX is null))
                    {
                        foreach (var vert in vertList)
                        {
                            if (!filter.PLX.Includes(vert.X))
                            {
                                include = false;
                                break;
                            }
                        }
                    }
                    if (!(filter.PLY is null))
                    {
                        foreach (var vert in vertList)
                        {
                            if (!filter.PLY.Includes(vert.Y))
                            {
                                include = false;
                                break;
                            }
                        }
                    }
                    if (!(filter.PLZ is null))
                    {
                        foreach (var vert in vertList)
                        {
                            if (!filter.PLZ.Includes(vert.Z))
                            {
                                include = false;
                                break;
                            }
                        }
                    }
                }
                if (!include) continue;
                outDictionaryLoads.Add(n.Key, n.Value);
            }
            return outDictionaryLoads;
        }

        public static List<RFFreePolygonLoad> GetRFPolyLoads(Dictionary<Tuple<int, int>, FreePolygonLoad> loads, IModelData data)
        {
            var rfPolyLoadList = new List<RFFreePolygonLoad>();
            // Get Planesss
            foreach (var item in loads)
            {
                var rfPolyLoad = new RFFreePolygonLoad(item.Value, item.Key.Item2);
                // Get polygons
                var rfsfcs = new List<RFSurface>();
                var allSfcs = GetRFSurfaces(data.GetSurfaces().ToList(), data);
                if (rfPolyLoad.SurfaceList != "")
                {
                    foreach (var sfcNo in rfPolyLoad.SurfaceList.ToInt())
                    {
                        rfsfcs.AddRange(allSfcs.Where(x => x.No == sfcNo));
                    }
                }
                else
                {
                    rfsfcs = allSfcs;
                }
                rfPolyLoad.Polygon = rfPolyLoad.GetPolygons(rfsfcs);
                rfPolyLoadList.Add(rfPolyLoad);
            }
            return rfPolyLoadList;
        }

        #endregion

        #region Fre  Line Loads

        public static Dictionary<Tuple<int, int>, FreeLineLoad> FilterFreeLineLoads(Dlubal.RFEM5.IModelData data, ILoads loads, List<RFFilter> filters)
        {
            var outDictionaryLoads = new Dictionary<Tuple<int, int>, FreeLineLoad>();
            // Select load cases
            var lcAll = loads.GetLoadCases();
            var lcSelected = new List<int>();
            foreach (var lc in lcAll)
            {
                int lcIndex = lc.Loading.No;
                bool include = true;
                foreach (var filter in filters)
                {
                    if (!(filter.FLLLC is null))
                    {
                        if (!filter.FLLLC.Contains(lc.Loading.No))
                        {
                            include = false;
                            break;
                        }
                    }
                }
                if (include)
                {
                    lcSelected.Add(lcIndex);
                }
            }
            // Select nodal Loads
            var dicSelected = new Dictionary<Tuple<int, int>, FreeLineLoad>();
            foreach (var index in lcSelected)
            {
                foreach (var load in loads.GetLoadCase(index, ItemAt.AtNo).GetFreeLineLoads())
                {
                    var loadTuple = new Tuple<int, int>(load.No, index);
                    dicSelected.Add(loadTuple, load);
                }
            }
            foreach (var n in (dicSelected))
            {
                var include = true;
                foreach (var filter in filters)
                {
                    if (!(filter.FLLList is null))
                    {
                        if (!filter.FLLList.Contains(n.Value.No))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.FLLComment is null))
                    {
                        if (!filter.FLLComment.Contains(n.Value.Comment))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.FLLSfcList is null))
                    {
                        var inLinesNo = n.Value.SurfaceList.ToInt();
                        if (!(inLinesNo.Intersect(filter.FLLSfcList).ToArray().Length > 0))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.FLLF1 is null))
                    {
                        if (!filter.FLLF1.Includes(n.Value.Magnitude1 / 1000))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.FLLF2 is null))
                    {
                        if (!filter.FLLF2.Includes(n.Value.Magnitude2 / 1000))
                        {
                            include = false;
                            break;
                        }
                    }                    
                    if (!(filter.FLLDir is null))
                    {
                        if (!filter.FLLDir.Contains(n.Value.Direction.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.FLLDist is null))
                    {
                        if (!filter.FLLDist.Contains(n.Value.Distribution.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.FLLProj is null))
                    {
                        if (!filter.FLLProj.Contains(n.Value.ProjectionPlane.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.FLLX is null))
                    {                        
                        if (!filter.FLLX.Includes(n.Value.Position1.X) || !filter.FLLX.Includes(n.Value.Position2.X))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.FLLY is null))
                    {
                        if (!filter.FLLY.Includes(n.Value.Position1.Y) || !filter.FLLY.Includes(n.Value.Position2.Y))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.FLLZ is null))
                    {
                        if (!filter.FLLZ.Includes(n.Value.Position1.Z) || !filter.FLLZ.Includes(n.Value.Position2.Z))
                        {
                            include = false;
                            break;
                        }
                    }
                }
                if (!include) continue;
                outDictionaryLoads.Add(n.Key, n.Value);
            }
            return outDictionaryLoads;
        }

        public static List<RFFreeLineLoad> GetRFFreeLineLoads(Dictionary<Tuple<int, int>, FreeLineLoad> loads, IModelData data)
        {
            var rfLineLoadList = new List<RFFreeLineLoad>();
            // Get Planesss
            foreach (var item in loads)
            {
                var rfLineLoad = new RFFreeLineLoad(item.Value, item.Key.Item2);
                rfLineLoadList.Add(rfLineLoad);
            }
            return rfLineLoadList;
        }
        #endregion

        #region LoadCase
        public static List<LoadCase> FilterLoadCases(Dlubal.RFEM5.IModelData data, ILoads loads, List<RFFilter> filters)
        {
            var outLoadCases = new List<LoadCase>();

            foreach (var lc in loads.GetLoadCases())
            {
                var include = true;
                foreach (var filter in filters)
                {
                    if (!(filter.LCList is null))
                    {
                        if (!filter.LCList.Contains(lc.Loading.No))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.LCComment is null))
                    {
                        if (!filter.LCComment.Contains(lc.Comment))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.LCDescription is null))
                    {
                        if (!filter.LCDescription.Contains(lc.Description))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.LCAction is null))
                    {
                        if (!filter.LCAction.Contains(lc.ActionCategory.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.LCSWX is null))
                    {
                        if (!filter.LCSWX.Includes(lc.SelfWeightFactor.X))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.LCSWY is null))
                    {
                        if (!filter.LCSWY.Includes(lc.SelfWeightFactor.Y))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.LCSWZ is null))
                    {
                        if (!filter.LCSWZ.Includes(lc.SelfWeightFactor.Z))
                        {
                            include = false;
                            break;
                        }
                    }

                    if (!(filter.LCToSolve is null))
                    {
                        if (!filter.LCToSolve.Contains(lc.ToSolve))
                        {
                            include = false;
                            break;
                        }
                    }                    
                }
                if (!include) continue;
                outLoadCases.Add(lc);
            }
            return outLoadCases;
        }

        public static List<RFLoadCase> GetRFLoadCases(List<LoadCase> loadcases, IModelData data)
        {
            var rfLoadCases = new List<RFLoadCase>();
            // Get Planesss
            foreach (var item in loadcases)
            {
                rfLoadCases.Add(new RFLoadCase(item));
            }
            return rfLoadCases;
        }

        #endregion

        #region LoadCombo

        public static List<LoadCombination> FilterLoadCombos(Dlubal.RFEM5.IModelData data, ILoads loads, List<RFFilter> filters)
        {
            var outLoadCombos = new List<LoadCombination>();

            foreach (var lc in loads.GetLoadCombinations())
            {
                var include = true;
                foreach (var filter in filters)
                {
                    if (!(filter.LCoList is null))
                    {
                        if (!filter.LCoList.Contains(lc.Loading.No))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.LCoComment is null))
                    {
                        if (!filter.LCoComment.Contains(lc.Comment))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.LCoDefinition is null))
                    {
                        if (!filter.LCoDefinition.Contains(lc.Definition))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.LCoDescription is null))
                    {
                        if (!filter.LCoDescription.Contains(lc.Description))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.LCoDesign is null))
                    {
                        if (!filter.LCoDesign.Contains(lc.DesignSituation.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.LCoToSolve is null))
                    {
                        if (!filter.LCoToSolve.Contains(lc.ToSolve))
                        {
                            include = false;
                            break;
                        }
                    }
                }
                if (!include) continue;
                outLoadCombos.Add(lc);
            }
            return outLoadCombos;
        }

        public static List<RFLoadCombo> GetRFLoadCombos(List<LoadCombination> loadcombos, IModelData data)
        {
            var rfLoadCombos = new List<RFLoadCombo>();
            // Get Planesss
            foreach (var item in loadcombos)
            {
                rfLoadCombos.Add(new RFLoadCombo(item));
            }
            return rfLoadCombos;
        }

        #endregion

        #region ResultCombo

        public static List<ResultCombination> FilterResultCombos(Dlubal.RFEM5.IModelData data, ILoads loads, List<RFFilter> filters)
        {
            var outLoadCombos = new List<ResultCombination>();

            foreach (var lc in loads.GetResultCombinations())
            {
                var include = true;
                foreach (var filter in filters)
                {
                    if (!(filter.RCoList is null))
                    {
                        if (!filter.RCoList.Contains(lc.Loading.No))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.RCoComment is null))
                    {
                        if (!filter.RCoComment.Contains(lc.Comment))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.RCoDefinition is null))
                    {
                        if (!filter.RCoDefinition.Contains(lc.Definition))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.RCoDescription is null))
                    {
                        if (!filter.RCoDescription.Contains(lc.Description))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.RCoDesign is null))
                    {
                        if (!filter.RCoDesign.Contains(lc.DesignSituation.ToString()))
                        {
                            include = false;
                            break;
                        }
                    }
                    if (!(filter.RCoToSolve is null))
                    {
                        if (!filter.RCoToSolve.Contains(lc.ToSolve))
                        {
                            include = false;
                            break;
                        }
                    }
                }
                if (!include) continue;
                outLoadCombos.Add(lc);
            }
            return outLoadCombos;
        }

        public static List<RFResultCombo> GetRFResultCombos(List<ResultCombination> resultcombos, IModelData data)
        {
            var rfResultCombos = new List<RFResultCombo>();
            // Get Planesss
            foreach (var item in resultcombos)
            {
                rfResultCombos.Add(new RFResultCombo(item));
            }
            return rfResultCombos;
        }

        #endregion

        #region RFEM
        public static void ClearOutput(ref List<RFNode> rfNodes, ref List<RFLine> rfLines, ref List<RFMember> rfMembers, ref List<RFSurface> rfSurfaces,
          ref List<RFOpening> rfOpenings, ref List<RFSupportP> rfSupportss, ref List<RFSupportL> rfSupportsL, ref List<RFLineHinge> rfLineHinges,
          ref List<RFCroSec> rfCroSecs, ref List<RFMaterial> rfMats, ref List<RFNodalLoad> rfNLoads)
        {
            rfNodes.Clear();
            rfLines.Clear();
            rfMembers.Clear();
            rfSurfaces.Clear();
            rfOpenings.Clear();
            rfSupportss.Clear();
            rfSupportsL.Clear();
            rfLineHinges.Clear();
            rfCroSecs.Clear();
            rfMats.Clear();
            rfNLoads.Clear();
        }

        public static void ClearOutput(ref List<RFNode> rfNodes, ref List<RFLine> rfLines, ref List<RFMember> rfMembers, ref List<RFSurface> rfSurfaces,
  ref List<RFOpening> rfOpenings, ref List<RFSupportP> rfSupportss, ref List<RFSupportL> rfSupportsL, ref List<RFLineHinge> rfLineHinges,
  ref List<RFCroSec> rfCroSecs, ref List<RFMaterial> rfMats, ref List<RFNodalLoad> rfNLoads, ref List<RFLineLoad> rfLLoads, ref List<RFMemberLoad> rfMLoads, 
  ref List<RFSurfaceLoad> rfSLoads, ref List<RFFreePolygonLoad> rfPLoads, ref List<RFLoadCase> rfLoadCases, ref List<RFLoadCombo> rfLoadCombos,
  ref List<RFResultCombo> rfResultCombos)
        {
            rfNodes.Clear();
            rfLines.Clear();
            rfMembers.Clear();
            rfSurfaces.Clear();
            rfOpenings.Clear();
            rfSupportss.Clear();
            rfSupportsL.Clear();
            rfLineHinges.Clear();
            rfCroSecs.Clear();
            rfMats.Clear();
            rfNLoads.Clear();
            rfLLoads.Clear();
            rfMLoads.Clear();
            rfSLoads.Clear();
            rfPLoads.Clear();
            rfLoadCases.Clear();
            rfLoadCombos.Clear();
            rfResultCombos.Clear();
        }

        public static void ClearOutput(ref List<RFNode> rfNodes, ref List<RFLine> rfLines, ref List<RFMember> rfMembers, ref List<RFSurface> rfSurfaces,
  ref List<RFOpening> rfOpenings, ref List<RFSupportP> rfSupportss, ref List<RFSupportL> rfSupportsL, ref List<RFSupportS> rfSupportsS, ref List<RFLineHinge> rfLineHinges,
  ref List<RFCroSec> rfCroSecs, ref List<RFMaterial> rfMats, ref List<RFNodalLoad> rfNLoads, ref List<RFLineLoad> rfLLoads, ref List<RFMemberLoad> rfMLoads,
  ref List<RFSurfaceLoad> rfSLoads, ref List<RFFreePolygonLoad> rfPLoads, ref List<RFLoadCase> rfLoadCases, ref List<RFLoadCombo> rfLoadCombos,
  ref List<RFResultCombo> rfResultCombos)
        {
            rfNodes.Clear();
            rfLines.Clear();
            rfMembers.Clear();
            rfSurfaces.Clear();
            rfOpenings.Clear();
            rfSupportss.Clear();
            rfSupportsL.Clear();
            rfSupportsS.Clear();
            rfLineHinges.Clear();
            rfCroSecs.Clear();
            rfMats.Clear();
            rfNLoads.Clear();
            rfLLoads.Clear();
            rfMLoads.Clear();
            rfSLoads.Clear();
            rfPLoads.Clear();
            rfLoadCases.Clear();
            rfLoadCombos.Clear();
            rfResultCombos.Clear();
        }

        public static void ClearOutput(ref List<RFNode> rfNodes, ref List<RFLine> rfLines, ref List<RFMember> rfMembers, ref List<RFSurface> rfSurfaces,
ref List<RFOpening> rfOpenings, ref List<RFSupportP> rfSupportss, ref List<RFSupportL> rfSupportsL, ref List<RFSupportS> rfSupportsS, ref List<RFLineHinge> rfLineHinges,
ref List<RFCroSec> rfCroSecs, ref List<RFMaterial> rfMats, ref List<RFNodalLoad> rfNLoads, ref List<RFLineLoad> rfLLoads, ref List<RFMemberLoad> rfMLoads,
ref List<RFSurfaceLoad> rfSLoads, ref List<RFFreePolygonLoad> rfPLoads, ref List<RFLoadCase> rfLoadCases, ref List<RFLoadCombo> rfLoadCombos,
ref List<RFResultCombo> rfResultCombos, ref List<RFMemberHinge> rfMemberHinges)
        {
            rfNodes.Clear();
            rfLines.Clear();
            rfMembers.Clear();
            rfSurfaces.Clear();
            rfOpenings.Clear();
            rfSupportss.Clear();
            rfSupportsL.Clear();
            rfSupportsS.Clear();
            rfLineHinges.Clear();
            rfCroSecs.Clear();
            rfMats.Clear();
            rfNLoads.Clear();
            rfLLoads.Clear();
            rfMLoads.Clear();
            rfSLoads.Clear();
            rfPLoads.Clear();
            rfLoadCases.Clear();
            rfLoadCombos.Clear();
            rfResultCombos.Clear();
            rfMemberHinges.Clear();
        }

        public static void ClearOutput(ref List<RFNode> rfNodes, ref List<RFLine> rfLines, ref List<RFMember> rfMembers, ref List<RFSurface> rfSurfaces,
ref List<RFOpening> rfOpenings, ref List<RFSupportP> rfSupportss, ref List<RFSupportL> rfSupportsL, ref List<RFSupportS> rfSupportsS, ref List<RFLineHinge> rfLineHinges,
ref List<RFCroSec> rfCroSecs, ref List<RFMaterial> rfMats, ref List<RFNodalLoad> rfNLoads, ref List<RFLineLoad> rfLLoads, ref List<RFMemberLoad> rfMLoads,
ref List<RFSurfaceLoad> rfSLoads, ref List<RFFreePolygonLoad> rfPLoads, ref List<RFLoadCase> rfLoadCases, ref List<RFLoadCombo> rfLoadCombos,
ref List<RFResultCombo> rfResultCombos, ref List<RFMemberHinge> rfMemberHinges, ref List<RFNodalRelease> rfNodalReleases, ref List<RFFreeLineLoad> rfFLLoads)
        {
            rfNodes.Clear();
            rfLines.Clear();
            rfMembers.Clear();
            rfSurfaces.Clear();
            rfOpenings.Clear();
            rfSupportss.Clear();
            rfSupportsL.Clear();
            rfSupportsS.Clear();
            rfLineHinges.Clear();
            rfCroSecs.Clear();
            rfMats.Clear();
            rfNLoads.Clear();
            rfLLoads.Clear();
            rfMLoads.Clear();
            rfSLoads.Clear();
            rfPLoads.Clear();
            rfLoadCases.Clear();
            rfLoadCombos.Clear();
            rfResultCombos.Clear();
            rfMemberHinges.Clear();
            rfNodalReleases.Clear();
            rfFLLoads.Clear();
        }


        public static void ConnectRFEM(ref IModel model, ref IModelData data)
        {
            try
            {
                // Connect to an opened RFEM model
                model = Marshal.GetActiveObject("RFEM5.Model") as IModel;
                // checks RF-COM license and locks the application for using by COM
                model.GetApplication().LockLicense();
                data = model.GetModelData();
                // We want to rewrite the lists, but not to add new items.   

            }
            catch (Exception ex)
            {
                //this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No REFM Model could be found.");
                model = null;
                data = null;
                throw new Exception("Impossible to connect to RFEM Model", ex);
            }
        }

        public static void ConnectRFEM(string modelName, ref IModel model, ref IModelData data)
        {
            try
            {
                // gets interface to a running RFEM
                IApplication app = Marshal.GetActiveObject("RFEM5.Application") as IApplication;
                // checks RF-COM license and locks the application for using by COM
                app.LockLicense();
                // Connect to an opened RFEM model
                model = null;
                data = null;
                var totalModels = app.GetModelCount();
                for (int i = 0; i < totalModels; i++)
                {
                    var myModel = app.GetModel(i);
                    var myName = myModel.GetName();
                    //var modelMatches = String.Compare(modelName, myName);
                    if (myName.Contains(modelName))
                    {
                        model = myModel;
                        break;
                    }
                }
                // checks RF-COM license and locks the application for using by COM
                app.UnlockLicense();
                if (model is null)
                {
                    //this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "There is no RFEM Model available with the provided name.");
                    throw new Exception("There is no RFEM Model available with the provided name.");
                }
                model.GetApplication().LockLicense();
                data = model.GetModelData();
                // We want to rewrite the lists, but not to add new items.    
            }
            catch (Exception ex)
            {
                //this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Impossible to connect to RFEM Model.");
                model = null;
                data = null;
                throw new Exception("Impossible to connect to RFEM Model.", ex);
            }
        }
        public static void DisconnectRFEM(ref IModel model, ref IModelData data)
        {
            // unlocks the application and releases RF-COM license
            model.GetApplication().UnlockLicense();
            // releases COM object
            model = null;
            data = null;
            // cleans Garbage Collector for releasing all COM interfaces and objects
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
        }

        public static void GetLoadsFromRFEM(IModel model, ref ILoads loads)
        {
            try
            {
                // Already Connected to an opened RFEM model
                //model = Marshal.GetActiveObject("RFEM5.Model") as IModel;
                loads = model.GetLoads();

                // We want to rewrite the lists, but not to add new items.    
            }
            catch (Exception ex)
            {
                //this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No REFM Model could be found.");
                model = null;
                loads = null;
                throw new Exception("Impossible to connect to RFEM Model", ex);
            }
        }
        #endregion
    }
}