using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DLS.ViewModel.Helpers
{
    public static class BitmapSourceExtension
    {
        public static (int Width, int Height, byte[] Data, PixelFormat Format) GetImageData(this BitmapSource source)
        {
            int channel = source.Format.BitsPerPixel / 8;

            var buffer = new byte[source.PixelWidth * source.PixelHeight * channel];
            source.CopyPixels(buffer, source.PixelWidth * channel, 0);

            return (source.PixelWidth, source.PixelHeight, buffer, source.Format);
        }
    }
}
