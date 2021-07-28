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
    public class DataViewModel : BindableBase
    {
        public LabelingStore LabelingStore { get; }
        public LayoutConfig LayoutConfig { get; }

        public DataViewModel(LabelingStore labelingStore, LayoutConfig layoutConfig)
        {
            LabelingStore = labelingStore;
            LayoutConfig = layoutConfig;
        }
    }
}
