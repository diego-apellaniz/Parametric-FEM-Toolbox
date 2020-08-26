using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System.Collections.Generic;
using RFEM_daq.UIWidgets;
using RFEM_daq.HelperLibraries;
using RFEM_daq.Utilities;

namespace RFEM_daq.GUI
{
    public class SubComponent_Filter_LineLoads_GUI : SubComponent
    {
        public override string name()
        {
            return "Line Loads";
        }
        public override string display_name()
        {
            return "Line Loads";
        }
        public override void registerEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Filter line loads.");
            evaluationUnit.Icon = RFEM_daq.Properties.Resources.icon_FilterLineLoad;
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
            unit.RegisterInputParam(new Param_String(), "Lines No", "Lines", "Lines No", GH_ParamAccess.list);
            unit.Inputs[1].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "F1 [kN/m]", "F1", "F1 [kN/m]", GH_ParamAccess.list);
            unit.Inputs[2].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "F2 [kN/m]", "F2", "F2 [kN/m]", GH_ParamAccess.list);
            unit.Inputs[3].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "F3 [kN/m]", "F3", "F3 [kN/m]", GH_ParamAccess.list);
            unit.Inputs[4].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Distance A", "t1", "Distance A", GH_ParamAccess.list);
            unit.Inputs[5].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Distance B", "t2", "Distance B", GH_ParamAccess.list);
            unit.Inputs[6].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Load Type", "Type", "Load Type", GH_ParamAccess.list);
            unit.Inputs[7].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Load Direction Type", "Dir", "Load Direction Type", GH_ParamAccess.list);
            unit.Inputs[8].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Load Distribution Type", "Dist", "Load Distribution Type", GH_ParamAccess.list);
            unit.Inputs[9].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Line Load Reference Type", "RefType", "LLine Load Reference Type", GH_ParamAccess.list);
            unit.Inputs[10].Parameter.Optional = true;
             unit.RegisterInputParam(new Param_Boolean(), "Over Total Length", "Total", "Over Total Length", GH_ParamAccess.list);
            unit.Inputs[11].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Relative Distances", "Rel", "Relative Distances", GH_ParamAccess.list);
            unit.Inputs[12].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "X [m]", "X", "X Coordinate", GH_ParamAccess.list);
            unit.Inputs[13].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Y [m]", "Y", "Y Coordinate", GH_ParamAccess.list);
            unit.Inputs[14].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Z [m]", "Z", "Z Coordinate", GH_ParamAccess.list);
            unit.Inputs[15].Parameter.Optional = true;            
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
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[10]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[11]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[12]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[13]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[14]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[15]);
            unit.AddMenu(gH_ExtendableMenu);
        }
        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Warning;

            var myFilter = new RFFilter();
            myFilter.Type = "Filter (Line Loads)";
            var llList = new List<string>();            
            var commentList = new List<string>();
            var x = new List<Interval>();
            var y = new List<Interval>();
            var z = new List<Interval>();
            var f1 = new List<Interval>();
            var f2 = new List<Interval>();
            var f3 = new List<Interval>();
            var t1 = new List<Interval>();
            var t2 = new List<Interval>();
            var mz = new List<Interval>();
            var linesList = new List<string>();
            var lcList = new List<string>();
            var typeList = new List<string>();
            var dirList = new List<string>();
            var distList = new List<string>();
            var refList = new List<string>();
            var overtotallength = new List<bool>();
            var relativedistances = new List<bool>();

            if (DA.GetDataList(0, llList))
            {
                var lineListAll = new List<int>();
                foreach (var no in llList)
                {
                    lineListAll.AddRange(no.ToInt());
                }
                myFilter.LLList = lineListAll;
            }            
            if(DA.GetDataList(1, commentList))
            {
                myFilter.LLComment = commentList;
            }
            if (DA.GetDataList(2, lcList))
            {
                var lcListAll = new List<int>();
                foreach (var no in lcList)
                {
                    lcListAll.AddRange(no.ToInt());
                }
                myFilter.LLLC = lcListAll;
            }
            if (DA.GetDataList(3, linesList))
            {
                var lineListAll = new List<int>();
                foreach (var no in linesList)
                {
                    lineListAll.AddRange(no.ToInt());
                }
                myFilter.LLLineList = lineListAll;
            }            
            if (DA.GetDataList(4, f1))
            {
                myFilter.LLF1 = f1;
            }
            if (DA.GetDataList(5, f2))
            {
                myFilter.LLF2 = f2;
            }
            if (DA.GetDataList(6, f3))
            {
                myFilter.LLF3 = f3;
            }
            if (DA.GetDataList(7, t1))
            {
                myFilter.LLt1 = t1;
            }
            if (DA.GetDataList(8, t2))
            {
                myFilter.LLt2 = t2;
            }
            if (DA.GetDataList(9, typeList))
            {
                myFilter.LLType = typeList;
            }
            if (DA.GetDataList(10, dirList))
            {
                myFilter.LLDir = dirList;
            }
            if (DA.GetDataList(11, distList))
            {
                myFilter.LLDist = distList;
            }
            if (DA.GetDataList(12, refList))
            {
                myFilter.LLRef = refList;
            }
            if (DA.GetDataList(13, overtotallength))
            {
                myFilter.LLTotalLength = overtotallength;
            }
            if (DA.GetDataList(14, relativedistances))
            {
                myFilter.LLRelativeDistances = relativedistances;
            }
            if (DA.GetDataList(15, x))
            {
                myFilter.LLX = x;
            }
            if (DA.GetDataList(16, y))
            {
                myFilter.LLY = y;
            }
            if (DA.GetDataList(17, z))
            {
                myFilter.LLZ = z;
            }
            DA.SetData(0, myFilter);
        }
    }
}
