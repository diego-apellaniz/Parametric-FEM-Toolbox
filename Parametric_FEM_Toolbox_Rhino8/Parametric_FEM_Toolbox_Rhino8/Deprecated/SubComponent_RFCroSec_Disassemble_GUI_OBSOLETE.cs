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

namespace Parametric_FEM_Toolbox.Deprecated
{
    public class SubComponent_RFCroSec_Disassemble_GUI_OBSOLETE : SubComponent
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
            unit.RegisterInputParam(new GUI.Param_RFEM(), "RF Cross Section", "RF CroSec", "Input RFCroSec.", GH_ParamAccess.item);
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
        }
    }
}
