using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLS.Model.Models
{
    [Serializable]
    public class ImageData
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public byte[] Data { get; private set; }
        public string Format { get; private set; }

        public ImageData(int width, int height, byte[] data, string format)
        {
            Width = width;
            Height = height;
            Data = data;
            Format = format;
        }
    }
}
