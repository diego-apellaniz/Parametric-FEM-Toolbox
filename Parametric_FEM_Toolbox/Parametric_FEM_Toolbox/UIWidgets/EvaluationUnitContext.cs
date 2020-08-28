using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parametric_FEM_Toolbox.UIWidgets
{
    public class EvaluationUnitContext
    {
        private EvaluationUnit unit;
        private GH_MenuCollection collection;
        public GH_MenuCollection Collection
        {
            get
            {
                return collection;
            }
            set
            {
                collection = value;
            }
        }
        public EvaluationUnitContext(EvaluationUnit unit)
        {
            this.unit = unit;
            collection = new GH_MenuCollection();
        }
    }
}
