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
    public static class Component_SetData
    {
        // Tolerance for merging nodes closer to this distance [m].
        static double tol = 0.001;

        public static void SetRFNodes (this IModelData data, List<GH_RFEM> ghNodes, ref List<RFNode> index)
        {
            var newData = false;
            var inNodes = new List<RFNode>();
            var existingNodes = new List<RFNode>();
            var lastNoNo = 0;

            inNodes = ghNodes.Select(x => new RFNode((RFNode)x.Value)).ToList();
            foreach (var rfNode in inNodes)
            {
                if (rfNode.ToModify)
                {
                    var myNode = (Node)rfNode;
                    var myNo = myNode.No;
                    //if (rfNode.NewNo>0)
                    //{
                    //    myNode.No = rfNode.NewNo;
                    //}
                    data.GetNode(myNo, ItemAt.AtNo).SetData(ref myNode);
                    index.Add(rfNode);
                }
                else if (rfNode.ToDelete)
                {
                    data.GetNode(rfNode.No, ItemAt.AtNo).Delete();
                    index.Add(rfNode);
                }
                else
                {
                    if (newData == false)
                    {
                        existingNodes = Component_GetData.GetRFNodes(data.GetNodes().ToList(), data);
                        lastNoNo = data.GetLastObjectNo(ModelObjectType.NodeObject);
                        newData = true;
                    }
                    //var myNode = (Node)rfNode;
                    index.Add(data.SetRFNode(rfNode, ref existingNodes, ref lastNoNo, tol));
                }
            }
        }

        public static void SetRFNodes(this IModelData data, List<GH_RFEM> ghNodes, ref List<RFNode> index, ref List<string> errorMsg)
        {
            var newData = false;
            var inNodes = new List<RFNode>();
            var existingNodes = new List<RFNode>();
            var lastNoNo = 0;

            inNodes = ghNodes.Select(x => new RFNode((RFNode)x.Value)).ToList();
            foreach (var rfNode in inNodes)
            {
                try
                {
                    if (rfNode.ToModify)
                {
                    var myNode = (Node)rfNode;
                    var myNo = myNode.No;
                    //if (rfNode.NewNo>0)
                    //{
                    //    myNode.No = rfNode.NewNo;
                    //}
                    data.GetNode(myNo, ItemAt.AtNo).SetData(ref myNode);
                    index.Add(rfNode);
                }
                else if (rfNode.ToDelete)
                {
                    data.GetNode(rfNode.No, ItemAt.AtNo).Delete();
                    index.Add(rfNode);
                }
                else
                {
                    if (newData == false)
                    {
                        existingNodes = Component_GetData.GetRFNodes(data.GetNodes().ToList(), data);
                        lastNoNo = data.GetLastObjectNo(ModelObjectType.NodeObject);
                        newData = true;
                    }
                    //var myNode = (Node)rfNode;
                    index.Add(data.SetRFNode(rfNode, ref existingNodes, ref lastNoNo, tol));
                }
                }
                catch (Exception ex)
                {
                    index.Add(null);
                    errorMsg.Add($"Import of Node No.{rfNode.No} failed! " + ex.Message);
                }
            }
        }

        public static RFNode SetRFNode(this IModelData data, ref RFNode rfNode, ref List<RFNode> existingNodes, ref int lastNo, double tol)
        {
            // Check if there is already a node close to the target location
            foreach (var node in existingNodes)
            {
                if (rfNode.Location.DistanceTo(node.Location) < tol)
                {
                    return node;
                }
            }
            // Set node with a provided index number
            if (!(rfNode.No == 0))
            {
                data.SetNode(rfNode);
                existingNodes.Add(rfNode);
                return rfNode;
            }
            // Set node without provided index number
            lastNo += 1;
            rfNode.No = lastNo;
            data.SetNode(rfNode);
            existingNodes.Add(rfNode);
            return rfNode;
        }
        public static void SetRFLines(this IModelData data, List<GH_RFEM> ghLines, ref List<RFLine> index)
        {
            var newData = false;
            var inLines = new List<RFLine>();
            var existingNodes = new List<RFNode>();
            var existingLines = new List<RFLine>();
            var lastNoNo = 0;
            var lastLineNo = 0;

            inLines = ghLines.Select(x => new RFLine((RFLine)x.Value)).ToList();
            foreach (var rfLine in inLines)
            {
                if (rfLine.ToModify)
                {
                    var myLine = (Dlubal.RFEM5.Line)rfLine;
                    var myNo = rfLine.No;
                    data.GetLine(myNo, ItemAt.AtNo).SetData(ref myLine);
                    index.Add(rfLine);
                }
                else if (rfLine.ToDelete)
                {
                    data.GetLine(rfLine.No, ItemAt.AtNo).Delete();
                    index.Add(rfLine);
                }
                else
                {
                    if (newData == false)
                    {
                        existingNodes = Component_GetData.GetRFNodes(data.GetNodes().ToList(), data);
                        existingLines = Component_GetData.GetRFLines(data.GetLines().ToList(), data);
                        lastNoNo = data.GetLastObjectNo(ModelObjectType.NodeObject);
                        lastLineNo = data.GetLastObjectNo(ModelObjectType.LineObject);
                        newData = true;
                    }
                    index.Add(data.SetRFLine(rfLine, ref existingNodes, ref existingLines, ref lastNoNo, ref lastLineNo));
                }
            }
        }
        public static void SetRFLines(this IModelData data, List<GH_RFEM> ghLines, ref List<RFLine> index, ref List<string> errorMsg)
        {
            var newData = false;
            var inLines = new List<RFLine>();
            var existingNodes = new List<RFNode>();
            var existingLines = new List<RFLine>();
            var lastNoNo = 0;
            var lastLineNo = 0;

            inLines = ghLines.Select(x => new RFLine((RFLine)x.Value)).ToList();
            foreach (var rfLine in inLines)
            {
                    try
                    {
                        if (rfLine.ToModify)
                {
                    var myLine = (Dlubal.RFEM5.Line)rfLine;
                    var myNo = rfLine.No;
                    data.GetLine(myNo, ItemAt.AtNo).SetData(ref myLine);
                    index.Add(rfLine);
                }
                else if (rfLine.ToDelete)
                {
                    data.GetLine(rfLine.No, ItemAt.AtNo).Delete();
                    index.Add(rfLine);
                }
                else
                {
                    if (newData == false)
                    {
                        existingNodes = Component_GetData.GetRFNodes(data.GetNodes().ToList(), data);
                        existingLines = Component_GetData.GetRFLines(data.GetLines().ToList(), data);
                        lastNoNo = data.GetLastObjectNo(ModelObjectType.NodeObject);
                        lastLineNo = data.GetLastObjectNo(ModelObjectType.LineObject);
                        newData = true;
                    }
                    index.Add(data.SetRFLine(rfLine, ref existingNodes, ref existingLines, ref lastNoNo, ref lastLineNo));
                }
                }
                catch (Exception ex)
                {
                    index.Add(null);
                    errorMsg.Add($"Import of Line No.{rfLine.No} failed! " + ex.Message);
                }
            }
        }
        public static RFLine SetRFLine(this IModelData data, ref RFLine rfLine, ref List<RFNode> existingNodes, ref List<RFLine> existingLines, ref int lastNoNo, ref int lastLineNo)
        {
            // Get NodeList
            var nodeList = "";
            var newNodes = false;
            foreach (var point in rfLine.ControlPoints)
            {
                var prevLastNoNo = lastNoNo;
                var index = data.SetRFNode(new RFNode(new Node(), point), ref existingNodes, ref lastNoNo, tol).No;
                nodeList += index.ToString() + ",";
                if (!(prevLastNoNo - lastNoNo == 0))
                {
                    newNodes = true;
                }
            }
            rfLine.NodeList = nodeList.Substring(0, nodeList.Length - 1);
            // Check if line already exists in RFEM
            if (!newNodes)
            {
                var newNodeSet = new HashSet<int>(rfLine.NodeList.ToInt());
                foreach (var line in existingLines)
                {
                    if (newNodeSet.SetEquals(line.NodeList.ToInt()))
                    {
                        return line;
                    }
                }
            }
            // Set line with a provided index number or without
            if (rfLine.No == 0)
            {
                lastLineNo += 1;
                rfLine.No = lastLineNo;
            }
            if (rfLine.Type == LineType.PolylineType)
            {
                data.SetLine(rfLine);
            }
            else
            {
                // data.SetLine(rfLine);
                data.SetNurbSpline(rfLine);
            }
            existingLines.Add(rfLine);
            return rfLine;
        }
        public static void SetRFMembers(this IModelData data, List<GH_RFEM> ghmembers, ref List<RFMember> index)
        {
            var newData = false;
            var inMembers = new List<RFMember>();
            var existingNodes = new List<RFNode>();
            var existingLines = new List<RFLine>();
            var lastNoNo = 0;
            var lastLineNo = 0;
            var lastMemberNo = 0;

            inMembers = ghmembers.Select(x => new RFMember ((RFMember)x.Value)).ToList();
            foreach (var rfMember in inMembers)
            {
                if (rfMember.ToModify)
                {
                    var myMember = (Member)rfMember;
                    var myNo = rfMember.No;
                    data.GetMember(myNo, ItemAt.AtNo).SetData(ref myMember);
                    index.Add(rfMember);
                }
                else if (rfMember.ToDelete)
                {
                    data.GetMember(rfMember.No, ItemAt.AtNo).Delete();
                    index.Add(rfMember);
                }
                else
                {
                    if (newData == false)
                    {
                        existingNodes = Component_GetData.GetRFNodes(data.GetNodes().ToList(), data);
                        existingLines = Component_GetData.GetRFLines(data.GetLines().ToList(), data);
                        lastNoNo = data.GetLastObjectNo(ModelObjectType.NodeObject);
                        lastLineNo = data.GetLastObjectNo(ModelObjectType.LineObject);
                        lastMemberNo = data.GetLastObjectNo(ModelObjectType.MemberObject);
                        newData = true;
                    }
                    index.Add(data.SetRFMember(rfMember, ref existingNodes, ref existingLines, ref lastNoNo, ref lastLineNo, ref lastMemberNo));
                }
            }
        }
        public static void SetRFMembers(this IModelData data, List<GH_RFEM> ghmembers, ref List<RFMember> index, ref List<string> errorMsg)
        {
            var newData = false;
            var inMembers = new List<RFMember>();
            var existingNodes = new List<RFNode>();
            var existingLines = new List<RFLine>();
            var lastNoNo = 0;
            var lastLineNo = 0;
            var lastMemberNo = 0;

            inMembers = ghmembers.Select(x => new RFMember((RFMember)x.Value)).ToList();
            foreach (var rfMember in inMembers)
            {

                try
                {
                    if (rfMember.ToModify)
                {
                    var myMember = (Member)rfMember;
                    var myNo = rfMember.No;
                    data.GetMember(myNo, ItemAt.AtNo).SetData(ref myMember);
                    if ((rfMember.Kcry != 0) || (rfMember.Kcrz != 0))
                    {
                        var efflengths = new MemberEffectiveLengths();
                        efflengths.FactorY = rfMember.Kcry;
                        efflengths.FactorZ = rfMember.Kcrz;
                        //efflengths.FactorU = 1;
                        //efflengths.FactorV = 1;
                        data.GetMember(rfMember.No, ItemAt.AtNo).SetEffectiveLengths(efflengths);
                    }
                    index.Add(rfMember);
                }
                else if (rfMember.ToDelete)
                {
                    data.GetMember(rfMember.No, ItemAt.AtNo).Delete();
                    index.Add(rfMember);
                }
                else
                {
                    if (newData == false)
                    {
                        existingNodes = Component_GetData.GetRFNodes(data.GetNodes().ToList(), data);
                        existingLines = Component_GetData.GetRFLines(data.GetLines().ToList(), data);
                        lastNoNo = data.GetLastObjectNo(ModelObjectType.NodeObject);
                        lastLineNo = data.GetLastObjectNo(ModelObjectType.LineObject);
                        lastMemberNo = data.GetLastObjectNo(ModelObjectType.MemberObject);
                        newData = true;
                    }
                    index.Add(data.SetRFMember(rfMember, ref existingNodes, ref existingLines, ref lastNoNo, ref lastLineNo, ref lastMemberNo));
                }
                }
                catch (Exception ex)
                {
                    index.Add(null);
                    errorMsg.Add($" Import of Member No.{rfMember.No} failed! " + ex.Message);
                    //throw ex;
                }
            }
        }
        public static RFMember SetRFMember(this IModelData data, ref RFMember rfMember, ref List<RFNode> existingNodes, ref List<RFLine> existingLines, ref int lastNoNo, ref int lastLineNo, ref int lastMemberNo)
        {
            // Draw Line in RFEM
            var myRFLine = rfMember.BaseLine;
            rfMember.LineNo = data.SetRFLine(ref myRFLine, ref existingNodes, ref existingLines, ref lastNoNo, ref lastLineNo).No;
            // Set member with a provided index number or without
            if (rfMember.No == 0)
            {
                lastMemberNo += 1;
                rfMember.No = lastMemberNo;                
            }
            data.SetMember(rfMember);
            if ((rfMember.Kcry !=0) || (rfMember.Kcrz != 0))
            {
                var efflengths = new MemberEffectiveLengths();
                efflengths.FactorY = rfMember.Kcry;
                efflengths.FactorZ = rfMember.Kcrz;
                //efflengths.FactorU = 1;
                //efflengths.FactorV = 1;
                data.GetMember(rfMember.No, ItemAt.AtNo).SetEffectiveLengths(efflengths);
            }                
            return rfMember;
        }
        public static void SetRFSurfaces(this IModelData data, List<GH_RFEM> ghsfcs, ref List<RFSurface> index)
        {
            var newData = false;
            var inSfcs = new List<RFSurface>();
            var existingNodes = new List<RFNode>();
            var existingLines = new List<RFLine>();
            var lastNoNo = 0;
            var lastLineNo = 0;
            var lastSfcNo = 0;
            var lastOpNo = 0;

            inSfcs = ghsfcs.Select(x => new RFSurface((RFSurface)x.Value)).ToList();
            foreach (var rfSfc in inSfcs)
            {
                if (rfSfc.ToModify)
                {
                    var mySfc = (Dlubal.RFEM5.Surface)rfSfc;
                    var myNo = rfSfc.No;
                    //if (rfSfc.NewNo > 0)
                    //{
                    //    mySfc.No = rfSfc.NewNo;
                    //}
                    data.GetSurface(myNo, ItemAt.AtNo).SetData(ref mySfc);
                    index.Add(rfSfc);
                }
                else if (rfSfc.ToDelete)
                {
                    data.GetSurface(rfSfc.No, ItemAt.AtNo).Delete();
                    index.Add(rfSfc);
                }
                else
                {
                    if (newData == false)
                    {
                        existingNodes = Component_GetData.GetRFNodes(data.GetNodes().ToList(), data);
                        existingLines = Component_GetData.GetRFLines(data.GetLines().ToList(), data);
                        lastNoNo = data.GetLastObjectNo(ModelObjectType.NodeObject);
                        lastLineNo = data.GetLastObjectNo(ModelObjectType.LineObject);
                        lastSfcNo = data.GetLastObjectNo(ModelObjectType.SurfaceObject);
                        lastOpNo = data.GetLastObjectNo(ModelObjectType.OpeningObject);
                        newData = true;
                    }
                        index.Add(data.SetRFSfc(rfSfc, ref existingNodes, ref existingLines, ref lastNoNo, ref lastLineNo, ref lastSfcNo));
                    // Add openings
                    if (!(rfSfc.Openings == null))
                    {
                        foreach (var op in rfSfc.Openings)
                        {
                            data.SetRFOpening(op, ref existingNodes, ref existingLines, ref lastNoNo, ref lastLineNo, ref lastOpNo);
                        }
                    }
                }
            }
        }
        public static void SetRFSurfaces(this IModelData data, List<GH_RFEM> ghsfcs, ref List<RFSurface> index, ref List<string> errorMsg)
        {
            var newData = false;
            var inSfcs = new List<RFSurface>();
            var existingNodes = new List<RFNode>();
            var existingLines = new List<RFLine>();
            var lastNoNo = 0;
            var lastLineNo = 0;
            var lastSfcNo = 0;
            var lastOpNo = 0;

            inSfcs = ghsfcs.Select(x => new RFSurface((RFSurface)x.Value)).ToList();
            foreach (var rfSfc in inSfcs)
            {
                try
                {
                        if (rfSfc.ToModify)
                    {
                        var mySfc = (Dlubal.RFEM5.Surface)rfSfc;
                        var myNo = rfSfc.No;
                        //if (rfSfc.NewNo > 0)
                        //{
                        //    mySfc.No = rfSfc.NewNo;
                        //}
                        data.GetSurface(myNo, ItemAt.AtNo).SetData(ref mySfc);
                        index.Add(rfSfc);
                    }
                    else if (rfSfc.ToDelete)
                    {
                        data.GetSurface(rfSfc.No, ItemAt.AtNo).Delete();
                        index.Add(rfSfc);
                    }
                    else
                    {
                        if (newData == false)
                        {
                            existingNodes = Component_GetData.GetRFNodes(data.GetNodes().ToList(), data);
                            existingLines = Component_GetData.GetRFLines(data.GetLines().ToList(), data);
                            lastNoNo = data.GetLastObjectNo(ModelObjectType.NodeObject);
                            lastLineNo = data.GetLastObjectNo(ModelObjectType.LineObject);
                            lastSfcNo = data.GetLastObjectNo(ModelObjectType.SurfaceObject);
                            lastOpNo = data.GetLastObjectNo(ModelObjectType.OpeningObject);
                            newData = true;
                        }
                    
                            index.Add(data.SetRFSfc(rfSfc, ref existingNodes, ref existingLines, ref lastNoNo, ref lastLineNo, ref lastSfcNo));                    
                        // Add openings
                        if (!(rfSfc.Openings == null))
                        {
                            foreach (var op in rfSfc.Openings)
                            {
                                data.SetRFOpening(op, ref existingNodes, ref existingLines, ref lastNoNo, ref lastLineNo, ref lastOpNo);
                            }
                        }                    
                    }
                }
                catch(Exception ex)
                {
                    index.Add(null);
                    errorMsg.Add($"Import of Surface No.{rfSfc.No} failed! " + ex.Message);
                }
            }
        }
        public static RFSurface SetRFSfc(this IModelData data, ref RFSurface rfSfc, ref List<RFNode> existingNodes, ref List<RFLine> existingLines, ref int lastNoNo, ref int lastLineNo, ref int lastSfcNo)
        {
            if (rfSfc.BoundaryLineList == "" || rfSfc.BoundaryLineList == null)
            {
                // Get boundaries
                var boundList = "";
                foreach (var edge in rfSfc.Edges)
                {
                    edge.No = data.SetRFLine(edge, ref existingNodes, ref existingLines, ref lastNoNo, ref lastLineNo).No;
                    boundList += edge.No;
                    boundList += ",";
                }
                rfSfc.BoundaryLineList = boundList.Substring(0, boundList.Length - 1);
                // If NURBS, get index of control points
                if (rfSfc.GeometryType == SurfaceGeometryType.NurbsSurfaceType)
                {
                    rfSfc.Nodes = new int[rfSfc.ControlPoints.GetLength(0), rfSfc.ControlPoints.GetLength(1)];
                    for (int i = 0; i < rfSfc.ControlPoints.GetLength(0); i++)
                    {
                        for (int j = 0; j < rfSfc.ControlPoints.GetLength(1); j++)
                        {
                            var myrfNode = new RFNode(new Node(), rfSfc.ControlPoints[i, j]);
                            var myrfNode2 = data.SetRFNode(myrfNode, ref existingNodes, ref lastNoNo, tol);
                            rfSfc.Nodes[i, j] = myrfNode2.No;
                        }
                    }
                }
            }
            // Set element with a provided index number or without
            if (rfSfc.No == 0)
            {
                lastSfcNo += 1;
                rfSfc.No = lastSfcNo;
            }
            if (rfSfc.GeometryType == SurfaceGeometryType.NurbsSurfaceType)
            {
                // data.SetNurbsSurface(rfSfc);
                data.SetSurface(rfSfc);
            }else if (rfSfc.GeometryType == SurfaceGeometryType.QuadrangleSurfaceType)
            {
                // If surface has 3 boundary lines, a fourth null line has to be created
                if (rfSfc.Edges.Length == 3)
                {
                    // In case of triangles, we need an auxiliary null line to set the quad
                    // Null line exists?
                    var myboundarylinelist = "";
                    if (UtilLibrary.NullLineExists(data.GetLines(), rfSfc.Edges, ref myboundarylinelist))
                    {
                        rfSfc.BoundaryLineList = myboundarylinelist;
                        data.SetSurface(rfSfc);
                    }
                    else
                    {
                        // Define a quad with to other auxiliary lines and then move back the new vertex to form the triangle
                        // We will replace Edges[0] -> therefore the aux Lines begin in its end points
                        var nodeListStart = rfSfc.Edges[0].NodeList.ToInt();
                        var curve0 = rfSfc.Edges[0].ToCurve();
                        var startPoint1 = curve0.PointAtStart;
                        var startPoint2 = curve0.PointAtEnd;
                        // Create aux node in a location where no other node is expected:
                        var auxPoint = UtilLibrary.CreateAuxiliaryPoint(rfSfc.Edges[1].ToCurve(), rfSfc.Edges[2].ToCurve(), 1000);
                        var auxRFnode = new RFNode(new Node(), auxPoint);
                        var auxIndex = data.SetRFNode(ref auxRFnode, ref existingNodes, ref lastNoNo, tol).No;
                        // Create aux lineS
                        var auxLine1 = new LineCurve(startPoint1, auxPoint);
                        var auxLine2 = new LineCurve(startPoint2, auxPoint);
                        // Create Brep
                        var auxBoundaries = new List<Curve>() { rfSfc.Edges[1].ToCurve(), rfSfc.Edges[2].ToCurve(), auxLine1, auxLine2};
                        var auxBrep = UtilLibrary.CreateNonPlanarBrep(auxBoundaries,1);
                        // Create auxiliary RFSurface
                        var auxRFsfc = new RFSurface(rfSfc);
                        Component_RFSurface.SetGeometry(auxBrep, ref auxRFsfc);
                        data.SetRFSfc(ref auxRFsfc, ref existingNodes, ref existingLines, ref lastNoNo, ref lastLineNo, ref lastSfcNo);
                        // Move auxNode back to the other line end point to create the null line
                        data.GetNode(auxIndex, ItemAt.AtNo).SetData(data.GetNode(nodeListStart[0], ItemAt.AtNo).GetData());
                        // In case of NURBS, replace auxiliary line with the original
                        data.FinishModification();
                        data.PrepareModification();
                        if (data.GetLines().Select(x => x.No).Contains(auxRFsfc.Edges[3].No))
                        {
                            // -> Verificar si al mover linea y se elimina, existing lines siguen estando bien!!!
                            var oldLineList = auxRFsfc.BoundaryLineList.ToInt();
                            oldLineList[oldLineList.FindIndex(ind => ind.Equals(auxRFsfc.Edges[3].No))] = rfSfc.Edges[0].No;
                            var newlinelist = string.Join(",", oldLineList);
                            auxRFsfc.BoundaryLineList = newlinelist;
                            data.GetSurface(auxRFsfc.No, ItemAt.AtNo).SetData(auxRFsfc);
                            data.GetLine(auxRFsfc.Edges[3].No, ItemAt.AtNo).Delete();
                            rfSfc = auxRFsfc;
                            data.FinishModification();
                            data.PrepareModification();
                        }
                        // Delete from existing lines and nodes
                        existingLines.Remove(auxRFsfc.Edges[2]);
                        existingLines.Remove(auxRFsfc.Edges[3]);
                        existingNodes.Remove(auxRFnode);
                        lastLineNo -= 1;
                        lastNoNo -= 1;
                    }
                }
                else
                {
                    data.SetSurface(rfSfc);
                }
            }else // neither NURBS nor Quadrangle
            {
                data.SetSurface(rfSfc);
            }                
            return rfSfc;
        }

        public static void SetRFOpenings(this IModelData data, List<GH_RFEM> ghsfcs, ref List<RFOpening> index)
        {
            var newData = false;
            var inOps = new List<RFOpening>();
            var existingNodes = new List<RFNode>();
            var existingLines = new List<RFLine>();
            var lastNoNo = 0;
            var lastLineNo = 0;
            var lastOpNo = 0;

            inOps = ghsfcs.Select(x => new RFOpening((RFOpening)x.Value)).ToList();
            foreach (var rfOp in inOps)
            {
                if (rfOp.ToModify)
                {
                    var myOp = (Opening)rfOp;
                    var myNo = rfOp.No;
                    //if (rfOp.NewNo > 0)
                    //{
                    //    myOp.No = rfOp.NewNo;
                    //}
                    data.GetOpening(myNo, ItemAt.AtNo).SetData(ref myOp);
                    index.Add(rfOp);
                }
                else if (rfOp.ToDelete)
                {
                    data.GetOpening(rfOp.No, ItemAt.AtNo).Delete();
                    index.Add(rfOp);
                }
                else
                {
                    if (newData == false)
                    {
                        existingNodes = Component_GetData.GetRFNodes(data.GetNodes().ToList(), data);
                        existingLines = Component_GetData.GetRFLines(data.GetLines().ToList(), data);
                        lastNoNo = data.GetLastObjectNo(ModelObjectType.NodeObject);
                        lastLineNo = data.GetLastObjectNo(ModelObjectType.LineObject);
                        lastOpNo = data.GetLastObjectNo(ModelObjectType.OpeningObject);
                        newData = true;
                    }
                    index.Add(data.SetRFOpening(rfOp, ref existingNodes, ref existingLines, ref lastNoNo, ref lastLineNo, ref lastOpNo));
                }
            }
        }
        public static void SetRFOpenings(this IModelData data, List<GH_RFEM> ghsfcs, ref List<RFOpening> index, ref List<string> errorMsg)
        {
            var newData = false;
            var inOps = new List<RFOpening>();
            var existingNodes = new List<RFNode>();
            var existingLines = new List<RFLine>();
            var lastNoNo = 0;
            var lastLineNo = 0;
            var lastOpNo = 0;

            inOps = ghsfcs.Select(x => new RFOpening((RFOpening)x.Value)).ToList();
            foreach (var rfOp in inOps)
            {
                            try
                            {
                                if (rfOp.ToModify)
                {
                    var myOp = (Opening)rfOp;
                    var myNo = rfOp.No;
                    //if (rfOp.NewNo > 0)
                    //{
                    //    myOp.No = rfOp.NewNo;
                    //}
                    data.GetOpening(myNo, ItemAt.AtNo).SetData(ref myOp);
                    index.Add(rfOp);
                }
                else if (rfOp.ToDelete)
                {
                    data.GetOpening(rfOp.No, ItemAt.AtNo).Delete();
                    index.Add(rfOp);
                }
                else
                {
                    if (newData == false)
                    {
                        existingNodes = Component_GetData.GetRFNodes(data.GetNodes().ToList(), data);
                        existingLines = Component_GetData.GetRFLines(data.GetLines().ToList(), data);
                        lastNoNo = data.GetLastObjectNo(ModelObjectType.NodeObject);
                        lastLineNo = data.GetLastObjectNo(ModelObjectType.LineObject);
                        lastOpNo = data.GetLastObjectNo(ModelObjectType.OpeningObject);
                        newData = true;
                    }
                    index.Add(data.SetRFOpening(rfOp, ref existingNodes, ref existingLines, ref lastNoNo, ref lastLineNo, ref lastOpNo));
                }
                }
                catch(Exception ex)
                {
                    index.Add(null);
                    errorMsg.Add($"Import of Opening No.{rfOp.No} failed! " + ex.Message);
                }
            }
        }

        public static RFOpening SetRFOpening(this IModelData data, ref RFOpening rfOpening, ref List<RFNode> existingNodes, ref List<RFLine> existingLines, ref int lastNoNo, ref int lastLineNo, ref int lastOpNo)
        {
            // Get boundaries
            var boundList = "";
            foreach (var edge in rfOpening.Edges)
            {
                boundList += data.SetRFLine(edge, ref existingNodes, ref existingLines, ref lastNoNo, ref lastLineNo).No;
                boundList += ",";
            }
            rfOpening.BoundaryLineList = boundList.Substring(0, boundList.Length - 1);
            // Set member with a provided index number or without
            if (rfOpening.No == 0)
            {
                lastOpNo += 1;
                rfOpening.No = lastOpNo;
            }
            data.SetOpening(rfOpening);
            return rfOpening;
        }

        public static void SetRFSupportsP(this IModelData data, List<GH_RFEM> ghNodes, ref List<RFSupportP> index)
        {
            var newData = false;
            var inSups = new List<RFSupportP>();
            var existingNodes = new List<RFNode>();
            var lastNoNo = 0;
            var lastSupNo = 0;

            inSups = ghNodes.Select(x => new RFSupportP((RFSupportP)x.Value)).ToList();
            foreach (var rfSup in inSups)
            {
                if (rfSup.ToModify)
                {
                    var mySup = (NodalSupport)rfSup;
                    var myNo = mySup.No;
                    //if (rfSup.NewNo > 0)
                    //{
                    //    myNo = rfSup.NewNo;
                    //}
                    data.GetNodalSupport(myNo, ItemAt.AtNo).SetData(ref mySup);
                    index.Add(rfSup);
                }
                else if (rfSup.ToDelete)
                {
                    data.GetNodalSupport(rfSup.No, ItemAt.AtNo).Delete();
                    index.Add(rfSup);
                }
                else
                {
                    if (newData == false)
                    {
                        existingNodes = Component_GetData.GetRFNodes(data.GetNodes().ToList(), data);
                        lastNoNo = data.GetLastObjectNo(ModelObjectType.NodeObject);
                        lastSupNo = data.GetLastObjectNo(ModelObjectType.NodalSupportObject);
                        newData = true;
                    }
                    //var myNode = (Node)rfNode;
                    index.Add(data.SetRFSupportP(rfSup, ref existingNodes, ref lastNoNo, ref lastSupNo, tol));
                }
            }
        }
        public static void SetRFSupportsP(this IModelData data, List<GH_RFEM> ghNodes, ref List<RFSupportP> index, ref List<string> errorMsg)
        {
            var newData = false;
            var inSups = new List<RFSupportP>();
            var existingNodes = new List<RFNode>();
            var lastNoNo = 0;
            var lastSupNo = 0;

            inSups = ghNodes.Select(x => new RFSupportP((RFSupportP)x.Value)).ToList();
            foreach (var rfSup in inSups)
            {
                                try
                                {
                                    if (rfSup.ToModify)
                {
                    var mySup = (NodalSupport)rfSup;
                    var myNo = mySup.No;
                    //if (rfSup.NewNo > 0)
                    //{
                    //    myNo = rfSup.NewNo;
                    //}
                    data.GetNodalSupport(myNo, ItemAt.AtNo).SetData(ref mySup);
                    index.Add(rfSup);
                }
                else if (rfSup.ToDelete)
                {
                    data.GetNodalSupport(rfSup.No, ItemAt.AtNo).Delete();
                    index.Add(rfSup);
                }
                else
                {
                    if (newData == false)
                    {
                        existingNodes = Component_GetData.GetRFNodes(data.GetNodes().ToList(), data);
                        lastNoNo = data.GetLastObjectNo(ModelObjectType.NodeObject);
                        lastSupNo = data.GetLastObjectNo(ModelObjectType.NodalSupportObject);
                        newData = true;
                    }
                    //var myNode = (Node)rfNode;
                    index.Add(data.SetRFSupportP(rfSup, ref existingNodes, ref lastNoNo, ref lastSupNo, tol));
                }
                }
                catch(Exception ex)
                {
                    index.Add(null);
                    errorMsg.Add($"Import of Nodal Support No.{rfSup.No} failed! " + ex.Message);
                }
            }
        }
        public static RFSupportP SetRFSupportP(this IModelData data, ref RFSupportP rfSupP, ref List<RFNode> existingNodes, ref int lastNo, ref int lastSupNo, double tol)
        {

            // Get Node List?
            if (rfSupP.NodeList == null)
            {
                var nodeList = "";
                foreach (var plane in rfSupP.Orientation)
                {
                    var rfNode = new RFNode(new Node(), plane.Origin);
                    nodeList += data.SetRFNode(ref rfNode, ref existingNodes, ref lastNo, tol).No.ToString() + ",";
                }
                rfSupP.NodeList = nodeList.Substring(0, nodeList.Length - 1);
            }
            // Set support with a provided index number
            if (!(rfSupP.No == 0))
            {
                data.SetNodalSupport(rfSupP);
                return rfSupP;
            }
            // Set node without provided index number
            lastSupNo += 1;
            rfSupP.No = lastSupNo;
            data.SetNodalSupport(rfSupP);
            return rfSupP;
        }
        public static void SetRFSupportsL(this IModelData data, List<GH_RFEM> ghLines, ref List<RFSupportL> index)
        {
            var newData = false;
            var inSups = new List<RFSupportL>();
            var existingNodes = new List<RFNode>();
            var existingLines = new List<RFLine>();
            var lastNoNo = 0;
            var lastLineNo = 0;
            var lastSupNo = 0;

            inSups = ghLines.Select(x => new RFSupportL((RFSupportL)x.Value)).ToList();
            foreach (var rfSup in inSups)
            {
                if (rfSup.ToModify)
                {
                    var mySup = (LineSupport)rfSup;
                    var myNo = mySup.No;
                    //if (rfSup.NewNo > 0)
                    //{
                    //    myNo = rfSup.NewNo;
                    //}
                    data.GetLineSupport(myNo, ItemAt.AtNo).SetData(ref mySup);
                    index.Add(rfSup);
                }
                else if (rfSup.ToDelete)
                {
                    data.GetLineSupport(rfSup.No, ItemAt.AtNo).Delete();
                    index.Add(rfSup);
                }
                else
                {
                    if (newData == false)
                    {
                        existingNodes = Component_GetData.GetRFNodes(data.GetNodes().ToList(), data);
                        existingLines = Component_GetData.GetRFLines(data.GetLines().ToList(), data);
                        lastNoNo = data.GetLastObjectNo(ModelObjectType.NodeObject);
                        lastLineNo = data.GetLastObjectNo(ModelObjectType.LineObject);
                        lastSupNo = data.GetLastObjectNo(ModelObjectType.LineSupportObject);
                        newData = true;
                    }
                    //var myNode = (Node)rfNode;
                    index.Add(data.SetRFSupportL(rfSup, ref existingNodes, ref existingLines, ref lastNoNo, ref lastLineNo, ref lastSupNo));
                }
            }
        }
        public static void SetRFSupportsL(this IModelData data, List<GH_RFEM> ghLines, ref List<RFSupportL> index, ref List<string> errorMsg)
        {
            var newData = false;
            var inSups = new List<RFSupportL>();
            var existingNodes = new List<RFNode>();
            var existingLines = new List<RFLine>();
            var lastNoNo = 0;
            var lastLineNo = 0;
            var lastSupNo = 0;

            inSups = ghLines.Select(x => new RFSupportL((RFSupportL)x.Value)).ToList();
            foreach (var rfSup in inSups)
            {
                                    try
                                    {
                                        if (rfSup.ToModify)
                {
                    var mySup = (LineSupport)rfSup;
                    var myNo = mySup.No;
                    //if (rfSup.NewNo > 0)
                    //{
                    //    myNo = rfSup.NewNo;
                    //}
                    data.GetLineSupport(myNo, ItemAt.AtNo).SetData(ref mySup);
                    index.Add(rfSup);
                }
                else if (rfSup.ToDelete)
                {
                    data.GetLineSupport(rfSup.No, ItemAt.AtNo).Delete();
                    index.Add(rfSup);
                }
                else
                {
                    if (newData == false)
                    {
                        existingNodes = Component_GetData.GetRFNodes(data.GetNodes().ToList(), data);
                        existingLines = Component_GetData.GetRFLines(data.GetLines().ToList(), data);
                        lastNoNo = data.GetLastObjectNo(ModelObjectType.NodeObject);
                        lastLineNo = data.GetLastObjectNo(ModelObjectType.LineObject);
                        lastSupNo = data.GetLastObjectNo(ModelObjectType.LineSupportObject);
                        newData = true;
                    }
                    //var myNode = (Node)rfNode;
                    index.Add(data.SetRFSupportL(rfSup, ref existingNodes, ref existingLines, ref lastNoNo, ref lastLineNo, ref lastSupNo));
                }
                }
                catch(Exception ex)
                {
                    index.Add(null);
                    errorMsg.Add($"Import of Line Support No.{rfSup.No} failed! " + ex.Message);
                }
            }
        }
        public static RFSupportL SetRFSupportL(this IModelData data, ref RFSupportL rfSupL, ref List<RFNode> existingNodes, ref List<RFLine> existingLines, ref int lastNoNo, ref int lastLineNo, ref int lastSupNo)
        {
            // Get Line List?
            if (rfSupL.LineList == null)
            {
                var lineList = "";
                foreach (var rfLine in rfSupL.BaseLines)
                {
                    var myRFLine = new RFLine(rfLine);
                    lineList += data.SetRFLine(ref myRFLine, ref existingNodes, ref existingLines, ref lastNoNo, ref lastLineNo).No.ToString() + ",";
                }
                rfSupL.LineList = lineList.Substring(0, lineList.Length - 1);
            }
            // Set support with a provided index number
            if (!(rfSupL.No == 0))
            {
                data.SetLineSupport(rfSupL);
                return rfSupL;
            }
            // Set node without provided index number
            lastSupNo += 1;
            rfSupL.No = lastSupNo;
            data.SetLineSupport(rfSupL);
            return rfSupL;
        }

        public static void SetRFSupportsS(this IModelData data, List<GH_RFEM> ghLines, ref List<RFSupportS> index, ref List<string> errorMsg)
        {
            var inSups = new List<RFSupportS>();
            var existingNodes = new List<RFNode>();
            var existingLines = new List<RFLine>();
            var lastSupNo = 0;

            inSups = ghLines.Select(x => new RFSupportS((RFSupportS)x.Value)).ToList();
            foreach (var rfSup in inSups)
            {
                try
                {
                    if (rfSup.ToModify)
                    {
                        var mySup = (SurfaceSupport)rfSup;
                        var myNo = mySup.No;
                        data.GetSurfaceSupport(myNo, ItemAt.AtNo).SetData(ref mySup);
                        index.Add(rfSup);
                    }
                    else if (rfSup.ToDelete)
                    {
                        data.GetSurfaceSupport(rfSup.No, ItemAt.AtNo).Delete();
                        index.Add(rfSup);
                    }
                    else
                    {
                        lastSupNo = data.GetLastObjectNo(ModelObjectType.SurfaceSupportObject);
                        index.Add(data.SetRFSupportS(rfSup, ref lastSupNo));
                    }
                }
                catch (Exception ex)
                {
                    index.Add(null);
                    errorMsg.Add($"Import of Line Support No.{rfSup.No} failed! " + ex.Message);
                }
            }
        }
        public static RFSupportS SetRFSupportS(this IModelData data, ref RFSupportS rfSupS, ref int lastSupNo)
        {
            // Get Line List? -> no
            // Set support with a provided index number
            if (!(rfSupS.No == 0))
            {
                data.SetSurfaceSupport(rfSupS);
                return rfSupS;
            }
            // Set node without provided index number
            lastSupNo += 1;
            rfSupS.No = lastSupNo;
            data.SetSurfaceSupport(rfSupS);
            return rfSupS;
        }

        public static void SetRFLineHinges(this IModelData data, List<GH_RFEM> ghLines, ref List<RFLineHinge> index)
        {
            var newData = false;
            var inHinges = new List<RFLineHinge>();
            var existingNodes = new List<RFNode>();
            var existingLines = new List<RFLine>();
            var lastNoNo = 0;
            var lastLineNo = 0;
            var lastHingeNo = 0;

            inHinges = ghLines.Select(x => new RFLineHinge((RFLineHinge)x.Value)).ToList();
            foreach (var rfHinge in inHinges)
            {
                if (rfHinge.ToModify)
                {
                    var myHinge = (LineHinge)rfHinge;
                    var myNo = myHinge.No;
                    //if (rfSup.NewNo > 0)
                    //{
                    //    myNo = rfSup.NewNo;
                    //}
                    data.GetLineHinge(myNo, ItemAt.AtNo).SetData(ref myHinge);
                    index.Add(rfHinge);
                }
                else if (rfHinge.ToDelete)
                {
                    data.GetLineHinge(rfHinge.No, ItemAt.AtNo).Delete();
                    index.Add(rfHinge);
                }
                else
                {
                    if (newData == false)
                    {
                        existingNodes = Component_GetData.GetRFNodes(data.GetNodes().ToList(), data);
                        existingLines = Component_GetData.GetRFLines(data.GetLines().ToList(), data);
                        lastNoNo = data.GetLastObjectNo(ModelObjectType.NodeObject);
                        lastLineNo = data.GetLastObjectNo(ModelObjectType.LineObject);
                        lastHingeNo = data.GetLastObjectNo(ModelObjectType.LineHingeObject);
                        newData = true;
                    }
                    index.Add(data.SetRFLineHinge(rfHinge, ref existingNodes, ref existingLines, ref lastNoNo, ref lastLineNo, ref lastHingeNo));
                }
            }
        }
        public static void SetRFLineHinges(this IModelData data, List<GH_RFEM> ghLines, ref List<RFLineHinge> index, ref List<string> errorMsg)
        {
            var newData = false;
            var inHinges = new List<RFLineHinge>();
            var existingNodes = new List<RFNode>();
            var existingLines = new List<RFLine>();
            var lastNoNo = 0;
            var lastLineNo = 0;
            var lastHingeNo = 0;

            inHinges = ghLines.Select(x => new RFLineHinge((RFLineHinge)x.Value)).ToList();
            foreach (var rfHinge in inHinges)
            {
                                        try
                                        {
                                            if (rfHinge.ToModify)
                {
                    var myHinge = (LineHinge)rfHinge;
                    var myNo = myHinge.No;
                    //if (rfSup.NewNo > 0)
                    //{
                    //    myNo = rfSup.NewNo;
                    //}
                    data.GetLineHinge(myNo, ItemAt.AtNo).SetData(ref myHinge);
                    index.Add(rfHinge);
                }
                else if (rfHinge.ToDelete)
                {
                    data.GetLineHinge(rfHinge.No, ItemAt.AtNo).Delete();
                    index.Add(rfHinge);
                }
                else
                {
                    if (newData == false)
                    {
                        existingNodes = Component_GetData.GetRFNodes(data.GetNodes().ToList(), data);
                        existingLines = Component_GetData.GetRFLines(data.GetLines().ToList(), data);
                        lastNoNo = data.GetLastObjectNo(ModelObjectType.NodeObject);
                        lastLineNo = data.GetLastObjectNo(ModelObjectType.LineObject);
                        lastHingeNo = data.GetLastObjectNo(ModelObjectType.LineHingeObject);
                        newData = true;
                    }
                    index.Add(data.SetRFLineHinge(rfHinge, ref existingNodes, ref existingLines, ref lastNoNo, ref lastLineNo, ref lastHingeNo));
                }
                }
                catch(Exception ex)
                {
                    index.Add(null);
                    errorMsg.Add($"Import of Line Hinge No.{rfHinge.No} failed! " + ex.Message);
                }
            }
        }

        public static RFLineHinge SetRFLineHinge(this IModelData data, ref RFLineHinge rfLH, ref List<RFNode> existingNodes, ref List<RFLine> existingLines, ref int lastNoNo, ref int lastLineNo, ref int lastLHNo)
        {
            // Get Line List?
            if (rfLH.LineNo == 0)
            {
                var myRFLine = rfLH.BaseLine;
                rfLH.LineNo = data.SetRFLine(ref myRFLine, ref existingNodes, ref existingLines, ref lastNoNo, ref lastLineNo).No;
            }
            // Set support with a provided index number
            if (!(rfLH.No == 0))
            {
                data.SetLineHinge(rfLH);
                return rfLH;
            }
            // Set node without provided index number
            lastLHNo += 1;
            rfLH.No = lastLHNo;
            data.SetLineHinge(rfLH);
            return rfLH;
        }

        public static void SetRFCroSecs(this IModelData data, List<GH_RFEM> ghCSecs, ref List<RFCroSec> index)
        {
            var newData = false;
            var inCroSecs = new List<RFCroSec>();
            var lastNoNo = 0;

            inCroSecs = ghCSecs.Select(x => new RFCroSec((RFCroSec)x.Value)).ToList();
            foreach (var rfCS in inCroSecs)
            {
                if (rfCS.ToModify)
                {
                    var myCS = (CrossSection)rfCS;
                    var myNo = rfCS.No;
                    //if (rfNode.NewNo>0)
                    //{
                    //    myNode.No = rfNode.NewNo;
                    //}
                    data.GetCrossSection(myNo, ItemAt.AtNo).SetData(ref myCS);
                    index.Add(rfCS);
                }
                else if (rfCS.ToDelete)
                {
                    data.GetCrossSection(rfCS.No, ItemAt.AtNo).Delete();
                    index.Add(rfCS);
                }
                else
                {
                    if (newData == false)
                    {
                        lastNoNo = data.GetLastObjectNo(ModelObjectType.CrossSectionObject);
                        newData = true;
                    }
                    //var myNode = (Node)rfNode;
                    index.Add(data.SetRFCroSec(rfCS, ref lastNoNo));
                }
            }
        }

        public static void SetRFCroSecs(this IModelData data, List<GH_RFEM> ghCSecs, ref List<RFCroSec> index, ref List<string> errorMsg)
        {
            var newData = false;
            var inCroSecs = new List<RFCroSec>();
            var lastNoNo = 0;

            inCroSecs = ghCSecs.Select(x => new RFCroSec((RFCroSec)x.Value)).ToList();
            foreach (var rfCS in inCroSecs)
            {
                                            try
                                            {
                                                if (rfCS.ToModify)
                {
                    var myCS = (CrossSection)rfCS;
                    var myNo = rfCS.No;
                    //if (rfNode.NewNo>0)
                    //{
                    //    myNode.No = rfNode.NewNo;
                    //}
                    data.GetCrossSection(myNo, ItemAt.AtNo).SetData(ref myCS);
                    index.Add(rfCS);
                }
                else if (rfCS.ToDelete)
                {
                    data.GetCrossSection(rfCS.No, ItemAt.AtNo).Delete();
                    index.Add(rfCS);
                }
                else
                {
                    if (newData == false)
                    {
                        lastNoNo = data.GetLastObjectNo(ModelObjectType.CrossSectionObject);
                        newData = true;
                    }
                    //var myNode = (Node)rfNode;
                    index.Add(data.SetRFCroSec(rfCS, ref lastNoNo));
                }
                }
                catch(Exception ex)
                {
                    index.Add(null);
                    errorMsg.Add($"Import of Cross Section No.{rfCS.No} failed! " + ex.Message);
                }
            }
        }

        public static RFCroSec SetRFCroSec(this IModelData data, ref RFCroSec rFCroSec, ref int lastNo)
        {
            // Set CS with a provided index number
            if (!(rFCroSec.No == 0))
            {
                data.SetCrossSection(rFCroSec);
                return rFCroSec;
            }
            // Set node without provided index number
            lastNo += 1;
            rFCroSec.No = lastNo;
            data.SetCrossSection(rFCroSec);
            return rFCroSec;
        }

        public static void SetRFMaterials(this IModelData data, List<GH_RFEM> ghMats, ref List<RFMaterial> index)
        {
            var newData = false;
            var inMaterials = new List<RFMaterial>();
            var lastNoNo = 0;

            inMaterials = ghMats.Select(x => new RFMaterial((RFMaterial)x.Value)).ToList();
            foreach (var rfMat in inMaterials)
            {
                if (rfMat.ToModify)
                {
                    var myMat = (Material)rfMat;
                    data.GetMaterial(rfMat.No, ItemAt.AtNo).SetData(ref myMat);
                    index.Add(rfMat);
                }
                else if (rfMat.ToDelete)
                {
                    data.GetMaterial(rfMat.No, ItemAt.AtNo).Delete();
                    index.Add(rfMat);
                }
                else
                {
                    if (newData == false)
                    {
                        lastNoNo = data.GetLastObjectNo(ModelObjectType.MaterialObject);
                        newData = true;
                    }
                    //var myNode = (Node)rfNode;
                    index.Add(data.SetMaterial(rfMat, ref lastNoNo));
                }
            }
        }

        public static void SetRFMaterials(this IModelData data, List<GH_RFEM> ghMats, ref List<RFMaterial> index, ref List<string> errorMsg)
        {
            var newData = false;
            var inMaterials = new List<RFMaterial>();
            var lastNoNo = 0;

            inMaterials = ghMats.Select(x => new RFMaterial((RFMaterial)x.Value)).ToList();
            foreach (var rfMat in inMaterials)
            {
                                                try
                                                {
                                                    if (rfMat.ToModify)
                {
                    var myMat = (Material)rfMat;
                    data.GetMaterial(rfMat.No, ItemAt.AtNo).SetData(ref myMat);
                    index.Add(rfMat);
                }
                else if (rfMat.ToDelete)
                {
                    data.GetMaterial(rfMat.No, ItemAt.AtNo).Delete();
                    index.Add(rfMat);
                }
                else
                {
                    if (newData == false)
                    {
                        lastNoNo = data.GetLastObjectNo(ModelObjectType.MaterialObject);
                        newData = true;
                    }
                    //var myNode = (Node)rfNode;
                    index.Add(data.SetMaterial(rfMat, ref lastNoNo));
                }
                }
                catch(Exception ex)
                {
                    index.Add(null);
                    errorMsg.Add($"Import of Material No.{rfMat.No} failed! " + ex.Message);
                }
            }
        }

        public static RFMaterial SetMaterial(this IModelData data, ref RFMaterial rFMat, ref int lastNo)
        {
            // Set CS with a provided index number
            if (!(rFMat.No == 0))
            {
                data.SetMaterial(rFMat);
                return rFMat;
            }
            // Set node without provided index number
            lastNo += 1;
            rFMat.No = lastNo;
            data.SetMaterial(rFMat);
            return rFMat;
        }

        public static void SetRFNodalLoads(this IModelData data, ILoads loads, List<GH_RFEM> ghNodes, ref List<RFNodalLoad> index)
        {
            var newData = false;
            var inNLoads = new List<RFNodalLoad>();
            var existingNodes = new List<RFNode>();
            var lastNoNo = 0;
            var lastNLNo = 0;

            inNLoads = ghNodes.Select(x => new RFNodalLoad((RFNodalLoad)x.Value)).ToList();
            foreach (var rfLoad in inNLoads)
            {
                var loadcase = loads.GetLoadCase(rfLoad.LoadCase, ItemAt.AtNo);
                if (rfLoad.ToModify)
                {
                    var myload = (NodalLoad)rfLoad;
                    //if (rfSup.NewNo > 0)
                    //{
                    //    myNo = rfSup.NewNo;
                    //}
                    loadcase.GetNodalLoad(rfLoad.No, ItemAt.AtNo).SetData(ref myload);
                    index.Add(rfLoad);
                }
                else if (rfLoad.ToDelete)
                {
                    loadcase.GetNodalLoad(rfLoad.No, ItemAt.AtNo).Delete();
                    index.Add(rfLoad);
                }
                else
                {
                    if (newData == false)
                    {
                        existingNodes = Component_GetData.GetRFNodes(data.GetNodes().ToList(), data);
                        lastNoNo = data.GetLastObjectNo(ModelObjectType.NodeObject);
                        lastNLNo = loadcase.GetLastObjectNo(LoadObjectType.NodalLoadObject);
                        newData = true;
                    }
                    index.Add(data.SetRFNodalLoad(loadcase, rfLoad, ref existingNodes, ref lastNoNo, ref lastNLNo, tol));
                }
            }
        }

        public static void SetRFNodalLoads(this IModelData data, ILoads loads, List<GH_RFEM> ghNodes, ref List<RFNodalLoad> index, ref List<string> errorMsg)
        {
            var newData = false;
            var inNLoads = new List<RFNodalLoad>();
            var existingNodes = new List<RFNode>();
            var lastNoNo = 0;
            var lastNLNo = 0;

            inNLoads = ghNodes.Select(x => new RFNodalLoad((RFNodalLoad)x.Value)).ToList();
            foreach (var rfLoad in inNLoads)
            {
                                                    try
                                                    {
                                                        var loadcase = loads.GetLoadCase(rfLoad.LoadCase, ItemAt.AtNo);
                if (rfLoad.ToModify)
                {
                    var myload = (NodalLoad)rfLoad;
                    //if (rfSup.NewNo > 0)
                    //{
                    //    myNo = rfSup.NewNo;
                    //}
                    loadcase.GetNodalLoad(rfLoad.No, ItemAt.AtNo).SetData(ref myload);
                    index.Add(rfLoad);
                }
                else if (rfLoad.ToDelete)
                {
                    loadcase.GetNodalLoad(rfLoad.No, ItemAt.AtNo).Delete();
                    index.Add(rfLoad);
                }
                else
                {
                    if (newData == false)
                    {
                        existingNodes = Component_GetData.GetRFNodes(data.GetNodes().ToList(), data);
                        lastNoNo = data.GetLastObjectNo(ModelObjectType.NodeObject);
                        lastNLNo = loadcase.GetLastObjectNo(LoadObjectType.NodalLoadObject);
                        newData = true;
                    }
                    index.Add(data.SetRFNodalLoad(loadcase, rfLoad, ref existingNodes, ref lastNoNo, ref lastNLNo, tol));
                }
                }
                catch(Exception ex)
                {
                    index.Add(null);
                    errorMsg.Add($"Import of Nodal Load No.{rfLoad.No} failed! " + ex.Message);
                }
            }
        }

        public static RFNodalLoad SetRFNodalLoad(this IModelData data, ILoadCase loadcase, ref RFNodalLoad rfNodalLoad, ref List<RFNode> existingNodes, ref int lastNo, ref int lastNLNo, double tol)
        {
            // Get Node List?
            if (rfNodalLoad.NodeList == null)
            {
                var nodeList = "";
                data.PrepareModification();
                foreach (var pt in rfNodalLoad.Location)
                {
                    var rfNode = new RFNode(new Node(), pt);
                    nodeList += data.SetRFNode(ref rfNode, ref existingNodes, ref lastNo, tol).No.ToString() + ",";
                }
                data.FinishModification();
                rfNodalLoad.NodeList = nodeList.Substring(0, nodeList.Length - 1);
            }
            // Set support with a provided index number
            if (!(rfNodalLoad.No == 0))
            {
                loadcase.PrepareModification();
                loadcase.SetNodalLoad(rfNodalLoad);
                loadcase.FinishModification();
                return rfNodalLoad;
            }
            // Set node without provided index number
            lastNLNo += 1;
            rfNodalLoad.No = lastNLNo;
            loadcase.PrepareModification();
            loadcase.SetNodalLoad(rfNodalLoad);
            loadcase.FinishModification();
            return rfNodalLoad;
        }


        public static void SetRFLineLoads(this IModelData data, ILoads loads, List<GH_RFEM> ghLLoads, ref List<RFLineLoad> index, ref List<string> errorMsg)
        {
            var newData = false;
            var inLLoads = new List<RFLineLoad>();
            var existingNodes = new List<RFNode>();
            var existingLines = new List<RFLine>();
            var lastNoNo = 0;
            var lastLineNo = 0;
            var lastLLNo = 0;

            inLLoads = ghLLoads.Select(x => new RFLineLoad((RFLineLoad)x.Value)).ToList();
            foreach (var rfLoad in inLLoads)
            {
                                                        try
                                                        {
                                                            var loadcase = loads.GetLoadCase(rfLoad.LoadCase, ItemAt.AtNo);
                if (rfLoad.ToModify)
                {
                    var myload = (LineLoad)rfLoad;
                    //if (rfSup.NewNo > 0)
                    //{
                    //    myNo = rfSup.NewNo;
                    //}
                    loadcase.GetLineLoad(rfLoad.No, ItemAt.AtNo).SetData(ref myload);
                    index.Add(rfLoad);
                }
                else if (rfLoad.ToDelete)
                {
                    loadcase.GetLineLoad(rfLoad.No, ItemAt.AtNo).Delete();
                    index.Add(rfLoad);
                }
                else
                {
                    if (newData == false)
                    {
                        existingNodes = Component_GetData.GetRFNodes(data.GetNodes().ToList(), data);
                        existingLines = Component_GetData.GetRFLines(data.GetLines().ToList(), data);
                        lastNoNo = data.GetLastObjectNo(ModelObjectType.NodeObject);
                        lastLineNo = data.GetLastObjectNo(ModelObjectType.LineObject);
                        lastLLNo = loadcase.GetLastObjectNo(LoadObjectType.LineLoadObject);
                        newData = true;
                    }
                    index.Add(data.SetRFLineLoad(loadcase, rfLoad, ref existingNodes, ref existingLines, ref lastNoNo, ref lastLineNo, ref lastLLNo, tol));
                }
                }
                catch(Exception ex)
                {
                    index.Add(null);
                    errorMsg.Add($"Import of Line Load No.{rfLoad.No} failed! " + ex.Message);
                }
            }
        }

        public static RFLineLoad SetRFLineLoad(this IModelData data, ILoadCase loadcase, ref RFLineLoad rfLineLoad, ref List<RFNode> existingNodes, ref List<RFLine> existingLines, ref int lastNoNo, ref int lastLineNo, ref int lastLLNo, double tol)
        {
            // Get Node List?
            if (rfLineLoad.LineList == null)
            {
                var lineList = "";
                data.PrepareModification();
                foreach (var rfLine in rfLineLoad.BaseLines)
                {
                    var myRFLine = new RFLine(rfLine);
                    lineList += data.SetRFLine(ref myRFLine, ref existingNodes, ref existingLines, ref lastNoNo, ref lastLineNo).No.ToString() + ",";
                }
                data.FinishModification();
                rfLineLoad.LineList = lineList.Substring(0, lineList.Length - 1);
            }
            // Set support with a provided index number
            if (!(rfLineLoad.No == 0))
            {
                loadcase.PrepareModification();
                loadcase.SetLineLoad(rfLineLoad);
                loadcase.FinishModification();
                return rfLineLoad;
            }
            // Set node without provided index number
            lastLLNo += 1;
            rfLineLoad.No = lastLLNo;
            loadcase.PrepareModification();
            loadcase.SetLineLoad(rfLineLoad);
            loadcase.FinishModification();
            return rfLineLoad;
        }

        public static void SetRFMemberLoads(this IModelData data, ILoads loads, List<GH_RFEM> ghMLoads, ref List<RFMemberLoad> index, ref List<string> errorMsg)
        {
            var newData = false;
            var inMLoads = new List<RFMemberLoad>();
            var lastMLNo = 0;

            inMLoads = ghMLoads.Select(x => new RFMemberLoad((RFMemberLoad)x.Value)).ToList();
            foreach (var rfLoad in inMLoads)
            {
                                                            try
                                                            {
                                                                var loadcase = loads.GetLoadCase(rfLoad.LoadCase, ItemAt.AtNo);
                if (rfLoad.ToModify)
                {
                    var myload = (MemberLoad)rfLoad;
                    //if (rfSup.NewNo > 0)
                    //{
                    //    myNo = rfSup.NewNo;
                    //}
                    loadcase.GetMemberLoad(rfLoad.No, ItemAt.AtNo).SetData(ref myload);
                    index.Add(rfLoad);
                }
                else if (rfLoad.ToDelete)
                {
                    loadcase.GetMemberLoad(rfLoad.No, ItemAt.AtNo).Delete();
                    index.Add(rfLoad);
                }
                else
                {
                    if (newData == false)
                    {
                        lastMLNo = loadcase.GetLastObjectNo(LoadObjectType.MemberLoadObject);
                        newData = true;
                    }
                    index.Add(data.SetRFMemberLoad(loadcase, rfLoad, ref lastMLNo));
                }
                }
                catch(Exception ex)
                {
                    index.Add(null);
                    errorMsg.Add($"Import of Member Load No.{rfLoad.No} failed! " + ex.Message);
                }
            }
        }

        public static RFMemberLoad SetRFMemberLoad(this IModelData data, ILoadCase loadcase, ref RFMemberLoad rfMemberLoad, ref int lastMLNo)
        {
            // Get Node List?
            // Set support with a provided index number
            if (!(rfMemberLoad.No == 0))
            {
                loadcase.PrepareModification();
                loadcase.SetMemberLoad(rfMemberLoad);
                loadcase.FinishModification();
                return rfMemberLoad;
            }
            // Set node without provided index number
            lastMLNo += 1;
            rfMemberLoad.No = lastMLNo;
            loadcase.PrepareModification();
            loadcase.SetMemberLoad(rfMemberLoad);
            loadcase.FinishModification();
            return rfMemberLoad;
        }

        public static void SetRFSurfaceLoads(this IModelData data, ILoads loads, List<GH_RFEM> ghSLoads, ref List<RFSurfaceLoad> index, ref List<string> errorMsg)
        {
            var newData = false;
            var inSLoads = new List<RFSurfaceLoad>();
            var lastSLNo = 0;

            inSLoads = ghSLoads.Select(x => new RFSurfaceLoad((RFSurfaceLoad)x.Value)).ToList();
            foreach (var rfLoad in inSLoads)
            {
                                                                try
                                                                {
                                                                    var loadcase = loads.GetLoadCase(rfLoad.LoadCase, ItemAt.AtNo);
                if (rfLoad.ToModify)
                {
                    var myload = (SurfaceLoad)rfLoad;
                    //if (rfSup.NewNo > 0)
                    //{
                    //    myNo = rfSup.NewNo;
                    //}
                    loadcase.GetSurfaceLoad(rfLoad.No, ItemAt.AtNo).SetData(ref myload);
                    index.Add(rfLoad);
                }
                else if (rfLoad.ToDelete)
                {
                    loadcase.GetSurfaceLoad(rfLoad.No, ItemAt.AtNo).Delete();
                    index.Add(rfLoad);
                }
                else
                {
                    if (newData == false)
                    {
                        lastSLNo = loadcase.GetLastObjectNo(LoadObjectType.SurfaceLoadObject);
                        newData = true;
                    }
                    index.Add(data.SetRFSurfaceLoad(loadcase, rfLoad, ref lastSLNo));
                }
                }
                catch(Exception ex)
                {
                    index.Add(null);
                    errorMsg.Add($"Import of Surface Load No.{rfLoad.No} failed! " + ex.Message);
                }
            }
        }


        public static RFSurfaceLoad SetRFSurfaceLoad(this IModelData data, ILoadCase loadcase, ref RFSurfaceLoad rfSurfaceLoad, ref int lastSLNo)
        {
            // Get Node List?
            // Set support with a provided index number
            if (!(rfSurfaceLoad.No == 0))
            {
                loadcase.PrepareModification();
                loadcase.SetSurfaceLoad(rfSurfaceLoad);
                loadcase.FinishModification();
                return rfSurfaceLoad;
            }
            // Set node without provided index number
            lastSLNo += 1;
            rfSurfaceLoad.No = lastSLNo;
            loadcase.PrepareModification();
            loadcase.SetSurfaceLoad(rfSurfaceLoad);
            loadcase.FinishModification();
            return rfSurfaceLoad;
        }

        public static void SetRFFreePolygonLoads(this IModelData data, ILoads loads, List<GH_RFEM> ghSLoads, ref List<RFFreePolygonLoad> index, ref List<string> errorMsg)
        {
            var newData = false;
            var inPLoads = new List<RFFreePolygonLoad>();
            var lastPLNo = 0;

            inPLoads = ghSLoads.Select(x => new RFFreePolygonLoad((RFFreePolygonLoad)x.Value)).ToList();
            foreach (var rfLoad in inPLoads)
            {
                                                                    try
                                                                    {
                                                                        var loadcase = loads.GetLoadCase(rfLoad.LoadCase, ItemAt.AtNo);
                if (rfLoad.ToModify)
                {
                    var myload = (FreePolygonLoad)rfLoad;
                    //if (rfSup.NewNo > 0)
                    //{
                    //    myNo = rfSup.NewNo;
                    //}
                    loadcase.GetFreePolygonLoad(rfLoad.No, ItemAt.AtNo).SetData(ref myload);
                    index.Add(rfLoad);
                }
                else if (rfLoad.ToDelete)
                {
                    loadcase.GetFreePolygonLoad(rfLoad.No, ItemAt.AtNo).Delete();
                    index.Add(rfLoad);
                }
                else
                {
                    if (newData == false)
                    {
                        lastPLNo = loadcase.GetLastObjectNo(LoadObjectType.SurfaceLoadObject);
                        newData = true;
                    }
                    index.Add(data.SetRFFreePolygonLoad(loadcase, rfLoad, ref lastPLNo));
                }
                }
                catch(Exception ex)
                {
                    index.Add(null);
                    errorMsg.Add($"Import of Free Polygon Load No.{rfLoad.No} failed! " + ex.Message);
                }
            }
        }

        public static RFFreePolygonLoad SetRFFreePolygonLoad(this IModelData data, ILoadCase loadcase, ref RFFreePolygonLoad rfPolyLoad, ref int lastPLNo)
        {
            // Get Node List?
            // Set support with a provided index number
            if (!(rfPolyLoad.No == 0))
            {
                loadcase.PrepareModification();
                loadcase.SetFreePolygonLoad(rfPolyLoad);
                loadcase.FinishModification();
                return rfPolyLoad;
            }
            // Set node without provided index number
            lastPLNo += 1;
            rfPolyLoad.No = lastPLNo;
            loadcase.PrepareModification();
            loadcase.SetFreePolygonLoad(rfPolyLoad);
            loadcase.FinishModification();
            return rfPolyLoad;
        }

        public static void SetRFLoadCases(this IModelData data, ILoads loads, List<GH_RFEM> ghLoadCases, ref List<RFLoadCase> index, ref List<string> errorMsg)
        {
            var newData = false;
            var inLoadCases = new List<RFLoadCase>();
            var lastLCNo = 0;

            inLoadCases = ghLoadCases.Select(x => new RFLoadCase((RFLoadCase)x.Value)).ToList();
            foreach (var rfLoadCase in inLoadCases)
            {
            try
            {
                if (rfLoadCase.ToModify)
                {
                    var myloadcase = (LoadCase)rfLoadCase;
                    //if (rfSup.NewNo > 0)
                    //{
                    //    myNo = rfSup.NewNo;
                    //}
                    loads.GetLoadCase(rfLoadCase.No, ItemAt.AtNo).SetData(ref myloadcase);
                    index.Add(rfLoadCase);
                }
                else if (rfLoadCase.ToDelete)
                {
                    loads.GetLoadCase(rfLoadCase.No, ItemAt.AtNo).Delete();
                    index.Add(rfLoadCase);
                }
                else
                {
                    if (newData == false)
                    {
                        lastLCNo = loads.GetLastObjectNo(LoadingType.LoadCaseType);
                        newData = true;
                    }
                    index.Add(data.SetRFLoadCase(loads, rfLoadCase, ref lastLCNo));
                }
                }
                catch(Exception ex)
                {
                    index.Add(null);
                    errorMsg.Add($"Import of Load Case No.{rfLoadCase.No} failed! " + ex.Message);
                }
            }
        }

        public static RFLoadCase SetRFLoadCase(this IModelData data, ILoads loads, ref RFLoadCase rfLoadCase, ref int lastLCNo)
        {
            // Get Node List?
            // Set support with a provided index number
            if (!(rfLoadCase.No == 0))
            {
                loads.PrepareModification();
                loads.SetLoadCase(rfLoadCase);
                loads.FinishModification();
                return rfLoadCase;
            }
            // Set node without provided index number
            lastLCNo += 1;
            rfLoadCase.No = lastLCNo;
            loads.PrepareModification();
            loads.SetLoadCase(rfLoadCase);
            loads.FinishModification();
            return rfLoadCase;
        }

        public static void SetRFLoadCombos(this IModelData data, ILoads loads, List<GH_RFEM> ghLoadCases, ref List<RFLoadCombo> index, ref List<string> errorMsg)
        {
            var newData = false;
            var inLoadCombos = new List<RFLoadCombo>();
            var lastLCNo = 0;

            inLoadCombos = ghLoadCases.Select(x => new RFLoadCombo((RFLoadCombo)x.Value)).ToList();
            foreach (var rfLoadCombo in inLoadCombos)
            {
                try
                {
                    if (rfLoadCombo.ToModify)
                    {
                        var myloadcombo = (LoadCombination)rfLoadCombo;
                        //if (rfSup.NewNo > 0)
                        //{
                        //    myNo = rfSup.NewNo;
                        //}
                        loads.GetLoadCombination(rfLoadCombo.No, ItemAt.AtNo).SetData(ref myloadcombo);
                        index.Add(rfLoadCombo);
                    }
                    else if (rfLoadCombo.ToDelete)
                    {
                        loads.GetLoadCombination(rfLoadCombo.No, ItemAt.AtNo).Delete();
                        index.Add(rfLoadCombo);
                    }
                    else
                    {
                        if (newData == false)
                        {
                            lastLCNo = loads.GetLastObjectNo(LoadingType.LoadCombinationType);
                            newData = true;
                        }
                        index.Add(data.SetRFLoadCombo(loads, rfLoadCombo, ref lastLCNo));
                    }
                }
                catch(Exception ex)
                {
                    index.Add(null);
                    errorMsg.Add($"Import of Load Combination No.{rfLoadCombo.No} failed! " + ex.Message);
                }
            }
        }

        public static RFLoadCombo SetRFLoadCombo(this IModelData data, ILoads loads, ref RFLoadCombo rfLoadCombo, ref int lastLCNo)
        {
            // Set analysis parameters to calculate nach Th.I.Ordnung
            var anal = new AnalysisParameters();
            anal.Method = AnalysisMethodType.GeometricallyLinearStatic;

            // Get Node List?
            // Set support with a provided index number
            if (!(rfLoadCombo.No == 0))
            {
                loads.PrepareModification();
                loads.SetLoadCombination(rfLoadCombo);
                loads.GetLoadCombination(rfLoadCombo.No, ItemAt.AtNo).SetAnalysisParameters(anal);
                loads.FinishModification();
                return rfLoadCombo;
            }
            // Set node without provided index number
            lastLCNo += 1;
            rfLoadCombo.No = lastLCNo;
            loads.PrepareModification();
            loads.SetLoadCombination(rfLoadCombo);
            loads.GetLoadCombination(rfLoadCombo.No, ItemAt.AtNo).SetAnalysisParameters(anal);
            loads.FinishModification();
            return rfLoadCombo;
        }

        public static void SetRFResultCombos(this IModelData data, ILoads loads, List<GH_RFEM> ghLoadCases, ref List<RFResultCombo> index, ref List<string> errorMsg)
        {
            var newData = false;
            var inLoadCombos = new List<RFResultCombo>();
            var lastLCNo = 0;

            inLoadCombos = ghLoadCases.Select(x => new RFResultCombo((RFResultCombo)x.Value)).ToList();
            foreach (var rfLoadCombo in inLoadCombos)
            {
                                                                                try
                                                                                {
                                                                                    if (rfLoadCombo.ToModify)
                {
                    var myloadcombo = (ResultCombination)rfLoadCombo;
                    //if (rfSup.NewNo > 0)
                    //{
                    //    myNo = rfSup.NewNo;
                    //}
                    loads.GetResultCombination(rfLoadCombo.No, ItemAt.AtNo).SetData(ref myloadcombo);
                    index.Add(rfLoadCombo);
                }
                else if (rfLoadCombo.ToDelete)
                {
                    loads.GetResultCombination(rfLoadCombo.No, ItemAt.AtNo).Delete();
                    index.Add(rfLoadCombo);
                }
                else
                {
                    if (newData == false)
                    {
                        lastLCNo = loads.GetLastObjectNo(LoadingType.ResultCombinationType);
                        newData = true;
                    }
                    index.Add(data.SetRFResultCombo(loads, rfLoadCombo, ref lastLCNo));
                }
                }
                catch(Exception ex)
                {
                    index.Add(null);
                    errorMsg.Add($"Import of Result Combination No.{rfLoadCombo.No} failed! " + ex.Message);
                }
            }
        }

        public static RFResultCombo SetRFResultCombo(this IModelData data, ILoads loads, ref RFResultCombo rfLoadCombo, ref int lastLCNo)
        {
            // Get Node List?
            // Set support with a provided index number
            if (!(rfLoadCombo.No == 0))
            {
                loads.PrepareModification();
                loads.SetResultCombination(rfLoadCombo);
                loads.FinishModification();
                return rfLoadCombo;
            }
            // Set node without provided index number
            lastLCNo += 1;
            rfLoadCombo.No = lastLCNo;
            loads.PrepareModification();
            loads.SetResultCombination(rfLoadCombo);
            loads.FinishModification();
            return rfLoadCombo;
        }

        public static void ClearOutput(ref List<RFNode> nodelist, ref List<RFLine> linelist, ref List<RFMember> memberlist, 
            ref List<RFSurface> srfclist, ref List<RFOpening> oplist, ref List<RFSupportP> supPlist, ref List<RFSupportL> supLlist,
            ref List<RFLineHinge> lineHingelist, ref List<RFCroSec> croSeclist, ref List<RFMaterial> matlist,
            ref List<RFNodalLoad> nodalLoadList)
        {
            nodelist.Clear();
            linelist.Clear();
            memberlist.Clear();
            srfclist.Clear();
            oplist.Clear();
            supPlist.Clear();
            supLlist.Clear();
            lineHingelist.Clear();
            croSeclist.Clear();
            matlist.Clear();
            nodalLoadList.Clear();
        }

        public static void ClearOutput(ref List<RFNode> nodelist, ref List<RFLine> linelist, ref List<RFMember> memberlist,
          ref List<RFSurface> srfclist, ref List<RFOpening> oplist, ref List<RFSupportP> supPlist, ref List<RFSupportL> supLlist,
         ref List<RFLineHinge> lineHingelist, ref List<RFCroSec> croSeclist, ref List<RFMaterial> matlist,
         ref List<RFNodalLoad> nodalLoadList, ref List<RFLineLoad> lineLoadList, ref List<RFMemberLoad> memberLoadList, 
         ref List<RFSurfaceLoad> surfaceLoadList, ref List<RFFreePolygonLoad> polyLoadList, ref List<RFLoadCase> loadCaseList,
         ref List<RFLoadCombo> loadComboList, ref List<RFResultCombo> resultComboList)
        {
            nodelist.Clear();
            linelist.Clear();
            memberlist.Clear();
            srfclist.Clear();
            oplist.Clear();
            supPlist.Clear();
            supLlist.Clear();
            lineHingelist.Clear();
            croSeclist.Clear();
            matlist.Clear();
            nodalLoadList.Clear();
            lineLoadList.Clear();
            memberLoadList.Clear();
            surfaceLoadList.Clear();
            polyLoadList.Clear();
            loadCaseList.Clear();
            loadComboList.Clear();
            resultComboList.Clear();
        }

        public static void ClearOutput(ref List<RFNode> nodelist, ref List<RFLine> linelist, ref List<RFMember> memberlist,
  ref List<RFSurface> srfclist, ref List<RFOpening> oplist, ref List<RFSupportP> supPlist, ref List<RFSupportL> supLlist,
 ref List<RFSupportS> supSlist, ref List<RFLineHinge> lineHingelist, ref List<RFCroSec> croSeclist, ref List<RFMaterial> matlist,
 ref List<RFNodalLoad> nodalLoadList, ref List<RFLineLoad> lineLoadList, ref List<RFMemberLoad> memberLoadList,
 ref List<RFSurfaceLoad> surfaceLoadList, ref List<RFFreePolygonLoad> polyLoadList, ref List<RFLoadCase> loadCaseList,
 ref List<RFLoadCombo> loadComboList, ref List<RFResultCombo> resultComboList)
        {
            nodelist.Clear();
            linelist.Clear();
            memberlist.Clear();
            srfclist.Clear();
            oplist.Clear();
            supPlist.Clear();
            supLlist.Clear();
            supSlist.Clear();
            lineHingelist.Clear();
            croSeclist.Clear();
            matlist.Clear();
            nodalLoadList.Clear();
            lineLoadList.Clear();
            memberLoadList.Clear();
            surfaceLoadList.Clear();
            polyLoadList.Clear();
            loadCaseList.Clear();
            loadComboList.Clear();
            resultComboList.Clear();
        }
    }
}
