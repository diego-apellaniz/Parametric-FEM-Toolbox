using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;

namespace RFEM_daq.UIWidgets
{
    public class EvaluationUnit
    {
        private string name;
        private string displayName;
        private string description;
        private List<ExtendedPlug> inputs;
        private List<ExtendedPlug> outputs;
        private bool active;
        private Bitmap icon;
        private bool keepLinks;
        private EvaluationUnitContext cxt;
        private GH_SwitcherComponent compontent;
        public GH_SwitcherComponent Component
        {
            get
            {
                return compontent;
            }
            set
            {
                compontent = value;
            }
        }
        public List<ExtendedPlug> Inputs => inputs;
        public List<ExtendedPlug> Outputs => outputs;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        public string DisplayName
        {
            get
            {
                return displayName;
            }
            set
            {
                displayName = value;
            }
        }
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
            }
        }
        public bool Active
        {
            get
            {
                return active;
            }
            set
            {
                active = value;
            }
        }
        public bool KeepLinks
        {
            get
            {
                return keepLinks;
            }
            set
            {
                keepLinks = value;
            }
        }
        public Bitmap Icon
        {
            get
            {
                return icon;
            }
            set
            {
                icon = value;
            }
        }
        public EvaluationUnitContext Context => cxt;

        public EvaluationUnit(string name, string displayName, string description, Bitmap icon = null)
        {
            this.name = name;
            this.displayName = displayName;
            this.description = description;
            inputs = new List<ExtendedPlug>();
            outputs = new List<ExtendedPlug>();
            keepLinks = false;
            this.icon = icon;
            cxt = new EvaluationUnitContext(this);
        }
        private static Type GetGenericType(Type generic, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                Type right = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == right)
                {
                    return toCheck;
                }
                toCheck = toCheck.BaseType;
            }
            return null;
        }
        public void RegisterInputParam(IGH_Param param, string name, string nickName, string description, GH_ParamAccess access, IGH_Goo defaultValue)
        {            
            param.Name = name;
            param.NickName = nickName;
            param.Description = description;
            param.Access = access;
            try
            {
                if (defaultValue != null && typeof(IGH_Goo).IsAssignableFrom(param.GetType()))
                {
                    //Type genericType = GetGenericType(typeof(GH_PersistentParam), ((object)param).GetType());
                    Type genericType = GetGenericType(typeof(GH_PersistentParam<>), ((object)param).GetType());
                    if (genericType != null)
                    {
                        Type[] genericArguments = genericType.GetGenericArguments();
                        if (genericArguments.Length != 0)
                        {
                            Type type = genericArguments[0];
                            genericType.GetMethod("SetPersistentData", BindingFlags.Instance | BindingFlags.Public, null, new Type[1]
                            {
                            genericArguments[0]
                            }, null).Invoke(param, new object[1]
                            {
                            defaultValue
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            ExtendedPlug extendedPlug = new ExtendedPlug(param);
            extendedPlug.Unit = this;
            inputs.Add(extendedPlug);
        }

        public void RegisterInputParam(IGH_Param param, string name, string nickName, string description, GH_ParamAccess access)
        {
              RegisterInputParam(param, name, nickName, description, access, null);
        }

        public void RegisterOutputParam(IGH_Param param, string name, string nickName, string description)
        {
            param.Name = name;
            param.NickName = nickName;
            param.Description = description;
            ExtendedPlug extendedPlug = new ExtendedPlug(param);
            outputs.Add(extendedPlug);
        }

        public void NewParameterIds()
        {
            foreach (ExtendedPlug input in inputs)
            {
                input.Parameter.NewInstanceGuid();
            }
            foreach (ExtendedPlug output in outputs)
            {
                output.Parameter.NewInstanceGuid();
            }
        }

        public void AddMenu(GH_ExtendableMenu menu)
        {
            Context.Collection.AddMenu(menu);
        }

        public bool Write(GH_IWriter writer)
        {
            writer.SetString("name", Name);
            GH_IWriter val = writer.CreateChunk("params");
            GH_IWriter val2 = val.CreateChunk("input");
            val2.SetInt32("index", 0);
            val2.SetInt32("count", Inputs.Count);
            for (int i = 0; i < inputs.Count; i++)
            {
                if (inputs[i].Parameter.Attributes != null)
                {
                    GH_IWriter val3 = val2.CreateChunk("p", i);
                    inputs[i].Parameter.Write(val3);
                }
            }
            GH_IWriter val4 = val.CreateChunk("output");
            val4.SetInt32("index", 0);
            val4.SetInt32("count", Outputs.Count);
            for (int j = 0; j < outputs.Count; j++)
            {
                if (outputs[j].Parameter.Attributes != null)
                {
                    GH_IWriter val5 = val4.CreateChunk("p", j);
                    outputs[j].Parameter.Write(val5);
                }
            }
            GH_IWriter writer2 = writer.CreateChunk("context");
            return cxt.Collection.Write(writer2);
        }

        public bool Read(GH_IReader reader)
        {
            if (reader.ChunkExists("params"))
            {
                GH_IReader val = reader.FindChunk("params");
                if (val.ChunkExists("input") && inputs != null)
                {
                    GH_IReader val2 = val.FindChunk("input");
                    int num = -1;
                    if (val2.TryGetInt32("count", ref num) && inputs.Count == num)
                    {
                        for (int i = 0; i < num; i++)
                        {
                            if (val2.ChunkExists("p", i))
                            {
                                inputs[i].Parameter.Read(val2.FindChunk("p", i));
                            }
                            else if (val2.ChunkExists("param", i))
                            {
                                inputs[i].Parameter.Read(val2.FindChunk("param", i));
                            }
                        }
                    }
                }
                if (val.ChunkExists("output") && outputs != null)
                {
                    GH_IReader val3 = val.FindChunk("output");
                    int num2 = -1;
                    if (val3.TryGetInt32("count", ref num2) && outputs.Count == num2)
                    {
                        for (int j = 0; j < num2; j++)
                        {
                            if (val3.ChunkExists("p", j))
                            {
                                outputs[j].Parameter.Read(val3.FindChunk("p", j));
                            }
                            else if (val3.ChunkExists("param", j))
                            {
                                outputs[j].Parameter.Read(val3.FindChunk("param", j));
                            }
                        }
                    }
                }
            }
            try
            {
                GH_IReader val4 = reader.FindChunk("context");
                if (val4 != null)
                {
                    cxt.Collection.Read(val4);
                }
            }
            catch (Exception ex)
            {
                RhinoApp.WriteLine("Component error:" + ex.Message + "\n" + ex.StackTrace);
            }
            return true;
        }

        public string GetMenuDescription()
        {
            return Context.Collection.GetMenuDescription();
        }


    }
}
