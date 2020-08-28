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
using Dlubal.STEEL_EC3;
using System.Runtime.InteropServices;

namespace Parametric_FEM_Toolbox.GUI
{
    public class Component_OptimizeCS_GUI : GH_SwitcherComponent
    {

        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public Component_OptimizeCS_GUI()
          : base("Optimize Cross Sections", "Optimize CroSec", "Performs a Cross Section Optimization with the EC3 Module.", "B+G Toolbox", "RFEM")
        {
        }

        // Define Keywords to search for this Component more easily in Grasshopper
        public override IEnumerable<string> Keywords => new string[] {"rf", "optimize", "crosec", "cross section", "optimization" };

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Use the pManager object to register your input parameters.
            //pManager.AddIntegerParameter("Cross Sections", "CroSecs", "Cross Section Indexes to optimize with the EC3 Module.", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Load Combinations", "LoadCombo", "Load Combinations to run in the EC3 Module.", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Run component?", "Run", "If true, the programm is executed.", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
             //Use the pManager object to register your output parameters.
             //Output parameters do not have default values, but they too must have the correct access type.
            pManager.AddNumberParameter("Steel Mass", "Mass", "Steel Mass [kg] of the optimized structure.", GH_ParamAccess.item);

            // Sometimes you want to hide a specific parameter from the Rhino preview.
            // You can use the HideParameter() method as a quick way:
            // pManager.HideParameter(0);
        }

        protected override void RegisterEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit("Optimize Cross Sections", "Optimize CroSec", "Performs a Cross Section Optimization with the EC3 Module.", Properties.Resources.icon_SetData);
            mngr.RegisterUnit(evaluationUnit);


            // Advanced

            evaluationUnit.RegisterInputParam(new Param_RFEM(), "Trigger", "Trigger", "Input to trigger the optimization", GH_ParamAccess.tree);
            evaluationUnit.Inputs[0].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_String(), "Model Name", "Model Name", "Segment of the name of the RFEM Model", GH_ParamAccess.item);
            evaluationUnit.Inputs[1].Parameter.Optional = true;

            GH_ExtendableMenu gH_ExtendableMenu = new GH_ExtendableMenu(0, "Advanced Items");
            gH_ExtendableMenu.Name = "Advanced";
            gH_ExtendableMenu.Collapse();
            evaluationUnit.AddMenu(gH_ExtendableMenu);
            gH_ExtendableMenu.RegisterInputPlug(evaluationUnit.Inputs[0]);
            gH_ExtendableMenu.RegisterInputPlug(evaluationUnit.Inputs[1]);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA, EvaluationUnit unit)
        {
            var run = false;

            var combos = new List<int>();
            var mass = 0.0;


            var modelName = "";
            IModel model = null;
            IModelData data = null;

            var errorMsg = new List<string>();

            DA.GetData(1, ref run);
            if (run)
            {
                if (!DA.GetData(2+1, ref modelName))
                {
                    Component_GetData.ConnectRFEM(ref model, ref data);
                }else
                {
                    Component_GetData.ConnectRFEM(modelName, ref model, ref data);
                }
                try
                {
                    //Calculate Load Combos
                    DA.GetDataList(0, combos);
                    var myCalculation = model.GetCalculation();
                    myCalculation.Clean();
                    foreach (var no in combos)
                    {
                        ErrorInfo[] errormsg = myCalculation.Calculate(LoadingType.LoadCombinationType, 1);
                    }

                    // Get EC3 LoadCase
                    var myEC3 = (Dlubal.STEEL_EC3.Module)model.GetModule("STEEL_EC3");
                    Dlubal.STEEL_EC3.ICase myCaseEC3 = myEC3.moGetCase(1, ITEM_AT.AT_NO);

                    // Get Cross Sections
                    // Select cross sections?
                    var countCS = myCaseEC3.moGetCrossSectionsCount();
                    for (int i = 0; i < countCS; i++)
                    {
                        Dlubal.STEEL_EC3.CROSS_SECTION myCSEC3 = myCaseEC3.moGetCrossSection(i + 1, ITEM_AT.AT_NO);
                        myCSEC3.Optimization = 1;
                        myCaseEC3.moSetCrossSection(myCSEC3.No, ITEM_AT.AT_NO, myCSEC3);
                    }

                    // Berechnung durchführen
                    var error = myCaseEC3.moCalculate();

                    //Querschnitt an RFEM übergeben.
                    var myCSECRFEM = new List<CrossSection>();
                    for (int i = 0; i < countCS; i++)
                    {
                        Dlubal.STEEL_EC3.CROSS_SECTION myCSEC3 = myCaseEC3.moGetCrossSection(i + 1, ITEM_AT.AT_NO);
                        var myCS = data.GetCrossSection(i + 1, ItemAt.AtNo).GetData();
                        myCS.TextID = myCSEC3.Description;
                        myCS.Description = myCSEC3.Description;
                        myCSECRFEM.Add(myCS);
                    }

                    // Set Data
                    data.PrepareModification();
                    foreach (var crosec in myCSECRFEM)
                    {
                        data.SetCrossSection(crosec);
                    }
                    data.FinishModification();

                    // Get steel mass
                    var members = data.GetMembers();
                    mass = members.Sum(item => item.Weight);

                }
                catch (Exception ex)
                {
                    Component_GetData.DisconnectRFEM(ref model, ref data);
                    throw ex;
                }
            Component_GetData.DisconnectRFEM(ref model, ref data);
            }
            // Assign Output
            DA.SetData(0, mass);


            if (errorMsg.Count != 0)
            {
                //errorMsg.Add("List item index may be one unit lower than object number");
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, String.Join(System.Environment.NewLine, errorMsg.ToArray()));
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
                return Properties.Resources.Disassemble_Surface;                
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("da5f13a0-a824-4444-abe5-efb224d7e419"); }
        }
    }
}
