
namespace Player
{
    public class PlayerIdleState : PlayerBaseStat
    {
        public override void OnEnter(PlayerStateMachine stateMachine)
        {
            stateMachine.EnablePlayerInputs();
        }
        

        public override void OnExit(PlayerStateMachine stateMachine)
        {
            stateMachine.DisablePlayerInputs();
        }
    }
}