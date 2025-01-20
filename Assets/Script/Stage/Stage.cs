using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;
using Puzzle.Controller;
using Puzzle.Core;
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
            this.stageBuilder = stageBuilder;

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
            // 로컬 좌표 -> 보드의 블록 인덱스로 변환
            Vector2 pos = new Vector2(point.x + (maxColumn / 2.0f), point.y + (maxRow / 2.0f));
            int row = (int)pos.y;
            int column = (int)pos.x;

            // 리턴할 블록 인덱스 생성
            blockPos = new BlockPos(row, column);

            // 스왑 가능한지 체크
            return board.IsSwipeable(row, column);
        }

        public bool IsInsideBoard(Vector2 ptrOrg)
        {
            // 계산의 편의를 위해서 (0,0)을 기준으로 좌표를 이동
            // 8*8 보드인 경우: x(-4,+4), y(-4,+4) -> x(0~8), y(0~8)
            Vector2 point = new Vector2(ptrOrg.x + (maxColumn / 2.0f), ptrOrg.y + (maxRow / 2.0f));

            if (point.y < 0 || point.x < 0 || point.y > maxRow || point.x > maxColumn)
                return false;
            return true;
        }

        public IEnumerator CoDoSwipeAction(int row, int column, Swipe swipeDir, Returnable<bool> actionResult)
        {
            actionResult.value = false;     // 코루틴 리턴값 reset

            // 스왑되는 상대 블럭 위치 구함
            int swipeRow = row, swipeColumn = column;
            swipeRow += swipeDir.GetTargetRow();            // right: +1, left: -1
            swipeColumn += swipeDir.GetTargetColumn();         // up: +1, down: -1

            Debug.Assert(row != swipeRow || column != swipeColumn, "Invalid Swipe: ({swipeRow}, {swipeColumn})");
            Debug.Assert(swipeRow >= 0 && swipeRow < maxRow && swipeColumn >= 0 && swipeColumn < maxColumn, $"Swipe 타겟 블럭 인덱스 오류 = ({swipeRow}, {swipeColumn})");

            // 스왑 가능한 블럭인지 체크
            if(board.IsSwipeable(swipeRow, swipeColumn))
            {
                Block targetBlock = blocks[swipeRow, swipeColumn];
                Block baseBlock = blocks[this.row, this.column];
                Debug.Assert(baseBlock != null && targetBlock != null);

                Vector3 basePos = baseBlock.blockObj.transform.position;
                Vector3 targetPos = targetBlock.blockObj.transform.position;

                // 스왑 액션 실행
                if(targetBlock.IsSwipeable(baseBlock))
                {
                    // 상대의 블록 위치로 이동하는 애니메이션 수행
                    baseBlock.MoveTo(targetPos, Constants.SWIPE_DURATION);
                    targetBlock.MoveTo(basePos, Constants.SWIPE_DURATION);

                    yield return new WaitForSeconds(Constants.SWIPE_DURATION);

                    // 보드에 저장된 블록위 위치를 교환
                    blocks[row, column] = targetBlock;
                    blocks[swipeRow, swipeColumn] = baseBlock;

                    actionResult.value = true;
                }
            }

            yield break;
        }

        public bool IsValideSwipe(int row, int column, Swipe swipeDir)
        {
            switch(swipeDir)
            {
                case Swipe.DOWN: return row > 0;
                case Swipe.UP: return row < maxRow - 1;
                case Swipe.LEFT: return column > 0;
                case Swipe.RIGHT: return column < maxColumn - 1;
                default:
                    return false;
            }
        }

        public IEnumerator Evaluate(Returnable<bool> matchResult)
        {
            yield return m_Board.Evaluate(matchResult);
        }

        public IEnumerator PostprocessAfterEvaluate()
        {
            List<KeyValuePair<int, int>> unfilledBlock = new List<KeyValuePair<int, int>>();
            List<Block> movingBlocks = new List<Block>();

            // 제거된 블록에 따라, 블록 재배치(상위 -> 하위 이동/애니메이션)
            yield return m_Board.ArrangeBlocksAfterClean(unfilledBlock, movingBlocks);

            // 유저에게 생성도니 블록이 잠시동안 보이도록 다른 블록이 드롭되는 동안 대기
            yield return WaitForDropping(movingBlocks);
        }

        public IEnumerator WaitForDropping(List<Block> movingBlocks)
        {
            WaitForSeconds waitForSeconds = new WaitForSeconds(0.5f);

            while(true)
            {
                bool bContinue = false;

                // 이동 중인 블록이 있는지 검사
                for(int i=0; i<movingBlocks.Count; i++)
                {
                    if(movingBlocks[i].isMoving)
                    {
                        bContinue = true;
                        break;
                    }
                }

                if (!bContinue)
                    break;

                yield return waitForSeconds;
            }

            movingBlocks.Clear();
            yield break;
        }
    }
}