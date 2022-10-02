using CameraSystem;
using UnityEngine;
using Grid = GridSystem.Grid;

namespace LevelSystem
{
    public class LevelBackground : MonoBehaviour
    {
        [SerializeField] private Material backgroundMaterial;

        private float m_AspectRatio = 9f / 16f;
        
        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        private void ChangeXZPositionToCameraPos(Vector3 camPosition)
        {
            transform.position = new Vector3(camPosition.x, transform.position.y, camPosition.z);
        }
        
        private void CalculateNewBackgroundSize(Vector3 camPosition)
        {
            float cameraHeightInScale = (camPosition.y / 10);
            float yScale = cameraHeightInScale + (cameraHeightInScale * 25 / 100);
            float xScale = yScale * m_AspectRatio;

            transform.localScale = new Vector3(xScale * 2, transform.localScale.y, yScale * 2);
        }

        private void HandleOnBackgroundColorSet(Color color)
        {
            backgroundMaterial.color = color;
        }
        
        private void HandleOnCameraPositionFound(Vector3 camPosition)
        {
            ChangeXZPositionToCameraPos(camPosition);
            CalculateNewBackgroundSize(camPosition);
        }
        
        private void AddListeners()
        {
            Grid.OnBackgorundColorSet += HandleOnBackgroundColorSet;
            CameraHandler.OnCameraPositionFound += HandleOnCameraPositionFound;
        }
        
        private void RemoveListeners()
        {
            Grid.OnBackgorundColorSet -= HandleOnBackgroundColorSet;
            CameraHandler.OnCameraPositionFound -= HandleOnCameraPositionFound;
        }
    }
}