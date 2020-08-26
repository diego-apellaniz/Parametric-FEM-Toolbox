using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using RFEM_daq.UIWidgets;

using RFEM_daq.HelperLibraries;
using RFEM_daq.RFEM;
using RFEM_daq.Utilities;
using Dlubal.RFEM5;
using System.Runtime.InteropServices;
using RFEM_daq.GUI;

namespace RFEM_daq.Deprecated
{
    public class Component_RFNode_GUI_OBSOLETE : GH_SwitcherComponent
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
        public Component_RFNode_GUI_OBSOLETE()
          : base("RF Node", "RFNode", "Creates a RFNode object to define new data or modify existing data " +
                "in the RFEM model.", "B+G Toolbox", "RFEM")
        {
        }

        // Define Keywords to search for this Component more easily in Grasshopper
        public override IEnumerable<string> Keywords => new string[] {"rf", "rfnode", "node"};

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Use the pManager object to register your input parameters.
            pManager.AddPointParameter("Point Coordinates [m]", "Point", "Point Coordinates [m] to assemble the RFNode from.", GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager.AddIntegerParameter("Node Number", "No", "Optional index number to assign to the RFEM object.", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddTextParameter("Comment", "Comment", "Comment.", GH_ParamAccess.item);
            pManager[2].Optional = true;

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
            pManager.RegisterParam(new Param_RFEM(), "Node", "Node", "Output RFNode.", GH_ParamAccess.item);

            // Sometimes you want to hide a specific parameter from the Rhino preview.
            // You can use the HideParameter() method as a quick way:
            // pManager.HideParameter(0);
        }

        protected override void RegisterEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit("Assemble Node", "Node", "Creates a RFNode object to define new data or modify existing data " +
                "in the RFEM model.");
            mngr.RegisterUnit(evaluationUnit);
            evaluationUnit.RegisterInputParam(new Param_RFEM(), "Node Coordinates [m]", "Node", "Node object from the RFEM model to modify", GH_ParamAccess.item);
            evaluationUnit.Inputs[0].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Boolean(), "Modify", "Modify", "Modify node?", GH_ParamAccess.item);
            evaluationUnit.Inputs[1].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Boolean(), "Delete", "Delete", "Delete node?", GH_ParamAccess.item);
            evaluationUnit.Inputs[2].Parameter.Optional = true;

            GH_ExtendableMenu gH_ExtendableMenu = new GH_ExtendableMenu(0, "modify");
            gH_ExtendableMenu.Name = "Modify";
            gH_ExtendableMenu.Collapse();
            evaluationUnit.AddMenu(gH_ExtendableMenu);
            for (int i = 0; i < 3; i++)
            {
                gH_ExtendableMenu.RegisterInputPlug(evaluationUnit.Inputs[i]);
            }
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA, EvaluationUnit unit)
        {
            var point = new Point3d();
            var noIndex = 0;
            var comment = "";
            var rFNode = new RFNode();
            var inRFEM = new GH_RFEM();
            var mod = false;
            var del = false;
            //int newNo = 0;
            var dataprovided = false;

            if (DA.GetData(3, ref inRFEM))
            {
                rFNode = new RFNode((RFNode)inRFEM.Value);
                dataprovided = true;
            }
            if (DA.GetData(0, ref point))
            {
                rFNode.RefObjectNo = 0;
                rFNode.X = point.X;
                rFNode.Y = point.Y;
                rFNode.Z = point.Z;
                rFNode.Location = point;
                dataprovided = true;
            }
            if (!dataprovided)
            {
                return;
            }
            if (DA.GetData(1, ref noIndex))
            {
                rFNode.No = noIndex;
            }
            if (DA.GetData(4, ref mod))
            {
                rFNode.ToModify = mod;
                //if (!(rFNode.No>0))
                //{
                //    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Index nummer required in order to modify or delete RFEM objects.");
                //}
            }
            if (DA.GetData(5, ref del))
            {
                rFNode.ToDelete = del;
            }
            if (DA.GetData(2, ref comment))
            {
                rFNode.Comment = comment;
            }

            DA.SetData(0, rFNode);
        }

        // Additonal functions
        
      


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
                return RFEM_daq.Properties.Resources.Assemble_Node;                
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("56145206-35c1-4c83-9822-54126a1f89b7"); }
        }
    }
}
