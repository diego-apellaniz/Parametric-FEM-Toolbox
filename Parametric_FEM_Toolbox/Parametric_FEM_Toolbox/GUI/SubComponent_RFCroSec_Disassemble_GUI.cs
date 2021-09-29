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
    public class SubComponent_RFCroSec_Disassemble_GUI : SubComponent
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
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Disassemble Cross Sections.");
            evaluationUnit.Icon = Properties.Resources.Disassemble_CroSec;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {
            unit.RegisterInputParam(new Param_RFEM(), "RF Cross Section", "RF CroSec", "Input RFCroSec.", GH_ParamAccess.item);
            // unit.Inputs[0].Parameter.Optional = true;

            unit.RegisterOutputParam(new Param_String(), "Description", "Desc", "Name or Description of Cross Section.");
            unit.RegisterOutputParam(new Param_Integer(), "Cross Section Number", "No", "Optional index number to assign to the RFEM object.");
            unit.RegisterOutputParam(new Param_String(), "Comment", "Comment", "Comment.");
            unit.RegisterOutputParam(new Param_Integer(), "Material Number", "MatNo", "Number of Material assigned to the Cross-Section");
            unit.RegisterOutputParam(new Param_Number(), "AxialArea [m²]", "A", "AxialArea [m²]");
            unit.RegisterOutputParam(new Param_Number(), "ShearAreaY [m²]", "Ay", "ShearAreaY [m²]");
            unit.RegisterOutputParam(new Param_Number(), "ShearAreaZ [m²]", "Az", "ShearAreaZ [m²]");
            unit.RegisterOutputParam(new Param_Number(), "BendingMomentY [m⁴]", "Iy", "BendingMomentY [m⁴]");
            unit.RegisterOutputParam(new Param_Number(), "BendingMomentZ [m⁴]", "Iz", "BendingMomentZ [m⁴]");
            unit.RegisterOutputParam(new Param_Number(), "TorsionMoment [m⁴]", "Jt", "TorsionMoment [m⁴]");
            unit.RegisterOutputParam(new Param_Number(), "Rotation Angle [°]", "β", "Rotation Angle [°]");
            unit.RegisterOutputParam(new Param_Number(), "TemperatureLoadWidth [m]", "TempW", "TemperatureLoadWidth [m]");
            unit.RegisterOutputParam(new Param_Number(), "TemperatureLoadDepth [m]", "TempD", "TemperatureLoadDepth [m]");
            unit.RegisterOutputParam(new Param_String(), "TextID", "TextID", "TextID.");
            unit.RegisterOutputParam(new Param_Boolean(), "User Defined", "User", "User Defined.");            

            GH_ExtendableMenu gH_ExtendableMenu = new GH_ExtendableMenu(0, "Shape");
            gH_ExtendableMenu.Name = "Shape Geometry";
            gH_ExtendableMenu.Collapse();
            unit.RegisterOutputParam(new Param_Curve(), "Section Shape", "Shape", "Section Shape from the Cross Section Database of RFEM.");
            unit.RegisterOutputParam(new Param_String(), "Type", "Type", "Section Type");
            unit.RegisterOutputParam(new Param_Number(), "Height [m]", "H", "Height / Diameter");
            unit.RegisterOutputParam(new Param_Number(), "Width [m]", "W", "Width");
            unit.RegisterOutputParam(new Param_Number(), "Web Thickness [m]", "Tw", "Web Thickness / Thickness");
            unit.RegisterOutputParam(new Param_Number(), "Flange Thickness [m]", "Tf", "Flange Thickness");
            unit.RegisterOutputParam(new Param_Number(), "Inner Radius [m]", "Ri", "Fillet Radius, Inner Radius");
            unit.RegisterOutputParam(new Param_Number(), "Outer Radius [m]", "Ro", "Flange Edge Radius, Outer Radius");
            gH_ExtendableMenu.RegisterOutputPlug(unit.Outputs[15]);
            gH_ExtendableMenu.RegisterOutputPlug(unit.Outputs[16]);
            gH_ExtendableMenu.RegisterOutputPlug(unit.Outputs[17]);
            gH_ExtendableMenu.RegisterOutputPlug(unit.Outputs[18]);
            gH_ExtendableMenu.RegisterOutputPlug(unit.Outputs[19]);
            gH_ExtendableMenu.RegisterOutputPlug(unit.Outputs[20]);
            gH_ExtendableMenu.RegisterOutputPlug(unit.Outputs[21]);
            gH_ExtendableMenu.RegisterOutputPlug(unit.Outputs[22]);
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
            var rFCroSec = (RFCroSec)inGH.Value;
            // Output
            DA.SetData(0, rFCroSec.Description);
            DA.SetData(1, rFCroSec.No);
            DA.SetData(2, rFCroSec.Comment);
            DA.SetData(3, rFCroSec.MatNo);
            DA.SetData(4, rFCroSec.A);
            DA.SetData(5, rFCroSec.Ay);
            DA.SetData(6, rFCroSec.Az);
            DA.SetData(7, rFCroSec.Iy);
            DA.SetData(8, rFCroSec.Iz);
            DA.SetData(9, rFCroSec.Jt);
            DA.SetData(10, rFCroSec.RotationAngle);
            DA.SetData(11, rFCroSec.TempWidth);
            DA.SetData(12, rFCroSec.TempDepth);
            DA.SetData(13, rFCroSec.TextID);
            DA.SetData(14, rFCroSec.UserDefined);
            DA.SetDataList(15, rFCroSec.Shape);
            DA.SetData(16, rFCroSec.Type);
            DA.SetData(17, rFCroSec.H);
            DA.SetData(18, rFCroSec.W);
            DA.SetData(19, rFCroSec.Tw);
            DA.SetData(20, rFCroSec.Tf);
            DA.SetData(21, rFCroSec.Ri);
            DA.SetData(22, rFCroSec.Ro);
        }
    }
}
