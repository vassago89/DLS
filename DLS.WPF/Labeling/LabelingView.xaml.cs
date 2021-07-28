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
    public partial class LabelingView : UserControl
    {
        public LabelingViewModel ViewModel => DataContext as LabelingViewModel;

        public LabelingView()
        {
            InitializeComponent();
        }

        private void Labeling_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.IsKeyDown(Key.Right) || e.KeyboardDevice.IsKeyDown(Key.Space))
                ViewModel?.ReadFrame();
            else if (e.KeyboardDevice.IsKeyDown(Key.Left) || e.KeyboardDevice.IsKeyDown(Key.Back))
                ViewModel?.PrevFrame();
            else if (e.KeyboardDevice.IsKeyDown(Key.Delete))
                ViewModel?.RemoveSelection();
            else if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl))
            {
                if (e.KeyboardDevice.IsKeyDown(Key.Z))
                    ViewModel?.Undo();
                else if (e.KeyboardDevice.IsKeyDown(Key.R))
                    ViewModel?.Redo();
            }
            //else if (e)
        }
        
        private void UserControl_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ViewModel.IsFocused = true;
        }

        private void UserControl_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ViewModel.IsFocused = false;
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
                Keyboard.Focus(this);
            else
                Keyboard.ClearFocus();
        }
    }
}
