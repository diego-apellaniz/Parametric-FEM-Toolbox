using Grasshopper.Kernel;
using Rhino;
using System.Collections.Generic;
using System.Drawing;

namespace RFEM_daq.UIWidgets
{
    public class RuntimeComponentData
    {
        private GH_SwitcherComponent component;

        private string cachedName;

        private string cachedNickname;

        private string cachedDescription;

        private Bitmap cachedIcon;

        private List<IGH_Param> stcInputs;

        private List<IGH_Param> stcOutputs;

        private List<IGH_Param> dynInputs;

        private List<IGH_Param> dynOutputs;

        public string CachedName => cachedName;

        public string CachedNickname => cachedNickname;

        public string CachedDescription => cachedDescription;

        public Bitmap CachedIcon => cachedIcon;

        public List<IGH_Param> StaticInputs => stcInputs;

        public List<IGH_Param> StaticOutputs => stcOutputs;

        public List<IGH_Param> DynamicInputs => dynInputs;

        public List<IGH_Param> DynamicOutputs => dynOutputs;

        public RuntimeComponentData(GH_SwitcherComponent component)
        {
            this.component = component;
            cachedName = component.Name;
            cachedNickname = component.NickName;
            cachedDescription = component.Description;
            cachedIcon = component.Icon_24x24;
            dynInputs = new List<IGH_Param>();
            dynOutputs = new List<IGH_Param>();
            stcInputs = new List<IGH_Param>();
            stcOutputs = new List<IGH_Param>();
            foreach (IGH_Param item in component.Params.Input)
            {
                stcInputs.Add(item);
            }
            foreach (IGH_Param item2 in component.Params.Output)
            {
                stcOutputs.Add(item2);
            }
        }

        private void UnregisterParameter(IGH_Param param, bool input, bool isolate)
        {
            if (input)
            {
                component.Params.UnregisterInputParameter(param, isolate);
            }
            else
            {
                component.Params.UnregisterOutputParameter(param, isolate);
            }
            if (param.Attributes != null)
            {
                if (param.Attributes.Parent != null)
                {
                    RhinoApp.WriteLine("This should not happen");
                }
                param.Attributes.Parent = component.Attributes;
            }
        }

        public void PrepareWrite(EvaluationUnit unit)
        {
            if (unit != null)
            {
                foreach (ExtendedPlug input in unit.Inputs)
                {
                    UnregisterParameter(input.Parameter, input: true, isolate: false);
                }
                foreach (ExtendedPlug output in unit.Outputs)
                {
                    UnregisterParameter(output.Parameter, input: false, isolate: false);
                }
            }
        }

        public void RestoreWrite(EvaluationUnit unit)
        {
            if (unit != null)
            {
                foreach (ExtendedPlug input in unit.Inputs)
                {
                    component.Params.RegisterInputParam(input.Parameter);
                }
                foreach (ExtendedPlug output in unit.Outputs)
                {
                    component.Params.RegisterOutputParam(output.Parameter);
                }
            }
        }

        public void UnregisterUnit(EvaluationUnit unit)
        {
            stcInputs.Clear();
            stcOutputs.Clear();
            dynInputs.Clear();
            dynOutputs.Clear();
            foreach (ExtendedPlug input in unit.Inputs)
            {
                UnregisterParameter(input.Parameter, input: true, isolate: true);
            }
            foreach (ExtendedPlug output in unit.Outputs)
            {
                UnregisterParameter(output.Parameter, input: false, isolate: true);
            }
            foreach (IGH_Param item in component.Params.Input)
            {
                stcInputs.Add(item);
            }
            foreach (IGH_Param item2 in component.Params.Output)
            {
                stcOutputs.Add(item2);
            }
        }

        public void RegisterUnit(EvaluationUnit unit)
        {
            stcInputs.Clear();
            stcOutputs.Clear();
            dynInputs.Clear();
            dynOutputs.Clear();
            foreach (IGH_Param item in component.Params.Input)
            {
                stcInputs.Add(item);
            }
            foreach (IGH_Param item2 in component.Params.Output)
            {
                stcOutputs.Add(item2);
            }
            foreach (ExtendedPlug input in unit.Inputs)
            {
                component.Params.RegisterInputParam(input.Parameter);
                if (input.IsMenu)
                {
                    dynInputs.Add(input.Parameter);
                }
                else
                {
                    stcInputs.Add(input.Parameter);
                }
            }
            foreach (ExtendedPlug output in unit.Outputs)
            {
                component.Params.RegisterOutputParam(output.Parameter);
                if (output.IsMenu)
                {
                    dynOutputs.Add(output.Parameter);
                }
                else
                {
                    stcOutputs.Add(output.Parameter);
                }
            }
        }

        public List<IGH_Param> GetComponentInputs()
        {
            List<IGH_Param> list = new List<IGH_Param>(stcInputs);
            list.AddRange(dynInputs);
            return list;
        }

        public List<IGH_Param> GetComponentInputSection()
        {
            return stcInputs;
        }

        public List<IGH_Param> GetComponentOutputs()
        {
            List<IGH_Param> list = new List<IGH_Param>(stcOutputs);
            list.AddRange(dynOutputs);
            return list;
        }

        public List<IGH_Param> GetComponentOutputSection()
        {
            return stcOutputs;
        }
    }
}
