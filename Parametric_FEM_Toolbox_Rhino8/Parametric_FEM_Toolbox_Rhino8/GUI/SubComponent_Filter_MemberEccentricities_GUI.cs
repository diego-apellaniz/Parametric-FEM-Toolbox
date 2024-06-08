using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System.Collections.Generic;
using Parametric_FEM_Toolbox.UIWidgets;
using Parametric_FEM_Toolbox.HelperLibraries;
using Parametric_FEM_Toolbox.Utilities;

namespace Parametric_FEM_Toolbox.GUI
{
    public class SubComponent_Filter_MemberEccentricities_GUI : SubComponent
    {
        public override string name()
        {
            return "Member Eccentricities";
        }
        public override string display_name()
        {
            return "Member Eccentricities";
        }
        public override void registerEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Filter member releases.");
            evaluationUnit.Icon = Properties.Resources.icon_RFEM_FilterMemberEccentricity;
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
            unit.RegisterInputParam(new Param_String(), "Reference system", "Sys", "Reference system", GH_ParamAccess.list);
            unit.Inputs[0].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Horizontal alignment", "H", "Horizontal alignment", GH_ParamAccess.list);
            unit.Inputs[1].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Vertical alignment", "V", "Vertical alignment", GH_ParamAccess.list);
            unit.Inputs[2].Parameter.Optional = true; 
            unit.RegisterInputParam(new Param_Interval(), "Start Eccentricity X", "Sx", "Start Eccentricity X [mm]", GH_ParamAccess.list);
            unit.Inputs[3].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Start Eccentricity Y", "Sy", "Start Eccentricity Y [mm]", GH_ParamAccess.list);
            unit.Inputs[4].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Start Eccentricity Z", "Sz", "Start Eccentricity Z [mm]", GH_ParamAccess.list);
            unit.Inputs[5].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "End Eccentricity X", "Ex", "End Eccentricity X [mm]", GH_ParamAccess.list);
            unit.Inputs[6].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "End Eccentricity Y", "Ey", "End Eccentricity Y [mm]", GH_ParamAccess.list);
            unit.Inputs[7].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "End Eccentricity Z", "Ez", "End Eccentricity Z [mm]", GH_ParamAccess.list);
            unit.Inputs[8].Parameter.Optional = true;
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[0]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[1]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[2]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[3]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[4]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[5]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[6]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[7]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[8]);
            unit.AddMenu(gH_ExtendableMenu);
        }
        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Warning;

            var myFilter = new RFFilter();
            myFilter.Type = "Filter (Member Eccentricities)";
            var eccList = new List<string>();            
            var commentList = new List<string>();
            var sysList = new List<string>();
            var hList = new List<string>();
            var vList = new List<string>();
            var sx = new List<Interval>();
            var sy = new List<Interval>();
            var sz = new List<Interval>();
            var ex = new List<Interval>();
            var ey = new List<Interval>();
            var ez = new List<Interval>();

            if (DA.GetDataList(0, eccList))
            {
                var eccListAll = new List<int>();
                foreach (var no in eccList)
                {
                    eccListAll.AddRange(no.ToInt());
                }
                myFilter.MEccList = eccListAll;
            }            
            if(DA.GetDataList(1, commentList))
            {
                myFilter.MEccComment = commentList;                       
            }
            if (DA.GetDataList(2, sysList))
            {
                myFilter.MEccReferenceType = sysList;
            }
            if (DA.GetDataList(3, hList))
            {
                myFilter.MEccHorAl = hList;
            }
            if (DA.GetDataList(4, vList))
            {
                myFilter.MEccVertAl = vList;
            }
            if (DA.GetDataList(5, sx))
            {
                myFilter.MEccX1 = sx;
            }
            if (DA.GetDataList(6, sy))
            {
                myFilter.MEccY1 = sy;
            }
            if (DA.GetDataList(7, sz))
            {
                myFilter.MEccZ1 = sz;
            }
            if (DA.GetDataList(8, ex))
            {
                myFilter.MEccX2 = ex;
            }
            if (DA.GetDataList(9, ey))
            {
                myFilter.MEccY2 = ey;
            }
            if (DA.GetDataList(10, ez))
            {
                myFilter.MEccZ2 = ez;
            }
            DA.SetData(0, myFilter);
        }
    }
}
