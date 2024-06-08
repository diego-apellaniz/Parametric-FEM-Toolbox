using Eto.Drawing;
using GH_IO.Serialization;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parametric_FEM_Toolbox.UIWidgets
{
	public class MenuSlider : GH_Attr_Widget
	{
		private double _step;

		private float _sliderWidth;

		private double currentValue;

		private double maxValue = 1.0;

		private double minValue;

		private int numDecimals = 2;

		private System.Drawing.RectangleF _handleArea;

		private System.Drawing.RectangleF _textBounds;

		private float _handleDiameter = 8f;

		private string _number_format = "{0:0.00}";

		public string NumberFormat
		{
			get
			{
				return _number_format;
			}
			set
			{
				_number_format = value;
			}
		}

		public double Step
		{
			get
			{
				return _step;
			}
			set
			{
				_step = value;
				if (_step < 0.0)
				{
					_step = 0.0;
				}
			}
		}

		public int NumDecimals
		{
			get
			{
				return numDecimals;
			}
			set
			{
				numDecimals = Math.Min(0, Math.Max(12, value));
			}
		}

		public double Value
		{
			get
			{
				return currentValue;
			}
			set
			{
				if (value > maxValue)
				{
					currentValue = maxValue;
				}
				else if (value < minValue)
				{
					currentValue = minValue;
				}
				else
				{
					currentValue = value;
				}
			}
		}

		public double MaxValue
		{
			get
			{
				return maxValue;
			}
			set
			{
				maxValue = value;
				if (currentValue > maxValue)
				{
					currentValue = maxValue;
				}
			}
		}

		public double MinValue
		{
			get
			{
				return minValue;
			}
			set
			{
				minValue = value;
				if (currentValue < minValue)
				{
					currentValue = minValue;
				}
			}
		}

		public event ValueChangeEventHandler ValueChanged;

		public MenuSlider(int index, string id, double min, double max, double value, int numDecimals)
			: base(index, id)
		{
			minValue = min;
			maxValue = max;
			currentValue = value;
			this.numDecimals = numDecimals;
			_step = 0.0;
			FixValues();
		}

		private void FixValues()
		{
			numDecimals = Math.Max(0, Math.Min(12, numDecimals));
			minValue = TruncateValue(minValue, numDecimals);
			maxValue = TruncateValue(maxValue, numDecimals);
			currentValue = TruncateValue(currentValue, numDecimals);
			if (minValue > maxValue)
			{
				minValue = maxValue;
			}
			if (currentValue < minValue)
			{
				currentValue = minValue;
			}
			if (currentValue > maxValue)
			{
				currentValue = maxValue;
			}
			if (_step < 0.0)
			{
				_step = 0.0;
			}
		}

		private static double TruncateValue(double value, int numDecimals)
		{
			if (numDecimals < 0)
			{
				numDecimals = 0;
			}
			decimal d = (decimal)value;
			decimal d2 = (decimal)Math.Pow(10.0, numDecimals);
			return (double)(Math.Truncate(d * d2) / d2);
		}

		public override bool Write(GH_IWriter writer)
		{
			GH_IWriter gH_IWriter = writer.CreateChunk("Slider", Index);
			gH_IWriter.SetDouble("MinValue", minValue);
			gH_IWriter.SetDouble("MaxValue", maxValue);
			gH_IWriter.SetDouble("CurrentValue", currentValue);
			gH_IWriter.SetInt32("NumDecimals", numDecimals);
			return true;
		}

		public override bool Read(GH_IReader reader)
		{
			GH_IReader gH_IReader = reader.FindChunk("Slider", Index);
			minValue = gH_IReader.GetDouble("MinValue");
			maxValue = gH_IReader.GetDouble("MaxValue");
			currentValue = gH_IReader.GetDouble("CurrentValue");
			if (!gH_IReader.TryGetInt32("NumDecimals", ref numDecimals))
			{
				numDecimals = 2;
			}
			FixValues();
			return true;
		}

		public override System.Drawing.SizeF ComputeMinSize()
		{
			float height = Math.Max(TextRenderer.MeasureText("1", WidgetServer.Instance.SliderValueTagFont).Height, _handleDiameter);
			return new System.Drawing.SizeF(_handleDiameter + 20f, height);
		}

		public override void Layout()
		{
			double num = 0.0;
			double num2 = maxValue - minValue;
			if (_step > 0.001)
			{
				currentValue = (double)(int)((currentValue + _step / 2.0) / _step) * _step;
			}
			num = ((!(num2 < 1E-10)) ? ((currentValue - minValue) / num2) : 0.5);
			currentValue = TruncateValue(currentValue, numDecimals);
			float num3 = (float)((double)_sliderWidth * num);
			_handleArea = new System.Drawing.RectangleF(base.CanvasPivot.X + num3, base.CanvasPivot.Y + base.Height / 2f - _handleDiameter / 2f, _handleDiameter, _handleDiameter);
			if (Value < (MaxValue + MinValue) / 2.0)
			{
				_textBounds = new System.Drawing.RectangleF(_handleArea.Right + 3f, base.CanvasPivot.Y, canvasBounds.Right - (_handleArea.Right + 3f), base.Height);
			}
			else
			{
				_textBounds = new System.Drawing.RectangleF(base.CanvasPivot.X, base.CanvasPivot.Y, _handleArea.Left - base.CanvasPivot.X - 3f, base.Height);
			}
		}

		public override void Render(WidgetRenderArgs args)
		{
			GH_Canvas canvas = args.Canvas;
			System.Drawing.Graphics graphics = canvas.Graphics;
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
			int alpha = (int)((double)num * 0.2);
			System.Drawing.Pen pen = null;
			System.Drawing.Pen pen2 = null;
			System.Drawing.SolidBrush solidBrush = null;
			System.Drawing.SolidBrush solidBrush2 = null;
			if (_enabled)
			{
				pen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(alpha, 0, 0, 0), 1f);
				pen2 = new System.Drawing.Pen(System.Drawing.Color.FromArgb(num, 0, 0, 0), 2f);
				solidBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(num, 255, 255, 255));
				solidBrush2 = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(num, 0, 0, 0));
			}
			else
			{
				pen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(alpha, 50, 50, 50), 1f);
				pen2 = new System.Drawing.Pen(System.Drawing.Color.FromArgb(num, 50, 50, 50), 2f);
				solidBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(num, 150, 150, 150));
				solidBrush2 = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(num, 50, 50, 50));
			}
			graphics.DrawLine(pen, base.CanvasPivot.X + _handleDiameter / 2f, base.CanvasPivot.Y + base.Height / 2f, base.CanvasPivot.X + base.Width - _handleDiameter / 2f, base.CanvasPivot.Y + base.Height / 2f);
			graphics.DrawLine(pen, base.CanvasPivot.X + _handleDiameter / 2f, base.CanvasPivot.Y + base.Height / 2f - 3f, base.CanvasPivot.X + _handleDiameter / 2f, base.CanvasPivot.Y + base.Height / 2f + 3f);
			graphics.DrawLine(pen, base.CanvasPivot.X + base.Width - _handleDiameter / 2f, base.CanvasPivot.Y + base.Height / 2f - 3f, base.CanvasPivot.X + base.Width - _handleDiameter / 2f, base.CanvasPivot.Y + base.Height / 2f + 3f);
			graphics.FillEllipse(solidBrush, _handleArea);
			graphics.DrawEllipse(pen2, _handleArea);
			StringFormat stringFormat = new StringFormat();
			stringFormat.Trimming = StringTrimming.EllipsisCharacter;
			stringFormat.LineAlignment = StringAlignment.Near;
			if (Value < (MaxValue + MinValue) / 2.0)
			{
				stringFormat.Alignment = StringAlignment.Near;
			}
			else
			{
				stringFormat.Alignment = StringAlignment.Far;
			}
			string s = currentValue.ToString();
			graphics.DrawString(s, WidgetServer.Instance.SliderValueTagFont, solidBrush2, _textBounds, stringFormat);
		}

		public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
		{
			return (GH_ObjectResponse)2;
		}

		public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
		{
			return (GH_ObjectResponse)3;
		}

		public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
		{
			System.Drawing.PointF canvasPivot = base.CanvasPivot;
			if (e.CanvasLocation.X < canvasPivot.X + 5f)
			{
				currentValue = minValue;
			}
			else if (e.CanvasLocation.X > canvasPivot.X + 5f + _sliderWidth)
			{
				currentValue = maxValue;
			}
			else
			{
				double num = (double)(e.CanvasLocation.X - (canvasPivot.X + 5f)) / (double)_sliderWidth;
				double num2 = (maxValue - minValue) * num;
				currentValue = num2 + minValue;
			}
			Layout();
			FireChangedEvent();
			return (GH_ObjectResponse)1;
		}

		public void FireChangedEvent()
		{
			if (this.ValueChanged != null)
			{
				this.ValueChanged(this, new EventArgs());
			}
		}

		public override GH_ObjectResponse RespondToMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
		{
			try
			{
				float scalingFactor = WidgetServer.getScalingFactor();
				SilderDialog silderDialog = new SilderDialog(this);
				System.Drawing.Rectangle bounds = System.Windows.Forms.Screen.FromControl((System.Windows.Forms.Control)(object)sender).Bounds;
				System.Drawing.Point point = new System.Drawing.Point((int)((float)(bounds.Width / 2) / scalingFactor), (int)((float)(bounds.Height / 2) / scalingFactor));
				silderDialog.Location = new Eto.Drawing.Point(point.X - 200, point.Y - 200);
				SliderDialogResult sliderDialogResult = silderDialog.ShowModal();
				if (sliderDialogResult.Status == Eto.Forms.DialogResult.Ok)
				{
					minValue = sliderDialogResult.MinValue;
					maxValue = sliderDialogResult.MaxValue;
					currentValue = sliderDialogResult.CurrentValue;
					numDecimals = sliderDialogResult.NumDecimals;
					FireChangedEvent();
				}
			}
			catch (Exception)
			{
			}
			return (GH_ObjectResponse)0;
		}

		public override void PostUpdateBounds(out float outHeight)
		{
			_sliderWidth = base.Width - 10f;
			outHeight = ComputeMinSize().Height;
		}

		public override GH_Attr_Widget IsTtipPoint(System.Drawing.PointF pt)
		{
			if (new System.Drawing.RectangleF(transfromation.X, transfromation.Y, base.Width, 10f).Contains(pt))
			{
				return this;
			}
			return null;
		}

		public override void TooltipSetup(System.Drawing.PointF canvasPoint, GH_TooltipDisplayEventArgs e)
		{
			e.Icon=((System.Drawing.Bitmap)null);
			e.Title=(_name + " (Slider)");
			e.Text=(_header);
			e.Description=("Value: " + currentValue + "\nMinValue: " + minValue + "\nMaxValue: " + maxValue);
		}

		public override bool Contains(System.Drawing.PointF pt)
		{
			if (_handleArea.Contains(pt))
			{
				return true;
			}
			return false;
		}
	}
}
