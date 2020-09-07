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
    public class Component_SetData_GUI : GH_SwitcherComponent
    {
        // Declare class variables outside the method "SolveInstance" so their values persist 
        // when the method is called again.
        List<RFNode> nodesNo = new List<RFNode>();
        List<RFLine> linesNo = new List<RFLine>();
        List<RFMember> membersNo = new List<RFMember>();
        List<RFSurface> srfcNo = new List<RFSurface>();
        List<RFOpening> opNo = new List<RFOpening>();
        List<RFSupportP> nodSupNo = new List<RFSupportP>();
        List<RFSupportL> lineSupNo = new List<RFSupportL>();
        List<RFSupportS> sfcSupNo = new List<RFSupportS>();
        List<RFLineHinge> lineHingeNo = new List<RFLineHinge>();
        List<RFCroSec> croSecNo = new List<RFCroSec>();
        List<RFMaterial> matNo = new List<RFMaterial>();
        List<RFNodalLoad> nodalLoadNo = new List<RFNodalLoad>();
        List<RFLineLoad> lineLoadNo = new List<RFLineLoad>();
        List<RFMemberLoad> memberLoadNo = new List<RFMemberLoad>();
        List<RFSurfaceLoad> surfaceLoadNo = new List<RFSurfaceLoad>();
        List<RFFreePolygonLoad> polyLoadNo = new List<RFFreePolygonLoad>();
        List<RFLoadCase> loadcaseNo = new List<RFLoadCase>();
        List<RFLoadCombo> loadcomboNo = new List<RFLoadCombo>();
        List<RFResultCombo> resultcomboNo = new List<RFResultCombo>();

        int modelDataCount = 0;
        int modelDataCount2 = 0;
        int modelDataCount3 = 0;

        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public Component_SetData_GUI()
          : base("Set Data", "Set Data", "Sets Data in the RFEM Model.", "B+G Toolbox", "RFEM")
        {
        }

        // Define Keywords to search for this Component more easily in Grasshopper
        public override IEnumerable<string> Keywords => new string[] {"rf", "set", "data"};

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Use the pManager object to register your input parameters.
            //pManager.AddParameter(new Param_RFEM(), "Nodes", "Nodes", "Nodes to set in the RFEM Model.", GH_ParamAccess.list);
            //pManager[0].Optional = true;
            //pManager.AddParameter(new Param_RFEM(), "Lines", "Lines", "Lines to set in the RFEM Model.", GH_ParamAccess.list);
            //pManager[1].Optional = true;
            //pManager.AddParameter(new Param_RFEM(), "Members", "Members", "Members to set in the RFEM Model.", GH_ParamAccess.list);
            //pManager[2].Optional = true;
            //pManager.AddParameter(new Param_RFEM(), "Surfaces", "Sfcs", "Surfaces to set in the RFEM Model.", GH_ParamAccess.list);
            //pManager[3].Optional = true;
            //pManager.AddParameter(new Param_RFEM(), "Openings", "Ops", "Openings to set in the RFEM Model.", GH_ParamAccess.list);
            //pManager[4].Optional = true;
            //pManager.AddParameter(new Param_RFEM(), "Nodal Supports", "NodSup", "Nodal Supports to set in the RFEM Model.", GH_ParamAccess.list);
            //pManager[5].Optional = true;
            //pManager.AddParameter(new Param_RFEM(), "Line Supports", "LineSup", "Line Supports to set in the RFEM Model.", GH_ParamAccess.list);
            //pManager[6].Optional = true;
            //pManager.AddParameter(new Param_RFEM(), "Line Hinges", "LineHinge", "Line Hinges to set in the RFEM Model.", GH_ParamAccess.list);
            //pManager[7].Optional = true;
            //pManager.AddParameter(new Param_RFEM(), "Cross Sections", "CroSec", "Cross Sections to set in the RFEM Model.", GH_ParamAccess.list);
            //pManager[8].Optional = true;
            //pManager.AddParameter(new Param_RFEM(), "Materials", "Mat", "Materials to set in the RFEM Model.", GH_ParamAccess.list);
            //pManager[9].Optional = true;
            pManager.AddBooleanParameter("Run component?", "Run", "If true, the programm is executed.", GH_ParamAccess.item, false);
            // modelDataCount = pManager.ParamCount;
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
            //pManager.AddParameter(new Param_RFEM(), "Nodes", "Nodes", "Nodes from the RFEM Model.", GH_ParamAccess.list);
            //pManager.AddParameter(new Param_RFEM(), "Lines", "Lines", "Lines from the RFEM Model.", GH_ParamAccess.list);
            //pManager.AddParameter(new Param_RFEM(), "Members", "Members", "Members from the RFEM Model.", GH_ParamAccess.list);
            //pManager.AddParameter(new Param_RFEM(), "Surfaces", "Sfcs", "Surfaces from the RFEM Model.", GH_ParamAccess.list);
            //pManager.AddParameter(new Param_RFEM(), "Openings", "Ops", "Openings from the RFEM Model.", GH_ParamAccess.list);
            //pManager.AddParameter(new Param_RFEM(), "Nodal Supports", "NodSup", "Nodal Supports from in the RFEM Model.", GH_ParamAccess.list);
            //pManager.AddParameter(new Param_RFEM(), "Line Supports", "LineSup", "Line Supports from in the RFEM Model.", GH_ParamAccess.list);
            //pManager.AddParameter(new Param_RFEM(), "Line Hinges", "LineHinges", "Line Hinges from in the RFEM Model.", GH_ParamAccess.list);
            //pManager.AddParameter(new Param_RFEM(), "Cross Sections", "CroSecs", "Cross Sections from in the RFEM Model.", GH_ParamAccess.list);
            //pManager.AddParameter(new Param_RFEM(), "Material", "Mat", "Materials from the RFEM Model.", GH_ParamAccess.list);
            // Sometimes you want to hide a specific parameter from the Rhino preview.
            // You can use the HideParameter() method as a quick way:
            // pManager.HideParameter(0);
        }

        protected override void RegisterEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit("Set Data", "Set Data", "Sets Data from the RFEM Model.", Properties.Resources.icon_SetData);
            mngr.RegisterUnit(evaluationUnit);

            // Model Data

            evaluationUnit.RegisterInputParam(new Param_RFEM(), "RF Nodes", "RF Nodes", "Nodes to set in the RFEM Model.", GH_ParamAccess.list);
            evaluationUnit.Inputs[0].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_RFEM(), "RF Lines", "RF Lines", "Lines to set in the RFEM Model.", GH_ParamAccess.list);
            evaluationUnit.Inputs[1].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_RFEM(), "RF Members", "RF Members", "Members to set in the RFEM Model.", GH_ParamAccess.list);
            evaluationUnit.Inputs[2].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_RFEM(), "RF Surfaces", "RF Surfaces", "Surfaces to set in the RFEM Model.", GH_ParamAccess.list);
            evaluationUnit.Inputs[3].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_RFEM(), "RF Openings", "RF Openings", "Openings to set in the RFEM Model.", GH_ParamAccess.list);
            evaluationUnit.Inputs[4].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_RFEM(), "RF Nodal Supports", "RF NodSup", "Nodal Supports to set in the RFEM Model.", GH_ParamAccess.list);
            evaluationUnit.Inputs[5].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_RFEM(), "RF Line Supports", "RF LineSup", "Line Supports to set in the RFEM Model.", GH_ParamAccess.list);
            evaluationUnit.Inputs[6].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_RFEM(), "RF Surface Supports", "RF SfcSup", "Surface Supports to set in the RFEM Model.", GH_ParamAccess.list);
            evaluationUnit.Inputs[7].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_RFEM(), "RF Line Hinges", "RF LineHinges", "Line Hinges from in the RFEM Model.", GH_ParamAccess.list);
            evaluationUnit.Inputs[8].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_RFEM(), "RF Cross Sections", "RF CroSecs", "Cross Sections from in the RFEM Model.", GH_ParamAccess.list);
            evaluationUnit.Inputs[9].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_RFEM(), "RF Materials", "RF Mat", "Materials to set in the RFEM Model.", GH_ParamAccess.list);
            evaluationUnit.Inputs[10].Parameter.Optional = true;

            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Nodes", "RF Nodes", "Nodes from the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Lines", "RF Lines", "Lines from the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Members", "RF Members", "Members from the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Surfaces", "RF Surfaces", "Surfaces from the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Openings", "RF Openings", "Openings from the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Nodal Supports", "RF NodSup", "Nodal Supports from in the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Line Supports", "RF LineSup", "Line Supports from in the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Surface Supports", "RF SrfcSup", "Surface Supports from in the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Line Hinges", "RF LineHinges", "Line Hinges from in the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Cross Sections", "RF CroSecs", "Cross Sections from in the RFEM Model.");
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

            evaluationUnit.RegisterInputParam(new Param_RFEM(), "RF Nodal Loads", "RF NLoads", "Nodal Loads to set in the RFEM Model.", GH_ParamAccess.list);
            evaluationUnit.Inputs[modelDataCount + 0].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_RFEM(), "RF Line Loads", "RF LLoads", "Line Loads to set in the RFEM Model.", GH_ParamAccess.list);
            evaluationUnit.Inputs[modelDataCount + 1].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_RFEM(), "RF Member Loads", "RF MLoads", "Member Loads to set in the RFEM Model.", GH_ParamAccess.list);
            evaluationUnit.Inputs[modelDataCount + 2].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_RFEM(), "RF Surface Loads", "RF SLoads", "Surface Loads to set in the RFEM Model.", GH_ParamAccess.list);
            evaluationUnit.Inputs[modelDataCount + 3].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_RFEM(), "RF Free Polygon Loads", "RF PolyLoads", "Free Polygon Loads to set in the RFEM Model.", GH_ParamAccess.list);
            evaluationUnit.Inputs[modelDataCount + 4].Parameter.Optional = true;

            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Nodal Loads", "RF NLoads", "Nodal Loads from the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Line Loads", "RF LLoads", "Line Loads from the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Member Loads", "RF MLoads", "Member Loads from the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Surface Loads", "RF SLoads", "Surface Loads from the RFEM Model.");
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "RF Free Polygon Loads", "RF PolyLoads", "Free Polygon Loads to set in the RFEM Model.");

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

            evaluationUnit.RegisterInputParam(new Param_RFEM(), "RF Load Cases", "RF LCases", "Load Cases to set in the RFEM Model.", GH_ParamAccess.list);
            evaluationUnit.Inputs[modelDataCount2 + 0].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_RFEM(), "RF Load Combos", "RF LCombos", "Load Combinations to set in the RFEM Model.", GH_ParamAccess.list);
            evaluationUnit.Inputs[modelDataCount2 + 1].Parameter.Optional = true;
            evaluationUnit.RegisterInputParam(new Param_RFEM(), "RF Result Combos", "RF RCombos", "Result Combinations to set in the RFEM Model.", GH_ParamAccess.list);
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

            evaluationUnit.RegisterInputParam(new Param_String(), "Model Name", "Model Name", "Segment of the name of the RFEM Model to set data in", GH_ParamAccess.item);
            evaluationUnit.Inputs[modelDataCount3].Parameter.Optional = true;

            GH_ExtendableMenu gH_ExtendableMenu3 = new GH_ExtendableMenu(3, "Advanced");
            gH_ExtendableMenu3.Name = "Advanced";
            gH_ExtendableMenu3.Collapse();
            evaluationUnit.AddMenu(gH_ExtendableMenu3);
            gH_ExtendableMenu3.RegisterInputPlug(evaluationUnit.Inputs[modelDataCount3]);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA, EvaluationUnit unit)
        {
            var run = false;

            var ghNodes = new List<GH_RFEM>();
            var ghLines = new List<GH_RFEM>();
            var ghMembers = new List<GH_RFEM>();
            var ghSfcs = new List<GH_RFEM>();
            var ghOps = new List<GH_RFEM>();
            var ghSupsP = new List<GH_RFEM>();
            var ghSupsL = new List<GH_RFEM>();
            var ghSupsS = new List<GH_RFEM>();
            var ghLHs = new List<GH_RFEM>();
            var ghCSs = new List<GH_RFEM>();
            var ghMats = new List<GH_RFEM>();

            var ghNodalLoads = new List<GH_RFEM>();
            var ghLineLoads = new List<GH_RFEM>();
            var ghMemberLoads = new List<GH_RFEM>();
            var ghSurfaceLoads = new List<GH_RFEM>();
            var ghPolyLoads = new List<GH_RFEM>();

            var ghLoadCases = new List<GH_RFEM>();
            var ghLoadCombos = new List<GH_RFEM>();
            var ghResultCombos = new List<GH_RFEM>();

            var modelName = "";
            IModel model = null;
            IModelData data = null;
            ILoads loads = null;

            var errorMsg = new List<string>();

            DA.GetData(0, ref run);
            if (run)
            {
                Component_SetData.ClearOutput(ref nodesNo, ref linesNo, ref membersNo, ref srfcNo, ref opNo, ref nodSupNo, ref lineSupNo, ref sfcSupNo,
                    ref lineHingeNo, ref croSecNo, ref matNo, ref nodalLoadNo, ref lineLoadNo, ref memberLoadNo, ref surfaceLoadNo,
                    ref polyLoadNo, ref loadcaseNo, ref loadcomboNo, ref resultcomboNo);
                if (!DA.GetData(modelDataCount3+1, ref modelName))
                {
                    Component_GetData.ConnectRFEM(ref model, ref data);
                }else
                {
                    Component_GetData.ConnectRFEM(modelName, ref model, ref data);
                }
                try
                {
                    // Set data
                    data.PrepareModification();
                    if (DA.GetDataList(11, ghMats))
                    {
                        data.SetRFMaterials(ghMats, ref matNo, ref errorMsg);
                    }
                    if (DA.GetDataList(10, ghCSs))
                    {
                        data.SetRFCroSecs(ghCSs, ref croSecNo, ref errorMsg);
                    }
                    if (DA.GetDataList(1, ghNodes))
                    {
                        data.SetRFNodes(ghNodes, ref nodesNo, ref errorMsg);
                    }
                    if (DA.GetDataList(2, ghLines))
                    {
                        data.SetRFLines(ghLines, ref linesNo, ref errorMsg);
                    }
                    if (DA.GetDataList(3, ghMembers))
                    {
                        data.SetRFMembers(ghMembers, ref membersNo, ref errorMsg);
                    }
                    if (DA.GetDataList(4, ghSfcs))
                    {
                        data.SetRFSurfaces(ghSfcs, ref srfcNo, ref errorMsg);
                    }
                    if (DA.GetDataList(5, ghOps))
                    {
                        data.SetRFOpenings(ghOps, ref opNo, ref errorMsg);
                    }
                    if (DA.GetDataList(6, ghSupsP))
                    {
                        data.SetRFSupportsP(ghSupsP, ref nodSupNo, ref errorMsg);
                    }
                    if (DA.GetDataList(7, ghSupsL))
                    {
                        data.SetRFSupportsL(ghSupsL, ref lineSupNo, ref errorMsg);
                    }
                    if (DA.GetDataList(8, ghSupsS))
                    {
                        data.SetRFSupportsS(ghSupsS, ref sfcSupNo, ref errorMsg);
                    }
                    if (DA.GetDataList(9, ghLHs))
                    {
                        data.SetRFLineHinges(ghLHs, ref lineHingeNo, ref errorMsg);
                    }
                    data.FinishModification();
                    // Set Load Cases and Combos
                    if (DA.GetDataList(modelDataCount2 + 1, ghLoadCases))
                    {
                        if (loads == null)
                        {
                            Component_GetData.GetLoadsFromRFEM(model, ref loads);
                        }
                        data.SetRFLoadCases(loads, ghLoadCases, ref loadcaseNo, ref errorMsg);
                    }
                    if (DA.GetDataList(modelDataCount2 + 2, ghLoadCombos))
                    {
                        if (loads == null)
                        {
                            Component_GetData.GetLoadsFromRFEM(model, ref loads);
                        }
                        data.SetRFLoadCombos(loads, ghLoadCombos, ref loadcomboNo, ref errorMsg);
                    }
                    if (DA.GetDataList(modelDataCount2 + 3, ghResultCombos))
                    {
                        if (loads == null)
                        {
                            Component_GetData.GetLoadsFromRFEM(model, ref loads);
                        }
                        data.SetRFResultCombos(loads, ghLoadCases, ref resultcomboNo, ref errorMsg);
                    }
                    // Set Loads
                    if (DA.GetDataList(modelDataCount + 1, ghNodalLoads))
                    {
                        if (loads == null)
                        {
                            Component_GetData.GetLoadsFromRFEM(model, ref loads);
                        }
                        data.SetRFNodalLoads(loads, ghNodalLoads, ref nodalLoadNo, ref errorMsg);
                    }
                    if (DA.GetDataList(modelDataCount+2, ghLineLoads))
                    {
                        if (loads == null)
                        {
                            Component_GetData.GetLoadsFromRFEM(model, ref loads);
                        }                        
                        data.SetRFLineLoads(loads, ghLineLoads, ref lineLoadNo, ref errorMsg);
                    }
                    if (DA.GetDataList(modelDataCount + 3, ghMemberLoads))
                    {
                        if (loads == null)
                        {
                            Component_GetData.GetLoadsFromRFEM(model, ref loads);
                        }
                        data.SetRFMemberLoads(loads, ghMemberLoads, ref memberLoadNo, ref errorMsg);
                    }
                    if (DA.GetDataList(modelDataCount + 4, ghSurfaceLoads))
                    {
                        if (loads == null)
                        {
                            Component_GetData.GetLoadsFromRFEM(model, ref loads);
                        }
                        data.SetRFSurfaceLoads(loads, ghSurfaceLoads, ref surfaceLoadNo, ref errorMsg);
                    }
                    if (DA.GetDataList(modelDataCount + 5, ghPolyLoads))
                    {
                        if (loads == null)
                        {
                            Component_GetData.GetLoadsFromRFEM(model, ref loads);
                        }
                        data.SetRFFreePolygonLoads(loads, ghPolyLoads, ref polyLoadNo, ref errorMsg);
                    }
                }
                catch (Exception ex)
                {
                    Component_SetData.ClearOutput(ref nodesNo, ref linesNo, ref membersNo, ref srfcNo, ref opNo, ref nodSupNo, ref lineSupNo, ref sfcSupNo,
                        ref lineHingeNo, ref croSecNo, ref matNo, ref nodalLoadNo, ref lineLoadNo, ref memberLoadNo, ref surfaceLoadNo,
                        ref polyLoadNo, ref loadcaseNo, ref loadcomboNo, ref resultcomboNo);
                    Component_GetData.DisconnectRFEM(ref model, ref data);
                    throw ex;
                }
            Component_GetData.DisconnectRFEM(ref model, ref data);
            }
            // Assign Output
            DA.SetDataList(0, nodesNo);
            DA.SetDataList(1, linesNo);
            DA.SetDataList(2, membersNo);
            DA.SetDataList(3, srfcNo);
            DA.SetDataList(4, opNo);
            DA.SetDataList(5, nodSupNo);
            DA.SetDataList(6, lineSupNo);
            DA.SetDataList(7, sfcSupNo);
            DA.SetDataList(8, lineHingeNo);
            DA.SetDataList(9, croSecNo);
            DA.SetDataList(10, matNo);
            DA.SetDataList(11, nodalLoadNo);
            DA.SetDataList(12, lineLoadNo);
            DA.SetDataList(13, memberLoadNo);
            DA.SetDataList(14, surfaceLoadNo);
            DA.SetDataList(15, polyLoadNo);
            DA.SetDataList(16, loadcaseNo);
            DA.SetDataList(17, loadcomboNo);
            DA.SetDataList(18, resultcomboNo);

            if (errorMsg.Count != 0)
            {
                //errorMsg.Add("List item index may be one unit lower than object number");
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, String.Join(System.Environment.NewLine, errorMsg.ToArray()));
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
                return Properties.Resources.icon_SetData;                
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("b4d8e545-4331-419b-b6e2-362a308557f2"); }
        }
    }
}
