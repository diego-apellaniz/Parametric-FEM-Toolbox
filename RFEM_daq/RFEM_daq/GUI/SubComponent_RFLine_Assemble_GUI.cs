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
    public class SubComponent_RFLine_Assemble_GUI : SubComponent
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
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Assemble Lines.");
            evaluationUnit.Icon = RFEM_daq.Properties.Resources.Assemble_Line;
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
            unit.RegisterInputParam(new Param_Integer(), "Line Number", "No", "Optional index number to assign to the RFEM object.", GH_ParamAccess.item);
            unit.Inputs[1].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Comment", "Comment", "Comment.", GH_ParamAccess.item);
            unit.Inputs[2].Parameter.Optional = true;

            GH_ExtendableMenu gH_ExtendableMenu = new GH_ExtendableMenu(0, "advanced");
            gH_ExtendableMenu.Name = "Advanced";
            gH_ExtendableMenu.Collapse();
            unit.RegisterInputParam(new Param_String(), "NodeList", "NodeList", "Node List", GH_ParamAccess.item);
            unit.Inputs[3].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Line Type", "Type", UtilLibrary.DescriptionRFTypes(typeof(LineType)), GH_ParamAccess.item);
            unit.Inputs[4].EnumInput = UtilLibrary.ListRFTypes(typeof(LineType));
            unit.Inputs[4].Parameter.Optional = true;            
            unit.RegisterInputParam(new Param_Number(), "Rotation Angle [°]", "β", "Rotation Angle [°]", GH_ParamAccess.item);
            unit.Inputs[5].Parameter.Optional = true;
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[3]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[4]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[5]);
            unit.AddMenu(gH_ExtendableMenu);

            GH_ExtendableMenu gH_ExtendableMenu2 = new GH_ExtendableMenu(1, "modify");
            gH_ExtendableMenu2.Name = "Modify";
            gH_ExtendableMenu2.Collapse();
            unit.RegisterInputParam(new Param_RFEM(), "RF Line", "RF Line", "Line object from the RFEM model to modify", GH_ParamAccess.item);
            unit.Inputs[6].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Modify", "Modify", "Modify object?", GH_ParamAccess.item);
            unit.Inputs[7].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Delete", "Delete", "Delete object?", GH_ParamAccess.item);
            unit.Inputs[8].Parameter.Optional = true;
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[6]);
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[7]);
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[8]);
            unit.AddMenu(gH_ExtendableMenu2);

            unit.RegisterOutputParam(new Param_RFEM(), "RF Line", "RF Line", "Output RFNode.");
        }

        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Blank;
            //var line = new LineCurve();
            Curve inCurve = null;
            var noIndex = 0;
            var comment = "";
            var rFLine = new RFLine();
            var inRFEM = new GH_RFEM();
            var mod = false;
            var del = false;
            var nodeList = "";
            var lineType = 0;
            var rotAngle = 0.0;
            //int intPoints = 4;
            //int newNo = 0;

            if (DA.GetData(6, ref inRFEM))
            {
                rFLine = new RFLine((RFLine)inRFEM.Value);
            }
            else if (DA.GetData(0, ref inCurve))
            {
                Component_RFLine.SetGeometry(inCurve, ref rFLine);
            }
            else
            {
                msg = "Insufficient input parameters. Provide either Input Curve or existing RFLine Object. ";
                level = GH_RuntimeMessageLevel.Warning;
                return;
            }
            if (DA.GetData(7, ref mod))
            {
                rFLine.ToModify = mod;
            }
            if (DA.GetData(8, ref del))
            {
                rFLine.ToDelete = del;
            }
            if (DA.GetData(1, ref noIndex))
            {
                rFLine.No = noIndex;
            }
            if (DA.GetData(2, ref comment))
            {
                rFLine.Comment = comment;
            }
            if (DA.GetData(3, ref nodeList))
            {
                rFLine.NodeList = nodeList;
            }
            if (DA.GetData(4, ref lineType))
            {
                rFLine.Type = (LineType)lineType;
                if (rFLine.Type == LineType.UnknownLineType)
                {
                    msg = "Line Type not supported. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
            }
            if (DA.GetData(5, ref rotAngle))
            {
                rFLine.RotationType = RotationType.Angle;
                rFLine.RotationAngle = rotAngle;
            }

            DA.SetData(0, rFLine);
        }
    }
}
