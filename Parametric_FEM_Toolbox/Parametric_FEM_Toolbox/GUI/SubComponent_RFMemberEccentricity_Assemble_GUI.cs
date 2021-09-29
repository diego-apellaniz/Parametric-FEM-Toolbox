using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using Parametric_FEM_Toolbox.UIWidgets;
using Parametric_FEM_Toolbox.Utilities;
using Parametric_FEM_Toolbox.HelperLibraries;
using Dlubal.RFEM5;
using Parametric_FEM_Toolbox.RFEM;

namespace Parametric_FEM_Toolbox.GUI
{
    public class SubComponent_RFMemberEccentricity_Assemble_GUI : SubComponent
    {
        public override string name()
        {
            return "Assemble";
        }
        public override string display_name()
        {
            return "Assemble";
        }
        public override void registerEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Assemble Member Eccentricities.");
            evaluationUnit.Icon = Properties.Resources.Assemble_MemberEccentricity;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {
            unit.RegisterInputParam(new Param_Integer(), "Hinge Number", "No", "Optional index number to assign to the RFEM object.", GH_ParamAccess.item);
            unit.Inputs[0].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Vector(), "Start Eccentricities", "Start", "Member Start Eccentricities [mm]", GH_ParamAccess.item);
            unit.Inputs[1].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Vector(), "End Eccentricities", "End", "Member End Eccentricities [mm]", GH_ParamAccess.item);
            unit.Inputs[2].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Horizontal alignment", "H", UtilLibrary.DescriptionRFTypes(typeof(HorizontalAlignmentType)), GH_ParamAccess.item);
            unit.Inputs[3].EnumInput = UtilLibrary.ListRFTypes(typeof(HorizontalAlignmentType));
            unit.Inputs[3].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Vertical alignment", "V", UtilLibrary.DescriptionRFTypes(typeof(VerticalAlignmentType)), GH_ParamAccess.item);
            unit.Inputs[4].EnumInput = UtilLibrary.ListRFTypes(typeof(VerticalAlignmentType));
            unit.Inputs[4].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Comment", "Comment", "Comment.", GH_ParamAccess.item);
            unit.Inputs[5].Parameter.Optional = true;


            GH_ExtendableMenu gH_ExtendableMenu = new GH_ExtendableMenu(0, "advanced");
            gH_ExtendableMenu.Name = "Advanced";
            gH_ExtendableMenu.Collapse();
            unit.RegisterInputParam(new Param_Integer(), "Reference system", "Sys", UtilLibrary.DescriptionRFTypes(typeof(ReferenceSystemType)), GH_ParamAccess.item);
            unit.Inputs[6].EnumInput = UtilLibrary.ListRFTypes(typeof(ReferenceSystemType));
            unit.Inputs[6].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Hinge At Start Node", "HStart", "Member hinge location at start node", GH_ParamAccess.item);
            unit.Inputs[7].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Hinge At End Node", "HEnd", "Member hinge location at end node", GH_ParamAccess.item);
            unit.Inputs[8].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Transverse offset", "TOff", "Transverse offset from cross-section of other object", GH_ParamAccess.item);
            unit.Inputs[9].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Reference object type", "Ref", UtilLibrary.DescriptionRFTypes(typeof(ModelObjectType)), GH_ParamAccess.item);
            unit.Inputs[10].EnumInput = UtilLibrary.ListRFTypes(typeof(ModelObjectType));
            unit.Inputs[10].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Reference object No", "ObjNo", "Reference object No", GH_ParamAccess.item);
            unit.Inputs[11].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Horizontal axis offset", "Ho", UtilLibrary.DescriptionRFTypes(typeof(HorizontalAlignmentType)), GH_ParamAccess.item);
            unit.Inputs[12].EnumInput = UtilLibrary.ListRFTypes(typeof(HorizontalAlignmentType));
            unit.Inputs[12].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Vertical axis offset", "Vo", UtilLibrary.DescriptionRFTypes(typeof(VerticalAlignmentType)), GH_ParamAccess.item);
            unit.Inputs[13].EnumInput = UtilLibrary.ListRFTypes(typeof(VerticalAlignmentType));
            unit.Inputs[13].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Start Adjoining Members Offset", "OStart", "Axial offset from adjoining members at member start", GH_ParamAccess.item);
            unit.Inputs[14].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "End Adjoining Members Offset", "OEnd", "Axial offset from adjoining members at member end", GH_ParamAccess.item);
            unit.Inputs[15].Parameter.Optional = true;
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[6]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[7]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[8]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[9]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[10]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[11]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[12]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[13]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[14]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[15]);
            unit.AddMenu(gH_ExtendableMenu);


            GH_ExtendableMenu gH_ExtendableMenu2 = new GH_ExtendableMenu(1, "modify");
            gH_ExtendableMenu2.Name = "Modify";
            gH_ExtendableMenu2.Collapse();
            
            unit.RegisterInputParam(new Param_RFEM(), "RF Member Eccentricity", "RF MemberEcc", "Member Ecentricity object from the RFEM model to modify", GH_ParamAccess.item);
            unit.Inputs[16].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Modify", "Modify", "Modify object?", GH_ParamAccess.item);
            unit.Inputs[17].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Delete", "Delete", "Delete object?", GH_ParamAccess.item);
            unit.Inputs[18].Parameter.Optional = true;
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[16]);
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[17]);
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[18]);
            unit.AddMenu(gH_ExtendableMenu2);

            unit.RegisterOutputParam(new Param_RFEM(), "RF Member Eccentricity", "RF MemberEcc", "Output RFMemberEccentricity.");
        }

        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Blank;
            var noIndex = 0;
            var comment = "";
            var rfEcc = new RFMemberEccentricity();
            var inRFEM = new GH_RFEM();
            var mod = false;
            var del = false;
            var start = new Vector3d();
            var end = new Vector3d();
            var h = 0;
            var v = 0;
            var sys = 0;
            var hstart = false;
            var hend = false;
            var toff = false;
            var refobj = 0;
            var tobjno = 0;
            var ho = 0;
            var vo = 0;
            var ostart = false;
            var oend = false;

            if (DA.GetData(16, ref inRFEM))
            {
                rfEcc = new RFMemberEccentricity((RFMemberEccentricity)inRFEM.Value);
                if (DA.GetData(1, ref start))
                    rfEcc.Start = new Point3d(start);
                if (DA.GetData(2, ref end))
                    rfEcc.End = new Point3d(end);
                if (DA.GetData(3, ref h))
                {
                    rfEcc.HorizontalAlignment = (HorizontalAlignmentType)h;
                    if (rfEcc.HorizontalAlignment == HorizontalAlignmentType.UnknownHAlignment)
                    {
                        msg = "Alignment Type not supported. ";
                        level = GH_RuntimeMessageLevel.Warning;
                        return;
                    }
                }
                if (DA.GetData(4, ref v))
                {
                    rfEcc.VerticalAlignment = (VerticalAlignmentType)v;
                    if (rfEcc.VerticalAlignment == VerticalAlignmentType.UnknownVAlignment)
                    {
                        msg = "Alignment Type not supported. ";
                        level = GH_RuntimeMessageLevel.Warning;
                        return;
                    }
                }
            }
            else
            {
                rfEcc = new RFMemberEccentricity();
                if (DA.GetData(1, ref start) && DA.GetData(2, ref end) && DA.GetData(3, ref h) && DA.GetData(4, ref v))
                {
                    rfEcc.Start = new Point3d(start);
                    rfEcc.End = new Point3d(end);
                    rfEcc.HorizontalAlignment = (HorizontalAlignmentType)h;                    
                    rfEcc.VerticalAlignment = (VerticalAlignmentType)v;
                    if (rfEcc.HorizontalAlignment == HorizontalAlignmentType.UnknownHAlignment || rfEcc.VerticalAlignment == VerticalAlignmentType.UnknownVAlignment)
                    {
                        msg = "Alignment Type not supported. ";
                        level = GH_RuntimeMessageLevel.Warning;
                        return;
                    }
                }
                else
                {
                    msg = "Insufficient input parameters. Provide eccentricity vectors an allignment types or existing RFMemberEccentricity Object. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
            }
            if (DA.GetData(17, ref mod))
            {
                rfEcc.ToModify = mod;
            }
            if (DA.GetData(18, ref del))
            {
                rfEcc.ToDelete = del;
            }
            if (DA.GetData(0, ref noIndex))
            {
                rfEcc.No = noIndex;
            }
            if (DA.GetData(5, ref comment))
            {
                rfEcc.Comment = comment;
            }
            if (DA.GetData(6, ref sys))
            {
                rfEcc.ReferenceSystemType = (ReferenceSystemType)sys;
                if (rfEcc.ReferenceSystemType == ReferenceSystemType.UnknownReferenceSystemType || rfEcc.ReferenceSystemType == ReferenceSystemType.UserDefinedSystemType)
                {
                    msg = "Reference System Type not supported. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
            }
            if (DA.GetData(7, ref hstart))
            {
                rfEcc.HingeAtStartNode = hstart;
            }
            if (DA.GetData(8, ref hend))
            {
                rfEcc.HingeAtEndNode = hend;
            }
            if (DA.GetData(9, ref toff))
            {
                rfEcc.TransverseOffset = toff;
            }
            if (DA.GetData(10, ref refobj))
            {
                rfEcc.ReferenceObjectType = (ModelObjectType)refobj;
                if ((rfEcc.ReferenceObjectType != ModelObjectType.MemberObject) && (rfEcc.ReferenceObjectType != ModelObjectType.SurfaceObject))
                {
                    msg = "Object Type not supported. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
            }
            if (DA.GetData(11, ref tobjno))
            {
                rfEcc.ReferenceObjectNo = tobjno;
            }
            if (DA.GetData(12, ref ho))
            {
                rfEcc.HorizontalAxisOffset = (HorizontalAlignmentType)ho;
                if (rfEcc.HorizontalAxisOffset == HorizontalAlignmentType.UnknownHAlignment)
                {
                    msg = "Alignment Type not supported. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
            }
            if (DA.GetData(13, ref vo))
            {
                rfEcc.VerticalAxisOffset = (VerticalAlignmentType)vo;
                if (rfEcc.VerticalAxisOffset == VerticalAlignmentType.UnknownVAlignment)
                {
                    msg = "Alignment Type not supported. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
            }
            if (DA.GetData(14, ref ostart))
            {
                rfEcc.StartAdjoiningMembersOffset = ostart;
            }
            if (DA.GetData(15, ref oend))
            {
                rfEcc.EndAdjoiningMembersOffset = oend;
            }
            DA.SetData(0, rfEcc);
        }
    }
}
