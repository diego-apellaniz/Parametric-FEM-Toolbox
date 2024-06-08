using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System.Collections.Generic;
using Parametric_FEM_Toolbox.UIWidgets;
using Parametric_FEM_Toolbox.HelperLibraries;
using Parametric_FEM_Toolbox.Utilities;

namespace Parametric_FEM_Toolbox.GUI
{
    public class SubComponent_Filter_Nodes_GUI : SubComponent
    {
        public override string name()
        {
            return "Nodes";
        }
        public override string display_name()
        {
            return "Nodes";
        }
        public override void registerEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Filter nodes.");
            evaluationUnit.Icon = Properties.Resources.icon_FilterNode;
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
            unit.RegisterInputParam(new Param_Interval(), "X [m]", "X", "X Coordinate", GH_ParamAccess.list);
            unit.Inputs[0].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Y [m]", "Y", "Y Coordinate", GH_ParamAccess.list);
            unit.Inputs[1].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Z [m]", "Z", "Z Coordinate", GH_ParamAccess.list);
            unit.Inputs[2].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Coordinate System", "CSys", "Coordinate System", GH_ParamAccess.list);
            unit.Inputs[3].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Reference Node", "Ref Node", "Reference Node", GH_ParamAccess.list);
            unit.Inputs[4].Parameter.Optional = true;
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[0]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[1]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[2]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[3]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[4]);
            unit.AddMenu(gH_ExtendableMenu);
        }
        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Warning;

            var myFilter = new RFFilter();
            myFilter.Type = "Filter (Nodes)";
            var nodeList = new List<string>();            
            var commentList = new List<string>();
            var x = new List<Interval>();
            var y = new List<Interval>();
            var z = new List<Interval>();
            var csys = new List<string>();
            var noRef = new List<string>();

            if (DA.GetDataList(0, nodeList))
            {
                var nodeListAll = new List<int>();
                foreach (var no in nodeList)
                {
                    nodeListAll.AddRange(no.ToInt());
                }
                myFilter.NodeList = nodeListAll;
            }            
            if(DA.GetDataList(1, commentList))
            {
                myFilter.NodeComment = commentList;
            }
            if(DA.GetDataList(2, x))
            {
                myFilter.NodesX = x;
            }
            if (DA.GetDataList(3, y))
            {
                myFilter.NodesY = y;
            }
            if (DA.GetDataList(4, z))
            {
                myFilter.NodesZ = z;
            }
            if (DA.GetDataList(5, csys))
            {
                myFilter.NodeCS = csys;
            }
            if (DA.GetDataList(6, noRef))
            {
                var noRefAll = new List<int>();
                foreach (var no in noRef)
                {
                    noRefAll.AddRange(no.ToInt());
                }
                myFilter.NodeRef = noRefAll;
            }

            DA.SetData(0, myFilter);
        }
    }
}
