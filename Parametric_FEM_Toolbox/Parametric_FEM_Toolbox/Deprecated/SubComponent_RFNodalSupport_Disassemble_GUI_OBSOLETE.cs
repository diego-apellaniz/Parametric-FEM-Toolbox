using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using Parametric_FEM_Toolbox.UIWidgets;
using Parametric_FEM_Toolbox.Utilities;
using Parametric_FEM_Toolbox.HelperLibraries;
using Dlubal.RFEM5;
using Parametric_FEM_Toolbox.RFEM;
using Parametric_FEM_Toolbox.GUI;

namespace Parametric_FEM_Toolbox.Deprecated
{
    public class SubComponent_RFNodalSupport_Disassemble_GUI_OBSOLETE : SubComponent
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
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Disassemble Nodal Supports.");
            evaluationUnit.Icon = Properties.Resources.Disassemble_SupportP;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {

            unit.RegisterInputParam(new Param_RFEM(), "RF Nodal Support", "RF NodSup", "Input RFNodalSupport.", GH_ParamAccess.item);
            // unit.Inputs[0].Parameter.Optional = true;

            unit.RegisterOutputParam(new Param_Plane(), "Location", "Loc", "Plane related to the RFSupportP object.");
            unit.RegisterOutputParam(new Param_Integer(), "Support Number", "No", "Index number of the RFEM object.");
            unit.RegisterOutputParam(new Param_Number(), "Displacement Dir X", "Tx", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]");
            unit.RegisterOutputParam(new Param_Number(), "Displacement Dir Y", "Ty", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]");
            unit.RegisterOutputParam(new Param_Number(), "Displacement Dir Z", "Tz", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]");
            unit.RegisterOutputParam(new Param_Number(), "Rotation Dir X", "Rx", "(-1): Fixed; (0): Free; Other: Stiffness in [kNm/rad]");
            unit.RegisterOutputParam(new Param_Number(), "Rotation Dir Y", "Ry", "(-1): Fixed; (0): Free; Other: Stiffness in [kNm/rad]");
            unit.RegisterOutputParam(new Param_Number(), "Rotation Dir Z", "Rz", "(-1): Fixed; (0): Free; Other: Stiffness in [kNm/rad]");
            unit.RegisterOutputParam(new Param_String(), "Comment", "Comment", "Comment.");
            unit.RegisterOutputParam(new Param_String(), "Node List", "NodeList", "List of nodes the support is attached to");
            unit.RegisterOutputParam(new Param_String(), "Reference System Type", "RSType", "Reference System Type");
            unit.RegisterOutputParam(new Param_String(), "Rotation Sequence", "RotSeq", "ZYX, ZXY, YZX, YXZ, XYZ, XZY");
            unit.RegisterOutputParam(new Param_Vector(), "Rotation Angles", "Rot", "Euler rotation angles in [°]");

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
            var rfSupport = (RFSupportP)inGH.Value;
            // Output
            DA.SetDataList(0, rfSupport.Orientation);
            DA.SetData(1, rfSupport.No);
            DA.SetData(2, ((rfSupport.Tx < 0) ? rfSupport.Tx * 1000 : rfSupport.Tx));
            DA.SetData(3, ((rfSupport.Ty < 0) ? rfSupport.Ty * 1000 : rfSupport.Ty));
            DA.SetData(4, ((rfSupport.Tz < 0) ? rfSupport.Tz * 1000 : rfSupport.Tz));
            DA.SetData(5, ((rfSupport.Rx < 0) ? rfSupport.Rx * 1000 : rfSupport.Rx));
            DA.SetData(6, ((rfSupport.Ry < 0) ? rfSupport.Ry * 1000 : rfSupport.Ry));
            DA.SetData(7, ((rfSupport.Rz < 0) ? rfSupport.Rz * 1000 : rfSupport.Rz));
            DA.SetData(8, rfSupport.Comment);
            DA.SetData(9, rfSupport.NodeList);
            DA.SetData(10, rfSupport.RSType);
            DA.SetData(11, rfSupport.RSeq);
            DA.SetData(12, new Vector3d(rfSupport.RotX.ToDegrees(),
                rfSupport.RotY.ToDegrees(), rfSupport.RotZ.ToDegrees()));

        }
    }
}
