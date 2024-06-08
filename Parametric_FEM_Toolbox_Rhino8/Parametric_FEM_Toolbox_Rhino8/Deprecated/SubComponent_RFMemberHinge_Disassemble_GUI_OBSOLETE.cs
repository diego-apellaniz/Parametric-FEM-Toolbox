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
    public class SubComponent_RFMemberHinge_Disassemble_GUI_OBSOLETE : SubComponent
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
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Disassemble Member Hinges.");
            evaluationUnit.Icon = Properties.Resources.Disassemble_MemberRelease;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {
            unit.RegisterInputParam(new Param_RFEM(), "RF Member Hinge", "RF MemberHinge", "Input RFMemberHinge.", GH_ParamAccess.item);
            // unit.Inputs[0].Parameter.Optional = true;

            unit.RegisterOutputParam(new Param_Integer(), "Hinge Number", "No", "Index number of the RFEM object.");
            unit.RegisterOutputParam(new Param_Number(), "Displacement Dir X", "Tx", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]");
            unit.RegisterOutputParam(new Param_Number(), "Displacement Dir Y", "Ty", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]");
            unit.RegisterOutputParam(new Param_Number(), "Displacement Dir Z", "Tz", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]");
            unit.RegisterOutputParam(new Param_Number(), "Rotation Dir X", "Rx", "(-1): Fixed; (0): Free; Other: Stiffness in [kNm/rad]");
            unit.RegisterOutputParam(new Param_Number(), "Rotation Dir Y", "Ry", "(-1): Fixed; (0): Free; Other: Stiffness in [kNm/rad]");
            unit.RegisterOutputParam(new Param_Number(), "Rotation Dir Z", "Rz", "(-1): Fixed; (0): Free; Other: Stiffness in [kNm/rad]");
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
            var rfMemberHinge = (RFMemberHinge)inGH.Value;
            // Output
            DA.SetData(0, rfMemberHinge.No);
            DA.SetData(1, ((rfMemberHinge.Tx < 0) ? rfMemberHinge.Tx * 1000 : rfMemberHinge.Tx));
            DA.SetData(2, ((rfMemberHinge.Ty < 0) ? rfMemberHinge.Ty * 1000 : rfMemberHinge.Ty));
            DA.SetData(3, ((rfMemberHinge.Tz < 0) ? rfMemberHinge.Tz * 1000 : rfMemberHinge.Tz));
            DA.SetData(4, ((rfMemberHinge.Rx < 0) ? rfMemberHinge.Rx * 1000 : rfMemberHinge.Rx));
            DA.SetData(5, ((rfMemberHinge.Ry < 0) ? rfMemberHinge.Ry * 1000 : rfMemberHinge.Ry));
            DA.SetData(6, ((rfMemberHinge.Rz < 0) ? rfMemberHinge.Rz * 1000 : rfMemberHinge.Rz));
            DA.SetData(7, rfMemberHinge.Comment);
        }
    }
}
