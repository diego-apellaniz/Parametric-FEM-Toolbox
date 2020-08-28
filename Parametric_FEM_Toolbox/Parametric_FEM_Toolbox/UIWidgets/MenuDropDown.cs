using GH_IO.Serialization;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Parametric_FEM_Toolbox.UIWidgets
{
    public class MenuDropDown : GH_Attr_Widget
    {
        private class MenuDropDownWindow : GH_Attr_Widget
        {
            private class MenuScrollBar : GH_Attr_Widget
            {
                private Rectangle _content;

                private Rectangle _drag;

                public bool _active;

                public int numItems;

                public int numVisibleItems;

                private PointF _clickPos;

                private float _localTop;

                private float _localBottom;

                private float _dragHeight;

                public double _ratio;

                private float _currentHight;

                private int _startIndex;

                private int _endIndex;

                public int[] VisibleRange => new int[2]
                {
                _startIndex,
                _endIndex
                };

                public MenuScrollBar()
                    : base(0, "")
                {
                    base.Width = 10f;
                }

                public void SetClick(PointF click)
                {
                    _clickPos = click;
                    _dragHeight = base.Height * (float)_ratio;
                    float num = _clickPos.Y - transformation.Y;
                    _localTop = num - _currentHight;
                    _localBottom = _dragHeight - _localTop;
                }

                public override void Render(WidgetRenderArgs args)
                {
                    if (args.Channel == WidgetChannel.Overlay)
                    {
                        Graphics graphics = args.Canvas.Graphics;
                        graphics.FillRectangle(Brushes.Gray, _content);
                        graphics.FillRectangle(Brushes.Black, _drag);
                    }
                }

                public override SizeF ComputeMinSize()
                {
                    return new SizeF(10f, 10f);
                }

                public override void Layout()
                {
                    _content = GH_Attr_Widget.Convert(base.CanvasBounds);
                    _drag = new Rectangle((int)transformation.X, (int)transformation.Y + (int)_currentHight, (int)base.Width, (int)((double)base.Height * _ratio));
                }

                public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
                {
                    float num = e.CanvasLocation.Y - transformation.Y;
                    float num2 = num - _localTop;
                    float num3 = num + _localBottom;
                    if (num2 < 0f)
                    {
                        _currentHight = 0f;
                        _startIndex = 0;
                        _endIndex = numVisibleItems;
                    }
                    else if (num3 > base.Height)
                    {
                        _currentHight = base.Height - _dragHeight;
                        _startIndex = numItems - numVisibleItems;
                        _endIndex = numItems;
                    }
                    else
                    {
                        _currentHight = num2;
                        _startIndex = (int)(_currentHight / base.Height * (float)numItems);
                        _endIndex = _startIndex + numVisibleItems;
                    }
                    return GH_ObjectResponse.Capture;
                }

                public void Update()
                {
                    if (_currentHight == 0f)
                    {
                        _startIndex = 0;
                        _endIndex = numVisibleItems;
                    }
                    else if (_currentHight == base.Height - _dragHeight)
                    {
                        _startIndex = numItems - numVisibleItems;
                        _endIndex = numItems;
                    }
                    else
                    {
                        _startIndex = (int)(_currentHight / base.Height * (float)numItems);
                        _endIndex = _startIndex + numVisibleItems;
                    }
                }

                public void SetSlider(int start, int length)
                {
                    _startIndex = start;
                    _endIndex = start + length;
                    double num = (double)start / (double)numItems * (double)base.Height;
                    _currentHight = (float)num;
                    _dragHeight = (float)(_ratio * (double)base.Height);
                }

                public override bool Contains(PointF pt)
                {
                    return _content.Contains((int)pt.X, (int)pt.Y);
                }
            }

            private MenuDropDown _dropMenu;

            private MenuScrollBar _scrollBar;

            private int _tempActive = -1;

            private int _tempStart;

            private double _ratio = 1.0;

            private Rectangle _contentBox;

            private Rectangle _resizeBox;

            private int _resizeBoxSize = 10;

            private bool _resizeActive;

            private int _maxLen;

            public MenuDropDownWindow(MenuDropDown parent)
                : base(0, "")
            {
                _scrollBar = new MenuScrollBar();
                _scrollBar.Width = 10f;
                _scrollBar.Parent = this;
                _dropMenu = parent;
            }

            public void Update()
            {
                int count = _dropMenu.Items.Count;
                int num = Math.Min(_dropMenu.VisibleItemCount, count);
                if (_dropMenu.LastValidValue > count)
                {
                    _dropMenu.Value = -1;
                }
                if (_dropMenu.Items.Count == 0)
                {
                    _tempStart = 0;
                }
                _maxLen = num;
                _ratio = (double)num / (double)count;
                int num2 = num * 20;
                base.Height = num2 + _resizeBoxSize;
                _contentBox = new Rectangle((int)base.CanvasPivot.X, (int)base.CanvasPivot.Y, (int)base.Width, num2);
                _scrollBar.Height = num2;
                _scrollBar._ratio = _ratio;
                _scrollBar.numItems = count;
                _scrollBar.numVisibleItems = num;
                _scrollBar.SetSlider(_tempStart, count);
                _scrollBar.Layout();
                PointF transform = new PointF(base.CanvasPivot.X + base.Width - _scrollBar.Width, base.CanvasPivot.Y);
                _scrollBar.UpdateBounds(transform, _scrollBar.Width);
            }

            public override SizeF ComputeMinSize()
            {
                if (_dropMenu != null && !_dropMenu.Empty)
                {
                    foreach (Entry item in _dropMenu.Items)
                    {
                        Entry entry = item;
                    }
                }
                return _scrollBar.ComputeMinSize();
            }

            public override void Layout()
            {
                Update();
                _resizeBox = new Rectangle((int)base.Transform.X + (int)base.Width - _resizeBoxSize, (int)base.Transform.Y + (int)base.Height - _resizeBoxSize, _resizeBoxSize, _resizeBoxSize);
            }

            public override void Render(WidgetRenderArgs args)
            {
                if (args.Channel != WidgetChannel.Overlay)
                {
                    return;
                }
                Graphics graphics = args.Canvas.Graphics;
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                Pen pen = new Pen(Brushes.Black);
                graphics.DrawRectangle(pen, _contentBox);
                graphics.FillRectangle(Brushes.White, _contentBox);
                int num = 0;
                for (int i = _tempStart; i < _tempStart + _maxLen; i++)
                {
                    Brush white = Brushes.White;
                    Brush white2 = Brushes.White;
                    if (i == _tempActive)
                    {
                        white = Brushes.Blue;
                        white2 = Brushes.White;
                    }
                    else if (i == _dropMenu.Value)
                    {
                        white = Brushes.LightBlue;
                        white2 = Brushes.Black;
                    }
                    else
                    {
                        white = new SolidBrush(Color.White);
                        white2 = Brushes.Black;
                    }
                    Rectangle rect = new Rectangle((int)base.Transform.X, (int)base.Transform.Y + 20 * num, (int)base.Width, 20);
                    graphics.FillRectangle(white, rect);
                    graphics.DrawString(_dropMenu.Items[i].content, WidgetServer.Instance.DropdownFont, white2, base.Transform.X + base.Width / 2f, (int)base.Transform.Y + 20 * num + 5, stringFormat);
                    num++;
                }
                _scrollBar.Render(args);
                graphics.FillRectangle(new SolidBrush(Color.FromArgb(120, 80, 80, 80)), _resizeBox);
                graphics.DrawString("+", WidgetServer.Instance.DropdownFont, new SolidBrush(Color.FromArgb(255, 0, 0, 0)), _resizeBox.Location.X + 5, _resizeBox.Location.Y - 3, stringFormat);
            }

            public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
            {
                if (_resizeActive || _scrollBar._active)
                {
                    _scrollBar._active = false;
                    _resizeActive = false;
                }
                return GH_ObjectResponse.Capture;
            }

            public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
            {
                if (_scrollBar._active)
                {
                    _scrollBar.RespondToMouseMove(sender, e);
                    int[] visibleRange = _scrollBar.VisibleRange;
                    _tempStart = visibleRange[0];
                }
                else if (_resizeActive)
                {
                    float num = e.CanvasLocation.Y - transformation.Y;
                    if (num > 20f)
                    {
                        int num2 = (int)(num / 20f);
                        if (num2 + _tempStart < _dropMenu.Items.Count)
                        {
                            _dropMenu.VisibleItemCount = num2;
                            Update();
                        }
                        else if (num2 <= _dropMenu.Items.Count)
                        {
                            _tempStart = _dropMenu.Items.Count - num2;
                            _dropMenu.VisibleItemCount = num2;
                            Update();
                        }
                    }
                }
                else if (Contains(e.CanvasLocation))
                {
                    _tempActive = _tempStart + (int)((e.CanvasLocation.Y - base.Transform.Y) / 20f);
                }
                else
                {
                    _tempActive = -1;
                }
                return GH_ObjectResponse.Capture;
            }

            public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
            {
                _scrollBar._active = false;
                _resizeActive = false;
                if (_scrollBar.Contains(e.CanvasLocation))
                {
                    _scrollBar._active = true;
                    _scrollBar.SetClick(e.CanvasLocation);
                    return GH_ObjectResponse.Capture;
                }
                if (_resizeBox.Contains((int)e.CanvasLocation.X, (int)e.CanvasLocation.Y))
                {
                    _resizeActive = true;
                    return GH_ObjectResponse.Capture;
                }
                if (_contentBox.Contains((int)e.CanvasLocation.X, (int)e.CanvasLocation.Y))
                {
                    _dropMenu.Value = _tempStart + (int)((e.CanvasLocation.Y - base.Transform.Y) / 20f);
                    _tempActive = -1;
                    _resizeActive = false;
                    _dropMenu.HideWindow(fire: true);
                    return GH_ObjectResponse.Release;
                }
                _dropMenu.HideWindow(fire: false);
                return GH_ObjectResponse.Release;
            }

            public override bool Contains(PointF pt)
            {
                return canvasBounds.Contains(pt);
            }
        }

        public class Entry
        {
            public string content;

            public string name;

            public int index;

            public object data;

            public Entry(string name, string content, int ind)
            {
                this.content = content;
                this.name = name;
                index = ind;
            }
        }

        private MenuDropDownWindow _window;

        public bool expanded;

         private static int default_item_index = 0;

        private int current_value;

        private int last_valid_value;

        private int _visibleItemCount = 4;

        private List<Entry> _items;

        private string _emptyText = "empty";

        public int Value
        {
            get
            {
                return current_value;
            }
            set
            {
                current_value = Math.Max(value, 0);
                last_valid_value = ((value >= 0) ? value : 0);
            }
        }

        public int LastValidValue => last_valid_value;

        public List<Entry> Items => _items;

        private bool Empty => _items.Count == 0;

        public int VisibleItemCount
        {
            get
            {
                return _visibleItemCount;
            }
            set
            {
                if (_visibleItemCount < 1)
                {
                    _visibleItemCount = 1;
                }
                else
                {
                    _visibleItemCount = value;
                }
            }
        }

        public event ValueChangeEventHandler ValueChanged;

        public int FindIndex(string name)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].name.Equals(name))
                {
                    return i;
                }
            }
            return -1;
        }

        public override void PostUpdateBounds(out float outHeight)
        {
            _window.Width = base.Width;
            outHeight = ComputeMinSize().Height;
        }

        public MenuDropDown(int index, string id, string tag)
            : base(index, id)
        {
            _items = new List<Entry>();
            _window = new MenuDropDownWindow(this);
            _window.Parent = this;
        }

        public void AddItem(string name, string cont)
        {
            Entry item = new Entry(name, cont, _items.Count);
            _items.Add(item);
            Update();
        }

        public void AddItem(string name, string cont, object data)
        {
            Entry entry = new Entry(name, cont, _items.Count);
            entry.data = data;
            _items.Add(entry);
            Update();
        }

        private void Update()
        {
            if (_items.Count == 0)
            {
                current_value = 0;
            }
            _window.Update();
        }

        public override void Layout()
        {
            _window.UpdateBounds(base.CanvasPivot, base.Width);
            _window.Layout();
        }

        public void Clear()
        {
            _items.Clear();
            Update();
        }

        public override SizeF ComputeMinSize()
        {
            int num = 0;
            int num2 = 0;
            if (Empty)
            {
                Size size = TextRenderer.MeasureText(_emptyText, WidgetServer.Instance.DropdownFont);
                num = size.Width + 4 + 10;
                num2 = size.Height + 2;
            }
            else
            {
                foreach (Entry item in _items)
                {
                    Size size2 = TextRenderer.MeasureText(item.content, WidgetServer.Instance.DropdownFont);
                    int val = size2.Width + 4 + 10;
                    int val2 = size2.Height + 2;
                    num = Math.Max(num, val);
                    num2 = Math.Max(num2, val2);
                }
            }
            return new SizeF(num, num2);
        }

        public override void Render(WidgetRenderArgs args)
        {
            GH_Canvas canvas = args.Canvas;
            if (args.Channel == WidgetChannel.Overlay)
            {
                if (expanded)
                {
                    _window.Render(args);
                }
            }
            else if (args.Channel == WidgetChannel.Object)
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
                SolidBrush brush = new SolidBrush(Color.FromArgb(num, 90, 90, 90));
                SolidBrush brush2 = new SolidBrush(Color.FromArgb(num, 150, 150, 150));
                SolidBrush brush3 = new SolidBrush(Color.FromArgb(num, 0, 0, 0));
                SolidBrush brush4 = new SolidBrush(Color.FromArgb(num, 255, 255, 255));
                Pen pen = new Pen(brush3);
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                if (Empty)
                {
                    PointF point = new PointF(base.CanvasPivot.X + base.Width / 2f, base.CanvasBounds.Y + 2f);
                    graphics.DrawRectangle(pen, GH_Attr_Widget.Convert(base.CanvasBounds));
                    graphics.FillRectangle(brush2, base.CanvasBounds);
                    graphics.DrawString(_emptyText, WidgetServer.Instance.DropdownFont, brush, point, stringFormat);
                }
                else
                {
                    PointF point2 = new PointF(base.CanvasPivot.X + (base.Width - 10f) / 2f, base.CanvasBounds.Y + 2f);
                    graphics.DrawRectangle(pen, GH_Attr_Widget.Convert(base.CanvasBounds));
                    graphics.FillRectangle(brush4, base.CanvasBounds);
                    graphics.DrawString(_items[current_value].content, WidgetServer.Instance.DropdownFont, brush3, point2, stringFormat);
                    Rectangle rect = new Rectangle((int)(base.CanvasPivot.X + base.Width - 10f), (int)base.CanvasPivot.Y, 10, (int)base.Height);
                    graphics.FillRectangle(brush4, rect);
                    graphics.DrawRectangle(pen, rect);
                }
            }
        }

        public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (Empty)
            {
                return GH_ObjectResponse.Release;
            }
            if (expanded)
            {
                return _window.RespondToMouseUp(sender, e);
            }
            return GH_ObjectResponse.Ignore;
        }

        public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (expanded)
            {
                return _window.RespondToMouseMove(sender, e);
            }
            return GH_ObjectResponse.Ignore;
        }

        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (Empty)
            {
                return GH_ObjectResponse.Handled;
            }
            if (expanded)
            {
                if (_window.Contains(e.CanvasLocation))
                {
                    return _window.RespondToMouseDown(sender, e);
                }
                HideWindow(fire: false);
                return GH_ObjectResponse.Release;
            }
            ShowWindow();
            return GH_ObjectResponse.Capture;
        }

        public void ShowWindow()
        {
            if (!expanded)
            {
                expanded = true;
                TopCollection.ActiveWidget = this;
                Update();
            }
        }

        public void HideWindow(bool fire)
        {
            if (expanded)
            {
                expanded = false;
                TopCollection.ActiveWidget = null;
                TopCollection.MakeAllInActive();
                if (fire && this.ValueChanged != null)
                {
                    this.ValueChanged(this, new EventArgs());
                }
            }
        }

        public override bool Contains(PointF pt)
        {
            return base.CanvasBounds.Contains(pt);
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.CreateChunk("MenuDropDown", Index).SetInt32("ActiveItemIndex", current_value);
            return true;
        }

        public override bool Read(GH_IReader reader)
        {
            GH_IReader val = reader.FindChunk("MenuDropDown", Index);
            try
            {
                current_value = val.GetInt32("ActiveItemIndex");
            }
            catch
            {
                current_value = default_item_index;
            }
            return true;
        }

    }
}
