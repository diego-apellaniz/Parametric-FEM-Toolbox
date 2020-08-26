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
using RFEM_daq.GUI;
using Dlubal.RFEM5;
using System.Runtime.InteropServices;

namespace RFEM_daq.Deprecated
{
    public class Component_RFLineObject_GUI_OBSOLETE : GH_SwitcherComponent
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
        public Component_RFLineObject_GUI_OBSOLETE()
          : base("RF Line Object", "RFLine", "Creates a RFLine object to define new data or modify existing data " +
                "in the RFEM model.", "B+G Toolbox", "RFEM")
        {
        }

        // Define Keywords to search for this Component more easily in Grasshopper
        public override IEnumerable<string> Keywords => new string[] {"rf", "rfline", "line"};

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Use the pManager object to register your input parameters.
            pManager.AddCurveParameter("Line", "Line", "Line or Curve to assemble the RFLine from.", GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager.AddIntegerParameter("Line Number", "No", "Optional index number to assign to the RFEM object.", GH_ParamAccess.item);
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
            pManager.RegisterParam(new Param_RFEM(), "LineObject", "Line Object", "Output RFLine.", GH_ParamAccess.item);

            // Sometimes you want to hide a specific parameter from the Rhino preview.
            // You can use the HideParameter() method as a quick way:
            // pManager.HideParameter(0);
        }

        protected override void RegisterEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit("Assemble Line", "Line", "Creates a RFLine object to define new data or modify existing data " +
                "in the RFEM model.");
            mngr.RegisterUnit(evaluationUnit);

            evaluationUnit.RegisterInputParam(new Param_String(), "NodeList", "NodeList", "Node List", GH_ParamAccess.item);
            evaluationUnit.Inputs[0].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_String(), "Line Type", "Type", UtilLibrary.DescriptionRFTypes(typeof(LineType)), GH_ParamAccess.item);
            evaluationUnit.Inputs[1].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Number(), "Rotation Angle [°]", "β", "Rotation Angle [°]", GH_ParamAccess.item);
            evaluationUnit.Inputs[2].Parameter.Optional = true;
            //evaluationUnit.RegisterInputParam(new Param_Integer(), "Interpolated Points", "n", "Number of interpolated points for NURBS", GH_ParamAccess.item);
            //evaluationUnit.Inputs[3].Parameter.Optional = true;

            evaluationUnit.RegisterInputParam(new Param_RFEM(), "Line", "Line", "Line object from the RFEM model to modify", GH_ParamAccess.item);
            evaluationUnit.Inputs[3].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Boolean(), "Modify", "Modify", "Modify node?", GH_ParamAccess.item);
            evaluationUnit.Inputs[4].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Boolean(), "Delete", "Delete", "Delete node?", GH_ParamAccess.item);
            evaluationUnit.Inputs[5].Parameter.Optional = true;

            GH_ExtendableMenu gH_ExtendableMenu = new GH_ExtendableMenu(0, "advanced");
            gH_ExtendableMenu.Name = "Advanced";
            gH_ExtendableMenu.Collapse();
            evaluationUnit.AddMenu(gH_ExtendableMenu);
            for (int i = 0; i < 3; i++)
            {
                gH_ExtendableMenu.RegisterInputPlug(evaluationUnit.Inputs[i]);
            }

            GH_ExtendableMenu gH_ExtendableMenu2 = new GH_ExtendableMenu(1, "modify");
            gH_ExtendableMenu2.Name = "Modify";
            gH_ExtendableMenu2.Collapse();
            evaluationUnit.AddMenu(gH_ExtendableMenu2);
            for (int i = 3; i < 3+3; i++)
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
            Curve inCurve = null;
            var noIndex = 0;
            var comment = "";
            var rFLine = new RFLine();
            var inRFEM = new GH_RFEM();
            var mod = false;
            var del = false;
            var nodeList = "";
            var lineType = "";
            var rotAngle = 0.0;
            //int intPoints = 4;
            //int newNo = 0;

            if (DA.GetData(6, ref inRFEM))
            {
                rFLine = new RFLine((RFLine)inRFEM.Value);
            }else if (DA.GetData(0, ref inCurve))
            {
                Component_RFLine.SetGeometry(inCurve, ref rFLine);
            }else
            {
                return;
            }
            if (DA.GetData(7, ref mod))
            {
                rFLine.ToModify = mod;
            }
            if (DA.GetData(8, ref del))
            {
                rFLine.ToDelete = del;
            }
            if (DA.GetData(1, ref noIndex))
            {
                rFLine.No = noIndex;
            }
            if (DA.GetData(2, ref comment))
            {
                rFLine.Comment = comment;
            }
            if (DA.GetData(3, ref nodeList))
            {
                rFLine.NodeList = nodeList;
            }
            if (DA.GetData(4, ref lineType))
            {
                Enum.TryParse(lineType, out LineType myLineType);
                rFLine.Type = myLineType;
            }
            if (DA.GetData(5, ref rotAngle))
            {
                rFLine.RotationType = RotationType.Angle;
                rFLine.RotationAngle = rotAngle;
            }

            DA.SetData(0, rFLine);
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
                return RFEM_daq.Properties.Resources.Assemble_Line;                
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("a8f92be6-c911-4003-8a5e-55387c102d50"); }
        }
    }
}
