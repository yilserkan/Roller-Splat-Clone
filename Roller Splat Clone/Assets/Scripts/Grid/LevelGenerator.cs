using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GridSystem
{
    public class LevelGenerator
    {
        private int m_Width;
        private int m_Height;

        private int xTileStartIndex;
        private int xTileEndIndex;
        private int yTileStartIndex;
        private int yTileEndIndex;

        public bool IsEdge(Vector2Int coord)
        {
            if (coord.x != xTileStartIndex && coord.x != xTileEndIndex && coord.y != yTileStartIndex && coord.y != yTileEndIndex)
            {
                return true;
            }

            return false;
        }
        
        private Dictionary<Vector2Int, Tile> m_Grid;

        public LevelGenerator(int width, int height,Dictionary<Vector2Int, Tile> grid)
        {
            m_Height = height;
            m_Width = width;

            m_Grid = grid;

            xTileStartIndex = 1;
            yTileStartIndex = 1;

            xTileEndIndex = width - 2;
            yTileEndIndex = height - 2;
        }
        
        public void GenerateRandomLevel()
        {
            int randomX = Random.Range(xTileStartIndex, xTileEndIndex);
            int randomY = Random.Range(yTileStartIndex, yTileEndIndex);
            

            
            Vector2Int startCoordinates = new Vector2Int(randomX, randomY);
            Debug.Log(startCoordinates);
            m_Grid[startCoordinates].IsBlocked = false;
            
            int cycle = 2;

            for (int i = 0; i < cycle; i++)
            {
                startCoordinates = UnblockTile(startCoordinates, GetRandomDirection());
            }
        }
        public Vector2Int UnblockTile(Vector2Int tileCoordinates,Neighbors dir)
        {
            Tile curTile = m_Grid[tileCoordinates].Neigbors[dir];

            int length = Random.Range(0, GetMaxLength(curTile.Coordinates, dir));
            Debug.Log("length " + GetMaxLength(curTile.Coordinates, dir) + " Dir " + dir);

            for (int i = 0; i < length; i++)
            {
                Debug.Log("Coloring");
                curTile.IsBlocked = false;
                
                if (m_Grid[curTile.Coordinates].Neigbors[dir] == null)
                {
                    return  curTile.Coordinates;
                }
                
                curTile = m_Grid[curTile.Coordinates].Neigbors[dir];
            }
            Debug.Log("----------------------------");
            return  curTile.Coordinates;
        }

        private int GetMaxLength(Vector2Int coordinates, Neighbors dir)
        {
            int length = 0;
            switch (dir)
            {
                case Neighbors.Up:
                    length = (yTileEndIndex + 1) - coordinates.y;
                    break;
                case Neighbors.Down:
                    length = coordinates.y - 1;
                    break;
                case Neighbors.Right:
                    length = (xTileEndIndex + 1) - coordinates.x;
                    break;
                case Neighbors.Left:
                    length = coordinates.x - 1;
                    break;
            }
            return length;
        }
        
  
        
        public Neighbors GetRandomDirection()
        {
            int directionCount = Enum.GetNames(typeof(Neighbors)).Length;
            int randomDirection = Random.Range(0, directionCount);
            Neighbors randomNeighbor = (Neighbors)randomDirection;

            return randomNeighbor;
        }
    }
}
