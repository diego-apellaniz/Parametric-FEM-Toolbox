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
using Dlubal.RFEM5;
using System.Runtime.InteropServices;
using Grasshopper;
using Grasshopper.Kernel.Data;

namespace Parametric_FEM_Toolbox.GUI
{
    public class Component_GetResults_GUI : GH_SwitcherComponent
    {
        // Declare class variables outside the method "SolveInstance" so their values persist 
        // when the method is called again.
        List<RFNode> rfNodes = new List<RFNode>(); // Calculation results
        DataTree<Mesh> _feMeshes = new DataTree<Mesh>(); // Output FE Meshes -> The teee path refers to the surface number that originated that mesh
        DataTree<Vector3d> _meshdisplacements = new DataTree<Vector3d>();
        DataTree<Mesh> _deformedMeshes = new DataTree<Mesh>();
        DataTree<Point3d> _controlPoints = new DataTree<Point3d>(); // For member displacements
        DataTree<Vector3d> _memberdisplacements = new DataTree<Vector3d>();
        DataTree<Curve> _deformedMembers = new DataTree<Curve>();

        //private List<string> _loadCases = new List<string>();
        //private List<string> _loadCombos = new List<string>();
        //private List<string> _resultCombos = new List<string>();
        private bool _resetLC = false; // Get displacements again
        private List<string> _lCasesAndCombos = new List<string>();
        private int _countCases = 0;
        private int _countCombos = 0;
        private int _countRcombos = 0;
        ICalculation _results = null;
        IFeMesh _rfemMesh = null;
        IResults _lcresults = null;
        IModelData _saveddata = null;
        //NodalDeformations[] _deformations = null;


        //int modelDataCount = 0;
        //int modelDataCount1 = 0;
        //int modelDataCount2 = 0;
        //int modelDataCount3 = 0;

        private MenuDropDown _loadDrop;
        //private List<string> _loadingTypes = new List<string>() {"Load Case", "Load Combo", "Result Combo" };
        //private MenuDropDown _loadDrop2;

        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public Component_GetResults_GUI()
          : base("Get Results", "Get Results", "Gets Results from the RFEM Model.", "B+G Toolbox", "RFEM")
        {
        }

        // Define Keywords to search for this Component more easily in Grasshopper
        public override IEnumerable<string> Keywords => new string[] { "rf", "get", "results" };

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Use the pManager object to register your input parameters.
            pManager.AddBooleanParameter("Run component?", "Run", "If true, the programm is executed.", GH_ParamAccess.item, false);
            //modelDataCount = pManager.ParamCount;
            // If you want to change properties of certain parameters, 
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
            // Sometimes you want to hide a specific parameter from the Rhino preview.
            // You can use the HideParameter() method as a quick way:
            // pManager.HideParameter(0);
        }

        protected override void RegisterEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit("Get Results", "Get Results", "Gets Results from the RFEM Model.", Properties.Resources.icon_GetResults);
            mngr.RegisterUnit(evaluationUnit);

            // Deformation

            evaluationUnit.RegisterInputParam(new Param_Number(), "Scale Factor", "Scale Factor", "Scale Factor applied to the deformed shape.", GH_ParamAccess.item, new GH_Number(1));
            evaluationUnit.Inputs[0].Parameter.Optional = true;

            evaluationUnit.RegisterOutputParam(new Param_Curve(), "Crv", "Curves", "Deformed shape of the linear elemnents of the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_Mesh(), "Msh", "Meshes", "Deformed shape of the surface elemnents of the RFEM Model.");

            //modelDataCount = evaluationUnit.Inputs.Count;

            GH_ExtendableMenu gH_ExtendableMenu0 = new GH_ExtendableMenu(0, "Deformation");
            gH_ExtendableMenu0.Name = "Deformation";
            gH_ExtendableMenu0.Expand();
            evaluationUnit.AddMenu(gH_ExtendableMenu0);
            gH_ExtendableMenu0.RegisterInputPlug(evaluationUnit.Inputs[0]);
            gH_ExtendableMenu0.RegisterOutputPlug(evaluationUnit.Outputs[0]);
            gH_ExtendableMenu0.RegisterOutputPlug(evaluationUnit.Outputs[1]);
            //for (int i = 0; i < modelDataCount; i++)
            //{
            //    gH_ExtendableMenu0.RegisterInputPlug(evaluationUnit.Inputs[i]);
            //    gH_ExtendableMenu0.RegisterOutputPlug(evaluationUnit.Outputs[i]);
            //}


            // Load Cases and Combos

            GH_ExtendableMenu gH_ExtendableMenu1 = new GH_ExtendableMenu(1, "Load Cases and Combos");
            gH_ExtendableMenu1.Name = "Load Cases and Combos";
            gH_ExtendableMenu1.Expand();
            evaluationUnit.AddMenu(gH_ExtendableMenu1);
            MenuPanel menuPanel = new MenuPanel(1, "panel_load");
            menuPanel.Header = "Set here the load case for display.\n";
            gH_ExtendableMenu1.AddControl(menuPanel);
            _loadDrop = new MenuDropDown(1, "dropdown_loads_1", "loading type");
            //_loadDrop.VisibleItemCount = 3;
            _loadDrop.ValueChanged += _loadDrop__valueChanged;
            _loadDrop.Header = "Set here the loading type for display.\n";
            menuPanel.AddControl(_loadDrop);

            // Advanced

            evaluationUnit.RegisterInputParam(new Param_RFEM(), "Trigger", "Trigger", "Input to trigger the optimization", GH_ParamAccess.tree);
            evaluationUnit.Inputs[1].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_String(), "Model Name", "Model Name", "Segment of the name of the RFEM Model to get information from", GH_ParamAccess.item);
            evaluationUnit.Inputs[2].Parameter.Optional = true;

            GH_ExtendableMenu gH_ExtendableMenu3 = new GH_ExtendableMenu(3, "Advanced");
            gH_ExtendableMenu3.Name = "Advanced";
            gH_ExtendableMenu3.Collapse();
            evaluationUnit.AddMenu(gH_ExtendableMenu3);
            for (int i = 1; i < 1 + 2; i++)
            {
                gH_ExtendableMenu3.RegisterInputPlug(evaluationUnit.Inputs[i]);
            }
        }



        protected override void OnComponentLoaded()
        {
            _loadDrop.Clear();
            updateDropDownMenu(_lCasesAndCombos);
        }

        protected override void OnComponentReset(GH_ExtendableComponentAttributes attr)
        {
            _loadDrop.Clear();
            updateDropDownMenu(_lCasesAndCombos);
        }

        private void _loadDrop__valueChanged(object sender, EventArgs e)
        {
            _ = (MenuDropDown)sender;
            _resetLC = true; // Get deformations
            setModelProps();
        }

        protected void setModelProps()
        {
            ((GH_DocumentObject)this).ExpireSolution(true);
        }

        private void updateDropDownMenu(List<string> _lcs)
        {
            _loadDrop.VisibleItemCount = Math.Min(10, _lcs.Count); // Maximum visible items = 10
            int value = 0;
            if (_lcs.Count == _loadDrop.Items.Count)
            {
                value = _loadDrop.Value;
            }
            _loadDrop.Clear();
            if (_lcs.Count > 0)
            {
                for (int i = 0; i < _lcs.Count; i++)
                {
                    _loadDrop.AddItem(_lcs[i], _lcs[i]);
                }
                if (_loadDrop.Items.Count > value)
                {
                    _loadDrop.Value = value;
                }
            }
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA, EvaluationUnit unit)
        {

            //OnComponentLoaded();

            // Input counter
            //modelDataCount1 = 1 + modelDataCount2;

            // RFEM variables
            var modelName = "";
            IModel model = null;
            IModelData data = null;
            ILoads loads = null;

            // Output message
            var msg = new List<string>();

            // Assign GH Input
            bool run = false;
            var scale = 0.0;
            DA.GetData(0, ref run);
            DA.GetData(1, ref scale);

            // Do stuff
            if (run)
            {
                if (!DA.GetData(3, ref modelName))
                {
                    Component_GetData.ConnectRFEM(ref model, ref data);
                }
                else
                {
                    Component_GetData.ConnectRFEM(modelName, ref model, ref data);
                }
                _saveddata = data;
                try
                {
                    // Get deformtions 
                    _resetLC = true;
                    // Get loads
                    Component_GetData.GetLoadsFromRFEM(model, ref loads);
                    // Get calculation results
                    _results = model.GetCalculation();
                    var errors = _results.CalculateAll();
                    if (errors != null)
                    {
                        msg.AddRange(errors.Select(x => x.Description));
                    }                    
                    // Update load cases and combos to display in dropdown menu
                    loads.GetLoadCasesAndCombos(ref _lCasesAndCombos, ref _countCases, ref _countCombos, ref _countRcombos);
                    updateDropDownMenu(_lCasesAndCombos);
                    // Get Fe Meshes from RFEM
                    _rfemMesh = _results.GetFeMesh();
                    _feMeshes = CreateFEMeshes(ref msg);
                    // _controlPoints = CreateControlPoints(ref msg); -> Obtained with displacements
                }
                catch (Exception ex)
                {
                    // Clear output!!!
                    _saveddata = null;
                    _rfemMesh = null;
                    _results = null;
                    _lcresults = null;
                    _feMeshes.Clear();
                    _meshdisplacements.Clear();
                    _deformedMeshes.Clear();
                    _controlPoints.Clear();
                    _memberdisplacements.Clear();
                    _deformedMembers.Clear();
                    throw ex;
                }
                Component_GetData.DisconnectRFEM(ref model, ref data);
            }
            // Get results to display
            if (_loadDrop.Items.Count > 0 && _resetLC && msg.Count==0)
            {
                int no = Int16.Parse(_loadDrop.Items[_loadDrop.Value].name.Split(' ')[1]);
                if (_loadDrop.Value < _countCases)
                {
                    _lcresults = _results.GetResultsInFeNodes(LoadingType.LoadCaseType, no);
                }
                else if (_loadDrop.Value < _countCases + _countCombos)
                {
                    _lcresults = _results.GetResultsInFeNodes(LoadingType.LoadCombinationType, no);
                }
                else if (_loadDrop.Value < _countCases + _countCombos + _countRcombos)
                {
                    _lcresults = _results.GetResultsInFeNodes(LoadingType.ResultCombinationType, no);
                }
                else
                {
                    msg.Add("Load case or combo not found");
                }
                // Get deformations
                _meshdisplacements = GetMeshDisplacements(ref msg);
                _memberdisplacements = GetMemberDisplacements(ref msg);
                // Set _resetLC to false again
                _resetLC = false;
            }

            // Get output
            _deformedMeshes = GetDeformedMeshes(scale, ref msg);
            _deformedMembers = GetDeformedMembers(scale, ref msg);

            // Assign GH Output
            DA.SetDataTree(0, _deformedMembers);
            DA.SetDataTree(1, _deformedMeshes);

            if (msg.Count != 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, String.Join(System.Environment.NewLine, msg.ToArray()));
            }
        }

        private DataTree<Mesh> CreateFEMeshes(ref List<string> msg)
        {
            var feMeshes = new DataTree<Mesh>();
            // Create fe mesh from each surface element
            foreach (var sfc in _saveddata.GetSurfaces())
            {
                // Get surface number
                var no = sfc.No;
                // Create mesh -> add vertices and faces
                var feMesh = new Mesh();
                var fesurfacenodes = _rfemMesh.GetSurfaceNodes(no, ItemAt.AtNo).OrderBy(o => o.No).ToList(); // Sort nodes so there are no errors when applying displacements
                var dicNodes = new Dictionary<int, int>(); // We need to save the node index numbers in order to define the mesh faces later
                for (int i = 0; i < fesurfacenodes.Count; i++)
                {
                    var node_location = new Point3d(fesurfacenodes[i].X, fesurfacenodes[i].Y, fesurfacenodes[i].Z);
                    feMesh.Vertices.Add(node_location);
                    dicNodes.Add(fesurfacenodes[i].No, i);
                }
                var fe2delements = _rfemMesh.GetSurfaceElements(no, ItemAt.AtNo);
                foreach (var element in fe2delements)
                {
                    var node_numbers = element.NodeNumbers;
                    if (node_numbers.Length == 4 && node_numbers[3] != 0)
                    {
                        dicNodes.TryGetValue(node_numbers[0], out int node0);
                        dicNodes.TryGetValue(node_numbers[1], out int node1);
                        dicNodes.TryGetValue(node_numbers[2], out int node2);
                        dicNodes.TryGetValue(node_numbers[3], out int node3);
                        feMesh.Faces.AddFace(node0, node1, node2, node3);
                    }
                    else if (node_numbers.Length == 4 && node_numbers[3] == 0)
                    {
                        dicNodes.TryGetValue(node_numbers[0], out int node0);
                        dicNodes.TryGetValue(node_numbers[1], out int node1);
                        dicNodes.TryGetValue(node_numbers[2], out int node2);
                        feMesh.Faces.AddFace(node0, node1, node2);
                    }
                    else
                    {
                        msg.Add($"Element {element.No} not imported.");
                    }
                }
                // Add mesh to tree
                var gh_path = new GH_Path(no);
                feMeshes.Add(feMesh, gh_path);
            }
            return feMeshes;
        }

        private DataTree<Vector3d> GetMeshDisplacements(ref List<string> msg)
        {
            var oDisplacements = new DataTree<Vector3d>();
            // Save defoirmation vectors into a tree
            var surfaceResults = _lcresults.GetSurfacesDeformations(false).OrderBy(o => o.LocationNo); // Sort according to nodes so there are no errors when applying displacements
            foreach (var result in surfaceResults)
            {
                var gh_path = new GH_Path(result.SurfaceNo, (int)result.Type);
                var displacement = new Vector3d(result.Displacements.ToPoint3d());
                oDisplacements.Add(displacement, gh_path);
            }
            return oDisplacements;
        }

        private DataTree<Mesh> GetDeformedMeshes(double scale, ref List<string> msg)
        {
            var oMeshes = new DataTree<Mesh>();
            // Same tree structure as displacements
            foreach (var path in _meshdisplacements.Paths)
            {
                var gh_path = new GH_Path(path);
                var mesh_path = new GH_Path(path.Indices[0]);
                // Get fe mesh as starting mesh
                var mesh = _feMeshes.Branch(mesh_path)[0].DuplicateMesh();
                // Move FE Nodes according to displacements
                for (int i = 0; i < mesh.Vertices.Count; i++)
                {
                    mesh.Vertices[i] += (Point3f)(Point3d)(scale * _meshdisplacements.Branch(path)[i]); // -_-
                }
                // Add mesh to tree
                oMeshes.Add(mesh, gh_path);
            }
            return oMeshes;
        }

        //private DataTree<Point3d> CreateControlPoints(ref List<string> msg)
        //{
        //    var oControlPoints = new DataTree<Point3d>();
        //    foreach (var member in _saveddata.GetMembers())
        //    {
        //        var gh_path = new GH_Path(member.No);
        //        var fe1delements = _rfemMesh.GetMemberNodes(member.No, ItemAt.AtNo); // We can't sort this list
        //        foreach (var node in fe1delements)
        //        {
        //            oControlPoints.Add(new Point3d(node.X, node.Y, node.Z), gh_path);
        //        }
        //    }            
        //    return oControlPoints;
        //}

        private DataTree<Vector3d> GetMemberDisplacements(ref List<string> msg)
        {
            // Get control points
            _controlPoints.Clear();
            var rfmembers = Component_GetData.GetRFMembers(_saveddata.GetMembers().ToList(), _saveddata);
            // Save defoirmation vectors into a tree;
            var oDisplacements = new DataTree<Vector3d>();
            foreach (var member in rfmembers)
            {
                // Add also control points. We are just going to get one set of control points for each curve regardless thne result type                
                var pts_path = new GH_Path(member.No);
                _controlPoints.RemovePath(pts_path);
                var baseline = member.BaseLine.ToCurve();
                // Get deformations
                var memberResults = _lcresults.GetMemberDeformations(member.No, ItemAt.AtNo, MemberAxesType.GlobalAxes); // We can't sort this list                
                var valueType = memberResults[0].Type; // Get deformation types to avoid duplicate control points
                foreach (var result in memberResults)
                {
                    var gh_path = new GH_Path(member.No, (int)result.Type);
                    var displacement = new Vector3d(result.Displacements.ToPoint3d());
                    oDisplacements.Add(displacement, gh_path);
                    // Get control points
                    if (result.Type == valueType)
                    {
                        _controlPoints.Add(baseline.PointAtNormalizedLength(Math.Min(result.Location / baseline.GetLength(), 1.0)), pts_path);
                    }                    
                }               
            }
            return oDisplacements;
        }

        private DataTree<Curve> GetDeformedMembers(double scale, ref List<string> msg)
        {
            var oCurves = new DataTree<Curve>();
            // Same tree structure as displacements
            foreach (var path in _memberdisplacements.Paths)
            {
                var gh_path = new GH_Path(path);
                var member_path = new GH_Path(path.Indices[0]);
                // Get deformed control points
                var ctrlPoints = _controlPoints.Branch(member_path);
                var deformations = _memberdisplacements.Branch(path);
                var deformedPoints = new List<Point3d>();
                for (int i = 0; i < ctrlPoints.Count; i++)
                {
                    deformedPoints.Add(new Point3d(ctrlPoints[i] + scale * deformations[i]));
                }
                // Add curve to tree
                var memberShape = Curve.CreateControlPointCurve(deformedPoints);
                oCurves.Add(memberShape, gh_path);
            }
            return oCurves;
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
                //return Resources.IconForThisComponent;
                return Properties.Resources.icon_GetResults;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("801735dc-9b7c-4e04-a14a-34b865f689a2"); }
        }
    }
}
