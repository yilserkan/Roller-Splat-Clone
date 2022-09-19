using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerIdleState : PlayerBaseStat
    {
        public override void OnEnter(PlayerStateMachine stateMachine)
        {
            // Enable Player Inputs
        }
        
        public override void OnExit(PlayerStateMachine stateMachine)
        {
            // Disable Player Inputs
        }
        
    }
}