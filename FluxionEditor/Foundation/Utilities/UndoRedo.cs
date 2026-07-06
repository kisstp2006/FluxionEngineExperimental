using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace FluxionEditor.Foundation.Utilities
{
    /// <summary>
    /// Represents a single undoable action with a description.
    /// </summary>
    public interface IUndoRedo
    {
        /// <summary>Human-readable description shown in undo/redo lists.</summary>
        string Name { get; }

        /// <summary>Re-applies the action (used for Redo).</summary>
        void Execute();

        /// <summary>Reverses the action (used for Undo).</summary>
        void Undo();
    }

    /// <summary>
    /// Concrete undoable action that stores a pair of <see cref="Action"/> delegates.
    /// </summary>
    public class UndoRedoCommand : IUndoRedo
    {
        private readonly Action _execute;
        private readonly Action _undo;

        public string Name { get; }

        /// <summary>
        /// Creates a named undo/redo action.
        /// </summary>
        /// <param name="name">Display name.</param>
        /// <param name="execute">Action to perform on Redo.</param>
        /// <param name="undo">Action to perform on Undo.</param>
        public UndoRedoCommand(string name, Action execute, Action undo)
        {
            Debug.Assert(undo != null && execute != null);
            Name = name;
            _execute = execute;
            _undo = undo;
        }

        /// <summary>
        /// Creates a named action without delegates (for placeholder use only).
        /// </summary>
        public UndoRedoCommand(string name)
        {
            Name = name;
        }


        public UndoRedoCommand(string name, string property, object instance, object undoValue, object redoValue)
            : this(
                name,
                () => instance.GetType().GetProperty(property)?.SetValue(instance, undoValue),
                () => instance.GetType().GetProperty(property)?.SetValue(instance, redoValue))
        {
        }

        public void Execute() => _execute();
        public void Undo() => _undo();
    }

    /// <summary>
    /// Manages undo and redo stacks. Adding a new action clears the redo stack.
    /// </summary>
    public class UndoRedo
    {
        private bool _enableAdd = true;
        private readonly ObservableCollection<IUndoRedo> _undoStack = new ObservableCollection<IUndoRedo>();
        private readonly ObservableCollection<IUndoRedo> _redoStack = new ObservableCollection<IUndoRedo>();

        /// <summary>Read-only view of the undo stack for UI binding.</summary>
        public ReadOnlyObservableCollection<IUndoRedo> UndoStack { get; }

        /// <summary>Read-only view of the redo stack for UI binding.</summary>
        public ReadOnlyObservableCollection<IUndoRedo> RedoStack { get; }

        public UndoRedo()
        {
            UndoStack = new ReadOnlyObservableCollection<IUndoRedo>(_undoStack);
            RedoStack = new ReadOnlyObservableCollection<IUndoRedo>(_redoStack);
        }

        /// <summary>Clears both undo and redo history.</summary>
        public void Reset()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }

        /// <summary>
        /// Pushes a new action onto the undo stack and clears the redo stack.
        /// </summary>
        public void Add(IUndoRedo cmd)
        {
            if (_enableAdd)
            {
                _undoStack.Add(cmd);
                _redoStack.Clear();
            }
        }

        /// <summary>
        /// Undoes the most recent action and moves it to the redo stack.
        /// </summary>
        public void Undo()
        {
            if (_undoStack.Count == 0)
                return;

            var action = _undoStack[^1];
            _enableAdd = false;
            action.Undo();
            _enableAdd = true;
            _undoStack.RemoveAt(_undoStack.Count - 1);
            _redoStack.Add(action);
        }

        /// <summary>
        /// Redoes the most recently undone action and moves it back to the undo stack.
        /// </summary>
        public void Redo()
        {
            if (_redoStack.Count == 0)
                return;

            var action = _redoStack[^1];
            _enableAdd = false;
            action.Execute();
            _enableAdd = true;
            _redoStack.RemoveAt(_redoStack.Count - 1);
            _undoStack.Add(action);
        }
    }
}
