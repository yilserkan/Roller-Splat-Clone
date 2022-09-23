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
        
        private static Vector2Int NOT_FOUND = new Vector2Int(-1, -1);
        
        private Neighbors m_Neighbors;
        private Neighbors m_PrevNeighbor;
        private bool isInitialized = false;
        private bool prevInitialized = false;
        private bool startTile = true;
        private int cycleIndex = -1;
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
        
        public Vector2Int GenerateRandomLevel()
        {
            int randomX = Random.Range(xTileStartIndex, xTileEndIndex);
            int randomY = Random.Range(yTileStartIndex, yTileEndIndex);
            
            Vector2Int startCoordinates = new Vector2Int(randomX, randomY);
            Vector2Int startPos = startCoordinates;

            //Debug.Log(startCoordinates);
            m_Grid[startCoordinates].IsBlocked = false;
            
            int cycle = 100;

            for (int i = 0; i < cycle; i++)
            {
                Debug.Log("------Cycle Index ; " + i );
                cycleIndex = i;
                Debug.Log("------Start Coords ; " + startCoordinates );
                m_Neighbors = GetRandomDirection(startCoordinates);
                Debug.Log("Trying Dir "+ m_Neighbors);
                Vector2Int newCoords = NOT_FOUND;
                
                newCoords = UnblockTile(startCoordinates, m_Neighbors);
                if (newCoords == NOT_FOUND)
                {
                    m_Neighbors = FindOppositeDir(m_Neighbors);
                    
                    Debug.Log("Trying Dir "+ m_Neighbors);
                    newCoords = UnblockTile(startCoordinates,m_Neighbors);
                    
                    if (newCoords == NOT_FOUND)
                    {
                        if (prevInitialized)
                        {
                            m_Neighbors = m_PrevNeighbor;
                            DisableControlBlock(startCoordinates,m_Neighbors);
                            newCoords = UnblockTile(startCoordinates,m_Neighbors);
                        }
                        else
                        {
                            m_Neighbors = GetRandomDirFromOtherAxis(m_Neighbors);
                            DisableControlBlock(startCoordinates,m_Neighbors);
                            newCoords = UnblockTile(startCoordinates,m_Neighbors);
                        }
                        
                        Debug.Log("*********************************Trying Dir  "+ m_Neighbors);
                    }
                }
                
                prevInitialized = true;
                m_PrevNeighbor = m_Neighbors;
                if (newCoords == NOT_FOUND)
                {
                    break;
                }
                startCoordinates = newCoords;
                Debug.Log( "--------- Found Dir ; " + m_Neighbors );
                Debug.Log("Unblocking Tile " + startCoordinates );
                Debug.Log("---------------" + i);
            }
            
            return startPos;
        }      
        public Vector2Int UnblockTile(Vector2Int tileCoordinates,Neighbors dir)
        {
            List<Tile> possibleEndPoints = new List<Tile>();
            possibleEndPoints = FindPossibleEndPoints(tileCoordinates, dir);

            Vector2Int curTile = UnblockPath(tileCoordinates, dir, possibleEndPoints);
            
            return curTile;
        }

        private Vector2Int UnblockPath(Vector2Int tileCoordinates, Neighbors dir, List<Tile> possibleEndPoints)
        {
            if (possibleEndPoints.Count <= 0)
            {
                return NOT_FOUND;
            }
            
            Tile curTile = m_Grid[tileCoordinates];

            int randomNumber = Random.Range(0, possibleEndPoints.Count);
            Tile randomTile = possibleEndPoints[randomNumber];

            List<Tile> path = new List<Tile>();
            bool pathContainsBlockedTile = false;
            path.Add(curTile);
            if (curTile.IsBlocked)
            {
                pathContainsBlockedTile = true;
            }
  
            while (curTile != randomTile)
            {
                if (IsNeigborControlBlock(curTile.Coordinates, dir))
                {
                    break;
                }
                
                
                curTile = m_Grid[curTile.Coordinates].Neigbors[dir];
                path.Add(curTile);
                if (curTile.IsBlocked)
                {
                    pathContainsBlockedTile = true;
                }
            }

            if (!pathContainsBlockedTile)
            {
                return NOT_FOUND;
            }

            foreach (var tile in path)
            {
                tile.IsBlocked = false;
            }
            
            ActivateControlBlock(tileCoordinates,curTile.Coordinates,dir);
            return  curTile.Coordinates;
            
        }

        private List<Tile> FindPossibleEndPoints(Vector2Int tileCoordinates, Neighbors dir)
        {
            Tile curTile = m_Grid[tileCoordinates];
            int maxLength = GetMaxLength(curTile.Coordinates,dir);

            List<Tile> possibleEndPoints = new List<Tile>();

            for (int i = 0; i < maxLength; i++)
            {
                if  (m_Grid[curTile.Coordinates].Neigbors[dir] == null || IsControlBlock(curTile.Coordinates))
                {
                    break;
                }
                
                if (IsNeigborBlocked(curTile.Coordinates, dir) && tileCoordinates != curTile.Coordinates)
                {
                    possibleEndPoints.Add(curTile);
                }
                
                curTile = m_Grid[curTile.Coordinates].Neigbors[dir];
            }
            
            return possibleEndPoints;
        }
        

        private void ActivateControlBlock(Vector2Int startCoords,Vector2Int endCoords, Neighbors dir)
        {
            if (m_Grid[endCoords].Neigbors[dir].IsBlocked)
            {
                m_Grid[endCoords].Neigbors[dir].SetControlIndex(cycleIndex);
                m_Grid[endCoords].Neigbors[dir].IsControlBlock = true;
            }

            
            Neighbors nb = FindOppositeDir(dir);
            m_Grid[startCoords].Neigbors[nb].IsControlBlock = true;
            m_Grid[startCoords].Neigbors[nb].SetControlIndex(cycleIndex);
            
        }

        private void DisableControlBlock(Vector2Int startCoords, Neighbors dir)
        {
            Debug.Log("*********************************Coords "+ m_Neighbors + "Dir " + dir + " cycle Index " + cycleIndex);

            if (m_Grid[startCoords].Neigbors[dir].IsControlSetIndex == cycleIndex-1)
            {
                Debug.Log("*********************************Disable Control Block "+ m_Neighbors);
                m_Grid[startCoords].Neigbors[dir].IsControlBlock = false;
            }
            
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
                    length = coordinates.y;
                    break;
                case Neighbors.Right:
                    length = (xTileEndIndex + 1) - coordinates.x;
                    break;
                case Neighbors.Left:
                    length = coordinates.x;
                    break;
            }
            return length;
        }
        
        public Neighbors GetRandomDirection(Vector2Int coords)
        {
            if (!isInitialized)
            {
                int directionCount = Enum.GetNames(typeof(Neighbors)).Length;
                int randomDirection = Random.Range(0, directionCount);
                Neighbors randomNeighbor = (Neighbors)randomDirection;
                isInitialized = true;
                return randomNeighbor;
            }
            
            if (m_Neighbors == Neighbors.Down || m_Neighbors == Neighbors.Up)
            {
                return GetRandomDirFromOtherAxis(Neighbors.Up);
            }
       
            return GetRandomDirFromOtherAxis(Neighbors.Left);
        }
        
        private Neighbors FindOppositeDir(Neighbors dir)
        {
            Neighbors nb = Neighbors.Down;
            switch (dir)
            {
                case Neighbors.Up:
                    nb = Neighbors.Down;
                    break;
                case Neighbors.Down:
                    nb =Neighbors.Up;
                    break;
                case Neighbors.Left:
                    nb = Neighbors.Right;
                    break;
                case Neighbors.Right:
                    nb = Neighbors.Left;
                    break;
            }

            return nb;
        }
        
        private Neighbors GetRandomDirFromOtherAxis(Neighbors dir)
        {
            Neighbors nb;
            if (dir == Neighbors.Up || dir == Neighbors.Down)
            {     
                int ran = Random.Range(2, 4);
                nb = (Neighbors)ran;
            }
            else 
            {
                int ran = Random.Range(0, 2);
                nb = (Neighbors)ran;
            }

            return nb;
        }
        
        public bool IsEdge(Vector2Int coord)
        {
            if (coord.x != xTileStartIndex && coord.x != xTileEndIndex && coord.y != yTileStartIndex && coord.y != yTileEndIndex)
            {
                return false;
            }

            return true;
        }

        public bool IsBlocked(Vector2Int coordinates)
        {
            return m_Grid[coordinates].IsBlocked;
        }

        public bool IsNeigborBlocked(Vector2Int coordinates, Neighbors dir)
        {
            return m_Grid[coordinates].Neigbors[dir].IsBlocked;
        }
        
        public bool IsControlBlock(Vector2Int coordinates)
        {
            return m_Grid[coordinates].IsControlBlock;
        }
        
        public bool IsNeigborControlBlock(Vector2Int coordinates, Neighbors dir)
        {
            return m_Grid[coordinates].Neigbors[dir].IsControlBlock;
        }
    }
}
