using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle.Stage
{
    public static class StageReader
    {
        // nStage = 스테이지 인덱스 번호
        public static StageInfo LoadStage(int nStage)
        {
            Debug.Log($"Load Stage: Stage/{GetFileName(nStage)}");

            // 리소스 파일에서 텍스트 읽어 오기
            TextAsset textAsset = Resources.Load<TextAsset>($"Stage/{GetFileName(nStage)}");
            if(textAsset != null)
            {
                // Json 문자열을 객체로 변환
                StageInfo stageInfo = JsonUtility.FromJson<StageInfo>(textAsset.text);

                // 변환된 객체가 유효한지 체크
                Debug.Assert(stageInfo.DoValidation());     // 에러2

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