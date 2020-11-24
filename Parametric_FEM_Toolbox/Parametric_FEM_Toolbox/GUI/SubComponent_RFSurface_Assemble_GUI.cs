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
using System.Linq;

namespace Parametric_FEM_Toolbox.GUI
{
    public class SubComponent_RFSurface_Assemble_GUI : SubComponent
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
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Assemble Surfaces.");
            evaluationUnit.Icon = Properties.Resources.Assemble_Surface;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {

            unit.RegisterInputParam(new Param_Surface(), "Surface", "Srfc", "Surface to assemble the RFSurface from.", GH_ParamAccess.item);
            unit.Inputs[0].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Surface Number", "No", "Optional index number to assign to the RFEM object.", GH_ParamAccess.item);
            unit.Inputs[1].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Material Number", "Mat", "Material index number.", GH_ParamAccess.item);
            unit.Inputs[2].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Thickness [m]", "H", "Surface thickness.", GH_ParamAccess.item);
            unit.Inputs[3].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Comment", "Comment", "Comment.", GH_ParamAccess.item);
            unit.Inputs[4].Parameter.Optional = true;

            GH_ExtendableMenu gH_ExtendableMenu = new GH_ExtendableMenu(0, "advanced");
            gH_ExtendableMenu.Name = "Advanced";
            gH_ExtendableMenu.Collapse();
            unit.RegisterInputParam(new Param_String(), "Boundary Line List", "Bound", "Boundary Line List", GH_ParamAccess.item);
            unit.Inputs[5].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Surface Type", "Type", UtilLibrary.DescriptionRFTypes(typeof(SurfaceGeometryType)), GH_ParamAccess.item);
            unit.Inputs[6].EnumInput = UtilLibrary.ListRFTypes(typeof(SurfaceGeometryType));
            unit.Inputs[6].Parameter.Optional = true;
            //unit.RegisterInputParam(new Param_Integer(), "Interpolated Points", "n", "Number of interpolated points for NURBS", GH_ParamAccess.item, new GH_Integer(4));
            //unit.Inputs[2].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Thickness Type", "Thick", UtilLibrary.DescriptionRFTypes(typeof(SurfaceThicknessType)), GH_ParamAccess.item);
            unit.Inputs[7].EnumInput = UtilLibrary.ListRFTypes(typeof(SurfaceThicknessType));
            unit.Inputs[7].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Stiffness Type", "Stiff", UtilLibrary.DescriptionRFTypes(typeof(SurfaceStiffnessType)), GH_ParamAccess.item);
            unit.Inputs[8].EnumInput = UtilLibrary.ListRFTypes(typeof(SurfaceStiffnessType));
            unit.Inputs[8].Parameter.Optional = true;
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[5]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[6]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[7]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[8]);
            unit.AddMenu(gH_ExtendableMenu);

            GH_ExtendableMenu gH_ExtendableMenu2 = new GH_ExtendableMenu(1, "modify");
            gH_ExtendableMenu2.Name = "Modify";
            gH_ExtendableMenu2.Collapse();
            unit.RegisterInputParam(new Param_RFEM(), "RF Surface", "RF Surface", "Surface object from the RFEM model to modify", GH_ParamAccess.item);
            unit.Inputs[9].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Modify", "Modify", "Modify object?", GH_ParamAccess.item);
            unit.Inputs[10].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Delete", "Delete", "Delete object?", GH_ParamAccess.item);
            unit.Inputs[11].Parameter.Optional = true;
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[9]);
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[10]);
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[11]);
            unit.AddMenu(gH_ExtendableMenu2);

            unit.RegisterOutputParam(new Param_RFEM(), "RF Surface", "RF Surface", "Output RFSurface.");
        }

        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Blank;
            //var line = new LineCurve();
            Brep inSrfc = null;
            var noIndex = 0;
            var comment = "";
            var rfSrfc = new RFSurface();
            var inRFEM = new GH_RFEM();
            var rfEdges = new List<RFLine>();
            var mod = false;
            var del = false;
            var boundList = "";
            var geomType = 0;
            var stiffType = 0;
            var thickType = 0;
            var thick = 0.0;
            var mat = 0;
            //int intPoints = 4;
            //int newNo = 0;

            if (DA.GetData(9, ref inRFEM))
            {
                rfSrfc = new RFSurface((RFSurface)inRFEM.Value);                
                if (DA.GetData(0, ref inSrfc))
                {
                    Component_RFSurface.SetGeometry(inSrfc, ref rfSrfc);
                }
                if (DA.GetData(6, ref geomType))
                {
                    rfSrfc.GeometryType = (SurfaceGeometryType)geomType;
                    if (rfSrfc.GeometryType == SurfaceGeometryType.UnknownGeometryType)
                    {
                        msg = "Surface Geometry Type not supported. ";
                        level = GH_RuntimeMessageLevel.Warning;
                        return;
                    }
                }
                if (DA.GetData(2, ref mat))
                {
                    rfSrfc.MaterialNo = mat;
                }
                if (DA.GetData(3, ref thick))
                {
                    rfSrfc.Thickness = thick;
                }
            }
            else if (DA.GetData(0, ref inSrfc))
            {
                if (!(DA.GetData(2, ref mat) && DA.GetData(3, ref thick)))
                {
                    msg = "Insufficient input parameters. Provide Material Number and Surface thickness. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
                else
                {
                    rfSrfc.MaterialNo = mat;
                    rfSrfc.Thickness = thick;
                }
                //DA.GetData(7, ref intPoints);
                Component_RFSurface.SetGeometry(inSrfc, ref rfSrfc);
                if (DA.GetData(6, ref geomType))
                {
                    rfSrfc.GeometryType = (SurfaceGeometryType)geomType;
                    if (rfSrfc.GeometryType == SurfaceGeometryType.UnknownGeometryType)
                    {
                        msg = "Surface Geometry Type not supported. ";
                        level = GH_RuntimeMessageLevel.Warning;
                        return;
                    }
                }                
            }
            if (rfSrfc.ToBrep().Edges.Select(x => x.GetLength()).Min() <= 0.001)
            {
                level = GH_RuntimeMessageLevel.Warning;
                msg = "Boundary lines are too short. It may cause import errors.";
            }
            else if (DA.GetData(5, ref boundList))
            {
                rfSrfc.BoundaryLineList = boundList;
                if (!(DA.GetData(2, ref mat) && DA.GetData(3, ref thick)))
                {
                    msg = "Insufficient input parameters. Provide Material Number and Surface thickness. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
                else
                {
                    rfSrfc.MaterialNo = mat;
                    rfSrfc.Thickness = thick;
                }
                if (DA.GetData(6, ref geomType))
                {
                    rfSrfc.GeometryType = (SurfaceGeometryType)geomType;
                    if (rfSrfc.GeometryType == SurfaceGeometryType.UnknownGeometryType)
                    {
                        msg = "Surface Geometry Type not supported. ";
                        level = GH_RuntimeMessageLevel.Warning;
                        return;
                    }
                }
                //}else
                //{
                //    rfSrfc.GeometryType = SurfaceGeometryType.PlaneSurfaceType;
                //}
            }
            else
            {
                msg = "Insufficient input parameters. Provide either Input Surface shape, Boundary Lines List or existing RFSurface Object. ";
                level = GH_RuntimeMessageLevel.Warning;
                return;
            }
            if (DA.GetData(10, ref mod))
            {
                rfSrfc.ToModify = mod;
            }
            if (DA.GetData(11, ref del))
            {
                rfSrfc.ToDelete = del;
            }
            if (DA.GetData(1, ref noIndex))
            {
                rfSrfc.No = noIndex;
            }
            if (DA.GetData(4, ref comment))
            {
                rfSrfc.Comment = comment;
            }
            if (DA.GetData(5, ref boundList))
            {
                rfSrfc.BoundaryLineList = boundList;
            }
            if (DA.GetData(7, ref thickType))
            {
                rfSrfc.ThicknessType = (SurfaceThicknessType)thickType;
                if (rfSrfc.ThicknessType == SurfaceThicknessType.UnknownThicknessType)
                {
                    msg = "Surface Thickness Type not supported. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
            }
            if (DA.GetData(8, ref stiffType))
            {
                rfSrfc.StiffnessType = (SurfaceStiffnessType)stiffType;
                if (rfSrfc.StiffnessType == SurfaceStiffnessType.UnknownStiffnessType)
                {
                    msg = "Taper Shape Type not supported. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
            }
            DA.SetData(0, rfSrfc);
        }
    }
}
