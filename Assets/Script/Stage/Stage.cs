using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;
using Puzzle.Controller;
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

        // �־��� ũ�⸦ ���� ���� ����
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

        public bool IsOnValideBlock(Vector2 point, out BlockPos blockPos)
        {
            // ���� ��ǥ -> ������ ��� �ε����� ��ȯ
            Vector2 pos = new Vector2(point.x + (maxColumn / 2.0f), point.y + (maxRow / 2.0f));
            int row = (int)pos.y;
            int column = (int)pos.x;

            // ������ ��� �ε��� ����
            blockPos = new BlockPos(row, column);

            // ���� �������� üũ
            return board.IsSwipeable(row, column);
        }

        public bool IsInsideBoard(Vector2 ptrOrg)
        {
            // ����� ���Ǹ� ���ؼ� (0,0)�� �������� ��ǥ�� �̵�
            // 8*8 ������ ���: x(-4,+4), y(-4,+4) -> x(0~8), y(0~8)
            Vector2 point = new Vector2(ptrOrg.x + (maxColumn / 2.0f), ptrOrg.y + (maxRow / 2.0f));

            if (point.y < 0 || point.x < 0 || point.y > maxRow || point.x > maxColumn)
                return false;
            return true;
        }
    }
}