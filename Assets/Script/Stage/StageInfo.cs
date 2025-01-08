using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;

namespace Puzzle.Stage
{
    [System.Serializable]
    public class StageInfo
    {
        public int row;
        public int column;
        public int[] cells;

        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }

        public CellType GetCellType(int row, int column)
        {
            Debug.Assert(cells != null && cells.Length > row * this.column + column, $"Invalid Row/Col = {row}, {column}");

            //if (cells.Length > row * this.column + column)
            //    return (CellType)cells[row * this.column + column];

            int revisedRow = (this.row - 1) - row;
            if (cells.Length > revisedRow * this.column + column)
                return (CellType)cells[revisedRow * this.column + column];

            Debug.Assert(false);

            return CellType.EMPTY;
        }

        public bool DoValidation()
        {
            Debug.Assert(cells.Length == row * column);         // ¿¡·¯1
            Debug.Log($"cell length: {cells.Length}, row, col = ({row}, {column})");

            if (cells.Length != row * column)
                return false;

            return true;
        }
    }
}