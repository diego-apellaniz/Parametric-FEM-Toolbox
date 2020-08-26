using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using RFEM_daq.UIWidgets;
using RFEM_daq.Utilities;
using RFEM_daq.HelperLibraries;
using Dlubal.RFEM5;
using RFEM_daq.RFEM;

namespace RFEM_daq.GUI
{
    public class SubComponent_RFMember_Disassemble_GUI : SubComponent
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
            evaluationUnit.Icon = RFEM_daq.Properties.Resources.Disassemble_Member;
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
        }
    }
}
