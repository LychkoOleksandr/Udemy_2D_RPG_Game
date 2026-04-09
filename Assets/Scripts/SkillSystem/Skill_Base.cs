using UnityEngine;

public class Skill_Base : MonoBehaviour
{
    [Header("General details")]
    [SerializeField] private float cooldown;

    private float lastTimeUsed;

    protected virtual void Awake()
    {
        lastTimeUsed = Time.time -  cooldown;
    }
    
    public bool CanUseSkill()
    {
        if (OnCooldown())
        {
            Debug.Log("On Cooldown");
            return false;
        }
        
        return true;
    }
    
    private bool OnCooldown() => Time.time < lastTimeUsed + cooldown;
    public void SetSkillOnCooldown() => lastTimeUsed = Time.time;
    private void ResetCooldown() => lastTimeUsed = Time.time;
    private void ResetCooldownBy(float cooldownReduction) => lastTimeUsed +=  cooldownReduction;
}
