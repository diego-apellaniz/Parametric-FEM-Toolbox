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
    public class SubComponent_RFOpening_Disassemble_GUI : SubComponent
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
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Disassemble Openings.");
            evaluationUnit.Icon = Properties.Resources.Disassemble_Opening;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {

            unit.RegisterInputParam(new Param_RFEM(), "RF Opening", "RF Opening", "Input RFOpening.", GH_ParamAccess.item);
            // unit.Inputs[0].Parameter.Optional = true;

            unit.RegisterOutputParam(new Param_Surface(), "Surface", "Sfc", "Surface related to the RFSurface object.");
            unit.RegisterOutputParam(new Param_Integer(), "Opening Number", "No", "Optional index number to assign to the RFEM object.");
            unit.RegisterOutputParam(new Param_Integer(), "In Surface No", "InSfcNo", "Index number assigned to the surface this opening belongs to.");
            unit.RegisterOutputParam(new Param_String(), "Comment", "Comment", "Comment.");
            unit.RegisterOutputParam(new Param_String(), "Boundary Lines List", "BoundList", "List of index numbers of boundary lines");

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
            var rfOpening = (RFOpening)inGH.Value;
            // Output
            if (rfOpening.IsPlanar())
            {
                DA.SetData(0, rfOpening.ToPlanarBrep());
            }
            DA.SetData(1, rfOpening.No);
            DA.SetData(2, rfOpening.InSurfaceNo);
            DA.SetData(3, rfOpening.Comment);
            DA.SetData(4, rfOpening.BoundaryLineList);

        }
    }
}
