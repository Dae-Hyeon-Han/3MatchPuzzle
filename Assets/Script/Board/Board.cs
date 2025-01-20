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

            BoardShuffler shffler = new BoardShuffler(this, true);
            shffler.Shuffle();

            // Cell, Block 프리펩을 이용해서 Board에 Cell / Block 오브젝트를 추가한다.
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

                // 같은 열에 빈 블록을 수집
                // 현재 열의 다른 행의 비어있는 블록 인덱스를 수집. sortedList이므로 첫번째 노드가 가장 아래쪽 블록 위치
                for (int row = 0; row < this.row; row++)
                {
                    if (CanBlockBeAllocatable(row, column))
                        emptyBlocks.Add(row, column);
                }

                // 아래쪽에 비어있는 블록이 없는 경우
                if (emptyBlocks.Count == 0)
                    continue;

                // 이동이 가능한 블록을 비어있는 하단으로 이동

                // 가장 아래쪽부터 비어있는 블록을 처리
                IntIntKV first = emptyBlocks.First();

                // 비어있는 블록 위쪽 방향으로 이동 가능한 블록을 탐색하면서 빈 블록을 채움
                for (int row = first.Value + 1; row < this.row; row++)
                {
                    Block block = m_Blocks[row, column];

                    // 이동 가능한 아이템이 아닌 경우 pass
                    if (block == null || m_Cells[row, column].cellType == CellType.EMPTY)
                        continue;

                    // 이동이 필요한 블록 발견
                    block.dropDistance = new Vector2(0, row - first.Value);     // 게임 오브젝트 애니메이션 이동
                    movingBlocks.Add(block);

                    // 빈 공간으로 이동
                    Debug.Assert(m_Cells[first.Value, column].IsObstracle() == false, $"{m_Cells[first.Value, column]}");
                    m_Blocks[first.Value, column] = block;

                    // 다른 곳으로 이동했으므로 현재 위치는 비움
                    m_Blocks[row, column] = null;

                    // 비어있는 블록 리스트에서 사용된 첫번째 노드를 삭제
                    emptyBlocks.RemoveAt(0);

                    // 현재 위치의 블록이 다른 위치로 이동했으므로 현재 위치가 빔
                    // 그러므로 비어있는 블록을 보관하는 emptyBlocks에 추가
                    emptyBlocks.Add(row, column);

                    // 다음 비어있는 블록을 처리하도록 기준을 변경
                    first = emptyBlocks.First();
                    row = first.Value;
                }
            }

            yield return null;

            // 드록으로 채워지지 않는 블록이 있는 경우(왼쪽 아래 순으로 들어있음)
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