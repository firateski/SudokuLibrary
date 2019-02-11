using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Model;

namespace Sudoku
{
    /// <summary>
    /// Sudoku Solver object.
    /// </summary>
    public class SudokuSolver
    {
        /// <summary>
        /// Sudoku board instance.
        /// </summary>
        readonly SudokuBoard SudokuBoard;

        /// <summary>
        /// Valid numbers to get random numbers for the cells.
        /// </summary>
        private readonly int[] Numbers = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        /// <summary>
        /// Cell indexes that excludes from backtracking.
        /// </summary>
        private List<int> TheIndexesOfFilledCells = new List<int>();

        /// <summary>
        /// The list to use for backtracking while solving the processes. Each list of specified index represents the blacklist of the cell.
        /// </summary>
        private List<List<int>> BlackListsOfCells;

        /// <summary>
        /// Random object to use creating random numbers.
        /// </summary>
        private readonly Random Random = new Random();

        /// <summary>
        /// Initialize the blacklist.
        /// </summary>
        private void InitializeBlackList()
        {
            BlackListsOfCells = new List<List<int>>(SudokuBoard.TOTAL_CELLS);
            for (int index = 0; index < BlackListsOfCells.Capacity; index++)
            {
                BlackListsOfCells.Add(new List<int>());
            }
        }

        /// <summary>
        /// Creates a solver object for the specified Sudoku object.
        /// </summary>
        /// <param name="sudoku">The sudoku game object to use.</param>
        public SudokuSolver(SudokuBoard sudoku)
        {
            SudokuBoard = sudoku ?? new SudokuBoard();

            InitializeBlackList();
        }

        /// <summary>
        /// Creates solved state to the game board and returns whether the puzzle solved.
        /// </summary>
        /// <param name="UseRandomGenerator">Set it to true to see a different result for each solution.</param>
        /// <returns>Returns whether the board solved.</returns>
        public bool SolveThePuzzle(bool UseRandomGenerator = true)
        {
            // Return false if the current state is board is not valid.
            if (!CheckTableStateIsValid()) return false;

            // Init protected index list to protect the current state of the board while backtracking.
            InitIndexListOfTheAlreadyFilledCells();

            // Clear the blacklist
            ClearBlackList();

            int currentCellIndex = 0;

            // Iterate all the cells of the board.
            while (currentCellIndex < SudokuBoard.TOTAL_CELLS)
            {
                // If the current cell index is protected(which means it was inner the current state of the board), pass it.
                if (TheIndexesOfFilledCells.Contains(currentCellIndex))
                {
                    ++currentCellIndex;
                    continue;
                }

                // Clear blacklists of the indexes after the current index.
                ClearBlackList(startCleaningFromThisIndex: currentCellIndex + 1);

                Cell currentCell = SudokuBoard.GetCell(cellIndex: currentCellIndex);

                int theFoundValidNumber = GetValidNumberForTheCell(currentCellIndex, UseRandomGenerator);

                // No valid number found for the cell. Let's backtrack.
                if (theFoundValidNumber == 0)
                {
                    // Let's backtrack
                    currentCellIndex = BacktrackTo(currentCellIndex);
                }
                else
                {
                    // Set found valid value to current cell.
                    SudokuBoard.SetCellValue(theFoundValidNumber, currentCell.Index);
                    ++currentCellIndex;
                }
            }

            return true;
        }

        /// <summary>
        /// Check current state of the table is valid.
        /// </summary>
        /// <returns>Returns whether is table is valid or not.</returns>
        public bool CheckTableStateIsValid(bool ignoreEmptyCells = false) =>
            SudokuBoard.Cells
            .Where(cell => !ignoreEmptyCells || cell.Value != -1)
            .FirstOrDefault(cell => cell.Value != -1 && !IsValidValueForTheCell(cell.Value, cell)) == null;

        /// <summary>
        /// Checks the specified cell can accept the specified value.
        /// </summary>
        public bool IsValidValueForTheCell(int val, Cell cell)
        {
            // Check the value whether exists in the 3x3 group.
            if (SudokuBoard.Cells.Where(c => c.Index != cell.Index && c.GroupNo == cell.GroupNo).FirstOrDefault(c2 => c2.Value == val) != null)
                return false;

            // Check the value whether exists in the row.
            if (SudokuBoard.Cells.Where(c => c.Index != cell.Index && c.Position.Row == cell.Position.Row).FirstOrDefault(c2 => c2.Value == val) != null)
                return false;

            // Check the value whether exists in the column.
            if (SudokuBoard.Cells.Where(c => c.Index != cell.Index && c.Position.Column == cell.Position.Column).FirstOrDefault(c2 => c2.Value == val) != null)
                return false;

            return true;
        }

        /// <summary>
        /// Init protected index list to protect the current state of the board while backtracking.
        /// </summary>
        public void InitIndexListOfTheAlreadyFilledCells()
        {
            TheIndexesOfFilledCells.Clear();
            TheIndexesOfFilledCells.AddRange(SudokuBoard.Cells
                .FindAll(cell => cell.Value != -1)
                .Select(cell => cell.Index));
        }

        /// <summary>
        /// Backtracking operation for the cell specified with index.
        /// </summary>
        private int BacktrackTo(int index)
        {
            // Pass over the protected cells.
            while (TheIndexesOfFilledCells.Contains(--index)) ;

            // Get the back-tracked Cell.
            Cell backTrackedCell = SudokuBoard.GetCell(index);

            // Add the value to the black-list of the back-tracked cell.
            AddToBlacklist(backTrackedCell.Value, cellIndex: index);

            // Reset the back-tracked cell value.
            backTrackedCell.Value = -1;

            // Reset the blacklist starting from the next one of the current tracking cell.
            ClearBlackList(startCleaningFromThisIndex: index + 1);

            return index;
        }

        /// <summary>
        /// Returns a valid number for the specified cell index.
        /// </summary>
        private int GetValidNumberForTheCell(int cellIndex, bool useRandomFactor)
        {
            int theFoundValidNumber = 0;

            // Find valid numbers for the cell.
            var validNumbers = Numbers.Where(x => !BlackListsOfCells[cellIndex].Contains(x)).ToArray();

            if (validNumbers.Length > 0)
            {
                // Return a (random) valid number from the valid numbers.
                int choosenIndex = useRandomFactor ? Random.Next(validNumbers.Length) : 0;
                theFoundValidNumber = validNumbers[choosenIndex];
            }

            // Try to get valid (random) value for the current cell, if no any valid value break the loop.
            do
            {
                Cell currentCell = SudokuBoard.GetCell(cellIndex);

                // Check the found number if valid for the cell, if is not valid number for the cell then add the value to the blacklist of the cell.
                if (theFoundValidNumber != 0 && !SudokuBoard.Solver.IsValidValueForTheCell(theFoundValidNumber, currentCell))
                    AddToBlacklist(theFoundValidNumber, cellIndex);
                else
                    break;

                // Get a valid (random) value from valid numbers.
                theFoundValidNumber = GetValidNumberForTheCell(cellIndex: cellIndex, useRandomFactor: useRandomFactor);
            } while (theFoundValidNumber != 0);

            return theFoundValidNumber;
        }

        /// <summary>
        /// Add given value into the specified index of the blacklist. 
        /// </summary>
        private void AddToBlacklist(int value, int cellIndex) => BlackListsOfCells[cellIndex].Add(value);

        /// <summary>
        /// Initializes the black lists of the cells.
        /// </summary>
        /// <param name="startCleaningFromThisIndex">Clear the rest of the blacklist starting from the index.</param>
        private void ClearBlackList(int startCleaningFromThisIndex = 0)
        {
            for (int index = startCleaningFromThisIndex; index < BlackListsOfCells.Count; index++)
                BlackListsOfCells[index].Clear();
        }
    }
}