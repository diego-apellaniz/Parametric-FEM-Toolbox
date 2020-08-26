using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System.Collections.Generic;
using RFEM_daq.UIWidgets;
using RFEM_daq.HelperLibraries;
using RFEM_daq.Utilities;

namespace RFEM_daq.GUI
{
    public class SubComponent_Filter_LoadCombos_GUI : SubComponent
    {
        public override string name()
        {
            return "Load Combos";
        }
        public override string display_name()
        {
            return "Load Combos";
        }
        public override void registerEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Filter Load Combos.");
            evaluationUnit.Icon = RFEM_daq.Properties.Resources.icon_RFEM_FilterLoadCombo;
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
            unit.RegisterInputParam(new Param_String(), "Definition", "Definition", "Definition of Load Combo.", GH_ParamAccess.list);
            unit.Inputs[0].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Description", "Description", "Description of Load Combo.", GH_ParamAccess.list);
            unit.Inputs[1].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Design Situation", "Design", "Design Situation Type.", GH_ParamAccess.list);
            unit.Inputs[2].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "To Solve", "Solve", "Solve Load Combo?", GH_ParamAccess.list);
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
            myFilter.Type = "Filter (Load Combos)";
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
                myFilter.LCoList = lcListAll;
            }            
            if(DA.GetDataList(1, commentList))
            {
                myFilter.LCoComment = commentList;
            }
            if (DA.GetDataList(2, definitionList))
            {
                myFilter.LCoDefinition = definitionList;
            }
            if (DA.GetDataList(3, descriptionList))
            {
                myFilter.LCoDescription = descriptionList;
            }
            if (DA.GetDataList(4, designList))
            {
                myFilter.LCoDesign = designList;
            }
            if (DA.GetDataList(5, toSolveList))
            {
                myFilter.LCoToSolve = toSolveList;
            }
            DA.SetData(0, myFilter);
        }
    }
}
