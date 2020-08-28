using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System.Collections.Generic;
using Parametric_FEM_Toolbox.UIWidgets;
using Parametric_FEM_Toolbox.HelperLibraries;
using Parametric_FEM_Toolbox.Utilities;

namespace Parametric_FEM_Toolbox.GUI
{
    public class SubComponent_Filter_LoadCases_GUI : SubComponent
    {
        public override string name()
        {
            return "Load Cases";
        }
        public override string display_name()
        {
            return "Load Cases";
        }
        public override void registerEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Filter Load Cases.");
            evaluationUnit.Icon = Properties.Resources.icon_RFEM_FilterLoadCase;
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
            unit.RegisterInputParam(new Param_String(), "Description", "Description", "Description of Load Case.", GH_ParamAccess.list);
            unit.Inputs[0].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Action Category", "Action", "Action Category Type.", GH_ParamAccess.list);
            unit.Inputs[1].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Self Weight Factor X", "SWx", "Self Weight Factor in X direction.", GH_ParamAccess.list);
            unit.Inputs[2].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Self Weight Factor Y", "SWy", "Self Weight Factor in Y direction.", GH_ParamAccess.list);
            unit.Inputs[3].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Self Weight Factor Z", "SWz", "Self Weight Factor in Z direction.", GH_ParamAccess.list);
            unit.Inputs[4].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "To Solve", "Solve", "Solve Load Case?", GH_ParamAccess.list);
            unit.Inputs[5].Parameter.Optional = true;
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[0]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[1]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[2]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[3]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[4]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[5]);
            unit.AddMenu(gH_ExtendableMenu);
        }
        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Warning;

            var myFilter = new RFFilter();
            myFilter.Type = "Filter (Load Cases)";
            var lcList = new List<string>();            
            var commentList = new List<string>();
            var swx = new List<Interval>();
            var swy = new List<Interval>();
            var swz = new List<Interval>();
            var descriptionList = new List<string>();
            var actionList = new List<string>();
            var toSolveList = new List<bool>();

            if (DA.GetDataList(0, lcList))
            {
                var lcListAll = new List<int>();
                foreach (var no in lcList)
                {
                    lcListAll.AddRange(no.ToInt());
                }
                myFilter.LCList = lcListAll;
            }            
            if(DA.GetDataList(1, commentList))
            {
                myFilter.LCComment = commentList;
            }
            if (DA.GetDataList(2, descriptionList))
            {
                myFilter.LCDescription = descriptionList;
            }
            if (DA.GetDataList(3, actionList))
            {
                myFilter.LCAction = actionList;
            }
            if (DA.GetDataList(4, swx))
            {
                myFilter.LCSWX = swx;
            }
            if (DA.GetDataList(5, swy))
            {
                myFilter.LCSWY = swy;
            }
            if (DA.GetDataList(6, swz))
            {
                myFilter.LCSWZ = swz;
            }
            if (DA.GetDataList(7, toSolveList))
            {
                myFilter.LCToSolve = toSolveList;
            }
            DA.SetData(0, myFilter);
        }
    }
}
