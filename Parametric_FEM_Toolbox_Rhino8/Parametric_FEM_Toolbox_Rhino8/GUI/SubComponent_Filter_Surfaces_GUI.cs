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
    public class SubComponent_Filter_Surfaces_GUI : SubComponent
    {
        public override string name()
        {
            return "Surfaces";
        }
        public override string display_name()
        {
            return "Surfaces";
        }
        public override void registerEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Filter surfaces.");
            evaluationUnit.Icon = Properties.Resources.icon_RFEM_FilterSurface;
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
            unit.RegisterInputParam(new Param_String(), "Surface Type", "Type", "Surface Type", GH_ParamAccess.list);
            unit.Inputs[3].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Stiffness Type", "Stiff", "Stiffness Type", GH_ParamAccess.list);
            unit.Inputs[4].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Boundary Lines", "Bound", "Number of Boundary Lines", GH_ParamAccess.list);
            unit.Inputs[5].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Material No", "Mat", "Number of Material", GH_ParamAccess.list);
            unit.Inputs[6].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Thickness Type", "Thick", "Thickness Type", GH_ParamAccess.list);
            unit.Inputs[7].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Thickness [mm]", "d", "Thickness Type", GH_ParamAccess.list);
            unit.Inputs[8].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Eccentricity [mm]", "Ecc", "Eccentricity", GH_ParamAccess.list);
            unit.Inputs[9].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Integrated Nodes No", "I Nodes", "Number of Integrated Nodes", GH_ParamAccess.list);
            unit.Inputs[10].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Integrated Lines No", "I Lines", "Number of Integrated Lines", GH_ParamAccess.list);
            unit.Inputs[11].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Integrated Openings No", "I Op", "Number of Integrated Openings", GH_ParamAccess.list);
            unit.Inputs[12].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Area [m²]", "A", "Surface Area", GH_ParamAccess.list);
            unit.Inputs[13].Parameter.Optional = true;
            //unit.RegisterInputParam(new Param_Interval(), "Weight [kg]", "W", "Surface Weight", GH_ParamAccess.list);
            //unit.Inputs[14].Parameter.Optional = true;
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
            //gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[14]);
            unit.AddMenu(gH_ExtendableMenu);
        }
        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Warning;

            var myFilter = new RFFilter();
            myFilter.Type = "Filter (Surfaces)";
            var srfcList = new List<string>();
            var srfcListAll = new List<int>();
            var commentList = new List<string>();
            var x = new List<Interval>();
            var y = new List<Interval>();
            var z = new List<Interval>();
            var srfType = new List<string>();
            var thickType = new List<string>();
            var stiffType = new List<string>();
            var boundLine = new List<string>();
            //var boundLineAll = new List<int>();
            var srfcEcc = new List<Interval>();
            var srfcThickness = new List<Interval>();
            var matNo = new List<string>();
            var matNoAll = new List<int>();
            var intNodes = new List<string>();
            var intLines = new List<string>();
            var intOps = new List<string>();
            var srfcArea = new List<Interval>();
            var srfcWeight = new List<Interval>();

            if (DA.GetDataList(0, srfcList))
            {
                foreach (var no in srfcList)
                {
                    srfcListAll.AddRange(no.ToInt());
                }
                myFilter.SrfcList = srfcListAll;
            }
            if (DA.GetDataList(1, commentList))
            {
                myFilter.SrfcComment = commentList;
            }
            if (DA.GetDataList(2, x))
            {
                myFilter.SrfcX = x;
            }
            if (DA.GetDataList(3, y))
            {
                myFilter.SrfcY = y;
            }
            if (DA.GetDataList(4, z))
            {
                myFilter.SrfcZ = z;
            }
            if (DA.GetDataList(5, srfType))
            {
                myFilter.SrfcType = srfType;
            }
            if (DA.GetDataList(6, stiffType))
            {
                myFilter.SrfcStiffType = stiffType;
            }
            if (DA.GetDataList(7, boundLine))
            {
                var boundLineAll = new List<int>();
                foreach (var no in boundLine)
                {
                    boundLineAll.AddRange(no.ToInt());
                }
                myFilter.SrfcBoundLineNo = boundLineAll;
            }
            if (DA.GetDataList(8, matNo))
            {
                foreach (var no in matNo)
                {
                    matNoAll.AddRange(no.ToInt());
                }
                myFilter.SrfcMaterial = matNoAll;
            }
            if (DA.GetDataList(9, thickType))
            {
                myFilter.ThickType = thickType;
            }
            if (DA.GetDataList(10, srfcThickness))
            {
                myFilter.SrfcThickness = srfcThickness;
            }
            if (DA.GetDataList(11, srfcEcc))
            {
                myFilter.SrfcEcc = srfcEcc;
            }
            if (DA.GetDataList(12, intNodes))
            {
                myFilter.SrfcIntNodeNo = intNodes;
            }
            if (DA.GetDataList(13, intLines))
            {
                myFilter.SrfcIntLineNo = intLines;
            }
            if (DA.GetDataList(14, intOps))
            {
                myFilter.SrfcIntOpNo = intOps;
            }
            if (DA.GetDataList(15, srfcArea))
            {
                myFilter.SrfcArea = srfcArea;
            }
            //if (DA.GetDataList(16, srfcWeight))
            //{
            //    myFilter.SrfcWeight = srfcWeight;
            //}

            DA.SetData(0, myFilter);
        }
    }
}
