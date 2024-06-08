using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System.Collections.Generic;
using Parametric_FEM_Toolbox.UIWidgets;
using Parametric_FEM_Toolbox.HelperLibraries;
using Parametric_FEM_Toolbox.Utilities;

namespace Parametric_FEM_Toolbox.GUI
{
    public class SubComponent_Filter_FreeLineLoads_GUI : SubComponent
    {
        public override string name()
        {
            return "Free Line Loads";
        }
        public override string display_name()
        {
            return "Free Line Loads";
        }
        public override void registerEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Filter free line loads.");
            evaluationUnit.Icon = Properties.Resources.icon_FilterFreeLineLoad;
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
            unit.RegisterInputParam(new Param_String(), "Load Cases", "LC", "Load Cases", GH_ParamAccess.list);
            unit.Inputs[0].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Surfaces No", "Sfcs", "Sfcs No", GH_ParamAccess.list);
            unit.Inputs[1].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "F1 [kN/m]", "F1", "F1 [kN/m]", GH_ParamAccess.list);
            unit.Inputs[2].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "F2 [kN/m]", "F2", "F2 [kN/m]", GH_ParamAccess.list);
            unit.Inputs[3].Parameter.Optional = true;
             unit.RegisterInputParam(new Param_String(), "Load Direction Type", "Dir", "Load Direction Type", GH_ParamAccess.list);
            unit.Inputs[4].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Load Distribution Type", "Dist", "Load Distribution Type", GH_ParamAccess.list);
            unit.Inputs[5].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Projection Type", "Proj", "Projection Type", GH_ParamAccess.list);
            unit.Inputs[6].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "X [m]", "X", "X Coordinate", GH_ParamAccess.list);
            unit.Inputs[7].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Y [m]", "Y", "Y Coordinate", GH_ParamAccess.list);
            unit.Inputs[8].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Z [m]", "Z", "Z Coordinate", GH_ParamAccess.list);
            unit.Inputs[9].Parameter.Optional = true;            
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[0]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[1]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[2]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[3]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[4]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[5]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[6]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[7]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[8]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[9]);
            unit.AddMenu(gH_ExtendableMenu);
        }
        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Warning;

            var myFilter = new RFFilter();
            myFilter.Type = "Filter (Free Line Loads)";
            var llList = new List<string>();            
            var commentList = new List<string>();
            var x = new List<Interval>();
            var y = new List<Interval>();
            var z = new List<Interval>();
            var f1 = new List<Interval>();
            var f2 = new List<Interval>();
            var sfcList = new List<string>();
            var lcList = new List<string>();
            var projList = new List<string>();
            var dirList = new List<string>();
            var distList = new List<string>();

            if (DA.GetDataList(0, llList))
            {
                var lineListAll = new List<int>();
                foreach (var no in llList)
                {
                    lineListAll.AddRange(no.ToInt());
                }
                myFilter.FLLList = lineListAll;
            }            
            if(DA.GetDataList(1, commentList))
            {
                myFilter.FLLComment = commentList;
            }
            if (DA.GetDataList(2, lcList))
            {
                var lcListAll = new List<int>();
                foreach (var no in lcList)
                {
                    lcListAll.AddRange(no.ToInt());
                }
                myFilter.FLLLC = lcListAll;
            }
            if (DA.GetDataList(3, sfcList))
            {
                var lineListAll = new List<int>();
                foreach (var no in sfcList)
                {
                    lineListAll.AddRange(no.ToInt());
                }
                myFilter.FLLSfcList = lineListAll;
            }            
            if (DA.GetDataList(4, f1))
            {
                myFilter.FLLF1 = f1;
            }
            if (DA.GetDataList(5, f2))
            {
                myFilter.FLLF2 = f2;
            }            
            if (DA.GetDataList(6, dirList))
            {
                myFilter.FLLDir = dirList;
            }
            if (DA.GetDataList(7, distList))
            {
                myFilter.FLLDist = distList;
            }
            if (DA.GetDataList(8, projList))
            {
                myFilter.FLLProj = projList;
            }            
            if (DA.GetDataList(9, x))
            {
                myFilter.FLLX = x;
            }
            if (DA.GetDataList(10, y))
            {
                myFilter.FLLY = y;
            }
            if (DA.GetDataList(11, z))
            {
                myFilter.FLLZ = z;
            }
            DA.SetData(0, myFilter);
        }
    }
}
