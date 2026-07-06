using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace FluxionEditor.Foundation.Utilities
{
    interface IUndoRedo
    {
        string Name { get; }

        void Execute();
        void Undo();
    }

    class UndoRedoCommand : IUndoRedo
    {
        private readonly Action _execute;
        private readonly Action _undo;
        public string Name { get; }
        public UndoRedoCommand(string name, Action execute, Action undo)
        {
            Debug.Assert(undo!= null && execute!=null);
            Name = name;
            _execute = execute;
            _undo = undo;
        }

        public UndoRedoCommand(string name)
        {
            Name = name;
        }

        public void Execute()
        {
            _execute();
        }
        public void Undo()
        {
            _undo();
        }
    } 

    internal class UndoRedo
    {
        private readonly ObservableCollection<IUndoRedo> _undoStack = new ObservableCollection<IUndoRedo>();
        private readonly ObservableCollection<IUndoRedo> _redoStack = new ObservableCollection<IUndoRedo>();
        public ReadOnlyObservableCollection<IUndoRedo> UndoStack { get; }
        public ReadOnlyObservableCollection<IUndoRedo> RedoStack { get; }

        public void Reset()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }

        public void Undo()
        {
            if (_undoStack.Count > 0)
            {
                var action = _undoStack[^1];
                action.Undo();
                _undoStack.RemoveAt(_undoStack.Count - 1);
                _redoStack.Add(action);
            }
        }

        public void Redo()
        {
            if (_redoStack.Count > 0)
            {
                var action = _redoStack[^1];
                action.Execute();
                _redoStack.RemoveAt(_redoStack.Count - 1);
                _undoStack.Add(action);
            }
        }

        public void Add(IUndoRedo cmd)
        {
            _undoStack.Add(cmd);
            _redoStack.Clear();
        }

        public UndoRedo()
        {
            UndoStack = new ReadOnlyObservableCollection<IUndoRedo>(_undoStack);
            RedoStack = new ReadOnlyObservableCollection<IUndoRedo>(_redoStack);
        }
    }
}
