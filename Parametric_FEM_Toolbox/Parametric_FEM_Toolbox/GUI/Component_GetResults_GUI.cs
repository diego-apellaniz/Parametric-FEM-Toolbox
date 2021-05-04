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
        List<RFMember> _rfMembers = new List<RFMember>(); // Calculation results
        DataTree<Mesh> _feMeshes = new DataTree<Mesh>(); // Output FE Meshes 
        DataTree<Vector3d> _meshdisplacements = new DataTree<Vector3d>(); // -> The teee path refers to the surface number that originated that mesh
        DataTree<Vector3d> _meshdisplacementsByType = new DataTree<Vector3d>();
        List<Mesh> _deformedMeshes = new List<Mesh>();
        List<int> _sfcNo = new List<int>();
        DataTree<Point3d> _controlPoints = new DataTree<Point3d>(); // For member displacements
        DataTree<Vector3d> _memberdisplacements = new DataTree<Vector3d>();
        DataTree<Vector3d> _memberdisplacementsByType = new DataTree<Vector3d>();
        List<Curve> _deformedMembers = new List<Curve>();
        List<int> _memberNo = new List<int>();

        //private List<string> _loadCases = new List<string>();
        //private List<string> _loadCombos = new List<string>();
        //private List<string> _resultCombos = new List<string>();
        private bool _resetLC = false; // Get displacements again
        private bool _resetResultType = false; // Get displacements again
        private List<string> _lCasesAndCombos = new List<string>();
        private int _countCases = 0;
        private int _countCombos = 0;
        private int _countRcombos = 0;
        private List<ResultsValueType> _resultTypes = new List<ResultsValueType>();
        ICalculation _results = null;
        IFeMesh _rfemMesh = null;
        IResults _lcresults = null;
        IModelData _saveddata = null;
        RFResults outResults = null;
        //NodalDeformations[] _deformations = null;

        List<string> msg = new List<string>();

        //int modelDataCount = 0;
        //int modelDataCount1 = 0;
        //int modelDataCount2 = 0;
        //int modelDataCount3 = 0;

        private MenuDropDown _loadDrop;
        private MenuDropDown _resulttypeDrop;
        private int _loadDropLastValue;
        private int _resultDropLastValue;
        private string _lastLoadCase;
        private int _lastType;
        private bool _restoreDropDown;
        private MenuCheckBox _memberForcesCheck;
        private MenuCheckBox _surfaceForcesCheck;
        private MenuCheckBox _nodalReactionsCheck;

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
            pManager.AddParameter(new Param_RFEM(), "Calculation Results", "Results", "Calculation results of the specified load case or load combination.", GH_ParamAccess.item);

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
            evaluationUnit.RegisterOutputParam(new Param_Integer(), "Member No", "No", "Member Number related to deformed curve.");
            evaluationUnit.RegisterOutputParam(new Param_Mesh(), "Msh", "Meshes", "Deformed shape of the surface elemnents of the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_Integer(), "Surface No", "No", "Surface Number related to deformed mesh.");

            GH_ExtendableMenu gH_ExtendableMenu0 = new GH_ExtendableMenu(0, "Deformation");
            gH_ExtendableMenu0.Name = "Deformation";
            gH_ExtendableMenu0.Expand();
            evaluationUnit.AddMenu(gH_ExtendableMenu0);
            gH_ExtendableMenu0.RegisterInputPlug(evaluationUnit.Inputs[0]);
            for (int i = 0; i < 4; i++)
            {
                gH_ExtendableMenu0.RegisterOutputPlug(evaluationUnit.Outputs[i]);
            }

            // Load Cases and Combos
            GH_ExtendableMenu gH_ExtendableMenu1 = new GH_ExtendableMenu(1, "Load Cases and Combos");
            gH_ExtendableMenu1.Name = "Load Cases and Combos";
            gH_ExtendableMenu1.Expand();
            evaluationUnit.AddMenu(gH_ExtendableMenu1);
            MenuPanel menuPanel = new MenuPanel(1, "panel_load");
            menuPanel.Header = "Set here the load case for display.\n";                     
            MenuStaticText menuStaticText0 = new MenuStaticText();
            menuStaticText0.Text = "Select Load Case or Combo";
            menuStaticText0.Header = "Load Case";
            menuPanel.AddControl(menuStaticText0);
            _loadDrop = new MenuDropDown(1, "dropdown_loads_1", "loading type");
            _loadDrop.ValueChanged += _loadDrop__valueChanged;
            _loadDrop.Header = "Set here the loading type for display.\n";
            menuPanel.AddControl(_loadDrop);
            MenuStaticText menuStaticText1 = new MenuStaticText();
            menuStaticText1.Text = "Select Result Type";
            menuStaticText1.Header = "Result Type";
            menuPanel.AddControl(menuStaticText1);
            _resulttypeDrop = new MenuDropDown(2, "dropdown_result_1", "result type");
            _resulttypeDrop.ValueChanged += _loadDrop__valueChanged2;
            _resulttypeDrop.Header = "Set here the loading type for display.\n";
            menuPanel.AddControl(_resulttypeDrop);
            gH_ExtendableMenu1.AddControl(menuPanel);

            // Overwrite
            GH_ExtendableMenu gH_ExtendableMenu2 = new GH_ExtendableMenu(2, "Overwrite");
            gH_ExtendableMenu2.Name = "Overwrite";            
            evaluationUnit.RegisterInputParam(new Param_String(), "Overwrite Load Case or Combo", "Load Case", "Overwrite selected load case or combo from the dropdown menu.", GH_ParamAccess.item);
            evaluationUnit.Inputs[1].Parameter.Optional = true;
            gH_ExtendableMenu2.RegisterInputPlug(evaluationUnit.Inputs[1]);
            evaluationUnit.RegisterInputParam(new Param_Integer(), "Overwrite Result type", "Result Type", UtilLibrary.DescriptionRFTypes(typeof(ResultsValueType)), GH_ParamAccess.item);
            evaluationUnit.Inputs[2].Parameter.Optional = true;
            evaluationUnit.Inputs[2].EnumInput = UtilLibrary.ListRFTypes(typeof(ResultsValueType));
            gH_ExtendableMenu2.RegisterInputPlug(evaluationUnit.Inputs[2]);
            evaluationUnit.AddMenu(gH_ExtendableMenu2);          

            // Select results
            GH_ExtendableMenu gH_ExtendableMenu3 = new GH_ExtendableMenu(3, "Select Results");
            gH_ExtendableMenu3.Name = "Select Results";
            evaluationUnit.AddMenu(gH_ExtendableMenu3);
            MenuPanel menuPanel2 = new MenuPanel(2, "panel_results");
            menuPanel2.Header = "Select output results.\n";
            _memberForcesCheck = new MenuCheckBox(0, "check member forces", "Member Forces");
            _memberForcesCheck.ValueChanged += _memberForcesCheck__valueChanged;
            _memberForcesCheck.Active = true;
            _memberForcesCheck.Header = "Add member forces to output results.";
            _surfaceForcesCheck = new MenuCheckBox(1, "check surface forces", "Surface Forces");
            _surfaceForcesCheck.ValueChanged += _surfaceForcesCheck__valueChanged;
            _surfaceForcesCheck.Active = true;
            _surfaceForcesCheck.Header = "Add surface forces to output results.";
            _nodalReactionsCheck = new MenuCheckBox(2, "check nodal reactions", "Nodal Reactions");
            _nodalReactionsCheck.ValueChanged += _nodalReactionsCheck__valueChanged;
            _nodalReactionsCheck.Active = true;
            _nodalReactionsCheck.Header = "Add nodal reactions to output results.";
            menuPanel2.AddControl(_memberForcesCheck);
            menuPanel2.AddControl(_surfaceForcesCheck);
            menuPanel2.AddControl(_nodalReactionsCheck);
            gH_ExtendableMenu3.AddControl(menuPanel2);

            // Advanced
            evaluationUnit.RegisterInputParam(new Param_RFEM(), "Trigger", "Trigger", "Input to trigger the optimization", GH_ParamAccess.tree);
            evaluationUnit.Inputs[3].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_String(), "Model Name", "Model Name", "Segment of the name of the RFEM Model to get information from", GH_ParamAccess.item);
            evaluationUnit.Inputs[4].Parameter.Optional = true;
            GH_ExtendableMenu gH_ExtendableMenu4 = new GH_ExtendableMenu(4, "Advanced");
            gH_ExtendableMenu4.Name = "Advanced";
            gH_ExtendableMenu4.Collapse();
            evaluationUnit.AddMenu(gH_ExtendableMenu4);
            for (int i = 3; i < 3 + 2; i++)
            {
                gH_ExtendableMenu4.RegisterInputPlug(evaluationUnit.Inputs[i]);
            }
        }

        protected override void OnComponentLoaded()
        {
            _loadDrop.Clear();
            _resulttypeDrop.Clear();
            //updateDropDownMenu(_lCasesAndCombos);
        }

        protected override void OnComponentReset(GH_ExtendableComponentAttributes attr)
        {
            _loadDrop.Clear();
            _resulttypeDrop.Clear();
            //updateDropDownMenu(_lCasesAndCombos);
        }

        private void _loadDrop__valueChanged(object sender, EventArgs e)
        {
            _ = (MenuDropDown)sender;
            _resetLC = true; // Get deformations
            _loadDropLastValue = _loadDrop.Value;
            setModelProps();
        }

        private void _loadDrop__valueChanged2(object sender, EventArgs e)
        {
            _ = (MenuDropDown)sender;
            _resetResultType = true;
            _resultDropLastValue = _resulttypeDrop.Value;
            setModelProps();
        }

        private void _memberForcesCheck__valueChanged(object sender, EventArgs e)
        {
            _ = ((MenuCheckBox)sender).Active;
            _resetLC = true; // Get results
            setModelProps();
        }

        private void _surfaceForcesCheck__valueChanged(object sender, EventArgs e)
        {
            _ = ((MenuCheckBox)sender).Active;
            _resetLC = true; // Get results
            setModelProps();
        }

        private void _nodalReactionsCheck__valueChanged(object sender, EventArgs e)
        {
            _ = ((MenuCheckBox)sender).Active;
            _resetLC = true; // Get results
            setModelProps();
        }

        protected void setModelProps()
        {
            ((GH_DocumentObject)this).ExpireSolution(true);
        }

        private void updateDropDownMenu(List<string> _lcs)
        {
            _loadDrop.VisibleItemCount = Math.Min(10, _lcs.Count); // Maximum visible items = 10
            // Save menu value if runing analysis again
            //int value = 0;
            //if (_lcs.Count == _loadDrop.Items.Count)
            //{
            //    value = _loadDrop.Value;
            //}
            _loadDrop.Clear();
            if (_lcs.Count > 0)
            {
                for (int i = 0; i < _lcs.Count; i++)
                {
                    _loadDrop.AddItem(_lcs[i], _lcs[i]);
                }
                //if (_loadDrop.Items.Count > value)
                //{
                //    _loadDrop.Value = value;
                //}
            }
            _loadDrop.Value = _loadDropLastValue;
        }

        private void updateDropDownMenu2(List<ResultsValueType> _results, ref int result_type)
        {
            _resulttypeDrop.VisibleItemCount = Math.Min(10, _results.Count); // Maximum visible items = 10
            //int value = 0;
            //if (_results.Count == _resulttypeDrop.Items.Count)
            //{
            //    value = _resulttypeDrop.Value;
            //}
            _resulttypeDrop.Clear();
            if (_results.Count > 0)
            {
                for (int i = 0; i < _results.Count; i++)
                {
                    _resulttypeDrop.AddItem(((int)_results[i]).ToString(), _results[i].ToString());
                }
                //if (_resulttypeDrop.Items.Count > value)
                //{
                //    _resulttypeDrop.Value = value;
                //}
            }
            _resulttypeDrop.Value = _resultDropLastValue;
            result_type = Int32.Parse(_resulttypeDrop.Items[_resulttypeDrop.Value].name);
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
            ILoads loads = null;            

            // Assign GH Input
            bool run = false;
            var scale = 0.0;
            string iLoadCase = "";
            DA.GetData(0, ref run);
            DA.GetData(1, ref scale);

            // Do stuff
            if (run)
            {
                msg = new List<string>();

                if (!DA.GetData(5, ref modelName))
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
                    // Update load cases and combos to display in dropdown menu
                    _results = model.GetCalculation();

                    // Get load cases and combos to 
                    var newLoadCasesAndCombos = new List<string>();
                    newLoadCasesAndCombos = loads.GetLoadCasesAndCombos(ref _countCases, ref _countCombos, ref _countRcombos);
                    if (_lCasesAndCombos == null || _lCasesAndCombos.Count == 0 || !_lCasesAndCombos.Equals(newLoadCasesAndCombos))
                    {
                        _lCasesAndCombos = newLoadCasesAndCombos;
                        _loadDropLastValue = 0; // reset dropdown menus if run
                        _resultDropLastValue = 0;
                        _lastLoadCase = "";
                        _lastType = 0;
                        _restoreDropDown = true;
                    }
                    // Get members
                    _rfMembers = Component_GetData.GetRFMembers(data.GetMembers().ToList(), data);
                    // Get Fe Meshes from RFEM
                    _rfemMesh = _results.GetFeMesh();
                    _feMeshes = CreateFEMeshes(ref msg);
                 
                    // Disconnect RFEM
                    Component_GetData.DisconnectRFEM(ref model, ref data);

                    // Results are displayed just when the button is set to 0 - if no LC is provided
                    var dummyLC = "";
                    if (!DA.GetData(2, ref dummyLC))
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Set run to false to display results");
                        return;
                    }                        
                }
                catch (Exception ex)
                {
                    Component_GetData.DisconnectRFEM(ref model, ref data);
                    // Clear output!!!
                    _saveddata = null;
                    _rfemMesh = null;
                    _results = null;
                    _lcresults = null;
                    outResults = null;
                    _feMeshes.Clear();
                    _meshdisplacements.Clear();
                    _meshdisplacementsByType.Clear();
                    _deformedMeshes.Clear();
                    _controlPoints.Clear();
                    _memberdisplacements.Clear();
                    _memberdisplacementsByType.Clear();
                    _deformedMembers.Clear();
                    _sfcNo.Clear();
                    _memberNo.Clear();
                    throw ex;
                }
            }

           // do stuff
            if (msg.Count == 0) // if there are no calculation errors
            {
                // Get Load Case Number and result type                
                int result_type = 0;
                string result_type_name = "";
                if (DA.GetData(2, ref iLoadCase))
                {
                    if (DA.GetData(3, ref result_type))
                    {
                        if (result_type <= 0 || result_type > Enum.GetNames(typeof(ResultsValueType)).Length - 1)
                        {
                            _loadDrop.Clear();
                            _resulttypeDrop.Clear();
                            _restoreDropDown = true; // for next time
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Provide Valid Result Type.");
                            return;
                        }
                        result_type_name = Enum.GetName(typeof(ResultsValueType), result_type);
                        if (_lastLoadCase != iLoadCase || _lastType != result_type) // otherwise execution comes from change in scale and no need to reset load case
                        {
                            _resetLC = true;
                        }                        
                        _loadDrop.Clear();
                        _resulttypeDrop.Clear();
                        _restoreDropDown = true; // for next time
                    }
                    else
                    {
                        _loadDrop.Clear();
                        _resulttypeDrop.Clear();
                        _restoreDropDown = true; // for next time
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Provide Result Type.");
                        return;
                    }
                }
                else if (DA.GetData(3, ref result_type))
                {
                    _loadDrop.Clear();
                    _resulttypeDrop.Clear();
                    _restoreDropDown = true; // for next time
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Provide Load Case or Combo.");
                    return;
                }
                else // get values from dropdown menus
                {
                    if (_lCasesAndCombos.Count == 0)
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No results to display");
                        _loadDrop.Clear();
                        _resulttypeDrop.Clear();
                        return;
                    }
                    if (_restoreDropDown) // from previous execution when it was overwritten
                    {
                        updateDropDownMenu(_lCasesAndCombos);
                        _resetLC = true;
                        _restoreDropDown = false; // for next time
                    }
                    iLoadCase = _loadDrop.Items[_loadDrop.Value].name;
                }

                // Get results to display
                // 1) Results from load case or combo for all types 2) Results for selected result type 3) Apply scale
                if (_resetLC)
                {
                    int no = Int16.Parse(iLoadCase.Split(' ')[1]);
                    var value = _lCasesAndCombos.IndexOf(iLoadCase);
                    _loadDropLastValue = value;
                    _lastLoadCase = iLoadCase;
                    if (value == -1)
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, $"{iLoadCase} has no results. Provide valid load case.");
                        return;
                    }
                    var lcresultsraster = _results.GetResultsInRasterPoints(LoadingType.LoadCaseType, no);
                    if (value < _countCases)
                    {
                        _lcresults = _results.GetResultsInFeNodes(LoadingType.LoadCaseType, no);     
                        
                    }
                    else if (value < _countCases + _countCombos)
                    {
                        _lcresults = _results.GetResultsInFeNodes(LoadingType.LoadCombinationType, no);
                    }
                    else if (value < _countCases + _countCombos + _countRcombos)
                    {
                        _lcresults = _results.GetResultsInFeNodes(LoadingType.ResultCombinationType, no);
                    }
                    else
                    {
                        msg.Add("Load case or combo not found");
                        return;
                    }
                    // Update drop down menu of result types
                    _resultTypes = new List<ResultsValueType>();                    
                    // Get deformations
                    _meshdisplacements = GetMeshDisplacements(ref _sfcNo, ref msg);
                    _memberdisplacements = GetMemberDisplacements(ref _memberNo, ref msg);
                    //Get results by type
                    if (result_type == 0) // if no value obtaines through overwrite
                    {
                        _resultDropLastValue = 0;
                        updateDropDownMenu2(_resultTypes.Distinct().ToList(), ref result_type);
                    }
                    _meshdisplacementsByType = GetMeshDisplacementsByType(result_type);
                    _memberdisplacementsByType = GetMemberDisplacementsByType(result_type);
                    // Get analysis results
                    outResults = new RFResults(_lcresults, _saveddata, iLoadCase,
                        _memberForcesCheck.Active, _surfaceForcesCheck.Active, _nodalReactionsCheck.Active);
                    // Set _resetLC to false again
                    _resetLC = false;                    
                }
                if(_resetResultType) // when there are changes in drop down menu
                {
                    if (result_type == 0) // if no value obtaines through overwrite
                    {
                        result_type = Int32.Parse(_resulttypeDrop.Items[_resulttypeDrop.Value].name);
                    }
                    _meshdisplacementsByType = GetMeshDisplacementsByType(result_type);
                    _memberdisplacementsByType = GetMemberDisplacementsByType(result_type);
                    // Get analysis results
                    outResults = new RFResults(_lcresults, _saveddata, iLoadCase,
                        _memberForcesCheck.Active, _surfaceForcesCheck.Active, _nodalReactionsCheck.Active);
                    // Set _resetType to false again
                    _resetResultType = false;
                }
                _lastType = result_type;
                // Check results
                if (_meshdisplacements.DataCount>0 && _meshdisplacementsByType.DataCount == 0)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, $"No surface results found for result type {((ResultsValueType)result_type).ToString()}");
                }
                if (_memberdisplacements.DataCount > 0 && _memberdisplacementsByType.DataCount == 0)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, $"No member results found for result type {((ResultsValueType)result_type).ToString()}");
                }
                // Get output
                _deformedMeshes = GetDeformedMeshes(scale, ref msg);                
                _deformedMembers = GetDeformedMembers(scale, ref msg);

                // Output calculation results
                DA.SetData(0, outResults);

                // Assign GH Output
                DA.SetDataList(1, _deformedMembers);
                DA.SetDataList(2, _memberNo);
                DA.SetDataList(3, _deformedMeshes);
                DA.SetDataList(4, _sfcNo);
            }
            if (msg.Count > 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, String.Join(System.Environment.NewLine, msg.ToArray()));
            }
        }

        private DataTree<Mesh> CreateFEMeshes(ref List<string> msg)
        {
            var feMeshes = new DataTree<Mesh>();
            // Create fe mesh from each surface element
            var sfcs = _saveddata.GetSurfaces();
            sfcs.OrderBy(x => x.No); // order surfaces by number
            foreach (var sfc in sfcs)
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
                //// Add mesh to tree
                var gh_path = new GH_Path(no);
                feMeshes.Add(feMesh, gh_path);

                //feMeshes.Add(feMesh);
            }
            return feMeshes;
        }

        private DataTree<Vector3d> GetMeshDisplacements(ref List<int> sfcNo, ref List<string> msg)
        {
            var oDisplacements = new DataTree<Vector3d>();
            sfcNo = new List<int>();
            // Save defoirmation vectors into a tree
            var surfaceResults = _lcresults.GetSurfacesDeformations(false).OrderBy(o => o.LocationNo); // Sort according to nodes so there are no errors when applying displacements
            _resultTypes.AddRange(surfaceResults.Select(x => x.Type));
            foreach (var result in surfaceResults) // GET RESULT TYPES!!!
            {
                var gh_path = new GH_Path(result.SurfaceNo, (int)result.Type);
                var displacement = new Vector3d(result.Displacements.ToPoint3d());
                oDisplacements.Add(displacement, gh_path);
                // Add surface numbers to output list
                if(sfcNo.Count == 0 || result.SurfaceNo != sfcNo[sfcNo.Count-1])
                {
                    sfcNo.Add(result.SurfaceNo);
                }
                
            }
            return oDisplacements;
        }

        private DataTree<Vector3d> GetMeshDisplacementsByType(int result_type)
        {
            var oDisplacementsByType = new DataTree<Vector3d>();
            for (int i = 0; i < _meshdisplacements.Paths.Count; i++)
            {
                if (_meshdisplacements.Paths[i].Indices[1] == result_type)
                {
                    var oldPath = _meshdisplacements.Paths[i];
                    var gh_path = new GH_Path(oldPath.Indices[0]);
                    oDisplacementsByType.AddRange(_meshdisplacements.Branch(oldPath), gh_path);
                }
            }
            return oDisplacementsByType;
        }

        private List<Mesh> GetDeformedMeshes(double scale, ref List<string> msg)
        {
            var oMeshes = new List<Mesh>();
            // Same tree structure as displacements
            //foreach (var path in _meshdisplacements.Paths)
            for (int i = 0; i < _meshdisplacementsByType.Paths.Count; i++)
            {
                //var gh_path = new GH_Path(_meshdisplacements.Paths[i]);
                var mesh_path = new GH_Path(_meshdisplacementsByType.Paths[i].Indices[0]);
                // Get fe mesh as starting mesh
                var mesh = _feMeshes.Branch(mesh_path)[0].DuplicateMesh();
                // Move FE Nodes according to displacements
                for (int j = 0; j < mesh.Vertices.Count; j++)
                {
                    mesh.Vertices[j] += (Point3f)(Point3d)(scale * _meshdisplacementsByType.Branch(_meshdisplacementsByType.Paths[i])[j]); // -_-
                }
                // Add mesh to tree
                //oMeshes.Add(mesh, gh_path);
                oMeshes.Add(mesh);
            }
            return oMeshes;
        }

        private DataTree<Vector3d> GetMemberDisplacements(ref List<int> memberNo, ref List<string> msg)
        {
            // Get control points
            _controlPoints.Clear();
            // Save defoirmation vectors into a tree;
            var oDisplacements = new DataTree<Vector3d>();
            memberNo = new List<int>();
            foreach (var member in _rfMembers)
            {
                if (member.Type == MemberType.NullMember)
                {
                    continue;
                }
                // Add also control points. We are just going to get one set of control points for each curve regardless of the result type                
                var pts_path = new GH_Path(member.No);
                _controlPoints.EnsurePath(pts_path);
                var baseline = member.BaseLine.ToCurve(); 
                // Get deformations
                var memberResults = _lcresults.GetMemberDeformations(member.No, ItemAt.AtNo, MemberAxesType.GlobalAxes); // We can't sort this list      
                _resultTypes.AddRange(memberResults.Select(x => x.Type));
                var valueType = memberResults[0].Type; // Get deformation types to avoid duplicate control points 
                memberNo.Add(member.No);
                foreach (var result in memberResults)
                {
                    var gh_path = new GH_Path(member.No, (int)result.Type); // GET RESULT TYPES!!!                    
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

        private DataTree<Vector3d> GetMemberDisplacementsByType(int result_type)
        {
            var oDisplacementsByType = new DataTree<Vector3d>();
            for (int i = 0; i < _memberdisplacements.Paths.Count; i++)
            {
                if (_memberdisplacements.Paths[i].Indices[1] == result_type)
                {
                    var oldPath = _memberdisplacements.Paths[i];
                    var gh_path = new GH_Path(oldPath.Indices[0]);
                    oDisplacementsByType.AddRange(_memberdisplacements.Branch(oldPath), gh_path);
                }
            }
            return oDisplacementsByType;
        }

        private List<Curve> GetDeformedMembers(double scale, ref List<string> msg)
        {
            var oCurves = new List<Curve>();
            // Same tree structure as displacements
            foreach (var path in _memberdisplacementsByType.Paths)
            {
                var gh_path = new GH_Path(path);
                var pts_Path = new GH_Path(path.Indices[0]);
                // Get deformed control points
                var ctrlPoints = _controlPoints.Branch(pts_Path);
                var deformations = _memberdisplacementsByType.Branch(path);
                var deformedPoints = new List<Point3d>();
                for (int i = 0; i < ctrlPoints.Count; i++)
                {
                    deformedPoints.Add(new Point3d(ctrlPoints[i] + scale * deformations[i]));
                }
                // Add curve to tree
                var memberShape = Curve.CreateControlPointCurve(deformedPoints);
                //oCurves.Add(memberShape, gh_path);
                oCurves.Add(memberShape);
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
            get { return new Guid("a9cb88ef-e194-445f-876a-298a2dd7ba4f"); }
        }
    }
}
