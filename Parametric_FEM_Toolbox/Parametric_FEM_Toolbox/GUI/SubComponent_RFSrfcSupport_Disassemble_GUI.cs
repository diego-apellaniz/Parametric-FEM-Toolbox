using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using Parametric_FEM_Toolbox.UIWidgets;
using Parametric_FEM_Toolbox.Utilities;
using Parametric_FEM_Toolbox.HelperLibraries;
using Dlubal.RFEM5;
using Parametric_FEM_Toolbox.RFEM;

namespace Parametric_FEM_Toolbox.GUI
{
    public class SubComponent_RFSrfcSupport_Disassemble_GUI : SubComponent
    {
        public override string name()
        {
            return "Disassemble";
        }
        public override string display_name()
        {
            return "Disassemble";
        }
        public override void registerEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Disassemble Surface Supports.");
            evaluationUnit.Icon = Properties.Resources.Disassemble_SupportS;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {

            unit.RegisterInputParam(new Param_RFEM(), "RF Surface Support", "RF SrfcSup", "Input RFSrfcSupport.", GH_ParamAccess.item);
            // unit.Inputs[0].Parameter.Optional = true;

            unit.RegisterOutputParam(new Param_Surface(), "Surfaces", "Sfc", "Surfaces the RFSupportS is attached to.");
            unit.RegisterOutputParam(new Param_Integer(), "Support Number", "No", "Index number of the RFEM object.");
            unit.RegisterOutputParam(new Param_Number(), "Displacement Dir X", "Tx", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]");
            unit.RegisterOutputParam(new Param_Number(), "Displacement Dir Y", "Ty", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]");
            unit.RegisterOutputParam(new Param_Number(), "Displacement Dir Z", "Tz", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]");
            unit.RegisterOutputParam(new Param_Number(), "Shear Constant Dir XZ", "Vxz", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m³]");
            unit.RegisterOutputParam(new Param_Number(), "Shear Constant Dir YZ", "Vyz", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m³]");
            unit.RegisterOutputParam(new Param_String(), "SupportNonlinearityZ", "NTz", "Nonlinearity Tpye Displacement Dir Z");
            unit.RegisterOutputParam(new Param_String(), "Comment", "Comment", "Comment.");
            unit.RegisterOutputParam(new Param_String(), "Surface List", "SrfcList", "List of surfaces the support is attached to");
        }

        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Blank;

            // Input
            var inGH = new GH_RFEM();
            if (!DA.GetData(0, ref inGH))
            {
                return;
            }
            var rfSupport = (RFSupportS)inGH.Value;
            // Output
            if (!(rfSupport.BaseSrfcs == null))
            {
                DA.SetDataList(0, rfSupport.BaseSrfcs.Select(x => x.ToBrep()));
            }
            DA.SetData(1, rfSupport.No);
            DA.SetData(2, ((rfSupport.Tx < 0) ? rfSupport.Tx * 1000 : rfSupport.Tx));
            DA.SetData(3, ((rfSupport.Ty < 0) ? rfSupport.Ty * 1000 : rfSupport.Ty));
            DA.SetData(4, ((rfSupport.Tz < 0) ? rfSupport.Tz * 1000 : rfSupport.Tz));
            DA.SetData(5, ((rfSupport.Vxz < 0) ? rfSupport.Vxz * 1000 : rfSupport.Vxz));
            DA.SetData(6, ((rfSupport.Vyz < 0) ? rfSupport.Vyz * 1000 : rfSupport.Vyz));
            DA.SetData(7, rfSupport.NTz);
            DA.SetData(8, rfSupport.Comment);
            DA.SetData(9, rfSupport.SurfaceList);

        }
    }
}
