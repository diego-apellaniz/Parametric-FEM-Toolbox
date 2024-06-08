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
    public class SubComponent_RFLineReleaseType_Disassemble_GUI : SubComponent
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
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Disassemble Line Release Type.");
            evaluationUnit.Icon = Properties.Resources.Disassemble_LineReleases;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {
            unit.RegisterInputParam(new Param_RFEM(), "RF Line ReleaseType", "RF LineReleaseType", "Input RFLineReleaseType.", GH_ParamAccess.item);
            // unit.Inputs[0].Parameter.Optional = true;

            unit.RegisterOutputParam(new Param_Integer(), "Release Tpye Number", "No", "Index number of the RFEM object.");
            unit.RegisterOutputParam(new Param_Number(), "Displacement Dir X", "Tx", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]");
            unit.RegisterOutputParam(new Param_Number(), "Displacement Dir Y", "Ty", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]");
            unit.RegisterOutputParam(new Param_Number(), "Displacement Dir Z", "Tz", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]");
            unit.RegisterOutputParam(new Param_Number(), "Rotation Dir X", "Rx", "(-1): Fixed; (0): Free; Other: Stiffness in [kNm/rad]");
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
            var rfNodalRelease = (RFLineReleaseType)inGH.Value;

            // Output
            DA.SetData(0, rfNodalRelease.No);
            DA.SetData(1, rfNodalRelease.TranslationalConstantX<0? rfNodalRelease.TranslationalConstantX*1000: rfNodalRelease.TranslationalConstantX);
            DA.SetData(2, rfNodalRelease.TranslationalConstantY < 0 ? rfNodalRelease.TranslationalConstantY * 1000 : rfNodalRelease.TranslationalConstantY);
            DA.SetData(3, rfNodalRelease.TranslationalConstantZ < 0 ? rfNodalRelease.TranslationalConstantZ * 1000 : rfNodalRelease.TranslationalConstantZ);
            DA.SetData(4, rfNodalRelease.RotationalConstantX < 0 ? rfNodalRelease.RotationalConstantX * 1000 : rfNodalRelease.RotationalConstantX);
            DA.SetData(5, rfNodalRelease.Comment);
        }
    }
}
