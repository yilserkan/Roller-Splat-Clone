namespace Player
{
    public class PlayerFinishState : PlayerBaseStat
    {
        public override void OnEnter(PlayerStateMachine stateMachine)
        { 
            stateMachine.DisablePlayerInputs();
        }
    }
}