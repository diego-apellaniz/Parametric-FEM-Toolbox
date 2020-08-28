using GH_IO.Serialization;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace Parametric_FEM_Toolbox.UIWidgets
{
    public class GH_ExtendableMenu : GH_Attr_Widget
    {
        private List<ExtendedPlug> inputs;
        private List<ExtendedPlug> outputs;
        private string name;
        private GH_MenuCollection collection;
        private GH_Capsule _menu;
        private RectangleF _headBounds;
        private RectangleF _contentBounds;
        private List<GH_Attr_Widget> _controls;
        private GH_Attr_Widget _activeControl;
        private bool _expanded;
        public List<ExtendedPlug> Inputs => inputs;
        public List<ExtendedPlug> Outputs => outputs;
        public bool Expanded => _expanded;
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
        public List<GH_Attr_Widget> Controlls => _controls;
        public override string Name
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
        public float TotalHeight
        {
            get
            {
                if (_expanded)
                {
                    int num = Math.Max(inputs.Count, outputs.Count) * 20;
                    if (num > 0)
                    {
                        num += 5;
                    }
                    return base.Height + (float)num;
                }
                return base.Height;
            }
        }
        public GH_ExtendableMenu(int index, string id)
        : base(index, id)
        {
            inputs = new List<ExtendedPlug>();
            outputs = new List<ExtendedPlug>();
            _controls = new List<GH_Attr_Widget>();
            _headBounds = default(RectangleF);
            _contentBounds = default(RectangleF);
        }
        public void RegisterInputPlug(ExtendedPlug plug)
        {
            plug.IsMenu = true;
            inputs.Add(plug);
        }
        public void RegisterOutputPlug(ExtendedPlug plug)
        {
            plug.IsMenu = true;
            outputs.Add(plug);
        }
        public void Expand()
        {
            if (!_expanded)
            {
                _expanded = true;
            }
        }
        public void Collapse()
        {
            if (_expanded)
            {
                _expanded = false;
            }
        }
        public void AddControl(GH_Attr_Widget control)
        {
            control.Parent = this;
            _controls.Add(control);
        }
        public void MakeAllInActive()
        {
            int count = _controls.Count;
            for (int i = 0; i < count; i++)
            {
                if (_controls[i] is MenuPanel)
                {
                    ((MenuPanel)_controls[i])._activeControl = null;
                }
            }
            _activeControl = null;
        }
        public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (_activeControl != null)
            {
                GH_ObjectResponse val = _activeControl.RespondToMouseUp(sender, e);
                if ((int)val == 2)
                {
                    _activeControl = null;
                    return val;
                }
                if ((int)val != 0)
                {
                    return val;
                }
                _activeControl = null;
            }
            return 0;
        }
        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (_headBounds.Contains(e.CanvasLocation))
            {
                if (Expanded)
                {
                    _activeControl = null;
                }
                _expanded = !_expanded;
                return GH_ObjectResponse.Handled;
            }
            if (_expanded)
            {
                if (_contentBounds.Contains(e.CanvasLocation))
                {
                    for (int i = 0; i < inputs.Count; i++)
                    {
                        if (inputs[i].Parameter.Attributes.Bounds.Contains(e.CanvasLocation))
                        {
                            return inputs[i].Parameter.Attributes.RespondToMouseDown(sender, e);
                        }
                    }
                    for (int j = 0; j < _controls.Count; j++)
                    {
                        if (_controls[j].Contains(e.CanvasLocation))
                        {
                            _activeControl = _controls[j];
                            return _controls[j].RespondToMouseDown(sender, e);
                        }
                    }
                }
                else if (_activeControl != null)
                {
                    _activeControl.RespondToMouseDown(sender, e);
                    _activeControl = null;
                    return GH_ObjectResponse.Handled;
                }
            }
            return 0;
        }
        public override GH_Attr_Widget IsTtipPoint(PointF pt)
        {
            if (_headBounds.Contains(pt))
            {
                return this;
            }
            if (_expanded && _contentBounds.Contains(pt))
            {
                int count = _controls.Count;
                for (int i = 0; i < count; i++)
                {
                    if (_controls[i].Contains(pt))
                    {
                        GH_Attr_Widget gH_Attr_Widget = _controls[i].IsTtipPoint(pt);
                        if (gH_Attr_Widget != null)
                        {
                            return gH_Attr_Widget;
                        }
                    }
                }
            }
            return null;
        }
        public override void TooltipSetup(PointF canvasPoint, GH_TooltipDisplayEventArgs e)
        {
            e.Icon=((Bitmap)null);
            e.Title=("Menu (" + name + ")");
            e.Text=(_header);
            if (_header != null)
            {
                e.Text=(e.Text + "\n");
            }
            if (_expanded)
            {
                e.Text=(e.Text + "Click to close menu");
            }
            else
            {
                e.Text = (e.Text + "Click to open menu");
            }
            e.Description=(_description);
        }
        public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (_activeControl != null)
            {
                return _activeControl.RespondToMouseMove(sender, e);
            }
            return 0;
        }
        public override GH_ObjectResponse RespondToMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            PointF canvasPivot = base.CanvasPivot;
            if (_headBounds.Contains(e.CanvasLocation))
            {
                return GH_ObjectResponse.Handled;
            }
            if (_expanded && _contentBounds.Contains(e.CanvasLocation))
            {
                int count = _controls.Count;
                for (int i = 0; i < count; i++)
                {
                    if (_controls[i].Contains(e.CanvasLocation))
                    {
                        return _controls[i].RespondToMouseDoubleClick(sender, e);
                    }
                }
            }
            return 0;
        }
        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("Expanded", _expanded);
            for (int i = 0; i < _controls.Count; i++)
            {
                _controls[i].Write(writer);
            }
            return base.Write(writer);
        }
        public override bool Read(GH_IReader reader)
        {
            _expanded = reader.GetBoolean("Expanded");
            for (int i = 0; i < _controls.Count; i++)
            {
                _controls[i].Read(reader);
            }
            return base.Read(reader);
        }
        public override SizeF ComputeMinSize()
        {
            SizeF menuHeadTextSize = GetMenuHeadTextSize();
            float num = menuHeadTextSize.Width;
            float num2 = menuHeadTextSize.Height;
            foreach (GH_Attr_Widget control in _controls)
            {
                SizeF sizeF = control.ComputeMinSize();
                num = Math.Max(sizeF.Width, num);
                if (_expanded)
                {
                    num2 += sizeF.Height;
                }
            }
            return new SizeF(num, num2);
        }
        private SizeF GetMenuHeadTextSize()
        {
            Size size = TextRenderer.MeasureText(name, WidgetServer.Instance.MenuHeaderFont);
            return new SizeF((float)(size.Width + 8), (float)(size.Height + 4));
        }
        public override void Layout()
        {
            SizeF menuHeadTextSize = GetMenuHeadTextSize();
            PointF canvasPivot = base.CanvasPivot;
            float x = canvasPivot.X;
            canvasPivot = base.CanvasPivot;
            _headBounds = new RectangleF(x, canvasPivot.Y, base.Width, menuHeadTextSize.Height);
            canvasPivot = base.CanvasPivot;
            float x2 = canvasPivot.X;
            canvasPivot = base.CanvasPivot;
            _contentBounds = new RectangleF(x2, canvasPivot.Y + menuHeadTextSize.Height, base.Width, base.Height - menuHeadTextSize.Height);
            Rectangle rectangle = new Rectangle((int)_headBounds.X + 1, (int)_headBounds.Y + 1, (int)_headBounds.Width - 2, (int)_headBounds.Height - 2);
            _menu = GH_Capsule.CreateTextCapsule(rectangle, rectangle, GH_Palette.Black, name, WidgetServer.Instance.MenuHeaderFont, 0, 2, 5);
            float num = menuHeadTextSize.Height;
            if (_expanded)
            {
                canvasPivot = base.CanvasPivot;
                float x3 = canvasPivot.X;
                canvasPivot = base.CanvasPivot;
                PointF transform = new PointF(x3, canvasPivot.Y + menuHeadTextSize.Height);
                foreach (GH_Attr_Widget control in _controls)
                {
                    control.UpdateBounds(transform, base.Width);
                    control.Transform = transform;
                    control.Style = style;
                    control.Palette = palette;
                    control.Layout();
                    num += control.Height;
                }
            }
            base.Height = num;
        }
        public override void Render(WidgetRenderArgs args)
        {
            GH_Canvas canvas = args.Canvas;
            WidgetChannel channel = args.Channel;
            float zoom = canvas.Viewport.Zoom;
            int num = 255;
            if (zoom < 1f)
            {
                float num2 = (zoom - 0.5f) * 2f;
                num = (int)((float)num * num2);
            }
            _menu.Render(canvas.Graphics, false, false, false);
            if (_expanded && num > 0)
            {
                RenderMenuParameters(canvas, canvas.Graphics);
                foreach (GH_Attr_Widget control in _controls)
                {
                    control.OnRender(args);
                }
            }
        }
        public void RenderMenuParameters(GH_Canvas canvas, Graphics graphics)
        {
            if (Math.Max(inputs.Count, outputs.Count) != 0)
            {
                int zoomFadeLow = GH_Canvas.ZoomFadeLow;
                if (zoomFadeLow >= 5)
                {
                    StringFormat farCenter = GH_TextRenderingConstants.FarCenter;
                    canvas.SetSmartTextRenderingHint();
                    SolidBrush solidBrush = new SolidBrush(Color.FromArgb(zoomFadeLow, style.Text));
                    List<ExtendedPlug>.Enumerator enumerator = inputs.GetEnumerator();
                    try
                    {
                        while (enumerator.MoveNext())
                        {
                            IGH_Param parameter = enumerator.Current.Parameter;
                            RectangleF bounds = parameter.Attributes.Bounds;
                            if (bounds.Width >= 1f)
                            {
                                graphics.DrawString(parameter.NickName, StandardFont.font(), solidBrush, bounds, farCenter);
                                GH_LinkedParamAttributes obj = (GH_LinkedParamAttributes)parameter.Attributes;
                                FieldInfo field = typeof(GH_LinkedParamAttributes).GetField("m_renderTags", BindingFlags.Instance | BindingFlags.NonPublic);
                                if (field != (FieldInfo)null)
                                {
                                    GH_StateTagList value = (GH_StateTagList)field.GetValue(obj);
                                    if (value != null)
                                    {
                                        value.RenderStateTags(graphics);
                                    }
                                }
                            }
                        }
                    }
                    finally
                    {
                        ((IDisposable)enumerator).Dispose();
                    }
                    farCenter = GH_TextRenderingConstants.NearCenter;
                    enumerator = outputs.GetEnumerator();
                    try
                    {
                        while (enumerator.MoveNext())
                        {
                            IGH_Param parameter2 = enumerator.Current.Parameter;
                            RectangleF bounds2 = parameter2.Attributes.Bounds;
                            if (bounds2.Width >= 1f)
                            {
                                graphics.DrawString(parameter2.NickName, StandardFont.font(), solidBrush, bounds2, farCenter);
                                GH_LinkedParamAttributes obj2 = (GH_LinkedParamAttributes)parameter2.Attributes;
                                FieldInfo field2 = typeof(GH_LinkedParamAttributes).GetField("m_renderTags", BindingFlags.Instance | BindingFlags.NonPublic);
                                if (field2 != (FieldInfo)null)
                                {
                                    GH_StateTagList value2 = (GH_StateTagList)field2.GetValue(obj2);
                                    if (value2 != null)
                                    {
                                        value2.RenderStateTags(graphics);
                                    }
                                }
                            }
                        }
                    }
                    finally
                    {
                        ((IDisposable)enumerator).Dispose();
                    }
                    solidBrush.Dispose();
                }
            }
        }
        public override string GetWidgetDescription()
        {
            string str = base.GetWidgetDescription() + "{\n";
            foreach (GH_Attr_Widget control in _controls)
            {
                str = str + control.GetWidgetDescription() + "\n";
            }
            return str + "}";
        }
    }
}
