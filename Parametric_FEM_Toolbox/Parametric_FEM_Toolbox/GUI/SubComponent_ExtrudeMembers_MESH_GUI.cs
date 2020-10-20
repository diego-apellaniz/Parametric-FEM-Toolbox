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
    public class SubComponent_ExtrudeMembers_MESH_GUI : SubComponent
    {
        // Global variables
        private MenuSlider _nFacSlider;
        private MenuSlider _nFacSlider2;
        double length_segment = 1.0;
        int nFaces = 1;

        public override string name()
        {
            return "MESH";
        }
        public override string display_name()
        {
            return "MESH";
        }
        public override void registerEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Extrude Members as Meshes.");
            evaluationUnit.Icon = Properties.Resources.Extrude_Members;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }


        protected void Setup(EvaluationUnit unit)
        {
            string str = "\ndouble-click on the knob to set the value range.";
            string str2 = " the number of mesh faces over a beam cross section that are used for rendering.";
            MenuPanel menupanel2 = new MenuPanel(0, "render_panel_2");
            GH_ExtendableMenu gh_extendablemenu2 = new GH_ExtendableMenu(1, "render_menu_2");
            menupanel2.ShowToolTip = true;
            gh_extendablemenu2.Name = "render settings";
            gh_extendablemenu2.Header = "Set properties to be rendered on the beams.";
            gh_extendablemenu2.Expand();
            MenuStaticText menuStaticText = new MenuStaticText();
            menuStaticText.Text = "Length/Segment [m]";
            menuStaticText.Header = "Controls the length [m]" + " of segments of rendered beams.\n" + str;
            menupanel2.AddControl(menuStaticText);
            _nFacSlider = new MenuSlider(0, "slider_nfac", 0.05, 5.0, 1.0, 2);
            _nFacSlider.ValueChanged += _nFacSlider__valueChanged;
            _nFacSlider.Header = "sets" + str2 + str;
            menupanel2.AddControl(_nFacSlider);
            MenuStaticText menuStaticText2 = new MenuStaticText();
            menuStaticText2.Text = "Faces/Cross section";
            menuStaticText2.Header = "Set the number of mesh faces for each beam side.\n" + str;
            menupanel2.AddControl(menuStaticText2);
            _nFacSlider2 = new MenuSlider(1, "slider_nfac2", 1, 30, 2, 0);
            _nFacSlider2.ValueChanged += _nFacSlider2__valueChanged;
            _nFacSlider2.Header = "sets" + str2 + str;
            menupanel2.AddControl(_nFacSlider2);
            gh_extendablemenu2.AddControl(menupanel2);
            unit.AddMenu(gh_extendablemenu2);

            unit.RegisterOutputParam(new Param_Mesh(), "Extrussions", "E", "Extruded members.");
        }

        private void _nFacSlider__valueChanged(object sender, EventArgs e)
        {
            length_segment = ((MenuSlider)sender).Value;
            setModelProps();
        }

        private void _nFacSlider2__valueChanged(object sender, EventArgs e)
        {
            nFaces = (int)((MenuSlider)sender).Value;
            setModelProps();
        }

        public override void OnComponentLoaded()
        {
            base.OnComponentLoaded();
            length_segment = _nFacSlider.Value;
            nFaces = (int)_nFacSlider2.Value;
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
            var oExtrussion = new List<Mesh>();

            // Register input parameters
            if (!DA.GetData(0, ref inGH)) return;
            iMember = (RFMember)inGH.Value;
            if (!DA.GetDataList(1, inGH2)) return;
            foreach (var cs in inGH2)
            {
                iCroSecs.Add((RFCroSec)cs.Value);
            }

            // Check input
            var cs_indeces = iCroSecs.Select(x => x.No);
            if (!(cs_indeces.Contains(iMember.StartCrossSectionNo)) || (!(cs_indeces.Contains(iMember.EndCrossSectionNo))))
            {
                level = GH_RuntimeMessageLevel.Warning;
                msg = $"Provide cross sections for member No {iMember.No}.";
                return;
            }

            // Get base geometry            
            var crosecs1 = iCroSecs.Where(x => x.No == iMember.StartCrossSectionNo).ToList()[0].Shape;
            var crosecs2 = iCroSecs.Where(x => x.No == iMember.EndCrossSectionNo).ToList()[0].Shape;
            var baseline = iMember.BaseLine.ToCurve();

            // Check geometry
            if ((crosecs1.Sum(x => x.SpanCount) != crosecs2.Sum(x => x.SpanCount)) || (crosecs1.Count != crosecs2.Count))
            {
                level = GH_RuntimeMessageLevel.Warning;
                msg = $"Provide similar cross sections for member No {iMember.No}.";
                return;
            }

            // Generate tween curves - still on the origin!
            List<Curve> segments;
            int nCroSecsInter = Math.Max((int)(baseline.GetLength() / length_segment), 2);
            var loft_crvs = Component_ExtrudeMembers.GenerateCroSecs(baseline, crosecs1, crosecs2, nCroSecsInter, 0.001, 0.001, out segments);

            // Orient cross sections
            loft_crvs = Component_ExtrudeMembers.OrientCroSecs(loft_crvs, segments, nCroSecsInter, iMember.Frames[0], crosecs1.Count);

            // Extrude members 
            for (int i = 0; i < loft_crvs.Count; i++)
            {
                for (int j = 0; j < loft_crvs[i].Count; j++)
                {
                    var beam_mesh = new Mesh(); // for each of the curves that make one cross section
                    for (int k = 0; k < loft_crvs[i][j].Count; k++)
                    {
                        var exploded_segments = new List<Curve>();
                        if (loft_crvs[i][j][k].SpanCount>1)
                        {
                            // exploded_segments = loft_crvs[i][j][k].DuplicateSegments().ToList();
                            var crv = loft_crvs[i][j][k].ToNurbsCurve();
                            // Get spilt parameters
                            var split_t = new List<double>();
                            for (int n = 0; n < crv.SpanCount; n++)
                            {
                                split_t.Add(crv.SpanDomain(n).T0);
                                //split_t.Add(crv.SpanDomain(n).T1);
                            }
                            split_t.Add(crv.SpanDomain(crv.SpanCount-1).T1);
                            //split_t = split_t.Distinct().ToList();
                            exploded_segments = crv.Split(split_t).ToList();
                        }
                        else
                        {
                            exploded_segments.Add(loft_crvs[i][j][k]);
                        }
                        //var real_segments = (exploded_segments.Where(x => x.GetLength()>0.001)).ToList();
                        var real_segments = exploded_segments;
                        var counter_nodes = loft_crvs[i][j][k].IsClosed ? real_segments.Count* nFaces : real_segments.Count * nFaces+1;
                        for (int n = 0; n < real_segments.Count; n++)
                        {
                            var domain = real_segments[n].Domain;
                            for (int m = 0; m < nFaces; m++)
                            {
                                // Add vertex
                                if (!(real_segments[n].IsClosed & m == nFaces-1 & n == real_segments.Count - 1)) // for closed section shapes ignore last point
                                {
                                    var t = (double)m / (double)nFaces * (domain.T1 - domain.T0) + domain.T0;
                                    var pt = real_segments[n].PointAt(t);
                                    beam_mesh.Vertices.Add(pt.X, pt.Y, pt.Z);
                                }
                                // Add mesh face
                                if ((m+n)>0 & k>0)
                                {
                                    int a = (m - 1) + n * nFaces + counter_nodes * (k-1);
                                    int b = ((m) + n * nFaces) % counter_nodes + counter_nodes * (k - 1); // get first node in the section if it is closed
                                    int c = ((m) + n * nFaces) % counter_nodes + counter_nodes * (k);
                                    int d = (m-1) + n * nFaces + counter_nodes * (k);
                                    beam_mesh.Faces.AddFace(new MeshFace(a,b,c,d));
                                }
                            }
                        }
                        // Add last face
                        if ( k > 0)
                        {
                            int a1 = (nFaces * real_segments.Count - 1) + counter_nodes * (k - 1);
                            int b1 = (nFaces * real_segments.Count) % counter_nodes + counter_nodes * (k - 1);
                            int c1 = (nFaces * real_segments.Count) % counter_nodes + counter_nodes * (k);
                            int d1 = (nFaces * real_segments.Count -1) % counter_nodes + counter_nodes * (k);
                            beam_mesh.Faces.AddFace(new MeshFace(a1, b1, c1, d1));
                        }                            
                    }
                    oExtrussion.Add(beam_mesh);
                }
            }

            // Assign output
            DA.SetDataList(0, oExtrussion);
        }
    }
}
