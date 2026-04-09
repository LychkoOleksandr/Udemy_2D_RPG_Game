using UnityEngine;

public class Entity_Stats : MonoBehaviour
{
    public Stat_SetupSO defaultStatSetup;
    
    public Stat_ResourceGroup resources;
    public Stat_OffenseGroup offense;
    public Stat_DefenseGroup defense;
    public Stat_MajorGroup major;

    public float GetElementalDamage(out ElementType element, float scaleFactor = 1)
    {
        float fireDamage = offense.fireDamage.GetValue();
        float iceDamage = offense.iceDamage.GetValue();
        float lightningDamage = offense.lightningDamage.GetValue();
        float bonusElementalDamage = major.intelligence.GetValue(); // Bonus elemental damage from Intelligence: +1 per INT

        float highestDamage = fireDamage;
        element = ElementType.Fire;
        
        if (iceDamage > highestDamage)
        {
            element = ElementType.Ice;
            highestDamage = iceDamage;
        }

        if (lightningDamage > highestDamage)
        {
            element = ElementType.Lightning;
            highestDamage = lightningDamage;
        }

        if (highestDamage <= 0)
        {
            element = ElementType.None;
            return 0;
        }

        float bonusFire = element == ElementType.Fire ? 0 : fireDamage * .5f;
        float bonusIce = element == ElementType.Ice ? 0 : iceDamage * .5f;
        float bonusLightning = element == ElementType.Lightning ? 0 : lightningDamage * .5f;
        
        float weakerElementsDamage = bonusFire + bonusIce + bonusLightning;
        float finalDamage = highestDamage + weakerElementsDamage + bonusElementalDamage;
        
        return finalDamage;
    }

    public float GetElementalResistance(ElementType element)
    {
        float baseResistance = 0;
        float bonusResistance = major.intelligence.GetValue() * .5f; // Bonus element resistance from Intelligence: +0.5% per INT

        baseResistance = element switch
        {
            ElementType.Fire => defense.fireRes.GetValue(),
            ElementType.Ice => defense.iceRes.GetValue(),
            ElementType.Lightning => defense.lightningRes.GetValue(),
            _ => baseResistance
        };

        float resistance = baseResistance + bonusResistance;
        float resistanceCap = 75f;

        float finalResistance = Mathf.Clamp(resistance, 0, resistanceCap) / 100; // Convert value into 0 to 1 multiplier
        
        return finalResistance;
    }
    
    public float GetPhysicalDamage(out bool isCrit, float scaleFactor = 1f)
    {
        float baseDamage = offense.damage.GetValue();
        float bonusDamage = major.strength.GetValue();
        float totalBaseDamage = baseDamage + bonusDamage;
        
        float baseCritChance = offense.critChance.GetValue();
        float bonusCritChance = major.agility.GetValue() * .3f; // Bonus crit chance from Agility: +0.3% per AGI
        float critChance = baseCritChance + bonusCritChance;
        
        float baseCritPower = offense.critPower.GetValue();
        float bonusCritPower = major.agility.GetValue() * .5f; // Bonus crit power from Strength: +0.5% per STR
        float critPower = (baseCritPower + bonusCritPower) / 100f; // Total crit power as multiplier (e.g 150 / 100 = 1.5 - multiplier)
        
        isCrit = Random.Range(0, 100) < critChance;
        float finalDamage = isCrit ? totalBaseDamage * critPower : totalBaseDamage;
        
        return finalDamage * scaleFactor;
    }

    public float GetArmorMitigation(float armorReduction)
    {
        float baseArmor = defense.armor.GetValue();
        float bonusArmor = major.vitality.GetValue(); // Bonus armor for vitality: +1 pre VIT
        float totalArmor = baseArmor + bonusArmor;
        
        float reductionMultiplier = Mathf.Clamp01(1f - armorReduction);
        float effectiveArmor = totalArmor * reductionMultiplier;
        
        float mitigation = effectiveArmor / (effectiveArmor + 100f);
        float mitigationCap = .85f; // Max mitigation will be capped at 85%
        
        float finalMitigation = Mathf.Clamp(mitigation, 0f, mitigationCap);
        
        return finalMitigation;
    }

    public float GetArmorReduction()
    {
        // Total armor reduction as multiplier (e.g 30 / 100 = 0.3f - multiplier)
        float finalReduction = offense.armorReduction.GetValue() / 100f;
        return finalReduction;
    }

    public float GetMaxHealth()
    {
        float baseMaxHp = resources.maxHealth.GetValue();
        float bonusMaxHp = major.vitality.GetValue() * 5;
        float finalMaxHp = baseMaxHp + bonusMaxHp;
        
        return finalMaxHp;
    }

    public float GetEvasion()
    {
        float baseEvasion = defense.evasion.GetValue();
        float bonusEvasion = major.agility.GetValue() * .5f; // Bonus evasion from Agility: +0.5% per AGI
        
        float totalEvasion = baseEvasion + bonusEvasion;
        float evasionCap = 80f; //Evasion will be capped at 80%
        float finalEvasion = Mathf.Clamp(totalEvasion, 0f, evasionCap);
        
        return finalEvasion;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public Stat GetStatByType(StatType type)
    {
        switch (type)
        {
            case StatType.MaxHealth: return resources.maxHealth;
            case StatType.HealthRegen: return resources.healthRegen;
            
            case StatType.Strength: return major.strength;
            case StatType.Vitality: return major.vitality;
            case StatType.Agility: return major.agility;
            case StatType.Intelligence: return major.intelligence;
            
            case StatType.AttackSpeed: return offense.attackSpeed;
            case StatType.Damage: return offense.damage;
            case StatType.CritChance: return offense.critChance;
            case StatType.CritPower: return offense.critPower;
            case StatType.ArmorReduction:  return offense.armorReduction;
            
            case StatType.FireDamage: return offense.fireDamage;
            case StatType.IceDamage:  return offense.iceDamage;
            case StatType.LightningDamage: return offense.lightningDamage;
            
            case StatType.Armor: return defense.armor;
            case StatType.Evasion:  return defense.evasion;
            
            case StatType.FireResistance: return defense.fireRes;
            case StatType.IceResistance: return defense.iceRes;
            case StatType.LightningResistance: return defense.lightningRes;
            
            default: 
                Debug.LogError($"Stat type {type} not implemented");
                return null;
        }
    }

    [ContextMenu("Update Default Stat Setup")]
    public void ApplyDefaultStatSetup()
    {
        if (!defaultStatSetup)
        {
            Debug.Log("No default stat setup assigned");
            return;
        }
        
        resources.maxHealth.SetBaseValue(defaultStatSetup.maxHealth);
        resources.healthRegen.SetBaseValue(defaultStatSetup.healthRegen);
        
        offense.attackSpeed.SetBaseValue(defaultStatSetup.attackSpeed);
        offense.damage.SetBaseValue(defaultStatSetup.damage);
        offense.critChance.SetBaseValue(defaultStatSetup.critChance);
        offense.critPower.SetBaseValue(defaultStatSetup.critPower);
        offense.armorReduction.SetBaseValue(defaultStatSetup.armorReduction);
        
        offense.fireDamage.SetBaseValue(defaultStatSetup.fireDamage);
        offense.iceDamage.SetBaseValue(defaultStatSetup.iceDamage);
        offense.lightningDamage.SetBaseValue(defaultStatSetup.lightningDamage);
        
        defense.armor.SetBaseValue(defaultStatSetup.armor);
        defense.evasion.SetBaseValue(defaultStatSetup.evasion);
        
        defense.fireRes.SetBaseValue(defaultStatSetup.fireResistance);
        defense.iceRes.SetBaseValue(defaultStatSetup.iceResistance);
        defense.lightningRes.SetBaseValue(defaultStatSetup.lightningResistance);
        
        major.strength.SetBaseValue(defaultStatSetup.strength);
        major.agility.SetBaseValue(defaultStatSetup.agility);
        major.intelligence.SetBaseValue(defaultStatSetup.intelligence);
        major.vitality.SetBaseValue(defaultStatSetup.vitality);
    }
}
