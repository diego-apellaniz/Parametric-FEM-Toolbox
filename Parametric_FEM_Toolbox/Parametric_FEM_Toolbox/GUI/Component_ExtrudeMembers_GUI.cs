using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Parametric_FEM_Toolbox.HelperLibraries;
using Parametric_FEM_Toolbox.RFEM;
using Rhino.Geometry;
using System.Linq;
using Grasshopper.Kernel.Types.Transforms;
using Parametric_FEM_Toolbox.UIWidgets;

namespace Parametric_FEM_Toolbox.GUI
{
    public class Component_ExtrudeMembers_GUI : GH_SwitcherComponent    
    {
        private List<SubComponent> subcomponents_ = new List<SubComponent>();
        public override string UnitMenuName => "Type of Component";
        protected override string DefaultEvaluationUnit => subcomponents_[0].name();

        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public Component_ExtrudeMembers_GUI()
          : base("Extrude Members", "Extrude", "Extrude members using the geometry of the cross sections assigned to them.", "B+G Toolbox", "RFEM")
        {
            this.Hidden = (false);
        }

        // Define Keywords to search for this Component more easily in Grasshopper
        public override IEnumerable<string> Keywords => new string[] {"rf", "rfextrude", "extrude", "members"};

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Use the pManager object to register your input parameters.
            pManager.AddParameter(new Param_RFEM(), "RFMembers", "M", "RF-Memebers from the RFEM model to extrude in 3d.", GH_ParamAccess.item);
            pManager.AddParameter(new Param_RFEM(), "RFCroSec", "CS", "Cross sections attached to the input members", GH_ParamAccess.list);
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
            //pManager.AddBrepParameter("Extrussions", "E", "Extruded members.", GH_ParamAccess.list);
            //pManager.AddPlaneParameter("Extrussions", "Caux", "Extruded members.", GH_ParamAccess.list);

            // Sometimes you want to hide a specific parameter from the Rhino preview.
            // You can use the HideParameter() method as a quick way:
            // pManager.HideParameter(0);
        }

        protected override void RegisterEvaluationUnits(EvaluationUnitManager mngr)
        {
            subcomponents_.Add(new SubComponent_ExtrudeMembers_NURBS_GUI());
            subcomponents_.Add(new SubComponent_ExtrudeMembers_MESH_GUI());

            foreach (SubComponent item in subcomponents_)
            {
                item.registerEvaluationUnits(mngr);
                item.Parent_Component = (GH_DocumentObject)this;
            }
        }


        protected override void OnComponentLoaded()
        {
            base.OnComponentLoaded();
            foreach (SubComponent item in subcomponents_)
            {
                item.OnComponentLoaded();
            }
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA, EvaluationUnit unit)
        {
            if (unit != null)
            {
                foreach (SubComponent item in subcomponents_)
                {
                    if (unit.Name.Equals(item.name()))
                    {
                        item.SolveInstance(DA, out string msg, out GH_RuntimeMessageLevel level);
                        if (msg != "")
                        {
                            this.AddRuntimeMessage(level, msg + "It may cause errors.");
                        }
                        return;
                    }
                }
                throw new Exception("Invalid sub-component");
            }
        }

        /// <summary>
        /// The Exposure property controls where in the panel a component icon 
        /// will appear. There are seven possible locations (primary to septenary), 
        /// each of which can be combined with the GH_Exposure.obscure flag, which 
        /// ensures the component will only be visible on panel dropdowns.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
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
                return Properties.Resources.Extrude_Members;                
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("a71f7b05-a7a0-40ff-9bbb-d1492eb15112"); }
        }
    }
}
