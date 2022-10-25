//using System;
//using System.Collections.Generic;
//using Dlubal.RFEM5;
//using Grasshopper.Kernel;
//using Grasshopper.Kernel.Parameters;
//using Parametric_FEM_Toolbox.HelperLibraries;
//using Parametric_FEM_Toolbox.RFEM;
//using Parametric_FEM_Toolbox.UIWidgets;
//using Rhino.Geometry;

//namespace Parametric_FEM_Toolbox.GUI
//{
//    public class Component_RFDiagram_GUI : GH_SwitcherComponent
//    {
//        // Declare class variables outside the method "SolveInstance" so their values persist 
//        // when the method is called again.

//        /// <summary>
//        /// Each implementation of GH_Component must provide a public 
//        /// constructor without any arguments.
//        /// Category represents the Tab in which the component will appear, 
//        /// Subcategory the panel. If you use non-existing tab or panel names, 
//        /// new tabs/panels will automatically be created.
//        /// </summary>
//        public Component_RFDiagram_GUI()
//          : base("RF Diagram", "RFDiagram", "Creates a diagram to define a non-linear behavior.", "B+G Toolbox", "RFEM")
//        {
//        }

//        // Define Keywords to search for this Component more easily in Grasshopper
//        public override IEnumerable<string> Keywords => new string[] { "rf", "rfdiagram", "diagram", };

//        /// <summary>
//        /// Registers all the input parameters for this component.
//        /// </summary>
//        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
//        {
//            // Use the pManager object to register your input parameters.
//        }

//        /// <summary>
//        /// Registers all the output parameters for this component.
//        /// </summary>
//        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
//        {
//            // Use the pManager object to register your output parameters.
//            // Output parameters do not have default values, but they too must have the correct access type.
//            pManager.AddParameter(new Param_RFEM(), "RF Diagram", "RF Diagram", "Output RFDiagram.", GH_ParamAccess.item);

//            // Sometimes you want to hide a specific parameter from the Rhino preview.
//            // You can use the HideParameter() method as a quick way:
//            // pManager.HideParameter(0);
//        }

//        protected override void RegisterEvaluationUnits(EvaluationUnitManager mngr)
//        {
//            EvaluationUnit evaluationUnit = new EvaluationUnit("RF Diagram", "RFDiagram", "Creates a diagram to define a non-linear behavior.", Properties.Resources.Results_MemberForces);
//            mngr.RegisterUnit(evaluationUnit);
//            evaluationUnit.RegisterInputParam(new Param_Point(), "Points Positive zone", "Pts+", "Pointd that define the diagram in the positive zone", GH_ParamAccess.list);
//            evaluationUnit.RegisterInputParam(new Param_Integer(), "End Poitive Zone", "End+", "Define the end behavior in the positive zone", GH_ParamAccess.item);
//            evaluationUnit.Inputs[1].EnumInput = UtilLibrary.ListRFTypes(typeof(DiagramAfterLastStepType));
//            evaluationUnit.RegisterInputParam(new Param_Point(), "Points Negative zone", "Pts-", "Pointd that define the diagram in the negative zone", GH_ParamAccess.list);
//            evaluationUnit.Inputs[2].Parameter.Optional = true;
//            evaluationUnit.RegisterInputParam(new Param_Integer(), "End Negative Zone", "End-", "Define the end behavior in the negative zone", GH_ParamAccess.item);
//            evaluationUnit.Inputs[3].EnumInput = UtilLibrary.ListRFTypes(typeof(DiagramAfterLastStepType));
//            evaluationUnit.Inputs[3].Parameter.Optional = true;
//            evaluationUnit.RegisterInputParam(new Param_Boolean(), "Symmetric", "Sym", "If true, define only the positive zone", GH_ParamAccess.item);
//            evaluationUnit.Inputs[4].Parameter.Optional = true;
//            evaluationUnit.RegisterInputParam(new Param_String(), "Comment", "Comment", "Comment.", GH_ParamAccess.item);
//            evaluationUnit.Inputs[5].Parameter.Optional = true;
//        }

//        /// <summary>
//        /// This is the method that actually does the work.
//        /// </summary>
//        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
//        /// to store data in output parameters.</param>
//        protected override void SolveInstance(IGH_DataAccess DA, EvaluationUnit unit)
//        {
//            var rfDiagram = new RFDiagram();
//            var pospts = new List<Point3d>();
//            var laststeppos = 0;
//            var negpts = new List<Point3d>();
//            var laststepneg = 0;
//            var sym = false;
//            var comment = "";

//            var rfDiag = new RFDiagram();
//            if (DA.GetDataList(0, pospts))
//            {
//                rfDiag.PositiveZone = pospts;
//            }
//            if (DA.GetData(1, ref laststeppos))
//            {
//                rfDiag.PositiveZoneType = (DiagramAfterLastStepType)laststeppos;
//            }
//            if (DA.GetDataList(2, negpts))
//            {
//                rfDiag.NegativeZone = negpts;
//            }
//            if (DA.GetData(3, ref laststepneg))
//            {
//                rfDiag.NegativeZoneType = (DiagramAfterLastStepType)laststepneg;
//            }
//            if (DA.GetData(4, ref sym))
//            {
//                rfDiag.Symmetric = sym;
//            }
//            if (DA.GetData(5, ref comment))
//            {
//                rfDiag.Comment = comment;
//            }

//            DA.SetData(0, rfDiag);
//        }

//        /// <summary>
//        /// The Exposure property controls where in the panel a component icon 
//        /// will appear. There are seven possible locations (primary to septenary), 
//        /// each of which can be combined with the GH_Exposure.obscure flag, which 
//        /// ensures the component will only be visible on panel dropdowns.
//        /// </summary>
//        public override GH_Exposure Exposure
//        {
//            get { return GH_Exposure.secondary; }
//        }

//        /// <summary>
//        /// Provides an Icon for every component that will be visible in the User Interface.
//        /// Icons need to be 24x24 pixels.
//        /// </summary>
//        protected override System.Drawing.Bitmap Icon
//        {
//            get
//            {
//                // You can add image files to your project resources and access them like this:
//                //return Resources.IconForThisComponent;
//                return Properties.Resources.Results_MemberForces;
//            }
//        }

//        /// <summary>
//        /// Each component must have a unique Guid to identify it. 
//        /// It is vital this Guid doesn't change otherwise old ghx files 
//        /// that use the old ID will partially fail during loading.
//        /// </summary>
//        public override Guid ComponentGuid
//        {
//            get { return new Guid("d8fc6989-1b37-4c68-b764-4224865164a9"); }
//        }
//    }
//}
