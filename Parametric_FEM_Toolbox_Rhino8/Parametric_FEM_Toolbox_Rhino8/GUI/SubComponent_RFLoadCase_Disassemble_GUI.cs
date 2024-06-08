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
    public class SubComponent_RFLoadCase_Disassemble_GUI : SubComponent
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
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Disassemble Load Cases.");
            evaluationUnit.Icon = Properties.Resources.Disassemble_LoadCase;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {
            unit.RegisterInputParam(new Param_RFEM(), "RF Load Case", "RF LoadCase", "Intput RFLoadCase.", GH_ParamAccess.item);
            //unit.Inputs[0].Parameter.Optional = true;

            unit.RegisterOutputParam(new Param_Integer(), "Load Case Number", "No", "Optional index number to assign to the RFEM object.");
            unit.RegisterOutputParam(new Param_String(), "Description", "Description", "Description of Load Case.");
            unit.RegisterOutputParam(new Param_String(), "Action Category", "Action", "Action Category Type.");
            unit.RegisterOutputParam(new Param_String(), "Comment", "Comment", "Comment.");
            unit.RegisterOutputParam(new Param_Vector(),"Self Weight Factor", "Self Weight", "Self Weight Factor.");
            unit.RegisterOutputParam(new Param_Boolean(), "To Solve", "Solve", "Solve Load Case?");
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
            var rfLoadCase = (RFLoadCase)inGH.Value;

            // Output            
            DA.SetData(0, rfLoadCase.No);
            DA.SetData(1, rfLoadCase.Description);
            DA.SetData(2, rfLoadCase.ActionCategory);
            DA.SetData(3, rfLoadCase.Comment);
            DA.SetData(4, rfLoadCase.SelfWeightFactor);
            DA.SetData(5, rfLoadCase.ToSolve);
        }
    }
}
