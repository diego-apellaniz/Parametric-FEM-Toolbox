using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using Parametric_FEM_Toolbox.UIWidgets;

using Parametric_FEM_Toolbox.HelperLibraries;
using Parametric_FEM_Toolbox.RFEM;
using Parametric_FEM_Toolbox.Utilities;
using Dlubal.RFEM5;
using System.Runtime.InteropServices;
using Grasshopper;
using Grasshopper.Kernel.Data;

namespace Parametric_FEM_Toolbox.GUI
{
    public class Component_Calculate_GUI : GH_SwitcherComponent
    {
        // Declare class variables outside the method "SolveInstance" so their values persist 
        // when the method is called again.

        private List<Vector3d> _maxDisplacements;
        private List<string> _loadCasesAndCombos;
  

        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public Component_Calculate_GUI()
          : base("Calculate Model", "Calculate", "Calculate RFEM Model.", "B+G Toolbox", "RFEM")
        {
        }

        // Define Keywords to search for this Component more easily in Grasshopper
        public override IEnumerable<string> Keywords => new string[] { "rf", "calculate", "model" };

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Use the pManager object to register your input parameters.
            pManager.AddBooleanParameter("Run component?", "Run", "If true, the programm is executed.", GH_ParamAccess.item, false);
            pManager.AddTextParameter("Load Case to be Calculated", "LCases", "Load Case to be Calculated. If no input is provided, all load cases and combos will be calculated.", GH_ParamAccess.list);
            pManager[1].Optional = true;
            //modelDataCount = pManager.ParamCount;
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
            pManager.AddTextParameter("Load Cases", "LCases", "Calculated Load Cases and Load Combinations.", GH_ParamAccess.list);
            pManager.AddVectorParameter("Maximum Displacement", "Disp", "Maximum Displacement [m].", GH_ParamAccess.list);

            // Output parameters do not have default values, but they too must have the correct access type.
            // Sometimes you want to hide a specific parameter from the Rhino preview.
            // You can use the HideParameter() method as a quick way:
            // pManager.HideParameter(0);
        }

        protected override void RegisterEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit("Calculate", "Calculate", "Calculate RFEM Model.", Properties.Resources.Calculate);
            mngr.RegisterUnit(evaluationUnit);

            // Advanced
            evaluationUnit.RegisterInputParam(new Param_RFEM(), "Trigger", "Trigger", "Input to trigger the optimization", GH_ParamAccess.tree);
            evaluationUnit.Inputs[0].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_String(), "Model Name", "Model Name", "Segment of the name of the RFEM Model to get information from", GH_ParamAccess.item);
            evaluationUnit.Inputs[1].Parameter.Optional = true;
            GH_ExtendableMenu gH_ExtendableMenu4 = new GH_ExtendableMenu(0, "Advanced");
            gH_ExtendableMenu4.Name = "Advanced";
            gH_ExtendableMenu4.Expand();
            evaluationUnit.AddMenu(gH_ExtendableMenu4);
            gH_ExtendableMenu4.RegisterInputPlug(evaluationUnit.Inputs[0]);
            gH_ExtendableMenu4.RegisterInputPlug(evaluationUnit.Inputs[1]);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA, EvaluationUnit unit)
        {
            // RFEM variables
            var modelName = "";
            IModel model = null;
            IModelData data = null;
            ILoads loads = null;
            ICalculation results = null;            

            // Output message
            var msg = new List<string>();

            // Assign GH Input
            bool run = false;
            var iLcases = new List<string>();
            DA.GetData(0, ref run);

            // Do stuff
            if (run)
            {
                if (!DA.GetData(3, ref modelName))
                {
                    Component_GetData.ConnectRFEM(ref model, ref data);
                }
                else
                {
                    Component_GetData.ConnectRFEM(modelName, ref model, ref data);
                }
                try
                {
                      // Get loads
                    Component_GetData.GetLoadsFromRFEM(model, ref loads);
                    // Get calculation
                    results = model.GetCalculation();                    
                    // Update results
                    _maxDisplacements = new List<Vector3d>();
                    _loadCasesAndCombos = new List<string>();
                    // Get load cases and combos
                    if (!DA.GetDataList(1, iLcases))
                    {
                        var errors = results.CalculateApp();
                        if (errors != null)
                        {
                            msg.AddRange(errors.Select(x => x.Description));
                        }
                        _loadCasesAndCombos = loads.GetLoadCasesAndCombos(0, 0, 0);
                    }else
                    {
                        _loadCasesAndCombos = iLcases;                        
                    }
                    // Get results
                    foreach (var lc in _loadCasesAndCombos)
                    {
                        Vector3d displacement = new Vector3d(0, 0, 0);
                        Component_GetResults.GetResults(ref results, lc, ref displacement, ref msg);
                        _maxDisplacements.Add(displacement);
                    }
                }
                catch (Exception ex)
                {
                    // Clear output!!!
                    results = null;
                    _maxDisplacements = null;
                    _loadCasesAndCombos = null;
                    Component_GetData.DisconnectRFEM(ref model, ref data);
                    throw ex;
                }
                Component_GetData.DisconnectRFEM(ref model, ref data);
            }
            if (msg.Count > 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, String.Join(System.Environment.NewLine, msg.ToArray()));
            }
            {
                DA.SetDataList(0, _loadCasesAndCombos);
                DA.SetDataList(1, _maxDisplacements);
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
            get { return GH_Exposure.senary; }
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
                return Properties.Resources.Calculate;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("5cc558f8-d57e-4364-9d96-6707bdf85844"); }
        }
    }
}