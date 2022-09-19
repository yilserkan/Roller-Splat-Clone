using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerMoveState : PlayerBaseStat
    {
        public override void OnEnter(PlayerStateMachine stateMachine)
        {
            // Move Ball in dir 
        }
        

        public override void OnExit(PlayerStateMachine stateMachine)
        {
            // Stop Ball On Exit Or On Collision
        }

        public override void OnCollisionEnter(PlayerStateMachine stateMachine, Collision collision)
        {
            // Stop Ball On Exit Or On Collision
        }
    }
}