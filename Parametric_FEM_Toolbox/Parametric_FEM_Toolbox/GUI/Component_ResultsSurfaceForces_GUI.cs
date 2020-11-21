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
    public class Component_ResultsSurfaceForces_GUI : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public Component_ResultsSurfaceForces_GUI()
            : base("Surface Forces", "SForces", "Get Surface Forces from Calculation Results.", "B+G Toolbox", "RFEM")
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

            //pManager.AddPointParameter("Nodes", "N", "Tensegrity nodes", GH_ParamAccess.tree);

            pManager.AddIntegerParameter("Surface Number", "No", "Index of the RFEM Surface", GH_ParamAccess.tree);
            pManager.AddPointParameter("Coordinates", "Pt", "Coordinates of results", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("Location", "Loc", "Location of results", GH_ParamAccess.tree);
            pManager.AddTextParameter("Type", "Type", "Results Type", GH_ParamAccess.tree);
            pManager.AddNumberParameter("AxialForceAlphaM", "αm", "Axial force αm [kN/m]", GH_ParamAccess.tree);
            pManager.AddNumberParameter("AxialForceN1", "n1", "Axial force n1 [kN/m]", GH_ParamAccess.tree);
            pManager.AddNumberParameter("AxialForceN2", "n2", "Axial force n2 [kN/m]", GH_ParamAccess.tree);
            pManager.AddNumberParameter("AxialForceNcD", "nc,D", "Axial force nc,D [kN/m]", GH_ParamAccess.tree);
            pManager.AddNumberParameter("AxialForceNx", "nx", "Axial force nx [kN/m]", GH_ParamAccess.tree);
            pManager.AddNumberParameter("AxialForceNxD", "nx,D", "Axial force nx,D [kN/m]", GH_ParamAccess.tree);
            pManager.AddNumberParameter("AxialForceNxy", "nxy", "Axial force nxy [kN/m]", GH_ParamAccess.tree);
            pManager.AddNumberParameter("AxialForceNy", "ny", "Axial force ny [kN/m]", GH_ParamAccess.tree);
            pManager.AddNumberParameter("AxialForceNyD", "ny,D", "Axial force ny,D [kN/m]", GH_ParamAccess.tree);
            pManager.AddNumberParameter("AxialForceVMaxM", "vmax,m", "Axial force vmax,m [kN/m]", GH_ParamAccess.tree);
            pManager.AddNumberParameter("MomentAlphaB", "αb", "Moment αb [kNm/m]", GH_ParamAccess.tree);
            pManager.AddNumberParameter("MomentM1", "m1", "Moment m1 [kNm/m]", GH_ParamAccess.tree);
            pManager.AddNumberParameter("MomentM2", "m2", "Moment m2 [kNm/m]", GH_ParamAccess.tree);
            pManager.AddNumberParameter("MomentMcDNegative", "mc,D-", "Moment mc,D- [kNm/m]", GH_ParamAccess.tree);
            pManager.AddNumberParameter("MomentMcDPositive", "mc,D+", "Moment mc,D+ [kNm/m]", GH_ParamAccess.tree);
            pManager.AddNumberParameter("MomentMx", "mx", "Moment mx [kNm/m]", GH_ParamAccess.tree);
            pManager.AddNumberParameter("MomentMxDNegative", "mx,D-", "Moment mx,D- [kNm/m]", GH_ParamAccess.tree);
            pManager.AddNumberParameter("MomentMxDPositive", "mx,D+", "Moment mx,D+ [kNm/m]", GH_ParamAccess.tree);
            pManager.AddNumberParameter("MomentMxy", "mxy", "Moment mxy [kNm/m]", GH_ParamAccess.tree);
            pManager.AddNumberParameter("MomentMy", "my", "Moment my [kNm/m]", GH_ParamAccess.tree);
            pManager.AddNumberParameter("MomentMyDNegative", "my,D-", "Moment my,D- [kNm/m]", GH_ParamAccess.tree);
            pManager.AddNumberParameter("MomentMxDPositive", "mx,D+", "Moment mx,D+ [kNm/m]", GH_ParamAccess.tree);
            pManager.AddNumberParameter("MomentTMaxB", "mT,max,b", "Moment mT,max,b [kNm/m]", GH_ParamAccess.tree);
            pManager.AddNumberParameter("ShearForceBetaB", "Βb", "Shear force Βb [kN/m]", GH_ParamAccess.tree);
            pManager.AddNumberParameter("ShearForceVMaxB", "vmax,b", "Shear force vmax,b [kN/m]", GH_ParamAccess.tree);
            pManager.AddNumberParameter("ShearForceVx", "vx", "Shear force vx [kN/m]", GH_ParamAccess.tree);
            pManager.AddNumberParameter("ShearForceVy", "vy", "Shear force vy [kN/m]", GH_ParamAccess.tree);
           
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
            var treeLoc = new DataTree<int>();
            var treeType = new DataTree<string>();

            var axialForceAlphaM = new DataTree<double>();
            var axialForceN1 = new DataTree<double>();
            var axialForceN2 = new DataTree<double>();
            var axialForceNcD = new DataTree<double>();
            var axialForceNx = new DataTree<double>();
            var axialForceNxD = new DataTree<double>();
            var axialForceNxy = new DataTree<double>();
            var axialForceNy = new DataTree<double>();
            var axialForceNyD = new DataTree<double>();
            var axialForceVMaxM = new DataTree<double>();
            var momentAlphaB = new DataTree<double>();
            var momentM1 = new DataTree<double>();
            var momentM2 = new DataTree<double>();
            var momentMcDNegative = new DataTree<double>();
            var momentMcDPositive = new DataTree<double>();
            var momentMx = new DataTree<double>();
            var momentMxDNegative = new DataTree<double>();
            var momentMxDPositive = new DataTree<double>();
            var momentMxy = new DataTree<double>();
            var momentMy = new DataTree<double>();
            var momentMyDNegative = new DataTree<double>();
            var momentMyDPositive = new DataTree<double>();
            var momentTMaxB = new DataTree<double>();
            var shearForceBetaB = new DataTree<double>();
            var shearForceVMaxB = new DataTree<double>();
            var shearForceVx = new DataTree<double>();
            var shearForceVy = new DataTree<double>();

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
            var sfc_forces = new DataTree<RFSurfaceForces>();
            for (int i = 0; i < rfResults.Count; i++)
            {
                if (!(rfResults[i].SurfaceForces == null))
                {
                    var path = new GH_Path(i);
                    sfc_forces.AddRange(rfResults[i].SurfaceForces, path);
                }
            }

            // Get output
            if (sfc_forces.DataCount == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No results available");
                return;
            }
            for (int i = 0; i < sfc_forces.BranchCount; i++)
            {
                for (int j = 0; j < sfc_forces.Branch(i).Count; j++)
                {
                    var path = new GH_Path(i, j);
                    var path2 = new GH_Path(i);
                    treeNo.Add(sfc_forces.Branch(i)[j].SurfaceNo, path2);
                    treeCoor.AddRange(sfc_forces.Branch(i)[j].Location, path);
                    treeLoc.AddRange(sfc_forces.Branch(i)[j].LocationNo, path);
                    treeType.AddRange(sfc_forces.Branch(i)[j].Type, path);
                    axialForceAlphaM.AddRange(sfc_forces.Branch(i)[j].AxialForceAlphaM, path);
                    axialForceN1.AddRange(sfc_forces.Branch(i)[j].AxialForceN1, path);
                    axialForceN2.AddRange(sfc_forces.Branch(i)[j].AxialForceN2, path);
                    axialForceNcD.AddRange(sfc_forces.Branch(i)[j].AxialForceNcD, path);
                    axialForceNx.AddRange(sfc_forces.Branch(i)[j].AxialForceNx, path);
                    axialForceNxD.AddRange(sfc_forces.Branch(i)[j].AxialForceNxD, path);
                    axialForceNxy.AddRange(sfc_forces.Branch(i)[j].AxialForceNxy, path);
                    axialForceNy.AddRange(sfc_forces.Branch(i)[j].AxialForceNy, path);
                    axialForceNyD.AddRange(sfc_forces.Branch(i)[j].AxialForceNyD, path);
                    axialForceVMaxM.AddRange(sfc_forces.Branch(i)[j].AxialForceVMaxM, path);
                    momentAlphaB.AddRange(sfc_forces.Branch(i)[j].MomentAlphaB, path);
                    momentM1.AddRange(sfc_forces.Branch(i)[j].MomentM1, path);
                    momentM2.AddRange(sfc_forces.Branch(i)[j].MomentM2, path);
                    momentMcDNegative.AddRange(sfc_forces.Branch(i)[j].MomentMcDNegative, path);
                    momentMcDPositive.AddRange(sfc_forces.Branch(i)[j].MomentMcDPositive, path);
                    momentMx.AddRange(sfc_forces.Branch(i)[j].MomentMx, path);
                    momentMxDNegative.AddRange(sfc_forces.Branch(i)[j].MomentMxDNegative, path);
                    momentMxDPositive.AddRange(sfc_forces.Branch(i)[j].MomentMxDPositive, path);
                    momentMxy.AddRange(sfc_forces.Branch(i)[j].MomentMxy, path);
                    momentMy.AddRange(sfc_forces.Branch(i)[j].MomentMy, path);
                    momentMyDNegative.AddRange(sfc_forces.Branch(i)[j].MomentMyDNegative, path);
                    momentMyDPositive.AddRange(sfc_forces.Branch(i)[j].MomentMyDPositive, path);
                    momentTMaxB.AddRange(sfc_forces.Branch(i)[j].MomentTMaxB, path);
                    shearForceBetaB.AddRange(sfc_forces.Branch(i)[j].ShearForceBetaB, path);
                    shearForceVMaxB.AddRange(sfc_forces.Branch(i)[j].ShearForceVMaxB, path);
                    shearForceVx.AddRange(sfc_forces.Branch(i)[j].ShearForceVx, path);
                    shearForceVy.AddRange(sfc_forces.Branch(i)[j].ShearForceVy, path);
                }
            }

            // Output
            DA.SetDataTree(0, treeNo);
            DA.SetDataTree(1, treeCoor);
            DA.SetDataTree(2, treeLoc);
            DA.SetDataTree(3, treeType);
            DA.SetDataTree(4, axialForceAlphaM);
            DA.SetDataTree(5, axialForceN1);
            DA.SetDataTree(6, axialForceN2);
            DA.SetDataTree(7, axialForceNcD);
            DA.SetDataTree(8, axialForceNx);
            DA.SetDataTree(9, axialForceNxD);
            DA.SetDataTree(10, axialForceNxy);
            DA.SetDataTree(11, axialForceNy);
            DA.SetDataTree(12, axialForceNyD);
            DA.SetDataTree(13, axialForceVMaxM);
            DA.SetDataTree(14, momentAlphaB);
            DA.SetDataTree(15, momentM1);
            DA.SetDataTree(16, momentM2);
            DA.SetDataTree(17, momentMcDNegative);
            DA.SetDataTree(18, momentMcDPositive);
            DA.SetDataTree(19, momentMx);
            DA.SetDataTree(20, momentMxDNegative);
            DA.SetDataTree(21, momentMxDPositive);
            DA.SetDataTree(22, momentMxy);
            DA.SetDataTree(23, momentMy);
            DA.SetDataTree(24, momentMyDNegative);
            DA.SetDataTree(25, momentMyDPositive);
            DA.SetDataTree(26, momentTMaxB);
            DA.SetDataTree(27, shearForceBetaB);
            DA.SetDataTree(28, shearForceVMaxB);
            DA.SetDataTree(29, shearForceVx);
            DA.SetDataTree(30, shearForceVy);
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
                return Properties.Resources.Results_ShellForces;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("df9e7012-2731-47d4-987c-6e3ae4c6fb52"); }
        }
    }
}
