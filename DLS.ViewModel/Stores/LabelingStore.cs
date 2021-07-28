using DLS.Helper.Interfaces;
using DLS.Model.Enums;
using DLS.Model.Models;
using DLS.ViewModel.Configs;
using DLS.ViewModel.Helpers;
using DLS.ViewModel.Wrapper;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DLS.ViewModel.Stores
{
    public class LabelingStore : BindableBase
    {
        private AppConfig _appConfig;

        private int _index;
        private BitmapSource _frame;
        private System.Drawing.Rectangle _frameRect;

        private SettingStore _settingStore;
        
        private ObservableCollection<LabelingDataWrapper> _labelingDatas;
        public ObservableCollection<LabelingDataWrapper> LabelingDatas
        {
            get => _labelingDatas;
            private set => SetProperty(ref _labelingDatas, value);
        }

        public Dictionary<int, List<ILabelingData>> DataDicionary { get; }

        private LabelingDataWrapper _selected;
        public LabelingDataWrapper Selected
        {
            get => _selected;
            set
            {
                SetProperty(ref _selected, value);
                IsSelected = _selected != null;
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            private set => SetProperty(ref _isSelected, value);
        }

        private SelectDirection _selectDirection;
        public SelectDirection SelectDirection
        {
            get => _selectDirection;
            set => SetProperty(ref _selectDirection, value);
        }

        private System.Drawing.RectangleF _prevRect;
        private ICommandStack _commandStack;

        public LabelingStore(SettingStore settingStore, AppConfig appConfig, ICommandStack commandStack)
        {
            _settingStore = settingStore;
            _commandStack = commandStack;

            _appConfig = appConfig;

            DataDicionary = new Dictionary<int, List<ILabelingData>>();
        }

        private void AddCollection()
        {
            if (LabelingDatas == null)
            {
                LabelingDatas = new ObservableCollection<LabelingDataWrapper>();
                BindingOperations.EnableCollectionSynchronization(LabelingDatas, new object());
                DataDicionary[_index] = LabelingDatas.Select(d => d.Reference).ToList();
            }
        }

        public void AddPointData(Class @class, IEnumerable<Point> points)
        {
            AddCollection();

            var data = new PointLabelingData(@class, points.Select(p => new System.Drawing.PointF((float)p.X, (float)p.Y)));
            PushAddCommand(data);
            Add(_index, data);
        }

        public void AddRectData(Class @class, Rect rect)
        {
            if (rect.Width < _appConfig.MinSize || rect.Height < _appConfig.MinSize)
                return;

            AddCollection();

            var data = new RectLabelingData(@class, new System.Drawing.RectangleF((float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height));
            PushAddCommand(data);
            Add(_index, data);
        }

        public void EditDone(float xOffset, float yOffset)
        {
            PushEditCommand(xOffset, yOffset);

            Refresh();
        }

        private void Refresh()
        {
            if (_selected == null)
                return;

            var rect = _selected.Reference.GetBoundingRect();
            rect.Intersect(_frameRect);
            if (rect.Width == 0 || rect.Height == 0)
                return;

            _selected.Source = GetSource(rect);
        }

        private void PushEditCommand(float xOffset, float yOffset)
        {
            var command = new Command<(LabelingDataWrapper wrapper, SelectDirection direction, System.Drawing.RectangleF prev, System.Drawing.RectangleF next, float x, float y)>(
                (Selected, SelectDirection, _prevRect, Selected.Rect, xOffset, yOffset),
                (@class) =>
                {
                    @class.wrapper.Offset(@class.direction, @class.prev, @class.x, @class.y, _frame.PixelWidth, _frame.PixelHeight, _appConfig.MinSize);
                    Refresh();
                },
                (@class) =>
                {
                    @class.wrapper.Offset(@class.direction, @class.next, -@class.x, -@class.y, _frame.PixelWidth, _frame.PixelHeight, _appConfig.MinSize);
                    Refresh();
                });

            _commandStack.Push(command);
        }

        private void PushAddCommand(ILabelingData data)
        {
            var command = new Command<(int index, ILabelingData data)>(
               (_index, data),
               (@class) => Add(@class.index, @class.data),
               (@class) => Remove(@class.index, @class.data));

            _commandStack.Push(command);
        }

        private void PushRemoveCommand()
        {
            var command = new Command<(int index, ILabelingData data)>(
                (_index, _selected.Reference),
                (@class) => Remove(@class.index, @class.data),
                (@class) => Add(@class.index, @class.data));

            _commandStack.Push(command);
        }

        public void Add(int index, ILabelingData data)
        {
            DataDicionary[index].Add(data);
            AddWrapper(data);
        }

        private BitmapSource GetSource(System.Drawing.Rectangle rect)
        {
            var labelingData = new byte[rect.Width * rect.Height * (_frame.Format.BitsPerPixel / 8)];
            _frame.CopyPixels(new Int32Rect(rect.X, rect.Y, rect.Width, rect.Height), labelingData, rect.Width * (_frame.Format.BitsPerPixel / 8), 0);

            var source = BitmapSource.Create(
                       rect.Width, rect.Height, 96, 96,
                       _frame.Format, null,
                       labelingData, rect.Width * (_frame.Format.BitsPerPixel / 8));

            source.Freeze();

            return source;
        }

        private void AddWrapper(ILabelingData data)
        {
            var rect = data.GetBoundingRect();
            rect.Intersect(_frameRect);
            if (rect.Width == 0 || rect.Height == 0)
            {
                LabelingDatas.Add(new LabelingDataWrapper(null, data));
                return;
            }

            LabelingDatas.Add(new LabelingDataWrapper(GetSource(rect), data));
            return;
        }

        public void Remove(int index, ILabelingData data)
        {
            DataDicionary[index].Remove(data);

            if (LabelingDatas != null)
            {
                var founds = LabelingDatas.Where(d => d.Reference == data).ToList();

                foreach (var found in founds)
                    LabelingDatas.Remove(found);
            }
        }

        private IEnumerable<LabelingDataWrapper> GetFilteredData()
        {
            switch (_settingStore.LabelingMode)
            {
                case Model.Enums.LabelingMode.Rect:
                    return LabelingDatas.Where(d => d.LabelingType == LabelingType.Rect);
                case Model.Enums.LabelingMode.Polygon:
                case Model.Enums.LabelingMode.Pen:
                    return LabelingDatas.Where(d => d.LabelingType == LabelingType.Polygon);
            }

            return null;
        }

        public void SelectLabelingData(float x, float y)
        {
            if (LabelingDatas == null)
            {
                SelectDirection = SelectDirection.None;
                ClearSelection();
                return;
            }

            var datas = LabelingDatas.Where(d => CheckDirection(d, x, y) != SelectDirection.None);

            if (datas.Count() <= 0)
            {
                SelectDirection = SelectDirection.None;
                ClearSelection();
                return;
            }
            
            Selected = datas.First();
            SelectDirection = CheckDirection(Selected, x, y);

            _prevRect = Selected.Rect;
        }

        public SelectDirection GetDirection(float x, float y)
        {
            if (LabelingDatas == null)
                return SelectDirection.None;

            var datas = LabelingDatas.Where(d => CheckDirection(d, x, y) != SelectDirection.None);

            if (datas.Count() <= 0)
                return SelectDirection.None;
            
            var direction = CheckDirection(datas.First(), x, y);
            if (direction == SelectDirection.Move)
                return SelectDirection.None;

            return CheckDirection(datas.First(), x, y);
        }

        private SelectDirection CheckDirection(LabelingDataWrapper data, float x, float y)
        {
            switch (data.LabelingType)
            {
                case LabelingType.Rect:
                    return CheckRectDirection(data, x, y, _appConfig.Capacity);
                case LabelingType.Polygon:
                    break;
            }

            return SelectDirection.None;
        }

        public void ChangePosition(int index, BitmapSource frame)
        {
            _commandStack.Clear();

            _index = index;
            _frame = frame;
            _frameRect = new System.Drawing.Rectangle(0, 0, _frame.PixelWidth, _frame.PixelHeight);
            if (DataDicionary.ContainsKey(index))
            {
                var datas = DataDicionary[index];

                LabelingDatas = new ObservableCollection<LabelingDataWrapper>();
                BindingOperations.EnableCollectionSynchronization(LabelingDatas, new object());
                foreach (var data in datas)
                    AddWrapper(data);
            }
            else
            {
                LabelingDatas = null;
            }

            ClearSelection();
        }
        
        public void RemoveSelection()
        {
            if (_selected != null)
            {
                PushRemoveCommand();

                Remove(_index, _selected.Reference);
            }

            ClearSelection();
        }

        public void EditSelection(float xOffset, float yOffset)
        {
            Selected.Offset(SelectDirection, _prevRect, xOffset, yOffset, _frame.PixelWidth, _frame.PixelHeight, _appConfig.MinSize);
        }

        public void ClearSelection()
        {
            Selected = null;
            SelectDirection = SelectDirection.None;
        }

        private SelectDirection CheckRectDirection(LabelingDataWrapper data, float x, float y, double capacity)
        {
            var inflate = data.Rect;
            inflate.Inflate((float)capacity, (float)capacity);
            if (inflate.Contains(x, y) == false)
                return SelectDirection.None;
            
            var rect = data.Rect;

            if (Math.Abs(x - rect.X) < capacity && Math.Abs(y - rect.Y) < capacity)
                return SelectDirection.LeftTop;
            
            if (Math.Abs(x - rect.X) < capacity && Math.Abs(y - rect.Bottom) < capacity)
                return SelectDirection.LeftBottom;

            if (Math.Abs(x - rect.Right) < capacity && Math.Abs(y - rect.Y) < capacity)
                return SelectDirection.RightTop;

            if (Math.Abs(x - rect.Right) < capacity && Math.Abs(y - rect.Bottom) < capacity)
                return SelectDirection.RightBottom;
            
            if (Math.Abs(x - rect.X) < capacity)
                return SelectDirection.Left;

            if (Math.Abs(x - rect.Right) < capacity)
                return SelectDirection.Right;

            if (Math.Abs(y - rect.Y) < capacity)
                return SelectDirection.Top;

            if (Math.Abs(y - rect.Bottom) < capacity)
                return SelectDirection.Bottom;

            return SelectDirection.Move;
        }
    }
}