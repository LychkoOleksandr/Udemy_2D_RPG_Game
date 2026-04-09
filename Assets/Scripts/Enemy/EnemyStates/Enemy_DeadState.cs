using UnityEngine;

public class Enemy_DeadState : EnemyState
{
    private readonly Collider2D collider;
    
    public Enemy_DeadState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        collider = enemy.GetComponent<Collider2D>();
    }

    public override void Enter()
    {
        anim.enabled = false;
        collider.enabled = false;
        
        rb.gravityScale = 12f;
        rb.linearVelocity = new Vector2(rb.linearVelocityX, 15);
        
        stateMachine.SwitchOffStateMachine();
    }
}
