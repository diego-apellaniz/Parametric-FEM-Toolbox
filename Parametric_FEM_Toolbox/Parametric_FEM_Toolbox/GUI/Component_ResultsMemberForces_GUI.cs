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
    public class Component_ResultsMemberForces_GUI : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public Component_ResultsMemberForces_GUI()
            : base("Member Forces", "MForces", "Get Member Forces from Calculation Results.", "B+G Toolbox", "RFEM")
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
            pManager.AddParameter(new Param_RFEM(), "Calculation Results", "Results", "Calculation results of a certain load case or load combination.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            // Use the pManager object to register your output parameters.
            // Output parameters do not have default values, but they too must have the correct access type.

            //pManager.AddPointParameter("Nodes", "N", "Tensegrity nodes", GH_ParamAccess.tree);

            //pManager.AddGenericParameter("Poly", "P", "Polygon cables", GH_ParamAccess.list);
            //pManager.AddGenericParameter("Bracing", "B", "Bracing cables", GH_ParamAccess.list);
            //pManager.AddGenericParameter("Struts", "Str", "Strut elements", GH_ParamAccess.list);
            //pManager.AddGenericParameter("Supports", "Sup", "Support points", GH_ParamAccess.list);

            pManager.AddIntegerParameter("Member Number", "No", "Index of the RFEM Member", GH_ParamAccess.list);
            pManager.AddNumberParameter("Location", "Loc", "Location of results along member axis", GH_ParamAccess.tree);
            pManager.AddVectorParameter("Forces", "F", "Member Forces [kN]", GH_ParamAccess.tree);
            pManager.AddVectorParameter("Moments", "M", "Member Moments [kNm]", GH_ParamAccess.tree);
            pManager.AddTextParameter("Results Flag", "Flag", "Results Flag", GH_ParamAccess.list);            
            pManager.AddTextParameter("Results Type", "Type", "Results Value Type", GH_ParamAccess.list);

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
            var listNo = new List<int>();
            var treeLoc = new DataTree<double>();
            var treeF = new DataTree<Vector3d>();
            var treeM = new DataTree<Vector3d>();
            var listLoading = new List<string>();
            var listFlag = new List<string>();
            var listType = new List<string>();

            // Input
            var inGH = new GH_RFEM();
            if (!DA.GetData(0, ref inGH))
            {
                return;
            }
            var rfResults = (RFResults)inGH.Value;
            var member_forces = rfResults.MemberForces;

            // Get output
            for (int i = 0; i < member_forces.Count; i++)
            {
                var path = new GH_Path(i);
                listNo.Add(member_forces[i].MemberNo);
                treeLoc.AddRange(member_forces[i].Location, path);
                treeF.AddRange(member_forces[i].Forces, path);
                treeM.AddRange(member_forces[i].Moments, path);
                listFlag.Add(member_forces[i].Flag.ToString());
                listType.Add(member_forces[i].Type.ToString());
            }

            // Output
            DA.SetDataList(0, listNo);
            DA.SetDataTree(1, treeLoc);
            DA.SetDataTree(2, treeF);
            DA.SetDataTree(3, treeM);
            DA.SetDataList(4, listFlag);
            DA.SetDataList(5, listType);
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
                return Properties.Resources.Results_MemberForces;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("799b9e5b-74eb-402d-8f3d-27da6df15cea"); }
        }
    }
}
