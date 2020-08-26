using System;
using System.Collections.Generic;

namespace RFEM_daq.UIWidgets
{
    public class EvaluationUnitManager
    {
        private GH_SwitcherComponent component;

        private List<EvaluationUnit> units;

        public List<EvaluationUnit> Units => units;

        public EvaluationUnitManager(GH_SwitcherComponent component)
        {
            units = new List<EvaluationUnit>();
            this.component = component;
        }

        public EvaluationUnit GetUnit(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            foreach (EvaluationUnit unit in units)
            {
                if (unit.Name.Equals(name))
                {
                    return unit;
                }
            }
            return null;
        }

        public void RegisterUnit(EvaluationUnit unit)
        {
            string name = unit.Name;
            if (name != null)
            {
                if (GetUnit(name) != null)
                {
                    throw new ArgumentException("Duplicate evaluation unit[" + name + "] detected");
                }
                unit.Component = component;
                units.Add(unit);
            }
        }
    }
}
