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
    public class SubComponent_RFNodalRelease_Assemble_GUI : SubComponent
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
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Assemble Nodal Releases.");
            evaluationUnit.Icon = Properties.Resources.Assemble_NodalRelease;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {
            unit.RegisterInputParam(new Param_Integer(), "Release Number", "No", "Optional index number to assign to the RFEM object.", GH_ParamAccess.item);
            unit.Inputs[0].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Node Number", "Node", "Index Number of the node to assign the release to.", GH_ParamAccess.item);
            unit.Inputs[1].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Member List", "Members", "List of released members", GH_ParamAccess.item);
            unit.Inputs[2].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Hinge Number", "Hinge", "Nomber of member hinge to import properties from", GH_ParamAccess.item);
            unit.Inputs[3].Parameter.Optional = true;            
            unit.RegisterInputParam(new Param_Integer(), "Member Number", "Member", "Member (or line) number to get the axis system from", GH_ParamAccess.item);
            unit.Inputs[4].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Comment", "Comment", "Comment.", GH_ParamAccess.item);
            unit.Inputs[5].Parameter.Optional = true;

            GH_ExtendableMenu gH_ExtendableMenu = new GH_ExtendableMenu(0, "advanced");
            gH_ExtendableMenu.Name = "Advanced";
            gH_ExtendableMenu.Collapse();
            unit.RegisterInputParam(new Param_Integer(), "Axis System", "Axis", UtilLibrary.DescriptionRFTypes(typeof(NodalReleaseAxisSystem)), GH_ParamAccess.item);
            unit.Inputs[6].EnumInput = UtilLibrary.ListRFTypes(typeof(NodalReleaseAxisSystem));
            unit.Inputs[6].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Surface List", "Sfcs", "List of released surfaces", GH_ParamAccess.item);
            unit.Inputs[7].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Solid List", "Solids", "List of released solids", GH_ParamAccess.item);
            unit.Inputs[8].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Release Location", "Location", UtilLibrary.DescriptionRFTypes(typeof(ReleaseLocation)), GH_ParamAccess.item);
            unit.Inputs[9].EnumInput = UtilLibrary.ListRFTypes(typeof(ReleaseLocation));
            unit.Inputs[9].Parameter.Optional = true;
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[6]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[7]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[8]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[9]);
            unit.AddMenu(gH_ExtendableMenu);

            GH_ExtendableMenu gH_ExtendableMenu2 = new GH_ExtendableMenu(1, "modify");
            gH_ExtendableMenu2.Name = "Modify";
            gH_ExtendableMenu2.Collapse();
            unit.RegisterInputParam(new Param_RFEM(), "RF Nodal Release", "RF NodalRelease", "Nodal Release object from the RFEM model to modify", GH_ParamAccess.item);
            unit.Inputs[10].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Modify", "Modify", "Modify object?", GH_ParamAccess.item);
            unit.Inputs[11].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Delete", "Delete", "Delete object?", GH_ParamAccess.item);
            unit.Inputs[12].Parameter.Optional = true;
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[10]);
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[11]);
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[12]);
            unit.AddMenu(gH_ExtendableMenu2);

            unit.RegisterOutputParam(new Param_RFEM(), "RF Nodal Release", "RF NodalRelease", "Output RFNodalRelease.");
        }

        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Blank;
            //var line = new LineCurve();
            var noIndex = 0;
            var comment = "";
            var rfRelease = new RFNodalRelease();
            var inRFEM = new GH_RFEM();
            var mod = false;
            var del = false;
            var member_list = "";
            var sfc_list = "";
            var solid_list = "";
            var nodeNo = 0;
            var noHinge = 0;
            var noMember = 0;
            var axisSystem = 0;
            var location = 0;

            if (DA.GetData(10, ref inRFEM))
            {
                rfRelease = new RFNodalRelease((RFNodalRelease)inRFEM.Value);                
            }
            else if ((DA.GetData(2, ref member_list) | DA.GetData(7, ref sfc_list) | DA.GetData(8, ref solid_list)) && DA.GetData(1, ref nodeNo) && DA.GetData(3, ref noHinge) && DA.GetData(4, ref noMember))
            {
                // Check that axis are not being taken from released item                
                rfRelease = new RFNodalRelease();
                rfRelease.NodeNo = nodeNo;
                rfRelease.ReleasedMembers = member_list;
                rfRelease.ReleasedSurfaces = sfc_list;
                rfRelease.ReleasedSolids = solid_list;
                rfRelease.MemberHingeNo = noHinge;
                rfRelease.AxisSystemFromObjectNo = noMember;
                rfRelease.AxisSystem = NodalReleaseAxisSystem.LocalFromMember;
            }
            else
            {
                msg = "Insufficient input parameters. Provide item list, hinge number and member number. ";
                level = GH_RuntimeMessageLevel.Warning;
                return;
            }
            if (DA.GetData(11, ref mod))
            {
                rfRelease.ToModify = mod;
            }
            if (DA.GetData(12, ref del))
            {
                rfRelease.ToDelete = del;
            }
            if (DA.GetData(0, ref noIndex))
            {
                rfRelease.No = noIndex;
            }
            if (DA.GetData(5, ref comment))
            {
                rfRelease.Comment = comment;
            }
            if (DA.GetData(6, ref axisSystem))
            {
                rfRelease.AxisSystem = (NodalReleaseAxisSystem)axisSystem;
            }
            if (DA.GetData(9, ref location))
            {
                rfRelease.Location = (ReleaseLocation)location;
            }

            // Check that axis are not being taken from released item   
            if (rfRelease.ReleasedMembers != "")
            {
                var items = rfRelease.ReleasedMembers.ToInt();
                if (rfRelease.AxisSystem == NodalReleaseAxisSystem.LocalFromMember && items.Contains(rfRelease.MemberHingeNo))
                {
                    msg = "Choose a member to get the axis system that is not included in the release list. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
            }
            DA.SetData(0, rfRelease);
        }
    }
}
