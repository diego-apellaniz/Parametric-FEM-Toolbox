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
    public class Component_RFCroSec_GUI_OBSOLETE_2 : GH_SwitcherComponent
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
        public Component_RFCroSec_GUI_OBSOLETE_2()
          : base("RF Cross Section", "RFCroSec", "Creates a RFCroSec object to define new data or modify existing data " +
                "in the RFEM model.", "B+G Toolbox", "RFEM")
        {
        }

        // Define Keywords to search for this Component more easily in Grasshopper
        public override IEnumerable<string> Keywords => new string[] {"rf", "rfcrosec", "cross", "section"};

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Use the pManager object to register your input parameters.
            pManager.AddTextParameter("Description", "Desc", "Name or Description of Cross Section.", GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager.AddIntegerParameter("Material Number", "MatNo", "Number of Material assigned to the Cross-Section", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddIntegerParameter("Cross Section Number", "No", "Optional index number to assign to the RFEM object.", GH_ParamAccess.item);
            pManager[2].Optional = true;
            pManager.AddTextParameter("Comment", "Comment", "Comment.", GH_ParamAccess.item);
            pManager[3].Optional = true;

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
            pManager.RegisterParam(new Param_RFEM(), "Cross Section", "CroSec", "Output RFCroSec.", GH_ParamAccess.item);

            // Sometimes you want to hide a specific parameter from the Rhino preview.
            // You can use the HideParameter() method as a quick way:
            // pManager.HideParameter(0);
        }

        protected override void RegisterEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit("Assemble Cross Section", "CroSec", "Creates a RFCroSec object to define new data or modify existing data " +
                "in the RFEM model.");
            mngr.RegisterUnit(evaluationUnit);


            evaluationUnit.RegisterInputParam(new Param_Number(), "AxialArea [m²]", "A", "AxialArea [m²]", GH_ParamAccess.item);
            evaluationUnit.Inputs[0].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Number(), "ShearAreaY [m²]", "Ay", "ShearAreaY [m²]", GH_ParamAccess.item);
            evaluationUnit.Inputs[1].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Number(), "ShearAreaZ [m²]", "Az", "ShearAreaZ [m²]", GH_ParamAccess.item);
            evaluationUnit.Inputs[2].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Number(), "BendingMomentY [m⁴]", "Iy", "BendingMomentY [m⁴]", GH_ParamAccess.item);
            evaluationUnit.Inputs[3].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Number(), "BendingMomentZ [m⁴]", "Iz", "BendingMomentZ [m⁴]", GH_ParamAccess.item);
            evaluationUnit.Inputs[4].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Number(), "TorsionMoment [m⁴]", "Jt", "TorsionMoment [m⁴]", GH_ParamAccess.item);
            evaluationUnit.Inputs[5].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Number(), "Rotation Angle [°]", "β", "Rotation Angle [°]", GH_ParamAccess.item);
            evaluationUnit.Inputs[6].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Number(), "TemperatureLoadWidth [m]", "TempW", "TemperatureLoadWidth [m]", GH_ParamAccess.item);
            evaluationUnit.Inputs[7].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Number(), "TemperatureLoadDepth [m]", "TempD", "TemperatureLoadDepth [m]", GH_ParamAccess.item);
            evaluationUnit.Inputs[8].Parameter.Optional = true;

            evaluationUnit.RegisterInputParam(new Param_RFEM(), "Cross Section", "CroSec", "Cross Section object from the RFEM model to modify", GH_ParamAccess.item);
            evaluationUnit.Inputs[9].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Boolean(), "Modify", "Modify", "Modify node?", GH_ParamAccess.item);
            evaluationUnit.Inputs[10].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Boolean(), "Delete", "Delete", "Delete node?", GH_ParamAccess.item);
            evaluationUnit.Inputs[11].Parameter.Optional = true;

            GH_ExtendableMenu gH_ExtendableMenu = new GH_ExtendableMenu(0, "advanced");
            gH_ExtendableMenu.Name = "Advanced";
            gH_ExtendableMenu.Collapse();
            evaluationUnit.AddMenu(gH_ExtendableMenu);
            for (int i = 0; i < 9; i++)
            {
                gH_ExtendableMenu.RegisterInputPlug(evaluationUnit.Inputs[i]);
            }

            GH_ExtendableMenu gH_ExtendableMenu2 = new GH_ExtendableMenu(1, "modify");
            gH_ExtendableMenu2.Name = "Modify";
            gH_ExtendableMenu2.Collapse();
            evaluationUnit.AddMenu(gH_ExtendableMenu2);
            for (int i = 9; i < 9+3; i++)
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
            var noIndex = 0;
            var comment = "";
            var description = "";
            var rFCroSec = new RFCroSec();
            var inRFEM = new GH_RFEM();
            var mod = false;
            var del = false;
            var rotAngle = 0.0;
            var matNo = 0;
            var A = 0.0;
            var Ay = 0.0;
            var Az = 0.0;
            var Iy = 0.0;
            var Iz = 0.0;
            var Jt = 0.0;
            var TempW = 0.0;
            var TempD = 0.0;

            if (DA.GetData(13, ref inRFEM))
            {
                rFCroSec = new RFCroSec((RFCroSec)inRFEM.Value);
                if (DA.GetData(0, ref description))
                {
                    rFCroSec.Description = description;
                }
                if (DA.GetData(1, ref matNo))
                {
                    rFCroSec.MatNo = matNo;
                }
            }
            else if (DA.GetData(0, ref description) && DA.GetData(1, ref matNo))
            {
                rFCroSec.Description = description;
                rFCroSec.MatNo = matNo;
            }
            else
            {
                return;
            }
            if (DA.GetData(14, ref mod))
            {
                rFCroSec.ToModify = mod;
            }
            if (DA.GetData(15, ref del))
            {
                rFCroSec.ToDelete = del;
            }
            if (DA.GetData(2, ref noIndex))
            {
                rFCroSec.No = noIndex;
            }
            if (DA.GetData(3, ref comment))
            {
                rFCroSec.Comment = comment;
            }
            if (DA.GetData(4, ref A))
            {
                rFCroSec.A = A;
            }
            if (DA.GetData(5, ref Ay))
            {
                rFCroSec.Ay = Ay;
            }
            if (DA.GetData(6, ref Az))
            {
                rFCroSec.Az = Az;
            }
            if (DA.GetData(7, ref Iy))
            {
                rFCroSec.Iy = Iy;
            }
            if (DA.GetData(8, ref Iz))
            {
                rFCroSec.Iz = Iz;
            }
            if (DA.GetData(9, ref Jt))
            {
                rFCroSec.Jt = Jt;
            }
            if (DA.GetData(10, ref rotAngle))
            {
                rFCroSec.RotationAngle = rotAngle;
            }
            if (DA.GetData(11, ref TempW))
            {
                rFCroSec.TempWidth = TempW;
            }
            if (DA.GetData(12, ref TempD))
            {
                rFCroSec.TempDepth = TempD;
            }
            DA.SetData(0, rFCroSec);
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
                return Properties.Resources.Assemble_CroSec;                
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("861beba2-129a-44be-8d5a-2af2ebf5b2be"); }
        }
    }
}
