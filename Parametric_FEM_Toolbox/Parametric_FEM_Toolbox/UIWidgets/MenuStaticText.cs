using System.Drawing;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Parametric_FEM_Toolbox.UIWidgets;

namespace Parametric_FEM_Toolbox.UIWidgets
{
    public class MenuStaticText : GH_Attr_Widget
	{
		private string _text;

		public string Text
		{
			get
			{
				return _text;
			}
			set
			{
				_text = value;
			}
		}

		public MenuStaticText()
			: base(0, "")
		{
		}

		public override bool Write(GH_IWriter writer)
		{
			return true;
		}

		public override void Layout()
		{
		}

		public override void PostUpdateBounds(out float outHeight)
		{
			outHeight = ComputeMinSize().Height;
		}

		public override SizeF ComputeMinSize()
		{
			if (Text == null)
			{
				return default(SizeF);
			}
			Size size = TextRenderer.MeasureText(Text, WidgetServer.Instance.TextFont);
			return new SizeF(size.Width, size.Height);
		}

		public override void Render(WidgetRenderArgs args)
		{
			GH_Canvas canvas = args.Canvas;
			if (Text != null)
			{
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
				SolidBrush brush = new SolidBrush(Color.FromArgb(num, 0, 0, 0));
				PointF point = new PointF(base.CanvasPivot.X, base.CanvasPivot.Y);
				graphics.DrawString(_text, WidgetServer.Instance.TextFont, brush, point);
			}
		}

		public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
		{
			return (GH_ObjectResponse)2;
		}

		public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
		{
			return (GH_ObjectResponse)0;
		}

		public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
		{
			return (GH_ObjectResponse)1;
		}
	}
}