using UnityEngine;

public class Player_BasicAttackState : PlayerState
{
    private static readonly int BasicAttackIndex = Animator.StringToHash("basicAttackIndex");
    private float attackVelocityTimer;
    private float lastTimeAttacked;

    private bool comboAttackQueued;
    private const int FirstComboIndex = 1; // We start combo index with number 1, this parameter is used in the Animator
    private int comboIndex = 1;
    private readonly int comboLimit = 3;
    private int attackDir;
    
    public Player_BasicAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        if (comboLimit == player.attackVelocity.Length) return;
        
        Debug.LogWarning("I've adjusted combo limit, according to attack velocity array");
        comboLimit = player.attackVelocity.Length;
    }

    public override void Enter()
    {
        base.Enter();
        SyncAttackSpeed();
        comboAttackQueued =  false;
        ResetComboIndexIfNeeded();

        attackDir = player.moveInput.x != 0 ? (int)player.moveInput.x : player.facingDir;
        
        anim.SetInteger(BasicAttackIndex, comboIndex);
        ApplyAttackVelocity();
        
    }

    public override void Update()
    {
        base.Update();
        HandleAttackVelocity();

        if (input.Player.Attack.WasPressedThisFrame())
            QueueNextAttack();
        
        if (triggerCalled)
            HandleStateExit();
    }

    public override void Exit()
    {
        base.Exit();
        lastTimeAttacked = Time.time;
        comboIndex++;
    }

    private void HandleStateExit()
    {
        stateMachine.ChangeState(player.idleState);
        if (comboAttackQueued)
        {
            player.EnterAttackStateWithDelay();
        }
    }

    private void QueueNextAttack()
    {
        if (comboIndex < comboLimit)
        {
            comboAttackQueued = true;
        }
    }

    private void HandleAttackVelocity()
    {
        attackVelocityTimer -= Time.deltaTime;
        
        if (attackVelocityTimer < 0)
            player.SetVelocity(0, rb.linearVelocity.y);
    }

    private void ApplyAttackVelocity()
    {
        Vector2 attackVelocity = player.attackVelocity[comboIndex - 1];
        attackVelocityTimer = player.attackVelocityDuration;
        player.SetVelocity(attackVelocity.x * attackDir, attackVelocity.y);
    }
    
    private void ResetComboIndexIfNeeded()
    {
        if (Time.time > lastTimeAttacked + player.comboResetTime
            || comboIndex > comboLimit)
            comboIndex = FirstComboIndex;
    }
}
