using System;
using System.Collections;
using System.Collections.Generic;
using GridSystem;
using LevelSystem;
using UnityEngine;
using Grid = GridSystem.Grid;
using Random = UnityEngine.Random;

namespace Player
{
    public enum PlayerStates
    {
        None,
        Idle,
        Move,
        Finish
    }
    
    public class PlayerStateMachine : MonoBehaviour
    {
        [SerializeField] private PlayerStates initialState;
        [SerializeField] private PlayerInputSystem inputSystem;
        [SerializeField] private PlayerSettings playerSettings;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Transform playerMeshTransform;
        
        private PlayerStates m_CurrentPlayerState = PlayerStates.None;
        private PlayerBaseStat m_CurrentBaseState;
        private PlayerMoveState m_PlayerMoveState;
        private PlayerMoveState m_PlayerStopState;
        
        private Vector2Int m_SwipeDir;
        private List<Tile> m_Path = new List<Tile>();
        private Color m_CurrentColor;
        
        public Vector2Int SwipeDir
        {
            get => m_SwipeDir;
            private set => m_SwipeDir = value;
        }

        public Vector3 SwipeDirVector3
        {
            get => new Vector3(m_SwipeDir.x, 0, m_SwipeDir.y);
        }
        
        public List<Tile> Path
        {
            get => m_Path;
            
            set => m_Path = value;
            
        }

        public void EnablePlayerInputs() => inputSystem.EnablePlayerInputs();
        public void DisablePlayerInputs() => inputSystem.DisablePlayerInputs();
        private void OnStateEnter() => m_CurrentBaseState?.OnEnter(this);
        private void OnStateExit() => m_CurrentBaseState?.OnExit(this);
        private bool StateChanged(PlayerStates newState) => newState != m_CurrentPlayerState;
        public float MaxDistance => playerSettings.MaxDistance;
        public float RotateAngle => playerSettings.RotateAngle;
        public float RaycastMultiplicator => playerSettings.RaycastMultiplicator;
        public float MoveSpeed =>  playerSettings.MoveSpeed;
        public float DeltaTime => Time.deltaTime;

        public Transform PlayerMeshTransform => playerMeshTransform;
        public Color CurrentColor => m_CurrentColor;
        public bool IsPathEmpty => Path.Count == 0;
        public bool IsPathLeft(int currentPathIndex) => Path.Count > currentPathIndex;
        public bool HasPlayerPassedTile(Vector3 tilePos) => (tilePos - transform.position).sqrMagnitude < 0.1 * 0.1;
        public static event Action<Vector3, Vector2Int> OnPlayerSwipeDirectionFound;
        public static event Action<Vector3> OnWallHit;

        public void InvokeOnWallHit(Vector3 swipeDirection) => OnWallHit?.Invoke(swipeDirection);

        private Dictionary<PlayerStates, PlayerBaseStat> m_State = 
            new Dictionary<PlayerStates, PlayerBaseStat>()
        {
            { PlayerStates.Idle, new PlayerIdleState() },
            { PlayerStates.Move, new PlayerMoveState() },
            { PlayerStates.Finish, new PlayerFinishState() }
        };

        private void OnEnable()
        {
            AddListeners();
        }
        
        private void OnDisable()
        {
            RemoveListeners();
        }
        
        void Start()
        {
            SwitchState(initialState);
        }

        private void Update()
        {
            OnUpdate();
        }
        private void OnUpdate()
        {
            m_CurrentBaseState?.OnUpdate(this);
        }
        
        public void SwitchState(PlayerStates newState)
        {
            if (!StateChanged(newState) || m_CurrentPlayerState == PlayerStates.Finish)
            {
                return;
            }
            
            OnStateExit();
            ChangeState(newState);
            OnStateEnter();
        }

        private void ChangeState(PlayerStates newState)
        {
            m_CurrentPlayerState = newState;
            m_CurrentBaseState = m_State[m_CurrentPlayerState];
        }
        
        private void HandleOnPlayerPosFound(Vector3 position)
        {
            Debug.Log(position);
            transform.position = position;
        }

        private void HandleOnPlayerSwipe(Vector2Int swipeDir)
        {
            SwipeDir = swipeDir;
            OnPlayerSwipeDirectionFound?.Invoke(transform.position, SwipeDir);

        }

        private void HandleOnFoundPlayerPath(List<Tile> path)
        {
            Path = path;
            SwitchState(PlayerStates.Move);
        }
        
        private void HandleOnLevelFinished()
        {
            Debug.Log("------------------------- Player state finished");
            SwitchState(PlayerStates.Idle);
            Path.Clear();
        }
        
        private void HandleOnColorSet(Color color)
        {
            m_CurrentColor = color;
            //meshRenderer.material.color = m_CurrentColor;
            meshRenderer.sharedMaterial.color = m_CurrentColor;
        }

        private void AddListeners()
        {
            PlayerInputSystem.OnPlayerSwipe += HandleOnPlayerSwipe;
            Grid.OnFoundPlayerPath += HandleOnFoundPlayerPath;
            Grid.OnStartPointFound += HandleOnPlayerPosFound;
            Grid.OnLevelFinished += HandleOnLevelFinished;
            Grid.OnColorSet += HandleOnColorSet;
        }
        
        private void RemoveListeners()
        {
            PlayerInputSystem.OnPlayerSwipe -= HandleOnPlayerSwipe;
            Grid.OnFoundPlayerPath -= HandleOnFoundPlayerPath;
            Grid.OnStartPointFound -= HandleOnPlayerPosFound;
            Grid.OnLevelFinished += HandleOnLevelFinished;
            Grid.OnColorSet -= HandleOnColorSet;
        }
    }
}