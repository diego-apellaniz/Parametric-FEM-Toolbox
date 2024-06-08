using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Parametric_FEM_Toolbox.RFEM;
using Parametric_FEM_Toolbox.GUI;

namespace Parametric_FEM_Toolbox.Deprecated
{
    public class Component_RFMemberProperties_GUI_OBSOLETE : GH_Component
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
        public Component_RFMemberProperties_GUI_OBSOLETE()
          : base("RF Member Properties", "RFMemberProp", "Gets the properties of RFMember objects.", "B+G Toolbox", "RFEM")
        {
        }

        // Define Keywords to search for this Component more easily in Grasshopper
        public override IEnumerable<string> Keywords => new string[] {"rf", "rfmember", "member", "properties" };

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Use the pManager object to register your input parameters.
            pManager.AddParameter(new Param_RFEM(), "Member", "Member", "Input RFMember.", GH_ParamAccess.item);
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
            pManager.AddCurveParameter("Line", "Line", "Line or Curve to assemble the RFLine from.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Member Number", "No", "Index number of the RFEM object.", GH_ParamAccess.item);
            pManager.AddTextParameter("Comment", "Comment", "Comment.", GH_ParamAccess.item);
            pManager.AddTextParameter("Line Number", "LineNo", "Line Number", GH_ParamAccess.item);
            pManager.AddTextParameter("Member Type", "Type", "Member Type", GH_ParamAccess.item);
            pManager.AddNumberParameter("Rotation Angle [°]", "β", "Rotation Angle [°]", GH_ParamAccess.item);
            pManager.AddTextParameter("Rotation Type", "Rot Type", "Rotation Type", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Start Cross-Section", "S CroSec", "Number of End Cross-Section", GH_ParamAccess.item);
            pManager.AddIntegerParameter("End Cross-Section", "E CroSec", "Number of End Cross-Section", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Start Hinge", "S Hinge", "Number of Start Hinge", GH_ParamAccess.item);
            pManager.AddIntegerParameter("End Hinge", "E Hinge", "Number of End Hinge", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Eccentricity", "Ecc", "Number of Eccentricity", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Division", "Div", "Number of Division", GH_ParamAccess.item);
            pManager.AddTextParameter("Taper Shape", "Taper", "Taper Shape", GH_ParamAccess.item);
            pManager.AddNumberParameter("Weight [kg]", "W", "Member Weight", GH_ParamAccess.item);
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
            var rFMember = (RFMember)inGH.Value;
            // Output
            DA.SetData(0, rFMember.BaseLine.ToCurve());
            DA.SetData(1, rFMember.No);
            DA.SetData(2, rFMember.Comment);
            DA.SetData(3, rFMember.LineNo);
            DA.SetData(4, rFMember.Type);
            DA.SetData(5, rFMember.RotationAngle);
            DA.SetData(6, rFMember.RotationType);
            DA.SetData(7, rFMember.StartCrossSectionNo);
            DA.SetData(8, rFMember.EndCrossSectionNo);
            DA.SetData(9, rFMember.StartHingeNo);
            DA.SetData(10, rFMember.EndHingeNo);
            DA.SetData(11, rFMember.EccentricityNo);
            DA.SetData(12, rFMember.DivisionNo);
            DA.SetData(13, rFMember.TaperShape);
            DA.SetData(14, rFMember.Weight);
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
                return Properties.Resources.Disassemble_Member;                
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("689e8fce-b60a-42d2-8ad2-6094a8f3ef0d"); }
        }
    }
}
