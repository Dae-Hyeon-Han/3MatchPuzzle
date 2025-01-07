using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle.Board
{
    public class Board : MonoBehaviour
    {
        int row;
        int column;
        public int maxRow { get { return row; } }
        public int maxColumn { get { return column; } }

        Cell[,] m_Cells;
        public Cell[,] cells { get { return m_Cells; } }

        Block[,] m_Blocks;
        public Block[,] blocks { get { return m_Blocks; } }

        #region �̰� �� �ʿ�����?
        Transform m_Container;
        GameObject m_CellPrefab;
        GameObject m_BlockPrefab;
        #endregion

        public Board(int row, int column)
        {
            this.row = row;
            this.column = column;

            m_Cells = new Cell[row, column];
            m_Blocks = new Block[row, column];
        }

        internal void ComposeStage(GameObject cellPrefab, GameObject blockPrefab, Transform container)
        {
            // �������� ������ �ʿ��� Cell, Block, Container(Board) ������ ����
            m_CellPrefab = cellPrefab;
            m_BlockPrefab = blockPrefab;
            m_Container = container;

            // Cell, Block �������� �̿��ؼ� Board�� Cell / Block ������Ʈ�� �߰��Ѵ�.
            float initX = CalcInitX(0.5f);
            float initY = CalcInitY(0.5f);

            for(int nRow = 0; nRow<row; nRow++)
            {
                for(int nCol = 0; nCol < column; nCol++)
                {
                    Cell cell = m_Cells[nRow, nCol]?.InstantiateCellObj(cellPrefab, container);
                    cell?.Move(initX + nCol, initY + nRow);

                    Block block = m_Blocks[nRow, nCol]?.InstantiateBlockObj(blockPrefab, container);
                    block?.Move(initX + nCol, initY + nRow);
                }
            }
        }

        public float CalcInitX(float offset = 0)
        {
            return -column / 2.0f + offset;
        }

        public float CalcInitY(float offset = 0)
        {
            return -row / 2.0f + offset;
        }
    }
}