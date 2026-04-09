using UnityEngine;

public abstract class EntityState
{
    private static readonly int AttackSpeedMultiplier = Animator.StringToHash("attackSpeedMultiplier");
    protected readonly StateMachine stateMachine;
    protected readonly string animBoolName;
    
    protected Animator anim;
    protected Rigidbody2D rb;
    protected Entity_Stats stats;

    protected bool triggerCalled;
    protected float stateTimer;

    public EntityState(StateMachine stateMachine, string animBoolName)
    {
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }
    
    public virtual void Enter()
    {
        anim.SetBool(animBoolName, true);
        triggerCalled = false;
    }
    
    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;
        UpdateAnimationParameters();
    }
    
    public virtual void Exit()
    {
        anim.SetBool(animBoolName, false);
    }

    public void AnimationTrigger()
    {
        triggerCalled = true;
    }

    protected virtual void UpdateAnimationParameters()
    {
        
    }
    
    public void SyncAttackSpeed()
    {
        float attackSpeed = stats.offense.attackSpeed.GetValue();
        
        anim.SetFloat(AttackSpeedMultiplier, attackSpeed);
    }
}
