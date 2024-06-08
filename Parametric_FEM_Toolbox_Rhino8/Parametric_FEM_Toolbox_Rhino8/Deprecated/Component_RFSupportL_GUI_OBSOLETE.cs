﻿using System;
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
    public class Component_RFSupportL_GUI_OBSOLETE : GH_SwitcherComponent
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
        public Component_RFSupportL_GUI_OBSOLETE()
          : base("RF Line Support", "RFLineSup", "Creates a RFSupportL object to define new data or modify existing data " +
                "in the RFEM model.", "B+G Toolbox", "RFEM")
        {
        }

        // Define Keywords to search for this Component more easily in Grasshopper
        public override IEnumerable<string> Keywords => new string[] {"rf", "rfsupportl", "support", "line"};

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Use the pManager object to register your input parameters.
            pManager.AddCurveParameter("Line", "Line", "Line or Curve to attach RFSupportL to.", GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager.AddIntegerParameter("Support Number", "No", "Optional index number to assign to the RFEM object.", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddNumberParameter("Displacement Dir X", "Tx", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]", GH_ParamAccess.item);
            pManager[2].Optional = true;
            pManager.AddNumberParameter("Displacement Dir Y", "Ty", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]", GH_ParamAccess.item);
            pManager[3].Optional = true;
            pManager.AddNumberParameter("Displacement Dir Z", "Tz", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]", GH_ParamAccess.item);
            pManager[4].Optional = true;
            pManager.AddNumberParameter("Rotation Dir X", "Rx", "(-1): Fixed; (0): Free; Other: Stiffness in [kNm/rad]", GH_ParamAccess.item);
            pManager[5].Optional = true;
            pManager.AddNumberParameter("Rotation Dir Y", "Ry", "(-1): Fixed; (0): Free; Other: Stiffness in [kNm/rad]", GH_ParamAccess.item);
            pManager[6].Optional = true;
            pManager.AddNumberParameter("Rotation Dir Z", "Rz", "(-1): Fixed; (0): Free; Other: Stiffness in [kNm/rad]", GH_ParamAccess.item);
            pManager[7].Optional = true;
            pManager.AddTextParameter("Comment", "Comment", "Comment.", GH_ParamAccess.item);
            pManager[8].Optional = true;

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
            pManager.RegisterParam(new Param_RFEM(), "Line Support", "LineSup", "Output RFSupportL.", GH_ParamAccess.item);

            // Sometimes you want to hide a specific parameter from the Rhino preview.
            // You can use the HideParameter() method as a quick way:
            // pManager.HideParameter(0);
        }

        protected override void RegisterEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit("Assemble Line Support", "LineSup", "Creates a RFSupportL object to define new data or modify existing data " +
                "in the RFEM model.");
            mngr.RegisterUnit(evaluationUnit);

            evaluationUnit.RegisterInputParam(new Param_String(), "Line List", "LineList", "Line List", GH_ParamAccess.item);
            evaluationUnit.Inputs[0].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_String(), "Reference System Type", "RefSys", UtilLibrary.DescriptionRFTypes(typeof(ReferenceSystemType)), GH_ParamAccess.item);
            evaluationUnit.Inputs[1].Parameter.Optional = true;

            evaluationUnit.RegisterInputParam(new Param_RFEM(), "Line Support", "LineSup", "Support object from the RFEM model to modify", GH_ParamAccess.item);
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
            var rfSup = new RFSupportL();
            var inRFEM = new GH_RFEM();
            var mod = false;
            var del = false;
            var tx = 0.0;
            var ty = 0.0;
            var tz = 0.0;
            var rx = 0.0;
            var ry = 0.0;
            var rz = 0.0;
            var lineList = "";
            var refSys = "";
            //int newNo = 0;


            if (DA.GetData(11, ref inRFEM))
            {
                rfSup = new RFSupportL((RFSupportL)inRFEM.Value);
            }else if (DA.GetData(0, ref inCurve))
            {
                var myRFLine = new RFLine();                
                Component_RFLine.SetGeometry(inCurve, ref myRFLine);
                var myRFLines = new List<RFLine>() { myRFLine };
                rfSup = new RFSupportL(new LineSupport(), myRFLines);
            }
            else
            {
                return;
            }
            if (DA.GetData(12, ref mod))
            {
                rfSup.ToModify = mod;
            }
            if (DA.GetData(13, ref del))
            {
                rfSup.ToDelete = del;
            }
            if (DA.GetData(1, ref noIndex))
            {
                rfSup.No = noIndex;
            }
            if (DA.GetData(8, ref comment))
            {
                rfSup.Comment = comment;
            }
            if (DA.GetData(2, ref tx))
            {
                rfSup.Tx = tx;
            }
            if (DA.GetData(3, ref ty))
            {
                rfSup.Ty = ty;
            }
            if (DA.GetData(4, ref tz))
            {
                rfSup.Tz = tz;
            }
            if (DA.GetData(5, ref rx))
            {
                rfSup.Rx = rx;
            }
            if (DA.GetData(6, ref ry))
            {
                rfSup.Ry = ry;
            }
            if (DA.GetData(7, ref rz))
            {
                rfSup.Rz = rz;
            }
            if (DA.GetData(9, ref lineList))
            {
                rfSup.LineList = lineList;
            }
            if (DA.GetData(10, ref refSys))
            {
                Enum.TryParse(refSys, out ReferenceSystemType myrefSys);
                rfSup.RSType = myrefSys;
            }
            
            DA.SetData(0, rfSup);
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
                return Properties.Resources.Assemble_SupportL;                
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("d3db5e89-abdd-497b-8c11-bdc651a7dc77"); }
        }
    }
}
