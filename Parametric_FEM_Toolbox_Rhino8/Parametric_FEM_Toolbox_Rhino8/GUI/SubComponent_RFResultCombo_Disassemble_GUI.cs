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
    public class SubComponent_RFResultCombo_Disassemble_GUI : SubComponent
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
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Disassemble Result Combinations.");
            evaluationUnit.Icon = Properties.Resources.Disassemble_ResultCombination;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {
            unit.RegisterInputParam(new Param_RFEM(), "RF Result Combo", "RF ResultCombo", "Intput RFResultCombo.", GH_ParamAccess.item);
            //unit.Inputs[0].Parameter.Optional = true;

            unit.RegisterOutputParam(new Param_Integer(), "Result Combo Number", "No", "Optional index number to assign to the RFEM object.");
            unit.RegisterOutputParam(new Param_String(), "Definition", "Definition", "Definition of Result Combo.");
            unit.RegisterOutputParam(new Param_String(), "Description", "Description", "Description of Result Combo.");
            unit.RegisterOutputParam(new Param_String(), "Design Situation", "Design", "Design Situation Type.");
            unit.RegisterOutputParam(new Param_String(), "Comment", "Comment", "Comment.");
            unit.RegisterOutputParam(new Param_Boolean(), "To Solve", "Solve", "Solve Load Combo?");
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
            var rfLoadCombo = (RFResultCombo)inGH.Value;

            // Output            
            DA.SetData(0, rfLoadCombo.No);
            DA.SetData(1, rfLoadCombo.Definition);
            DA.SetData(2, rfLoadCombo.Description);
            DA.SetData(3, rfLoadCombo.DesignSituation);
            DA.SetData(4, rfLoadCombo.Comment);
            DA.SetData(5, rfLoadCombo.ToSolve);
        }
    }
}
