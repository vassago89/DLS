using DLS.Helper.Interfaces;
using DLS.Helper.Patterns;
using DLS.Model.Enums;
using DLS.Model.Services;
using DLS.ViewModel.Configs;
using DLS.WPF.Helper.Services;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DLS.ViewModel.Stores
{
    public class ImageStore : BindableBase
    {
        private BitmapSource _source;
        public BitmapSource Source
        {
            get => _source;
            private set => SetProperty(ref _source, value);
        }

        private int _totalFrame;
        public int TotalFrame
        {
            get => _totalFrame;
            private set => SetProperty(ref _totalFrame, value);
        }

        private int _frame;
        public int Frame
        {
            get => _frame;
            private set => SetProperty(ref _frame, value);
        }

        private bool _isOpened;
        public bool IsOpened
        {
            get => _isOpened;
            private set => SetProperty(ref _isOpened, value);
        }

        private string _filePath;
        public string FilePath
        {
            get => _filePath;
            private set => SetProperty(ref _filePath, value);
        }

        private bool _isEnd;
        public bool IsEnd
        {
            get => _isEnd;
            set => SetProperty(ref _isEnd, value);
        }
        
        private AppConfig _appConfig;
        private FrameService _frameService;
        private LabelingStore _labelingStore;
        
        public ZoomService ZoomService { get; }
        public FrameworkElement FrameworkElement { get; set; }

        private ICommandStack _commandStack;

        private PipeLine<int> _grabPipiLine;
        private PipeLine<(int Frame, int Width, int Height, int Channel, byte[] Data)> _updatePipiLine;

        public ImageStore(
            FrameService frameService, 
            LabelingStore labelingStore, 
            ICommandStack commandStack,
            AppConfig appConfig)
        {
            _frameService = frameService;
            _labelingStore = labelingStore;

            _commandStack = commandStack;

            _appConfig = appConfig;

            ZoomService = new ZoomService();

            _grabPipiLine = new SinglePipeLine<int>(
                skipCount => ReadFrameJob(skipCount), 2);
            _updatePipiLine = new SinglePipeLine<(int Frame, int Width, int Height, int Channel, byte[] Data)>(
                result => Update(result.Frame, result.Width, result.Height, result.Channel, result.Data));

            _grabPipiLine.Run(new System.Threading.CancellationToken());
            _updatePipiLine.Run(new System.Threading.CancellationToken());
        }

        public void Initialize(string filePath)
        {
            Release();
            FilePath = filePath;
            _frameService.Open(filePath, false);
            TotalFrame = _frameService.Count;
            IsOpened = true;
        }

        public void Release()
        {
            TotalFrame = 0;
            Frame = 0;

            Source = null;
            IsOpened = false;
            _commandStack.Clear();
        }

        private void Update(int frame, int width, int height, int channel, byte[] data)
        {
            var fitRequired = 
                Source == null || 
                Source.PixelWidth != width || 
                Source.PixelHeight != height;

            BitmapSource temp = null;
            switch (channel)
            {
                case 1:
                    temp = BitmapSource.Create(
                        width, height, 96, 96,
                        PixelFormats.Gray8, null,
                        data, width * channel);
                    break;
                case 3:
                    temp = BitmapSource.Create(
                        width, height, 96, 96,
                        PixelFormats.Bgr24, null,
                        data, width * channel);
                    break;
            }
            temp.Freeze();
            Source = temp;

            Frame = frame;

            IsEnd = Frame == TotalFrame;

            _labelingStore.ChangePosition(Math.Min(Math.Max(0, Frame - 1), TotalFrame - 1), Source);

            if (fitRequired)
                FrameworkElement.Dispatcher.Invoke(ZoomFit);
        }

        public void ReadFrame(int skipCount = 1)
        {
            _grabPipiLine.Enqueue(skipCount);
            //if ((IsEnd && skipCount >= 1)
            //    || (Frame == 1 && skipCount <= -1))
            //    return;

            //var data = _frameService.Read(skipCount);

            //_updatePipiLine.Enqueue((_frameService.Position, _frameService.Width, _frameService.Height, 3, data));
        }

        private void ReadFrameJob(int skipCount = 1)
        {
            if ((IsEnd && skipCount >= 1)
                || (Frame == 1 && skipCount <= -1))
                return;

            var data = _frameService.Read(skipCount);

            _updatePipiLine.Enqueue((_frameService.Position, _frameService.Width, _frameService.Height, 3, data));
        }

        public void ZoomIn()
        {
            if (Source == null || FrameworkElement == null)
                return;

            ZoomService.ZoomIn(FrameworkElement.ActualWidth, FrameworkElement.ActualHeight);
        }

        public void ZoomOut()
        {
            if (Source == null || FrameworkElement == null)
                return;

            ZoomService.ZoomOut(FrameworkElement.ActualWidth, FrameworkElement.ActualHeight);
        }

        public void ZoomFit()
        {
            if (Source == null || FrameworkElement == null)
                return;

            ZoomService.ZoomFit(FrameworkElement.ActualWidth, FrameworkElement.ActualHeight, Source.PixelWidth, Source.PixelHeight);
        }
    }
}
