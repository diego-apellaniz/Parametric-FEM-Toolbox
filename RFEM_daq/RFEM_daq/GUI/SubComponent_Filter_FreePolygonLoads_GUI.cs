using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System.Collections.Generic;
using RFEM_daq.UIWidgets;
using RFEM_daq.HelperLibraries;
using RFEM_daq.Utilities;

namespace RFEM_daq.GUI
{
    public class SubComponent_Filter_FreePolygonLoads_GUI : SubComponent
    {
        public override string name()
        {
            return "Poly Loads";
        }
        public override string display_name()
        {
            return "Poly Loads";
        }
        public override void registerEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Filter free polygon loads.");
            evaluationUnit.Icon = RFEM_daq.Properties.Resources.icon_FilterPolyLoad;
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
            unit.RegisterInputParam(new Param_String(), "Projection Type", "Projection", "Projection Type", GH_ParamAccess.list);
            unit.Inputs[5].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Load Direction Type", "Dir", "Load Direction Type", GH_ParamAccess.list);
            unit.Inputs[6].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Load Distribution Type", "Dist", "Load Distribution Type", GH_ParamAccess.list);
            unit.Inputs[7].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "X [m]", "X", "X Coordinate", GH_ParamAccess.list);
            unit.Inputs[8].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Y [m]", "Y", "Y Coordinate", GH_ParamAccess.list);
            unit.Inputs[9].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Z [m]", "Z", "Z Coordinate", GH_ParamAccess.list);
            unit.Inputs[10].Parameter.Optional = true;
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
            unit.AddMenu(gH_ExtendableMenu);
        }
        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Warning;

            var myFilter = new RFFilter();
            myFilter.Type = "Filter (Free Polygon Loads)";
            var plList = new List<string>();            
            var commentList = new List<string>();
            var x = new List<Interval>();
            var y = new List<Interval>();
            var z = new List<Interval>();
            var f1 = new List<Interval>();
            var f2 = new List<Interval>();
            var f3 = new List<Interval>();
            var surfaceList = new List<string>();
            var lcList = new List<string>();
            var projectionList = new List<string>();
            var dirList = new List<string>();
            var distList = new List<string>();


            if (DA.GetDataList(0, plList))
            {
                var plListAll = new List<int>();
                foreach (var no in plList)
                {
                    plListAll.AddRange(no.ToInt());
                }
                myFilter.PLList = plListAll;
            }            
            if(DA.GetDataList(1, commentList))
            {
                myFilter.PLComment = commentList;
            }
            if (DA.GetDataList(2, lcList))
            {
                var lcListAll = new List<int>();
                foreach (var no in lcList)
                {
                    lcListAll.AddRange(no.ToInt());
                }
                myFilter.PLLC = lcListAll;
            }
            if (DA.GetDataList(3, surfaceList))
            {
                var surfaceListAll = new List<int>();
                foreach (var no in surfaceList)
                {
                    surfaceListAll.AddRange(no.ToInt());
                }
                myFilter.PLSurfaceList = surfaceListAll;
            }            
            if (DA.GetDataList(4, f1))
            {
                myFilter.PLF1 = f1;
            }
            if (DA.GetDataList(5, f2))
            {
                myFilter.PLF2 = f2;
            }
            if (DA.GetDataList(6, f3))
            {
                myFilter.PLF3 = f3;
            }
            if (DA.GetDataList(7, projectionList))
            {
                myFilter.PLProjection = projectionList;
            }
            if (DA.GetDataList(8, dirList))
            {
                myFilter.PLDir = dirList;
            }
            if (DA.GetDataList(9, distList))
            {
                myFilter.PLDist = distList;
            }
            if (DA.GetDataList(10, x))
            {
                myFilter.PLX = x;
            }
            if (DA.GetDataList(11, y))
            {
                myFilter.PLY = y;
            }
            if (DA.GetDataList(12, z))
            {
                myFilter.PLZ = z;
            }
            DA.SetData(0, myFilter);
        }
    }
}
