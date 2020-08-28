using Grasshopper.Kernel;
using System.Collections.Generic;

namespace Parametric_FEM_Toolbox.UIWidgets
{
    public class ExtendedPlug
    {
        private bool isMenu;
        private IGH_Param parameter;
        public IGH_Param Parameter => parameter;
        public ExtendedPlug(IGH_Param parameter)
        {
            this.parameter = parameter;
        }
        public bool IsMenu
        {
            get
            {
                return isMenu;
            }
            set
            {
                isMenu = value;
            }
        }
        private EvaluationUnit unit;
        public EvaluationUnit Unit
        {
            get
            {
                return unit;
            }
            set
            {
                unit = value;
            }
        }

        // daq - Value List for Enums
        private List<string> EnumValues = null;
        public List<string> EnumInput {
            get
            {
                return EnumValues;
            }
            set
            {
                EnumValues = value;
            }
        }
    }
}
