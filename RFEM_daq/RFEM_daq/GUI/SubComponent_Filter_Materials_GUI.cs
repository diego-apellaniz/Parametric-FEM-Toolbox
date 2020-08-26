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
    public class SubComponent_Filter_Materials_GUI : SubComponent
    {
        public override string name()
        {
            return "Materials";
        }
        public override string display_name()
        {
            return "Materials";
        }
        public override void registerEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Filter materials.");
            evaluationUnit.Icon = RFEM_daq.Properties.Resources.icon_RFEM_FilterMaterial;
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
            unit.RegisterInputParam(new Param_String(), "Description", "Desc", "Name or Description of Material.", GH_ParamAccess.list);
            unit.Inputs[0].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Elasticity Modulus [N/m²]", "E", "Elasticity Modulus [N/m²]", GH_ParamAccess.list);
            unit.Inputs[1].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Poisson Ratio", "Mu", "Poisson Ratio", GH_ParamAccess.list);
            unit.Inputs[2].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Shear Modulus [N/m²]", "G", "Shear Modulus [N/m²]", GH_ParamAccess.list);
            unit.Inputs[3].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Specific Weight [N/m³]", "W", "Specific Weight [N/m³]", GH_ParamAccess.list);
            unit.Inputs[4].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Thermal Expansion [1/°]", "Alpha", "Thermal Expansion [1/°]", GH_ParamAccess.list);
            unit.Inputs[5].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Interval(), "Partial Safety Factor", "Gamma", "Partial Safety Factor", GH_ParamAccess.list);
            unit.Inputs[6].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Material Model Type", "Type", "Material Model Type.", GH_ParamAccess.list);
            unit.Inputs[7].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "User Defined", "User", "User Defined", GH_ParamAccess.list);
            unit.Inputs[8].Parameter.Optional = true;
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[0]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[1]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[2]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[3]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[4]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[5]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[6]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[7]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[8]);

            unit.AddMenu(gH_ExtendableMenu);
        }

        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Warning;

            var myFilter = new RFFilter();
            myFilter.Type = "Filter (Materials)";
            var matList = new List<string>();
            var matListAll = new List<int>();
            var commentList = new List<string>();
            var E = new List<Interval>();
            var Mu = new List<Interval>();
            var G = new List<Interval>();
            var W = new List<Interval>();
            var Alpha = new List<Interval>();
            var Gamma = new List<Interval>();
            var description = new List<string>();
            var modelType = new List<string>();
            var userDefined = new List<bool>();
           

            if (DA.GetDataList(0, matList))
            {
                foreach (var no in matList)
                {
                    matListAll.AddRange(no.ToInt());
                }
                myFilter.MatList = matListAll;
            }            
            if(DA.GetDataList(1, commentList))
            {
                myFilter.MatComment = commentList;
            }            
            if (DA.GetDataList(2, description))
            {
                myFilter.MatDes = description;
            }
            if (DA.GetDataList(3, E))
            {
                myFilter.MatE = E;
            }
            if (DA.GetDataList(4, Mu))
            {
                myFilter.MatMu = Mu;
            }
            if (DA.GetDataList(5, G))
            {
                myFilter.MatG = G;
            }
            if (DA.GetDataList(6, W))
            {
                myFilter.MatW = W;
            }
            if (DA.GetDataList(7, Alpha))
            {
                myFilter.MatAlpha = Alpha;
            }
            if (DA.GetDataList(8, Gamma))
            {
                myFilter.MatGamma = Gamma;
            }
            if (DA.GetDataList(9, modelType))
            {
                myFilter.MatModelType = modelType;
            }
            if (DA.GetDataList(10, userDefined))
            {
                myFilter.MatUserDefined = userDefined;
            }

            DA.SetData(0, myFilter);
        }
    }
}
