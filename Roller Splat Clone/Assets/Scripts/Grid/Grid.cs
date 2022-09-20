using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GridSystem
{
    public class Grid
    {
        private int m_Height;
        private int m_Width;
        private int m_Cellsize;

        private Dictionary<Vector2Int, Tile> m_Grid = new Dictionary<Vector2Int, Tile>();

        public Grid(int height, int width, int cellsize)
        {
            m_Height = height;
            m_Width = width;
            m_Cellsize = cellsize;

            for (int i = 0; i < m_Width; i++)
            {
                for (int h = 0; h < m_Height; h++)
                {
                    Vector2Int coordinates = new Vector2Int(i, h);
                    m_Grid.Add(coordinates, new Tile());
                }
            }
        }

        public Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(x,y) * m_Cellsize;
        }

        public Vector3 GetWorldPosFromCoordinates(Vector2Int coordinates)
        {
            float posX = coordinates.x * m_Cellsize;
            float posY = coordinates.y * m_Cellsize;
            
            return new Vector3(posX, 0, posY);
        }

        public Vector2Int GetCoordinatesFromWorldPos(Vector3 worldPosition)
        {
            int coordinateX = Mathf.RoundToInt(worldPosition.x / m_Cellsize);
            int coordinateY = Mathf.RoundToInt(worldPosition.z / m_Cellsize);

            return new Vector2Int(coordinateX, coordinateY);
        }

        public Tile GetTile(Vector2Int coordinates)
        {
            return m_Grid[coordinates];
        }
    }

}
