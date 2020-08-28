using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Parametric_FEM_Toolbox.RFEM;
using Parametric_FEM_Toolbox.GUI;

namespace Parametric_FEM_Toolbox.Deprecated
{
    public class Component_RFCroSecProperties_GUI_OBSOLETE : GH_Component
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
        public Component_RFCroSecProperties_GUI_OBSOLETE()
          : base("RF Cross Section Properties", "RFCroSecProp", "Gets the properties of RFCroSec objects.", "B+G Toolbox", "RFEM")
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
            pManager.AddParameter(new Param_RFEM(), "Cross Section", "CroSec", "Input RFCroSec.", GH_ParamAccess.item);
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
            pManager.AddTextParameter("Description", "Desc", "Name or Description of Cross Section.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Cross Section Number", "No", "Optional index number to assign to the RFEM object.", GH_ParamAccess.item);
            pManager.AddTextParameter("Comment", "Comment", "Comment.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Material Number", "MatNo", "Number of Material assigned to the Cross-Section", GH_ParamAccess.item);
            pManager.AddNumberParameter("AxialArea [m²]", "A", "AxialArea [m²]", GH_ParamAccess.item);
            pManager.AddNumberParameter("ShearAreaY [m²]", "Ay", "ShearAreaY [m²]", GH_ParamAccess.item);
            pManager.AddNumberParameter("ShearAreaZ [m²]", "Az", "ShearAreaZ [m²]", GH_ParamAccess.item);
            pManager.AddNumberParameter("BendingMomentY [m⁴]", "Iy", "BendingMomentY [m⁴]", GH_ParamAccess.item);
            pManager.AddNumberParameter("BendingMomentZ [m⁴]", "Iz", "BendingMomentZ [m⁴]", GH_ParamAccess.item);
            pManager.AddNumberParameter("TorsionMoment [m⁴]", "Jt", "TorsionMoment [m⁴]", GH_ParamAccess.item);
            pManager.AddNumberParameter("Rotation Angle [°]", "β", "Rotation Angle [°]", GH_ParamAccess.item);
            pManager.AddNumberParameter("TemperatureLoadWidth [m]", "TempW", "TemperatureLoadWidth [m]", GH_ParamAccess.item);
            pManager.AddNumberParameter("TemperatureLoadDepth [m]", "TempD", "TemperatureLoadDepth [m]", GH_ParamAccess.item);
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
            var rFCroSec = (RFCroSec)inGH.Value;
            // Output
            DA.SetData(0, rFCroSec.Description);
            DA.SetData(1, rFCroSec.No);
            DA.SetData(2, rFCroSec.Comment);
            DA.SetData(3, rFCroSec.MatNo);
            DA.SetData(4, rFCroSec.A);
            DA.SetData(5, rFCroSec.Ay);
            DA.SetData(6, rFCroSec.Az);
            DA.SetData(7, rFCroSec.Iy);
            DA.SetData(8, rFCroSec.Iz);
            DA.SetData(9, rFCroSec.Jt);
            DA.SetData(10, rFCroSec.RotationAngle);
            DA.SetData(11, rFCroSec.TempWidth);
            DA.SetData(12, rFCroSec.TempDepth);
            DA.SetData(13, rFCroSec.TextID);
            DA.SetData(14, rFCroSec.UserDefined);
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
                return Properties.Resources.Disassemble_CroSec;                
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("2d893cfa-ae0e-4c52-a805-5b0aea710931"); }
        }
    }
}
