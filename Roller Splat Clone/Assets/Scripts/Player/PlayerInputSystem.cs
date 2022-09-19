using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerInputSystem : MonoBehaviour
    {
        [SerializeField] private float touchDetectionThreshold;
        
        private Vector2 m_StartTouchPosition;
        private Vector2 m_LastTouchPosition;

        private bool m_IsTouching;


        private bool IsTouching => Input.touchCount > 0;
        private bool TouchingStarted => m_IsTouching;

        private bool PassedThreshold(Vector2 touchPos) => 
            Vector2.SqrMagnitude(m_StartTouchPosition - touchPos) >
                                       touchDetectionThreshold * touchDetectionThreshold;
        
        private void Update()
        {
            if (IsTouching)
            {
                Touch touch = Input.GetTouch(0);
                Vector2 touchPosition = touch.position;

                if (!TouchingStarted)
                {
                    SetTouchStartPosition(touchPosition);
                }
                
                SetIsTouching(true);

                if (PassedThreshold(touchPosition))
                {
                    FindMoveDirection(touchPosition);
                    
                }

                m_LastTouchPosition = touchPosition;
            }
            else
            {
                SetIsTouching(false);
            }
        }

        private Vector2 FindMoveDirection(Vector2 touchPos)
        {
            Vector2 swipeDir = (touchPos - m_StartTouchPosition).normalized;

            if (Mathf.Abs(swipeDir.x) >=  Mathf.Abs(swipeDir.y))
            {
                swipeDir = new Vector2(swipeDir.x, 0).normalized;
            }
            else
            {
                swipeDir = new Vector2( 0,swipeDir.y).normalized;
            }
            Debug.Log(swipeDir);
            return swipeDir;
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
    }
}