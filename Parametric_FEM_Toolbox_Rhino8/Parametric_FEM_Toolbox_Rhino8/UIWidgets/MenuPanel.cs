using GH_IO.Serialization;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Parametric_FEM_Toolbox.UIWidgets
{
    public class MenuPanel : GH_Attr_Widget
    {
        private List<GH_Attr_Widget> _controls;

        public GH_Capsule _menu;

        public GH_Attr_Widget _activeControl;

        public float LeftMargin { get; set; }
        public float RightMargin { get; set; }
        public float TopMargin { get; set; }
        public float BottomMargin { get; set; }
        public float LeftInnerMargin { get; set; }
        public float RightInnerMargin { get; set; }
        public float TopInnerMargin { get; set; }
        public float BottomInnerMargin { get; set; }
        public int PanelRadius { get; set; }
        public int Space { get; set; }
        public float EffectiveWidth => base.Width - ((float)(PanelRadius * 2) + LeftMargin + RightMargin + LeftInnerMargin + RightInnerMargin);
        public float EffectiveHeight => base.Height - ((float)(PanelRadius * 2) + TopMargin + BottomMargin + TopInnerMargin + BottomInnerMargin);
        public MenuPanel(int index, string id)
            : base(index, id)
        {
            _controls = new List<GH_Attr_Widget>();
            LeftMargin = 3f;
            RightMargin = 3f;
            TopMargin = 3f;
            BottomMargin = 3f;
            LeftInnerMargin = 1f;
            RightInnerMargin = 1f;
            TopInnerMargin = 1f;
            BottomInnerMargin = 1f;
            PanelRadius = 3;
            Space = 5;
        }
        public void AddControl(GH_Attr_Widget _control)
        {
            _controls.Add(_control);
            _control.Parent = this;
        }
        public override bool Write(GH_IWriter writer)
        {
            GH_IWriter writer2 = writer.CreateChunk("Panel", Index);
            for (int i = 0; i < _controls.Count; i++)
            {
                _controls[i].Write(writer2);
            }
            return base.Write(writer);
        }
        public override bool Read(GH_IReader reader)
        {
            GH_IReader reader2 = reader.FindChunk("Panel", Index);
            for (int i = 0; i < _controls.Count; i++)
            {
                _controls[i].Read(reader2);
            }
            return base.Read(reader);
        }
        public override SizeF ComputeMinSize()
        {
            float num = LeftMargin + RightMargin + (float)(PanelRadius * 2) + LeftInnerMargin + RightInnerMargin;
            float num2 = TopMargin + BottomMargin + (float)(PanelRadius * 2) + TopInnerMargin + BottomInnerMargin;
            float num3 = num;
            float num4 = num2;
            int num5 = 0;
            foreach (GH_Attr_Widget control in _controls)
            {
                SizeF sizeF = control.ComputeMinSize();
                num3 = Math.Max(sizeF.Width + num, num3);
                if (num5++ > 0)
                {
                    num4 += (float)Space;
                }
                num4 += sizeF.Height;
            }
            return new SizeF(num3, num4);
        }
        public override void Layout()
        {
            //IL_00a9: Unknown result type (might be due to invalid IL or missing references)
            //IL_012a: Unknown result type (might be due to invalid IL or missing references)
            float num = base.CanvasPivot.Y + TopMargin + TopInnerMargin + (float)PanelRadius;
            float y = base.CanvasPivot.Y;
            int num2 = 0;
            foreach (GH_Attr_Widget control in _controls)
            {
                if (num2++ > 0)
                {
                    num += (float)Space;
                }
                PointF transform = new PointF(base.CanvasPivot.X + LeftMargin + LeftInnerMargin + (float)PanelRadius, num);
                control.UpdateBounds(transform, EffectiveWidth);
                control.Style = base.Style;
                control.Palette = base.Palette;
                control.Layout();
                float height = control.Height;
                num += height;
            }
            num += (float)PanelRadius + BottomMargin + BottomInnerMargin;
            base.Height = num - y;
            RectangleF rectangleF = GH_Attr_Widget.Shrink(base.CanvasBounds, LeftMargin, RightMargin, TopMargin, BottomMargin);
            _menu = GH_Capsule.CreateTextCapsule(rectangleF, rectangleF, base.Palette, "", new Font(new FontFamily("Arial"), 8f, FontStyle.Bold), 0, PanelRadius, 8);
        }

        public override void Render(WidgetRenderArgs args)
        {
            //IL_00a7: Unknown result type (might be due to invalid IL or missing references)
            //IL_00ae: Expected O, but got Unknown
            GH_Canvas canvas = args.Canvas;
            float zoom = canvas.Viewport.Zoom;
            int num = 255;
            if (zoom < 1f)
            {
                float num2 = (zoom - 0.5f) * 2f;
                num = (int)((float)num * num2);
            }
            if (num < 0)
            {
                num = 0;
            }
            num = GH_Canvas.ZoomFadeLow;
            int r = base.Style.Fill.R;
            int g = base.Style.Fill.G;
            int b = base.Style.Fill.B;
            int red = 80;
            int green = 80;
            int blue = 80;
            GH_PaletteStyle val = new GH_PaletteStyle(Color.FromArgb(num, r, g, b), Color.FromArgb(num, red, green, blue));
            _menu.Render(canvas.Graphics, val);
            for (int i = 0; i < _controls.Count; i++)
            {
                _controls[i].OnRender(args);
            }
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
            if (base.CanvasBounds.Contains(e.CanvasLocation))
            {
                foreach (GH_Attr_Widget control in _controls)
                {
                    if (control.Contains(e.CanvasLocation) && control.Enabled)
                    {
                        GH_ObjectResponse val = control.RespondToMouseDown(sender, e);
                        if ((int)val != 0)
                        {
                            _activeControl = control;
                            return val;
                        }
                    }
                }
            }
            else if (_activeControl != null)
            {
                _activeControl.RespondToMouseDown(sender, e);
                _activeControl = null;
                return GH_ObjectResponse.Handled;
            }
            return 0;
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
            if (base.CanvasBounds.Contains(e.CanvasLocation))
            {
                int count = _controls.Count;
                for (int i = 0; i < count; i++)
                {
                    if (_controls[i].Contains(e.CanvasLocation) && _controls[i].Enabled)
                    {
                        return _controls[i].RespondToMouseDoubleClick(sender, e);
                    }
                }
            }
            return 0;
        }
        public override GH_Attr_Widget IsTtipPoint(PointF pt)
        {
            if (base.CanvasBounds.Contains(pt))
            {
                int count = _controls.Count;
                for (int i = 0; i < count; i++)
                {
                    GH_Attr_Widget gH_Attr_Widget = _controls[i].IsTtipPoint(pt);
                    if (gH_Attr_Widget != null)
                    {
                        return gH_Attr_Widget;
                    }
                }
                if (_showToolTip)
                {
                    return this;
                }
            }
            return null;
        }
        public override void TooltipSetup(PointF canvasPoint, GH_TooltipDisplayEventArgs e)
        {
            e.Icon=((Bitmap)null);
            e.Title=(_name + " (Group)");
            e.Text=(_header);
            e.Description=(_description);
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
