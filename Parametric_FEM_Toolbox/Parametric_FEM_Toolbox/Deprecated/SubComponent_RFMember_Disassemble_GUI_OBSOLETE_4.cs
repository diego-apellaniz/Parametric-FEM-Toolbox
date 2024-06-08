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
    public class SubComponent_RFMember_Disassemble_GUI_OBSOLETE_4 : SubComponent
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
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Disassemble Members.");
            evaluationUnit.Icon = Properties.Resources.Disassemble_Member;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {

            unit.RegisterInputParam(new Param_RFEM(), "RF Member", "RF Member", "Input RFMember.", GH_ParamAccess.item);
            // unit.Inputs[0].Parameter.Optional = true;

            unit.RegisterOutputParam(new Param_Curve(), "Line", "Line", "Line or Curve to assemble the RFLine from.");
            unit.RegisterOutputParam(new Param_Integer(), "Member Number", "No", "Index number of the RFEM object.");
            unit.RegisterOutputParam(new Param_String(), "Comment", "Comment", "Comment.");
            unit.RegisterOutputParam(new Param_Integer(), "Line Number", "LineNo", "Line Number");
            unit.RegisterOutputParam(new Param_String(), "Member Type", "Type", "Member Type");
            unit.RegisterOutputParam(new Param_Number(), "Rotation Angle [°]", "β", "Rotation Angle [°]");
            unit.RegisterOutputParam(new Param_String(), "Rotation Type", "Rot Type", "Rotation Type");
            unit.RegisterOutputParam(new Param_Integer(), "Start Cross-Section", "SCroSec", "Number of End Cross-Section");
            unit.RegisterOutputParam(new Param_Integer(), "End Cross-Section", "ECroSec", "Number of End Cross-Section");
            unit.RegisterOutputParam(new Param_Plane(), "Local Axis", "Axis", "Member Local Axis");
            unit.RegisterOutputParam(new Param_Integer(), "Start Hinge", "SHinge", "Number of Start Hinge");
            unit.RegisterOutputParam(new Param_Integer(), "End Hinge", "EHinge", "Number of End Hinge");
            unit.RegisterOutputParam(new Param_Integer(), "Eccentricity", "Ecc", "Number of Eccentricity");
            unit.RegisterOutputParam(new Param_Integer(), "Division", "Div", "Number of Division");
            unit.RegisterOutputParam(new Param_String(), "Taper Shape", "Taper", "Taper Shape");
            unit.RegisterOutputParam(new Param_Number(), "FactorY", "Kcr,y", "Effective length factor Kcr,y");
            unit.RegisterOutputParam(new Param_Number(), "FactorZ", "Kcr,z", "Effective length factor Kcr,z");
            unit.RegisterOutputParam(new Param_Number(), "Weight [kg]", "W", "Member Weight");

            GH_ExtendableMenu gH_ExtendableMenu = new GH_ExtendableMenu(0, "result_beam");
            gH_ExtendableMenu.Name = "Result Beam";
            gH_ExtendableMenu.Collapse();
            unit.RegisterOutputParam(new Param_String(), "Except Members", "EMembers", "List of members (as string) NOT to be included.");
            unit.RegisterOutputParam(new Param_String(), "Except Solids", "ESolids", "List of solids (as string) NOT to be included.");
            unit.RegisterOutputParam(new Param_String(), "Except Surfaces", "ESfcs", "List of surfaces (as string) NOT to be included.");
            unit.RegisterOutputParam(new Param_String(), "Include Members", "IMembers", "List of members (as string) to be included.");
            unit.RegisterOutputParam(new Param_String(), "Include Solids", "ISolids", "List of solids (as string) to be included.");
            unit.RegisterOutputParam(new Param_String(), "Include Surfaces", "ISfcs", "List of surfaces (as string) to be included.");
            unit.RegisterOutputParam(new Param_String(), "IntegrateStressesAndForcesType", "Integrate", "IntegrateStressesAndForcesType");
            unit.RegisterOutputParam(new Param_Number(), "Parameters", "Params", "Parameters to integrate stresses and forces.");
            gH_ExtendableMenu.RegisterOutputPlug(unit.Outputs[18]);
            gH_ExtendableMenu.RegisterOutputPlug(unit.Outputs[19]);
            gH_ExtendableMenu.RegisterOutputPlug(unit.Outputs[20]);
            gH_ExtendableMenu.RegisterOutputPlug(unit.Outputs[21]);
            gH_ExtendableMenu.RegisterOutputPlug(unit.Outputs[22]);
            gH_ExtendableMenu.RegisterOutputPlug(unit.Outputs[23]);
            gH_ExtendableMenu.RegisterOutputPlug(unit.Outputs[24]);
            gH_ExtendableMenu.RegisterOutputPlug(unit.Outputs[25]);
            unit.AddMenu(gH_ExtendableMenu);

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
            var rFMember = (RFMember)inGH.Value;
            // Output
            DA.SetData(0, rFMember.BaseLine.ToCurve());
            DA.SetData(1, rFMember.No);
            DA.SetData(2, rFMember.Comment);
            DA.SetData(3, rFMember.LineNo);
            DA.SetData(4, rFMember.Type);
            DA.SetData(5, rFMember.RotationAngle);
            DA.SetData(6, rFMember.RotationType);
            DA.SetData(7, rFMember.StartCrossSectionNo);
            DA.SetData(8, rFMember.EndCrossSectionNo);
            DA.SetDataList(9, rFMember.Frames);
            DA.SetData(10, rFMember.StartHingeNo);
            DA.SetData(11, rFMember.EndHingeNo);
            DA.SetData(12, rFMember.EccentricityNo);
            DA.SetData(13, rFMember.DivisionNo);
            DA.SetData(14, rFMember.TaperShape);
            DA.SetData(15, rFMember.Kcry);
            DA.SetData(16, rFMember.Kcrz);
            DA.SetData(17, rFMember.Weight);
            DA.SetData(18, rFMember.ExceptMembers);
            DA.SetData(19, rFMember.ExceptSolids);
            DA.SetData(20, rFMember.ExceptSurfaces);
            DA.SetData(21, rFMember.IncludeMembers);
            DA.SetData(22, rFMember.IncludeSolids);
            DA.SetData(23, rFMember.IncludeSurfaces);
            DA.SetData(24, rFMember.Integrate);
            DA.SetDataList(25, rFMember.Parameters);
        }
    }
}
