using System;
using System.Collections.Generic;
using ObjectPool;
using UnityEngine;

namespace GridSystem
{
    
    public enum Neighbors
    {
        Up,
        Down,
        Left,
        Right
    }

    public class Tile : AbstractObjectPoolObject<Tile>

    {
        public Vector2Int Coordinates;
        public Vector3 WorldPosition;
        public bool IsBlocked;
        public bool IsControlBlock;
        public bool IsColored;
        public int IsControlSetIndex;
        public Dictionary<Neighbors, Tile> Neigbors;

        private MeshRenderer m_MeshRenderer;
        
        public void Init(Vector2Int coordinates, Vector3 worldPos)
        {
            Coordinates = coordinates;
            WorldPosition = worldPos;
            m_MeshRenderer = GetComponent<MeshRenderer>();
            IsBlocked = false;
            IsColored = false;
            IsControlSetIndex = -1;
            Neigbors = new Dictionary<Neighbors, Tile>()
            {
                { Neighbors.Up, null },
                { Neighbors.Down, null },
                { Neighbors.Left, null },
                { Neighbors.Right, null }
            };
        }

        public void SetControlIndex(int index)
        {
            if (IsControlSetIndex == -1)
            {
                IsControlSetIndex = index;
            }
        }

        public void ColorTile(Color color)
        {
            m_MeshRenderer.material.color = color;
        }

        private void Update()
        {
            if (IsControlBlock)
            {
                //m_MeshRenderer.material.color = Color.black;
            }
            if (IsBlocked)
            {
                m_MeshRenderer.material.color = Color.red;
            }
        }
    }
}