using GH_IO.Serialization;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using Grasshopper.Kernel.Types;
using Rhino;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;
using static Grasshopper.GUI.Canvas.GH_Canvas;

namespace Parametric_FEM_Toolbox.UIWidgets
{
    public class GH_SwitcherComponentAttributes : GH_ComponentAttributes
    {
        private int offset;

        private float _minWidth;

        private GH_Attr_Widget _activeToolTip;

        protected GH_MenuCollection unitMenuCollection;

        protected GH_MenuCollection collection;

        private GH_MenuCollection composedCollection;

        private MenuDropDown _UnitDrop;

        public float MinWidth
        {
            get
            {
                return _minWidth;
            }
            set
            {
                _minWidth = value;
            }
        }

        public GH_SwitcherComponentAttributes(GH_SwitcherComponent component)
            : base(component)
        {
            collection = new GH_MenuCollection();
            composedCollection = new GH_MenuCollection();
            composedCollection.Merge(collection);
            CreateUnitDropDown();
            InitializeUnitParameters();
        }

        public void InitializeUnitParameters()
        {
            GH_SwitcherComponent gH_SwitcherComponent = (GH_SwitcherComponent)base.Owner;
            if (gH_SwitcherComponent.EvalUnits != null)
            {
                foreach (EvaluationUnit evalUnit in gH_SwitcherComponent.EvalUnits)
                {
                    foreach (ExtendedPlug input in evalUnit.Inputs)
                    {
                        if (input.Parameter.Attributes == null)
                        {
                            input.Parameter.Attributes = (new GH_LinkedParamAttributes(input.Parameter, this));
                        }
                    }
                    foreach (ExtendedPlug output in evalUnit.Outputs)
                    {
                        if (output.Parameter.Attributes == null)
                        {
                            output.Parameter.Attributes = (new GH_LinkedParamAttributes(output.Parameter, this));
                        }
                    }
                }
            }
        }

        private void ComposeMenu()
        {
            GH_SwitcherComponent gH_SwitcherComponent = (GH_SwitcherComponent)base.Owner;
            composedCollection = new GH_MenuCollection();
            EvaluationUnit activeUnit = gH_SwitcherComponent.ActiveUnit;
            if (activeUnit != null && activeUnit.Context.Collection != null)
            {
                composedCollection.Merge(gH_SwitcherComponent.ActiveUnit.Context.Collection);
            }
            if (collection != null)
            {
                composedCollection.Merge(collection);
            }
            if (unitMenuCollection != null)
            {
                composedCollection.Merge(unitMenuCollection);
            }
        }

        protected void CreateUnitDropDown()
        {
            GH_SwitcherComponent gH_SwitcherComponent = (GH_SwitcherComponent)base.Owner;
            if (gH_SwitcherComponent.EvalUnits != null && gH_SwitcherComponent.EvalUnits.Count != 0 && (gH_SwitcherComponent.EvalUnits.Count != 1 || gH_SwitcherComponent.UnitlessExistence))
                {
                MenuPanel menuPanel = new MenuPanel(0, "panel_units");
                menuPanel.Header = "Unit selection";
                string text = gH_SwitcherComponent.UnitMenuName;
                if (text == null)
                {
                    text = "Evaluation Units";
                }
                string text2 = gH_SwitcherComponent.UnitMenuHeader;
                if (text2 == null)
                {
                    text2 = "Select evaluation unit";
                }
                unitMenuCollection = new GH_MenuCollection();
                GH_ExtendableMenu gH_ExtendableMenu = new GH_ExtendableMenu(0, "menu_units");
                gH_ExtendableMenu.Name = text;
                gH_ExtendableMenu.Header = text2;
                gH_ExtendableMenu.AddControl(menuPanel);
                _UnitDrop = new MenuDropDown(0, "dropdown_units", "units");
                _UnitDrop.VisibleItemCount = 10;
                _UnitDrop.ValueChanged += _UnitDrop__valueChanged;
                _UnitDrop.Header = "Evaluation unit selector";
                menuPanel.AddControl(_UnitDrop);
                List<EvaluationUnit> evalUnits = gH_SwitcherComponent.EvalUnits;
                if (gH_SwitcherComponent.UnitlessExistence)
                {
                    _UnitDrop.AddItem("--NONE--", null);
                }
                for (int i = 0; i < evalUnits.Count; i++)
                {
                    _UnitDrop.AddItem(evalUnits[i].Name, evalUnits[i].DisplayName, evalUnits[i]);
                }
                gH_ExtendableMenu.Expand();
                unitMenuCollection.AddMenu(gH_ExtendableMenu);
            }
        }

        private void _UnitDrop__valueChanged(object sender, EventArgs e)
        {
            try
            {
                MenuDropDown menuDropDown = (MenuDropDown)sender;
                MenuDropDown.Entry entry = menuDropDown.Items[menuDropDown.Value];
                if (entry.data != null)
                {
                    EvaluationUnit evaluationUnit = (EvaluationUnit)entry.data;
                    ((GH_SwitcherComponent)base.Owner).SwitchUnit(evaluationUnit.Name);
                }
                else
                {
                    ((GH_SwitcherComponent)base.Owner).ClearUnit();
                }
            }
            catch (Exception ex)
            {
                RhinoApp.WriteLine("Error selection:" + ex.StackTrace);
            }
        }

        public void OnSwitchUnit()
        {
            EvaluationUnit activeUnit = ((GH_SwitcherComponent)base.Owner).ActiveUnit;
            ComposeMenu();
            if (activeUnit != null)
            {
                if (_UnitDrop != null)
                {
                    int num = _UnitDrop.FindIndex(activeUnit.Name);
                    if (num != -1)
                    {
                        _UnitDrop.Value = num;
                    }
                }
            }
            else if (((GH_SwitcherComponent)base.Owner).UnitlessExistence && _UnitDrop != null)
            {
                _UnitDrop.Value = 0;
            }
        }

        public void AddMenu(GH_ExtendableMenu menu)
        {
            collection.AddMenu(menu);
        }

        public override bool Write(GH_IWriter writer)
        {
            try
            {
                collection.Write(writer);
            }
            catch (Exception)
            {
            }
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            try
            {
                collection.Read(reader);
            }
            catch (Exception)
            {
            }
            return base.Read(reader);
        }

        protected override void PrepareForRender(Grasshopper.GUI.Canvas.GH_Canvas canvas)
        {
            base.PrepareForRender(canvas);
            LayoutMenuCollection();
        }

        protected void LayoutBaseComponent()
        {
            GH_SwitcherComponent gH_SwitcherComponent = (GH_SwitcherComponent)base.Owner;
            this.Pivot = ((PointF)GH_Convert.ToPoint(this.Pivot));
            base.m_innerBounds = LayoutComponentBox2(base.Owner);
            int num = ComputeW_ico(base.Owner);
            float width = composedCollection.GetMinLayoutSize().Width;
            float num2 = Math.Max(MinWidth, width);
            int add_offset = 0;
            if (num2 > (float)num)
            {
                add_offset = (int)((double)(num2 - (float)num) / 2.0);
            }
            LayoutInputParams2(base.Owner, base.m_innerBounds, add_offset);
            LayoutOutputParams2(base.Owner, base.m_innerBounds, add_offset);
            this.Bounds = (LayoutBounds2(base.Owner, base.m_innerBounds));
        }

        private int ComputeW_ico(IGH_Component owner)
        {
            GH_SwitcherComponent gH_SwitcherComponent = (GH_SwitcherComponent)owner;
            int num = 24;
            int num2 = 0;
            int num3 = 0;
            foreach (IGH_Param componentInput in gH_SwitcherComponent.StaticData.GetComponentInputs())
            {
                int val = ((List<IGH_StateTag>)componentInput.StateTags).Count * 20;
                num3 = Math.Max(num3, val);
                num2 = Math.Max(num2, GH_FontServer.StringWidth(componentInput.NickName, StandardFont.font()));
            }
            num2 = Math.Max(num2 + 6, 12);
            num2 += num3;
            int num4 = 0;
            int num5 = 0;
            foreach (IGH_Param componentOutput in gH_SwitcherComponent.StaticData.GetComponentOutputs())
            {
                int val2 = ((List<IGH_StateTag>)componentOutput.StateTags).Count * 20;
                num5 = Math.Max(num5, val2);
                num4 = Math.Max(num4, GH_FontServer.StringWidth(componentOutput.NickName, StandardFont.font()));
            }
            num4 = Math.Max(num4 + 6, 12);
            num4 += num5;
            return num2 + num + num4 + 6;
        }

        public RectangleF LayoutBounds2(IGH_Component owner, RectangleF bounds)
        {
            GH_SwitcherComponent gH_SwitcherComponent = (GH_SwitcherComponent)owner;
            foreach (IGH_Param item in gH_SwitcherComponent.StaticData.GetComponentInputSection())
            {
                bounds = RectangleF.Union(bounds, item.Attributes.Bounds);
            }
            foreach (IGH_Param item2 in gH_SwitcherComponent.StaticData.GetComponentOutputSection())
            {
                bounds = RectangleF.Union(bounds, item2.Attributes.Bounds);
            }
            bounds.Inflate(2f, 2f);
            return bounds;
        }

        public RectangleF LayoutComponentBox2(IGH_Component owner)
        {
            GH_SwitcherComponent gH_SwitcherComponent = (GH_SwitcherComponent)owner;
            int val = Math.Max(gH_SwitcherComponent.StaticData.GetComponentInputSection().Count, gH_SwitcherComponent.StaticData.GetComponentOutputSection().Count) * 20;
            val = Math.Max(val, 24);
            int num = 24;
            if (!GH_Attributes<IGH_Component>.IsIconMode(owner.IconDisplayMode))
            {
                val = Math.Max(val, GH_Convert.ToSize((SizeF)GH_FontServer.MeasureString(owner.NickName, StandardFont.largeFont())).Width + 6);
            }
            return GH_Convert.ToRectangle(new RectangleF(owner.Attributes.Pivot.X - 0.5f * (float)num, owner.Attributes.Pivot.Y - 0.5f * (float)val, num, val));
        }

        public void LayoutInputParams2(IGH_Component owner, RectangleF componentBox, int add_offset)
        {
            GH_SwitcherComponent gH_SwitcherComponent = (GH_SwitcherComponent)owner;
            List<IGH_Param> componentInputSection = gH_SwitcherComponent.StaticData.GetComponentInputSection();
            int count = componentInputSection.Count;
            if (count == 0)
            {
                return;
            }
            int num = 0;
            int num2 = 0;
            foreach (IGH_Param componentInput in gH_SwitcherComponent.StaticData.GetComponentInputs())
            {
                int val = ((List<IGH_StateTag>)componentInput.StateTags).Count * 20;
                num2 = Math.Max(num2, val);
                num = Math.Max(num, GH_FontServer.StringWidth(componentInput.NickName, StandardFont.font()));
            }
            num = Math.Max(num + 6, 12);
            num += num2 + add_offset;
            float num3 = componentBox.Height / (float)count;
            for (int i = 0; i < count; i++)
            {
                IGH_Param val2 = componentInputSection[i];
                if (val2.Attributes == null)
                {
                    val2.Attributes = (new GH_LinkedParamAttributes(val2, owner.Attributes));
                }
                float num4 = componentBox.X - (float)num;
                float num5 = componentBox.Y + (float)i * num3;
                float width = num - 3;
                float height = num3;
                PointF pivot = new PointF(num4 + 0.5f * (float)num, num5 + 0.5f * num3);
                val2.Attributes.Pivot = (pivot);
                RectangleF rectangleF = new RectangleF(num4, num5, width, height);
                val2.Attributes.Bounds = ((RectangleF)GH_Convert.ToRectangle(rectangleF));
            }
            bool flag = false;
            for (int j = 0; j < count; j++)
            {
                IGH_Param val3 = componentInputSection[j];
                GH_LinkedParamAttributes val4 = (GH_LinkedParamAttributes)val3.Attributes;
                FieldInfo field = typeof(GH_LinkedParamAttributes).GetField("m_renderTags", BindingFlags.Instance | BindingFlags.NonPublic);
                GH_StateTagList val5 = val3.StateTags;
                if (!(field != null))
                {
                    continue;
                }
                if (((List<IGH_StateTag>)val5).Count == 0)
                {
                    val5 = null;
                    field.SetValue(val4, val5);
                }
                if (val5 != null)
                {
                    flag = true;
                    Rectangle rectangle = GH_Convert.ToRectangle(val4.Bounds);
                    rectangle.X += num2;
                    rectangle.Width -= num2;
                    val5.Layout(rectangle, 0);
                    rectangle = val5.BoundingBox;
                    if (!rectangle.IsEmpty)
                    {
                        val4.Bounds = (RectangleF.Union(val4.Bounds, rectangle));
                    }
                    field.SetValue(val4, val5);
                }
            }
            if (flag)
            {
                float num6 = float.MaxValue;
                for (int k = 0; k < count; k++)
                {
                    IGH_Attributes attributes = componentInputSection[k].Attributes;
                    num6 = Math.Min(num6, attributes.Bounds.X);
                }
                for (int l = 0; l < count; l++)
                {
                    IGH_Attributes attributes2 = componentInputSection[l].Attributes;
                    RectangleF bounds = attributes2.Bounds;
                    bounds.Width = bounds.Right - num6;
                    bounds.X = num6;
                    attributes2.Bounds = (bounds);
                }
            }
        }

        public void LayoutOutputParams2(IGH_Component owner, RectangleF componentBox, int add_offset)
        {
            GH_SwitcherComponent gH_SwitcherComponent = (GH_SwitcherComponent)owner;
            List<IGH_Param> componentOutputSection = gH_SwitcherComponent.StaticData.GetComponentOutputSection();
            int count = componentOutputSection.Count;
            if (count == 0)
            {
                return;
            }
            int num = 0;
            int num2 = 0;
            foreach (IGH_Param componentOutput in gH_SwitcherComponent.StaticData.GetComponentOutputs())
            {
                int val = ((List<IGH_StateTag>)componentOutput.StateTags).Count * 20;
                num2 = Math.Max(num2, val);
                num = Math.Max(num, GH_FontServer.StringWidth(componentOutput.NickName, StandardFont.font()));
            }
            num = Math.Max(num + 6, 12);
            num += num2 + add_offset;
            float num3 = componentBox.Height / (float)count;
            for (int i = 0; i < count; i++)
            {
                IGH_Param val2 = componentOutputSection[i];
                if (val2.Attributes == null)
                {
                    val2.Attributes = (new GH_LinkedParamAttributes(val2, owner.Attributes));
                }
                float num4 = componentBox.Right + 3f;
                float num5 = componentBox.Y + (float)i * num3;
                float width = num;
                float height = num3;
                PointF pivot = new PointF(num4 + 0.5f * (float)num, num5 + 0.5f * num3);
                val2.Attributes.Pivot = (pivot);
                RectangleF rectangleF = new RectangleF(num4, num5, width, height);
                val2.Attributes.Bounds = ((RectangleF)GH_Convert.ToRectangle(rectangleF));
            }
            bool flag = false;
            for (int j = 0; j < count; j++)
            {
                IGH_Param val3 = componentOutputSection[j];
                GH_LinkedParamAttributes val4 = (GH_LinkedParamAttributes)val3.Attributes;
                FieldInfo field = typeof(GH_LinkedParamAttributes).GetField("m_renderTags", BindingFlags.Instance | BindingFlags.NonPublic);
                GH_StateTagList val5 = val3.StateTags;
                if (!(field != null))
                {
                    continue;
                }
                if (((List<IGH_StateTag>)val5).Count == 0)
                {
                    val5 = null;
                    field.SetValue(val4, val5);
                }
                if (val5 != null)
                {
                    flag = true;
                    Rectangle rectangle = GH_Convert.ToRectangle(val4.Bounds);
                    rectangle.Width -= num2;
                    val5.Layout(rectangle, GH_StateTagLayoutDirection.Right);
                    rectangle = val5.BoundingBox;
                    if (!rectangle.IsEmpty)
                    {
                        val4.Bounds = (RectangleF.Union(val4.Bounds, rectangle));
                    }
                    field.SetValue(val4, val5);
                }
            }
            if (flag)
            {
                float num6 = float.MinValue;
                for (int k = 0; k < count; k++)
                {
                    IGH_Attributes attributes = componentOutputSection[k].Attributes;
                    num6 = Math.Max(num6, attributes.Bounds.Right);
                }
                for (int l = 0; l < count; l++)
                {
                    IGH_Attributes attributes2 = componentOutputSection[l].Attributes;
                    RectangleF bounds = attributes2.Bounds;
                    bounds.Width = num6 - bounds.X;
                    attributes2.Bounds = (bounds);
                }
            }
        }

        protected override void Layout()
        {
            this.Pivot = ((PointF)GH_Convert.ToPoint(this.Pivot));
            LayoutBaseComponent();
            GH_SwitcherComponent gH_SwitcherComponent = (GH_SwitcherComponent)base.Owner;
            List<ExtendedPlug> inputs = new List<ExtendedPlug>();
            List<ExtendedPlug> outputs = new List<ExtendedPlug>();
            composedCollection.GetMenuPlugs(ref inputs, ref outputs, onlyVisible: true);
            LayoutMenuInputs(base.m_innerBounds);
            LayoutMenuOutputs(base.m_innerBounds);
            this.Bounds = (LayoutExtBounds(base.m_innerBounds, inputs, outputs));
            FixLayout(outputs);
            LayoutMenu();
        }

        public RectangleF LayoutExtBounds(RectangleF bounds, List<ExtendedPlug> ins, List<ExtendedPlug> outs)
        {
            GH_SwitcherComponent gH_SwitcherComponent = (GH_SwitcherComponent)base.Owner;
            foreach (IGH_Param componentInput in gH_SwitcherComponent.StaticData.GetComponentInputs())
            {
                RectangleF bounds2 = componentInput.Attributes.Bounds;
                if (bounds2.X < bounds.X)
                {
                    float num = bounds.X - bounds2.X;
                    bounds.X = bounds2.X;
                    bounds.Width += num;
                }
                if (bounds2.X + bounds2.Width > bounds.X + bounds.Width)
                {
                    float num2 = bounds2.X + bounds2.Width - (bounds.X + bounds.Width);
                    bounds.Width += num2;
                }
            }
            foreach (IGH_Param componentOutput in gH_SwitcherComponent.StaticData.GetComponentOutputs())
            {
                RectangleF bounds3 = componentOutput.Attributes.Bounds;
                if (bounds3.X < bounds.X)
                {
                    float num3 = bounds.X - bounds3.X;
                    bounds.X = bounds3.X;
                    bounds.Width += num3;
                }
                if (bounds3.X + bounds3.Width > bounds.X + bounds.Width)
                {
                    float num4 = bounds3.X + bounds3.Width - (bounds.X + bounds.Width);
                    bounds.Width += num4;
                }
            }
            bounds.Inflate(2f, 2f);
            return bounds;
        }

        public void LayoutMenuInputs(RectangleF componentBox)
        {
            GH_SwitcherComponent obj = (GH_SwitcherComponent)base.Owner;
            float num = 0f;
            int num2 = 0;
            foreach (IGH_Param componentInput in obj.StaticData.GetComponentInputs())
            {
                int val = 20 * ((List<IGH_StateTag>)componentInput.StateTags).Count;
                num2 = Math.Max(num2, val);
                num = Math.Max(num, GH_FontServer.StringWidth(componentInput.NickName, StandardFont.font()));
            }
            num = Math.Max(num + 6f, 12f);
            num += (float)num2;
            float num3 = this.Bounds.Height;
            for (int i = 0; i < composedCollection.Menus.Count; i++)
            {
                float num4 = -1f;
                float num5 = 0f;
                bool expanded = composedCollection.Menus[i].Expanded;
                if (expanded)
                {
                    num4 = num3 + composedCollection.Menus[i].Height;
                    num5 = Math.Max(composedCollection.Menus[i].Inputs.Count, composedCollection.Menus[i].Outputs.Count) * 20;
                }
                else
                {
                    num4 = num3 + 5f;
                    num5 = 0f;
                }
                List<ExtendedPlug> inputs = composedCollection.Menus[i].Inputs;
                int count = inputs.Count;
                if (count != 0)
                {
                    float num6 = num5 / (float)count;
                    for (int j = 0; j < count; j++)
                    {
                        IGH_Param parameter = inputs[j].Parameter;
                        if (parameter.Attributes == null)
                        {
                            parameter.Attributes = (new GH_LinkedParamAttributes(parameter, this));
                        }
                        float num7 = componentBox.X - num;
                        float num8 = num4 + componentBox.Y + (float)j * num6;
                        float width = num - 3f;
                        float height = num6;
                        PointF pivot = new PointF(num7 + 0.5f * num, num8 + 0.5f * num6);
                        parameter.Attributes.Pivot = (pivot);
                        RectangleF rectangleF = new RectangleF(num7, num8, width, height);
                        parameter.Attributes.Bounds = ((RectangleF)GH_Convert.ToRectangle(rectangleF));
                    }
                    for (int k = 0; k < count; k++)
                    {
                        IGH_Param parameter2 = inputs[k].Parameter;
                        GH_LinkedParamAttributes val2 = (GH_LinkedParamAttributes)parameter2.Attributes;
                        FieldInfo field = typeof(GH_LinkedParamAttributes).GetField("m_renderTags", BindingFlags.Instance | BindingFlags.NonPublic);
                        GH_StateTagList val3 = parameter2.StateTags;
                        if (field != null)
                        {
                            if (((List<IGH_StateTag>)val3).Count == 0)
                            {
                                val3 = null;
                                field.SetValue(val2, val3);
                            }
                            if (val3 != null)
                            {
                                Rectangle rectangle = GH_Convert.ToRectangle(val2.Bounds);
                                rectangle.X += num2;
                                rectangle.Width -= num2;
                                val3.Layout(rectangle, 0);
                                rectangle = val3.BoundingBox;
                                if (!rectangle.IsEmpty)
                                {
                                    val2.Bounds = (RectangleF.Union(val2.Bounds, rectangle));
                                }
                                field.SetValue(val2, val3);
                            }
                        }
                        if (!expanded)
                        {
                            val2.Bounds = (new RectangleF(val2.Bounds.X, val2.Bounds.Y, 5f, val2.Bounds.Height));
                        }
                    }
                }
                num3 += composedCollection.Menus[i].TotalHeight;
            }
        }

        public void LayoutMenuOutputs(RectangleF componentBox)
        {
            GH_SwitcherComponent obj = (GH_SwitcherComponent)base.Owner;
            float num = 0f;
            int num2 = 0;
            foreach (IGH_Param componentOutput in obj.StaticData.GetComponentOutputs())
            {
                int val = 20 * ((List<IGH_StateTag>)componentOutput.StateTags).Count;
                num2 = Math.Max(num2, val);
                num = Math.Max(num, GH_FontServer.StringWidth(componentOutput.NickName, StandardFont.font()));
            }
            num = Math.Max(num + 6f, 12f);
            num += (float)num2;
            float num3 = this.Bounds.Height;
            for (int i = 0; i < composedCollection.Menus.Count; i++)
            {
                float num4 = -1f;
                float num5 = 0f;
                bool expanded = composedCollection.Menus[i].Expanded;
                if (expanded)
                {
                    num4 = num3 + composedCollection.Menus[i].Height;
                    num5 = Math.Max(composedCollection.Menus[i].Inputs.Count, composedCollection.Menus[i].Outputs.Count) * 20;
                }
                else
                {
                    num4 = num3 + 5f;
                    num5 = 0f;
                }
                List<ExtendedPlug> outputs = composedCollection.Menus[i].Outputs;
                int count = outputs.Count;
                if (count != 0)
                {
                    float num6 = num5 / (float)count;
                    for (int j = 0; j < count; j++)
                    {
                        IGH_Param parameter = outputs[j].Parameter;
                        if (parameter.Attributes == null)
                        {
                            parameter.Attributes = (new GH_LinkedParamAttributes(parameter, this));
                        }
                        float num7 = componentBox.Right + 3f;
                        float num8 = num4 + componentBox.Y + (float)j * num6;
                        float width = num;
                        float height = num6;
                        PointF pivot = new PointF(num7 + 0.5f * num, num8 + 0.5f * num6);
                        parameter.Attributes.Pivot = (pivot);
                        RectangleF rectangleF = new RectangleF(num7, num8, width, height);
                        parameter.Attributes.Bounds = ((RectangleF)GH_Convert.ToRectangle(rectangleF));
                    }
                    for (int k = 0; k < count; k++)
                    {
                        IGH_Param parameter2 = outputs[k].Parameter;
                        GH_LinkedParamAttributes val2 = (GH_LinkedParamAttributes)parameter2.Attributes;
                        FieldInfo field = typeof(GH_LinkedParamAttributes).GetField("m_renderTags", BindingFlags.Instance | BindingFlags.NonPublic);
                        GH_StateTagList val3 = parameter2.StateTags;
                        if (field != null)
                        {
                            if (((List<IGH_StateTag>)val3).Count == 0)
                            {
                                val3 = null;
                                field.SetValue(val2, val3);
                            }
                            if (val3 != null)
                            {
                                Rectangle rectangle = GH_Convert.ToRectangle(val2.Bounds);
                                rectangle.Width -= num2;
                                val3.Layout(rectangle, GH_StateTagLayoutDirection.Right);
                                rectangle = val3.BoundingBox;
                                if (!rectangle.IsEmpty)
                                {
                                    val2.Bounds = (RectangleF.Union(val2.Bounds, rectangle));
                                }
                                field.SetValue(val2, val3);
                            }
                        }
                        if (!expanded)
                        {
                            val2.Bounds = (new RectangleF(val2.Bounds.X + val2.Bounds.Width - 5f, val2.Bounds.Y, 5f, val2.Bounds.Height));
                        }
                    }
                }
                num3 += composedCollection.Menus[i].TotalHeight;
            }
        }

        protected void FixLayout(List<ExtendedPlug> outs)
        {
            GH_SwitcherComponent gH_SwitcherComponent = (GH_SwitcherComponent)base.Owner;
            float width = this.Bounds.Width;
            if (_minWidth > width)
            {
                this.Bounds = (new RectangleF(this.Bounds.X, this.Bounds.Y, _minWidth, this.Bounds.Height));
            }
            float num = this.Bounds.Width - width;
            foreach (IGH_Param componentOutput in gH_SwitcherComponent.StaticData.GetComponentOutputs())
            {
                PointF pivot = componentOutput.Attributes.Pivot;
                RectangleF bounds = componentOutput.Attributes.Bounds;
                componentOutput.Attributes.Pivot = (new PointF(pivot.X + num, pivot.Y));
                componentOutput.Attributes.Bounds = (new RectangleF(bounds.Location.X + num, bounds.Location.Y, bounds.Width, bounds.Height));
            }
            foreach (IGH_Param componentInput in gH_SwitcherComponent.StaticData.GetComponentInputs())
            {
                PointF pivot2 = componentInput.Attributes.Pivot;
                RectangleF bounds2 = componentInput.Attributes.Bounds;
                componentInput.Attributes.Pivot = (new PointF(pivot2.X + num, pivot2.Y));
                componentInput.Attributes.Bounds = (new RectangleF(bounds2.Location.X + num, bounds2.Location.Y, bounds2.Width, bounds2.Height));
            }
        }

        private void LayoutMenuCollection()
        {
            GH_Palette impliedPalette = GH_CapsuleRenderEngine.GetImpliedPalette(base.Owner);
            GH_PaletteStyle impliedStyle = GH_CapsuleRenderEngine.GetImpliedStyle(impliedPalette, this.Selected, base.Owner.Locked, base.Owner.Hidden);
            composedCollection.Style = impliedStyle;
            composedCollection.Palette = impliedPalette;
            composedCollection.Layout();
        }

        protected void LayoutMenu()
        {
            offset = (int)this.Bounds.Height;
            composedCollection.Pivot = new PointF(this.Bounds.X, (int)this.Bounds.Y + offset);
            composedCollection.Width = this.Bounds.Width;
            LayoutMenuCollection();
            this.Bounds = (new RectangleF(this.Bounds.X, this.Bounds.Y, this.Bounds.Width, this.Bounds.Height + composedCollection.Height));
        }

        //protected unsafe override void Render(GH_Canvas iCanvas, Graphics graph, GH_CanvasChannel iChannel)
        //{
        //    //if ((int)iChannel == 0)
        //    //{
        //    //    iCanvas.remove_CanvasPostPaintWidgets(new CanvasPostPaintWidgetsEventHandler((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
        //    //    iCanvas.add_CanvasPostPaintWidgets(new CanvasPostPaintWidgetsEventHandler((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
        //    //}
        //    if ((int)iChannel != 10)
        //    {
        //        if ((int)iChannel == 20)
        //        {
        //            RenderComponentCapsule2(iCanvas, graph);
        //            composedCollection.Render(new WidgetRenderArgs(iCanvas, WidgetChannel.Object));
        //        }
        //    }
        //    else
        //    {
        //        foreach (IGH_Param item in base.Owner.Params.Input)
        //        {
        //            item.Attributes.RenderToCanvas(iCanvas,GH_CanvasChannel.Wires);
        //        }
        //    }
        //}

        protected override void Render(GH_Canvas iCanvas, Graphics graph, GH_CanvasChannel iChannel)
        {
            if ((int)iChannel == 0)
            {
                iCanvas.CanvasPostPaintWidgets += (new CanvasPostPaintWidgetsEventHandler(RenderPostWidgets));
            }
                if ((int)iChannel != 10)
            {
                if ((int)iChannel == 20)
                {
                    RenderComponentCapsule2(iCanvas, graph);
                    composedCollection.Render(new WidgetRenderArgs(iCanvas, WidgetChannel.Object));
                }
            }
            else
            {
                foreach (IGH_Param item in base.Owner.Params.Input)
                {
                    item.Attributes.RenderToCanvas(iCanvas, GH_CanvasChannel.Wires);
                }
            }
        }

        private void RenderPostWidgets(GH_Canvas sender)
        {
            composedCollection.Render(new WidgetRenderArgs(sender, WidgetChannel.Overlay));
        }

        protected void RenderComponentCapsule2(GH_Canvas canvas, Graphics graphics)
        {
            RenderComponentCapsule2(canvas, graphics, drawComponentBaseBox: true, drawComponentNameBox: true, drawJaggedEdges: true, drawParameterGrips: true, drawParameterNames: true, drawZuiElements: true);
        }

        protected void RenderComponentCapsule2(GH_Canvas canvas, Graphics graphics, bool drawComponentBaseBox, bool drawComponentNameBox, bool drawJaggedEdges, bool drawParameterGrips, bool drawParameterNames, bool drawZuiElements)
        {
            GH_SwitcherComponent gH_SwitcherComponent = (GH_SwitcherComponent)base.Owner;
            RectangleF bounds = this.Bounds;
            this.Bounds = (bounds);
            if (!canvas.Viewport.IsVisible(ref bounds, 10f))
            {
                return;
            }
            GH_Palette val = GH_CapsuleRenderEngine.GetImpliedPalette(base.Owner);
            if ((int)val == 0 && !base.Owner.IsPreviewCapable)
            {
                val = GH_Palette.Hidden;
            }
            GH_Capsule val2 = GH_Capsule.CreateCapsule(this.Bounds, val);
            bool flag = base.Owner.Params.Input
                .Count == 0;
            bool flag2 = base.Owner.Params.Output
                .Count == 0;
            val2.SetJaggedEdges(flag, flag2);
            GH_PaletteStyle impliedStyle = GH_CapsuleRenderEngine.GetImpliedStyle(val, this.Selected, base.Owner.Locked, base.Owner.Hidden);
            if (drawParameterGrips)
            {
                foreach (IGH_Param staticInput in gH_SwitcherComponent.StaticData.StaticInputs)
                {
                    val2.AddInputGrip(staticInput.Attributes.InputGrip.Y);
                }
                foreach (IGH_Param dynamicInput in gH_SwitcherComponent.StaticData.DynamicInputs)
                {
                    val2.AddInputGrip(dynamicInput.Attributes.InputGrip.Y);
                }
                foreach (IGH_Param staticOutput in gH_SwitcherComponent.StaticData.StaticOutputs)
                {
                    val2.AddOutputGrip(staticOutput.Attributes.OutputGrip.Y);
                }
                foreach (IGH_Param dynamicOutput in gH_SwitcherComponent.StaticData.DynamicOutputs)
                {
                    val2.AddOutputGrip(dynamicOutput.Attributes.OutputGrip.Y);
                }
            }
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            canvas.SetSmartTextRenderingHint();
            if (GH_Attributes<IGH_Component>.IsIconMode(base.Owner.IconDisplayMode))
            {
                if (drawComponentBaseBox)
                {
                    if (base.Owner.Message != null)
                    {
                        val2.RenderEngine.RenderMessage(graphics, base.Owner.Message, impliedStyle);
                    }
                    val2.Render(graphics, impliedStyle);
                }
                if (drawComponentNameBox && base.Owner.Icon_24x24 != null)
                {
                    if (base.Owner.Locked)
                    {
                        val2.RenderEngine.RenderIcon(graphics, (Image)base.Owner.Icon_24x24_Locked, base.m_innerBounds, 0, 0);
                    }
                    else
                    {
                        val2.RenderEngine.RenderIcon(graphics, (Image)base.Owner.Icon_24x24, base.m_innerBounds, 0, 0);
                    }
                }
            }
            else
            {
                if (drawComponentBaseBox)
                {
                    if (base.Owner.Message != null)
                    {
                        val2.RenderEngine.RenderMessage(graphics, base.Owner.Message, impliedStyle);
                    }
                    val2.Render(graphics, impliedStyle);
                }
                if (drawComponentNameBox)
                {
                    GH_Capsule obj = GH_Capsule.CreateTextCapsule(base.m_innerBounds, base.m_innerBounds, GH_Palette.Black, base.Owner.NickName, StandardFont.largeFont(), GH_Orientation.vertical_center, 3, 6);
                    obj.Render(graphics, this.Selected, base.Owner.Locked, false);
                    obj.Dispose();
                }
            }
            if (drawComponentNameBox && base.Owner.Obsolete && Grasshopper.CentralSettings.CanvasObsoleteTags && (int)canvas.DrawingMode == 0)
            {
                GH_GraphicsUtil.RenderObjectOverlay(graphics, Owner, base.m_innerBounds);
            }
            if (drawParameterNames)
            {
                RenderComponentParameters2(canvas, graphics, base.Owner, impliedStyle);
            }
            if (drawZuiElements)
            {
                this.RenderVariableParameterUI(canvas, graphics);
            }
            val2.Dispose();
        }

        public static void RenderComponentParameters2(GH_Canvas canvas, Graphics graphics, IGH_Component owner, GH_PaletteStyle style)
        {
            GH_SwitcherComponent gH_SwitcherComponent = (GH_SwitcherComponent)owner;
            int zoomFadeLow = GH_Canvas.ZoomFadeLow;
            if (zoomFadeLow >= 5)
            {
                StringFormat farCenter = GH_TextRenderingConstants.FarCenter;
                canvas.SetSmartTextRenderingHint();
                SolidBrush solidBrush = new SolidBrush(Color.FromArgb(zoomFadeLow, style.Text));
                foreach (IGH_Param staticInput in gH_SwitcherComponent.StaticData.StaticInputs)
                {
                    RectangleF bounds = staticInput.Attributes.Bounds;
                    if (bounds.Width >= 1f)
                    {
                        graphics.DrawString(staticInput.NickName, StandardFont.font(), solidBrush, bounds, farCenter);
                        GH_LinkedParamAttributes obj = (GH_LinkedParamAttributes)staticInput.Attributes;
                        GH_StateTagList val = (GH_StateTagList)typeof(GH_LinkedParamAttributes).GetField("m_renderTags", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(obj);
                        if (val != null)
                        {
                            val.RenderStateTags(graphics);
                        }
                    }
                }
                farCenter = GH_TextRenderingConstants.NearCenter;
                foreach (IGH_Param staticOutput in gH_SwitcherComponent.StaticData.StaticOutputs)
                {
                    RectangleF bounds2 = staticOutput.Attributes.Bounds;
                    if (bounds2.Width >= 1f)
                    {
                        graphics.DrawString(staticOutput.NickName, StandardFont.font(), solidBrush, bounds2, farCenter);
                        GH_LinkedParamAttributes obj2 = (GH_LinkedParamAttributes)staticOutput.Attributes;
                        GH_StateTagList val2 = (GH_StateTagList)typeof(GH_LinkedParamAttributes).GetField("m_renderTags", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(obj2);
                        if (val2 != null)
                        {
                            val2.RenderStateTags(graphics);
                        }
                    }
                }
                solidBrush.Dispose();
            }
        }

        public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            GH_ObjectResponse val = composedCollection.RespondToMouseUp(sender, e);
            if ((int)val == 1)
            {
                this.ExpireLayout();
                ((Control)sender).Invalidate();
                return val;
            }
            if ((int)val != 0)
            {
                this.ExpireLayout();
                ((Control)sender).Invalidate();
                return GH_ObjectResponse.Release;
            }
            return base.RespondToMouseUp(sender, e);
        }

        public override GH_ObjectResponse RespondToMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            GH_ObjectResponse val = composedCollection.RespondToMouseDoubleClick(sender, e);
            if ((int)val != 0)
            {
                this.ExpireLayout();
                ((Control)sender).Refresh();
                return val;
            }
            return base.RespondToMouseDoubleClick(sender, e);
        }

        public override GH_ObjectResponse RespondToKeyDown(GH_Canvas sender, KeyEventArgs e)
        {
            GH_ObjectResponse val = composedCollection.RespondToKeyDown(sender, e);
            if ((int)val != 0)
            {
                this.ExpireLayout();
                ((Control)sender).Refresh();
                return val;
            }
            return base.RespondToKeyDown(sender, e);
        }

        public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            GH_ObjectResponse val = composedCollection.RespondToMouseMove(sender, e);
            if ((int)val != 0)
            {
                this.ExpireLayout();
                ((Control)sender).Refresh();
                return val;
            }
            return base.RespondToMouseMove(sender, e);
        }

        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            GH_ObjectResponse val = composedCollection.RespondToMouseDown(sender, e);
            if ((int)val != 0)
            {
                this.ExpireLayout();
                ((Control)sender).Refresh();
                return val;
            }
            return base.RespondToMouseDown(sender, e);
        }

        public override bool IsTooltipRegion(PointF pt)
        {
            _activeToolTip = null;
            bool flag = base.IsTooltipRegion(pt);
            if (flag)
            {
                return flag;
            }
            if (base.m_innerBounds.Contains(pt))
            {
                GH_Attr_Widget gH_Attr_Widget = collection.IsTtipPoint(pt);
                if (gH_Attr_Widget != null)
                {
                    _activeToolTip = gH_Attr_Widget;
                    return true;
                }
            }
            return false;
        }

        public bool GetActiveTooltip(PointF pt)
        {
            GH_Attr_Widget gH_Attr_Widget = composedCollection.IsTtipPoint(pt);
            if (gH_Attr_Widget != null)
            {
                _activeToolTip = gH_Attr_Widget;
                return true;
            }
            return false;
        }

        public override void SetupTooltip(PointF canvasPoint, GH_TooltipDisplayEventArgs e)
        {
            
            GetActiveTooltip(canvasPoint);
            if (_activeToolTip != null)
            {
                _activeToolTip.TooltipSetup(canvasPoint, e);
                return;
            }
            e.Title = (this.PathName);
            e.Text = (base.Owner.Description);
            e.Description = (base.Owner.InstanceDescription);
            e.Icon = (base.Owner.Icon_24x24);
            if (base.Owner is IGH_Param)
            {
					//? val = base.Owner;
                //string text = val.TypeName;
                string text = base.Owner.GetType().ToString();
                //if ((int)val.get_Access() == 1)
                //{
                //    text += "[…]";
                //}
                //if ((int)val.get_Access() == 2)
                //{
                //    text += "{…;…;…}";
                //}
                e.Title = ($"{this.PathName} ({text})");
            }
        }

    }
}
