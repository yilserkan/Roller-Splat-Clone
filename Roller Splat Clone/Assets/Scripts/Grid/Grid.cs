using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Player;
using Unity.VisualScripting;
using UnityEngine;

namespace GridSystem
{
    
    public class Grid : MonoBehaviour
    {
        [SerializeField] private int height;
        [SerializeField] private int width;
        [SerializeField] private int cellsize;
        [SerializeField] private Vector3 gridStartPosition;
        [SerializeField] private GameObject prefab;

        public static event Action<List<Tile>> OnFoundPlayerPath;
        
        public static event Action<Vector3> OnStartPointFound;
        private Dictionary<Vector2Int, Tile> m_Grid = new Dictionary<Vector2Int, Tile>();

        private LevelGenerator m_LevelGenerator;
        
        public static Dictionary<Neighbors, Vector2Int> m_Directions = new Dictionary<Neighbors, Vector2Int>()
        {
            { Neighbors.Up, new Vector2Int(0, 1) },
            { Neighbors.Down, new Vector2Int(0, -1) },
            { Neighbors.Right, new Vector2Int(1, 0) },
            { Neighbors.Left, new Vector2Int(-1, 0) }
        };

        private void OnEnable()
        {
            PlayerStateMachine.OnPlayerEnterMoveState += HandleOnPlayerEnterMoveState;
        }

        private void OnDisable()
        {
            PlayerStateMachine.OnPlayerEnterMoveState -= HandleOnPlayerEnterMoveState;
        }
        
        private void Start()
        {
           
            CreateGrid();
            m_LevelGenerator = new LevelGenerator(width, height, m_Grid);
            Vector2Int startPos = m_LevelGenerator.GenerateRandomLevel();
            OnStartPointFound.Invoke(GetWorldPosFromCoordinates(startPos));
        }

        private void CreateGrid()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector2Int coordinates = new Vector2Int(x, y);
                    Vector3 worldPos = GetWorldPosFromCoordinates(coordinates);
                    Tile tile = Instantiate(prefab, worldPos, Quaternion.identity,transform).GetComponent<Tile>();
                    tile.gameObject.name = $"({coordinates.x},{coordinates.y})";
                    tile.Init(coordinates,worldPos);
                    m_Grid.Add(coordinates,tile);
                }
            }
            FindNeighbors();
            ResetAllTiles();
           
        }
        
        public Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(x,y) * cellsize;
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
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
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

        public List<Tile> FindPlayerPath(Vector2Int startCoordinate, Vector2Int moveDir)
        {
            List<Tile> path = new List<Tile>();
            var dir = m_Directions.FirstOrDefault(x => x.Value == moveDir).Key;
            
            Vector2Int currentCoordinate = startCoordinate;
            path.Add(m_Grid[currentCoordinate]);
            
            while (m_Grid[currentCoordinate].Neigbors[dir] != null && !m_Grid[currentCoordinate].Neigbors[dir].IsBlocked)
            {
                Debug.Log($"Way : {currentCoordinate}");
                currentCoordinate = m_Grid[currentCoordinate].Neigbors[dir].Coordinates;
                path.Add(m_Grid[currentCoordinate]);
            }
            Debug.Log($"Way : {currentCoordinate}");
            Debug.Log($"--------------");
            return path;
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
        
        private void HandleOnPlayerEnterMoveState(Vector3 worldPos, Vector2Int dir)
        {
            List<Tile> path = FindPlayerPath(GetCoordinatesFromWorldPos(worldPos) , dir);
            OnFoundPlayerPath?.Invoke(path);
        }
    }
}
