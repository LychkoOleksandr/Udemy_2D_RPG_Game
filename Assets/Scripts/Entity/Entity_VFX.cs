using System.Collections;
using UnityEngine;

public class Entity_VFX : MonoBehaviour
{
    private SpriteRenderer sr;
    private Material originalMaterial;
    private Coroutine onDamageVfxCoroutine;
    private Entity entity;
    
    [Header("On Taking Damage VFX")]
    [SerializeField] private Material onDamageMaterial;
    [SerializeField] private float onDamageVfxDuration = .2f;

    [Header("On Dealing Damage VFX")]
    [SerializeField] private Color hitVfxColor = Color.white;
    [SerializeField] private GameObject hitVfx;
    [SerializeField] private GameObject critHitVfx;
    
    [Header("Element Colors")]
    [SerializeField] private Color chillVfx = Color.cyan;
    [SerializeField] private Color burnVfx = Color.red;
    [SerializeField] private Color electrifyVfx = Color.yellow;
    private Color originalHitVfxColor;
    
    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        originalMaterial = sr.material;
        entity = GetComponent<Entity>();
        originalHitVfxColor = hitVfxColor;
    }

    public void PlayOnStatusVfx(float duration, ElementType element)
    {
        switch (element)
        {
            case ElementType.Ice:
                StartCoroutine(PlayStatusVfxCo(duration, chillVfx));
                break;
            case ElementType.Fire:
                StartCoroutine(PlayStatusVfxCo(duration, burnVfx));
                break;
            case ElementType.Lightning:
                StartCoroutine(PlayStatusVfxCo(duration, electrifyVfx));
                break;
            case ElementType.None:
            default:
                break;
        }
    }

    public void StopAllVfx()
    {
        StopAllCoroutines();
        sr.color = Color.white;
        sr.material = originalMaterial;
    }

    private IEnumerator PlayStatusVfxCo(float duration, Color color)
    {
        float tickInterval = .25f;
        float timeHasPassed = 0;

        Color lightColor = color * 1.2f;
        Color darkColor = color * 0.8f;

        bool toggle = false;

        while (timeHasPassed < duration)
        {
            sr.color = toggle ? lightColor : darkColor;
            toggle = !toggle;
            
            yield return new WaitForSeconds(tickInterval);
            
            timeHasPassed += tickInterval;
        }
        
        sr.color =  originalMaterial.color;
    }

    public void CreateOnHitVFX(Transform target, bool isCrit)
    {
        GameObject hitPrefab = isCrit? critHitVfx : hitVfx;
        GameObject vfx = Instantiate(hitPrefab, target.position, Quaternion.identity);
        vfx.GetComponentInChildren<SpriteRenderer>().color = hitVfxColor;

        if (entity.facingDir == -1 && isCrit)
            vfx.transform.Rotate(0, 180, 0);
    }

    public void UpdateOnHitColor(ElementType element)
    {
        if (element == ElementType.Ice)
            hitVfxColor = chillVfx;

        if (element == ElementType.None)
            hitVfxColor = originalHitVfxColor;
    }

    public void PlayOnDamageVfx()
    {
        if (onDamageVfxCoroutine != null) StopCoroutine(onDamageVfxCoroutine);

        onDamageVfxCoroutine = StartCoroutine(OnDamageVfxCo());
    }

    private IEnumerator OnDamageVfxCo()
    {
        sr.material = onDamageMaterial;
        yield return new WaitForSeconds(onDamageVfxDuration);
        sr.material = originalMaterial;
    }
}