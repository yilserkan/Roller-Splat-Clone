using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;
using Grid = GridSystem.Grid;

public class PlayerAnimations : MonoBehaviour
{
    [SerializeField] private AnimationCurve wallHitCurve;
    [SerializeField] private AnimationCurve jumpCurve;
    [SerializeField] private float lerpDuration = 1f;

    private Vector3 m_ScaleVector = Vector3.zero;
    private Vector3 m_SwipeDir;
    private float m_ValueToLerp;
    private float m_JumpStartValue;
    
    private Coroutine m_Lerp;
    private delegate void DelegateFunction();
    private DelegateFunction m_MethodToCall;
    
    private void OnEnable()
    {
        AddListeners();
    }
    
    private void OnDisable()
    {
        RemoveListeners();
    }
    
    IEnumerator LerpDelegatMethod(DelegateFunction delFunction, AnimationCurve curve)
    {
        float timeElapsed = 0;
        while (timeElapsed < lerpDuration)
        {
            m_ValueToLerp =  curve.Evaluate(timeElapsed/lerpDuration);
            delFunction();
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }
    
    private void ScaleAround()
    {
        if (m_SwipeDir == Vector3.forward || m_SwipeDir == Vector3.back)
        {
            m_ScaleVector = new Vector3(1, 1, m_ValueToLerp);
        }
        else
        {
            m_ScaleVector = new Vector3(m_ValueToLerp, 1, 1);
        }

        Vector3 pivot = transform.localPosition + (m_SwipeDir/2);
        Vector3 pivotDelta = transform.localPosition - pivot;
        Vector3 scaleFactor = new Vector3(
            m_ScaleVector.x / transform.localScale.x,
            m_ScaleVector.y / transform.localScale.y,
            m_ScaleVector.z / transform.localScale.z
        );
        pivotDelta.Scale(scaleFactor);
        transform.transform.localPosition = pivot + pivotDelta;
            
        transform.localScale = m_ScaleVector;
            
    }

    private void Jump()
    {
        Vector3 newPos = new Vector3(transform.position.x, m_JumpStartValue + m_ValueToLerp, transform.position.z);
        transform.position = newPos;
    }
    
    private void HandleOnWallHit(Vector3 swipeDirection)
    {
        if (m_Lerp != null)
        {
            StopCoroutine(m_Lerp);
        }

        m_SwipeDir = swipeDirection;
        
        m_MethodToCall = ScaleAround;
        m_Lerp = StartCoroutine(LerpDelegatMethod(m_MethodToCall, wallHitCurve));
    } 
    private void HandleOnLevelFinished()
    {
        if (m_Lerp != null)
        {
            StopCoroutine(m_Lerp);
        }
  
        m_JumpStartValue = transform.position.y;
        
        m_MethodToCall = Jump;
        m_Lerp = StartCoroutine(LerpDelegatMethod(m_MethodToCall,jumpCurve));
    }

    private void AddListeners()
    {
        PlayerStateMachine.OnWallHit += HandleOnWallHit;
        Grid.OnLevelFinished += HandleOnLevelFinished;
    }

    private void RemoveListeners()
    {
        PlayerStateMachine.OnWallHit -= HandleOnWallHit;
        Grid.OnLevelFinished -= HandleOnLevelFinished;
    }
}
