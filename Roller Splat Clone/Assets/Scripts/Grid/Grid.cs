using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GridSystem
{
    
    public class Grid
    {
        private int m_Height;
        private int m_Width;
        private int m_Cellsize;
        private Vector3 m_GridStartPosition;

        private Dictionary<Vector2Int, Tile> m_Grid = new Dictionary<Vector2Int, Tile>();

        private Dictionary<Neighbors, Vector2Int> m_Directions = new Dictionary<Neighbors, Vector2Int>()
        {
            { Neighbors.Up, new Vector2Int(0, 1) },
            { Neighbors.Down, new Vector2Int(0, -1) },
            { Neighbors.Right, new Vector2Int(1, 0) },
            { Neighbors.Left, new Vector2Int(-1, 0) }
        };

        public Grid(int height, int width, int cellsize, Vector3 gridStartPos)
        {
            m_Height = height;
            m_Width = width;
            m_Cellsize = cellsize;
            m_GridStartPosition = gridStartPos;
            
            for (int x = 0; x < m_Width; x++)
            {
                for (int y = 0; y < m_Height; y++)
                {
                    Vector2Int coordinates = new Vector2Int(x, y);
                    m_Grid.Add(coordinates, new Tile(new Vector2Int(x,y)));
                }
            }
        }

        public Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(x,y) * m_Cellsize;
        }

        public Vector3 GetWorldPosFromCoordinates(Vector2Int coordinates)
        {
            float posX = m_GridStartPosition.x + coordinates.x * m_Cellsize;
            float posY = m_GridStartPosition.z + coordinates.y * m_Cellsize;
            
            return new Vector3(posX, 0, posY);
        }

        public Vector2Int GetCoordinatesFromWorldPos(Vector3 worldPosition)
        {
            int coordinateX = Mathf.RoundToInt((worldPosition.x - m_GridStartPosition.x )/ m_Cellsize);
            int coordinateY = Mathf.RoundToInt((worldPosition.z -m_GridStartPosition.z)/ m_Cellsize);

            Vector2Int coordinate = new Vector2Int(coordinateX, coordinateY);
            
            if (m_Grid.ContainsKey(coordinate))
            {
                return coordinate;
            }

            return new Vector2Int(0, 0);
        }

        public Tile GetTile(Vector2Int coordinates)
        {
            if (m_Grid.ContainsKey(coordinates))
            {
                return m_Grid[coordinates];
            }

            return null;
        }   
        
        public void FindNeighbors()
        {
            for (int x = 0; x < m_Width; x++)
            {
                for (int y = 0; y < m_Height; y++)
                {
                    Vector2Int coordinates = new Vector2Int(x, y);
                
                    AddNeighbors(coordinates);
                }
            }

            //PrintNeighbors();
        }

        private void AddNeighbors(Vector2Int coordinates)
        {
            foreach (var direction in m_Directions)
            {
                Vector2Int neighborCoordinates = coordinates + direction.Value;
                if (m_Grid.ContainsKey(neighborCoordinates))
                {
                    m_Grid[coordinates].Neigbors[direction.Key] = m_Grid[neighborCoordinates];
                }
            }
        }

        private void PrintNeighbors()
        {
            foreach (var tile in m_Grid)
            {
                Debug.Log($"Tile : {tile.Value.Coordinates}");
                foreach (var neigbor in tile.Value.Neigbors)
                {
                    if (neigbor.Value != null)
                    {
                        Debug.Log ($"Neighbor : {neigbor.Value.Coordinates}");
                    }
                   
                }
                
                Debug.Log("----------------");
            }
        }

        public List<Vector2Int> FindPlayerPath(Vector2Int startCoordinate, Vector2Int moveDir)
        {
            List<Vector2Int> path = new List<Vector2Int>();
            var dir = m_Directions.FirstOrDefault(x => x.Value == moveDir).Key;

            Vector2Int currentCoordinate = startCoordinate;
            
            while (m_Grid[currentCoordinate].Neigbors[dir] != null && !m_Grid[currentCoordinate].IsBlocked)
            {
                Debug.Log($"Way : {currentCoordinate}");
                currentCoordinate = m_Grid[currentCoordinate].Neigbors[dir].Coordinates;
                path.Add(currentCoordinate);
                Debug.Log($"--------------");
              
            }
            Debug.Log($"Way : {currentCoordinate}");
            return path;
        }
    }

}
