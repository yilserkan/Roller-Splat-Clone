using System;
using System.Collections;
using System.Collections.Generic;
using GridSystem;
using UnityEngine;
using Grid = GridSystem.Grid;

namespace Player
{
    public enum PlayerStates
    {
        None,
        Idle,
        Move
    }
    
    public class PlayerStateMachine : MonoBehaviour
    {
        [SerializeField] private PlayerStates initialState;
        [SerializeField] private PlayerInputSystem inputSystem;
        [SerializeField] private float raycastMultiplicator = 0.5f;
        [SerializeField] private float moveSpeed;
        [SerializeField] private float rotateAngle;
        [SerializeField] private float maxDistance = 0.5f;

        private PlayerStates m_CurrentPlayerState = PlayerStates.None;
        private PlayerBaseStat m_CurrentBaseState;
        private PlayerMoveState m_PlayerMoveState;
        private PlayerMoveState m_PlayerStopState;
        
        private Vector2Int m_swipeDir;
        private List<Tile> m_Path;
        public Vector2Int SwipeDir
        {
            get { return  m_swipeDir; }
            private set { m_swipeDir = value; }
        }

        public List<Tile> Path
        {
            get
            {
                return m_Path;
            }
            set
            {
                m_Path = value;
            }
        }

        public float MaxDistance
        {
            get { return maxDistance; }
        }

        public float RotateAngle
        {
            get
            {
                return rotateAngle;
            }
        }
        public float RaycastMultiplicator
        {
            get
            {
                return raycastMultiplicator;
            }
        }

        public float MoveSpeed
        {
            get
            {
                return moveSpeed;
            }
        }
        
        public float DeltaTime => Time.deltaTime;

        public static event Action<Vector3, Vector2Int> OnPlayerEnterMoveState;

        public void InvokeOnPlayerEnterMoveState() => OnPlayerEnterMoveState?.Invoke(transform.position,SwipeDir);

        private Dictionary<PlayerStates, PlayerBaseStat> m_State = 
            new Dictionary<PlayerStates, PlayerBaseStat>()
        {
            { PlayerStates.Idle, new PlayerIdleState() },
            { PlayerStates.Move, new PlayerMoveState() }
        };

        private void OnEnable()
        {
            PlayerInputSystem.OnPlayerSwipe += HandleOnPlayerSwipe;
            Grid.OnFoundPlayerPath += HandleOnFoundPlayerPath;
            Grid.OnStartPointFound += HandleOnPlayerPosFound;
        }
        
        private void OnDisable()
        {
            PlayerInputSystem.OnPlayerSwipe -= HandleOnPlayerSwipe;
            Grid.OnFoundPlayerPath -= HandleOnFoundPlayerPath;
            Grid.OnStartPointFound -= HandleOnPlayerPosFound;
        }

        private void HandleOnPlayerPosFound(Vector3 position)
        {
            transform.position = position;
        }

        void Start()
        {
            //transform.position = new Vector3(0,1,0);
            SwitchState(initialState);
        }

        private void Update()
        {
            OnUpdate();
        }

        private void FixedUpdate()
        {
            OnFixedUpdate();
        }

        public void SwitchState(PlayerStates newState)
        {
            if (newState == m_CurrentPlayerState)
            {
                return;
            }
            
            m_CurrentBaseState?.OnExit(this);
            m_CurrentPlayerState = newState;
            m_CurrentBaseState = m_State[m_CurrentPlayerState];
            m_CurrentBaseState?.OnEnter(this);
        }
        
        private void OnUpdate()
        {
            m_CurrentBaseState?.OnUpdate(this);
        }
        
        private void OnFixedUpdate()
        {
            m_CurrentBaseState?.OnFixedUpdate(this);
        }

        
        private void HandleOnPlayerSwipe(Vector2Int swipeDir)
        {
            SwipeDir = swipeDir;
            SwitchState(PlayerStates.Move);
        }

        private void HandleOnFoundPlayerPath(List<Tile> path)
        {
            Path = path;
        }

        public void EnablePlayerInputs()
        {
            inputSystem.enabled = true;
        }

        public void DisablePlayerInputs()
        {
            inputSystem.ResetTouchStartPosition();
            inputSystem.enabled = false;
        }
    }
}