using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Rhino;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace Parametric_FEM_Toolbox.UIWidgets
{
    public abstract class GH_SwitcherComponent : GH_Component
    {
        protected EvaluationUnitManager evalUnits;

        protected EvaluationUnit activeUnit;

        protected RuntimeComponentData staticData;

        public RuntimeComponentData StaticData => staticData;

        public List<EvaluationUnit> EvalUnits => evalUnits.Units;

        public EvaluationUnit ActiveUnit => activeUnit;

        protected virtual string DefaultEvaluationUnit => null;

        public virtual string UnitMenuName => "Evaluation Units";

        public virtual string UnitMenuHeader => "Select evaluation unit";

        public virtual bool UnitlessExistence => false;


        protected internal GH_SwitcherComponent(string sName, string sAbbreviation, string sDescription, string sCategory, string sSubCategory)
            : base(sName, sAbbreviation, sDescription, sCategory, sSubCategory)
        {
            this.Phase = GH_SolutionPhase.Blank;
            SetupEvaluationUnits();
        }

        protected override void PostConstructor()
        {
            evalUnits = new EvaluationUnitManager(this);
            RegisterEvaluationUnits(evalUnits);
            base.PostConstructor();
            staticData = new RuntimeComponentData(this);
        }

        private void SetupEvaluationUnits()
        {
            if (activeUnit != null)
            {
                throw new ArgumentException("Invalid switcher state. No evaluation unit must be active at this point.");
            }
            EvaluationUnit evaluationUnit = GetUnit(DefaultEvaluationUnit);
            if (evaluationUnit == null && !UnitlessExistence)
            {
                if (EvalUnits.Count == 0)
                {
                    throw new ArgumentException("Switcher has no evaluation units registered and UnitlessExistence is false.");
                }
                evaluationUnit = EvalUnits[0];
            }
            if (this.OnPingDocument() != null)
            {
                RhinoApp.WriteLine("Component belongs to a document at a stage where it should not belong to one.");
            }
            SwitchUnit(evaluationUnit, recompute: false, recordEvent: false);
        }

        public EvaluationUnit GetUnit(string name)
        {
            return evalUnits.GetUnit(name);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            SolveInstance(DA, activeUnit);
        }

        protected abstract void SolveInstance(IGH_DataAccess DA, EvaluationUnit unit);

        public override void AppendAdditionalMenuItems(ToolStripDropDown menu)
        {
            base.AppendAdditionalMenuItems(menu);
            if (evalUnits.Units.Count > 0)
            {
                GH_DocumentObject.Menu_AppendSeparator((ToolStrip)menu);
                ToolStripMenuItem toolStripMenuItem = GH_DocumentObject.Menu_AppendItem((ToolStrip)menu, "Units");
                foreach (EvaluationUnit unit in evalUnits.Units)
                {
                    GH_DocumentObject.Menu_AppendItem((ToolStrip)toolStripMenuItem.DropDown, unit.Name, (EventHandler)Menu_ActivateUnit, (Image)null, true, unit.Active).Tag = unit;
                }
                GH_DocumentObject.Menu_AppendSeparator((ToolStrip)menu);
            }
        }

        private void Menu_ActivateUnit(object sender, EventArgs e)
        {
            try
            {
                EvaluationUnit evaluationUnit = (EvaluationUnit)((ToolStripMenuItem)sender).Tag;
                if (evaluationUnit != null)
                {
                    SwitchUnit(evaluationUnit);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SetReadState()
        {
            if (activeUnit != null)
            {
                staticData.UnregisterUnit(activeUnit);
                activeUnit.Active = false;
                activeUnit = null;
            }
            GH_Document val = this.OnPingDocument();
            if (val != null)
            {
                val.Modified();
                val.DestroyAttributeCache();
                val.DestroyObjectTable();
            }
            if (this.Attributes != null)
            {
                ((GH_SwitcherComponentAttributes)this.Attributes).OnSwitchUnit();
            }
            this.Name = staticData.CachedName;
            this.NickName = staticData.CachedNickname;
            this.Description = staticData.CachedDescription;
            this.SetIconOverride(staticData.CachedIcon);
            if (this.Attributes != null)
            {
                this.Attributes.ExpireLayout();
            }
        }

        public void ClearUnit(bool recompute = true, bool recordEvent = true)
        {
            if (!UnitlessExistence)
            {
                return;
            }
            if (activeUnit != null)
            {
                if (recordEvent)
                {
                    this.RecordUndoEvent("Switch Unit", new GH_SwitchAction(this, null));
                }
                staticData.UnregisterUnit(activeUnit);
                activeUnit.Active = false;
                activeUnit = null;
            }
            GH_Document val = this.OnPingDocument();
            if (val != null)
            {
                val.Modified();
                val.DestroyAttributeCache();
                val.DestroyObjectTable();
            }
            if (this.Attributes != null)
            {
                ((GH_SwitcherComponentAttributes)this.Attributes).OnSwitchUnit();
            }
            this.Name = staticData.CachedName;
            this.NickName = staticData.CachedNickname;
            this.Description = staticData.CachedDescription;
            this.SetIconOverride(staticData.CachedIcon);
            if (this.Attributes != null)
            {
                this.Attributes.ExpireLayout();
            }
            if (recompute)
            {
                this.ExpireSolution(true);
            }
        }

        public virtual void SwitchUnit(string unitName, bool recompute = true, bool recordEvent = true)
        {
            EvaluationUnit unit = evalUnits.GetUnit(unitName);
            if (unit != null)
            {
                SwitchUnit(unit, recompute, recordEvent);
            }
        }

        protected virtual void SwitchUnit(EvaluationUnit unit, bool recompute = true, bool recordEvent = true)
        {
            if (unit != null && (activeUnit == null || activeUnit != unit))
            {
                if (recordEvent)
                {
                    this.RecordUndoEvent("Switch Unit", new GH_SwitchAction(this, unit.Name));
                }
                if (activeUnit != null)
                {
                    staticData.UnregisterUnit(activeUnit);
                    activeUnit.Active = false;
                    activeUnit = null;
                }
                staticData.RegisterUnit(unit);
                activeUnit = unit;
                activeUnit.Active = true;
                GH_Document val = this.OnPingDocument();
                if (val != null)
                {
                    val.Modified();
                    val.DestroyAttributeCache();
                    val.DestroyObjectTable();
                }
                if (this.Attributes != null)
                {
                    ((GH_SwitcherComponentAttributes)this.Attributes).OnSwitchUnit();
                }
                if (unit.DisplayName != null)
                {
                    this.SetIconOverride(unit.Icon);
                }
                if (this.Attributes != null)
                {
                    this.Attributes.ExpireLayout();
                }
                if (recompute)
                {
                    this.ExpireSolution(true);
                }
            }
        }

        protected virtual void RegisterEvaluationUnits(EvaluationUnitManager mngr)
        {
        }

        private void _Setup()
        {
            Setup((GH_SwitcherComponentAttributes)this.Attributes);
        }

        protected virtual void Setup(GH_SwitcherComponentAttributes attr)
        {
        }

        protected virtual void OnComponentLoaded()
        {
        }

        protected virtual void OnComponentReset(GH_ExtendableComponentAttributes attr)
        {
        }

        public override bool Write(GH_IWriter writer)
        {
            staticData.PrepareWrite(activeUnit);
            bool result = base.Write(writer);
            staticData.RestoreWrite(activeUnit);
            if (activeUnit != null)
            {
                writer.CreateChunk("ActiveUnit").SetString("unitname", activeUnit.Name);
            }
            try
            {
                GH_IWriter val = writer.CreateChunk("EvalUnits");
                val.SetInt32("count", evalUnits.Units.Count);
                for (int i = 0; i < evalUnits.Units.Count; i++)
                {
                    EvaluationUnit evaluationUnit = evalUnits.Units[i];
                    GH_IWriter writer2 = val.CreateChunk("unit", i);
                    evaluationUnit.Write(writer2);
                }
                return result;
            }
            catch (Exception ex)
            {
                RhinoApp.WriteLine(ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
        }

        public override bool Read(GH_IReader reader)
        {
            bool flag = true;
            SetReadState();
            flag &= base.Read(reader);
            string text = null;
            if (reader.ChunkExists("ActiveUnit"))
            {
                text = reader.FindChunk("ActiveUnit").GetString("unitname");
            }
            if (reader.ChunkExists("EvalUnits"))
            {
                GH_IReader val = reader.FindChunk("EvalUnits");
                int num = -1;
                if (val.TryGetInt32("count", ref num))
                {
                    for (int i = 0; i < num; i++)
                    {
                        if (val.ChunkExists("unit", i))
                        {
                            GH_IReader val2 = val.FindChunk("unit", i);
                            string @string = val2.GetString("name");
                            if (text != null)
                            {
                                @string.Equals(text);
                            }
                            evalUnits.GetUnit(@string)?.Read(val2);
                        }
                    }
                }
            }
            if (text != null)
            {
                GetUnit(text);
                SwitchUnit(GetUnit(text), recompute: false, recordEvent: false);
            }
            for (int j = 0; j < EvalUnits.Count; j++)
            {
                if (!EvalUnits[j].Active)
                {
                    EvalUnits[j].NewParameterIds();
                }
            }
            OnComponentLoaded();
            return flag;
        }

        public override void CreateAttributes()
        {
            Setup((GH_SwitcherComponentAttributes)(base.m_attributes = new GH_SwitcherComponentAttributes(this)));
        }

        // daq - GH_ValueList from Enum
        protected override void BeforeSolveInstance()
        {
            foreach (var unit in EvalUnits)
            {
                foreach (var item in unit.Inputs)
                {
                    // Provide the possibility of a drop-down list in case of enumeration inputs:
                    if (item.EnumInput != null && item.Parameter.SourceCount == 1 && item.Parameter.Sources[0] is GH_ValueList)
                    {
                        GH_ValueList val = item.Parameter.Sources[0] as GH_ValueList;
                        val.ListMode = GH_ValueListMode.DropDown;
                        // We just want to reset the list once!!!
                        if (val.ListItems.Count != item.EnumInput.Count || !(val.ListItems.Select(x => x.Name).SequenceEqual(item.EnumInput)))
                        {
                            var counter = 0;
                            val.ListItems.Clear();
                            foreach (var input in item.EnumInput)
                            {
                                val.ListItems.Add(new GH_ValueListItem(input, counter.ToString()));
                                counter++;
                            }
                            val.ExpireSolution(true);
                        }
                    }
                }
            }

        }
    }
}
