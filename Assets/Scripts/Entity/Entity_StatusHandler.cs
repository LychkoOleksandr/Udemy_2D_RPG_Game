using System.Collections;
using UnityEngine;

public class Entity_StatusHandler : MonoBehaviour
{
    private ElementType currentEffect = ElementType.None;
    private Entity entity;
    private Entity_Stats entityStats;
    private Entity_Health entityHealth;
    private Entity_VFX entityVfx;

    [SerializeField] private GameObject lightningStrikeVfx;
    [SerializeField] private float currentCharge;
    [SerializeField] private float maxCharge = 1f;
    private Coroutine electrifyCo;

    private void Awake()
    {
        entity = GetComponent<Entity>();
        entityHealth = GetComponent<Entity_Health>();
        entityStats =  GetComponent<Entity_Stats>();
        entityVfx = GetComponent<Entity_VFX>();
    }

    public void ApplyElectrifyEffect(float duration, float damage, float charge)
    {
        float lightningResistance = entityStats.GetElementalResistance(ElementType.Lightning);
        float finalCharge = charge * (1 - lightningResistance);
        
        currentCharge += finalCharge;

        if (currentCharge >= maxCharge)
        {
            DoLightningStrike(damage);
            StopElectrifyEffect();
            return;
        }

        if (electrifyCo != null)
        {
            StopCoroutine(electrifyCo);
        }

        electrifyCo = StartCoroutine(ElectrifyEffectCo(duration));
    }

    private IEnumerator ElectrifyEffectCo(float duration)
    {
        currentEffect = ElementType.Lightning;
        entityVfx.PlayOnStatusVfx(duration, ElementType.Lightning);
        
        yield return new WaitForSeconds(duration);
        StopElectrifyEffect();
    } 

    private void StopElectrifyEffect()
    {
        currentEffect = ElementType.None;
        currentCharge = 0;
        entityVfx.StopAllVfx();
    }

    private void DoLightningStrike(float damage)
    {
        Instantiate(lightningStrikeVfx, transform.position, Quaternion.identity);
        entityHealth.ReduceHealth(damage);
    }

    public void ApplyBurnEffect(float duration, float damage)
    {
        float fireResistance = entityStats.GetElementalResistance(ElementType.Fire);
        float finalDamage = damage * (1 - fireResistance);

        StartCoroutine(BurnEffectCo(duration, finalDamage));
    }

    private IEnumerator BurnEffectCo(float duration, float totalDamage)
    {
        currentEffect = ElementType.Fire;
        entityVfx.PlayOnStatusVfx(duration, ElementType.Fire);

        int ticksPerSecond = 2;
        int tickCount =  Mathf.RoundToInt(duration * ticksPerSecond);
        
        float damagePerTick = totalDamage / tickCount;
        float tickInterval = duration / tickCount;

        for (int i = 0; i < tickCount; i++)
        {
            entityHealth.ReduceHealth(damagePerTick);
            yield return new WaitForSeconds(tickInterval);
        }
        
        currentEffect = ElementType.None;
    }
    
    public void ApplyChillEffect(float duration, float slowMultiplier)
    {
        float iceResistance = entityStats.GetElementalResistance(ElementType.Ice);
        float finalDuration =  duration * (1 - iceResistance);
        
        StartCoroutine(ChillEffectCo(finalDuration, slowMultiplier));
    }
    
    private IEnumerator ChillEffectCo(float duration, float  slowMultiplier)
    {
        entity.SlowDownEntity(duration, slowMultiplier);
        currentEffect = ElementType.Ice;
        entityVfx.PlayOnStatusVfx(duration, ElementType.Ice);
        
        yield return new WaitForSeconds(duration);
        currentEffect = ElementType.None;
    }
    
    public bool CanBeApplied()
    {
        if (currentEffect == ElementType.Lightning)
            return true;
        
        return currentEffect == ElementType.None;
    }
}
