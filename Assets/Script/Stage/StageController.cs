using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Controller;

namespace Puzzle.Stage
{
    public class StageController : MonoBehaviour
    {
        bool init;
        Stage stage;

        [SerializeField] Transform container;
        [SerializeField] GameObject cellPrefab;
        [SerializeField] GameObject blockPrefab;

        InputManager inputManager;
        ActionManager actionManager;

        bool touchDown;             // 입력상태 처리 플래그. 유효한 블록을 클릭한 경우 true
        BlockPos blockDownPos;      // 블록 위치(보드에 저장된 위치)
        Vector3 clickPos;           // 다운 위치(보드 기준 local 좌표)


        // Start is called before the first frame update
        void Start()
        {
            InitStage();
        }

        void InitStage()
        {
            if (init)
                return;

            init = true;
            inputManager = new InputManager(container);

            BuildStage();
            //stage.PrintAll();
        }

        void BuildStage()
        {
            // 스테이지 구성
            stage = StageBuilder.BuildStage(nStage : 2);
            actionManager = new ActionManager(container, stage);

            // 생성한 stage 정보를 이용하여 씬 구성
            stage.ComposeStage(cellPrefab, blockPrefab, container);
        }

        private void Update()
        {
            if (!init)
                return;
            OnInputHandler();
        }

        //void OnInputHandler()
        //{
        //    if(inputManager.isTouchDown)
        //    {
        //        Vector2 point = inputManager.touchPosition;
        //        Debug.Log($"Input Down = {point}, local = {inputManager.touch2BoardPosition}");
        //    }
        //    else if(inputManager.isTouchUp)
        //    {
        //        Vector2 point = inputManager.touchPosition;
        //        Debug.Log($"Input Up = {point}, local = {inputManager.touch2BoardPosition}");
        //    }
        //}

        void OnInputHandler()
        {
            // TouchDown
            if(touchDown && inputManager.isTouchDown)
            {
                // 보드 기준 로컬 좌표
                Vector2 point = inputManager.touch2BoardPosition;

                // 보드에서 클릭하지 않는 경우에는 무시
                if(stage.IsInsideBoard(point))
                {
                    return;
                }

                // 클릭한 위치의 블록 구하기
                BlockPos blockPos;
                if(stage.IsOnValideBlock(point, out blockPos))
                {
                    // 스왑 가능한 블록에서 클릭한 경우
                    touchDown = true;
                    blockDownPos = blockPos;
                    clickPos = point;
                }
            }
            // 유효한 블록 위에서 Down 후에만 up 이벤트 처리
            else if(touchDown && inputManager.isTouchUp)
            {
                // 보드 기준 로컬 좌표를 구한다.
                Vector2 point = inputManager.touch2BoardPosition;

                // 스왑 방향을 구한다
                Swipe swipeDir = inputManager.EvalSwipeDir(clickPos, point);
                Debug.Log($"Swipe: {swipeDir}, Block = {blockDownPos}");

                if (swipeDir != Swipe.NA)
                    actionManager.DoSwipeAction(blockDownPos.row, blockDownPos.column, swipeDir);

                touchDown = false;      // 클릭 상태 플래그 off
            }
        }
    }
}