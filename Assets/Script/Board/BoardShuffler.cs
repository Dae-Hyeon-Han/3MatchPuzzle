using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Core;

namespace Puzzle.Board
{
    using BlockVectorKV = KeyValuePair<Block, Vector2Int>;

    public class BoardShuffler
    {
        Board board;
        bool loadingMode;

        SortedList<int, BlockVectorKV> orgBlock = new SortedList<int, BlockVectorKV>();
        IEnumerator<KeyValuePair<int, BlockVectorKV>> m_it;
        Queue<BlockVectorKV> unUsedBlocks = new Queue<BlockVectorKV>();
        bool listComplete;

        public BoardShuffler(Board board, bool lodingMode)
        {
            this.board = board;
            this.loadingMode = lodingMode;
        }


        public void Shuffle(bool animation = false)
        {
            // ���� ���� ���� �� ����� ��Ī ������ ������Ʈ
            PrepareDuplicationDates();

            // ���� ��� ����� ���� ����Ʈ�� ����
            PrepareShuffleBlocks();

            // ������ �غ��� �����͸� �̿�ۿ� ���� ����
            RunShuffle(animation);
        }

        void PrepareDuplicationDates()
        {
            for (int nRow = 0; nRow < board.maxRow; nRow++)
            {
                for (int nCol = 0; nCol < board.maxColumn; nCol++)
                {
                    Block block = board.blocks[nRow, nCol];

                    if (block == null)
                        continue;

                    if (board.CanShuffle(nRow, nCol, loadingMode))
                    {
                        block.ResetDuplicationInfo();
                    }
                    // �������� ���ϴ� ����� ��Ī ������ ���
                    else
                    {
                        block.horzDuplicate = 1;
                        block.vertDuplicate = 1;
                    }

                    // ���� ��ġ�� ���� �̴���� ����� ��ġ ���¸� �ݿ�
                    if (nCol > 0 && !board.CanShuffle(nRow, nCol - 1, loadingMode) && board.blocks[nRow, nCol - 1].IsSafeEqual(block))
                    {
                        block.horzDuplicate = 2;
                        board.blocks[nRow, nCol - 1].horzDuplicate = 2;
                    }

                    if (nRow > 0 && !board.CanShuffle(nRow - 1, nCol, loadingMode) && board.blocks[nRow - 1, nCol].IsSafeEqual(block))
                    {
                        block.vertDuplicate = 2;
                        board.blocks[nRow - 1, nCol].vertDuplicate = 2;
                    }
                }
            }
        }

        void PrepareShuffleBlocks()
        {
            for (int nRow = 0; nRow < board.maxRow; nRow++)
            {
                for (int nCol = 0; nCol < board.maxColumn; nCol++)
                {
                    if (!board.CanShuffle(nRow, nCol, loadingMode))
                        continue;

                    // Sorted List�� ������ ���ϱ� ���ؼ� �ߺ����� ������ ���� ���� ������ �� Ű ������ ����
                    while (true)
                    {
                        int rand = UnityEngine.Random.Range(0, 10000);
                        if (orgBlock.ContainsKey(rand))
                            continue;

                        orgBlock.Add(rand, new BlockVectorKV(board.blocks[nRow, nCol], new Vector2Int(nCol, nRow)));
                        break;
                    }
                }
            }
            m_it = orgBlock.GetEnumerator();
        }

        void RunShuffle(bool animation)
        {
            for(int nRow = 0; nRow<board.maxRow; nRow++)
            {
                for(int nCol = 0; nCol<board.maxColumn; nCol++)
                {
                    // ���� �̴�� ����� �н�
                    if (!board.CanShuffle(nRow, nCol, loadingMode))
                        continue;

                    // ���� ��� ����� ���� ��ġ�� ����� ���� �޾Ƽ� ����
                    board.blocks[nRow, nCol] = GetShuffledBlock(nRow, nCol);
                }
            }
        }

        Block GetShuffledBlock(int row, int column)
        {
            BlockBreed prevBreed = BlockBreed.NA;
            Block firstBlock = null;

            bool useQueue = true;
            while(true)
            {
                // Queue���� ��� �ϳ� ������
                BlockVectorKV blockInfo = NextBlock(useQueue);
                Block block = blockInfo.Key;

                // ����Ʈ���� ����� ���� ó���� ���: ��ü �������� 1ȸ�� �߻�
                if(block == null)
                {
                    blockInfo = NextBlock(true);
                    block = blockInfo.Key;
                }

                Debug.Assert(block != null, $"block can't be null : queue count -> {unUsedBlocks.Count}");

                if (prevBreed == BlockBreed.NA)
                    prevBreed = block.breed;

                // ����Ʈ�� ��� ó���� ���
                if(listComplete)
                {
                    if(firstBlock == null)
                    {
                        // ��ü ����Ʈ�� �����ϰ�, ó������ ť���� ���� ���
                        firstBlock = block;
                    }
                    else if(System.Object.ReferenceEquals(firstBlock,block))
                    {
                        // ó�� ���Ҵ� ����� �ٽ� ó���ϴ� ���. ��, ť�� ����ִ� ��� ����� ���ǿ� ���� �ʴ� ���
                        board.ChangeBlock(block, prevBreed);
                    }
                }

                // �����¿� ���� ��ϰ� ��ġ�� ������ ���
                Vector2Int vtDup = CalcDuplications(row, column, block);

                // 2�� �̻� ��ġ�Ǵ� ���. ��T ��ġ�� �ش� ����� �� �� �����Ƿ� ť�� �����ϰ� �ٽ� ��� ó���ϵ��� continue �Ѵ�
                if(vtDup.x > 2 || vtDup.y>2)
                {
                    unUsedBlocks.Enqueue(blockInfo);
                    useQueue = listComplete || !useQueue;

                    continue;
                }

                // ����� ��ġ�� �� �ִ� ���, ã�� ��ġ�� block GameObject�� �̵���Ų��.
                block.vertDuplicate = vtDup.y;
                block.horzDuplicate = vtDup.x;

                if(block.blockObj != null)
                {
                    float initX = board.CalcInitX(Constants.BLOCK_ORG);
                    float initY = board.CalcInitY(Constants.BLOCK_ORG);
                    block.Move(initX + column, initY + row);
                }

                // ã�� ����� ����
                return block;
            }
        }

        BlockVectorKV NextBlock(bool useQueue)
        {
            if (useQueue && unUsedBlocks.Count > 0)
                return unUsedBlocks.Dequeue();

            if (!listComplete && m_it.MoveNext())
                return m_it.Current.Value;

            listComplete = true;

            return new BlockVectorKV(null, Vector2Int.zero);
        }

        Vector2Int CalcDuplications(int row, int column, Block block)
        {
            int colDup = 1, rowDup = 1;

            if (column > 0 && board.blocks[row, column - 1].IsSafeEqual(block))
                colDup += board.blocks[row, column - 1].horzDuplicate;

            if (row > 0 && board.blocks[row - 1, column].IsSafeEqual(block))
                rowDup += board.blocks[row - 1, column].vertDuplicate;

            if(column < board.maxColumn -1 && board.blocks[row, column+1].IsSafeEqual(block))
            {
                Block rightBlock = board.blocks[row, column + 1];
                colDup += rightBlock.horzDuplicate;

                // ���� �̴�� ����� ���� ��ϰ� �ߺ��Ǵ� ���, ���ù̴�� ����� �ߺ� ������ �Բ� ������Ʈ �Ѵ�.
                if (rightBlock.horzDuplicate == 1)
                    rightBlock.horzDuplicate = 2;
            }

            if(row < board.maxRow-1 && board.blocks[row+1,column].IsSafeEqual(block))
            {
                Block upperBlock = board.blocks[row + 1, column];
                rowDup += upperBlock.vertDuplicate;

                // ���� �̴�� ����� ���� ��ϰ� �ߺ��Ǵ� ���, ���� �̴�� ����� �ߺ� ������ �Բ� ������Ʈ �Ѵ�.
                if (upperBlock.vertDuplicate == 1)
                    upperBlock.vertDuplicate = 2;
            }
            return new Vector2Int(colDup, rowDup);
        }
    }
}