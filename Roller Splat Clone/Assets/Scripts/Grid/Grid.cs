using System;
using System.Collections.Generic;
// using System.Linq;
using LevelSystem;
using ObjectPool;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GridSystem
{
    
    public class Grid : MonoBehaviour
    {
        [SerializeField] private PlayerStateMachine player;
        
        [SerializeField] private int height;
        [SerializeField] private int width;
        [SerializeField] private int cycles;
        [SerializeField] private int cellsize;
        [SerializeField] private Vector3 gridStartPosition = Vector3.zero;
        [SerializeField] private bool useBothAxisOnStartingPoint;
        
        public static event Action<List<Tile>> OnFoundPlayerPath;
        public static event Action OnLevelCreated;
        public static event Action OnLevelFinished; 
        //public static event Action OnResetTiles;
        public static event Action<Vector3> OnStartPointFound;
        // public static event Action<Vector2Int> OnStartPointFound;
        public static event Action<Color> OnColorSet;
        public static event Action<Color> OnBackgorundColorSet;
        
        private Dictionary<Vector2Int, Tile> m_Grid = new Dictionary<Vector2Int, Tile>();

        private LevelGenerator m_LevelGenerator;
        private int m_ColoredTilesCount = 0;
        
        public static Dictionary<Direction, Vector2Int> m_Directions = new Dictionary<Direction, Vector2Int>()
        {
            { Direction.Up, Vector2Int.up },
            { Direction.Down, Vector2Int.down },
            { Direction.Right, Vector2Int.right },
            { Direction.Left, Vector2Int.left }
        };

        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
           RemoveListeners();
        }
        
        private void HandleOnGenerateLevel(Level level)
        {
            height = level.Height;
            width = level.Width;
            cycles = level.Cycles;
            Random.InitState(level.Seed);
            
            Color color = Random.ColorHSV(0f,1f,1f,1f,0.5f,1f);
            OnColorSet?.Invoke(color);
            
            Color backgroundColor = Random.ColorHSV(0f,1f,.5f,.5f,0.5f,1f);
            OnBackgorundColorSet?.Invoke(backgroundColor);
            
            CreateLevel();
        }

    
        private void CreateLevel()
        {
            ResetGrid();
            CreateGrid();
            
            m_LevelGenerator = new LevelGenerator(width, height,cycles, m_Grid,useBothAxisOnStartingPoint);
            Vector2Int startPos = m_LevelGenerator.GenerateRandomLevel();
            
            OnLevelCreated?.Invoke();
            
            Vector3 startPosition = GetWorldPosFromCoordinates(startPos);
            OnStartPointFound?.Invoke(startPosition);
            // OnStartPointFound?.Invoke(startPos);
        }

        private void ResetGrid()
        {
            if (m_Grid.Count > 0)
            {
                m_Grid.Clear();
                m_ColoredTilesCount = 0;
            }
        }

        private void CreateGrid()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector2Int coordinates = new Vector2Int(x, y);
                    Vector3 worldPos = GetWorldPosFromCoordinates(coordinates);
                    var tile = TileSpawner.Instance.OnObjectPool(worldPos);
                    tile.gameObject.name = $"({coordinates.x},{coordinates.y})";
                    tile.Init(coordinates,worldPos);
                    m_Grid.Add(coordinates,tile);
                }
            }
            
            FindNeighbors();
            ResetAllTiles();
        }
        
        public Vector3 GetWorldPosFromCoordinates(Vector2Int coordinates)
        {
            float posX = gridStartPosition.x + coordinates.x * cellsize;
            float posY = gridStartPosition.z + coordinates.y * cellsize;

            return new Vector3(posX, 0, posY);
        }

        public Vector2Int GetCoordinatesFromWorldPos(Vector3 worldPosition)
        {
            int coordinateX = Mathf.RoundToInt((worldPosition.x - gridStartPosition.x )/ cellsize);
            int coordinateY = Mathf.RoundToInt((worldPosition.z -gridStartPosition.z)/ cellsize);

            Vector2Int coordinate = new Vector2Int(coordinateX, coordinateY);
            
            if (m_Grid.ContainsKey(coordinate))
            {
                return coordinate;
            }

            return new Vector2Int(0, 0);
        }
        
        public void FindNeighbors()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector2Int coordinates = new Vector2Int(x, y);
                
                    AddNeighbors(coordinates);
                }
            }
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
        

        public List<Tile> FindPlayerPath(Vector2Int startCoordinate, Vector2Int moveDir)
        {
            List<Tile> path = new List<Tile>();
            // var dir = m_Directions.FirstOrDefault(x => x.Value == moveDir).Key;

            var dir = FindDirectionFromVector2Int(moveDir);
            
            Vector2Int currentCoordinate = startCoordinate;
            path.Add(m_Grid[currentCoordinate]);
            
            while (m_Grid[currentCoordinate].Neigbors[dir] != null && !m_Grid[currentCoordinate].Neigbors[dir].IsBlocked)
            {
                currentCoordinate = m_Grid[currentCoordinate].Neigbors[dir].Coordinates;
                path.Add(m_Grid[currentCoordinate]);
            }
            
            return path;
        }

        private Direction FindDirectionFromVector2Int(Vector2Int coordinates)
        {
            for (int i = 0; i < m_Directions.Count; i++)
            {
                Direction direction = (Direction)i;
                if (m_Directions[direction] == coordinates)
                {
                    return direction;
                }
            }

            return Direction.Up;
        }
        
        public void ResetAllTiles()
        {
            foreach (var tile in m_Grid)
            {
                tile.Value.IsControlBlock = false;
                tile.Value.IsBlocked = true;
                tile.Value.IsColored = false;
            }
        }
        
        private void HandleOnPlayerSwipeDirectionFound(Vector3 worldPos, Vector2Int dir)
        {
            List<Tile> path = FindPlayerPath(GetCoordinatesFromWorldPos(worldPos) , dir);
            OnFoundPlayerPath?.Invoke(path);
        }
        
        private void HandleOnTileColored()
        {
            m_ColoredTilesCount++;
            
            if (m_ColoredTilesCount == m_LevelGenerator.m_PathCount)
            {
                LevelFinished();
            }
        }

        private void LevelFinished()
        {
            ResetGrid();
            // OnResetTiles?.Invoke();
            OnLevelFinished?.Invoke();
        }

        private void AddListeners()
        {
            PlayerStateMachine.OnPlayerSwipeDirectionFound += HandleOnPlayerSwipeDirectionFound;
            Tile.OnTileColored += HandleOnTileColored;
            LevelGeneratorManager.OnGenerateLevel += HandleOnGenerateLevel;
            LevelManager.OnGenerateLevel += HandleOnGenerateLevel;
        }

        private void RemoveListeners()
        {
            PlayerStateMachine.OnPlayerSwipeDirectionFound -= HandleOnPlayerSwipeDirectionFound;
            Tile.OnTileColored -= HandleOnTileColored;
            LevelGeneratorManager.OnGenerateLevel -= HandleOnGenerateLevel;
            LevelManager.OnGenerateLevel -= HandleOnGenerateLevel;
        }
    }
}
