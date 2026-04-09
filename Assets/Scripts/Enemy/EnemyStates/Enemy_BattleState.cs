using UnityEngine;

public class Enemy_BattleState : EnemyState
{
    private Transform player;
    private float lastTimeWasInBattle;
    
    public Enemy_BattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        UpdateBattleTimer();
        
        player ??= enemy.GetPlayerReference();
        
        if (!ShouldRetreat()) return;
        
        rb.linearVelocity = new Vector2(enemy.retreatVelocity.x * -DirectionToPlayer(), enemy.retreatVelocity.y);
        enemy.HandleFlip(DirectionToPlayer());
    }

    public override void Update()
    {
        base.Update();

        if (enemy.PlayerDetected())
            UpdateBattleTimer();
        
        if (BattleTimeIsOver()) 
            stateMachine.ChangeState(enemy.idleState);
        
        if (WithinAttackRange() && enemy.PlayerDetected())
            stateMachine.ChangeState(enemy.attackState);
        else 
            enemy.SetVelocity(enemy.battleMoveSpeed * DirectionToPlayer(), rb.linearVelocityY);
    }

    private bool BattleTimeIsOver() => Time.time >  lastTimeWasInBattle + enemy.battleDuration;
    
    private void UpdateBattleTimer() => lastTimeWasInBattle = Time.time;

    private bool WithinAttackRange() => DistanceToPlayer() < enemy.attackDistance;

    private bool ShouldRetreat() => DistanceToPlayer() < enemy.minRetreatDistance;

    private float DistanceToPlayer()
    {
        return player ? Mathf.Abs(player.position.x - enemy.transform.position.x) : float.MaxValue ;
    }

    private int DirectionToPlayer()
    {
        if (!player)
        {
            return 0;
        }
        return player.position.x > enemy.transform.position.x ? 1 : -1;
    }
}
