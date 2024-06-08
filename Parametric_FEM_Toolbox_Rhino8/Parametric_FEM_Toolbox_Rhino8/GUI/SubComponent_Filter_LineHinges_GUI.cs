﻿using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System.Collections.Generic;
using Parametric_FEM_Toolbox.UIWidgets;
using Parametric_FEM_Toolbox.HelperLibraries;
using Parametric_FEM_Toolbox.Utilities;

namespace Parametric_FEM_Toolbox.GUI
{
    public class SubComponent_Filter_LineHinges_GUI : SubComponent
    {
        public override string name()
        {
            return "Line Hinges";
        }
        public override string display_name()
        {
            return "Line Hinges";
        }
        public override void registerEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Filter line hinges.");
            evaluationUnit.Icon = Properties.Resources.icon_RFEM_FilterLineHinge;
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
            unit.RegisterInputParam(new Param_String(), "Lines No", "Lines", "Lines No", GH_ParamAccess.list);
            unit.Inputs[3].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Sfcs No", "Sfcs", "Surfaces No", GH_ParamAccess.list);
            unit.Inputs[4].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(),"Displacement Dir X", "Tx", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]", GH_ParamAccess.list);
            unit.Inputs[5].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Displacement Dir Y", "Ty", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]", GH_ParamAccess.list);
            unit.Inputs[6].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Displacement Dir Z", "Tz", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]", GH_ParamAccess.list);
            unit.Inputs[7].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Rotation Dir X", "Rx", "(-1): Fixed; (0): Free; Other: Stiffness in [kNm/rad]", GH_ParamAccess.list);
            unit.Inputs[8].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Rotation Dir Y", "Ry", "(-1): Fixed; (0): Free; Other: Stiffness in [kNm/rad]", GH_ParamAccess.list);
            unit.Inputs[9].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Rotation Dir Z", "Rz", "(-1): Fixed; (0): Free; Other: Stiffness in [kNm/rad]", GH_ParamAccess.list);
            unit.Inputs[10].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Side", "Side", "Hinge Side Type", GH_ParamAccess.list);
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
            myFilter.Type = "Filter (Line Hinges)";
            var supList = new List<string>();            
            var commentList = new List<string>();
            var x = new List<Interval>();
            var y = new List<Interval>();
            var z = new List<Interval>();
            var tx = new List<Interval>();
            var ty = new List<Interval>();
            var tz = new List<Interval>();
            var rx = new List<Interval>();
            var ry = new List<Interval>();
            var rz = new List<Interval>();
            var linesList = new List<string>();
            var sfcsList = new List<string>();
            var sides = new List<string>();

            if (DA.GetDataList(0, supList))
            {
                var lineListAll = new List<int>();
                foreach (var no in supList)
                {
                    lineListAll.AddRange(no.ToInt());
                }
                myFilter.LHList = lineListAll;
            }            
            if(DA.GetDataList(1, commentList))
            {
                myFilter.LHComment = commentList;
            }
            if(DA.GetDataList(2, x))
            {
                myFilter.LHX = x;
            }
            if (DA.GetDataList(3, y))
            {
                myFilter.LHY = y;
            }
            if (DA.GetDataList(4, z))
            {
                myFilter.LHZ = z;
            }
            if (DA.GetDataList(5, linesList))
            {
                var lineListAll = new List<int>();
                foreach (var no in linesList)
                {
                    lineListAll.AddRange(no.ToInt());
                }
                myFilter.LHLineNo = lineListAll;
            }
            if (DA.GetDataList(6, sfcsList))
            {
                var lineListAll = new List<int>();
                foreach (var no in sfcsList)
                {
                    lineListAll.AddRange(no.ToInt());
                }
                myFilter.LHSfcNo = lineListAll;
            }
            if (DA.GetDataList(7, tx))
            {
                myFilter.LHTx = tx;
            }
            if (DA.GetDataList(8, ty))
            {
                myFilter.LHTy = ty;
            }
            if (DA.GetDataList(9, tz))
            {
                myFilter.LHTz = tz;
            }
            if (DA.GetDataList(10, rx))
            {
                myFilter.LHRx = rx;
            }
            if (DA.GetDataList(11, ry))
            {
                myFilter.LHRy = ry;
            }
            if (DA.GetDataList(12, rz))
            {
                myFilter.LHRz = rz;
            }
            if (DA.GetDataList(13, sides))
            {
                myFilter.LHSide = sides;
            }
            DA.SetData(0, myFilter);
        }
    }
}
