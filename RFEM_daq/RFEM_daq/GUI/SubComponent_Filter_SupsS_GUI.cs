using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System.Collections.Generic;
using RFEM_daq.UIWidgets;
using RFEM_daq.HelperLibraries;
using RFEM_daq.Utilities;

namespace RFEM_daq.GUI
{
    public class SubComponent_Filter_SupsS_GUI : SubComponent
    {
        public override string name()
        {
            return "Surface Supports";
        }
        public override string display_name()
        {
            return "Surface Supports";
        }
        public override void registerEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Filter surface supports.");
            evaluationUnit.Icon = RFEM_daq.Properties.Resources.icon_RFEM_FilterSupportS;
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
            unit.RegisterInputParam(new Param_String(), "Surfaces No", "Surfaces", "Sfcs No", GH_ParamAccess.list);
            unit.Inputs[3].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(),"Displacement Dir X", "Tx", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]", GH_ParamAccess.list);
            unit.Inputs[4].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Displacement Dir Y", "Ty", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]", GH_ParamAccess.list);
            unit.Inputs[5].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Displacement Dir Z", "Tz", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]", GH_ParamAccess.list);
            unit.Inputs[6].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Shear Constant Dir XZ", "Vxz", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m³]", GH_ParamAccess.list);
            unit.Inputs[7].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Shear Constant Dir YZ", "Vyz", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m³]", GH_ParamAccess.list);
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
            myFilter.Type = "Filter (Surface Supports)";
            var supList = new List<string>();            
            var commentList = new List<string>();
            var x = new List<Interval>();
            var y = new List<Interval>();
            var z = new List<Interval>();
            var tx = new List<Interval>();
            var ty = new List<Interval>();
            var tz = new List<Interval>();
            var vxz = new List<Interval>();
            var vyz = new List<Interval>();
            var srfcList = new List<string>();

            if (DA.GetDataList(0, supList))
            {
                var sfcListAll = new List<int>();
                foreach (var no in supList)
                {
                    sfcListAll.AddRange(no.ToInt());
                }
                myFilter.SupSList = sfcListAll;
            }            
            if(DA.GetDataList(1, commentList))
            {
                myFilter.SupSComment = commentList;
            }
            if(DA.GetDataList(2, x))
            {
                myFilter.SupSX = x;
            }
            if (DA.GetDataList(3, y))
            {
                myFilter.SupSY = y;
            }
            if (DA.GetDataList(4, z))
            {
                myFilter.SupSZ = z;
            }
            if (DA.GetDataList(5, srfcList))
            {
                var sfcListAll = new List<int>();
                foreach (var no in srfcList)
                {
                    sfcListAll.AddRange(no.ToInt());
                }
                myFilter.SupSrfcList = sfcListAll;
            }
            if (DA.GetDataList(6, tx))
            {
                myFilter.SupSTx = tx;
            }
            if (DA.GetDataList(7, ty))
            {
                myFilter.SupSTy = ty;
            }
            if (DA.GetDataList(8, tz))
            {
                myFilter.SupSTz = tz;
            }
            if (DA.GetDataList(9, vxz))
            {
                myFilter.SupSVxz = vxz;
            }
            if (DA.GetDataList(10, vyz))
            {
                myFilter.SupSVyz = vyz;
            }
            DA.SetData(0, myFilter);
        }
    }
}
