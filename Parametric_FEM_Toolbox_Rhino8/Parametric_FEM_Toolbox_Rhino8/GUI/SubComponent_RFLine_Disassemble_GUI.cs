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
    public class SubComponent_RFLine_Disassemble_GUI : SubComponent
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
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Disassemble Lines.");
            evaluationUnit.Icon = Properties.Resources.Disassemble_Line;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {

            unit.RegisterInputParam(new Param_RFEM(), "RF Line", "RF Line", "Input RFLine.", GH_ParamAccess.item);
            // unit.Inputs[0].Parameter.Optional = true;

            unit.RegisterOutputParam(new Param_Curve(), "Line", "Line", "Line or Curve to assemble the RFLine from.");
            unit.RegisterOutputParam(new Param_Integer(), "Line Number", "No", "Index number of the RFEM object.");
            unit.RegisterOutputParam(new Param_String(), "Comment", "Comment", "Comment.");
            unit.RegisterOutputParam(new Param_String(), "NodeList", "NodeList", "Node List");
            unit.RegisterOutputParam(new Param_String(), "Line Type", "Type", "Line Type");
            unit.RegisterOutputParam(new Param_Number(), "Rotation Angle [°]", "β", "Rotation Angle [°]");
            unit.RegisterOutputParam(new Param_String(), "Rotation Type", "Rot Type", "Rotation Type");

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
            var rFLine = (RFLine)inGH.Value;
            // Output
            DA.SetData(0, rFLine.ToCurve());
            DA.SetData(1, rFLine.No);
            DA.SetData(2, rFLine.Comment);
            DA.SetData(3, rFLine.NodeList);
            DA.SetData(4, rFLine.Type);
            DA.SetData(5, rFLine.RotationAngle);
            DA.SetData(6, rFLine.RotationType);

        }
    }
}
