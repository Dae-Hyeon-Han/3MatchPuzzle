using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Controller;

namespace Puzzle.Stage
{
    public class ActionManager
    {
        Transform container;                // 컨테이너
        Stage stage;
        MonoBehaviour monoBehaviour;        // 코루틴 호출 시 필요한 모노
        bool running;                       // 액션 실행 상태: 실행 중인 경우 true

        public ActionManager(Transform container, Stage stage)
        {
            this.container = container;
            this.stage = stage;

            monoBehaviour = container.gameObject.GetComponent<MonoBehaviour>();
        }

        // 코루틴 Wapper 메서드
        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return monoBehaviour.StartCoroutine(routine);
        }

        // 스왑 액션 시작
        // row, column 블록 위치
        // SwipeDir 스왑 방향
        public void DoSwipeAction(int row, int column, Swipe swipeDir)
        {
            Debug.Assert(row > 0 && row < stage.maxRow && column >= 0 && column < stage.maxColumn);

            if(stage.IsValideSwipe(row, column,swipeDir))
            {
                StartCoroutine(CoDoSwipeAction(row, column, swipeDir));
            }
        }

        // 스왑 액션을 수행하는 코루틴
        IEnumerator CoDoSwipeAction(int row, int column, Swipe swipeDir)
        {
            if(!running)            // 다른 액션 중이면 pass
            {
                running = true;     // 액션 실행 상태 on

                // Swipe Action 수행
                Returnable<bool> swipedBlock = new Returnable<bool>(false);
                yield return stage.CoDoSwipeAction(row, column, swipeDir, swipedBlock);

                running = false;
            }
            yield break;
        }

        IEnumerator EvaluateBoard(Returnable<bool> matchResult)
        {
            bool bFirst = true;

            while(true)
            {
                // 매치 블록 제거
                Returnable<bool> blockMatched = new Returnable<bool>(false);
                yield return StartCoroutine(stage.Evaluate(blockMatched));

                // 3매치 블록이 있는 경우 후처리 실행
                if (blockMatched.value)
                {
                    matchResult.value = true;

                    // 매칭 블록 제거 후 빈 블록 드롭 후 새 블록 생성
                    yield return StartCoroutine(stage.PostprocessAfterEvaluate());
                }
                else
                    break;
            }
            yield break;

            //// 매치 블록 제거
            //yield return stage.Evaluate(matchResult);

            //// 매칭 블록 제거 후 빈 블록 드롭 및 새 블록 생성을 처리하는 후처리 수행
            //if(matchResult.value)
            //{
            //    yield return stage.PostprocessAfterEvaluate();
            //}
        }
    }
}