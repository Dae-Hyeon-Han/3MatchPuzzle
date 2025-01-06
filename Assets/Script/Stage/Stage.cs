using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;
using System;

namespace Puzzle.Stage
{
    public class Stage : MonoBehaviour
    {
        int row;
        int column;
        public int maxRow { get { return row; } }
        public int maxColumn { get { return column; } }

        Puzzle.Board.Board m_Board;
        public Puzzle.Board.Board board { get { return m_Board; } }

        StageBuilder stageBuilder;

        public Block[,] blocks { get { return m_Board.blocks; } }
        public Cell[,] cells { get { return m_Board.cells; } }

        // 주어진 크기를 갖는 보드 생성
        public Stage(StageBuilder stageBuilder, int nRow, int nCol)
        {
            stageBuilder = stageBuilder;

            m_Board = new Puzzle.Board.Board(nRow, nCol);
        }

        internal void ComposeStage(GameObject cellPrefab, GameObject blockPrefab, Transform container)
        {
            m_Board.ComposeStage(cellPrefab, blockPrefab, container);
        }

        public void PrintAll()
        {
            System.Text.StringBuilder strCells = new System.Text.StringBuilder();
            System.Text.StringBuilder strBlocks = new System.Text.StringBuilder();

            for (int row = maxRow - 1; row >= 0; row--)
            {
                for (int column = 0; column < maxColumn; column++)
                {
                    strCells.Append($"{cells[row, column].cellType}, ");
                    strBlocks.Append($"{blocks[row, column].blockType}, ");
                }

                strCells.Append("\n");
                strBlocks.Append("\n");
            }
            Debug.Log(strCells.ToString());
            Debug.Log(strBlocks.ToString());
        }

    }
}