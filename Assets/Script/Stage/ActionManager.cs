using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Controller;

namespace Puzzle.Stage
{
    public class ActionManager
    {
        Transform container;                // �����̳�
        Stage stage;
        MonoBehaviour monoBehaviour;        // �ڷ�ƾ ȣ�� �� �ʿ��� ���
        bool running;                       // �׼� ���� ����: ���� ���� ��� true

        public ActionManager(Transform container, Stage stage)
        {
            this.container = container;
            this.stage = stage;

            monoBehaviour = container.gameObject.GetComponent<MonoBehaviour>();
        }

        // �ڷ�ƾ Wapper �޼���
        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return monoBehaviour.StartCoroutine(routine);
        }

        // ���� �׼� ����
        // row, column ��� ��ġ
        // SwipeDir ���� ����
        public void DoSwipeAction(int row, int column, Swipe swipeDir)
        {
            Debug.Assert(row > 0 && row < stage.maxRow && column >= 0 && column < stage.maxColumn);

            if(stage.IsValideSwipe(row, column,swipeDir))
            {
                StartCoroutine(CoDoSwipeAction(row, column, swipeDir));
            }
        }

        // ���� �׼��� �����ϴ� �ڷ�ƾ
        IEnumerator CoDoSwipeAction(int row, int column, Swipe swipeDir)
        {
            if(!running)            // �ٸ� �׼� ���̸� pass
            {
                running = true;     // �׼� ���� ���� on

                // Swipe Action ����
                Returnable<bool> swipedBlock = new Returnable<bool>(false);
                yield return stage.CoDoSwipeAction(row, column, swipeDir, swipedBlock);

                running = false;
            }
            yield break;
        }
    }
}