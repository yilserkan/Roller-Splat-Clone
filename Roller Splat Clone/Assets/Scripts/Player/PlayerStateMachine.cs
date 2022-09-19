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

        private PlayerStates m_CurrentPlayerState = PlayerStates.None;
        private PlayerBaseStat m_CurrentBaseState;
        private PlayerMoveState m_PlayerMoveState;
        private PlayerMoveState m_PlayerStopState;

        private Vector2 m_swipeDir;

        public Vector2 SwipeDir
        {
            get
            {
                return m_swipeDir;
            }
            private set
            {
                m_swipeDir = value;
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
            Tick();
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

        private void Tick()
        {
            m_CurrentBaseState?.Tick(this);
        }
        
        private void HandleOnPlayerSwipe(Vector2 swipeDir)
        {
            SwipeDir = swipeDir;
            SwitchState(PlayerStates.Move);
        }

    }
}