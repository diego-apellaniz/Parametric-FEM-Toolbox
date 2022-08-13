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
            pManager.AddBooleanParameter("Run component?", "Run", "If true, the programm is executed.", GH_ParamAccess.item, false);
            pManager.AddIntegerParameter("Load Combinations", "LoadCombo", "Load Combinations to run in the EC3 Module.", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Optimize?", "Opt", "If true, the cross sections are optimized.", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            //Use the pManager object to register your output parameters.
            //Output parameters do not have default values, but they too must have the correct access type.
            pManager.AddIntegerParameter("Member No", "No", "Member number.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Location", "X", "Location of result along member.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Utilization", "Util", "Utilization ratio.", GH_ParamAccess.list);
            pManager.AddTextParameter("Comment", "Check", "Comment about the design check.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Steel Mass", "Mass", "Steel Mass [kg] of the optimized structure.", GH_ParamAccess.item);

            // Sometimes you want to hide a specific parameter from the Rhino preview.
            // You can use the HideParameter() method as a quick way:
            // pManager.HideParameter(0);
        }

        protected override void RegisterEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit("Optimize Cross Sections", "Optimize CroSec", "Performs a Cross Section Optimization with the EC3 Module.", Properties.Resources.icon_OptimizeCS);
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
            var opt = false;

            var combos = new List<int>();
            var mass = 0.0;
            var outMembers = new List<int>();
            var outUtils = new List<double>();
            var outLocation = new List<double>();
            var outCheck = new List<string>();

            var modelName = "";
            IModel model = null;
            IModelData data = null;

            var errorMsg = new List<string>();

            DA.GetData(0, ref run);
            DA.GetData(2, ref opt);
            if (run)
            {
                if (!DA.GetData(3+1, ref modelName))
                {
                    Component_GetData.ConnectRFEM(ref model, ref data);
                }else
                {
                    Component_GetData.ConnectRFEM(modelName, ref model, ref data);
                }
                try
                {
                    //Calculate Load Combos
                    DA.GetDataList(1, combos);
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
                        if(opt)
                        {
                            myCSEC3.Optimization = 1;
                        }else
                        {
                            myCSEC3.Optimization = 0;
                        }
                        
                        myCaseEC3.moSetCrossSection(myCSEC3.No, ITEM_AT.AT_NO, myCSEC3);
                    }

                    // Berechnung durchführen
                    var error = myCaseEC3.moCalculate();

                    //Querschnitt an RFEM übergeben.
                    if (opt)
                    {
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
                    }                    

                    // Get utilization of members
                    var results = myCaseEC3.moGetResults().moGetDesignByMemberAll();
                    foreach (var r in results)
                    {
                        outMembers.Add(r.MemberNo);
                        outLocation.Add(r.X);
                        outUtils.Add(r.DesignRatio);
                        outCheck.Add(r.comment);
                    }                    

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
            DA.SetDataList(0,outMembers);
            DA.SetDataList(1, outLocation);
            DA.SetDataList(2,outUtils);
            DA.SetDataList(3, outCheck);
            DA.SetData(4, mass);


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
                return Properties.Resources.icon_OptimizeCS;                
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("7cb59f27-f52e-4a8c-97e5-b345b4d8aa37"); }
        }
    }
}
