using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using Parametric_FEM_Toolbox.UIWidgets;
using Parametric_FEM_Toolbox.Utilities;
using Parametric_FEM_Toolbox.HelperLibraries;

namespace Parametric_FEM_Toolbox.GUI
{
    public class SubComponent_Filter_Members_GUI : SubComponent
    {
        public override string name()
        {
            return "Members";
        }
        public override string display_name()
        {
            return "Members";
        }
        public override void registerEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Filter members.");
            evaluationUnit.Icon = Properties.Resources.icon_RFEM_FilterMember;
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
            unit.RegisterInputParam(new Param_Interval(), "X [m]", "X", "X Coordinate", GH_ParamAccess.list);
            unit.Inputs[0].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Y [m]", "Y", "Y Coordinate", GH_ParamAccess.list);
            unit.Inputs[1].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Z [m]", "Z", "Z Coordinate", GH_ParamAccess.list);
            unit.Inputs[2].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Member Type", "M Type", "Member Type", GH_ParamAccess.list);
            unit.Inputs[3].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Line No", "Line", "Line No", GH_ParamAccess.list);
            unit.Inputs[4].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Start Cross-Section", "S CroSec", "Number of Start Cross-Section", GH_ParamAccess.list);
            unit.Inputs[5].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "End Cross-Section", "E CroSec", "Number of End Cross-Section", GH_ParamAccess.list);
            unit.Inputs[6].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Rotation Type", "Rot Type", "Rotation Type", GH_ParamAccess.list);
            unit.Inputs[7].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Rotation Angle [°]", "β", "Rotation Angle", GH_ParamAccess.list);
            unit.Inputs[8].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Start Hinge", "S Hinge", "Number of Start Hinge", GH_ParamAccess.list);
            unit.Inputs[9].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "End Hinge", "E Hinge", "Number of End Hinge", GH_ParamAccess.list);
            unit.Inputs[10].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Eccentricity", "Ecc", "Number of Eccentricity", GH_ParamAccess.list);
            unit.Inputs[11].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Division", "Div", "Number of Division", GH_ParamAccess.list);
            unit.Inputs[12].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Taper Shape", "Taper", "Taper Shape", GH_ParamAccess.list);
            unit.Inputs[13].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "FactorY", "Kcr,y", "Effective length factor Kcr,y", GH_ParamAccess.list);
            unit.Inputs[14].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "FactorZ", "Kcr,z", "Effective length factor Kcr,z", GH_ParamAccess.list);
            unit.Inputs[15].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Length [m]", "L", "Member Length", GH_ParamAccess.list);
            unit.Inputs[16].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Weight [kg]", "W", "Member Weight", GH_ParamAccess.list);
            unit.Inputs[17].Parameter.Optional = true;
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[0]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[1]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[2]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[3]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[4]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[5]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[6]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[7]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[8]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[9]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[10]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[11]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[12]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[13]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[14]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[15]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[16]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[17]);
            unit.AddMenu(gH_ExtendableMenu);
        }

        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Warning;

            var myFilter = new RFFilter();
            myFilter.Type = "Filter (Members)";
            var memberList = new List<string>();
            var memberListAll = new List<int>();
            var commentList = new List<string>();
            var memType = new List<string>();
            var memberLine = new List<string>();
            var memberLineAll = new List<int>();
            var x = new List<Interval>();
            var y = new List<Interval>();
            var z = new List<Interval>();
            var memberSCS = new List<string>();
            var memberSCSAll = new List<int>();
            var memberECS = new List<string>();
            var memberECSAll = new List<int>();
            var rotType = new List<string>();
            var rotAngle = new List<Interval>();
            var memberSH = new List<string>();
            var memberSHAll = new List<int>();
            var memberEH = new List<string>();
            var memberEHAll = new List<int>();
            var memberEcc = new List<string>();
            var memberEccAll = new List<int>();
            var memberDiv = new List<string>();
            var memberDivAll = new List<int>();
            var memberTaper = new List<string>();
            var memberFactorY = new List<Interval>();
            var memberFactorZ = new List<Interval>();
            var memberLength = new List<Interval>();
            var memberWeight = new List<Interval>();

            if (DA.GetDataList(0, memberList))
            {
                foreach (var no in memberList)
                {
                    memberListAll.AddRange(no.ToInt());
                }
                myFilter.MemberList = memberListAll;
            }            
            if(DA.GetDataList(1, commentList))
            {
                myFilter.MemberComment = commentList;
            }            
            if (DA.GetDataList(2, x))
            {
                myFilter.MemberX = x;
            }
            if (DA.GetDataList(3, y))
            {
                myFilter.MemberY = y;
            }
            if (DA.GetDataList(4, z))
            {
                myFilter.MemberZ = z;
            }
            if (DA.GetDataList(5, memType))
            {
                myFilter.MemberType = memType;
            }
            if (DA.GetDataList(6, memberLine))
            {
                foreach (var no in memberLine)
                {
                    memberLineAll.AddRange(no.ToInt());
                }
                myFilter.MemberLineNo = memberLineAll;
            }
            if (DA.GetDataList(7, memberSCS))
            {
                foreach (var no in memberSCS)
                {
                    memberSCSAll.AddRange(no.ToInt());
                }
                myFilter.MemberStartCS = memberSCSAll;
            }
            if (DA.GetDataList(8, memberECS))
            {
                foreach (var no in memberECS)
                {
                    memberECSAll.AddRange(no.ToInt());
                }
                myFilter.MemberEndCS = memberECSAll;
            }
            if (DA.GetDataList(9, rotType))
            {
                myFilter.MemberRotType = rotType;
            }
            if (DA.GetDataList(10, rotAngle))
            {
                myFilter.MemberRotAngle = rotAngle;
            }
            if (DA.GetDataList(11, memberSH))
            {
                foreach (var no in memberSH)
                {
                    memberSHAll.AddRange(no.ToInt());
                }
                myFilter.MemberStartHinge = memberSHAll;
            }
            if (DA.GetDataList(12, memberEH))
            {
                foreach (var no in memberEH)
                {
                    memberEHAll.AddRange(no.ToInt());
                }
                myFilter.MemberEndHinge = memberEHAll;
            }
            if (DA.GetDataList(13, memberEcc))
            {
                foreach (var no in memberEcc)
                {
                    memberEccAll.AddRange(no.ToInt());
                }
                myFilter.MemberEcc = memberEccAll;
            }
            if (DA.GetDataList(14, memberDiv))
            {
                foreach (var no in memberDiv)
                {
                    memberDivAll.AddRange(no.ToInt());
                }
                myFilter.MemberDivision = memberDivAll;
            }
            if (DA.GetDataList(15, memberTaper))
            {
                myFilter.MemberTaper = memberTaper;
            }
            if (DA.GetDataList(16, memberFactorY))
            {
                myFilter.MemberFactorY = memberFactorY;
            }
            if (DA.GetDataList(17, memberFactorZ))
            {
                myFilter.MemberFactorZ = memberFactorZ;
            }
            if (DA.GetDataList(18, memberLength))
            {
                myFilter.MemberLength = memberLength;
            }
            if (DA.GetDataList(19, memberWeight))
            {
                myFilter.MemberWeight = memberWeight;
            }

            DA.SetData(0, myFilter);
        }
    }
}
