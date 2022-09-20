using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;

namespace Player
{
    public class PlayerMoveState : PlayerBaseStat
    {
        private float passedTime = 0;
        private float testTime = 2f;

        public override void OnEnter(PlayerStateMachine stateMachine)
        {
            // Move Ball in dir 
            Debug.Log($"Move Ball in direction {stateMachine.SwipeDir}");
      
        }

        public override void OnFixedUpdate(PlayerStateMachine stateMachine)
        {
            
            MovePlayer();
            RotatePlayer();

            void MovePlayer()
            {
                stateMachine.transform.Translate( stateMachine.MoveSpeed* stateMachine.DeltaTime*stateMachine.SwipeDir, Space.World);
            }

            void RotatePlayer()
            {
                Vector3 rotateAxis = Vector3.Cross( Vector3.up,stateMachine.SwipeDir);
                stateMachine.transform.Rotate(rotateAxis,stateMachine.RotateAngle,Space.World);
            }
            
          
            
        }

        public override void OnExit(PlayerStateMachine stateMachine)
        {
            // Stop Ball On Exit Or On Collision
            Debug.Log("Stop Ball");
            passedTime = 0;
        }

        public override void OnCollisionEnter(PlayerStateMachine stateMachine, Collision collision)
        {
            // Stop Ball On Exit Or On Collision
        }
    }
}