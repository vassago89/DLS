using DLS.Helper.Abstracts;
using DLS.Model.Enums;
using DLS.Model.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Xml.Serialization;
using YamlDotNet.Serialization;

namespace DLS.ViewModel.Configs
{
    //TODO Yaml => binary로 변경
    [Serializable]
    public class AppConfig : BinarySerializable<AppConfig>
    {
        [YamlIgnore]
        public bool SettingMode { get; }
        
        public int SkipFrame { get; set; }

        public bool IsFillMode { get; set; }
        public bool IsSelectedFillMode { get; set; }

        public int MinSize { get; set; }
        public int Capacity { get; set; }

        public AppConfig()
        {
            SkipFrame = 1;
            MinSize = 50;
            Capacity = 15;
        }
    }
}
