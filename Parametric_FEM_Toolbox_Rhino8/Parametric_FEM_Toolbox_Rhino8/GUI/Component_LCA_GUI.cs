using System;
using System.Collections.Generic;
using System.Linq;
using Dlubal.RFEM5;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Parameters;
using Parametric_FEM_Toolbox.HelperLibraries;
using Parametric_FEM_Toolbox.RFEM;
using Parametric_FEM_Toolbox.UIWidgets;
using Rhino.Geometry;

namespace Parametric_FEM_Toolbox.GUI
{
    public class Component_LCA_GUI : GH_SwitcherComponent
    {
        // Declare class variables outside the method "SolveInstance" so their values persist 
        // when the method is called again.

        private List<string> _materials = new List<string>();
        private DataTree<double> _massTree = new DataTree<double>();
        private DataTree<double> _volumeTree = new DataTree<double>();
        private DataTree<Brep> _brepTree = new DataTree<Brep>();


        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public Component_LCA_GUI()
          : base("Input for LCA", "LCA", "Get Model masses groupes by material for Life Cycle Analysis", "B+G Toolbox", "RFEM")
        {
        }

        // Define Keywords to search for this Component more easily in Grasshopper
        public override IEnumerable<string> Keywords => new string[] {"rf", "rflist", "lca", "rfem"};

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Use the pManager object to register your input parameters.
            pManager.AddBooleanParameter("Run component?", "Run", "If true, the programm is executed.", GH_ParamAccess.item, false);
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
            pManager.AddTextParameter("Materials", "Mat", "Materials of the RFEM model", GH_ParamAccess.list);
            pManager.AddBrepParameter("Geometry", "Geo", "Geometry of the RFEM Objects", GH_ParamAccess.tree);
            pManager.AddNumberParameter("Mass", "Mass", "Mass of the RFEM objects in [kg]", GH_ParamAccess.tree);
            pManager.AddNumberParameter("Volume", "Vol", "Volume of the RFEM objects in [m3]", GH_ParamAccess.tree);

            // Sometimes you want to hide a specific parameter from the Rhino preview.
            // You can use the HideParameter() method as a quick way:
            // pManager.HideParameter(0);
        }

        protected override void RegisterEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit("Input for LCA", "LCA", "Get Model masses groupes by material for Life Cycle Analysis", Properties.Resources.RFEM_LCA);
            mngr.RegisterUnit(evaluationUnit);
            evaluationUnit.RegisterInputParam(new Param_String(), "Model Name", "Model Name", "Segment of the name of the RFEM Model to get information from", GH_ParamAccess.item);
            evaluationUnit.Inputs[0].Parameter.Optional = true;
            GH_ExtendableMenu gH_ExtendableMenu0 = new GH_ExtendableMenu(0, "Advanced");
            gH_ExtendableMenu0.Name = "Advanced";
            gH_ExtendableMenu0.Expand();
            gH_ExtendableMenu0.RegisterInputPlug(evaluationUnit.Inputs[0]);
            evaluationUnit.AddMenu(gH_ExtendableMenu0);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA, EvaluationUnit unit)
        {
            // RFEM variables
            var modelName = "";
            IModel model = null;
            IModelData data = null;

            //Additional variables
            var dicMat = new Dictionary<int, Material>();
            var dicMass = new Dictionary<int, List<double>>();
            var dicVol = new Dictionary<int, List<double>>();
            var dicGeo = new Dictionary<int, List<Brep>>();

            // Input
            var msg = "";
            var msgs = new List<string>();
            var run = false;
            DA.GetData(0, ref run);


            if (run)
            {
                if (!DA.GetData(1, ref modelName))
                {
                    Component_GetData.ConnectRFEM(ref model, ref data);
                }
                else
                {
                    Component_GetData.ConnectRFEM(modelName, ref model, ref data);
                }
                _materials.Clear();
                _massTree.Clear();
                _volumeTree.Clear();
                _brepTree.Clear();
                try
                {
                    // Get Surfaces
                    var sfcs = Component_GetData.GetRFSurfaces(data.GetSurfaces().ToList(), data);
                    foreach (var sfc in sfcs)
                    {
                        // Add material to dictionary
                        if (!dicMat.ContainsKey(sfc.MaterialNo))
                        {
                            dicMat.Add(sfc.MaterialNo, data.GetMaterial(sfc.MaterialNo, ItemAt.AtNo).GetData());
                            dicMass.Add(sfc.MaterialNo, new List<double>());
                            dicVol.Add(sfc.MaterialNo, new List<double>());
                            dicGeo.Add(sfc.MaterialNo, new List<Brep>());
                        }
                        // Add mass to output list
                        dicVol[sfc.MaterialNo].Add(sfc.Area * sfc.Thickness);
                        dicMass[sfc.MaterialNo].Add(sfc.Area * sfc.Thickness* dicMat[sfc.MaterialNo].SpecificWeight / 10.0);

                        // Add Geometry to output list
                        dicGeo[sfc.MaterialNo].Add(sfc.ToBrep());
                    }
                    // Get Members
                    var members = Component_GetData.GetRFMembers(data.GetMembers().ToList(), data);
                    var crosecs = Component_GetData.GetRFCroSecs(data.GetCrossSections().ToList(), model, ref msgs);
                    var cs_indeces = crosecs.Select(x => x.No).ToList();
                    foreach (var member in members)
                    {
                        var mat_index = crosecs.Where(x => x.No == member.StartCrossSectionNo).ToList()[0].MatNo;
                       

                        // Add material to dictionary
                        if (!dicMat.ContainsKey(mat_index))
                        {
                            dicMat.Add(mat_index, data.GetMaterial(mat_index, ItemAt.AtNo).GetData());
                            dicMass.Add(mat_index, new List<double>());
                            dicVol.Add(mat_index, new List<double>());
                            dicGeo.Add(mat_index, new List<Brep>());
                        }

                        // Add mass to output list
                        if (member.EndCrossSectionNo == 0) // In case of tension members, etc.
                        {
                            member.EndCrossSectionNo = member.StartCrossSectionNo;
                        }
                        if (!(cs_indeces.Contains(member.StartCrossSectionNo)) || (!(cs_indeces.Contains(member.EndCrossSectionNo))))
                        {
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,$"Provide cross sections for member No {member.No}.");
                            continue;
                        }
                        var startCroSec = crosecs[cs_indeces.IndexOf(member.StartCrossSectionNo)];
                        var endCroSec = crosecs[cs_indeces.IndexOf(member.EndCrossSectionNo)];
                        var volume = (startCroSec.A + endCroSec.A) / 2 * member.Length;
                        dicVol[mat_index].Add(volume);
                        dicMass[mat_index].Add(member.Weight);                        

                        // Add Geometry to output list
                        var memberShape = Component_ExtrudeMembers.ExtrudeMembersToBrep(member, crosecs, member.Length / 8.0, out msg);
                        if (memberShape == null)
                        {
                            dicGeo[mat_index].Add(null);
                        }else
                        {
                            dicGeo[mat_index].Add(memberShape[0]);
                        }                        
                    }
                    // Prepare Output
                    var matSorted = dicMat.Values.OrderBy(x => x.No).ToList();
                    _materials = matSorted.Select(x => x.Description).ToList();
                    for (int i = 0; i < matSorted.Count; i++)
                    {
                        var path = new GH_Path(i);
                        _volumeTree.EnsurePath(path);
                        _volumeTree.Branch(path).AddRange(dicVol[matSorted[i].No]);
                        _massTree.EnsurePath(path);
                        _massTree.Branch(path).AddRange(dicMass[matSorted[i].No]);
                        _brepTree.EnsurePath(path);
                        _brepTree.Branch(path).AddRange(dicGeo[matSorted[i].No]);
                    }

                }
                catch (Exception ex)
                {
                    Component_GetData.DisconnectRFEM(ref model, ref data);
                    _materials.Clear();
                    _massTree.Clear();
                    _volumeTree.Clear();
                    _brepTree.Clear();
                    throw ex;
                }
                Component_GetData.DisconnectRFEM(ref model, ref data);
                if (msgs.Count > 0)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, String.Join(System.Environment.NewLine, msg.ToArray()));
                }
                if (msg != "")
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, msg);
                }
            }
            DA.SetDataList(0, _materials);
            DA.SetDataTree(1, _brepTree);
            DA.SetDataTree(2, _massTree);
            DA.SetDataTree(3, _volumeTree);
        }
        /// <summary>
        /// The Exposure property controls where in the panel a component icon 
        /// will appear. There are seven possible locations (primary to septenary), 
        /// each of which can be combined with the GH_Exposure.obscure flag, which 
        /// ensures the component will only be visible on panel dropdowns.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
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
                return Properties.Resources.RFEM_LCA;                
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get {return new Guid("f9ce04dd-f445-4e13-a846-1de928182c1a");}
        }
    }
}
