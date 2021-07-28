using DLS.Model.Models;
using DLS.ViewModel.Configs;
using DLS.ViewModel.Wrapper;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace DLS.ViewModel.Stores
{
    public class ClassStore : BindableBase
    {
        public Class SelectedClass
        {
            get
            {
                if (Classes.All(c => c.IsSelected == false))
                    return null;

                return Classes.First(c => c.IsSelected).Reference;
            }
        }

        public ClassWrapperCollection Classes { get; private set; }

        private ClassConfig _classConfig;

        public ClassStore(ClassConfig classConfig)
        {
            _classConfig = classConfig;

            Classes = new ClassWrapperCollection();

            BindingOperations.EnableCollectionSynchronization(Classes, new object());

            foreach (var @class in _classConfig.Classes)
            {
                Classes.Add(new ClassWrapper(@class, Classes));
            }

            if (Classes.Count > 0)
                Classes.First().IsSelected = true;
        }

        public bool AddClass(double id, string name, int width, int height, byte[] data, PixelFormat pixelFormat)
        {
            if (_classConfig.Classes.Any(c => c.Name == name))
                return false;

            var @class = new Class(id, name, new ImageData(width, height, data, pixelFormat.ToString()));
            _classConfig.Classes.Add(@class);
            _classConfig.Serialize();

            Classes.Add(new ClassWrapper(@class, Classes));

            return true;
        }

        public void RemoveClass(ClassWrapper classWrapper)
        {
            _classConfig.Classes.Remove(classWrapper.Reference);
            _classConfig.Serialize();

            Classes.Remove(classWrapper);
        }

        public void Refresh()
        {
            _classConfig.Classes.Clear();
            _classConfig.Classes.AddRange(Classes.Select(c => c.Reference));
            _classConfig.Serialize();
        }

        public bool Contains(string name)
        {
            if (string.IsNullOrEmpty(name))
                return true;

            return _classConfig.Classes.Any(c => c.Name == name);
        }

        public bool Contains(double id)
        {
            return _classConfig.Classes.Any(c => c.ID == id);
        }
    }

}