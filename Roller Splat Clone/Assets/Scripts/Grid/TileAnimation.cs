using System.Collections;
using UnityEngine;

namespace GridSystem
{
    public class TileAnimation : MonoBehaviour
    {
        [SerializeField] private AnimationCurve wallHitCurve;
        [SerializeField] private float lerpDuration = 1f;

        private Vector3 m_ScaleVector;
        private bool m_LocalTransformChanged = false;
        private Vector3 m_StartLocalPos;
        private Vector3 m_StartLocalScale;

        private Coroutine m_Lerp;

        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();

            ResetTileAnimationVars();
        }
        
        public void PlayHitAnim(Direction dir)
        {
            if (m_Lerp != null)
            {
                ResetLocalPositionAndScale();
                StopCoroutine(m_Lerp);
            }

            SetLocalPositionAndScale();
            m_Lerp = StartCoroutine(Lerp(dir));
        }

        IEnumerator Lerp(Direction direction)
        {
            float timeElapsed = 0;
            float valueToLerp = 0;
            while (timeElapsed < lerpDuration)
            {
                valueToLerp =  wallHitCurve.Evaluate(timeElapsed/lerpDuration);
                ScaleAround(direction, valueToLerp);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            ResetLocalPositionAndScale();
        }
        
        private void ScaleAround(Direction dir, float scaleValue)
        {
            Vector2Int dirInVector2Int = Grid.m_Directions[dir];
            Vector3 dirInVector3 = new Vector3(dirInVector2Int.x, 0, dirInVector2Int.y);
            
            if (Directions.IsDirectionVertical(dir))
            {
                m_ScaleVector = new Vector3(1, 1, scaleValue);
            }
            else
            {
                m_ScaleVector = new Vector3(scaleValue, 1, 1);
            }
            
            Vector3 pivot = transform.localPosition + (dirInVector3/2);
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

        private void ResetTileAnimationVars()
        {
            if (m_LocalTransformChanged)
            {
                ResetLocalPositionAndScale();
                m_LocalTransformChanged = false;
            }
        }
        
        private void SetLocalPositionAndScale()
        {
            m_LocalTransformChanged = true;
            
            m_StartLocalPos = transform.localPosition;
            m_StartLocalScale = transform.localScale;
        }
        
        private void ResetLocalPositionAndScale()
        {
            transform.localPosition = m_StartLocalPos;
            transform.localScale = m_StartLocalScale;
        }
        
        private void HandleOnLevelCreated()
        {
            SetLocalPositionAndScale();
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
}