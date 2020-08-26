using GH_IO.Serialization;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RFEM_daq.UIWidgets
{
    public class GH_ExtendableComponentAttributes : GH_ComponentAttributes
    {
        private float _minWidth;

        private GH_Attr_Widget _activeToolTip;

        private GH_MenuCollection _collection;

        private const bool RENDER_OVERLAY_OVERWIDGETS = true;

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

        public GH_ExtendableComponentAttributes(IGH_Component nComponent)
            : base(nComponent)
        {
            _collection = new GH_MenuCollection();
        }

        public void AddMenu(GH_ExtendableMenu _menu)
        {
            _collection.AddMenu(_menu);
        }

        public override bool Write(GH_IWriter writer)
        {
            try
            {
                _collection.Write(writer);
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
                _collection.Read(reader);
            }
            catch (Exception)
            {
            }
            return base.Read(reader);
        }

        protected override void PrepareForRender(GH_Canvas canvas)
        {
            base.PrepareForRender(canvas);
            LayoutStyle();
        }

        protected override void Layout()
        {
            this.Pivot = ((PointF)GH_Convert.ToPoint(this.Pivot));
            //this.Layout();
            base.Layout();
            FixLayout();
            LayoutMenu();
        }

        protected void FixLayout()
        {
            float width = this.Bounds.Width;
            float num = Math.Max(_collection.GetMinLayoutSize().Width, _minWidth);
            float num2 = 0f;
            if (num > this.Bounds.Width)
            {
                num2 = num - this.Bounds.Width;
                this.Bounds = (new RectangleF(this.Bounds.X - num2 / 2f, this.Bounds.Y, num, this.Bounds.Height));
            }
            foreach (IGH_Param item in base.Owner.Params.Output)
            {
                PointF pivot = item.Attributes.Pivot;
                RectangleF bounds = item.Attributes.Bounds;
                item.Attributes.Pivot = (new PointF(pivot.X, pivot.Y));
                item.Attributes.Bounds = (new RectangleF(bounds.Location.X, bounds.Location.Y, bounds.Width + num2 / 2f, bounds.Height));
            }
            foreach (IGH_Param item2 in base.Owner.Params.Input)
            {
                PointF pivot2 = item2.Attributes.Pivot;
                RectangleF bounds2 = item2.Attributes.Bounds;
                item2.Attributes.Pivot = (new PointF(pivot2.X - num2 / 2f, pivot2.Y));
                item2.Attributes.Bounds = (new RectangleF(bounds2.Location.X - num2 / 2f, bounds2.Location.Y, bounds2.Width + num2 / 2f, bounds2.Height));
            }
        }

        private void LayoutStyle()
        {
            _collection.Palette = GH_CapsuleRenderEngine.GetImpliedPalette(base.Owner);
            _collection.Style = GH_CapsuleRenderEngine.GetImpliedStyle(_collection.Palette, this.Selected, base.Owner.Locked, base.Owner.Hidden);
            _collection.Layout();
        }

        protected void LayoutMenu()
        {
            _collection.Width = this.Bounds.Width;
            _collection.Pivot = new PointF(this.Bounds.Left, this.Bounds.Bottom);
            LayoutStyle();
            this.Bounds = (new RectangleF(this.Bounds.X, this.Bounds.Y, this.Bounds.Width, this.Bounds.Height + _collection.Height));
        }

        protected override void Render(GH_Canvas iCanvas, Graphics graph, GH_CanvasChannel iChannel)
        {
            if ((int)iChannel == 0)
            {
                iCanvas.CanvasPostPaintWidgets += (new GH_Canvas.CanvasPostPaintWidgetsEventHandler(RenderPostWidgets));
            }
            this.Render(iCanvas, graph, iChannel);
            if ((int)iChannel == 20)
            {
                _collection.Render(new WidgetRenderArgs(iCanvas, WidgetChannel.Object));
            }
        }

        private void RenderPostWidgets(GH_Canvas sender)
        {
            _collection.Render(new WidgetRenderArgs(sender, WidgetChannel.Overlay));
        }

        public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            GH_ObjectResponse val = _collection.RespondToMouseUp(sender, e);
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
            GH_ObjectResponse val = _collection.RespondToMouseDoubleClick(sender, e);
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
            GH_ObjectResponse val = _collection.RespondToKeyDown(sender, e);
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
            GH_ObjectResponse val = _collection.RespondToMouseMove(sender, e);
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
            try
            {
                GH_ObjectResponse val = _collection.RespondToMouseDown(sender, e);
                if ((int)val != 0)
                {
                    this.ExpireLayout();
                    ((Control)sender).Refresh();
                    return val;
                }
                return this.RespondToMouseDown(sender, e);
            }
            catch (Exception)
            {
                return this.RespondToMouseDown(sender, e);
            }
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
                GH_Attr_Widget gH_Attr_Widget = _collection.IsTtipPoint(pt);
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
            GH_Attr_Widget gH_Attr_Widget = _collection.IsTtipPoint(pt);
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
                //? val = base.get_Owner();
                //             string text = val.get_TypeName();
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

        public string GetMenuDescription()
        {
            return _collection.GetMenuDescription();
        }
    }
}
