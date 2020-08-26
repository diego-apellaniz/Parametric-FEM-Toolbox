using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using RFEM_daq.UIWidgets;
using RFEM_daq.Utilities;
using RFEM_daq.HelperLibraries;
using Dlubal.RFEM5;
using RFEM_daq.RFEM;

namespace RFEM_daq.GUI
{
    public class SubComponent_RFLoadCase_Assemble_GUI : SubComponent
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
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Assemble Load Cases.");
            evaluationUnit.Icon = RFEM_daq.Properties.Resources.Assemble_LoadCase;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {
            unit.RegisterInputParam(new Param_Integer(), "Load Case Number", "No", "Optional index number to assign to the RFEM object.", GH_ParamAccess.item);
            unit.Inputs[0].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Description", "Description", "Description of Load Case.", GH_ParamAccess.item);
            unit.Inputs[1].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Action Category", "Action", UtilLibrary.DescriptionRFTypes(typeof(ActionCategoryType)), GH_ParamAccess.item);
            unit.Inputs[2].EnumInput = UtilLibrary.ListRFTypes(typeof(ActionCategoryType));
            unit.Inputs[2].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Comment", "Comment", "Comment.", GH_ParamAccess.item);
            unit.Inputs[3].Parameter.Optional = true;

            GH_ExtendableMenu gH_ExtendableMenu = new GH_ExtendableMenu(0, "advanced");
            gH_ExtendableMenu.Name = "Advanced";
            gH_ExtendableMenu.Collapse();
            unit.RegisterInputParam(new Param_Vector(), "Self Weight Factor", "Self Weight", "Self Weight Factor", GH_ParamAccess.item);
            unit.Inputs[4].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "To Solve", "Solve", "Solve Load Case?", GH_ParamAccess.item);
            unit.Inputs[5].Parameter.Optional = true;
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[4]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[5]);
            unit.AddMenu(gH_ExtendableMenu);

            GH_ExtendableMenu gH_ExtendableMenu2 = new GH_ExtendableMenu(1, "modify");
            gH_ExtendableMenu2.Name = "Modify";
            gH_ExtendableMenu2.Collapse();
            unit.RegisterInputParam(new Param_RFEM(), "RF Load Case", "RF LoadCase", "Load object from the RFEM model to modify", GH_ParamAccess.item);
            unit.Inputs[6].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Modify", "Modify", "Modify object?", GH_ParamAccess.item);
            unit.Inputs[7].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Delete", "Delete", "Delete object?", GH_ParamAccess.item);
            unit.Inputs[8].Parameter.Optional = true;
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[6]);
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[7]);
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[8]);
            unit.AddMenu(gH_ExtendableMenu2);

            unit.RegisterOutputParam(new Param_RFEM(), "RF Load Case", "RF LoadCase", "Output RFLoadCase.");
        }

        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Blank;

            var noIndex = 0;
            var comment = "";
            var description = "";
            var rfLoadCase = new RFLoadCase();
            var inRFEM = new GH_RFEM();
            var mod = false;
            var del = false;
            var toSolve = true;
            var selfweight = new Vector3d(0, 0, 0);
            var action = 0;


            if (DA.GetData(6, ref inRFEM))
            {
                rfLoadCase = new RFLoadCase((RFLoadCase)inRFEM.Value);
                if (DA.GetData(2, ref action))
                {
                    rfLoadCase.ActionCategory = (ActionCategoryType)action;
                }
                if (DA.GetData(0, ref noIndex))
                {
                    rfLoadCase.No = noIndex;
                }
            }
            else if  (DA.GetData(0, ref noIndex) && (DA.GetData(2, ref action)))
            {
                rfLoadCase.No = noIndex;
                rfLoadCase.ActionCategory = (ActionCategoryType)action;
                // Set ToSolve = true
                rfLoadCase.ToSolve = true;
            }
            else
            {
                msg = "Insufficient input parameters. Provide either Load Case Number and Action Category or existing RFLoadCase Object. ";
                level = GH_RuntimeMessageLevel.Warning;
                return;
            }
            if (DA.GetData(7, ref mod))
            {
                rfLoadCase.ToModify = mod;
            }
            if (DA.GetData(8, ref del))
            {
                rfLoadCase.ToDelete = del;
            }
            if (DA.GetData(1, ref description))
            {
                rfLoadCase.Description = description;
            }
            if (DA.GetData(3, ref comment))
            {
                rfLoadCase.Comment = comment;
            }
            if (DA.GetData(4, ref selfweight))
            {
                rfLoadCase.SelfWeightFactor = selfweight;
            }
            if (DA.GetData(5, ref toSolve))
            {
                rfLoadCase.ToSolve = toSolve;
            }

            // Check Action Category
            if (rfLoadCase.ActionCategory == ActionCategoryType.UnknownActionCategory)
            {
                msg = "Action Category Type not supported. ";
                level = GH_RuntimeMessageLevel.Warning;
                return;
            }            
            DA.SetData(0, rfLoadCase);
        }
    }
}
