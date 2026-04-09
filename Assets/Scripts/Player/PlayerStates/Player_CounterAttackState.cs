using UnityEngine;

public class Player_CounterAttackState : PlayerState
{
    private static readonly int CounterAttackPerformed = Animator.StringToHash("counterAttackPerformed");
    private Player_Combat combat;
    private bool counteredSomebody;
    
    public Player_CounterAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        combat = player.GetComponent<Player_Combat>();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public override void Enter()
    {
        base.Enter();

        counteredSomebody = combat.CounterAttackPerformed();
        stateTimer = combat.GetCounterRecoveryDuration();
        anim.SetBool(CounterAttackPerformed, counteredSomebody);
        
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public override void Update()
    {
        base.Update();
        player.SetVelocity(0, rb.linearVelocityY);

        if (triggerCalled)
            stateMachine.ChangeState(player.idleState);

        if (stateTimer < 0 && !counteredSomebody)
            stateMachine.ChangeState(player.idleState);
    }
}
