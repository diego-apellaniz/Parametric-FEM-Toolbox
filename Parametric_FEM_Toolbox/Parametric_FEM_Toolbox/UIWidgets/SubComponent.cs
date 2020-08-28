using Grasshopper.Kernel;

namespace Parametric_FEM_Toolbox.UIWidgets
{
    public abstract class SubComponent
    {
        public abstract string name();

        public abstract string display_name();

        public abstract void registerEvaluationUnits(EvaluationUnitManager mngr);

        public abstract void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level);

        public virtual void OnComponentLoaded()
        {
        }
    }
}
