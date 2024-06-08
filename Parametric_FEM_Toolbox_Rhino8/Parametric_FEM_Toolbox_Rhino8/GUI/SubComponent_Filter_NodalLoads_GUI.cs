using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System.Collections.Generic;
using Parametric_FEM_Toolbox.UIWidgets;
using Parametric_FEM_Toolbox.HelperLibraries;
using Parametric_FEM_Toolbox.Utilities;

namespace Parametric_FEM_Toolbox.GUI
{
    public class SubComponent_Filter_NodalLoads_GUI : SubComponent
    {
        public override string name()
        {
            return "Nodal Loads";
        }
        public override string display_name()
        {
            return "Nodal Loads";
        }
        public override void registerEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Filter nodal loads.");
            evaluationUnit.Icon = Properties.Resources.icon_FilterNodalLoad;
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
            unit.RegisterInputParam(new Param_String(), "Nodes No", "Nodes", "Nodes No", GH_ParamAccess.list);
            unit.Inputs[1].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Load Definition", "Def", "Load Definition", GH_ParamAccess.list);
            unit.Inputs[2].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Fx [kN]", "Fx", "Fx [kN]", GH_ParamAccess.list);
            unit.Inputs[3].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Fy [kN]", "Fy", "Fy [kN]", GH_ParamAccess.list);
            unit.Inputs[4].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Fz [kN]", "Fz", "Fz [kN]", GH_ParamAccess.list);
            unit.Inputs[5].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Mx [kN]", "Mx", "Mx [kN]", GH_ParamAccess.list);
            unit.Inputs[6].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "My [kN]", "My", "My [kN]", GH_ParamAccess.list);
            unit.Inputs[7].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Mz [kN]", "Mz", "Mz [kN]", GH_ParamAccess.list);
            unit.Inputs[8].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "X [m]", "X", "X Coordinate", GH_ParamAccess.list);
            unit.Inputs[9].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Y [m]", "Y", "Y Coordinate", GH_ParamAccess.list);
            unit.Inputs[10].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Z [m]", "Z", "Z Coordinate", GH_ParamAccess.list);
            unit.Inputs[11].Parameter.Optional = true;            
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
            unit.AddMenu(gH_ExtendableMenu);
        }
        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Warning;

            var myFilter = new RFFilter();
            myFilter.Type = "Filter (Nodal Loads)";
            var nlList = new List<string>();            
            var commentList = new List<string>();
            var x = new List<Interval>();
            var y = new List<Interval>();
            var z = new List<Interval>();
            var fx = new List<Interval>();
            var fy = new List<Interval>();
            var fz = new List<Interval>();
            var mx = new List<Interval>();
            var my = new List<Interval>();
            var mz = new List<Interval>();
            var nodesList = new List<string>();
            var lcList = new List<string>();
            var defList = new List<string>();

            if (DA.GetDataList(0, nlList))
            {
                var nodeListAll = new List<int>();
                foreach (var no in nlList)
                {
                    nodeListAll.AddRange(no.ToInt());
                }
                myFilter.NLList = nodeListAll;
            }            
            if(DA.GetDataList(1, commentList))
            {
                myFilter.NLComment = commentList;
            }
            if (DA.GetDataList(2, lcList))
            {
                var nodeListAll = new List<int>();
                foreach (var no in lcList)
                {
                    nodeListAll.AddRange(no.ToInt());
                }
                myFilter.NLLC = nodeListAll;
            }
            if (DA.GetDataList(3, nodesList))
            {
                var nodeListAll = new List<int>();
                foreach (var no in nodesList)
                {
                    nodeListAll.AddRange(no.ToInt());
                }
                myFilter.NLNodeList = nodeListAll;
            }
            if (DA.GetDataList(4, defList))
            {
                myFilter.NLDefinition = defList;
            }
            if (DA.GetDataList(5, fx))
            {
                myFilter.NLFx = fx;
            }
            if (DA.GetDataList(6, fy))
            {
                myFilter.NLFy = fy;
            }
            if (DA.GetDataList(7, fz))
            {
                myFilter.NLFz = fz;
            }
            if (DA.GetDataList(8, mx))
            {
                myFilter.NLMx = mx;
            }
            if (DA.GetDataList(9, my))
            {
                myFilter.NLMy = my;
            }
            if (DA.GetDataList(10, mz))
            {
                myFilter.NLMz = mz;
            }
            if (DA.GetDataList(11, x))
            {
                myFilter.NLX = x;
            }
            if (DA.GetDataList(12, y))
            {
                myFilter.NLY = y;
            }
            if (DA.GetDataList(13, z))
            {
                myFilter.NLZ = z;
            }
            DA.SetData(0, myFilter);
        }
    }
}
