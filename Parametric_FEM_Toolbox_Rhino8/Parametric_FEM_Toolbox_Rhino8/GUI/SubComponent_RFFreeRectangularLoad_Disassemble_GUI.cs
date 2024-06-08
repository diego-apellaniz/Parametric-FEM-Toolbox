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
    public class SubComponent_RFFreeRectangularLoad_Disassemble_GUI : SubComponent
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
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Disassemble Free Rectangular Loads.");
            evaluationUnit.Icon = Properties.Resources.Disassemble_PolyLoad;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {

            unit.RegisterInputParam(new Param_RFEM(), "RF Free Rectangular Load", "RF RectLoad", "Intput RFFreeRectangularLoad.", GH_ParamAccess.item);
            //unit.Inputs[0].Parameter.Optional = true;

            unit.RegisterOutputParam(new Param_Integer(), "Load Number", "No", "Index number of the RFEM object.");
            unit.RegisterOutputParam(new Param_Integer(), "Load Case", "LC", "Load Case to which the load object belongs.");
            unit.RegisterOutputParam(new Param_String(), "Comment", "Comment", "Comment.");
            unit.RegisterOutputParam(new Param_Point(), "Position 1", "Pos1", "Position 1.");
            unit.RegisterOutputParam(new Param_Point(), "Position 2", "Pos2", "Position 2.");
            unit.RegisterOutputParam(new Param_Number(), "Magnitude 1 [kN/m²]", "F1", "Load Value [kN/m²].");
            unit.RegisterOutputParam(new Param_Number(), "Magnitude 2 [kN/m²]", "F2", "Load Value [kN/m²].");
            unit.RegisterOutputParam(new Param_String(), "Surface List", "Srfc", "Surface List.");
            unit.RegisterOutputParam(new Param_String(), "Projection Type", "Projection", "Projection Type.");
            unit.RegisterOutputParam(new Param_String(), "Load Direction Type", "Dir", "Load Direction Type.");
            unit.RegisterOutputParam(new Param_String(), "Load Distribution Type", "Dist", "Load Distribution Type.");
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
            var rfPolyLoad = (RFFreeRectangularLoad)inGH.Value;

            // Output            
            DA.SetData(0, rfPolyLoad.No);
            DA.SetData(1, rfPolyLoad.LoadCase);
            DA.SetData(2, rfPolyLoad.Comment);
            DA.SetData(3, rfPolyLoad.Position1);
            DA.SetData(4, rfPolyLoad.Position2);
            DA.SetData(5, rfPolyLoad.Magnitude1);
            DA.SetData(6, rfPolyLoad.Magnitude2);
            DA.SetData(7, rfPolyLoad.SurfaceList);
            DA.SetData(8, rfPolyLoad.ProjectionType);
            DA.SetData(9, rfPolyLoad.LoadDirType);
            DA.SetData(10, rfPolyLoad.LoadDistType);
        }
    }
}
