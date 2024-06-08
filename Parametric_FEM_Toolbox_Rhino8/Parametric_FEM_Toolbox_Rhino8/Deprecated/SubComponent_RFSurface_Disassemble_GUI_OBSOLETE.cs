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
    public class SubComponent_RFSurface_Disassemble_GUI_OBSOLETE : SubComponent
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
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Disassemble Surfaces.");
            evaluationUnit.Icon = Properties.Resources.Disassemble_Surface;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {

            unit.RegisterInputParam(new Param_RFEM(), "RF Surface", "RF Surface", "Input RFSurface.", GH_ParamAccess.item);
            // unit.Inputs[0].Parameter.Optional = true;

            unit.RegisterOutputParam(new Param_Surface(), "Surface", "Srfc", "Surface related to the RFSurface object.");
            unit.RegisterOutputParam(new Param_Integer(), "Surface Number", "No", "Index number of the RFEM object.");
            unit.RegisterOutputParam(new Param_Integer(), "Material Number", "Mat", "Material index number.");
            unit.RegisterOutputParam(new Param_Number(), "Thickness [m]", "H", "Surface thickness.");
            unit.RegisterOutputParam(new Param_String(), "Comment", "Comment", "Comment.");
            unit.RegisterOutputParam(new Param_String(), "Boundary Lines List", "BoundList", "List of index numbers of boundary lines");
            unit.RegisterOutputParam(new Param_String(), "Geometry Type", "GType", "Geometry Type");
            unit.RegisterOutputParam(new Param_String(), "Thickness Type", "ThType", "Thickness Type");
            unit.RegisterOutputParam(new Param_String(), "Stiffness Type", "SType", "Stiffness Type");
            unit.RegisterOutputParam(new Param_Number(), "Area [m²]", "A", "Surface area");

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
            var rfSurface = (RFSurface)inGH.Value;
            // Output
            DA.SetData(0, rfSurface.ToBrep());
            DA.SetData(1, rfSurface.No);
            DA.SetData(2, rfSurface.MaterialNo);
            DA.SetData(3, rfSurface.Thickness);
            DA.SetData(4, rfSurface.Comment);
            DA.SetData(5, rfSurface.BoundaryLineList);
            DA.SetData(6, rfSurface.GeometryType);
            DA.SetData(7, rfSurface.ThicknessType);
            DA.SetData(8, rfSurface.StiffnessType);
            DA.SetData(9, rfSurface.Area);

        }
    }
}
