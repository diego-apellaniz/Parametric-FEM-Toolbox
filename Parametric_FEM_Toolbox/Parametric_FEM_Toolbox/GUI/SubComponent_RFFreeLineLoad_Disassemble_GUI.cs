using System.Linq;
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
    public class SubComponent_RFFreeLineLoad_Disassemble_GUI : SubComponent
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
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Disassemble Free Line Loads.");
            evaluationUnit.Icon = Properties.Resources.Disassemble_FreeLineLoad;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {

            unit.RegisterInputParam(new Param_RFEM(), "RF Free Line Load", "RF FLLoad", "Intput RFFreeLineLoad.", GH_ParamAccess.item);
            // unit.Inputs[0].Parameter.Optional = true;

            unit.RegisterOutputParam(new Param_Line(), "Base Line", "Line", "Load Application Line.");
            unit.RegisterOutputParam(new Param_Integer(), "Load Number", "No", "Index number of the RFEM object.");
            unit.RegisterOutputParam(new Param_Integer(), "Load Case", "LC", "Load Case to which the load object belongs.");
            unit.RegisterOutputParam(new Param_String(), "Comment", "Comment", "Comment.");
            unit.RegisterOutputParam(new Param_Number(), "Magnitude 1 [kN/m]", "F1", "Load Value [kN/m].");
            unit.RegisterOutputParam(new Param_Number(), "Magnitude 2 [kN/m]", "F2", "Load Value [kN/m].");
            unit.RegisterOutputParam(new Param_String(), "Surface List", "SfcList", "Surface List.");            
            unit.RegisterOutputParam(new Param_String(), "LoadDirectionType", "Dir", "LoadDirectionType.");
            unit.RegisterOutputParam(new Param_String(), "LoadDistributionType", "Dist", "LoadDistributionType.");
            unit.RegisterOutputParam(new Param_String(), "ProjectionPlane", "Proj", "ProjectionPlane.");
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
            var rfLineLoad = (RFFreeLineLoad)inGH.Value;

            // Output
            DA.SetData(0, rfLineLoad.ToLine());
            DA.SetData(1, rfLineLoad.No);
            DA.SetData(2, rfLineLoad.LoadCase);
            DA.SetData(3, rfLineLoad.Comment);
            DA.SetData(4, rfLineLoad.Magnitude1);
            DA.SetData(5, rfLineLoad.Magnitude2);
            DA.SetData(6, rfLineLoad.SurfaceList);
            DA.SetData(7, rfLineLoad.LoadDirType);
            DA.SetData(8, rfLineLoad.LoadDistType);
            DA.SetData(9, rfLineLoad.ProjectionPlane);
        }
    }
}
