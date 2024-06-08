using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using Parametric_FEM_Toolbox.UIWidgets;
using Parametric_FEM_Toolbox.Utilities;
using Parametric_FEM_Toolbox.HelperLibraries;
using Dlubal.RFEM5;
using Parametric_FEM_Toolbox.RFEM;

namespace Parametric_FEM_Toolbox.GUI
{
    public class SubComponent_RFMemberEccentricity_Disassemble_GUI : SubComponent
    {
        public override string name()
        {
            return "Disassemble";
        }
        public override string display_name()
        {
            return "Disassemble";
        }
        public override void registerEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Disassemble Member Eccentricities.");
            evaluationUnit.Icon = Properties.Resources.Disassemble_MemberEccentricity;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {
            unit.RegisterInputParam(new Param_RFEM(), "RF Member Eccentricity", "RF MemberEcc", "Input RFMemberEccentricity.", GH_ParamAccess.item);
            // unit.Inputs[0].Parameter.Optional = true;

            unit.RegisterOutputParam(new Param_Integer(), "Eccentricity Number", "No", "Index number of the RFEM object.");
            unit.RegisterOutputParam(new Param_Vector(), "Start Eccentricities", "Start", "Member Start Eccentricities [mm]");
            unit.RegisterOutputParam(new Param_Vector(), "End Eccentricities", "End", "Member End Eccentricities [mm]");
            unit.RegisterOutputParam(new Param_String(), "Horizontal alignment", "H", "Horizontal alignment");
            unit.RegisterOutputParam(new Param_String(), "Vertical alignment", "V", "Vertical alignment");
            unit.RegisterOutputParam(new Param_String(), "Reference system", "Sys", "Reference system");
            unit.RegisterOutputParam(new Param_Boolean(), "Hinge At Start Node", "HStart", "Member hinge location at start node");
            unit.RegisterOutputParam(new Param_Boolean(), "Hinge At End Node", "HEnd", "Member hinge location at end node");
            unit.RegisterOutputParam(new Param_Boolean(), "Transverse offset", "TOff", "Transverse offset from cross-section of other object");
            unit.RegisterOutputParam(new Param_String(), "Reference object type", "Ref", "Reference object type");
            unit.RegisterOutputParam(new Param_Integer(), "Reference object No", "ObjNo", "Reference object No");
            unit.RegisterOutputParam(new Param_String(), "Horizontal axis offset", "Ho", "Horizontal axis offset");
            unit.RegisterOutputParam(new Param_String(), "Vertical axis offset", "Vo", "Vertical axis offset");
            unit.RegisterOutputParam(new Param_Boolean(), "Start Adjoining Members Offset", "OStart", "Axial offset from adjoining members at member start");
            unit.RegisterOutputParam(new Param_Boolean(), "End Adjoining Members Offset", "OEnd", "Axial offset from adjoining members at member end");
            unit.RegisterOutputParam(new Param_String(), "Comment", "Comment", "Comment.");
        }

        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Blank;

            // Input
            var inGH = new GH_RFEM();
            if (!DA.GetData(0, ref inGH))
            {
                return;
            }
            var rfMemberEccentricity = (RFMemberEccentricity)inGH.Value;
            // Output
            DA.SetData(0, rfMemberEccentricity.No);
            DA.SetData(1, rfMemberEccentricity.Start);
            DA.SetData(2, rfMemberEccentricity.End);
            DA.SetData(3, rfMemberEccentricity.HorizontalAlignment);
            DA.SetData(4, rfMemberEccentricity.VerticalAlignment);
            DA.SetData(5, rfMemberEccentricity.ReferenceSystemType);
            DA.SetData(6, rfMemberEccentricity.HingeAtStartNode);
            DA.SetData(7, rfMemberEccentricity.HingeAtEndNode);
            DA.SetData(8, rfMemberEccentricity.TransverseOffset);
            DA.SetData(9, rfMemberEccentricity.ReferenceObjectType);
            DA.SetData(10, rfMemberEccentricity.ReferenceObjectNo);
            DA.SetData(11, rfMemberEccentricity.HorizontalAxisOffset);
            DA.SetData(12, rfMemberEccentricity.VerticalAxisOffset);
            DA.SetData(13, rfMemberEccentricity.StartAdjoiningMembersOffset);
            DA.SetData(14, rfMemberEccentricity.EndAdjoiningMembersOffset);
            DA.SetData(15, rfMemberEccentricity.Comment);
        }
    }
}
