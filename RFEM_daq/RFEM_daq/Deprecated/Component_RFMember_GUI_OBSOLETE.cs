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
    public class Component_RFMember_GUI_OBSOLETE : GH_SwitcherComponent
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
        public Component_RFMember_GUI_OBSOLETE()
          : base("RF Member", "RFMember", "Creates a RFMember object to define new data or modify existing data " +
                "in the RFEM model.", "B+G Toolbox", "RFEM")
        {
        }

        // Define Keywords to search for this Component more easily in Grasshopper
        public override IEnumerable<string> Keywords => new string[] {"rf", "rfmember", "member"};

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Use the pManager object to register your input parameters.
            pManager.AddCurveParameter("Line", "Line", "Line or Curve to assemble the RFMember from.", GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager.AddIntegerParameter("Start Cross-Section", "S CroSec", "Number of Start Cross-Section", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddIntegerParameter("Member Number", "No", "Optional index number to assign to the RFEM object.", GH_ParamAccess.item);
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
            pManager.RegisterParam(new Param_RFEM(), "Member", "Member", "Output RFMember.", GH_ParamAccess.item);

            // Sometimes you want to hide a specific parameter from the Rhino preview.
            // You can use the HideParameter() method as a quick way:
            // pManager.HideParameter(0);
        }

        protected override void RegisterEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit("Assemble Member", "Member", "Creates a RFMember object to define new data or modify existing data " +
                "in the RFEM model.");
            mngr.RegisterUnit(evaluationUnit);


            evaluationUnit.RegisterInputParam(new Param_Integer(), "LineNo", "LineNo", "Line Number", GH_ParamAccess.item);
            evaluationUnit.Inputs[0].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_String(), "Member Type", "Type", UtilLibrary.DescriptionRFTypes(typeof(MemberType)), GH_ParamAccess.item);
            evaluationUnit.Inputs[1].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Number(), "Rotation Angle [°]", "β", "Rotation Angle [°]", GH_ParamAccess.item);
            evaluationUnit.Inputs[2].Parameter.Optional = true;
            //evaluationUnit.RegisterInputParam(new Param_Integer(), "Interpolated Points", "n", "Number of interpolated points for NURBS", GH_ParamAccess.item);
            //evaluationUnit.Inputs[3].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Integer(), "End Cross-Section", "E CroSec", "Number of End Cross-Section", GH_ParamAccess.item);
            evaluationUnit.Inputs[3].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Integer(), "Start Hinge", "S Hinge", "Number of Start Hinge", GH_ParamAccess.item);
            evaluationUnit.Inputs[4].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Integer(), "End Hinge", "E Hinge", "Number of End Hinge", GH_ParamAccess.item);
            evaluationUnit.Inputs[5].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Integer(), "Eccentricity", "Ecc", "Number of Eccentricity", GH_ParamAccess.item);
            evaluationUnit.Inputs[6].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Integer(), "Division", "Div", "Number of Division", GH_ParamAccess.item);
            evaluationUnit.Inputs[7].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_String(), "Taper Shape", "Taper", UtilLibrary.DescriptionRFTypes(typeof(TaperShapeType)), GH_ParamAccess.item);
            evaluationUnit.Inputs[8].Parameter.Optional = true;

            evaluationUnit.RegisterInputParam(new Param_RFEM(), "Member", "Member", "Member object from the RFEM model to modify", GH_ParamAccess.item);
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
            Curve inCurve = null;
            var noIndex = 0;
            var comment = "";
            var rFMember = new RFMember();
            var inRFEM = new GH_RFEM();
            var mod = false;
            var del = false;
            var lineNo = 0;
            var memberType = "";
            var taperType = "";
            var rotAngle = 0.0;
            //var intPoints = 4;
            var sCS = 0;
            var eCS = 0;
            var sH = 0;
            var eH = 0;
            var ecc = 0;
            var div = 0;
            //int newNo = 0;

            if (DA.GetData(13, ref inRFEM))
            {
                rFMember = new RFMember((RFMember)inRFEM.Value);
                if (DA.GetData(1, ref sCS))
                {
                    rFMember.StartCrossSectionNo = sCS;
                }
            }
            else if (DA.GetData(0, ref inCurve) && DA.GetData(1, ref sCS))
            {
                var myRFLine = new RFLine();
                Component_RFLine.SetGeometry(inCurve, ref myRFLine);
                rFMember.BaseLine = myRFLine;
                rFMember.StartCrossSectionNo = sCS;
            }
            else
            {
                return;
            }
            if (DA.GetData(14, ref mod))
            {
                rFMember.ToModify = mod;
            }
            if (DA.GetData(15, ref del))
            {
                rFMember.ToDelete = del;
            }
            if (DA.GetData(2, ref noIndex))
            {
                rFMember.No = noIndex;
            }
            if (DA.GetData(3, ref comment))
            {
                rFMember.Comment = comment;
            }
            if (DA.GetData(4, ref lineNo))
            {
                rFMember.LineNo = lineNo;
            }
            if (DA.GetData(5, ref memberType))
            {
                Enum.TryParse(memberType, out MemberType myMemberType);
                rFMember.Type = myMemberType;
            }
            if (DA.GetData(6, ref rotAngle))
            {
                rFMember.RotationType = RotationType.Angle;
                rFMember.RotationAngle = rotAngle;
            }
            if (DA.GetData(7, ref eCS))
            {
                rFMember.EndCrossSectionNo = eCS;
            }
            if (DA.GetData(8, ref sH))
            {
                rFMember.StartHingeNo = sH;
            }
            if (DA.GetData(9, ref eH))
            {
                rFMember.EndHingeNo = eH;
            }
            if (DA.GetData(10, ref ecc))
            {
                rFMember.EccentricityNo = ecc;
            }
            if (DA.GetData(11, ref div))
            {
                rFMember.DivisionNo = div;
            }
            if (DA.GetData(12, ref taperType))
            {
                Enum.TryParse(taperType, out TaperShapeType myTaperType);
                rFMember.TaperShape = myTaperType;
            }
            DA.SetData(0, rFMember);
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
                return RFEM_daq.Properties.Resources.Assemble_Member;                
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("5777bc55-241d-4900-8990-095ef2739255"); }
        }
    }
}
