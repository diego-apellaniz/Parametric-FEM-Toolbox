using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System.Collections.Generic;
using Parametric_FEM_Toolbox.UIWidgets;
using Parametric_FEM_Toolbox.HelperLibraries;
using Parametric_FEM_Toolbox.Utilities;

namespace Parametric_FEM_Toolbox.GUI
{
    public class SubComponent_Filter_MemberLoads_GUI : SubComponent
    {
        public override string name()
        {
            return "Member Loads";
        }
        public override string display_name()
        {
            return "Member Loads";
        }
        public override void registerEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Filter member loads.");
            evaluationUnit.Icon = Properties.Resources.icon_FilterMemberLoad;
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
            unit.RegisterInputParam(new Param_String(), "Members No", "Members", "Members No", GH_ParamAccess.list);
            unit.Inputs[1].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "F1 [kN/m]", "F1", "F1 [kN/m]", GH_ParamAccess.list);
            unit.Inputs[2].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "F2 [kN/m]", "F2", "F2 [kN/m]", GH_ParamAccess.list);
            unit.Inputs[3].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "F3 [kN/m]", "F3", "F3 [kN/m]", GH_ParamAccess.list);
            unit.Inputs[4].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "T4 [°C]", "T4", "T4 [°C]", GH_ParamAccess.list);
            unit.Inputs[5].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "T5 [°C]", "T5", "T5 [°C]", GH_ParamAccess.list);
            unit.Inputs[6].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "T6 [°C]", "T6", "T6 [°C]", GH_ParamAccess.list);
            unit.Inputs[7].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Distance A", "t1", "Distance A", GH_ParamAccess.list);
            unit.Inputs[8].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Distance B", "t2", "Distance B", GH_ParamAccess.list);
            unit.Inputs[9].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Load Type", "Type", "Load Type", GH_ParamAccess.list);
            unit.Inputs[10].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Load Direction Type", "Dir", "Load Direction Type", GH_ParamAccess.list);
            unit.Inputs[11].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Load Distribution Type", "Dist", "Load Distribution Type", GH_ParamAccess.list);
            unit.Inputs[12].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Line Load Reference Type", "RefType", "LLine Load Reference Type", GH_ParamAccess.list);
            unit.Inputs[13].Parameter.Optional = true;
             unit.RegisterInputParam(new Param_Boolean(), "Over Total Length", "Total", "Over Total Length", GH_ParamAccess.list);
            unit.Inputs[14].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Relative Distances", "Rel", "Relative Distances", GH_ParamAccess.list);
            unit.Inputs[15].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "X [m]", "X", "X Coordinate", GH_ParamAccess.list);
            unit.Inputs[16].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Y [m]", "Y", "Y Coordinate", GH_ParamAccess.list);
            unit.Inputs[17].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Z [m]", "Z", "Z Coordinate", GH_ParamAccess.list);
            unit.Inputs[18].Parameter.Optional = true;            
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
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[16]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[17]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[18]);
            unit.AddMenu(gH_ExtendableMenu);
        }
        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Warning;

            var myFilter = new RFFilter();
            myFilter.Type = "Filter (Member Loads)";
            var mlList = new List<string>();            
            var commentList = new List<string>();
            var x = new List<Interval>();
            var y = new List<Interval>();
            var z = new List<Interval>();
            var f1 = new List<Interval>();
            var f2 = new List<Interval>();
            var f3 = new List<Interval>();
            var f4 = new List<Interval>();
            var f5 = new List<Interval>();
            var f6 = new List<Interval>();
            var t1 = new List<Interval>();
            var t2 = new List<Interval>();
            var mz = new List<Interval>();
            var memberList = new List<string>();
            var lcList = new List<string>();
            var typeList = new List<string>();
            var dirList = new List<string>();
            var distList = new List<string>();
            var refList = new List<string>();
            var overtotallength = new List<bool>();
            var relativedistances = new List<bool>();

            if (DA.GetDataList(0, mlList))
            {
                var memberListAll = new List<int>();
                foreach (var no in mlList)
                {
                    memberListAll.AddRange(no.ToInt());
                }
                myFilter.MLList = memberListAll;
            }            
            if(DA.GetDataList(1, commentList))
            {
                myFilter.MLComment = commentList;
            }
            if (DA.GetDataList(2, lcList))
            {
                var lcListAll = new List<int>();
                foreach (var no in lcList)
                {
                    lcListAll.AddRange(no.ToInt());
                }
                myFilter.MLLC = lcListAll;
            }
            if (DA.GetDataList(3, memberList))
            {
                var memberListAll = new List<int>();
                foreach (var no in memberList)
                {
                    memberListAll.AddRange(no.ToInt());
                }
                myFilter.MLMemberList = memberListAll;
            }            
            if (DA.GetDataList(4, f1))
            {
                myFilter.MLF1 = f1;
            }
            if (DA.GetDataList(5, f2))
            {
                myFilter.MLF2 = f2;
            }
            if (DA.GetDataList(6, f3))
            {
                myFilter.MLF3 = f3;
            }
            if (DA.GetDataList(7, f4))
            {
                myFilter.MLT4 = f3;
            }
            if (DA.GetDataList(8, f5))
            {
                myFilter.MLT5 = f3;
            }
            if (DA.GetDataList(9, f6))
            {
                myFilter.MLT6 = f3;
            }
            if (DA.GetDataList(10, t1))
            {
                myFilter.MLt1 = t1;
            }
            if (DA.GetDataList(11, t2))
            {
                myFilter.MLt2 = t2;
            }
            if (DA.GetDataList(12, typeList))
            {
                myFilter.MLType = typeList;
            }
            if (DA.GetDataList(13, dirList))
            {
                myFilter.MLDir = dirList;
            }
            if (DA.GetDataList(14, distList))
            {
                myFilter.MLDist = distList;
            }
            if (DA.GetDataList(15, refList))
            {
                myFilter.MLRef = refList;
            }
            if (DA.GetDataList(16, overtotallength))
            {
                myFilter.MLTotalLength = overtotallength;
            }
            if (DA.GetDataList(17, relativedistances))
            {
                myFilter.MLRelativeDistances = relativedistances;
            }
            if (DA.GetDataList(18, x))
            {
                myFilter.MLX = x;
            }
            if (DA.GetDataList(19, y))
            {
                myFilter.MLY = y;
            }
            if (DA.GetDataList(20, z))
            {
                myFilter.MLZ = z;
            }
            DA.SetData(0, myFilter);
        }
    }
}
