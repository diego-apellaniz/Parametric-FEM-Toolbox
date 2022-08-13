using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using Parametric_FEM_Toolbox.UIWidgets;
using Parametric_FEM_Toolbox.Utilities;
using Parametric_FEM_Toolbox.HelperLibraries;
using Dlubal.RFEM5;
using Parametric_FEM_Toolbox.RFEM;
using System.Linq;

namespace Parametric_FEM_Toolbox.GUI
{
    public class SubComponent_ExtrudeMembers_NURBS_GUI : SubComponent
    {
        // Global variables
        public GH_Document Parent = null;
        private MenuSlider _nFacSlider;
        double length_segment = 1.0;

        public override string name()
        {
            return "NURBS";
        }
        public override string display_name()
        {
            return "NURBS";
        }
        public override void registerEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Extrude Members as NURBS.");
            evaluationUnit.Icon = Properties.Resources.Extrude_Members;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }


        protected void Setup(EvaluationUnit unit)
        {
            string str = "\ndouble-click on the knob to set the value range.";
            string str2 = " the number of mesh faces over a beams cross section that are used for rendering.";
            MenuPanel menupanel = new MenuPanel(0, "render_panel");            
            GH_ExtendableMenu gh_extendablemenu = new GH_ExtendableMenu(1, "render_menu");
            menupanel.ShowToolTip = true;
            gh_extendablemenu.Name = "render settings";
            gh_extendablemenu.Header = "Set properties to be rendered on the beams.";
            gh_extendablemenu.Expand();
            MenuStaticText menuStaticText = new MenuStaticText();
            menuStaticText.Text = "Length/Segment [m]";
            menuStaticText.Header = "Controls the length [m]" + " of segments of rendered beams.\n" + str;
            menupanel.AddControl(menuStaticText);
            _nFacSlider = new MenuSlider(1, "slider_nfac", 0.05, 5.0, 1.0, 2);
            _nFacSlider.ValueChanged += _nFacSlider__valueChanged;
            _nFacSlider.Header = "sets" + str2 + str;
            menupanel.AddControl(_nFacSlider);
            gh_extendablemenu.AddControl(menupanel);
            unit.AddMenu(gh_extendablemenu);

            unit.RegisterOutputParam(new Param_Brep(), "Extrussions", "E", "Extruded members.");
        }

        private void _nFacSlider__valueChanged(object sender, EventArgs e)
        {
            length_segment = ((MenuSlider)sender).Value;
            setModelProps();
        }

        public override void OnComponentLoaded()
        {
            base.OnComponentLoaded();
            length_segment = _nFacSlider.Value;
        }

        protected void setModelProps()
        {
            this.Parent_Component.ExpireSolution(true);
        }

        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Blank;

            OnComponentLoaded();

            // Input
            var inGH = new GH_RFEM();
            var inGH2 = new List<GH_RFEM>();
            var iMember = new RFMember();
            var iCroSecs = new List<RFCroSec>();

            // Output
            var oExtrussion = new List<Brep>();

            // Register input parameters
            if (!DA.GetData(0, ref inGH)) return;
            iMember = (RFMember)inGH.Value;
            if (!DA.GetDataList(1, inGH2)) return;
            foreach (var cs in inGH2)
            {
                iCroSecs.Add((RFCroSec)cs.Value);
            }

            oExtrussion = Component_ExtrudeMembers.ExtrudeMembersToBrep(iMember, iCroSecs, length_segment, out msg);
            if (msg != "")
            {
                level = GH_RuntimeMessageLevel.Warning;
                return;
            }

            // Assign output
            DA.SetDataList(0, oExtrussion);
        }
    }
}