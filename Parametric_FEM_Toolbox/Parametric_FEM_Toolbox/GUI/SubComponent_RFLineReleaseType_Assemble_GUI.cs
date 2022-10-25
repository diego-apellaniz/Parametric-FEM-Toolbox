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
    public class SubComponent_RFLineReleaseType_Assemble_GUI : SubComponent
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
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Assemble Line Release Types.");
            evaluationUnit.Icon = Properties.Resources.Assemble_LineReleases;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }
        public override void OnComponentLoaded()
        {
        }
        protected void Setup(EvaluationUnit unit)
        {
            unit.RegisterInputParam(new Param_Integer(), "Release Number", "No", "Optional index number to assign to the RFEM object.", GH_ParamAccess.item);
            unit.Inputs[0].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Rotational Constant X", "RotX", "(-1): Fixed; (0): Free; Other: Stiffness in [kNm/rad/m]", GH_ParamAccess.item);
            unit.Inputs[1].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Translational Constant X", "TransX", "(-1): Fixed; (0): Free; Other: Stiffness in [kNm/m2]", GH_ParamAccess.item);
            unit.Inputs[2].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Translational Constant Y", "TransY", "(-1): Fixed; (0): Free; Other: Stiffness in [kNm/m2]", GH_ParamAccess.item);
            unit.Inputs[3].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Translational Constant Z", "TransZ", "(-1): Fixed; (0): Free; Other: Stiffness in [kNm/m2]", GH_ParamAccess.item);
            unit.Inputs[4].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Comment", "Comment", "Comment.", GH_ParamAccess.item);
            unit.Inputs[5].Parameter.Optional = true;

            GH_ExtendableMenu gH_ExtendableMenu = new GH_ExtendableMenu(0, "advanced");
            gH_ExtendableMenu.Name = "Advanced";
            gH_ExtendableMenu.Collapse();
            unit.RegisterInputParam(new Param_Integer(), "Rotational Nonlinearity X", "RotNonX", "Nonlinearity in φx.", GH_ParamAccess.item);
            unit.Inputs[6].EnumInput = UtilLibrary.ListRFTypes(typeof(NonlinearityType));
            unit.Inputs[6].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_RFEM(), "Diagram for Rotational Nonlinearity X", "DiagRotX", "Diagram for Rotational Nonlinearity X.", GH_ParamAccess.item);
            unit.Inputs[7].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Translational Nonlinearity X", "TransNonX", "Nonlinearity in ux.", GH_ParamAccess.item);
            unit.Inputs[8].EnumInput = UtilLibrary.ListRFTypes(typeof(NonlinearityType));
            unit.Inputs[8].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_RFEM(), "Diagram for Translational Nonlinearity X", "DiagTransX", "Diagram for Translational Nonlinearity X.", GH_ParamAccess.item);
            unit.Inputs[9].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Translational Nonlinearity Y", "TransNonY", "Nonlinearity in uy.", GH_ParamAccess.item);
            unit.Inputs[10].EnumInput = UtilLibrary.ListRFTypes(typeof(NonlinearityType));
            unit.Inputs[10].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_RFEM(), "Diagram for Translational Nonlinearity Y", "DiagTransY", "Diagram for Translational Nonlinearity Y.", GH_ParamAccess.item);
            unit.Inputs[11].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Translational Nonlinearity Z", "TransNonZ", "Nonlinearity in uz.", GH_ParamAccess.item);
            unit.Inputs[12].EnumInput = UtilLibrary.ListRFTypes(typeof(NonlinearityType));
            unit.Inputs[12].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_RFEM(), "Diagram for Translational Nonlinearity Z", "DiagTransZ", "Diagram for Translational Nonlinearity Z.", GH_ParamAccess.item);
            unit.Inputs[13].Parameter.Optional = true;
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[6]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[7]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[8]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[9]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[10]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[11]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[12]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[13]);

            unit.AddMenu(gH_ExtendableMenu);

            GH_ExtendableMenu gH_ExtendableMenu2 = new GH_ExtendableMenu(1, "modify");
            gH_ExtendableMenu2.Name = "Modify";
            gH_ExtendableMenu2.Collapse();
            unit.RegisterInputParam(new Param_RFEM(), "RF Line Release Type", "RF LineReleaseType", "Line Release Type object from the RFEM model to modify", GH_ParamAccess.item);
            unit.Inputs[14].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Modify", "Modify", "Modify object?", GH_ParamAccess.item);
            unit.Inputs[15].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Delete", "Delete", "Delete object?", GH_ParamAccess.item);
            unit.Inputs[16].Parameter.Optional = true;
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[14]);
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[15]);
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[16]);
            unit.AddMenu(gH_ExtendableMenu2);

            unit.RegisterOutputParam(new Param_RFEM(), "RF Line Release Type", "RF LineReleaseType", "Output RFLineReleaseType.");
        }
        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Blank;
            //var line = new LineCurve();
            var noIndex = 0;
            var comment = "";
            var rfRelease = new RFLineReleaseType();
            var inRFEM = new GH_RFEM();
            var mod = false;
            var del = false;
            var tx = 0.0;
            var ty = 0.0;
            var tz = 0.0;
            var rx = 0.0;
            var ntx = 0;
            var nty = 0;
            var ntz = 0;
            var nrx = 0;
            var inDiagTx = new GH_RFEM();
            var inDiagTy = new GH_RFEM();
            var inDiagTz = new GH_RFEM();
            var inDiagRx = new GH_RFEM();

            if (DA.GetData(14, ref inRFEM))
            {
                rfRelease = new RFLineReleaseType((RFLineReleaseType)inRFEM.Value);                
            }
            if (DA.GetData(15, ref mod))
            {
                rfRelease.ToModify = mod;
            }
            if (DA.GetData(16, ref del))
            {
                rfRelease.ToDelete = del;
            }
            if (DA.GetData(0, ref noIndex))
            {
                rfRelease.No = noIndex;
            }
            if (DA.GetData(5, ref comment))
            {
                rfRelease.Comment = comment;
            }
            if (DA.GetData(2, ref tx))
            {
                rfRelease.TranslationalConstantX = tx;
            }
            if (DA.GetData(3, ref ty))
            {
                rfRelease.TranslationalConstantY = ty;
            }
            if (DA.GetData(4, ref tz))
            {
                rfRelease.TranslationalConstantZ = tz;
            }
            if (DA.GetData(1, ref rx))
            {
                rfRelease.RotationalConstantX = rx;
            }
            if (DA.GetData(8, ref ntx))
            {
                rfRelease.TranslationalNonlinearityX = (NonlinearityType)ntx;
                if (rfRelease.TranslationalNonlinearityX == NonlinearityType.UnknownNonlinearityType || (rfRelease.TranslationalNonlinearityX != NonlinearityType.NoneNonlinearityType && 
                    rfRelease.TranslationalNonlinearityX != NonlinearityType.FixedIfNegativeType && rfRelease.TranslationalNonlinearityX != NonlinearityType.FixedIfPositiveType &&
                    rfRelease.TranslationalNonlinearityX != NonlinearityType.WorkingDiagramType))
                {
                    msg = "Nonlinearity Type not supported. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
                else if (rfRelease.TranslationalConstantX == -1)
                {
                    msg = "Unassigned release in Dir X. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
            }
            if (DA.GetData(9, ref inDiagTx))
            {
                if (ntx!=7)
                {
                    msg = "Nonlinearity Type must be a diagram. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
                var diag = new RFDiagram((RFDiagram)inDiagTx.Value);
                rfRelease.DiagramTransX = diag;
            }
            if (DA.GetData(10, ref nty))
            {
                rfRelease.TranslationalNonlinearityY = (NonlinearityType)nty;
                if (rfRelease.TranslationalNonlinearityY == NonlinearityType.UnknownNonlinearityType || (rfRelease.TranslationalNonlinearityY != NonlinearityType.NoneNonlinearityType &&
                    rfRelease.TranslationalNonlinearityY != NonlinearityType.FixedIfNegativeType && rfRelease.TranslationalNonlinearityY != NonlinearityType.FixedIfPositiveType &&
                    rfRelease.TranslationalNonlinearityY != NonlinearityType.WorkingDiagramType))
                {
                    msg = "Nonlinearity Type not supported. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
                else if (rfRelease.TranslationalConstantY == -1)
                {
                    msg = "Unassigned release in Dir Y. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
            }
            if (DA.GetData(11, ref inDiagTy))
            {
                if (nty != 7)
                {
                    msg = "Nonlinearity Type must be a diagram. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
                var diag = new RFDiagram((RFDiagram)inDiagTy.Value);
                rfRelease.DiagramTransY = diag;
            }
            if (DA.GetData(12, ref ntz))
            {
                rfRelease.TranslationalNonlinearityZ = (NonlinearityType)ntz;
                if (rfRelease.TranslationalNonlinearityZ == NonlinearityType.UnknownNonlinearityType || (rfRelease.TranslationalNonlinearityZ != NonlinearityType.NoneNonlinearityType &&
                    rfRelease.TranslationalNonlinearityZ != NonlinearityType.FixedIfNegativeType && rfRelease.TranslationalNonlinearityZ != NonlinearityType.FixedIfPositiveType &&
                    rfRelease.TranslationalNonlinearityZ != NonlinearityType.WorkingDiagramType))
                {
                    msg = "Nonlinearity Type not supported. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
                else if (rfRelease.TranslationalConstantZ == -1)
                {
                    msg = "Unassigned release in Dir Z. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
            }
            if (DA.GetData(13, ref inDiagTz))
            {
                if (ntz != 7)
                {
                    msg = "Nonlinearity Type must be a diagram. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
                var diag = new RFDiagram((RFDiagram)inDiagTz.Value);
                rfRelease.DiagramTransZ = diag;
            }
            if (DA.GetData(6, ref nrx))
            {
                rfRelease.RotationalNonlinearityX = (NonlinearityType)nrx;
                if (rfRelease.RotationalNonlinearityX == NonlinearityType.UnknownNonlinearityType || (rfRelease.RotationalNonlinearityX != NonlinearityType.NoneNonlinearityType &&
                    rfRelease.RotationalNonlinearityX != NonlinearityType.FixedIfNegativeType && rfRelease.RotationalNonlinearityX != NonlinearityType.FixedIfPositiveType &&
                    rfRelease.RotationalNonlinearityX != NonlinearityType.WorkingDiagramType))
                {
                    msg = "Nonlinearity Type not supported. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
                else if (rfRelease.RotationalConstantX == -1)
                {
                    msg = "Unassigned release in Dir X. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
            }
            if (DA.GetData(7, ref inDiagRx))
            {
                if (nrx != 7)
                {
                    msg = "Nonlinearity Type must be a diagram. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
                var diag = new RFDiagram((RFDiagram)inDiagRx.Value);
                rfRelease.DiagramRotX = diag;
            }
            DA.SetData(0, rfRelease);
        }
    }
}