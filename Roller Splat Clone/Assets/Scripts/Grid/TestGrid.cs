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

        private Grid m_Grid;
        
        private void Start()
        {
            m_Grid = new Grid(heigth, width, cellSize);
            Debug.Log(m_Grid.GetWorldPosFromCoordinates(new Vector2Int( 1,1)));
        }
    }
}
