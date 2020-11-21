using System;
using System.Drawing;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;

namespace Parametric_FEM_Toolbox.UIWidgets
{
	public class MenuCheckBox : GH_Attr_Widget
	{
		private bool _active;

		private Rectangle _checkBounds;

		private Size _checkSize;

		private float _padding = 4f;

		public bool RenderTag
		{
			get;
			set;
		}

		public string Tag
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

		public MenuCheckBox(int index, string id, string tag)
			: base(index, id)
		{
			_checkSize = WidgetServer.Instance.CheckBoxSize;
			_padding = WidgetServer.Instance.CheckBoxPadding;
			_active = false;
			Tag = tag;
			RenderTag = true;
		}

		public override SizeF ComputeMinSize()
		{
			int num = _checkSize.Width;
			int num2 = _checkSize.Height;
			if (RenderTag && Tag != null)
			{
				Size size = TextRenderer.MeasureText(Tag, WidgetServer.Instance.TextFont);
				num += size.Width + (int)_padding;
				num2 = Math.Max(size.Height, num2);
			}
			return new SizeF(num, num2);
		}

		public override void PostUpdateBounds(out float outHeight)
		{
			outHeight = ComputeMinSize().Height;
		}

		public override void Layout()
		{
			float num = base.CanvasPivot.Y + (base.Height - (float)_checkSize.Height) / 2f;
			_checkBounds = new Rectangle((int)(base.CanvasPivot.X + base.CanvasBounds.Width - (float)_checkSize.Width), (int)num, _checkSize.Width, _checkSize.Height);
		}

		public override bool Write(GH_IWriter writer)
		{
			writer.CreateChunk("Checkbox", Index).SetBoolean("Active", _active);
			return true;
		}

		public override bool Read(GH_IReader reader)
		{
			GH_IReader gH_IReader = reader.FindChunk("Checkbox", Index);
			_active = gH_IReader.GetBoolean("Active");
			return true;
		}

		public override void Render(WidgetRenderArgs args)
		{
			GH_Canvas canvas = args.Canvas;
			Graphics graphics = canvas.Graphics;
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
			Pen pen = new Pen(Color.FromArgb(num, 0, 0, 0), 1.5f);
			SolidBrush brush = new SolidBrush(Color.FromArgb(num, 255, 255, 255));
			SolidBrush brush2 = new SolidBrush(Color.FromArgb(num, 0, 0, 0));
			if (RenderTag && Tag != null)
			{
				PointF point = new PointF(base.CanvasPivot.X, base.CanvasPivot.Y);
				graphics.DrawString(Tag, WidgetServer.Instance.TextFont, brush2, point);
			}
			graphics.FillRectangle(brush, _checkBounds);
			graphics.DrawRectangle(pen, _checkBounds);
			if (_active)
			{
				int num3 = (int)((double)_padding / 2.0);
				Rectangle rect = new Rectangle(_checkBounds.X + num3, _checkBounds.Y + num3, _checkBounds.Width - num3 * 2, _checkBounds.Height - num3 * 2);
				graphics.FillRectangle(brush2, rect);
			}
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
			return GH_ObjectResponse.Release;
		}

		public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
		{
			if (_checkBounds.Contains((int)e.CanvasX, (int)e.CanvasY))
			{
				return GH_ObjectResponse.Handled;
			}
			return GH_ObjectResponse.Ignore;
		}

		public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
		{
			return GH_ObjectResponse.Capture;
		}

		public override GH_Attr_Widget IsTtipPoint(PointF pt)
		{
			if (_checkBounds.Contains((int)pt.X, (int)pt.Y))
			{
				return this;
			}
			return null;
		}

		public override void TooltipSetup(PointF canvasPoint, GH_TooltipDisplayEventArgs e)
		{
			e.Icon = null;
			e.Title = _name + " (Checkbox)";
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
