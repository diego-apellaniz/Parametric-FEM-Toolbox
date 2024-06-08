using System;
using System.Collections.Generic;
using System.Drawing;
using GH_IO.Serialization;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;

namespace Parametric_FEM_Toolbox.UIWidgets
{
    class MenuRadioButtonGroup : GH_Attr_Widget
    {
		public enum LayoutDirection
		{
			Vertical,
			Horizontal
		}

		private List<MenuRadioButton> _buttons;

		private int minActive = 1;

		private int maxActive = 1;

		private List<MenuRadioButton> actives;

		private MenuRadioButton _activeControl;

		private float _space;

		public LayoutDirection Direction
		{
			get;
			set;
		}

		public override bool Enabled
		{
			get
			{
				return _enabled;
			}
			set
			{
				_enabled = value;
				int count = _buttons.Count;
				for (int i = 0; i < count; i++)
				{
					_buttons[i].Enabled = _enabled;
				}
			}
		}

		public int MaxActive
		{
			get
			{
				return maxActive;
			}
			set
			{
				if (value < 1)
				{
					maxActive = 1;
				}
				else
				{
					maxActive = value;
				}
				UpdateSettings();
			}
		}

		public int MinActive
		{
			get
			{
				return minActive;
			}
			set
			{
				if (value < 0)
				{
					minActive = 0;
				}
				else
				{
					minActive = value;
				}
				UpdateSettings();
			}
		}

		public event ValueChangeEventHandler ValueChanged;

		public MenuRadioButtonGroup(int index, string id)
			: base(index, id)
		{
			_buttons = new List<MenuRadioButton>();
			actives = new List<MenuRadioButton>();
			Direction = LayoutDirection.Vertical;
			_space = 5f;
		}

		public override SizeF ComputeMinSize()
		{
			if (Direction == LayoutDirection.Vertical)
			{
				float num = 0f;
				float num2 = 0f;
				int num3 = 0;
				foreach (MenuRadioButton button in _buttons)
				{
					if (num3++ > 0)
					{
						num2 += _space;
					}
					SizeF sizeF = button.ComputeMinSize();
					num = Math.Max(sizeF.Width, num);
					num2 += sizeF.Height;
				}
				return new SizeF(num, num2);
			}
			if (Direction == LayoutDirection.Horizontal)
			{
				float num4 = 0f;
				float num5 = 0f;
				foreach (MenuRadioButton button2 in _buttons)
				{
					SizeF sizeF2 = button2.ComputeMinSize();
					num4 = Math.Max(sizeF2.Width, num4);
					num5 = Math.Max(sizeF2.Height, num5);
				}
				return new SizeF(num4 * (float)_buttons.Count + _space * (float)_buttons.Count, num5);
			}
			throw new NotImplementedException("todo");
		}

		public override void Layout()
		{
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			if (Direction == LayoutDirection.Vertical)
			{
				float num = base.CanvasPivot.Y;
				float num2 = num;
				int num3 = 0;
				foreach (MenuRadioButton button in _buttons)
				{
					if (num3++ > 0)
					{
						num += _space;
					}
					PointF transform = new PointF(base.CanvasPivot.X, num);
					button.UpdateBounds(transform, base.Width);
					button.Style = base.Style;
					button.Palette = base.Palette;
					button.Layout();
					num += button.Height;
				}
				base.Height = num - num2;
				return;
			}
			if (Direction == LayoutDirection.Horizontal)
			{
				float num4 = base.CanvasPivot.X + _space / 2f;
				int num5 = 0;
				float num6 = 0f;
				if (_buttons.Count > 0)
				{
					num6 = base.Width / (float)_buttons.Count - _space;
				}
				float num7 = 0f;
				foreach (MenuRadioButton button2 in _buttons)
				{
					if (num5++ > 0)
					{
						num4 += _space;
					}
					PointF transform2 = new PointF(num4, base.CanvasPivot.Y);
					button2.UpdateBounds(transform2, num6);
					button2.Style = base.Style;
					button2.Palette = base.Palette;
					button2.Layout();
					num4 += num6;
					num7 = Math.Max(button2.Height, num7);
				}
				base.Height = num7;
				return;
			}
			throw new NotImplementedException("todo");
		}

		public void AddButton(MenuRadioButton button)
		{
			if (button.Active && actives.Count < maxActive)
			{
				actives.Add(button);
			}
			button.Parent = this;
			button.Enabled = _enabled;
			_buttons.Add(button);
		}

		public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
		{
			int count = _buttons.Count;
			for (int i = 0; i < count; i++)
			{
				if (!_buttons[i].Contains(e.CanvasLocation) || _buttons[i] != _activeControl)
				{
					continue;
				}
				if (_buttons[i].Active)
				{
					if (actives.Count - 1 >= minActive)
					{
						_buttons[i].Active = false;
						actives.Remove(_buttons[i]);
					}
				}
				else
				{
					if (actives.Count == maxActive)
					{
						actives[0].Active = false;
						actives.RemoveAt(0);
					}
					_buttons[i].Active = true;
					actives.Add(_buttons[i]);
				}
				if (this.ValueChanged != null)
				{
					this.ValueChanged(this, new EventArgs());
				}
				Update();
			}
			return (GH_ObjectResponse)2;
		}

		public override bool Write(GH_IWriter writer)
		{
			GH_IWriter obj = writer.CreateChunk("RadioButtonGroup", Index);
			int count = _buttons.Count;
			obj.SetInt32("Count", count);
			GH_IWriter val = obj.CreateChunk("Active");
			for (int i = 0; i < count; i++)
			{
				val.SetBoolean("button", i, _buttons[i].Active);
			}
			return true;
		}

		public override bool Read(GH_IReader reader)
		{
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Expected O, but got Unknown
			string text = "RadioButtonGroup";
			GH_IReader val = reader.FindChunk(text, Index);
			if (val == null)
			{
				List<GH_IChunk> chunks = ((GH_IChunk)reader).Chunks;
				GH_IChunk val2 = null;
				foreach (GH_IChunk chunk in ((GH_IChunk)reader).Chunks)
				{
					if (chunk.Name.Equals(text))
					{
						val2 = chunk;
						break;
					}
				}
				if (val2 == null)
				{
					throw new ArgumentException("fail");
				}
				if (chunks.Count != 1)
				{
					throw new ArgumentException("RadioButtonGroup could not be loaded");
				}
				val = (GH_IReader)(GH_Chunk)val2;
			}
			int count = _buttons.Count;
			int @int = val.GetInt32("Count");
			GH_IReader val3 = val.FindChunk("Active");
			actives.Clear();
			for (int i = 0; i < count; i++)
			{
				bool boolean = val3.GetBoolean("button", i);
				_buttons[i].Active = boolean;
				if (boolean)
				{
					actives.Add(_buttons[i]);
				}
			}
			return true;
		}

		public override bool Contains(PointF pt)
		{
			int count = _buttons.Count;
			for (int i = 0; i < count; i++)
			{
				if (_buttons[i].Contains(pt))
				{
					return true;
				}
			}
			return false;
		}

		public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			int count = _buttons.Count;
			for (int i = 0; i < count; i++)
			{
				if (_buttons[i].Contains(e.CanvasLocation))
				{
					GH_ObjectResponse val = _buttons[i].RespondToMouseDown(sender, e);
					if ((int)val != 0)
					{
						_activeControl = _buttons[i];
						return val;
					}
				}
			}
			return (GH_ObjectResponse)0;
		}

		public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
		{
			return (GH_ObjectResponse)1;
		}

		public override GH_Attr_Widget IsTtipPoint(PointF pt)
		{
			int count = _buttons.Count;
			for (int i = 0; i < count; i++)
			{
				GH_Attr_Widget gH_Attr_Widget = _buttons[i].IsTtipPoint(pt);
				if (gH_Attr_Widget != null)
				{
					return gH_Attr_Widget;
				}
			}
			return null;
		}

		public override void Render(WidgetRenderArgs args)
		{
			int count = _buttons.Count;
			for (int i = 0; i < count; i++)
			{
				_buttons[i].Render(args);
			}
		}

		public List<MenuRadioButton> GetActive()
		{
			return actives;
		}

		public List<bool> GetPattern()
		{
			List<bool> list = new List<bool>();
			int count = _buttons.Count;
			for (int i = 0; i < count; i++)
			{
				list.Add(_buttons[i].Active);
			}
			return list;
		}

		public List<int> GetActiveInt()
		{
			List<int> list = new List<int>();
			int count = _buttons.Count;
			for (int i = 0; i < count; i++)
			{
				if (_buttons[i].Active)
				{
					list.Add(i);
				}
			}
			return list;
		}

		public bool SetActive(int index)
		{
			if (index < _buttons.Count)
			{
				if (!_buttons[index].Active)
				{
					actives.Add(_buttons[index]);
					_buttons[index].Active = true;
				}
				return true;
			}
			return false;
		}

		private void UpdateSettings()
		{
			if (minActive > maxActive)
			{
				minActive = maxActive;
			}
		}

		private void Update()
		{
			_ = _buttons.Count;
			int count = actives.Count;
			if (count > maxActive)
			{
				int num = count - maxActive;
				for (int i = 0; i < num; i++)
				{
					MenuRadioButton menuRadioButton = actives[0];
					actives.RemoveAt(0);
					menuRadioButton.Active = false;
				}
			}
			else
			{
				if (count >= minActive)
				{
					return;
				}
				int num2 = count - maxActive;
				int num3 = 0;
				for (int j = 0; j < num2; j++)
				{
					if (!actives.Contains(_buttons[num3]))
					{
						MenuRadioButton menuRadioButton2 = _buttons[num3];
						menuRadioButton2.Active = true;
						actives.Add(menuRadioButton2);
						j--;
					}
					num3++;
				}
			}
		}

		public override string GetWidgetDescription()
		{
			string str = base.GetWidgetDescription() + "{\n";
			foreach (MenuRadioButton button in _buttons)
			{
				str = str + button.GetWidgetDescription() + "\n";
			}
			return str + "}";
		}
	}
}
