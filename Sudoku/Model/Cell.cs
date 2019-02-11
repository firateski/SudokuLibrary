namespace Sudoku.Model
{
    /// <summary>
    /// Game board cell 
    /// </summary>
    public class Cell
    {
        /// <summary>
        /// Row-Column number based position data structure.
        /// </summary>
        public struct RCPosition
        {
            /// <summary>
            /// Row number
            /// </summary>
            public int Row { get; internal set; }

            /// <summary>
            /// Columns number
            /// </summary>
            public int Column { get; internal set; }

            /// <summary>
            /// Initialize a new instance of the Row-Column based Position with given parameters.
            /// </summary>
            /// <param name="row"></param>
            /// <param name="column"></param>
            public RCPosition(int row, int column)
            {
                Row = row;
                Column = column;
            }
        }

        /// <summary>
        /// Cell value.
        /// </summary>
        public int Value { get; internal set; }

        /// <summary>
        /// Cell index in which the cell is located in the single-dimensional list.
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Group number in which the cell is located.
        /// </summary>
        public int GroupNo { get; }

        /// <summary>
        /// Row-Column number based position of the cell.
        /// </summary>
        public RCPosition Position { get; }

        /// <summary>
        /// Initialize a new instance of the Cell with given parameters.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <param name="groupNo"></param>
        /// <param name="position"></param>
        public Cell(int value, int index, int groupNo, RCPosition position)
        {
            Value = value;
            Index = index;
            GroupNo = groupNo;
            Position = position;
        }
    }
}