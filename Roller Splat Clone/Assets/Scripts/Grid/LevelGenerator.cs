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
        private int m_Cycles;

        private int m_XTileStartIndex;
        private int m_XTileEndIndex;
        private int m_YTileStartIndex;
        private int m_YTileEndIndex;

        private Direction m_CurrentDirection;
        private Direction m_PrevDirection;
        private bool m_IsInitialized = false;
        private bool m_PrevInitialized = false;
        private bool m_StartTile = true;
        private bool m_UseBothAxisOnStartingPoint;
        private int m_CycleIndex = -1;
        private Dictionary<Vector2Int, Tile> m_Grid;

        private Directions m_Directions = new Directions();
            
        public int m_PathCount = 0;
        
        private static Vector2Int NOT_FOUND = new Vector2Int(-1, -1);
        public LevelGenerator(int width, int height,int cycles ,Dictionary<Vector2Int, Tile> grid, bool mUseBothAxis)
        {
            m_Height = height;
            m_Width = width;
            m_Cycles = cycles;
            
            m_Grid = grid;

            m_UseBothAxisOnStartingPoint = mUseBothAxis;

            m_XTileStartIndex = 1;
            m_YTileStartIndex = 1;

            m_XTileEndIndex = width - 2;
            m_YTileEndIndex = height - 2;
        }
        
        public Vector2Int GenerateRandomLevel()
        {
            int randomX = Random.Range(m_XTileStartIndex, m_XTileEndIndex);
            int randomY = Random.Range(m_YTileStartIndex, m_YTileEndIndex);
            
            Vector2Int startCoordinates = new Vector2Int(randomX, randomY);
            Vector2Int startPos = startCoordinates;

            Direction startDir = Direction.Up;
            bool startDirInitialized = false;
            bool triedOppositeStartDir = false;
            
            SetTileAsPath(m_Grid[startCoordinates]);
            
            for (int i = 0; i < m_Cycles; i++)
            {
                m_CycleIndex = i;
                m_CurrentDirection = m_Directions.GetRandomDirection(m_CurrentDirection,ref m_IsInitialized);
                Vector2Int newCoords = NOT_FOUND;
                
                newCoords = UnblockTile(startCoordinates, m_CurrentDirection);
                if (newCoords == NOT_FOUND)
                {
                    m_CurrentDirection = m_Directions.FindOppositeDirection(m_CurrentDirection);
                    newCoords = UnblockTile(startCoordinates,m_CurrentDirection);
                    
                    if (newCoords == NOT_FOUND)
                    {
                        if (m_PrevInitialized)
                        {
                            m_CurrentDirection = m_PrevDirection;
                            DisableControlBlock(startCoordinates,m_CurrentDirection);
                            newCoords = UnblockTile(startCoordinates,m_CurrentDirection);
                        }
                        else
                        {
                            m_CurrentDirection = m_Directions.GetRandomDirFromOtherAxis(m_CurrentDirection);
                            DisableControlBlock(startCoordinates,m_CurrentDirection);
                            newCoords = UnblockTile(startCoordinates,m_CurrentDirection);
                        }
                    }
                }

                if (!startDirInitialized)
                {
                    startDir = m_CurrentDirection;
                    startDirInitialized = true;
                }
                
                m_PrevInitialized = true;
                m_PrevDirection = m_CurrentDirection;
                if (newCoords == NOT_FOUND && !triedOppositeStartDir && m_UseBothAxisOnStartingPoint)
                {
                    triedOppositeStartDir = true;
                    Direction newDir = m_Directions.GetRandomDirFromOtherAxis(startDir);
                    newCoords = UnblockTile(startPos,newDir);

                    if (newCoords == NOT_FOUND)
                    {
                        newDir = m_Directions.FindOppositeDirection(newDir);
                        newCoords = UnblockTile(startPos, newDir);
                        if (newCoords == NOT_FOUND)
                        {
                            break;
                        }
                    }
                    
                    m_CurrentDirection = newDir;
               }
                
                if (newCoords == NOT_FOUND)
                {
                    break;
                }
                
                startCoordinates = newCoords;
            }
            
            return startPos;
        }      
        public Vector2Int UnblockTile(Vector2Int tileCoordinates,Direction dir)
        {
            List<Tile> possibleEndPoints = new List<Tile>();
            possibleEndPoints = FindPossibleEndPoints(tileCoordinates, dir);

            Vector2Int curTile = UnblockPath(tileCoordinates, dir, possibleEndPoints);
            
            return curTile;
        }

        private Vector2Int UnblockPath(Vector2Int tileCoordinates, Direction dir, List<Tile> possibleEndPoints)
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
                if (tile.IsBlocked)
                {
                    SetTileAsPath(tile);
                }
               
            }
            
            ActivateControlBlock(tileCoordinates,curTile.Coordinates,dir);
            return  curTile.Coordinates;
            
        }

        private void SetTileAsPath(Tile tile)
        {
            tile.IsBlocked = false;
            tile.SetTileAsPath();
            m_PathCount++;
        }
        
        private List<Tile> FindPossibleEndPoints(Vector2Int tileCoordinates, Direction dir)
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
        

        private void ActivateControlBlock(Vector2Int startCoords,Vector2Int endCoords, Direction dir)
        {
            if (m_Grid[endCoords].Neigbors[dir].IsBlocked)
            {
                m_Grid[endCoords].Neigbors[dir].SetControlIndex(m_CycleIndex);
                m_Grid[endCoords].Neigbors[dir].IsControlBlock = true;
            }

            
            Direction nb = m_Directions.FindOppositeDirection(dir);
            m_Grid[startCoords].Neigbors[nb].IsControlBlock = true;
            m_Grid[startCoords].Neigbors[nb].SetControlIndex(m_CycleIndex);
            
        }

        private void DisableControlBlock(Vector2Int startCoords, Direction dir)
        {
            if (m_Grid[startCoords].Neigbors[dir].IsControlIndex == m_CycleIndex-1)
            {
                m_Grid[startCoords].Neigbors[dir].IsControlBlock = false;
            }
        }
        
        private int GetMaxLength(Vector2Int coordinates, Direction dir)
        {
            int length = 0;
            switch (dir)
            {
                case Direction.Up:
                    length = (m_YTileEndIndex + 1) - coordinates.y;
                    break;
                case Direction.Down:
                    length = coordinates.y;
                    break;
                case Direction.Right:
                    length = (m_XTileEndIndex + 1) - coordinates.x;
                    break;
                case Direction.Left:
                    length = coordinates.x;
                    break;
            }
            return length;
        }
        
        public bool IsEdge(Vector2Int coord) => coord.x == m_XTileStartIndex || coord.x == m_XTileEndIndex ||
                                                coord.y == m_YTileStartIndex || coord.y == m_YTileEndIndex;

        public bool IsBlocked(Vector2Int coordinates) => m_Grid[coordinates].IsBlocked;

        public bool IsNeigborBlocked(Vector2Int coordinates, Direction dir) => m_Grid[coordinates].Neigbors[dir].IsBlocked;

        public bool IsControlBlock(Vector2Int coordinates) => m_Grid[coordinates].IsControlBlock;
        
        public bool IsNeigborControlBlock(Vector2Int coordinates, Direction dir)=> m_Grid[coordinates].Neigbors[dir].IsControlBlock;
        
    }
}
