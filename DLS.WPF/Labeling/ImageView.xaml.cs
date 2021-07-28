using DLS.Model.Enums;
using DLS.ViewModel.Labeling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DLS.WPF.Labeling
{
    /// <summary>
    /// DrawingView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ImageView : UserControl
    {
        public ImageViewModel ViewModel => DataContext as ImageViewModel;

        public ImageView()
        {
            InitializeComponent();

            ViewModel.ImageStore.FrameworkElement = Canvas;
        }
        
        private void Border_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(Canvas);
            ViewModel?.MouseDown(pos);
        }

        private void Border_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var pos = e.GetPosition(sender as IInputElement);
            ViewModel?.MouseWheel(pos, e.Delta);
        }

        private void Border_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(Canvas);
            var direction = ViewModel.MouseMove(pos);

            switch (direction)
            {
                case SelectDirection.None:
                    Cursor = Cursors.Arrow;
                    break;
                case SelectDirection.Move:
                    Cursor = Cursors.Hand;
                    break;
                case SelectDirection.Left:
                case SelectDirection.Right:
                    Cursor = Cursors.SizeWE;
                    break;
                case SelectDirection.Top:
                case SelectDirection.Bottom:
                    Cursor = Cursors.SizeNS;
                    break;
                case SelectDirection.LeftTop:
                case SelectDirection.RightBottom:
                    Cursor = Cursors.SizeNWSE;
                    break;
                case SelectDirection.RightTop:
                case SelectDirection.LeftBottom:
                    Cursor = Cursors.SizeNESW;
                    break;
                case SelectDirection.Select:
                    Cursor = Cursors.Hand;
                    break;
            }
        }

        private void Border_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(Canvas);
            ViewModel?.MouseUp(pos);
        }
        
        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            ViewModel?.MouseLeave();
        }
    }
}
