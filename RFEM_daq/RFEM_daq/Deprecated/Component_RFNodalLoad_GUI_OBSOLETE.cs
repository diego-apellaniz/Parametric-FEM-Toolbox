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
    public class Component_RFNodalLoad_GUI_OBSOLETE : GH_SwitcherComponent
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
        public Component_RFNodalLoad_GUI_OBSOLETE()
          : base("RF Nodal Load", "RFNodLoad", "Creates a RFNodalLoad object to define new data or modify existing data " +
                "in the RFEM model.", "B+G Toolbox", "RFEM")
        {
        }

        // Define Keywords to search for this Component more easily in Grasshopper
        public override IEnumerable<string> Keywords => new string[] {"rf", "rfnodalload", "load", "nodal"};

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Use the pManager object to register your input parameters.
            pManager.AddPointParameter("Location", "Loc", "Load Application point.", GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager.AddIntegerParameter("Load Case", "LC", "Load Case to assign the load to.", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddVectorParameter("Force [kN]", "F", "Load Force [kN]", GH_ParamAccess.item);
            pManager[2].Optional = true;
            pManager.AddVectorParameter("Moment [kNm]", "M", "Load Moment [kN]", GH_ParamAccess.item);
            pManager[3].Optional = true;
            pManager.AddIntegerParameter("Load Number", "No", "Optional index number to assign to the RFEM object.", GH_ParamAccess.item);
            pManager[4].Optional = true;
            pManager.AddTextParameter("Comment", "Comment", "Comment.", GH_ParamAccess.item);
            pManager[5].Optional = true;

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
            pManager.RegisterParam(new Param_RFEM(), "Nodal Load", "NodLoad", "Output RFNodalLoad.", GH_ParamAccess.item);

            // Sometimes you want to hide a specific parameter from the Rhino preview.
            // You can use the HideParameter() method as a quick way:
            // pManager.HideParameter(0);
        }

        protected override void RegisterEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit("Assemble Nodal Support", "NodSup", "Creates a RFSupportP object to define new data or modify existing data " +
                "in the RFEM model.");
            mngr.RegisterUnit(evaluationUnit);

            evaluationUnit.RegisterInputParam(new Param_String(), "Node List", "NodeList", "Node List", GH_ParamAccess.item);
            evaluationUnit.Inputs[0].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_String(), "LoadDefinitionType", "Def", UtilLibrary.DescriptionRFTypes(typeof(LoadDefinitionType)), GH_ParamAccess.item);
            evaluationUnit.Inputs[1].Parameter.Optional = true;


            evaluationUnit.RegisterInputParam(new Param_RFEM(), "Nodal Load", "NodLoad", "Load object from the RFEM model to modify", GH_ParamAccess.item);
            evaluationUnit.Inputs[2].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Boolean(), "Modify", "Modify", "Modify node?", GH_ParamAccess.item);
            evaluationUnit.Inputs[3].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Boolean(), "Delete", "Delete", "Delete node?", GH_ParamAccess.item);
            evaluationUnit.Inputs[4].Parameter.Optional = true;

            GH_ExtendableMenu gH_ExtendableMenu = new GH_ExtendableMenu(0, "advanced");
            gH_ExtendableMenu.Name = "Advanced";
            gH_ExtendableMenu.Collapse();
            evaluationUnit.AddMenu(gH_ExtendableMenu);
            for (int i = 0; i < 2; i++)
            {
                gH_ExtendableMenu.RegisterInputPlug(evaluationUnit.Inputs[i]);
            }

            GH_ExtendableMenu gH_ExtendableMenu2 = new GH_ExtendableMenu(1, "modify");
            gH_ExtendableMenu2.Name = "Modify";
            gH_ExtendableMenu2.Collapse();
            evaluationUnit.AddMenu(gH_ExtendableMenu2);
            for (int i = 2; i < 2+3; i++)
            {
                gH_ExtendableMenu2.RegisterInputPlug(evaluationUnit.Inputs[i]);
            }
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA, EvaluationUnit unit)
        {
            //var line = new LineCurve();
            var point = new Point3d();
            var noIndex = 0;
            var loadCase = 0;
            var comment = "";
            var rfNodalLoad = new RFNodalLoad();
            var inRFEM = new GH_RFEM();
            var mod = false;
            var del = false;
            var force = new Vector3d();
            var moment = new Vector3d();
            var nodeList = "";
            var definition = "";
            //int newNo = 0;


            if (DA.GetData(8, ref inRFEM))
            {
                rfNodalLoad = new RFNodalLoad((RFNodalLoad)inRFEM.Value);
            }else if ((DA.GetData(0, ref point)) && (DA.GetData(1, ref loadCase)))
            {
                var inPoints = new List<Point3d>();
                inPoints.Add(point);
                rfNodalLoad = new RFNodalLoad(new NodalLoad(), inPoints, loadCase);
                rfNodalLoad.LoadDefType = LoadDefinitionType.ByComponentsType;
            }
            else
            {
                return;
            }
            if (DA.GetData(9, ref mod))
            {
                rfNodalLoad.ToModify = mod;
            }
            if (DA.GetData(10, ref del))
            {
                rfNodalLoad.ToDelete = del;
            }
            if (DA.GetData(4, ref noIndex))
            {
                rfNodalLoad.No = noIndex;
            }
            if (DA.GetData(5, ref comment))
            {
                rfNodalLoad.Comment = comment;
            }
            if (DA.GetData(2, ref force))
            {
                rfNodalLoad.Force = force;                
            }
            if (DA.GetData(3, ref moment))
            {
                rfNodalLoad.Moment = moment;
            }
            // Add warning in case of null forces
            if ((rfNodalLoad.Force.Length == 0.0) && (rfNodalLoad.Moment.Length == 0))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Null forces will result in an error in RFEM");
            }
            if (DA.GetData(6, ref nodeList))
            {
                rfNodalLoad.NodeList = nodeList;
            }
            if (DA.GetData(7, ref definition))
            {
                Enum.TryParse(definition, out LoadDefinitionType myDef);
                rfNodalLoad.LoadDefType = myDef;
            }

            DA.SetData(0, rfNodalLoad);
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
                return RFEM_daq.Properties.Resources.Assemble_NodalLoad;                
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("1798fa54-7920-4a8a-a3ba-caff0fbc7061"); }
        }
    }
}
