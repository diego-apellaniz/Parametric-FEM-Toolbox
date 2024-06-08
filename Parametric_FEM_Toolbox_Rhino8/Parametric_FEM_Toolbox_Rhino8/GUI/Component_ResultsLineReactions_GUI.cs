using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Parametric_FEM_Toolbox.GUI;
using Parametric_FEM_Toolbox.RFEM;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parametric_FEM_Toolbox.GUI
{
    public class Component_ResultsLineReactions_GUI : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public Component_ResultsLineReactions_GUI()
            : base("Line Support Forces", "LSForces", "Get Line Support Forces from Calculation Results.", "B+G Toolbox", "RFEM")
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
            pManager.AddIntegerParameter("Line Number", "No", "Index of the RFEM Line", GH_ParamAccess.tree);
            pManager.AddNumberParameter("Location", "Loc", "Location of results along line axis", GH_ParamAccess.tree);
            pManager.AddVectorParameter("Forces", "F", "Reaction Forces [kN]", GH_ParamAccess.tree);
            pManager.AddVectorParameter("Moments", "M", "Reaction Moments [kNm]", GH_ParamAccess.tree);
            pManager.AddTextParameter("Results Flag", "Flag", "Results Flag", GH_ParamAccess.tree);            

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
            var treeLoc = new DataTree<double>();
            var treeF = new DataTree<Vector3d>();
            var treeM = new DataTree<Vector3d>();
            //var listLoading = new List<string>();
            var treeFlag = new DataTree<string>();
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
            var support_forces = new DataTree<RFLineSupportForces>();
            for (int i = 0; i < rfResults.Count; i++)
            {
                if (!(rfResults[i].LineSupportForces == null))
                {
                    var path = new GH_Path(i);
                    support_forces.AddRange(rfResults[i].LineSupportForces, path);
                }                
            }

            // Get output
            if (support_forces.DataCount == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No results available");
                return;
            }
            for (int i = 0; i < support_forces.BranchCount; i++)
            {                               
                for (int j = 0; j < support_forces.Branch(i).Count; j++)
                {
                    var path1 = new GH_Path(i,j);
                    var path2 = new GH_Path(i);
                    treeNo.Add(support_forces.Branch(i)[j].LineNo, path2);
                    for (int k = 0; k < support_forces.Branch(i)[j].Location.Count; k++)
                    {
                        if(support_forces.Branch(i)[j].Type[k] == rfResults[i].ResultType)
                        {
                            treeLoc.Add(support_forces.Branch(i)[j].Location[k], path1);
                            treeF.Add(support_forces.Branch(i)[j].Forces[k], path1);
                            treeM.Add(support_forces.Branch(i)[j].Moments[k], path1);
                        }
                    }
                    treeFlag.Add(support_forces.Branch(i)[j].Flag.ToString(), path2);
                }                
            }

            // Output
            DA.SetDataTree(0, treeNo);
            DA.SetDataTree(1, treeLoc);
            DA.SetDataTree(2, treeF);
            DA.SetDataTree(3, treeM);
            DA.SetDataTree(4, treeFlag);
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
                return Properties.Resources.Results_LineReactions;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("d216ddd1-61c8-4c22-af07-31e58c22dbdc"); }
        }
    }
}
