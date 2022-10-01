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
        private bool m_IsStartTileInitialized = false;
        private bool m_PrevDirectionInitialized = false;
        private bool m_UseBothAxisOnStartingPoint;
        private int m_CycleIndex = -1;
        private Dictionary<Vector2Int, Tile> m_Grid;

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
            Vector2Int currentTileCoordinates = GetRandomStartPosition();
            Vector2Int startTileCoordinates = currentTileCoordinates;

            Direction startDirection = Direction.Up;
            bool startDirectionInitialized = false;
            bool triedOppositeStartDirection = false;
            
            UnblockTile(m_Grid[currentTileCoordinates]);
            
            for (int i = 0; i < m_Cycles; i++)
            {
                m_CycleIndex = i;
                
                Vector2Int newCoords = NOT_FOUND;
                newCoords = TryFindPath(currentTileCoordinates ,ref newCoords);

                
                
                InitializeStartDirection(ref startDirection, ref startDirectionInitialized, newCoords);
                SetPrevoiusDirection();
                
                if (!PathFound(newCoords) && !triedOppositeStartDirection && m_UseBothAxisOnStartingPoint)
                {
                    triedOppositeStartDirection = true;
                    TryOtherStartPointAxis(startTileCoordinates,startDirection,ref newCoords);
                }
                
                if (!PathFound(newCoords))
                {
                    break;
                }
                
                currentTileCoordinates = newCoords;
            }
            
            return startTileCoordinates;
        }

        private Vector2Int TryFindPath(Vector2Int coordinates, ref Vector2Int newCoords)
        {
            m_CurrentDirection = Directions.GetRandomDirection(m_CurrentDirection,ref m_IsStartTileInitialized);
            
            newCoords = TryDirection(coordinates, m_CurrentDirection);
                
            if (!PathFound(newCoords))
            {
                m_CurrentDirection = Directions.FindOppositeDirection(m_CurrentDirection);
                newCoords = TryDirection(coordinates,m_CurrentDirection);
                    
                if (!PathFound(newCoords))
                {
                    m_CurrentDirection = GetDirFromOtherAxis(m_CurrentDirection);
                        
                    DisableControlBlock(coordinates,m_CurrentDirection);
                        
                    newCoords = TryDirection(coordinates,m_CurrentDirection);
                }
            }
            

            return newCoords;
        }

        private void TryOtherStartPointAxis(Vector2Int startTileCoordinates, Direction startDirection, ref Vector2Int newCoords)
        {
            Direction newDir = Directions.GetRandomDirFromOtherAxis(startDirection);
            newCoords = TryDirection(startTileCoordinates,newDir);

            if (!PathFound(newCoords))
            {
                newDir = Directions.FindOppositeDirection(newDir);
                newCoords = TryDirection(startTileCoordinates, newDir);
            }
                    
            m_CurrentDirection = newDir;
        }
        
        private void InitializeStartDirection(ref Direction startDirection, ref bool startDirectionInitialized, Vector2Int coords)
        {
            if (!startDirectionInitialized)
            {
                startDirection = m_CurrentDirection;
                startDirectionInitialized = true;
                
                Direction randomDir = Directions.GetRandomDirFromOtherAxis(m_CurrentDirection);
                m_Grid[coords].Neigbors[randomDir].IsControlBlock = true;
            }
        }

        private void SetPrevoiusDirection()
        {
            m_PrevDirectionInitialized = true;
            m_PrevDirection = m_CurrentDirection;
        }
        
        private Direction GetDirFromOtherAxis(Direction currentDirection)
        {
            Direction direction;
            if (m_PrevDirectionInitialized)
            {
                direction = m_PrevDirection;
            }
            else
            {
                direction = Directions.GetRandomDirFromOtherAxis(currentDirection);
            }

            return direction;
        }

        private Vector2Int GetRandomStartPosition()
        {
            int randomX = Random.Range(m_XTileStartIndex, m_XTileEndIndex);
            int randomY = Random.Range(m_YTileStartIndex, m_YTileEndIndex);
            
            return new Vector2Int(randomX, randomY);
        }
        
        
        public Vector2Int TryDirection(Vector2Int tileCoordinates,Direction dir)
        {
            List<Tile> possibleEndPoints = new List<Tile>();
            
            possibleEndPoints = FindPossibleEndPoints(tileCoordinates, dir);

            Vector2Int curTile = SearchPath(tileCoordinates, dir, possibleEndPoints);
            
            return curTile;
        }

        private Vector2Int SearchPath(Vector2Int startCoordinates, Direction dir, List<Tile> possibleEndPoints)
        {
            int endPointCount = possibleEndPoints.Count;
            
            if (EqualsNull(endPointCount))
            {
                return NOT_FOUND;
            }
            
            Tile curTile = m_Grid[startCoordinates];
            Debug.Log("Start Coords " + curTile.Coordinates + "Direction + " + m_CurrentDirection);
            
            int randomEndPoint = Random.Range(0, endPointCount);
            Tile randomEndPointTile = possibleEndPoints[randomEndPoint];

            List<Tile> path = new List<Tile>();
            path.Add(curTile);
            
            bool pathContainsBlockedTile = false;
            
            if (IsBlocked(curTile))
            {
                pathContainsBlockedTile = true;
            }

            while (!AreCoordinatesEqual(curTile.Coordinates, randomEndPointTile.Coordinates))
            {
                if (IsNeigborControlBlock(curTile.Coordinates, dir))
                {
                    break;
                }

                curTile = m_Grid[curTile.Coordinates].Neigbors[dir];
                path.Add(curTile);
                
                if (IsBlocked(curTile))
                {
                    pathContainsBlockedTile = true;
                }
            }

            if (!pathContainsBlockedTile)
            {
                return NOT_FOUND;
            }

            UnblockPath(path);
            
            EnableControlBlocks(startCoordinates,curTile.Coordinates,dir);
            return  curTile.Coordinates;
            
        }

        private void UnblockPath(List<Tile> path)
        {
            foreach (var tile in path)
            {
                if (IsBlocked(tile))
                {
                    UnblockTile(tile);
                }
            }
        }

        private void UnblockTile(Tile tile)
        {
            tile.UnblockTile();
            m_PathCount++;
        }
        
        private List<Tile> FindPossibleEndPoints(Vector2Int tileCoordinates, Direction dir)
        {
            Tile curTile = m_Grid[tileCoordinates];
            int maxLength = GetMaxLength(curTile.Coordinates,dir);

            List<Tile> possibleEndPoints = new List<Tile>();

            for (int i = 0; i < maxLength; i++)
            {
                if  (IsNeighborNull(curTile.Coordinates, dir) || IsControlBlock(curTile.Coordinates))
                {
                    break;
                }
                
                if (IsNeigborBlocked(curTile.Coordinates, dir) && !AreCoordinatesEqual(tileCoordinates,curTile.Coordinates))
                {
                    possibleEndPoints.Add(curTile);
                }
                
                curTile = m_Grid[curTile.Coordinates].Neigbors[dir];
            }
            
            return possibleEndPoints;
        }
        

        private void EnableControlBlocks(Vector2Int startCoords,Vector2Int endCoords, Direction dir)
        {
            Tile endTileNeighbor = m_Grid[endCoords].Neigbors[dir];
            endTileNeighbor.SetControlBlock(m_CycleIndex);
            
            Direction nb = Directions.FindOppositeDirection(dir);
            
            Tile startTileNeighbor = m_Grid[startCoords].Neigbors[nb];
            startTileNeighbor.SetControlBlock(m_CycleIndex);
        }
  
        private void DisableControlBlock(Vector2Int startCoords, Direction dir)
        {
            Tile tile = m_Grid[startCoords].Neigbors[dir];
            
            if (tile.ControlBlockSetOnPreviousCycle(m_CycleIndex-1))
            {
                tile.IsControlBlock = false;
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

        private bool IsBlocked(Tile tile) => m_Grid[tile.Coordinates].IsBlocked;
        private bool IsBlocked(Vector2Int coordinates) => m_Grid[coordinates].IsBlocked;

        private bool IsNeigborBlocked(Vector2Int coordinates, Direction dir) => m_Grid[coordinates].Neigbors[dir].IsBlocked;

        private bool IsControlBlock(Vector2Int coordinates) => m_Grid[coordinates].IsControlBlock;
        
        private bool IsNeigborControlBlock(Vector2Int coordinates, Direction dir)=> m_Grid[coordinates].Neigbors[dir].IsControlBlock;

        private bool IsNeighborNull(Vector2Int coordinates, Direction dir) => m_Grid[coordinates].Neigbors[dir] == null;

        private bool AreCoordinatesEqual(Vector2Int firstCoordinate, Vector2Int secondCoordinates) => firstCoordinate == secondCoordinates;

        private bool EqualsNull(int count) => count <= 0;

        private bool PathFound(Vector2Int coords) => coords != NOT_FOUND;
    }
}
