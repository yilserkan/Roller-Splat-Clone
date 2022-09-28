using System;
using System.Collections;
using System.Collections.Generic;
using LevelSystem;
using UnityEngine;
using Grid = GridSystem.Grid;
using Random = UnityEngine.Random;

namespace CameraSystem
{
    public class CameraHandler : MonoBehaviour
    {
        [SerializeField] private Transform cameraParent;
        [SerializeField] private float shakeMagnitude;
        [SerializeField] private float shakeDuration;
        
        [Range(60, 90)] 
        [SerializeField] private float cameraLookAngle;

        private float m_DefaultAngle = 90f;
        
        private int m_Height;
        private int m_Width;
        
        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        private void Start()
        {
            Application.targetFrameRate = 60;
            Camera.main.aspect = 9f/16f;
        }

        private void SetCameraPosition()
        {
            float xPosition = (float)m_Width / 2;
            xPosition -= .5f;
            
            float zPosition = (float)m_Height / 2;
            zPosition -= .5f;

            float yPosition = m_Width * 2;
            
            SetCameraRotation(yPosition, ref zPosition);

            Vector3 newPos = new Vector3(xPosition, yPosition, zPosition);
            Quaternion newRot = Quaternion.Euler(cameraLookAngle, 0,0);
            
            cameraParent.SetPositionAndRotation(newPos,newRot);
            
        }

        private void SetCameraRotation(float yPosition, ref float zPosition)
        {
            float angle = (m_DefaultAngle - cameraLookAngle);
            float angleInRad = angle * Mathf.Deg2Rad;
            float tan = Mathf.Tan(angleInRad);
            
            float zSubstractValue = tan * yPosition;
            
            zPosition -= zSubstractValue;
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
        
        
        private void HandleOnGenerateLevel(Level level)
        {
            m_Height = level.Height;
            m_Width = level.Width;

            SetCameraPosition();
        }

        private void HandleOnLevelFinished()
        {
            StartCoroutine(ShakeCamera());
        }
        
        private void AddListeners()
        {
            LevelManager.OnGenerateLevel += HandleOnGenerateLevel;
            Grid.OnLevelFinished += HandleOnLevelFinished;
        }
        
        private void RemoveListeners()
        {
            LevelManager.OnGenerateLevel -= HandleOnGenerateLevel;
            Grid.OnLevelFinished -= HandleOnLevelFinished;
        }

        
    }
}