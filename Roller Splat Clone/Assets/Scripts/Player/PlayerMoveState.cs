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
            CheckWallCollision();
            CheckGroundCollision();


            void MovePlayer()
            {
                stateMachine.transform.Translate( stateMachine.MoveSpeed* stateMachine.DeltaTime*stateMachine.SwipeDir, Space.World);
            }

            void RotatePlayer()
            {
                Vector3 rotateAxis = Vector3.Cross( Vector3.up,stateMachine.SwipeDir);
                stateMachine.transform.Rotate(rotateAxis,stateMachine.RotateAngle,Space.World);
            }
            
            void CheckWallCollision()
            {
                Vector3 raycastDir = stateMachine.transform.position + stateMachine.SwipeDir;
            
                if (Physics.Raycast(stateMachine.transform.position, raycastDir,stateMachine.MaxDistance))
                {
                    Debug.Log("Hit with Wall");
                    stateMachine.SwitchState(PlayerStates.Idle);
                }
                Debug.DrawLine(stateMachine.transform.position, raycastDir,Color.red,testTime);
            }

            void CheckGroundCollision()
            {
                Vector3 groundRaycastDir = stateMachine.transform.position + Vector3.down;
            
                if (Physics.Raycast(stateMachine.transform.position, groundRaycastDir,stateMachine.MaxDistance))
                {
                   
                }
            
                Debug.DrawLine(stateMachine.transform.position, groundRaycastDir,Color.red,testTime);
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