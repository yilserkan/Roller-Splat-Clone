using System;
using UnityEngine;

namespace Player
{
    public class PlayerInputSystem : MonoBehaviour
    {
        [SerializeField] private float touchDetectionThreshold;
        
        private Vector2 m_StartTouchPosition;
        
        private bool m_IsTouching;
        private bool IsTouching => Input.touchCount > 0;
        private bool IsTouchStartPositionSet => m_IsTouching;

        private bool IsSwipeDirHorizontal(Vector2 swipeDir) => Mathf.Abs(swipeDir.x) >= Mathf.Abs(swipeDir.y);

        private bool PassedThreshold(Vector2 touchPos) => 
            Vector2.SqrMagnitude(m_StartTouchPosition - touchPos) >
                                       touchDetectionThreshold * touchDetectionThreshold;
        
        public static event Action<Vector2Int> OnPlayerSwipe; 
        
        private void Update()
        {
            if (IsTouching)
            {
                Touch touch = Input.GetTouch(0);
                Vector2 touchPosition = touch.position;

                if (!IsTouchStartPositionSet)
                {
                    SetTouchStartPosition(touchPosition);
                }
                
                SetIsTouching(true);

                if (PassedThreshold(touchPosition))
                {
                    Vector2Int swipeDir = FindMoveDirection(touchPosition);
                    OnPlayerSwipe?.Invoke(swipeDir);
                }
            }
            else
            {
                SetIsTouching(false);
            }
        }

        private Vector2Int FindMoveDirection(Vector2 touchPos)
        {
            Vector2 swipeDir = (touchPos - m_StartTouchPosition).normalized;

            if (IsSwipeDirHorizontal(swipeDir))
            {
                swipeDir = new Vector2(swipeDir.x, 0).normalized;
            }
            else
            {
                swipeDir = new Vector2( 0,swipeDir.y).normalized;
            }

            return new Vector2Int(
                Mathf.RoundToInt(swipeDir.x), 
                Mathf.RoundToInt(swipeDir.y)
                );
        }
        
        private void SetIsTouching(bool value)
        {
            if (m_IsTouching != value)
            {
                m_IsTouching = value;
            }
        }

        private void SetTouchStartPosition(Vector2 touchPos)
        {
            m_StartTouchPosition = touchPos;
        }

        public void ResetTouchStartPosition()
        {
            SetIsTouching(false);
            m_StartTouchPosition = Vector2.zero;
        }
        
        public void EnablePlayerInputs()
        {
            enabled = true;
        }

        public void DisablePlayerInputs()
        {
            ResetTouchStartPosition();
            enabled = false;
        }
    }
}