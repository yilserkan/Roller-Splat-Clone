using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        private Vector2 m_swipeDir;

        public Vector3 SwipeDir
        {
            get { return  new Vector3(m_swipeDir.x, 0, m_swipeDir.y); }
            private set { m_swipeDir =  value; }
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
        
        private Dictionary<PlayerStates, PlayerBaseStat> m_State = 
            new Dictionary<PlayerStates, PlayerBaseStat>()
        {
            { PlayerStates.Idle, new PlayerIdleState() },
            { PlayerStates.Move, new PlayerMoveState() }
        };

        private void OnEnable()
        {
            PlayerInputSystem.OnPlayerSwipe += HandleOnPlayerSwipe;
        }

        private void OnDisable()
        {
            PlayerInputSystem.OnPlayerSwipe -= HandleOnPlayerSwipe;
        }
        
        void Start()
        {
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
        
        private void OnCollisionEnter(Collision collision)
        {
            OnCollided(collision);
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
        
        private void OnCollided(Collision collision)
        {
            m_CurrentBaseState?.OnCollisionEnter(this, collision);
        }

        private void OnUpdate()
        {
            m_CurrentBaseState?.OnUpdate(this);
        }
        
        private void OnFixedUpdate()
        {
            m_CurrentBaseState?.OnFixedUpdate(this);
        }

        
        private void HandleOnPlayerSwipe(Vector2 swipeDir)
        {
            SwipeDir = swipeDir;
            SwitchState(PlayerStates.Move);
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