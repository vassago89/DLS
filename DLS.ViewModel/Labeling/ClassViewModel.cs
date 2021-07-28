using DLS.ViewModel.Configs;
using DLS.ViewModel.Helpers;
using DLS.ViewModel.Stores;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DLS.ViewModel.Labeling
{
    public class ClassViewModel : BindableBase
    {
        //TODO New 조건
        //TODO User 권한

        public ClassStore ClassStore { get; }
        public DelegateCommand AddClassCommand { get; }

        private BitmapSource _selectedIcon;
        public BitmapSource SelectedIcon
        {
            get => _selectedIcon;
            set
            {
                SetProperty(ref _selectedIcon, value);
                IsEnabledAdd = ClassStore.Contains(_newID) == false && ClassStore.Contains(_newClassName) == false && SelectedIcon != null;
            }
        }

        private string _newClassName;
        public string NewClassName
        {
            get => _newClassName;
            set
            {
                SetProperty(ref _newClassName, value);
                IsEnabledAdd = ClassStore.Contains(_newID) == false && ClassStore.Contains(_newClassName) == false && SelectedIcon != null;
            }
        }

        private double _newID;
        public double NewID
        {
            get => _newID;
            set
            {
                SetProperty(ref _newID, value);
                IsEnabledAdd = ClassStore.Contains(_newID) == false && ClassStore.Contains(_newClassName) == false && SelectedIcon != null;
            }
        }

        private bool _isEnabledAdd;
        public bool IsEnabledAdd
        {
            get => _isEnabledAdd;
            set => SetProperty(ref _isEnabledAdd, value);
        }

        public LayoutConfig LayoutConfig { get; }

        public ClassViewModel(ClassStore classStore, LayoutConfig layoutConfig)
        {
            ClassStore = classStore;
            LayoutConfig = layoutConfig;

            AddClassCommand = new DelegateCommand(AddClass);
        }

        private void AddClass()
        {
            var (Width, Height, Data, Format) = SelectedIcon.GetImageData();
         
            if (ClassStore.AddClass(NewID, NewClassName, Width, Height, Data, Format))
            {
                NewClassName = string.Empty;
                SelectedIcon = null;
            }
        }

        public void ChangePosition(int prev, int next)
        {
            ClassStore.Refresh();
        }
    }
}
