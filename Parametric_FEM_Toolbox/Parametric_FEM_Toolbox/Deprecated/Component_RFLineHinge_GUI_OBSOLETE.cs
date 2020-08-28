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
using Parametric_FEM_Toolbox.GUI;
using Dlubal.RFEM5;
using System.Runtime.InteropServices;

namespace Parametric_FEM_Toolbox.Deprecated
{
    public class Component_RFLineHinge_GUI_OBSOLETE : GH_SwitcherComponent
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
        public Component_RFLineHinge_GUI_OBSOLETE()
          : base("RF Line Hinge", "RFLineHinge", "Creates a RFLineHinge object to define new data or modify existing data " +
                "in the RFEM model.", "B+G Toolbox", "RFEM")
        {
        }

        // Define Keywords to search for this Component more easily in Grasshopper
        public override IEnumerable<string> Keywords => new string[] {"rf", "rflinehinge", "hinge", "line"};

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Use the pManager object to register your input parameters.
            pManager.AddCurveParameter("Line", "Line", "Line or Curve to attach RFSupportL to.", GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager.AddIntegerParameter("Hinge Number", "No", "Optional index number to assign to the RFEM object.", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddIntegerParameter("Surface Number", "SfcNo", "Index number of the Surface to apply the hinge to.", GH_ParamAccess.item);
            pManager[2].Optional = true;
            pManager.AddNumberParameter("Displacement Dir X", "Tx", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]", GH_ParamAccess.item);
            pManager[3].Optional = true;
            pManager.AddNumberParameter("Displacement Dir Y", "Ty", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]", GH_ParamAccess.item);
            pManager[4].Optional = true;
            pManager.AddNumberParameter("Displacement Dir Z", "Tz", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]", GH_ParamAccess.item);
            pManager[5].Optional = true;
            pManager.AddNumberParameter("Rotation Dir X", "Rx", "(-1): Fixed; (0): Free; Other: Stiffness in [kNm/rad]", GH_ParamAccess.item);
            pManager[6].Optional = true;
            pManager.AddNumberParameter("Rotation Dir Y", "Ry", "(-1): Fixed; (0): Free; Other: Stiffness in [kNm/rad]", GH_ParamAccess.item);
            pManager[7].Optional = true;
            pManager.AddNumberParameter("Rotation Dir Z", "Rz", "(-1): Fixed; (0): Free; Other: Stiffness in [kNm/rad]", GH_ParamAccess.item);
            pManager[8].Optional = true;
            pManager.AddTextParameter("Comment", "Comment", "Comment.", GH_ParamAccess.item);
            pManager[9].Optional = true;

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
            pManager.RegisterParam(new Param_RFEM(), "Linear Hinge", "LinearHinge", "Output RFLineHinge.", GH_ParamAccess.item);

            // Sometimes you want to hide a specific parameter from the Rhino preview.
            // You can use the HideParameter() method as a quick way:
            // pManager.HideParameter(0);
        }

        protected override void RegisterEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit("Assemble Line Hinge", "LineHinge", "Creates a RFLineHinge object to define new data or modify existing data " +
                "in the RFEM model.");
            mngr.RegisterUnit(evaluationUnit);

            evaluationUnit.RegisterInputParam(new Param_Integer(), "LineNo", "LineNo", "Line Index related to the Line Hinge", GH_ParamAccess.item);
            evaluationUnit.Inputs[0].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_String(), "Side", "Side", UtilLibrary.DescriptionRFTypes(typeof(HingeSideType)), GH_ParamAccess.item);
            evaluationUnit.Inputs[1].Parameter.Optional = true;

            evaluationUnit.RegisterInputParam(new Param_RFEM(), "Line Hinge", "LineHinge", "Line Hinge object from the RFEM model to modify", GH_ParamAccess.item);
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
            for (int i = 2; i <2+3; i++)
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
            var rfHinge = new RFLineHinge();
            var inRFEM = new GH_RFEM();
            var mod = false;
            var del = false;
            var tx = 0.0;
            var ty = 0.0;
            var tz = 0.0;
            var rx = 0.0;
            var ry = 0.0;
            var rz = 0.0;
            var lineNo = 0;
            var sfcNo = 0;
            var side = "";
            //int newNo = 0;


            if (DA.GetData(12, ref inRFEM))
            {
                rfHinge = new RFLineHinge((RFLineHinge)inRFEM.Value);
            }else if (DA.GetData(0, ref inCurve))
            {
                var myRFLine = new RFLine();                
                Component_RFLine.SetGeometry(inCurve, ref myRFLine);
                rfHinge = new RFLineHinge(new RFLineHinge(), myRFLine);

            }else if (DA.GetData(10, ref lineNo))
            {
                rfHinge.LineNo = lineNo;
            }
            else
            {
                return;
            }
            if (DA.GetData(13, ref mod))
            {
                rfHinge.ToModify = mod;
            }
            if (DA.GetData(14, ref del))
            {
                rfHinge.ToDelete = del;
            }
            if (DA.GetData(1, ref noIndex))
            {
                rfHinge.No = noIndex;
            }
            if (DA.GetData(2, ref sfcNo))
            {
                rfHinge.SfcNo = sfcNo;
            }
            if (DA.GetData(9, ref comment))
            {
                rfHinge.Comment = comment;
            }
            if (DA.GetData(3, ref tx))
            {
                rfHinge.Tx = tx;
            }
            if (DA.GetData(4, ref ty))
            {
                rfHinge.Ty = ty;
            }
            if (DA.GetData(5, ref tz))
            {
                rfHinge.Tz = tz;
            }
            if (DA.GetData(6, ref rx))
            {
                rfHinge.Rx = rx;
            }
            if (DA.GetData(7, ref ry))
            {
                rfHinge.Ry = ry;
            }
            if (DA.GetData(8, ref rz))
            {
                rfHinge.Rz = rz;
            }

            if (DA.GetData(11, ref side))
            {
                Enum.TryParse(side, out HingeSideType mySide);
                rfHinge.Side = mySide;
            }
            
            DA.SetData(0, rfHinge);
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
                return Properties.Resources.Assemble_LineHinge;                
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("776c0298-d225-4f25-b1cf-abb9b3558c6a"); }
        }
    }
}
