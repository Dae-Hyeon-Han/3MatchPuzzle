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

        // �־��� ũ�⸦ ���� ���� ����
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

        public IEnumerator CoDoSwipeAction(int row, int column, Swipe swipeDir, Returnable<bool> actionResult)
        {
            actionResult.value = false;     // �ڷ�ƾ ���ϰ� reset

            // ���ҵǴ� ��� �� ��ġ ����
            int swipeRow = row, swipeColumn = column;
            swipeRow += swipeDir.GetTargetRow();            // right: +1, left: -1
            swipeColumn += swipeDir.GetTargetColumn();         // up: +1, down: -1

            Debug.Assert(row != swipeRow || column != swipeColumn, "Invalid Swipe: ({swipeRow}, {swipeColumn})");
            Debug.Assert(swipeRow >= 0 && swipeRow < maxRow && swipeColumn >= 0 && swipeColumn < maxColumn, $"Swipe Ÿ�� �� �ε��� ���� = ({swipeRow}, {swipeColumn})");

            // ���� ������ ������ üũ
            if(board.IsSwipeable(swipeRow, swipeColumn))
            {
                Block targetBlock = blocks[swipeRow, swipeColumn];
                Block baseBlock = blocks[this.row, this.column];
                Debug.Assert(baseBlock != null && targetBlock != null);

                Vector3 basePos = baseBlock.blockObj.transform.position;
                Vector3 targetPos = targetBlock.blockObj.transform.position;

                // ���� �׼� ����
                if(targetBlock.IsSwipeable(baseBlock))
                {
                    // ����� ��� ��ġ�� �̵��ϴ� �ִϸ��̼� ����
                    baseBlock.MoveTo(targetPos, Constants.SWIPE_DURATION);
                    targetBlock.MoveTo(basePos, Constants.SWIPE_DURATION);

                    yield return new WaitForSeconds(Constants.SWIPE_DURATION);

                    // ���忡 ����� ����� ��ġ�� ��ȯ
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

            // ���ŵ� ��Ͽ� ����, ��� ���ġ(���� -> ���� �̵�/�ִϸ��̼�)
            yield return m_Board.ArrangeBlocksAfterClean(unfilledBlock, movingBlocks);

            // �������� �������� ����� ��õ��� ���̵��� �ٸ� ����� ��ӵǴ� ���� ���
            yield return WaitForDropping(movingBlocks);
        }

        public IEnumerator WaitForDropping(List<Block> movingBlocks)
        {
            WaitForSeconds waitForSeconds = new WaitForSeconds(0.5f);

            while(true)
            {
                bool bContinue = false;

                // �̵� ���� ����� �ִ��� �˻�
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