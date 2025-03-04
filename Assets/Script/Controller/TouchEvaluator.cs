using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle.Controller
{
    public enum Swipe
    {
        NA = -1,
        RIGHT = 0,
        UP = 1,
        LEFT = 2,
        DOWN = 3
    }

    public static class TouchEvaluator
    {
        // 두 지점을 사용하여 Swipe 방향을 구함
        // up: 45~135, left: 135~255, down: 225~315, right: 0~45, 0~315
        public static Swipe EvalSwipeDir(Vector2 vtStart, Vector2 vtEnd)
        {
            float angle = EvalDragAngle(vtStart, vtEnd);

            if (angle < 0)
                return Swipe.NA;

            int swipe = (((int)angle + 45) % 360) / 90;

            switch(swipe)
            {
                case 0: return Swipe.RIGHT;
                case 1: return Swipe.UP;
                case 2: return Swipe.LEFT;
                case 3: return Swipe.DOWN;
            }
            return Swipe.NA;
        }

        // 두 포인트 사이의 각도를 구하는 메서드
        // 인풋 장치 드래그 시에 드래그한 각도를 구하는데 활용
        // 두 포인트 사이의 각도를 반환
        static float EvalDragAngle(Vector2 vtStart, Vector2 vtEnd)
        {
            Vector2 dragDirection = vtEnd - vtStart;
            if (dragDirection.magnitude <= 0.2f)
                return -1f;

            float aimAngle = Mathf.Atan2(dragDirection.y, dragDirection.x);
            if(aimAngle < 0f)
            {
                aimAngle = Mathf.PI * 2 + aimAngle;
            }
            return aimAngle * Mathf.Rad2Deg;
        }
    }

    public static class SwipeDirMethod
    {
        public static int GetTargetRow(this Swipe swipeDir)
        {
            switch(swipeDir)
            {
                case Swipe.DOWN: return -1; ;
                case Swipe.UP: return 1;
                default: return 0;
            }
        }

        public static int GetTargetColumn(this Swipe swipeDir)
        {
            switch(swipeDir)
            {
                case Swipe.LEFT: return -1; ;
                case Swipe.RIGHT: return 1;
                default: return 0;
            }
        }
    }
}