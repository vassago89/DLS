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
    public partial class ClassView : UserControl
    {
        public ClassViewModel ViewModel => this.DataContext as ClassViewModel;

        public ClassView()
        {
            InitializeComponent();
        }

        private void ItemsSizeChanged(object sender, DevExpress.Xpf.Core.ValueChangedEventArgs<Size> e)
        {
            //ViewModel?.SizeChange()
        }

        private void ItemPositionChanged(object sender, DevExpress.Xpf.Core.ValueChangedEventArgs<int> e)
        {
            ViewModel.ChangePosition(e.OldValue, e.NewValue);
        }

        private void Item_MouseEnter(object sender, MouseEventArgs e)
        {
            var panel = sender as Panel;

            foreach (var child in panel.Children)
            {
                var fe = (child as FrameworkElement);
                if (fe == null)
                    continue;

                if (fe.Name == "MouseOverElement")
                    fe.Visibility = Visibility.Visible;
            }
        }

        private void Item_MouseLeave(object sender, MouseEventArgs e)
        {
            var panel = sender as Panel;

            foreach (var child in panel.Children)
            {
                var fe = (child as FrameworkElement);
                if (fe == null)
                    continue;

                if (fe.Name == "MouseOverElement")
                    fe.Visibility = Visibility.Collapsed;
            }
        }

        private void Grid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Item_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var fe = sender as FrameworkElement;
            var @class = fe.DataContext as ViewModel.Wrapper.ClassWrapper;
            @class.IsSelected = true;
        }
    }
}
