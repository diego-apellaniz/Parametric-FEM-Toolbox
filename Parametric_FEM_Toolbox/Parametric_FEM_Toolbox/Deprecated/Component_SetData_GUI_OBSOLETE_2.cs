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
    public class Component_SetData_GUI_OBSOLETE_2 : GH_SwitcherComponent
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
        List<RFLineHinge> lineHingeNo = new List<RFLineHinge>();
        List<RFCroSec> croSecNo = new List<RFCroSec>();
        List<RFMaterial> matNo = new List<RFMaterial>();
        List<RFNodalLoad> nodalLoadNo = new List<RFNodalLoad>();

        int modelDataCount = 0;
        int modelDataCount2 = 0;

        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public Component_SetData_GUI_OBSOLETE_2()
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
            pManager.AddParameter(new Param_RFEM(), "Nodes", "Nodes", "Nodes to set in the RFEM Model.", GH_ParamAccess.list);
            pManager[0].Optional = true;
            pManager.AddParameter(new Param_RFEM(), "Lines", "Lines", "Lines to set in the RFEM Model.", GH_ParamAccess.list);
            pManager[1].Optional = true;
            pManager.AddParameter(new Param_RFEM(), "Members", "Members", "Members to set in the RFEM Model.", GH_ParamAccess.list);
            pManager[2].Optional = true;
            pManager.AddParameter(new Param_RFEM(), "Surfaces", "Sfcs", "Surfaces to set in the RFEM Model.", GH_ParamAccess.list);
            pManager[3].Optional = true;
            pManager.AddParameter(new Param_RFEM(), "Openings", "Ops", "Openings to set in the RFEM Model.", GH_ParamAccess.list);
            pManager[4].Optional = true;
            pManager.AddParameter(new Param_RFEM(), "Nodal Supports", "NodSup", "Nodal Supports to set in the RFEM Model.", GH_ParamAccess.list);
            pManager[5].Optional = true;
            pManager.AddParameter(new Param_RFEM(), "Line Supports", "LineSup", "Line Supports to set in the RFEM Model.", GH_ParamAccess.list);
            pManager[6].Optional = true;
            pManager.AddParameter(new Param_RFEM(), "Line Hinges", "LineHinge", "Line Hinges to set in the RFEM Model.", GH_ParamAccess.list);
            pManager[7].Optional = true;
            pManager.AddParameter(new Param_RFEM(), "Cross Sections", "CroSec", "Cross Sections to set in the RFEM Model.", GH_ParamAccess.list);
            pManager[8].Optional = true;
            pManager.AddParameter(new Param_RFEM(), "Materials", "Mat", "Materials to set in the RFEM Model.", GH_ParamAccess.list);
            pManager[9].Optional = true;
            pManager.AddBooleanParameter("Run component?", "Run", "If true, the programm is executed.", GH_ParamAccess.item, false);
            modelDataCount = pManager.ParamCount;
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
            pManager.AddParameter(new Param_RFEM(), "Nodes", "Nodes", "Nodes from the RFEM Model.", GH_ParamAccess.list);
            pManager.AddParameter(new Param_RFEM(), "Lines", "Lines", "Lines from the RFEM Model.", GH_ParamAccess.list);
            pManager.AddParameter(new Param_RFEM(), "Members", "Members", "Members from the RFEM Model.", GH_ParamAccess.list);
            pManager.AddParameter(new Param_RFEM(), "Surfaces", "Sfcs", "Surfaces from the RFEM Model.", GH_ParamAccess.list);
            pManager.AddParameter(new Param_RFEM(), "Openings", "Ops", "Openings from the RFEM Model.", GH_ParamAccess.list);
            pManager.AddParameter(new Param_RFEM(), "Nodal Supports", "NodSup", "Nodal Supports from in the RFEM Model.", GH_ParamAccess.list);
            pManager.AddParameter(new Param_RFEM(), "Line Supports", "LineSup", "Line Supports from in the RFEM Model.", GH_ParamAccess.list);
            pManager.AddParameter(new Param_RFEM(), "Line Hinges", "LineHinges", "Line Hinges from in the RFEM Model.", GH_ParamAccess.list);
            pManager.AddParameter(new Param_RFEM(), "Cross Sections", "CroSecs", "Cross Sections from in the RFEM Model.", GH_ParamAccess.list);
            pManager.AddParameter(new Param_RFEM(), "Material", "Mat", "Materials from the RFEM Model.", GH_ParamAccess.list);
            // Sometimes you want to hide a specific parameter from the Rhino preview.
            // You can use the HideParameter() method as a quick way:
            // pManager.HideParameter(0);
        }

        protected override void RegisterEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit("Set Data", "Set Data", "Sets Data from the RFEM Model.", Properties.Resources.icon_SetData);
            mngr.RegisterUnit(evaluationUnit);

            evaluationUnit.RegisterInputParam(new Param_RFEM(), "Nodal Loads", "NLoads", "Nodal Loads to set in the RFEM Model.", GH_ParamAccess.list);
            evaluationUnit.Inputs[0].Parameter.Optional = true;
            evaluationUnit.RegisterOutputParam(new Param_RFEM(), "Nodal Loads", "NLoads", "Nodal Loads from the RFEM Model.");


            modelDataCount2 = evaluationUnit.Inputs.Count;

            GH_ExtendableMenu gH_ExtendableMenu = new GH_ExtendableMenu(0, "Load Data");
            gH_ExtendableMenu.Name = "Load Data";
            gH_ExtendableMenu.Collapse();
            evaluationUnit.AddMenu(gH_ExtendableMenu);
            for (int i = 0; i < modelDataCount2; i++)
            {
                gH_ExtendableMenu.RegisterInputPlug(evaluationUnit.Inputs[i]);
                gH_ExtendableMenu.RegisterOutputPlug(evaluationUnit.Outputs[i]);
            }


            evaluationUnit.RegisterInputParam(new Param_String(), "Model Name", "Model Name", "Segment of the name of the RFEM Model to get information from", GH_ParamAccess.item);
            evaluationUnit.Inputs[modelDataCount2].Parameter.Optional = true;

            GH_ExtendableMenu gH_ExtendableMenu2 = new GH_ExtendableMenu(1, "advanced");
            gH_ExtendableMenu2.Name = "Advanced";
            gH_ExtendableMenu2.Collapse();
            evaluationUnit.AddMenu(gH_ExtendableMenu2);
            gH_ExtendableMenu2.RegisterInputPlug(evaluationUnit.Inputs[modelDataCount2]);
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
            var ghLHs = new List<GH_RFEM>();
            var ghCSs = new List<GH_RFEM>();
            var ghMats = new List<GH_RFEM>();

            var ghNodalLoads = new List<GH_RFEM>();

            var modelName = "";
            IModel model = null;
            IModelData data = null;
            ILoads loads = null;

            DA.GetData(modelDataCount - 1, ref run);
            if (run)
            {
                Component_SetData.ClearOutput(ref nodesNo, ref linesNo, ref membersNo, ref srfcNo, ref opNo, ref nodSupNo, ref lineSupNo,
                    ref lineHingeNo, ref croSecNo, ref matNo, ref nodalLoadNo);
                if (!DA.GetData(modelDataCount + modelDataCount2, ref modelName))
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
                    if (DA.GetDataList(9, ghMats))
                    {
                        data.SetRFMaterials(ghMats, ref matNo);
                    }
                    if (DA.GetDataList(8, ghCSs))
                    {
                        data.SetRFCroSecs(ghCSs, ref croSecNo);
                    }
                    if (DA.GetDataList(0, ghNodes))
                    {
                        data.SetRFNodes(ghNodes, ref nodesNo);
                    }
                    if (DA.GetDataList(1, ghLines))
                    {
                        data.SetRFLines(ghLines, ref linesNo);
                    }
                    if (DA.GetDataList(2, ghMembers))
                    {
                        data.SetRFMembers(ghMembers, ref membersNo);
                    }
                    if (DA.GetDataList(3, ghSfcs))
                    {
                        data.SetRFSurfaces(ghSfcs, ref srfcNo);
                    }
                    if (DA.GetDataList(4, ghOps))
                    {
                        data.SetRFOpenings(ghOps, ref opNo);
                    }
                    if (DA.GetDataList(5, ghSupsP))
                    {
                        data.SetRFSupportsP(ghSupsP, ref nodSupNo);
                    }
                    if (DA.GetDataList(6, ghSupsL))
                    {
                        data.SetRFSupportsL(ghSupsL, ref lineSupNo);
                    }
                    if (DA.GetDataList(7, ghLHs))
                    {
                        data.SetRFLineHinges(ghLHs, ref lineHingeNo);
                    }
                    data.FinishModification();
                    // Set Loads
                    if (DA.GetDataList(modelDataCount, ghNodalLoads))
                    {
                        Component_GetData.GetLoadsFromRFEM(ref model, ref loads);
                        data.SetRFNodalLoads(loads, ghNodalLoads, ref nodalLoadNo);
                    }
                }
                catch (Exception ex)
            {
                Component_SetData.ClearOutput(ref nodesNo, ref linesNo, ref membersNo, ref srfcNo, ref opNo, ref nodSupNo, ref lineSupNo,
                    ref lineHingeNo, ref croSecNo, ref matNo, ref nodalLoadNo);
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
            DA.SetDataList(7, lineHingeNo);
            DA.SetDataList(8, croSecNo);
            DA.SetDataList(9, matNo);
            DA.SetDataList(10, nodalLoadNo);
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
            get { return new Guid("a52adb84-451a-4c2d-81f7-30b8ddfa61d1"); }
        }
    }
}
