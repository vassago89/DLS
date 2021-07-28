using DLS.Helper.Patterns;
using DLS.Model.Services;
using DLS.ViewModel.Configs;
using DLS.ViewModel.Stores;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DLS.ViewModel.Shell
{
    public class RibbonViewModel : BindableBase
    {
        public DelegateCommand NewDatabaseCommand { get; }
        public DelegateCommand NextFrameCommand { get; }
        public DelegateCommand PrevFrameCommand { get; }
        public ImageStore ImageStore { get; }
        public AppConfig AppConfig { get; }
        public SettingStore SettingStore { get; }

        public RibbonViewModel(ImageStore imageStore, SettingStore settingStore, AppConfig appConfig)
        {
            ImageStore = imageStore;
            AppConfig = appConfig;
            SettingStore = settingStore;

            NewDatabaseCommand = new DelegateCommand(() =>
            {
                try
                {
                    var dialog = new OpenFileDialog()
                    {
                        Filter = "Vidio File (*.avi, *mp4)|*.avi;*mp4|Image Files (*.bmp, *jpg, *png)|*.bmp;*.jpg;*.png",
                        Multiselect = true
                    };

                    var result = dialog.ShowDialog();

                    if (result.HasValue && result.Value)
                    {
                        var extension = Path.GetExtension(dialog.FileNames.First());
                        if (extension == ".avi" || extension == ".mp4")
                        {
                            ImageStore.Initialize(dialog.FileNames.First());
                            ImageStore.ReadFrame();
                        }
                        else
                        {

                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            });

            NextFrameCommand = new DelegateCommand(() =>
            {
                ImageStore.ReadFrame();
            });

            PrevFrameCommand = new DelegateCommand(() =>
            {
                ImageStore.ReadFrame(-1);
            });
        }
    }
}
