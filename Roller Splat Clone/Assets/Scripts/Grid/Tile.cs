using System;
using System.Collections.Generic;
using Effects;
using ObjectPool;
using UnityEngine;

namespace GridSystem
{
    public class Tile : AbstractObjectPoolObject<Tile>

    {
        [SerializeField] private Material wallMaterial;
        [SerializeField] private Material pathUncoloredMaterial;
        [SerializeField] private Material pathColoredMaterial;

        [SerializeField] private TileAnimation tileAnimation;
        
        public Vector2Int Coordinates;
        public Vector3 WorldPosition;
        public bool IsBlocked;
        public bool IsControlBlock;
        public bool IsColored;
        public int IsControlIndex;
        public Dictionary<Direction, Tile> Neigbors;
        
        private MeshRenderer m_MeshRenderer;
        private bool m_PathPosSet = false;

        public static event Action OnTileColored;

        private void Awake()
        {
            m_MeshRenderer = GetComponent<MeshRenderer>();
        }

        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        public void Init(Vector2Int coordinates, Vector3 worldPos)
        {
            ResetTile();
            
            SetCoordinates(coordinates);
            SetWorldPosition(worldPos);
        }

        private void SetCoordinates(Vector2Int coordinates)
        {
            Coordinates = coordinates;
        }

        private void SetWorldPosition(Vector3 worldPos)
        {
            WorldPosition = worldPos;
        }

        public void ColorTile(Color color)
        {
            if (pathColoredMaterial.color != color)
            {
                pathColoredMaterial.color = color;
            }
            
            if (!IsColored && !IsBlocked)
            {
                // m_MeshRenderer.material.color = color;
                m_MeshRenderer.sharedMaterial = pathColoredMaterial;
                IsColored = true;
                OnTileColored?.Invoke();
            }
        }
        
        public void UnblockTile()
        {
            if (IsBlocked)
            {
                // m_MeshRenderer.material.color = Color.gray;
                m_MeshRenderer.sharedMaterial = pathUncoloredMaterial;
                transform.position += Vector3.down;
                IsBlocked = false;
            }
        }
        
        public void SetControlBlock(int cycleIndex)
        {
            if (IsBlocked)
            {
                SetControlIndex(cycleIndex);
                IsControlBlock = true;
            }
        }
        private void SetControlIndex(int index)
        {
            if (IsControlIndex == -1)
            {
                IsControlIndex = index;
            }
        }

        public bool ControlBlockSetOnPreviousCycle(int cycleIndex)
        {
            return IsControlIndex == cycleIndex;
        }

        public void CallHitAnim(Direction hitDirection)
        {
            if (IsBlocked)
            {
                tileAnimation.PlayHitAnim(hitDirection);
            }
        }
        
        public void ResetTile()
        {
            // m_MeshRenderer.material.color = Color.white;
            m_MeshRenderer.sharedMaterial = wallMaterial;
            Coordinates = new Vector2Int(-1,-1);
            WorldPosition = Vector3.zero;
            IsBlocked = false;
            IsColored = false;
            IsControlIndex = -1;
            m_PathPosSet = false;
            Neigbors = new Dictionary<Direction, Tile>()
            {
                { Direction.Up, null },
                { Direction.Down, null },
                { Direction.Left, null },
                { Direction.Right, null }
            };
        }
        
        private void HandleOnResetTiles()
        {
            ResetTile();
            ReleaseObject();
        }
        
        private void AddListeners()
        {
            Grid.OnResetTiles += HandleOnResetTiles;
            LevelGeneratorUI.OnResetTiles += HandleOnResetTiles;
            LevelFinishEffects.OnResetTiles += HandleOnResetTiles;
        }
        
        private void RemoveListeners()
        {
            Grid.OnResetTiles -= HandleOnResetTiles;
            LevelGeneratorUI.OnResetTiles -= HandleOnResetTiles;
            LevelFinishEffects.OnResetTiles -= HandleOnResetTiles;
        }
    }
}