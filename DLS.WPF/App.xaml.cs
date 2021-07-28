using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils.Themes;
using DLS.Helper.Interfaces;
using DLS.ViewModel;
using DLS.ViewModel.Configs;
using DLS.ViewModel.Helpers;
using DLS.ViewModel.Stores;
using DLS.WPF.Shell;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace DLS.WPF
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override void OnInitialized()
        {
            base.OnInitialized();

            CultureInfo culture = CultureInfo.CreateSpecificCulture("ko-kr");

            Thread.CurrentThread.CurrentUICulture = culture;

            Thread.CurrentThread.CurrentCulture = culture;

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            
            //var systemConfig = Container.Resolve<SystemConfig>();
            //var protocolService = Container.Resolve<ProtocolService>();

            //protocolService.SetLed(systemConfig.ValueLed * 1000.0);
            //protocolService.Set660(systemConfig.ValueDictionary[ELazer.L660] * 1000.0);
            //protocolService.Set760(systemConfig.ValueDictionary[ELazer.L760] * 1000.0);

            //protocolService.SetExposure(systemConfig.ExposureLed, ELazer.L660, true);
            //protocolService.SetExposure(systemConfig.ExposureDictionary[ELazer.L660], ELazer.L660);
            //protocolService.SetExposure(systemConfig.ExposureDictionary[ELazer.L760], ELazer.L760);

            //protocolService.SetGain(systemConfig.GainLed, ELazer.L660, true);
            //protocolService.SetGain(systemConfig.GainDictionary[ELazer.L660], ELazer.L660);
            //protocolService.SetGain(systemConfig.GainDictionary[ELazer.L760], ELazer.L760);
        }

        //private static void OnFocusChangedHandler(object src, AutomationFocusChangedEventArgs args)
        //{
        //    Console.WriteLine("Focus changed!");
        //    AutomationElement element = src as AutomationElement;
        //    if (element != null)
        //    {
        //        string name = element.Current.Name;
        //        string id = element.Current.AutomationId;
        //        int processId = element.Current.ProcessId;
        //        using (Process process = Process.GetProcessById(processId))
        //        {
        //            Console.WriteLine("  Name: {0}, Id: {1}, Process: {2}", name, id, process.ProcessName);
        //        }
        //    }
        //}

        protected override Window CreateShell()
        {
            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;

            return new ShellView();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

            var classConfig = ClassConfig.Deserialize(Path.Combine(Environment.CurrentDirectory, "ClassConfig.bin"));
            var appConfig = AppConfig.Deserialize(Path.Combine(Environment.CurrentDirectory, "AppConfig.bin"));
            var layoutConfig = LayoutConfig.Deserialize(Path.Combine(Environment.CurrentDirectory, "LayoutConfig.bin"));
            var colorConfig = ColorConfig.Deserialize(Path.Combine(Environment.CurrentDirectory, "ColorConfig.bin"));
            var opacityConfig = OpacityConfig.Deserialize(Path.Combine(Environment.CurrentDirectory, "ColorConfig.bin"));
            containerRegistry
                .RegisterSingleton<ImageStore>()
                .RegisterSingleton<ClassStore>()
                .RegisterSingleton<LabelingStore>()
                .RegisterSingleton<SettingStore>()
                .RegisterSingleton<ICommandStack, CommandStack>()
                .RegisterInstance(classConfig)
                .RegisterInstance(appConfig)
                .RegisterInstance(layoutConfig)
                .RegisterInstance(colorConfig)
                .RegisterInstance(opacityConfig);

            //containerRegistry
            //    .RegisterSingleton<GrabService>()
            //    .RegisterSingleton<ProcessService>()
            //    .RegisterSingleton<RecordService>()
            //    .RegisterSingleton<ProtocolService>()
            //    .RegisterInstance(SystemConfig.Load(Environment.CurrentDirectory));
            //throw new NotImplementedException();


        }

        protected override void OnExit(ExitEventArgs e)
        {
            Container.Resolve<AppConfig>().Serialize();
            Container.Resolve<ColorConfig>().Serialize();
            Container.Resolve<LayoutConfig>().Serialize();
            Container.Resolve<OpacityConfig>().Serialize();
            Container.Resolve<ClassConfig>().Serialize();
            //var protocolService = Container.Resolve<ProtocolService>();
            //protocolService.OffLed();
            //protocolService.Off660();
            //protocolService.Off760();

            //MatroxObjectPool.Dispose();
            //MatroxApplicationHelper.Dispose();

            //if (e.ApplicationExitCode == 0)
            //{

            //    //MatroxHelper.FreeApplication();
            //    //try
            //    //{
            //    //    c
            //    //    Container.Resolve<IoService>().Stop();
            //    //    Container.Resolve<IoService>().Cancle();
            //    //}
            //    //catch
            //    //{
            //    //    var pm = new PatternMatching();
            //    //    pm.AddPattern
            //    //}
            //}

            ////MatroxHelper.FreeApplication();
            ////CudaMethods.CUDA_RELEASE();

            base.OnExit(e);
        }

        private void PrismApplication_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.Exception.Message);
            e.Handled = true;
            return;
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
            {
                var name = $"{viewType.FullName.Replace("View", "ViewModel").Replace("WPF", "ViewModel")}, {Assembly.LoadFrom("DLS.ViewModel.dll").FullName}";
                var type = Type.GetType(name);
                return type;
            });
        }
    }
}
