using System;
using System.Collections.Generic;
using Dlubal.RFEM5;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Parametric_FEM_Toolbox.HelperLibraries;
using Parametric_FEM_Toolbox.RFEM;
using Parametric_FEM_Toolbox.UIWidgets;
using Rhino.Geometry;

namespace Parametric_FEM_Toolbox.GUI
{
    public class Component_RFPartialActivity_GUI : GH_SwitcherComponent
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
        public Component_RFPartialActivity_GUI()
          : base("RF Partial Activity", "RFActivity", "Creates a diagram to define partial activity behavior.", "B+G Toolbox", "RFEM")
        {
        }

        // Define Keywords to search for this Component more easily in Grasshopper
        public override IEnumerable<string> Keywords => new string[] { "rf", "rfactivity", "partial", "activity" };

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Use the pManager object to register your input parameters.

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            // Use the pManager object to register your output parameters.
            // Output parameters do not have default values, but they too must have the correct access type.
            pManager.AddParameter(new Param_RFEM(), "RF Partial Activity", "RF Activity", "Output RFPartialActivity.", GH_ParamAccess.item);

            // Sometimes you want to hide a specific parameter from the Rhino preview.
            // You can use the HideParameter() method as a quick way:
            // pManager.HideParameter(0);
        }

        protected override void RegisterEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit("RF Partial Activity", "RFActivity", "Creates a diagram to define partial activity behavior.", Properties.Resources.Partial_Activity);
            mngr.RegisterUnit(evaluationUnit);
            evaluationUnit.RegisterInputParam(new Param_Integer(), "Positive zone", "Zone+", "Type of partial activity in positive zone", GH_ParamAccess.item);
            evaluationUnit.Inputs[0].EnumInput = UtilLibrary.ListRFTypes(typeof(PartialActivityType));
            evaluationUnit.RegisterInputParam(new Param_Number(), "Positive limit", "Limit+", "Limit in positive slippage", GH_ParamAccess.item);
            evaluationUnit.Inputs[1].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Number(), "Positive slippage", "Slippage+", "Slippage in positive zone", GH_ParamAccess.item);
            evaluationUnit.Inputs[2].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Integer(), "Negative zone", "Zone-", "Type of partial activity in negative zone", GH_ParamAccess.item);
            evaluationUnit.Inputs[3].EnumInput = UtilLibrary.ListRFTypes(typeof(PartialActivityType));
            evaluationUnit.RegisterInputParam(new Param_Number(), "Negative limit", "Limit-", "Limit in negative slippage", GH_ParamAccess.item);
            evaluationUnit.Inputs[4].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Number(), "Negative slippage", "Slippage-", "Slippage in negative zone", GH_ParamAccess.item);
            evaluationUnit.Inputs[5].Parameter.Optional = true;
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA, EvaluationUnit unit)
        {
            var rfAct = new RFPartialActivity();
            var pos_zone = 0;
            var pos_limit = 0.0;
            var pos_slippage = 0.0;
            var neg_zone = 0;
            var neg_limit = 0.0;
            var neg_slippage = 0.0;

            if (DA.GetData(0, ref pos_zone))
            {
                rfAct.PositiveZone = (PartialActivityType)pos_zone;
                if (rfAct.PositiveZone == PartialActivityType.UnknownPartialActivityType)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, String.Join(System.Environment.NewLine, "Nonlinearity Type not supported. "));
                    return;                   
                }
            }else
            {
                rfAct.PositiveZone = PartialActivityType.CompleteActivityType;
            }
            if (DA.GetData(1, ref pos_limit))
            {
                rfAct.PositiveLimit = pos_limit;
                if (rfAct.PositiveZone == PartialActivityType.YieldingActivityType || rfAct.PositiveZone == PartialActivityType.TearingActivityType)
                {
                    rfAct.PositiveLimit *= 1000;
                }
            }
            if (DA.GetData(2, ref pos_slippage))
            {
                rfAct.PositiveSlippage = pos_slippage;
            }
            if (DA.GetData(3, ref neg_zone))
            {
                rfAct.NegativeZone = (PartialActivityType)neg_zone;
                if (rfAct.NegativeZone == PartialActivityType.UnknownPartialActivityType)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, String.Join(System.Environment.NewLine, "Nonlinearity Type not supported. "));
                    return;
                }
            }
            else
            {
                rfAct.NegativeZone = PartialActivityType.CompleteActivityType;
            }
            if (DA.GetData(4, ref neg_limit))
            {
                rfAct.NegativeLimit = neg_limit;
                if (rfAct.NegativeZone == PartialActivityType.YieldingActivityType || rfAct.NegativeZone == PartialActivityType.TearingActivityType)
                {
                    rfAct.NegativeLimit *= 1000;
                }
            }
            if (DA.GetData(5, ref neg_slippage))
            {
                rfAct.NegativeSlippage = neg_slippage;                
            }
            DA.SetData(0, rfAct);
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
                return Properties.Resources.Partial_Activity;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("c4d89e24-1b26-49a5-a794-eccc5f47d0a0"); }
        }
    }
}
