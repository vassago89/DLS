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
    public class LayoutConfig : BinarySerializable<LayoutConfig>
    {
        public Dictionary<string, BindableValue<double>> LayoutDictionary { get; private set; }

        public BindableValue<double> this[string key] => LayoutDictionary.ContainsKey(key) ? LayoutDictionary[key] : null;
        

        public LayoutConfig()
        {
            LayoutDictionary = new Dictionary<string, BindableValue<double>>
            {
                { "ClassLayout", new BindableValue<double>(100) },
                { "ClassImageLength", new BindableValue<double>(75) },
                { "ClassFontSize", new BindableValue<double>(8) },

                { "DataLayout", new BindableValue<double>(100) },
                { "DataImageLength", new BindableValue<double>(75) },
                { "DataFontSize", new BindableValue<double>(8) }
            };
        }
    }
}
