using DLS.Helper.Interfaces;
using DLS.Model.Enums;
using DLS.ViewModel.Configs;
using DLS.ViewModel.Stores;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLS.ViewModel.Labeling
{
    public class LabelingViewModel : BindableBase
    {
        public LayoutConfig LayoutConfig { get; }
        public AppConfig AppConfig { get; }

        private ImageStore _imageStore;
        private readonly SettingStore _settingStore;
        private LabelingStore _labelingStore;

        private bool _isFocused;
        public bool IsFocused
        {
            get => _isFocused;
            set => SetProperty(ref _isFocused, value);
        }

        private ICommandStack _commandStack;

        public LabelingViewModel(
            ImageStore imageStore, 
            LabelingStore labelingStore, 
            SettingStore settingStore, 
            ICommandStack commandStack,
            AppConfig appConfig,
            LayoutConfig layoutConfig)
        {
            _imageStore = imageStore;
            _settingStore = settingStore;
            _labelingStore = labelingStore;
            _commandStack = commandStack;

            LayoutConfig = layoutConfig;
            AppConfig = appConfig;
        }

        public void ReadFrame()
        {
            _imageStore.ReadFrame(AppConfig.SkipFrame);
        }

        public void PrevFrame()
        {
            _imageStore.ReadFrame(-1);
        }

        public void RemoveSelection()
        {
            _labelingStore.RemoveSelection();
        }

        public void Undo()
        {
            _commandStack.Undo();
        }

        public void Redo()
        {
            _commandStack.Redo();
        }
    }
}
