using System;
using System.Collections.Generic;
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

    public class Tile : MonoBehaviour

    {
        public Vector2Int Coordinates;
        public Vector3 WorldPosition;
        public bool IsBlocked;
        public bool IsColored;
        public Dictionary<Neighbors, Tile> Neigbors;

        private MeshRenderer m_MeshRenderer;
        
        public void Init(Vector2Int coordinates, Vector3 worldPos)
        {
            Coordinates = coordinates;
            WorldPosition = worldPos;
            m_MeshRenderer = GetComponent<MeshRenderer>();
            IsBlocked = false;
            IsColored = false;
            Neigbors = new Dictionary<Neighbors, Tile>()
            {
                { Neighbors.Up, null },
                { Neighbors.Down, null },
                { Neighbors.Left, null },
                { Neighbors.Right, null }
            };
        }

        public void ColorTile(Color color)
        {
            m_MeshRenderer.material.color = color;
        }

        private void Update()
        {
            if (IsBlocked)
            {
                m_MeshRenderer.material.color = Color.red;
            }
            else
            {
                m_MeshRenderer.material.color = Color.green;
            }
        }
    }
}