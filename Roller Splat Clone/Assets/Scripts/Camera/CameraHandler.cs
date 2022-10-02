using LevelSystem;
using UnityEngine;

namespace CameraSystem
{
    public class CameraHandler : MonoBehaviour
    { 
        [SerializeField] private Transform cameraParent;

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
            float xPosition = 0, yPosition = 0, zPosition = 0 ;
            FindNewPosition(ref xPosition, ref yPosition, ref zPosition);
            
            SetCameraRotation(yPosition, ref zPosition);

            Vector3 newPos = new Vector3(xPosition, yPosition, zPosition);
            Quaternion newRot = Quaternion.Euler(cameraLookAngle, 0,0);
            cameraParent.SetPositionAndRotation(newPos,newRot);
        }

        private void FindNewPosition(ref float xPosition, ref float yPosition, ref float zPosition )
        {
            xPosition = (float)m_Width / 2;
            xPosition -= .5f;
            
            zPosition = (float)m_Height / 2;
            zPosition -= .5f;

            if (m_Width >= m_Height)
            {
                yPosition = m_Width * 2;
            }
            else
            {
                yPosition = m_Height * 1.25f;
            }
            
        }

        private void SetCameraRotation(float yPosition, ref float zPosition)
        {
            float angle = (m_DefaultAngle - cameraLookAngle);
            float angleInRad = angle * Mathf.Deg2Rad;
            float tan = Mathf.Tan(angleInRad);
            
            float zSubstractValue = tan * yPosition;
            
            zPosition -= zSubstractValue;
        }
        
        private void HandleOnGenerateLevel(Level level)
        {
            m_Height = level.Height;
            m_Width = level.Width;

            SetCameraPosition();
        }
        
        private void AddListeners()
        {
            LevelManager.OnGenerateLevel += HandleOnGenerateLevel;
            LevelGeneratorManager.OnGenerateLevel += HandleOnGenerateLevel;
        }
        
        private void RemoveListeners()
        {
            LevelManager.OnGenerateLevel -= HandleOnGenerateLevel;
            LevelGeneratorManager.OnGenerateLevel -= HandleOnGenerateLevel;
        }
    }
}