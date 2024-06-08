using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eto.Forms;

namespace Parametric_FEM_Toolbox.UIWidgets
{
    public sealed class SliderDialogResult
    {
		public DialogResult Status
		{
			get;
			private set;
		}

		public double MinValue
		{
			get;
			private set;
		}

		public double MaxValue
		{
			get;
			private set;
		}

		public double CurrentValue
		{
			get;
			private set;
		}

		public int NumDecimals
		{
			get;
			private set;
		}

		public SliderDialogResult(DialogResult result, double min, double max, double current, int numDecimals)
		{
			Status = result;
			MinValue = min;
			MaxValue = max;
			CurrentValue = current;
			NumDecimals = numDecimals;
		}
	}
}
