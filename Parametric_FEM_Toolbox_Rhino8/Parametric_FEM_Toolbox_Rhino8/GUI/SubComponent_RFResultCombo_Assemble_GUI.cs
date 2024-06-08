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
    public class SubComponent_RFResultCombo_Assemble_GUI : SubComponent
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
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Assemble Result Combinations.");
            evaluationUnit.Icon = Properties.Resources.Assemble_ResultCombination;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {
            unit.RegisterInputParam(new Param_Integer(), "Load Combo Number", "No", "Optional index number to assign to the RFEM object.", GH_ParamAccess.item);
            unit.Inputs[0].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Definition", "Definition", "Definition of Result Combination.", GH_ParamAccess.item);
            unit.Inputs[1].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Description", "Description", "Description of Result Combination.", GH_ParamAccess.item);
            unit.Inputs[2].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Comment", "Comment", "Comment.", GH_ParamAccess.item);
            unit.Inputs[3].Parameter.Optional = true;

            GH_ExtendableMenu gH_ExtendableMenu = new GH_ExtendableMenu(0, "advanced");
            gH_ExtendableMenu.Name = "Advanced";
            gH_ExtendableMenu.Collapse();
            unit.RegisterInputParam(new Param_Integer(), "Design Situation", "Design", UtilLibrary.DescriptionRFTypes(typeof(DesignSituationType)), GH_ParamAccess.item);
            unit.Inputs[4].EnumInput = UtilLibrary.ListRFTypes(typeof(DesignSituationType));
            unit.Inputs[4].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "To Solve", "Solve", "Solve Result Combination?", GH_ParamAccess.item);
            unit.Inputs[5].Parameter.Optional = true;
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[4]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[5]);
            unit.AddMenu(gH_ExtendableMenu);

            GH_ExtendableMenu gH_ExtendableMenu2 = new GH_ExtendableMenu(1, "modify");
            gH_ExtendableMenu2.Name = "Modify";
            gH_ExtendableMenu2.Collapse();
            unit.RegisterInputParam(new Param_RFEM(), "RF Result Combination", "RF ResultCombo", "Load object from the RFEM model to modify", GH_ParamAccess.item);
            unit.Inputs[6].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Modify", "Modify", "Modify object?", GH_ParamAccess.item);
            unit.Inputs[7].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Delete", "Delete", "Delete object?", GH_ParamAccess.item);
            unit.Inputs[8].Parameter.Optional = true;
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[6]);
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[7]);
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[8]);
            unit.AddMenu(gH_ExtendableMenu2);

            unit.RegisterOutputParam(new Param_RFEM(), "RF Result Combo", "RF ResultCombo", "Output RFResultCombo.");
        }

        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Blank;

            var noIndex = 0;
            var comment = "";
            var description = "";
            var definition = "";
            var rfLoadCcombo = new RFResultCombo();
            var inRFEM = new GH_RFEM();
            var mod = false;
            var del = false;
            var toSolve = true;
            var design = 0;


            if (DA.GetData(6, ref inRFEM))
            {
                rfLoadCcombo = new RFResultCombo((RFResultCombo)inRFEM.Value);
                if (DA.GetData(1, ref definition))
                {
                    rfLoadCcombo.Definition = definition;
                }
                if (DA.GetData(0, ref noIndex))
                {
                    rfLoadCcombo.No = noIndex;
                }
            }
            else if  (DA.GetData(0, ref noIndex) && (DA.GetData(1, ref definition)))
            {
                rfLoadCcombo.No = noIndex;
                rfLoadCcombo.Definition = definition;
                // Set ToSolve = true
                rfLoadCcombo.ToSolve = true;
            }
            else
            {
                msg = "Insufficient input parameters. Provide either Load Case Number and Definition or existing RFLoadCombo Object. ";
                level = GH_RuntimeMessageLevel.Warning;
                return;
            }
            if (DA.GetData(7, ref mod))
            {
                rfLoadCcombo.ToModify = mod;
            }
            if (DA.GetData(8, ref del))
            {
                rfLoadCcombo.ToDelete = del;
            }
            if (DA.GetData(2, ref description))
            {
                rfLoadCcombo.Description = description;
            }
            if (DA.GetData(3, ref comment))
            {
                rfLoadCcombo.Comment = comment;
            }
            if (DA.GetData(4, ref design))
            {
                rfLoadCcombo.DesignSituation = (DesignSituationType)design;
                // Check Action Category
                if (rfLoadCcombo.DesignSituation == DesignSituationType.UnknownDesignSituation)
                {
                    msg = "Design Situation Type not supported. ";
                    level = GH_RuntimeMessageLevel.Warning;
                    return;
                }
            }
            if (DA.GetData(5, ref toSolve))
            {
                rfLoadCcombo.ToSolve = toSolve;
            }                      
            DA.SetData(0, rfLoadCcombo);
        }
    }
}