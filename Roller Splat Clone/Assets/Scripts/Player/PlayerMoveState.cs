using System.Collections;
using System.Collections.Generic;
using GridSystem;
using UnityEditor.Timeline.Actions;
using UnityEngine;

namespace Player
{
    public class PlayerMoveState : PlayerBaseStat
    {
        private int m_CurrentPathIndex;
  
        public override void OnEnter(PlayerStateMachine stateMachine)
        {
            // Move Ball in dir 
            //Debug.Log($"Move Ball in direction {stateMachine.SwipeDir}");
            stateMachine.InvokeOnPlayerEnterMoveState();
            m_CurrentPathIndex = 0;
        }

        public override void OnUpdate(PlayerStateMachine stateMachine)
        {
            if (stateMachine.Path.Count == 0)
            {
              stateMachine.SwitchState(PlayerStates.Idle);
              return;
            }
            
            MovePlayer();
            RotatePlayer();
            
            void MovePlayer()
            {
                //stateMachine.transform.Translate( stateMachine.MoveSpeed* stateMachine.DeltaTime*stateMachine.SwipeDir, Space.World);
                Tile targetTile = stateMachine.Path[m_CurrentPathIndex];
                Vector3 targetPos = targetTile.WorldPosition;
                targetPos.y = stateMachine.transform.position.y;
                stateMachine.transform.position = Vector3.MoveTowards(stateMachine.transform.position, targetPos,
                    stateMachine.DeltaTime * stateMachine.MoveSpeed);
                
                if (Vector3.Distance(stateMachine.transform.position, targetPos) < 0.1)
                {
                    targetTile.ColorTile(Color.green);
                    
                    if (stateMachine.Path.Count > m_CurrentPathIndex+1)
                    {
                        m_CurrentPathIndex++;
                    }
                    else
                    {
                        stateMachine.SwitchState(PlayerStates.Idle);
                    }  
                    
                    //Debug.Log($"Moving to {m_CurrentPathIndex}");
                }
            }

            void RotatePlayer()
            {
                Vector3 dir = new Vector3(stateMachine.SwipeDir.x, 0, stateMachine.SwipeDir.y);
                Vector3 rotateAxis = Vector3.Cross( Vector3.up,dir);
                stateMachine.transform.Rotate(rotateAxis,stateMachine.RotateAngle,Space.World);
            }
        }

        public override void OnExit(PlayerStateMachine stateMachine)
        {
            // Stop Ball On Exit Or On Collision
            //Debug.Log("Stop Ball");
            stateMachine.Path = null;
        }
    }
}