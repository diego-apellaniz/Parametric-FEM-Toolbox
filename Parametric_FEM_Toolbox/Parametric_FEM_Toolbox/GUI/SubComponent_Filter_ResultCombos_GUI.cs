using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System.Collections.Generic;
using Parametric_FEM_Toolbox.UIWidgets;
using Parametric_FEM_Toolbox.HelperLibraries;
using Parametric_FEM_Toolbox.Utilities;

namespace Parametric_FEM_Toolbox.GUI
{
    public class SubComponent_Filter_ResultCombos_GUI : SubComponent
    {
        public override string name()
        {
            return "Result Combos";
        }
        public override string display_name()
        {
            return "Result Combos";
        }
        public override void registerEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Filter Result Combos.");
            evaluationUnit.Icon = Properties.Resources.icon_RFEM_FilterResultCombination;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {
            GH_ExtendableMenu gH_ExtendableMenu = new GH_ExtendableMenu(0, "menu_settings");
            gH_ExtendableMenu.Name = "Advanced Options";
            gH_ExtendableMenu.Collapse();
            unit.RegisterInputParam(new Param_String(), "Definition", "Definition", "Definition of Result Combo.", GH_ParamAccess.list);
            unit.Inputs[0].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Description", "Description", "Description of Result Combo.", GH_ParamAccess.list);
            unit.Inputs[1].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Design Situation", "Design", "Design Situation Type.", GH_ParamAccess.list);
            unit.Inputs[2].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "To Solve", "Solve", "Solve Result Combo?", GH_ParamAccess.list);
            unit.Inputs[3].Parameter.Optional = true;
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[0]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[1]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[2]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[3]);
            unit.AddMenu(gH_ExtendableMenu);
        }
        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Warning;

            var myFilter = new RFFilter();
            myFilter.Type = "Filter (Result Combos)";
            var lcList = new List<string>();            
            var commentList = new List<string>();
            var descriptionList = new List<string>();
            var definitionList = new List<string>();
            var designList = new List<string>();
            var toSolveList = new List<bool>();

            if (DA.GetDataList(0, lcList))
            {
                var lcListAll = new List<int>();
                foreach (var no in lcList)
                {
                    lcListAll.AddRange(no.ToInt());
                }
                myFilter.RCoList = lcListAll;
            }            
            if(DA.GetDataList(1, commentList))
            {
                myFilter.RCoComment = commentList;
            }
            if (DA.GetDataList(2, definitionList))
            {
                myFilter.RCoDefinition = definitionList;
            }
            if (DA.GetDataList(3, descriptionList))
            {
                myFilter.RCoDescription = descriptionList;
            }
            if (DA.GetDataList(4, designList))
            {
                myFilter.RCoDesign = designList;
            }
            if (DA.GetDataList(5, toSolveList))
            {
                myFilter.RCoToSolve = toSolveList;
            }
            DA.SetData(0, myFilter);
        }
    }
}
