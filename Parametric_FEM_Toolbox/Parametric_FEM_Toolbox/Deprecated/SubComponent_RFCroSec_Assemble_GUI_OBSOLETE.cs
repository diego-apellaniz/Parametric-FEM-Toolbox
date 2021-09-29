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
    public class SubComponent_RFCroSec_Assemble_GUI_OBSOLETE : SubComponent
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
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Assemble Cross Sections.");
            evaluationUnit.Icon = Properties.Resources.Assemble_CroSec;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {
            unit.RegisterInputParam(new Param_String(), "Description", "Desc", "Name or Description of Cross Section.", GH_ParamAccess.item);
            unit.Inputs[0].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Material Number", "MatNo", "Number of Material assigned to the Cross-Section", GH_ParamAccess.item);
            unit.Inputs[1].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Cross Section Number", "No", "Optional index number to assign to the RFEM object.", GH_ParamAccess.item);
            unit.Inputs[2].Parameter.Optional = true;
             unit.RegisterInputParam(new Param_String(), "Comment", "Comment", "Comment.", GH_ParamAccess.item);
            unit.Inputs[3].Parameter.Optional = true;

            GH_ExtendableMenu gH_ExtendableMenu = new GH_ExtendableMenu(0, "advanced");
            gH_ExtendableMenu.Name = "Advanced";
            gH_ExtendableMenu.Collapse();
            unit.RegisterInputParam(new Param_Number(), "AxialArea [m²]", "A", "AxialArea [m²]", GH_ParamAccess.item);
            unit.Inputs[4].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "ShearAreaY [m²]", "Ay", "ShearAreaY [m²]", GH_ParamAccess.item);
            unit.Inputs[5].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "ShearAreaZ [m²]", "Az", "ShearAreaZ [m²]", GH_ParamAccess.item);
            unit.Inputs[6].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "BendingMomentY [m⁴]", "Iy", "BendingMomentY [m⁴]", GH_ParamAccess.item);
            unit.Inputs[7].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "BendingMomentZ [m⁴]", "Iz", "BendingMomentZ [m⁴]", GH_ParamAccess.item);
            unit.Inputs[8].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "TorsionMoment [m⁴]", "Jt", "TorsionMoment [m⁴]", GH_ParamAccess.item);
            unit.Inputs[9].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Rotation Angle [°]", "β", "Rotation Angle [°]", GH_ParamAccess.item);
            unit.Inputs[10].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "TemperatureLoadWidth [m]", "TempW", "TemperatureLoadWidth [m]", GH_ParamAccess.item);
            unit.Inputs[11].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "TemperatureLoadDepth [m]", "TempD", "TemperatureLoadDepth [m]", GH_ParamAccess.item);
            unit.Inputs[12].Parameter.Optional = true;
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[4]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[5]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[6]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[7]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[8]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[9]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[10]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[11]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[12]);
            unit.AddMenu(gH_ExtendableMenu);

            GH_ExtendableMenu gH_ExtendableMenu2 = new GH_ExtendableMenu(1, "modify");
            gH_ExtendableMenu2.Name = "Modify";
            gH_ExtendableMenu2.Collapse();
            unit.RegisterInputParam(new Param_RFEM(), "RF Cross Section", "RF CroSec", "Cross Section object from the RFEM model to modify", GH_ParamAccess.item);
            unit.Inputs[13].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Modify", "Modify", "Modify object?", GH_ParamAccess.item);
            unit.Inputs[14].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Delete", "Delete", "Delete object?", GH_ParamAccess.item);
            unit.Inputs[15].Parameter.Optional = true;
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[13]);
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[14]);
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[15]);
            unit.AddMenu(gH_ExtendableMenu2);

            unit.RegisterOutputParam(new Param_RFEM(), "RF Cross Section", "RF CroSec", "Output RFCroSec.");
        }

        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Blank;
            //var line = new LineCurve();
            var noIndex = 0;
            var comment = "";
            var description = "";
            var rFCroSec = new RFCroSec();
            var inRFEM = new GH_RFEM();
            var mod = false;
            var del = false;
            var rotAngle = 0.0;
            var matNo = 0;
            var A = 0.0;
            var Ay = 0.0;
            var Az = 0.0;
            var Iy = 0.0;
            var Iz = 0.0;
            var Jt = 0.0;
            var TempW = 0.0;
            var TempD = 0.0;

            if (DA.GetData(13, ref inRFEM))
            {
                rFCroSec = new RFCroSec((RFCroSec)inRFEM.Value);
                if (DA.GetData(0, ref description))
                {
                    rFCroSec.Description = description;
                }
                if (DA.GetData(1, ref matNo))
                {
                    rFCroSec.MatNo = matNo;
                }
            }
            else if (DA.GetData(0, ref description) && DA.GetData(1, ref matNo))
            {
                rFCroSec.Description = description;
                rFCroSec.MatNo = matNo;
            }
            else
            {
                msg = "Insufficient input parameters. Provide either Description and Material Number or existing RFCroSec Object. ";
                level = GH_RuntimeMessageLevel.Warning;
                return;
            }
            if (DA.GetData(14, ref mod))
            {
                rFCroSec.ToModify = mod;
            }
            if (DA.GetData(15, ref del))
            {
                rFCroSec.ToDelete = del;
            }
            if (DA.GetData(2, ref noIndex))
            {
                rFCroSec.No = noIndex;
            }
            if (DA.GetData(3, ref comment))
            {
                rFCroSec.Comment = comment;
            }
            if (DA.GetData(4, ref A))
            {
                rFCroSec.A = A;
            }
            if (DA.GetData(5, ref Ay))
            {
                rFCroSec.Ay = Ay;
            }
            if (DA.GetData(6, ref Az))
            {
                rFCroSec.Az = Az;
            }
            if (DA.GetData(7, ref Iy))
            {
                rFCroSec.Iy = Iy;
            }
            if (DA.GetData(8, ref Iz))
            {
                rFCroSec.Iz = Iz;
            }
            if (DA.GetData(9, ref Jt))
            {
                rFCroSec.Jt = Jt;
            }
            if (DA.GetData(10, ref rotAngle))
            {
                rFCroSec.RotationAngle = rotAngle;
            }
            if (DA.GetData(11, ref TempW))
            {
                rFCroSec.TempWidth = TempW;
            }
            if (DA.GetData(12, ref TempD))
            {
                rFCroSec.TempDepth = TempD;
            }
            DA.SetData(0, rFCroSec);
        }
    }
}
