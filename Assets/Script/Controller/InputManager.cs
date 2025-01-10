using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle.Controller
{
    public class InputManager
    {
        Transform container;

#if UNITY_ANDROID && !UNITY_EDITOR
        IInputHandlerBase m_InputHandler = new TouchHandler();
#else
        IInputHandlerBase m_InputHandler = new MouseHandler();
#endif
        public InputManager(Transform container)
        {
            this.container = container;
        }

        public bool isTouchDown => m_InputHandler.isInputDown;
        public bool isTouchUp => m_InputHandler.isInputUp;
        public Vector2 touchPosition => m_InputHandler.inputPosition;
        public Vector2 touch2BoardPosition => TouchToPosition(m_InputHandler.inputPosition);

        Vector2 TouchToPosition(Vector3 vtInput)
        {
            Vector3 vtMousePosW = Camera.main.ScreenToWorldPoint(vtInput);
            Vector3 vtContainerLocal = container.transform.InverseTransformPoint(vtMousePosW);

            return vtContainerLocal;
        }

        public Swipe EvalSwipeDir(Vector2 vtStart, Vector2 vtEnd)
        {
            return TouchEvaluator.EvalSwipeDir(vtStart, vtEnd);
        }
    }
}