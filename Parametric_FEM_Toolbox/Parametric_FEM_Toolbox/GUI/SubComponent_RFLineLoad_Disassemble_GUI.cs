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
    public class SubComponent_RFLineLoad_Disassemble_GUI : SubComponent
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
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Disassemble Line Loads.");
            evaluationUnit.Icon = Properties.Resources.Disassemble_LineLoad;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {

            unit.RegisterInputParam(new Param_RFEM(), "RF Line Load", "RF LLoad", "Intput RFLineLoad.", GH_ParamAccess.item);
            // unit.Inputs[0].Parameter.Optional = true;

            unit.RegisterOutputParam(new Param_Curve(), "Base Line", "Line", "Load Application Line.");
            unit.RegisterOutputParam(new Param_Integer(), "Load Number", "No", "Index number of the RFEM object.");
            unit.RegisterOutputParam(new Param_Integer(), "Load Case", "LC", "Load Case to which the load object belongs.");
            unit.RegisterOutputParam(new Param_String(), "Comment", "Comment", "Comment.");
            unit.RegisterOutputParam(new Param_Number(), "Magnitude 1 [kN/m]", "F1", "Load Value [kN/m].");
            unit.RegisterOutputParam(new Param_Number(), "Magnitude 2 [kN/m]", "F2", "Load Value [kN/m].");
            unit.RegisterOutputParam(new Param_Number(), "Magnitude 3 [kN/m]", "F3", "Load Value [kN/m].");
            unit.RegisterOutputParam(new Param_String(), "Line List", "LineList", "Line List.");
            unit.RegisterOutputParam(new Param_Number(), "Distance A", "t1", "Distance A.");
            unit.RegisterOutputParam(new Param_Number(), "Distance B", "t2", "Distance B.");
            unit.RegisterOutputParam(new Param_Number(), "Forces [kN/m]", "F", "Array of Loads.");
            unit.RegisterOutputParam(new Param_Number(), "Relative Distances", "L", "Array of Loads.");
            unit.RegisterOutputParam(new Param_String(), "LoadType", "Type", "LoadType.");
            unit.RegisterOutputParam(new Param_String(), "LoadDirectionType", "Dir", "LoadDirectionType.");
            unit.RegisterOutputParam(new Param_String(), "LoadDistributionType", "Dist", "LoadDistributionType.");
            unit.RegisterOutputParam(new Param_String(), "LineLoadReferenceType", "RefType", "LineLoadReferenceType.");
            unit.RegisterOutputParam(new Param_Boolean(), "OverTotalLength", "Total", "Over Total Length");
            unit.RegisterOutputParam(new Param_Boolean(), "RelativeDistances", "Rel", "Relative Distances");
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
            var rfLineLoad = (RFLineLoad)inGH.Value;

            // Output
            DA.SetDataList(0, rfLineLoad.BaseLines.Select(x => x.ToCurve()));
            DA.SetData(1, rfLineLoad.No);
            DA.SetData(2, rfLineLoad.LoadCase);
            DA.SetData(3, rfLineLoad.Comment);
            DA.SetData(4, rfLineLoad.Magnitude1);
            DA.SetData(5, rfLineLoad.Magnitude2);
            DA.SetData(6, rfLineLoad.Magnitude3);
            DA.SetData(7, rfLineLoad.LineList);
            DA.SetData(8, rfLineLoad.DistanceA);
            DA.SetData(9, rfLineLoad.DistanceB);
            if (!(rfLineLoad.LoadArray == null))
            {
                var distances = new List<double>();
                var forces = new List<double>();
                for (int i = 0; i < rfLineLoad.LoadArray.GetUpperBound(0); i++)
                {
                    forces.Add(rfLineLoad.LoadArray[i, 1]);
                    distances.Add(rfLineLoad.LoadArray[i, 0] / 100);
                }
                DA.SetDataList(10, forces);
                DA.SetDataList(11, distances);
            }
            DA.SetData(12, rfLineLoad.LoadType);
            DA.SetData(13, rfLineLoad.LoadDirType);
            DA.SetData(14, rfLineLoad.LoadDistType);
            DA.SetData(15, rfLineLoad.LoadRefType);
            DA.SetData(16, rfLineLoad.OverTotalLength);
            DA.SetData(17, rfLineLoad.RelativeDistances);
        }
    }
}
