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
            m_CurrentPathIndex = 0;
        }

        public override void OnUpdate(PlayerStateMachine stateMachine)
        {
            if (stateMachine.IsPathEmpty)
            {
              stateMachine.SwitchState(PlayerStates.Idle);
              return;
            }
            
            MovePlayer();
            //RotatePlayer();
            
            void MovePlayer()
            {
                Tile targetTile = stateMachine.Path[m_CurrentPathIndex];
                
                Vector3 targetTilePos = targetTile.WorldPosition;
                targetTilePos.y = stateMachine.transform.position.y;
                
                stateMachine.transform.position = Vector3.MoveTowards(stateMachine.transform.position, targetTilePos,
                    stateMachine.DeltaTime * stateMachine.MoveSpeed);
          
                if (stateMachine.HasPlayerPassedTile(targetTilePos))
                {  
                    targetTile.ColorTile(stateMachine.CurrentColor);
                    
                    if (stateMachine.IsPathLeft(m_CurrentPathIndex+1))
                    {
                        m_CurrentPathIndex++;
                    }
                    else
                    {
                        stateMachine.InvokeOnWallHit(
                            stateMachine.transform,
                            stateMachine.SwipeDirVector3
                            );
                        
                        stateMachine.SwitchState(PlayerStates.Idle);
                    }
                }
            }

            void RotatePlayer()
            {
                Vector3 dir = new Vector3(stateMachine.SwipeDir.x, 0, stateMachine.SwipeDir.y);
                Vector3 rotateAxis = Vector3.Cross( Vector3.up,dir);
                stateMachine.transform.Rotate(rotateAxis,stateMachine.RotateAngle,Space.World);
            }
        }
        
       
    }
}