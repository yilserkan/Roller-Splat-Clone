using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GridSystem
{
    public class TestGrid : MonoBehaviour
    {
        [SerializeField] private int heigth;
        [SerializeField] private int width;
        [SerializeField] private int cellSize;
        [SerializeField] private Vector3 gridStartPos;
        [SerializeField] private GameObject prefab;
        
        private Grid m_Grid;
        
        private void Start()
        {
            m_Grid = new Grid(heigth, width, cellSize, gridStartPos);
            CreateGrid();
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
