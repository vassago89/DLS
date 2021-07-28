using DLS.Helper.Interfaces;
using DLS.Model.Enums;
using DLS.Model.Models;
using DLS.ViewModel.Configs;
using DLS.ViewModel.Stores;
using DLS.ViewModel.Wrapper;
using DLS.WPF.Helper.Services;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DLS.ViewModel.Labeling
{
    public class ImageViewModel : BindableBase
    {
        public ImageStore ImageStore { get; }
        public SettingStore SettingStore { get; }
        public LabelingStore LabelingStore { get; }
        public ClassStore ClassStore { get; }

        public AppConfig AppConfig { get; }
        public ColorConfig ColorConfig { get; }
        public OpacityConfig OpacityConfig { get; }

        public DelegateCommand ZoomInCommand { get; }
        public DelegateCommand ZoomOutCommand { get; }
        public DelegateCommand ZoomFitCommand { get; }

        public DelegateCommand RedoCommand { get; }
        public DelegateCommand UndoCommand { get; }
        public DelegateCommand DeleteCommand { get; }

        private Point _initPos;

        private Rect _curRect;
        public Rect CurRect
        {
            get => _curRect;
            set => SetProperty(ref _curRect, value);
        }

        private List<Point> _curPoints;
        public List<Point> CurPoints
        {
            get => _curPoints;
            set => SetProperty(ref _curPoints, value);
        }

        private bool _isDrag;

        public ICommandStack CommandStack { get; }

        public ImageViewModel(
            ImageStore imageStore, 
            LabelingStore labelingStore, 
            ClassStore classStore, 
            SettingStore settingStore,
            ICommandStack commandStack,
            AppConfig appConfig,
            ColorConfig colorConfig,
            OpacityConfig opacityConfig)
        {
            CurPoints = new List<Point>();

            ImageStore = imageStore;
            LabelingStore = labelingStore;
            ClassStore = classStore;
            SettingStore = settingStore;

            CommandStack = commandStack;

            AppConfig = appConfig;
            ColorConfig = colorConfig;
            OpacityConfig = opacityConfig;

            ZoomInCommand = new DelegateCommand(() =>
            {
                ImageStore.ZoomIn();
            });

            ZoomOutCommand = new DelegateCommand(() =>
            {
                ImageStore.ZoomOut();
            });

            ZoomFitCommand = new DelegateCommand(() =>
            {
                ImageStore.ZoomFit();
            });

            DeleteCommand = new DelegateCommand(() =>
            {
                LabelingStore.RemoveSelection();
            });

            UndoCommand = new DelegateCommand(() =>
            {
                CommandStack.Undo();
            });

            RedoCommand = new DelegateCommand(() =>
            {
                CommandStack.Redo();
            });
        }

        public void MouseDown(Point pos)
        {
            if (ImageStore.IsOpened == false)
                return;

            _initPos = pos;
            _isDrag = true;

            LabelingStore.SelectLabelingData((float)pos.X, (float)pos.Y);

            if (LabelingStore.SelectDirection == SelectDirection.None)
                CreateLabelingData(pos);
        }

        public void CreateLabelingData(Point pos)
        {
            switch (SettingStore.LabelingMode)
            {
                case Model.Enums.LabelingMode.Rect:
                    CurRect = new Rect(pos.X, pos.Y, 1, 1);
                    break;
                case Model.Enums.LabelingMode.Polygon:
                case Model.Enums.LabelingMode.Pen:
                    var points = new List<Point>
                    {
                        new Point(pos.X, pos.Y),
                        new Point(pos.X, pos.Y)
                    };

                    CurPoints = points;
                    break;
            }
        }

        public void MouseUp(Point pos)
        {
            if (ImageStore.IsOpened == false)
                return;

            _isDrag = false;

            if (LabelingStore.IsSelected)
            {
                if (Math.Abs(pos.X - _initPos.X) > 0 || Math.Abs(pos.Y - _initPos.Y) > 0)
                    LabelingStore.EditDone((float)(pos.X - _initPos.X), (float)(pos.Y - _initPos.Y));
            }
            else
            {
                AddLabelingData();
            }
        }

        public void AddLabelingData()
        {
            if (ClassStore.SelectedClass == null || ImageStore.Source == null)
                return;

            switch (SettingStore.LabelingMode)
            {
                case Model.Enums.LabelingMode.Rect:
                    var newRect = CurRect;
                    newRect.Intersect(new Rect(0,0,ImageStore.Source.PixelWidth, ImageStore.Source.PixelHeight));
                    LabelingStore.AddRectData(ClassStore.SelectedClass, newRect);
                    CurRect = Rect.Empty;
                    break;
                case Model.Enums.LabelingMode.Polygon:
                case Model.Enums.LabelingMode.Pen:
                    break;
            }
        }

        public SelectDirection MouseMove(Point pos)
        {
            if (ImageStore.IsOpened == false)
                return SelectDirection.None;

            if (_isDrag == false)
                return LabelingStore.GetDirection((float)pos.X, (float)pos.Y);

            switch (SettingStore.LabelingMode)
            {
                case Model.Enums.LabelingMode.Rect:
                    if (LabelingStore.IsSelected)
                    {
                        LabelingStore.EditSelection((float)(pos.X - _initPos.X), (float)(pos.Y - _initPos.Y));
                    }
                    else
                    {
                        CurRect = new Rect(
                            Math.Min(_initPos.X, pos.X),
                            Math.Min(_initPos.Y, pos.Y),
                            Math.Abs(_initPos.X - pos.X),
                            Math.Abs(_initPos.Y - pos.Y));
                    }
                    break;
                case Model.Enums.LabelingMode.Polygon:
                    if (CurPoints.Count == 0)
                        break;
                    var points = new List<Point>(CurPoints);
                    points.RemoveAt(points.Count - 1);
                    points.Add(pos);
                    CurPoints = points;
                    break;
                case Model.Enums.LabelingMode.Pen:
                    break;
            }

            return LabelingStore.SelectDirection;
        }

        public void MouseWheel(Point pos, double delta)
        {
            if (ImageStore.IsOpened == false)
                return;

            if (delta > 0)
                ImageStore?.ZoomService.ExecuteZoom(pos.X, pos.Y, 1.1f);
            else
                ImageStore?.ZoomService.ExecuteZoom(pos.X, pos.Y, 0.9f);
        }

        public void MouseLeave()
        {
            //_isDrag = false;
            //LabelingStore.ClearSelection();
        }
    }
}