using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle.Board
{
    public static class CellFactory
    {
        public static Cell SpawnCell(Stage.StageInfo stageInfo, int row, int column)
        {
            Debug.Assert(stageInfo != null);
            Debug.Assert(row < stageInfo.row && column < stageInfo.column);

            return SpawnCell(stageInfo.GetCellType(row, column));
        }

        public static Cell SpawnCell(CellType cellType)
        {
            return new Cell(cellType);
        }
    }
}