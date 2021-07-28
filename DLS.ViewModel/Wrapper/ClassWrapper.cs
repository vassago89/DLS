using DLS.Model.Models;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DLS.ViewModel.Wrapper
{
    public class ClassWrapperCollection : ObservableCollection<ClassWrapper>
    {
        public void Refresh()
        {
            foreach (var @class in this)
                @class.IsSelected = false;
        }
    }

    public class ClassWrapper : BindableBase
    {
        private static PixelFormatConverter _converter = new PixelFormatConverter();

        public Class Reference { get; }
        public BitmapSource Source { get; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (value)
                    _classCollection.Refresh();

                SetProperty(ref _isSelected, value);
            }
        }

        public string Name => Reference.Name;
        ClassWrapperCollection _classCollection;

        public ClassWrapper(Class @class, ClassWrapperCollection classCollection)
        {
            Reference = @class;
            _classCollection = classCollection;

            var pixelFormat = (PixelFormat)_converter.ConvertFromString(Reference.ImageData.Format);

            Source = BitmapSource.Create(
                Reference.ImageData.Width, Reference.ImageData.Height, 96, 96,
                pixelFormat, null,
                Reference.ImageData.Data, Reference.ImageData.Width * pixelFormat.BitsPerPixel / 8);

            Source.Freeze();
        }
    }
}
