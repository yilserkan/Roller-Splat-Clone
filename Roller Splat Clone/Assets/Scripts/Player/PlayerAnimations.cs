using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    [SerializeField] private AnimationCurve animationCurve;
    [SerializeField] private float lerpDuration = 1f;
    
    private Vector3 scaleVector = Vector3.zero;

    private Coroutine m_Lerp;
    
    private void OnEnable()
    {
        AddListeners();
    }
    
    private void OnDisable()
    {
        RemoveListeners();
    }
    

    private void ScaleAroundRelative(Transform player, Vector3 pivot, Vector3 newScale)
    {
        Vector3 pivotDelta = player.localPosition - pivot;
        pivotDelta.Scale(newScale);
        player.transform.localPosition = pivot + pivotDelta;

        var finalScale = player.transform.localScale;
        finalScale.Scale(newScale);
        player.localScale = finalScale;
            
    }
    private void ScaleAround(Transform player, Vector3 swipeDirection, float newScale)
    {
        if (swipeDirection == Vector3.forward || swipeDirection == Vector3.back)
        {
            scaleVector = new Vector3(1, 1, newScale);
        }
        else
        {
            scaleVector = new Vector3(newScale, 1, 1);
        }

        Vector3 pivot = player.localPosition + swipeDirection;
        Vector3 pivotDelta = player.localPosition - pivot;
        Vector3 scaleFactor = new Vector3(
            scaleVector.x / player.localScale.x,
            scaleVector.y / player.localScale.y,
            scaleVector.z / player.localScale.z
        );
        pivotDelta.Scale(scaleFactor);
        player.transform.localPosition = pivot + pivotDelta;
            
        player.localScale = scaleVector;
            
    }

    IEnumerator Lerp(Transform player, Vector3 swipeDir)
    {
        float timeElapsed = 0;
        float valueToLerp = 0.5f;
        while (timeElapsed < lerpDuration)
        {
           
            valueToLerp = Mathf.Lerp(.5f, 1,   animationCurve.Evaluate(timeElapsed/lerpDuration));
            ScaleAround(player,swipeDir,valueToLerp);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        valueToLerp = 1;
        ScaleAround(player,swipeDir,valueToLerp);
    }
    
    private void HandleOnWallHit(Transform transform, Vector3 swipeDirection)
    {
        if (m_Lerp != null)
        {
            StopCoroutine(m_Lerp);
        }
        m_Lerp = StartCoroutine(Lerp(transform, swipeDirection));
    }

    private void AddListeners()
    {
        PlayerStateMachine.OnWallHit += HandleOnWallHit;
    }
    
    private void RemoveListeners()
    {
        PlayerStateMachine.OnWallHit -= HandleOnWallHit;
    }
}
