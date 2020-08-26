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
    public class SubComponent_RFLineLoad_Assemble_GUI : SubComponent
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
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Assemble Line Loads.");
            evaluationUnit.Icon = RFEM_daq.Properties.Resources.Assemble_LineLoad;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {

            unit.RegisterInputParam(new Param_Curve(), "Base Line", "Line", "Load Application Line.", GH_ParamAccess.item);
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
            unit.RegisterInputParam(new Param_String(), "Line List", "LineList", "Line List", GH_ParamAccess.item);
            unit.Inputs[5].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Magnitude 2 [kN/m]", "F2", "Load Value [kN/m]", GH_ParamAccess.item);
            unit.Inputs[6].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Magnitude 3 [kN/m]", "F3", "Load Value [kN/m]", GH_ParamAccess.item);
            unit.Inputs[7].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Distance A", "t1", "Distance A", GH_ParamAccess.item);
            unit.Inputs[8].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Distance B", "t2", "Distance B", GH_ParamAccess.item);
            unit.Inputs[9].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Forces [kN/m]", "F", "Array of Loads", GH_ParamAccess.list);
            unit.Inputs[10].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Relative Distances", "L", "Array of Loads", GH_ParamAccess.list);
            unit.Inputs[11].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Load Type", "Type", UtilLibrary.DescriptionRFTypes(typeof(LoadType)), GH_ParamAccess.item);
            unit.Inputs[12].EnumInput = UtilLibrary.ListRFTypes(typeof(LoadType));
            unit.Inputs[12].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Load Direction Type", "Dir", UtilLibrary.DescriptionRFTypes(typeof(LoadDirectionType)), GH_ParamAccess.item);
            unit.Inputs[13].EnumInput = UtilLibrary.ListRFTypes(typeof(LoadDirectionType));
            unit.Inputs[13].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Load Distribution Type", "Dist", UtilLibrary.DescriptionRFTypes(typeof(LoadDistributionType)), GH_ParamAccess.item);
            unit.Inputs[14].EnumInput = UtilLibrary.ListRFTypes(typeof(LoadDistributionType));
            unit.Inputs[14].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Line Load Reference Type", "RefType", UtilLibrary.DescriptionRFTypes(typeof(LineLoadReferenceType)), GH_ParamAccess.item);
            unit.Inputs[15].EnumInput = UtilLibrary.ListRFTypes(typeof(LineLoadReferenceType));
            unit.Inputs[15].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Over Total Length", "Total", "Over Total Length", GH_ParamAccess.item);
            unit.Inputs[16].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Relative Distances", "Rel", "Relative Distances", GH_ParamAccess.item);
            unit.Inputs[17].Parameter.Optional = true;
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
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[16]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[17]);
            unit.AddMenu(gH_ExtendableMenu);

            GH_ExtendableMenu gH_ExtendableMenu2 = new GH_ExtendableMenu(1, "modify");
            gH_ExtendableMenu2.Name = "Modify";
            gH_ExtendableMenu2.Collapse();
            unit.RegisterInputParam(new Param_RFEM(), "RF Line Load", "RF LLoad", "Load object from the RFEM model to modify", GH_ParamAccess.item);
            unit.Inputs[18].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Modify", "Modify", "Modify object?", GH_ParamAccess.item);
            unit.Inputs[19].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Delete", "Delete", "Delete object?", GH_ParamAccess.item);
            unit.Inputs[20].Parameter.Optional = true;
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[18]);
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[19]);
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[20]);
            unit.AddMenu(gH_ExtendableMenu2);

            unit.RegisterOutputParam(new Param_RFEM(), "RF Line Load", "RF LLoad", "Output RFLineLoad.");
        }


        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Blank;

            Curve inCurve = null;
            var noIndex = 0;
            var loadCase = 0;
            var comment = "";
            var rfLineLoad = new RFLineLoad();
            var inRFEM = new GH_RFEM();
            var mod = false;
            var del = false;
            var lineList = "";
            var type = 0;
            var dir = 0;
            var dist = 0;
            var reference = 0;
            var f1 = 0.0;
            var f2 = 0.0;
            var f3 = 0.0;
            var t1 = 0.0;
            var t2 = 0.0;
            var loads = new List<double>();
            var distances = new List<double>();
            var totallength = true;
            var reldistances = true;


            if (DA.GetData(18, ref inRFEM))
            {
                rfLineLoad = new RFLineLoad((RFLineLoad)inRFEM.Value);
                if (DA.GetData(0, ref inCurve))
                {
                    var myRFLine = new RFLine();
                    Component_RFLine.SetGeometry(inCurve, ref myRFLine);
                    var myRFLines = new List<RFLine>() { myRFLine };
                    rfLineLoad = new RFLineLoad(rfLineLoad, myRFLines, rfLineLoad.LoadCase);
                }
                if (DA.GetData(1, ref loadCase))
                {
                    rfLineLoad.LoadCase = loadCase;
                }
                if (DA.GetData(5, ref lineList))
                {
                    rfLineLoad.LineList = lineList;
                }
            }
            else if ((DA.GetData(0, ref inCurve)) && (DA.GetData(1, ref loadCase)))
            {
                var myRFLine = new RFLine();
                Component_RFLine.SetGeometry(inCurve, ref myRFLine);
                var myRFLines = new List<RFLine>() { myRFLine };
                rfLineLoad = new RFLineLoad(new LineLoad(), myRFLines, loadCase);
                rfLineLoad.LoadType = LoadType.ForceType;
                rfLineLoad.LoadDirType = LoadDirectionType.LocalZType;
                rfLineLoad.LoadDistType = LoadDistributionType.UniformType;
                rfLineLoad.LoadRefType = LineLoadReferenceType.LinesType;
                rfLineLoad.OverTotalLength = true;
                rfLineLoad.RelativeDistances = true;
            }
            else if (DA.GetData(5, ref lineList) && (DA.GetData(1, ref loadCase)))
            {
                rfLineLoad = new RFLineLoad();
                rfLineLoad.LoadCase = loadCase;
                rfLineLoad.LineList = lineList;
                rfLineLoad.LoadType = LoadType.ForceType;
                rfLineLoad.LoadDirType = LoadDirectionType.LocalZType;
                rfLineLoad.LoadDistType = LoadDistributionType.UniformType;
                rfLineLoad.LoadRefType = LineLoadReferenceType.LinesType;
                rfLineLoad.OverTotalLength = true;
                rfLineLoad.RelativeDistances = true;
            }
            else
            {
                msg = "Insufficient input parameters. Provide either base line and load case or line list and load case or existing RFLineLoad Object. ";
                level = GH_RuntimeMessageLevel.Warning;
                return;
            }
            if (DA.GetData(19, ref mod))
            {
                rfLineLoad.ToModify = mod;
            }
            if (DA.GetData(20, ref del))
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
            if (DA.GetDataList(10, loads) && DA.GetDataList(11, distances))
            {
                if (loads.Count != distances.Count)
                {
                    msg = "Null forces will result in an error in RFEM. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
                var loadsarray = new double[loads.Count, 2];
                for (int i = 0; i < loads.Count; i++)
                {
                    loadsarray[i, 0] = distances[i] * 100;
                    loadsarray[i, 1] = loads[i];
                }
                rfLineLoad.LoadArray = loadsarray;
            }
            if (DA.GetData(2, ref f1))
            {
                rfLineLoad.Magnitude1 = f1;
            }
            if (DA.GetData(6, ref f2))
            {
                rfLineLoad.Magnitude2 = f2;
            }
            if (DA.GetData(7, ref f3))
            {
                rfLineLoad.Magnitude3 = f3;
            }
            // Add warning in case of null forces
            if ((rfLineLoad.Magnitude1 == 0.0) && (rfLineLoad.Magnitude2 == 0.0) && (rfLineLoad.Magnitude3 == 0.0) && (loads.Count == 0))
            {
                msg = "Null forces will result in an error in RFEM. ";
                level = GH_RuntimeMessageLevel.Warning;
                return;
            }
            if (DA.GetData(8, ref t1) && DA.GetData(9, ref t2))
            {
                rfLineLoad.DistanceA = t1;
                rfLineLoad.DistanceB = t2;
                rfLineLoad.OverTotalLength = false;
                if ((t1 < 0.0) || (t1 > 1.0) || (t2 < 0.0) || (t2 > 1.0))
                {
                    msg = "Invalid interval. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
            }
            if (DA.GetData(13, ref dir))
            {
                rfLineLoad.LoadDirType = (LoadDirectionType)dir;
            }
            if (DA.GetData(14, ref dist))
            {
                rfLineLoad.LoadDistType = (LoadDistributionType)dist;
                if (rfLineLoad.LoadDistType == LoadDistributionType.UnknownLoadDistributionType)
                {
                    msg = "Load Distribution not supported. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
            }
            if (DA.GetData(12, ref type))
            {
                var myType = (LoadType)type;
                // Check Load Orientation
                switch (myType)
                {
                    case LoadType.ForceType:
                        if (rfLineLoad.LoadDirType == LoadDirectionType.LocalUType || rfLineLoad.LoadDirType == LoadDirectionType.LocalVType || rfLineLoad.LoadDirType == LoadDirectionType.UnknownLoadDirectionType || rfLineLoad.LoadDirType == LoadDirectionType.PerpendicularZType)
                        {
                            msg = "Load Direction Type not supported for this Loading Type. ";
                            level = GH_RuntimeMessageLevel.Warning;
                            return;
                        }
                        break;
                    case LoadType.MomentType:
                        if (rfLineLoad.LoadDirType == LoadDirectionType.LocalUType || rfLineLoad.LoadDirType == LoadDirectionType.LocalVType || rfLineLoad.LoadDirType == LoadDirectionType.ProjectXType || rfLineLoad.LoadDirType == LoadDirectionType.ProjectYType || rfLineLoad.LoadDirType == LoadDirectionType.ProjectZType || rfLineLoad.LoadDirType == LoadDirectionType.UnknownLoadDirectionType || rfLineLoad.LoadDirType == LoadDirectionType.PerpendicularZType)
                        {
                            msg = "Load Direction Type not supported for this Loading Type. ";
                            level = GH_RuntimeMessageLevel.Warning;
                            return;
                        }
                        break;
                    default:
                        {
                            msg = "Load Type not supported. ";
                            level = GH_RuntimeMessageLevel.Warning;
                            return;
                        }
                }
                rfLineLoad.LoadType = myType;
            }
            if (DA.GetData(15, ref reference))
            {
                rfLineLoad.LoadRefType = (LineLoadReferenceType)reference;
            }
            if (DA.GetData(16, ref totallength))
            {
                rfLineLoad.OverTotalLength = totallength;
            }
            if (DA.GetData(17, ref reldistances))
            {
                rfLineLoad.RelativeDistances = reldistances;
            }
            DA.SetData(0, rfLineLoad);
        }
    }
}
