using System.Collections;
using System.Collections.Generic;
using Effects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CameraSystem
{
    public class CameraEffects : MonoBehaviour
    {
        [SerializeField] private List<ParticleSystem> confettiParticles;
        
        [SerializeField] private float shakeMagnitude;
        [SerializeField] private float shakeDuration;
        
        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }
        
        IEnumerator ShakeCamera()
        {
            Vector3 startLocalPos = transform.localPosition;
            
            float elapsedTime = 0;
            while (elapsedTime < shakeDuration)
            {
                float randomX = Random.Range(-1, 1) * shakeMagnitude;
                float randomY = Random.Range(-1, 1) * shakeMagnitude;

                Vector3 newLocalPos = new Vector3(randomX, randomY, startLocalPos.z);

                transform.localPosition = newLocalPos;
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = startLocalPos;
        }

        private void PlayConfetti()
        {
            for (int i = 0; i < confettiParticles.Count; i++)
            {
                confettiParticles[i].gameObject.SetActive(true);
                confettiParticles[i].Play();
            }
        }

        private void DisableConfettiParticles()
        {
            for (int i = 0; i < confettiParticles.Count; i++)
            {
                confettiParticles[i].gameObject.SetActive(false);
            }
        }
        
        private void HandleOnLevelFinished()
        {
            StartCoroutine(ShakeCamera());
            PlayConfetti();
        }
        
        private void HandleOnLoadNextLevel()
        { 
            DisableConfettiParticles();
        }

        private void AddListeners()
        {
            GridSystem.Grid.OnLevelFinished += HandleOnLevelFinished;
            LevelFinishEffects.OnLoadNextLevel += HandleOnLoadNextLevel;
        }
        
        private void RemoveListeners()
        {
            GridSystem.Grid.OnLevelFinished -= HandleOnLevelFinished;
            LevelFinishEffects.OnLoadNextLevel -= HandleOnLoadNextLevel;
        }
    }
}