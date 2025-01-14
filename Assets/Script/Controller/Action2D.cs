using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle.Controller
{
    public static class Action2D
    {
        // 지정된 시간 동안 지정된 위치로 이동
        /// <param name="target"> 애니메이션 적용할 타겟 obj </param>
        /// <param name="to"> 이동할 목표 위치 </param>
        /// <param name="duraion"> 이동 시간 </param>
        /// <param name="selfRemove"> 애니메이션 종료 후 타겟 obj 삭제 여부 플래그 </param>
        /// <returns></returns>
        public static IEnumerator MoveTo(Transform target, Vector3 to, float duraion, bool selfRemove = false)
        {
            Vector2 startPos = target.transform.position;
            
            float elapsed = 0.0f;
            while(elapsed<duraion)
            {
                elapsed += Time.smoothDeltaTime;
                target.transform.position = Vector2.Lerp(startPos, to, elapsed / duraion);

                yield return null;
            }

            target.transform.position = to;

            if (selfRemove)
                Object.Destroy(target.gameObject, 0.1f);

            yield break;
        }
    }
}