using System.Collections;
using UnityEngine;
using Grid = GridSystem.Grid;


public class PlayerAnimations : MonoBehaviour
{
    [SerializeField] private AnimationCurve wallHitCurve;
    [SerializeField] private AnimationCurve jumpCurve;
    [SerializeField] private float lerpDuration = 1f;
    [SerializeField] private float jumpLerpDuration = 1f;
    
    private Vector3 m_ScaleVector = Vector3.zero;
    private Vector3 m_SwipeDir;
    private float m_ValueToLerp;
    private float m_JumpStartValue;
    private bool m_PlayingJumpAnimation = false;
    
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
    
    public void PlayWallHitAnimation(Vector3 swipeDirection)
    {
        if (m_PlayingJumpAnimation)
        {
            return;
        }
        
        StopLerp();
        
        m_SwipeDir = swipeDirection;
        
        m_MethodToCall = ScaleAround;
        m_Lerp = StartCoroutine(LerpDelegateMethod(m_MethodToCall, wallHitCurve, lerpDuration));
    } 
    
    public void PlayLevelFinishedAnimation()
    {
        StopLerp();

        m_PlayingJumpAnimation = true;
        m_JumpStartValue = transform.position.y;
        
        m_MethodToCall = Jump;
        m_Lerp = StartCoroutine(LerpDelegateMethod(m_MethodToCall,jumpCurve, jumpLerpDuration));
    }

    private void StopLerp()
    {
        if (m_Lerp != null)
        {
            StopCoroutine(m_Lerp);
        }
    }
    
    IEnumerator LerpDelegateMethod(DelegateFunction delFunction, AnimationCurve curve, float lerpDur)
    {
        float timeElapsed = 0;
        while (timeElapsed < lerpDur)
        {
            m_ValueToLerp =  curve.Evaluate(timeElapsed/lerpDur);
            delFunction();
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }
    
    private void ScaleAround()
    {
        if (Directions.IsDirectionVertical(m_SwipeDir))
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
        Vector3 newPos = new Vector3(
            transform.position.x, 
            m_JumpStartValue + m_ValueToLerp, 
            transform.position.z
            );
        
        transform.position = newPos;
    }

    private void HandleOnLevelCreated()
    {
        m_PlayingJumpAnimation = false;
    }
    
    private void AddListeners()
    {
        Grid.OnLevelCreated += HandleOnLevelCreated;
    }

    private void RemoveListeners()
    {
        Grid.OnLevelCreated -= HandleOnLevelCreated;
    }
}
