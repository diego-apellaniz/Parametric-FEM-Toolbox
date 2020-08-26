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
    public class SubComponent_RFNode_Assemble_GUI : SubComponent
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
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Assemble Nodes.");
            evaluationUnit.Icon = RFEM_daq.Properties.Resources.Assemble_Node;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {

            unit.RegisterInputParam(new Param_Point(), "Point Coordinates [m]", "Point", "Point Coordinates [m] to assemble the RFNode from.", GH_ParamAccess.item);
            unit.Inputs[0].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Node Number", "No", "Optional index number to assign to the RFEM object.", GH_ParamAccess.item);
            unit.Inputs[1].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Comment", "Comment", "Comment.", GH_ParamAccess.item);
            unit.Inputs[2].Parameter.Optional = true;

            GH_ExtendableMenu gH_ExtendableMenu2 = new GH_ExtendableMenu(0, "modify");
            gH_ExtendableMenu2.Name = "Modify";
            gH_ExtendableMenu2.Collapse();
            unit.RegisterInputParam(new Param_RFEM(), "RF Node", "RF Node", "Node object from the RFEM model to modify", GH_ParamAccess.item);
            unit.Inputs[3].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Modify", "Modify", "Modify object?", GH_ParamAccess.item);
            unit.Inputs[4].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Delete", "Delete", "Delete object?", GH_ParamAccess.item);
            unit.Inputs[5].Parameter.Optional = true;
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[3]);
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[4]);
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[5]);
            unit.AddMenu(gH_ExtendableMenu2);

            unit.RegisterOutputParam(new Param_RFEM(), "RF Node", "RF Node", "Output RFNode.");
        }

        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Blank;
            var point = new Point3d();
            var noIndex = 0;
            var comment = "";
            var rFNode = new RFNode();
            var inRFEM = new GH_RFEM();
            var mod = false;
            var del = false;
            //int newNo = 0;
            var dataprovided = false;

            if (DA.GetData(3, ref inRFEM))
            {
                rFNode = new RFNode((RFNode)inRFEM.Value);
                dataprovided = true;
            }
            if (DA.GetData(0, ref point))
            {
                rFNode.RefObjectNo = 0;
                rFNode.X = point.X;
                rFNode.Y = point.Y;
                rFNode.Z = point.Z;
                rFNode.Location = point;
                dataprovided = true;
            }
            if (!dataprovided)
            {
                msg = "Insufficient input parameters. Provide either Input Point or existing RFNode Object. ";
                level = GH_RuntimeMessageLevel.Warning;
                return;
            }
            if (DA.GetData(1, ref noIndex))
            {
                rFNode.No = noIndex;
            }
            if (DA.GetData(4, ref mod))
            {
                rFNode.ToModify = mod;
                //if (!(rFNode.No>0))
                //{
                //    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Index nummer required in order to modify or delete RFEM objects.");
                //}
            }
            if (DA.GetData(5, ref del))
            {
                rFNode.ToDelete = del;
            }
            if (DA.GetData(2, ref comment))
            {
                rFNode.Comment = comment;
            }

            DA.SetData(0, rFNode);
        }
    }
}
