using Eto.Drawing;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parametric_FEM_Toolbox.UIWidgets
{
	public class SilderDialog : Dialog<SliderDialogResult>
	{
		private NumericStepper precision;

		private NumericStepper numLower;

		private NumericStepper numUpper;

		private NumericStepper numValue;

		private MenuSlider _slider;

		public SilderDialog(MenuSlider slider)
		{
			((Window)this).Resizable = true;
			((Window)this).Title = "Slider Control";
			_slider = slider;
			InitializeDialog();
			((Control)numValue).Focus();
		}

		private SliderDialogResult CreateResult(DialogResult result)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return new SliderDialogResult(result, numLower.Value, numUpper.Value, numValue.Value, (int)precision.Value);
		}

		private static void UpdateStepperPrecision(NumericStepper stepper, int numDecimals)
		{
			if (numDecimals < 0)
			{
				numDecimals = 0;
			}
			double increment = 1.0 / Math.Pow(10.0, numDecimals);
			stepper.DecimalPlaces = numDecimals;
			stepper.Increment = increment;
			decimal d = (decimal)stepper.Value;
			decimal d2 = (decimal)Math.Pow(10.0, numDecimals);
			d = Math.Truncate(d * d2) / d2;
			stepper.Value = (double)d;
		}

		private void InitializeDialog()
		{
			Label label = new Label();
			label.Text = "Precision(Number of Decimals)";
			precision = new NumericStepper();
			precision.Increment = 1.0;
			precision.Value = _slider.NumDecimals;
			precision.DecimalPlaces = 0;
			precision.MinValue = 0.0;
			precision.MaxValue = 12.0;
			precision.ValueChanged += delegate
			{
				int numDecimals = (int)precision.Value;
				UpdateStepperPrecision(numLower, numDecimals);
				UpdateStepperPrecision(numUpper, numDecimals);
				UpdateStepperPrecision(numValue, numDecimals);
			};
			base.DefaultButton = new Button
			{
				Text = "OK"
			};
			base.DefaultButton.Click += delegate
			{
				Close(CreateResult(DialogResult.Ok));
			};
			base.AbortButton = new Button
			{
				Text = "Cancel"
			};
			base.AbortButton.Click += delegate
			{
				Close(CreateResult(DialogResult.Cancel));
			};
			Label label2 = new Label();
			label2.Text = "Min Value";
			Label label3 = new Label();
			label3.Text = "Max Value";
			Label label4 = new Label();
			label4.Text = "Current Value";
			double increment = 1.0 / Math.Pow(10.0, _slider.NumDecimals);
			numLower = new NumericStepper();
			numLower.Increment = increment;
			numLower.ToolTip = "Minimum slider value";
			numLower.DecimalPlaces = _slider.NumDecimals;
			numUpper = new NumericStepper();
			numUpper.Increment = increment;
			numUpper.ToolTip = "Maximum slider value";
			numUpper.DecimalPlaces = _slider.NumDecimals;
			numValue = new NumericStepper();
			numValue.Increment = increment;
			numValue.ToolTip = "Current slider value";
			numValue.DecimalPlaces = _slider.NumDecimals;
			numLower.ValueChanged += delegate
			{
				double value3 = numLower.Value;
				if (value3 > numUpper.Value)
				{
					numUpper.Value = value3;
				}
				if (value3 > numValue.Value)
				{
					numValue.Value = value3;
				}
			};
			numUpper.ValueChanged += delegate
			{
				double value2 = numUpper.Value;
				if (value2 < numLower.Value)
				{
					numLower.Value = value2;
				}
				if (value2 < numValue.Value)
				{
					numValue.Value = value2;
				}
			};
			numValue.ValueChanged += delegate
			{
				double value = numValue.Value;
				if (value < numLower.Value)
				{
					numLower.Value = value;
				}
				if (value > numUpper.Value)
				{
					numUpper.Value = value;
				}
			};
			numLower.Value = _slider.MinValue;
			numUpper.Value = _slider.MaxValue;
			numValue.Value = _slider.Value;
			TableLayout tableLayout = new TableLayout();
			tableLayout.Padding = new Padding(5, 10, 5, 5);
			tableLayout.Spacing = new Size(5, 5);
			tableLayout.Rows.Add(new TableRow(new TableCell(label), new TableCell(precision, scaleWidth: true)));
			tableLayout.Rows.Add(new TableRow(label2, new TableCell(numLower, scaleWidth: true)));
			tableLayout.Rows.Add(new TableRow(label3, new TableCell(numUpper, scaleWidth: true)));
			tableLayout.Rows.Add(new TableRow(label4, new TableCell(numValue, scaleWidth: true)));
			TableLayout control = tableLayout;
			tableLayout = new TableLayout();
			tableLayout.Padding = new Padding(5, 10, 5, 5);
			tableLayout.Spacing = new Size(5, 5);
			tableLayout.Rows.Add(new TableRow(null, base.DefaultButton, base.AbortButton, null));
			TableLayout control2 = tableLayout;
			base.Content = new TableLayout
			{
				Padding = new Padding(5),
				Spacing = new Size(5, 5),
				Rows =
			{
				new TableRow(control),
				new TableRow(control2)
			}
			};
		}
	}
}
