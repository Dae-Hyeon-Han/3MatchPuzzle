using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;

namespace Puzzle.Stage
{
    public class StageBuilder
    {
        int stage;

        public StageBuilder(int stage)
        {
            this.stage = stage;
        }

        public Stage ComposeStage(int row, int column)
        {
            // 1. Stage 按眉 积己
            Stage stage = new Stage(this, row, column);

            // 2. Cell.Block 檬扁蔼 积己
            for(int nRow = 0; nRow< row; nRow++)
            {
                stage.blocks[nRow, column] = SpawnBlockForStage(nRow, column);
                stage.cells[nRow, column] = SpawnCellForStage(nRow, column);
            }

            return stage;
        }

        Block SpawnBlockForStage(int row, int column)
        {
            return new Block(BlockType.BASIC);
        }

        Cell SpawnCellForStage(int row, int column)
        {
            return new Cell(CellType.BASIC);
        }

        public static Stage BuildStage(int nStage, int row, int column)
        {
            StageBuilder stageBuilder = new StageBuilder(0);
            Stage stage = stageBuilder.ComposeStage(row, column);

            return stage;
        }
    }
}