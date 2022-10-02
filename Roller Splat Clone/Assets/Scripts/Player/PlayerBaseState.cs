
namespace Player
{
    public abstract class PlayerBaseStat
    {
        public virtual void OnEnter(PlayerStateMachine stateMachine){}
        public virtual void OnUpdate(PlayerStateMachine stateMachine){}
        public virtual void OnExit(PlayerStateMachine stateMachine){}
    }
}