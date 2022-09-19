using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public enum PlayerStates
    {
        Idle,
        Move
    }
    
    public class PlayerStateMachine : MonoBehaviour
    {
        [SerializeField] private PlayerStates initialState;

        private PlayerStates m_CurrentPlayerState;
        private PlayerBaseStat m_CurrentBaseState;
        private PlayerMoveState m_PlayerMoveState;
        private PlayerMoveState m_PlayerStopState;

        private Dictionary<PlayerStates, PlayerBaseStat> m_State = 
            new Dictionary<PlayerStates, PlayerBaseStat>()
        {
            { PlayerStates.Idle, new PlayerIdleState() },
            { PlayerStates.Move, new PlayerMoveState() }
        };

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

        private void SwitchState(PlayerStates newState)
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
    }
}