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
            // 셔플 시작 전에 각 블록의 매칭 정보를 업데이트
            PrepareDuplicationDates();

            // 셔플 대상 블록을 별도 리스트에 보관
            PrepareShuffleBlocks();

            // 위에서 준비한 데이터를 이요앟여 셔플 수행
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
                    // 움직이지 못하는 블록의 매칭 정보를 계산
                    else
                    {
                        block.horzDuplicate = 1;
                        block.vertDuplicate = 1;
                    }

                    // 좌하 위치에 셔플 미대상인 블록의 매치 상태를 반영
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

                    // Sorted List의 순서를 정하기 위해서 중복값이 없도록 랜덤 값을 생성한 후 키 값으로 저장
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
                    // 셔플 미대상 블록은 패스
                    if (!board.CanShuffle(nRow, nCol, loadingMode))
                        continue;

                    // 셔플 대상 블록은 새로 배치할 블록을 리턴 받아서 저장
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
                // Queue에서 블록 하나 꺼낸다
                BlockVectorKV blockInfo = NextBlock(useQueue);
                Block block = blockInfo.Key;

                // 리스트에서 블록을 전부 처리한 경우: 전체 루프에서 1회만 발생
                if(block == null)
                {
                    blockInfo = NextBlock(true);
                    block = blockInfo.Key;
                }

                Debug.Assert(block != null, $"block can't be null : queue count -> {unUsedBlocks.Count}");

                if (prevBreed == BlockBreed.NA)
                    prevBreed = block.breed;

                // 리스트를 모두 처리한 경우
                if(listComplete)
                {
                    if(firstBlock == null)
                    {
                        // 전체 리스트를 저리하고, 처음으로 큐에서 꺼낸 경우
                        firstBlock = block;
                    }
                    else if(System.Object.ReferenceEquals(firstBlock,block))
                    {
                        // 처음 보았던 블록을 다시 처리하는 경우. 즉, 큐에 들어있는 모든 블록이 조건에 맞지 않는 경우
                        board.ChangeBlock(block, prevBreed);
                    }
                }

                // 상하좌우 인접 블록과 겹치는 개수를 계산
                Vector2Int vtDup = CalcDuplications(row, column, block);

                // 2개 이상 매치되는 경우. 혀냊 위치에 해당 블록이 올 수 없으므로 큐에 보관하고 다시 블록 처리하도록 continue 한다
                if(vtDup.x > 2 || vtDup.y>2)
                {
                    unUsedBlocks.Enqueue(blockInfo);
                    useQueue = listComplete || !useQueue;

                    continue;
                }

                // 블록이 위치할 수 있는 경우, 찾은 위치로 block GameObject를 이동시킨다.
                block.vertDuplicate = vtDup.y;
                block.horzDuplicate = vtDup.x;

                if(block.blockObj != null)
                {
                    float initX = board.CalcInitX(Constants.BLOCK_ORG);
                    float initY = board.CalcInitY(Constants.BLOCK_ORG);
                    block.Move(initX + column, initY + row);
                }

                // 찾은 블록을 리턴
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

                // 셔플 미대상 블록이 현재 블록과 중복되는 경우, 셔플미대상 블록의 중복 정보도 함께 업데이트 한다.
                if (rightBlock.horzDuplicate == 1)
                    rightBlock.horzDuplicate = 2;
            }

            if(row < board.maxRow-1 && board.blocks[row+1,column].IsSafeEqual(block))
            {
                Block upperBlock = board.blocks[row + 1, column];
                rowDup += upperBlock.vertDuplicate;

                // 셔플 미대상 블록이 현재 블록과 중복되는 경우, 셔플 미대상 블록의 중복 정보도 함께 업데이트 한다.
                if (upperBlock.vertDuplicate == 1)
                    upperBlock.vertDuplicate = 2;
            }
            return new Vector2Int(colDup, rowDup);
        }
    }
}