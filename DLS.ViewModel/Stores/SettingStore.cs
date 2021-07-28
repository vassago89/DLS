using DLS.Model.Enums;
using DLS.ViewModel.Configs;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLS.ViewModel.Stores
{
    public class SettingStore : BindableBase
    {
        private LabelingMode _labelingMode;
        public LabelingMode LabelingMode
        {
            get => _labelingMode;
            set => SetProperty(ref _labelingMode, value);
        }
        
        private bool _isLabelingEnabled;
        public bool IsLabelingEnabled
        {
            get => _isLabelingEnabled;
            set => SetProperty(ref _isLabelingEnabled, value);
        }

        private bool _isRectMode;
        public bool IsRectMode
        {
            get => _isRectMode;
            set
            {
                SetProperty(ref _isRectMode, value);
                if (value)
                    LabelingMode = LabelingMode.Rect;
            }
        }

        private bool _isPolygonMode;
        public bool IsPolygonMode
        {
            get => _isPolygonMode;
            set
            {
                SetProperty(ref _isPolygonMode, value);
                if (value)
                    LabelingMode = LabelingMode.Polygon;
            }
        }

        private bool _isPenMode;
        public bool IsPenMode
        {
            get => _isPenMode;
            set
            {
                SetProperty(ref _isPenMode, value);
                if (value)
                    LabelingMode = LabelingMode.Pen;
            }
        }
        
        public SettingStore()
        {
            IsRectMode = true;
        }
    }
}
