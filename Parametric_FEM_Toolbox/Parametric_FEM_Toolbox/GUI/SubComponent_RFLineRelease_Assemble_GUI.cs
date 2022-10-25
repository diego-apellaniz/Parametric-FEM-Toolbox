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

namespace Parametric_FEM_Toolbox.GUI
{
    public class SubComponent_RFLineRelease_Assemble_GUI : SubComponent
    {
        public override string name()
        {
            return "Assemble";
        }
        public override string display_name()
        {
            return "Assemble";
        }
        public override void registerEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Assemble Line Releases.");
            evaluationUnit.Icon = Properties.Resources.Assemble_LineReleases;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {
            unit.RegisterInputParam(new Param_Integer(), "Release Number", "No", "Optional index number to assign to the RFEM object.", GH_ParamAccess.item);
            unit.Inputs[0].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Line Number", "Line", "Index Number of the line to assign the release to.", GH_ParamAccess.item);
            unit.Inputs[1].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Line Release Type", "TypeNo", "Index Number of the line release type.", GH_ParamAccess.item);
            unit.Inputs[2].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Member List", "Members", "List of released members", GH_ParamAccess.item);
            unit.Inputs[3].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Surface List", "Surfaces", "List of released surfaces", GH_ParamAccess.item);
            unit.Inputs[4].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Comment", "Comment", "Comment.", GH_ParamAccess.item);
            unit.Inputs[5].Parameter.Optional = true;

            GH_ExtendableMenu gH_ExtendableMenu = new GH_ExtendableMenu(0, "advanced");
            gH_ExtendableMenu.Name = "Advanced";
            gH_ExtendableMenu.Collapse();
            //unit.RegisterInputParam(new Param_Integer(), "Axis System", "Axis", UtilLibrary.DescriptionRFTypes(typeof(LineReleaseAxisSystem)), GH_ParamAccess.item);
            //unit.Inputs[6].EnumInput = UtilLibrary.ListRFTypes(typeof(LineReleaseAxisSystem));
            //unit.Inputs[6].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Release Location", "Location", UtilLibrary.DescriptionRFTypes(typeof(ReleaseLocation)), GH_ParamAccess.item);
            unit.Inputs[6].EnumInput = UtilLibrary.ListRFTypes(typeof(ReleaseLocation));
            unit.Inputs[6].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Definition nodes", "DefNodes", "Nodes to use as definition nodes.", GH_ParamAccess.item);
            unit.Inputs[7].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Rotation angle", "Rot", "Rotation angle [°].", GH_ParamAccess.item);
            unit.Inputs[8].Parameter.Optional = true;
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[6]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[7]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[8]);
            //gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[8]);
            unit.AddMenu(gH_ExtendableMenu);

            GH_ExtendableMenu gH_ExtendableMenu2 = new GH_ExtendableMenu(1, "modify");
            gH_ExtendableMenu2.Name = "Modify";
            gH_ExtendableMenu2.Collapse();
            unit.RegisterInputParam(new Param_RFEM(), "RF Line Release", "RF LineRelease", "Line Release object from the RFEM model to modify", GH_ParamAccess.item);
            unit.Inputs[9].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Modify", "Modify", "Modify object?", GH_ParamAccess.item);
            unit.Inputs[10].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Delete", "Delete", "Delete object?", GH_ParamAccess.item);
            unit.Inputs[11].Parameter.Optional = true;
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[9]);
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[10]);
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[11]);
            unit.AddMenu(gH_ExtendableMenu2);

            unit.RegisterOutputParam(new Param_RFEM(), "RF Line Release", "RF LineRelease", "Output RFLineRelease.");
        }

        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Blank;
            //var line = new LineCurve();
            var noIndex = 0;
            var comment = "";
            var rfRelease = new RFLineRelease();
            var inRFEM = new GH_RFEM();
            var mod = false;
            var del = false;
            var member_list = "";
            var sfc_list = "";
            var lineNo = 0;
            var typeNo = 0;
            var location = 0;
            var axis_system = 0;
            var rotation = 0.0;
            var definition_nodes = "";

            if (DA.GetData(9, ref inRFEM))
            {
                rfRelease = new RFLineRelease((RFLineRelease)inRFEM.Value);                
            }
            else if ((DA.GetData(3, ref member_list) | DA.GetData(4, ref sfc_list) && DA.GetData(1, ref lineNo) && DA.GetData(2, ref typeNo)))
            {
                // Check that axis are not being taken from released item                
                rfRelease = new RFLineRelease();
                rfRelease.LineNo = lineNo;
                rfRelease.ReleasedMembers = member_list;
                rfRelease.ReleasedSurfaces = sfc_list;
                rfRelease.TypeNo = typeNo;
            }
            else
            {
                msg = "Insufficient input parameters. Provide line number, type number and surface list. ";
                level = GH_RuntimeMessageLevel.Warning;
                return;
            }
            if (DA.GetData(10, ref mod))
            {
                rfRelease.ToModify = mod;
            }
            if (DA.GetData(11, ref del))
            {
                rfRelease.ToDelete = del;
            }
            if (DA.GetData(0, ref noIndex))
            {
                rfRelease.No = noIndex;
            }
            if (DA.GetData(5, ref comment))
            {
                rfRelease.Comment = comment;
            }
            if (DA.GetData(6, ref location))
            {
                rfRelease.Location = (ReleaseLocation)location;
            }
            //if (DA.GetData(6, ref axis_system))
            //{
            //    rfRelease.AxisSystem = (LineReleaseAxisSystem)axis_system;
            //}
            if (DA.GetData(7, ref definition_nodes))
            {
                rfRelease.DefinitionNodes = definition_nodes;
            }
            if (DA.GetData(8, ref rotation))
            {
                rfRelease.Rotation = rotation*Math.PI/180;
            }
            DA.SetData(0, rfRelease);
        }
    }
}