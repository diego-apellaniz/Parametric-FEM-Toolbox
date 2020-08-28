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
using Parametric_FEM_Toolbox.GUI;
using System.Runtime.InteropServices;

namespace Parametric_FEM_Toolbox.Deprecated
{
    public class Component_RFMaterial_GUI_OBSOLETE : GH_SwitcherComponent
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
        public Component_RFMaterial_GUI_OBSOLETE()
          : base("RF Material", "RFMaterial", "Creates a RFMaterial object to define new data or modify existing data " +
                "in the RFEM model.", "B+G Toolbox", "RFEM")
        {
        }

        // Define Keywords to search for this Component more easily in Grasshopper
        public override IEnumerable<string> Keywords => new string[] {"rf", "rfmaterial", "material"};

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Use the pManager object to register your input parameters.
            pManager.AddTextParameter("Description", "Desc", "Name or Description of Cross Section.", GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager.AddIntegerParameter("Material Number", "No", "Optional index number to assign to the RFEM object.", GH_ParamAccess.item);
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
            pManager.RegisterParam(new Param_RFEM(), "Material", "Mat", "Output RFMat.", GH_ParamAccess.item);

            // Sometimes you want to hide a specific parameter from the Rhino preview.
            // You can use the HideParameter() method as a quick way:
            // pManager.HideParameter(0);
        }

        protected override void RegisterEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit("Assemble Material", "Mat", "Creates a RFMaterial object to define new data or modify existing data " +
                "in the RFEM model.");
            mngr.RegisterUnit(evaluationUnit);


            evaluationUnit.RegisterInputParam(new Param_Number(), "Elasticity Modulus [N/m²]", "E", "Elasticity Modulus [N/m²]", GH_ParamAccess.item);
            evaluationUnit.Inputs[0].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Number(), "Poisson Ratio", "Mu", "Poisson Ratio", GH_ParamAccess.item);
            evaluationUnit.Inputs[1].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Number(), "Shear Modulus [N/m²]", "G", "Shear Modulus [N/m²]", GH_ParamAccess.item);
            evaluationUnit.Inputs[2].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Number(), "Specific Weight [N/m³]", "W", "Specific Weight [N/m³]", GH_ParamAccess.item);
            evaluationUnit.Inputs[3].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Number(), "Thermal Expansion [1/°]", "Alpha", "Thermal Expansion [1/°]", GH_ParamAccess.item);
            evaluationUnit.Inputs[4].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Number(), "Partial Safety Factor", "Gamma", "Partial Safety Factor", GH_ParamAccess.item);
            evaluationUnit.Inputs[5].Parameter.Optional = true;


            evaluationUnit.RegisterInputParam(new Param_RFEM(), "Material", "Mat", "Material object from the RFEM model to modify", GH_ParamAccess.item);
            evaluationUnit.Inputs[6].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Boolean(), "Modify", "Modify", "Modify node?", GH_ParamAccess.item);
            evaluationUnit.Inputs[7].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Boolean(), "Delete", "Delete", "Delete node?", GH_ParamAccess.item);
            evaluationUnit.Inputs[8].Parameter.Optional = true;

            GH_ExtendableMenu gH_ExtendableMenu = new GH_ExtendableMenu(0, "advanced");
            gH_ExtendableMenu.Name = "Advanced";
            gH_ExtendableMenu.Collapse();
            evaluationUnit.AddMenu(gH_ExtendableMenu);
            for (int i = 0; i < 6; i++)
            {
                gH_ExtendableMenu.RegisterInputPlug(evaluationUnit.Inputs[i]);
            }

            GH_ExtendableMenu gH_ExtendableMenu2 = new GH_ExtendableMenu(1, "modify");
            gH_ExtendableMenu2.Name = "Modify";
            gH_ExtendableMenu2.Collapse();
            evaluationUnit.AddMenu(gH_ExtendableMenu2);
            for (int i = 6; i < 6+3; i++)
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
            var rfMaterial = new RFMaterial();
            var inRFEM = new GH_RFEM();
            var mod = false;
            var del = false;
            var E = 0.0;
            var W = 0.0;
            var Alpha = 0.0;
            var Mu = 0.0;
            var G = 0.0;
            var Gamma = 0.0;


            if (DA.GetData(9, ref inRFEM))
            {
                rfMaterial = new RFMaterial((RFMaterial)inRFEM.Value);
                if (DA.GetData(0, ref description))
                {
                    rfMaterial.Description = description;
                }
            }
            else if (DA.GetData(0, ref description))
            {
                rfMaterial.Description = description;
            }
            else
            {
                return;
            }
            if (DA.GetData(10, ref mod))
            {
                rfMaterial.ToModify = mod;
            }
            if (DA.GetData(11, ref del))
            {
                rfMaterial.ToDelete = del;
            }
            if (DA.GetData(1, ref noIndex))
            {
                rfMaterial.No = noIndex;
            }
            if (DA.GetData(2, ref comment))
            {
                rfMaterial.Comment = comment;
            }
            if (DA.GetData(3, ref E))
            {
                rfMaterial.E = E;
            }
            if (DA.GetData(4, ref Mu))
            {
                rfMaterial.Mu = Mu;
            }
            if (DA.GetData(5, ref G))
            {
                rfMaterial.G = G;
            }
            if (DA.GetData(6, ref W))
            {
                rfMaterial.W = W;
            }
            if (DA.GetData(7, ref Alpha))
            {
                rfMaterial.Alpha = Alpha;
            }
            if (DA.GetData(8, ref Gamma))
            {
                rfMaterial.Gamma = Gamma;
            }
            DA.SetData(0, rfMaterial);
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
                return Properties.Resources.Assemble_Material;                
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("7d7f3d68-3742-4092-af83-242d3da8fca3"); }
        }
    }
}
