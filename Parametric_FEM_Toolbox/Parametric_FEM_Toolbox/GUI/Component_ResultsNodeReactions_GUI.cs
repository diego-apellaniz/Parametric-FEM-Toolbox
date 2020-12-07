using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Parametric_FEM_Toolbox.RFEM;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parametric_FEM_Toolbox.GUI
{
    public class Component_ResultsNodeReactions_GUI : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public Component_ResultsNodeReactions_GUI()
            : base("Nodal Support Forces", "NSForces", "Get Nodal Support Forces from Calculation Results.", "B+G Toolbox", "RFEM")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Use the pManager object to register your input parameters.
            // You can often supply default values when creating parameters.
            // All parameters must have the correct access type. If you want 
            // to import lists or trees of values, modify the ParamAccess flag.
            pManager.AddParameter(new Param_RFEM(), "Calculation Results", "Results", "Calculation results of a certain load case or load combination.", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            // Use the pManager object to register your output parameters.
            // Output parameters do not have default values, but they too must have the correct access type.

            pManager.AddIntegerParameter("Support Number", "No", "Index of the RFEM Nodal Support", GH_ParamAccess.tree);
            pManager.AddPointParameter("Location", "Loc", "Support Location", GH_ParamAccess.tree);
            pManager.AddVectorParameter("Forces", "F", "Member Forces in global CSys [kN]", GH_ParamAccess.tree);
            pManager.AddVectorParameter("Moments", "M", "Member Moments in global CSys [kNm]", GH_ParamAccess.tree);
            pManager.AddTextParameter("Results Type", "Type", "Results Value Type", GH_ParamAccess.tree);

            // Sometimes you want to hide a specific parameter from the Rhino preview.
            // You can use the HideParameter() method as a quick way:
            //pManager.HideParameter(0);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Output variables
            var treeNo = new DataTree<int>();
            var treeCoor = new DataTree<Point3d>();
            var treeF = new DataTree<Vector3d>();
            var treeM = new DataTree<Vector3d>();
            var treeType = new DataTree<string>();

            // Input
            var inGH = new List<GH_RFEM>();
            var rfResults = new List<RFResults>();
            if (!DA.GetDataList(0, inGH))
            {
                return;
            }
            foreach (var gh in inGH)
            {
                if (!(gh == null))
                {
                    rfResults.Add((RFResults)gh.Value);
                }
            }
            var reaction_forces = new DataTree<RFNodalSupportForces>();
            for (int i = 0; i < rfResults.Count; i++)
            {
                if (!(rfResults[i].NodalSupportForces == null))
                {
                    var path = new GH_Path(i);
                    reaction_forces.AddRange(rfResults[i].NodalSupportForces, path);
                }
            }

            // Get output
            if (reaction_forces.DataCount == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No results available");
                return;
            }
            for (int i = 0; i < reaction_forces.BranchCount; i++)
            {
                for (int j = 0; j < reaction_forces.Branch(i).Count; j++)
                {
                    var path = new GH_Path(i);
                    treeNo.Add(reaction_forces.Branch(i)[j].NodeNo, path);
                    treeCoor.Add(reaction_forces.Branch(i)[j].Location, path);
                    treeF.Add(reaction_forces.Branch(i)[j].Forces, path);
                    treeM.Add(reaction_forces.Branch(i)[j].Moments, path);
                    treeType.Add(reaction_forces.Branch(i)[j].Type, path);
                }
            }

            // Output
            DA.SetDataTree(0, treeNo);
            DA.SetDataTree(1, treeCoor);
            DA.SetDataTree(2, treeF);
            DA.SetDataTree(3, treeM);
            DA.SetDataTree(4, treeType);
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
                return Properties.Resources.Results_Reactions;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("1dd90138-c7ef-4811-ac44-ad0e68199921"); }
        }
    }
}
