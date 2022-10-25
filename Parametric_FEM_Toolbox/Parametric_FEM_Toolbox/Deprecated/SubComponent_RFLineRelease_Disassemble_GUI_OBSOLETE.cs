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
using Parametric_FEM_Toolbox.GUI;

namespace Parametric_FEM_Toolbox.Deprecated
{
    public class SubComponent_RFLineRelease_Disassemble_GUI_OBSOLETE : SubComponent
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
            evaluationUnit.Icon = Properties.Resources.Disassemble_LineReleases;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {
            unit.RegisterInputParam(new Param_RFEM(), "RF Line Release", "RF LineRelease", "Input RFLineRelease.", GH_ParamAccess.item);
            // unit.Inputs[0].Parameter.Optional = true;

            unit.RegisterOutputParam(new Param_Integer(), "Release Number", "No", "Index number of the RFEM object.");
            unit.RegisterOutputParam(new Param_Integer(), "Line Number", "Line", "Line number of the Line Release.");
            unit.RegisterOutputParam(new Param_Integer(), "Line Release Type", "TypeNo", "Index Number of the line release type.");
            unit.RegisterOutputParam(new Param_String(), "Member List", "Members", "List of released members");
            unit.RegisterOutputParam(new Param_String(), "Surface List", "Sfcs", "List of released surfaces");
            unit.RegisterOutputParam(new Param_String(), "Solid List", "Solids", "List of released solids");
            unit.RegisterOutputParam(new Param_String(), "Axis System", "Axis", "Axis System");
            unit.RegisterOutputParam(new Param_String(), "Release Location", "Location", "Location");
            unit.RegisterOutputParam(new Param_String(), "Definition nodes", "DefNodes", "Nodes to use as definition nodes");
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
            var rfNodalRelease = (RFLineRelease)inGH.Value;

            // Output
            DA.SetData(0, rfNodalRelease.No);
            DA.SetData(1, rfNodalRelease.LineNo);
            DA.SetData(2, rfNodalRelease.TypeNo);
            DA.SetData(3, rfNodalRelease.ReleasedMembers);
            DA.SetData(4, rfNodalRelease.ReleasedSurfaces);
            DA.SetData(5, rfNodalRelease.ReleasedSolids);
            DA.SetData(6, rfNodalRelease.AxisSystem);
            DA.SetData(7, rfNodalRelease.Location);
            DA.SetData(8, rfNodalRelease.DefinitionNodes);
            DA.SetData(9, rfNodalRelease.Comment);
        }
    }
}
