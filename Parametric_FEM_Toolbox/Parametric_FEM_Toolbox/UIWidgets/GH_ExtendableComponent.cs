using GH_IO.Serialization;
using Grasshopper.Kernel;
using System.Collections.Generic;

namespace Parametric_FEM_Toolbox.UIWidgets
{
    public abstract class GH_ExtendableComponent : GH_Component
    {
        public enum GH_ComponentState
        {
            COMPUTED,
            EMPTY
        }

        private GH_RuntimeMessageLevel m_state;

        protected internal GH_ExtendableComponent(string sName, string sAbbreviation, string sDescription, string sCategory, string sSubCategory)
            : base(sName, sAbbreviation, sDescription, sCategory, sSubCategory)
        {
            this.Phase = (0);
        }

        protected virtual void Setup(GH_ExtendableComponentAttributes attr)
        {
        }

        protected virtual void OnComponentLoaded()
        {
        }

        protected virtual void OnComponentReset(GH_ExtendableComponentAttributes attr)
        {
        }

        public override void ComputeData()
        {
            base.ComputeData();
            if (m_state != this.RuntimeMessageLevel && (int)this.RuntimeMessageLevel == 10)
            {
                List<IGH_Param> input = this.Params.Input;
                int count = input.Count;
                bool flag = true;
                for (int i = 0; i < count; i++)
                {
                    if (input[i].SourceCount == 0 && !input[i].Optional && input[i].VolatileData.IsEmpty)
                    {
                        flag = false;
                        break;
                    }
                }
                if (!flag && (int)this.RuntimeMessageLevel == 10)
                {
                    OnComponentReset((GH_ExtendableComponentAttributes)this.Attributes);
                }
            }
            m_state = this.RuntimeMessageLevel;
        }

        public override bool Read(GH_IReader reader)
        {
            bool result = base.Read(reader);
            OnComponentLoaded();
            return result;
        }

        public override void CreateAttributes()
        {
            Setup((GH_ExtendableComponentAttributes)(base.m_attributes = new GH_ExtendableComponentAttributes(this)));
        }

        public bool OutputConnected(int ind)
        {
            return this.Params.Output[ind].Recipients.Count != 0;
        }

        public bool OutputInUse(int ind)
        {
            if (this.Params.Output[ind] is IGH_PreviewObject && !this.Hidden)
            {
                return true;
            }
            if (this.Params.Output[ind].Recipients.Count != 0)
            {
                return true;
            }
            return false;
        }

        public bool OutputInUse()
        {
            for (int i = 0; i < this.Params.Output.Count; i++)
            {
                if (OutputInUse(i))
                {
                    return true;
                }
            }
            return false;
        }

        public string GetMenuDescription()
        {
            return ((GH_ExtendableComponentAttributes)this.Attributes).GetMenuDescription();
        }
    }
}
