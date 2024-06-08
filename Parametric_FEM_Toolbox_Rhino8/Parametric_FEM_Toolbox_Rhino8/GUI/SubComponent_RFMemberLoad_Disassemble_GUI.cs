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
    public class SubComponent_RFMemberLoad_Disassemble_GUI : SubComponent
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
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Disassemble Member Loads.");
            evaluationUnit.Icon = Properties.Resources.Disassemble_MemberLoad;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {

            unit.RegisterInputParam(new Param_RFEM(), "RF Member Load", "RF MLoad", "Intput RFMemberLoad.", GH_ParamAccess.item);
            //unit.Inputs[0].Parameter.Optional = true;

            unit.RegisterOutputParam(new Param_String(), "Member List", "MemberList", "Member List.");
            unit.RegisterOutputParam(new Param_Integer(), "Load Number", "No", "Index number of the RFEM object.");
            unit.RegisterOutputParam(new Param_Integer(), "Load Case", "LC", "Load Case to which the load object belongs.");
            unit.RegisterOutputParam(new Param_String(), "Comment", "Comment", "Comment.");
            unit.RegisterOutputParam(new Param_Number(), "Magnitude 1 [kN/m]", "F1", "Load Value [kN/m].");
            unit.RegisterOutputParam(new Param_Number(), "Magnitude 2 [kN/m]", "F2", "Load Value [kN/m].");
            unit.RegisterOutputParam(new Param_Number(), "Magnitude 3 [kN/m]", "F3", "Load Value [kN/m].");
            unit.RegisterOutputParam(new Param_Number(), "Magnitude 4 [°C]", "T4", "Load Value [°C].");
            unit.RegisterOutputParam(new Param_Number(), "Magnitude 5 [°C]", "T5", "Load Value [°C].");
            unit.RegisterOutputParam(new Param_Number(), "Magnitude 6 [°C]", "T6", "Load Value [°C].");
            unit.RegisterOutputParam(new Param_Number(), "Distance A", "t1", "Distance A.");
            unit.RegisterOutputParam(new Param_Number(), "Distance B", "t2", "Distance B.");
            unit.RegisterOutputParam(new Param_Number(), "Forces [kN/m]", "F", "Array of Loads.");
            unit.RegisterOutputParam(new Param_Number(), "Relative Distances", "L", "Array of Loads.");
            unit.RegisterOutputParam(new Param_String(), "LoadType", "Type", "LoadType.");
            unit.RegisterOutputParam(new Param_String(), "LoadDirectionType", "Dir", "LoadDirectionType.");
            unit.RegisterOutputParam(new Param_String(), "LoadDistributionType", "Dist", "LoadDistributionType.");
            unit.RegisterOutputParam(new Param_String(), "MemberLoadReferenceType", "RefType", "MemberLoadReferenceType.");
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
            var rfMemberLoad = (RFMemberLoad)inGH.Value;

            // Output
            DA.SetData(0, rfMemberLoad.MemberList);
            DA.SetData(1, rfMemberLoad.No);
            DA.SetData(2, rfMemberLoad.LoadCase);
            DA.SetData(3, rfMemberLoad.Comment);
            DA.SetData(4, rfMemberLoad.Magnitude1);
            DA.SetData(5, rfMemberLoad.Magnitude2);
            DA.SetData(6, rfMemberLoad.Magnitude3);
            DA.SetData(7, rfMemberLoad.Magnitude4);
            DA.SetData(8, rfMemberLoad.Magnitude5);
            DA.SetData(9, rfMemberLoad.Magnitude6);
            DA.SetData(10, rfMemberLoad.DistanceA);
            DA.SetData(11, rfMemberLoad.DistanceB);
            if (!(rfMemberLoad.LoadArray == null))
            {
                var distances = new List<double>();
                var forces = new List<double>();
                for (int i = 0; i < rfMemberLoad.LoadArray.GetUpperBound(0); i++)
                {
                    forces.Add(rfMemberLoad.LoadArray[i, 1]);
                    distances.Add(rfMemberLoad.LoadArray[i, 0] / 100);
                }
                DA.SetDataList(12, forces);
                DA.SetDataList(13, distances);
            }
            DA.SetData(14, rfMemberLoad.LoadType);
            DA.SetData(15, rfMemberLoad.LoadDirType);
            DA.SetData(16, rfMemberLoad.LoadDistType);
            DA.SetData(17, rfMemberLoad.LoadRefType);
            DA.SetData(18, rfMemberLoad.OverTotalLength);
            DA.SetData(19, rfMemberLoad.RelativeDistances);
        }
    }
}
