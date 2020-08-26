using System;
using System.Collections.Generic;
using Rhino.Geometry;
using Grasshopper.Kernel;
using RFEM_daq.RFEM;
using RFEM_daq.GUI;
using System.Linq;

namespace RFEM_daq.Deprecated
{
    public class Component_RFLineHingeProperties_GUI_OBSOLETE : GH_Component
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
        public Component_RFLineHingeProperties_GUI_OBSOLETE()
          : base("RF Line Hinge Properties", "RFLineHingeProp", "Gets the properties of RFLineHinge objects.", "B+G Toolbox", "RFEM")
        {
        }

        // Define Keywords to search for this Component more easily in Grasshopper
        public override IEnumerable<string> Keywords => new string[] {"rf", "rflinehinge", "line", "hinge", "properties"};

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Use the pManager object to register your input parameters.
            pManager.AddParameter(new Param_RFEM(), "Line Hinge", "LineHinge", "Input RFLineHinge.", GH_ParamAccess.item);
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
            pManager.AddCurveParameter("Line", "Line", "Line or Curve the RFLineHinge is attached to.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Hinge Number", "No", "Index number of the RFEM object.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Line No", "LineNo", "Index number of the line the hinge is attached to", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Sfc No", "SfcNo", "Index number of the surface the hinge is attached to", GH_ParamAccess.item);
            pManager.AddNumberParameter("Displacement Dir X", "Tx", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]", GH_ParamAccess.item);
            pManager.AddNumberParameter("Displacement Dir Y", "Ty", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]", GH_ParamAccess.item);
            pManager.AddNumberParameter("Displacement Dir Z", "Tz", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]", GH_ParamAccess.item);
            pManager.AddNumberParameter("Rotation Dir X", "Rx", "(-1): Fixed; (0): Free; Other: Stiffness in [kNm/rad]", GH_ParamAccess.item);
            pManager.AddNumberParameter("Rotation Dir Y", "Ry", "(-1): Fixed; (0): Free; Other: Stiffness in [kNm/rad]", GH_ParamAccess.item);
            pManager.AddNumberParameter("Rotation Dir Z", "Rz", "(-1): Fixed; (0): Free; Other: Stiffness in [kNm/rad]", GH_ParamAccess.item);
            pManager.AddTextParameter("Comment", "Comment", "Comment.", GH_ParamAccess.item);            
            pManager.AddTextParameter("Side", "Side", "Hinge Side Type", GH_ParamAccess.item);


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
            var rfLineHinge = (RFLineHinge)inGH.Value;
            // Output
            DA.SetData(0, rfLineHinge.BaseLine.ToCurve());
            DA.SetData(1, rfLineHinge.No);
            DA.SetData(2, rfLineHinge.LineNo);
            DA.SetData(3, rfLineHinge.SfcNo);
            DA.SetData(4, ((rfLineHinge.Tx < 0) ? rfLineHinge.Tx * 1000 : rfLineHinge.Tx));
            DA.SetData(5, ((rfLineHinge.Ty < 0) ? rfLineHinge.Ty * 1000 : rfLineHinge.Ty));
            DA.SetData(6, ((rfLineHinge.Tz < 0) ? rfLineHinge.Tz * 1000 : rfLineHinge.Tz));
            DA.SetData(7, ((rfLineHinge.Rx < 0) ? rfLineHinge.Rx * 1000 : rfLineHinge.Rx));
            DA.SetData(8, ((rfLineHinge.Ry < 0) ? rfLineHinge.Ry * 1000 : rfLineHinge.Ry));
            DA.SetData(9, ((rfLineHinge.Rz < 0) ? rfLineHinge.Rz * 1000 : rfLineHinge.Rz));
            DA.SetData(10, rfLineHinge.Comment);            
            DA.SetData(11, rfLineHinge.Side);
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
                return RFEM_daq.Properties.Resources.Disassemble_LineHinge;                
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("4cc263fd-c9a2-4bd9-bd5d-d81d7a7858ab"); }
        }
    }
}
