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

        public override void Tick(PlayerStateMachine stateMachine)
        {
            passedTime += stateMachine.DeltaTime;
            if (passedTime > testTime)
            {
                stateMachine.SwitchState(PlayerStates.Idle);
            }

            Vector3 raycastDir = stateMachine.transform.position + new Vector3(stateMachine.SwipeDir.x, 0, stateMachine.SwipeDir.y);
            
            if (Physics.Raycast(stateMachine.transform.position, raycastDir,stateMachine.MaxDistance))
            {
                Debug.Log("Hit with Wall");
            }
            
            Debug.DrawLine(stateMachine.transform.position, raycastDir,Color.red,testTime);
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