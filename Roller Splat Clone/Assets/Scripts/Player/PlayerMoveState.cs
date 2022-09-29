using GridSystem;
using ObjectPool;
using UnityEngine;

namespace Player
{
    public class PlayerMoveState : PlayerBaseStat
    {
        private int m_CurrentPathIndex;
        private float lastParticleSpawnTime = 0;

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
            
            SpawnColorTileParticles(stateMachine);
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
                        // stateMachine.InvokeOnWallHit(stateMachine.SwipeDirVector3);
                        stateMachine.PlayWallHitAnimation();
                        
                        CallTileHitAnim(targetTile, stateMachine.SwipeDir);
                        
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

        private void CallTileHitAnim(Tile targetTile, Vector2Int swipeDir)
        {
            Direction dir = Directions.FindDirectionFromVector2Int(swipeDir);
            
            targetTile.Neigbors[dir].CallHitAnim(dir);
        }

        private void SpawnColorTileParticles(PlayerStateMachine stateMachine)
        {
            if (stateMachine.PassedTime> lastParticleSpawnTime)
            {
                TileColorParticleSpawner.Instance.OnObjectPool(stateMachine.PlayerMeshTransform.position);
                lastParticleSpawnTime = stateMachine.PassedTime + stateMachine.ParticleSpawnInterval;
            }
            
        }
    }
}