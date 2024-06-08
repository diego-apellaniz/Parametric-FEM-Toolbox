using System;
using System.Collections.Generic;
using Rhino.Geometry;
using Grasshopper.Kernel;
using Parametric_FEM_Toolbox.RFEM;
using Parametric_FEM_Toolbox.HelperLibraries;
using Parametric_FEM_Toolbox.GUI;

namespace Parametric_FEM_Toolbox.Deprecated
{
    public class Component_RFNodalLoadProperties_GUI_OBSOLETE : GH_Component
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
        public Component_RFNodalLoadProperties_GUI_OBSOLETE()
          : base("RF Nodal Load Properties", "RFNodLoadProp", "Gets the properties of RFNodalLoad objects.", "B+G Toolbox", "RFEM")
        {
        }

        // Define Keywords to search for this Component more easily in Grasshopper
        public override IEnumerable<string> Keywords => new string[] {"rf", "rfnodalload", "nodal", "load", "properties" };

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Use the pManager object to register your input parameters.
            pManager.AddParameter(new Param_RFEM(), "Nodal Load", "NodLoad", "Input RFNodalLoad.", GH_ParamAccess.item);
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
            pManager.AddPointParameter("Location", "Loc", "Load Application point.", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Load Number", "No", "Index number of the RFEM object.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Load Case", "LC", "Load Case to assign the load to.", GH_ParamAccess.item);
            pManager.AddVectorParameter("Force [kN]", "F", "Load Force [kN]", GH_ParamAccess.item);
            pManager.AddVectorParameter("Moment [kNm]", "M", "Load Moment [kN]", GH_ParamAccess.item);
            pManager.AddTextParameter("Comment", "Comment", "Comment.", GH_ParamAccess.item);
            pManager.AddTextParameter("Node List", "NodeList", "List of nodes the support is attached to", GH_ParamAccess.item);
            pManager.AddTextParameter("LoadDefinitionType", "Def", "LoadDefinitionType", GH_ParamAccess.item);


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
            var rfNodalLoad = (RFNodalLoad)inGH.Value;
            // Output
            DA.SetDataList(0, rfNodalLoad.Location);
            DA.SetData(1, rfNodalLoad.No);
            DA.SetData(2, rfNodalLoad.LoadCase);
            DA.SetData(3, rfNodalLoad.Force);
            DA.SetData(4, rfNodalLoad.Moment);
            DA.SetData(5, rfNodalLoad.Comment);
            DA.SetData(6, rfNodalLoad.NodeList);
            DA.SetData(7, rfNodalLoad.LoadDefType);

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
                return Properties.Resources.Disassemble_NodalLoad;                
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("d546ce22-1c4d-40b0-afc1-8f2ccc78486e"); }
        }
    }
}
