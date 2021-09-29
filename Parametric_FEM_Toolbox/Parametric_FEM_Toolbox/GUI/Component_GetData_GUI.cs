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
    public class Component_GetData_GUI : GH_SwitcherComponent
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
        List<RFMemberHinge> rfMemberHinges = new List<RFMemberHinge>();
        List<RFMemberEccentricity> rfMemberEccentricities = new List<RFMemberEccentricity>();
        List<RFNodalRelease> rfNodalReleases = new List<RFNodalRelease>();
        List<RFCroSec> rfCroSecs = new List<RFCroSec>();
        List<RFMaterial> rfMaterials = new List<RFMaterial>();
        List<RFNodalLoad> rfNodalLoads = new List<RFNodalLoad>();
        List<RFLineLoad> rfLineLoads = new List<RFLineLoad>();
        List<RFMemberLoad> rfMemberLoads = new List<RFMemberLoad>();
        List<RFSurfaceLoad> rfSurfaceLoads = new List<RFSurfaceLoad>();
        List<RFFreePolygonLoad> rfPolyLoads = new List<RFFreePolygonLoad>();
        List<RFFreeLineLoad> rfFreeLineLoads = new List<RFFreeLineLoad>();
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
        public Component_GetData_GUI()
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
            pManager.AddBooleanParameter("Run component?", "Run", "If true, the programm is executed.", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
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
            evaluationUnit.RegisterInputParam(new Param_Boolean(), "Member Hinges", "MemberHinges", "Member Hinges to get from the RFEM Model.", GH_ParamAccess.item, new GH_Boolean(false));
            evaluationUnit.Inputs[8].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Boolean(), "Member Eccentricities", "MemberEcc", "Member Eccentricities to get from the RFEM Model.", GH_ParamAccess.item, new GH_Boolean(false));
            evaluationUnit.Inputs[9].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Boolean(), "Line Hinges", "LineHinges", "Line Hinges to get from the RFEM Model.", GH_ParamAccess.item, new GH_Boolean(false));
            evaluationUnit.Inputs[10].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Boolean(), "Nodal Releases", "NodalReleases", "Nodal Releases to get from the RFEM Model.", GH_ParamAccess.item, new GH_Boolean(false));
            evaluationUnit.Inputs[11].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Boolean(), "Cross Sections", "CroSecs", "Cross Sections from the RFEM Model.", GH_ParamAccess.item, new GH_Boolean(false));
            evaluationUnit.Inputs[12].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Boolean(), "Materials", "Mat", "Materials to get from the RFEM Model.", GH_ParamAccess.item, new GH_Boolean(false));
            evaluationUnit.Inputs[13].Parameter.Optional = true;

            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Nodes", "RF Nodes", "Nodes from the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Lines", "RF Lines", "Lines from the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Members", "RF Members", "Members from the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Surfaces", "RF Surfaces", "Surfaces from the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Openings", "RF Openings", "Openings from the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Nodal Supports", "RF NodSup", "Nodal Supports from the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Line Supports", "RF LineSup", "Line Supports from the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Surface Supports", "RF SrfcSup", "Surface Supports from the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Member Hinges", "RF MemberHinges", "Member Hinges from the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Member Eccentricities", "RF MemberEcc", "Member Hinges from the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Line Hinges", "RF LineHinges", "Line Hinges from the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "Nodal Releases", "NodalReleases", "Nodal Releases to get from the RFEM Model.");
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
            evaluationUnit.RegisterInputParam(new Param_Boolean(), "Free Line Loads", "FLLoads", "Free Line Loads from the RFEM Model.", GH_ParamAccess.item, new GH_Boolean(false));
            evaluationUnit.Inputs[modelDataCount + 4].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_Boolean(), "Free Polygon Loads", "PolyLoads", "Free Polygon Loads from the RFEM Model.", GH_ParamAccess.item, new GH_Boolean(false));
            evaluationUnit.Inputs[modelDataCount + 5].Parameter.Optional = true;

            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Nodal Loads", "RF NLoads", "Nodal Loads from the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Line Loads", "RF LLoads", "Line Loads from the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Member Loads", "RF MLoads", "Member Loads from the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Surface Loads", "RF SLoads", "Surface Loads from the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Free Line Loads", "RF FLLoads", "Free Line Loads from the RFEM Model.");
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
            bool getMemberHinges = false;
            bool getMemberEccentricities = false;
            bool getLineHinges = false;
            bool getNodalReleases = false;
            bool getCroSecs = false;
            bool getMaterials = false;
            bool getNodalLoads = false;
            bool getLineLoads = false;
            bool getMemberLoads = false;
            bool getSurfaceLoads = false;
            bool getFreeLineLoads = false;
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
            DA.GetData(9, ref getMemberHinges);
            DA.GetData(10, ref getMemberEccentricities);
            DA.GetData(11, ref getLineHinges);
            DA.GetData(12, ref getNodalReleases);
            DA.GetData(13, ref getCroSecs);
            DA.GetData(14, ref getMaterials);
            DA.GetData(15, ref getNodalLoads);
            DA.GetData(16, ref getLineLoads);
            DA.GetData(17, ref getMemberLoads);
            DA.GetData(18, ref getSurfaceLoads);
            DA.GetData(19, ref getFreeLineLoads);
            DA.GetData(20, ref getPolyLoads);
            DA.GetData(21, ref getLoadCases);
            DA.GetData(22, ref getLoadCombos);
            DA.GetData(23, ref getResultCombos);
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
                    ref rfResultCombos, ref rfMemberHinges, ref rfMemberEccentricities, ref rfNodalReleases, ref rfFreeLineLoads);

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
                    if (getMemberHinges)
                    {
                        var filMemberHinges = Component_GetData.FilterMH(data, inFilters);
                        rfMemberHinges = Component_GetData.GetRFMemberHinges(filMemberHinges, data);
                    }
                    if (getMemberEccentricities)
                    {
                        var filMemberEccentricities = Component_GetData.FilterMEcc(data, inFilters);
                        rfMemberEccentricities = Component_GetData.GetRFMemberEccentricities(filMemberEccentricities, data);
                    }
                    if (getLineHinges)
                    {
                        var filLineHinges = Component_GetData.FilterLH(data, inFilters);
                        rfLineHinges = Component_GetData.GetRFLineHinges(filLineHinges, data);
                    }
                    if (getNodalReleases)
                    {
                        var filNodalReleases = Component_GetData.FilterNR(data, inFilters);
                        rfNodalReleases = Component_GetData.GetRFNodalReleases(filNodalReleases, data);
                    }
                    if (getCroSecs)
                    {
                        var filCroSecs = Component_GetData.FilterCroSecs(data, inFilters);
                        //rfCroSecs = Component_GetData.GetRFCroSecs(filCroSecs, model, ref msg);
                        rfCroSecs = Component_GetData.GetRFCroSecs(filCroSecs, model, data, ref msg);
                    }
                    if (getMaterials)
                    {
                        var filMaterials = Component_GetData.FilterMaterials(data, inFilters);
                        rfMaterials = Component_GetData.GetRFMaterials(filMaterials, data);
                    }
                    //Get Loads?
                    if (getNodalLoads || getLineLoads || getMemberLoads || getSurfaceLoads || getPolyLoads
                        || getLoadCases || getLoadCombos || getResultCombos || getFreeLineLoads)
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
                    if (getFreeLineLoads)
                    {
                        var filLineLoads = Component_GetData.FilterFreeLineLoads(data, loads, inFilters);
                        rfFreeLineLoads = Component_GetData.GetRFFreeLineLoads(filLineLoads, data);
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
                    ref rfResultCombos, ref rfMemberHinges, ref rfMemberEccentricities, ref rfNodalReleases, ref rfFreeLineLoads);
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
            DA.SetDataList(8, rfMemberHinges);
            DA.SetDataList(9, rfMemberEccentricities);
            DA.SetDataList(10, rfLineHinges);
            DA.SetDataList(11, rfNodalReleases);
            DA.SetDataList(12, rfCroSecs);
            DA.SetDataList(13, rfMaterials);
            DA.SetDataList(14, rfNodalLoads);
            DA.SetDataList(15, rfLineLoads);
            DA.SetDataList(16, rfMemberLoads);
            DA.SetDataList(17, rfSurfaceLoads);
            DA.SetDataList(18, rfFreeLineLoads);
            DA.SetDataList(19, rfPolyLoads);
            DA.SetDataList(20, rfLoadCases);
            DA.SetDataList(21, rfLoadCombos);
            DA.SetDataList(22, rfResultCombos);

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
            get { return new Guid("80c63240-e0e6-4fee-b6a6-415c2b387864"); }
        }
    }
}
