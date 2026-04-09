using UnityEngine;

public class Object_Chest : MonoBehaviour, IDamageable
{
    private static readonly int ChestOpen = Animator.StringToHash("chestOpen");
    
    private Rigidbody2D rb => GetComponent<Rigidbody2D>();
    private Animator anim => GetComponentInChildren<Animator>();
    private Entity_VFX fx => GetComponent<Entity_VFX>();

    [Header("Open details")] 
    [SerializeField] private Vector2 knockback;
    [SerializeField] private float openRotationLimit = 150f;
    
    public bool TakeDamage(float damage, float elementalDamage, ElementType element, Transform damageDealer)
    {
        fx.PlayOnDamageVfx();
        anim.SetBool(ChestOpen, true);
        rb.linearVelocity = knockback;
        rb.angularVelocity = Random.Range(-openRotationLimit, openRotationLimit);
        
        return true;
    }
}
