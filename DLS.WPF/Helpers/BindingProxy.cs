using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DLS.WPF.Helpers
{
    public class BindingProxy : Freezable
    {
        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }

        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new UIPropertyMetadata(null));
    }
    public interface ICommand
    {
        void Redo();
        void Undo();
    }

    public class UndoRedo
    {
        private Stack<ICommand> _Undocommands = new Stack<ICommand>();
        private Stack<ICommand> _Redocommands = new Stack<ICommand>();

        public void Redo(int levels)
        {
            for (int i = 1; i <= levels; i++)
            {
                if (_Redocommands.Count != 0)
                {
                    ICommand command = _Redocommands.Pop();
                    command.Redo();
                    _Undocommands.Push(command);
                }

            }
        }

        public void Undo(int levels)
        {
            for (int i = 1; i <= levels; i++)
            {
                if (_Undocommands.Count != 0)
                {
                    ICommand command = _Undocommands.Pop();
                    command.Undo();
                    _Redocommands.Push(command);
                }
            }
        }
    }
}
