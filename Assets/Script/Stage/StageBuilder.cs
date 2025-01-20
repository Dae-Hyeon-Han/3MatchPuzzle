using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;

namespace Puzzle.Stage
{
    public class StageBuilder
    {
        int stage;
        //int m_nStage;
        StageInfo stageInfo;
        //StageInfo m_StageInfo;
        //public Stage ComposeStage();

        public StageBuilder(int stage)
        {
            this.stage = stage;
            //m_nStage = stage;
        }

        public Stage ComposeStage()
        {
            Debug.Assert(this.stage > 0, $"Invalidate Stage: {this.stage}");

            // 0. �������� ������ �ε�. ���� ũ��, cell, ��� ���� ��
            stageInfo = LoadStage(this.stage);

            //Debug.Log($"����: {stageInfo.row}, {stageInfo.col}");

            // 1. Stage ��ü ����
            Stage stage = new Stage(this, stageInfo.row, stageInfo.column);

            // 2. Cell.Block �ʱⰪ ����
            for (int nRow = 0; nRow < stageInfo.row; nRow++)
            {
                for (int nCol = 0; nCol < stageInfo.column; nCol++)
                {
                    stage.blocks[nRow, nCol] = SpawnBlockForStage(nRow, nCol);
                    stage.cells[nRow, nCol] = SpawnCellForStage(nRow, nCol);
                }
            }

            return stage;
        }

        //public Stage ComposeStage()
        //{
        //    Debug.Assert(m_nStage > 0, $"Invalide Stage : {m_nStage}");

        //    //0. �������� ������ �ε��Ѵ�.(���� ũ��, Cell/�� ���� ��)
        //    m_StageInfo = LoadStage(m_nStage);

        //    //1. Stage ��ü�� �����Ѵ�.
        //    Stage stage = new Stage(this, m_StageInfo.row, m_StageInfo.column);

        //    //2. Cell,Block �ʱ� ���� �����Ѵ�.
        //    for (int nRow = 0; nRow < m_StageInfo.row; nRow++)
        //    {
        //        for (int nCol = 0; nCol < m_StageInfo.column; nCol++)
        //        {
        //            stage.blocks[nRow, nCol] = SpawnBlockForStage(nRow, nCol);
        //            stage.cells[nRow, nCol] = SpawnCellForStage(nRow, nCol);
        //        }
        //    }

        //    return stage;
        //}

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