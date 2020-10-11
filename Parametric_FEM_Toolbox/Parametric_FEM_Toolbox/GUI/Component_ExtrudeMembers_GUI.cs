using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Parametric_FEM_Toolbox.HelperLibraries;
using Parametric_FEM_Toolbox.RFEM;
using Rhino.Geometry;
using System.Linq;
using Grasshopper.Kernel.Types.Transforms;

namespace Parametric_FEM_Toolbox.GUI
{
    public class Component_ExtrudeMembers_GUI : GH_Component
    {
        // Declare class variables outside the method "SolveInstance" so their values persist 
        // when the method is called again.

        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public Component_ExtrudeMembers_GUI()
          : base("Extrude Members", "Extrude", "Extrude members using the geometry of the cross sections assigned to them.", "B+G Toolbox", "RFEM")
        {
        }

        // Define Keywords to search for this Component more easily in Grasshopper
        public override IEnumerable<string> Keywords => new string[] {"rf", "rfextrude", "extrude", "members"};

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Use the pManager object to register your input parameters.
            pManager.AddParameter(new Param_RFEM(), "RFMembers", "M", "RF-Memebers from the RFEM model to extrude in 3d.", GH_ParamAccess.item);
            pManager.AddParameter(new Param_RFEM(), "RFCroSec", "CS", "Cross sections attached to the input members", GH_ParamAccess.list);
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
            pManager.AddBrepParameter("Extrussions", "E", "Extruded members.", GH_ParamAccess.list);

            // Sometimes you want to hide a specific parameter from the Rhino preview.
            // You can use the HideParameter() method as a quick way:
            // pManager.HideParameter(0);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input
            var inGH = new GH_RFEM();
            var inGH2 = new List<GH_RFEM>();
            var iMember = new RFMember();
            var iCroSecs = new List<RFCroSec>();

            // Output
            var oExtrussion = new List<Brep>();
            var auxcrvs = new List<Curve>();

            // Register input parameters
            if(!DA.GetData(0, ref inGH)) return;
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
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, $"Provide cross sections for member No {iMember.No}.");
                return;
            }

            // Get base geometry
            var crosecs1 = iCroSecs.Where(x => x.No == iMember.StartCrossSectionNo).ToList()[0].Shape;
            var crosecs2 = iCroSecs.Where(x => x.No == iMember.EndCrossSectionNo).ToList()[0].Shape;
            var baseline = iMember.BaseLine.ToCurve();

            // Check geometry
            if ((crosecs1.Sum(x => x.SpanCount) != crosecs2.Sum(x => x.SpanCount)) || (crosecs1.Count != crosecs2.Count))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, $"Provide similar cross sections for member No {iMember.No}.");
                return;
            }

            // Orient cross sections
            var plane1 = iMember.Frames[0];
            var orientation_1 = (ITransform)new Orientation(new Plane(Point3d.Origin, new Vector3d(0, 0, 1)), plane1);
            var trans1 = orientation_1.ToMatrix();
            var plane2 = iMember.Frames[1];
            var orientation_2 = (ITransform)new Orientation(new Plane(Point3d.Origin, new Vector3d(0, 0, 1)), plane2);
            var trans2 = orientation_2.ToMatrix();
            var trancrvs_1 = new List<Curve>();
            var trancrvs_2 = new List<Curve>();
            for (int i = 0; i < crosecs1.Count; i++)
            {             
                var cro_aux_1 = crosecs1[i].DuplicateCurve();
                cro_aux_1.Transform(trans1);
                trancrvs_1.Add(cro_aux_1);
                var cro_aux_2 = crosecs2[i].DuplicateCurve();
                cro_aux_2.Transform(trans2);
                trancrvs_2.Add(cro_aux_2);
            }


            // Extrude member -> just non tapered members
            //
            //for (int i = 0; i < crosecs1.Count; i++)
            //{
            //    oExtrussion.AddRange(Brep.CreateFromLoft(new List<Curve>() { trancrvs_1[i], trancrvs_2[i] }, Point3d.Unset, Point3d.Unset, LoftType.Straight, false));
            //}

            for (int i = 0; i < crosecs1.Count; i++)
            {
                var baseline_aux = baseline.DuplicateCurve();
                Vector3d translationVector = trancrvs_1[i].PointAtStart - baseline_aux.PointAtStart;
                baseline_aux.Translate(translationVector);
                SumSurface sumSurface = SumSurface.Create(trancrvs_1[i], baseline_aux);
                oExtrussion.Add(sumSurface.ToBrep());
            }


            // Assign output
            DA.SetDataList(0, oExtrussion);
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
                return Properties.Resources.Extrude_Members;                
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("3570bd81-f52c-4225-a028-e25b8b63bc98"); }
        }
    }
}
