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
        public bool IsEdge(Vector2Int coord)
        {
            if (coord.x != xTileStartIndex && coord.x != xTileEndIndex && coord.y != yTileStartIndex && coord.y != yTileEndIndex)
            {
                return false;
            }

            return true;
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
        
        public Vector2Int GenerateRandomLevel()
        {
            int randomX = Random.Range(xTileStartIndex, xTileEndIndex);
            int randomY = Random.Range(yTileStartIndex, yTileEndIndex);
            
            Vector2Int startCoordinates = new Vector2Int(randomX, randomY);
            Vector2Int startPos = startCoordinates;
            
            //Debug.Log(startCoordinates);
            m_Grid[startCoordinates].IsBlocked = false;
            
            int cycle = 250;

            for (int i = 0; i < cycle; i++)
            {
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
                            newCoords = UnblockTile(startCoordinates,m_Neighbors);
                        }
                        else
                        {
                            m_Neighbors = GetRandomDirFromOtherAxis(m_Neighbors);
                            newCoords = UnblockTile(startCoordinates,m_Neighbors);
                        }
                        
                        Debug.Log("Trying Dir "+ m_Neighbors);
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
                Debug.Log("---------------" + i);
            }

            return startPos;
        }      
        public Vector2Int UnblockTile(Vector2Int tileCoordinates,Neighbors dir)
        {
            Debug.Log("Unblocking Tile " + tileCoordinates );
            Tile curTile = m_Grid[tileCoordinates];
            int maxLength = GetMaxLength(curTile.Coordinates,dir);
            List<Tile> path = new List<Tile>();
            List<Tile> possibleEndPoints = new List<Tile>();

            for (int i = 0; i < maxLength; i++)
            {
                if (m_Grid[curTile.Coordinates].IsControlBlock)
                {
                    break;
                }
                
                path.Add(curTile);
                
                if (m_Grid[curTile.Coordinates].Neigbors[dir].IsBlocked && tileCoordinates != curTile.Coordinates)
                {
                    possibleEndPoints.Add(curTile);
                }

                if (m_Grid[curTile.Coordinates].Neigbors[dir] == null)
                {
                    break;
                }
                
                curTile = m_Grid[curTile.Coordinates].Neigbors[dir];
               
            }

            if (possibleEndPoints.Count > 0)
            {
                int randomNumber = Random.Range(0, possibleEndPoints.Count);
                Tile randomTile = possibleEndPoints[randomNumber];
                
                curTile = m_Grid[tileCoordinates];
                
                while (curTile != randomTile)
                {/*
                    if (startTile)
                    {
                        Neighbors randDir = GetRandomDirection(curTile.Coordinates);
                        m_Grid[curTile.Coordinates].Neigbors[randDir].IsControlBlock = true;
                    }
                    */
                    curTile.IsBlocked = false;
                 
                    
                    if (curTile.Neigbors[dir].IsControlBlock)
                    {
                        break;
                    }
                    
                    curTile = m_Grid[curTile.Coordinates].Neigbors[dir];
                   /*
                    if (m_Grid[curTile.Coordinates].Neigbors[dir].IsControlBlock)
                    {
                        break;
                    }
                    */
                    //Debug.Log("Tile" + curTile);
                  
                }
                m_Grid[curTile.Coordinates].Neigbors[dir].IsControlBlock = true;
                ToggleControlBlock(tileCoordinates,dir);
                return  curTile.Coordinates;
            }

            return NOT_FOUND;

        }
        /*
        public Vector2Int UnblockTile(Vector2Int tileCoordinates,Neighbors dir)
        {
            ToggleControlBlock(tileCoordinates,dir);
            //Tile curTile = m_Grid[tileCoordinates].Neigbors[dir];
            Tile curTile = m_Grid[tileCoordinates];
            int maxLength = GetMaxLength(curTile.Coordinates,dir);
            int length = Random.Range(maxLength/2, maxLength );
            if (length == 1)
            {
                length = 1;
            }
            Debug.Log("length " + length + " Dir " + dir);

            for (int i = 0; i < length; i++)
            {
                if (m_Grid[curTile.Coordinates].Neigbors[dir].IsControlBlock)
                {
                     m_Grid[curTile.Coordinates].Neigbors[dir].IsControlBlock = true;
                     return  curTile.Coordinates;
                }
                Debug.Log("Coloring Tile :" + curTile);
                curTile.IsBlocked = false;
                
                if (m_Grid[curTile.Coordinates].Neigbors[dir] == null)
                {
                    m_Grid[curTile.Coordinates].Neigbors[dir].IsControlBlock = true;
                    return  curTile.Coordinates;
                }
                
                curTile = m_Grid[curTile.Coordinates].Neigbors[dir];
            }
            Debug.Log("----------------------------");
            m_Grid[curTile.Coordinates].Neigbors[dir].IsControlBlock = true;
            return  curTile.Coordinates;
        }

        public Vector2Int UnblockTile(Vector2Int tileCoordinates,Neighbors dir)
        {
            ToggleControlBlock(tileCoordinates,dir);
            //Tile curTile = m_Grid[tileCoordinates].Neigbors[dir];
            Tile curTile = m_Grid[tileCoordinates];
            int maxLength = GetMaxLength(curTile.Coordinates,dir);
            int length = Random.Range(maxLength/2, maxLength );
            if (length == 1)
            {
                length = 1;
            }
            Debug.Log("length " + length + " Dir " + dir);

            List<Tile> path = new List<Tile>();

            for (int i = 0; i < length; i++)
            {
                if (m_Grid[curTile.Coordinates].Neigbors[dir].IsControlBlock)
                {
                    m_Grid[curTile.Coordinates].Neigbors[dir].IsControlBlock = true;
                    break;
                }
                path.Add(curTile);
                
                if (m_Grid[curTile.Coordinates].Neigbors[dir] == null)
                {
                    m_Grid[curTile.Coordinates].Neigbors[dir].IsControlBlock = true;
                    break;
                }
                
                curTile = m_Grid[curTile.Coordinates].Neigbors[dir];
            }

            int pathCount = path.Count;

            if ( pathCount > 0 && !path[pathCount-1].IsBlocked)
            {
                while(m_Grid[path[pathCount-1].Coordinates].Neigbors[dir] != null)
                {
                    path.Add(m_Grid[path[pathCount-1].Coordinates].Neigbors[dir]);
                    if (m_Grid[path[pathCount-1].Coordinates].Neigbors[dir].IsBlocked)
                    {
                        break;
                    }
                  
                }

                if (!path[pathCount-1].IsBlocked)
                {
                    return tileCoordinates;
                }
            }

            foreach (var tile in path)
            {
                tile.IsBlocked = false;
            }
            
            Debug.Log("----------------------------");
            m_Grid[curTile.Coordinates].Neigbors[dir].IsControlBlock = true;
            return  curTile.Coordinates;
        }
        */

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
        
        private void ToggleControlBlock(Vector2Int coords, Neighbors dir)
        {
            Neighbors nb;
            if (dir == Neighbors.Up)
            {
                nb = Neighbors.Down;
            }
            else if (dir == Neighbors.Down)
            {
                nb = Neighbors.Up;
            }
            else if (dir == Neighbors.Right)
            {
                nb = Neighbors.Left;
            }
            else 
            {
                nb = Neighbors.Right;
            }

            m_Grid[coords].Neigbors[nb].IsControlBlock = true;
            
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
            else
            {
                if (m_Neighbors == Neighbors.Down || m_Neighbors == Neighbors.Up)
                {
                    int ran = Random.Range(2, 4);
                    Neighbors randomNeighbor = (Neighbors)ran;
                    if (m_Grid[coords].Neigbors[randomNeighbor] == null)
                    {
                        if (randomNeighbor == Neighbors.Right)
                        {
                            return Neighbors.Left;
                        }
                    
                        return Neighbors.Right;
                      
                    }
                    return randomNeighbor;
                }
                else
                {
                    int ran = Random.Range(0, 2);
                    Neighbors randomNeighbor = (Neighbors)ran;
                    if (m_Grid[coords].Neigbors[randomNeighbor] == null)
                    {
                        if (randomNeighbor == Neighbors.Up)
                        {
                            return Neighbors.Down;
                        }
                    
                        return Neighbors.Up;
                      
                    }
                    return randomNeighbor;
                }
                
            }
        
        }  
        
      /*
        public List<Neighbors> GetRandomDirection(Vector2Int coords)
        {
            List<Neighbors> possibleDirs = new List<Neighbors>();

            int directionCount = Enum.GetNames(typeof(Neighbors)).Length;
            for (int i = 0; i < directionCount; i++)
            {
                Neighbors randomNeighbor = (Neighbors)i;
                if (!m_Grid[coords].Neigbors[randomNeighbor].IsControlBlock && !IsEdge(m_Grid[coords].Neigbors[randomNeighbor].Coordinates))
                {
                    if (isInitialized && m_Neighbors == FindOppositeDir(randomNeighbor))
                    {
                        continue;
                    }
                    possibleDirs.Add(randomNeighbor);
                }
            }

            isInitialized = true;

            return possibleDirs;
        }
       */
    }
}
