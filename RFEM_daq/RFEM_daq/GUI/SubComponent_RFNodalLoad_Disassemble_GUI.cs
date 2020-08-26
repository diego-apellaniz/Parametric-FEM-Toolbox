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
    public class SubComponent_RFNodalLoad_Disassemble_GUI : SubComponent
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
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Disassemble Nodal Loads.");
            evaluationUnit.Icon = RFEM_daq.Properties.Resources.Disassemble_NodalLoad;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {

            unit.RegisterInputParam(new Param_RFEM(), "RF Nodal Load", "RF NLoad", "Intput RFNodalLoad.", GH_ParamAccess.item);
            // unit.Inputs[0].Parameter.Optional = true;

            unit.RegisterOutputParam(new Param_Point(), "Location", "Loc", "Load Application point.");
            unit.RegisterOutputParam(new Param_Integer(), "Load Number", "No", "Index number of the RFEM object.");
            unit.RegisterOutputParam(new Param_Integer(), "Load Case", "LC", "Load Case to which the load object belongs.");
            unit.RegisterOutputParam(new Param_Vector(), "Force [kN]", "F", "Load Force [kN]");
            unit.RegisterOutputParam(new Param_Vector(), "Moment [kNm]", "M", "Load Moment [kN]");            
            unit.RegisterOutputParam(new Param_String(), "Comment", "Comment", "Comment.");
            unit.RegisterOutputParam(new Param_String(), "Node List", "NodeList", "List of nodes the support is attached to.");
            unit.RegisterOutputParam(new Param_String(), "LoadDefinitionType", "Def", "LoadDefinitionType.");

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
            var rfNodalLoad = (RFNodalLoad)inGH.Value;

            // Output
            DA.SetDataList(0, rfNodalLoad.Location);
            DA.SetData(1, rfNodalLoad.No);
            DA.SetData(2, rfNodalLoad.LoadCase);
            DA.SetData(3, rfNodalLoad.Force);
            DA.SetData(4, rfNodalLoad.Moment);
            DA.SetData(5, rfNodalLoad.Comment);
            DA.SetData(6, rfNodalLoad.NodeList);
            DA.SetData(7, rfNodalLoad.LoadDefType);

        }
    }
}
