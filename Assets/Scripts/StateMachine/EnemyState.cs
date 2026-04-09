using UnityEngine;

public class EnemyState : EntityState
{
    private static readonly int MoveAnimSpeedMultiplier = Animator.StringToHash("moveAnimSpeedMultiplier");
    private static readonly int XVelocity = Animator.StringToHash("xVelocity");
    private static readonly int BattleAnimSpeedMultiplier = Animator.StringToHash("battleAnimSpeedMultiplier");

    protected Enemy enemy;
    
    public EnemyState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
    {
        this.enemy = enemy;

        rb = enemy.rb;
        anim = enemy.anim;
        stats = enemy.stats;
    }
    
    protected override void UpdateAnimationParameters()
    {
        base.UpdateAnimationParameters();
        
        float battleAnimSpeedMultiplier = enemy.battleMoveSpeed / enemy.moveSpeed;
        
        anim.SetFloat(MoveAnimSpeedMultiplier, enemy.moveAnimSpeedMultiplier);
        anim.SetFloat(BattleAnimSpeedMultiplier, battleAnimSpeedMultiplier);
        anim.SetFloat(XVelocity, rb.linearVelocityX);
    }
}
