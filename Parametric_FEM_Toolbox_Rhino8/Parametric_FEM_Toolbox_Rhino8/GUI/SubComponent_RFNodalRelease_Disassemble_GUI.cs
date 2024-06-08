using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using Parametric_FEM_Toolbox.UIWidgets;
using Parametric_FEM_Toolbox.Utilities;
using Parametric_FEM_Toolbox.HelperLibraries;
using Dlubal.RFEM5;
using Parametric_FEM_Toolbox.RFEM;

namespace Parametric_FEM_Toolbox.GUI
{
    public class SubComponent_RFNodalRelease_Disassemble_GUI : SubComponent
    {
        public override string name()
        {
            return "Disassemble";
        }
        public override string display_name()
        {
            return "Disassemble";
        }
        public override void registerEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Disassemble Nodal Releases.");
            evaluationUnit.Icon = Properties.Resources.Disassemble_NodalRelease;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {
            unit.RegisterInputParam(new Param_RFEM(), "RF Nodal Release", "RF NodalRelease", "Input RFNodalRelease.", GH_ParamAccess.item);
            // unit.Inputs[0].Parameter.Optional = true;

            unit.RegisterOutputParam(new Param_Integer(), "Release Number", "No", "Index number of the RFEM object.");
            unit.RegisterOutputParam(new Param_Integer(), "Node Number", "Node", "Node number of the Nodal Release.");
            unit.RegisterOutputParam(new Param_String(), "Member List", "Members", "List of released members");
            unit.RegisterOutputParam(new Param_String(), "Surface List", "Sfcs", "List of released surfaces");
            unit.RegisterOutputParam(new Param_String(), "Solid List", "Solids", "List of released solids");
            unit.RegisterOutputParam(new Param_Integer(), "Hinge Number", "Hinge", "Nomber of member hinge to import properties from");
            unit.RegisterOutputParam(new Param_Integer(), "Member Number", "Member", "Member (or line) number to get the axis system from");
            unit.RegisterOutputParam(new Param_String(), "Axis System", "Axis", "Axis System");
            unit.RegisterOutputParam(new Param_String(), "Release Location", "Location", "Location");
            unit.RegisterOutputParam(new Param_String(), "Comment", "Comment", "Comment.");
        }

        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Blank;

            // Input
            var inGH = new GH_RFEM();
            if (!DA.GetData(0, ref inGH))
            {
                return;
            }
            var rfNodalRelease = (RFNodalRelease)inGH.Value;

            // Output
            DA.SetData(0, rfNodalRelease.No);
            DA.SetData(1, rfNodalRelease.NodeNo);
            DA.SetData(2, rfNodalRelease.ReleasedMembers);
            DA.SetData(3, rfNodalRelease.ReleasedSurfaces);
            DA.SetData(4, rfNodalRelease.ReleasedSolids);
            DA.SetData(5, rfNodalRelease.MemberHingeNo);
            DA.SetData(6, rfNodalRelease.AxisSystemFromObjectNo);
            DA.SetData(7, rfNodalRelease.AxisSystem);
            DA.SetData(8, rfNodalRelease.Location);
            DA.SetData(9, rfNodalRelease.Comment);
        }
    }
}
