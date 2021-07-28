using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DLS.WPF.Helpers
{
    public class BindableValue<T> : BindableBase
    {
        private T _value;
        public T Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }

        public BindableValue()
        {

        }

        public BindableValue(T value)
        {
            _value = value;
        }
    }
}
