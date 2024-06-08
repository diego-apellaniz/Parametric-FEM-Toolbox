using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System.Collections.Generic;
using Parametric_FEM_Toolbox.UIWidgets;
using Parametric_FEM_Toolbox.HelperLibraries;
using Parametric_FEM_Toolbox.Utilities;

namespace Parametric_FEM_Toolbox.GUI
{
    public class SubComponent_Filter_SurfaceLoads_GUI : SubComponent
    {
        public override string name()
        {
            return "Surface Loads";
        }
        public override string display_name()
        {
            return "Surface Loads";
        }
        public override void registerEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Filter surface loads.");
            evaluationUnit.Icon = Properties.Resources.icon_FilterSurfaceLoad;
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
            unit.RegisterInputParam(new Param_String(), "Surfaces No", "Surfaces", "Surfaces No", GH_ParamAccess.list);
            unit.Inputs[1].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "F1 [kN/m²]", "F1", "F1 [kN/m²]", GH_ParamAccess.list);
            unit.Inputs[2].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "F2 [kN/m²]", "F2", "F2 [kN/m²]", GH_ParamAccess.list);
            unit.Inputs[3].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "F3 [kN/m²]", "F3", "F3 [kN/m²]", GH_ParamAccess.list);
            unit.Inputs[4].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "T4 [°C]", "T4", "T4 [°C]", GH_ParamAccess.list);
            unit.Inputs[5].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "T5 [°C]", "T5", "T5 [°C]", GH_ParamAccess.list);
            unit.Inputs[6].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "T6 [°C]", "T6", "T6 [°C]", GH_ParamAccess.list);
            unit.Inputs[7].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Load Type", "Type", "Load Type", GH_ParamAccess.list);
            unit.Inputs[8].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Load Direction Type", "Dir", "Load Direction Type", GH_ParamAccess.list);
            unit.Inputs[9].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Load Distribution Type", "Dist", "Load Distribution Type", GH_ParamAccess.list);
            unit.Inputs[10].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "X [m]", "X", "X Coordinate", GH_ParamAccess.list);
            unit.Inputs[11].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Y [m]", "Y", "Y Coordinate", GH_ParamAccess.list);
            unit.Inputs[12].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Z [m]", "Z", "Z Coordinate", GH_ParamAccess.list);
            unit.Inputs[13].Parameter.Optional = true;            
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
            unit.AddMenu(gH_ExtendableMenu);
        }
        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Warning;

            var myFilter = new RFFilter();
            myFilter.Type = "Filter (Surface Loads)";
            var slList = new List<string>();            
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
            var surfaceList = new List<string>();
            var lcList = new List<string>();
            var typeList = new List<string>();
            var dirList = new List<string>();
            var distList = new List<string>();


            if (DA.GetDataList(0, slList))
            {
                var slListAll = new List<int>();
                foreach (var no in slList)
                {
                    slListAll.AddRange(no.ToInt());
                }
                myFilter.SLList = slListAll;
            }            
            if(DA.GetDataList(1, commentList))
            {
                myFilter.SLComment = commentList;
            }
            if (DA.GetDataList(2, lcList))
            {
                var lcListAll = new List<int>();
                foreach (var no in lcList)
                {
                    lcListAll.AddRange(no.ToInt());
                }
                myFilter.SLLC = lcListAll;
            }
            if (DA.GetDataList(3, surfaceList))
            {
                var surfaceListAll = new List<int>();
                foreach (var no in surfaceList)
                {
                    surfaceListAll.AddRange(no.ToInt());
                }
                myFilter.SLSurfaceList = surfaceListAll;
            }            
            if (DA.GetDataList(4, f1))
            {
                myFilter.SLF1 = f1;
            }
            if (DA.GetDataList(5, f2))
            {
                myFilter.SLF2 = f2;
            }
            if (DA.GetDataList(6, f3))
            {
                myFilter.SLF3 = f3;
            }
            if (DA.GetDataList(7, f4))
            {
                myFilter.SLT4 = f4;
            }
            if (DA.GetDataList(8, f5))
            {
                myFilter.SLT5 = f5;
            }
            if (DA.GetDataList(9, f6))
            {
                myFilter.SLT6 = f6;
            }
            if (DA.GetDataList(10, typeList))
            {
                myFilter.SLType = typeList;
            }
            if (DA.GetDataList(11, dirList))
            {
                myFilter.SLDir = dirList;
            }
            if (DA.GetDataList(12, distList))
            {
                myFilter.SLDist = distList;
            }
            if (DA.GetDataList(13, x))
            {
                myFilter.SLX = x;
            }
            if (DA.GetDataList(14, y))
            {
                myFilter.SLY = y;
            }
            if (DA.GetDataList(15, z))
            {
                myFilter.SLZ = z;
            }
            DA.SetData(0, myFilter);
        }
    }
}
