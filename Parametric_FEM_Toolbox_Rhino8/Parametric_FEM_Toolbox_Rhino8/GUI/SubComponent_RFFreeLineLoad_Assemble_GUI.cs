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
    public class SubComponent_RFFreeLineLoad_Assemble_GUI : SubComponent
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
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Assemble Free Line Loads.");
            evaluationUnit.Icon = Properties.Resources.Assemble_FreeLineLoad;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {

            unit.RegisterInputParam(new Param_Line(), "Base Line", "Line", "Load Application Line.", GH_ParamAccess.item);
            unit.Inputs[0].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Load Case", "LC", "Load Case to assign the load to.", GH_ParamAccess.item);
            unit.Inputs[1].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Magnitude 1 [kN/m]", "F1", "Load Value [kN/m]", GH_ParamAccess.item);
            unit.Inputs[2].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Load Number", "No", "Optional index number to assign to the RFEM object.", GH_ParamAccess.item);
            unit.Inputs[3].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Comment", "Comment", "Comment.", GH_ParamAccess.item);
            unit.Inputs[4].Parameter.Optional = true;

            GH_ExtendableMenu gH_ExtendableMenu = new GH_ExtendableMenu(0, "advanced");
            gH_ExtendableMenu.Name = "Advanced";
            gH_ExtendableMenu.Collapse();
            unit.RegisterInputParam(new Param_String(), "Surface List", "SfcList", "Surface List", GH_ParamAccess.item);
            unit.Inputs[5].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Magnitude 2 [kN/m]", "F2", "Load Value [kN/m]", GH_ParamAccess.item);
            unit.Inputs[6].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Load Direction Type", "Dir", UtilLibrary.DescriptionRFTypes(typeof(LoadDirectionType)), GH_ParamAccess.item);
            unit.Inputs[7].EnumInput = UtilLibrary.ListRFTypes(typeof(LoadDirectionType));
            unit.Inputs[7].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Load Distribution Type", "Dist", UtilLibrary.DescriptionRFTypes(typeof(LoadDistributionType)), GH_ParamAccess.item);
            unit.Inputs[8].EnumInput = UtilLibrary.ListRFTypes(typeof(LoadDistributionType));
            unit.Inputs[8].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "ProjectionPlane", "Proj", UtilLibrary.DescriptionRFTypes(typeof(PlaneType)), GH_ParamAccess.item);
            unit.Inputs[9].EnumInput = UtilLibrary.ListRFTypes(typeof(PlaneType));
            unit.Inputs[9].Parameter.Optional = true;
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[5]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[6]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[7]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[8]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[9]);
            unit.AddMenu(gH_ExtendableMenu);

            GH_ExtendableMenu gH_ExtendableMenu2 = new GH_ExtendableMenu(1, "modify");
            gH_ExtendableMenu2.Name = "Modify";
            gH_ExtendableMenu2.Collapse();
            unit.RegisterInputParam(new Param_RFEM(), "RF Free Line Load", "RF FLLoad", "Load object from the RFEM model to modify", GH_ParamAccess.item);
            unit.Inputs[10].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Modify", "Modify", "Modify object?", GH_ParamAccess.item);
            unit.Inputs[11].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Delete", "Delete", "Delete object?", GH_ParamAccess.item);
            unit.Inputs[12].Parameter.Optional = true;
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[10]);
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[11]);
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[12]);
            unit.AddMenu(gH_ExtendableMenu2);

            unit.RegisterOutputParam(new Param_RFEM(), "RF Free Line Load", "RF FLLoad", "Output RFLineLoad.");
        }


        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Blank;

            Rhino.Geometry.Line inCurve = new Rhino.Geometry.Line();
            var noIndex = 0;
            var loadCase = 0;
            var comment = "";
            var rfLineLoad = new RFFreeLineLoad();
            var inRFEM = new GH_RFEM();
            var mod = false;
            var del = false;
            var sfcList = "";
            var dir = 0;
            var dist = 0;
            var proj = 0;
            var f1 = 0.0;
            var f2 = 0.0;


            if (DA.GetData(10, ref inRFEM))
            {
                rfLineLoad = new RFFreeLineLoad((RFFreeLineLoad)inRFEM.Value);
                if (DA.GetData(0, ref inCurve))
                {
                    rfLineLoad.Position1 = inCurve.From;
                    rfLineLoad.Position2 = inCurve.To;
                }
                if (DA.GetData(1, ref loadCase))
                {
                    rfLineLoad.LoadCase = loadCase;
                }                
            }
            else if ((DA.GetData(0, ref inCurve)) && (DA.GetData(1, ref loadCase)))
            {
                rfLineLoad = new RFFreeLineLoad(new FreeLineLoad(), loadCase);
                rfLineLoad.Position1 = inCurve.From;
                rfLineLoad.Position2 = inCurve.To;
                rfLineLoad.LoadCase = loadCase;
            }
            else
            {
                msg = "Insufficient input parameters. Provide either base line and load case or existing RFFreeLineLoad Object. ";
                level = GH_RuntimeMessageLevel.Warning;
                return;
            }
            if (DA.GetData(11, ref mod))
            {
                rfLineLoad.ToModify = mod;
            }
            if (DA.GetData(12, ref del))
            {
                rfLineLoad.ToDelete = del;
            }
            if (DA.GetData(3, ref noIndex))
            {
                rfLineLoad.No = noIndex;
            }
            if (DA.GetData(4, ref comment))
            {
                rfLineLoad.Comment = comment;
            }
            if (DA.GetData(5, ref sfcList))
            {
                rfLineLoad.SurfaceList = sfcList;
            }           
            if (DA.GetData(2, ref f1))
            {
                rfLineLoad.Magnitude1 = f1;
            }
            if (DA.GetData(6, ref f2))
            {
                rfLineLoad.Magnitude2 = f2;
            }
            // Add warning in case of null forces
            if ((rfLineLoad.Magnitude1 == 0.0) && (rfLineLoad.Magnitude2 == 0.0))
            {
                msg = "Null forces will result in an error in RFEM. ";
                level = GH_RuntimeMessageLevel.Warning;
                return;
            }
            if (DA.GetData(7, ref dir))
            {
                rfLineLoad.LoadDirType = (LoadDirectionType)dir;
            }
            if (DA.GetData(8, ref dist))
            {
                rfLineLoad.LoadDistType = (LoadDistributionType)dist;
                if (rfLineLoad.LoadDistType != LoadDistributionType.UniformType && rfLineLoad.LoadDistType != LoadDistributionType.LinearType)
                {
                    msg = "Load Distribution not supported. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
            }
            
            if (DA.GetData(9, ref proj))
            {
                rfLineLoad.ProjectionPlane = (PlaneType)proj;
            }

            DA.SetData(0, rfLineLoad);
        }
    }
}
