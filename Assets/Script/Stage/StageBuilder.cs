using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;

namespace Puzzle.Stage
{
    public class StageBuilder
    {
        int stage;
        StageInfo stageInfo;
        //public Stage ComposeStage();

        public StageBuilder(int stage)
        {
            this.stage = stage;
        }

        public Stage ComposeStage()
        {
            Debug.Assert(this.stage > 0, $"Invalidate Stage: {this.stage}");

            // 0. 스테이지 정보를 로드. 보드 크기, cell, 블록 정보 등
            stageInfo = LoadStage(this.stage);

            // 1. Stage 객체 생성
            Stage stage = new Stage(this, stageInfo.row, stageInfo.column);

            // 2. Cell.Block 초기값 생성
            for(int nRow = 0; nRow< stageInfo.row; nRow++)
            {
                for (int nCol = 0; nCol < stageInfo.column; nCol++)
                {
                    stage.blocks[nRow, nCol] = SpawnBlockForStage(nRow, nCol);
                    stage.cells[nRow, nCol] = SpawnCellForStage(nRow, nCol);
                }
            }

            return stage;
        }

        public StageInfo LoadStage(int stage)
        {
            StageInfo stageInfo = StageReader.LoadStage(stage);
            if (stageInfo != null)
                Debug.Log(stageInfo.ToString());

            return stageInfo;
        }

        Block SpawnBlockForStage(int row, int column)
        {
            //return new Block(BlockType.BASIC);
            //return new Block(row == column ? BlockType.EMPTY : BlockType.BASIC);
            //return row == column ? SpawnEmptyBlock() : SpawnBlock();

            if (stageInfo.GetCellType(row, column) == CellType.EMPTY)
                return SpawnEmptyBlock();

            return SpawnBlock();
        }

        Cell SpawnCellForStage(int row, int column)
        {
            //return new Cell(CellType.BASIC);
            //return new Cell(row == column ? CellType.EMPTY : CellType.BASIC);

            Debug.Assert(stageInfo != null);
            Debug.Assert(row < stageInfo.row && column < stageInfo.column);

            return CellFactory.SpawnCell(stageInfo, row, column);
        }

        public static Stage BuildStage(int nStage)
        {
            StageBuilder stageBuilder = new StageBuilder(nStage);
            Stage stage = stageBuilder.ComposeStage();

            return stage;
        }

        public Block SpawnBlock()
        {
            return BlockFactory.SpawnBlock(BlockType.BASIC);
        }

        public Block SpawnEmptyBlock()
        {
            Block newBlock = BlockFactory.SpawnBlock(BlockType.EMPTY);
            return newBlock;
        }
    }
}