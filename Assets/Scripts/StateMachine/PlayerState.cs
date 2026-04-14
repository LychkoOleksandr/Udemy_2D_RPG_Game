using UnityEngine;

public abstract class PlayerState : EntityState
{
    private static readonly int YVelocity = Animator.StringToHash("yVelocity");
    protected readonly PlayerInputSet input;
    protected readonly Player player;
    protected Player_SkillManager skillManager;
    
        
    protected PlayerState(Player player, StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
    {
        this.player = player;
        
        anim = player.anim;
        rb = player.rb;
        stats = player.stats;
        input = player.input;
        skillManager = player.skillManager;
    }

    public override void Update()
    {
        base.Update();

        if (input.Player.Dash.WasPressedThisFrame() && CanDash())
        {
            skillManager.dash.SetSkillOnCooldown();
            stateMachine.ChangeState(player.dashState);
        }
    }

    private bool CanDash()
    {
        if (!skillManager.dash.CanUseSkill())
            return false;
        
        return !player.wallDetected && stateMachine.currentState != player.dashState;
    }

    protected override void UpdateAnimationParameters()
    {
        base.UpdateAnimationParameters();
        
        anim.SetFloat(YVelocity,  rb.linearVelocityY);
    }
}
