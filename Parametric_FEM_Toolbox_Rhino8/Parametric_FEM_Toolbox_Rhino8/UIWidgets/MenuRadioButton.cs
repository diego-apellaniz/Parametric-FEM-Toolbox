using System;
using System.Drawing;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;

namespace Parametric_FEM_Toolbox.UIWidgets
{
    class MenuRadioButton : GH_Attr_Widget
    {
		public enum Alignment
		{
			Vertical,
			Horizontal
		}

		private bool _active;

		private Size _radioSize;

		private Rectangle _radioBounds;

		private float _padding = 4f;

		public Alignment Align
		{
			get;
			set;
		}

		public string Tag
		{
			get;
			set;
		}

		public bool RenderTag
		{
			get;
			set;
		}

		public bool Active
		{
			get
			{
				return _active;
			}
			set
			{
				_active = value;
			}
		}

		public event ValueChangeEventHandler ValueChanged;

		public MenuRadioButton(int index, string id, string tag, Alignment align)
			: base(index, id)
		{
			_active = false;
			_padding = WidgetServer.Instance.RadioButtonPadding;
			_radioSize = WidgetServer.Instance.RadioButtonSize;
			Tag = tag;
			RenderTag = true;
			Align = align;
		}

		public override bool Write(GH_IWriter writer)
		{
			writer.CreateChunk("RadioButton", Index).SetBoolean("Active", _active);
			return true;
		}

		public override bool Read(GH_IReader reader)
		{
			GH_IReader val = reader.FindChunk("RadioButton", Index);
			_active = val.GetBoolean("Active");
			return true;
		}

		public override SizeF ComputeMinSize()
		{
			int num = _radioSize.Width;
			int num2 = _radioSize.Height;
			if (Align == Alignment.Horizontal)
			{
				if (RenderTag && Tag != null)
				{
					Size size = TextRenderer.MeasureText(Tag, WidgetServer.Instance.TextFont);
					num += size.Width + (int)_padding;
					num2 = Math.Max(size.Height, num2);
				}
			}
			else
			{
				if (Align != 0)
				{
					throw new ArgumentException("invalid alignment(" + Align.ToString() + ")");
				}
				if (RenderTag && Tag != null)
				{
					Size size2 = TextRenderer.MeasureText(Tag, WidgetServer.Instance.TextFont);
					num = Math.Max(size2.Width, num);
					num2 += size2.Height + (int)_padding;
				}
			}
			return new SizeF(num, num2);
		}

		public override void PostUpdateBounds(out float outHeight)
		{
			outHeight = ComputeMinSize().Height;
		}

		public override void Layout()
		{
			if (Align == Alignment.Horizontal)
			{
				float num = base.CanvasPivot.Y + (base.Height - (float)_radioSize.Height) / 2f;
				_radioBounds = new Rectangle((int)(base.CanvasPivot.X + base.CanvasBounds.Width - (float)_radioSize.Width), (int)num, _radioSize.Width, _radioSize.Height);
			}
			else
			{
				float num2 = base.CanvasPivot.X + (base.Width - (float)_radioSize.Width) / 2f;
				_radioBounds = new Rectangle((int)num2, (int)(base.CanvasPivot.Y + base.CanvasBounds.Height - (float)_radioSize.Height), _radioSize.Width, _radioSize.Height);
			}
		}

		public override void Render(WidgetRenderArgs args)
		{
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
			SolidBrush solidBrush = null;
			SolidBrush solidBrush2 = null;
			Pen pen = null;
			if (_enabled)
			{
				solidBrush = new SolidBrush(Color.FromArgb(num, 0, 0, 0));
				solidBrush2 = new SolidBrush(Color.FromArgb(num, 255, 255, 255));
			}
			else
			{
				solidBrush = new SolidBrush(Color.FromArgb(num, 50, 50, 50));
				solidBrush2 = new SolidBrush(Color.FromArgb(num, 150, 150, 150));
			}
			pen = new Pen(solidBrush, 1.5f);
			Graphics graphics = canvas.Graphics;
			if (RenderTag && Tag != null)
			{
				if (Align == Alignment.Horizontal)
				{
					PointF point = new PointF(base.CanvasPivot.X, base.CanvasPivot.Y);
					graphics.DrawString(Tag, WidgetServer.Instance.TextFont, solidBrush, point);
				}
				else
				{
					StringFormat stringFormat = new StringFormat();
					stringFormat.Alignment = StringAlignment.Center;
					PointF point2 = new PointF(base.CanvasPivot.X + base.Width / 2f, base.CanvasPivot.Y);
					graphics.DrawString(Tag, WidgetServer.Instance.TextFont, solidBrush, point2, stringFormat);
				}
			}
			if (!_active)
			{
				graphics.FillEllipse(solidBrush2, _radioBounds);
				graphics.DrawEllipse(pen, _radioBounds);
				return;
			}
			int num3 = (int)((double)_padding / 2.0);
			RectangleF rect = new RectangleF(_radioBounds.X + num3, _radioBounds.Y + num3, _radioBounds.Width - num3 * 2, _radioBounds.Height - num3 * 2);
			graphics.FillEllipse(solidBrush2, _radioBounds);
			graphics.FillEllipse(solidBrush, rect);
			graphics.DrawEllipse(pen, _radioBounds);
		}

		public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
		{
			if (base.CanvasBounds.Contains(e.CanvasLocation))
			{
				_active = !_active;
			}
			if (this.ValueChanged != null)
			{
				this.ValueChanged(this, new EventArgs());
			}
			return (GH_ObjectResponse)2;
		}

		public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
		{
			if (!_radioBounds.Contains((int)e.CanvasX, (int)e.CanvasY))
			{
				return (GH_ObjectResponse)0;
			}
			return (GH_ObjectResponse)3;
		}

		public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
		{
			return (GH_ObjectResponse)1;
		}

		public override bool Contains(PointF pt)
		{
			return base.CanvasBounds.Contains(pt);
		}

		public override GH_Attr_Widget IsTtipPoint(PointF pt)
		{
			if (base.CanvasBounds.Contains(pt))
			{
				return this;
			}
			return null;
		}

		public override void TooltipSetup(PointF canvasPoint, GH_TooltipDisplayEventArgs e)
		{
			e.Icon = (Bitmap)null;
			e.Title = _name + " (RadioButton)";
			e.Text = _header;
			if (_active)
			{
				e.Description = "ON";
			}
			else
			{
				e.Description = "OFF";
			}
		}
	}
}
