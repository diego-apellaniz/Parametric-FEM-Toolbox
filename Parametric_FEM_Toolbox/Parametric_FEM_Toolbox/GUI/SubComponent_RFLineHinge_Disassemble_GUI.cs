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
    public class SubComponent_RFLineHinge_Disassemble_GUI : SubComponent
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
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Disassemble Line Hinges.");
            evaluationUnit.Icon = Properties.Resources.Disassemble_LineHinge;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {
            unit.RegisterInputParam(new Param_RFEM(), "RF Line Hinge", "RF LineHinge", "Input RFLineHinge.", GH_ParamAccess.item);
            // unit.Inputs[0].Parameter.Optional = true;

            unit.RegisterOutputParam(new Param_Curve(), "Line", "Line", "Line or Curve the RFLineHinge is attached to.");
            unit.RegisterOutputParam(new Param_Integer(), "Hinge Number", "No", "Index number of the RFEM object.");
            unit.RegisterOutputParam(new Param_Integer(), "Line No", "LineNo", "Index number of the line the hinge is attached to");
            unit.RegisterOutputParam(new Param_Integer(), "Sfc No", "SfcNo", "Index number of the surface the hinge is attached to");
            unit.RegisterOutputParam(new Param_Number(), "Displacement Dir X", "Tx", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]");
            unit.RegisterOutputParam(new Param_Number(), "Displacement Dir Y", "Ty", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]");
            unit.RegisterOutputParam(new Param_Number(), "Displacement Dir Z", "Tz", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]");
            unit.RegisterOutputParam(new Param_Number(), "Rotation Dir X", "Rx", "(-1): Fixed; (0): Free; Other: Stiffness in [kNm/rad]");
            unit.RegisterOutputParam(new Param_Number(), "Rotation Dir Y", "Ry", "(-1): Fixed; (0): Free; Other: Stiffness in [kNm/rad]");
            unit.RegisterOutputParam(new Param_Number(), "Rotation Dir Z", "Rz", "(-1): Fixed; (0): Free; Other: Stiffness in [kNm/rad]");
            unit.RegisterOutputParam(new Param_String(), "Comment", "Comment", "Comment.");
            unit.RegisterOutputParam(new Param_String(), "Side", "Side", "Hinge Side Type");
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
            var rfLineHinge = (RFLineHinge)inGH.Value;
            // Output
            DA.SetData(0, rfLineHinge.BaseLine.ToCurve());
            DA.SetData(1, rfLineHinge.No);
            DA.SetData(2, rfLineHinge.LineNo);
            DA.SetData(3, rfLineHinge.SfcNo);
            DA.SetData(4, ((rfLineHinge.Tx < 0) ? rfLineHinge.Tx * 1000 : rfLineHinge.Tx));
            DA.SetData(5, ((rfLineHinge.Ty < 0) ? rfLineHinge.Ty * 1000 : rfLineHinge.Ty));
            DA.SetData(6, ((rfLineHinge.Tz < 0) ? rfLineHinge.Tz * 1000 : rfLineHinge.Tz));
            DA.SetData(7, ((rfLineHinge.Rx < 0) ? rfLineHinge.Rx * 1000 : rfLineHinge.Rx));
            DA.SetData(8, ((rfLineHinge.Ry < 0) ? rfLineHinge.Ry * 1000 : rfLineHinge.Ry));
            DA.SetData(9, ((rfLineHinge.Rz < 0) ? rfLineHinge.Rz * 1000 : rfLineHinge.Rz));
            DA.SetData(10, rfLineHinge.Comment);
            DA.SetData(11, rfLineHinge.Side);
        }
    }
}
