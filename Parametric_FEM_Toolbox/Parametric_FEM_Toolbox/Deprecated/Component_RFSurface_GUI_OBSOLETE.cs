using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using Parametric_FEM_Toolbox.UIWidgets;

using Parametric_FEM_Toolbox.HelperLibraries;
using Parametric_FEM_Toolbox.RFEM;
using Parametric_FEM_Toolbox.GUI;
using Dlubal.RFEM5;
using System.Runtime.InteropServices;

namespace Parametric_FEM_Toolbox.Deprecated
{
    public class Component_RFSurface_GUI_OBSOLETE : GH_SwitcherComponent
    {
        // Declare class variables outside the method "SolveInstance" so their values persist 
        // when the method is called again.

        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public Component_RFSurface_GUI_OBSOLETE()
          : base("RF Surface", "RFSrfc", "Creates a RFSurface object to define new data or modify existing data " +
                "in the RFEM model.", "B+G Toolbox", "RFEM")
        {
        }

        // Define Keywords to search for this Component more easily in Grasshopper
        public override IEnumerable<string> Keywords => new string[] {"rf", "rfsurface", "surface", "srfc"};

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Use the pManager object to register your input parameters.
            pManager.AddSurfaceParameter("Surface", "Srfc", "Surface to assemble the RFSurface from.", GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager.AddIntegerParameter("Surface Number", "No", "Optional index number to assign to the RFEM object.", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddIntegerParameter("Material Number", "Mat", "Material index number.", GH_ParamAccess.item);
            pManager[2].Optional = true;
            pManager.AddNumberParameter("Thickness [m]", "H", "Surface thickness.", GH_ParamAccess.item);
            pManager[3].Optional = true;
            pManager.AddTextParameter("Comment", "Comment", "Comment.", GH_ParamAccess.item);
            pManager[4].Optional = true;

            // you can use the pManager instance to access them by index:
            // pManager[0].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            // Use the pManager object to register your output parameters.
            // Output parameters do not have default values, but they too must have the correct access type.
            pManager.RegisterParam(new Param_RFEM(), "Surface", "Srfc", "Output RFSurface.", GH_ParamAccess.item);

            // Sometimes you want to hide a specific parameter from the Rhino preview.
            // You can use the HideParameter() method as a quick way:
            // pManager.HideParameter(0);
        }

        protected override void RegisterEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit("Assemble Surface", "Srfc", "Creates a RFLine object to define new data or modify existing data " +
                "in the RFEM model.");
            mngr.RegisterUnit(evaluationUnit);

            evaluationUnit.RegisterInputParam(new Param_String(), "Boundary Line List", "Bound", "Boundary Line List", GH_ParamAccess.item);
            evaluationUnit.Inputs[0].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_String(), "Surface Type", "Type", UtilLibrary.DescriptionRFTypes(typeof(SurfaceGeometryType)), GH_ParamAccess.item);
            evaluationUnit.Inputs[1].Parameter.Optional = true;
            //evaluationUnit.RegisterInputParam(new Param_Integer(), "Interpolated Points", "n", "Number of interpolated points for NURBS", GH_ParamAccess.item, new GH_Integer(4));
            //evaluationUnit.Inputs[2].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_String(), "Thickness Type", "Th Type", UtilLibrary.DescriptionRFTypes(typeof(SurfaceThicknessType)), GH_ParamAccess.item);
            evaluationUnit.Inputs[2].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_String(), "Stiffness Type", "St Type", UtilLibrary.DescriptionRFTypes(typeof(SurfaceStiffnessType)), GH_ParamAccess.item);
            evaluationUnit.Inputs[3].Parameter.Optional = true;

            evaluationUnit.RegisterInputParam(new Param_RFEM(), "Surface", "Srfc", "Surface object from the RFEM model to modify", GH_ParamAccess.item);
            evaluationUnit.Inputs[4].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Boolean(), "Modify", "Modify", "Modify node?", GH_ParamAccess.item);
            evaluationUnit.Inputs[5].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Boolean(), "Delete", "Delete", "Delete node?", GH_ParamAccess.item);
            evaluationUnit.Inputs[6].Parameter.Optional = true;

            GH_ExtendableMenu gH_ExtendableMenu = new GH_ExtendableMenu(0, "advanced");
            gH_ExtendableMenu.Name = "Advanced";
            gH_ExtendableMenu.Collapse();
            evaluationUnit.AddMenu(gH_ExtendableMenu);
            for (int i = 0; i < 4; i++)
            {
                gH_ExtendableMenu.RegisterInputPlug(evaluationUnit.Inputs[i]);
            }

            GH_ExtendableMenu gH_ExtendableMenu2 = new GH_ExtendableMenu(1, "modify");
            gH_ExtendableMenu2.Name = "Modify";
            gH_ExtendableMenu2.Collapse();
            evaluationUnit.AddMenu(gH_ExtendableMenu2);
            for (int i = 4; i < 4+3; i++)
            {
                gH_ExtendableMenu2.RegisterInputPlug(evaluationUnit.Inputs[i]);
            }
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA, EvaluationUnit unit)
        {
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
            var geomType = "";
            var stiffType = "";
            var thickType = "";
            var thick = 0.0;
            var mat = 0;
            //int intPoints = 4;
            //int newNo = 0;

            if (DA.GetData(9, ref inRFEM))
            {
                rfSrfc = new RFSurface((RFSurface)inRFEM.Value);
                if (DA.GetData(6, ref geomType))
                {
                    Enum.TryParse(geomType, out SurfaceGeometryType myGeomType);
                    rfSrfc.GeometryType = myGeomType;
                }
            }
            else if (DA.GetData(0, ref inSrfc))
            {
                if (!(DA.GetData(2, ref mat) && DA.GetData(3, ref thick)))
                {
                    return;
                }
                else
                {
                    rfSrfc.MaterialNo = mat;
                    rfSrfc.Thickness = thick;
                }
                //DA.GetData(7, ref intPoints);
                if (DA.GetData(6, ref geomType))
                {
                    Enum.TryParse(geomType, out SurfaceGeometryType myGeomType);
                    rfSrfc.GeometryType = myGeomType;
                }
            Component_RFSurface.SetGeometry(inSrfc, ref rfSrfc);
            }
            else
            {
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
                Enum.TryParse(thickType, out SurfaceThicknessType myThicknessType);
                rfSrfc.ThicknessType = myThicknessType;
            }
            if (DA.GetData(8, ref stiffType))
            {
                Enum.TryParse(stiffType, out SurfaceStiffnessType myStiffType);
                rfSrfc.StiffnessType = myStiffType;
            }
            
            DA.SetData(0, rfSrfc);
        }
                 
        /// <summary>
        /// The Exposure property controls where in the panel a component icon 
        /// will appear. There are seven possible locations (primary to septenary), 
        /// each of which can be combined with the GH_Exposure.obscure flag, which 
        /// ensures the component will only be visible on panel dropdowns.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.hidden; }
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return Properties.Resources.Assemble_Surface;                
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("a222356a-ba44-4cff-9c6a-db502a00eb07"); }
        }
    }
}
