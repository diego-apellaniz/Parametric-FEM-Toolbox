using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel;
using Rhino.Geometry;

using Parametric_FEM_Toolbox.RFEM;
using Parametric_FEM_Toolbox.Utilities;
using Dlubal.RFEM5;
using System.Runtime.InteropServices;
using Parametric_FEM_Toolbox.UIWidgets;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Parametric_FEM_Toolbox.Deprecated;

namespace Parametric_FEM_Toolbox.GUI
{
    public class Component_RFMemberHinge_GUI_OBSOLETE : GH_SwitcherComponent
    {
        // Declare class variables outside the method "SolveInstance" so their values persist 
        // when the method is called again.
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
        public Component_RFMemberHinge_GUI_OBSOLETE()
          : base("RF Member Hinge", "RFMemberHinge", "Assembles and Disassembles RFMemberHinge objects.", "B+G Toolbox", "RFEM")
        {
            this.Hidden = (true);
        }

        // Define Keywords to search for this Component more easily in Grasshopper
        public override IEnumerable<string> Keywords => new string[] {"rf", "rfmemberhinge",};


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            // Use the pManager object to register your input parameters.
            //pManager.AddBooleanParameter("Nodes in RFEM Model", "Nodes", "Nodes to get from the RFEM Model.", GH_ParamAccess.item, false);

            // If you want to change properties of certain parameters, 
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


            // Sometimes you want to hide a specific parameter from the Rhino preview.
            // You can use the HideParameter() method as a quick way:
            // pManager.HideParameter(0);
        }

        protected override void OnComponentLoaded()
        {
            base.OnComponentLoaded();
            foreach (SubComponent item in subcomponents_)
            {
                item.OnComponentLoaded();
            }
        }

        // The PostConstructor is called from within each constructor.DO NOT OVERRIDE THIS unless you know what you are doing.
        protected override void RegisterEvaluationUnits(EvaluationUnitManager mngr)
        {
            subcomponents_.Add(new SubComponent_RFMemberHinge_Assemble_GUI_OBSOLETE());
            subcomponents_.Add(new SubComponent_RFMemberHinge_Disassemble_GUI_OBSOLETE());

            foreach (SubComponent item in subcomponents_)
            {
                item.registerEvaluationUnits(mngr);
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
                return Properties.Resources.Assemble_MemberRelease;                
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("a474b3db-6051-48db-9cd1-cfdcb30ec26b"); }
        }
    }
}
