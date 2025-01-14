using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle.Controller
{
    public static class Action2D
    {
        // ������ �ð� ���� ������ ��ġ�� �̵�
        /// <param name="target"> �ִϸ��̼� ������ Ÿ�� obj </param>
        /// <param name="to"> �̵��� ��ǥ ��ġ </param>
        /// <param name="duraion"> �̵� �ð� </param>
        /// <param name="selfRemove"> �ִϸ��̼� ���� �� Ÿ�� obj ���� ���� �÷��� </param>
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