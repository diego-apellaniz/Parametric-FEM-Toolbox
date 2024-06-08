using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System.Collections.Generic;
using Parametric_FEM_Toolbox.UIWidgets;
using Parametric_FEM_Toolbox.HelperLibraries;
using Parametric_FEM_Toolbox.Utilities;

namespace Parametric_FEM_Toolbox.GUI
{
    public class SubComponent_Filter_NodalReleases_GUI : SubComponent
    {
        public override string name()
        {
            return "Nodal Releases";
        }
        public override string display_name()
        {
            return "Nodal Releases";
        }
        public override void registerEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Filter nodal releases.");
            evaluationUnit.Icon = Properties.Resources.Filter_NodalRelease;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {
            GH_ExtendableMenu gH_ExtendableMenu = new GH_ExtendableMenu(0, "menu_settings");
            gH_ExtendableMenu.Name = "Advanced Options";
            gH_ExtendableMenu.Collapse();
            unit.RegisterInputParam(new Param_String(), "Node Number", "Node", "Node the release is attached to", GH_ParamAccess.list);
            unit.Inputs[0].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Member List", "Members", "List of released members", GH_ParamAccess.list);
            unit.Inputs[1].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Surface List", "Sfcs", "List of released surfaces", GH_ParamAccess.list);
            unit.Inputs[2].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Solid List", "Solids", "List of released solids", GH_ParamAccess.list);
            unit.Inputs[3].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Hinge Number", "Hinge", "Nomber of member hinge to import properties from", GH_ParamAccess.list);
            unit.Inputs[4].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Member Number", "Member", "Member (or line) number to get the axis system from", GH_ParamAccess.list);
            unit.Inputs[5].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Axis System", "Axis", "Axis System", GH_ParamAccess.list);
            unit.Inputs[6].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Release Location", "Location", "Location", GH_ParamAccess.list);
            unit.Inputs[7].Parameter.Optional = true;
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[0]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[1]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[2]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[3]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[4]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[5]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[6]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[7]);
            unit.AddMenu(gH_ExtendableMenu);
        }
        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Warning;

            var myFilter = new RFFilter();
            myFilter.Type = "Filter (Nodal Releases)";
            var releaseList = new List<string>();            
            var commentList = new List<string>();
            var nodeList = new List<string>();
            var memberList = new List<string>();
            var sfcsList = new List<string>();
            var solidList = new List<string>();
            var hingeList = new List<string>();
            var mamberaxisno = new List<string>();
            var axis = new List<string>();
            var location = new List<string>();

            if (DA.GetDataList(0, releaseList))
            {
                var lineListAll = new List<int>();
                foreach (var no in releaseList)
                {
                    lineListAll.AddRange(no.ToInt());
                }
                myFilter.NRList = lineListAll;
            }            
            if(DA.GetDataList(1, commentList))
            {
                myFilter.NRComment = commentList;
            }
            if (DA.GetDataList(2, nodeList))
            {
                var lineListAll = new List<int>();
                foreach (var no in nodeList)
                {
                    lineListAll.AddRange(no.ToInt());
                }
                myFilter.NRNodeNo = lineListAll;
            }
            if (DA.GetDataList(3, memberList))
            {
                var lineListAll = new List<int>();
                foreach (var no in memberList)
                {
                    lineListAll.AddRange(no.ToInt());
                }
                myFilter.NRReleasedMembersNo = lineListAll;
            }
            if (DA.GetDataList(4, sfcsList))
            {
                var lineListAll = new List<int>();
                foreach (var no in sfcsList)
                {
                    lineListAll.AddRange(no.ToInt());
                }
                myFilter.NRReleasedSurfacesNo = lineListAll;
            }
            if (DA.GetDataList(5, solidList))
            {
                var lineListAll = new List<int>();
                foreach (var no in solidList)
                {
                    lineListAll.AddRange(no.ToInt());
                }
                myFilter.NRReleasedSolidsNo = lineListAll;
            }
            if (DA.GetDataList(6, hingeList))
            {
                var lineListAll = new List<int>();
                foreach (var no in hingeList)
                {
                    lineListAll.AddRange(no.ToInt());
                }
                myFilter.NRHinge = lineListAll;
            }
            if (DA.GetDataList(7, mamberaxisno))
            {
                var lineListAll = new List<int>();
                foreach (var no in mamberaxisno)
                {
                    lineListAll.AddRange(no.ToInt());
                }
                myFilter.NRMemberNo = lineListAll;
            }
            if (DA.GetDataList(8, axis))
            {
                myFilter.NRAxisSystem = axis;
            }
            if (DA.GetDataList(9, location))
            {
                myFilter.NRLocation = location;
            }
            DA.SetData(0, myFilter);
        }
    }
}
