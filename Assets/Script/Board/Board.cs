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

        #region 이게 왜 필요한지?
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
            // 스테이지 구성에 필요한 Cell, Block, Container(Board) 정보를 저장
            m_CellPrefab = cellPrefab;
            m_BlockPrefab = blockPrefab;
            m_Container = container;

            // Cell, Block 프리펩을 이용해서 Board에 Cell / Block 오브젝트를 추가한다.
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