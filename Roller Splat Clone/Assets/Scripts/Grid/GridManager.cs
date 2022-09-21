using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

namespace GridSystem
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private int heigth;
        [SerializeField] private int width;
        [SerializeField] private int cellSize;
        [SerializeField] private Vector3 gridStartPos;
        [SerializeField] private GameObject prefab;
        
        private Grid m_Grid;

        public static event Action<List<Vector3>> OnFoundPlayerPath;
        
        private void OnEnable()
        {
            PlayerStateMachine.OnPlayerEnterMoveState += HandleOnPlayerEnterMoveState;
        }

        private void OnDisable()
        {
            PlayerStateMachine.OnPlayerEnterMoveState -= HandleOnPlayerEnterMoveState;
        }

        private void HandleOnPlayerEnterMoveState(Vector3 worldPos, Vector2Int dir)
        {
            List<Vector3> path = m_Grid.FindPlayerPath(m_Grid.GetCoordinatesFromWorldPos(worldPos) , dir);
            OnFoundPlayerPath?.Invoke(path);
        }

        private void Start()
        {
            m_Grid = new Grid(heigth, width, cellSize, gridStartPos);
            CreateGrid();
            Debug.Log(m_Grid.GetWorldPosFromCoordinates(new Vector2Int(1, 1)));
            m_Grid.FindNeighbors();
        }
        
        private void CreateGrid()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < heigth; j++)
                {
                    Vector2Int coordinates = new Vector2Int(i, j);
                    Instantiate(prefab, m_Grid.GetWorldPosFromCoordinates(coordinates), Quaternion.identity);
                }
            }
        }
        
    }
}
