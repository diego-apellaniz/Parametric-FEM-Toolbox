using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using Parametric_FEM_Toolbox.UIWidgets;
using Parametric_FEM_Toolbox.Utilities;
using Parametric_FEM_Toolbox.HelperLibraries;

namespace Parametric_FEM_Toolbox.GUI
{
    public class SubComponent_Filter_Lines_GUI : SubComponent
    {
        public override string name()
        {
            return "Lines";
        }
        public override string display_name()
        {
            return "Lines";
        }
        public override void registerEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Filter lines.");
            evaluationUnit.Icon = Properties.Resources.icon_FilterLine;
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
            unit.RegisterInputParam(new Param_String(), "Line Type", "L Type", "Line Type", GH_ParamAccess.list);
            unit.Inputs[3].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Rotation Type", "Rot Type", "Rotation Type", GH_ParamAccess.list);
            unit.Inputs[4].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Rotation Angle [°]", "β", "Rotation Angle", GH_ParamAccess.list);
            unit.Inputs[5].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Nodes No", "Nodes", "Nodes No", GH_ParamAccess.list);
            unit.Inputs[6].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Length [m]", "L", "Line Length", GH_ParamAccess.list);
            unit.Inputs[7].Parameter.Optional = true;
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[0]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[1]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[2]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[3]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[4]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[5]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[6]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[7]);
            unit.AddMenu(gH_ExtendableMenu);
        }
        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Warning;
            var myFilter = new RFFilter();
            myFilter.Type = "Filter (Lines)";

            var lineList = new List<string>();            
            var commentList = new List<string>();
            var x = new List<Interval>();
            var y = new List<Interval>();
            var z = new List<Interval>();
            var rotType = new List<string>();
            var rotAngle = new List<Interval>();
            var typeList = new List<string>();
            var nodesList = new List<string>();
            var lineLength = new List<Interval>();

            if (DA.GetDataList(0, lineList))
            {
                var lineListAll = new List<int>();
                foreach (var no in lineList)
                {
                    lineListAll.AddRange(no.ToInt());
                }
                myFilter.LineList = lineListAll;                
            }
            if(DA.GetDataList(1, commentList))
            {
                myFilter.LineComment = commentList;
            }
            if (DA.GetDataList(2, x))
            {
                myFilter.LinesX = x;
            }
            if (DA.GetDataList(3, y))
            {
                myFilter.LinesY = y;
            }
            if (DA.GetDataList(4, z))
            {
                myFilter.LinesZ = z;
            }
            if (DA.GetDataList(5, typeList))
            {
                myFilter.LineType = typeList;
            }
            if (DA.GetDataList(6, rotType))
            {
                myFilter.LineRotType = rotType;
            }
            if (DA.GetDataList(7, rotAngle))
            {
                myFilter.LineRotAngle = rotAngle;
            }
            if (DA.GetDataList(8, nodesList))
            {
                var lineListAll = new List<int>();
                foreach (var no in nodesList)
                {
                    lineListAll.AddRange(no.ToInt());
                }
                myFilter.LineNoList = lineListAll;
            }
            if (DA.GetDataList(9, lineLength))
            {
                myFilter.LineLength = lineLength;
            }

            DA.SetData(0, myFilter);
        }
    }
}
