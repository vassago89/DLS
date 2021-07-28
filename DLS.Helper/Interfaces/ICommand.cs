using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLS.Helper.Interfaces
{
    public interface ICommand
    {
        void Redo();
        void Undo();
    }

    public class Command : ICommand
    {
        Action _undo;
        Action _redo;

        public Command(Action undo, Action redo)
        {
            _redo = redo;
            _undo = undo;
        }

        public void Redo()
        {
            _redo();
        }

        public void Undo()
        {
            _undo();
        }
    }

    public class Command<T> : ICommand
    {
        T _class;
        Action<T> _undo;
        Action<T> _redo;

        public Command(T @class, Action<T> redo, Action<T> undo)
        {
            _class = @class;

            _redo = redo;
            _undo = undo;
        }

        public void Redo()
        {
            _redo(_class);
        }

        public void Undo()
        {
            _undo(_class);
        }
    }
}
