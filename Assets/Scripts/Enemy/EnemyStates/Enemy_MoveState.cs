public class Enemy_MoveState : Enemy_GroundedState
{
    public Enemy_MoveState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        if (enemy.groundDetected && !enemy.wallDetected) return;
        enemy.Flip();
    }

    public override void Update()
    {
        base.Update();
        
        enemy.SetVelocity(enemy.moveSpeed * enemy.facingDir, rb.linearVelocityY);

        if (enemy.groundDetected && !enemy.wallDetected) return;
        stateMachine.ChangeState(enemy.idleState);
    }
}
