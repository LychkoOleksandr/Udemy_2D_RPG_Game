using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Entity_Health : MonoBehaviour,  IDamageable
{
    private Slider healthBar;
    private Entity entity;
    private Entity_VFX entityVfx;
    private Entity_Stats entityStats;
    
    [SerializeField] protected float currentHealth;
    [SerializeField] protected bool isDead;

    [Header("Health Regen")] 
    [SerializeField] private float regenInterval = 1f;
    [SerializeField] private bool canRegenerateHealth = true;
    
    [Header("On Damage Knockback")]
    [SerializeField] private Vector2 knockBackPower = new (1.5f, 2.5f);
    [SerializeField] private Vector2 heavyKnockBackPower =  new (7f, 7f);
    [SerializeField] private float knockBackDuration = .2f;
    [SerializeField] private float heavyKnockBackDuration= .5f; // percentage of health you should lose to consider damage as heavy

    [Header("On Heavy Damage")] [SerializeField]
    private float heavyDamageThreshold = .3f;
    
    private void Awake()
    {
        entityVfx = GetComponent<Entity_VFX>();
        entity = GetComponent<Entity>();
        healthBar = GetComponentInChildren<Slider>();
        entityStats = GetComponent<Entity_Stats>();
        
        currentHealth = entityStats.GetMaxHealth();
        UpdateHealthBar();
        
        InvokeRepeating(nameof(RegenerateHealth), 0, regenInterval);
    }

    public virtual bool TakeDamage(float damage, float elementalDamage, ElementType element, Transform damageDealer)
    {
        if (isDead)
            return false;

        if (AttackEvaded()) 
            return false;

        Entity_Stats attackerStats = damageDealer.GetComponent<Entity_Stats>();
        float armorReduction = attackerStats ? attackerStats.GetArmorReduction() : 0f;
        float mitigation = entityStats.GetArmorMitigation(armorReduction);
        float physicalDamageTaken = damage * (1f - mitigation);

        float resistance = entityStats.GetElementalResistance(element);
        float elementalDamageTaken = elementalDamage * (1f - resistance);
        
        TakeKnockback(damageDealer, physicalDamageTaken);
        ReduceHealth(physicalDamageTaken + elementalDamageTaken);
        
        return true;
    }
    
    private bool AttackEvaded() => Random.Range(0, 100) < entityStats.GetEvasion();

    private void RegenerateHealth()
    {
        if (!canRegenerateHealth) return;

        float regenAmount = entityStats.resources.healthRegen.GetValue();
        IncreaseHealth(regenAmount);
    }
    
    public void IncreaseHealth(float healAmount)
    {
        if (isDead) return;
        
        float newHealth = currentHealth + healAmount;
        float maxHealth = entityStats.GetMaxHealth();
        
        currentHealth = Mathf.Min(newHealth, maxHealth);
        UpdateHealthBar();
    }
    
    public void ReduceHealth(float damage)
    {
        entityVfx?.PlayOnDamageVfx();
        currentHealth -= damage;
        
        UpdateHealthBar();

        if (currentHealth <= 0) Die();
    }

    private void Die()
    {
        isDead = true;
        
        entity.EntityDeath();
    } 
    
    private void UpdateHealthBar()
    {
        if (!healthBar) return;
        
        healthBar.value = currentHealth / entityStats.GetMaxHealth();
    }
    
    private void TakeKnockback(Transform damageDealer, float finalDamage)
    {
        Vector2 knockback = CalculateKnockback(finalDamage, damageDealer);
        float duration = CalculateDuration(finalDamage);
        
        entity?.ReceiveKnockback(knockback, duration);
    }

    private Vector2 CalculateKnockback(float damage, Transform damageDealer)
    {
        int direction = damageDealer.position.x < transform.position.x ? 1 : -1;

        Vector2 knockback = IsHeavyDamage(damage) ? heavyKnockBackPower : knockBackPower;
        
        knockback.x *= direction;
        return knockback;
    }

    private float CalculateDuration(float damage) => IsHeavyDamage(damage) ? heavyKnockBackDuration : knockBackDuration;

    private bool IsHeavyDamage(float damage) => damage / entityStats.GetMaxHealth() >= heavyDamageThreshold;
}
