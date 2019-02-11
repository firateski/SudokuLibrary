using System.Collections.Generic;
using System.Linq;
using Sudoku.Model;

namespace Sudoku
{
    /// <summary>
    /// Sudoku game board object
    /// </summary>
    public class SudokuBoard
    {
        /// <summary>
        /// The list of the cells.
        /// </summary>
        internal List<Cell> Cells { get; }

        /// <summary>
        /// Sudoku board object.
        /// </summary>
        public SudokuSolver Solver { get; }

        /// <summary>
        /// The number of columns on the rows.
        /// </summary>
        public int TOTAL_ROWS { get; } = 9;

        /// <summary>
        /// The number of columns on the board.
        /// </summary>
        public int TOTAL_COLUMNS { get; } = 9;

        /// <summary>
        /// The number of cells on the board.
        /// </summary>
        public int TOTAL_CELLS { get => TOTAL_ROWS * TOTAL_COLUMNS; }

        /// <summary>
        /// Creates and adds the cells into the list of the 'Cells'.
        /// </summary>
        private void InitializeCells()
        {
            for (int x = 0; x < TOTAL_ROWS; x++)
            {
                for (int y = 0; y < TOTAL_COLUMNS; y++)
                {
                    Cells.Add(new Cell(
                        index: x * TOTAL_ROWS + y,
                        groupNo: (x / 3) + 3 * (y / 3) + 1,
                        value: -1,
                        position: new Cell.RCPosition(row: x + 1, column: y + 1)
                    ));
                }
            }
        }

        /// <summary>
        /// Creates empty Sudoku game object.
        /// </summary>
        public SudokuBoard()
        {
            Cells = new List<Cell>(TOTAL_CELLS);
            Solver = new SudokuSolver(this);

            InitializeCells();
        }

        /// <summary>
        /// Gets Cell specified with the index.
        /// </summary>
        /// <param name="cellIndex">Index of the cell to return. (The index value indicates the position of the cell on the board. 0 for the first column of the first row, 1 for the second column of the first row and so on...)</param>
        public Cell GetCell(int cellIndex) => Cells[cellIndex];

        /// <summary>
        /// Gets Cell specified with row and column numbers.
        /// </summary>
        /// <param name="cellPosition">The row-column number of the cell specified.</param>
        public Cell GetCell(Cell.RCPosition cellPosition)
        {
            // Calculate the index from the Row-Column position.
            int cellIndex = (cellPosition.Row - 1) * TOTAL_ROWS + cellPosition.Column - 1;

            return GetCell(cellIndex);
        }

        /// <summary>
        /// Sets the value of the cell specified with the index.
        /// </summary>
        /// <param name="value">The value between 1 - 9 to set to the cell specified.</param>
        /// <param name="cellIndex">Index of the cell specified. (The index value indicates the position of the cell on the board. 0 for the first column of the first row, 1 for the second column of the first row and so on... )</param>
        public void SetCellValue(int value, int cellIndex) => Cells[cellIndex].Value = value;

        /// <summary>
        /// Sets the value of the cell specified with row and column numbers.
        /// </summary>
        /// <param name="value">The value between 1-9 to set to the cell specified.</param>
        /// <param name="cellPosition">The row-column number of the cell specified.</param>
        public void SetCellValue(int value, Cell.RCPosition cellPosition)
        {
            // Calculate the index from the Row-Column position.
            int cellIndex = (cellPosition.Row - 1) * TOTAL_ROWS + cellPosition.Column - 1;

            SetCellValue(value, cellIndex);
        }

        /// <summary>
        /// Checks the board is already filled.
        /// </summary>
        public bool IsBoardFilled() => Cells.FirstOrDefault(cell => cell.Value == -1) == null;

        /// <summary>
        /// Returns whether table is empty.
        /// </summary>
        public bool IsTableEmpty() => Cells.FirstOrDefault(cell => cell.Value != -1) == null;

        /// <summary>
        /// Fills the game board with -1 which is the default for the empty state. 
        /// </summary>
        public void Clear() => Cells.ForEach(cell => SetCellValue(-1, cell.Index));
    }
}