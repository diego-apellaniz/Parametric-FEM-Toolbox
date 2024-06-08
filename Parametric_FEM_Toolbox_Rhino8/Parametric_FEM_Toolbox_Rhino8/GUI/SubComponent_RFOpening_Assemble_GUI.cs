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
    public class SubComponent_RFOpening_Assemble_GUI : SubComponent
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
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Assemble Openings.");
            evaluationUnit.Icon = Properties.Resources.Assemble_Opening;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {

            unit.RegisterInputParam(new Param_Surface(), "Surface", "Sfc", "Surface to assemble the RFOpening from.", GH_ParamAccess.item);
            unit.Inputs[0].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Opening Number", "No", "Optional index number to assign to the RFEM object.", GH_ParamAccess.item);
            unit.Inputs[1].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "In Surface No", "InSrfcNo", "Index number assigned to the surface this opening belongs to.", GH_ParamAccess.item);
            unit.Inputs[2].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Comment", "Comment", "Comment.", GH_ParamAccess.item);
            unit.Inputs[3].Parameter.Optional = true;

            GH_ExtendableMenu gH_ExtendableMenu = new GH_ExtendableMenu(0, "advanced");
            gH_ExtendableMenu.Name = "Advanced";
            gH_ExtendableMenu.Collapse();
            unit.RegisterInputParam(new Param_String(), "Boundary Line List", "Bound", "Boundary Line List", GH_ParamAccess.item);
            unit.Inputs[4].Parameter.Optional = true;
            //unit.RegisterInputParam(new Param_Integer(), "Interpolated Points", "n", "Number of interpolated points for NURBS", GH_ParamAccess.item);
            //unit.Inputs[5].Parameter.Optional = true;
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[4]);
            //gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[5]);
            unit.AddMenu(gH_ExtendableMenu);

            GH_ExtendableMenu gH_ExtendableMenu2 = new GH_ExtendableMenu(1, "modify");
            gH_ExtendableMenu2.Name = "Modify";
            gH_ExtendableMenu2.Collapse();
            unit.RegisterInputParam(new Param_RFEM(), "RF Opening", "RF Opening", "Opening object from the RFEM model to modify", GH_ParamAccess.item);
            unit.Inputs[5].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Modify", "Modify", "Modify object?", GH_ParamAccess.item);
            unit.Inputs[6].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Delete", "Delete", "Delete object?", GH_ParamAccess.item);
            unit.Inputs[7].Parameter.Optional = true;
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[5]);
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[6]);
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[7]);
            unit.AddMenu(gH_ExtendableMenu2);

            unit.RegisterOutputParam(new Param_RFEM(), "RF Opening", "RF Opening", "Output RFOpening.");
        }

        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Blank;
            //var line = new LineCurve();
            Brep inSrfc = null;
            var noIndex = 0;
            var comment = "";
            var rfOpening = new RFOpening();
            var inRFEM = new GH_RFEM();
            var rfEdges = new List<RFLine>();
            var mod = false;
            var del = false;
            var boundList = "";
            var inSrfcNo = 0;
            //int newNo = 0;

            if (DA.GetData(5, ref inRFEM))
            {
                rfOpening = new RFOpening((RFOpening)inRFEM.Value);
                if (DA.GetData(0, ref inSrfc))
                {
                    Component_RFOpening.SetGeometry(inSrfc, ref rfOpening);
                }
            }
            else if (DA.GetData(0, ref inSrfc))
            {
                  Component_RFOpening.SetGeometry(inSrfc, ref rfOpening);
            }
            else if (DA.GetData(4, ref boundList))
            {
                rfOpening.BoundaryLineList = boundList;
            }
            else
            {
                msg = "Insufficient input parameters. Provide either Input Opening Shape, Boundary Lines List or existing RFOpening Object. ";
                level = GH_RuntimeMessageLevel.Warning;
                return;
            }
            if (DA.GetData(6, ref mod))
            {
                rfOpening.ToModify = mod;
            }
            if (DA.GetData(7, ref del))
            {
                rfOpening.ToDelete = del;
            }
            if (DA.GetData(1, ref noIndex))
            {
                rfOpening.No = noIndex;
            }
            if (DA.GetData(3, ref comment))
            {
                rfOpening.Comment = comment;
            }
            if (DA.GetData(2, ref inSrfcNo))
            {
                rfOpening.InSurfaceNo = inSrfcNo;
            }
            if (DA.GetData(4, ref boundList))
            {
                rfOpening.BoundaryLineList = boundList;
            }
            DA.SetData(0, rfOpening);
        }
    }
}