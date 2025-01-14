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

        bool touchDown;             // �Է»��� ó�� �÷���. ��ȿ�� ����� Ŭ���� ��� true
        BlockPos blockDownPos;      // ��� ��ġ(���忡 ����� ��ġ)
        Vector3 clickPos;           // �ٿ� ��ġ(���� ���� local ��ǥ)


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
            // �������� ����
            stage = StageBuilder.BuildStage(nStage : 2);
            actionManager = new ActionManager(container, stage);

            // ������ stage ������ �̿��Ͽ� �� ����
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
                // ���� ���� ���� ��ǥ
                Vector2 point = inputManager.touch2BoardPosition;

                // ���忡�� Ŭ������ �ʴ� ��쿡�� ����
                if(stage.IsInsideBoard(point))
                {
                    return;
                }

                // Ŭ���� ��ġ�� ��� ���ϱ�
                BlockPos blockPos;
                if(stage.IsOnValideBlock(point, out blockPos))
                {
                    // ���� ������ ��Ͽ��� Ŭ���� ���
                    touchDown = true;
                    blockDownPos = blockPos;
                    clickPos = point;
                }
            }
            // ��ȿ�� ��� ������ Down �Ŀ��� up �̺�Ʈ ó��
            else if(touchDown && inputManager.isTouchUp)
            {
                // ���� ���� ���� ��ǥ�� ���Ѵ�.
                Vector2 point = inputManager.touch2BoardPosition;

                // ���� ������ ���Ѵ�
                Swipe swipeDir = inputManager.EvalSwipeDir(clickPos, point);
                Debug.Log($"Swipe: {swipeDir}, Block = {blockDownPos}");

                if (swipeDir != Swipe.NA)
                    actionManager.DoSwipeAction(blockDownPos.row, blockDownPos.column, swipeDir);

                touchDown = false;      // Ŭ�� ���� �÷��� off
            }
        }
    }
}