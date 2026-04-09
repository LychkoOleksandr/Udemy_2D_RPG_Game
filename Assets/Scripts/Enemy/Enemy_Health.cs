using UnityEngine;

public class Enemy_Health : Entity_Health
{
    private Enemy enemy => GetComponent<Enemy>();
        
    public override bool TakeDamage(float damage, float elementalDamage, ElementType element, Transform damageDealer)
    {
        bool gotHit = base.TakeDamage(damage, elementalDamage, element, damageDealer);

        if (!gotHit) return false;
        
        if (damageDealer.GetComponent<Player>())
            enemy.TryEnterBattleState(damageDealer);
        
        return true;
    }
}
