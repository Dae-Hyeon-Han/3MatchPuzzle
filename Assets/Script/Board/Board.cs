using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle.Board
{
    using IntIntKV = KeyValuePair<int, int>;
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

            BoardShuffler shffler = new BoardShuffler(this, true);
            shffler.Shuffle();

            // Cell, Block �������� �̿��ؼ� Board�� Cell / Block ������Ʈ�� �߰��Ѵ�.
            float initX = CalcInitX(0.5f);
            float initY = CalcInitY(0.5f);

            for (int nRow = 0; nRow < row; nRow++)
            {
                for (int nCol = 0; nCol < column; nCol++)
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

        public bool CanShuffle(int row, int column, bool loading)
        {
            if (cells[row, column].cellType.IsBlockMovableType())
                return false;
            return true;
        }

        public void ChangeBlock(Block block, BlockBreed notAllowedBreed)
        {
            BlockBreed genBreed;

            while (true)
            {
                genBreed = (BlockBreed)UnityEngine.Random.Range(0, 6);

                if (notAllowedBreed == genBreed)
                    continue;

                break;
            }

            block.breed = genBreed;
        }

        public bool IsSwipeable(int row, int column)
        {
            return cells[row, column].cellType.IsBlockMovableType();
        }

        public IEnumerator ArrangeBlocksAfterClean(List<IntIntKV> unfilledBlocks, List<Block> movingBlocks)
        {
            SortedList<int, int> emptyBlocks = new SortedList<int, int>();
            List<IntIntKV> emptyRemainBlocks = new List<IntIntKV>();

            for (int column = 0; column < this.column; column++)
            {
                emptyBlocks.Clear();

                // ���� ���� �� ����� ����
                // ���� ���� �ٸ� ���� ����ִ� ��� �ε����� ����. sortedList�̹Ƿ� ù��° ��尡 ���� �Ʒ��� ��� ��ġ
                for (int row = 0; row < this.row; row++)
                {
                    if (CanBlockBeAllocatable(row, column))
                        emptyBlocks.Add(row, column);
                }

                // �Ʒ��ʿ� ����ִ� ����� ���� ���
                if (emptyBlocks.Count == 0)
                    continue;

                // �̵��� ������ ����� ����ִ� �ϴ����� �̵�

                // ���� �Ʒ��ʺ��� ����ִ� ����� ó��
                IntIntKV first = emptyBlocks.First();

                // ����ִ� ��� ���� �������� �̵� ������ ����� Ž���ϸ鼭 �� ����� ä��
                for (int row = first.Value + 1; row < this.row; row++)
                {
                    Block block = m_Blocks[row, column];

                    // �̵� ������ �������� �ƴ� ��� pass
                    if (block == null || m_Cells[row, column].cellType == CellType.EMPTY)
                        continue;

                    // �̵��� �ʿ��� ��� �߰�
                    block.dropDistance = new Vector2(0, row - first.Value);     // ���� ������Ʈ �ִϸ��̼� �̵�
                    movingBlocks.Add(block);

                    // �� �������� �̵�
                    Debug.Assert(m_Cells[first.Value, column].IsObstracle() == false, $"{m_Cells[first.Value, column]}");
                    m_Blocks[first.Value, column] = block;

                    // �ٸ� ������ �̵������Ƿ� ���� ��ġ�� ���
                    m_Blocks[row, column] = null;

                    // ����ִ� ��� ����Ʈ���� ���� ù��° ��带 ����
                    emptyBlocks.RemoveAt(0);

                    // ���� ��ġ�� ����� �ٸ� ��ġ�� �̵������Ƿ� ���� ��ġ�� ��
                    // �׷��Ƿ� ����ִ� ����� �����ϴ� emptyBlocks�� �߰�
                    emptyBlocks.Add(row, column);

                    // ���� ����ִ� ����� ó���ϵ��� ������ ����
                    first = emptyBlocks.First();
                    row = first.Value;
                }
            }

            yield return null;

            // ������� ä������ �ʴ� ����� �ִ� ���(���� �Ʒ� ������ �������)
            if(emptyRemainBlocks.Count>0)
            {
                unfilledBlocks.AddRange(emptyRemainBlocks);
            }
        }

        bool CanBlockBeAllocatable(int row, int column)
        {
            if (!m_Cells[row, column].cellType.IsBlockAllocatableType())
                return false;

            return m_Blocks[row, column] == null;
        }
    }
}