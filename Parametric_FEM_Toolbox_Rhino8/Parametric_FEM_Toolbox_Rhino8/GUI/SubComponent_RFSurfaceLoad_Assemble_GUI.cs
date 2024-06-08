﻿using Grasshopper.Kernel;
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
    public class SubComponent_RFSurfaceLoad_Assemble_GUI : SubComponent
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
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Assemble Surface Loads.");
            evaluationUnit.Icon = Properties.Resources.Assemble_SurfaceLoad;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {

            unit.RegisterInputParam(new Param_String(), "Surface List", "Srfc", "Surface List.", GH_ParamAccess.item);
            unit.Inputs[0].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Load Case", "LC", "Load Case to assign the load to.", GH_ParamAccess.item);
            unit.Inputs[1].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Magnitude 1 [kN/m²]", "F1", "Load Value [kN/m²]", GH_ParamAccess.item);
            unit.Inputs[2].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Load Number", "No", "Optional index number to assign to the RFEM object.", GH_ParamAccess.item);
            unit.Inputs[3].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Comment", "Comment", "Comment.", GH_ParamAccess.item);
            unit.Inputs[4].Parameter.Optional = true;

            GH_ExtendableMenu gH_ExtendableMenu = new GH_ExtendableMenu(0, "advanced");
            gH_ExtendableMenu.Name = "Advanced";
            gH_ExtendableMenu.Collapse();
            unit.RegisterInputParam(new Param_Number(), "Magnitude 2 [kN/m²]", "F2", "Load Value [kN/m²]", GH_ParamAccess.item);
            unit.Inputs[5].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Magnitude 3 [kN/m²]", "F3", "Load Value [kN/m²]", GH_ParamAccess.item);
            unit.Inputs[6].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Magnitude 4 [°C]", "T4", "Load Value [°C]", GH_ParamAccess.item);
            unit.Inputs[7].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Magnitude 5 [°C]", "T5", "Load Value [°C]", GH_ParamAccess.item);
            unit.Inputs[8].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Magnitude 6 [°C]", "T6", "Load Value [°C]", GH_ParamAccess.item);
            unit.Inputs[9].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Node1No", "Node1", "Node Index (useful for surface loads with linear distribution)", GH_ParamAccess.item);
            unit.Inputs[10].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Node2No", "Node2", "Node Index (useful for surface loads with linear distribution)", GH_ParamAccess.item);
            unit.Inputs[11].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Node3No", "Node3", "Node Index (useful for surface loads with linear distribution)", GH_ParamAccess.item);
            unit.Inputs[12].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Load Type", "Type", UtilLibrary.DescriptionRFTypes(typeof(LoadType)), GH_ParamAccess.item);
            unit.Inputs[13].EnumInput = UtilLibrary.ListRFTypes(typeof(LoadType));
            unit.Inputs[13].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Load Direction Type", "Dir", UtilLibrary.DescriptionRFTypes(typeof(LoadDirectionType)), GH_ParamAccess.item);
            unit.Inputs[14].EnumInput = UtilLibrary.ListRFTypes(typeof(LoadDirectionType));
            unit.Inputs[14].Parameter.Optional = true;            
            unit.RegisterInputParam(new Param_Integer(), "Load Distribution Type", "Dist", UtilLibrary.DescriptionRFTypes(typeof(LoadDistributionType)), GH_ParamAccess.item);
            unit.Inputs[15].EnumInput = UtilLibrary.ListRFTypes(typeof(LoadDistributionType));
            unit.Inputs[15].Parameter.Optional = true;
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[5]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[6]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[7]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[8]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[9]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[10]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[11]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[12]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[13]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[14]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[15]);
            unit.AddMenu(gH_ExtendableMenu);

            GH_ExtendableMenu gH_ExtendableMenu2 = new GH_ExtendableMenu(1, "modify");
            gH_ExtendableMenu2.Name = "Modify";
            gH_ExtendableMenu2.Collapse();
            unit.RegisterInputParam(new Param_RFEM(), "RF Surface Load", "RF SLoad", "Load object from the RFEM model to modify", GH_ParamAccess.item);
            unit.Inputs[16].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Modify", "Modify", "Modify object?", GH_ParamAccess.item);
            unit.Inputs[17].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Delete", "Delete", "Delete object?", GH_ParamAccess.item);
            unit.Inputs[18].Parameter.Optional = true;
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[16]);
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[17]);
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[18]);
            unit.AddMenu(gH_ExtendableMenu2);

            unit.RegisterOutputParam(new Param_RFEM(), "RF Surface Load", "RF SLoad", "Output RFSurfaceLoad.");
        }


        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Blank;

            var noIndex = 0;
            var loadCase = 0;
            var comment = "";
            var rfSrfcLoad = new RFSurfaceLoad();
            var inRFEM = new GH_RFEM();
            var mod = false;
            var del = false;
            var srfclist = "";
            var type = 0;
            var dir = 0;
            var dist = 0;
            var f1 = 0.0;
            var f2 = 0.0;
            var f3 = 0.0;
            var f4 = 0.0;
            var f5 = 0.0;
            var f6 = 0.0;
            var node1 = 0;
            var node2 = 0;
            var node3 = 0;

            if (DA.GetData(16, ref inRFEM))
            {
                rfSrfcLoad = new RFSurfaceLoad((RFSurfaceLoad)inRFEM.Value);
                if (DA.GetData(0, ref srfclist))
                {
                    rfSrfcLoad.SurfaceList = srfclist;
                }
                if (DA.GetData(1, ref loadCase))
                {
                    rfSrfcLoad.LoadCase = loadCase;
                }
            }
            else if  (DA.GetData(0, ref srfclist) && (DA.GetData(1, ref loadCase)))
            {
                rfSrfcLoad = new RFSurfaceLoad();
                rfSrfcLoad.LoadCase = loadCase;
                rfSrfcLoad.SurfaceList = srfclist;
                rfSrfcLoad.LoadType = LoadType.ForceType;
                rfSrfcLoad.LoadDirType = LoadDirectionType.LocalZType;
                rfSrfcLoad.LoadDistType = LoadDistributionType.UniformType;
            }
            else
            {
                msg = "Insufficient input parameters. Provide either surface list and load case or existing RFSurfaceLoad Object. ";
                level = GH_RuntimeMessageLevel.Warning;
                return;
            }
            if (DA.GetData(17, ref mod))
            {
                rfSrfcLoad.ToModify = mod;
            }
            if (DA.GetData(18, ref del))
            {
                rfSrfcLoad.ToDelete = del;
            }
            if (DA.GetData(3, ref noIndex))
            {
                rfSrfcLoad.No = noIndex;
            }
            if (DA.GetData(4, ref comment))
            {
                rfSrfcLoad.Comment = comment;
            }
            if (DA.GetData(2, ref f1))
            {
                rfSrfcLoad.Magnitude1 = f1;
            }
            if (DA.GetData(5, ref f2))
            {
                rfSrfcLoad.Magnitude2 = f2;
            }
            if (DA.GetData(6, ref f3))
            {
                rfSrfcLoad.Magnitude3 = f3;
            }
            // Add warning in case of null forces
            if ((rfSrfcLoad.Magnitude1 == 0.0) && (rfSrfcLoad.Magnitude2 == 0) && (rfSrfcLoad.Magnitude3 == 0))
            {
                msg = "Null forces will result in an error in RFEM. ";
                level = GH_RuntimeMessageLevel.Warning;
                return;
            }
            if (DA.GetData(7, ref f4))
            {
                rfSrfcLoad.Magnitude4 = f4;
            }
            if (DA.GetData(8, ref f5))
            {
                rfSrfcLoad.Magnitude5 = f5;
            }
            if (DA.GetData(9, ref f6))
            {
                rfSrfcLoad.Magnitude6 = f6;
            }
            if (DA.GetData(10, ref node1))
            {
                rfSrfcLoad.Node1No = node1;
            }
            if (DA.GetData(11, ref node2))
            {
                rfSrfcLoad.Node2No = node2;
            }
            if (DA.GetData(12, ref node3))
            {
                rfSrfcLoad.Node3No = node3;
            }
            if (DA.GetData(13, ref type))
            {
                var myType = (LoadType)type;
                rfSrfcLoad.LoadType = myType;
            }
            if (DA.GetData(14, ref dir))
            {
                rfSrfcLoad.LoadDirType = (LoadDirectionType)dir;
            }
            if (DA.GetData(15, ref dist))
            {
                rfSrfcLoad.LoadDistType = (LoadDistributionType)dist;
                if(rfSrfcLoad.LoadDistType == LoadDistributionType.UnknownLoadDistributionType)
                {
                    msg = "Load Distribution not supported. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
            }
            // Check Load Orientation
            switch (rfSrfcLoad.LoadType)
            {
                case LoadType.ForceType:
                    if (rfSrfcLoad.LoadDirType == LoadDirectionType.LocalUType || rfSrfcLoad.LoadDirType == LoadDirectionType.LocalVType || rfSrfcLoad.LoadDirType == LoadDirectionType.UnknownLoadDirectionType || rfSrfcLoad.LoadDirType == LoadDirectionType.PerpendicularZType)
                    {
                        msg = "Load Direction Type not supported for this Loading Type. ";
                        level = GH_RuntimeMessageLevel.Warning;
                        return;
                    }
                    break;
                case LoadType.TemperatureType:
                    break;
                case LoadType.AxialStrainType:
                    break;
                case LoadType.PrecamberType:
                    break;
                default:
                    {
                        msg = "Load Type not supported. ";
                        level = GH_RuntimeMessageLevel.Warning;
                        return;
                    }
            }
            DA.SetData(0, rfSrfcLoad);
        }
    }
}
