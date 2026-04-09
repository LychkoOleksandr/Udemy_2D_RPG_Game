using UnityEngine;

public class Player_Combat : Entity_Combat
{
    [Header("Counter Attack Details")]
    [SerializeField] private float counterRecovery;
    
    public bool CounterAttackPerformed()
    {
        bool performedCounter = false;
        
        foreach (var target in GetDetectedColliders())
        {
            ICounterable counterable = target.GetComponent<ICounterable>();
            
            if (counterable == null) continue;

            if (!counterable.CanBeCountered) continue;
            
            performedCounter = true;
            counterable.HandleCounter();
        }
        
        return performedCounter;
    }
    
    public float GetCounterRecoveryDuration() =>  counterRecovery;
}
