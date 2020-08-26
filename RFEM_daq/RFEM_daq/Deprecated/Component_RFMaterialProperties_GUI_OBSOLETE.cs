using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using RFEM_daq.RFEM;
using RFEM_daq.GUI;

namespace RFEM_daq.Deprecated
{
    public class Component_RFMaterialProperties_GUI_OBSOLETE : GH_Component
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
        public Component_RFMaterialProperties_GUI_OBSOLETE()
          : base("RF Material Properties", "RFMaterial", "Gets the properties of RFMaterial objects.", "B+G Toolbox", "RFEM")
        {
        }

        // Define Keywords to search for this Component more easily in Grasshopper
        public override IEnumerable<string> Keywords => new string[] {"rf", "rfcrosec", "cross", "section", "properties" };

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Use the pManager object to register your input parameters.
            pManager.AddParameter(new Param_RFEM(), "Material", "Mat", "Input RFMaterial.", GH_ParamAccess.item);
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
            pManager.AddTextParameter("Description", "Desc", "Name or Description of Material.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Material Number", "No", "Optional index number to assign to the RFEM object.", GH_ParamAccess.item);
            pManager.AddTextParameter("Comment", "Comment", "Comment.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Elasticity Modulus [N/m²]", "E", "Elasticity Modulus [N/m²]", GH_ParamAccess.item);
            pManager.AddNumberParameter("Poisson Ratio", "Mu", "Poisson Ratio", GH_ParamAccess.item);
            pManager.AddNumberParameter("Shear Modulus [N/m²]", "G", "Shear Modulus [N/m²]", GH_ParamAccess.item);
            pManager.AddNumberParameter("Specific Weight [N/m³]", "W", "Specific Weight [N/m³]", GH_ParamAccess.item);
            pManager.AddNumberParameter("Thermal Expansion [1/°]", "Alpha", "Thermal Expansion [1/°]", GH_ParamAccess.item);
            pManager.AddNumberParameter("Partial Safety Factor", "Gamma", "Partial Safety Factor", GH_ParamAccess.item);
            pManager.AddTextParameter("Material Model Type", "Type", "Material Model Type.", GH_ParamAccess.item);
            pManager.AddTextParameter("TextID", "TextID", "TextID.", GH_ParamAccess.item);
            pManager.AddBooleanParameter("User Defined", "User", "User Defined.", GH_ParamAccess.item);

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
            var rfMat = (RFMaterial)inGH.Value;
            // Output
            DA.SetData(0, rfMat.Description);
            DA.SetData(1, rfMat.No);
            DA.SetData(2, rfMat.Comment);
            DA.SetData(3, rfMat.E);
            DA.SetData(4, rfMat.Mu);
            DA.SetData(5, rfMat.G);
            DA.SetData(6, rfMat.W);
            DA.SetData(7, rfMat.Alpha);
            DA.SetData(8, rfMat.Gamma);
            DA.SetData(9, rfMat.ModelType);
            DA.SetData(10, rfMat.TextID);
            DA.SetData(11, rfMat.UserDefined);
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
                return RFEM_daq.Properties.Resources.Disassemble_Material;                
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("5d8de6ab-6ecb-4314-af00-120f374af2c7"); }
        }
    }
}
