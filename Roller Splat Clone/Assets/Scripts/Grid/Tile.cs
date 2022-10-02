using System;
using System.Collections.Generic;
using Effects;
using GameUI;
using LevelSystem;
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
        public int ControlIndex;
        public Dictionary<Direction, Tile> Neigbors;
        
        private MeshRenderer m_MeshRenderer;
        private bool m_PathPosSet = false;
        
        private bool CanBeColored => !IsColored && !IsBlocked;
        private bool ControlIndexSet => ControlIndex != -1;
        private bool PathColoredMaterialSet(Color color) => pathColoredMaterial.color == color;
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
            if (!PathColoredMaterialSet(color))
            {
                pathColoredMaterial.color = color;
            }
            
            if (CanBeColored)
            {
                m_MeshRenderer.sharedMaterial = pathColoredMaterial;
                IsColored = true;
                OnTileColored?.Invoke();
            }
        }

        public void UnblockTile()
        {
            if (IsBlocked)
            {
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
            if (!ControlIndexSet)
            {
                ControlIndex = index;
            }
        }

        public bool ControlBlockSetOnPreviousCycle(int cycleIndex)
        {
            return ControlIndex == cycleIndex;
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
            ControlIndex = -1;
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
            LevelGeneratorManager.OnResetTiles += HandleOnResetTiles;
            LevelFinishEffects.OnResetTiles += HandleOnResetTiles;
            LevelMainUIHandler.OnResetLevel += HandleOnResetTiles;
        }
        
        private void RemoveListeners()
        {
            LevelGeneratorManager.OnResetTiles -= HandleOnResetTiles;
            LevelFinishEffects.OnResetTiles -= HandleOnResetTiles;
            LevelMainUIHandler.OnResetLevel -= HandleOnResetTiles;
        }
    }
}