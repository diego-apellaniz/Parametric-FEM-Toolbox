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
    public class SubComponent_RFMaterial_Disassemble_GUI : SubComponent
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
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Disassemble Cross Sections.");
            evaluationUnit.Icon = Properties.Resources.Disassemble_Material;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {
            unit.RegisterInputParam(new Param_RFEM(), "RF Material", "RF Mat", "Input RFMaterial.", GH_ParamAccess.item);
            // unit.Inputs[0].Parameter.Optional = true;

            unit.RegisterOutputParam(new Param_String(), "Description", "Desc", "Name or Description of Cross Section.");
            unit.RegisterOutputParam(new Param_Integer(), "Material Number", "No", "Index number of the RFEM object.");
            unit.RegisterOutputParam(new Param_String(), "Comment", "Comment", "Comment.");
            unit.RegisterOutputParam(new Param_Number(), "Elasticity Modulus [N/m²]", "E", "Elasticity Modulus [N/m²]");
            unit.RegisterOutputParam(new Param_Number(), "Poisson Ratio", "Mu", "Poisson Ratio");
            unit.RegisterOutputParam(new Param_Number(), "Shear Modulus [N/m²]", "G", "Shear Modulus [N/m²]");
            unit.RegisterOutputParam(new Param_Number(), "Specific Weight [N/m³]", "W", "Specific Weight [N/m³]");
            unit.RegisterOutputParam(new Param_Number(), "Thermal Expansion [1/°]", "Alpha", "Thermal Expansion [1/°]");
            unit.RegisterOutputParam(new Param_Number(), "Partial Safety Factor", "Gamma", "Partial Safety Factor");
            unit.RegisterOutputParam(new Param_String(), "Material Model Type", "Type", "Material Model Type.");
            unit.RegisterOutputParam(new Param_String(), "TextID", "TextID", "TextID.");
            unit.RegisterOutputParam(new Param_Boolean(), "User Defined", "User", "User Defined.");
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
            var rfMat = (RFMaterial)inGH.Value;
            // Output
            DA.SetData(0, rfMat.Description);
            DA.SetData(1, rfMat.No);
            DA.SetData(2, rfMat.Comment);
            DA.SetData(3, rfMat.E);
            DA.SetData(4, rfMat.Mu);
            DA.SetData(5, rfMat.G);
            DA.SetData(6, rfMat.W);
            DA.SetData(7, rfMat.Alpha);
            DA.SetData(8, rfMat.Gamma);
            DA.SetData(9, rfMat.ModelType);
            DA.SetData(10, rfMat.TextID);
            DA.SetData(11, rfMat.UserDefined);
        }
    }
}
