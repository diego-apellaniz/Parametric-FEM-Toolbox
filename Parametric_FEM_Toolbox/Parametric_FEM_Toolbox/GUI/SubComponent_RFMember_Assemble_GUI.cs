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
    public class SubComponent_RFMember_Assemble_GUI : SubComponent
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
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Assemble Members.");
            evaluationUnit.Icon = Properties.Resources.Assemble_Member;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {

            unit.RegisterInputParam(new Param_Curve(), "Line", "Line", "Line or Curve to assemble the RFLine from.", GH_ParamAccess.item);
            unit.Inputs[0].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Start Cross-Section", "SCroSec", "Number of Start Cross-Section", GH_ParamAccess.item);
            unit.Inputs[1].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Member Number", "No", "Optional index number to assign to the RFEM object.", GH_ParamAccess.item);
            unit.Inputs[2].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Comment", "Comment", "Comment.", GH_ParamAccess.item);
            unit.Inputs[3].Parameter.Optional = true;

            GH_ExtendableMenu gH_ExtendableMenu = new GH_ExtendableMenu(0, "advanced");
            gH_ExtendableMenu.Name = "Advanced";
            gH_ExtendableMenu.Collapse();
            unit.RegisterInputParam(new Param_Integer(), "LineNo", "LineNo", "Line Number", GH_ParamAccess.item);
            unit.Inputs[4].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Member Type", "Type", UtilLibrary.DescriptionRFTypes(typeof(MemberType)), GH_ParamAccess.item);
            unit.Inputs[5].EnumInput = UtilLibrary.ListRFTypes(typeof(MemberType));
            unit.Inputs[5].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Rotation Angle [°]", "β", "Rotation Angle [°]", GH_ParamAccess.item);
            unit.Inputs[6].Parameter.Optional = true;
            //unit.RegisterInputParam(new Param_Integer(), "Interpolated Points", "n", "Number of interpolated points for NURBS", GH_ParamAccess.item);
            //unit.Inputs[3].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "End Cross-Section", "ECroSec", "Number of End Cross-Section", GH_ParamAccess.item);
            unit.Inputs[7].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Start Hinge", "SHinge", "Number of Start Hinge", GH_ParamAccess.item);
            unit.Inputs[8].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "End Hinge", "EHinge", "Number of End Hinge", GH_ParamAccess.item);
            unit.Inputs[9].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Eccentricity", "Ecc", "Number of Eccentricity", GH_ParamAccess.item);
            unit.Inputs[10].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Division", "Div", "Number of Division", GH_ParamAccess.item);
            unit.Inputs[11].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Taper Shape", "Taper", UtilLibrary.DescriptionRFTypes(typeof(TaperShapeType)), GH_ParamAccess.item);
            unit.Inputs[12].EnumInput = UtilLibrary.ListRFTypes(typeof(TaperShapeType));
            unit.Inputs[12].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "FactorY", "Kcr,y", "Effective length factor Kcr,y", GH_ParamAccess.item);
            unit.Inputs[13].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "FactorZ", "Kcr,z", "Effective length factor Kcr,z", GH_ParamAccess.item);
            unit.Inputs[14].Parameter.Optional = true;
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
            unit.AddMenu(gH_ExtendableMenu);

            GH_ExtendableMenu gH_ExtendableMenu2 = new GH_ExtendableMenu(1, "modify");
            gH_ExtendableMenu2.Name = "Modify";
            gH_ExtendableMenu2.Collapse();
            unit.RegisterInputParam(new Param_RFEM(), "RF Member", "RF Member", "Member object from the RFEM model to modify", GH_ParamAccess.item);
            unit.Inputs[15].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Modify", "Modify", "Modify object?", GH_ParamAccess.item);
            unit.Inputs[16].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Delete", "Delete", "Delete object?", GH_ParamAccess.item);
            unit.Inputs[17].Parameter.Optional = true;
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[15]);
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[16]);
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[17]);
            unit.AddMenu(gH_ExtendableMenu2);

            unit.RegisterOutputParam(new Param_RFEM(), "RF Member", "RF Member", "Output RFMember.");
        }

        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Blank;
            //var line = new LineCurve();
            Curve inCurve = null;
            var noIndex = 0;
            var comment = "";
            var rFMember = new RFMember();
            var inRFEM = new GH_RFEM();
            var mod = false;
            var del = false;
            var lineNo = 0;
            var memberType = 0;
            var taperType = 0;
            var rotAngle = 0.0;
            //var intPoints = 4;
            var sCS = 0;
            var eCS = 0;
            var sH = 0;
            var eH = 0;
            var ecc = 0;
            var div = 0;
            var kcry = 1.0;
            var kcrz = 1.0;
            //int newNo = 0;

            if (DA.GetData(15, ref inRFEM))
            {
                rFMember = new RFMember((RFMember)inRFEM.Value);
                if (DA.GetData(1, ref sCS))
                {
                    rFMember.StartCrossSectionNo = sCS;
                }
                if (DA.GetData(0, ref inCurve))
                {
                    var myRFLine = new RFLine();
                    Component_RFLine.SetGeometry(inCurve, ref myRFLine);
                    rFMember.SetFrames();
                }
                if (DA.GetData(1, ref sCS))
                {
                    rFMember.StartCrossSectionNo = sCS;
                }
            }
            else if (DA.GetData(0, ref inCurve) && DA.GetData(1, ref sCS))
            {
                var myRFLine = new RFLine();
                Component_RFLine.SetGeometry(inCurve, ref myRFLine);
                rFMember.BaseLine = myRFLine;
                rFMember.SetFrames();                
                rFMember.StartCrossSectionNo = sCS;
            }
            else if (DA.GetData(4, ref lineNo) && DA.GetData(1, ref sCS))
            {
                rFMember.LineNo = lineNo;
                rFMember.BaseLine = null;
                rFMember.StartCrossSectionNo = sCS;
            }
            else
            {
                msg = "Insufficient input parameters. Provide either Input Curve and Start Cross Section or existing RFMember Object. ";
                level = GH_RuntimeMessageLevel.Warning;
            return;
            }
            if (DA.GetData(16, ref mod))
            {
                rFMember.ToModify = mod;
            }
            if (DA.GetData(17, ref del))
            {
                rFMember.ToDelete = del;
            }
            if (DA.GetData(2, ref noIndex))
            {
                rFMember.No = noIndex;
            }
            if (DA.GetData(3, ref comment))
            {
                rFMember.Comment = comment;
            }
            //if (DA.GetData(4, ref lineNo))
            //{
            //    rFMember.LineNo = lineNo;
            //}
            if (DA.GetData(5, ref memberType))
            {
                rFMember.Type = (MemberType)memberType;
                if (rFMember.Type == MemberType.UnknownMemberType)
                {
                    msg = "Member Type not supported. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
            }
            if (DA.GetData(6, ref rotAngle))
            {
                rFMember.RotationType = RotationType.Angle;
                rFMember.RotationAngle = rotAngle;
            }
            if (DA.GetData(7, ref eCS))
            {
                rFMember.EndCrossSectionNo = eCS;
            }
            if (DA.GetData(8, ref sH))
            {
                rFMember.StartHingeNo = sH;
            }
            if (DA.GetData(9, ref eH))
            {
                rFMember.EndHingeNo = eH;
            }
            if (DA.GetData(10, ref ecc))
            {
                rFMember.EccentricityNo = ecc;
            }
            if (DA.GetData(11, ref div))
            {
                rFMember.DivisionNo = div;
            }
            if (DA.GetData(12, ref taperType))
            {
                rFMember.TaperShape = (TaperShapeType)taperType;
                if (rFMember.TaperShape == TaperShapeType.UnknownTaperShape)
                {
                    msg = "Taper Shape Type not supported. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
            }
            if (DA.GetData(13, ref kcry))
            {
                if (kcry<0 || kcry > 1000)
                {
                    msg = "Effective length factor Kcr,y out of range. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
                rFMember.Kcry = kcry;
            }
            if (DA.GetData(14, ref kcrz))
            {
                if (kcrz < 0 || kcrz > 1000)
                {
                    msg = "Effective length factor Kcr,z out of range. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
                rFMember.Kcrz = kcrz;
            }
            DA.SetData(0, rFMember);
        }
    }
}
