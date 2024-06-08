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

namespace Parametric_FEM_Toolbox.GUI
{
    public class Component_GetData_GUI_OBSOLETE_4 : GH_SwitcherComponent
    {
        // Declare class variables outside the method "SolveInstance" so their values persist 
        // when the method is called again.
        List<RFNode> rfNodes = new List<RFNode>();
        List<RFLine> rfLines = new List<RFLine>();
        List<RFMember> rfMembers = new List<RFMember>();
        List<RFSurface> rfSurfaces = new List<RFSurface>();
        List<RFOpening> rfOpenings = new List<RFOpening>();
        List<RFSupportP> rfSupportsP = new List<RFSupportP>();
        List<RFSupportL> rfSupportsL = new List<RFSupportL>();
        List<RFSupportS> rfSupportsS = new List<RFSupportS>();
        List<RFLineHinge> rfLineHinges = new List<RFLineHinge>();
        List<RFCroSec> rfCroSecs = new List<RFCroSec>();
        List<RFMaterial> rfMaterials = new List<RFMaterial>();
        List<RFNodalLoad> rfNodalLoads = new List<RFNodalLoad>();
        List<RFLineLoad> rfLineLoads = new List<RFLineLoad>();
        List<RFMemberLoad> rfMemberLoads = new List<RFMemberLoad>();
        List<RFSurfaceLoad> rfSurfaceLoads = new List<RFSurfaceLoad>();
        List<RFFreePolygonLoad> rfPolyLoads = new List<RFFreePolygonLoad>();
        List<RFLoadCase> rfLoadCases = new List<RFLoadCase>();
        List<RFLoadCombo> rfLoadCombos = new List<RFLoadCombo>();
        List<RFResultCombo> rfResultCombos = new List<RFResultCombo>();

        int modelDataCount = 0;
        int modelDataCount1 = 0;
        int modelDataCount2 = 0;
        int modelDataCount3 = 0;

        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public Component_GetData_GUI_OBSOLETE_4()
          : base("Get Data", "Get Data", "Gets Data from the RFEM Model.", "B+G Toolbox", "RFEM")
        {
        }

        // Define Keywords to search for this Component more easily in Grasshopper
        public override IEnumerable<string> Keywords => new string[] {"rf", "get", "data"};

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Use the pManager object to register your input parameters.
            //pManager.AddBooleanParameter("Nodes in RFEM Model", "Nodes", "Nodes to get from the RFEM Model.", GH_ParamAccess.item, false);
            //pManager.AddBooleanParameter("Lines in RFEM Model", "Lines", "Lines to get from the RFEM Model.", GH_ParamAccess.item, false);
            //pManager.AddBooleanParameter("Members in RFEM Model", "Members", "Members to get from the RFEM Model.", GH_ParamAccess.item, false);
            //pManager.AddBooleanParameter("Surfaces in RFEM Model", "Surfaces", "Surfaces to get from the RFEM Model.", GH_ParamAccess.item, false);
            //pManager.AddBooleanParameter("Openings in RFEM Model", "Openings", "Openings to get from the RFEM Model.", GH_ParamAccess.item, false);
            //pManager.AddBooleanParameter("Nodal Supports in RFEM Model", "NodSup", "Nodal Supports to get from the RFEM Model.", GH_ParamAccess.item, false);
            //pManager.AddBooleanParameter("Line Supports in RFEM Model", "LineSup", "Line Supports to get from the RFEM Model.", GH_ParamAccess.item, false);
            //pManager.AddBooleanParameter("Line Hinges in RFEM Model", "LineHinges", "Line Hinges to get from the RFEM Model.", GH_ParamAccess.item, false);
            //pManager.AddBooleanParameter("Cross Sections in RFEM Model", "CroSecs", "Cross Sections to get from the RFEM Model.", GH_ParamAccess.item, false);
            //pManager.AddBooleanParameter("Materials in RFEM Model", "Materials", "Materials to get from the RFEM Model.", GH_ParamAccess.item, false);
            //pManager.AddParameter(new Param_Filter_GUI(), "Filter", "Filter", "Filter RFEM Objects", GH_ParamAccess.list);
            //pManager[5].Optional = true;
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
            //pManager.RegisterParam(new Param_RFEM(), "Nodes", "Nodes", "Nodes from the RFEM Model.", GH_ParamAccess.list);
            //pManager.RegisterParam(new Param_RFEM(), "Lines", "Lines", "Lines from the RFEM Model.", GH_ParamAccess.list);
            //pManager.RegisterParam(new Param_RFEM(), "Members", "Members", "Members from the RFEM Model.", GH_ParamAccess.list);
            //pManager.RegisterParam(new Param_RFEM(), "Surfaces", "Surfaces", "Surfaces from the RFEM Model.", GH_ParamAccess.list);
            //pManager.RegisterParam(new Param_RFEM(), "Openings", "Openings", "Openings from the RFEM Model.", GH_ParamAccess.list);
            //pManager.RegisterParam(new Param_RFEM(), "Nodal Supports", "NodSup", "Nodal Supports from the RFEM Model.", GH_ParamAccess.list);
            //pManager.RegisterParam(new Param_RFEM(), "Line Supports", "LineSup", "Line Supports from the RFEM Model.", GH_ParamAccess.list);
            //pManager.RegisterParam(new Param_RFEM(), "Line Hinges", "LHinges", "Line Hinges from the RFEM Model.", GH_ParamAccess.list);
            //pManager.RegisterParam(new Param_RFEM(), "Cross Sections", "CroSec", "Cross Sections from the RFEM Model.", GH_ParamAccess.list);
            //pManager.RegisterParam(new Param_RFEM(), "Materials", "Mat", "Materials from the RFEM Model.", GH_ParamAccess.list);
            // Sometimes you want to hide a specific parameter from the Rhino preview.
            // You can use the HideParameter() method as a quick way:
            // pManager.HideParameter(0);
        }

        protected override void RegisterEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit("Get Data", "Get Data", "Gets Data from the RFEM Model.", Properties.Resources.icon_GetData);
            mngr.RegisterUnit(evaluationUnit);

            // Model Data

            evaluationUnit.RegisterInputParam(new Param_Boolean(), "Nodes", "Nodes", "Nodes to get from the RFEM Model.", GH_ParamAccess.item, new GH_Boolean(false));
            evaluationUnit.Inputs[0].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Boolean(), "Lines", "Lines", "Lines to get from the RFEM Model.", GH_ParamAccess.item, new GH_Boolean(false));
            evaluationUnit.Inputs[1].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Boolean(), "Members", "Members", "Members to get from the RFEM Model.", GH_ParamAccess.item, new GH_Boolean(false));
            evaluationUnit.Inputs[2].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Boolean(), "Surfaces", "Surfaces", "Surfaces to get from the RFEM Model.", GH_ParamAccess.item, new GH_Boolean(false));
            evaluationUnit.Inputs[3].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Boolean(), "Openings", "Openings", "Openings to get from the RFEM Model.", GH_ParamAccess.item, new GH_Boolean(false));
            evaluationUnit.Inputs[4].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Boolean(), "Nodal Supports", "NodSup", "Nodal Supports to get from the RFEM Model.", GH_ParamAccess.item, new GH_Boolean(false));
            evaluationUnit.Inputs[5].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Boolean(), "Line Supports", "LineSup", "Line Supports to get from the RFEM Model.", GH_ParamAccess.item, new GH_Boolean(false));
            evaluationUnit.Inputs[6].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Boolean(), "Surface Supports", "SrfcSup", "Surface Supports to get from the RFEM Model.", GH_ParamAccess.item, new GH_Boolean(false));
            evaluationUnit.Inputs[7].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Boolean(), "Line Hinges", "LineHinges", "Line Hinges to get from the RFEM Model.", GH_ParamAccess.item, new GH_Boolean(false));
            evaluationUnit.Inputs[8].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Boolean(), "Cross Sections", "CroSecs", "Cross Sections from the RFEM Model.", GH_ParamAccess.item, new GH_Boolean(false));
            evaluationUnit.Inputs[9].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Boolean(), "Materials", "Mat", "Materials to get from the RFEM Model.", GH_ParamAccess.item, new GH_Boolean(false));
            evaluationUnit.Inputs[10].Parameter.Optional = true;

            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Nodes", "RF Nodes", "Nodes from the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Lines", "RF Lines", "Lines from the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Members", "RF Members", "Members from the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Surfaces", "RF Surfaces", "Surfaces from the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Openings", "RF Openings", "Openings from the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Nodal Supports", "RF NodSup", "Nodal Supports from the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Line Supports", "RF LineSup", "Line Supports from the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Surface Supports", "RF SrfcSup", "Surface Supports from the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Line Hinges", "RF LineHinges", "Line Hinges from the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Cross Sections", "RF CroSecs", "Cross Sections to get from the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Materials", "RF Mat", "Materials from the RFEM Model.");

            modelDataCount = evaluationUnit.Inputs.Count;

            GH_ExtendableMenu gH_ExtendableMenu0 = new GH_ExtendableMenu(0, "Model Data");
            gH_ExtendableMenu0.Name = "Model Data";
            gH_ExtendableMenu0.Expand();
            evaluationUnit.AddMenu(gH_ExtendableMenu0);
            for (int i = 0; i < modelDataCount; i++)
            {
                gH_ExtendableMenu0.RegisterInputPlug(evaluationUnit.Inputs[i]);
                gH_ExtendableMenu0.RegisterOutputPlug(evaluationUnit.Outputs[i]);
            }


            // Load Data

            evaluationUnit.RegisterInputParam(new Param_Boolean(), "Nodal Loads", "NLoads", "Nodal Loads from the RFEM Model.", GH_ParamAccess.item, new GH_Boolean(false));
            evaluationUnit.Inputs[modelDataCount + 0].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Boolean(), "Line Loads", "LLoads", "Line Loads from the RFEM Model.", GH_ParamAccess.item, new GH_Boolean(false));
            evaluationUnit.Inputs[modelDataCount+ 1].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Boolean(), "Member Loads", "MLoads", "Member Loads from the RFEM Model.", GH_ParamAccess.item, new GH_Boolean(false));
            evaluationUnit.Inputs[modelDataCount + 2].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Boolean(), "Surface Loads", "SLoads", "Surface Loads from the RFEM Model.", GH_ParamAccess.item, new GH_Boolean(false));
            evaluationUnit.Inputs[modelDataCount + 3].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Boolean(), "Free Polygon Loads", "PolyLoads", "Free Polygon Loads from the RFEM Model.", GH_ParamAccess.item, new GH_Boolean(false));
            evaluationUnit.Inputs[modelDataCount + 4].Parameter.Optional = true;

            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Nodal Loads", "RF NLoads", "Nodal Loads from the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Line Loads", "RF LLoads", "Line Loads from the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Member Loads", "RF MLoads", "Member Loads from the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Surface Loads", "RF SLoads", "Surface Loads from the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Free Polygon Loads", "RF PolyLoads", "Free Polygon Loads from the RFEM Model.");

            modelDataCount2 = evaluationUnit.Inputs.Count;

            GH_ExtendableMenu gH_ExtendableMenu1 = new GH_ExtendableMenu(1, "Load Data");
            gH_ExtendableMenu1.Name = "Load Data";
            gH_ExtendableMenu1.Collapse();
            evaluationUnit.AddMenu(gH_ExtendableMenu1);
            for (int i = modelDataCount; i < modelDataCount2; i++)
            {
                gH_ExtendableMenu1.RegisterInputPlug(evaluationUnit.Inputs[i]);
                gH_ExtendableMenu1.RegisterOutputPlug(evaluationUnit.Outputs[i]);
            }

            // Load Cases and Combos

            evaluationUnit.RegisterInputParam(new Param_Boolean(), "Load Cases", "LCases", "Load Cases from the RFEM Model.", GH_ParamAccess.item, new GH_Boolean(false));
            evaluationUnit.Inputs[modelDataCount2 + 0].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Boolean(), "Load Combos", "LCombos", "Load Combinations from the RFEM Model.", GH_ParamAccess.item, new GH_Boolean(false));
            evaluationUnit.Inputs[modelDataCount2 + 1].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Boolean(), "Result Combos", "RCombos", "Result Combinations from the RFEM Model.", GH_ParamAccess.item, new GH_Boolean(false));
            evaluationUnit.Inputs[modelDataCount2 + 2].Parameter.Optional = true;

            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Load Cases", "RF LCases", "Load Cases from the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Load Combos", "RF LCombos", "Load Combinations from the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Result Combos", "RF RCombos", "Result Combinations from the RFEM Model.");

            modelDataCount3 = evaluationUnit.Inputs.Count;

            GH_ExtendableMenu gH_ExtendableMenu2 = new GH_ExtendableMenu(2, "Load Cases and Combos");
            gH_ExtendableMenu2.Name = "Load Cases and Combos";
            gH_ExtendableMenu2.Collapse();
            evaluationUnit.AddMenu(gH_ExtendableMenu2);
            for (int i = modelDataCount2; i < modelDataCount3; i++)
            {
                gH_ExtendableMenu2.RegisterInputPlug(evaluationUnit.Inputs[i]);
                gH_ExtendableMenu2.RegisterOutputPlug(evaluationUnit.Outputs[i]);
            }

            // Advanced

            evaluationUnit.RegisterInputParam(new Param_Filter(), "Filter", "Filter", "Filter RFEM Objects", GH_ParamAccess.list);
            evaluationUnit.Inputs[modelDataCount3].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_String(), "Model Name", "Model Name", "Segment of the name of the RFEM Model to get information from", GH_ParamAccess.item);
            evaluationUnit.Inputs[modelDataCount3 + 1].Parameter.Optional = true;

            GH_ExtendableMenu gH_ExtendableMenu3 = new GH_ExtendableMenu(3, "Advanced");
            gH_ExtendableMenu3.Name = "Advanced";
            gH_ExtendableMenu3.Collapse();
            evaluationUnit.AddMenu(gH_ExtendableMenu3);
            for (int i = modelDataCount3; i < modelDataCount3 + 2; i++)
            {
                gH_ExtendableMenu3.RegisterInputPlug(evaluationUnit.Inputs[i]);
            }       
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA, EvaluationUnit unit)
        {
            // Input counter
            modelDataCount1 = 1 + modelDataCount2;

            // RFEM variables
            var modelName = "";
            IModel model = null;
            IModelData data = null;
            ILoads loads = null;

            // Output message
            var msg = new List<string>();

            // Assign GH Input
            bool getnodes = false;
            bool getlines = false;
            bool getmembers = false;
            bool getsurfaces = false;
            bool getopenings = false;
            bool getsupportsP = false;
            bool getsupportsL = false;
            bool getsupportsS = false;
            bool getLineHinges = false;
            bool getCroSecs = false;
            bool getMaterials = false;
            bool getNodalLoads = false;
            bool getLineLoads = false;
            bool getMemberLoads = false;
            bool getSurfaceLoads = false;
            bool getPolyLoads = false;
            bool getLoadCases = false;
            bool getLoadCombos = false;
            bool getResultCombos = false;
            bool run = false;
            var ghFilters = new List<GH_RFFilter>();
            var inFilters = new List<RFFilter>();
            DA.GetData(0, ref run);
            DA.GetData(1, ref getnodes);
            DA.GetData(2, ref getlines);
            DA.GetData(3, ref getmembers);
            DA.GetData(4, ref getsurfaces);
            DA.GetData(5, ref getopenings);
            DA.GetData(6, ref getsupportsP);
            DA.GetData(7, ref getsupportsL);
            DA.GetData(8, ref getsupportsS);
            DA.GetData(9, ref getLineHinges);
            DA.GetData(10, ref getCroSecs);
            DA.GetData(11, ref getMaterials);
            DA.GetData(12, ref getNodalLoads);
            DA.GetData(13, ref getLineLoads);
            DA.GetData(14, ref getMemberLoads);
            DA.GetData(15, ref getSurfaceLoads);
            DA.GetData(16, ref getPolyLoads);
            DA.GetData(17, ref getLoadCases);
            DA.GetData(18, ref getLoadCombos);
            DA.GetData(19, ref getResultCombos);
            if (DA.GetDataList(modelDataCount3+1, ghFilters))
            {
                inFilters = ghFilters.Select(x => x.Value).ToList();
            }

            // Do stuff
            if (run)
            {
                if (!DA.GetData(modelDataCount3+2, ref modelName))
                {
                    Component_GetData.ConnectRFEM(ref model, ref data);
                }else
                {
                    Component_GetData.ConnectRFEM(modelName, ref model, ref data);
                }                
                Component_GetData.ClearOutput(ref rfNodes, ref rfLines, ref rfMembers, ref rfSurfaces, ref rfOpenings,
                    ref rfSupportsP, ref rfSupportsL, ref rfSupportsS, ref rfLineHinges, ref rfCroSecs, ref rfMaterials, ref rfNodalLoads,
                    ref rfLineLoads, ref rfMemberLoads, ref rfSurfaceLoads, ref rfPolyLoads, ref rfLoadCases, ref rfLoadCombos,
                    ref rfResultCombos);            

                  try
                {
                    if (getnodes)
                    {
                        var filNodes = Component_GetData.FilterNodes(data, inFilters);
                        rfNodes = Component_GetData.GetRFNodes(filNodes, data);
                    }
                    if (getlines)
                    {
                        var filLines = Component_GetData.FilterLines(data, inFilters);
                        rfLines = Component_GetData.GetRFLines(filLines, data);
                    }
                    if (getmembers)
                    {
                        var filMembers = Component_GetData.FilterMembers(data, inFilters);
                        rfMembers = Component_GetData.GetRFMembers(filMembers, data);
                    }
                    if (getsurfaces)
                    {
                        var filSrfcs = Component_GetData.FilterSurfaces(data, inFilters);
                        rfSurfaces = Component_GetData.GetRFSurfaces(filSrfcs, data);
                    }
                    if (getopenings)
                    {
                        var filOpenings = Component_GetData.FilterOpenings(data, inFilters);
                        rfOpenings = Component_GetData.GetRFOpenings(filOpenings,data);
                    }
                    if (getsupportsP)
                    {
                        var filSupportsP = Component_GetData.FilterSupsP(data, inFilters);
                        rfSupportsP = Component_GetData.GetRFSupportsP(filSupportsP, data);
                    }
                    if (getsupportsL)
                    {
                        var filSupportsL = Component_GetData.FilterSupsL(data, inFilters);
                        rfSupportsL = Component_GetData.GetRFSupportsL(filSupportsL, data);
                    }
                    if (getsupportsS)
                    {
                        var filSupportsS = Component_GetData.FilterSupsS(data, inFilters);
                        rfSupportsS = Component_GetData.GetRFSupportsS(filSupportsS, data);
                    }
                    if (getLineHinges)
                    {
                        var filLineHinges = Component_GetData.FilterLH(data, inFilters);
                        rfLineHinges = Component_GetData.GetRFLineHinges(filLineHinges, data);
                    }
                    if (getCroSecs)
                    {
                        var filCroSecs = Component_GetData.FilterCroSecs(data, inFilters);
                        rfCroSecs = Component_GetData.GetRFCroSecs(filCroSecs, model, ref msg);
                    }
                    if (getMaterials)
                    {
                        var filMaterials = Component_GetData.FilterMaterials(data, inFilters);
                        rfMaterials = Component_GetData.GetRFMaterials(filMaterials, data);
                    }
                    //Get Loads?
                    if (getNodalLoads || getLineLoads || getMemberLoads || getSurfaceLoads || getPolyLoads
                        || getLoadCases || getLoadCombos || getResultCombos)
                    {
                        Component_GetData.GetLoadsFromRFEM(model, ref loads);
                    }
                    if (getNodalLoads)
                    {
                        var filNodalLoads = Component_GetData.FilterNodalLoads(data, loads, inFilters);
                        rfNodalLoads = Component_GetData.GetRFNodalLoads(filNodalLoads, data);
                    }
                    if (getLineLoads)
                    {
                        var filLineLoads = Component_GetData.FilterLineLoads(data, loads, inFilters);
                        rfLineLoads = Component_GetData.GetRFLineLoads(filLineLoads, data);
                    }
                    if (getMemberLoads)
                    {
                        var filMemberLoads = Component_GetData.FilterMemberLoads(data, loads, inFilters);
                        rfMemberLoads = Component_GetData.GetRFMemberLoads(filMemberLoads, data);
                    }
                    if (getSurfaceLoads)
                    {
                        var filSurfaceLoads = Component_GetData.FilterSurfaceLoads(data, loads, inFilters);
                        rfSurfaceLoads = Component_GetData.GetRFSurfaceLoads(filSurfaceLoads, data);
                    }
                    if (getPolyLoads)
                    {
                        var filPolyLoads = Component_GetData.FilterPolyLoads(data, loads, inFilters);
                        rfPolyLoads = Component_GetData.GetRFPolyLoads(filPolyLoads, data);
                    }
                    if (getLoadCases)
                    {
                        var filLoadCases = Component_GetData.FilterLoadCases(data, loads, inFilters);
                        rfLoadCases = Component_GetData.GetRFLoadCases(filLoadCases, data);
                    }
                    if (getLoadCombos)
                    {
                        var filLoadCombos = Component_GetData.FilterLoadCombos(data, loads, inFilters);
                        rfLoadCombos = Component_GetData.GetRFLoadCombos(filLoadCombos, data);
                    }
                    if (getResultCombos)
                    {
                        var filResultombos = Component_GetData.FilterResultCombos(data, loads, inFilters);
                        rfResultCombos = Component_GetData.GetRFResultCombos(filResultombos, data);
                    }
                }
                catch (Exception ex)
                {
                    Component_GetData.ClearOutput(ref rfNodes, ref rfLines, ref rfMembers, ref rfSurfaces, ref rfOpenings,
                        ref rfSupportsP, ref rfSupportsL, ref rfSupportsS, ref rfLineHinges, ref rfCroSecs, ref rfMaterials, ref rfNodalLoads,
                        ref rfLineLoads, ref rfMemberLoads, ref rfSurfaceLoads, ref rfPolyLoads, ref rfLoadCases, ref rfLoadCombos,
                        ref rfResultCombos);
                    throw ex;
                }
                Component_GetData.DisconnectRFEM(ref model, ref data);
            }

            // Assign GH Output
            DA.SetDataList(0, rfNodes);
            DA.SetDataList(1, rfLines);
            DA.SetDataList(2, rfMembers);
            DA.SetDataList(3, rfSurfaces);
            DA.SetDataList(4, rfOpenings);
            DA.SetDataList(5, rfSupportsP);
            DA.SetDataList(6, rfSupportsL);
            DA.SetDataList(7, rfSupportsS);
            DA.SetDataList(8, rfLineHinges);
            DA.SetDataList(9, rfCroSecs);
            DA.SetDataList(10, rfMaterials);
            DA.SetDataList(11, rfNodalLoads);
            DA.SetDataList(12, rfLineLoads);
            DA.SetDataList(13, rfMemberLoads);
            DA.SetDataList(14, rfSurfaceLoads);
            DA.SetDataList(15, rfPolyLoads);
            DA.SetDataList(16, rfLoadCases);
            DA.SetDataList(17, rfLoadCombos);
            DA.SetDataList(18, rfResultCombos);

            if (msg.Count != 0)
            {
                //errorMsg.Add("List item index may be one unit lower than object number");
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, String.Join(System.Environment.NewLine, msg.ToArray()));
            }
        }

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
                return Properties.Resources.icon_GetData;                
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("6fee913e-a534-405c-b71e-a4e86433d0bf"); }
        }
    }
}
