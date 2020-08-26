using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using RFEM_daq.UIWidgets;
using RFEM_daq.Utilities;
using RFEM_daq.HelperLibraries;

namespace RFEM_daq.GUI
{
    public class SubComponent_Filter_CroSecs_GUI : SubComponent
    {
        public override string name()
        {
            return "Cross Sections";
        }
        public override string display_name()
        {
            return "Cross Sections";
        }
        public override void registerEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Filter cross sections.");
            evaluationUnit.Icon = RFEM_daq.Properties.Resources.icon_RFEM_FilterCS;
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
            unit.RegisterInputParam(new Param_String(), "Description", "Desc", "Name or Description of Cross Section.", GH_ParamAccess.list);
            unit.Inputs[0].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Material Number", "MatNo", "Number of Material assigned to the Cross-Section", GH_ParamAccess.list);
            unit.Inputs[1].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "AxialArea [m²]", "A", "AxialArea [m²]", GH_ParamAccess.list);
            unit.Inputs[2].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "ShearAreaY [m²]", "Ay", "ShearAreaY [m²]", GH_ParamAccess.list);
            unit.Inputs[3].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "ShearAreaZ [m²]", "Az", "ShearAreaZ [m²]", GH_ParamAccess.list);
            unit.Inputs[4].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "BendingMomentY [m⁴]", "Iy", "BendingMomentY [m⁴]", GH_ParamAccess.list);
            unit.Inputs[5].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "BendingMomentZ [m⁴]", "Iz", "BendingMomentZ [m⁴]", GH_ParamAccess.list);
            unit.Inputs[6].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "TorsionMoment [m⁴]", "Jt", "TorsionMoment [m⁴]", GH_ParamAccess.list);
            unit.Inputs[7].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Rotation Angle [°]", "β", "Rotation Angle [°]", GH_ParamAccess.list);
            unit.Inputs[8].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "TemperatureLoadWidth [m]", "TempW", "TemperatureLoadWidth [m]", GH_ParamAccess.list);
            unit.Inputs[9].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "TemperatureLoadDepth [m]", "TempD", "TemperatureLoadDepth [m]", GH_ParamAccess.list);
            unit.Inputs[10].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "User Defined", "User", "User Defined", GH_ParamAccess.list);
            unit.Inputs[11].Parameter.Optional = true;
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
 
            unit.AddMenu(gH_ExtendableMenu);
        }

        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Warning;

            var myFilter = new RFFilter();
            myFilter.Type = "Filter (Cross Sections)";
            var csList = new List<string>();
            var csListAll = new List<int>();
            var commentList = new List<string>();
            var A = new List<Interval>();
            var Ay = new List<Interval>();
            var Az = new List<Interval>();
            var Iy = new List<Interval>();
            var Iz = new List<Interval>();
            var Jt = new List<Interval>();
            var rot = new List<Interval>();
            var tempW = new List<Interval>();
            var tempD = new List<Interval>();
            var description = new List<string>();
            var matNo = new List<string>();
            var matNoAll = new List<int>();
            var userDefined = new List<bool>();
           

            if (DA.GetDataList(0, csList))
            {
                foreach (var no in csList)
                {
                    csListAll.AddRange(no.ToInt());
                }
                myFilter.CSList = csListAll;
            }            
            if(DA.GetDataList(1, commentList))
            {
                myFilter.CSComment = commentList;
            }            
            if (DA.GetDataList(2, description))
            {
                myFilter.CSDes = description;
            }
            if (DA.GetDataList(3, matNo))
            {
                foreach (var no in matNo)
                {
                    matNoAll.AddRange(no.ToInt());
                }
                myFilter.CSMatNo = matNoAll;
            }
            if (DA.GetDataList(4, A))
            {
                myFilter.CSA = A;
            }
            if (DA.GetDataList(5, Ay))
            {
                myFilter.CSAy = Ay;
            }
            if (DA.GetDataList(6, Az))
            {
                myFilter.CSAz = Az;
            }
            if (DA.GetDataList(7, Iy))
            {
                myFilter.CSIy = Iy;
            }
            if (DA.GetDataList(8, Iz))
            {
                myFilter.CSIz = Iz;
            }
            if (DA.GetDataList(9, Jt))
            {
                myFilter.CSJt = Jt;
            }
            if (DA.GetDataList(10, rot))
            {
                myFilter.CSRotAngle = rot;
            }
            if (DA.GetDataList(11, tempW))
            {
                myFilter.CSTempW = tempW;
            }
            if (DA.GetDataList(12, tempD))
            {
                myFilter.CSTempD = tempD;
            }
            if (DA.GetDataList(13, userDefined))
            {
                myFilter.CSUserDefined = userDefined;
            }

            DA.SetData(0, myFilter);
        }
    }
}
