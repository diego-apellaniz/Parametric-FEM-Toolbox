using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Parametric_FEM_Toolbox.RFEM;
using Parametric_FEM_Toolbox.GUI;

namespace Parametric_FEM_Toolbox.Deprecated
{
    public class Component_RFSurfaceProperties_GUI_OBSOLETE : GH_Component
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
        public Component_RFSurfaceProperties_GUI_OBSOLETE()
          : base("RF Surface Properties", "RFSrfcProp", "Gets the properties of RFSurface objects.", "B+G Toolbox", "RFEM")
        {
        }

        // Define Keywords to search for this Component more easily in Grasshopper
        public override IEnumerable<string> Keywords => new string[] {"rf", "rfsurface", "surface", "srfc", "properties"};

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Use the pManager object to register your input parameters.
            pManager.AddParameter(new Param_RFEM(), "Surface", "Srfc", "Input RFSurface.", GH_ParamAccess.item);
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
            pManager.AddSurfaceParameter("Surface", "Srfc", "Surface related to the RFSurface object.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Surface Number", "No", "Index number of the RFEM object.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Material Number", "Mat", "Material index number.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Thickness [m]", "H", "Surface thickness.", GH_ParamAccess.item);
            pManager.AddTextParameter("Comment", "Comment", "Comment.", GH_ParamAccess.item);
            pManager.AddTextParameter("Boundary Lines List", "BoundList", "List of index numbers of boundary lines", GH_ParamAccess.item);
            pManager.AddTextParameter("Geometry Type", "G Type", "Geometry Type", GH_ParamAccess.item);
            pManager.AddTextParameter("Thickness Type", "Th Type", "Thickness Type", GH_ParamAccess.item);
            pManager.AddTextParameter("Stiffness Type", "S Type", "Stiffness Type", GH_ParamAccess.item);
            pManager.AddTextParameter("Area [m²]", "A", "Surface area", GH_ParamAccess.item);


            // Sometimes you want to hide a specific parameter from the Rhino preview.
            // You can use the HideParameter() method as a quick way:
            // pManager.HideParameter(0);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input
            var inGH = new GH_RFEM();
            if(!DA.GetData(0, ref inGH))
            {
                return;
            }
            var rfSurface = (RFSurface)inGH.Value;
            // Output
            DA.SetData(0, rfSurface.ToBrep());
            DA.SetData(1, rfSurface.No);
            DA.SetData(2, rfSurface.MaterialNo);
            DA.SetData(3, rfSurface.Thickness);
            DA.SetData(4, rfSurface.Comment);
            DA.SetData(5, rfSurface.BoundaryLineList);
            DA.SetData(6, rfSurface.GeometryType);
            DA.SetData(7, rfSurface.ThicknessType);
            DA.SetData(8, rfSurface.StiffnessType);
            DA.SetData(9, rfSurface.Area);
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
                return Properties.Resources.Disassemble_Surface;                
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("395a73dd-5a12-4063-a4e2-5115a00aeaaf"); }
        }
    }
}
