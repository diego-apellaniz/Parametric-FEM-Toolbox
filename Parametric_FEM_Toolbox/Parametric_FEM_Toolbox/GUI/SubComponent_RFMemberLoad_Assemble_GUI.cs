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
    public class SubComponent_RFMemberLoad_Assemble_GUI : SubComponent
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
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Assemble Member Loads.");
            evaluationUnit.Icon = Properties.Resources.Assemble_MemberLoad;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {

            unit.RegisterInputParam(new Param_String(), "Member List", "Member", "Member List.", GH_ParamAccess.item);
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
            unit.RegisterInputParam(new Param_Number(), "Magnitude 2 [kN/m]", "F2", "Load Value [kN/m]", GH_ParamAccess.item);
            unit.Inputs[5].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Magnitude 3 [kN/m]", "F3", "Load Value [kN/m]", GH_ParamAccess.item);
            unit.Inputs[6].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Magnitude 4 [°C]", "T4", "Load Value [°C]", GH_ParamAccess.item);
            unit.Inputs[7].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Magnitude 5 [°C]", "T5", "Load Value [°C]", GH_ParamAccess.item);
            unit.Inputs[8].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Magnitude 6 [°C]", "T6", "Load Value [°C]", GH_ParamAccess.item);
            unit.Inputs[9].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Distance A", "t1", "Distance A", GH_ParamAccess.item);
            unit.Inputs[10].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Distance B", "t2", "Distance B", GH_ParamAccess.item);
            unit.Inputs[11].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Forces [kN/m]", "F", "Array of Loads", GH_ParamAccess.list);
            unit.Inputs[12].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Relative Distances", "L", "Array of Loads", GH_ParamAccess.list);
            unit.Inputs[13].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Load Type", "Type", UtilLibrary.DescriptionRFTypes(typeof(LoadType)), GH_ParamAccess.item);
            unit.Inputs[14].EnumInput = UtilLibrary.ListRFTypes(typeof(LoadType));
            unit.Inputs[14].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Load Direction Type", "Dir", UtilLibrary.DescriptionRFTypes(typeof(LoadDirectionType)), GH_ParamAccess.item);
            unit.Inputs[15].EnumInput = UtilLibrary.ListRFTypes(typeof(LoadDirectionType));
            unit.Inputs[15].Parameter.Optional = true;            
            unit.RegisterInputParam(new Param_Integer(), "Load Distribution Type", "Dist", UtilLibrary.DescriptionRFTypes(typeof(LoadDistributionType)), GH_ParamAccess.item);
            unit.Inputs[16].EnumInput = UtilLibrary.ListRFTypes(typeof(LoadDistributionType));
            unit.Inputs[16].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Member Load Reference Type", "RefType", UtilLibrary.DescriptionRFTypes(typeof(MemberLoadReferenceType)), GH_ParamAccess.item);
            unit.Inputs[17].EnumInput = UtilLibrary.ListRFTypes(typeof(MemberLoadReferenceType));
            unit.Inputs[17].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Over Total Length", "Total", "Over Total Length", GH_ParamAccess.item);
            unit.Inputs[18].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Relative Distances", "Rel", "Relative Distances", GH_ParamAccess.item);
            unit.Inputs[19].Parameter.Optional = true;
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
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[18]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[19]);
            unit.AddMenu(gH_ExtendableMenu);

            GH_ExtendableMenu gH_ExtendableMenu2 = new GH_ExtendableMenu(1, "modify");
            gH_ExtendableMenu2.Name = "Modify";
            gH_ExtendableMenu2.Collapse();
            unit.RegisterInputParam(new Param_RFEM(), "RF Member Load", "RF MLoad", "Load object from the RFEM model to modify", GH_ParamAccess.item);
            unit.Inputs[20].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Modify", "Modify", "Modify object?", GH_ParamAccess.item);
            unit.Inputs[21].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Delete", "Delete", "Delete object?", GH_ParamAccess.item);
            unit.Inputs[22].Parameter.Optional = true;
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[20]);
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[21]);
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[22]);
            unit.AddMenu(gH_ExtendableMenu2);

            unit.RegisterOutputParam(new Param_RFEM(), "RF Member Load", "RF MLoad", "Output RFMemberLoad.");
        }


        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Blank;

            var noIndex = 0;
            var loadCase = 0;
            var comment = "";
            var rfMemberLoad = new RFMemberLoad();
            var inRFEM = new GH_RFEM();
            var mod = false;
            var del = false;
            var memberlist = "";
            var type = 0;
            var dir = 0;
            var dist = 0;
            var reference = 0;
            var f1 = 0.0;
            var f2 = 0.0;
            var f3 = 0.0;
            var f4 = 0.0;
            var f5 = 0.0;
            var f6 = 0.0;
            var t1 = 0.0;
            var t2 = 0.0;
            var loads = new List<double>();
            var distances = new List<double>();
            var totallength = true;
            var reldistances = true;


            if (DA.GetData(20, ref inRFEM))
            {
                rfMemberLoad = new RFMemberLoad((RFMemberLoad)inRFEM.Value);
                if (DA.GetData(0, ref memberlist))
                {
                    rfMemberLoad.MemberList = memberlist;
                }
                if (DA.GetData(1, ref loadCase))
                {
                    rfMemberLoad.LoadCase = loadCase;
                }
            }
            else if  (DA.GetData(0, ref memberlist) && (DA.GetData(1, ref loadCase)))
            {
                rfMemberLoad = new RFMemberLoad();
                rfMemberLoad.LoadCase = loadCase;
                rfMemberLoad.MemberList = memberlist;
                rfMemberLoad.LoadType = LoadType.ForceType;
                rfMemberLoad.LoadDirType = LoadDirectionType.LocalZType;
                rfMemberLoad.LoadDistType = LoadDistributionType.UniformType;
                rfMemberLoad.LoadRefType = MemberLoadReferenceType.MembersType;
                rfMemberLoad.OverTotalLength = true;
                rfMemberLoad.RelativeDistances = true;
            }
            else
            {
                msg = "Insufficient input parameters. Provide either member list and load case or existing RFMemberLoad Object. ";
                level = GH_RuntimeMessageLevel.Warning;
                return;
            }
            if (DA.GetData(21, ref mod))
            {
                rfMemberLoad.ToModify = mod;
            }
            if (DA.GetData(22, ref del))
            {
                rfMemberLoad.ToDelete = del;
            }
            if (DA.GetData(3, ref noIndex))
            {
                rfMemberLoad.No = noIndex;
            }
            if (DA.GetData(4, ref comment))
            {
                rfMemberLoad.Comment = comment;
            }
            if (DA.GetDataList(12, loads) && DA.GetDataList(13, distances))
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
                rfMemberLoad.LoadArray = loadsarray;
            }
            if (DA.GetData(2, ref f1))
            {
                rfMemberLoad.Magnitude1 = f1;
            }
            if (DA.GetData(5, ref f2))
            {
                rfMemberLoad.Magnitude2 = f2;
            }
            if (DA.GetData(6, ref f3))
            {
                rfMemberLoad.Magnitude3 = f3;
            }
            // Add warning in case of null forces
            if ((rfMemberLoad.Magnitude1 == 0.0) && (rfMemberLoad.Magnitude2 == 0) && (rfMemberLoad.Magnitude3 == 0) && (loads.Count == 0))
            {
                msg = "Null forces will result in an error in RFEM. ";
                level = GH_RuntimeMessageLevel.Warning;
                return;
            }
            if (DA.GetData(7, ref f4))
            {
                rfMemberLoad.Magnitude4 = f4;
            }
            if (DA.GetData(8, ref f5))
            {
                rfMemberLoad.Magnitude5 = f5;
            }
            if (DA.GetData(9, ref f6))
            {
                rfMemberLoad.Magnitude6 = f6;
            }
            if (DA.GetData(10, ref t1) && DA.GetData(11, ref t2))
            {
                rfMemberLoad.DistanceA = t1;
                rfMemberLoad.DistanceB = t2;
                rfMemberLoad.OverTotalLength = false;
                if ((t1 < 0.0) || (t1 > 1.0) || (t2 < 0.0) || (t2 > 1.0))
                {
                    msg = "Invalid interval. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }

            }
            if (DA.GetData(14, ref type))
            {
                var myType = (LoadType)type;
                rfMemberLoad.LoadType = myType;
            }
            if (DA.GetData(15, ref dir))
            {
                rfMemberLoad.LoadDirType = (LoadDirectionType)dir;
            }
            if (DA.GetData(16, ref dist))
            {
                rfMemberLoad.LoadDistType = (LoadDistributionType)dist;
                if (rfMemberLoad.LoadDistType == LoadDistributionType.UnknownLoadDistributionType)
                {
                    msg = "Load Distribution not supported. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
            }
            if (DA.GetData(17, ref reference))
            {
                rfMemberLoad.LoadRefType = (MemberLoadReferenceType)reference;
            }
            // Check Load Orientation
            switch (rfMemberLoad.LoadType)
            {
                case LoadType.ForceType:
                    if (rfMemberLoad.LoadDirType == LoadDirectionType.LocalUType || rfMemberLoad.LoadDirType == LoadDirectionType.LocalVType || rfMemberLoad.LoadDirType == LoadDirectionType.UnknownLoadDirectionType || rfMemberLoad.LoadDirType == LoadDirectionType.PerpendicularZType)
                    {
                        msg = "Load Direction Type not supported for this Loading Type. ";
                        level = GH_RuntimeMessageLevel.Warning;
                        return;
                    }
                    break;
                case LoadType.MomentType:
                    if (rfMemberLoad.LoadDirType == LoadDirectionType.LocalUType || rfMemberLoad.LoadDirType == LoadDirectionType.LocalVType || rfMemberLoad.LoadDirType == LoadDirectionType.ProjectXType || rfMemberLoad.LoadDirType == LoadDirectionType.ProjectYType || rfMemberLoad.LoadDirType == LoadDirectionType.ProjectZType || rfMemberLoad.LoadDirType == LoadDirectionType.UnknownLoadDirectionType || rfMemberLoad.LoadDirType == LoadDirectionType.PerpendicularZType)
                    {
                        msg = "Load Direction Type not supported for this Loading Type. ";
                        level = GH_RuntimeMessageLevel.Warning;
                        return;
                    }
                    break;
                case LoadType.TemperatureType:
                    if (!(rfMemberLoad.LoadDirType == LoadDirectionType.LocalYType || rfMemberLoad.LoadDirType == LoadDirectionType.LocalZType))
                    {
                        msg = "Load Direction Type not supported for this Loading Type. ";
                        level = GH_RuntimeMessageLevel.Warning;
                        return;
                    }
                    break;
                case LoadType.AxialStrainType:
                    if (!(rfMemberLoad.LoadDirType == LoadDirectionType.LocalXType))
                    {
                        msg = "Load Direction Type not supported for this Loading Type. ";
                        level = GH_RuntimeMessageLevel.Warning;
                        return;
                    }
                    break;
                case LoadType.AxialDisplacementType:
                    if (!(rfMemberLoad.LoadDirType == LoadDirectionType.LocalXType))
                    {
                        msg = "Load Direction Type not supported for this Loading Type. ";
                        level = GH_RuntimeMessageLevel.Warning;
                        return;
                    }
                    break;
                case LoadType.PrecamberType:
                    if (!(rfMemberLoad.LoadDirType == LoadDirectionType.LocalYType || rfMemberLoad.LoadDirType == LoadDirectionType.LocalZType))
                    {
                        msg = "Load Direction Type not supported for this Loading Type. ";
                        level = GH_RuntimeMessageLevel.Warning;
                        return;
                    }
                    break;
                case LoadType.InitialPrestressType:
                    if (!(rfMemberLoad.LoadDirType == LoadDirectionType.LocalXType))
                    {
                        msg = "Load Direction Type not supported for this Loading Type. ";
                        level = GH_RuntimeMessageLevel.Warning;
                        return;
                    }
                        msg = "Prrestress force must be specified in kN since v1.2. ";
                        level = GH_RuntimeMessageLevel.Warning;
                        // not return!
                    break;
                case LoadType.EndPrestressType:
                    if (!(rfMemberLoad.LoadDirType == LoadDirectionType.LocalXType))
                    {
                        msg = "Load Direction Type not supported for this Loading Type. ";
                        level = GH_RuntimeMessageLevel.Warning;
                        return;
                    }
                    msg = "Prrestress force must be specified in kN since v1.2. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    // not return!
                    break;
                case LoadType.DisplacementType:
                    if (rfMemberLoad.LoadDirType == LoadDirectionType.LocalUType || rfMemberLoad.LoadDirType == LoadDirectionType.LocalVType || rfMemberLoad.LoadDirType == LoadDirectionType.UnknownLoadDirectionType || rfMemberLoad.LoadDirType == LoadDirectionType.PerpendicularZType)
                    {
                        msg = "Load Direction Type not supported for this Loading Type. ";
                        level = GH_RuntimeMessageLevel.Warning;
                        return;
                    }
                    break;
                case LoadType.RotationLoadType:
                    if (rfMemberLoad.LoadDirType == LoadDirectionType.LocalUType || rfMemberLoad.LoadDirType == LoadDirectionType.LocalVType || rfMemberLoad.LoadDirType == LoadDirectionType.ProjectXType || rfMemberLoad.LoadDirType == LoadDirectionType.ProjectYType || rfMemberLoad.LoadDirType == LoadDirectionType.ProjectZType || rfMemberLoad.LoadDirType == LoadDirectionType.UnknownLoadDirectionType || rfMemberLoad.LoadDirType == LoadDirectionType.PerpendicularZType)
                    {
                        msg = "Load Direction Type not supported for this Loading Type. ";
                        level = GH_RuntimeMessageLevel.Warning;
                        return;
                    }
                    break;
                case LoadType.FullPipeContentType:
                    if (!(rfMemberLoad.LoadDirType == LoadDirectionType.GlobalZType))
                    {
                        msg = "Load Direction Type not supported for this Loading Type. ";
                        level = GH_RuntimeMessageLevel.Warning;
                        return;
                    }
                    break;
                case LoadType.PartialPipeContentType:
                    if (!(rfMemberLoad.LoadDirType == LoadDirectionType.GlobalZType))
                    {
                        msg = "Load Direction Type not supported for this Loading Type. ";
                        level = GH_RuntimeMessageLevel.Warning;
                        return;
                    }
                    break;
                case LoadType.PipeInternalPressureType:
                    if (!(rfMemberLoad.LoadDirType == LoadDirectionType.LocalXType))
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
            if (DA.GetData(18, ref totallength))
            {
                rfMemberLoad.OverTotalLength = totallength;
            }
            if (DA.GetData(19, ref reldistances))
            {
                rfMemberLoad.RelativeDistances = reldistances;
            }
            DA.SetData(0, rfMemberLoad);
        }
    }
}
