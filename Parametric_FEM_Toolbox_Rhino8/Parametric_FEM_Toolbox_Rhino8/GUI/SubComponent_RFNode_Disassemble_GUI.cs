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
    public class SubComponent_RFNode_Disassemble_GUI : SubComponent
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
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Disassemble Nodes.");
            evaluationUnit.Icon = Properties.Resources.Disassemble_NodalLoad;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {

            unit.RegisterInputParam(new Param_RFEM(), "RF Node", "RF Node", "Input RFNode.", GH_ParamAccess.item);
            // unit.Inputs[0].Parameter.Optional = true;

            unit.RegisterOutputParam(new Param_Point(), "Point", "Point", "Point Coordinates of the RFNode.");
            unit.RegisterOutputParam(new Param_Integer(), "Node Number", "No", "Index number of the RFEM object.");
            unit.RegisterOutputParam(new Param_String(), "Comment", "Comment", "Comment.");
            unit.RegisterOutputParam(new Param_String(), "Coordinate System", "CSys", "Coordinate System");
            unit.RegisterOutputParam(new Param_Integer(), "Reference Node", "Ref Node", "Reference Node");            

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
            var rFNode = (RFNode)inGH.Value;
            // Output
            DA.SetData(0, rFNode.Location);
            DA.SetData(1, rFNode.No);
            DA.SetData(2, rFNode.Comment);
            DA.SetData(3, rFNode.CS);
            DA.SetData(4, rFNode.RefObjectNo);

        }
    }
}
