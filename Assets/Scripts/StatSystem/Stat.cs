using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stat
{
    [SerializeField] private float baseValue;
    [SerializeField] private List<StatModifier> modifiers = new();
    
    private bool isModified = true;
    private float finalValue;
    
    public float GetValue()
    {
        if (isModified)
        {
            finalValue = GetFinalValue();
            isModified = false;
        }
        
        return finalValue;
    }

    public void AddModifier(float value, string source)
    {
        StatModifier modToAdd = new(value, source);
        modifiers.Add(modToAdd);
        isModified = true;
    }

    public void RemoveModifier(string source)
    {
        modifiers.RemoveAll(modifier => modifier.source == source);
        isModified = true;
    }

    public float GetFinalValue()
    {
        finalValue = baseValue;

        foreach (StatModifier modifier in modifiers)
        {
            finalValue += modifier.value;
        }

        return finalValue;
    }
    
    public void SetBaseValue(float value) => baseValue = value;
}

[Serializable]
public class StatModifier
{
    public float value;
    public string source;

    public StatModifier(float value, string source)
    {
        this.value = value;
        this.source = source;
    }
} 
