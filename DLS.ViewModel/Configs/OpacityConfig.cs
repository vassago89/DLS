using DLS.Helper.Abstracts;
using DLS.WPF.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLS.ViewModel.Configs
{
    [Serializable]
    public class OpacityConfig : BinarySerializable<LayoutConfig>
    {
        public Dictionary<string, BindableValue<double>> OpacityDictionary { get; private set; }

        public BindableValue<double> this[string key] => OpacityDictionary.ContainsKey(key) ? OpacityDictionary[key] : null;


        public OpacityConfig()
        {
            OpacityDictionary = new Dictionary<string, BindableValue<double>>
            {
                { "Select", new BindableValue<double>(0.25) },
                { "Common", new BindableValue<double>(0.25) }
            };
        }
    }
}