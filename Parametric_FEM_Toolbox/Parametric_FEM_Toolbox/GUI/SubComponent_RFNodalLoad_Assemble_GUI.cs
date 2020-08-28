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
    public class SubComponent_RFNodalLoad_Assemble_GUI : SubComponent
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
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Assemble Nodal Loads.");
            evaluationUnit.Icon = Properties.Resources.Assemble_NodalLoad;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {
            unit.RegisterInputParam(new Param_Point(), "Location", "Loc", "Load Application point.", GH_ParamAccess.item);
            unit.Inputs[0].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Load Case", "LC", "Load Case to assign the load to.", GH_ParamAccess.item);
            unit.Inputs[1].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Vector(), "Force [kN]", "F", "Load Force [kN]", GH_ParamAccess.item);
            unit.Inputs[2].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Vector(), "Moment [kNm]", "M", "Load Moment [kN]", GH_ParamAccess.item);
            unit.Inputs[3].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Load Number", "No", "Optional index number to assign to the RFEM object.", GH_ParamAccess.item);
            unit.Inputs[4].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Comment", "Comment", "Comment.", GH_ParamAccess.item);
            unit.Inputs[5].Parameter.Optional = true;

            GH_ExtendableMenu gH_ExtendableMenu = new GH_ExtendableMenu(0, "advanced");
            gH_ExtendableMenu.Name = "Advanced";
            gH_ExtendableMenu.Collapse();
            unit.RegisterInputParam(new Param_String(), "Node List", "NodeList", "Node List", GH_ParamAccess.item);
            unit.Inputs[6].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "LoadDefinitionType", "Def", UtilLibrary.DescriptionRFTypes(typeof(LoadDefinitionType)), GH_ParamAccess.item);
            unit.Inputs[7].EnumInput = UtilLibrary.ListRFTypes(typeof(LoadDefinitionType));
            unit.Inputs[7].Parameter.Optional = true;
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[6]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[7]);
            unit.AddMenu(gH_ExtendableMenu);

            GH_ExtendableMenu gH_ExtendableMenu2 = new GH_ExtendableMenu(1, "modify");
            gH_ExtendableMenu2.Name = "Modify";
            gH_ExtendableMenu2.Collapse();
            unit.RegisterInputParam(new Param_RFEM(), "RF Nodal Load", "RF NLoad", "Load object from the RFEM model to modify", GH_ParamAccess.item);
            unit.Inputs[8].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Modify", "Modify", "Modify object?", GH_ParamAccess.item);
            unit.Inputs[9].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Delete", "Delete", "Delete object?", GH_ParamAccess.item);
            unit.Inputs[10].Parameter.Optional = true;
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[8]);
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[9]);
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[10]);
            unit.AddMenu(gH_ExtendableMenu2);

            unit.RegisterOutputParam(new Param_RFEM(), "RF Nodal Load", "RF NLoad", "Output RFNodalLoad.");
        }

        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Blank;
            //var line = new LineCurve();
            var point = new Point3d();
            var noIndex = 0;
            var loadCase = 0;
            var comment = "";
            var rfNodalLoad = new RFNodalLoad();
            var inRFEM = new GH_RFEM();
            var mod = false;
            var del = false;
            var force = new Vector3d();
            var moment = new Vector3d();
            var nodeList = "";
            var definition = 0;
            //int newNo = 0;


            if (DA.GetData(8, ref inRFEM))
            {
                rfNodalLoad = new RFNodalLoad((RFNodalLoad)inRFEM.Value);
                if (DA.GetData(0, ref point))
                {
                    var inPoints = new List<Point3d>();
                    inPoints.Add(point);
                    rfNodalLoad = new RFNodalLoad(rfNodalLoad, inPoints, rfNodalLoad.LoadCase);
                }
                if (DA.GetData(1, ref loadCase))
                {
                    rfNodalLoad.LoadCase = loadCase;
                }
            }
            else if ((DA.GetData(0, ref point)) && (DA.GetData(1, ref loadCase)))
            {
                var inPoints = new List<Point3d>();
                inPoints.Add(point);
                rfNodalLoad = new RFNodalLoad(new NodalLoad(), inPoints, loadCase);
                rfNodalLoad.LoadDefType = LoadDefinitionType.ByComponentsType;
            }
            else if ((DA.GetData(6, ref nodeList)) && (DA.GetData(1, ref loadCase)))
            {
                rfNodalLoad = new RFNodalLoad();
                rfNodalLoad.LoadCase = loadCase;
                rfNodalLoad.NodeList = nodeList;
                rfNodalLoad.LoadDefType = LoadDefinitionType.ByComponentsType;
            }
            else
            {
                msg = "Insufficient input parameters. Provide either base point and load case or node list and load case or existing RFNodalLoad Object. ";
                level = GH_RuntimeMessageLevel.Warning;
                return;
            }
            if (DA.GetData(9, ref mod))
            {
                rfNodalLoad.ToModify = mod;
            }
            if (DA.GetData(10, ref del))
            {
                rfNodalLoad.ToDelete = del;
            }
            if (DA.GetData(4, ref noIndex))
            {
                rfNodalLoad.No = noIndex;
            }
            if (DA.GetData(5, ref comment))
            {
                rfNodalLoad.Comment = comment;
            }
            if (DA.GetData(2, ref force))
            {
                rfNodalLoad.Force = force;
            }
            if (DA.GetData(3, ref moment))
            {
                rfNodalLoad.Moment = moment;
            }
            // Add warning in case of null forces
            if ((rfNodalLoad.Force.Length == 0.0) && (rfNodalLoad.Moment.Length == 0))
            {
                msg = "Null forces will result in an error in RFEM. ";
                level = GH_RuntimeMessageLevel.Warning;
            }
            if (DA.GetData(6, ref nodeList))
            {
                rfNodalLoad.NodeList = nodeList;
            }
            if (DA.GetData(7, ref definition))
            {
                rfNodalLoad.LoadDefType = (LoadDefinitionType)definition;
                if (rfNodalLoad.LoadDefType == LoadDefinitionType.UnknownLoadDefinitionType)
                {                    
                    msg = "Load Definition not supported. ";
                    level = GH_RuntimeMessageLevel.Warning;
                }
            }
            DA.SetData(0, rfNodalLoad);
        }
    }
}
