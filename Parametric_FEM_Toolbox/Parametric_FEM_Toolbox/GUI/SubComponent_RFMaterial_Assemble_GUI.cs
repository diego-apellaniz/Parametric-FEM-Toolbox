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
    public class SubComponent_RFMaterial_Assemble_GUI : SubComponent
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
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Assemble Material.");
            evaluationUnit.Icon = Properties.Resources.Assemble_Material;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {
            unit.RegisterInputParam(new Param_String(), "Description", "Desc", "Name or Description of Cross Section.", GH_ParamAccess.item);
            unit.Inputs[0].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Material Number", "No", "Optional index number to assign to the RFEM object.", GH_ParamAccess.item);
            unit.Inputs[1].Parameter.Optional = true;
             unit.RegisterInputParam(new Param_String(), "Comment", "Comment", "Comment.", GH_ParamAccess.item);
            unit.Inputs[2].Parameter.Optional = true;

            GH_ExtendableMenu gH_ExtendableMenu = new GH_ExtendableMenu(0, "advanced");
            gH_ExtendableMenu.Name = "Advanced";
            gH_ExtendableMenu.Collapse();
            unit.RegisterInputParam(new Param_Number(), "Elasticity Modulus [N/m²]", "E", "Elasticity Modulus [N/m²]", GH_ParamAccess.item);
            unit.Inputs[3].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Poisson Ratio", "Mu", "Poisson Ratio", GH_ParamAccess.item);
            unit.Inputs[4].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Shear Modulus [N/m²]", "G", "Shear Modulus [N/m²]", GH_ParamAccess.item);
            unit.Inputs[5].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Specific Weight [N/m³]", "W", "Specific Weight [N/m³]", GH_ParamAccess.item);
            unit.Inputs[6].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Thermal Expansion [1/°]", "Alpha", "Thermal Expansion [1/°]", GH_ParamAccess.item);
            unit.Inputs[7].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Partial Safety Factor", "Gamma", "Partial Safety Factor", GH_ParamAccess.item);
            unit.Inputs[8].Parameter.Optional = true;
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[3]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[4]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[5]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[6]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[7]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[8]);
            unit.AddMenu(gH_ExtendableMenu);

            GH_ExtendableMenu gH_ExtendableMenu2 = new GH_ExtendableMenu(1, "modify");
            gH_ExtendableMenu2.Name = "Modify";
            gH_ExtendableMenu2.Collapse();
            unit.RegisterInputParam(new Param_RFEM(), "RF Material", "RF Mat", "Material object from the RFEM model to modify", GH_ParamAccess.item);
            unit.Inputs[9].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Modify", "Modify", "Modify object?", GH_ParamAccess.item);
            unit.Inputs[10].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Delete", "Delete", "Delete object?", GH_ParamAccess.item);
            unit.Inputs[11].Parameter.Optional = true;
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[9]);
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[10]);
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[11]);
            unit.AddMenu(gH_ExtendableMenu2);

            unit.RegisterOutputParam(new Param_RFEM(), "RF Material", "RF Mat", "Output RFMaterial.");
        }

        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Blank;
            //var line = new LineCurve();
            var noIndex = 0;
            var comment = "";
            var description = "";
            var rfMaterial = new RFMaterial();
            var inRFEM = new GH_RFEM();
            var mod = false;
            var del = false;
            var E = 0.0;
            var W = 0.0;
            var Alpha = 0.0;
            var Mu = 0.0;
            var G = 0.0;
            var Gamma = 0.0;


            if (DA.GetData(9, ref inRFEM))
            {
                rfMaterial = new RFMaterial((RFMaterial)inRFEM.Value);
                if (DA.GetData(0, ref description))
                {
                    rfMaterial.Description = description;
                }
            }
            else if (DA.GetData(0, ref description))
            {
                rfMaterial.Description = description;
            }
            else
            {
                msg = "Insufficient input parameters. Provide either Material Description or existing RFMaterial Object. ";
                level = GH_RuntimeMessageLevel.Warning;
                return;
            }
            if (DA.GetData(10, ref mod))
            {
                rfMaterial.ToModify = mod;
            }
            if (DA.GetData(11, ref del))
            {
                rfMaterial.ToDelete = del;
            }
            if (DA.GetData(1, ref noIndex))
            {
                rfMaterial.No = noIndex;
            }
            if (DA.GetData(2, ref comment))
            {
                rfMaterial.Comment = comment;
            }
            if (DA.GetData(3, ref E))
            {
                rfMaterial.E = E;
            }
            if (DA.GetData(4, ref Mu))
            {
                rfMaterial.Mu = Mu;
            }
            if (DA.GetData(5, ref G))
            {
                rfMaterial.G = G;
            }
            if (DA.GetData(6, ref W))
            {
                rfMaterial.W = W;
            }
            if (DA.GetData(7, ref Alpha))
            {
                rfMaterial.Alpha = Alpha;
            }
            if (DA.GetData(8, ref Gamma))
            {
                rfMaterial.Gamma = Gamma;
            }
            DA.SetData(0, rfMaterial);
        }
    }
}
