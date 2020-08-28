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

namespace Parametric_FEM_Toolbox.GUI
{
    public class SubComponent_RFLineSupport_Assemble_GUI : SubComponent
    {
        public override string name()
        {
            return "Assemble";
        }
        public override string display_name()
        {
            return "Assemble";
        }
        public override void registerEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Assemble Line Supports.");
            evaluationUnit.Icon = Properties.Resources.Assemble_SupportL;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {

            unit.RegisterInputParam(new Param_Curve(), "Line", "Line", "Line or Curve to attach RFSupportL to.", GH_ParamAccess.item);
            unit.Inputs[0].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Support Number", "No", "Optional index number to assign to the RFEM object.", GH_ParamAccess.item);
            unit.Inputs[1].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Displacement Dir X", "Tx", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]", GH_ParamAccess.item);
            unit.Inputs[2].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Displacement Dir Y", "Ty", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]", GH_ParamAccess.item);
            unit.Inputs[3].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Displacement Dir Z", "Tz", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]", GH_ParamAccess.item);
            unit.Inputs[4].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Rotation Dir X", "Rx", "(-1): Fixed; (0): Free; Other: Stiffness in [kNm/rad]", GH_ParamAccess.item);
            unit.Inputs[5].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Rotation Dir Y", "Ry", "(-1): Fixed; (0): Free; Other: Stiffness in [kNm/rad]", GH_ParamAccess.item);
            unit.Inputs[6].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Rotation Dir Z", "Rz", "(-1): Fixed; (0): Free; Other: Stiffness in [kNm/rad]", GH_ParamAccess.item);
            unit.Inputs[7].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Comment", "Comment", "Comment.", GH_ParamAccess.item);
            unit.Inputs[8].Parameter.Optional = true;

            GH_ExtendableMenu gH_ExtendableMenu = new GH_ExtendableMenu(0, "advanced");
            gH_ExtendableMenu.Name = "Advanced";
            gH_ExtendableMenu.Collapse();
            unit.RegisterInputParam(new Param_String(), "Line List", "LineList", "Line List", GH_ParamAccess.item);
            unit.Inputs[9].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Reference System Type", "RefSys", UtilLibrary.DescriptionRFTypes(typeof(ReferenceSystemType)), GH_ParamAccess.item);
            unit.Inputs[10].EnumInput = UtilLibrary.ListRFTypes(typeof(ReferenceSystemType));
            unit.Inputs[10].Parameter.Optional = true;
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[9]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[10]);
            unit.AddMenu(gH_ExtendableMenu);

            GH_ExtendableMenu gH_ExtendableMenu2 = new GH_ExtendableMenu(1, "modify");
            gH_ExtendableMenu2.Name = "Modify";
            gH_ExtendableMenu2.Collapse();
            unit.RegisterInputParam(new Param_RFEM(), "RF Line Support", "RF LineSup", "Support object from the RFEM model to modify", GH_ParamAccess.item);
            unit.Inputs[11].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Modify", "Modify", "Modify object?", GH_ParamAccess.item);
            unit.Inputs[12].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Delete", "Delete", "Delete object?", GH_ParamAccess.item);
            unit.Inputs[13].Parameter.Optional = true;
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[11]);
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[12]);
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[13]);
            unit.AddMenu(gH_ExtendableMenu2);

            unit.RegisterOutputParam(new Param_RFEM(), "RF Line Support", "RF LineSup", "Output RFLineSupport.");
        }

        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Blank;

            Curve inCurve = null;
            var noIndex = 0;
            var comment = "";
            var rfSup = new RFSupportL();
            var inRFEM = new GH_RFEM();
            var mod = false;
            var del = false;
            var tx = 0.0;
            var ty = 0.0;
            var tz = 0.0;
            var rx = 0.0;
            var ry = 0.0;
            var rz = 0.0;
            var lineList = "";
            var refSys = 0;


            if (DA.GetData(11, ref inRFEM))
            {
                rfSup = new RFSupportL((RFSupportL)inRFEM.Value);
                if (DA.GetData(0, ref inCurve))
                {
                    var myRFLine = new RFLine();
                    Component_RFLine.SetGeometry(inCurve, ref myRFLine);
                    var myRFLines = new List<RFLine>() { myRFLine };
                    rfSup = new RFSupportL(rfSup, myRFLines);
                }
            }
            else if (DA.GetData(0, ref inCurve))
            {
                var myRFLine = new RFLine();
                Component_RFLine.SetGeometry(inCurve, ref myRFLine);
                var myRFLines = new List<RFLine>() { myRFLine };
                rfSup = new RFSupportL(new LineSupport(), myRFLines);
            }
            else if (DA.GetData(9, ref lineList))
            {
                rfSup = new RFSupportL();
                rfSup.LineList = lineList;
            }
            else
            {
                msg = "Insufficient input parameters. Provide either Input Curve or Line List or existing RFSupportL Object. ";
                level = GH_RuntimeMessageLevel.Warning;
                return;
            }
            if (DA.GetData(12, ref mod))
            {
                rfSup.ToModify = mod;
            }
            if (DA.GetData(13, ref del))
            {
                rfSup.ToDelete = del;
            }
            if (DA.GetData(1, ref noIndex))
            {
                rfSup.No = noIndex;
            }
            if (DA.GetData(8, ref comment))
            {
                rfSup.Comment = comment;
            }
            if (DA.GetData(2, ref tx))
            {
                rfSup.Tx = tx;
            }
            if (DA.GetData(3, ref ty))
            {
                rfSup.Ty = ty;
            }
            if (DA.GetData(4, ref tz))
            {
                rfSup.Tz = tz;
            }
            if (DA.GetData(5, ref rx))
            {
                rfSup.Rx = rx;
            }
            if (DA.GetData(6, ref ry))
            {
                rfSup.Ry = ry;
            }
            if (DA.GetData(7, ref rz))
            {
                rfSup.Rz = rz;
            }
            if (DA.GetData(9, ref lineList))
            {
                rfSup.LineList = lineList;
            }
            if (DA.GetData(10, ref refSys))
            {
                rfSup.RSType = (ReferenceSystemType)refSys;
                if (rfSup.RSType == ReferenceSystemType.UnknownReferenceSystemType)
                {
                    msg = "Reference System Type not supported. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
            }

            DA.SetData(0, rfSup);
        }
    }
}
