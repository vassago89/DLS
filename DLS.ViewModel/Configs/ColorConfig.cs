using DLS.Helper.Abstracts;
using DLS.Model.Enums;
using DLS.Model.Models;
using DLS.WPF.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace DLS.ViewModel.Configs
{
    [Serializable]
    public class ColorConfig : BinarySerializable<ColorConfig>
    {
        public Dictionary<string, BindableValue<string>> ColorDictionary { get; private set; }
        public BindableValue<string> this[string key] =>    ColorDictionary.ContainsKey(key) ? ColorDictionary[key] : null;

        public ColorConfig()
        {
            ColorDictionary = new Dictionary<string, BindableValue<string>>
            {
                { "Select", new BindableValue<string>("Red") },
                { "Common", new BindableValue<string>("Yellow") }
            };
        }
    }
}
