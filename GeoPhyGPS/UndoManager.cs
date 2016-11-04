using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace GPSDataWithList
{

    internal class UndoManager
    {
        private Stack<UndoAction> undoStack;

        private Stack<UndoAction> redoStack;

        private DataGridView grid;

        private IList dataSource;

        private object previousCellValue;

        /// <summary>

        /// Initializes a new instance of the <see cref="DataGridViewUndoManager"/> class to track changes to 

        /// the specified DataGridView.  Provide methods to undo and redo changes.

        /// </summary>

        /// <param name="grid">The grid to track changes for.</param>

        public UndoManager(DataGridView grid)

        {

            // Create undo and redo stacks

            undoStack = new Stack<UndoAction>();

            redoStack = new Stack<UndoAction>();

            this.grid = grid;



            // Get a reference to the list that the grid is bound to.

            dataSource = (IList)grid.DataSource;



            // Attach event handlers to the important grid events, to track changes.


            grid.RowsRemoved += new DataGridViewRowsRemovedEventHandler(grid_RowsRemoved);

            grid.RowsAdded += new DataGridViewRowsAddedEventHandler(grid_RowsAdded);

            grid.CellBeginEdit += new DataGridViewCellCancelEventHandler(grid_CellBeginEdit);

            grid.CellEndEdit += new DataGridViewCellEventHandler(grid_CellEndEdit);
        }

            /// <summary>

            /// Clears the undo and redo stacks.

            /// </summary>

        public void Clear()

        {

            undoStack.Clear();

            redoStack.Clear();

        }



        /// <summary>

        /// Push the undo action onto the stack.

        /// </summary>

        /// <param name="action">The action.</param>

        private void Push(UndoAction action)

        {

            undoStack.Push(action);

        }



        /// <summary>

        /// Undo the previous change.

        /// </summary>

        public void Undo()

        {

            if (undoStack.Count == 0)

                throw new InvalidOperationException("Undo stack is empty.");



            // Get the last change from the undo stack.

            UndoAction action = undoStack.Pop();



            // Add the change to the redo stack.

            redoStack.Push(action);



            switch (action.Change)

            {



                case UndoAction.ChangeMode.AddRow:

                    this.RemoveRow(action);

                    break;



                case UndoAction.ChangeMode.DeleteRow:

                    this.InsertRow(action);

                    break;



                case UndoAction.ChangeMode.ModifyCell:

                    this.UpdateCell(action);

                    break;



                default:

                    throw new InvalidOperationException("Unknown undo action change: " + action.Change);

            }



        }



        /// <summary>

        /// Redo the previous undo change.

        /// </summary>

        public void Redo()

        {

            if (redoStack.Count == 0)

                throw new InvalidOperationException("Redo stack is empty.");



            // Get the last change from the undo stack.

            UndoAction action = redoStack.Pop();



            // Add the change to the undo stack.

            undoStack.Push(action);



            switch (action.Change)

            {



                case UndoAction.ChangeMode.AddRow:

                    this.RemoveRow(action);

                    break;



                case UndoAction.ChangeMode.DeleteRow:

                    this.InsertRow(action);

                    break;



                case UndoAction.ChangeMode.ModifyCell:

                    this.UpdateCell(action);

                    break;



                default:

                    throw new InvalidOperationException("Unknown redo action change: " + action.Change);

            }



        }



        /// <summary>

        /// Updates the cell value.

        /// </summary>

        /// <param name="action">The action that was performed.</param>

        private void UpdateCell(UndoAction action)

        {

            // Check for valid arguments (value, row, column).

            if (action.Arguments == null || action.Arguments.Length < 3)

                throw new InvalidOperationException("Invalid arguments");

            object previousCellValue = action.Arguments[0];



            int rowIndex = (int)action.Arguments[1];

            int columnIndex = (int)action.Arguments[2];



            // Restore the previous cell value.

            grid[columnIndex, rowIndex].Value = previousCellValue;

        }



        /// <summary>

        /// Inserts the row into the grid's list.

        /// </summary>

        /// <param name="action">The action that was performed.</param>

        private void InsertRow(UndoAction action)

        {

            // Check for valid arguments (row).

            if (action.Arguments == null || action.Arguments.Length < 1)

                throw new InvalidOperationException("Invalid arguments");



            int rowIndex = (int)action.Arguments[0];



            // Add previously removed row, at the previous row.

            dataSource.Insert(rowIndex, action.TrackObject);

        }



        /// <summary>

        /// Removes the row from the grid's list.

        /// </summary>

        /// <param name="action">The action that was performed.</param>

        private void RemoveRow(UndoAction action)

        {

            // Check for valid arguments (row).

            if (action.Arguments == null || action.Arguments.Length < 1)

                throw new InvalidOperationException("Invalid arguments");



            int rowIndex = (int)action.Arguments[0];



            // Remove previously added row.

            dataSource.RemoveAt(rowIndex);

        }



        private void grid_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)

        {

            // Store the previous cell value.

            previousCellValue = grid[e.ColumnIndex, e.RowIndex].Value;

        }



        private void grid_CellEndEdit(object sender, DataGridViewCellEventArgs e)

        {

            // Get the current cell value, to compare to the previous cell value.

            object cellValue = grid[e.ColumnIndex, e.RowIndex].Value;



            if (!previousCellValue.Equals(cellValue))

            {

                // If the cell value changed, push ModifyCell action on the stack, with the previous cell value to restore.

                this.Push(new UndoAction(grid.Rows[e.RowIndex].DataBoundItem, UndoAction.ChangeMode.ModifyCell,

                    previousCellValue, e.RowIndex, e.ColumnIndex));

            }

        }



        private void grid_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)

        {

            // Push an AddRow change to the stack.

            this.Push(new UndoAction(grid.Rows[e.RowIndex].DataBoundItem, UndoAction.ChangeMode.AddRow, e.RowIndex));

        }



        private void grid_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)

        {

            // Push a DeleteRow change to the stack.

            this.Push(new UndoAction(grid.Rows[e.RowIndex].DataBoundItem, UndoAction.ChangeMode.DeleteRow, e.RowIndex));

        }



       
    

    }
}
