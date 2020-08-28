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
    public class SubComponent_Filter_Openings_GUI : SubComponent
    {
        public override string name()
        {
            return "Openings";
        }
        public override string display_name()
        {
            return "Openings";
        }
        public override void registerEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Filter openings.");
            evaluationUnit.Icon = Properties.Resources.icon_RFEM_FilterOpening;
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
            unit.RegisterInputParam(new Param_String(), "Boundary Lines", "Bound", "Number of Boundary Lines", GH_ParamAccess.list);
            unit.Inputs[3].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "In Surface No", "Srfc", "In Surface No", GH_ParamAccess.list);
            unit.Inputs[4].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Area [m²]", "A", "Opening Area", GH_ParamAccess.list);
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
            myFilter.Type = "Filter (Openings)";

            var opList = new List<string>();
            var opListAll = new List<int>();
            var commentList = new List<string>();
            var x = new List<Interval>();
            var y = new List<Interval>();
            var z = new List<Interval>();
            var boundLine = new List<string>();
            var boundLineAll = new List<int>();
            var inSrfc = new List<string>();
            var inSrfcAll = new List<int>();
            var opArea = new List<Interval>();

            if (DA.GetDataList(0, opList))
            {
                foreach (var no in opList)
                {
                    opListAll.AddRange(no.ToInt());
                }
                myFilter.OpList = opListAll;
            }
            if (DA.GetDataList(1, commentList))
            {
                myFilter.OpComment = commentList;
            }
            if (DA.GetDataList(2, x))
            {
                myFilter.OpX = x;
            }
            if (DA.GetDataList(3, y))
            {
                myFilter.OpY = y;
            }
            if (DA.GetDataList(4, z))
            {
                myFilter.OpZ = z;
            }
            if (DA.GetDataList(5, boundLine))
            {
                foreach (var no in boundLine)
                {
                    boundLineAll.AddRange(no.ToInt());
                }
                myFilter.OpBoundLineNo = boundLineAll;
            }
            if (DA.GetDataList(6, inSrfc))
            {
                foreach (var no in inSrfc)
                {
                    inSrfcAll.AddRange(no.ToInt());
                }
                myFilter.OpSrfcNo = inSrfcAll;
            }
            if (DA.GetDataList(7, opArea))
            {
                myFilter.OpArea = opArea;
            }

            DA.SetData(0, myFilter);
        }
    }
}
