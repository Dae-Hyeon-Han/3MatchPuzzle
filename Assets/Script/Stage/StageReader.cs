using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle.Stage
{
    public static class StageReader
    {
        // nStage = �������� �ε��� ��ȣ
        public static StageInfo LoadStage(int nStage)
        {
            Debug.Log($"Load Stage: Stage/{GetFileName(nStage)}");

            // ���ҽ� ���Ͽ��� �ؽ�Ʈ �о� ����
            TextAsset textAsset = Resources.Load<TextAsset>($"Stage/{GetFileName(nStage)}");
            if(textAsset != null)
            {
                // Json ���ڿ��� ��ü�� ��ȯ
                StageInfo stageInfo = JsonUtility.FromJson<StageInfo>(textAsset.text);

                // ��ȯ�� ��ü�� ��ȿ���� üũ
                Debug.Assert(stageInfo.DoValidation());     // ����2

                return stageInfo;
            }
            return null;
        }

        static string GetFileName(int nStage)
        {
            return string.Format("stage_{0:D4}", nStage);
        }
    }
}