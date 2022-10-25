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
    public class SubComponent_RFFreeRectangularLoad_Assemble_GUI : SubComponent
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
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Assemble Free Rectangular Loads.");
            evaluationUnit.Icon = Properties.Resources.Assemble_PolyLoad;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {
            unit.RegisterInputParam(new Param_Point(), "Position1", "Pt1", "Position 1.", GH_ParamAccess.item);
            unit.Inputs[0].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Point(), "Position2", "Pt2", "Position 2.", GH_ParamAccess.item);
            unit.Inputs[1].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Surface List", "Srfc", "Surface List.", GH_ParamAccess.item);
            unit.Inputs[2].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Load Case", "LC", "Load Case to assign the load to.", GH_ParamAccess.item);
            unit.Inputs[3].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Magnitude 1 [kN/m²]", "F1", "Load Value [kN/m²]", GH_ParamAccess.item);
            unit.Inputs[4].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Load Number", "No", "Optional index number to assign to the RFEM object.", GH_ParamAccess.item);
            unit.Inputs[5].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Comment", "Comment", "Comment.", GH_ParamAccess.item);
            unit.Inputs[6].Parameter.Optional = true;

            GH_ExtendableMenu gH_ExtendableMenu = new GH_ExtendableMenu(0, "advanced");
            gH_ExtendableMenu.Name = "Advanced";
            gH_ExtendableMenu.Collapse();
            unit.RegisterInputParam(new Param_Number(), "Magnitude 2 [kN/m²]", "F2", "Load Value [kN/m²]", GH_ParamAccess.item);
            unit.Inputs[7].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Projection Type", "Projection", UtilLibrary.DescriptionRFTypes(typeof(PlaneType)), GH_ParamAccess.item);
            unit.Inputs[8].EnumInput = UtilLibrary.ListRFTypes(typeof(PlaneType));
            unit.Inputs[8].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Load Direction Type", "Dir", UtilLibrary.DescriptionRFTypes(typeof(LoadDirectionType)), GH_ParamAccess.item);
            unit.Inputs[9].EnumInput = UtilLibrary.ListRFTypes(typeof(LoadDirectionType));
            unit.Inputs[9].Parameter.Optional = true;            
            unit.RegisterInputParam(new Param_Integer(), "Load Distribution Type", "Dist", UtilLibrary.DescriptionRFTypes(typeof(LoadDistributionType)), GH_ParamAccess.item);
            unit.Inputs[10].EnumInput = UtilLibrary.ListRFTypes(typeof(LoadDistributionType));
            unit.Inputs[10].Parameter.Optional = true;
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[7]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[8]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[9]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[10]);
            unit.AddMenu(gH_ExtendableMenu);

            GH_ExtendableMenu gH_ExtendableMenu2 = new GH_ExtendableMenu(1, "modify");
            gH_ExtendableMenu2.Name = "Modify";
            gH_ExtendableMenu2.Collapse();
            unit.RegisterInputParam(new Param_RFEM(), "RF Free Rectangular Load", "RF RectLoad", "Load object from the RFEM model to modify", GH_ParamAccess.item);
            unit.Inputs[11].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Modify", "Modify", "Modify object?", GH_ParamAccess.item);
            unit.Inputs[12].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Delete", "Delete", "Delete object?", GH_ParamAccess.item);
            unit.Inputs[13].Parameter.Optional = true;
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[11]);
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[12]);
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[13]);
            unit.AddMenu(gH_ExtendableMenu2);

            unit.RegisterOutputParam(new Param_RFEM(), "RF Free Rectangular Load", "RF RectLoad", "Output RFFreeRectLoad.");
        }

        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Blank;

            var noIndex = 0;
            var loadCase = 0;
            var comment = "";
            var rfPolyLoad = new RFFreeRectangularLoad();
            var inRFEM = new GH_RFEM();
            var mod = false;
            var del = false;
            var srfclist = "";
            var projection = 0;
            var dir = 0;
            var dist = 0;
            var f1 = 0.0;
            var f2 = 0.0;
            var pt1 = new Point3d();
            var pt2 = new Point3d();

            if (DA.GetData(11, ref inRFEM))
            {
                rfPolyLoad = new RFFreeRectangularLoad((RFFreeRectangularLoad)inRFEM.Value);
                if (DA.GetData(8, ref projection))
                {
                    rfPolyLoad.ProjectionType = (PlaneType)projection;
                }
                else
                {
                    rfPolyLoad.ProjectionType = PlaneType.PlaneXY;
                }
                if (DA.GetData(3, ref loadCase))
                {
                    rfPolyLoad.LoadCase = loadCase;
                }
                if (DA.GetData(0, ref pt1))
                {
                    rfPolyLoad.Position1 = pt1;
                }
                if (DA.GetData(1, ref pt2))
                {
                    rfPolyLoad.Position2 = pt2;
                }
            }
            else if  (DA.GetData(0, ref pt1) && DA.GetData(1, ref pt2) && (DA.GetData(3, ref loadCase)))
            {
                rfPolyLoad = new RFFreeRectangularLoad();
                if (DA.GetData(8, ref projection))
                {
                    rfPolyLoad.ProjectionType = (PlaneType)projection;
                }else
                {
                    rfPolyLoad.ProjectionType = PlaneType.PlaneXY;
                }
                rfPolyLoad.Position1 = pt1;
                rfPolyLoad.Position2 = pt2;
                rfPolyLoad.LoadCase = loadCase;                
                rfPolyLoad.LoadDirType = LoadDirectionType.LocalZType;
                rfPolyLoad.LoadDistType = LoadDistributionType.UniformType;
            }
            else
            {
                msg = "Insufficient input parameters. Provide either Positions and Load Case or existing RFFreePolygonLoad Object. ";
                level = GH_RuntimeMessageLevel.Warning;
                return;
            }
            if (DA.GetData(12, ref mod))
            {
                rfPolyLoad.ToModify = mod;
            }
            if (DA.GetData(13, ref del))
            {
                rfPolyLoad.ToDelete = del;
            }
            if (DA.GetData(2, ref srfclist))
            {
                rfPolyLoad.SurfaceList = srfclist;
            }
            if (DA.GetData(5, ref noIndex))
            {
                rfPolyLoad.No = noIndex;
            }
            if (DA.GetData(6, ref comment))
            {
                rfPolyLoad.Comment = comment;
            }
            if (DA.GetData(4, ref f1))
            {
                rfPolyLoad.Magnitude1 = f1;
            }
            if (DA.GetData(7, ref f2))
            {
                rfPolyLoad.Magnitude2 = f2;
            }
            // Add warning in case of null forces
            if ((rfPolyLoad.Magnitude1 == 0.0) && (rfPolyLoad.Magnitude2 == 0))
            {
                msg = "Null forces will result in an error in RFEM. ";
                level = GH_RuntimeMessageLevel.Warning;
                return;
            }
            if (DA.GetData(9, ref dir))
            {
                rfPolyLoad.LoadDirType = (LoadDirectionType)dir;
            }
            if (DA.GetData(10, ref dist))
            {
                rfPolyLoad.LoadDistType = (LoadDistributionType)dist;
            }
            // Check Load Orientation
            if (rfPolyLoad.LoadDirType == LoadDirectionType.LocalUType || rfPolyLoad.LoadDirType == LoadDirectionType.LocalVType || rfPolyLoad.LoadDirType == LoadDirectionType.UnknownLoadDirectionType || rfPolyLoad.LoadDirType == LoadDirectionType.PerpendicularZType)
            {
                msg = "Load Direction Type not supported. ";
                level = GH_RuntimeMessageLevel.Warning;
                return;
            }
            if (!(rfPolyLoad.LoadDistType == LoadDistributionType.UniformType || rfPolyLoad.LoadDistType == LoadDistributionType.LinearType || rfPolyLoad.LoadDistType == LoadDistributionType.LinearXType || rfPolyLoad.LoadDistType == LoadDistributionType.LinearYType || rfPolyLoad.LoadDistType == LoadDistributionType.LinearZType))
            {
                msg = "Load Distribution Type not supported. ";
                level = GH_RuntimeMessageLevel.Warning;
                return;
            }
            if (rfPolyLoad.ProjectionType == PlaneType.UnknownPlane)
            {
                msg = "Load Distribution Type not supported. ";
                level = GH_RuntimeMessageLevel.Warning;
                return;
            }
            DA.SetData(0, rfPolyLoad);
        }
    }
}
