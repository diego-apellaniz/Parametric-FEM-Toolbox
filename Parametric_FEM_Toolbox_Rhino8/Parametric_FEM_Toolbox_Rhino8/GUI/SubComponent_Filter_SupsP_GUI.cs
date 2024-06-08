using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System.Collections.Generic;
using Parametric_FEM_Toolbox.UIWidgets;
using Parametric_FEM_Toolbox.HelperLibraries;
using Parametric_FEM_Toolbox.Utilities;

namespace Parametric_FEM_Toolbox.GUI
{
    public class SubComponent_Filter_SupsP_GUI : SubComponent
    {
        public override string name()
        {
            return "Nodal Supports";
        }
        public override string display_name()
        {
            return "Nodal Supports";
        }
        public override void registerEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Filter nodal supports.");
            evaluationUnit.Icon = Properties.Resources.icon_RFEM_FilterSupportP;
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
            unit.RegisterInputParam(new Param_String(), "Nodes No", "Nodes", "Nodes No", GH_ParamAccess.list);
            unit.Inputs[3].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(),"Displacement Dir X", "Tx", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]", GH_ParamAccess.list);
            unit.Inputs[4].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Displacement Dir Y", "Ty", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]", GH_ParamAccess.list);
            unit.Inputs[5].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Displacement Dir Z", "Tz", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]", GH_ParamAccess.list);
            unit.Inputs[6].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Rotation Dir X", "Rx", "(-1): Fixed; (0): Free; Other: Stiffness in [kNm/rad]", GH_ParamAccess.list);
            unit.Inputs[7].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Rotation Dir Y", "Ry", "(-1): Fixed; (0): Free; Other: Stiffness in [kNm/rad]", GH_ParamAccess.list);
            unit.Inputs[8].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Rotation Dir Z", "Rz", "(-1): Fixed; (0): Free; Other: Stiffness in [kNm/rad]", GH_ParamAccess.list);
            unit.Inputs[9].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "SupportNonlinearityX", "NTx", "Nonlinearity Tpye Displacement Dir X", GH_ParamAccess.list);
            unit.Inputs[10].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "SupportNonlinearityY", "NTy", "Nonlinearity Tpye Displacement Dir Y", GH_ParamAccess.list);
            unit.Inputs[11].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "SupportNonlinearityZ", "NTz", "Nonlinearity Tpye Displacement Dir Z", GH_ParamAccess.list);
            unit.Inputs[12].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "RestraintNonlinearityX", "NRx", "Nonlinearity Tpye Rotation Dir X", GH_ParamAccess.list);
            unit.Inputs[13].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "RestraintNonlinearityY", "NRy", "Nonlinearity Tpye Rotation Dir Y", GH_ParamAccess.list);
            unit.Inputs[14].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "RestraintNonlinearityZ", "NRz", "Nonlinearity Tpye Rotation Dir Z", GH_ParamAccess.list);
            unit.Inputs[15].Parameter.Optional = true;
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
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[14]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[15]);
            unit.AddMenu(gH_ExtendableMenu);
        }
        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Warning;

            var myFilter = new RFFilter();
            myFilter.Type = "Filter (Nodal Supports)";
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
            var ntx = new List<string>();
            var nty = new List<string>();
            var ntz = new List<string>();
            var nrx = new List<string>();
            var nry = new List<string>();
            var nrz = new List<string>();
            var nodesList = new List<string>();

            if (DA.GetDataList(0, supList))
            {
                var nodeListAll = new List<int>();
                foreach (var no in supList)
                {
                    nodeListAll.AddRange(no.ToInt());
                }
                myFilter.SupList = nodeListAll;
            }            
            if(DA.GetDataList(1, commentList))
            {
                myFilter.SupComment = commentList;
            }
            if(DA.GetDataList(2, x))
            {
                myFilter.SupX = x;
            }
            if (DA.GetDataList(3, y))
            {
                myFilter.SupY = y;
            }
            if (DA.GetDataList(4, z))
            {
                myFilter.SupZ = z;
            }
            if (DA.GetDataList(5, nodesList))
            {
                var nodeListAll = new List<int>();
                foreach (var no in nodesList)
                {
                    nodeListAll.AddRange(no.ToInt());
                }
                myFilter.SupNodeList = nodeListAll;
            }
            if (DA.GetDataList(6, tx))
            {
                myFilter.SupTx = tx;
            }
            if (DA.GetDataList(7, ty))
            {
                myFilter.SupTy = ty;
            }
            if (DA.GetDataList(8, tz))
            {
                myFilter.SupTz = tz;
            }
            if (DA.GetDataList(9, rx))
            {
                myFilter.SupRx = rx;
            }
            if (DA.GetDataList(10, ry))
            {
                myFilter.SupRy = ry;
            }
            if (DA.GetDataList(11, rz))
            {
                myFilter.SupRz = rz;
            }
            if (DA.GetDataList(12, ntx))
            {
                myFilter.SupNTx = ntx;
            }
            if (DA.GetDataList(13, nty))
            {
                myFilter.SupNTy = nty;
            }
            if (DA.GetDataList(14, ntz))
            {
                myFilter.SupNTz = ntz;
            }
            if (DA.GetDataList(15, nrx))
            {
                myFilter.SupNRx = nrx;
            }
            if (DA.GetDataList(15, nry))
            {
                myFilter.SupNRy = nry;
            }
            if (DA.GetDataList(15, nrz))
            {
                myFilter.SupNRz = nrz;
            }
            DA.SetData(0, myFilter);
        }
    }
}
