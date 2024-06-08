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
    public class SubComponent_RFMemberHinge_Assemble_GUI : SubComponent
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
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Assemble Member Hinges.");
            evaluationUnit.Icon = Properties.Resources.Assemble_MemberRelease;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {
            unit.RegisterInputParam(new Param_Integer(), "Hinge Number", "No", "Optional index number to assign to the RFEM object.", GH_ParamAccess.item);
            unit.Inputs[0].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Displacement Dir X", "Tx", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]", GH_ParamAccess.item);
            unit.Inputs[1].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Displacement Dir Y", "Ty", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]", GH_ParamAccess.item);
            unit.Inputs[2].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Displacement Dir Z", "Tz", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]", GH_ParamAccess.item);
            unit.Inputs[3].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Rotation Dir X", "Rx", "(-1): Fixed; (0): Free; Other: Stiffness in [kNm/rad]", GH_ParamAccess.item);
            unit.Inputs[4].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Rotation Dir Y", "Ry", "(-1): Fixed; (0): Free; Other: Stiffness in [kNm/rad]", GH_ParamAccess.item);
            unit.Inputs[5].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Rotation Dir Z", "Rz", "(-1): Fixed; (0): Free; Other: Stiffness in [kNm/rad]", GH_ParamAccess.item);
            unit.Inputs[6].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Comment", "Comment", "Comment.", GH_ParamAccess.item);
            unit.Inputs[7].Parameter.Optional = true;

            GH_ExtendableMenu gH_ExtendableMenu1 = new GH_ExtendableMenu(0, "nonlinieraity");
            gH_ExtendableMenu1.Name = "Nonlinearity";
            gH_ExtendableMenu1.Collapse();
            unit.RegisterInputParam(new Param_Integer(), "DisplacementNonlinearityX", "NTx", UtilLibrary.DescriptionRFTypes(typeof(NonlinearityType)), GH_ParamAccess.item);
            unit.Inputs[8].EnumInput = UtilLibrary.ListRFTypes(typeof(NonlinearityType));
            unit.Inputs[8].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_RFEM(), "Diagram for Translational Nonlinearity X", "DiagTransX", "Diagram for Translational Nonlinearity X.", GH_ParamAccess.item);
            unit.Inputs[9].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_RFEM(), "Partial Activity for Translational Nonlinearity X", "ActTransX", "Partial Activity for Translational Nonlinearity X.", GH_ParamAccess.item);
            unit.Inputs[10].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "DisplacementNonlinearityY", "NTy", UtilLibrary.DescriptionRFTypes(typeof(NonlinearityType)), GH_ParamAccess.item);
            unit.Inputs[11].EnumInput = UtilLibrary.ListRFTypes(typeof(NonlinearityType));
            unit.Inputs[11].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_RFEM(), "Diagram for Translational Nonlinearity Y", "DiagTransY", "Diagram for Translational Nonlinearity Y.", GH_ParamAccess.item);
            unit.Inputs[12].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_RFEM(), "Partial Activity for Translational Nonlinearity Y", "ActTransY", "Partial Activity for Translational Nonlinearity Y.", GH_ParamAccess.item);
            unit.Inputs[13].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "DisplacementNonlinearityZ", "NTz", UtilLibrary.DescriptionRFTypes(typeof(NonlinearityType)), GH_ParamAccess.item);
            unit.Inputs[14].EnumInput = UtilLibrary.ListRFTypes(typeof(NonlinearityType));
            unit.Inputs[14].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_RFEM(), "Diagram for Translational Nonlinearity Z", "DiagTransZ", "Diagram for Translational Nonlinearity Z.", GH_ParamAccess.item);
            unit.Inputs[15].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_RFEM(), "Partial Activity for Translational Nonlinearity Z", "ActTransZ", "Partial Activity for Translational Nonlinearity Z.", GH_ParamAccess.item);
            unit.Inputs[16].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "RotationNonlinearityX", "NRx", UtilLibrary.DescriptionRFTypes(typeof(NonlinearityType)), GH_ParamAccess.item);
            unit.Inputs[17].EnumInput = UtilLibrary.ListRFTypes(typeof(NonlinearityType));
            unit.Inputs[17].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_RFEM(), "Diagram for Rotational Nonlinearity X", "DiagRotX", "Diagram for Rotational Nonlinearity X.", GH_ParamAccess.item);
            unit.Inputs[18].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_RFEM(), "Partial Activity for Rotational Nonlinearity X", "ActRotX", "Partial Activity for Rotational Nonlinearity X.", GH_ParamAccess.item);
            unit.Inputs[19].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "RotationNonlinearityY", "NRy", UtilLibrary.DescriptionRFTypes(typeof(NonlinearityType)), GH_ParamAccess.item);
            unit.Inputs[20].EnumInput = UtilLibrary.ListRFTypes(typeof(NonlinearityType));
            unit.Inputs[20].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_RFEM(), "Diagram for Rotational Nonlinearity Y", "DiagRotY", "Diagram for Rotational Nonlinearity Y.", GH_ParamAccess.item);
            unit.Inputs[21].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_RFEM(), "Partial Activity for Rotational Nonlinearity Y", "ActRotY", "Partial Activity for Rotational Nonlinearity Y.", GH_ParamAccess.item);
            unit.Inputs[22].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "RotationNonlinearityZ", "NRz", UtilLibrary.DescriptionRFTypes(typeof(NonlinearityType)), GH_ParamAccess.item);
            unit.Inputs[23].EnumInput = UtilLibrary.ListRFTypes(typeof(NonlinearityType));
            unit.Inputs[23].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_RFEM(), "Diagram for Rotational Nonlinearity Z", "DiagRotZ", "Diagram for Rotational Nonlinearity Z.", GH_ParamAccess.item);
            unit.Inputs[24].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_RFEM(), "Partial Activity for Rotational Nonlinearity Z", "ActRotZ", "Partial Activity for Rotational Nonlinearity ZY.", GH_ParamAccess.item);
            unit.Inputs[25].Parameter.Optional = true;
            gH_ExtendableMenu1.RegisterInputPlug(unit.Inputs[8]);
            gH_ExtendableMenu1.RegisterInputPlug(unit.Inputs[9]);
            gH_ExtendableMenu1.RegisterInputPlug(unit.Inputs[10]);
            gH_ExtendableMenu1.RegisterInputPlug(unit.Inputs[11]);
            gH_ExtendableMenu1.RegisterInputPlug(unit.Inputs[12]);
            gH_ExtendableMenu1.RegisterInputPlug(unit.Inputs[13]);
            gH_ExtendableMenu1.RegisterInputPlug(unit.Inputs[14]);
            gH_ExtendableMenu1.RegisterInputPlug(unit.Inputs[15]);
            gH_ExtendableMenu1.RegisterInputPlug(unit.Inputs[16]);
            gH_ExtendableMenu1.RegisterInputPlug(unit.Inputs[17]);
            gH_ExtendableMenu1.RegisterInputPlug(unit.Inputs[18]);
            gH_ExtendableMenu1.RegisterInputPlug(unit.Inputs[19]);
            gH_ExtendableMenu1.RegisterInputPlug(unit.Inputs[20]);
            gH_ExtendableMenu1.RegisterInputPlug(unit.Inputs[21]);
            gH_ExtendableMenu1.RegisterInputPlug(unit.Inputs[22]);
            gH_ExtendableMenu1.RegisterInputPlug(unit.Inputs[23]);
            gH_ExtendableMenu1.RegisterInputPlug(unit.Inputs[24]);
            gH_ExtendableMenu1.RegisterInputPlug(unit.Inputs[25]);
            unit.AddMenu(gH_ExtendableMenu1);

            GH_ExtendableMenu gH_ExtendableMenu2 = new GH_ExtendableMenu(1, "modify");
            gH_ExtendableMenu2.Name = "Modify";
            gH_ExtendableMenu2.Collapse();
            unit.RegisterInputParam(new Param_RFEM(), "RF Member Hinge", "RF MemberHinge", "Member Hinge object from the RFEM model to modify", GH_ParamAccess.item);
            unit.Inputs[26].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Modify", "Modify", "Modify object?", GH_ParamAccess.item);
            unit.Inputs[27].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Delete", "Delete", "Delete object?", GH_ParamAccess.item);
            unit.Inputs[28].Parameter.Optional = true;
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[26]);
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[27]);
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[28]);
            unit.AddMenu(gH_ExtendableMenu2);

            unit.RegisterOutputParam(new Param_RFEM(), "RF Member Hinge", "RF MemberHinge", "Output RFMemberHinge.");
        }

        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Blank;
            //var line = new LineCurve();
            var noIndex = 0;
            var comment = "";
            var rfHinge = new RFMemberHinge();
            var inRFEM = new GH_RFEM();
            var mod = false;
            var del = false;
            var tx = 0.0;
            var ty = 0.0;
            var tz = 0.0;
            var rx = 0.0;
            var ry = 0.0;
            var rz = 0.0;
            var ntx = 0;
            var nty = 0;
            var ntz = 0;
            var nrx = 0;
            var nry = 0;
            var nrz = 0;
            var inDiagTx = new GH_RFEM();
            var inDiagTy = new GH_RFEM();
            var inDiagTz = new GH_RFEM();
            var inDiagRx = new GH_RFEM();
            var inDiagRy = new GH_RFEM();
            var inDiagRz = new GH_RFEM();
            var inActTx = new GH_RFEM();
            var inActTy = new GH_RFEM();
            var inActTz = new GH_RFEM();
            var inActRx = new GH_RFEM();
            var inActRy = new GH_RFEM();
            var inActRz = new GH_RFEM();

            if (DA.GetData(26, ref inRFEM))
            {
                rfHinge = new RFMemberHinge((RFMemberHinge)inRFEM.Value);                
            }
            else
            {
                rfHinge = new RFMemberHinge();
                //msg = "Insufficient input parameters. Provide either Input Curve or Line Number or existing RFLineHinge Object. ";
                //level = GH_RuntimeMessageLevel.Warning;
                //return;
            }
            if (DA.GetData(27, ref mod))
            {
                rfHinge.ToModify = mod;
            }
            if (DA.GetData(28, ref del))
            {
                rfHinge.ToDelete = del;
            }
            if (DA.GetData(0, ref noIndex))
            {
                rfHinge.No = noIndex;
            }
            if (DA.GetData(7, ref comment))
            {
                rfHinge.Comment = comment;
            }
            if (DA.GetData(1, ref tx))
            {
                rfHinge.Tx = tx;
            }
            if (DA.GetData(2, ref ty))
            {
                rfHinge.Ty = ty;
            }
            if (DA.GetData(3, ref tz))
            {
                rfHinge.Tz = tz;
            }
            if (DA.GetData(4, ref rx))
            {
                rfHinge.Rx = rx;
            }
            if (DA.GetData(5, ref ry))
            {
                rfHinge.Ry = ry;
            }
            if (DA.GetData(6, ref rz))
            {
                rfHinge.Rz = rz;
            }
            if (DA.GetData(8, ref ntx))
            {
                rfHinge.TranslationalNonlinearityX = (NonlinearityType)ntx;
                if (rfHinge.TranslationalNonlinearityX == NonlinearityType.UnknownNonlinearityType || (rfHinge.TranslationalNonlinearityX != NonlinearityType.NoneNonlinearityType &&
                    rfHinge.TranslationalNonlinearityX != NonlinearityType.FixedIfNegativeType && rfHinge.TranslationalNonlinearityX != NonlinearityType.FixedIfPositiveType &&
                    rfHinge.TranslationalNonlinearityX != NonlinearityType.WorkingDiagramType && rfHinge.TranslationalNonlinearityX != NonlinearityType.PartialActivityNLType))
                {
                    msg = "Nonlinearity Type not supported. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
                else if (rfHinge.Tx == -1)
                {
                    msg = "Unassigned release in Dir X. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
            }
            if (DA.GetData(9, ref inDiagTx))
            {
                if (ntx != 7)
                {
                    msg = "Nonlinearity Type must be a diagram. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
                var diag = new RFDiagram((RFDiagram)inDiagTx.Value);
                rfHinge.DiagramTransX = diag;
            }
            if (DA.GetData(10, ref inActTx))
            {
                if (ntx != 6)
                {
                    msg = "Nonlinearity Type must be partial activity. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
                var act = new RFPartialActivity((RFPartialActivity)inActTx.Value);
                rfHinge.PartialActivityTransX = act;
            }
            if (DA.GetData(11, ref nty))
            {
                rfHinge.TranslationalNonlinearityY = (NonlinearityType)nty;
                if (rfHinge.TranslationalNonlinearityY == NonlinearityType.UnknownNonlinearityType || (rfHinge.TranslationalNonlinearityY != NonlinearityType.NoneNonlinearityType &&
                    rfHinge.TranslationalNonlinearityY != NonlinearityType.FixedIfNegativeType && rfHinge.TranslationalNonlinearityY != NonlinearityType.FixedIfPositiveType &&
                    rfHinge.TranslationalNonlinearityY != NonlinearityType.WorkingDiagramType && rfHinge.TranslationalNonlinearityY != NonlinearityType.PartialActivityNLType))
                {
                    msg = "Nonlinearity Type not supported. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
                else if (rfHinge.Ty == -1)
                {
                    msg = "Unassigned release in Dir Y. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
            }
            if (DA.GetData(12, ref inDiagTy))
            {
                if (nty != 7)
                {
                    msg = "Nonlinearity Type must be a diagram. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
                var diag = new RFDiagram((RFDiagram)inDiagTy.Value);
                rfHinge.DiagramTransY = diag;
            }
            if (DA.GetData(13, ref inActTy))
            {
                if (nty != 6)
                {
                    msg = "Nonlinearity Type must be partial activity. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
                var act = new RFPartialActivity((RFPartialActivity)inActTy.Value);
                rfHinge.PartialActivityTransY = act;
            }
            if (DA.GetData(14, ref ntz))
            {
                rfHinge.TranslationalNonlinearityZ = (NonlinearityType)ntz;
                if (rfHinge.TranslationalNonlinearityZ == NonlinearityType.UnknownNonlinearityType || (rfHinge.TranslationalNonlinearityZ != NonlinearityType.NoneNonlinearityType &&
                    rfHinge.TranslationalNonlinearityZ != NonlinearityType.FixedIfNegativeType && rfHinge.TranslationalNonlinearityZ != NonlinearityType.FixedIfPositiveType &&
                    rfHinge.TranslationalNonlinearityZ != NonlinearityType.WorkingDiagramType && rfHinge.TranslationalNonlinearityZ != NonlinearityType.PartialActivityNLType))
                {
                    msg = "Nonlinearity Type not supported. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
                else if (rfHinge.Tz == -1)
                {
                    msg = "Unassigned release in Dir Z. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
            }
            if (DA.GetData(15, ref inDiagTz))
            {
                if (ntz != 7)
                {
                    msg = "Nonlinearity Type must be a diagram. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
                var diag = new RFDiagram((RFDiagram)inDiagTz.Value);
                rfHinge.DiagramTransZ = diag;
            }
            if (DA.GetData(16, ref inActTz))
            {
                if (ntz != 6)
                {
                    msg = "Nonlinearity Type must be partial activity. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
                var act = new RFPartialActivity((RFPartialActivity)inActTz.Value);
                rfHinge.PartialActivityTransZ = act;
            }
            if (DA.GetData(17, ref nrx))
            {
                rfHinge.RotationalNonlinearityX = (NonlinearityType)nrx;
                if (rfHinge.RotationalNonlinearityX == NonlinearityType.UnknownNonlinearityType || (rfHinge.RotationalNonlinearityX != NonlinearityType.NoneNonlinearityType &&
                    rfHinge.RotationalNonlinearityX != NonlinearityType.FixedIfNegativeType && rfHinge.RotationalNonlinearityX != NonlinearityType.FixedIfPositiveType &&
                    rfHinge.RotationalNonlinearityX != NonlinearityType.WorkingDiagramType && rfHinge.RotationalNonlinearityX != NonlinearityType.PartialActivityNLType))
                {
                    msg = "Nonlinearity Type not supported. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
                else if (rfHinge.Rx == -1)
                {
                    msg = "Unassigned release in Dir X. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
            }
            if (DA.GetData(18, ref inDiagRx))
            {
                if (nrx != 7)
                {
                    msg = "Nonlinearity Type must be a diagram. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
                var diag = new RFDiagram((RFDiagram)inDiagRx.Value);
                rfHinge.DiagramRotX = diag;
            }
            if (DA.GetData(19, ref inActRx))
            {
                if (nrx != 6)
                {
                    msg = "Nonlinearity Type must be partial activity. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
                var act = new RFPartialActivity((RFPartialActivity)inActRx.Value);
                rfHinge.PartialActivityRotX = act;
            }
            if (DA.GetData(20, ref nry))
            {
                rfHinge.RotationalNonlinearityY = (NonlinearityType)nry;
                if (rfHinge.RotationalNonlinearityY == NonlinearityType.UnknownNonlinearityType || (rfHinge.RotationalNonlinearityY != NonlinearityType.NoneNonlinearityType &&
                    rfHinge.RotationalNonlinearityY != NonlinearityType.FixedIfNegativeType && rfHinge.RotationalNonlinearityY != NonlinearityType.FixedIfPositiveType &&
                    rfHinge.RotationalNonlinearityY != NonlinearityType.WorkingDiagramType && rfHinge.RotationalNonlinearityY != NonlinearityType.PartialActivityNLType))
                {
                    msg = "Nonlinearity Type not supported. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
                else if (rfHinge.Ry == -1)
                {
                    msg = "Unassigned release in Dir Y. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
            }
            if (DA.GetData(21, ref inDiagRy))
            {
                if (nry != 7)
                {
                    msg = "Nonlinearity Type must be a diagram. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
                var diag = new RFDiagram((RFDiagram)inDiagRy.Value);
                rfHinge.DiagramRotY = diag;
            }
            if (DA.GetData(22, ref inActRy))
            {
                if (nry != 6)
                {
                    msg = "Nonlinearity Type must be partial activity. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
                var act = new RFPartialActivity((RFPartialActivity)inActRy.Value);
                rfHinge.PartialActivityRotY = act;
            }
            if (DA.GetData(23, ref nrz))
            {
                rfHinge.RotationalNonlinearityZ = (NonlinearityType)nrz;
                if (rfHinge.RotationalNonlinearityZ == NonlinearityType.UnknownNonlinearityType || (rfHinge.RotationalNonlinearityZ != NonlinearityType.NoneNonlinearityType &&
                    rfHinge.RotationalNonlinearityZ != NonlinearityType.FixedIfNegativeType && rfHinge.RotationalNonlinearityZ != NonlinearityType.FixedIfPositiveType &&
                    rfHinge.RotationalNonlinearityZ != NonlinearityType.WorkingDiagramType && rfHinge.RotationalNonlinearityZ != NonlinearityType.PartialActivityNLType))
                {
                    msg = "Nonlinearity Type not supported. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
                else if (rfHinge.Rz == -1)
                {
                    msg = "Unassigned release in Dir Z. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
            }
            if (DA.GetData(24, ref inDiagRz))
            {
                if (nrz != 7)
                {
                    msg = "Nonlinearity Type must be a diagram. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
                var diag = new RFDiagram((RFDiagram)inDiagRz.Value);
                rfHinge.DiagramRotZ = diag;
            }
            if (DA.GetData(25, ref inActRz))
            {
                if (nrz != 6)
                {
                    msg = "Nonlinearity Type must be partial activity. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
                var act = new RFPartialActivity((RFPartialActivity)inActRz.Value);
                rfHinge.PartialActivityRotZ = act;
            }
            DA.SetData(0, rfHinge);
        }
    }
}