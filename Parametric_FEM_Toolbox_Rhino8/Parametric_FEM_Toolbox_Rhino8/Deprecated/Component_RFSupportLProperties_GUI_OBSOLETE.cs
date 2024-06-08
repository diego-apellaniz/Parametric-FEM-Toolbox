using System;
using System.Collections.Generic;
using Rhino.Geometry;
using Grasshopper.Kernel;
using Parametric_FEM_Toolbox.RFEM;
using Parametric_FEM_Toolbox.GUI;
using System.Linq;

namespace Parametric_FEM_Toolbox.Deprecated
{
    public class Component_RFSupportLProperties_GUI_OBSOLETE : GH_Component
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
        public Component_RFSupportLProperties_GUI_OBSOLETE()
          : base("RF Line Support Properties", "RFLineSupProp", "Gets the properties of RFSupportL objects.", "B+G Toolbox", "RFEM")
        {
        }

        // Define Keywords to search for this Component more easily in Grasshopper
        public override IEnumerable<string> Keywords => new string[] {"rf", "rfsupportl", "line", "support", "properties"};

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Use the pManager object to register your input parameters.
            pManager.AddParameter(new Param_RFEM(), "Line Support", "LineSup", "Input RFSupportL.", GH_ParamAccess.item);
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
            pManager.AddCurveParameter("Lines", "Lines", "Lines or Curves the RFSupportL is attached to.", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Support Number", "No", "Index number of the RFEM object.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Displacement Dir X", "Tx", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]", GH_ParamAccess.item);
            pManager.AddNumberParameter("Displacement Dir Y", "Ty", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]", GH_ParamAccess.item);
            pManager.AddNumberParameter("Displacement Dir Z", "Tz", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]", GH_ParamAccess.item);
            pManager.AddNumberParameter("Rotation Dir X", "Rx", "(-1): Fixed; (0): Free; Other: Stiffness in [kNm/rad]", GH_ParamAccess.item);
            pManager.AddNumberParameter("Rotation Dir Y", "Ry", "(-1): Fixed; (0): Free; Other: Stiffness in [kNm/rad]", GH_ParamAccess.item);
            pManager.AddNumberParameter("Rotation Dir Z", "Rz", "(-1): Fixed; (0): Free; Other: Stiffness in [kNm/rad]", GH_ParamAccess.item);
            pManager.AddTextParameter("Comment", "Comment", "Comment.", GH_ParamAccess.item);
            pManager.AddTextParameter("Line List", "LineList", "List of lines the support is attached to", GH_ParamAccess.item);
            pManager.AddTextParameter("Reference System Type", "RSType", "Reference System Type", GH_ParamAccess.item);


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
            var rfSupport = (RFSupportL)inGH.Value;
            // Output
            DA.SetDataList(0, rfSupport.BaseLines.Select(x => x.ToCurve()));
            DA.SetData(1, rfSupport.No);
            DA.SetData(2, ((rfSupport.Tx < 0) ? rfSupport.Tx * 1000 : rfSupport.Tx));
            DA.SetData(3, ((rfSupport.Ty < 0) ? rfSupport.Ty * 1000 : rfSupport.Ty));
            DA.SetData(4, ((rfSupport.Tz < 0) ? rfSupport.Tz * 1000 : rfSupport.Tz));
            DA.SetData(5, ((rfSupport.Rx < 0) ? rfSupport.Rx * 1000 : rfSupport.Rx));
            DA.SetData(6, ((rfSupport.Ry < 0) ? rfSupport.Ry * 1000 : rfSupport.Ry));
            DA.SetData(7, ((rfSupport.Rz < 0) ? rfSupport.Rz * 1000 : rfSupport.Rz));
            DA.SetData(8, rfSupport.Comment);
            DA.SetData(9, rfSupport.LineList);
            DA.SetData(10, rfSupport.RSType);
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
                return Properties.Resources.Disassemble_SupportL;                
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("13148012-92e3-4ff4-9d3c-566b94ab56c9"); }
        }
    }
}
