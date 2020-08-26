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
    public class SubComponent_RFSurfaceLoad_Disassemble_GUI : SubComponent
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
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Disassemble Surface Loads.");
            evaluationUnit.Icon = RFEM_daq.Properties.Resources.Disassemble_SurfaceLoad;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {

            unit.RegisterInputParam(new Param_RFEM(), "RF Surface Load", "RF SLoad", "Intput RFSurfaceLoad.", GH_ParamAccess.item);
            // unit.Inputs[0].Parameter.Optional = true;

            unit.RegisterOutputParam(new Param_String(), "Surface List", "Srfc", "Surface List.");
            unit.RegisterOutputParam(new Param_Integer(), "Load Number", "No", "Index number of the RFEM object.");
            unit.RegisterOutputParam(new Param_Integer(), "Load Case", "LC", "Load Case to which the load object belongs.");
            unit.RegisterOutputParam(new Param_String(), "Comment", "Comment", "Comment.");
            unit.RegisterOutputParam(new Param_Number(), "Magnitude 1 [kN/m²]", "F1", "Load Value [kN/m²].");
            unit.RegisterOutputParam(new Param_Number(), "Magnitude 2 [kN/m²]", "F2", "Load Value [kN/m²].");
            unit.RegisterOutputParam(new Param_Number(), "Magnitude 3 [kN/m²]", "F3", "Load Value [kN/m²].");
            unit.RegisterOutputParam(new Param_Number(), "Magnitude 4 [°C]", "T4", "Load Value [°C].");
            unit.RegisterOutputParam(new Param_Number(), "Magnitude 5 [°C]", "T5", "Load Value [°C].");
            unit.RegisterOutputParam(new Param_Number(), "Magnitude 6 [°C]", "T6", "Load Value [°C].");
            unit.RegisterOutputParam(new Param_Integer(), "Node1No", "Node1", "Node Index (useful for surface loads with linear distribution)");
            unit.RegisterOutputParam(new Param_Integer(), "Node2No", "Node2", "Node Index (useful for surface loads with linear distribution)");
            unit.RegisterOutputParam(new Param_Integer(), "Node2No", "Node2", "Node Index (useful for surface loads with linear distribution)");
            unit.RegisterOutputParam(new Param_String(), "LoadType", "Type", "LoadType.");
            unit.RegisterOutputParam(new Param_String(), "LoadDirectionType", "Dir", "LoadDirectionType.");
            unit.RegisterOutputParam(new Param_String(), "LoadDistributionType", "Dist", "LoadDistributionType.");
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
            var rfSurfaceLoad = (RFSurfaceLoad)inGH.Value;

            // Output
            DA.SetData(0, rfSurfaceLoad.SurfaceList);
            DA.SetData(1, rfSurfaceLoad.No);
            DA.SetData(2, rfSurfaceLoad.LoadCase);
            DA.SetData(3, rfSurfaceLoad.Comment);
            DA.SetData(4, rfSurfaceLoad.Magnitude1);
            DA.SetData(5, rfSurfaceLoad.Magnitude2);
            DA.SetData(6, rfSurfaceLoad.Magnitude3);
            DA.SetData(7, rfSurfaceLoad.Magnitude4);
            DA.SetData(8, rfSurfaceLoad.Magnitude5);
            DA.SetData(9, rfSurfaceLoad.Magnitude6);
            DA.SetData(10, rfSurfaceLoad.Node1No);
            DA.SetData(11, rfSurfaceLoad.Node2No);
            DA.SetData(12, rfSurfaceLoad.Node3No);
            DA.SetData(13, rfSurfaceLoad.LoadType);
            DA.SetData(14, rfSurfaceLoad.LoadDirType);
            DA.SetData(15, rfSurfaceLoad.LoadDistType);
        }
    }
}
