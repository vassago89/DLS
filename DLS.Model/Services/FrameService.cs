using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using dlsmanaged;

namespace DLS.Model.Services
{
    public class FrameService
    {
        private ImageManager _imageManager;

        public int Width => _imageManager.GetWidth();
        public int Height => _imageManager.GetHeight();
        public int Count => _imageManager.GetCount();
        public int Position => _imageManager.GetPos();
        public bool IsDone => _imageManager.IsDone();

        public FrameService()
        {
            _imageManager = new ImageManager();
        }

        public void Open(string filePath, bool isGpu)
        {
            _imageManager.Open(filePath, isGpu);
        }

        public byte[] Read(int skipCount = 1)
        {
            return _imageManager.Read(skipCount);
        }

        public void Jump(int index)
        {
            _imageManager.Jump(index);
        }
    }
}
