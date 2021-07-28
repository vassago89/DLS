using DLS.Model.Enums;
using DLS.Model.Models;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace DLS.ViewModel.Wrapper
{
    public class LabelingDataWrapper : BindableBase
    {
        public ILabelingData Reference { get; }
        public string Name => Reference.Name;
        public double ID => Reference.ID;

        private BitmapSource _source;
        public BitmapSource Source
        {
            get => _source;
            set => SetProperty(ref _source, value);
        }

        public LabelingType LabelingType => Reference.LabelingType;
        
        public RectangleF Rect
        {
            get
            {
                if (!(Reference is RectLabelingData data))
                    return RectangleF.Empty;

                return data.Rect;
            }
            private set
            {
                if (!(Reference is RectLabelingData data))
                    return;
                
                data.Update(value.X, value.Y, value.Width, value.Height);
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Rect"));
            }
        }

        public LabelingDataWrapper(BitmapSource source, ILabelingData labelingData)
        {
            Reference = labelingData;

            Source = source;
        }

        public bool IsContains(PointF pos)
        {
            return Reference.IsContains(pos);
        }

        public PointF GetCenterPoint()
        {
            return Reference.GetCenterPoint();
        }

        public void Offset(SelectDirection direction, RectangleF rect, float x, float y, int width, int height, int minSize)
        {
            RectangleF temp = RectangleF.Empty;
            switch (direction)
            {
                case SelectDirection.Move:
                    temp = new RectangleF(rect.X + x, rect.Y + y, rect.Width, rect.Height);
                    break;
                case SelectDirection.Left:
                    temp = new RectangleF(rect.X + x, rect.Y, rect.Width - x, rect.Height);
                    break;
                case SelectDirection.Right:
                    temp = new RectangleF(rect.X, rect.Y, rect.Width + x, rect.Height);
                    break;
                case SelectDirection.Top:
                    temp = new RectangleF(rect.X, rect.Y + y, rect.Width, rect.Height - y);
                    break;
                case SelectDirection.Bottom:
                    temp = new RectangleF(rect.X, rect.Y, rect.Width, rect.Height + y);
                    break;
                case SelectDirection.LeftTop:
                    temp = new RectangleF(rect.X + x, rect.Y + y, rect.Width - x, rect.Height - y);
                    break;
                case SelectDirection.RightTop:
                    temp = new RectangleF(rect.X, rect.Y + y, rect.Width + x, rect.Height - y);
                    break;
                case SelectDirection.LeftBottom:
                    temp = new RectangleF(rect.X + x, rect.Y, rect.Width - x, rect.Height + y);
                    break;
                case SelectDirection.RightBottom:
                    temp = new RectangleF(rect.X, rect.Y, rect.Width + x, rect.Height + y);
                    break;
            }

            if (direction == SelectDirection.Move)
            {
                if (temp.X < 0 || temp.Y < 0 || temp.Right >= width || temp.Bottom >= height)
                    return;
            }

            if (temp.Width < minSize || temp.Height < minSize)
                return;

            Rect = temp;
        }
    }
}
